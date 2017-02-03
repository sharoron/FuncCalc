using FuncCalc.Expression;
using FuncCalc.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Interface
{
    /// <summary>
    /// 微分可能な値である
    /// </summary>
    public interface IDiff
    {
        bool InfinitelyDifferentiable { get; }

        INumber ExecuteDiff(RuntimeData runtime, string t);
        
    }
}
