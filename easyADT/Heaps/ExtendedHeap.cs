using System.Collections;
using System.Collections.Generic;
using static easyLib.DebugHelper;

namespace easyLib.ADT.Heaps
{
    public abstract class ExtendedHeap<T> : Heap<T>, IEnumerable<T>
    {
        public void Remove(T item)
        {
            Assert(Contains(item));

            RemoveItem(item);
        }

        public IEnumerator<T> GetEnumerator() => GetItemEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        //protected:
        protected abstract void RemoveItem(T item);
        protected abstract IEnumerator<T> GetItemEnumerator();

        protected override T PopItem()
        {
            T item = PeekItem();
            RemoveItem(item);
            return item;
        }

    }
}
