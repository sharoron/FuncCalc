using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Expression
{
    public class Operator : IExpression
    {
        private IOperator evaluator = null;

        private Operator() : this(null, null){ }
        public Operator(Token t, IOperator evaluator) {
            if (evaluator == null)
                throw new ArgumentException("evaluator");

            this.Token = t;
            this.evaluator = evaluator;
        }

        public Token Token
        {
            get; private set;
        }
        public IOperator Evaluator
        {
            get { return evaluator; }
        }
        public int SortPriority {
            get { return -1; }
        }

        public override string ToString() {
            return this.Token.Text;
        }
    }
}
