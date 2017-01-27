using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Expression;
using FuncCalc.Exceptions;

namespace FuncCalc.Runtime.Func.Math
{
    public class Randam : IFunction
    {
        public override string Name
        {
            get
            {
                return "rand";
            }
        }
        public override string Description
        {
            get
            {
                return "指定された値が最大の0以上の整数を返します。";
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
        public override bool EnabledPow
        {
            get
            {
                return false;
            }
        }

        public override INumber Execute(RuntimeData runtime, params INumber[] parameters) {
            var l = parameters[0].FinalEval(runtime);

            if (!(l is IConstParameter))
                throw new RuntimeException("パラメータは値である必要があります。", l);

            return Number.New((new Random()).Next((int)(l as IConstParameter).ConstValue));
        }
    }
}
