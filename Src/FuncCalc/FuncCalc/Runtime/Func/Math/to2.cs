using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncCalc.Expression;
using FuncCalc.Exceptions;

namespace FuncCalc.Runtime.Func.Math
{
    public class to2 : IFunction
    {
        public override string Name
        {
            get
            {
                return "to2";
            }
        }
        public override string Description
        {
            get
            {
                return "2進数に変換します。";
            }
        }

        public override ExpressionType[] Parameter
        {
            get
            {
                return new[] { ExpressionType.Number };
            }
        }

        public override ExpressionType ReturnType
        {
            get
            {
                return ExpressionType.String;
            }
        }

        public override INumber Execute(RuntimeData runtime, params INumber[] parameters) {

            if (!(parameters[0] is Number))
                throw new RuntimeException("パラメータは値である必要があります。");


            byte[] buffer = (parameters[0] as Number).Value.ToByteArray();
            StringBuilder sb = new StringBuilder();
            for (int i = buffer.Length - 1; i >= 0; i--) {
                byte b = buffer[i];
                foreach (byte filter in new[] { 128, 64, 32, 16, 8, 4, 2, 1 })
                    if ((b & filter) != 0) sb.Append("1");
                    else sb.Append("0");
            }

            return new Expression.String(new Token(sb.ToString(), Analyzer.TokenType.String));
        }
    }
}
