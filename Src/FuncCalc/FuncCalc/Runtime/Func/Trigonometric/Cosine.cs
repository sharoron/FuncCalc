using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Expression;
using FuncCalc.Exceptions;

namespace FuncCalc.Runtime.Func.Trigonometric
{
    public class Cosine : IFunction, IDiffWithParameters, IIntegrateWithParameters
    {
        public override string Name
        {
            get
            {
                return "cos";
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
        public bool InfinitelyIntegrable
        {
            get { return true; }
        }

        public override INumber Execute(RuntimeData runtime, params INumber[] parameters) {
            
            if (parameters[0] is IConstParameter && false) {
                return new FloatNumber((decimal)System.Math.Cos((double)(parameters[0] as IConstParameter).ConstValue));
            }

            return new FuncedINumber(this, parameters);

        }
        public override INumber ForceExecute(RuntimeData runtime, params INumber[] parameters) {
            var param = parameters[0].FinalEval(runtime);
            if (runtime.IsConstValue(param)) {
                return new FloatNumber((decimal)(System.Math.Cos((double)(param as IConstParameter).ConstValue)));
            }
            return param;
        }

        public INumber ExecuteDiff(RuntimeData runtime, string t, INumber[] parameters) {
            
            var res = new FuncedINumber(runtime.Functions["sin"], parameters);

            MultipleFormula mf = new Expression.MultipleFormula();
            mf.AddItem(runtime, Number.New(-1));
            mf.AddItem(runtime, res);

            var vaDiff = parameters[0].ExecuteDiff(runtime, t);
            if (vaDiff .Equals(runtime, Number.New(0))) {
                return mf;
            }

            mf.AddItem(runtime, vaDiff);
            return mf;

        }
        public INumber Integrate(RuntimeData runtime, string t, INumber[] parameters) {

            // パラメータ内に関数を含む場合は部分積分法で積分する必要がある
            if (runtime.IsFunctionINumber(parameters[0], t)) {

                var prmDiff = parameters[0].ExecuteDiff(runtime, t);
                if ((prmDiff.InfinitelyDifferentiable && this.InfinitelyIntegrable) ||
                    (prmDiff.InfinitelyIntegrable && this.InfinitelyDifferentiable))
                    throw new RuntimeException("パラメータを微分した結果が無限回微分可能関数または無限回積分可能関数の場合、積分を行うことはできません。", this);

                MultipleFormula mf = new Expression.MultipleFormula();
                mf.AddItem(runtime, new Fraction(prmDiff, Number.New(1)));
                mf.AddItem(runtime, new FuncedINumber(runtime.Functions["sin"], parameters));

                {
                    runtime.AddLogWay("_DisplacementIntegralWay1",
                        parameters[0], new Variable(t));
                    runtime.AddLogWay("_DisplacementIntegralWay2",
                        parameters[0], prmDiff);
                    runtime.AddLogWay("_DisplacementIntegralWay3",
                        new FuncedINumber(this, new[] { new Variable("t") }), prmDiff,
                        new FuncedINumber(this, parameters) ,
                        mf, new Variable(t));

                }

                return mf;
            }

            return
                new FuncedINumber(runtime.Functions["sin"], parameters)
                ;
        }

    }
}
