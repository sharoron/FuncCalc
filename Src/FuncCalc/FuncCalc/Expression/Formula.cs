using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Interface;
using FuncCalc.Runtime;
using FuncCalc.Analyzer;
using FuncCalc.Exceptions;
using System.Diagnostics;

namespace FuncCalc.Expression {
    
    public class Formula : INumber, IExpression, IBlock, IEval, IFormula, IUnknownParameter {

        internal List<IExpression> _items = new List<IExpression>();
        private INumber multi = Number.New(1);
        private Token token = null;

        public Formula() {

        }

        public IExpression[] Items {
            get { return this._items.ToArray(); }
        }
        public override Token Token
        {
            get
            {
                return this.token == null ? 
                    this._items.Last().Token as Token : 
                    this.token;
            }
            protected internal set
            {
                this.token = value;
            }
        }
        public Token EndToken
        {
            get;
            internal set;
        }
        public override ExpressionType Type
        {
            get
            {
                return ExpressionType.Formula;
            }
        }
        public override ValueType ValueType
        {
            get
            {
                return ValueType.Unknown;
            }
        }
        public override int SortPriority {
            get { return 1500; }
        }
        public INumber Multi
        {
            get { return this.multi; }
            set { this.multi = value; }
        }
        public int Count
        {
            get { return this._items.Count; }
        }

        public override INumber Clone() {
            var res = this.MemberwiseClone() as Formula;
            res._items = new List<IExpression>();
            res._items.AddRange(this._items);
            return res;
        }
        public void AddItem(RuntimeData runtime, IExpression exp) {
            this._items.Add(exp);
        }
        public void Insert(int index, IExpression exp) {
            this._items.Insert(index, exp);
        }
        internal void Remove(IExpression exp) {
            this._items.Remove(exp);
        }
        internal void RemoveAt(int index) {
            this._items.RemoveAt(index);
        }

        public override INumber Add(RuntimeData runtime, INumber val) {
            throw new NotImplementedException();
        }
        public override INumber Multiple(RuntimeData runtime, INumber val) {
            var me = this.Clone() as Formula;
            me.multi = me.multi.Multiple(runtime, val);
            return me;
        }
        public override INumber Divide(RuntimeData runtime, INumber val) {
            var me = this.Clone() as Formula;
            me.multi = me.multi.Divide(runtime, val);
            return me;
        }
        public override bool Equals(RuntimeData runtime, INumber val) {
            return false;
        }
        public override bool CanJoin(RuntimeData runtime, INumber val) {
            return false;
        }
        public override INumber Eval(RuntimeData runtime) {
            if (this.Count == 0) {
                return null;
            }
            else if (this.Count == 1) {
                return (this.Items[0] as IEval).Eval(runtime);
            }
            else if (this.Items.Where(i => i is LineBreak).Count() >= 1) {

                // 一番最後がLineBreakじゃなければ追加する
                if (!(this._items.Last() is LineBreak))
                    this._items.Add(new LineBreak(new Token(";", TokenType.Syntax)));
                

                // LineBreakがあるごとに逆ポーランド記法に変換して実行
                Formula f = new Expression.Formula();
                IEval res = null;
                Array res_a = null;
                // 配列形式のフォーマットなら配列形式で返す
                if (this.token != null && this.token.Text == "[") res_a = new Array();

                for (int i = 0; i < this._items.Count; i++) {
                    if (this._items[i] is LineBreak) {
                        if ((this._items[i] as LineBreak).Token.Text == "," && res_a == null)
                            throw new SyntaxException(string.Format("不正な文字です。 '{0}' 関数ではないものを関数のように扱っている可能性があります。",
                                this._items[i].Token.Text), this._items[i]);
                        if (runtime.Setting.IsDebug) {
                            Console.WriteLine("Eval Formula : " + f.ToString());
                        }
                        SyntaxAnalyzer.ConvertToRPEFormula ea = new SyntaxAnalyzer.ConvertToRPEFormula(
                            f, runtime.Setting);
                        res = ea.ConvertToRPE().Eval(runtime);

                        if (res_a != null) res_a.Items[0].Add(res as INumber); // '配列' 部の処理

                        f._items.Clear();
                    }
                    else
                        f._items.Add(this._items[i]);
                }

                if (res_a == null)
                    return res as INumber;
                else // 配列作成でこの式を読んでいる場合は配列として返す
                    return res_a as INumber; 
            }
            else { // This.Items.Countが2以上のときじゃないと逆ポーランド式に変換ｓれない
                SyntaxAnalyzer.ConvertToRPEFormula ea = new Analyzer.SyntaxAnalyzer.ConvertToRPEFormula(this, runtime.Setting);
                return ea.ConvertToRPE().Eval(runtime);
            }
        }

        public override string ToString() {

            string startBracket = this.Token.Text;
            string endBracket = "";
            if (startBracket == "(") endBracket = ")";
            else if (startBracket == "[") endBracket = "]";
            else startBracket = null;

            string str = "";
            foreach (var item in this.Items) {
                if (item is LineBreak && (item as LineBreak).Token.Text == null) continue;

                if (str.Length != 0) str += " ";

                str += item.ToString();
            }

            return string.Format("{0}{1}{2}", startBracket, str, endBracket);

        }

        public void ExecuteAsParameter(RuntimeData runtime) {
            List<IExpression> line = new List<Interface.IExpression>();

            if (this.Count == 0)
                return;

            if (!(this.Items.Last() is LineBreak))
                this.AddItem(runtime, new LineBreak());

            for (int i = 0; i < this.Count; i++) {
                if (this.Items[i] is LineBreak) {
                    if (line.Count == 0)
                        throw new SyntaxException(string.Format("パラメータが空です。"), this);

                    if (line.Count == 1) {
                        runtime.NowBlock.Push(line[0]);
                        line.Clear();
                    }
                    else {
                        Formula f = new Expression.Formula();
                        foreach (var item in line) {
                            f.AddItem(runtime, item);
                        }
                        runtime.NowBlock.Push(f);

                        line.Clear();
                    }
                    continue;
                } else {
                    line.Add(this.Items[i]);
                }
            }
        }

        public override string Output(OutputType type) {

            switch (type) {
                case OutputType.Mathjax: {
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < this._items.Count; i++) {
                            if (this._items[i] is IOutput)
                                sb.Append((this.Items[i] as IOutput).Output(type));
                            else
                                sb.Append(this.Items[i].ToString());
                        }
                        return sb.ToString();
                    }
            }

            return base.Output(type);
        }

        public override INumber ExecuteDiff(RuntimeData runtime, string t) {

            var pow = Runtime.Func.Differential.DiffPow(runtime, t, this);

            runtime.Setting.Logger.AddWarning("Formula型の表現を直接微分はバグってる可能性あり");
            Debugger.Break();

            var res = this.Eval(runtime).ExecuteDiff(runtime, t);
            if (pow != null) {
                pow.AddItem(runtime, res);
                return pow;
            }
            else {
                return res;
            }
        }
        public override INumber Integrate(RuntimeData runtime, string t) {

            var res = this.Eval(runtime);
            if (res is Formula)
                throw new NotImplementedException();

            return res.Integrate(runtime, t);


        }

    }

}
