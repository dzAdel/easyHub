using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

        public bool Contains(T item)
        {
            if (IsEmpty)
                return false;

            int lvlLast = -1;
            bool lookNextLevel = true;

            foreach (var (value, level) in LevelOrderTraversal())
            {
                bool itemAfter = Before(value, item);

                if (!Before(item, value) && !itemAfter)
                    return true;

                if (lvlLast != level)
                {
                    if (!lookNextLevel)
                        break;

                    lookNextLevel = false;
                    lvlLast = level;
                }

                if (itemAfter)
                    lookNextLevel = true;
            }

            return false;
        }


        //protected:
        protected abstract Func<T, T, bool> Before { get; }        
        protected abstract int GetItemCount();
        protected abstract T PeekItem();
        protected abstract T PopItem();
        protected abstract void AddItem(T item);        
        protected abstract IEnumerable<(T Value, int Level)> LevelOrderTraversal();

        protected bool Same(T a, T b) => !Before(a, b) && !Before(b, a);
    }
}
