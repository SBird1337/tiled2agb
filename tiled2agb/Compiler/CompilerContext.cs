using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tiled2agb.Compiler
{
    public enum CompilerExitCode
    {
        EXIT_SUCCESS = 0,
        EXIT_FAILURE = -1
    }

    public class CompilerContext
    {
        public CompilerExitCode ExitCode { get; set; }
        public string CompilerError { get; set; }
        public Stack<string> CompilerWarnings { get; set; }

        public CompilerContext()
        {
            CompilerWarnings = new Stack<string>();
        }

        public void ExitError(string errorMessage, params object[] arguments)
        {
            CompilerError = string.Format(errorMessage, arguments);
            ExitCode = CompilerExitCode.EXIT_FAILURE;
        }

        public void PushWarning(string warningMessage, params object[] arguments)
        {
            CompilerWarnings.Push(string.Format(warningMessage, arguments));
        }
    }
}
