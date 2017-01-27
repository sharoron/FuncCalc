using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Expression;
using FuncCalc.Runtime.Func.Runtime;

namespace FuncCalc.Runtime.Func
{
    public class Evaluation : IFunction
    {
        public override string Name
        {
            get
            {
                return "eval";
            }
        }
        public override string Description
        {
            get
            {
                return "式を評価します。";
            }
        }
        public override ExpressionType[] Parameter
        {
            get { return new ExpressionType[] { ExpressionType.Anything }; }
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
            var param = parameters[0];
            if (param is Variable || param is Member) {
                string name = "";
                if (param is Variable) name = (param as Variable).Name;
                if (param is Member) name = (param as Member).Text;

                if (runtime.ContainsFunc(name)) {
                    var func = runtime.GetFunc(param, false);
                    if (func is UserDefineFunction) {
                        return (func as UserDefineFunction).Formula as INumber;
                    }
                }
            }


            return parameters[0].Eval(runtime);
        }
    }
}
