using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalc.Analyzer;
using FuncCalc.Runtime;
using FuncCalc.Exceptions;
using FuncCalc.Expression;
using FuncCalc.Interface;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Reflection;

namespace FuncCalc
{
    class Program
    {
        const bool LispMode = false;

        internal static string SaveDir = "";
        internal static RuntimeSetting setting = new RuntimeSetting();
        internal static RuntimeData data = setting.CreateNewRuntimedata();
        internal static IFormula formula = null;
        internal static Analyzer.Analyzer analyzer = null;

        public static void Main(string[] args) {

            SaveDir = Application.StartupPath + "/save/";
            if (!Directory.Exists(SaveDir))
                Directory.CreateDirectory(SaveDir);


        

            Console.WriteLine("============================");
            Console.WriteLine("    Func Calc ");
            Console.WriteLine("============================");
            for (;;) {
                Console.WriteLine(" 1. New Formula");
                Console.WriteLine(" 2. Show Functions");
                Console.WriteLine(" 3. Show Operators");
                if (formula != null) {
                    Console.WriteLine(" 4. Get Result");
                    Console.WriteLine(" 5. Run Addition Formula");
                    Console.WriteLine(" 6. Show DebugInformation");
                    Console.WriteLine(" 7. Show Expression Tree");
                    Console.WriteLine(" 8. Enabled DebugMode");
                }
                Console.WriteLine(" 9. Quit");
                char c = Console.ReadKey(true).KeyChar;
                Console.Clear();

                switch (c) {
                    case '1':
                        setting = new RuntimeSetting();
                        if (LispMode) {
                            Console.WriteLine("Lispモード");
                            setting.DefaultSyntaxAnalyzer =
                                typeof(FuncCalc.Lisp.SyntaxAnalyzer);
                        }
                        setting.AcceptBitLength = 4096;

                        data = setting.CreateNewRuntimedata();
                        Console.WriteLine("Initialized FormulaRuntime");
                        Console.WriteLine("Input formula");
                        Console.Write("> ");
                        try {
                            analyzer = new Analyzer.Analyzer(Console.ReadLine(), setting);
                            Stopwatch sw1 = Stopwatch.StartNew();
                            formula = analyzer.GetResult() as IFormula;
                            sw1.Stop();
                            Console.WriteLine("Success! Time : " + sw1.Elapsed.ToString());
                        }
                        catch (SyntaxException ex) {

                            Console.WriteLine("構文エラーが発生しました。計算式の書式のミスを確認してください。");
                            Console.WriteLine("エラー : " + ex.Message);
                            Console.WriteLine("トークン : {0}", ex.Token);
                            Console.Write("場所 : ");
                            ConsoleColor cc = Console.BackgroundColor;
                            if (analyzer.Tokens == null)
                                Console.WriteLine("トークン情報がありませんでした");
                            else
                                foreach (var t in analyzer.Tokens) {
                                    if (t != ex.Token) Console.BackgroundColor = cc;
                                    else Console.BackgroundColor = ConsoleColor.Red;
                                    Console.Write(t.Text);
                                }
                            Console.BackgroundColor = cc; Console.WriteLine();
                            Console.WriteLine("詳細: \n{0}", ex.ToString());
                        }
                        catch (RuntimeException ex) {
                            Console.WriteLine("実行エラーが発生しました。");
                            Console.WriteLine("エラー : " + ex.Message);
                            Console.WriteLine("トークン : {0}", ex.Token);
                            Console.WriteLine("詳細: \n{0}", ex.ToString());
                        }
                        break;

                    case '2':
                        foreach (var func in data.Setting.Functions) {
                            StringBuilder param = new StringBuilder();
                            foreach (var p in func.Value.Parameter) {
                                if (param.Length != 0)
                                    param.Append(", ");
                                param.Append(p.ToString());
                            }
                            Console.WriteLine("{0}({1}) - {2}",
                                func.Value.Name, param, func.Value.Description);
                        }
                        break;
                    case '3':
                        Console.WriteLine("Prior. L  Op   R - Description");
                        foreach (var op in setting.Spec.Operations.OrderBy(d => d.Value).Reverse()) {
                            Console.WriteLine("{0,6} {1} {2,5} {3} - {4}",
                                op.Value,
                                op.Key.RequireLeftParameter ? "#" : " ",
                                op.Key.Text,
                                op.Key.RequireRightParameter ? "#" : " ",
                                op.Key.Name);
                        }
                        break;
                    case '4':
                        if (formula == null) continue;
                        RuntimeData dd = data.Clone() as RuntimeData;
                        Stopwatch sw = Stopwatch.StartNew();
                        var res = formula.Eval(dd);
                        sw.Stop();
                        Console.WriteLine("Input  : " + analyzer.Line);
                        Console.WriteLine("Formula: " + formula.ToString());
                        Console.WriteLine("Result : " + (res == null ? "戻り値はありませんでした" : res.ToString()));
                        Console.WriteLine("Mathjax: " + (res == null ? "戻り値はありませんでした" : res.Output(OutputType.Mathjax)));
                        Console.WriteLine("Time   : " + sw.Elapsed.ToString());
                        Console.WriteLine("Variables:");
                        foreach (var block in data.Blocks) {
                            foreach (var item in block.Variables) {
                                Console.WriteLine("{0:10} : {1}", item.Key, item.Value);
                            }
                        }
                        break;
                    case '5':
                        if (formula == null) continue;
                        else {
                            Console.WriteLine("Input Addition Formula:");
                            Console.Write(" >");
                            Stopwatch sw3 = Stopwatch.StartNew();
                            Analyzer.Analyzer anal = new Analyzer.Analyzer(
                                Console.ReadLine(), setting);
                            Console.WriteLine("Success!");
                            var d = anal.GetResult();
                            var dres = d.Eval(data);
                            sw3.Stop();
                            Console.WriteLine("Done. (ReturnVal: " +
                                (dres == null ? "null" : dres.ToString()) + ")");
                            Console.WriteLine("Time : " + sw3.Elapsed.ToString());
                        }
                        break;
                    case '6':
                        if (formula == null) continue;
                        data.OutputDebug();
                        break;
                    case '7':
                        if (formula == null) continue;
                        Console.WriteLine("Input  : " + analyzer.Line);
                        Console.WriteLine("Output Expression: " + formula.GetType().Name);
                        OutputFormula(data, formula, 0);
                        break;

                    case '8':
                        if (formula == null) continue;
                        if (!setting.IsDebug) {
                            setting.IsDebug = true;
                            Console.WriteLine("Debug Enabled!");
                        }else {
                            setting.IsDebug = false;
                            Console.WriteLine("Debug Disabled!");
                        }
                        break;


                    case '9': return;
                    case 'w':
                        ToWritingMode(data);
                        break;
                    case 'l': {
                            {
                                setting = new RuntimeSetting();
                                if (LispMode) {
                                    Console.WriteLine("Lispモード");
                                    setting.DefaultSyntaxAnalyzer =
                                        typeof(FuncCalc.Lisp.SyntaxAnalyzer);
                                }
                                setting.AcceptBitLength = 4096;

                                data = setting.CreateNewRuntimedata();
                            }
                            var files = (new DirectoryInfo("./source")).GetFiles();
                            for (int i = 0; i < files.Length; i++) {
                                Console.WriteLine("{0,3}: {1}", i, files[i].Name);
                            }
                            Console.Write("Index > ");
                            using (StreamReader sr = new StreamReader(files[int.Parse(Console.ReadLine())].FullName)) {
                                ExecuteFormula(sr.ReadToEnd());
                            }
                        } break;
                    case 'c': {
                            setting = new RuntimeSetting();
                            if (LispMode) {
                                Console.WriteLine("Lispモード");
                                setting.DefaultSyntaxAnalyzer =
                                    typeof(FuncCalc.Lisp.SyntaxAnalyzer);
                            }
                            setting.AcceptBitLength = 4096;

                            data = setting.CreateNewRuntimedata();
                        }
                        Console.Write(" > ");
                        ExecuteFormula(Console.ReadLine());
                        break;
                    default:
                        continue;
                }
                Console.WriteLine();

            }

        }

        static void ExecuteFormula(string line) {

            try {
                if (string.IsNullOrEmpty(line)) return;
                analyzer = new Analyzer.Analyzer(
                    line, setting);
                var d = analyzer.GetResult();
                var dres = d.Eval(data);
                Console.WriteLine((dres == null ? "null" : dres.ToString()));
            }
            catch (FuncCalcException ex) {
                if (ex is SyntaxException)
                    Console.WriteLine("Syntax Error!");
                else
                    Console.WriteLine("Runtime Error");

                Console.WriteLine("エラー : " + ex.Message);
                Console.WriteLine("トークン : {0} [Line: {1}, Num: {2}, Index: {3}]", ex.Token,
                    ex.Token.Line, ex.Token.Number, ex.Token.Index);
                Console.Write("場所 : ");
                ConsoleColor cc = Console.BackgroundColor;
                if (analyzer.Tokens == null)
                    Console.WriteLine("トークン情報がありませんでした");
                else
                    foreach (var t in analyzer.Tokens) {
                        if (t != ex.Token) Console.BackgroundColor = cc;
                        else Console.BackgroundColor = ConsoleColor.Red;
                        Console.Write(t.Text);
                    }
                Console.BackgroundColor = cc; Console.WriteLine();
            }
        }

        static void OutputFormula(RuntimeData runtime, IFormula f, int level) {
            for (int i = 0; i < f.Count; i++) {
                if (f.Items[i] is IFormula) {
                    Console.WriteLine("{0}{1} [{2}]{3}",
                        "".PadLeft(level * 2, ' '),
                        i + 1,
                        f.Items[i].GetType().Name,
                        f.Items[i]);
                    OutputFormula(runtime, f.Items[i] as IFormula, level + 1);
                }
                else if (f.Items[i] is Operator) {
                    Operator e = (f.Items[i] as Operator);
                    Console.WriteLine("{0}{1}. {2}[{3}] => {4}",
                        "".PadLeft(level * 2, ' '),
                        i + 1,
                        f.Items[i],
                        f.Items[i].GetType().Name,
                        e.Evaluator.Name);
                }
                else {
                    IEval e = (f.Items[i] as IEval);
                    var ee = e != null ? e.Eval(runtime) : null;
                    Console.WriteLine("{0}{1}. {2}[{3}] => {4}",
                        "".PadLeft(level * 2, ' '),
                        i + 1,
                        f.Items[i],
                        f.Items[i].GetType().Name,
                        e != null ? ee.ToString() + "[" + ee.GetType().Name + "]" : "不明");
                }
            }
        }
        static void ToWritingMode(RuntimeData runtime)
        {
            
        }

    }
}
