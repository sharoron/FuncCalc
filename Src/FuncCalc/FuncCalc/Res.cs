using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FuncCalc
{
    internal static class GetRes
    {
        internal static string Text(string source, string name) {

            var asm = typeof(GetRes).Assembly;

            System.Resources.ResourceManager rm = new System.Resources.ResourceManager(
                asm.GetName().Name + "." + source,
                asm);

            return rm.GetString(name);
        }
        internal static string Text(string source, string name, params object[] objs) {
            return string.Format(
                GetRes.Text(source, name), objs.ToArray());
        }
    }
}
