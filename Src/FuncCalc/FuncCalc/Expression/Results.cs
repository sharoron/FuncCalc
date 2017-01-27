using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Interface;

namespace FuncCalc.Expression {
    public class Results: INumber {
        internal List<INumber> items = new List<INumber>();
        
        public override ExpressionType Type {
            get { return ExpressionType.Anything; }
        }
        public override ValueType ValueType {
            get { return Expression.ValueType.Unknown; }
        }
        public override int SortPriority {
            get { return 300; }
        }
        public INumber[] Items {
            get { return this.items.ToArray(); }
        }


        public override INumber Add(Runtime.RuntimeData runtime, INumber val) {
            throw new NotImplementedException();
        }
        public override INumber Multiple(Runtime.RuntimeData runtime, INumber val) {
            throw new NotImplementedException();
        }

        public override bool Equals(Runtime.RuntimeData runtime, INumber val) {
            throw new NotImplementedException();
        }
        public override bool CanJoin(Runtime.RuntimeData runtime, INumber val) {
            throw new NotImplementedException();
        }
        public override INumber ExecuteDiff(Runtime.RuntimeData runtime, string t) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            for (int i = 0; i < this.items.Count; i++) {
                if (i != 0) sb.Append(", ");
                sb.Append(this.items[i].ToString());
            }
            sb.Append("}");
            return sb.ToString();
        }
        public override string Output(Runtime.OutputType type) {
            switch (type) {
                case FuncCalc.Runtime.OutputType.String:
                    return this.ToString();
                case FuncCalc.Runtime.OutputType.Mathjax: {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("\\{");
                        for (int i = 0; i < this.items.Count; i++) {
                            if (i != 0) sb.Append(", ");
                            sb.Append(this.items[i].Output(type));
                        }
                        sb.Append("\\}");
                        return sb.ToString();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
