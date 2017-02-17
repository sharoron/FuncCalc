using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Expression;
using FuncCalc.Interface;
using FuncCalc.Runtime;

namespace FuncCalc.Analyzer {

    public partial class SyntaxAnalyzer {
        public class ConvertMFormula {

            private bool converted = false;
            private Formula input = null;
            private int index = 0;
            private RuntimeSetting setting = null;
            private Operator op = null;


            public ConvertMFormula(Formula f, RuntimeSetting setting) {
                this.input = f;
                this.setting = setting;
            }
            
            public static Formula Convert(Formula f, RuntimeSetting setting) {
                ConvertMFormula c = new ConvertMFormula(f, setting);
                return c.Convert();
            }

            public Formula Convert() {
                if (converted) return this.input;
                this.Initialize();

                for (this.index = 0; this.index < this.input._items.Count; this.index++)
                    this.Step();


                converted = true;
                return this.input;
            }
            private void Initialize() {
                this.op =
                    new Expression.Operator(new FuncCalc.Token("*", TokenType.Operation), setting.Spec.Operations.Where(k => k.Key.Text == "*").Last().Key);
            }
            private void Step() {
                var exp = this.input._items[this.index];

                
                if (!((exp is Operator) || 
                    exp is LineBreak)) {
                    if (this.index  < this.input._items.Count - 1) {
                        var next = this.input._items[this.index + 1];
                        if (!(next is Operator || next is LineBreak)) {
                            this.input._items.Insert(++this.index,
                                this.op);
                        }
                    }
                }
                


            }


        }
    }
}
