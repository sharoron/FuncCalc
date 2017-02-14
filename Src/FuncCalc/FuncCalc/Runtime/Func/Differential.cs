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

            string t = null;
            if (parameters[0] is Variable ||
                parameters[1] is Member) {
                t = parameters[0].Token.Text;
            }
            else
                throw new SyntaxException("diff関数の第1パラメータの値はメンバーである必要があります。", parameters[0]);

            var res = parameters[1].Differentiate(runtime, t);
            if (runtime.Setting.DoOptimize)
                return res.Optimise(runtime);
            else
                return res;
            

        }
        public static bool IsConstValue(RuntimeData runtime , INumber num, string t) {
            return !DifferentialData.IsFunction(num, 
                t);
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
