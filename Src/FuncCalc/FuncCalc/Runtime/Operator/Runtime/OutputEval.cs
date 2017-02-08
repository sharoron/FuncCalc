using FuncCalc.Expression;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Runtime.Operator
{
    public class OutputEval : IOperator
    {
        public string Name
        {
            get
            {
                return "結果を評価して出力します";
            }
        }
        public string Text
        {
            get
            {
                return "??";
            }
        }
        public bool RequireLeftParameter
        {
            get
            {
                return false;
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
            var func = runtime.GetFunc(new Member(new Token("eval", Analyzer.TokenType.Member)), false, ExpressionType.Unknown);
            return func.Execute(runtime, right);
        }
    }
}
