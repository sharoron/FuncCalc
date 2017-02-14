using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Expression;
using FuncCalc.Expression.Const;
using FuncCalc.Exceptions;

namespace FuncCalc.Runtime.Func
{
    public class Logarithm : IFunction, IFunctionOutput, IDiffWithParameters
    {
        public override string Name
        {
            get
            {
                return "log";
            }
        }
        public override string Description {
            get {
                return "底を指定して対数を求めます。"; 
            }
        }
        public override ExpressionType[] Parameter
        {
            get
            {
                return new ExpressionType[] { ExpressionType.Unknown, ExpressionType.Unknown };
            }
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

        public bool InfinitelyDifferentiable { get { return false; } }

        public override INumber Execute(RuntimeData runtime, params INumber[] parameters) {

            // log(x) ただしx > 0なのでおかしい値なら弾く
            if (parameters[1].ValueType == Expression.ValueType.Minus || 
                parameters[1].IsZero)
                throw new RuntimeException("logの中身は0より大きい必要があります。", parameters[1]);
            // log_n(1) => 0 但しnは任意の値
            if (parameters[1].IsOne)
                return Number.New(0);

            var param0 = parameters[0];

            if (param0 is Variable || param0 is Member) {
                string t = "";
                if (param0 is Variable) t = (param0 as Variable).Name;
                if (param0 is Member) t = (param0 as Member).Text;

                if (t == "e")
                    param0 = new NaturalLogarithm();
            }else {
                param0 = param0.Eval(runtime);
            }

            return new FuncedINumber(this, new INumber[] { param0, parameters[1].Eval(runtime) });
        }
        public override INumber ForceExecute(RuntimeData runtime, params INumber[] parameters) {
            var param = parameters[1].FinalEval(runtime);
            var Base = parameters[0].FinalEval(runtime);
            if (parameters[0] is NaturalLogarithm && 
                parameters[0].Pow.IsOne) {
                return new FloatNumber((decimal)(System.Math.Log(
                    (double)(param as IConstParameter).ConstValue)));
            }
            if (runtime.IsConstValue(param) && runtime.IsConstValue(Base)) {
                return new FloatNumber((decimal)(System.Math.Log(
                    (double)(param as IConstParameter).ConstValue,
                    (double)(Base as IConstParameter).ConstValue)));
            }
            return param;
        }

        public override string Output(OutputType type) {
            switch (type) {
                case OutputType.String:
                case OutputType.Mathjax:
                    return this.Name;
            }

            throw new NotImplementedException();
        }
        public string Output(OutputType type, INumber[] parameters, INumber pow) {
            if (type == OutputType.Mathjax) {
                StringBuilder sb = new StringBuilder();
                sb.Append("log");
                if (!pow.IsOne) {
                    sb.Append("^");
                    sb.Append(pow.Output(type));
                }
                if (!(parameters[0] is NaturalLogarithm) ||
                    !parameters[0].Pow.IsOne) {
                    sb.Append("_");
                    sb.Append("{");
                    sb.Append(parameters[0].Output(type));
                    sb.Append("}");
                }
                sb.Append("{");
                sb.Append(parameters[1].Output(type));
                sb.Append("}");
                return sb.ToString();
            }
            if (type == OutputType.String)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("log");
                if (!pow.IsOne) {
                    sb.Append("^{");
                    sb.Append(pow.Output(type));
                    sb.Append("}");
                }
                sb.Append("(");
                if (parameters.Length >= 2)
                {
                    if (parameters[0] is NaturalLogarithm &&
                        parameters[0].Pow.IsOne) {
                        sb.Append(parameters[1].Output(type));
                    }
                    else {
                        sb.Append(parameters[0].Output(type));
                        sb.Append(", ");
                        sb.Append(parameters[1].Output(type));
                    }
                }
                else
                    sb.Append(parameters[0].Output(type));
                sb.Append(")");
                return sb.ToString();
            }

            return this.ToString();

            // return base.Output(type, parameters);
        }
        public override string ToString()
        {
            return this.Output(OutputType.String);
        }


        public INumber Differentiate(RuntimeData runtime, DifferentialData ddata, INumber[] parameters) {

            // log(xy) => log(x) + log(y)
            if (parameters[1] is MultipleFormula) {
                AdditionFormula af = new Expression.AdditionFormula();
                foreach (var item in (parameters[1] as MultipleFormula).Items) {
                    DifferentialData newDdata = new FuncCalc.Runtime.DifferentialData(runtime) {
                        T =  ddata.T
                    };
                    af.AddItem(runtime, this.Differentiate(runtime, newDdata, new INumber[] { parameters[0], item }));
                }
                return af;
            }
            // log ( x / y )みたいに中身に分数が入っている
            if (parameters[1] is Fraction) {
                AdditionFormula af = new Expression.AdditionFormula();
                Fraction f = parameters[1] as Fraction;
                DifferentialData newDdata1 = new DifferentialData(runtime) { T = ddata.T };
                DifferentialData newDdata2 = new DifferentialData(runtime) { T = ddata.T };
                af.AddItem(runtime, this.Differentiate(runtime, newDdata1, new INumber[] { parameters[0], f.Numerator}));
                af.AddItem(runtime, this.Differentiate(runtime, newDdata2, new INumber[] { parameters[0], f.Denominator }).Multiple(runtime, Number.New(-1)));
                return af;
            }

            // log_n(1) => 0 ただしnは任意の値
            // なので (log_n(1))' => 0
            if (parameters[1].IsOne)
                return Number.New(0);

            // parameters[1]の微分を答えの中にかけておく
            ddata.AddParam(parameters[1]);

            // 普通のlogの微分
            if (parameters[0] is NaturalLogarithm) {
                NaturalLogarithm e = parameters[0] as NaturalLogarithm;
                if (e.Pow.IsOne) {
                    return new Fraction(
                        parameters[1], Number.New(1));
                }
            }
            else if (ddata.IsFunction(parameters[0])) {
                throw new RuntimeException("底に定数以外が含まれたlogの微分を行うことはできません。");
            }
            
            MultipleFormula den = new MultipleFormula();
            den.AddItem(runtime, parameters[1]);
            den.AddItem(runtime, new FuncedINumber(this, new INumber[] { new NaturalLogarithm() , parameters[0] }));
            Fraction res = new Fraction(den, Number.New(1));

            return res;
        }
    }
}
