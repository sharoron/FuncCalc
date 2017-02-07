using FuncCalc.Exceptions;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Runtime {
    public class RuntimeSetting {

        private Spec spec = new Spec();
        private bool doOptimize = true;
        private bool isDebug = false;
        private bool _ignoreBracketLevel = true;
        private List<IOptimizer> optimizers = new List<IOptimizer>();
        private ILogger _logger = new ConsoleLogger();
        private Type _defaultSyntaxAnalyzer = typeof(Analyzer.SyntaxAnalyzer);
        private Dictionary<string, INumber> _constant = new Dictionary<string, INumber>();
        private int _acceptBitLength = 64;
        private List<KeyValuePair<string, IFunction>> funcs = new List<KeyValuePair<string, IFunction>>();

        // Initializer
        public RuntimeSetting() {
            this.Initialize();
        }

        // Properties
        public Spec Spec {
            get { return this.spec; }
        }
        public bool DoOptimize
        {
            get { return this.doOptimize; }
            set { this.doOptimize = value; }
        }
        public bool IsDebug
        {
            get { return this.isDebug; }
            set { this.isDebug = value; }
        }
        public ILogger Logger
        {
            get { return this._logger; }
            set { this._logger = value; }
        }
        public bool IgnoreBracketLevel
        {
            get { return this._ignoreBracketLevel; }
            set { this._ignoreBracketLevel = value; }
        }
        public Type DefaultSyntaxAnalyzer {
            get { return this._defaultSyntaxAnalyzer; }
            set { this._defaultSyntaxAnalyzer = value; }
        }
        public Dictionary<string, INumber> Constants
        {
            get { return this._constant; }
        }
        public List<KeyValuePair<string, IFunction>> Functions
        {
            get
            {
                return this.funcs;
            }
        }
        public int AcceptBitLength
        {
            get { return this._acceptBitLength; }
            set { this._acceptBitLength = value; }
        }

        // Public Methods
        public IOperator GetOperator(string op, Token t, bool throwExc) {
            foreach (var item in this.Spec.Operations) {
                if (item.Key.Text == op)
                    return item.Key;
            }
            if (throwExc)
                throw new SyntaxException(string.Format("演算子 '{0}' は見つかりませんでした。", op), t);
            else
                return null;
        }
        public RuntimeData CreateNewRuntimedata() {
            RuntimeData data = new Runtime.RuntimeData(this);
            return data;
        }
        public IExpression GetExpression(string formula) {
            return Analyzer.Analyzer.Compile(formula, this);
        }

        // Private Methods
        private void Initialize() {
            var me = typeof(RuntimeSetting).Assembly;

            this.LoadFunctions(me);
            this.LoadOptimizer(me);
            this.LoadConstant(me);
        }
        private void LoadFunctions(Assembly asm) {
            foreach (Type t in asm.GetTypes()) {
                if (t.IsClass && t.IsPublic && !t.IsAbstract &&
                    t.IsSubclassOf(typeof(IFunction))) {

                    var func = asm.CreateInstance(t.FullName) as IFunction;
                    this.funcs.Add(new KeyValuePair<string, IFunction>(func.Name, func));
                }
            }
        }
        private void LoadOptimizer(Assembly asm) {
            foreach (Type t in asm.GetTypes()) {
                if (t.IsClass && t.IsPublic && !t.IsAbstract &&
                    t.IsSubclassOf(typeof(IOptimizer))) {

                    var func = asm.CreateInstance(t.FullName) as IOptimizer;
                    this.optimizers.Add(func);
                }
            }
        }
        private void LoadConstant(Assembly asm) {

            foreach (Type t in asm.GetTypes()) {
                if (t.IsClass && t.IsPublic && !t.IsAbstract &&
                    t.GetInterface(typeof(IConstant).FullName) != null) {

                    var cst = asm.CreateInstance(t.FullName) as IConstant;
                    this._constant.Add(cst.Name, cst as INumber);
                }
            }
        }
    }
}
