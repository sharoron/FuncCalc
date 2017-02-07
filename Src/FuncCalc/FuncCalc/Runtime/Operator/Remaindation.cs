using FuncCalc.Expression;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Runtime.Operator
{
    public class Remaindation : IOperator
    {
        public string Name
        {
            get
            {
                return "mod";
            }
        }
        public string Text
        {
            get
            {
                return "%";
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
            
            if (left is Number && right is Number) {
                return Number.New(runtime, (left as Number).Value % (right as Number).Value);
            }

            throw new NotImplementedException();

        }
    }
}
