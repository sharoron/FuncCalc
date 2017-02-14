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
        bool InfinitelyDifferentiable { get; }
        INumber Differentiate(RuntimeData runtime, DifferentialData ddata, INumber[] parameters);
    }
}
