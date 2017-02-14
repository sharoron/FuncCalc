using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Expression;
using FuncCalc.Exceptions;

namespace FuncCalc.Runtime.Func.Math
{
    public class NaturalLogarithm : IFunction
    {
        public override string Name
        {
            get
            {
                return "in";
            }
        }
        public override string Description
        {
            get
            {
                return "自然対数を底に対数を求めます。";
            }
        }
        public override ExpressionType[] Parameter
        {
            get { return new ExpressionType[] { ExpressionType.Number }; }
        }
        public override ExpressionType ReturnType
        {
            get
            {
                return ExpressionType.Number;
            }
        }
        public override INumber Execute(RuntimeData runtime, params INumber[] parameters) {
            var l = parameters[0].FinalEval(runtime);

            var func = runtime.GetFunc("log");
            return func.Execute(
                runtime, 
                new Expression.Const.NaturalLogarithm(), l);

        }
    }
}
