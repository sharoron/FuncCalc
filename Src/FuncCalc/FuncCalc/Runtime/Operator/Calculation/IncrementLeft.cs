using FuncCalc.Expression;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Runtime.Operator
{
    public class IncrementLeft : IOperator
    {
        public string Name
        {
            get
            {
                return "インクリメント";
            }
        }
        public string Text
        {
            get
            {
                return "++";
            }
        }
        public bool RequireLeftParameter
        {
            get
            {
                return false;
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
            return 
                runtime.Setting.Spec.Operations.Where(a => a.Key.GetType() == typeof(Substitution)).First().Key
                .Execute(runtime, right, right.Add(runtime, Number.New(1)));
            
        }
    }
}
