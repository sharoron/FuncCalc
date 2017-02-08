using FuncCalc.Expression;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Runtime.Operator
{
    public class DecrementRight : IOperator
    {
        public string Name
        {
            get
            {
                return "デクリメント";
            }
        }
        public string Text
        {
            get
            {
                return "--";
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
                return false;
            }
        }
        public bool DoEvaledParam
        {
            get { return false; }
        }

        public INumber Execute(RuntimeData runtime, INumber left, INumber right) {
            var dt = left.Eval(runtime);
            runtime.Setting.Spec.Operations.Where(a => a.Key.GetType() == typeof(Substitution)).First().Key
            .Execute(runtime, left, left.Subtract(runtime, Number.New(1)));
            return dt;
        }
    }
}
