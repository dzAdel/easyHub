using System;
using System.Collections.Generic;

namespace easyLib.ADT.Trees
{
    public interface ISearchTree<T, K, N> : ITree<T, N>
            where N : ITreeNode<T>
    {
        Comparison<K> KeyCompare { get; }
        Func<T, K> KeySelect { get; }
        int Count { get; }
        IEnumerable<K> Keys { get; }
        bool ContainsKey(K key);
        N Min();
        N Max();        
        N Ceiling(K Key);
        N Floor(K key);
        N Locate(K key);
        IEnumerable<N> GetRange(K loKey, K hiKey);
        N GetPredecessor(N node);
        N GetSuccessor(N node);
    }
}

