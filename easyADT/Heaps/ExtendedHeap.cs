using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static easyLib.DebugHelper;

namespace easyLib.ADT.Heaps
{
    public abstract class ExtendedHeap<T>: Heap<T>
    {
        public void Remove(T item)
        {
            Assert(Contains(item));

            RemoveItem(item);
        }


        //protected:
        protected abstract void RemoveItem(T item);

        protected override T PopItem()
        {
            T item = PeekItem();
            RemoveItem(item);
            return item;
        }
    }
}
