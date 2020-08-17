using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace easyLib
{
    public static class DebugHelper
    {
        [Conditional("DEBUG")]
        public static void Assert(bool exp, string msg = null,
            [CallerMemberName] string callerName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lnNber = 0)
        {
            if (!exp)
            {
                if (msg == null)
                    msg = "Assertion failure.";

                string txt = $"{msg}\nMethod: {callerName}\nFile: {filePath}\nLine: {lnNber}";
                Debug.Fail(txt);

                if (Debugger.IsAttached)
                    Debugger.Break();
            }
        }

        [Conditional("DEBUG")]
        public static void Log(this Exception ex,
            [CallerMemberName] string callerName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lnNber = 0)
        {
            Assert(ex != null);

            string txt = $"An exception occured\nMethod: {callerName}\nFile: {filePath}\nLine: {lnNber}";

            Debug.WriteLine(txt);
            PrintHelper(ex, 0);
        }


        //private:
        [Conditional("DEBUG")]
        static void PrintHelper(Exception ex, int indents)
        {
            string spaces = new string(' ', indents << 1);

            Debug.WriteLine(spaces + $"Type: {ex.GetType()}.");
            Debug.WriteLine(spaces + $"Site: {ex.TargetSite}");

            if (ex.InnerException != null)
            {
                Debug.WriteLine(spaces + "Inner exception:");
                PrintHelper(ex.InnerException, indents + 1);
            }
        }
    }
}
