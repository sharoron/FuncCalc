using FuncCalc.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Interface
{
    public interface IOperator
    {
        string Name { get; }
        string Text { get; }
        bool RequireLeftParameter { get; }
        bool RequireRightParameter { get; }
        bool DoEvaledParam { get; }
        

        INumber Execute(RuntimeData runtime, INumber left, INumber right);
    }
}
