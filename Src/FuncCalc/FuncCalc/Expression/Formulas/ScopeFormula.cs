using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncCalc.Runtime;
using FuncCalc.Exceptions;

namespace FuncCalc.Expression
{
    public class ScopeFormula : IMember, IFormula
    {
        internal ScopeFormula() {
            this.Items = new List<Interface.IExpression>();
            
        }

        public int Count
        {
            get
            {
                return this.Items.Count;
            }
        }

        public override bool InfinitelyDifferentiable
        {
            get
            {
                return false;
            }
        }

        public List<IExpression> Items
        {
            get; private set;
        }

        public override string Name
        {
            get
            {
                return this.Items.Last().Token.Text;
            }
        }

        public override int SortPriority
        {
            get
            {
                return 500;
            }
        }

        public Token Token
        {
            get
            {
                return this.Items.First().Token;
            }
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

        public override INumber Add(RuntimeData runtime, INumber val) {
            return this.Eval(runtime).Add(runtime, val);
        }

        public void AddItem(RuntimeData runtime, IExpression exp) {
            this.Items.Add(exp);
        }

        public override bool CanJoin(RuntimeData runtime, INumber val) {
            return false;
        }
        public override INumber Get(RuntimeData runtime) {
            return this.Eval(runtime);
        }
        public override void Set(RuntimeData runtime, INumber value) {
            var dt = this.Eval(runtime, false);
            if (dt is IMember) {
                (dt as IMember).Set(runtime, value);
            }
            else
                throw new SyntaxException("このメンバーは代入することができません。", this);
        }

        public override INumber Differentiate(RuntimeData runtime, DifferentialData ddata) {
            return this.Eval(runtime).Differentiate(runtime, ddata);
        }

        public override bool Equals(RuntimeData runtime, INumber val) {
            return this.Eval(runtime).Equals(runtime, val);
        }

        public INumber Eval(RuntimeData runtime) {
            return this.Eval(runtime, true);
        }
        public INumber Eval(RuntimeData runtime, bool getData) {
            IOperator op = runtime.Setting.GetOperator(".", null, false);

            IExpression obj = null; bool flag = false;
            for (int i = 0; i < this.Items.Count; i++) {
                var item = this.Items[i];

                if (flag) {
                    obj = op.Execute(runtime, obj as INumber, item as INumber);
                    flag = false;
                    continue;
                }

                if (item is Operator && item.Token.Text != runtime.Setting.Spec.ScopeOperator) {
                    throw new RuntimeException("入るはずのない演算子が入っています。", item);
                }
                else {
                    if (obj == null)
                        obj = item;
                    else if (item is Operator && item.Token.Text == runtime.Setting.Spec.ScopeOperator) {
                        flag = true;
                    }
                    else {
                        throw new RuntimeException("ここに来るはずがない");
                    }
                }
            }

            if (getData) {
                var res = (obj as INumber).Eval(runtime);
                if (res == null)
                    return obj as INumber;
                else
                    return res;
            }
            else {
                return obj as INumber;
            }
        }
        public void ExecuteAsParameter(RuntimeData runtime) {
            runtime.NowBlock.Push(this.Eval(runtime));
        }

        public override INumber Multiple(RuntimeData runtime, INumber val) {
            return this.Eval(runtime).Multiple(runtime, val);
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this.Items.Count; i++) {
                sb.Append(this.Items[i].ToString());
            }
            return sb.ToString();
        }

    }
}
