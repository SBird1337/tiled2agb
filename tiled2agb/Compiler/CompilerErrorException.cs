using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tiled2agb.Compiler
{
    public class CompilerErrorException : Exception
    {
        public CompilerErrorException(string message) : base(message)
        { }

        public CompilerErrorException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
