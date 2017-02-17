using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuncCalc.Attributes
{
    [System.AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class FuncCalcAttribute : Attribute
    {
        public FuncCalcAttribute() {
            
        }

    }
}
