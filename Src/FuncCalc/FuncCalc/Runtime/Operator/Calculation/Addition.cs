using FuncCalc.Expression;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Runtime.Operator
{
    public class Addition : IOperator
    {
        public string Name
        {
            get
            {
                return "加算";
            }
        }
        public string Text
        {
            get
            {
                return "+";
            }
        }
        public bool RequireLeftParameter
        {
            get
            {
                return true;
            }
        }
        public bool RequireRightParameter
        {
            get
            {
                return true;
            }
        }
        public bool DoEvaledParam
        {
            get { return true; }
        }

        public INumber Execute(RuntimeData runtime, INumber left, INumber right) {
            if (right is Expression.String && left is Expression.String) {
                return new Expression.String(new FuncCalc.Token(
                    left.Token.Text + right.Token.Text));
            }
            if (left is Expression.String) {
                return new Expression.String(new Token(left.Token.Text + right.ToString()));
            } else if (right is Expression.String) {
                return new Expression.String(new Token(left.ToString() + right.Token.Text));
            }

            return left.Add(runtime, right);
        }
    }
}
