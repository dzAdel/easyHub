using easyLib.ADT.Heaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static easyLib.DebugHelper;


namespace easyLib.ADT
{
    public interface IPriorityQueue<out T>
    {
        int Count { get; }
        bool IsEmpty { get; }
        T Peek();
        T Dequeue();
    }

    public sealed class PriorityQueue<T> : IPriorityQueue<T>
    {
        public enum QueueImpl
        {
            Flat,
            Linked
        }


        readonly ExtendedHeap<T> m_heap;


        public PriorityQueue(Comparison<T> comparison = null):
            this(QueueImpl.Linked, comparison)
        { }

        public PriorityQueue(int capacity, Comparison<T> comparison = null)
        {
            Assert(capacity >= 0);

            m_heap = new FlatHeap<T>(capacity, comparison ?? Comparer<T>.Default.Compare);
        }

        public PriorityQueue(QueueImpl impl, Comparison<T> comparison = null)
        {
            Assert(Enum.IsDefined(typeof(QueueImpl), impl));

            if (impl == QueueImpl.Flat)
                m_heap = new FlatHeap<T>(comparison ?? Comparer<T>.Default.Compare);
            else
                m_heap = new LinkedHeap<T>(comparison ?? Comparer<T>.Default.Compare);
        }


        public int Count => m_heap.Count;

        public bool IsEmpty => m_heap.IsEmpty;

        public T Peek()
        {
            Assert(!IsEmpty);

            return m_heap.Peek();
        }

        public T Dequeue()
        {
            Assert(!IsEmpty);

            return m_heap.Pop();            
        }

        public void Enqueue(T item) => m_heap.Add(item);        

        public void Remove(T item)
        {
            Assert(Contains(item));

            m_heap.Remove(item);
        }

        public bool Contains(T item) => m_heap.Contains(item);
    }
}
