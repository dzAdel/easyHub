using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static easyLib.DebugHelper;


namespace easyTest
{
    public abstract partial class UnitTest : ITest
    {

        List<FailureInfo> m_results = new List<FailureInfo>();


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

            return m_results;
        }


        //protected:
        public UnitTest(string name)
        {
            Assert(!string.IsNullOrWhiteSpace(name));

            Name = name;
        }

        protected abstract void Start();


        protected void Ensure(bool exp,
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
        }

        protected void EnsureThrow<T>(Action act,
                [CallerMemberName] string caller = null,
                [CallerFilePath] string file = null,
                [CallerLineNumber] int line = 0)
            where T : Exception
        {
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
            }

        }
    }
}
