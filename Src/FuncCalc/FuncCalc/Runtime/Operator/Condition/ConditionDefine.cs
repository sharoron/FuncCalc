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
    public class ConditionDefine : IOperator
    {
        public string Name
        {
            get
            {
                return "定義";
            }
        }
        public string Text
        {
            get
            {
                return "?=";
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
            get { return true; }
        }

        public INumber Execute(RuntimeData runtime, INumber left, INumber right) {

            // 両辺に含まれる変数
            List<Variable> vars = new List<Expression.Variable>();

            //両辺に含まれる変数をチェックする
            Action<Variable> addVar = new Action<Variable>(v => {
                foreach (var va in vars) if (va.Name == v.Name) return;
                vars.Add(v);
            });
            Action<INumber> checkFormula = null; checkFormula = new Action<INumber>((formula) => {
                if (formula is Variable) addVar(formula as Variable);
                if (formula is Member) addVar((formula as Member).GetVariable(runtime));
                if (formula is IFormula) {
                    foreach (var item in (formula as IFormula).Items) {
                        if (item is INumber) checkFormula(item as INumber);
                    }
                }
            });
            checkFormula(left); checkFormula(right);

            foreach (var var in vars) {
                var res = CheckValue(var, left, right);

            }


            throw new NotImplementedException();
        }
        INumber CheckValue(Variable var, INumber left, INumber right) {

            // 左辺に変数を持ってくるように調整する
            INumber l = right.Clone();
            INumber r = right.Clone();

            // 左辺を右辺に持っていく
            throw new NotImplementedException();

        }
        // 方程式の解を求める
        INumber CheckEquation(Variable var, INumber formula) {
            throw new NotImplementedException();
        }
    }
}
