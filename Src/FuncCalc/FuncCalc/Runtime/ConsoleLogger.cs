using FuncCalc.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuncCalc.Runtime
{
    public class ConsoleLogger : ILogger
    {
        public OutputType OutputType
        {
            get
            {
                return OutputType.String;
            }
        }

        public void AddCondition(string str) {
            ConsoleColor cc = Console.ForegroundColor;
            ConsoleColor cc2 = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine("ただし、" + str);
            Console.ForegroundColor = cc;
            Console.BackgroundColor = cc2;
        }

        public void AddInfo(string str) {
            ConsoleColor cc = Console.ForegroundColor;
            ConsoleColor cc2 = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine("[Info]" + str);
            Console.ForegroundColor = cc;
            Console.BackgroundColor = cc2;
        }

        public void AddWarning(string str) {
            ConsoleColor cc = Console.ForegroundColor;
            ConsoleColor cc2 = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("[!!]" + str);
            Console.ForegroundColor = cc;
            Console.BackgroundColor = cc2;
        }

        public void AddWay(string str) {
            ConsoleColor cc = Console.ForegroundColor;
            ConsoleColor cc2 = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine("→ " + str);
            Console.ForegroundColor = cc;
            Console.BackgroundColor = cc2;
        }
    }
}
