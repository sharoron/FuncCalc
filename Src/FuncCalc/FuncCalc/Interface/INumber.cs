using FuncCalc.Expression;
using FuncCalc.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Interface
{
    public abstract class INumber : IEval, IExpression, IOutput, IDiff
    {
        private INumber _pow = null;


        public virtual INumber Pow
        {
            get {
                if (this._pow == null)
                    return Number.New(1);
                else
                    return this._pow;
            }
            protected internal set { this._pow = value; }
        }
        public abstract ExpressionType Type { get; }
        public abstract Expression.ValueType ValueType { get; }
        public abstract int SortPriority { get; }

        public virtual Token Token
        {
            get; protected internal set;
        }
        
        public virtual INumber ExecuteOperator(RuntimeData runtime, IOperator op, INumber left, INumber right) {
            throw new NotImplementedException();
        }
        public abstract INumber Add(RuntimeData runtime, INumber val);
        public virtual INumber Subtract(RuntimeData runtime, INumber val) {
            var num = val.Clone();
            num = num.Multiple(runtime, new Number(-1));

            return this.Add(runtime, num);
        }
        public abstract INumber Multiple(RuntimeData runtime, INumber val);
        public virtual INumber Divide(RuntimeData runtime, INumber val) {
            return new Fraction(val, this);
        }
        public virtual INumber Power(RuntimeData runtime, INumber val) {
            var me = this.Clone();
            me.Pow = me.Pow.Multiple(runtime, val);
            return me;
        }
        public abstract bool Equals(RuntimeData runtime, INumber val);
        public virtual INumber Optimise(RuntimeData runtime) {
            if (this.Pow.Equals(runtime, Number.New(0))) {
                if (this is IMulti)
                    return (this as IMulti).Multi;
                return Number.New(1);
            }
            return this;
        }
        public abstract bool CanJoin(RuntimeData runtime, INumber val);

        public virtual INumber Clone() {
            return this.MemberwiseClone() as INumber;
        }
        public virtual INumber Eval(RuntimeData runtime) {
            return this;
        }
        public virtual INumber FinalEval(RuntimeData runtime) {
            return this.Eval(runtime);
        }

        public virtual string Output(OutputType type) {
            switch (type) {
                case OutputType.String:
                case OutputType.Mathjax:
                    return this.ToString();

                    string str = this.ToString();
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < str.Length; i++) {
                        sb.Append("\\" + str[i]);
                    }
                    return sb.ToString();
                default:
                    throw new NotImplementedException();
            }
        }

        public abstract INumber ExecuteDiff(RuntimeData runtime, string t);
    }
}
