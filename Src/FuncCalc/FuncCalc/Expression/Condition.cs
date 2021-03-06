﻿using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Runtime;

namespace FuncCalc.Expression
{
    public class Condition : INumber
    {
        public override ExpressionType Type
        {
            get
            {
                return ExpressionType.Unknown;
            }
        }

        public override ValueType ValueType
        {
            get
            {
                return ValueType.Unknown;
            }
        }
        public override int SortPriority {
            get { return 5000; }
        }

        public override INumber Add(RuntimeData runtime, INumber val) {
            throw new NotImplementedException();
        }

        public override bool CanJoin(RuntimeData runtime, INumber val) {
            throw new NotImplementedException();
        }

        public override bool Equals(RuntimeData runtime, INumber val) {
            throw new NotImplementedException();
        }

        public override INumber ExecuteDiff(RuntimeData runtime, string t) {
            throw new NotImplementedException();
        }

        public override INumber Multiple(RuntimeData runtime, INumber val) {
            throw new NotImplementedException();
        }
    }
}
