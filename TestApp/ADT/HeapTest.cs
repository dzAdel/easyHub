using easyLib.ADT.Heaps;
using easyLib.Test;
using System;
using System.Linq;

namespace TestApp.ADT
{
    class HeapTest : UnitTest
    {
        public HeapTest() :
            base("Heap<> Test")
        { }


        //protected:
        protected override void Start()
        {
            LinkedHeapTest();
            FlatHeapTest();
        }

        //private:
        static int RandCount => SampleFactory.CreateInts(0, 200).First();

        void FlatHeapTest()
        {
            var heap = CreateFlatHeap(RandCount);

            int lastItem = int.MinValue;

            while (!heap.IsEmpty)
            {
                var item = heap.Peek();
                Ensure(heap.Pop() == item);
                Ensure(lastItem <= item);
                lastItem = item;
            }


            Comparison<int> comp = (a, b) => a < b ? -1 : a == b ? 0 : 1;
            heap = CreateFlatHeap(RandCount, comp);
            lastItem = int.MinValue;

            while (!heap.IsEmpty)
            {
                var item = heap.Peek();
                Ensure(heap.Pop() == item);
                Ensure(lastItem <= item);
                lastItem = item;
            }


            heap = CreateFlatHeap(RandCount, (a, b) => comp(b, a));
            lastItem = int.MaxValue;

            while (!heap.IsEmpty)
            {
                var item = heap.Peek();
                Ensure(heap.Pop() == item);
                Ensure(lastItem >= item);
                lastItem = item;
            }


            Func<int, int, bool> before = (a, b) => a < b;
            heap = CreateFlatHeap(RandCount, before);
            lastItem = int.MinValue;

            while (!heap.IsEmpty)
            {
                var item = heap.Peek();
                Ensure(heap.Pop() == item);
                Ensure(lastItem <= item);
                lastItem = item;
            }


            heap = CreateFlatHeap(RandCount, (a, b) => before(b, a));
            lastItem = int.MaxValue;

            while (!heap.IsEmpty)
            {
                var item = heap.Peek();
                Ensure(heap.Pop() == item);
                Ensure(lastItem >= item);
                lastItem = item;
            }

            heap = CreateFlatHeap(RandCount);
            int count = heap.Count;

            for (int i = 0; i < count; ++i)
                heap.Pop();

            Ensure(heap.IsEmpty);


            heap = CreateFlatHeap(RandCount + 2);
            int n = heap.Count / 2;

            for (int i = 0; i < n; ++i)
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


            heap = new FlatHeap<int>();
            var data = SampleFactory.CreateInts(0).Take(RandCount).ToArray();
            foreach (var datum in data)
                heap.Add(datum);

            Ensure(data.All(d => heap.Contains(d)));

            data = SampleFactory.CreateInts(limit: 0).Take(RandCount).ToArray();
            Ensure(data.All(d => !heap.Contains(d)));

            heap = new FlatHeap<int>();
            data = SampleFactory.CreateInts(0, 100).Take(RandCount + 2).ToArray();

            foreach (var datum in data)
                heap.Add(datum);


            var indices = SampleFactory.CreateInts(0, data.Length).Distinct().Take(data.Length / 2).ToArray();

            foreach (var ndx in indices)
                heap.Remove(data[ndx]);

            lastItem = int.MinValue;

            while (!heap.IsEmpty)
            {
                var item = heap.Peek();
                Ensure(heap.Pop() == item);
                Ensure(lastItem <= item);
                lastItem = item;
            }
        }

        void LinkedHeapTest()
        {
            var heap = CreateLinkedHeap(RandCount);

            int lastItem = int.MinValue;

            while (!heap.IsEmpty)
            {
                var item = heap.Peek();
                Ensure(heap.Pop() == item);
                Ensure(lastItem <= item);
                lastItem = item;
            }

            Comparison<int> comp = (a, b) => a < b ? -1 : a == b ? 0 : 1;
            heap = CreateLinkedHeap(RandCount, comp);
            lastItem = int.MinValue;

            while (!heap.IsEmpty)
            {
                var item = heap.Peek();
                Ensure(heap.Pop() == item);
                Ensure(lastItem <= item);
                lastItem = item;
            }

            heap = CreateLinkedHeap(RandCount, (a, b) => comp(b, a));
            lastItem = int.MaxValue;


            while (!heap.IsEmpty)
            {
                var item = heap.Peek();
                Ensure(heap.Pop() == item);
                Ensure(lastItem >= item);
                lastItem = item;
            }

            Func<int, int, bool> before = (a, b) => a < b;
            heap = CreateLinkedHeap(RandCount, before);
            lastItem = int.MinValue;

            while (!heap.IsEmpty)
            {
                var item = heap.Peek();
                Ensure(heap.Pop() == item);
                Ensure(lastItem <= item);
                lastItem = item;
            }

            heap = CreateLinkedHeap(RandCount, (a, b) => before(b, a));
            lastItem = int.MaxValue;

            while (!heap.IsEmpty)
            {
                var item = heap.Peek();
                Ensure(heap.Pop() == item);
                Ensure(lastItem >= item);
                lastItem = item;
            }

            heap = CreateLinkedHeap(RandCount);
            int count = heap.Count;

            for (int i = 0; i < count; ++i)
                heap.Pop();

            Ensure(heap.IsEmpty);


            heap = CreateLinkedHeap(RandCount + 2);
            int n = heap.Count / 2;

            for (int i = 0; i < n; ++i)
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


            heap = new LinkedHeap<int>();
            var data = SampleFactory.CreateInts(0).Take(RandCount).ToArray();
            foreach (var datum in data)
                heap.Add(datum);

            Ensure(data.All(d => heap.Contains(d)));

            data = SampleFactory.CreateInts(limit: 0).Take(RandCount).ToArray();
            Ensure(data.All(d => !heap.Contains(d)));

            heap = new LinkedHeap<int>();
            data = SampleFactory.CreateInts(0, 100).Take(RandCount + 2).ToArray();

            foreach (var datum in data)
                heap.Add(datum);


            var indices = SampleFactory.CreateInts(0, data.Length).Distinct().Take(data.Length / 2).ToArray();

            foreach (var ndx in indices)
                heap.Remove(data[ndx]);

            lastItem = int.MinValue;

            while (!heap.IsEmpty)
            {
                var item = heap.Peek();
                Ensure(heap.Pop() == item);
                Ensure(lastItem <= item);
                lastItem = item;
            }

        }


        static LinkedHeap<int> CreateLinkedHeap(int itemCount, Func<int, int, bool> before = null)
        {
            var heap = new LinkedHeap<int>(before);

            foreach (int n in SampleFactory.CreateInts(0, 100).Take(itemCount))
                heap.Add(n);

            return heap;
        }

        static LinkedHeap<int> CreateLinkedHeap(int itemCount, Comparison<int> comparison)
        {
            var heap = new LinkedHeap<int>(comparison);

            foreach (int n in SampleFactory.CreateInts(0, 100).Take(itemCount))
                heap.Add(n);

            return heap;
        }

        static FlatHeap<int> CreateFlatHeap(int itemCount, Func<int, int, bool> before = null)
        {
            var heap = SampleFactory.NextBool ? new FlatHeap<int>(itemCount, before) : new FlatHeap<int>(before);

            foreach (int n in SampleFactory.CreateInts(0, 100).Take(itemCount))
                heap.Add(n);

            return heap;
        }

        static FlatHeap<int> CreateFlatHeap(int itemCount, Comparison<int> comparison)
        {
            var heap = SampleFactory.NextBool ? new FlatHeap<int>(itemCount, comparison) : new FlatHeap<int>(comparison);

            foreach (int n in SampleFactory.CreateInts(0, 100).Take(itemCount))
                heap.Add(n);
            
            return heap;
        }
    }
}
