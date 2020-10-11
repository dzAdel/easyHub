using System;
using System.Collections.Generic;
using System.Linq;
using static easyLib.DebugHelper;


namespace easyLib.ADT.Heaps
{
    public sealed class FlatHeap<T> : ExtendedHeap<T>
    {
        readonly List<T> m_items;
        
        public FlatHeap(Func<T, T, bool> before = null)
        {
            m_items = new List<T>();
            Before = before ?? ((a, b) => Comparer<T>.Default.Compare(a, b) < 0);
        }

        
        public FlatHeap(Comparison<T> comparison)
        {
            Assert(comparison != null);

            m_items = new List<T>();
            Before = (a, b) => comparison(a, b) < 0;

            Assert(ClassInvariant);
        }

        public FlatHeap(int intCapacity, Func<T, T, bool> before = null)
        {
            Assert(intCapacity >= 0);

            m_items = new List<T>(intCapacity);
            Before = before ?? ((a, b) => Comparer<T>.Default.Compare(a, b) < 0);

            Assert(ClassInvariant);
        }

        public FlatHeap(int intCapacity, Comparison<T> comparison)
        {
            Assert(intCapacity >= 0);
            Assert(comparison != null);

            Before = (a, b) => comparison(a, b) < 0;
            m_items = new List<T>(intCapacity);

            Assert(ClassInvariant);
        }


        //protected:
        protected override Func<T, T, bool> Before { get; }
        protected override int GetItemCount() => m_items.Count;
        protected override T PeekItem() => m_items[0];

        protected override void AddItem(T item)
        {
            m_items.Add(item);            
            BubbleUp(m_items.Count - 1);
        }        

        protected override void RemoveItem(T item)
        {
            int ndx = Same(m_items[0], item) ? 0 : LevelOrderTraversal().
                Select((p, Index) => (p.Value, Index)).
                First(p => Same(p.Value, item)).Index;

            int ndxLast = m_items.Count - 1;
            m_items[ndx] = m_items[ndxLast];            
            m_items.RemoveAt(ndxLast);

            if (ndx != ndxLast)            
                if (ndx != 0 && Before(m_items[ndx], m_items[GetParentIndex(ndx)]))
                    BubbleUp(ndx);
                else
                    BubbleDown(ndx);            
        }

        protected override IEnumerable<(T Value, int Level)> LevelOrderTraversal()
        {
            if (!IsEmpty)
            {
                var queue = new Queue<(int, int)>();

                queue.Enqueue((0, 0));

                while (queue.Count > 0)
                {
                    var (ndx, lvl) = queue.Dequeue();
                    yield return (m_items[ndx], lvl);

                    int ndxChild = GetLeftChildIndex(ndx);

                    if (ndxChild < m_items.Count)
                    {
                        queue.Enqueue((ndxChild, lvl + 1));

                        ndxChild = GetRightChildIndex(ndx);

                        if (ndxChild < m_items.Count)
                            queue.Enqueue((ndxChild, lvl + 1));
                    }
                }
            }
        }


        //private:        
        void BubbleUp(int index)
        {
            while (index != 0)
            {
                int ndxParent = GetParentIndex(index);

                if (!Before(m_items[index], m_items[ndxParent]))
                    break;

                (m_items[index], m_items[ndxParent]) = (m_items[ndxParent], m_items[index]);
                index = ndxParent;

            } 
        }

        void BubbleDown(int index)
        {
            int ndxLast = m_items.Count - 1;
            
                while (true)
                {
                    int ndxLeft = GetLeftChildIndex(index);

                    if (ndxLast < ndxLeft)
                        break;
                    else
                    {
                        int ndxRight = GetRightChildIndex(index);
                        int ndxChild = ndxLast < ndxRight ? ndxLeft :
                            Before(m_items[ndxLeft], m_items[ndxRight]) ? ndxLeft : ndxRight;

                        if (!Before(m_items[ndxChild], m_items[index]))
                            break;

                        (m_items[index], m_items[ndxChild]) = (m_items[ndxChild], m_items[index]);
                        index = ndxChild;
                    }

                }
        }

        static int GetParentIndex(int ndx) => (ndx - 1) >> 1;
        static int GetLeftChildIndex(int ndx) => 1 + (ndx << 1);
        static int GetRightChildIndex(int ndx) => 2 + (ndx << 1);

        bool ClassInvariant => IsEmpty || m_items.Skip(1).
            Select((item, ndx) => (item, ndx)).
            All(p => !Before(p.item, m_items[GetParentIndex(p.ndx)]));
    }
}
