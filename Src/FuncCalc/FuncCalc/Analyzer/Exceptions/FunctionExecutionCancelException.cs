using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalc.Exceptions
{

    [Serializable]
    internal class FunctionExecutionCancelException : Exception
    {
        public FunctionExecutionCancelException() { }
        public FunctionExecutionCancelException(string message) : base(message) { }
        public FunctionExecutionCancelException(string message, Exception inner) : base(message, inner) { }
        protected FunctionExecutionCancelException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
