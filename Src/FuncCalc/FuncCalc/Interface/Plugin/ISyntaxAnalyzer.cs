using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuncCalc.Interface
{
    public interface ISyntaxAnalyzer
    {
        Token[] Items { get; set; }
        Runtime.RuntimeSetting Setting { get; set; }

        IExpression GetResult();
    }
}
