using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncCalc.Expression;
using FuncCalc.Runtime;
using System.Numerics;
using FuncCalc.Exceptions;

namespace FuncCalc.Cryptographic
{
    public class IsPrime : IFunction
    {
        public override string Name
        {
            get
            {
                return "isPrimeC";
            }
        }
        public override string Description
        {
            get
            {
                return "巨大な値に対応した素数判定機です。99.999999999999911%の確率で正しい結果を返します。";
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
                return ExpressionType.Anything;
            }
        }

        public override INumber Execute(RuntimeData runtime, params INumber[] parameters) {

            if (!(parameters[0] is Number))
                throw new RuntimeException("isPrimeC関数のパラメータは整数でないといけません。", parameters[0]);

            return this.ScreenCheck(runtime,
                (parameters[0] as Number).Value) ?
                Number.New(1) :
                Number.New(0);


        }

        private readonly int[] _prime100 = new[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67,
                          71, 73, 79, 83, 89, 97 };     // 1～100の間の素数(25個)


        /// <summary> [ScreenCheck]メソッド
        /// </summary>
        /// <param name="vn"></param>
        /// <returns>
        ///  true：合成数とはいえない、false：合成数
        /// </returns>
        private bool ScreenCheck(RuntimeData runtime, BigInteger vn) {

            if (vn.IsEven)
                return false;

            for (int i = 0; i < _prime100.Length; i++) {
                if (!NotComposite(runtime, _prime100[i], vn)) {
                    return false;
                }
            }
            return true;
        }

        /// <summary> [NotComposite]メソッド
        /// </summary>
        /// <param name="vbase">底(int)</param>
        /// <param name="vtarget">対象数</param>
        /// <returns>true：合成数とはいえない、false：合成数</returns>
        private bool NotComposite(RuntimeData runtime, int vbase, BigInteger vtarget) {
            if (Gcd(runtime, vtarget, vbase) != 1) return false;

            BigInteger u = vtarget - 1;
            int k = 0;
            while (BigInteger.Remainder(u, 2) == 0) {
                k += 1;
                u = BigInteger.Divide(u, 2);
            }

            BigInteger x0, x1;
            x0 = BigInteger.ModPow(vbase, u, vtarget);
            if (x0 == 1) return true;
            for (int i = 1; i <= k; i++) {
                if (x0 == vtarget - 1) return true;
                x1 = BigInteger.ModPow(x0, 2, vtarget);
                x0 = x1;
            }
            return false;
        }

        private BigInteger Gcd(RuntimeData runtime, BigInteger v1, BigInteger v2) {
            return (
                runtime.Functions.Where(k => k.Key == "gcd")
                .First()
                .Value.Execute(runtime,
                Number.New(runtime, v1),
                Number.New(runtime, v2)) as Number).Value;
        }

    }
}
