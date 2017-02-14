using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncCalc.Runtime;
using FuncCalc.Exceptions;

namespace FuncCalc.Expression
{
    public class Vector : INumber
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
                throw new NotImplementedException();
            }
        }
        public override int SortPriority
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public override ExpressionType Type
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public override ValueType ValueType
        {
            get
            {
                throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

    }
}
