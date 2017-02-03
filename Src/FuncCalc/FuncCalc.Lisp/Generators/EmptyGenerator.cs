using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncCalc.Interface;

namespace FuncCalc.Lisp.Generators
{
    public class EmptyGenerator : IGenerator
    {
        public override IExpression Create() {
            throw new NotImplementedException();
        }
    }
}
