﻿using FuncCalc.Exceptions;
using FuncCalc.Expression;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Runtime.Operator
{
    public class SubstitutionOr : IOperator
    {
        public string Name
        {
            get
            {
                return "論理和を求めて代入";
            }
        }
        public string Text
        {
            get
            {
                return "|=";
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

            var res = runtime.Setting.GetOperator("|", null, false)
                .Execute(runtime, left.Eval(runtime), right.Eval(runtime));

            if (left is IMember) {
                (left as IMember).Set(runtime, res);
                return right;
            }

            if (!(left is Variable) && !(left is Member) &&
                !(left is FunctionFormula)) {
                throw new SyntaxException("代入は変数にのみ行えます。", left);
            }

            return runtime.Setting.GetOperator("=", null, false).Execute(runtime, left, res);
        }
    }
}