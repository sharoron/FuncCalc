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
    public class Integrate : IFunction, IFunctionOutput
    {
        public override string Name
        {
            get
            {
                return "intg";
            }
        }
        public override string Description
        {
            get
            {
                return "積分します";
            }
        }
        public override ExpressionType[] Parameter
        {
            get { return new ExpressionType[] { ExpressionType.Pointer, ExpressionType.Unknown }; }
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
            var res = parameters[1].Integrate(runtime, parameters[0].Token.Text);

            var c = IntegralConstant.Create(runtime);
            AdditionFormula af = new AdditionFormula();
            af.AddItem(runtime, res);
            af.AddItem(runtime, c);

            runtime.AddLogWay("Equal", new FuncedINumber(this, parameters), af);
            runtime.AddLogCondition("IntegralConstant", c);
            runtime.SetVariable(runtime, new Variable(c.ToString()), c);

            return af;
        }

        public override string Output(OutputType type) {
            switch (type) {
                case OutputType.String:
                    return "∫";
                case OutputType.Mathjax:
                    return "\\int ";
                default:
                    throw new NotImplementedException();
            }
        }
        public string Output(OutputType type, INumber[] parameters, INumber pow) {
            switch (type) {
                case OutputType.String: {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("∫(");
                        sb.Append(parameters[1].Output(type));
                        sb.Append(")");
                        sb.Append("d" + parameters[0].Token.Text);
                        return sb.ToString();
                    }
                case OutputType.Mathjax: {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("\\int {(");
                        sb.Append(parameters[1].Output(type));
                        sb.Append(") d");
                        sb.Append(parameters[0].Token.Text);
                        sb.Append("}");
                        return sb.ToString();
                    }
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
