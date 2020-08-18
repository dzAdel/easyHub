using System;
using System.Collections.Generic;
using static easyLib.DebugHelper;

namespace easyTest
{
    public abstract class FailureInfo : ITestResult
    {
        public string Caption { get; }

        public IEnumerable<string> Report
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(CallerName))
                    yield return $"In {CallerName}";

                if (!string.IsNullOrWhiteSpace(FilePath))
                    yield return $"File: {FilePath}";

                if (LineNumber > 0)
                    yield return $"Line Number: {LineNumber}";

                foreach (string ln in FailureLog)
                {
                    string[] msg = ln.Split(Environment.NewLine);

                    foreach (string s in msg)
                        if (!string.IsNullOrWhiteSpace(s))
                            yield return s;
                }
            }
        }

        public string CallerName { get; set; }
        public string FilePath { get; set; }
        public int LineNumber { get; set; }
        public bool IsFailure { get; }

        //protected:
        protected FailureInfo(string caption)
        {
            Assert(!string.IsNullOrWhiteSpace(caption));

            Caption = caption;
            IsFailure = true;
        }

        protected abstract IEnumerable<string> FailureLog { get; }
    }
}
