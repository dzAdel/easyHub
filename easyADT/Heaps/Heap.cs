using System.Collections;
using System.Collections.Generic;
using static easyLib.DebugHelper;


namespace easyLib.ADT.Heaps
{
    public interface IHeap<out T>
    {
        int Count { get; }
        bool IsEmpty { get; }
        T Peek();
        T Pop();
    }
    //------------------------------------------------------

    public abstract class Heap<T> : IHeap<T>
    {
        public int Count => GetItemCount();

        public bool IsEmpty => GetItemCount() == 0;

        public void Add(T item) => AddItem(item);
        
        public T Peek() 
        {
            Assert(!IsEmpty);

            return PeekItem();
        }

        public T Pop()
        {
            Assert(!IsEmpty);

            return PopItem();
        }
        

        //protected:
        protected abstract int GetItemCount();
        protected abstract T PeekItem();
        protected abstract T PopItem();
        protected abstract void AddItem(T item);
    }
}
