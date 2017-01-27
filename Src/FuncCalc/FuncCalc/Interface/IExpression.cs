using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Interface {
    public interface IExpression {


        Token Token { get; }
        int SortPriority { get; }

    }
}
