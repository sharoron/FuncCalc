using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Runtime;
using FuncCalc.Exceptions;

namespace FuncCalc.Expression
{
    /// <sumarry>乗算式</sumarry>
    public class MultipleFormula : INumber, IFormula
    {
        private List<INumber> items = new List<INumber>();

        public override ExpressionType Type
        {
            get
            {
                return ExpressionType.Unknown;
            }
        }
        public List<INumber> Items
        {
            get { return this.items; }
        }
        public override ValueType ValueType
        {
            get
            {
                ValueType t = ValueType.Plus;
                foreach (var item in this.items) {
                    if (item.ValueType == ValueType.Unknown)
                        return ValueType.Unknown;
                    if (item.ValueType == ValueType.Minus)
                        t = t == ValueType.Plus ? ValueType.Minus : ValueType.Plus;
                }
                return t;
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
                    if (this.items[i].InfinitelyDifferentiable)
                        return true;
                }
                return false;
            }
        }
        public bool DontSort
        {
            get; set;
        }

        List<IExpression> IFormula.Items
        {
            get
            {
                return this.items.ToList<IExpression>();
            }
        }

        public int Count
        {
            get
            {
                return this.items.Count;
            }
        }
        public override bool ContainsImaginalyNumber
        {
            get
            {
                for (int i = 0; i < this.items.Count; i++) {
                    if (this.items[i] is ImaginaryNumber)
                        return true;
                }
                return false;
            }
        }

        public void AddItem(RuntimeData runtime, INumber val) {
            var v = val.Eval(runtime);

            if (v is Number && (v as Number).Value == 1)
                return;

            if (val is AdditionFormula && (val as AdditionFormula).Count == 1) {
                this.AddItem(runtime, (val as AdditionFormula).Items[0]);
                return;
            } 
            if (val is MultipleFormula) {
                foreach (var item in (val as MultipleFormula).items) {
                    this.AddItem(runtime, item);
                }
                return;
            }
            
            for (int i = 0; i < this.items.Count; i++) {
                if (runtime.Setting.DoOptimize && (
                    this.items[i].CanJoin(runtime, val) ||
                    val.CanJoin(runtime, this.items[i]))) {
                    this.items[i] =
                        this.items[i].Multiple(runtime, val);
                    return;
                }
            }

            this.items.Add(v);
        }
        public void AddItem(RuntimeData runtime, IExpression exp) {
            this.AddItem(runtime, exp as INumber);
        }
        public void ExecuteAsParameter(RuntimeData runtime) {
            runtime.NowBlock.Push(this);
        }

        public override INumber Clone() {
            var res = this.MemberwiseClone() as MultipleFormula;
            res.items = new List<INumber>();
            res.items.AddRange(this.items);
            return res;
        }

        public override INumber Add(RuntimeData runtime, INumber val) {
            if (val is AdditionFormula) {
                var af = val.Clone() as AdditionFormula;
                af.AddItem(runtime, this);
                return af;
            }else {
                AdditionFormula af = new AdditionFormula();
                af.AddItem(runtime, val);
                af.AddItem(runtime, this);
                return af;
            }
        }
        public override INumber Multiple(RuntimeData runtime, INumber val) {

            if (val is Fraction) {
                var f = val.Clone() as Fraction;
                f.Numerator = f.Numerator.Multiple(runtime, this);
                return f;
            }
            if (val is Number && (val as Number).Value == 0) {
                return Number.New(0);
            }

            if (!(val is ImaginaryNumber) && val.ContainsImaginalyNumber)
                throw new NotImplementedException("虚数を含む値をかけるのはまだ未対応です。");


            if (val is Number || val is Variable || val is Member) {
                var me = this.Clone() as MultipleFormula;
                me.AddItem(runtime, val);
                return me;
            }
            else if (val is AdditionFormula) {
                var res = this.Clone() as MultipleFormula;
                res.AddItem(runtime, val);
                return res;
            }
            else if (val is MultipleFormula) {
                var res = new MultipleFormula();
                var v = val as MultipleFormula;
                for (int i = 0; i < this.items.Count; i++) {
                    bool flag = false;
                    for (int j = 0; j < v.items.Count; j++) {
                        if (this.items[i].CanJoin(runtime, v.items[j])) {
                            res.AddItem(runtime, this.items[i].Multiple(runtime, v.items[j]));
                            flag = true; break;
                        }
                    }
                    if (!flag) // ConJoinに引っかからなかった
                        res.AddItem(runtime, this.items[i]);
                }
                return res;
            }
            else {
                var me = this.Clone() as MultipleFormula;
                me.AddItem(runtime, val);
                return me;
            }
        }
        public override INumber Divide(RuntimeData runtime, INumber val) {
            // MultipleFormula同士の演算の場合以外はBase.Divideに投げる

            bool res;
            var result = this.TryDivide(runtime, val, out res);
            if (res) return result;

            return base.Divide(runtime, val);
        }
        public INumber TryDivide(RuntimeData runtime, INumber val, out bool result) {

            // IConstParameterで割ろうとした場合はBase.Divideでも対応できるのでここでは割愛する

            // メンバー(変数)で割ろうとした場合
            Variable v = val as Variable;
            if (v != null || (v = val.Eval(runtime) as Variable) is Variable) {
                var me = this.Clone() as MultipleFormula;

                for (int i = 0; i < me.items.Count; i++) {
                    var item = me.items[i];
                    if (item is MultipleFormula)
                        throw new FuncCalcException("MultipleFormulaの中にMultipleFormulaが含まれています。", this);

                    if (item is Variable) {
                        var res = item.Divide(runtime, v);
                        if (res is Number && (res as Number).Value == 1) {
                            me.items.RemoveAt(i);
                            result = true;
                            return me;
                        }
                        me.items[i] = res;
                        result = true;
                        return me;
                    }
                }
            }
            if (val is MultipleFormula) {
                MultipleFormula den = new Expression.MultipleFormula();
                INumber num = this.Clone() as MultipleFormula;
                MultipleFormula f = val as MultipleFormula;
                bool flag = false;

                for (int i = 0; i < f.items.Count; i++) {
                    var ex = f.items[i];
                    bool dv_res; INumber res;
                    res = (num as MultipleFormula).TryDivide(runtime, ex, out dv_res);
                    if (dv_res) {
                        flag = true;
                        if (res is MultipleFormula) {
                            num = res as MultipleFormula;
                        } else {
                            num = res;
                            for (int j = i + 1; j < f.items.Count; j++)
                                den.AddItem(runtime, f.items[j]);
                            break;
                        }
                    } else {
                        den.AddItem(runtime, ex);
                    }
                }
                if (!flag) {
                    result = false; return null;
                }
                else {

                    result = true;

                    // numの条件判定はdenより先に行う
                    if (num is MultipleFormula) {
                        INumber d = den;
                        if (den.items.Count == 0) return num;
                        if (den.items.Count == 1) d = den.items[0];

                        if ((num as MultipleFormula).items.Count == 0)
                            return new Fraction(d, Number.New(1));
                        if ((num as MultipleFormula).items.Count == 1)
                            return new Fraction(d, (num as MultipleFormula).items[0]);
                    }

                    // 最低限の形で返す
                    if (den.items.Count == 0)
                        return num;
                    if (den.items.Count == 1) {
                        var res = new Fraction(den.items[0], num);
                        if (runtime.Setting.DoOptimize)
                            return res.Optimise(runtime);
                        else
                            return res;
                    }

                    return new Fraction(den, num);
                }
            }

            result = false;
            return null;
        }
        public override bool Equals(RuntimeData runtime, INumber val) {
            return false;
        }
        public override INumber Optimise(RuntimeData runtime) {
            INumber res = Number.New(1);
            for (int i = 0; i < this.Count; i++) {
                res = res.Multiple(runtime, this.items[i]);
                if (!(res is MultipleFormula))
                    res = res.Optimise(runtime);
            }
            //if (!(res is MultipleFormula))
            //    return res.Optimise(runtime);

            return res;
        }
        public override bool CanJoin(RuntimeData runtime, INumber val) {
            return false;
        }

        public override INumber ExecuteDiff(RuntimeData runtime, string t) {

            // アイテムがない場合は0を返す
            if (this.items.Count == 0) {
                return Number.New(0);
            }
            // アイテムが1つしかない場合はそれを微分する
            if (this.items.Count == 1) {
                return this.items[0].ExecuteDiff(runtime, t);
            }

            // アイテムが複数個ある場合は定数と変数を取り出し、場合によって合成関数の微分んを行う
            // 参考: 合成関数の微分 
            // 参考: 合成関数内に合成関数 http://note.chiebukuro.yahoo.co.jp/detail/n86698
            INumber constant = Number.New(1);
            List<INumber> funcs = new List<INumber>();
            for (int i = 0; i < this.items.Count; i++) {
                var item = this.items[i];

                if (item is IConstParameter || item is ImaginaryNumber) {
                    constant = constant.Multiple(runtime, item);
                } else {
                    funcs.Add(item);
                }
            }

            AdditionFormula res = new AdditionFormula();
            // 合成関数の微分を行う
            for (int i = 0; i < funcs.Count; i++) {
                INumber r = Number.New(1);
                for (int j = 0; j < funcs.Count; j++) {
                    if (i == j) {
                        r = r.Multiple(runtime, funcs[j].ExecuteDiff(runtime, t));
                    }else {
                        r = r.Multiple(runtime, funcs[j]);
                    }
                }
                res.AddItem(runtime, r);
            }


            // 最適化する
            INumber resOpt = res;
            if (runtime.Setting.DoOptimize)
                resOpt = resOpt.Optimise(runtime);

            // constantの状態を見て結果を返す
            if (constant.Equals(runtime, Number.New(1))) {
                return resOpt;
            } else {
                MultipleFormula mf = new MultipleFormula();
                mf.AddItem(runtime, constant);
                mf.AddItem(runtime, resOpt);
                return mf;
            }
 
        }
        public override INumber Integrate(RuntimeData runtime, string t) {

            if (!(this.Pow is Number && (this.Pow as Number).Value == 1)) {

                throw new NotImplementedException("べき乗を含む積の式の積分はまだ未対応です。");
            }

            MultipleFormula mf = new MultipleFormula();
            List<INumber> intg = new List<INumber>();

            foreach (var item in this.items) {

                // itemが定数なら
                if (item is IConstParameter || item is ImaginaryNumber)
                    mf.AddItem(runtime, item);
                else // ちがうなら
                    intg.Add(item);
            }

            if (intg.Count == 0) {
                mf.AddItem(runtime, new Variable(t));
                return mf;
            }
            else if (intg.Count == 1) {
                mf.AddItem(runtime, intg[0].Integrate(runtime, t));
                if (runtime.Setting.DoOptimize)
                    return mf.Optimise(runtime);
                else return mf;
            }
            else if (intg.Count == 2) {
                // 両方とも無限回[微分]可能な関数なら積分を行う子てゃできない
                if (intg[0].InfinitelyDifferentiable &&
                    intg[1].InfinitelyDifferentiable) {
                    MultipleFormula mmf = new MultipleFormula();
                    for (int i = 0; i < intg.Count; i++) {
                        mmf.AddItem(runtime, intg[i]);
                    }
                    mf.AddItem(runtime, new FuncedINumber(
                        runtime.GetFunc("intg"), new INumber[] { new Variable(t), mmf }));
                    return mf;
                }

                INumber diffable = null, other = null;
                if (intg[0].InfinitelyDifferentiable) {
                    diffable = intg[1];
                    other = intg[0];
                }
                else {
                    diffable = intg[0];
                    other = intg[1];
                }

                // diffable * other <=> f(x)g(x)
                AdditionFormula af = new AdditionFormula();
                INumber diffedDiffable = null, intedOthr = null;
                INumber res1 = null, res2 = null;
                // f*intg(g)
                {
                    MultipleFormula mf2 = new MultipleFormula();
                    mf2.AddItem(runtime, diffable);
                    mf2.AddItem(runtime, intedOthr = other.Integrate(runtime, t));
                    res1 = mf2;
                }
                // -intg(f' * intg(g))
                {
                    MultipleFormula mf3 = new MultipleFormula();
                    mf3.AddItem(runtime, diffedDiffable = diffable.ExecuteDiff(runtime, t));
                    mf3.AddItem(runtime, other.Integrate(runtime, t));
                    res2 = mf3.Integrate(runtime, t).Multiple(runtime, Number.New(-1));
                }

                // ログに出力
                if (runtime.EnabledLogging) {
                    MultipleFormula mf4 = new Expression.MultipleFormula();
                    mf4.Items.AddRange(intg);
                    runtime.AddLogWay("_PartialIntegralWay1", new Variable("a"));
                    runtime.AddLogWay("_PartialIntegralWay2",
                        mf4, new Variable(t), diffable, other, diffedDiffable, intedOthr, mf);
                }
                af.AddItem(runtime, res1);
                af.AddItem(runtime, res2);

                mf.AddItem(runtime, af);

                if (runtime.EnabledLogging) {
                    AdditionFormula res = new Expression.AdditionFormula();
                    if (runtime.Setting.DoOptimize)
                        res.AddItem(runtime, mf.Optimise(runtime));
                    else
                        res.AddItem(runtime, mf);
                    var c = IntegralConstant.Create(runtime);
                    res.AddItem(runtime, c);

                    runtime.AddLogWay("Way", res);
                    runtime.AddLogCondition("IntegralConstant", c);
                }

                return mf;
            }
            else
                throw new RuntimeException("3つ以上の合成関数の積分はまだ実装していません。", this);

        }

        public override INumber Eval(RuntimeData runtime) {
            MultipleFormula mf = new Expression.MultipleFormula();
            for (int i = 0; i < this.items.Count; i++) {
                var res = this.items[i].Eval(runtime);
                bool flag = false;
                for (int j = 0; j < mf.items.Count; j++) {
                    if (mf.items[j].CanJoin(runtime, res)) {
                        var itm = mf.items[j];
                        mf.items.RemoveAt(j);
                        mf.AddItem(runtime, itm.Multiple(runtime, res));
                        flag = true; break;
                    }
                }
                if (!flag)
                    mf.AddItem(runtime, res);
            }
            if (mf.items.Count == 0)
                return Number.New(1);
            if (mf.items.Count == 1)
                return mf.items[0];
            else
                return mf;
        }
        public override INumber FinalEval(RuntimeData runtime) {
            INumber res = Number.New(1);
            for (int i = 0; i < this.items.Count; i++) {
                res = res.Multiple(runtime, this.items[i].FinalEval(runtime));
            }
            return res;
        }

        public override string ToString() {

            if (this.items.Count == 0)
                return "1";

            // 文字列化する前にアイテムをソートしておく
            if (!this.DontSort)
                FuncCalc.Runtime.Support.SortFormula.Sort(this.items);

            StringBuilder sb = new StringBuilder();
            foreach (var item in this.items) {
                if (sb.Length != 0) sb.Append(" ");
                sb.Append(string.Format("{0}", item));
            }
            if (!(this.Pow is Number && (this.Pow as Number).Value == 1))
                sb.Append(string.Format("^{0}", this.Pow));
            return sb.ToString();
        }



        public override string Output(OutputType type) {
            
            if (this.items.Count == 0)
                return "1";

            // 文字列化する前にアイテムをソートしておく
            if (!this.DontSort)
                FuncCalc.Runtime.Support.SortFormula.Sort(this.items);

            switch (type) {
                case OutputType.Mathjax: {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("(");
                        for (int i = 0; i < this.Count; i++) {
                            if (i != 0) sb.Append(" ");
                            sb.Append("{");
                            sb.Append(this.items[i].Output(type));
                            sb.Append("}");
                        }
                        sb.Append(")");
                        if (this.Count == 0)
                            return "";
                        return sb.ToString();
                    }

            }

            return base.Output(type);
        }
    }
}
