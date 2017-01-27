using FuncCalc.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Interface
{
    public interface IFunctionOutput : IOutput
    {
        string Output(OutputType type, INumber[] parameters, INumber pow);
    }
}
