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
    public class Lcm : IFunction
    {
        public override string Name
        {
            get
            {
                return "lcm";
            }
        }
        public override string Description
        {
            get
            {
                return "最小公倍数を求めます。";
            }
        }
        public override ExpressionType[] Parameter
        {
            get { return new ExpressionType[] { ExpressionType.Number, ExpressionType.Number }; }
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
            var r = parameters[1].FinalEval(runtime);

            // lcm(l, r) = l * r / gcd(l, r)
            return l.Multiple(runtime, r).Divide(runtime, 
                runtime.GetFunc("gcd").Execute(runtime, l, r));

        }
    }
}
