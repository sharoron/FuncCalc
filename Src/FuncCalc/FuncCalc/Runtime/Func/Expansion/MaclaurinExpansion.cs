using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Expression;
using FuncCalc.Exceptions;

namespace FuncCalc.Runtime.Func.Expansion
{
    public class MaclaurinExpansion : IFunction
    {
        public override string Name
        {
            get
            {
                return "maclaurin";
            }
        }
        public override string Description
        {
            get
            {
                return "指定された式を、指定した変数で、指定した回数マクローリン展開します。";
            }
        }
        public override ExpressionType[] Parameter
        {
            get { return new ExpressionType[] { ExpressionType.Number, ExpressionType.Pointer, ExpressionType.Formula }; }
        }
        public override ExpressionType ReturnType
        {
            get
            {
                return ExpressionType.Formula;
            }
        }
        public override bool DoEvaledParam
        {
            get
            {
                return false;
            }
        }

        public override INumber Execute(RuntimeData runtime, params INumber[] parameters) {
            Number time = parameters[0] as Number;
            INumber var = parameters[1];
            INumber f = parameters[2];
            Variable v = null;

            // timeのパラメータをチェックする
            if (time == null || (time as Number).Value <= 0)
                throw new RuntimeException("timeパラメータは正の整数である必要があります。", parameters[0]);
            // varのパラメータをチェックする
            if (var is Variable) v = var as Variable;
            else if (var is Member) v = (var as Member).GetVariable(runtime);
            else throw new RuntimeException("varパラメータは変数である必要があります。", parameters[1]);

            AdditionFormula res = new AdditionFormula() { DontSort = true };
            INumber formula = f;
            

            // f(0)を求める
            runtime.AddBlock(new BlockData() { MoreScope = false });
            runtime.SetVariable(runtime, v, Number.New(0));
            var f0 = formula.Eval(runtime);
            res.AddItem(f0);
            runtime.AddLogWay("MaclaurinWay1", v, f0);
            runtime.PopBlock();

            INumber den = Number.New(1);
            for (int i = 0; i < (time as Number).Value; i++) {

                // formulaを微分し、varに0を代入する
                // 要約: f(i + 1)'を求める
                runtime.AddBlock(new BlockData() { MoreScope = false });
                formula = formula.ExecuteDiff(runtime, v.Name);

                // f(i + 1)'(0)を求める
                runtime.SetVariable(runtime, v, Number.New(0));
                var diffF = formula.Eval(runtime);
                runtime.AddLogWay("MaclaurinWay2", Number.New(i + 1), v, diffF);

                // このタイミングでBlockを新しくする。じゃないとその先にあるvを0として評価してしまう
                runtime.PopBlock();
                runtime.AddBlock(new BlockData() { MoreScope = false });

                // f(i + 1)' v / (i + 1)! 
                res.AddItem(runtime, new Fraction(
                    den = den.Multiple(runtime, Number.New(i + 1)),
                    diffF.Multiple(runtime, v)));

                runtime.PopBlock();

            }

            runtime.AddLogWay("MaclaurinWay3", v, res);

            return res;
        }
    }
}
