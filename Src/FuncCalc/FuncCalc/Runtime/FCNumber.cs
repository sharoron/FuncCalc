using FuncCalc.Exceptions;
using FuncCalc.Expression;
using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuncCalc.Runtime
{
    public class FCNumber : IOutput, IConvertible
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
                this.parent.Differentiate(this.runtime, t));
        }
        public FCNumber Integrate(string t = "x") {
            return new FuncCalc.Runtime.FCNumber(this.runtime,
                this.parent.Integrate(this.runtime, t));
        }

        public static implicit operator Byte(FCNumber val) {
            if (val.parent is Expression.Number)
                return (Byte)(val.parent as Expression.Number).Value;

            if (val.parent is Interface.IConstParameter)
                return (Byte)(val.parent as IConstParameter).ConstValue;

            throw new RuntimeException(
                string.Format("'{0}' は '{1}' に変換することができません。",
                val.parent.GetType().FullName,
                typeof(Byte)), val.parent);
        }
        public static implicit operator SByte(FCNumber val) {
            if (val.parent is Expression.Number)
                return (SByte)(val.parent as Expression.Number).Value;

            if (val.parent is Interface.IConstParameter)
                return (SByte)(val.parent as IConstParameter).ConstValue;

            throw new RuntimeException(
                string.Format("'{0}' は '{1}' に変換することができません。",
                val.parent.GetType().FullName,
                typeof(SByte)), val.parent);
        }
        public static implicit operator Int16(FCNumber val) {
            if (val.parent is Expression.Number)
                return (Int16)(val.parent as Expression.Number).Value;

            if (val.parent is Interface.IConstParameter)
                return (Int16)(val.parent as IConstParameter).ConstValue;

            throw new RuntimeException(
                string.Format("'{0}' は '{1}' に変換することができません。",
                val.parent.GetType().FullName,
                typeof(Int16)), val.parent);
        }
        public static implicit operator Int32(FCNumber val) {
            if (val.parent is Expression.Number)
                return (Int32)(val.parent as Expression.Number).Value;

            if (val.parent is Interface.IConstParameter)
                return (Int32)(val.parent as IConstParameter).ConstValue;

            throw new RuntimeException(
                string.Format("'{0}' は '{1}' に変換することができません。",
                val.parent.GetType().FullName,
                typeof(Int32)), val.parent);
        }
        public static implicit operator Int64(FCNumber val) {
            if (val.parent is Expression.Number)
                return (Int64)(val.parent as Expression.Number).Value;

            if (val.parent is Interface.IConstParameter)
                return (Int64)(val.parent as IConstParameter).ConstValue;

            throw new RuntimeException(
                string.Format("'{0}' は '{1}' に変換することができません。",
                val.parent.GetType().FullName,
                typeof(Int64)), val.parent);
        }
        public static implicit operator UInt16(FCNumber val) {
            if (val.parent is Expression.Number)
                return (UInt16)(val.parent as Expression.Number).Value;

            if (val.parent is Interface.IConstParameter)
                return (UInt16)(val.parent as IConstParameter).ConstValue;

            throw new RuntimeException(
                string.Format("'{0}' は '{1}' に変換することができません。",
                val.parent.GetType().FullName,
                typeof(UInt16)), val.parent);
        }
        public static implicit operator UInt32(FCNumber val) {
            if (val.parent is Expression.Number)
                return (UInt32)(val.parent as Expression.Number).Value;

            if (val.parent is Interface.IConstParameter)
                return (UInt32)(val.parent as IConstParameter).ConstValue;

            throw new RuntimeException(
                string.Format("'{0}' は '{1}' に変換することができません。",
                val.parent.GetType().FullName,
                typeof(UInt32)), val.parent);
        }
        public static implicit operator UInt64(FCNumber val) {
            if (val.parent is Expression.Number)
                return (UInt64)(val.parent as Expression.Number).Value;

            if (val.parent is Interface.IConstParameter)
                return (UInt64)(val.parent as IConstParameter).ConstValue;

            throw new RuntimeException(
                string.Format("'{0}' は '{1}' に変換することができません。",
                val.parent.GetType().FullName,
                typeof(UInt64)), val.parent);
        }
        public static implicit operator Single(FCNumber val) {
            if (val.parent is Expression.Number)
                return (Single)(val.parent as Expression.Number).Value;

            if (val.parent is Interface.IConstParameter)
                return (Single)(val.parent as IConstParameter).ConstValue;

            throw new RuntimeException(
                string.Format("'{0}' は '{1}' に変換することができません。",
                val.parent.GetType().FullName,
                typeof(Single)), val.parent);
        }
        public static implicit operator Double(FCNumber val) {
            if (val.parent is Expression.Number)
                return (Double)(val.parent as Expression.Number).Value;

            if (val.parent is Interface.IConstParameter)
                return (Double)(val.parent as IConstParameter).ConstValue;

            throw new RuntimeException(
                string.Format("'{0}' は '{1}' に変換することができません。",
                val.parent.GetType().FullName,
                typeof(Double)), val.parent);
        }
        public static implicit operator Decimal(FCNumber val) {
            if (val.parent is Expression.Number)
                return (Decimal)(val.parent as Expression.Number).Value;

            if (val.parent is Interface.IConstParameter)
                return (Decimal)(val.parent as IConstParameter).ConstValue;

            throw new RuntimeException(
                string.Format("'{0}' は '{1}' に変換することができません。",
                val.parent.GetType().FullName,
                typeof(Decimal)), val.parent);
        }
        public static implicit operator System.String(FCNumber val) {
            if (val.parent is Expression.String)
                return (val.parent as Expression.String).Token.Text;
            
            throw new RuntimeException(
                string.Format("'{0}' は '{1}' に変換することができません。",
                val.parent.GetType().FullName,
                typeof(System.String)), val.parent);
        }
        public static implicit operator Boolean(FCNumber val) {
            if (val.parent is Expression.Number)
                return (val.parent as Expression.Number).Value != 0;

            if (val.parent is Interface.IConstParameter)
                return (val.parent as IConstParameter).ConstValue != 0;

            val.ThrowUnconvertable(typeof(Boolean));
            return false;
        }
        private void ThrowUnconvertable(Type t) {

            throw new RuntimeException(
                string.Format("'{0}' は '{1}' に変換することができません。",
                this.parent.GetType().FullName,
                typeof(Boolean)), this.parent);
        }
        

        public string Output(OutputType type) {
            return this.parent.Output(type);
        }
        public override string ToString() {
            return parent.ToString();
        }

        public TypeCode GetTypeCode() {
            if (this.parent is Expression.Number)
                return TypeCode.Int64;
            if (this.parent is IConstParameter)
                return TypeCode.Decimal;
            if (this.parent is Expression.String)
                return TypeCode.String;

            return TypeCode.Object;
        }

        public bool ToBoolean(IFormatProvider provider) {
            return this;
        }
        public char ToChar(IFormatProvider provider) {
            this.ThrowUnconvertable(typeof(char));
            return '\0';
        }
        public sbyte ToSByte(IFormatProvider provider) {
            return this;
        }
        public byte ToByte(IFormatProvider provider) {
            return this;
        }
        public short ToInt16(IFormatProvider provider) {
            return this;
        }
        public ushort ToUInt16(IFormatProvider provider) {
            return this;
        }
        public int ToInt32(IFormatProvider provider) {
            return this;
        }
        public uint ToUInt32(IFormatProvider provider) {
            return this;
        }
        public long ToInt64(IFormatProvider provider) {
            return this;
        }
        public ulong ToUInt64(IFormatProvider provider) {
            return this;
        }
        public float ToSingle(IFormatProvider provider) {
            return this;
        }
        public double ToDouble(IFormatProvider provider) {
            return this;
        }
        public decimal ToDecimal(IFormatProvider provider) {
            return this;
        }
        public DateTime ToDateTime(IFormatProvider provider) {
            ThrowUnconvertable(typeof(DateTime));
            return DateTime.MinValue;
        }
        public string ToString(IFormatProvider provider) {
            return this;
        }
        public object ToType(Type conversionType, IFormatProvider provider) {
            return this;
        }
    }
}
