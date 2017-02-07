using FuncCalc.Exceptions;
using FuncCalc.Expression;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuncCalc.Runtime.Func.Math
{
    public class SquareRoot : IFunction, IOutput, IFunctionOutput
    {
        public override string Name
        {
            get
            {
                return "sqrt";
            }
        }
        public override string Description
        {
            get
            {
                return "平方根を求めます";
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
            var val = parameters[0].FinalEval(runtime);

            if (val is Number) {

                // 答えが虚数になるもの
                if ((double)(val as Number).Value < 0) {
                    if ((val as Number).Value * -1 > long.MaxValue)
                        throw new RuntimeException("long.MaxValueを超える値の平方根を求めることはできません。");

                    var ires = this.Execute(runtime, Number.New(runtime, (val as Number).Value * -1));
                    if (ires is Number) {
                        return new ImaginaryNumber((long)(ires as Number).Value);
                    }else {
                        return ires;
                    }
                }

                if ((val as Number).Value > long.MaxValue)
                    throw new RuntimeException("long.MaxValueを超える値の平方根を求めることはできません。");
                
                var res = System.Math.Sqrt((double)(val as Number).Value);

                if (res - (long)res == 0d) {
                    return Number.New((long)res);
                }
                else {
                    return new FuncedINumber(this, parameters);
                }
            }
            if (val is IConstParameter)
                return new FloatNumber((val as IConstParameter).ConstValue);

            if (val is FuncedINumber) {
                var res = new Fraction(
                    this.Execute(runtime, (val as Fraction).Denominator),
                    this.Execute(runtime, (val as Fraction).Numerator)
                    );

                return res;
            }

            return new FuncedINumber(this, parameters);
        }
        public override INumber ForceExecute(RuntimeData runtime, params INumber[] parameters) {
            var val = parameters[0].FinalEval(runtime);

            if (val is Number) {

                if ((double)(val as Number).Value < 0)
                    throw new RuntimeException("現在、虚数には対応していません。", parameters[0]);

                var res = System.Math.Sqrt((double)(val as Number).Value);

                if (res - (long)res == 0d) {
                    return Number.New((long)res);
                }
                else {
                    return new FloatNumber((decimal)res);
                }
            }
            if (val is IConstParameter)
                return new FloatNumber((val as IConstParameter).ConstValue);

            if (val is FuncedINumber) {
                var res = new Fraction(
                    this.Execute(runtime, (val as Fraction).Denominator),
                    this.Execute(runtime, (val as Fraction).Numerator)
                    );

                return res.FinalEval(runtime);
            }

            return new FuncedINumber(this, parameters);
        }

        public string Output(OutputType type) {
            switch (type) {
                case OutputType.String:
                    return "√";
                case OutputType.Mathjax:
                    return "\\sqrt";
                default:
                    throw new NotFiniteNumberException();
            }
        }
        public string Output(OutputType type, INumber[] parameters, INumber pow) {
            switch (type) {
                case OutputType.String: {
                        string res = "√(" + parameters[0].Output(type) + ")";
                        if (!(pow is Number && (pow as Number).Value == 1)) {
                            res += "^" + pow.Output(type);
                        }
                        return res;
                    }
                case OutputType.Mathjax: {
                        string res = "\\sqrt {" + parameters[0].Output(type) + "}";
                        if (!(pow is Number && (pow as Number).Value == 1)) {
                            res += "^ {" + pow.Output(type) + "}";
                        }
                        return res;
                    }
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
