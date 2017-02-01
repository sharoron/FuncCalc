using FuncCalc.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuncCalc.Interface
{
    public interface IIntegratable
    {
        INumber Integrate(RuntimeData runtime, string t);
    }
}
