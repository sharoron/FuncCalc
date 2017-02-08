using FuncCalc.Expression;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Runtime.Operator
{
    public class And : IOperator
    {
        public string Name
        {
            get
            {
                return "論理積";
            }
        }
        public string Text
        {
            get
            {
                return "&";
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

            Number l = left as Number, r = right as Number;

            return Number.New(runtime, l.Value & r.Value);
        }
    }
}
