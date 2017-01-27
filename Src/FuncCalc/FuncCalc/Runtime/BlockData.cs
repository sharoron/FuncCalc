using FuncCalc.Expression;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Runtime
{
    public class BlockData
    {
        private Dictionary<string, INumber> variables = new Dictionary<string, INumber>();
        private Stack<IExpression> stack = new Stack<IExpression>();
        private List<Condition> conditions = new List<Condition>();
        private bool moreScope = false;

        public Dictionary<string, INumber> Variables
        {
            get { return this.variables; }
        }
        public List<Condition> Conditions
        {
            get { return this.conditions; }
        }
        public int Count
        {
            get { return this.stack.Count; }
        }
        public IExpression[] Stack
        {
            get { return this.stack.ToArray<IExpression>(); }
        }
        public bool MoreScope
        {
            get { return this.moreScope; }
            internal set { this.moreScope = value; }
        }

        internal IExpression Peek() {
            return this.stack.Peek();
        }
        internal IExpression Pop() {
            return this.stack.Pop();
        }
        internal void Push(IExpression exp) {
            if (exp == null && false)
                throw new ArgumentNullException("exp");

            this.stack.Push(exp);
        }

        public bool ContainsKey(string name) {
            return this.variables.ContainsKey(name);
        }
        public INumber GetData(string name) {
            return this.variables[name];
        }
    }
}
