using FuncCalc.Expression;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Runtime.Operator
{
    public class BitNot : IOperator
    {
        public string Name
        {
            get
            {
                return "ビットごとの否定";
            }
        }
        public string Text
        {
            get
            {
                return "~";
            }
        }
        public bool RequireLeftParameter
        {
            get
            {
                return false;
            }
        }
        public bool RequireRightParameter
        {
            get
            {
                return true;
            }
        }
        public bool DoEvaledParam
        {
            get { return true; }
        }

        public INumber Execute(RuntimeData runtime, INumber left, INumber right) {
            Number r = right as Number;

            byte[] buffer = r.Value.ToByteArray();
            for (int i = 0; i < buffer.Length; i++) {
                buffer[i] ^= 255;
            }

            return Number.New(runtime, new BigInteger(buffer));
        }
    }
}
