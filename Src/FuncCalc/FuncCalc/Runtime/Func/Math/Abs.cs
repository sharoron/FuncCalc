using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Expression;
using FuncCalc.Exceptions;
using FuncCalc.Interface.Math;

namespace FuncCalc.Runtime.Func.Math
{
    public class AbsoluteValue : IFunction
    {
        public override string Name
        {
            get
            {
                return "abs";
            }
        }
        public override string Description
        {
            get
            {
                return "絶対値を返します。";
            }
        }
        public override ExpressionType[] Parameter
        {
            get { return new ExpressionType[] { ExpressionType.Number }; }
        }
        public override ExpressionType ReturnType
        {
            get
            {
                return ExpressionType.Number;
            }
        }
        public override bool DoEvaledParam
        {
            get
            {
                return true;
            }
        }

        public override INumber Execute(RuntimeData runtime, params INumber[] parameters) {
            var prm = parameters[0];

            if (prm is IAbs)
                return (prm as IAbs).Abs(runtime);

            var func = runtime.GetFunc("abs");
            return new FuncedINumber(func, parameters);
        }
    }
}
