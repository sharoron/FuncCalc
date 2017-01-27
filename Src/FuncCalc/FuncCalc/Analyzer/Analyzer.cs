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
    public class Analyzer
    {
        private string str = null;
        private RuntimeSetting setting = null;
        private bool doOptimize = true;
        private Token[] tokens = null;
        private IFormula formula = null;
        
        private Analyzer() { }
        public Analyzer(string str) : this(str, new RuntimeSetting()) { }
        public Analyzer(string str, RuntimeSetting setting) {
            this.str = str;
            this.setting = setting;
        }

        public string Line
        {
            get { return this.str; }
        }
        public RuntimeSetting Setting
        {
            get { return this.setting; }
        }
        public bool DoOptimize
        {
            get { return this.doOptimize; }
            set { this.doOptimize = value; }
        }
        public Token[] Tokens
        {
            get { return this.tokens; }
        }
        public IFormula Formula
        {
            get { return this.formula; }
        }

        public IEval GetResult() {

            TokenAnaluzer ta = new FuncCalc.Analyzer.TokenAnaluzer(this.str, this.setting);
            this.tokens = ta.GetResult();

            SyntaxAnalyzer sa = new FuncCalc.Analyzer.SyntaxAnalyzer(this.tokens, this.setting) {
                DoNormalize = this.doOptimize
            };
            this.formula = sa.GetResult() as IFormula;

            return this.formula;
        }
        
        public static IEval Compile(string str) {
            return Analyzer.Compile(str, new Runtime.RuntimeSetting());
        }
        public static IEval Compile(string str, RuntimeSetting setting) {
            Analyzer a = new FuncCalc.Analyzer.Analyzer(str, setting);
            return a.GetResult();
        }
    }
}
