using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncCalc.Expression;
using System.Reflection;
using FuncCalc.Exceptions;

namespace FuncCalc.Runtime
{
    public class FuncCalcMemberAccessor : IFunction
    {
        private INumber _obj = null;
        private MemberInfo[] _minfo = null;

        public FuncCalcMemberAccessor() { }
        public FuncCalcMemberAccessor(INumber obj, MemberInfo[] minfo) {
            this._obj = obj;
            this._minfo = minfo;
        }

        public override string Name
        {
            get
            {
                if (this._minfo == null || this._minfo.Length == 0)
                    return "#fcMemberAccessor";
                return this._minfo.First().Name;
            }
        }
        public override ExpressionType[] Parameter
        {
            get
            {
                List<ExpressionType> res = new List<ExpressionType>();
                var ms = (this._minfo)?.Where(m => m.MemberType == MemberTypes.Method);
                if (ms == null || ms.Count() == 0)
                    return new ExpressionType[] { };

                for (int i = 0; i < 
                    (ms.First() as MethodInfo).GetParameters().Length - 1; i++)
                    res.Add(ExpressionType.Unknown);
                return res.ToArray();
            }
        }
        public override ExpressionType ReturnType
        {
            get
            {
                return ExpressionType.Unknown;
            }
        }
        public MemberInfo[] MInfo
        {
            get { return this._minfo; }
        }

        public override INumber Execute(RuntimeData runtime, params INumber[] parameters) {

            List<object> prm = new List<object>();
            for (int i = 0; i < parameters.Length; i++) {
                prm.Add(new FCNumber(runtime, parameters[i]));
            }

            var mems = this._minfo.Where(
                m => {
                    if (!(m is MethodInfo)) return false;

                    var prD = (m as MethodInfo).GetParameters();
                    if (m.MemberType == MemberTypes.Method &&
                        (prD.Length == prm.Count() ||
                        (prD.Length != 0 && prD[0].ParameterType == typeof(RuntimeData) && prD.Length == 1 + prm.Count())))
                        return true;
                    return false;
                });
            if (mems.Count() == 0)
                throw new RuntimeException(string.Format("メソッド '{0}' は見つかりませんでした。", this._minfo.First().Name), this);
            if (mems.Count() >= 2)
                throw new RuntimeException(string.Format("メソッド '{0}' が複数個定義されています。どれを実行すればいいのかわかりません。", this._minfo.First().Name), this);

            var pr = (mems.First() as MethodInfo).GetParameters();
            if (prm.Count == 0 && pr.Length == 0) { }
            else if (pr[0].ParameterType == typeof(RuntimeData)) {
                prm.Insert(0, runtime);
            }
            else { }


            var res = (mems.First() as MethodInfo).Invoke(this._obj, prm.ToArray());

            if (res == null)
                return null;

            return ConvertToFC(res);
        }
        public override INumber Get(RuntimeData runtime) {

            if (this._minfo.Length >= 2)
                throw new RuntimeException(
                    string.Format("'{0}'の中に'{1}'は複数個定義され亭ます。",
                    this._obj.GetType().FullName,
                    this._minfo.First().Name), this._obj);

            var mem = this._minfo.First();

            return new FuncCalcMemberAccessor(this._obj, new[] { mem });
        }
        private INumber ConvertToFC(object obj) {
            if (obj is FCNumber)
                return (obj as FCNumber).Parent;
            else if (obj is INumber)
                return obj as INumber;
            else {
                var resFC = Support.DotnetToFC.ConvertToFC(obj);
                return resFC;

                // 型変換エラーはDotnetToFCで行われる
                throw new FuncCalcException(
                    string.Format("'{0}' の戻り値がINumber型ではありません。",
                    this._minfo.First().Name), this);
            }
        }
        public override void Set(RuntimeData runtime, INumber value) {

            if (this._minfo.Length >= 2)
                throw new RuntimeException(
                    string.Format("'{0}'の中に'{1}'は複数個定義され亭ます。",
                    this._obj.GetType().FullName,
                    this._minfo.First().Name), this._obj);

            var mem = this._minfo.First();
            if (mem is MethodInfo)
                throw new RuntimeException(
                    string.Format("メソッドメンバー '{0}'にオブジェクトを代入することはできません。", mem.Name), this._obj);
            if (mem is PropertyInfo) {
                ConvertToFC(
                    (mem as PropertyInfo).GetSetMethod().Invoke(this._obj, new object[] {
                    Convert.ChangeType(new FCNumber(runtime, value), (mem as PropertyInfo).PropertyType)}));
                return;
            }

            throw new FuncCalcException(
                string.Format("'{0}'の実行はまだ未対応です。", mem.MemberType), this._obj);

        }
        public override INumber Eval(RuntimeData runtime) {

            if (this._minfo.Length >= 2)
                throw new RuntimeException(
                    string.Format("'{0}'の中に'{1}'は複数個定義され亭ます。",
                    this._obj.GetType().FullName,
                    this._minfo.First().Name), this._obj);

            var mem = this._minfo.First();

            if (mem is MethodInfo)
                return new FuncCalcMemberAccessor(this._obj, new[] { mem });
            if (mem is PropertyInfo)
                return ConvertToFC(
                    (mem as PropertyInfo).GetGetMethod().Invoke(this._obj, new object[] { }));

            throw new FuncCalcException(
                string.Format("'{0}'の実行はまだ未対応です。", mem.MemberType), this._obj);


        }

        public override string ToString() {
            return this._minfo.First().Name;
        }
    }
}
