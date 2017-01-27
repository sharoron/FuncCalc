using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Expression;

namespace FuncCalc.Runtime.Func
{
    public class Test2 : IFunction
    {
        public override string Name
        {
            get
            {
                return "test2";
            }
        }
        public override string Description
        {
            get
            {
                return "5をかける関数です";
            }
        }
        public override ExpressionType[] Parameter
        {
            get { return new ExpressionType[] { ExpressionType.Number }; }
        }
        public override ExpressionType ReturnType
        {
            get
            {
                return ExpressionType.Number;
            }
        }
        public override INumber Execute(RuntimeData runtime, params INumber[] parameters) {
            return parameters[0].Multiple(runtime, Number.New(5));
        }
    }
}
