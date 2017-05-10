using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncCalc.Runtime;
using FuncCalc.Exceptions;
using System.Reflection;

namespace FuncCalc.Expression
{
    public class DynamicObject : IObject
    {
        private Dictionary<string, INumber> _variable = new Dictionary<string, INumber>();
        private List<IMember> _mem = new List<IMember>();
        private bool _acceptNotfoundMember = true;

        public DynamicObject() {
            Initialize();
        }
        private void Initialize() {
            foreach (var mem in 
                this.GetType().GetMembers()
                .Where(m=> {
                    return m.GetCustomAttributes(typeof(FuncCalc.Attributes.FuncCalcAttribute), true).Length > 0;
                })) {

                if (this._mem.Where(m=> m.Name == mem.Name).Count() > 0)
                    throw new FuncCalcException(
                        string.Format("'{0}' 初期化中にエラーが発生しました。FuncCalc.DynamicObjectはオーバーロードメンバーを含むことはできません。", 
                        this.GetType().FullName), this);

                this._mem.Add(new Runtime.FuncCalcMemberAccessor(this,
                    new MemberInfo[] { mem }));

            }
        }


        public override List<IMember> Members
        {
            get
            {
                return this._mem;
            }
        }
        public INumber this[string name]
        {
            get
            {
                if (this._variable.ContainsKey(name))
                    return this._variable[name];
                else
                    return null;
            }
            set
            {
                if (this._variable.ContainsKey(name))
                    this._variable[name] = value;
                else {
                    this._variable.Add(name, value);

                    DynamicObjectProperty prop = new DynamicObjectProperty(this, name);
                    this._mem.Add(prop);
                }
            }
        }
        public bool AcceptNotfoundMember
        {
            get { return this._acceptNotfoundMember; }
            protected internal set { this._acceptNotfoundMember = value; }
        }

        #region INumber 

        public override bool InfinitelyDifferentiable
        {
            get
            {
                return false;
            }
        }
        public override int SortPriority
        {
            get
            {
                return -1;
            }
        }
        public override ExpressionType Type
        {
            get
            {
                return ExpressionType.Object;
            }
        }
        public override ValueType ValueType
        {
            get
            {
                return ValueType.Unknown;
            }
        }
        public override INumber Add(RuntimeData runtime, INumber val) {
            if (val is Number && (val as Number).Value == 0)
                return this;

            throw new RuntimeException("dynamicオブジェクトに対して計算をすることはできません。", this);
        }
        public override INumber Differentiate(RuntimeData runtime, DifferentialData ddata) {
            throw new RuntimeException("dynamicオブジェクトに対して計算をすることはできません。", this);
        }
        public override INumber Multiple(RuntimeData runtime, INumber val) {
            if (val is Number && (val as Number).Value == 1)
                return this;

            throw new RuntimeException("dynamicオブジェクトに対して計算をすることはできません。", this);
        }
        public override bool CanJoin(RuntimeData runtime, INumber val) {
            return false;
        }
        public override bool Equals(RuntimeData runtime, INumber val) {
            return this == val;
        }
        public override INumber Power(RuntimeData runtime, INumber val) {
            if (val is Number && (val as Number).Value == 1)
                return this;

            return base.Power(runtime, val);
        }

        #endregion

        public override INumber Get(RuntimeData runtime, string key) {

            if (this._variable.ContainsKey(key))
                return this._variable[key];
            if (this._acceptNotfoundMember)
                return null;

            throw new RuntimeException(string.Format("'{0}'メンバーは見つかりませんでした。", key), this);
        }
        public override void Set(RuntimeData runtime, string key, INumber value) {

            var val = value;

            if (this._variable.ContainsKey(key))
                this._variable[key] = val;
            else {

                if (!this.AcceptNotfoundMember)
                    throw new RuntimeException(
                        string.Format("'{0}'は見つかりませんでした。", key, this));

                DynamicObjectProperty prop = new DynamicObjectProperty(this, key);
                this._mem.Add(prop);
                this._variable.Add(key, val);
            }

        }
        public override IMember GetMember(string name) {
            if (!this._acceptNotfoundMember) {
                var mems = this.Members.Where(mem => {
                    return mem.Name == name;
                });
                if (mems.Count() == 0)
                    throw new RuntimeException(string.Format("'{0}'は見つかりませんでした。", name), this);

                List<MemberInfo> minf = new List<MemberInfo>();
                foreach (var item in mems) {
                    minf.AddRange((item as FuncCalcMemberAccessor).MInfo);
                }

                return new FuncCalcMemberAccessor(this, minf.ToArray());
            }

            return new DynamicObjectProperty(this, name);
        }
        public override INumber Execute(RuntimeData runtime, string key, IObject obj, params INumber[] parameters) {
            throw new NotImplementedException("DynamicObjectメンバーの実行はまだ未実装です。");
        }

        public override string Output(OutputType type) {
            StringBuilder sb = new StringBuilder();
            sb.Append("{ ");

            bool flag = false;
            foreach (var item in this._variable.OrderBy(m => m.Key)) {
                if (flag) {
                    sb.Append(", ");
                }
                flag = true;
                sb.Append(string.Format("{0}: {1}", item.Key, item.Value.Output(type)));
            }

            sb.Append(" }");
            return sb.ToString();
        }
        public override string ToString() {
            return this.Output(OutputType.String);
        }
    }
}
