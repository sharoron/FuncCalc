using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Runtime.Operator
{
    public class Lamda : IOperator
    {
        public string Name
        {
            get
            {
                return "ラムダ式";
            }
        }
        public string Text
        {
            get
            {
                return "->";
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
            var r = right.Eval(runtime);

            runtime.Setting.GetOperator("?=", null, true).Execute(runtime, left, r);
            return r;
        }
    }
}
