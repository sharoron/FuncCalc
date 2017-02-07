using FuncCalc.Exceptions;
using FuncCalc.Expression;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace FuncCalc.Runtime.Func.Math
{
    public class SquareRootInt : IFunction, IOutput, IFunctionOutput
    {
        public override string Name
        {
            get
            {
                return "sqrtInt";
            }
        }
        public override string Description
        {
            get
            {
                return "平方根を求めます。結果は整数になりますが巨大な値でも求められます。Newton-Raphson法を使用しています。";
            }
        }
        public override ExpressionType[] Parameter
        {
            get { return new ExpressionType[] { ExpressionType.Number }; }
        }
        public override ExpressionType ReturnType
        {
            get
            {
                return ExpressionType.Number;
            }
        }
        public override INumber Execute(RuntimeData runtime, params INumber[] parameters) {
            var val = parameters[0].FinalEval(runtime);

            if (!(val is Number))
                throw new RuntimeException("パラメータは値でないといけません。", parameters[0]);

            // 答えが虚数になるもの
            if ((val as Number).Value.Sign <= 0)
                throw new NotImplementedException("この関数は正の整数でないと計算できません。");
            
            var res = this.SQRT((val as Number).Value);

            return Number.New(runtime, res);
        }

        /// <summary>
        /// Newton-Raphson法で平方根を求める
        /// </summary>
        /// <param name="vnum"></param>
        /// <returns></returns>
        public BigInteger SQRT(BigInteger vnum) {
            BigInteger a, b;
            a = 0;
            b = vnum;
            while (BigInteger.Abs(b - a) >= 1) {
                a = b;
                b = (a + vnum / a) / 2;
            }

            return b;
        }


        public string Output(OutputType type) {
            switch (type) {
                case OutputType.String:
                    return "√";
                case OutputType.Mathjax:
                    return "\\sqrt";
                default:
                    throw new NotFiniteNumberException();
            }
        }
        public string Output(OutputType type, INumber[] parameters, INumber pow) {
            switch (type) {
                case OutputType.String: {
                        string res = "√(" + parameters[0].Output(type) + ")";
                        if (!(pow is Number && (pow as Number).Value == 1)) {
                            res += "^" + pow.Output(type);
                        }
                        return res;
                    }
                case OutputType.Mathjax: {
                        string res = "\\sqrt {" + parameters[0].Output(type) + "}";
                        if (!(pow is Number && (pow as Number).Value == 1)) {
                            res += "^ {" + pow.Output(type) + "}";
                        }
                        return res;
                    }
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
