using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncCalc.Interface;
using FuncCalc.Expression;

namespace FuncCalc.Lisp.Generators
{
    public class LispFunction : IGenerator
    {
        public override IExpression Create() {

            FunctionFormula res = new FunctionFormula();
            res.Items.Add(this.Exp[0]);

            Formula prm = new Formula() { Token = this.Token, EndToken = this.EndToken };
            for (int i = 1; i < this.Exp.Count; i++) {
                if (i != 1)
                    prm.Items.Add(new LineBreak(new Token(",", Analyzer.TokenType.Syntax)));

                prm.Items.Add(this.Exp[i]);
            }
            res.Items.Add(prm);

            return res;
        }
    }
}
