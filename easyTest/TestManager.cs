using System;
using System.Collections.Generic;
using System.Threading;
using static easyLib.DebugHelper;

namespace easyTest
{
    public class TestManager
    {
        readonly List<ITest> m_tests = new List<ITest>();

        public void AddTest(ITest test)
        {
            Assert(test != null);

            m_tests.Add(test);
        }

        public void Execute(int passCount = 1)
        {
            Assert(passCount >= 1);

            if (m_tests.Count == 0)
            {
                Console.WriteLine("No test to execute");
                return;
            }

            int totalErr = 0;

            foreach (ITest t in m_tests)
            {
                for (int i = 0; i < passCount; ++i)
                {
                    Console.WriteLine($" ------- Running {t.Name}, pass {i}...");
                    Console.WriteLine("|");
                                        
                    IEnumerable<ITestResult> res = t.Run();                    
                    int nErr = 0;

                    foreach (ITestResult tr in res)
                    {
                        Console.WriteLine($"|> {tr.Caption}");

                        foreach (string msg in tr.Report)
                            Console.WriteLine($"|  {msg}");

                        if (tr.IsFailure)
                            ++nErr;

                        Console.WriteLine("|");
                    }

                    Console.WriteLine("|");
                    Console.Write(" - Done.");
                    
                    if (nErr <= 0)
                        Console.WriteLine(" (No Failure.)");
                    else
                        Console.WriteLine($" ({nErr} Failures.)");

                    Console.WriteLine();
                    totalErr += nErr;
                }
            }

            Console.WriteLine($"*** {m_tests.Count} test(s) executed.");
            Console.WriteLine($"*** {passCount} passes.");
            Console.WriteLine($"*** {totalErr} error(s).");
        }
    }
}
