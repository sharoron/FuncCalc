using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Exceptions;
using FuncCalc.Expression;
using FuncCalc.Interface;

namespace FuncCalc.Runtime.Func.Runtime {
    public class UserDefineFunction : IFunction {

        private string name = "#userDefine";
        private string description = "ユーザー定義の関数。':=' 演算子を使用して作成します";
        private string[] _paramName = null;
        private List<ExpressionType> paramType = new List<ExpressionType>();
        private ExpressionType returnType = ExpressionType.Unknown;
        private IEval formula = null;

        public UserDefineFunction() {
        }
        public UserDefineFunction(string funcName, IEval f, string[] paramName) {
            this.name = funcName;
            this.formula = f;
            this.description = "";

            this._paramName = paramName;
            for (int i = 0; i < paramName.Length; i++) {
                this.paramType.Add(ExpressionType.Unknown);
            }
        }

        public override string Name {
            get { return this.name; }
        }
        public override string Description {
            get {
                return this.description;
            }
        }
        public IEval Formula {
            get { return this.formula; }
        }

        public string[] ParameterName {
            get { return _paramName; }
        }
        public override Expression.ExpressionType[] Parameter {
            get { return paramType.ToArray(); }
        }
        public override Expression.ExpressionType ReturnType {
            get { return returnType; }
        }
        public override INumber Execute(RuntimeData runtime, params INumber[] parameters) {

            if (this.name == "#userDefine") {
                throw new RuntimeException("この関数をそのまま呼び出すことはできません。" );
            }


            runtime.AddBlock(new BlockData());

            for (int i = 0; i < parameters.Length; i++) {
                var exp = parameters[i];
                runtime.SetVariable(runtime, 
                    new Variable(new Token(this._paramName[i], Analyzer.TokenType.Member)),
                    exp.Eval(runtime));
            }
            var res = this.formula.Eval(runtime);

            runtime.PopBlock();

            return res;

        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.name);
            sb.Append("(");
            if (this._paramName != null)
                for (int i = 0; i < _paramName.Length; i++) {
                    if (i != 0) sb.Append(", ");
                    sb.Append(this._paramName[i]);
                }
            sb.Append(")");
            if (this.formula != null) {
                sb.Append(" => ");
                sb.Append(this.formula.ToString());
            }
            return sb.ToString();
        }
    }
}
