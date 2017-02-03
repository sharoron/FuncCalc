using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Exceptions
{
    public class FuncCalcException : Exception
    {

        private Token token = null;
        private IExpression exp = null;

        public FuncCalcException() { }
        public FuncCalcException(string message) : base(message) { }
        public FuncCalcException(string message, Token token) : base(message) {
            this.token = token;
        }
        public FuncCalcException(string message, IExpression exp) : base(message) {
            this.exp = exp;
        }
        public FuncCalcException(string message, Token token, Exception inner) : base(message, inner) {
            this.token = token;
        }
        protected FuncCalcException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        public Token Token
        {
            get
            {
                if (exp != null)
                    return exp.Token;
                else
                    return this.token;
            }
        }
        public IExpression Expression
        {
            get { return this.exp; }
        }
    }
}
