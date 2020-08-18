using easyTest;
using Microsoft.VisualBasic;
using System;
using System.Runtime.CompilerServices;

namespace TestApp
{
    class Program
    {
        static void Main()
        {
            var mgr = new TestManager();
            mgr.AddTest(new MultiByteCodecTest());

            mgr.Execute(byte.MaxValue);
        }
    }
}
