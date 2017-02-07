using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncCalc.Expression;
using FuncCalc.Runtime;

namespace FuncCalc.Cryptographic
{
    public class IsPrime : IFunction
    {
        public override string Name
        {
            get
            {
                return "isPrimeC";
            }
        }

        public override ExpressionType[] Parameter
        {
            get
            {
                return new[] { ExpressionType.Number };
            }
        }

        public override ExpressionType ReturnType
        {
            get
            {
                return ExpressionType.Anything;
            }
        }

        public override INumber Execute(RuntimeData runtime, params INumber[] parameters) {


            throw new NotImplementedException();

        }
    }
}
