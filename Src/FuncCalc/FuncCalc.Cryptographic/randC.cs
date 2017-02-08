using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncCalc.Expression;
using FuncCalc.Runtime;
using FuncCalc.Exceptions;
using System.Numerics;

namespace FuncCalc.Cryptographic
{
    public class RandC : IFunction
    {
        public override string Name
        {
            get
            {
                return "randC";
            }
        }
        public override string Description
        {
            get
            {
                return "指定されたビット長の乱数を生成します。";
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
                return ExpressionType.Number;
            }
        }

        public override INumber Execute(RuntimeData runtime, params INumber[] parameters) {

            // パラメータチェック一通り
            if (!(parameters[0] is Number) ||
                (parameters[0] as Number).Value.Sign <= 0)
                throw new RuntimeException("パラメータは正の整数である必要があります。", parameters[0]);
            if ((parameters[0] as Number).Value % 8 != 0)
                throw new RuntimeException("パラメータは8の倍数である必要があります。", parameters[0]);
            if ((parameters[0] as Number).Value > runtime.Setting.AcceptBitLength)
                throw new RuntimeException("指定されたビット数は許可されたビット数を上回っています。" +
                    runtime.Setting.AcceptBitLength + "ビット以下にしてください。", parameters[0]);

            // 乱数を生成する
            Number res = null;
            for (;;) {
                var length = (parameters[0] as Number).Value;
                Random r = new Random();

                // 乱数を生成する
                byte[] buffer = new byte[(int)(length / 8)];
                r.NextBytes(buffer);

                buffer[buffer.Length - 1] &= 64 + 32 + 16 + 8 + 4 + 2 + 1;

                // FuncCalcで取り扱える型にする
                BigInteger bi = new BigInteger(buffer);
                res = Number.New(runtime, bi);



                return res;

            }
        }
    }
}
