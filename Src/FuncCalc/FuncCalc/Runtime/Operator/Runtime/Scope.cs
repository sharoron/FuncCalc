using FuncCalc.Attributes;
using FuncCalc.Exceptions;
using FuncCalc.Expression;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            string name = null;
            if (right is Variable) name = (right as Variable).Name;
            else if (right is Expression.Member) name = (right as Expression.Member).Text;
            else if (right is Expression.String) name = (right as Expression.String).Token.Text;
            else if (right is FunctionFormula) {
                var ff = right as FunctionFormula;

                runtime.AddBlock(new BlockData() {
                    Parent = left,
                    MoreScope = true,
                });
                var res = ff.Eval(runtime);
                runtime.PopBlock();

                return res;
            }
            else throw new SyntaxException(string.Format("メンバースコープ演算子は文字列である必要があります。", right.Token), right);

            if (l is IObject) {
                IObject obj = l as IObject;

                var mem = obj.GetMember(name);
                // return mem.Get(runtime);
                return mem;
            } else {

                // FuncCalcMethodAttribute属性のあるメソッドへスコープ
                var mems = l.GetType().GetMember(name)
                    .Where((mem)=> {
                        if (mem.MemberType == System.Reflection.MemberTypes.Method) {
                            if ((mem as MethodInfo).GetCustomAttributes(typeof(FuncCalcAttribute), true) != null)
                                return true;
                        }
                        return false;
                    });
                if (mems.Count() == 0)
                    throw new RuntimeException(
                        string.Format("'{0}'に'{1}'は含まれていませんでした。",
                        l.GetType().FullName, name), right);

                return new FuncCalcMemberAccessor(
                    l, mems.ToArray());


            }

            throw new SyntaxException("メンバースコープ演算子の左辺はオブジェクトである必要があります。", left);
        }



    }
}
