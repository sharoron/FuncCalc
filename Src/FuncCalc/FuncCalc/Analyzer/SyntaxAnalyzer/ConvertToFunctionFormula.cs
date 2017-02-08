using FuncCalc.Expression;
using FuncCalc.Interface;
using FuncCalc.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Analyzer
{
    public partial class SyntaxAnalyzer
    {
        public class ConvertToFunctionFormula
        {
            private Formula input = null;
            private RuntimeSetting setting = null;

            private int index = 0;
            private FunctionFormula nowFormula = null;
            private bool flag = false;

            private ConvertToFunctionFormula() { }
            public ConvertToFunctionFormula(Formula input, RuntimeSetting setting) {
                this.input = input;
                this.setting = setting;
            }

            public Formula Convert() {
                if (this.input.Count == 1)
                    return this.input;

                this.Initialize();

                for (this.index = 0; this.index < this.input.Count; this.index++) {
                    Step();
                }
                SetOffFlag();

                return this.input;
            }
            private void Initialize() {
                this.index = 0;
                this.flag = false;
            }
            private void Step() {

                IExpression exp = this.input.Items[this.index];

                if (exp is Member) {
                    this.AddFuncFormula();
                    return;
                }
                //if (exp.Token != null &&
                //    exp.Token.Type == TokenType.Operation && 
                //    exp.Token.Text == this.setting.Spec.ScopeOperator) {

                //    this.AddFuncFormula();
                //    return;
                //}
                if (flag && exp is IFormula) {
                    this.AddFuncFormula();
                    return;
                }

                this.SetOffFlag();
            }
            private void AddFuncFormula() {
                if (!this.flag) {
                    this.flag = true;
                    this.nowFormula = new Expression.FunctionFormula();

                    this.nowFormula.AddItem(null, this.input.Items[this.index]);
                    this.input.RemoveAt(this.index);
                    this.input.Insert(this.index, this.nowFormula);

                }
                else {
                    this.nowFormula.AddItem(null, this.input.Items[this.index]);
                    this.input.RemoveAt(this.index);
                    this.index--;

                }
            }
            private void SetOffFlag() {
                if (this.flag) {
                    this.flag = false;

                    if (this.nowFormula.Count == 1) {
                        // nowFormulaの中に1つしかアイテムがなければ元に戻してあげる
                        // 変数部分とかよくここにくる
                        for (int i = 0; i < this.input.Count; i++) {
                            if (this.input.Items[i] == this.nowFormula) {
                                this.input.Insert(i, this.nowFormula.Items[0]);
                                this.input.RemoveAt(i + 1);
                                break;
                            }
                        }
                    }
                    this.nowFormula = null;
                }
            }

            public static Formula Convert(Formula input, RuntimeSetting setting) {
                ConvertToFunctionFormula ff = new SyntaxAnalyzer.ConvertToFunctionFormula(
                    input, setting);
                return ff.Convert();
            }
        }
    }
}
