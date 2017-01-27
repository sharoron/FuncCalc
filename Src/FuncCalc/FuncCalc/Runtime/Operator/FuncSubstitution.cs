using FuncCalc.Exceptions;
using FuncCalc.Expression;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Runtime.Func.Runtime;

namespace FuncCalc.Runtime.Operator
{
    public class FuncSubstitution : IOperator
    {
        public string Name
        {
            get
            {
                return "未評価式の代入";
            }
        }
        public string Text
        {
            get
            {
                return ":=";
            }
        }
        public bool RequireLeftParameter
        {
            get
            {
                return true;
            }
        }
        public bool RequireRightParameter
        {
            get
            {
                return true;
            }
        }
        public bool DoEvaledParam
        {
            get { return false; }
        }

        public INumber Execute(RuntimeData runtime, INumber left, INumber right) {
            if (left is FunctionFormula) {
                FunctionFormula ff = left as FunctionFormula;
                
                // 形式の確認をする
               if (!(ff.Items[0] is Member && ff.Items[1] is Formula))
                    throw new SyntaxException("左辺は '関数名'(['パラメータ']) の形式である必要があります", left);
                if (!(ff.Items[1] is Formula))
                    throw new NotImplementedException("現在左辺の式は通常の式のみに対応しています");

                string name = (ff.Items[0] as Member).Text;
                Formula param = ff.Items[1] as Formula;
                List<string> paramName = new List<string>();
                List<ExpressionType> paramType = new List<ExpressionType>();
                List<IExpression> stock = new List<IExpression>();
                for (int i = 0; i < param.Count; i++) {
                    if (!(param.Items[i] is LineBreak)) {
                        stock.Add(param.Items[i]);
                    }
                    if (param.Items[i] is LineBreak || i == param.Count - 1) {
                        if (stock.Count >= 3) throw new SyntaxException("関数のパラメータの指定がおかしいです。", stock[2]);
                        if (stock.Count == 1) {
                            paramName.Add((stock[0] as Member).Text);
                            paramType.Add(ExpressionType.Unknown);
                        }
                        if (stock.Count == 2) {
                            throw new NotImplementedException("関数のパラメータに型を指定するのは未実装です。");
                        }
                        stock.Clear();
                    }
                }

                UserDefineFunction f = new UserDefineFunction(name, right, paramName.ToArray());

                if (runtime.Functions.ContainsKey(name)) {
                    var func = runtime.Functions[name];
                    if (func is UserDefineFunction)
                        runtime.Functions.Remove(name);
                    else
                        throw new RuntimeException(string.Format("'{0}' はすでに定義されています。", name), left);

                    runtime.Functions.Add(name, f);
                }
                else
                    runtime.Functions.Add(name, f);

                return right;
            }
            else if (left is Variable || left is Member) {
                string name = ""; Variable v = null;
                if (left is Variable) {
                    v = left as Variable;
                    name = v.Name;
                }
                if (left is Member) {
                    name = (left as Member).Text;
                    v = new Expression.Variable((left as Member).Token);
                }

                runtime.SetVariable(runtime, v, right);
                return right;
            }
            else
                throw new SyntaxException("左辺は関数の形式またはメンバーである必要があります", left);

        }
    }
}
