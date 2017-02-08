using FuncCalc.Exceptions;
using FuncCalc.Expression;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Runtime.Operator
{
    public class Scope : IOperator
    {
        public string Name
        {
            get
            {
                return "スコープ";
            }
        }
        public string Text
        {
            get
            {
                return ".";
            }
        }
        public bool RequireLeftParameter
        {
            get
            {
                return true;
            }
        }
        public bool RequireRightParameter
        {
            get
            {
                return true;
            }
        }
        public bool DoEvaledParam
        {
            get { return false; }
        }

        public INumber Execute(RuntimeData runtime, INumber left, INumber right) {

            // this.DoEvaledParamがfalseなので、MoreScope=falseのブロックが入っている
            // というわけでMoreScopeをtrueにする
            runtime.NowBlock.MoreScope = true;

            var l = left.Eval(runtime);

            if (l is IObject) {
                IObject obj = l as IObject;
                string name = null;
                if (right is Variable) name = (right as Variable).Name;
                else if (right is Member) name = (right as Member).Text;
                else if (right is Expression.String) name = (right as Expression.String).Token.Text;
                else throw new SyntaxException(string.Format("メンバースコープ演算子は文字列である必要があります。", right.Token), right);

                var mem = obj.GetMember(name);
                return mem as INumber;
            }

            throw new SyntaxException("メンバースコープ演算子の左辺はオブジェクトである必要があります。", left);
        }
    }
}
