using FuncCalc.Exceptions;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Runtime {
    public class RuntimeSetting {

        private Spec spec = new Spec();
        private bool doOptimize = true;
        private bool isDebug = false;
        private List<IOptimizer> optimizers = new List<IOptimizer>();

        public RuntimeSetting() {
            this.Initialize();
        }

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

        private void Initialize() {
            this.LoadOptimizer();
        }
        private void LoadOptimizer() {
            System.Reflection.Assembly asm = typeof(RuntimeData).Assembly;

            foreach (Type t in asm.GetTypes()) {
                if (t.IsClass && t.IsPublic && !t.IsAbstract &&
                    t.IsSubclassOf(typeof(IOptimizer))) {

                    var func = asm.CreateInstance(t.FullName) as IOptimizer;
                    this.optimizers.Add(func);
                }
            }
        }
    }
}
