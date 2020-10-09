using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static easyLib.DebugHelper;

namespace easyLib.Test
{
    public class TestManager
    {
        readonly List<ITest> m_tests = new List<ITest>();
        readonly Dictionary<string, List<IEnumerable<ITestResult>>> m_reports =
            new Dictionary<string, List<IEnumerable<ITestResult>>>();


        public void AddTest(ITest test)
        {
            Assert(test != null);

            m_tests.Add(test);
        }

        public void Execute(int passCount = 1)
        {
            Assert(passCount > 0);

            m_reports.Clear();

            if (m_tests.Count == 0)
            {
                Console.WriteLine("No test to execute");
                return;
            }


            int LastRrow = Console.CursorTop;
            Console.WriteLine($"Running {passCount} passes tests.");

            Console.CursorVisible = false;            
            Parallel.ForEach(m_tests, RunTest);
            Console.CursorVisible = true;

            Console.CursorTop = ++LastRrow;            
            Console.WriteLine("All tests finished. Press any key to continue.");
            Console.ReadKey();

            if (LogRepport(Console.Out) > 0)
            {
                string fName = "AppTestLog.txt";

                using (TextWriter txtWrier = File.CreateText(fName))
                {
                    txtWrier.WriteLine(DateTime.Now);
                    txtWrier.WriteLine();

                    LogRepport(txtWrier);
                }

                var psi = new ProcessStartInfo(fName)
                {
                    UseShellExecute = true
                };

                Process.Start(psi);
            }


            //--------------------
            void RunTest(ITest tst)
            {
                var results = new List<IEnumerable<ITestResult>>();

                lock (m_reports)
                    m_reports.Add(tst.Name, results);

                var sw = new Stopwatch();

                int row = Interlocked.Increment(ref LastRrow);

                lock (Console.Out)
                {
                    Console.CursorTop = row;
                    Console.WriteLine($"Running {tst.Name}...");
                }


                sw.Start();

                for (int i = 0; i < passCount; ++i)
                {
                    IEnumerable<ITestResult> strs = tst.Run();
                    results.Add(strs);
                }

                sw.Stop();

                lock (Console.Out)
                {
                    Console.CursorTop = row;
                    Console.WriteLine($"{tst.Name} done. {FormatTime(sw.ElapsedMilliseconds)}");
                }
            }
        }

        public static string FormatTime(long ms)
        {
            var ts = TimeSpan.FromMilliseconds(ms);
            var sb = new StringBuilder("(");

            if (ts.Hours > 0)
                sb.Append(ts.Hours).Append("h:").Append(ts.Minutes).Append("m:").Append(ts.Seconds).Append('s');
            else if (ts.Minutes > 0)
                sb.Append(ts.Minutes).Append("m:").Append(ts.Seconds).Append('s');
            else if (ts.Seconds > 0)
                sb.Append(ts.Seconds).Append("s:").Append(ts.Milliseconds).Append("ms");
            else
                sb.Append(ts.Milliseconds).Append("ms");

            sb.Append(')');

            return sb.ToString();
        }

        //private:

        int LogRepport(TextWriter txtWriter)
        {
            int nErr = 0;

            foreach (string key in m_reports.Keys)
            {
                txtWriter.WriteLine($"*** {key}");
                List<IEnumerable<ITestResult>> tstResults = m_reports[key];

                int passCount = 1;
                foreach (IEnumerable<ITestResult> passResult in tstResults)
                {
                    txtWriter.WriteLine($"Pass: {passCount++}");

                    foreach (ITestResult tr in passResult)
                    {
                        txtWriter.WriteLine($"\t{tr.Caption}");

                        foreach (string str in tr.Report)
                            txtWriter.WriteLine($"\t{str}");

                        if (tr.IsFailure)
                            ++nErr;

                        txtWriter.WriteLine();
                    }
                }

                txtWriter.WriteLine($"*** {key} done.");
                txtWriter.WriteLine();
            }

            txtWriter.WriteLine($"*** {m_tests.Count} test(s) executed.");
            Console.WriteLine($"*** {nErr} error(s).");

            return nErr;
        }

    }
}
