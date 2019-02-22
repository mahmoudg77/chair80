using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace Chari80Admin.Libs
{
    public static class ErrorHandler
    {
        public static string Message(Exception ex,
                [CallerLineNumber] int lineNumber = 0,
                [CallerMemberName] string caller = null)
        {
            return ex.Message + " at line " + lineNumber + " (" + caller + ")";
        }
    }
}