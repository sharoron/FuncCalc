using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Runtime;
using FuncCalc.Exceptions;

namespace FuncCalc.Expression
{
    public class InfinityValue : INumber, IConstParameter, IOutput
    {
        private static InfinityValue value;
        private static InfinityValue minusvalue;

        private ValueType valtype = ValueType.Plus;

        static InfinityValue() {
            value = new InfinityValue() { valtype = ValueType.Plus };
            minusvalue = new InfinityValue() { valtype = ValueType.Minus };
        }
        private InfinityValue() { }

        public override ExpressionType Type
        {
            get
            {
                return ExpressionType.Number;
            }
        }
        public override ValueType ValueType
        { get { return this.valtype; } }

        public static InfinityValue PlusValue
        {
            get { return InfinityValue.value; }
        }
        public static InfinityValue MinusValue
        {
            get { return InfinityValue.minusvalue; }
        }
        public override int SortPriority {
            get { return 50; }
        }

        public decimal ConstValue
        {
            get
            {
                throw new FuncCalcException("Infinity ValueからConstValueを取得することはできません。");
            }
        }

        public override INumber Clone() {
            return this;
        }

        public override INumber Add(RuntimeData runtime, INumber val) {
            if ((this == InfinityValue.value && val == InfinityValue.minusvalue) || 
                 this == InfinityValue.minusvalue && val == InfinityValue.value)
                throw new RuntimeException("Infinityと-Infinityで計算することはできません", val);

            return this;
        }
        public override INumber Multiple(RuntimeData runtime, INumber val) {
            if (val.ValueType == ValueType.Plus)
                return this == InfinityValue.value ? InfinityValue.value : InfinityValue.minusvalue;
            else if (val.ValueType == ValueType.Minus)
                return this == InfinityValue.value ? InfinityValue.minusvalue : InfinityValue.value;
            else { // val.ValueType == ValueType.Unknown
                if (val is MultipleFormula) {
                    var res = this;
                    MultipleFormula mf = new Expression.MultipleFormula();
                    for (int i = 0; i < (val as MultipleFormula).Items.Count; i++) {
                        var item = (val as MultipleFormula).Items[i];
                        if (item.ValueType == ValueType.Minus)
                            res = (res == InfinityValue.value) ? InfinityValue.minusvalue : InfinityValue.value;
                        if (item.ValueType == ValueType.Unknown)
                            mf.AddItem(runtime, item);
                    }
                    mf.AddItem(runtime, res);
                    return mf;
                }
                else {
                    MultipleFormula mf = new MultipleFormula();
                    mf.AddItem(runtime, this);
                    mf.AddItem(runtime, val);
                    return mf;
                }
            }
        }
        public override INumber Power(RuntimeData runtime, INumber val) {
            return this;
        }

        public override bool CanJoin(RuntimeData runtime, INumber val) {
            if (val is Number || val is Fraction || val is IConstParameter)
                return true;
            else
                return false;
        }
        public override bool Equals(RuntimeData runtime, INumber val) {
            return false;
        }

        public override string ToString() {
            return this.ValueType == ValueType.Plus ?
                "∞" :
                "-∞";
        }
        public override string Output(OutputType type) {
            switch (type) {
                case OutputType.Mathjax:
                    return "\\infty";
                default:
                    return this.ToString();
            }
        }

        public override INumber ExecuteDiff(RuntimeData runtime, string t) {
            throw new RuntimeException("無限を微分することはできません。");
        }
    }
}
