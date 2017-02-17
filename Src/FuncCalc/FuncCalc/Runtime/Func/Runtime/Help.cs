using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncCalc.Expression;

namespace FuncCalc.Runtime.Func.Runtime
{
    public class Help : IFunction
    {
        public override string Name
        {
            get
            {
                return "help";
            }
        }
        public override ExpressionType[] Parameter
        {
            get
            {
                return new[] { ExpressionType.String };
            }
        }
        public override ExpressionType ReturnType
        {
            get
            {
                return ExpressionType.Nothing;
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
            string name = parameters[0].Token.Text;

            if (!runtime.ContainsFunc(name)) {
                runtime.AddLogWay(string.Format(GetRes.Text("Res", "FuncNotfound"), name));
            }
            else {
                var func = runtime.GetFunc(name);
                StringBuilder prm = new StringBuilder();
                for (int i = 0; i < func.Parameter.Length; i++) {
                    if (i != 0) prm.Append(", ");
                    prm.Append(func.Parameter[i]);
                }

                runtime.AddLogWay(string.Format(GetRes.Text("Res", "FunctionName"), func.Name));
                runtime.AddLogWay(string.Format(GetRes.Text("Res", "Description"), func.Description));
                runtime.AddLogWay(string.Format(GetRes.Text("Res", "Parameters"),
                    prm));
                runtime.AddLogWay(string.Format(GetRes.Text("Res", "ReturnType"), func.ReturnType));
                runtime.AddLogWay(string.Format(GetRes.Text("Res", "Assembly"), func.GetType().Assembly.FullName));
                runtime.AddLogWay(string.Format(GetRes.Text("Res", "ClassFullname"), func.GetType().FullName));
            }

            return null;
        }
    }
}
