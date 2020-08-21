using System.Collections.Generic;

namespace easyLib.Test
{
    public interface ITestResult
    {
        string Caption { get; }
        IEnumerable<string> Report { get; }
        bool IsFailure { get; }
    }
}
