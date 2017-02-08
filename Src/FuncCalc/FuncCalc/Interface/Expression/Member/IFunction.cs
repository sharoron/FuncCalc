using FuncCalc.Expression;
using FuncCalc.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Interface
{
    public abstract class IFunction : IMember, IExpression, IEvalWithParameters {
        public override abstract string Name { get; }
        public virtual string Description { get { return null; } }
        public abstract ExpressionType[] Parameter { get; }
        public abstract ExpressionType ReturnType { get; }
        public virtual bool DoEvaledParam
        {
            get { return true; }
        }
        public virtual bool EnabledPow {
            get { return true; }
        }
        public int SortPriority {
            get { return -1; }
        }

        public virtual Token Token
        { get { return null; } }

        /// <summary>微分可能である</summary>
        public virtual bool CanDifferential { get { return false; } }
        /// <summary>積分可能である</summary>
        public virtual bool CanIntegral { get { return false; } }


        public abstract INumber Execute(RuntimeData runtime, params INumber[] parameters);
        public virtual INumber ForceExecute(RuntimeData runtime, params INumber[] parameters) {
            return this.Execute(runtime, parameters);
        }

        /* public virtual string Output(OutputType type, INumber[] parameters) {
            switch (type) {
                case OutputType.String: {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(this.Name);

                        sb.Append("(");
                        for (int i = 0; i < parameters.Length; i++) {
                            if (i != 0) sb.Append(", ");
                            sb.Append(parameters[i].Output(type));
                        }
                        sb.Append(")");
                        return sb.ToString();
                    }

                case OutputType.Mathjax: {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(this.Name);
                        sb.Append("(");
                        for (int i = 0; i < parameters.Length; i++) {
                            if (i != 0) sb.Append(", ");
                            sb.Append("{");
                            sb.Append(parameters[i].Output(type));
                            sb.Append("}");
                        }
                        sb.Append(")");
                        return sb.ToString();
                    }
            }

            throw new NotImplementedException();
        } // */

        public virtual string Output(OutputType type) {
            switch (type) {
                case OutputType.String:
                case OutputType.Mathjax:
                    return this.Name;
            }

            throw new NotImplementedException();
        }
    }
}
