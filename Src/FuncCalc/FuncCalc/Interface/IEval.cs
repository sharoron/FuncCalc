using FuncCalc.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Interface
{
    public interface IEval : IExpression
    {
        Token Token { get; }

        INumber Eval(RuntimeData runtime);
    }
}
