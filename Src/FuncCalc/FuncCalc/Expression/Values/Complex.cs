using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncCalc.Runtime;
using FuncCalc.Exceptions;

namespace FuncCalc.Expression
{
    /// <summary>複素数</summary>
    public class Complex : INumber, IHighPriorityValue
    {
        private INumber
            real = null,
            imaginary = null;

        private Complex() { }
        public Complex(INumber real, INumber imaginary) {
            this.real = real;
            this.imaginary = imaginary;
        }

        public override bool InfinitelyDifferentiable
        {
            get
            {
                return
                    real.InfinitelyDifferentiable ||
                    imaginary.InfinitelyDifferentiable;
            }
        }
        public override int SortPriority
        {
            get
            {
                return 110;
            }
        }
        public override ExpressionType Type
        {
            get
            {
                return ExpressionType.Number;
            }
        }
        public override ValueType ValueType
        {
            get
            {
                return real.ValueType;
            }
        }

        public override INumber Add(RuntimeData runtime, INumber val) {

            var res = this.Clone() as Complex;
            if (val is ImaginaryNumber) {
                res.imaginary = res.imaginary.Add(runtime, val);
                return res;
            }
            if (val is Complex) {
                var v = val as Complex;
                res.real = res.real.Add(runtime, v.real);
                res.imaginary = res.imaginary.Add(runtime, v.imaginary);
                return res;
            }

            // その他
            {
                res.real = res.real.Add(runtime, val);
                return res;
            }
        }
        public override INumber Multiple(RuntimeData runtime, INumber val) {

            var res = this.Clone() as Complex;
            if (val is ImaginaryNumber) {
                res.real = this.imaginary
                    .Multiple(runtime, Number.New((val as ImaginaryNumber).Value))
                    .Multiple(runtime, Number.New(-1));
                res.imaginary = this.real
                    .Multiple(runtime, Number.New((val as ImaginaryNumber).Value));
                return res;
            }
            if (val is Complex) {
                var v = val as Complex;
                res.real = res.real.Add(runtime, v.real);
                res.imaginary = res.imaginary.Add(runtime, v.imaginary);
                return res;
            }

            // その他
            {
                res.real = this.real
                    .Multiple(runtime, val);
                res.imaginary = this.imaginary
                    .Multiple(runtime, val);
                return res;
            }

        }
        public override bool CanJoin(RuntimeData runtime, INumber val) {
            return true;
        }
        public override bool Equals(RuntimeData runtime, INumber val) {
            if (val is ImaginaryNumber) {
                return this.real.IsZero &&
                    this.imaginary.Equals(runtime, Number.New((val as ImaginaryNumber).Value));
            }
            if (val is Complex) {
                return
                    this.real.Equals(runtime, (val as Complex).real) &&
                    this.imaginary.Equals(runtime, (val as Complex).imaginary);
            }

            return this.imaginary.IsZero &&
                this.real.Equals(runtime, val);
        }
        public override INumber Differentiate(RuntimeData runtime, DifferentialData ddata) {
            throw new FuncCalcException("複素数の微分はまだ実装していません。", this);
        }

        public override string ToString() {
            return string.Format("({0} + ({1})i)", this.real, this.imaginary);
        }
        public override string Output(OutputType type) {
            switch (type) {
                case OutputType.String:
                    return this.ToString();
                case OutputType.Mathjax:
                    return string.Format("{0} + {1}i",
                        this.real.Output(type), this.imaginary.Output(type));
                default:
                    throw new NotImplementedException();
            }
        }

    }
}
