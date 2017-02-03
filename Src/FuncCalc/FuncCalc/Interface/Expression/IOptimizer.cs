using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Runtime;

namespace FuncCalc.Interface {
    public interface IOptimizer {
        INumber Optimize(RuntimeData runtime, INumber val);

    }
}
