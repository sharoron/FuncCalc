using FuncCalc.Expression;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Runtime.Operator
{
    public class Multiplication : IOperator
    {
        public string Name
        {
            get
            {
                return "乗算";
            }
        }
        public string Text
        {
            get
            {
                return "*";
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

            // 意味のない計算はここで弾く
            if (left.IsOne)
                return right;
            if (right.IsOne)
                return left;
            if ((left is Number && (left as Number).Value == 0) ||
                (right is Number && (right as Number).Value == 0))
                return Number.New(0);

            if (left is Number && right is Number) {
                return Number.New(runtime, (left as Number).Value * (right as Number).Value);
            }

            return left.Multiple(runtime, right);

        }
    }
}
