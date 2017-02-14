using FuncCalc.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuncCalc.Interface.Math
{
    public interface IAbs
    {
        INumber Abs(RuntimeData runtime);
    }
}
