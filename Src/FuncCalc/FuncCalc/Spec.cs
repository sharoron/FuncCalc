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
            { new Scope(), 10000 },             // .    スコープ演算子
            { new MinusValue(), 5000 },         // -    マイナスの値
            { new IncrementLeft(), 5000 },      // ++   インクリメント
            { new IncrementRight(), 5000 },     // ++   インクリメント
            { new DecrementLeft(), 5000 },      // --   デクリメント
            { new DecrementRight(), 5000 },     // --   デクリメント
            { new Diff(), 5000 },               // '    微分
            { new Power(), 4000 },              // ^    累乗
            { new Remaindation(), 3000 },       // %    余り
            { new Multiplication(), 2000 },     // *    乗算
            { new Division(), 2000 },           // /    除算
            { new Addition(), 1000 },           // +    加算
            { new Subtraction(), 1000 },        // -    減算
            { new And(), 750},                  // &    ビットごとの論理積
            { new Or(), 750},                   // |    ビットごとの論理和
            { new BitNot(), 750},               // ~    ビットごとの否定
            { new Not(), 750},                  // !    否定
            { new Lamda(), 500},                // ->   ラムダ式
            { new Substitution(), 200},         // =   変数への代入
            { new SubstitutionAdd(), 200},      // +=   加算して代入
            { new SubstitutionSubtraction(), 200},// -= 減算して代入
            { new SubstitutionMultiple(), 200}, // *=   乗算して代入
            { new SubstitutionDivite(), 200},   // /=   除算して代入
            { new SubstitutionMod(), 200},      // %=   余りを求めて代入
            { new SubstitutionAnd(), 200},      // &=   論理積を求めて代入
            { new SubstitutionOr(), 200},       // |=   論理和を求めて代入
            { new ConditionDefine(), 200},      // ?=    変数への代入
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
