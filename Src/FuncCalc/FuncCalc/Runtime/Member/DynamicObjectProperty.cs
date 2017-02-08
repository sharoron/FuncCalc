using FuncCalc.Expression;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuncCalc.Runtime
{
    public class DynamicObjectProperty : IMember
    {
        private DynamicObject baseObj = null;
        private string name = "";


        private DynamicObjectProperty() { }
        public DynamicObjectProperty(DynamicObject parent, string name) {
            this.baseObj = parent;
            this.name = name;
        }

        public override string Name
        {
            get
            {
                return this.name;
            }
        }
        public DynamicObject ParentObject
        {
            get { return this.baseObj; }
        }
        public override INumber Get(RuntimeData runtime) {
            return this.baseObj.Get(runtime, this.name);
        }
        public override void Set(RuntimeData runtime, INumber value) {
            this.baseObj.Set(runtime, this.name, value);
        }
        public override INumber Eval(RuntimeData runtime) {
            return this.Get(runtime);
        }

        public override bool Equals(RuntimeData runtime, INumber val) {
            return this == val;
        }

        public override string Output(OutputType type) {
            return string.Format("{0},{1}", this.baseObj.Token, this.name);
        }
    }
}
