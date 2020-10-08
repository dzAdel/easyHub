using easyLib.Test;
using System;
using TestApp.ADT;
using TestApp.Extensions;

namespace TestApp
{
    class Program
    {
        TestManager m_mgr = new TestManager();

        static void Main()
        {
            var app = new Program();

            //app.EasyLibTest();
            //app.EasyLibIOTest();
            app.EasyLibADTTreesTest();

            app.m_mgr.Execute(new Random().Next(1, byte.MaxValue));
        }

        void EasyLibTest()
        {
            m_mgr.AddTest(new SampleFactoryTest());
            m_mgr.AddTest(new MultiByteCodecTest());
            m_mgr.AddTest(new ListExTest());
        }

        void EasyLibIOTest()
        {
            m_mgr.AddTest(new BinStreamTest());
        }

        void EasyLibADTTreesTest()
        {
            m_mgr.AddTest(new BasicTreeNodeTest());
            m_mgr.AddTest(new BasicTreeTest());
            m_mgr.AddTest(new BinaryTreeNodeTest());
            m_mgr.AddTest(new BinaryTreeTest());
            m_mgr.AddTest(new HeapTest());
        }

    }
}
