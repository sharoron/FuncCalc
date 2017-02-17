﻿using FuncCalc.Attributes;
using FuncCalc.Expression;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuncCalc.Runtime.Objects
{
    [FuncCalc]
    public class Graph : DynamicObject
    {
        public Graph() {
            this.AcceptNotfoundMember = false;
        }

        [FuncCalc.Attributes.FuncCalc]
        public FCNumber test(FCNumber n1, FCNumber n2) {
            return n1 * n2;
        }
    }
}
