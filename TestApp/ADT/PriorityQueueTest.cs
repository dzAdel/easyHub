using easyLib.ADT;
using easyLib.Test;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace TestApp.ADT
{
    public class PriorityQueueTest : UnitTest
    {
        public PriorityQueueTest() :
            base("PriorityQueue<> Test")
        { }

        //protected:
        protected override void Start()
        {
            ConstructionTest();
            ContainsRemovalTest();
        }

        //private: 
        int RandCount => SampleFactory.CreateInts(0, 500).First();

        void ConstructionTest()
        {
            int count = RandCount;
            var pq = CreateFlatPriorityQueue(count);

            Ensure(count == pq.Count);
            Ensure(count > 0 || pq.IsEmpty);
            
            var items = new List<int>(count);
            while(!pq.IsEmpty)
            {
                var nextItem = pq.Peek();

                if (!Ensure(nextItem == pq.Dequeue()))
                    break;
                
                items.Add(nextItem);
            }
            
            Ensure(IsSorted(items));

            //---------------------

            count = RandCount;
            pq = CreateLinkedPriorityQueue(count);

            Ensure(count == pq.Count);
            Ensure(count > 0 || pq.IsEmpty);

            items = new List<int>(count);
            while (!pq.IsEmpty)
            {
                var nextItem = pq.Peek();

                if (!Ensure(nextItem == pq.Dequeue()))
                    break;

                items.Add(nextItem);
            }

            Ensure(IsSorted(items));

            //-------------

            pq = CreatePriorityQueue(comp: (a,b) => Comparer<int>.Default.Compare(b,a));

            
            items = new List<int>(pq.Count);
            while (!pq.IsEmpty)
            {
                var nextItem = pq.Peek();

                if (!Ensure(nextItem == pq.Dequeue()))
                    break;

                items.Add(nextItem);
            }

            items.Reverse();
            Ensure(IsSorted(items));
        }

        void ContainsRemovalTest()
        {
            PriorityQueue<int> pq;
            int count = RandCount + 2;

            if (SampleFactory.NextBool)
                pq = new PriorityQueue<int>(PriorityQueue<int>.QueueImpl.Flat);
            else
                pq = new PriorityQueue<int>(PriorityQueue<int>.QueueImpl.Linked);


            var dataSrc = SampleFactory.CreateInts().Take(count).ToList();
            foreach (var item in dataSrc)
                pq.Enqueue(item);

            Ensure(dataSrc.All(x => pq.Contains(x)));

            var delItems = new List<int>();
            foreach(var ndx in SampleFactory.CreateInts(0, dataSrc.Count).Distinct().Take(dataSrc.Count/ 2))
            {
                var item = dataSrc[ndx];
                pq.Remove(item);                
                delItems.Add(ndx);
            }

            var qry = dataSrc.Select((x, idx) => (x, idx)).Where(p => !delItems.Contains(p.idx)).Select(p => p.x);

            Ensure(qry.All(x => pq.Contains(x)));
            Ensure(delItems.All(x => !pq.Contains(dataSrc[x])));


            dataSrc.Clear();

            while (!pq.IsEmpty)
                dataSrc.Add(pq.Dequeue());

            Ensure(IsSorted(dataSrc));
        }

        PriorityQueue<int> CreateLinkedPriorityQueue(int itemCount)
        {
            var pq = new PriorityQueue<int>();
            var items = SampleFactory.CreateInts(0, 100).Take(itemCount);

            foreach (var item in items)
                pq.Enqueue(item);

            return pq;
        }

        PriorityQueue<int> CreateFlatPriorityQueue(int itemCount)
        {
            var pq = new PriorityQueue<int>(itemCount);
            var items = SampleFactory.CreateInts(0, 100).Take(itemCount);

            foreach (var item in items)
                pq.Enqueue(item);

            return pq;
        }

        PriorityQueue<int> CreatePriorityQueue(int count = -1, Comparison<int> comp = null)
        {
            PriorityQueue<int> pq;

            if (count < 0)
                count = RandCount;

            if (SampleFactory.NextBool)
                pq = new PriorityQueue<int>(PriorityQueue<int>.QueueImpl.Flat, comp);
            else
                pq = new PriorityQueue<int>(PriorityQueue<int>.QueueImpl.Linked, comp);

            var items = SampleFactory.CreateInts(0, 100).Take(count);

            foreach (var item in items)
                pq.Enqueue(item);

            return pq;
        }
        
        bool IsSorted(IEnumerable<int> data)
        {
            var items = data.ToArray();
            var sortedItems = items.OrderBy(x => x);

            return items.SequenceEqual(sortedItems);
        }
    }
}
