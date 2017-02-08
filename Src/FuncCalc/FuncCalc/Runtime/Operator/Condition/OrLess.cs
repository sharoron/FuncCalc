using FuncCalc.Exceptions;
using FuncCalc.Expression;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Runtime.Operator
{
    public class OrLess : IOperator
    {
        public string Name
        {
            get
            {
                return "以下である";
            }
        }
        public string Text
        {
            get
            {
                return ">=";
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
            if (l == null || r == null)
                throw new RuntimeException("両辺は値である必要があります。", l == null ? left : right);

            if (l.Value >= r.Value)
                return Number.New(1);
            else
                return Number.New(0);
        }
    }
}
