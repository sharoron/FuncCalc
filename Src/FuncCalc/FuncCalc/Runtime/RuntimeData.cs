using FuncCalc.Exceptions;
using FuncCalc.Expression;
using FuncCalc.Interface;
using FuncCalc.Runtime.Func.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Runtime
{
    public class RuntimeData
    {
        private RuntimeSetting setting = null;
        private List<BlockData> blocks = null;
        private Dictionary<string, IFunction> funcs = null;

        private RuntimeData() { }
        public RuntimeData(RuntimeSetting setting) {
            this.setting = setting;
            this.blocks = new List<Runtime.BlockData>();
            this.funcs = new Dictionary<string, IFunction>();

            this.blocks.Add(new BlockData());

            LoadFunctions();
        }
        private void LoadFunctions() {
            System.Reflection.Assembly asm = typeof(RuntimeData).Assembly;

            foreach (Type t in asm.GetTypes()) {
                if (t.IsClass && t.IsPublic && !t.IsAbstract &&
                    t.IsSubclassOf(typeof(IFunction))) {

                    var func = asm.CreateInstance(t.FullName) as IFunction;
                    this.funcs.Add(func.Name, func);
                }
            }
        }

        public RuntimeSetting Setting
        {
            get { return this.setting; }
        }
        public BlockData NowBlock
        {
            get { return this.blocks[this.blocks.Count - 1]; }
        }
        public Dictionary<string, IFunction> Functions
        {
            get { return this.funcs; }
        }
        public BlockData[] Blocks
        {
            get { return this.blocks.ToArray(); }
        }
        public OutputType LogType
        {
            get { return this.setting.Logger.OutputType; }
        }
        public bool EnabledLogging
        {
            get { return true; }
        }


        public RuntimeData Clone() {
            return this.MemberwiseClone() as RuntimeData;

        }
        internal void AddBlock(BlockData block) {
            if (this.setting.IsDebug)
                Console.WriteLine("Add Block: [MoreScope:{0}]", block.MoreScope);
            this.blocks.Add(block);
        }
        internal void PopBlock() {
            if (this.setting.IsDebug)
                Console.WriteLine("Pop Block");
            this.blocks.RemoveAt(this.blocks.Count - 1);
        }
        public bool ContainsKey(string name, bool moreScope =false) {

            // 定数の中から調べる
            if (this.setting.Constants.ContainsKey(name))
                return true;

            // 変数の中から調べる
            for (int i = this.blocks.Count - 1; i >= 0; i--) {
                if (this.blocks[i].ContainsKey(name))
                    return true;
                if (!moreScope && !this.blocks[i].MoreScope)
                    return false;
            }
            return false;
        }
        public bool ContainsFunc(string name) {
            return this.funcs.ContainsKey(name);
        }
        public INumber GetData(Token t) {
            return this.GetData(t, false);
        }
        public INumber GetData(Token t, bool moreScope) {

            // 定数が定義されている場合はそれを返す
            if (this.setting.Constants.ContainsKey(t.Text)) {
                return this.setting.Constants[t.Text].Clone();
            }

            for (int i = this.blocks.Count - 1; i >= 0; i--) {
                if (this.blocks[i].ContainsKey(t.Text)) {
                    var res = this.blocks[i].GetData(t.Text);
                    if (this.setting.IsDebug) Console.WriteLine("Get Variable : {0, 10}:{1}", t.Text, res);
                    return res;

                }
                if (!moreScope && !this.blocks[i].MoreScope)
                    break;
            }
            if (this.setting.IsDebug)
                Console.WriteLine("Get Variable : {0, 10}:{1}", t.Text, t.Text);
            return new Variable(t);

        }
        public void SetVariable(RuntimeData runtime, Variable v, INumber value) {

            // 定数に代入することはできない
            if (this.setting.Constants.ContainsKey(v.Name))
                throw new SyntaxException("定数に代入することはできません。", v);

            if ((v is Variable && value is Variable && (v as Variable).Name == (value as Variable).Name)) {
                // x = xみたいに同じ名前の変数をそのまんま入れようとしてたら無視する
                if (v.Multi.Equals(runtime, (value as Variable).Multi) &&
                    v.Pow.Equals(runtime, (value as Variable).Pow)) {
                    return;
                }
                else {
                    throw new RuntimeException("変数に同じ名前の変数を代入することはできません。", v);
                }
            }

            // 代入することがそもそもできるか確認する
             if ((value is IFormula && ContainsMember(value as IFormula, v))) {
                 throw new RuntimeException("変数に回帰式を入れることはできません。", v);
            }


            // ブロックデータから検索して変数に代入する
            for (int i = this.blocks.Count - 1; i >= 0; i--) {
                if (this.blocks[i].Variables.ContainsKey(v.Name)) {
                    if (value != null) {
                        this.blocks[i].Variables[v.Name] = value;
                        if (this.setting.IsDebug) Console.WriteLine("変数の値を上書きしました: {0, 10}:{1}", v.Name, value.ToString());
                        return;
                    }
                    else {
                        this.blocks[i].Variables.Remove(v.Name);
                        if (this.setting.IsDebug) Console.WriteLine("変数を削除しました: {0, 10}", v.Name);
                
                    }
                }
                if (!this.blocks[i].MoreScope)
                    break;
            }

            if (value != null) {
                this.NowBlock.Variables.Add(v.Name, value);
                if (this.setting.IsDebug) Console.WriteLine("変数を作成しました {0, 10}:{1}", v.Name, value.ToString());


            }
        }
        private bool ContainsMember(IFormula formula, INumber exp) {
            foreach (var item in formula.Items) {
                if (item is IFormula) {
                    var res = ContainsMember(item as IFormula, exp);
                    if (res) return true;
                } else {
                    if (item is Member && exp is Member &&
                        (item as Member).Text == (exp as Member).Text)
                        return true;
                    if (item is Variable && exp is Variable &&
                        (item as Variable).Name == (exp as Variable).Name)
                        return true;
                    if (item is Member && exp is Variable &&
                       (item as Member).Text == (exp as Variable).Name)
                        return true;
                    if (item is Variable && exp is Member &&
                        (item as Variable).Name == (exp as Member).Text)
                        return true;
                }
            }
            return false;
        }
        public IFunction GetFunc(IExpression name, bool isParamCheck, params ExpressionType[] parameters) {
            if (parameters.Length == 0 && !isParamCheck) {
                if (name is IFunction)
                    return name as IFunction;
                else
                    return this.funcs[name.Token.Text];
            }
            else {
                string nm = "";
                if (name is IFunction)
                    nm = (name as IFunction).Name;
                else
                    nm = name.Token.Text;

                foreach (var item in
                    (from f in this.funcs where f.Key == nm select f)) {
                    bool flag = item.Value.Parameter.Length == parameters.Length;
                    for (int i = 0; i < item.Value.Parameter.Length; i++) {
                        if (parameters.Length <= i) {
                            flag = false;
                            break;
                        }
                        if (parameters[i] == ExpressionType.Unknown ||
                            item.Value.Parameter[i] == ExpressionType.Anything) { flag = true; continue; }
                        if (parameters[i] != item.Value.Parameter[i]) { flag = false; break; }
                        flag = true;
                    }
                    if (flag)
                        return item.Value;
                }

                StringBuilder sb = new StringBuilder();
                foreach (var pa in parameters) {
                    if (sb.Length != 0) sb.Append(", ");
                    sb.Append(pa.ToString());
                }
                throw new SyntaxException(string.Format("'{0}({1})'は見つかりませんでした。", nm, sb), name, new KeyNotFoundException());
            }
        }
        public bool IsConstValue(INumber num) {
            return FuncCalc.Runtime.Func.Differential.IsConstValue(this, num);
        }
        public void OutputDebug() {
            Console.WriteLine("変数");
            for (int i = this.blocks.Count - 1; i >= 0; i--) {
                Console.WriteLine("  {0,2} Block : [MoreScope: {1}]", i, this.blocks[i].MoreScope);
                foreach (var item in this.blocks[i].Variables.Keys) {
                    Console.WriteLine("    {0, 10}:{1}", item, this.blocks[i].Variables[item]);
                }
            }
            Console.WriteLine("ユーザー定義関数:");
            foreach (var func in this.Functions) {
                if (func.Value is UserDefineFunction) {
                    Console.WriteLine("  {0}", func.Value);
                }
            }
        }

        public void AddLogCondition(string keyname, params IExpression[] parameter) {
            if (this.EnabledLogging)
                this.setting.Logger.AddCondition(GetRes.Text("Res", "But") + GetResStr(keyname, parameter));
        }
        public void AddLogWay(string str) {
            this.setting.Logger.AddWay(str);
        }
        public void AddLogWay(string keyname, params IExpression[] parameter) {
            if (this.EnabledLogging)
                this.AddLogWay(GetResStr(keyname, parameter));
        }
        private string GetResStr(string keyname, params IExpression[] parameter) {
            string source = "Res-";
            switch (this.LogType) {
                case OutputType.String:
                    source += "str";
                    break;
                case OutputType.Mathjax:
                    source += "mathjax";
                    break;
                default:
                    throw new NotImplementedException();
            }
            
            string str = GetRes.Text(source, keyname);

            switch (this.LogType) {
                case OutputType.Mathjax:
                    if (keyname.StartsWith("_"))
                        str = " $ " + str + " $ ";
                    break;
            }

            List<object> prm = new List<object>();
            for (int i = 0; i < parameter.Length; i++) {
                if (parameter[i] is IOutput) {
                    string st = "";

                    switch (this.LogType) {
                        case OutputType.Mathjax:
                            if (keyname.StartsWith("_"))
                                st = ((parameter[i] as IOutput).Output(this.LogType));
                            else
                                st = (" $ " + (parameter[i] as IOutput).Output(this.LogType) + " $ ");
                            break;
                        default:
                            st = ((parameter[i] as IOutput).Output(this.LogType));
                            break;
                    }

                    str = str.Replace("{" + i + "}", st);
                }
                else
                    prm.Add(parameter[i].ToString());

            }


            return str;
        }

        public bool IsFunctionINumber(INumber val, string varname) {

            if (val is Variable) {
                if ((val as Variable).Name != varname)
                    return true;
                else if (!(val.Pow is Number &&
                    (val.Pow as Number).Value == 1))
                    return true;
                else if (!((val as IMulti).Multi is Number &&
                    ((val as IMulti).Multi as Number).Value == 1))
                    return true;
                else return false;
            }
            if (val is Member) {
                if ((val as Member).Text != varname)
                    return true;
                else if (!(val.Pow is Number &&
                    (val.Pow as Number).Value == 1))
                    return true;
                else if (!((val as IMulti).Multi is Number &&
                    ((val as IMulti).Multi as Number).Value == 1))
                    return true;
                else
                    return false;
            }

            if (val is AdditionFormula) {

                AdditionFormula af = val as AdditionFormula;
                bool flag = false;
                for (int i = 0; i < af.Items.Length; i++) {
                    if (af.Items[i] is Number && (af.Items[i] as Number).Value == 0)
                        continue;
                    if (af.Items[i] is Number && (af.Items[i] as Number).Value == 1)
                        continue;
                    if (af.Items[i] is IConstParameter || af.Items[i] is ImaginaryNumber)
                        continue;

                    if (af.Items[i] is Variable) {
                        if ((af.Items[i] as Variable).Name != varname || flag)
                            return true;
                        else if (!(af.Items[i].Pow is Number &&
                            (af.Items[i].Pow as Number).Value == 1))
                            return true;
                        else { flag = true; continue; }
                    }
                    if (af.Items[i] is Member) {
                        if ((af.Items[i] as Member).Text != varname || flag)
                            return true;
                        else if (!(af.Items[i].Pow is Number &&
                            (af.Items[i].Pow as Number).Value == 1))
                            return true;
                        else { flag = true; continue; }
                    }
                    if (af.Items[i] is AdditionFormula &&
                        !this.IsFunctionINumber(af.Items[i], varname))
                        continue;
                    if (af.Items[i] is MultipleFormula &&
                        !this.IsFunctionINumber(af.Items[i], varname))
                        continue;

                    return true;
                }
            }
            if (val is MultipleFormula) {
                MultipleFormula mf = val as MultipleFormula;
                bool flag = false;
                for (int i = 0; i < mf.Items.Count; i++) {
                    if (mf.Items[i] is Number && (mf.Items[i] as Number).Value == 0)
                        return false;
                    if (mf.Items[i] is Number && (mf.Items[i] as Number).Value == 1)
                        continue;
                    if (mf.Items[i] is IConstParameter || mf.Items[i] is ImaginaryNumber)
                        return true;

                    if (mf.Items[i] is Variable) {
                        if ((mf.Items[i] as Variable).Name != varname || flag)
                            return true;
                        else if (!(mf.Items[i].Pow is Number &&
                            (mf.Items[i].Pow as Number).Value == 1))
                            return true;
                        else { flag = true; continue; }
                    }
                    if (mf.Items[i] is Member) {
                        if ((mf.Items[i] as Member).Text != varname || flag)
                            return true;
                        else if (!(mf.Items[i].Pow is Number &&
                            (mf.Items[i].Pow as Number).Value == 1))
                            return true;
                        else { flag = true; continue; }
                    }
                    if (mf.Items[i] is AdditionFormula &&
                        !this.IsFunctionINumber(mf.Items[i], varname))
                        continue;
                    if (mf.Items[i] is MultipleFormula &&
                        !this.IsFunctionINumber(mf.Items[i], varname))
                        continue;

                    return true;
                }
            }

            return true;

        }

    }

}
