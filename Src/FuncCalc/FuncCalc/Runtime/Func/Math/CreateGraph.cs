using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncCalc.Expression;

namespace FuncCalc.Runtime.Func.Math
{
    public class CreateGraph : IFunction
    {

        public override string Name
        {
            get
            {
                return "graph";
            }
        }
        public override string Description
        {
            get
            {
                return "グラフを生成します。";
            }
        }
        public override ExpressionType[] Parameter
        {
            get
            {
                return new ExpressionType[] { };
            }
        }

        public override ExpressionType ReturnType
        {
            get
            {
                return ExpressionType.Object;
            }
        }

        public override INumber Execute(RuntimeData runtime, params INumber[] parameters) {
            return new Objects.Graph();
        }
    }
}
