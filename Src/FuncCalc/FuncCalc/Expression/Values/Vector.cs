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
    public class Vector : INumber, IAbs
    {
        private INumber 
            x = null, 
            y = null;

        private Vector() { }
        public Vector(INumber x, INumber y) {
            this.x = x;
            this.y = y;
        }

        public override bool InfinitelyDifferentiable
        {
            get
            {
                return false;
            }
        }
        public override int SortPriority
        {
            get
            {
                return 600;
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
                return ValueType.Unknown;
            }
        }

        public INumber X
        {
            get { return this.x; }
            set { this.x = value; }
        }
        public INumber Y
        {
            get { return this.y; }
            set { this.y = value; }
        }

        public override INumber Add(RuntimeData runtime, INumber val) {
            throw new NotImplementedException();
        }
        public override INumber Multiple(RuntimeData runtime, INumber val) {
            throw new NotImplementedException();
        }

        public override bool CanJoin(RuntimeData runtime, INumber val) {
            return false;
        }
        public override INumber Differentiate(RuntimeData runtime, DifferentialData ddata) {
            throw new RuntimeException("ベクトルを微分することはできません。", this);
        }
        public override bool Equals(RuntimeData runtime, INumber val) {
            if (val is Vector) {
                Vector v = val as Vector;
                return
                    this.x.Equals(runtime, v.X) &&
                    this.y.Equals(runtime, v.Y);
            }
            else
                return false;
        }

        public INumber Abs(RuntimeData runtime) {
            var sqrt = runtime.GetFunc("sqrt");
            AdditionFormula af = new Expression.AdditionFormula();
            af.AddItem(runtime, this.x.Eval(runtime).Power(runtime, Number.New(2)));
            af.AddItem(runtime, this.y.Eval(runtime).Power(runtime, Number.New(2)));
            return sqrt.Execute(runtime, af.Optimise(runtime));
        }

    }
}
