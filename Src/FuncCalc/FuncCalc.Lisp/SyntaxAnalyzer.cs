using FuncCalc.Exceptions;
using FuncCalc.Interface;
using FuncCalc.Lisp.Generators;
using FuncCalc.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuncCalc.Lisp
{
    public class SyntaxAnalyzer : ISyntaxAnalyzer
    {
        public Token[] Items { get; set; }
        public RuntimeSetting Setting { get; set; }
        
        public SyntaxAnalyzer() { }

        private List<Token> brackets = new List<Token>();
        private List<IGenerator> parents = new List<IGenerator>();
        private Stack<IGenerator> gens = new Stack<IGenerator>();

        public IExpression GetResult() {

            this.Initialize();

            for (int i = 0; i < this.Items.Length; i++) {
                NextToken(this.Items[i]);
            }
            if (parents.Count == 0) {
                if (gens.Count == 0)
                    throw new SyntaxException("構文解析した結果、何も返ってきませんでした。", this.Items?.First());

                parents.AddRange(gens);
            }

            if (parents.Count >= 2)
                throw new NotImplementedException("まだ複数の命令を同時に実行するのは実装していません。");

            return parents.First().Create();
        }
        private void Initialize() {
            this.brackets = new List<Token>();
            this.gens = new Stack<Lisp.IGenerator>();
            this.parents = new List<Lisp.IGenerator>();
        }
        private void NextToken(Token t) {
            if (t.Type == Analyzer.TokenType.Syntax) {

                // カッコ始まり
                if (this.Setting.Spec.StartBrackets
                    .Where(c => c.ToString() == t.Text).Count() >= 1) {
                    gens.Push(new EmptyGenerator() { Token = t });
                }
                // カッコ終わり
                else if (this.Setting.Spec.EndBrackets
                    .Where(c => c.ToString() == t.Text).Count() >= 1) {
                    EndSymbolicExp(t);
                }
                else
                    throw new SyntaxException(
                        string.Format("不正なトークン '{0}'.", t.Text),
                        t);

            }
            else {
                if (gens.Count == 0)
                    gens.Push(new EmptyGenerator());

                if (gens.Peek() is EmptyGenerator)
                    StartSymbolicExp(t);
                else
                    this.gens.Peek().Exp.Add(
                        this.TokenToExpression(t));
            }
        }
        private void StartSymbolicExp(Token t) {
            EmptyGenerator emp = null;
            if ((emp = this.gens.Pop() as EmptyGenerator) == null)
                throw new ArgumentException();

            if (t.Type == Analyzer.TokenType.Operation) {
                gens.Push(new LispFormula() { Token = emp.Token });
                gens.Peek().Exp.Add(
                    this.TokenToExpression(t));
                return;
            }
            if (t.Type == Analyzer.TokenType.Member || 
                t.Type == Analyzer.TokenType.String) {
                gens.Push(new LispFunction() { Token = emp.Token });
                gens.Peek().Exp.Add(TokenToExpression(t));
                return;
            }

            throw new SyntaxException(
                string.Format("不明なトークン '{0}' です。", t.Text), t);

        }
        private void EndSymbolicExp(Token t) {
            if (gens.Count == 0)
                throw new SyntaxException("カッコの数が多すぎます。", t);

            if (gens.Count == 1) {
                parents.Add(gens.Pop());
            }
            else {
                var res = gens.Pop().Create();
                gens.Peek().Exp.Add(res);
            }
        }
        internal IExpression TokenToExpression(Token t) {
            return FuncCalc.Analyzer.SyntaxAnalyzer.TokenToExpression(this.Setting, t);
        }


    }
}
