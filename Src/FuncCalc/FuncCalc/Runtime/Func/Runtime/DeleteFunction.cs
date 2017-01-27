using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Expression;
using FuncCalc.Exceptions;
using FuncCalc.Runtime.Func.Runtime;

namespace FuncCalc.Runtime.Func
{
    public class DeleteFunction : IFunction
    {
        public override string Name
        {
            get
            {
                return "delfunc";
            }
        }
        public override string Description
        {
            get
            {
                return "ユーザー定義の関数を削除します";
            }
        }
        public override ExpressionType[] Parameter
        {
            get { return new ExpressionType[] { ExpressionType.Unknown }; }
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

                if (!runtime.Functions.ContainsKey(v.Name))
                    throw new RuntimeException(string.Format("関数 '{0}' は見つかりませんでした。", parameters[0]));
                if (!(runtime.GetFunc(v, false) is UserDefineFunction))
                    throw new RuntimeException("さdelfunc関数で削除できるのはユーザー定義関数のみです。", parameters[9]);

                runtime.Functions.Remove(v.Name);

            }

            return null;
        }
    }
}
