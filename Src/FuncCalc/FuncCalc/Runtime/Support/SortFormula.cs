using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Interface;

namespace FuncCalc.Runtime.Support {
    internal static class SortFormula {

        internal static void Sort(List<IExpression> items) {

            items.Sort((i1, i2) => i1.SortPriority - i2.SortPriority);

        }
        internal static void Sort(List<INumber> items) {

            items.Sort((i1, i2) => i1.SortPriority - i2.SortPriority);

        }

    }
}
