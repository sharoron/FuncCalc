using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Expression;
using FuncCalc.Exceptions;
using FuncCalc.Runtime.Func.Runtime;
using FuncCalc.Expression.Const;

namespace FuncCalc.Runtime.Func
{
    public class Differential : IFunction, IFunctionOutput
    {
        public override string Name
        {
            get
            {
                return "diff";
            }
        }
        public override string Description
        {
            get
            {
                return "指定した式を微分します。";
            }
        }
        public override ExpressionType[] Parameter
        {
            get { return new ExpressionType[] { ExpressionType.Pointer, ExpressionType.Unknown }; }
        }
        public override ExpressionType ReturnType
        {
            get
            {
                return ExpressionType.Unknown;
            }
        }
        public override bool DoEvaledParam
        {
            get
            {
                return false;
            }
        }

        public override INumber Execute(RuntimeData runtime, params INumber[] parameters) {

            string t = "";
            if (parameters[0] is Variable) t = (parameters[0] as Variable).Name;
            else if (parameters[0] is Member) t = (parameters[0] as Member).Text;
            else throw new RuntimeException(string.Format("'{0}'関数の第１パラメータは変数のみ対応しています", this.Name), parameters[0]);
            if (string.IsNullOrEmpty(t))
                throw new RuntimeException("param[0]が不正です", parameters[0]);

            var formula = parameters[1].Eval(runtime);

            var res = formula.ExecuteDiff(runtime, t);
            if (runtime.Setting.DoOptimize)
                return res.Optimise(runtime);
            else
                return res;

            return ExecuteDiff(runtime, formula, t, parameters[0]);

        }
        private INumber ExecuteDiff(RuntimeData runtime, INumber formula, string t, INumber param0) {
            

            if (formula is AdditionFormula) {
                AdditionFormula af = new AdditionFormula();
                AdditionFormula f = formula as AdditionFormula;
                for (int i = 0; i < f.Count; i++) {
                    var res = DiffExecute(runtime, t, f.Items[i], param0, false);
                    if (res is Number && (res as Number).Value == 0) continue;

                    af.AddItem(res);
                }
                return af;
            }
            else if (formula is Variable && // 微分しようとした変数がユーザー定義関数だった場合
                runtime.Functions.ContainsKey((formula as Variable).Name) &&
                runtime.Functions[(formula as Variable).Name] is UserDefineFunction) {

                var f = runtime.Functions[(formula as Variable).Name] as UserDefineFunction;
                return this.Execute(runtime, param0, f.Formula.Eval(runtime));
            }
            else if (formula is Member && // 微分しようとした変数がユーザー定義関数だった場合
                runtime.Functions.ContainsKey((formula as Member).Text) &&
                runtime.Functions[(formula as Member).Text] is UserDefineFunction) {

                var f = runtime.Functions[(formula as Member).Text] as UserDefineFunction;
                return this.Execute(runtime, param0, f.Formula.Eval(runtime));
            }
            else if (formula is MultipleFormula) {
                throw new NotImplementedException();
            }
            else if (formula is Variable) {
                return DiffExecute(runtime, t, formula as Variable, param0, false);
            }
            else if (formula is Member) {
                var v = new Variable((formula as Member).Token);
                return DiffExecute(runtime, t, v, param0, false);
            }
            else if (formula is IConstParameter) {
                return Number.New(0);
            }


            throw new NotImplementedException();
        }
        private INumber DiffExecute(RuntimeData runtime, string t, INumber m, INumber param0, bool kaiki) {
            if (m is IConstParameter)
                return Number.New(0);

            if (m is Variable) {
                Variable v = m.Clone() as Variable;
                if (v.Name == t) {
                    if (v.Pow.Equals(runtime, Number.New(1)))
                        return v.Multi;
                    INumber multiple = v.Multi.Multiple(runtime, v.Pow);
                    bool powFormula = !(v.Pow is IConstParameter);
                    var pow = v.Pow;
                    v.Pow = v.Pow.Subtract(runtime, Number.New(1));
                    v.Multi = multiple;

                    if (powFormula) {
                        MultipleFormula mf = new Expression.MultipleFormula();
                        mf.AddItem(runtime, v);
                        mf.AddItem(runtime, ExecuteDiff(runtime, pow, t, param0));
                        return mf;
                    }
                    else {
                        if (runtime.Setting.DoOptimize)
                            return v.Optimise(runtime);
                        else
                            return v;
                    }
                }
                else {
                    if (kaiki) throw new FunctionExecutionCancelException();

                    // y*(y')みたいな形にする
                    MultipleFormula mf = new Expression.MultipleFormula();
                    FunctionFormula ff = new Expression.FunctionFormula();
                    Formula f = new Formula() { Token = new Token("(", Analyzer.TokenType.Syntax) };
                    var d = new Variable(new FuncCalc.Token(t, Analyzer.TokenType.Member));
                    mf.Items.Add(m);
                    ff.AddItem(runtime, new Member(new Token("diff", Analyzer.TokenType.Member)));
                    f.AddItem(runtime, d);
                    f.AddItem(runtime, new LineBreak(new Token(",", Analyzer.TokenType.Syntax)));
                    f.AddItem(runtime, v);
                    ff.AddItem(runtime, f);
                    mf.Items.Add(ff); // AddItemを使うと無限ループ
                    // return mf;
                    return ff;
                }
            }

            throw new NotImplementedException();
        }

        public static MultipleFormula DiffPow(RuntimeData runtime, string t, INumber val) {

            if (!val.Pow.Equals(runtime, Number.New(1))) {
                var res = val.Pow.ExecuteDiff(runtime, t);
                if (!res.Equals(runtime, Number.New(0))) {
                    // powの所に式が含まれているため対数微分法を使う必要がある
                    throw new NotImplementedException("未実装: Powに式が含まれているため対数微分法を使って微分する必要あり");
                } else {
                    MultipleFormula mf = new Expression.MultipleFormula();
                    mf.AddItem(runtime, val.Pow);
                    return mf;
                }
            }
            return null;
        }
        public static bool IsConstValue(RuntimeData runtime , INumber num) {

            var res = num.Eval(runtime);

            if (res is IConstParameter)
                return true;
            if (res is AdditionFormula || res is MultipleFormula) {
                for (int i = 0; i < (res as IFormula).Count; i++) {
                    if (!((res as IFormula).Items[i] is IConstParameter))
                        return false;
                }
                return true;
            }
            if (res is Fraction) {
                return
                    IsConstValue(runtime, (res as Fraction).Denominator) &&
                    IsConstValue(runtime, (res as Fraction).Numerator);
            }

            return false;

        }

        public string Output(OutputType type, INumber[] num, INumber pow) {

            StringBuilder sb = new StringBuilder();

            if (num[0] is Variable && (num[0] as Variable).Name == "x")
                sb.Append(string.Format("({0})'", num[1]));
            else
                sb.Append(string.Format("Diff({0}, {1})", num[0], num[1]));

            if (!(pow is Number && (pow as Number).Value == 1)) {
                sb.Insert(0, "(");
                sb.Append("^");
                if (type == OutputType.String) {
                    sb.Append(pow.Output(type));
                }
                else if (type == OutputType.Mathjax) {
                    sb.Append("{");
                    sb.Append(pow.Output(type));
                    sb.Append("}");
                }
                else

                    throw new NotImplementedException();
                sb.Append(")");
            }

            return sb.ToString();
        }

    }
}
