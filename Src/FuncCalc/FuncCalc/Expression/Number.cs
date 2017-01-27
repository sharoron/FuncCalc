using FuncCalc.Exceptions;
using FuncCalc.Interface;
using FuncCalc.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Expression
{
    public class Number : INumber, IExpression, IConstParameter
    {

        private long _value = 0;

        private Number() { }
        public Number(Token t) {
            this.Token = t;
            this._value = long.Parse(t.Text);
        }
        public Number(long value)
        {
            this._value = value;
        }

        public long Value
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
            get { return this.Value; }
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

        public override INumber Add(RuntimeData runtime, INumber val) {

            if (val is Number) {
                if (this.Pow.Equals(runtime, val.Pow))
                    return Number.New(this.Value + (val as Number).Value);
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
                    return Number.New(this.Value - (val as Number).Value);
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
                    return Number.New(this.Value * (v as Number).Value);
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

            if (val is Number && (val as Number).Value == 0) {
                if (this.ValueType == ValueType.Plus)
                    return InfinityValue.PlusValue;
                else
                    return InfinityValue.MinusValue;
                // ValueType.Unknownになることはない
            }

            if (v is Number) {
                if (this.Pow.Equals(runtime, val.Pow)) {
                    if (this.Value % (v as Number).Value == 0) // 割り切れるなら数値として返す
                        return Number.New(this.Value / (v as Number).Value);
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

            if (val is Number && (val as Number).Value < 0)
                throw new NotImplementedException("マイナスを含むべき乗はまだ正常に実装されていません。");

            if (val is Number) {
                // 正の整数の場合のみ対応する
                if ((val as Number).Value >= 1) {
                    var res = me.Value;
                    for (int i = 0; i < (val as Number).Value - 1; i++) {
                        me.Value *= res;
                    }
                    me.Pow = Number.New(1);
                    return me;
                }
                else if ((val as Number).Value == 0)
                    return Number.New(1);
                else
                    return new Fraction(me.Power(runtime, Number.New((val as Number).Value * -1)), Number.New(1));
            }
            me.Pow = val;
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
        public override INumber ExecuteDiff(RuntimeData runtime, string t) {
            return Number.New(0);
        }
        public static Number New(long val) {
            return new Number(val);
        }
        public override string ToString() {
            return 
                string.Format("{0}{1}", 
                    this._value.ToString(),
                    this.Pow == null || (this.Pow is Number && (this.Pow as Number).Value == 1) ? 
                        "" : "^(" + this.Pow.ToString() + ")");
        }
        public override string Output(OutputType type) {
            switch (type) {
                case OutputType.Mathjax:
                    return this.Value.ToString();
                    break;
            }
            return base.Output(type);
        }
    }
}
