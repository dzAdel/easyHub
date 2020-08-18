using System.Collections.Generic;

namespace easyTest
{
    public interface ITest
    {
        string Name { get; }
        IEnumerable<ITestResult> Run();
    }
}
