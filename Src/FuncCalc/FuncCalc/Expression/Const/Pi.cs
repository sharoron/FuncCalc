using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Interface;

namespace FuncCalc.Expression.Const {
    public class Pi : INumber, IConstParameter, IOutput {

        public override ExpressionType Type {
            get { return ExpressionType.Number; }
        }
        public override ValueType ValueType {
            get { return Expression.ValueType.Plus; }
        }
        public override int SortPriority {
            get { return 150; }
        }
        public override bool InfinitelyDifferentiable
        {
            get
            {
                return false;
            }
        }

        public override INumber Add(Runtime.RuntimeData runtime, INumber val) {
            AdditionFormula af = new AdditionFormula();
            af.AddItem(runtime, this);
            af.AddItem(runtime, val);
            return af;
        }
        public override INumber Multiple(Runtime.RuntimeData runtime, INumber val) {
            MultipleFormula mf = new MultipleFormula();
            mf.AddItem(runtime, this);
            mf.AddItem(runtime, val);
            return mf;
        }
        public override bool Equals(Runtime.RuntimeData runtime, INumber val) {
            if (val is Pi && val.Pow.Equals(runtime, Number.New(1))) {
                return true;
            }
            return false;
        }
        public override bool CanJoin(Runtime.RuntimeData runtime, INumber val) {
            return false;
        }
        public override INumber ExecuteDiff(Runtime.RuntimeData runtime, string t) {
            return Number.New(0);
        }

        #region IConstParameter メンバー

        public decimal ConstValue {
            get { return (decimal)Math.PI; }
        }

        #endregion

        public override string Output(Runtime.OutputType type) {
            return "π";
        }
        public override string ToString() {
            return this.Output(Runtime.OutputType.String);
        }

    }
}
