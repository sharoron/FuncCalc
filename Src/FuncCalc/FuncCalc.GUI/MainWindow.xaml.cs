using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FuncCalc.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() {
            InitializeComponent();
            
        }

        string form = "";
        string input = "0";
        bool flag = false;

        void RefreshResult() {

            // if (result_web == null)
            if (result == null)
                return;

            form = formula.Text;

            StringBuilder html = new StringBuilder();
            using (StreamReader sr = new StreamReader("result.html")) {
                html.Append(sr.ReadToEnd());
            }

            try {
                Analyzer.Analyzer anal = new Analyzer.Analyzer(form);
                Runtime.RuntimeData d = new Runtime.RuntimeData(anal.Setting);
                var f = anal.GetResult();
                var res = f.Eval(d).Optimise(d);

                if (res == null)
                    result.Text = "NULL";
                //     result_web.NavigateToString(html.Replace("{0}", "NULL").ToString());
                else {
                    result.Text = res.ToString();
                    //StringBuilder res_mathjax = new StringBuilder(res.Output(Runtime.OutputType.Mathjax));
                    //res_mathjax.Insert(0, "$ ");
                    //res_mathjax.Append(" $");

                    //result_web.NavigateToString(html.Replace("{0}",
                    //   res_mathjax.ToString()).ToString());
                }
            }
            catch (Exception ex) {
                result.Text = ex.Message;
                //result_web.NavigateToString(html.Replace("{0}", ex.Message).ToString());
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {

            string text = (sender as Button).Content.ToString();
            formula.Text += text;

            form += (sender as Button).Content.ToString();
            form = formula.Text;

            RefreshResult();
        }

        private void OpButton_Click(object sender, RoutedEventArgs e) {
            Button_Click(sender, e);
        }

        private void formula_TextChanged(object sender, TextChangedEventArgs e) {
            RefreshResult();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            formula.Focus();
        }
    }
}
