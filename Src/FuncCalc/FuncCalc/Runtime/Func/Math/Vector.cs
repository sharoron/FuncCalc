using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncCalc.Expression;

namespace FuncCalc.Runtime.Func.Math
{
    public class Vector : IFunction
    {
        public override string Name
        {
            get
            {
                return "vector";
            }
        }
        public override string Description
        {
            get
            {
                return "ベクトルオブジェクトを生成する";
            }
        }

        public override ExpressionType[] Parameter
        {
            get
            {
                return new[] { ExpressionType.Anything, ExpressionType.Anything };
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
                return true;
            }
        }

        public override INumber Execute(RuntimeData runtime, params INumber[] parameters) {
            return new Expression.Vector(parameters[0], parameters[1]);
        }
    }
}
