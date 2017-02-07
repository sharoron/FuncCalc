using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Expression;
using FuncCalc.Exceptions;
using System.Collections;

namespace FuncCalc.Runtime.Func.Math
{
    public class IsPrime : IFunction
    {
        public override string Name
        {
            get
            {
                return "isPrime";
            }
        }
        public override string Description
        {
            get
            {
                return "素数か判定します。";
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
            var num = parameters[0].Eval(runtime);

            if (!(num is Number))
                throw new RuntimeException("isPrime関数は整数の値のみに対応しています。");

            // 参考: http://qiita.com/gushwell/items/5d3988a2f5de0587c88f

            if ((num as Number).Value > int.MaxValue - 1)
                throw new RuntimeException("大きすぎる値は指定できません。");

            return isPrime((int)(num as Number).Value) ?
                Number.New(1) : 
                Number.New(0);


        }

        private bool isPrime(int maxnum) {
            var sieve = new BitArray(maxnum + 1, true);
            int squareroot = (int)System.Math.Sqrt(maxnum);
            for (int i = 2; i <= squareroot; i++) {
                if (sieve[i] == false)
                    continue;
                for (int n = i * 2; n <= maxnum; n += i) {
                    if (n == maxnum) return false;
                    if (sieve.Length <= n) continue;
                    sieve[n] = false;
                }
            }
            return sieve[maxnum];
        }

    }
}
