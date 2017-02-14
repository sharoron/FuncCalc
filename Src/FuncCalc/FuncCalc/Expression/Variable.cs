using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Runtime;

namespace FuncCalc.Expression
{
    public class Variable : INumber, IExpression, IMulti {
        private INumber multi = Number.New(1);

        public override ExpressionType Type
        {
            get
            {
                return ExpressionType.Unknown;
            }
        }
        public string Name
        {
            get { return this.Token.Text; }
        }
        public INumber Multi
        {
            get { return this.multi; }
            set { this.multi = value; }
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

        private Variable() { }
        public Variable(string name) {
            this.Token = new FuncCalc.Token(name, Analyzer.TokenType.Member);
        }
        public Variable(Token token) {
            this.Token = token;
        }

        public override INumber Add(RuntimeData runtime, INumber val) {

            if (this.CanJoin(runtime, val)) {
                var me = this.Clone() as Variable;
                me.multi = me.multi.Add(runtime, (val as Variable).multi);
                return me;
            }

            AdditionFormula af = new AdditionFormula();
            af.AddItem(runtime, this);
            af.AddItem(runtime, val);
            return af;
        }
        public override INumber Multiple(RuntimeData runtime, INumber val) {

            var v = val.Eval(runtime);

            if (v is Variable) {
                if ((v as Variable).Name == this.Name) {
                    var me = this.Clone() as Variable;
                    me.multi = me.multi.Multiple(runtime, (v as Variable).multi);
                    me.Pow = me.Pow.Add(runtime, (v as Variable).Pow);
                    return me;
                } else {
                    MultipleFormula mf = new MultipleFormula();
                    mf.AddItem(runtime, this);
                    mf.AddItem(runtime, v);
                    return mf;
                }
            }
            if (v is Number && (v as Number).Value == 1)
                return this;
            if (v is Number) {
                var me = this.Clone() as Variable;
                me.multi = me.multi.Multiple(runtime, v);
                return me;
            }

            return v.Multiple(runtime, this);

            MultipleFormula af = new MultipleFormula();
            af.AddItem(runtime, this);
            af.AddItem(runtime, v);
            return af;
        }
        public override INumber Divide(RuntimeData runtime, INumber val) {
            
            if (val is Variable) {
                var v = val as Variable;
                if (this.Name == v.Name) {
                    if (!this.Pow.Equals(runtime, v.Pow)) {
                        INumber pow = this.Pow.Subtract(runtime, val.Pow);
                        var newMulti = this.multi.Divide(runtime, v.multi);
                        if (newMulti is Number) {
                            if ((newMulti as Number).Value == 0)
                                return new Variable(this.Token) { Pow = pow };
                        }
                        return new Variable(this.Token) {
                            multi = newMulti,
                            Pow = pow,
                        };
                    }
                    else {
                        var newMulti = this.multi.Subtract(runtime, v.multi);
                        if (newMulti is Number) {
                            if ((newMulti as Number).Value == 0)
                                return Number.New(1);
                            else
                                return newMulti;
                        }
                        else {
                            return new Variable(this.Token) {
                                multi = newMulti,
                                Pow = this.Pow,
                            };
                        }
                    }
                }
            }

            return base.Divide(runtime, val);
        }
        public override bool Equals(RuntimeData runtime, INumber val) {
            var me = this.Eval(runtime);
            if (me is Variable && val is Variable) {
                Variable m = me as Variable, v = val as Variable;
                if (m.Name == v.Name &&
                    m.multi.Equals(runtime, v.multi) &&
                    m.Pow.Equals(runtime, v.Pow))
                    return true;
                else
                    return false;
            }
            return false; // 正確には一致しているか分からない
        }
        public override bool CanJoin(RuntimeData runtime, INumber val) {
            // 同じ名前の変数でべき乗の値が同じであれば結合可能
            if (val is Variable &&
                this.Name == (val as Variable).Name &&
                this.Pow.Equals(runtime, val.Pow))
                return true;

            return false;
        }
        public override INumber Eval(RuntimeData runtime) {
            if (runtime.ContainsKey(this.Token.Text, runtime.NowBlock.MoreScope)) {
                var res = runtime.GetData(this.Token, runtime.NowBlock.MoreScope).Clone();
                res = res.Multiple(runtime, this.multi);
                res = res.Power(runtime, this.Pow);
                if (res == this)
                    return this;
                return res.Eval(runtime);
            } else {
                return this;
            }
        }
        public override INumber Optimise(RuntimeData runtime) {

            return base.Optimise(runtime);
        }
        public override INumber FinalEval(RuntimeData runtime) {
            var res = runtime.GetData(this.Token);
            // resをFinalEvalする前にresがVariableじゃないことを確認する
            if (res is Variable)
                return this;
            res = res.FinalEval(runtime);

            res = res.Power(runtime, this.Pow);
            return res;
        }

        public override INumber Differentiate(RuntimeData runtime, DifferentialData ddata) {
            
            if (runtime.ContainsKey(this.Name)) {
                return this.Eval(runtime).Differentiate(runtime, ddata);   
            }
            else {
                // this.Multiの内容が1じゃないなら合成関数を作ってそこで微分する
                if (!Number.IsOne(this.multi)) {
                    MultipleFormula mf = new Expression.MultipleFormula();
                    var clone = this.Clone() as Variable;
                    clone.multi = Number.New(1);
                    mf.AddItem(runtime, clone);
                    mf.AddItem(runtime, this.multi);
                    return mf.Differentiate(runtime, ddata);
                }
                if (ddata.IsFunction(this.Pow)) {
                    // (変数)^(変数)なので対数微分法を使用して微分することになる
                    return ddata.LogarithmicDifferentiate(this);
                }

                // ここから先は確実に(変数)^(定数)
                if (this.Name == ddata.T) {
                    if (!(this.multi is IConstParameter))
                        throw new NotImplementedException();
                    var pow = this.Pow;
                    var me = this.Clone();
                    me.Pow = me.Pow.Subtract(runtime, Number.New(1));
                    me = me.Multiple(runtime, pow);

                    me = me.Optimise(runtime);
                    return me;
                }
                else {
                    MultipleFormula res = new MultipleFormula();
                    var diffY = new FuncedINumber(
                        runtime.GetFunc("diff"), new INumber[] { new Variable(new Token(ddata.T, Analyzer.TokenType.Member)), this });
                    if (this.Pow.IsOne)
                        return diffY;
                    
                    var clone = this.Clone();
                    clone.Pow = clone.Pow.Subtract(runtime, Number.New(1));
                    res.AddItem(runtime, this.Pow);
                    res.AddItem(runtime, clone);
                    res.AddItem(runtime, diffY);
                    return res;
                }
            }
        }
        public override INumber Integrate(RuntimeData runtime, string t) {
            return (new Member(this.Token) { Pow = this.Pow }).Integrate(runtime, t);
        }


        public override string ToString() {
            return string.Format("{0}{1}{2}",
                (!(this.multi is Number) || (this.multi as Number).Value != 1) ? this.multi.ToString() : "",
                this.Name,
                ((!(this.Pow is Number) || (this.Pow as Number).Value != 1)) ? "^" + this.Pow.ToString() : "");
        }
        public override string Output(OutputType type) {
            switch (type) {
                case OutputType.String:
                    return this.ToString();
                case OutputType.Mathjax: {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(this.Name);

                        if (!(this.Multi is Number && (this.Multi as Number).Value == 1))
                            sb.Insert(0, this.Multi.Output(type));
                        if (!(this.Pow is Number && (this.Pow as Number).Value == 1))
                            sb.Append("^{" + this.Pow.Output(type) + "}");

                        return sb.ToString();
                    }
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
