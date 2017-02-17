using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Expression;

namespace FuncCalc.Runtime.Func
{
    public class Pow : IFunction
    {
        public override string Name
        {
            get
            {
                return "pow";
            }
        }
        public override string Description
        {
            get
            {
                return "べき乗の計算をします";
            }
        }
        public override ExpressionType[] Parameter
        {
            get { return new ExpressionType[] { ExpressionType.Anything, ExpressionType.Anything }; }
        }
        public override ExpressionType ReturnType
        {
            get
            {
                return ExpressionType.Unknown;
            }
        }
        public override INumber Execute(RuntimeData runtime, params INumber[] parameters) {
            var v = parameters[0].Clone();
            v = v.Power(runtime, parameters[1]);
            return v;
        }
    }
}
