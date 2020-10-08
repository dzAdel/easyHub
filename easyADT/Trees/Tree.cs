using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static easyLib.DebugHelper;


namespace easyLib.ADT.Trees
{
    public enum TraversalOrder
    {
        PreOrder,
        InOrder,
        PostOrder,
        BreadthFirst
    }
    //-------------------------------------

    public interface ITree<out T, out N> : IEnumerable<T>
        where N : ITreeNode<T>
    {
        N Root { get; }
        IEnumerable<N> Nodes { get; }
        IEnumerable<N> Leaves { get; }
        bool IsEmpty { get; }
        IEnumerable<N> Enumerate(TraversalOrder order);
        int GetCount();        
    }
    //-------------------------------------------------


    public static class Trees
    {
        public static int GetInternalPathLength<T, N>(this ITree<T, N> tree)
            where N : ITreeNode<T>
        {
            Assert(tree != null);
            Assert(!tree.IsEmpty);

            if (tree.Root.IsLeaf)
                return 0;

            var queue = new Queue<(N, int)>();
            int len = 0;

            queue.Enqueue((tree.Root, 0));

            while (queue.Count > 0)
            {
                (N node, int h) = queue.Dequeue();
                len += h;

                foreach (N child in node.Children)
                    if (!child.IsLeaf)
                        queue.Enqueue((child, h + 1));
            }

            return len;
        }

        public static int GetExternalPathLength<T, N>(this ITree<T, N> tree)
            where N : ITreeNode<T>
        {
            Assert(tree != null);
            Assert(!tree.IsEmpty);

            if (tree.Root.IsLeaf)
                return 0;

            var queue = new Queue<(N, int)>();
            int len = 0;

            queue.Enqueue((tree.Root, 0));

            while (queue.Count > 0)
            {
                (N node, int h) = queue.Dequeue();

                if (node.IsLeaf)
                    len += h;
                else
                    foreach (N child in node.Children)
                        queue.Enqueue((child, h + 1));
            }

            return len;
        }

        public static int GetWeightedExternalPathLength<T, N>(this ITree<T, N> tree, Func<N, int> leafWeight)
            where N : ITreeNode<T>
        {
            Assert(tree != null);
            Assert(!tree.IsEmpty);
            Assert(leafWeight != null);

            if (tree.Root.IsLeaf)
                return 0;

            var queue = new Queue<(N, int)>();
            int len = 0;

            queue.Enqueue((tree.Root, 0));

            while (queue.Count > 0)
            {
                (N node, int h) = queue.Dequeue();

                if (node.IsLeaf)
                    len += h * leafWeight(node);
                else
                    foreach (N child in node.Children)
                        queue.Enqueue((child, h + 1));
            }

            return len;
        }

        public static IEnumerable<ITreeNode<T>> Enumerate<T, N>(this ITree<T, N> tree, TraversalOrder order)
            where N : ITreeNode<T>
        {
            Assert(tree != null);
            Assert(Enum.IsDefined(typeof(TraversalOrder), order));

            if (tree.IsEmpty)
                return Enumerable.Empty<ITreeNode<T>>();

            N root = tree.Root;

            if (!root.IsLeaf)
                switch (order)
                {
                    case TraversalOrder.PreOrder:
                        return PreOrderTraversal(root);

                    case TraversalOrder.PostOrder:
                        return PostOrderTraversal(root);

                    case TraversalOrder.InOrder:
                        return InOrderTraversal(root);

                    case TraversalOrder.BreadthFirst:
                        return LevelOrderTraversal(root).Select(pair => pair.node);
                }

            return Enumerable.Repeat<ITreeNode<T>>(root, 1);
        }

        public static IEnumerable<(ITreeNode<T> node, int level)> LevelOrderTraversal<T, N>(this ITree<T, N> tree)
            where N : ITreeNode<T>
        {
            Assert(tree != null);

            if (tree.IsEmpty)
                return Enumerable.Empty<(ITreeNode<T>, int)>();

            return LevelOrderTraversal(tree.Root);
        }

        public static int GetHeight<T, N>(this ITree<T, N> tree)
            where N : ITreeNode<T>
        {
            Assert(tree != null);
            Assert(!tree.IsEmpty);

            if (tree.Root.Degree == 0)
                return 0;

            var heights = new int[tree.Root.Degree];

            Parallel.ForEach(tree.Root.Children, (node, _, ndx)
                => heights[ndx] = GetHeight(node));

            return heights.Max() + 1;

            //---
            int GetHeight(ITreeNode<T> node)
            {
                if (node.IsLeaf)
                    return 0;

                int h = 0;
                foreach (var nd in node.Children)
                    h = Math.Max(h, GetHeight(nd));

                return h + 1;
            }
        }

        public static bool Contains<T, N>(this ITree<T, N> tree, ITreeNode<T> node)
            where N : ITreeNode<T>
        {
            Assert(tree != null);
            Assert(node != null);

            ITreeNode<T> root = tree.Root;

            do
            {
                if (node == root)
                    return true;

                node = node.Parent;

            } while (node != null);

            return false;
        }

        public static int GetDepth<T, N>(this ITree<T,N> tree, ITreeNode<T> node)
            where N: ITreeNode<T>
        {
            Assert(tree != null);
            Assert(node != null);
            Assert(tree.Contains(node));

            return node.GetPath(tree.Root).Count() - 1;
        }


        //private:
        static IEnumerable<ITreeNode<T>> PreOrderTraversal<T>(ITreeNode<T> root)
        {
            IEnumerable<ITreeNode<T>> res = Enumerable.Repeat(root, 1);
            int childCount = root.Degree;

            if (childCount == 1)
                res = res.Concat(PreOrder(root.Children.Single()));
            else
            {
                var seqs = new IEnumerable<ITreeNode<T>>[childCount];

                Parallel.ForEach(root.Children, (node, _, ndx)
                    => seqs[ndx] = PreOrder(node));


                foreach (var seq in seqs)
                    res = res.Concat(seq);
            }

            return res;

            //--------------

            IEnumerable<ITreeNode<T>> PreOrder(ITreeNode<T> node)
            {
                var queue = new Queue<ITreeNode<T>>();
                PushNodes(queue, node);

                return queue;
            }

            void PushNodes(Queue<ITreeNode<T>> queue, ITreeNode<T> node)
            {
                queue.Enqueue(node);
                foreach (var nd in node.Children)
                    PushNodes(queue, nd);
            }
        }

        static IEnumerable<ITreeNode<T>> PostOrderTraversal<T>(ITreeNode<T> root)
        {
            IEnumerable<ITreeNode<T>> res;
            int childCount = root.Degree;

            if (childCount == 1)
                res = PostOrder(root.Children.Single());
            else
            {
                var seqs = new IEnumerable<ITreeNode<T>>[childCount];

                Parallel.ForEach(root.Children, (node, _, ndx)
                    => seqs[ndx] = PostOrder(node));

                res = Enumerable.Empty<ITreeNode<T>>();
                foreach (var seq in seqs)
                    res = res.Concat(seq);
            }

            res = res.Concat(Enumerable.Repeat(root, 1));
            return res;

            //--------
            IEnumerable<ITreeNode<T>> PostOrder(ITreeNode<T> node)
            {
                var queue = new Queue<ITreeNode<T>>();
                PushNodes(queue, node);

                return queue;
            }

            void PushNodes(Queue<ITreeNode<T>> queue, ITreeNode<T> node)
            {
                foreach (var nd in node.Children)
                    PushNodes(queue, nd);

                queue.Enqueue(node);
            }
        }

        static IEnumerable<ITreeNode<T>> InOrderTraversal<T>(ITreeNode<T> root)
        {
            IEnumerable<ITreeNode<T>> rootOnly = Enumerable.Repeat(root, 1);
            IEnumerable<ITreeNode<T>> res;
            int childCount = root.Degree;

            if (childCount == 1)
                res = InOrder(root.Children.Single()).Concat(rootOnly);
            else
            {
                var seqs = new IEnumerable<ITreeNode<T>>[childCount];

                Parallel.ForEach(root.Children, (node, _, ndx)
                    => seqs[ndx] = InOrder(node));

                res = seqs[0].Concat(rootOnly);

                for (int i = 1; i < seqs.Length; ++i)
                    res = res.Concat(seqs[i]);
            }

            return res;

            //---------------

            IEnumerable<ITreeNode<T>> InOrder(ITreeNode<T> node)
            {
                var queue = new Queue<ITreeNode<T>>();
                PushNodes(queue, node);

                return queue;
            }


            void PushNodes(Queue<ITreeNode<T>> queue, ITreeNode<T> node)
            {
                if (!node.IsLeaf)
                {
                    var firstChild = node.Children.First();
                    PushNodes(queue, firstChild);

                    queue.Enqueue(node);

                    foreach (var nd in node.Children.Skip(1))
                        PushNodes(queue, nd);
                }
                else
                    queue.Enqueue(node);
            }
        }

        static IEnumerable<(ITreeNode<T> node, int level)> LevelOrderTraversal<T>(ITreeNode<T> root)
        {
            var queue = new Queue<(ITreeNode<T>, int)>();

            queue.Enqueue((root, 0));

            while (queue.Count > 0)
            {
                var (node, lvl) = queue.Dequeue();
                yield return (node, lvl);

                foreach (var child in node.Children)
                    queue.Enqueue((child, lvl + 1));
            }
        }
    }
}
