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
                parameters[0].Pow is Number && 
                (parameters[0].Pow as Number).Value == 1) {
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
                if (!(pow is Number && (pow as Number).Value == 1)) {
                    sb.Append("^");
                    sb.Append(pow.Output(type));
                }
                if (!(parameters[0] is NaturalLogarithm) ||
                    !(parameters[0].Pow is Number) || (parameters[0].Pow as Number).Value != 1) {
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
                if (!(pow is Number && (pow as Number).Value == 1)) {
                    sb.Append("^{");
                    sb.Append(pow.Output(type));
                    sb.Append("}");
                }
                sb.Append("(");
                if (parameters.Length >= 2)
                {
                    if (((parameters[0] is NaturalLogarithm) && ((parameters[0] as NaturalLogarithm).Pow is Number) &&
                    (parameters[0].Pow as Number).Value == 1)) {
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


        public INumber ExecuteDiff(RuntimeData runtime, string t, INumber[] parameters) {

            // log(xy) => log(x) + log(y)
            if (parameters[1] is MultipleFormula) {
                AdditionFormula af = new Expression.AdditionFormula();
                foreach (var item in (parameters[1] as MultipleFormula).Items) {
                    af.AddItem(runtime, this.ExecuteDiff(runtime, t, new INumber[] { parameters[0], item }));
                }
                return af;
            }
            if (parameters[1] is Fraction) {
                AdditionFormula af = new Expression.AdditionFormula();
                Fraction f = parameters[1] as Fraction;
                af.AddItem(runtime, this.ExecuteDiff(runtime, t, new INumber[] { parameters[0], f.Numerator}));
                af.AddItem(runtime, this.ExecuteDiff(runtime, t, new INumber[] { parameters[0], f.Denominator }).Multiple(runtime, Number.New(-1)));
                return af;
            }

            // 普通のlogの微分
            if (parameters[0] is NaturalLogarithm) {
                NaturalLogarithm e = parameters[0] as NaturalLogarithm;
                if (e.Pow.Equals(runtime, Number.New(1))) {
                    var prm = parameters[1].ExecuteDiff(runtime, t);
                    if (prm.Equals(runtime, Number.New(0)) || 
                        prm.Equals(runtime, Number.New(1))) {
                        return new Fraction(
                            parameters[1], Number.New(1));
                    }
                    else {
                        MultipleFormula mf = new Expression.MultipleFormula();
                        mf.AddItem(runtime, prm);
                        mf.AddItem(runtime, new Fraction(
                            parameters[1], Number.New(1)));
                        return mf;
                    }
                }
            }
            else if (!FuncCalc.Runtime.Func.Differential.IsConstValue(runtime, parameters[0])) {
                throw new RuntimeException("底に定数以外が含まれたlogの微分を行うことはできません。");
            }

            INumber param1Diff = parameters[1].ExecuteDiff(runtime, t);
            if (param1Diff.Equals(runtime, Number.New(0)))
                param1Diff = Number.New(1);

            MultipleFormula den = new MultipleFormula();
            den.AddItem(runtime, parameters[1]);
            den.AddItem(runtime, new FuncedINumber(this, new INumber[] { new NaturalLogarithm() , parameters[0] }));
            Fraction res = new Fraction(den, param1Diff);

            return res;
        }
    }
}
