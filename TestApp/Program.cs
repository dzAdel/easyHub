using easyLib.Test;
using System;
using System.Linq;
using TestApp.ADT;

namespace TestApp
{
    class Program
    {
        TestManager m_mgr = new TestManager();

        static void Main()
        {
            var app = new Program();

            app.EasyLibADTTreesTest();            

            app.m_mgr.Execute(new Random().Next(1, byte.MaxValue));
        }

        void EasyLibTest()
        {
            m_mgr.AddTest(new SampleFactoryTest());
            m_mgr.AddTest(new MultiByteCodecTest());            
        }

        void EasyLibIOTest()
        {
            m_mgr.AddTest(new SampleFactoryTest());
            m_mgr.AddTest(new MultiByteCodecTest());
            m_mgr.AddTest(new BinStreamTest());
        }

        void EasyLibADTTreesTest()
        {
            //m_mgr.AddTest(new SampleFactoryTest());
            //m_mgr.AddTest(new ADT.BasicTreeNodeTest());
            //m_mgr.AddTest(new ADT.BasicTreeTest());
            //m_mgr.AddTest(new ADT.BinaryTreeNodeTest());
            m_mgr.AddTest(new BinaryTreeTest());
        }

    }
}
