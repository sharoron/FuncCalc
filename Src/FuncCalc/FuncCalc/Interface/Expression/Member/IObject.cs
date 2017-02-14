using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncCalc.Expression;
using FuncCalc.Runtime;
using FuncCalc.Exceptions;

namespace FuncCalc.Interface
{
    public abstract class IObject : INumber
    {
        public override INumber Power(RuntimeData runtime, INumber val) {
            throw new RuntimeException("IObject型に対して四則計算を行うことはできません。", this);
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
                return -1;
            }
        }
        public override INumber Add(RuntimeData runtime, INumber val) {
            throw new RuntimeException("IObject型に対して四則計算を行うことはできません。", this);
        }
        public override INumber Multiple(RuntimeData runtime, INumber val) {
            throw new RuntimeException("IObject型に対して四則計算を行うことはできません。", this);
        }
        public override INumber Differentiate(RuntimeData runtime, DifferentialData ddata) {
            throw new RuntimeException("IObject型を微分することはできません。", this);
        }
        public override INumber Integrate(RuntimeData runtime, string t) {
            throw new RuntimeException("IObject型を積分することはできません。", this);
        }
        public override bool CanJoin(RuntimeData runtime, INumber val) {
            return false;
        }
        public override Expression.ValueType ValueType
        {
            get
            {
                return Expression.ValueType.Unknown;
            }
        }
        public override ExpressionType Type
        {
            get
            {
                return ExpressionType.Object;
            }
        }

        public abstract List<IMember> Members { get; }
        public abstract INumber Get(RuntimeData runtime, string key);
        public abstract void Set(RuntimeData runtime, string key, INumber value);
        public abstract INumber Execute(RuntimeData runtime, string key, IObject obj, params INumber[] parameters);
        public virtual IMember GetMember(string name) {
            var mem = this.Members.Where(i => i.Name == name);
            if (mem.Count() == 0)
                throw new RuntimeException(string.Format("'{0}'は見つかりませんでした。"), this);
            else {
                return mem.First();
            }
        }
    }
}
