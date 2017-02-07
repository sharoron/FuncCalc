using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Runtime;
using FuncCalc.Exceptions;
using FuncCalc.Expression.Const;

namespace FuncCalc.Expression
{
    /// <summary>分数</summary>
    public class Fraction : INumber
    {
        private INumber _denominator = null;
        private INumber _numerator = null;

        public Fraction() : this(Number.New(1), Number.New(1)) { }
        /// <param name="denominator">分母</param>
        /// <param name="numerator">分子</param>
        public Fraction(INumber denominator, INumber numerator) {
            this._denominator = denominator;
            this._numerator = numerator;
        }

        /// <summary>分母</summary>
        public INumber Denominator
        {
            get { return this._denominator; }
            set { this._denominator = value; }
        }
        /// <summary>分子</summary>
        public INumber Numerator
        {
            get { return this._numerator; }
            set { this._numerator = value; }
        }
        public override ExpressionType Type
        {
            get
            {
                return ExpressionType.Number;
            }
        }
        public override ValueType ValueType
        {
            get
            {
                if (this.Denominator.ValueType == ValueType.Unknown ||
                    this.Numerator.ValueType == ValueType.Unknown)
                    return ValueType.Unknown;

                if (this.Denominator.ValueType == this.Numerator.ValueType)
                    return this.Denominator.ValueType;
                else
                    return ValueType.Minus;
            }
        }
        public override int SortPriority {
            get { return 200; }
        }
        public override INumber Pow
        {
            get
            {
                return base.Pow;
            }
            protected internal set
            {
                throw new FuncCalcException("FunctionにPowを直接指定することはできません。", null, null);
            }
        }
        public override bool InfinitelyDifferentiable
        {
            get
            {
                return
                    this.Denominator.InfinitelyDifferentiable ||
                    this.Numerator.InfinitelyDifferentiable;
            }
        }
        public override bool ContainsImaginalyNumber
        {
            get
            {
                return
                    this.Denominator.ContainsImaginalyNumber ||
                    this.Numerator.ContainsImaginalyNumber;
            }
        }

        public override INumber Add(RuntimeData runtime, INumber val) {

            if (val.Pow is Number && (val.Pow as Number).Value == 1) {
                if (val is Number) {
                    var me = this.Clone() as Fraction;
                    me.Numerator = me.Numerator.Add(runtime, this.Denominator.Multiple(runtime, val));
                    return me;
                }
                if (val is Fraction) {
                    var me = this.Clone() as Fraction;

                    // 分母が同じ場合は単純に足す
                    if (this.Denominator.Equals(runtime, val)) {
                        var res = new Fraction(this.Denominator, this.Numerator.Add(runtime, (val as Fraction).Numerator));
                        if (runtime.Setting.DoOptimize)
                            return res.Optimise(runtime);
                        else
                            return res;
                    }
                    // 違う場合はいろいろやる
                    else {
                        var res = new Fraction(
                            this.Denominator.Multiple(runtime, (val as Fraction).Denominator),
                            this.Numerator.Multiple(runtime, (val as Fraction).Denominator).Add(runtime,
                            this.Denominator.Multiple(runtime, (val as Fraction).Numerator))
                            );
                        if (runtime.Setting.DoOptimize)
                            return res.Optimise(runtime);
                        else
                            return res;
                    }
                }
            } else if (val is Number || val is Fraction) {
                AdditionFormula af = new AdditionFormula();
                af.AddItem(runtime, this);
                af.AddItem(runtime, val);
                return af;
            }

            return val.Add(runtime, this);
        }
        public override INumber Multiple(RuntimeData runtime, INumber val)
        {


            if (val is Number)
            { // Powあっても大丈夫
                INumber res = new Expression.Fraction(
                    this.Denominator,
                    this.Numerator.Multiple(runtime, val)
                    );
                if (runtime.Setting.DoOptimize)
                    res = res.Optimise(runtime);

                return res;
            }
            if (val is Fraction)
            {
                INumber res = new Fraction(
                    this.Denominator.Multiple(runtime, (val as Fraction).Denominator),
                    this.Numerator.Multiple(runtime, (val as Fraction).Numerator));
                if (runtime.Setting.DoOptimize) return res.Optimise(runtime);
                else
                    return res;
            }

            // 旧コード 2017/01/12
            // return val.Multiple(runtime, this);
            {
                var res = this.Clone() as Fraction;
                res.Numerator = res.Numerator.Multiple(runtime, val);
                return res;

            }
        }
        public override INumber Divide(RuntimeData runtime, INumber val) {
            
            if (val is Number) {
                Fraction me = this.Clone() as Fraction;
                me.Denominator = me.Denominator.Multiple(runtime, val);
                if (runtime.Setting.DoOptimize)
                    return me.Optimise(runtime);
                else
                    return me;
            }
            if (val is Fraction) {
                var res = new Fraction(
                    this.Denominator.Multiple(runtime, (val as Fraction).Numerator),
                    this.Numerator.Multiple(runtime, (val as Fraction).Denominator));
                if (runtime.Setting.DoOptimize)
                    return res.Optimise(runtime);
                else
                    return res;
            }
            if (this.Denominator is MultipleFormula) {
                bool mf_res;
                var dv_res = (this.Denominator as MultipleFormula).TryDivide(runtime, val, out mf_res);
                if (mf_res) {
                    var me = this.Clone() as Fraction;
                    me.Denominator = dv_res;
                    if (runtime.Setting.DoOptimize)
                        return me.Optimise(runtime);
                    else
                        return me;
                }
                else {
                    var me = this.Clone() as Fraction;
                    me.Denominator = me.Denominator.Multiple(runtime, val);
                    if (runtime.Setting.DoOptimize)
                        return me.Optimise(runtime);
                    else
                        return me;
                }
            }

            return val.Multiple(runtime, new Fraction(this.Numerator, this.Denominator));
        }
        public override INumber Eval(RuntimeData runtime) {
            var den = this.Denominator.Eval(runtime);
            var num = this.Numerator.Eval(runtime);
            Fraction f = new Expression.Fraction(den, num);

            if (runtime.Setting.DoOptimize)
                return f.Optimise(runtime);
            else
                return f;
        }
        public override INumber Optimise(RuntimeData runtime)
        {

            var me = this.Clone() as Fraction;

            // [分母][分子]が分数の場合は有理化する
            if (me.Denominator is Fraction && me.Numerator is Fraction)
            {
                me = new Fraction(
                    (me.Denominator as Fraction).Numerator.Multiple(runtime, (me.Numerator as Fraction).Denominator),
                    (me.Denominator as Fraction).Denominator.Multiple(runtime, (me.Denominator as Fraction).Numerator)
                    );
            }
            else if (me.Denominator is Fraction)
            {
                me = new Fraction(
                    (me.Denominator as Fraction).Numerator,
                    (me.Denominator as Fraction).Denominator.Multiple(runtime, me.Numerator)
                    );
            }
            else if (me.Numerator is Fraction)
            {
                me = new Fraction(
                    me.Denominator.Multiple(runtime, (me.Numerator as Fraction).Denominator),
                    (me.Numerator as Fraction).Numerator
                    );
            }

            // 分子が0ならそのまま0を返す
            if (me.Numerator.Equals(runtime, Number.New(0))) {
                return Number.New(0);
            }

            // 分母が0だったら無限を返す
            if (me.Denominator.Eval(runtime).Equals(runtime, Number.New(0)))
            {
                if (me.Numerator.ValueType == ValueType.Plus)
                    return InfinityValue.PlusValue;
                else if (me.Numerator.ValueType == ValueType.Minus)
                    return InfinityValue.MinusValue;
                else
                {
                    MultipleFormula mf = new MultipleFormula();
                    mf.AddItem(runtime, this.Numerator);
                    mf.AddItem(runtime, InfinityValue.PlusValue);
                    return mf;
                    ;
                }
            }

            if (me.Numerator is Number && me.Denominator is Number)
            {
                // 最大公約数を求めて分母分子を割る
                var func = runtime.Functions.Where(
                    f => f.Key == "gcd" && f.Value.Parameter[0] == ExpressionType.Number && f.Value.Parameter[1] == ExpressionType.Number);
                var res = func.First().Value.Execute(runtime, me.Denominator, me.Numerator);
                me.Denominator = me.Denominator.Divide(runtime, res);
                me.Numerator = me.Numerator.Divide(runtime, res);
            }
            else if (me.Denominator is MultipleFormula && this.Numerator is Variable)
            {
                bool dv_res; INumber dr_res;
                dr_res = (me.Denominator as MultipleFormula).TryDivide(runtime, me.Numerator, out dv_res);
                if (dv_res)
                {
                    me.Numerator = Number.New(1);
                    me.Denominator = dr_res;
                }
                else
                {
                }
            }
            else if (me.Denominator is MultipleFormula && me.Numerator is MultipleFormula)
            {
                for (int i = 0; i < (me.Numerator as MultipleFormula).Count; i++)
                {
                    var dt = (me.Numerator as MultipleFormula).Items[i];
                    bool dv_res;
                    var r = (me.Denominator as MultipleFormula).TryDivide(runtime, dt, out dv_res);
                    if (dv_res)
                    {
                        me.Denominator = r;
                        (me.Numerator as MultipleFormula).Items.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                    }
                }
            }

            // 分母分子を最適化する

            me.Denominator = me.Denominator.Optimise(runtime);
            me.Numerator = me.Numerator.Optimise(runtime);


            // メンバーを含む式で約分できるか確かめる
            if (me.Numerator is MultipleFormula && me.Numerator.Pow.Equals(runtime, Number.New(1)) && 
                me.Denominator is MultipleFormula && me.Denominator.Pow.Equals(runtime, Number.New(1)))
            {
                var den = me.Denominator as MultipleFormula;
                bool flag = false;
                for (int i = 0; i < (me.Numerator as MultipleFormula).Count; i++)
                {
                    bool res;
                    var fm = den.TryDivide(runtime, (me.Numerator as MultipleFormula).Items[i], out res);
                    if (res)
                    {
                        flag = true;
                        me.Denominator = den;
                        if (fm is MultipleFormula) { den = fm as MultipleFormula; }
                        else
                        { break; }

                        (me.Numerator as MultipleFormula).Items.RemoveAt(i);
                        i--;
                    }
                }
            }


            // 分母が1だったら分子部分だけを返す
            if (me.Denominator.Equals(runtime, Number.New(1)))
            {
                return me.Numerator;
            }




            return me;
        }
        public override INumber Power(RuntimeData runtime, INumber val) {

            if (val is Number && (val as Number).Value == 0)
                return Number.New(1);

            var me = this.Clone() as Fraction;
            me.Denominator = me.Denominator.Power(runtime, val);
            me.Numerator = me.Numerator.Power(runtime, val);
            return me;
        }
        public override bool CanJoin(RuntimeData runtime, INumber val) {
            if (val is Number || val is Fraction)
                return true;

            return val.CanJoin(runtime, null);
        }
        public override bool Equals(RuntimeData runtime, INumber val) {
            
            if (val is Number) {
                var me = this.Optimise(runtime);
                if (me is Number && me.Equals(runtime, val))
                    return true;
                return false;
            }
            if (val is Fraction) {
                INumber f1 = this.Optimise(runtime);
                INumber f2 = (val as Fraction).Optimise(runtime);

                if (f1 is Fraction && f2 is Fraction &&
                    (f1 as Fraction).Denominator.Equals(runtime, (f2 as Fraction).Denominator) &&
                    (f1 as Fraction).Numerator.Equals(runtime, (f2 as Fraction).Numerator))
                    return true;
                if (f1 is Number && f2 is Number &&
                    f1.Equals(runtime, f2))
                    return true;
                if ((f1 is Number && f2 is Fraction) ||
                    (f1 is Fraction && f2 is Number))
                    return false;

                return f1.Equals(runtime, f2);
            }

            return val.Equals(runtime, this);
        }
        public override INumber FinalEval(RuntimeData runtime) {
            var me = this.Eval(runtime).Optimise(runtime) as Fraction;
            me.Denominator = me.Denominator.FinalEval(runtime);
            me.Numerator = me.Numerator.FinalEval(runtime);

            if (FuncCalc.Runtime.Func.Differential.IsConstValue(runtime, me.Denominator) &&
                FuncCalc.Runtime.Func.Differential.IsConstValue(runtime, me.Numerator)) {
                var res = 
                    (me.Numerator.FinalEval(runtime) as IConstParameter).ConstValue / 
                    (me.Denominator.FinalEval(runtime) as IConstParameter).ConstValue;
                return new FloatNumber(res);
            }
            return me;
        }

        public override string ToString() {
            return string.Format("({0} / {1})", this.Numerator, this.Denominator);
        }

        public override string Output(OutputType type) {
            switch (type) {
                case OutputType.Mathjax: {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("\\frac{");
                        sb.Append(this.Numerator.Output(type));
                        sb.Append("}{");
                        sb.Append(this.Denominator.Output(type));
                        sb.Append("}");
                        return sb.ToString();
                    }
                    break;
            }
            return base.Output(type);
        }

        public override INumber ExecuteDiff(RuntimeData runtime, string t) {
            if (Runtime.Func.Differential.IsConstValue(runtime, this.Denominator)) {
                var res = this.Numerator.ExecuteDiff(runtime, t);
                return res;
            } else {
                AdditionFormula af = new Expression.AdditionFormula();
                {
                    MultipleFormula mf = new Expression.MultipleFormula();
                    mf.AddItem(runtime, this.Numerator.ExecuteDiff(runtime, t));
                    mf.AddItem(runtime, this.Denominator);
                    af.AddItem(runtime, mf);
                }
                {
                    MultipleFormula mf = new Expression.MultipleFormula();
                    mf.AddItem(runtime, Number.New(-1));
                    mf.AddItem(runtime, this.Denominator.ExecuteDiff(runtime, t));
                    mf.AddItem(runtime, this.Numerator);
                    af.AddItem(mf);
                }
                Fraction f = new Expression.Fraction(
                    this.Denominator.Power(runtime, Number.New(2)),
                    af
                    );
                return f;
                
            }
        }
        public override INumber Integrate(RuntimeData runtime, string t) {

            // 計算し易いように条件を最適化しておく
            var ress = this.Clone().Optimise(runtime);
            if (!(ress is Fraction))
                return ress.Integrate(runtime, t);

            var res = ress as Fraction;

            

            // 特殊な定義: ∫(b / x^a)dx → b*log(x) / a
            // ∫(1 / x^2)dxとかはこの条件に当てはまらないので注意しないといけない
            if (runtime.IsConstValue(res.Numerator) && 
                ((res.Denominator is Variable && (res.Denominator as Variable).Name == t) || 
                (res.Denominator is Member && (res.Denominator as Member).Text == t)) &&
                runtime.IsConstValue(res.Denominator.Pow)) {

                MultipleFormula mf = new Expression.MultipleFormula();
                mf.AddItem(runtime, res.Numerator);
                mf.AddItem(runtime, new Fraction(res.Denominator.Pow, Number.New(1)));
                mf.AddItem(runtime,  new FuncedINumber(
                    runtime.Functions["log"], 
                    new INumber[] { new NaturalLogarithm(), res.Denominator }));
                if (runtime.Setting.DoOptimize)
                    return mf.Optimise(runtime);
                else
                    return mf;
            }

            // 分母も分子も定数しか含まれていない → 定数として扱う
            if (runtime.IsConstValue(res.Denominator) && 
                runtime.IsConstValue(res.Numerator)) {
                MultipleFormula mf = new Expression.MultipleFormula();
                mf.AddItem(runtime, res);
                mf.AddItem(runtime, new Variable(t));
                return mf;
            }

            // 分母が定数なら積分の外に出す
            if (runtime.IsConstValue(this.Denominator) &&
                !(this.Denominator is Number && (this.Denominator as Number).Value == 1)) {
                MultipleFormula mf = new Expression.MultipleFormula();
                mf.AddItem(runtime, new Fraction(this.Denominator, Number.New(1)));
                mf.AddItem(runtime, this.Numerator.Integrate(runtime, t));
                return mf;
            }
            // 分子が定数なら積分の外に出す
            if (runtime.IsConstValue(this.Numerator) && 
                !(this.Numerator is Number && (this.Numerator as Number).Value == 1)) {
                MultipleFormula mf = new Expression.MultipleFormula();
                mf.AddItem(runtime, this.Numerator);
                mf.AddItem(runtime, (new Fraction(this.Denominator, Number.New(1))));
                return mf;
            }

            // 事前に分母と分子に存在する定数を外に出して計算した方がいい
            runtime.Setting.Logger.AddInfo("分数の積分は申し訳程度にしか実装していません。");

            throw new NotImplementedException("分数の積分はあまり対応してない");
        }
    }
}
