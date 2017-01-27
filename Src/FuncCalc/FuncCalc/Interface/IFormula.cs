using FuncCalc.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Interface
{
    public interface IFormula : IExpression, IEval
    {
        void ExecuteAsParameter(RuntimeData runtime);
        IExpression[] Items { get; }
        int Count { get; }

        void AddItem(RuntimeData runtime, IExpression exp);

    }
}
