using FuncCalc.Expression;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Runtime.Operator
{
    public class Not : IOperator
    {
        public string Name
        {
            get
            {
                return "否定";
            }
        }
        public string Text
        {
            get
            {
                return "|";
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

            IConstParameter num = (right as Number).FinalEval(runtime) as IConstParameter;

            if (num == null) return Number.New(0);
            if (num.ConstValue == 0) return Number.New(1);
            else return Number.New(0);
                        
        }
    }
}
