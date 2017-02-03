using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Exceptions {
    [Serializable]
    public class SyntaxException : Exception {

        private Token token = null;
        private IExpression exp = null;

        public SyntaxException(string message, Token token) : base(message) {
            this.token = token;
        }
        public SyntaxException(string message, IExpression exp) : base(message) {
            this.exp = exp;
        }
        public SyntaxException(string message, Token token, Exception inner) : base(message, inner) {
            this.token = token;
        }
        public SyntaxException(string message, IExpression exp, Exception inner) : base(message, inner) {
            this.exp = exp;
        }
        protected SyntaxException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        public Token Token {
            get {
                if (this.exp == null)
                    return this.token;
                else
                    return this.exp.Token;
            }
        }
        public IExpression Expression
        {
            get { return this.exp; }
        }

    }
}
