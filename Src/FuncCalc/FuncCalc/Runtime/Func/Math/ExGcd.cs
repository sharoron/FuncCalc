using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Expression;
using FuncCalc.Exceptions;
using System.Numerics;

namespace FuncCalc.Runtime.Func
{
    public class exGcd : IFunction
    {
        public override string Name
        {
            get
            {
                return "exgcd";
            }
        }
        public override string Description
        {
            get
            {
                return "拡張ユークリッドの互除法を使用して逆元(逆数)を求めます。";
            }
        }
        public override ExpressionType[] Parameter
        {
            get { return new ExpressionType[] { ExpressionType.Number, ExpressionType.Number }; }
        }
        public override ExpressionType ReturnType
        {
            get
            {
                return ExpressionType.Number;
            }
        }
        public override INumber Execute(RuntimeData runtime, params INumber[] parameters) {
            INumber left = parameters[0], right = parameters[1];

            // 無効パラメータはここで弾く
            if (left is Fraction || left is AdditionFormula || left is MultipleFormula ||
                left is InfinityValue || left is Variable || left is Member) {
                throw new RuntimeException(string.Format("exgcd関数では使用できないパラメータ '{0}'が含まれています。", left.GetType().Name), left);
            }
            
            if (right is Fraction || right is AdditionFormula || right is MultipleFormula ||
                right is InfinityValue || right is Variable || right is Member) {
                throw new RuntimeException(string.Format("exgcd関数では使用できないパラメータ '{0}'が含まれています。", right.GetType().Name), right);
            }

            if (left is Number && right is Number) {
                // ユークリッドの互除法を使用して求める
                BigInteger l = (left as Number).Value, r = (right as Number).Value;

                // 引用 : http://arc360.info/algo/privatekey.html
                BigInteger x1, x2, y1, y2, z1, z2, q, t;
                x1 = 1;
                y1 = 0;
                z1 = l;
                x2 = 0;
                y2 = 1;
                z2 = r;

                while (z2 != 1)
                {
                    if (z2 == 0) break;
                    q = (z1 - (z1 % z2)) / z2;
                    // Console.WriteLine("{0}a + {1}b = {2}", x1, y1, z1);
                    x1 = x1 - (q * x2);
                    y1 = y1 - (q * y2);
                    z1 = z1 - (q * z2);

                    t = x1;
                    x1 = x2;
                    x2 = t;

                    t = y1;
                    y1 = y2;
                    y2 = t;

                    t = z1;
                    z1 = z2;
                    z2 = t;
                }

                if (y2 < 0)
                {
                    Console.WriteLine("({0},{1})", x2 - r, y2 + l);
                    var res = new Expression.Array();
                    res.Items[0].Add(Number.New(runtime, x2 - r));
                    res.Items[0].Add(Number.New(runtime, y2 + l));
                    return res;
                }
                else
                {
                    Console.WriteLine("({0},{1})", x2, y2);
                    var res = new Expression.Array();
                    res.Items[0].Add(Number.New(runtime, x2));
                    res.Items[0].Add(Number.New(runtime, y2));
                    return res;
                }



            } else {
                throw new RuntimeException("exgcd関数はパラメータが整数であることが前提条件としたものなので、整数以外のパラメータが指定されている場合は計算を行うことができません。");
            }



        }
    }
}
