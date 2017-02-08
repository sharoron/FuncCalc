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
    public class Lamda : IOperator
    {
        public string Name
        {
            get
            {
                return "ラムダ式";
            }
        }
        public string Text
        {
            get
            {
                return "=>";
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
            var r = right.Eval(runtime);

            // 旧コード: // 定義 
            // runtime.Setting.GetOperator("?=", null, true).Execute(runtime, left, r);

            string name = runtime.GetAnonymousId();
            List<string> prms = new List<string>();

            if (left is Formula) {
                var l = left as Formula;
                bool flag = true;
                for (int i = 0; i < l._items.Count; i++) {
                    var item = l._items[i];
                    if ( item is LineBreak) {
                        flag = true;
                        continue;
                    }

                    if (flag) {
                        flag = false;
                        if (item is Variable || item is Member)
                            prms.Add(item.Token.Text);
                        else {
                            throw new SyntaxException(
                                string.Format("パラメータに変数以外 '{0}' を含むことはできません。", item), item);
                        }
                    } else {
                        throw new SyntaxException(
                            string.Format("パラメータに変数以外 '{0}' を含むことはできません。", item), item);
                    }
                }
            }
            else if (left is Variable || left is Member) {
                prms.Add(left.Token.Text);
            }
            else
                throw new SyntaxException("左辺式が不正です。", left);


            Runtime.Func.Runtime.UserDefineFunction ud = 
                new Runtime.Func.Runtime.UserDefineFunction(name, right, prms.ToArray());


            return ud;
        }
    }
}
