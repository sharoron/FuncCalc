using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Analyzer;

namespace FuncCalc
{
    public class Token
    {
        private string _text = "";
        private int _line = 0;
        private int _number = 0;
        private int _index = -1;
        private TokenType _type = TokenType.None;

        protected internal Token() : this(null, TokenType.Unknown, 0, 0, -1) { }
        public Token(string text) : this(text, TokenType.Runtime) { }
        public Token(string text, TokenType type, int line, int number, int index) {
            this._text = text;
            this._type = type;
            this._line = line;
            this._number = number;
            this._index = index;
        }
        public Token(string text, TokenType type) : this(text, type, 0, 0, -1) { }

        public string Text {
            get {
                return this._text;
            }
        }
        public int Line {
            get {
                return this._line;
            }
        }
        public int Number {
            get { return this._number; }
        }
        public int Index {
            get { return this._index; }
        }
        public TokenType Type {
            get { return this._type; }
        }

        public override string ToString() {
            return string.Format("Token '{0}'({1})", this.Text, this.Type.ToString());
        }
    }
}
