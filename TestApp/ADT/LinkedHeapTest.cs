using easyLib.ADT.Heaps;
using easyLib.Test;
using System;
using System.Linq;

namespace TestApp.ADT
{
    class LinkedHeapTest : UnitTest
    {
        public LinkedHeapTest() :
            base("LinkedHeap<> test")
        { }


        //protected:
        protected override void Start()
        {
            HeapCreationTest();
        }

        //private:
        static int RandCount => SampleFactory.CreateInts(0, 500).First();

        void HeapCreationTest()
        {
            var heap = CreateHeap(RandCount);
            Ensure(heap.Nodes.Skip(1).All(nd => nd.Parent.Item <= nd.Item));

            Comparison<int> comp = (a, b) => a < b ? -1 : a == b ? 0 : 1;

            heap = CreateHeap(RandCount, comp);
            Ensure(heap.Nodes.Skip(1).All(nd => nd.Item >= nd.Parent.Item));

            heap = CreateHeap(RandCount, (a, b) => comp(b, a));
            Ensure(heap.Nodes.Skip(1).All(nd => nd.Item <= nd.Parent.Item));

            Func<int, int, bool> before = (a, b) => a < b;

            heap = CreateHeap(RandCount, before);
            Ensure(heap.Nodes.Skip(1).All(nd => nd.Item >= nd.Parent.Item));

            heap = CreateHeap(RandCount, (a, b) => before(b, a));
            Ensure(heap.Nodes.Skip(1).All(nd => nd.Item <= nd.Parent.Item));
        }


        static LinkedHeap<int> CreateHeap(int itemCount, Func<int, int, bool> before = null)
        {
            var heap = new LinkedHeap<int>(before);

            foreach (int n in SampleFactory.CreateInts().Take(itemCount))
                heap.Add(n);

            return heap;
        }

        static LinkedHeap<int> CreateHeap(int itemCount, Comparison<int> comparison)
        {
            var heap = new LinkedHeap<int>(comparison);

            foreach (int n in SampleFactory.CreateInts().Take(itemCount))
                heap.Add(n);

            return heap;
        }
    }
}
