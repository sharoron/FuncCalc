using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Analyzer {

    public enum TokenType : int {

        None = 0,

        Operation,
        Number,
        String,
        Member,
        Syntax,
        Unknown,
        Runtime
    }

}
