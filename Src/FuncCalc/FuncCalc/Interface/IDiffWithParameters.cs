using FuncCalc.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Interface
{
    public interface IDiffWithParameters
    {
        INumber ExecuteDiff(RuntimeData runtime, string t, INumber[] parameters);
    }
}
