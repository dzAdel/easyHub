using System.Collections.Generic;

namespace easyTest
{
    public interface ITestResult
    {
        string Caption { get; }
        IEnumerable<string> Report { get; }
        bool IsFailure { get; }
    }
}
