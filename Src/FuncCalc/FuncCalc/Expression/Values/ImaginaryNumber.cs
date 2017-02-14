using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncCalc.Runtime;
using FuncCalc.Exceptions;
using FuncCalc.Interface.Math;

namespace FuncCalc.Expression
{
    // 実態のある数値ではないのでIConstParamはつけないで
    public class ImaginaryNumber : INumber, IOutput,
        IAbs
    {
        private long _val = 0;

        private ImaginaryNumber() { }
        public ImaginaryNumber(long val) {
            this._val = val;
        }

        public long Value
        {
            get { return this._val; }
        }
        public override int SortPriority
        {
            get
            {
                return 125;
            }
        }

        public override ExpressionType Type
        {
            get
            {
                return ExpressionType.Unknown;
            }
        }

        public override ValueType ValueType
        {
            get
            {
                return ValueType.Unknown;
            }
        }
        public override INumber Pow
        {
            get
            {
                return base.Pow;
            }
            protected internal set
            {
                throw new FuncCalcException("虚数のpowを直接いじることはできません。", this);
            }
        }
        public override bool InfinitelyDifferentiable
        {
            get
            {
                return false;
            }
        }
        public override bool ContainsImaginalyNumber
        {
            get
            {
                return true;
            }
        }

        public override INumber Add(RuntimeData runtime, INumber val) {

            if (val is ImaginaryNumber) {
                ImaginaryNumber res = new ImaginaryNumber();
                res._val = this.Value + (val as ImaginaryNumber).Value;
                return res;
            }

            if (!(val is IConstParameter))
                throw new NotImplementedException("複素数の四則計算は現在、実数と虚数しか対応していません。");

            AdditionFormula af = new Expression.AdditionFormula();
            af.AddItem(runtime, val);
            af.AddItem(runtime, this);
            return af;
        }
        public override INumber Multiple(RuntimeData runtime, INumber val) {

            if (val is ImaginaryNumber) {
                Number res = Number.New(this.Value * (val as ImaginaryNumber).Value * -1);
                return res;
            }

            if (!(val is IConstParameter))
                throw new NotImplementedException("複素数の四則計算は現在、実数と虚数しか対応していません。");

            MultipleFormula mf = new MultipleFormula();
            mf.AddItem(runtime, val);
            mf.AddItem(runtime, this);
            return mf;

        }
        public override INumber Power(RuntimeData runtime, INumber val) {
            if (val is Number) {
                var pow = val as Number;
                if (pow.Value == 0) return Number.New(1);
                if (pow.Value < 0) { // 乗数がマイナスだったら
                    var res = this.Clone();
                    res = res.Power(runtime, Number.New(runtime, pow.Value * -1));
                    return new Fraction(res, Number.New(1));
                }
                if (pow.Value % 2 == 0) { // 乗数が偶数だったら
                    if (pow.Value % 4 == 0) {
                        return Number.New(this.Value).Power(runtime, pow);
                    } else {
                        return Number.New(this.Value).Power(runtime, pow).Multiple(runtime, Number.New(-1));
                    }
                }else {
                    MultipleFormula mf = new MultipleFormula();
                    mf.AddItem(runtime, Number.New(this.Value).Power(runtime, Number.New(runtime, pow.Value - 1)));
                    mf.AddItem(runtime, this);
                    return mf;
                }
            }

            throw new NotImplementedException("虚数に数値以外のべき乗含むのはまだ実装していません。");
        }

        public override bool CanJoin(RuntimeData runtime, INumber val) {
            return val is ImaginaryNumber;
        }
        public override bool Equals(RuntimeData runtime, INumber val) {
            return
                val is ImaginaryNumber &&
                this.Value == (val as ImaginaryNumber).Value;
        }

        public override INumber Differentiate(RuntimeData runtime, DifferentialData ddata) {
            // 参考 http://takeno.iee.niit.ac.jp/~shige/math/lecture/basic3/quotef4/node8.html
            return ddata.DifferentiateConstant(this);
        }
        public override INumber Integrate(RuntimeData runtime, string t) {
            // 参考 https://oshiete.goo.ne.jp/qa/2664611.html

            MultipleFormula mf = new Expression.MultipleFormula();
            mf.AddItem(runtime, new Variable(t));
            mf.AddItem(runtime, this);
            return mf;


        }
        public INumber Abs(RuntimeData runtime) {
            return Number.New(Math.Abs(this.Value));
        }


        public override string Output(OutputType type) {
            switch (type) {
                case OutputType.String:
                    return string.Format("{0}i", this.Value);
                case OutputType.Mathjax:
                    return string.Format("{0}i", this.Value);
                default:
                    throw new NotImplementedException();
            }
        }
        public override string ToString() {
            return this.Output(OutputType.String);
        }
    }
}
