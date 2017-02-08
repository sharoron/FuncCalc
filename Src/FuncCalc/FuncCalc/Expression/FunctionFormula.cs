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
    /// <summary>
    /// メンバー+パラメータのまとまり
    /// </summary>
    public class FunctionFormula : INumber, IBlock, IExpression, IFormula
    {
        private List<IExpression> items = null;
        
        public FunctionFormula() {
            this.items = new List<Interface.IExpression>();
        }
        
        public List<IExpression> Items
        {
            get { return this.items; }
        }

        public override Token Token
        {
            get
            {
                return this.items.First().Token;
            }
        }
        public Token EndToken
        {
            get
            {
                return this.items.Last().Token;
            }
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
        public int Count
        {
            get { return this.items.Count; }
        }
        public override int SortPriority {
            get { return 3000; }
        }
        public override bool InfinitelyDifferentiable
        {
            get
            {
                return true;
            }
        }

        public override INumber Clone() {
            var res = this.MemberwiseClone() as FunctionFormula;
            res.items = new List<IExpression>();
            res.items.AddRange(this.items);
            return res;
        }
        public void AddItem(RuntimeData runtime, IExpression exp) {
            this.items.Add(exp);
        }

        public override INumber ExecuteOperator(RuntimeData runtime, IOperator op, INumber left, INumber right) {
            throw new NotImplementedException();
        }

        public override INumber Add(RuntimeData runtime, INumber val) {
            AdditionFormula sf = new Expression.AdditionFormula();
            sf.AddItem(runtime, this);
            sf.AddItem(runtime, val);

            return sf;
        }
        public override INumber Multiple(RuntimeData runtime, INumber val) {
            throw new NotImplementedException();
        }
        public override INumber Divide(RuntimeData runtime, INumber val) {
            throw new NotImplementedException();
        }
        public override bool Equals(RuntimeData runtime, INumber val) {
            return false;
        }
        public override INumber Eval(RuntimeData runtime) {

            // 1しかない場合は変数
            if (this.items.Count == 1) {
                if (!(this.items[0] is INumber))
                    throw new NotImplementedException("FunctionFormulaに値以外が紛れ込んでいます");
                return (this.items[0] as INumber).Eval(runtime);
            }

            bool nextFlag = false;
            IExpression func = null;
            INumber multiple = Number.New(1);

            for (int i = 0; i < this.items.Count; i++) {
                IExpression ex = this.items[i];

                if (ex is String) {
                    func = ex;
                    nextFlag = false;
                    continue;
                }
                
                if (ex is Member && 
                    ((nextFlag && func != null) || (!nextFlag && func == null))) {
                    func = FunctionFormula.GetMember(runtime, func, ex as Member);
                    if (func == null)
                        throw new SyntaxException(string.Format("'{0}'は見つかりませんでした", ex.Token.Text), ex.Token,
                            new KeyNotFoundException());
                    if (func is INumber && !(func is IEvalWithParameters))
                    {
                        multiple = multiple.Multiple(runtime, func as INumber);
                        func = null;
                    }
                    nextFlag = false;
                    continue;
                }
                if (ex is Operator && 
                    ex.Token.Text == runtime.Setting.Spec.ScopeOperator) {

                    nextFlag = true;
                    continue;
                }
                // パラメータ関係
                if ((ex is IFormula) && !nextFlag) {
                    if (func == null && multiple is IEvalWithParameters) {
                        func = multiple;
                        multiple = Number.New(1);
                    }

                    if (func != null)
                    {
                        runtime.AddBlock(new BlockData() { MoreScope = true });
                        runtime.NowBlock.Push(new LineBreak());

                        (ex as IFormula).ExecuteAsParameter(runtime);

                        IExpression res = null;
                        if (func is INumber && func is IEvalWithParameters)
                        {
                            List<INumber> prm = new List<INumber>();
                            for (; runtime.NowBlock.Stack.Length != 0; )
                            {
                                var it = runtime.NowBlock.Pop(); if (it is LineBreak) continue;
                                if (!(it is INumber)) throw new RuntimeException("インデックス指定で値型(INumber)以外のパラメータを指定することはできません。", it);

                                if (func is IFunction && 
                                    (func as IFunction).DoEvaledParam) it = (it as IEval).Eval(runtime);

                                prm.Insert(0, it as INumber);
                            }
                            res = (func as IEvalWithParameters).Execute(runtime, prm.ToArray()) as IExpression;
                        }
                        else
                            res = ExecuteFunc(runtime, func, multiple);

                        runtime.PopBlock();

                        if (res is INumber)
                        {
                            if (multiple is Number && (multiple as Number).Value != 1)
                                multiple = multiple.Multiple(runtime, res as INumber);
                            else
                                multiple = res as INumber;
                            func = null;
                        }
                        else if (res == null)
                        {
                            func = null;
                            multiple = null;
                        }
                        else
                        {
                            func = res;
                            throw new NotImplementedException("関数の戻り値は現在 '値' しか対応していません");
                        }

                        continue;
                    }
                    else
                    {
                        if (multiple.Equals(runtime, Number.New(1)))
                            return this; // 未解決のメンバーを実行しようとしている式は評価せずに返す
                        else
                        {
                            var res = (ex as IFormula).Eval(runtime);
                            multiple = multiple.Multiple(runtime, res);
                        }
                        continue;
                    }
                }

                throw new NotImplementedException();
            }

            if (func == null && multiple == null)
                return null;
            if (func == null)
                return multiple;
            if (!(func is INumber))
                throw new RuntimeException("現在、関数の戻り値は値のみに対応しています。関数ポインターを戻り値にすることはできません。", this);
            

            return (func as INumber).Eval(runtime);

        }
        public override INumber FinalEval(RuntimeData runtime) {
            return this.Eval(runtime).FinalEval(runtime);
        }
        public override bool CanJoin(RuntimeData runtime, INumber val) {
            return false;
        }
        private static IExpression ExecuteFunc(RuntimeData runtime, IExpression func, INumber multiple) {
            if (func == null) throw new ArgumentNullException("func");

            if (func is IFunction) {
                List<IExpression> param = new List<Interface.IExpression>();
                int count = runtime.NowBlock.Count;
                for (int i = 0; i < count; i++) {
                    IExpression ex = runtime.NowBlock.Pop();
                    if (ex is LineBreak) break;
                    param.Insert(0, ex);
                }
                List<ExpressionType> paramType = new List<Expression.ExpressionType>();
                for (int i = 0; i < param.Count; i++) {
                    IExpression ex = param[i];
                    if (ex is LineBreak) break;
                    if (ex is IConstParameter) {
                        paramType.Add(ExpressionType.Number);
                        continue;
                    }
                    else if (ex is IUnknownParameter ||
                        ex is Member || ex is Variable) {
                        paramType.Add(ExpressionType.Unknown);
                        continue;
                    }
                    else if (ex is AdditionFormula ||
                        ex is MultipleFormula) {
                        paramType.Add(ExpressionType.Formula);
                        continue;
                    }
                    else if (ex is Fraction || ex is FunctionFormula || 
                        ex is FuncedINumber) {
                        paramType.Add(ExpressionType.Unknown);
                        continue;
                    }

                    throw new NotImplementedException();
                }
                var function = runtime.GetFunc(func, true, paramType.ToArray());

                var res = function.Execute(runtime, param.ConvertAll<INumber>(a => {
                    if (function.DoEvaledParam)
                        return (a as IEval).Eval(runtime);
                    else if (a is INumber)
                        return a as INumber; 
                    else {
                        runtime.AddBlock(new Runtime.BlockData() { MoreScope = false });
                        var a_res = (a as IEval).Eval(runtime);
                        runtime.PopBlock();
                        return a_res;
                    }
                }).ToArray());
                if (runtime.Setting.IsDebug) {
                    StringBuilder sb = new StringBuilder("Func Eval Result : ");
                    sb.Append(function.Name);
                    sb.Append("(");
                    for (int i = 0; i < param.Count; i++) {
                        if (i != 0) sb.Append(", ");
                        sb.Append(param[i].ToString());
                    }
                    sb.Append(")");
                    sb.Append(" => " + res == null ? "" : res.ToString());
                    Console.WriteLine(sb.ToString());
                }
                return res;
            }

            if (func is IConstParameter) {
                return (func as INumber).Multiple(runtime, multiple);
            }

            throw new RuntimeException("関数と数値以外にパラメータを指定して実行することはできません", func);
        }
        private static IExpression GetMember(RuntimeData runtime, IExpression parent, Member searchToken) {

            // まずは変数を検索する
            if (parent == null) {
                if (runtime.ContainsKey(searchToken.Text))
                    return runtime.GetData(searchToken.Token);
            }

            // 親メンバーから検索
            if (parent != null) {
                throw new NotImplementedException();
            }

            // フィールドメンバーから検索
            if (parent == null) {
                if (runtime.ContainsFunc(searchToken.Text))
                    return runtime.GetFunc(searchToken, false);
            }

            // 不明なものはとりあえず変数と見立てる
            if (parent == null) {
                return new Variable(searchToken.Token);
            }

            throw new SyntaxException(string.Format("'{0}'は見つかりませんでした", searchToken.Text), 
                searchToken.Token, new KeyNotFoundException());

        }
        public void ExecuteAsParameter(RuntimeData runtime) {
            throw new RuntimeException("FunctionFormulaはパラメータとして実行することはできません", this);
        }
        public override INumber Integrate(RuntimeData runtime, string t) {
            return this.Eval(runtime).Integrate(runtime, t);
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            foreach (var item in this.items) {
                sb.Append(item.ToString());
            }
            return sb.ToString();
        }


        public override string Output(OutputType type) {
            switch (type) {
                case OutputType.Mathjax: {
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < this.items.Count; i++) {
                            if (this.items[i] is IOutput)
                                sb.Append((this.items[i] as IOutput).Output(type));
                            else
                                sb.Append(this.items[i].ToString());
                        }
                        return sb.ToString();
                    }
                    break;
            }
            return base.Output(type);
        }

        public override INumber ExecuteDiff(RuntimeData runtime, string t) {

            MultipleFormula mf = Runtime.Func.Differential.DiffPow(runtime, t, this);

            var res = this.Eval(runtime).ExecuteDiff(runtime, t);
            if (mf != null) {
                mf.AddItem(runtime, res);
                return mf;
            }
            else
                return res;

        }
    }
}
