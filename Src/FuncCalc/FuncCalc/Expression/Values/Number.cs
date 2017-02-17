using FuncCalc.Exceptions;
using FuncCalc.Interface;
using FuncCalc.Interface.Math;
using FuncCalc.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Expression
{
    public class Number : INumber, IExpression, IConstParameter,
        IAbs
    {

        private BigInteger _value = 0;
        private readonly BigInteger maxVal = BigInteger.Parse("100000000000"); // 1000億

        private Number() { }
        private Number(Token t) {
            this.Token = t;
            this._value = long.Parse(t.Text);
        }
        public Number(long value)
        {
            this._value = value;
        }
        public Number(BigInteger value) {
            this._value = value;
        }

        public BigInteger Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
            }
        }
        public decimal ConstValue
        {
            get { return (decimal)this.Value; }
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
                return this._value < 0 ? ValueType.Minus : ValueType.Plus;
            }
        }
        public override int SortPriority {
            get { return 100; }
        }
        public override bool InfinitelyDifferentiable
        {
            get
            {
                return false;
            }
        }

        public override INumber Add(RuntimeData runtime, INumber val) {

            if (val is Number) {
                if (this.Pow.Equals(runtime, val.Pow))
                    return Number.New(runtime, this.Value + (val as Number).Value);
                else {
                    AdditionFormula af = new Expression.AdditionFormula();
                    af.AddItem(runtime, this);
                    af.AddItem(runtime, val);
                    return af;
                }
            }

            return val.Add(runtime, this);
        }
        public override INumber Subtract(RuntimeData runtime, INumber val) {
            var v = val.Eval(runtime);

            if (v is Number) {
                if (this.Pow.Equals(runtime, val.Pow))
                    return Number.New(runtime, this.Value - (val as Number).Value);
                else {
                    AdditionFormula af = new Expression.AdditionFormula();
                    af.AddItem(runtime, this);
                    af.AddItem(runtime, val.Multiple(runtime, Number.New(-1)));
                    return af;
                }
            }
            if (v is Fraction)
                return v.Multiple(runtime, Number.New(-1)).Add(runtime, this);

            return v.Subtract(runtime, this);

        }
        public override INumber Multiple(RuntimeData runtime, INumber val) {
            if (this.Value == 0)
                return Number.New(0);

            var v = val.Eval(runtime);

            if (v is Number) {
                if (this.Pow.Equals(runtime, val.Pow)) {
                    if ((this.Value.ToByteArray().Length +
                        (v as Number).Value.ToByteArray().Length - 1)
                        * sizeof(byte) >
                        runtime.Setting.AcceptBitLength)
                        throw new RuntimeException("このかけ算は許可された範囲桁数を超える可能性があるため、計算できません。", val);

                    return Number.New(runtime, this.Value * (v as Number).Value);
                } else {
                    MultipleFormula mf = new MultipleFormula();
                    mf.AddItem(runtime, this);
                    mf.AddItem(runtime, val);
                    return mf;
                }
            }

            return val.Multiple(runtime, this);

        }
        public override INumber Divide(RuntimeData runtime, INumber val) {
            var v = val.Eval(runtime);

            if (val.IsZero) {
                if (this.ValueType == ValueType.Plus)
                    return InfinityValue.PlusValue;
                else
                    return InfinityValue.MinusValue;
                // ValueType.Unknownになることはない
            }

            if (v is Number) {
                if (this.Pow.Equals(runtime, val.Pow)) {
                    if (this.Value % (v as Number).Value == 0) // 割り切れるなら数値として返す
                        return Number.New(runtime, this.Value / (v as Number).Value);
                    else // 割り切れないなら分数として返す
                        return new Fraction(v, this);
                } else {
                    return new Fraction(v, this);
                }
            }

            if (v is Fraction) 
                return
                    (new Fraction((v as Fraction).Numerator, (v as Fraction).Denominator)).Multiple(runtime, this);

            // こちらの方で対応していないものも半ば強引に解決させる
            return new Fraction(val, this);

        }
        public override INumber Power(RuntimeData runtime, INumber val) {
            var me = this.Clone() as Number;
            me.Pow = me.Pow.Multiple(runtime, val);
            me.Pow = me.Pow.Optimise(runtime);

            if (me.Pow is Number && (me.Pow as Number).Value < 0) {
                return new Fraction(
                    Number.New(runtime, this.Value).Power(runtime, me.Pow.Multiple(runtime, Number.New(-1))) , 
                    Number.New(1));
            }

            if (me.Pow is Number) {
                if ((me.Pow as Number).Value <= 20) {
                    // 10乗以下ならそのまま計算する
                    BigInteger res = new BigInteger(1);
                    bool flag = false;
                    for (int i = 0; i < (me.Pow as Number).Value; i++) {
                        res *= me.Value;
                        
                        // maxValを超えた場合、大きすぎる値として取り扱わないことにする
                        if (false && res >= maxVal) {
                            flag = true;
                            break;
                        }

                        if (res.ToByteArray().Length * sizeof(byte) > runtime.Setting.AcceptBitLength) {
                            flag = true;
                            break;
                        }
                    }

                    if (!flag)
                        return Number.New(runtime, res);
                }
                else if ((val as Number).Value >= 1) {
                    // Powに代入する
                }
                else if ((val as Number).Value == 0)
                    return Number.New(1);
                else
                    throw new FuncCalcException("上部にすでに同等のコードが存在しているにも関わらずこのコードが呼び出されました。");
            }

            return me;
        }
        public override bool Equals(RuntimeData runtime, INumber val) {
            
            if (val is Number) {
                return this._value == (val as Number).Value;
            }

            if (val is Fraction) {
                return (val as Fraction).Optimise(runtime).Equals(val);
            }

            return val.Equals(runtime, this);

        }
        public override bool CanJoin(RuntimeData runtime, INumber val) {
            if (val is Number || val is Fraction)
                return true;
            return false;
        }

        public override INumber Differentiate(RuntimeData runtime, DifferentialData ddata) {
            return ddata.DifferentiateConstant(this);
        }
        public override INumber Integrate(RuntimeData runtime, string t) {
            return base.Integrate(runtime, t);
        }
        public INumber Abs(RuntimeData runtime) {
            if (this.Value < 0)
                return this.Multiple(runtime, Number.New(-1));
            else
                return this.Clone();
        }

        public static Number New(RuntimeData runtime, Token t) {
            BigInteger bi = BigInteger.Parse(t.Text);

            return Number.New(runtime.Setting, bi);
        }
        public static Number New(RuntimeSetting setting, Token t) {
            BigInteger bi = BigInteger.Parse(t.Text);

            return Number.New(setting, bi);
        }
        public static Number New(long val) {
            return new Number(val);
        }
        public static Number New(RuntimeData runtime, BigInteger val) {
            return Number.New(runtime.Setting, val);
        }
        public static Number New(RuntimeSetting setting, BigInteger val) {
            if (val.ToByteArray().Length * sizeof(byte) > setting.AcceptBitLength)
                throw new RuntimeException("これ以上の桁の値の演算は許可されていません。");

            return new Number(val);
        }
        internal static Number New(BigInteger val) {
            return new Number(val);
        }
        public static bool IsZero(INumber num) {
            if (num is Number && (num as Number).Value == 0)
                return true;
            else
                return false;
        }
        public static bool IsOne(INumber num) {
            // 無限ループ防止に_powの内容がnullなら1と事前にみなす
            bool powOne = (num is Number && (num as Number)._pow == null);
                

            if (!powOne && num.Pow.IsZero)
                return true;
            if (num is Number && (num as Number).Value == 1) {
                if (powOne || num.Pow.IsOne)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public override string ToString() {
            return 
                string.Format("{0}{1}", 
                    this._value.ToString(),
                    this._pow != null && !this._pow.IsOne ? 
                        "^(" + this._pow.ToString() + ")" : "");
        }
        public override string Output(OutputType type) {
            switch (type) {
                case OutputType.Mathjax:
                    return this.Value.ToString() + 
                        (this._pow != null && !this._pow.IsOne ? 
                        "^{" + this._pow.Output(type) + "}" : "");
            }
            return base.Output(type);
        }
    }
}
