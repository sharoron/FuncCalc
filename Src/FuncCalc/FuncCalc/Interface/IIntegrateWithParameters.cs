﻿using FuncCalc.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Interface
{
    public interface IIntegrateWithParameters
    {
        INumber Integrate(RuntimeData runtime, string t, INumber[] parameters);
    }
}
