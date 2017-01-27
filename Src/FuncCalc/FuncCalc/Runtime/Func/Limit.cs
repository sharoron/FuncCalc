using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Expression;
using FuncCalc.Exceptions;
using FuncCalc.Runtime.Operator;

namespace FuncCalc.Runtime.Func
{
    public class Limit : IFunction
    {
        public override string Name
        {
            get
            {
                return "lim";
            }
        }
        public override string Description
        {
            get
            {
                return "極限を求めます";
            }
        }
        public override ExpressionType[] Parameter
        {
            get { return new ExpressionType[] { ExpressionType.Unknown, ExpressionType.Unknown }; }
        }
        public override ExpressionType ReturnType
        {
            get
            {
                return ExpressionType.Unknown;
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

            runtime.AddBlock(new BlockData());
            var res = parameters[1].Eval(runtime);
            res = res.Optimise(runtime);

            parameters[0].Eval(runtime);
            res = res.Eval(runtime);

            runtime.PopBlock();

            return res;
        }
    }
}
