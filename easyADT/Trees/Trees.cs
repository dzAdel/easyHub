using System;
using System.Collections.Generic;

namespace easyLib.ADT.Trees
{
    public enum TraversalOrder
    {
        InOrder,
        PreOrder,
        PostOrder,
        LevelOrder
    }

    public static class Trees
    {
        public static int InternalPathLength<TItem, TNode>(this ITree<TItem, TNode> tree)
            where TNode : INode<TItem>
            => throw new NotImplementedException();

        public static int ExternalPathLength<TItem, TNode>(this ITree<TItem, TNode> tree)
            where TNode : INode<TItem>
            => throw new NotImplementedException();

        public static int WeightedPathLength<TItem, TNode>(this ITree<TItem, TNode> tree)
            where TNode : INode<TItem>
            => throw new NotImplementedException();

        public static IEnumerable<TNode> Enumerate<TItem, TNode>(this   ITree<TItem, TNode> tree, TraversalOrder order)
            where TNode : INode<TItem>
            => throw new NotImplementedException();


    }
}
