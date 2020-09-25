using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using static easyLib.DebugHelper;


namespace easyLib.Test
{
    public abstract partial class UnitTest : ITest
    {

        List<ITestResult> m_results = new List<ITestResult>();


        public string Name { get; }

        public IEnumerable<ITestResult> Run()
        {
            try
            {
                m_results.Clear();
                Start();
            }
            catch (Exception ex)
            {
                m_results.Add(new RTExceptionInfo(ex));
            }

            return m_results.ToArray();
        }


        //protected:
        public UnitTest(string name)
        {
            Assert(!string.IsNullOrWhiteSpace(name));

            Name = name;
        }

        protected abstract void Start();


        protected bool Ensure(bool exp,
            [CallerMemberName] string caller = null,
            [CallerFilePath] string file = null,
            [CallerLineNumber] int line = 0,
            [CallerArgumentExpression("exp")] string testExp = null)
        {
            if (!exp)
            {
                var res = new AssertionFailureInfo
                {
                    CallerName = caller,
                    FilePath = file,
                    LineNumber = line,
                    Expression = testExp
                };

                m_results.Add(res);
            }

            return exp;
        }

        protected bool EnsureThrow<T>(Action act,
                [CallerMemberName] string caller = null,
                [CallerFilePath] string file = null,
                [CallerLineNumber] int line = 0)
            where T : Exception
        {
            bool ok = false;

            try
            {
                act();

                var res = new ExceptionFailureInfo(typeof(T))
                {
                    CallerName = caller,
                    FilePath = file,
                    LineNumber = line
                };

                m_results.Add(res);
            }
            catch (Exception ex)
            {
                if (!(ex is T))
                {
                    var res = new ExceptionFailureInfo(typeof(T), ex)
                    {
                        CallerName = caller,
                        FilePath = file,
                        LineNumber = line
                    };

                    m_results.Add(res);
                }
                else
                    ok = true;
            }

            return ok;
        }

        protected void Trace(string msg, params string[] lines)
        {
            var ti = new TraceInfo(msg);


            foreach (string s in lines)
                ti.AddLine(s);

            m_results.Add(ti);

            var sb = new StringBuilder();

            sb.AppendLine(ti.Caption);
            foreach (string s in ti.Report)
                sb.AppendLine(s);

            System.Diagnostics.Debug.WriteLine(sb.ToString());
        }
    }
}
