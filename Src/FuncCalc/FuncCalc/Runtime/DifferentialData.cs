using FuncCalc.Expression;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuncCalc.Runtime
{
    /// <summary>
    /// 微分に関する専用クラス
    /// </summary>
    public class DifferentialData
    {
        private int _id = 0;
        private Dictionary<int, INumber> _datas = new Dictionary<int, INumber>();
        private RuntimeSetting _setting = null;
        private RuntimeData _rdata = null;
        private string _t = null;
        private INumber _res = null;

        private DifferentialData() { }
        public DifferentialData(RuntimeData runtime) {

            if (runtime == null)
                throw new ArgumentNullException("runtime");

            this._res = Number.New(1);
            this._rdata = runtime;
            this._setting = runtime.Setting;
        }

        public int ID
        {
            get { return this._id; }
            set { this._id = value; }
        }
        public Dictionary<int, INumber> Datas
        {
            get { return this._datas; }
        }
        public RuntimeSetting Setting
        {
            get { return this._setting; }
        }
        public RuntimeData RData
        {
            get { return this._rdata; }
        }
        public string T
        {
            get { return this._t; }
            set { this._t = value; }
        }
        public INumber Result
        {
            get { return this._res; }
        }

        public INumber this[int index]
        {
            get { return this._datas[index]; }
            set { this._datas[index] = value; }
        }


        public INumber CheckPow(INumber value) {
            INumber pow = value.Pow;

            if (!IsFunction(value))
                throw new ArgumentException("CheckPowに通せる値は関数のみです。", "value");

            if (IsFunction(pow)) {
                // 対数微分法を使用して微分することになる
                // 参考: http://mathtrain.jp/logbibun
                throw new NotImplementedException("塁上部分に関数が含まれている式の微分はまだできません。");
            }

            if (Number.IsOne(pow))
                return value;

            var res = value.Clone();
            this.AddResult(pow);
            res.Pow = res.Pow.Subtract(this.RData, Number.New(1));
            this.AddResult(res);
            return res;
        }
        public void AddResult(INumber value) {
            
            this._res = this.Result.Multiple(this.RData, value);
        }
        public void AddParam(INumber value) {
            if (IsFunction(value)) {

                this._res =  this._res
                    .Multiple(this.RData, value.Differentiate(this.RData, this.T));
                
            } else {
                // スルーする
            }
        }
        public bool IsFunction(INumber value) {
            return DifferentialData.IsFunction(value, this.T);
        }
        public static bool IsFunction(INumber value, string t) {
            if (value is IConstParameter) {
                // value.Powが1じゃなくても定数ならばvalueは定数
                if (value.Pow is IConstParameter ||
                    value.Pow is ImaginaryNumber)
                    return false;
                else
                    return true;
            }

            if (!Number.IsOne(value.Pow))
                return true;
            if (value is Variable) {
                return
                    IsFunction((value as Variable).Multi, t) ||
                    (value as Variable).Name != t;
            }
            if (value is Member) {
                return
                    IsFunction((value as Member).Multi, t) ||
                    (value as Member).Text != t;
            }

            if (value is AdditionFormula) {
                foreach (var item in (value as AdditionFormula).Items) {
                    if (IsFunction(item, t))
                        return true;
                }
                return false;
            }
            if (value is MultipleFormula) {
                foreach (var item in (value as MultipleFormula).Items)
                    if (IsFunction(item, t)) return true;
                return false;
            }
            if (value is FuncedINumber)
                return true;

            throw new NotImplementedException("関数判定機に対応していないタイプ'" + 
                value.GetType().FullName+ "'です。");
        }
        public INumber DifferentiateConstant(INumber value) {
            throw new NotImplementedException("定数の微分の未実装");
        }

        /// <summary>対数微分法を利用して微分する</summary>
        public INumber LogarithmicDifferentiate(INumber val) {
            this.RData.AddLogWay("LogarithmDiffWay1", val);

            // 参考: http://mathtrain.jp/logbibun

            throw new NotImplementedException();
        }





    }
}
