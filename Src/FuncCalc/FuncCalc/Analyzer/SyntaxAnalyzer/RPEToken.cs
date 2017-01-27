using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Analyzer
{
    internal class RPEToken : Token, IExpression
    {
        private IExpression exp = null;

        internal RPEToken(IExpression t) {
            if (t == null)
                throw new ArgumentNullException("t");
            this.exp = t;
        }

        public IExpression Expression
        {
            get { return this.exp; }
            set {
                if (value == null)
                    throw new ArgumentNullException("value");
                this.exp = value;
            }
        }
        public int SortPriority {
            get { return -1; }
        }

        public Token Token
        {
            get
            {
                return exp == null ? null : exp.Token;
            }
        }

        public override string ToString() {
            return string.Format("#{0}", 
                this.Token == null ? "null" : this.Token.Text);
        }
    }
}
