using FuncCalc.Expression;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Runtime.Operator
{
    public class Diff : IOperator
    {
        public string Name
        {
            get
            {
                return "微分";
            }
        }
        public string Text
        {
            get
            {
                return "'";
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
                return false;
            }
        }
        public bool DoEvaledParam
        {
            get { return false; }
        }

        public INumber Execute(RuntimeData runtime, INumber left, INumber right) {
            return runtime.GetFunc(new Member(new Token("diff", Analyzer.TokenType.Member)), false).Execute(runtime, new Variable(new Token("x", Analyzer.TokenType.Member)), left);
        }

    }
}
