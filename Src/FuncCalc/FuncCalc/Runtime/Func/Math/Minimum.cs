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
    public class Minimum : IFunction
    {
        public override string Name
        {
            get
            {
                return "min";
            }
        }
        public override string Description
        {
            get
            {
                return "指定された値のうち、小さい方を返します。";
            }
        }
        public override ExpressionType[] Parameter
        {
            get { return new ExpressionType[] { ExpressionType.Number, ExpressionType.Number }; }
        }
        public override ExpressionType ReturnType
        {
            get
            {
                return ExpressionType.Number;
            }
        }
        public override INumber Execute(RuntimeData runtime, params INumber[] parameters) {
            var l = parameters[0].FinalEval(runtime);
            var r = parameters[1].FinalEval(runtime);

            if (!(l is IConstParameter && r is IConstParameter))
                throw new RuntimeException("パラメータは値である必要があります。",
                    l is IConstParameter ? r : l);

            if ((l as IConstParameter).ConstValue > (r as IConstParameter).ConstValue)
                return parameters[1];
            else
                return parameters[0];

        }
    }
}
