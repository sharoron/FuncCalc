using FuncCalc.Exceptions;
using FuncCalc.Expression;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Runtime.Operator
{
    public class Substitution : IOperator
    {
        public string Name
        {
            get
            {
                return "代入";
            }
        }
        public string Text
        {
            get
            {
                return "=";
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
            get { return false; }
        }

        public INumber Execute(RuntimeData runtime, INumber left, INumber right) {
            if (!(left is Variable) && !(left is Member) &&
                !(left is FunctionFormula)) {
                throw new SyntaxException("代入は変数にのみ行えます。", left);
            }
            
            Variable v = null;
            if (left is Variable)
                v = left as Variable;
            else if (left is Member)
                v = new Variable((left as Member).Token);
            else if (left is FunctionFormula && (left as FunctionFormula).Count == 1)
                v = new Variable(((left as FunctionFormula).Items[0] as Member).Token);
            else throw new NotImplementedException();

            var res = right.Eval(runtime);
            runtime.SetVariable(runtime, v, res);
            return res;
        }
    }
}
