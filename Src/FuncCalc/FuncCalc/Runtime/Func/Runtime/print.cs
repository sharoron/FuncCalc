using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncCalc.Expression;
using System.Diagnostics;

namespace FuncCalc.Runtime.Func.Runtime
{
    public class Print : IFunction
    {
        public override string Name
        {
            get
            {
                return "print";
            }
        }
        public override string Description
        {
            get
            {
                return "Loggerに記録します。";
            }
        }
        public override ExpressionType[] Parameter
        {
            get
            {
                return new[] { ExpressionType.Anything };
            }
        }
        public override ExpressionType ReturnType
        {
            get
            {
                return ExpressionType.Nothing;
            }
        }

        public override INumber Execute(RuntimeData runtime, params INumber[] parameters) {

            string str = parameters[0].Output(runtime.Setting.Logger.OutputType);
            runtime.Setting.Logger.AddInfo(str);
            
            return null;
        }
    }
}
