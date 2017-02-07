using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Expression;
using FuncCalc.Interface;
using FuncCalc.Exceptions;
using FuncCalc.Runtime;

namespace FuncCalc.Analyzer {

    /// <summary>
    ///  通常の数式(Formula)を逆ポーランド記法の式(RPEFormula)に変換します
    /// </summary>
    public partial class SyntaxAnalyzer :ISyntaxAnalyzer {


        private Formula result = null;
        private IExpression finalResult = null;
        private int index = 0;
        private Stack<Formula> formulas = null;

        private Token[] items = null;
        private Runtime.RuntimeSetting setting;

        public SyntaxAnalyzer() { }
        public SyntaxAnalyzer(Token[] tokens, Runtime.RuntimeSetting setting) {
            this.items = tokens;
            this.setting = setting;
        }

        public Token[] Items {
            get { return this.items; }
            set { this.items = value; }
        }
        public Runtime.RuntimeSetting Setting {
            get { return this.setting; }
            set { this.setting = value; }
        }


        public IExpression GetResult() {
            this.Initialize();

            for (this.index = 0; this.index < items.Length; this.index++) {
                Step();
            }

            this.finalResult = this.result;
            if (this.setting.DoOptimize)
                this.Normalization();

            return this.finalResult;
        }
        private void Initialize() {
            this.result = new Formula();
            
            this.formulas = new Stack<Formula>();
            this.index = 0;


            this.formulas.Push(this.result);

        }
        private void Step() {

            Token t = this.items[this.index];

            if (t.Type == TokenType.Syntax) {

                if (this.setting.Spec.StartBrackets.Contains(t.Text[0])) {
                    Formula f = new Expression.Formula() { Token = t };
                    this.formulas.Peek().AddItem(null, f);
                    this.formulas.Push(f);
                }
                else if (this.setting.Spec.EndBrackets.Contains(t.Text[0])) {
                    IExpression f = this.formulas.Pop();
                    if (this.setting.DoOptimize) {
                        f = this.Normalization(f as Formula);
                        this.formulas.Peek().RemoveAt(this.formulas.Peek().Count - 1);
                        this.formulas.Peek().AddItem(null, f);
                    }
                }
                else if (t.Text == ";" || t.Text == ",") {
                    this.formulas.Peek().AddItem(null, new LineBreak(t));
                }
                else
                    throw new NotImplementedException(string.Format("制御文字 '{0}'を解析することができませんでした。", t.Text));

                return;
            }


            this.formulas.Peek().AddItem(null, TokenToExpression(t));

        }
        private void Normalization() {

            this.finalResult = this.Normalization(this.finalResult as Formula) as IExpression;

        }
        private IFormula Normalization(Formula f) {

            // メンバーを一括りにまとめる
            IFormula result = f;

            result = ConvertToFunctionFormula.Convert(result as Formula, this.setting);

            result = ConvertMFormula.Convert(result as Formula, this.setting);

            // 式を逆ポーランド記法に変換する
            if (result.Count >= 2) {
                result =
                    ConvertToRPEFormula.ConvertToRPE(result as Formula, this.setting);
            }

            return result;
        }
        private IExpression TokenToExpression(Token t) {
            return TokenToExpression(this.setting, t);
        }
        public static IExpression TokenToExpression(RuntimeSetting setting, Token t) {

            switch (t.Type) {
                case TokenType.None:
                    throw new SyntaxException(string.Format("'{0}'のキーワードの役割を解析することができませんでした。", t.Text), t);
                case TokenType.Operation:

                    var eval = setting.GetOperator(t.Text, t, true);

                    return new Operator(t, eval);
                case TokenType.Number:
                    return Number.New(setting, t);
                case TokenType.String:
                    return new Expression.String(t);
                case TokenType.Member:
                    return new Member(t);
                case TokenType.Syntax:
                    throw new NotImplementedException("/Analyzer/SyntaxAnalyzer : 未対応の制御文字です");
                case TokenType.Unknown:
                    throw new SyntaxException(string.Format("'{0}'のキーワードの役割を解析することができませんでした。", t.Text), t);
                default:
                    throw new NotImplementedException("/Analyzer/SyntaxAnalyzer : 未対応のトークンタイプです");
            }

        }
        





    }

}
