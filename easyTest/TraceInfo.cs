using System;
using System.Collections.Generic;

namespace easyLib.Test
{
    public sealed class TraceInfo : ITestResult
    {
        readonly List<string> m_lines = new List<string>();

        public TraceInfo(string msg)
        {
            if (string.IsNullOrEmpty(msg))
                Caption = "";
            else
            {
                string[] strs = msg.Replace(Environment.NewLine, "\n").Split('\n');
                Caption = strs[0];

                for (int i = 1; i < strs.Length; ++i)
                    m_lines.Add(strs[i]);
            }
        }

        public string Caption { get; }

        public IEnumerable<string> Report => m_lines;

        public bool IsFailure => false;

        public void AddLine(string line)
        {
            foreach (string s in line.Replace(Environment.NewLine, "\n").Split('\n'))
                m_lines.Add(s);
        }
    }
}
