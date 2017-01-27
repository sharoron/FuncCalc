using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Expression;

namespace FuncCalc.Runtime.Func
{
    public class Test : IFunction
    {
        public override string Name
        {
            get
            {
                return "test";
            }
        }
        public override string Description
        {
            get
            {
                return "123を返す関数です";
            }
        }
        public override ExpressionType[] Parameter
        {
            get { return new ExpressionType[] { }; }
        }
        public override ExpressionType ReturnType
        {
            get
            {
                return ExpressionType.Number;
            }
        }
        public override INumber Execute(RuntimeData runtime, params INumber[] parameters) {
            return Number.New(123);
        }
    }
}
