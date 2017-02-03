using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Exceptions
{
    public class RuntimeException : Exception
    {

        private Token token = null;
        private IExpression exp = null;

        public RuntimeException() { }
        public RuntimeException(string message) : base(message) { }
        public RuntimeException(string message, Token token) : base(message) {
            this.token = token;
        }
        public RuntimeException(string message, IExpression exp) : base(message) {
            this.exp = exp;
        }
        public RuntimeException(string message, Token token, Exception inner) : base(message, inner) {
            this.token = token;
        }
        protected RuntimeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        public Token Token
        {
            get {
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
