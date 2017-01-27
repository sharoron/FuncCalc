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
    }
}
