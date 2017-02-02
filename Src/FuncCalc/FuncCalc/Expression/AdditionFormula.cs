using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Runtime;

namespace FuncCalc.Expression
{
    /// <summary>
    /// 加算式
    /// </summary>
    public class AdditionFormula : INumber, IUnknownParameter, IFormula
    {
        private List<INumber> items = new List<INumber>();

        internal AdditionFormula() { }

        /// <summary>
        /// 足し算でつなげるアイテム
        /// </summary>
        public INumber[] Items
        {
            get { return this.items.ToArray(); }
        }
        public int Count
        {
            get { return this.items.Count; }
        }
        public override ExpressionType Type
        {
            get
            {
                return ExpressionType.Formula;
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
            get { return 1000; }
        }
        public override bool InfinitelyDifferentiable
        {
            get
            {
                for (int i = 0; i < this.items.Count; i++) {
                    if (this.items[i].InfinitelyDifferentiable) {
                        return true;
                    }
                }
                return false;
            }
        }

        IExpression[] IFormula.Items
        {
            get
            {
                return Items.ToArray<IExpression>();
            }
        }

        public override INumber Clone() {
            var res = this.MemberwiseClone() as AdditionFormula;
            res.items = new List<Interface.INumber>();
            res.items.AddRange(this.items);
            return res;
        }
        public void AddItem(RuntimeData runtime, INumber val) {
            var v = val.Eval(runtime);
            if (v is Number && (val as Number).Value == 0) { }
            else if (v is AdditionFormula && 
                (v.Pow is Number && (v.Pow as Number).Value == 1)) {
                foreach (var item in (val as AdditionFormula).items) {
                    this.items.Add(item);
                }
            }
            else
                this.items.Add(v);
        }
        public void AddItem(INumber val) {
            if (val is Number && (val as Number).Value == 0)
                return;

            this.items.Add(val);
        }

        public override INumber Add(RuntimeData runtime, INumber val) {

            if (val is AdditionFormula) {
                var res = this.Clone() as AdditionFormula;
                res.AddItem(runtime, val);
                return res;
            }

            // くっつけれるものがあればくっつけて終了
            AdditionFormula af = new AdditionFormula();
            bool flag = false;
            for (int i = 0; i < this.items.Count; i++) {
                if (this.items[i].CanJoin(runtime, val) && !flag) {
                    af.AddItem(runtime, this.items[i].Add(runtime, val));
                    flag = true;
                }else {
                    af.AddItem(runtime, this.items[i]);
                }
            }

            // くっつけれなかったらアイテムにぶっ込んで終了
            if (!flag)
                af.AddItem(runtime, val);

            return af;
        }
        public override INumber Multiple(RuntimeData runtime, INumber val) {

            if (val is Number && (val as Number).Value == 1)
                return this.Clone();

            if (val is Number || val is Fraction) {
                if (this.Pow is Number && (this.Pow as Number).Value == 1) {
                    AdditionFormula aa = new AdditionFormula();
                    foreach (var item in this.items) {
                        aa.AddItem(runtime, item.Multiple(runtime, val));
                    }
                    return aa;
                }
            }
            else {
                MultipleFormula mf = new MultipleFormula();
                mf.AddItem(runtime, this);
                mf.AddItem(runtime, val);
                return mf;
            }

            throw new NotImplementedException();
        }
        public override bool CanJoin(RuntimeData runtime, INumber val) {
            // ぶっちゃけなんでもいける
            // けどできないことにする
            return false;
        }
        public override bool Equals(RuntimeData runtime, INumber val) {
            return false;
        }
        public override INumber Eval(RuntimeData runtime) {
            AdditionFormula af = new Expression.AdditionFormula();
            for (int i = 0; i < this.items.Count; i++) {
                var res = this.items[i].Eval(runtime);
                bool flag = false;
                for (int j = 0; j < af.Count; j++) {
                    if (res.CanJoin(runtime, af.items[j])) {
                        var itm = af.items[j];
                        af.items.RemoveAt(j);
                        af.AddItem(runtime, itm.Add(runtime, res));
                        flag = true; break;
                    }
                }
                if (!flag) af.AddItem(runtime, res);
            }

            INumber result = null;

            if (af.items.Count == 1)
                result = af.items[0];
            else
                result = af;

            result = result.Power(runtime, this.Pow);
            return result;
        }
        public override INumber FinalEval(RuntimeData runtime) {
            INumber res = Number.New(0);
            for (int i = 0; i < this.items.Count; i++) {
                res = res.Add(runtime, this.items[i].FinalEval(runtime));
            }
            return res;
        }
        public override INumber Optimise(RuntimeData runtime) {
            AdditionFormula af = new Expression.AdditionFormula();
            for (int i = 0; i < this.Count; i++) {
                var val = this.items[i].Optimise(runtime);
                af.AddItem(runtime, val);
            }
            return af;
        }

        public override string ToString() {

            // 文字列化する前にアイテムをソートしておく
            FuncCalc.Runtime.Support.SortFormula.Sort(this.items);

            StringBuilder sb = new StringBuilder("(");
            for (int i = 0; i < this.items.Count; i++) {
                if (i != 0) sb.Append(" + ");
                sb.Append(this.items[i].ToString());
            }
            sb.Append(")");
            if (!(this.Pow is Number) || (this.Pow as Number).Value != 1)
                sb.Append(string.Format("^{0}", this.Pow));

            return sb.ToString();
        }

        public void ExecuteAsParameter(RuntimeData runtime) {
            runtime.NowBlock.Push(this);
        }

        public override INumber ExecuteDiff(RuntimeData runtime, string t) {
            AdditionFormula af = new AdditionFormula();
            for (int i = 0; i < this.items.Count; i++) {
                var res = this.items[i].Eval(runtime).ExecuteDiff(runtime, t);
                af.AddItem(runtime, res);
            }

            var pow = Runtime.Func.Differential.DiffPow(runtime, t, this);
            if (pow != null) {
                pow.AddItem(runtime, af);
                return pow;
            }
            else
                return af;
        }
        public override INumber Integrate(RuntimeData runtime, string t) {
            AdditionFormula af = new Expression.AdditionFormula();

            for (int i = 0; i < this.items.Count; i++) {
                af.AddItem(runtime, this.items[i].Integrate(runtime, t));
            }

            return af.Optimise(runtime);
        }

        public void AddItem(RuntimeData runtime, IExpression exp) {
            this.AddItem(runtime, exp as INumber);
        }

        public override string Output(OutputType type) {

            // 文字列化する前にアイテムをソートしておく
            FuncCalc.Runtime.Support.SortFormula.Sort(this.items);

            switch (type) {
                case OutputType.Mathjax: {
                        StringBuilder sb = new StringBuilder();
                        for(int i = 0; i < this.Count; i++) {
                            if (i != 0) sb.Append("+");
                            sb.Append("{");
                            sb.Append(this.items[i].Output(type));
                            sb.Append("}");
                        }
                        return sb.ToString();
                    }
                    break;
            }
            return base.Output(type);
        }
    }
}
