using FuncCalc.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuncCalc.Interface
{
    public interface ILogger
    {
        OutputType OutputType { get; }

        void AddCondition(string str);
        void AddWay(string str);
    }
}
