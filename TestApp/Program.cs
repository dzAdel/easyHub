using easyLib.Test;
using System.Linq;

namespace TestApp
{
    class Program
    {
        static void Main()
        {
            var mgr = new TestManager();
            mgr.AddTest(new MultiByteCodecTest());
            mgr.AddTest(new BinStreamTest());
            mgr.AddTest(new ADT.BasicTreeNodeTest());
            mgr.AddTest(new ADT.BasicTreeTest());

            mgr.Execute(SampleFactory.CreateSBytes(1).First());
        }
    }
}
