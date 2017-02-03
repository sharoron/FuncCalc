using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncCalc.Interface;
using FuncCalc.Expression;

namespace FuncCalc.Lisp.Generators
{
    public class LispFormula : IGenerator
    {
        public override IExpression Create() {

            Formula f = new Formula();
            f.Token = this.Token;
            f.Items.AddRange(this.Exp);
            f.EndToken = this.EndToken;

            RPEFormula res = new RPEFormula(f);

            for (int i = 1; i < this.Exp.Count; i++) {
                res.Items.Add(this.Exp[i]);
            }

            for (int i = 2; i < this.Exp.Count; i++) {
                res.Items.Add(this.Exp[0]);
            }


            return res;
        }
    }
}
