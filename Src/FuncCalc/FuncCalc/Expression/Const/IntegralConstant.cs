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
    public class IntegralConstant : INumber, IConstParameter, IOutput
    {
        int id = 0;

        private IntegralConstant() {
        }
        public static IntegralConstant Create(RuntimeData runtime) {
            int i = 1;
            for (; ; i++) {
                if (!runtime.ContainsKey("C" + i, true))
                    break;
            }

            IntegralConstant res = new Expression.IntegralConstant();
            res.id = i;
            return res;
        }

        public override ExpressionType Type
        {
            get
            {
                return ExpressionType.Number;
            }
        }
        public override ValueType ValueType
        { get { return ValueType.Unknown; } }
        
        public override int SortPriority {
            get { return 9000; }
        }

        public decimal ConstValue
        {
            get
            {
                return 0;
            }
        }

        public override INumber Clone() {
            return this;
        }

        public override INumber Add(RuntimeData runtime, INumber val) {
            AdditionFormula af = new AdditionFormula();
            af.AddItem(runtime, val);
            af.AddItem(runtime, this);
            return af;
        }
        public override INumber Multiple(RuntimeData runtime, INumber val) {
            MultipleFormula mf = new MultipleFormula();
            mf.AddItem(runtime, val);
            mf.AddItem(runtime, this);
            return mf;
        }
        public override bool CanJoin(RuntimeData runtime, INumber val) {
            return false;
        }
        public override bool Equals(RuntimeData runtime, INumber val) {
            return false;
        }

        public override string ToString() {
            return "C" + id;
        }
        public override string Output(OutputType type) {
            switch (type) {
                case OutputType.Mathjax:
                    return "C_" + id;
                default:
                    return "C" + id;
            }
        }

        public override INumber ExecuteDiff(RuntimeData runtime, string t) {
            return Number.New(0);
        }
    }
}
