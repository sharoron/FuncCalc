using FuncCalc.Expression;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Runtime.Operator
{
    public class MinusValue : IOperator
    {
        public string Name
        {
            get
            {
                return "マイナスの値";
            }
        }
        public string Text
        {
            get
            {
                return "-";
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
            get { return true; }
        }

        public INumber Execute(RuntimeData runtime, INumber left, INumber right) {
            return right.Multiple(runtime, Number.New(-1));
        }
    }
}
