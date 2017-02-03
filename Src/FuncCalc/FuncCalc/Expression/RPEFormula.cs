using FuncCalc.Interface;
using FuncCalc.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Expression
{
    public class RPEFormula : IExpression, IBlock, IEval, IFormula, IUnknownParameter
    {
        internal List<IExpression> items = null;
        private Formula rawFormula = null;

        public Token Token
        {
            get { return this.rawFormula.Items.First().Token; }
        }
        public Token EndToken
        {
            get { return this.rawFormula.Items.Last().Token; }
        }
        public Formula RawFormula
        {
            get { return this.rawFormula; }
        }
        public int Count
        {
            get { return this.items.Count; }
        }
        public int SortPriority {
            get { return 3000; }
        }

        private RPEFormula() : this(null) { }
        public RPEFormula(Formula rawFormula) {

            this.items = new List<IExpression>();
            this.rawFormula = rawFormula;
        }

        public void AddItem(RuntimeData runtime, IExpression exp) {
            this.items.Add(exp);
        }
        internal void Insert(int index, IExpression exp) {
            this.items.Insert(index, exp);
        }

        public INumber Eval(RuntimeData runtime) {

            BlockData data = runtime.NowBlock;


            for (int i = 0; i < this.Count; i++) {
                IExpression exp = this.Items[i];

                if (exp is Operator) {
                    INumber left = null, right = null;
                    Operator op = exp as Operator;

                    if (runtime.Setting.IsDebug) {
                        // Console.Clear();
                        OutputStackData(runtime, exp);
                    }

                    bool willEval = true;
                    // 後ろの方にEvalせずに実行する演算子がある場合、変数を評価しないようにする
                    for (int j = i+1; j < this.Count; j++) {
                        if (this.items[j] is Operator && 
                            !(this.items[j] as Operator).Evaluator.DoEvaledParam) {
                            willEval = false;
                            runtime.AddBlock(new BlockData() { MoreScope = false });
                            if (runtime.Setting.IsDebug)
                                Console.WriteLine("AddBlock[MoreScope=false] しました");
                            break;
                        }
                    }

                    if (op.Evaluator.RequireRightParameter) {
                        var r = data.Pop();
                        if (op.Evaluator.DoEvaledParam && willEval)
                            r = (r as IEval).Eval(runtime);
                        if (!op.Evaluator.DoEvaledParam && r is INumber) { }
                        else if (r is Variable || r is Member) { }
                        else if (r is Member)
                            r = (r as Member).GetVariable(runtime);
                        else {
                            r = (r as IEval).Eval(runtime); 
                        }
                        right = r as INumber;
                        if (op.Evaluator.DoEvaledParam && willEval)
                            right = right.Eval(runtime);
                    }
                    if (op.Evaluator.RequireLeftParameter) {
                        var l = data.Pop();
                        if (op.Evaluator.DoEvaledParam && willEval)
                            l = (l as IEval).Eval(runtime);
                        if(!op.Evaluator.DoEvaledParam && l is INumber) { }
                        else if (l is Variable) { }
                        else if (l is Member)
                            l = (l as Member).GetVariable(runtime);
                        else {
                            l = (l as IEval).Eval(runtime);
                        }
                        
                        left = l as INumber;
                        if (op.Evaluator.DoEvaledParam && willEval)
                            left = left.Eval(runtime);
                    }

                    var res = (exp as Operator).Evaluator.Execute(runtime, left as INumber, right as INumber);
                    if (res != null && runtime.Setting.DoOptimize)
                        res = res.Optimise(runtime);

                    if (!willEval)
                        runtime.PopBlock();

                    data.Push(res);

                    if (runtime.Setting.IsDebug) {
                        // Console.Clear();
                        Console.WriteLine("Eval RPE Formula : ({0})[{1}] {2} ({3})[{4}] => {5}[{6}]",
                            left != null ? left.ToString() : "null",
                            left != null ? left.GetType().Name : "",
                            (exp as Operator).Evaluator.Name,
                            right != null ? right.ToString() : "null",
                            right != null ? right.GetType().Name : "",
                            res != null ? res.ToString() : "null",
                            res != null ? res.GetType().Name : "");
                        OutputStackData(runtime, null);
                    }
                }
                else {
                    if (exp is IEval)
                        data.Push(exp as IEval);
                    else
                        throw new NotImplementedException();
                }

            }
            
            if (data.Count == 0)
                return null;
            else {
               var result = data.Pop() as INumber;
                if (result != null && runtime.Setting.DoOptimize)
                    return result.Optimise(runtime);
                return result;
            }
        }
        private void OutputStackData(RuntimeData runtime, IExpression exp) {
                Console.WriteLine("RPE Status :");
                Console.Write("Stack : ");
                foreach (var item in runtime.NowBlock.Stack.ToArray().Reverse())
                    Console.Write(string.Format("[{0}] ", item));
            if (exp != null) Console.Write(string.Format("[{0}]", exp));
                Console.WriteLine("\n");
                Console.ReadKey(true);
            runtime.OutputDebug();
        }

        public List<IExpression> Items
        {
            get { return this.items; }
        }
        public override string ToString() {
            StringBuilder sb = new StringBuilder("[RPE](");
            for (int i = 0; i < this.items.Count; i++) {
                if (i != 0) sb.Append(" ");
                if (this.items[i] is RPEFormula)
                    sb.Append((this.items[i] as RPEFormula).ToStringSimply());
                else
                    sb.Append(this.items[i].ToString());
            }
            sb.Append(")");
            return sb.ToString();
        }
        string ToStringSimply() {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this.items.Count; i++) {
                if (i != 0) sb.Append(" ");
                sb.Append(this.items[i].ToString());
            }
            return sb.ToString();
        }

        public void ExecuteAsParameter(RuntimeData runtime) {
            var res = this.Eval(runtime);
            runtime.NowBlock.Push(res);
        }

        
    }
}
