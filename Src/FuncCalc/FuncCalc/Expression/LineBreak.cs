using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Expression
{
    public class LineBreak : IExpression
    {
        private static readonly Token LToken = new Token();

        internal LineBreak() : this(LToken) { }
        public LineBreak(Token t) {
            this.Token = t;
        }

        public Token Token
        {
            get; private set;
        }
        public int SortPriority {
            get { return 10000; }
        }

        public override string ToString() {
            return this.Token == null ? ";" : this.Token.Text;
        }
    }
}
