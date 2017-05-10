using FuncCalc.Expression;
using FuncCalc.Interface;
using FuncCalc.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuncCalc.Analyzer
{
    public partial class SyntaxAnalyzer
    {


        public class ScopeAnalyze
        {
            private ScopeAnalyze(IFormula f, RuntimeSetting setting) {
                this.Setting = setting;
                this.Formula = f;
            }

            public RuntimeSetting Setting { get; set; }
            public IFormula Formula { get; set; }

            public void Start() {

                IExpression num = null;
                Expression.ScopeFormula scope = null;

                for (int i = 0; i < this.Formula.Items.Count; i++) {
                    var item = this.Formula.Items[i];
                    
                    if (item is Operator && 
                        item.Token.Text == this.Setting.Spec.ScopeOperator) {
                        if (scope == null) {
                            scope = new Expression.ScopeFormula();
                            this.Formula.Items.Insert(i - 1, scope);
                            this.Formula.Items.RemoveAt(i);
                            scope.AddItem(null, num);
                        }
                        scope.AddItem(null, item);
                        this.Formula.Items.RemoveAt(i);
                        i--;
                        continue;
                    }
                    if (scope == null) {
                        num = item;
                    } else {
                        if (!(item is Member || item is Variable || item is Expression.String)) {
                            scope = null;
                        } else {
                            this.Formula.Items.RemoveAt(i);
                            scope.AddItem(null, item);
                            i--;
                        }
                    }
                    
                }


            }

            public static IFormula Convert(IFormula f, RuntimeSetting setting) {

                ScopeAnalyze sa = new SyntaxAnalyzer.ScopeAnalyze(f, setting);

                sa.Start();

                return sa.Formula;

            }

        }
    }
}