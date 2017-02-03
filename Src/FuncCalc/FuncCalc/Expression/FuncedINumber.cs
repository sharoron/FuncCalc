using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Runtime;
using System.Diagnostics;

namespace FuncCalc.Expression
{
    public class FuncedINumber : INumber
    {
        public INumber[] param = null;
        public IFunction func = null;

        private FuncedINumber() { }
        public FuncedINumber(IFunction func, INumber[] parameter) {
            this.param = parameter;
            this.func = func;
        }

        public INumber[] Parameters
        {
            get { return this.param; }
        }
        public IFunction Function
        {
            get { return this.func; }
        }

        public override ExpressionType Type
        {
            get
            {
                return ExpressionType.Unknown;
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
            get { return 3000; }
        }
        public override bool InfinitelyDifferentiable
        {
            get
            {
                if (this.Function is IDiffWithParameters)
                    return (this.Function as IDiffWithParameters).InfinitelyDifferentiable;
                if (this.Function is IDiff)
                    return (this.Function as IDiff).InfinitelyDifferentiable;

                return true;
            }
        }

        public override INumber Add(RuntimeData runtime, INumber val) {
            AdditionFormula af = new Expression.AdditionFormula();
            af.AddItem(runtime, val);
            af.AddItem(runtime, this);
            return af;
        }
        public override INumber Multiple(RuntimeData runtime, INumber val) {
            if (val is FuncedINumber && this.CanJoin(runtime, val)) {
                var res = this.Clone();
                res.Pow = res.Pow.Add(runtime, val.Pow);
                return res;
            }

            MultipleFormula mf = new Expression.MultipleFormula();
            mf.AddItem(runtime, val);
            mf.AddItem(runtime, this);
            return mf;
        }
        public override bool Equals(RuntimeData runtime, INumber val) {
            return false;
        }
        public override bool CanJoin(RuntimeData runtime, INumber val) {
            if (val is FuncedINumber &&
                this.Function == (val as FuncedINumber).Function &&
                this.Function.EnabledPow && 
                this.Parameters.Length == (val as FuncedINumber).Parameters.Length) {
                bool res = true;

                for (int i = 0; i < this.Parameters.Length; i++) {
                    if (!this.Parameters[i].Equals(runtime, (val as FuncedINumber).Parameters[i])) {
                        res = false; break;
                    }
                }

                return res;
            }

            return false;
        }
        public override INumber FinalEval(RuntimeData runtime) {

            var res = this.func.ForceExecute(runtime, param);
            return res.Power(runtime, this.Pow);

        }

        public override string ToString() {
            return this.Output(OutputType.String);
        }
        public override INumber ExecuteDiff(RuntimeData runtime, string t) {

            INumber res = null;
            if (this.func is IDiffWithParameters) {
                res = (this.func as IDiffWithParameters).ExecuteDiff(runtime, t, this.param);

                // この段階ではpowのことは感がられていないので、pow部をここで処理する
                if (!(this.Pow is Number && (this.Pow as Number).Value == 1)) {
                    MultipleFormula mf = new MultipleFormula();
                    mf.AddItem(runtime, res);
                    INumber pow = this.Clone();
                    pow.Pow = pow.Pow.Subtract(runtime, Number.New(1));
                    mf.AddItem(runtime, pow);

                    res = mf;
                }

                if (runtime.Setting.DoOptimize)
                    res = res.Optimise(runtime);
            }
            else {
                res = new FuncedINumber(
                    runtime.Functions["diff"], new INumber[] { new Variable(new FuncCalc.Token(t, Analyzer.TokenType.Member)), this });

                // たぶんpowの部分が怪しいのでチェックすること
                Debugger.Break();
            }

            //最終処理部
            {
                var mf = Runtime.Func.Differential.DiffPow(runtime, t, this);
                if (mf != null) {
                    mf.AddItem(runtime, res);
                    return mf;
                }
                else
                    return res;
            }

        }
        public override INumber Integrate(RuntimeData runtime, string t) {

            // べき乗の指定があったら部分積分法で解く
            if (!(this.Pow is Number && (this.Pow as Number).Value == 1)) {

                throw new NotImplementedException("べき乗を含む関数式の積分はまだ未対応です。");


            }
            else {

                INumber res = null;
                if (this.func is IIntegrateWithParameters) {
                    res = (this.func as IIntegrateWithParameters).Integrate(runtime, t, this.param);

                    // この段階ではpowのことは感がられていないので、pow部をここで処理する
                    if (!(this.Pow is Number && (this.Pow as Number).Value == 1)) {
                        throw new NotImplementedException("べき乗が1ではない関数の積分はまだ未対応です");

                        // 以下、微分のコードのコピペ
                        MultipleFormula mf = new MultipleFormula();
                        mf.AddItem(runtime, res);
                        INumber pow = this.Clone();
                        pow.Pow = pow.Pow.Subtract(runtime, Number.New(1));
                        mf.AddItem(runtime, pow);

                        res = mf;
                    }

                    if (runtime.Setting.DoOptimize)
                        res = res.Optimise(runtime);
                }
                else {
                    res = new FuncedINumber(
                        runtime.Functions["intg"], new INumber[] { new Variable(new FuncCalc.Token(t, Analyzer.TokenType.Member)), this });

                    // たぶんpowの部分が怪しいのでチェックすること
                    Debugger.Break();
                }

                //最終処理部
                {
                    // resが結果だけど、べき乗部分の処理をしていないかもしれないので要チェック

                    Debugger.Break();
                    if (!(this.Pow is Number && (this.Pow as Number).Value == 1))
                        runtime.Setting.Logger.AddWarning("関数式の積分の部分は1以外のべき乗の計算を考慮していないので、結果がおかしくなっている可能性があります。");

                    return res;
                }

            }
        }


        public override string Output(OutputType type) {

            if (this.func is IFunctionOutput) {
                StringBuilder sb = new StringBuilder();
                if (this.Parameters == null) {
                    sb.Append((this.func as IFunctionOutput).Output(type));
                    switch (type) {
                        case OutputType.Mathjax:
                            if (!(this.Pow is Number && (this.Pow as Number).Value == 1)) {
                                sb.Append("^{");
                                sb.Append(this.Pow.Output(type));
                                sb.Append("}");
                            }
                            break;
                        default:
                            if (!(this.Pow is Number && (this.Pow as Number).Value == 1)) {
                                sb.Append("^");
                                sb.Append(this.Pow.Output(type));
                            }
                            break;
                    }

                }
                else
                    sb.Append((this.func as IFunctionOutput).Output(type, this.Parameters, this.Pow));
                return sb.ToString();
            }

            switch (type) {
                case OutputType.Mathjax: {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(this.func.Name);
                        if (this.Pow is Number && (this.Pow as Number).Value != 1) {
                            sb.Append("^{");
                            sb.Append(this.Pow.Output(type));
                            sb.Append("}");
                        }

                        sb.Append("(");
                        for (int i = 0; i < this.param.Length; i++) {
                            if (i != 0) sb.Append(", ");
                            sb.Append(param[i].Output(type));
                        }
                        sb.Append(")");


                        return sb.ToString();
                    }
                case OutputType.String: {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(this.func.Name);
                        if (this.Pow is Number && (this.Pow as Number).Value != 1) {
                            sb.Append("^");
                            sb.Append(this.Pow.Output(type));
                        }
                        sb.Append("(");
                        for (int i = 0; i < this.param.Length; i++) {
                            if (i != 0) sb.Append(", ");
                            sb.Append(this.param[i].Output(type));
                        }
                        sb.Append(")");


                        return sb.ToString();
                    }
            }
            return base.Output(type);
        }
    }
}
