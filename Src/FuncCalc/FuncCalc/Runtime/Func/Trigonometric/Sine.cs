using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Expression;

namespace FuncCalc.Runtime.Func.Trigonometric
{
    public class Sine : IFunction, IDiffWithParameters
    {
        public override string Name
        {
            get
            {
                return "sin";
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

        public override INumber Execute(RuntimeData runtime, params INumber[] parameters) {
            
            if (parameters[0] is IConstParameter && false) {
                return new FloatNumber((decimal)System.Math.Sin((double)(parameters[0] as IConstParameter).ConstValue));
            }

            return new FuncedINumber(this, parameters);

        }
        public override INumber ForceExecute(RuntimeData runtime, params INumber[] parameters) {
            var param = parameters[0].FinalEval(runtime);
            if (runtime.IsConstValue(param)) {
                return new FloatNumber((decimal)(System.Math.Sin((double)(param as IConstParameter).ConstValue)));
            }
            return param;
        }

        public INumber ExecuteDiff(RuntimeData runtime, string t, INumber[] parameters) {
            
            var res = new FuncedINumber(runtime.Functions["cos"], parameters);

            var vaDiff = parameters[0].ExecuteDiff(runtime, t);
            if (vaDiff .Equals(runtime, Number.New(0))) {
                return res;
            }

            MultipleFormula mf = new MultipleFormula();
            mf.AddItem(runtime, vaDiff);
            mf.AddItem(runtime, res);
            return mf;

        }
    }
}
