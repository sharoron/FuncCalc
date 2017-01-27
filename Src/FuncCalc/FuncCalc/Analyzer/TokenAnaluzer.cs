using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Exceptions;

namespace FuncCalc.Analyzer {
    public class TokenAnaluzer {

        private string str = "";
        private TokenType type = TokenType.None;
        private string stock = "";
        private int index = 0;
        private int line = 0;
        private int number = 0;
        private List<Token> res = null;
        private List<Token> brackets = null;

        private Runtime.RuntimeSetting setting = null;

        private char[] operationChars = null;
        private char[] bracketChars = null;

        private TokenAnaluzer() : this(null, null) { }

        public TokenAnaluzer(string line, Runtime.RuntimeSetting setting) {
            this.str = line;
            this.setting = setting;
        }

        public string Line {
            get {
                return this.str;
            }
        }
        public Runtime.RuntimeSetting Setting {
            get { return this.setting; }
        }

        public Token[] GetResult() {
            this.Initialize();

            for (int i = 0; i < str.Length; i++) {
                this.Step();
            }

            if (!string.IsNullOrEmpty(this.stock)) {
                this.AddToken();
            }

            // エラーチェックする
            if (this.brackets.Count >= 1)
                throw new SyntaxException("対応するかっこが不足しています", this.brackets.Last());

            return res.ToArray();
        }

        private void Initialize() {
            this.res = new List<Token>();
            this.brackets = new List<Token>();
            this.index = 0;
            this.line = 1;
            this.number = 1;

            // 演算子に使われるcharを取り出す
            List<char> opChar = new List<char>();
            foreach (var op in this.setting.Spec.Operations) {
                var key = op.Key;
                for (int i = 0; i < key.Text.Length; i++) {
                    if (!opChar.Contains(key.Text[i]))
                        opChar.Add(key.Text[i]);
                }
            }
            this.operationChars = opChar.ToArray();

            List<char> brackets = new List<char>();
            brackets.AddRange(this.setting.Spec.StartBrackets);
            brackets.AddRange(this.setting.Spec.EndBrackets);
            this.bracketChars = brackets.ToArray();

        }
        private void Step() {

            TokenType t = TokenType.None;
            char c = this.str[this.index];

            // 制御文字(ソースコードとしての)
            if (c == '\n') {
                this.line++;
                this.number = 1;
                this.index++;
                return;
            }
            if (c == '\r' || c == '\n') {
                this.number++;
                this.index++;
                return;
            }
            if (c == ' ') {
                t = TokenType.Syntax;
                goto Finish;
            }

               

            // 演算子
            if (this.operationChars.Contains(c)) {
                t = TokenType.Operation;
                goto Finish;
            }
            
            // 数値
            if (this.type != TokenType.Member && ((c >= '0' && c <= '9') || c == '.')) {
                t = TokenType.Number;
                goto Finish;
            }

            // メンバー
            if ((c >= '0' && c <= '9') ||
                (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') ||
                c == '@' || c == '$') {

                t = TokenType.Member;
                goto Finish;
            }

            // 制御文関係
            {
                // かっこ
                if (this.bracketChars.Contains(c)) {
                    t = TokenType.Syntax;
                    goto Finish;
                }

                // 行の終了
                if (c == ';') {
                    t = TokenType.Syntax;
                    goto Finish;
                }

                if (c == ';' || c == ',') {
                    t = TokenType.Syntax;
                    goto Finish;
                }
            }


            throw new SyntaxException(string.Format("不正な文字です '{0}'", c),
                new Token(c.ToString(), TokenType.Unknown, this.line, this.number, this.index));



        Finish: // 処理終了
            {

                if (this.type == TokenType.None) {
                    this.type = t;
                }
                else if (this.type != t || t == TokenType.Syntax) {
                    AddToken();
                    this.type = t;
                }
                this.stock += c;

                this.index++;
                this.number++;
                return;
            }
        }
        private void AddToken() {
            this.stock = this.stock.Trim(' ', '\n', '\r', '\0');
            if (string.IsNullOrEmpty(this.stock))
                return;

            Token t = new Token(
                this.stock, this.type, this.line, this.number, this.index);
            this.res.Add(t);

            // かっこレベルを確認する
            if (this.type == TokenType.Syntax && this.stock.Length >= 1) {
                char c = this.stock[0];
                if (this.setting.Spec.StartBrackets.Contains(c)) {
                    brackets.Add(t);
                }
                if (this.setting.Spec.EndBrackets.Contains(this.stock[0])) {
                    if ((brackets[this.brackets.Count - 1].Text == "(" && c == ']') ||
                        (brackets[this.brackets.Count - 1].Text == "[" && c == ')'))
                        throw new SyntaxException(string.Format("'{0}'に対応するかっこがありません。",
                            this.brackets[this.brackets.Count - 1].Text), t);
                    brackets.RemoveAt(this.brackets.Count - 1);
                }
            }

            this.stock = "";
            this.type = TokenType.None;
        }
    }
}
