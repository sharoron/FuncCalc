using FuncCalc.Exceptions;
using FuncCalc.Interface;
using FuncCalc.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Expression
{
    public class Array : INumber, IEvalWithParameters
    {
        List<List<INumber>> items = new List<List<INumber>>();

        public Array()
        {
            this.items.Add(new List<INumber>());
        }

        public override ExpressionType Type
        {
            get { return ExpressionType.Anything; }
        }
        public override ValueType ValueType
        {
            get { return Expression.ValueType.Unknown; }
        }

        public List<List<INumber>> Items
        {
            get { return this.items; }
        }
        public override int SortPriority {
            get { return 750; }
        }
        public override bool InfinitelyDifferentiable
        {
            get
            {
                for (int i = 0; i < this.items.Count; i++) {
                    for (int k = 0; k < this.items[i].Count; k++) {
                        if (this.items[i][k].InfinitelyDifferentiable)
                            return true;
                    }
                }
                return false;
            }
        }

        public override INumber Add(Runtime.RuntimeData runtime, INumber val)
        {
            if (val is Array) {
                Array res = new Array();
                Array v = val as Array;
                for (int x = 0; x < Math.Max(this.items.Count, v.items.Count); x++) {
                    int c_count = 0; bool flag1 = false, flag2 = false;
                    if (this.items.Count > x) { c_count = this.items[x].Count; flag1 = true; }
                    if (v.items.Count > x) { c_count = Math.Max(c_count, v.items[x].Count); flag2 = true; }

                    if (x != 0)
                        res.items.Add(new List<INumber>());

                    for (int y = 0; y < c_count; y++) {
                        INumber d = Number.New(0);
                        if (flag1 && this.items[x].Count > y) d = d.Add(runtime, this.items[x][y]);
                        if (flag2 && v.items[x].Count > y) d = d.Add(runtime, v.items[x][y]);

                        res.items[x].Add(d);
                    }
                }

                return res;
            } else {
                var res = this.Clone() as Array;
                for (int x = 0; x < res.items.Count; x++) {
                    for (int y = 0; y < res.items[x].Count; y++) {
                        res.items[x][y] =
                            res.items[x][y].Add(runtime, val);
                    }
                }
                return res;
            }
        }
        public override INumber Multiple(Runtime.RuntimeData runtime, INumber val)
        {
            if (val is Array) {
                throw new NotImplementedException("配列同士の乗はまだ田対応していません。");

                var v = val as Array;
                int max_col = 0;
                for (int x = 0; x < v.items.Count; x++) {
                    max_col = Math.Max(max_col, v.items[x].Count);
                }

                for (int x = 0; x < this.items.Count; x++) {
                    INumber d = Number.New(0);
                    for (int y = 0; y < this.items[x].Count; y++) {
                        if (v.items.Count > y && v.items[y].Count > x)
                            d = d.Add(runtime, this.items[x][y].Multiple(runtime, v.items[y][x]));
                        
                    }
                }

            }
            else {
                var res = this.Clone() as Array;
                for (int x = 0; x < this.items.Count; x++) {
                    for (int y = 0; y < this.items[x].Count; y++) {
                        res.items[x][y] = res.items[x][y].Multiple(runtime, val);
                    }
                }
                return res;
            }
        }
        public override bool Equals(Runtime.RuntimeData runtime, INumber val)
        {
            throw new NotImplementedException();
        }
        public override bool CanJoin(Runtime.RuntimeData runtime, INumber val)
        {
            throw new NotImplementedException();
        }
        public override INumber ExecuteDiff(Runtime.RuntimeData runtime, string t)
        {
            throw new NotImplementedException();
        }

        public INumber Execute(Runtime.RuntimeData runtime, params INumber[] parameters)
        {
            if (this.items.Count == 1)
            {
                var index = parameters[0].Eval(runtime);
                if (!(index is Number))
                    throw new RuntimeException("配列のインデックスに整数以外のパラメータを指定することはできません。", index);

                return this.items[0][(int)(index as Number).Value];
            }
            else
            {
                throw new NotImplementedException("二次元配列のインデックス指定はまだ実装していません。");
            }
        }

        public override string Output(Runtime.OutputType type)
        {
            if (type == OutputType.String)
                return this.ToString();

            return base.Output(type);
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            if (this.items.Count == 1)
            {
                for (int i = 0; i < this.items[0].Count; i++)
                {
                    if (i != 0) sb.Append(", ");
                    sb.Append(this.items[0][i].Output(Runtime.OutputType.String));
                }
            }
            else
            {
                for (int i = 0; i < this.items.Count; i++)
                {
                    if (i != 0) sb.Append(", ");
                    sb.Append("[");
                    for (int k = 0; k < this.items[i].Count; k++)
                    {
                        if (k != 0) sb.Append(", ");
                        sb.Append(this.items[i][k].Output(Runtime.OutputType.String));
                    }
                    sb.Append("]");
                }
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}
