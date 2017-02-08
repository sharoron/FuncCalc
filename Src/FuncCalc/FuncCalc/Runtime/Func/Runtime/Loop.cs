using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncCalc.Expression;

namespace FuncCalc.Runtime.Func.Runtime
{
    public class Loop : IFunction
    {
        public override string Name
        {
            get
            {
                return "loop";
            }
        }
        public override string Description
        {
            get
            {
                return "#0が0になるまで#1を実行します。";
            }
        }
        public override ExpressionType[] Parameter
        {
            get
            {
                return new[] { ExpressionType.Formula, ExpressionType.Formula };
            }
        }
        public override ExpressionType ReturnType
        {
            get
            {
                return ExpressionType.Anything;
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

            INumber res = null;
            for (;;) {

                var cond = parameters[0].FinalEval(runtime);
                if (cond.Equals(runtime, Number.New(0)))
                    break;

                res = parameters[1].Eval(runtime);

            }

            return res;
        }
    }
}
