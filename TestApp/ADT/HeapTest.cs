using easyLib.ADT.Heaps;
using easyLib.Test;
using System;
using System.Linq;

namespace TestApp.ADT
{
    class HeapTest : UnitTest
    {
        public HeapTest() :
            base("LinkedHeap<> test")
        { }


        //protected:
        protected override void Start()
        {
            LinkedHeapTest();
        }

        //private:
        static int RandCount => SampleFactory.CreateInts(0, 200).First();

        void LinkedHeapTest()
        {
            var heap = CreateHeap(RandCount);

            int lastItem = int.MinValue;

            while (!heap.IsEmpty)
            {
                var item = heap.Peek();
                Ensure(heap.Pop() == item);
                Ensure(lastItem <= item);
                lastItem = item;
            }

            Comparison<int> comp = (a, b) => a < b ? -1 : a == b ? 0 : 1;
            heap = CreateHeap(RandCount, comp);
            lastItem = int.MinValue;

            while (!heap.IsEmpty)
            {
                var item = heap.Peek();
                Ensure(heap.Pop() == item);
                Ensure(lastItem <= item);
                lastItem = item;
            }

            heap = CreateHeap(RandCount, (a, b) => comp(b, a));
            lastItem = int.MaxValue;


            while (!heap.IsEmpty)
            {
                var item = heap.Peek();
                Ensure(heap.Pop() == item);
                Ensure(lastItem >= item);
                lastItem = item;
            }

            Func<int, int, bool> before = (a, b) => a < b;
            heap = CreateHeap(RandCount, before);
            lastItem = int.MinValue;

            while (!heap.IsEmpty)
            {
                var item = heap.Peek();
                Ensure(heap.Pop() == item);
                Ensure(lastItem <= item);
                lastItem = item;
            }

            heap = CreateHeap(RandCount, (a, b) => before(b, a));
            lastItem = int.MaxValue;

            while (!heap.IsEmpty)
            {
                var item = heap.Peek();
                Ensure(heap.Pop() == item);
                Ensure(lastItem >= item);
                lastItem = item;
            }

            heap = CreateHeap(RandCount);
            int count = heap.Count;

            for (int i = 0; i < count; ++i)
                heap.Pop();

            Ensure(heap.IsEmpty);


            heap = CreateHeap(RandCount + 2);
            int n = heap.Count / 2;

            for(int i = 0; i < n;++i)
            {
                heap.Add(SampleFactory.NextInt);
                heap.Pop();
            }

            lastItem = int.MinValue;

            while (!heap.IsEmpty)
            {
                var item = heap.Peek();
                Ensure(heap.Pop() == item);
                Ensure(lastItem <= item);
                lastItem = item;
            }
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
