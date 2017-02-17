using FuncCalc.Exceptions;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace FuncCalc.Runtime.Support
{
    internal static class DotnetToFC
    {
        internal static INumber ConvertToFC(object obj) {

            if (obj is INumber)
                return obj as INumber;
            if (obj is FCNumber)
                return (obj as FCNumber).Parent;

            if (obj is byte || 
                obj is short || obj is ushort || 
                obj is int || obj is uint ||
                obj is long) {
                return Expression.Number.New(
                    long.Parse(obj.ToString()));
            }
            if (obj is ulong || 
                obj is float ||
                obj is double ||
                obj is decimal) {
                return new Expression.FloatNumber((decimal)obj);
            }
            if (obj is string)
                return new Expression.String(
                    new Token((string)obj, Analyzer.TokenType.String));
            if (obj is bool)
                return Expression.Number.New((bool)obj ? 1 : 0);
            if (obj is BigInteger)
                return Expression.Number.New((BigInteger)obj);
            if (obj == null)
                return null;

            throw new RuntimeException(
                string.Format("'{0}'をFuncCalc型に変換することができませんでした。", obj.GetType().FullName));

        }
    }
}
