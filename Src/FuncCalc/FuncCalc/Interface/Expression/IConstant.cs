﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuncCalc.Interface
{
    public interface IConstant : IConstParameter
    {
        string Name { get; }
    }
}
