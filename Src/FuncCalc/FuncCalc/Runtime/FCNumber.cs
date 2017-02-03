using FuncCalc.Expression;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuncCalc.Runtime
{
    public class FCNumber : IOutput
    {
        private INumber parent = null;
        private RuntimeData runtime = null;

        public FCNumber(RuntimeData runtime, INumber num) {
            this.runtime = runtime;
            this.parent = num;
        }

        public INumber Parent
        {
            get { return this.parent; }
        }
        public RuntimeData Runtime
        {
            get { return this.runtime; }
        }
        public bool IsConstValue
        {
            get
            {
                return this.parent is IConstParameter ||
                    this.parent is ImaginaryNumber;
            }
        }
        public bool IsRealNumber
        {
            get
            {
                return
                    !(this.parent is ImaginaryNumber) &&
                    this.parent.ContainsImaginalyNumber;
            }
        }
        public bool? IsPlus
        {
            get
            {
                return
                    this.parent.ValueType == Expression.ValueType.Plus ? (bool?)true :
                    this.parent.ValueType == Expression.ValueType.Minus ? (bool?)false :
                    null;
            }
        }

        public static FCNumber operator +(FCNumber num1, FCNumber num2) {
            return new FCNumber(num1.runtime,
                num1.parent.Add(num1.runtime, num2.parent));
        }
        public static FCNumber operator -(FCNumber num1, FCNumber num2) {
            return new FCNumber(num1.runtime,
                num1.parent.Subtract(num1.runtime, num2.parent));
        }
        public static FCNumber operator *(FCNumber num1, FCNumber num2) {
            return new FCNumber(num1.runtime,
                num1.parent.Multiple(num1.runtime, num2.parent));
        }
        public static FCNumber operator /(FCNumber num1, FCNumber num2) {
            return new FCNumber(num1.runtime,
                num1.parent.Divide(num1.runtime, num2.parent));
        }
        public FCNumber Power(FCNumber num) {
            return new FCNumber(
                runtime, this.parent.Power(runtime, num.parent));
        }
        public FCNumber Differentiate(string t = "x") {
            return new FuncCalc.Runtime.FCNumber(this.runtime,
                this.parent.ExecuteDiff(this.runtime, t));
        }
        public FCNumber Integrate(string t = "x") {
            return new FuncCalc.Runtime.FCNumber(this.runtime,
                this.parent.Integrate(this.runtime, t));
        }

        public string Output(OutputType type) {
            return this.parent.Output(type);
        }
        public override string ToString() {
            return parent.ToString();
        }
    }
}
