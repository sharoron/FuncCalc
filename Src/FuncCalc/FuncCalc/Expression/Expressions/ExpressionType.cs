using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Expression
{
    public enum ExpressionType
    {
        Anything, 
        Nothing,

        Number, 
        String,
        Formula,
        Pointer,
        Object,
        Unknown = Anything,
    }
}
