using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuncCalc.Attributes
{
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class FuncCalcMethodAttribute : FuncCalcAttribute
    {
        public FuncCalcMethodAttribute() {
        }
    }
}
