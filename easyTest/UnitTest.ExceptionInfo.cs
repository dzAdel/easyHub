using System;
using System.Collections.Generic;
using static easyLib.DebugHelper;


namespace easyLib.Test
{
    partial class UnitTest
    {
        class ExceptionFailureInfo : FailureInfo
        {
            readonly Exception m_ex;

            public ExceptionFailureInfo(Type exWant, Exception exGot = null) :
                base("Exception test failure")
            {
                Assert(exWant != null);

                ExpectedType = exWant;
                m_ex = exGot;
            }

            public Type ExpectedType { get; }
            public Type CatchedType => m_ex?.GetType();


            //protected:
            protected override IEnumerable<string> FailureLog
            {
                get
                {
                    yield return $"Expected : {ExpectedType}";
                    yield return $"Got: {CatchedType?.ToString() ?? "None"}";

                    if (m_ex != null)
                        yield return $"Exception message: {m_ex.Message}";
                }
            }
        }

    }
}
