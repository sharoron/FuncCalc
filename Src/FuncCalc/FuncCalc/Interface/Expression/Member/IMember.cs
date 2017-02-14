using FuncCalc.Exceptions;
using FuncCalc.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncCalc.Expression;

namespace FuncCalc.Interface
{
    public abstract class IMember : INumber
    {
        public override INumber Pow
        {
            get
            {
                return Number.New(1);
            }
            protected internal set
            {
                throw new RuntimeException("メンバーにpowを設定することはできません。");
            }
        }

        public abstract string Name { get; }
        public override ExpressionType Type
        {
            get
            {
                return ExpressionType.Pointer;
            }
        }
        public override Expression.ValueType ValueType
        {
            get
            {
                return Expression.ValueType.Unknown;
            }
        }
        public override int SortPriority
        {
            get
            {
                return -1;
            }
        }
        public override bool InfinitelyDifferentiable
        {
            get
            {
                return true;
            }
        }
        public override INumber Add(RuntimeData runtime, INumber val) {
            throw new RuntimeException("メンバーに四則計算することはできません。");
        }
        public override INumber Multiple(RuntimeData runtime, INumber val) {
            throw new RuntimeException("メンバーに四則計算することはできません。");
        }
        public override bool Equals(RuntimeData runtime, INumber val) {
            return this == val;
        }
        public override INumber Differentiate(RuntimeData runtime, DifferentialData ddata) {
            return this.Get(runtime).Differentiate(runtime, ddata);
        }
        public override INumber Integrate(RuntimeData runtime, string t) {
            return this.Get(runtime).Integrate(runtime, t);
        }
        public override bool CanJoin(RuntimeData runtime, INumber val) {
            return false;
        }

        public virtual INumber Get(RuntimeData runtime) {
            throw new RuntimeException(string.Format("'{0}'はゲッターに対応していません。", this.Name));
        }
        public virtual void Set(RuntimeData runtime, INumber value) {
            throw new RuntimeException(string.Format("'{0}'はセッターに対応していません。", this.Name));
        }
        public virtual INumber Execute(RuntimeData runtime, params INumber[] parameters) {
            throw new RuntimeException(string.Format("'{0}'は実行できません。", this.Name));
        }
    }
}
