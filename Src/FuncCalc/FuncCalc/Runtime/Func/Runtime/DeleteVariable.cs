using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Expression;
using FuncCalc.Exceptions;

namespace FuncCalc.Runtime.Func
{
    public class DeleteVariable : IFunction
    {
        public override string Name
        {
            get
            {
                return "delvar";
            }
        }
        public override string Description
        {
            get
            {
                return "変数を削除します";
            }
        }
        public override ExpressionType[] Parameter
        {
            get { return new ExpressionType[] { ExpressionType.Pointer }; }
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

            if (parameters[0] is Variable || parameters[0] is Member) {
                Variable v = parameters[0] as Variable;
                if (v == null) v = new Variable((parameters[0] as Member).Token);
                runtime.SetVariable(runtime, v, null);
            }

            return null;
        }
    }
}
