using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Expression;
using FuncCalc.Expression.Const;
using FuncCalc.Exceptions;

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
            
            // パラメータが定数なら早速値を求める
            if (parameters[0] is IConstParameter) {
                return new FloatNumber((decimal)System.Math.Tan((double)(parameters[0] as IConstParameter).ConstValue));
            }
            else {
                // sin(x)の値を取得して、結果がFuncedINumberじゃないときは 
                // sin(x) / cos(x)で結果を求める
                var sin =
                    runtime.Functions.Where(i => i.Key == "sin")
                    .First()
                    .Value.Execute(runtime, parameters);
                if (sin is FuncedINumber) {
                    return new FuncedINumber(this, parameters);
                }
                else {
                    var cos =
                        runtime.Functions.Where(i => i.Key == "cos")
                        .First()
                        .Value.Execute(runtime, parameters);

                    // cosが0になる時があるので、その時はエラーを返す
                    if (cos.Equals(runtime, Number.New(0)))
                        throw new RuntimeException("tan(" + parameters[0].ToString() + ")の結果はありません。", parameters[0]);

                    return new Fraction(cos, sin).Optimise(runtime);
                }
            }
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
