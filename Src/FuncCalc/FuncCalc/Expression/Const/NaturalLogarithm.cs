using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Runtime;

namespace FuncCalc.Expression.Const
{
    public class NaturalLogarithm : INumber, IConstParameter, IConstant
    {
        public const decimal Val = 2.718281828459M;

        public string Name
        {
            get { return "e"; }
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
                return ValueType.Plus;
            }
        }
        public decimal ConstValue
        {
            get { return NaturalLogarithm.Val; }
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

        public override INumber Add(RuntimeData runtime, INumber val) {
            AdditionFormula af = new Expression.AdditionFormula();
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
            return val is NaturalLogarithm && this.Pow.Equals(runtime, val.Pow);
        }

        public override string ToString() {
            return
                string.Format("e{0}",
                !this.Pow.IsOne ? 
                "^" + this.Pow.ToString() : "");
        }
        public override string Output(OutputType type) {
            switch (type) {
                case OutputType.String:
                    return this.ToString();
                case OutputType.Mathjax: {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("e");
                        if (!this.Pow.IsOne)
                            sb.Append("^{" + this.Pow.Output(type) + "}");
                        return sb.ToString();
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        public override INumber Differentiate(RuntimeData runtime, DifferentialData ddata) {
            return ddata.DifferentiateConstant(this);
        }
    }
}
