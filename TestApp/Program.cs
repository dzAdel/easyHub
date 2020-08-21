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
            mgr.AddTest(new ADT.TreeNodeTest());

            mgr.Execute(SampleFactory.CreateBytes(1).First());
        }
    }
}
