using System.Collections.Generic;

namespace easyLib.Test
{
    public interface ITest
    {
        string Name { get; }
        IEnumerable<ITestResult> Run();
    }
}
