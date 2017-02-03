using FuncCalc.Interface;
using FuncCalc.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuncCalc.Lisp
{
    public abstract class IGenerator
    {
        private List<IExpression> _exp = new List<IExpression>();
        private SyntaxAnalyzer _parent = null;
        private Token _token = null;
        private Token _endtoken = null;

        public List<IExpression> Exp
        {
            get
            {
                return this._exp;
            }
        }
        public RuntimeSetting Setting
        {
            get; internal set;
        }
        public SyntaxAnalyzer Parent
        {
            get { return this._parent; }
            internal set { this._parent = value; }
        }
        public Token Token
        {
            get { return this._token; }
            internal set { this._token = value; }
        }
        public Token EndToken
        {
            get { return this._endtoken; }
            internal set { this._endtoken = value; }
        }

        public abstract IExpression Create();
    }
}
