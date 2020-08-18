using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easyTest
{
    public interface ITestResult
    {
        string Caption { get; }
        IEnumerable<string> Report { get; }
        bool IsFailure { get; }
    }
}
