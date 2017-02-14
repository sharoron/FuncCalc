using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Runtime;

namespace FuncCalc.Expression
{
    public class FloatNumber : INumber, IConstParameter
    {
        private decimal value = 0m;

        public FloatNumber() { }
        public FloatNumber(decimal val) {
            this.value = val;
        }

        public decimal Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }
        public decimal ConstValue
        {
            get { return this.value; }
            set { this.value = value; }
        }
        public override ExpressionType Type
        {
            get
            {
                return ExpressionType.Nothing;
            }
        }
        public override ValueType ValueType
        {
            get
            {
                return this.value >= 0 ? ValueType.Plus : ValueType.Minus;
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
            if (val is InfinityValue) {
                return val;
            }
            if ( val is IConstParameter) {
                var me = this.Clone() as FloatNumber;
                me.value += (val as IConstParameter).ConstValue;
                return me;
            }

            AdditionFormula af = new AdditionFormula();
            af.AddItem(runtime, val);
            af.AddItem(runtime, this);
            return af;
        }

        public override bool CanJoin(RuntimeData runtime, INumber val) {
            throw new NotImplementedException();
        }

        public override bool Equals(RuntimeData runtime, INumber val) {
            return 
                (val is IConstParameter && this.value == (val as IConstParameter).ConstValue);
        }

        public override INumber Differentiate(RuntimeData runtime, DifferentialData ddata) {
            return ddata.DifferentiateConstant(this);
        }

        public override INumber Multiple(RuntimeData runtime, INumber val) {
            if (val is IConstParameter) {
                var me = this.Clone() as FloatNumber;
                me.value *= (val as IConstParameter).ConstValue;
                return me;
            }
            if (val is InfinityValue) {
                return val.Multiple(runtime, this);
            }

            MultipleFormula mf = new MultipleFormula();
            mf.AddItem(runtime, val);
            mf.AddItem(runtime, this);

            return mf;
        }
        public override INumber Power(RuntimeData runtime, INumber val) {
            var pow = val.FinalEval(runtime);

            if (pow is IConstParameter && (pow as IConstParameter).ConstValue == 0)
                return Number.New(1);

            if (runtime.IsConstValue(pow)) {
                var me = this.Clone() as FloatNumber;
                me.value = (decimal)Math.Pow((double)this.value, (double)(pow as IConstParameter).ConstValue);
                return me;
            }
            return base.Power(runtime, val);
        }

        public override string Output(OutputType type) {
            return this.ToString();
        }
        public override string ToString() {
            return this.value.ToString();
        }
    }
}
