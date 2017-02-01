using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncCalc.Runtime;

namespace FuncCalc.Expression
{
    public class ImaginaryNumber : INumber, IOutput
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

        public override INumber Add(RuntimeData runtime, INumber val) {

            if (val is ImaginaryNumber) {
                ImaginaryNumber res = new ImaginaryNumber();
                res._val = this.Value + (val as ImaginaryNumber).Value;
                return res;
            }

            AdditionFormula af = new Expression.AdditionFormula();
            af.AddItem(runtime, val);
            af.AddItem(runtime, this);
            return af;
        }

        public override bool CanJoin(RuntimeData runtime, INumber val) {
            return val is ImaginaryNumber;
        }

        public override bool Equals(RuntimeData runtime, INumber val) {
            return
                val is ImaginaryNumber &&
                this.Value == (val as ImaginaryNumber).Value;
        }

        public override INumber ExecuteDiff(RuntimeData runtime, string t) {
            // 参考 http://takeno.iee.niit.ac.jp/~shige/math/lecture/basic3/quotef4/node8.html
            return Number.New(0);
        }

        public override INumber Multiple(RuntimeData runtime, INumber val) {
           
            if (val is ImaginaryNumber) {
                Number res = Number.New(this.Value * (val as ImaginaryNumber).Value * -1);
                return res;
            }

            MultipleFormula mf = new MultipleFormula();
            mf.AddItem(runtime, val);
            mf.AddItem(runtime, this);
            return mf;

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
    }
}
