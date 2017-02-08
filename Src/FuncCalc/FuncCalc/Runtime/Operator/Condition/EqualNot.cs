using FuncCalc.Expression;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Runtime.Operator
{
    public class EqualNot : IOperator
    {
        public string Name
        {
            get
            {
                return "等しくない";
            }
        }
        public string Text
        {
            get
            {
                return "!=";
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
            return !left.Equals(runtime, right) ? Number.New(1) : Number.New(0);
        }
    }
}
