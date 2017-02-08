using FuncCalc.Exceptions;
using FuncCalc.Interface;
using FuncCalc.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Expression
{
    public class Member : INumber, IExpression, IEval
    {
        private INumber multi = Number.New(1);

        public string Text
        {
            get { return this.Token.Text; }
        }
        public override ExpressionType Type
        {
            get
            {
                return ExpressionType.Pointer;
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
            get { return 500; }
        }
        public override bool InfinitelyDifferentiable
        {
            get
            {
                return false;
            }
        }

        private Member() { }
        public Member(Token t) {
            this.Token = t;
        }

        public INumber Multi
        {
            get { return this.multi; }
        }

        public override INumber ExecuteOperator(RuntimeData runtime, IOperator op, INumber left, INumber right) {

            throw new NotImplementedException();

        }
        public override INumber Eval(RuntimeData runtime) {
            if (runtime.ContainsKey(this.Text)) {
                var res = runtime.GetData(this.Token).Eval(runtime).Clone();
                if (!(this.Pow is Number && (this.Pow as Number).Value == 1))
                    res = res.Power(runtime, this.Pow);
                return res;
            }

            // 変数として処理する
            // その際に変数を評価して渡す
            return GetVariable(runtime).Eval(runtime);

            throw new RuntimeException(string.Format("'{0}'は見つかりませんでした。", this.Text), this.Token);
        }
        public Variable GetVariable(RuntimeData runtime) {
            return (new Variable(this.Token) {
                Pow = this.Pow
            });
        }

        public override INumber Add(RuntimeData runtime, INumber val) {
            return this.Eval(runtime).Add(runtime, val);
        }
        public override INumber Multiple(RuntimeData runtime, INumber val) {
            var me = this.Eval(runtime);
            me = me.Multiple(runtime, val);
            return me;
        }
        public override INumber Divide(RuntimeData runtime, INumber val) {
            return this.multi = this.multi.Divide(runtime, val);
        }
        public override bool Equals(RuntimeData runtime, INumber val) {
            return this.Eval(runtime).Equals(val);
        }
        public override bool CanJoin(RuntimeData runtime, INumber val) {
            return false;
        }
        public override INumber FinalEval(RuntimeData runtime) {
            var res = runtime.GetData(this.Token).FinalEval(runtime);
            res = res.Power(runtime, this.Pow);
            return res;
        }

        public override string ToString() {
            return string.Format("{0}{1}", 
                this.Text,
                this.Pow is Number && (this.Pow as Number).Value == 1 ? "" : "^" + this.Pow.ToString());
        }

        public override INumber ExecuteDiff(RuntimeData runtime, string t) {
            return new Variable(this.Token).ExecuteDiff(runtime, t);
        }
        public override INumber Integrate(RuntimeData runtime, string t) {
            
            // このメンバーの名前がtと同じだった場合
            if (this.Text == t) {

                if (this.Pow is IConstParameter) {
                    // x^a
                    
                    // 1/(a + 1)
                    MultipleFormula mf = new Expression.MultipleFormula();
                    mf.AddItem(runtime, new Fraction(this.Pow.Add(runtime, Number.New(1)), Number.New(1)));

                    // x~(a+1)
                    var cln = this.Clone(); cln.Pow = cln.Pow.Add(runtime, Number.New(1));
                    mf.AddItem(runtime, cln);

                    return mf;

                } else {
                    throw new NotImplementedException("べき乗が定数ではない変数の積分は未実装です。");
                }

            } else { // このメンバーの名前がtとは違う場合

                var res = runtime.GetData(this.Token);
                if (res is Member) throw new NotImplementedException("話が違う");

                if (res is Variable) {
                    if ((res as Variable).Name == this.Text) {
                        // yをxで微分しようとしている場合

                        // tとres(それぞれ違う名前の変数)がそれぞれ関係ないならば成り立つ
                        runtime.AddLogCondition("UnrelationWith", new Variable(t), res);

                        // resを定数として扱って積分する
                        // 参考: (2)~(3)の部分 http://www.geisya.or.jp/~mwm48961/electro/multi_integral3.htm
                        MultipleFormula mf = new Expression.MultipleFormula();
                        mf.AddItem(runtime, res);
                        mf.AddItem(runtime, new Variable(t));
                        return mf;
                    }
                    else {
                        // yからまた別の変数が出てきちゃった場合
                        return res.Integrate(runtime, t);
                    }
                }
                else
                    return res.Integrate(runtime, t);
            }


        }

        public override string Output(OutputType type) {
            switch (type) {
                case OutputType.String:
                    return this.ToString();
                case OutputType.Mathjax:
                    if (this.Pow is Number && (this.Pow as Number).Value == 1)
                        return this.Text;
                    else
                        return string.Format("{0}^{{1}}",
                            this.Text, this.Pow.Output(type));
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
