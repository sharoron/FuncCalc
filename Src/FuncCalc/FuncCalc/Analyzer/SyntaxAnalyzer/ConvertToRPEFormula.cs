using FuncCalc.Exceptions;
using FuncCalc.Expression;
using FuncCalc.Interface;
using FuncCalc.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Analyzer
{
    public partial class SyntaxAnalyzer
    {
        public class ConvertToRPEFormula
        {
            private Formula formula = null;
            private RuntimeSetting setting = null;
            private RPEFormula result = null;

            private List<IExpression> items = null;
            private int now = -1;
            private int index = 0;
            private int[] levels = null;

            private ConvertToRPEFormula() { }
            public ConvertToRPEFormula(Formula f, RuntimeSetting setting) {
                this.formula = f;
                this.setting = setting;
            }

            public Formula Formula
            {
                get { return this.formula; }
            }
            public RuntimeSetting Setting
            {
                get { return this.setting; }
            }

            /// <summary>
            /// 実行に適した逆ポーランド記法の計算式に変換します。
            /// </summary>
            public IFormula ConvertToRPE() {

                this.Initialize();

                // 逆ポーランド記法へ変換しても恩恵がないものは弾く
                if (this.items.Where(a => a is LineBreak).Count() >= 1 ||
                    this.items.Count <= 1)
                    return this.formula;

                foreach (var level in this.levels) {
                    // for (int i = this.items.Count - 1; i >= 0; i--) {
                    for (int i = 0; i < this.items.Count; i ++) {
                        foreach (var op in this.setting.Spec.Operations.Where(it => it.Value == level)) {
                            if (this.items.Count <= 0 || i >= this.items.Count || 
                                i < 0) break;
                            IExpression exp = this.items[i];

                            if (exp.Token.Type == TokenType.Operation &&
                                exp.Token.Text == op.Key.Text) {

                                //演算子の条件に適しているか確認する
                                if ((!op.Key.RequireLeftParameter && i >= 1 && !(this.items[i - 1] is Operator)) ||
                                    (!op.Key.RequireRightParameter && i < this.items.Count - 1 && !(this.items[i + 1] is Operator)))
                                    continue;

                                IExpression right = null, left = null;
                                
                                // 左の要素を取り出す
                                if (op.Key.RequireLeftParameter &&
                                    i >= 1 &&
                                    !(this.items[i - 1] is Operator)) {

                                    if (i > 0 && i <= this.items.Count - 2 && // 右がRPETokenなら
                                        (this.items[i + 1] is RPEToken)) {
                                        if (!(this.items[i - 1] is RPEToken)) {

                                            bool flag = false;
                                            for (int j = this.result.Count - 1; j >= 0; j--) {
                                                if (this.result.Items[j] ==
                                                        (this.items[i + 1] as RPEToken).Expression) {
                                                    this.result.Insert(j, this.items[i - 1]);
                                                    left = this.items[i - 1];
                                                    right = this.items[i + 1];
                                                    this.items.RemoveAt(i - 1);
                                                    flag = true;
                                                    i--;
                                                    break;
                                                }
                                            }
                                            if (!flag)
                                                throw new Exception("RPE変換に失敗しました");

                                            (this.items[i + 1] as RPEToken).Expression = left;
                                        }
                                        else {

                                            bool flag = false, flag2 = false; int lindex = -1, rindex = -1;

                                            // Right Index
                                            for (int j = this.result.Count - 1; j >= 0; j--) {
                                                if (this.result.Items[j] ==
                                                        (this.items[i + 1] as RPEToken).Expression) {
                                                    flag = true;
                                                    rindex = j;
                                                    break;
                                                }
                                            }
                                            // Left Index
                                            for (int j = this.result.Count - 1; j >= 0; j--) {
                                                if (this.result.Items[j] ==
                                                        (this.items[i - 1] as RPEToken).Expression) {
                                                    flag2 = true;
                                                    lindex = j;
                                                    break;
                                                }
                                            }
                                            if (!flag || !flag2) {
                                                throw new RuntimeException("RPE変換に失敗しました.", this.items[i]);
                                            }
                                            if (!(lindex <= rindex)) {
                                                // #R * #L+
                                                left = this.result.Items[lindex]; right = this.result.Items[rindex];
                                                for (int j = lindex, k = 0; j < this.result.Items.Count; j++, k++) {
                                                    this.result.items.Insert(rindex + k, this.result.Items[j]);
                                                    this.result.items.RemoveAt(j + 1);
                                                }
                                                this.items.RemoveAt(i - 1);
                                                i--;

                                                (this.items[i + 1] as RPEToken).Expression = left;
                                            }
                                            else {
                                                // #L * #R
                                                left = this.result.Items[lindex]; right = this.result.Items[rindex];
                                                (this.items[i + 1] as RPEToken).Expression =
                                                    (this.items[i - 1] as RPEToken).Expression;
                                                this.items.RemoveAt(i - 1);
                                                i--;
                                            }

                                        }
                                    }
                                    else {
                                        if (this.items[i - 1] is RPEToken) {

                                        }
                                        else {
                                            left = this.Normalization(this.items[i - 1]);
                                            this.result.AddItem(null, left);
                                            this.items.RemoveAt(i - 1);
                                            //forなら必要
                                            if (right is RPEToken) { // 右がRPETokenなら、RPETokenを新たに追加しない       
                                                i--;
                                            }
                                            else {
                                                this.items.Insert(i - 1, new RPEToken(left));
                                            }
                                        }
                                    }
                                }

                                // 右の要素を取り出す
                                if (op.Key.RequireRightParameter &&
                                    i < this.items.Count - 1 &&
                                    !(this.items[i + 1] is Operator)) {


                                    // 右がRPETokenなら無視する
                                    if (this.items[i + 1] is RPEToken) {
                                        if (op.Key.RequireLeftParameter) {
                                            // rightもすでに入ってる
                                        }
                                        else {
                                            
                                        }
                                    }
                                    else {
                                        right = this.Normalization(this.items[i + 1]);
                                        this.result.AddItem(null, right);
                                        this.items.RemoveAt(i + 1);


                                        if (!op.Key.RequireLeftParameter) {
                                            this.items.Insert(i, new RPEToken(right));
                                            i++;
                                        }
                                    }
                                }
                                


                                this.result.AddItem(null, new Operator(this.items[i].Token, op.Key));
                                this.items.RemoveAt(i);

                                i--; // forループの場合は必要
                                // forrループの場合は不要
                            }
                        }
                    }
                }

                var r = this.items.Where(i => !(i is RPEToken)).ToArray();
                this.items.Clear();
                this.items.AddRange(r);

                if (this.items.Count != 0) {
                    if (this.result.Count == 0 && this.items.Count == 1) {
                        foreach (var item in this.items) {
                            this.result.AddItem(null, item);
                        }
                    }
                    else {
                        throw new SyntaxException(string.Format("逆ポーランド記法に変換する際にトークンが余りました。 '{0}' など{1}個余りました",
                            this.items.First().Token, this.items.Count), this.items.First().Token);
                    }
                }

                return this.result;

            }
            private void Initialize() {
                this.result = new RPEFormula(this.formula);
                this.items = new List<Interface.IExpression>();
                this.items.AddRange(this.formula.Items);
                this.now = 99999;
                this.index = this.formula.Count - 1;

                // 演算子の優先順位を並び替えて変数へ格納
                this.levels = (
                    from f in this.setting.Spec.Operations.Values.Distinct()
                    orderby f descending
                    select f).ToArray<int>();


            }
            private IExpression Normalization(IExpression exp) {

                IExpression res = exp;

                if (res is Formula) {
                    return ConvertToRPEFormula.ConvertToRPE(res as Formula, this.setting) as IExpression;
                }
                if (res is RPEToken)
                    throw new Exception("RPETokenを一般化しようとしました");

                return res;

            }
            public static IFormula ConvertToRPE(Formula f, RuntimeSetting setting) {
                ConvertToRPEFormula fa = new ConvertToRPEFormula(f, setting);
                return fa.ConvertToRPE();
            }

            public override string ToString() {
                StringBuilder sb = new StringBuilder();
                foreach (var item in this.items) {
                    sb.Append(" " + item.ToString());
                }
                if (sb.Length != 0)
                sb = sb.Remove(0, 1);
                return sb.ToString();
            }
            
        }
    }
}