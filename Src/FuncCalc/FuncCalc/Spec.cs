using FuncCalc.Interface;
using FuncCalc.Runtime.Operator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc {
    public class Spec {

        internal Spec() { }

        private Dictionary<IOperator, int> operations = new Dictionary<IOperator, int>() {
            { new MinusValue(), 5000 },         // -    マイナスの値
            { new IncrementLeft(), 5000 },      // ++   インクリメント
            { new IncrementRight(), 5000 },     // ++   インクリメント
            { new DecrementLeft(), 5000 },      // --   デクリメント
            { new DecrementRight(), 5000 },     // --   デクリメント
            { new Diff(), 5000 },               // '    微分
            { new Power(), 4000 },              // ^
            { new Remaindation(), 3000 },       // %
            { new Multiplication(), 2000 },     // *
            { new Division(), 2000 },           // /
            { new Addition(), 1000 },           // +
            { new Subtraction(), 1000 },        // -
            { new Lamda(), 500},                // ->   ラムダ式
            { new Substitution(), 200},         // ?=   変数への代入
            { new ConditionDefine(), 200},      // =    変数への代入
            { new FuncSubstitution(), 100},     // :=   式の代入
            { new OutputNonEval(), 50 },        // ?    未評価式の出力
            { new OutputEval(), 50 },           // ??   評価式の出力
            { new OutputFinalEval(), 50 },      // ???  最終的な評価式の出力
        };
        private char[] startBrackets = new[] { '(', '[', '{' };
        private char[] endBrackets = new[] { ')', ']', '}' };
        private string scopeOperator = ".";


        public Dictionary<IOperator, int> Operations {
            get { return this.operations; }
        }
        public char[] StartBrackets {
            get { return this.startBrackets; }
        }
        public char[] EndBrackets {
            get { return this.endBrackets; }
        }
        public string ScopeOperator
        {
            get { return this.scopeOperator; }
        }

    }
}
