using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Expression;

namespace FuncCalc.Runtime.Func.Trigonometric
{
    public class Tangent : IFunction, IDiffWithParameters
    {
        public override string Name
        {
            get
            {
                return "tan";
            }
        }

        public override ExpressionType[] Parameter
        {
            get
            {
                return new ExpressionType[] { ExpressionType.Unknown };
            }
        }

        public override ExpressionType ReturnType
        {
            get
            {
                return ExpressionType.Unknown;
            }
        }
        public bool InfinitelyDifferentiable
        {
            get { return true; }
        }

        public override INumber Execute(RuntimeData runtime, params INumber[] parameters) {
            
            if (parameters[0] is IConstParameter && false) {
                return new FloatNumber((decimal)System.Math.Tan((double)(parameters[0] as IConstParameter).ConstValue));
            }

            return new FuncedINumber(this, parameters);

        }
        public override INumber ForceExecute(RuntimeData runtime, params INumber[] parameters) {
            var param = parameters[0].FinalEval(runtime);
            if (runtime.IsConstValue(param)) {
                return new FloatNumber((decimal)(System.Math.Tan((double)(param as IConstParameter).ConstValue)));
            }
            return param;
        }

        public INumber ExecuteDiff(RuntimeData runtime, string t, INumber[] parameters) {

            Fraction res = new Expression.Fraction(
                new FuncedINumber(runtime.Functions["cos"], parameters),
                new FuncedINumber(runtime.Functions["sin"], parameters)
                );

            return res.ExecuteDiff(runtime, t);

        }
    }
}
