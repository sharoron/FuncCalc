using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncCalc.Runtime;
using FuncCalc.Exceptions;
using FuncCalc.Expression;

namespace FuncCalc.Expression
{
    public class String : INumber, IExpression, IEval, IEvalWithParameters
    {
        private String() : this(null) { }
        public String(Token t) {
            this.Token = t;
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
                return 9500;
            }
        }      
        public Token Token
        {
            get; internal set;
        }
        public override ExpressionType Type
        {
            get
            {
                return ExpressionType.String;
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
            throw new NotImplementedException();
        }
        public override INumber Multiple(RuntimeData runtime, INumber val) {
            throw new RuntimeException("文字列を微分することはできません。", this);
        }
        public override bool CanJoin(RuntimeData runtime, INumber val) {
            throw new NotImplementedException();
        }
        public override bool Equals(RuntimeData runtime, INumber val) {
            throw new NotImplementedException();
        }
        public INumber Eval(RuntimeData runtime) {
            return this;
        }
        public INumber Execute(RuntimeData runtime, params INumber[] parameters) {
            List<ExpressionType> types = new List<Expression.ExpressionType>();
            for (int i = 0; i < parameters.Length; i++) {
                types.Add(parameters[i].Type);
            }

            var func = runtime.GetFunc(new Member(this.Token), true, types.ToArray());
            return func.Execute(runtime, parameters);
        }
        public override INumber Differentiate(RuntimeData runtime, DifferentialData ddata) {
            throw new RuntimeException("文字列を微分することはできません。", this);
        }

        public override string ToString() {
            return this.Token.Text;
        }
        public override string Output(OutputType type) {
            switch (type) {
                case OutputType.String:
                    return this.ToString();
                case OutputType.Mathjax:
                    return this.ToString();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
