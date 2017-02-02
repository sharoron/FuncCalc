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
            
            Dictionary<int, INumber> keisu = new Dictionary<int, INumber>();

            // 式の係数を調べる
            AdditionFormula af = f as AdditionFormula;
            for (int i = 0; i < af.Items.Length; i++) {

                var item = af.Items[i];

                INumber pow = null;
                var res = CheckMulti(runtime, varname, item, out pow);

                if (pow is Number) {
                    long p = (pow as Number).Value;
                    if (p > int.MaxValue)
                        throw new RuntimeException("int.MaxValueを超えるべき乗を含む式の解決はできません。");
                    int ip = (int)p;

                    if (!keisu.ContainsKey(ip))
                        keisu.Add(ip, Number.New(0));

                    keisu[ip] = keisu[ip].Add(runtime, res);

                }
                else {
                    throw new RuntimeException("整数以外のべき乗の変数が指定された状態で方程式を解決することはできません。");
                }
            }

            INumber result = null;

            // 一次方程式を解決する 
            // 条件: x^2以上のべき乗がない
            //      xの係数が0ではない
            if ((result = this.SolutionLinearEquation(runtime, varname, keisu)) != null)
                return result;

            // 二次方程式を解決する 
            // 条件: x^3以上のべき乗がない
            //      x^2, x, 1のそれぞれの係数が実数であること
            //      x^2の係数が0ではない
            if ((result = this.SolutionQuadraticEquation(runtime, varname, keisu)) != null)
                return result;


            // 五次方程式を解決する 
            // 無条件でエラーを出す。(解の公式は存在しないため計算を行うことができないため)
            if ((result = this.SolutionQuadraticEquation(runtime, varname, keisu)) != null)
                return result;

            throw new RuntimeException("式を解決することができませんでした");
        }

        /// <summary>
        /// 一次方程式を解決する
        /// </summary>
        private INumber SolutionLinearEquation(RuntimeData runtime, string varname, Dictionary<int, INumber> keisu) {
            if (!(keisu.Keys.Where(k => k >= 2).Count() == 0 && // x^3のような大きなべき乗がない
                keisu.ContainsKey(1) &&                      // xが含まれている
                !(keisu[1] is Number && (keisu[1] as Number).Value == 0)))  // x^2の係数が0ではない
                return null;

            runtime.AddLogWay("AsLinerEquation", keisu[1], keisu[0]);

            // ax + b = 0の時
            // 解: -(b / a)
            var res1 = keisu[0].Divide(runtime, keisu[1]).Multiple(runtime, Number.New(-1));
            
            var vr = new Variable(varname);
            runtime.AddLogWay("_LinerEquationWay1", vr, res1);

            Results res = new Results(new Variable(varname));
            res.items.Add(res1);

            return res;
        }
        /// <summary>
        /// 二次方程式を解決する
        /// </summary>
        private INumber SolutionQuadraticEquation(RuntimeData runtime, string varname, Dictionary<int, INumber> keisu) {

            if (!(keisu.Keys.Where(k => k >= 3).Count() == 0 && // x^3のような大きなべき乗がない
                keisu.Values.Where(v => !(v is IConstParameter)).Count() == 0 && // 係数が全て実数である
                keisu.ContainsKey(2) &&                      // x^2が含まれている
                !(keisu[2] is Number && (keisu[2] as Number).Value == 0)))  // x^2の係数が0ではない
                return null;

            INumber a = null, b = null, c = null;
            if (keisu.ContainsKey(2))
                a = keisu[2];
            else
                a = Number.New(0);
            if (keisu.ContainsKey(1))
                b = keisu[1];
            else
                b = Number.New(0);
            if (keisu.ContainsKey(0))
                c = keisu[0];
            else
                c = Number.New(0);

            runtime.AddLogWay("AsQuadraticEquation", a, b, c);

            INumber res1 = null, res2 = null;
            try {
                runtime.AddBlock(new BlockData() { MoreScope = false });
                runtime.NowBlock.Variables.Add("a", a);
                runtime.NowBlock.Variables.Add("b", b);
                runtime.NowBlock.Variables.Add("c", c);

                // ( -b ± √(b^2 - 4ac)) / 2a
                res1 = (runtime.Setting.GetExpression("((0-b)+sqrt(b^2-4a*c))/(2a)") as IEval).Eval(runtime);
                res2 = (runtime.Setting.GetExpression("((0-b)-sqrt(b^2-4a*c))/(2a)") as IEval).Eval(runtime);

                var vr = new Variable(varname);
                runtime.AddLogWay("_QuadraticEquationWay1", vr, res1);
                runtime.AddLogWay("_QuadraticEquationWay2", vr, res2);

            }
            finally { runtime.PopBlock(); }

            if (res1 == null && res2 == null)
                return null;

            Results ress = new Results(new Variable(varname));
            ress.items.Add(res1);
            if (!res1.Equals(runtime, res2))
                ress.items.Add(res2);
            return ress;
        }

        /// <summary>
        /// 五次方程式を解決する
        /// </summary>
        private INumber SolutionlQuinticEquation(RuntimeData runtime, string varname, Dictionary<int, INumber> keisu) {
            if (keisu.Keys.Where(k => k >= 4).Count() > 0)  // x^5の係数が含まれている
                return null;

            throw new MathExecutionException("五次方程式以上の解の公式は存在しないことになっているため式を解決することができません。");
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
