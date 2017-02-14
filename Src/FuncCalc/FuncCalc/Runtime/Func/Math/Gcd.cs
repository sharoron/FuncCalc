using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Expression;
using FuncCalc.Exceptions;

namespace FuncCalc.Runtime.Func
{
    public class Gcd : IFunction
    {
        public override string Name
        {
            get
            {
                return "gcd";
            }
        }
        public override string Description
        {
            get
            {
                return "最大公約数を求めます。";
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
            INumber left = parameters[0].Eval(runtime), right = parameters[1].Eval(runtime);

            // 無効パラメータはここで弾く
            if (left is Fraction || left is AdditionFormula || left is MultipleFormula ||
                left is InfinityValue || left is Variable || left is Member) {
                throw new RuntimeException(string.Format("gcd関数では使用できないパラメータ '{0}'が含まれています。", left.GetType().Name), left);
            }
            
            if (right is Fraction || right is AdditionFormula || right is MultipleFormula ||
                right is InfinityValue || right is Variable || right is Member) {
                throw new RuntimeException(string.Format("gcd関数では使用できないパラメータ '{0}'が含まれています。", right.GetType().Name), right);
            }

            if (left is Number && right is Number) {
                // ユークリッドの互除法を使用して求める
                Number l = (left.Clone()) as Number, r = (right.Clone()) as Number;
                // 並び替える
                if (l.Value > r.Value) {
                    Number d = l; l = r; r = d;
                }

                // 計算する
                Number res = r.Clone() as Number;
                for (;;) {
                    res.Value = r.Value % l.Value;
                    if (res.Value == 0) {
                        res = l;
                        break;
                    }
                    else {
                        r.Value = l.Value;
                        l.Value = res.Value;
                    }
                }
                return res;

            } else {
                throw new NotImplementedException();
            }



        }
    }
}
