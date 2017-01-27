using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Expression;
using FuncCalc.Exceptions;
using FuncCalc.Runtime.Operator;

namespace FuncCalc.Runtime.Func
{
    public class Solution : IFunction
    {
        public override string Name
        {
            get
            {
                return "sol";
            }
        }
        public override string Description
        {
            get
            {
                return "方程式を解決します。";
            }
        }
        public override ExpressionType[] Parameter
        {
            get { return new ExpressionType[] { ExpressionType.Unknown }; }
        }
        public override ExpressionType ReturnType
        {
            get
            {
                return ExpressionType.Unknown;
            }
        }
        public override bool DoEvaledParam
        {
            get
            {
                return false;
            }
        }

        public override INumber Execute(RuntimeData runtime, params INumber[] parameters) {
            
            string varname = "x";
            INumber f = parameters[0].Optimise(runtime);

            if (f is IConstParameter)
                throw new RuntimeException("定数から方程式を評価することはできません。", parameters[0]);
            if (f is Variable) {
                if ((f as Variable).Name == varname)
                    return Number.New(0);
                else
                    throw new RuntimeException("この条件では方程式を解決することはできません。");
            }
            if (f is Member) {
                if ((f as Member).Text == varname)
                    return Number.New(0);
                else
                    throw new RuntimeException("この条件では方程式を解決することはできません。");
            }
            if (f is MultipleFormula) {
                bool flag = false;
                MultipleFormula mf = f as MultipleFormula;
                for (int i = 0; i < mf.Items.Count; i++) {
                    if (mf.Items[i] is Variable && (mf.Items[i] as Variable).Name == varname)
                        flag = true;
                    if (mf.Items[i] is Member && (mf.Items[i] as Member).Text == varname)
                        flag = true;
                }
                if (flag) return Number.New(0);
                else throw new RuntimeException("この条件では方程式を解決することはできません。");
            }
            if (!(f is AdditionFormula))
                throw new RuntimeException(string.Format("まだ '{0}' の式の解決は対応していません。", f.GetType().FullName));

            INumber
                c = Number.New(0),
                b = Number.New(0),
                a = Number.New(0);

            AdditionFormula af = f as AdditionFormula;
            for (int i = 0; i < af.Items.Length; i++) {

                var item = af.Items[i];

                INumber pow = null;
                var res = CheckMulti(runtime, varname, item, out pow);

                if (pow is Number) {
                    long p = (pow as Number).Value;
                    if (p == 0) c = c.Add(runtime, res);
                    else if (p == 1) b = b.Add(runtime, res);
                    else if (p == 2) a = a.Add(runtime, res);
                    else throw new NotImplementedException("3以上のべき乗の変数が含まれる方程式を解決することはまだできません。");
                }
                else {
                    throw new RuntimeException("整数以外のべき乗の変数が指定された状態で方程式を解決することはできません。");   
                }
            }

            var test = b.Power(runtime, Number.New(2))
                    .Add(runtime,
                        Number.New(-4)
                        .Multiple(runtime, a)
                        .Multiple(runtime, c)
                        ).Power(runtime, Number.New(-1))
                    ;

            var res1 =
                b.Multiple(runtime, Number.New(-1))
                .Add(runtime,
                    b.Power(runtime, Number.New(2))
                    .Add(runtime,
                        Number.New(-4)
                        .Multiple(runtime, a)
                        .Multiple(runtime, c)
                        ).Power(runtime, Number.New(-1))
                    )
                .Divide(runtime, Number.New(2).Multiple(runtime, b));
            var res2 =
                b.Multiple(runtime, Number.New(-1))
                .Add(runtime,
                    b.Power(runtime, Number.New(2))
                    .Add(runtime,
                        Number.New(-4)
                        .Multiple(runtime, a)
                        .Multiple(runtime, c)
                        ).Power(runtime, Number.New(-1)).Multiple(runtime, Number.New(-1))
                    )
                .Divide(runtime, Number.New(2).Multiple(runtime, b));
            

            Results ress = new Results();
            ress.items.Add(res1);
            if (!res1.Equals(runtime, res2))
                ress.items.Add(res2);
            return ress;

        }

        INumber CheckMulti(RuntimeData runtime, string varname ,INumber val, out INumber pow) {

            

            if (val is IConstParameter) {
                pow = Number.New(0);
                return val;
            }
            if (val is Variable) {
                if ((val as Variable).Name == varname) {
                    pow = val.Pow;
                    return (val as Variable).Multi;
                }
                else {
                    pow = Number.New(0);
                    return val;
                }
            }
            if (val is Member) {
                if ((val as Member).Text == varname) {
                    pow = val.Pow;
                    return (val as Member).Multi;
                }
                else {
                    pow = Number.New(0);
                    return val;
                }
            }
            if (val is MultipleFormula) {
                MultipleFormula mf = val as MultipleFormula;
                INumber res = Number.New(0);
                INumber pres = Number.New(1);
                bool flag = false;

                for (int i = 0; i < mf.Items.Count; i++) {
                    var item = mf.Items[i];

                    INumber p = null;
                    var r = this.CheckMulti(runtime, varname, item, out p);
                    if (p is Number && (p as Number).Value == 0) {
                        res = res.Multiple(runtime, r);
                    }
                    else {
                        flag = true;
                        pres = pres.Add(runtime, p);
                        res = res.Multiple(runtime, r);
                    }
                }
                if (flag) {
                    pow = pres;
                    return res;
                }
                else {
                    pow = Number.New(0);
                    return res;
                }
            }
            if (val is AdditionFormula) {
                throw new RuntimeException("バグ: [AF]のアイテム評価関数に[AF]が指定されました。", val);
            }

            throw new NotImplementedException("[AF]のアイテム評価関数の未実装");

        }
    }
}
