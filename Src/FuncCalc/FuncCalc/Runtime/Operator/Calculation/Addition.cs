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

            if (left is Expression.String || right is Expression.String) {
                AdditionFormula af = new AdditionFormula() { DontSort = true };
                af.AddItem(runtime, left);
                af.AddItem(runtime, right);
                return af;
            }

            return left.Add(runtime, right);
        }
    }
}
