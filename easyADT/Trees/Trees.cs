using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static easyLib.DebugHelper;


namespace easyLib.ADT.Trees
{
    public enum TraversalOrder
    {
        InOrder,
        PreOrder,
        PostOrder,
        BreadthFirst
    }

    public static class Trees
    {
        public static int InternalPathLength<TItem, TNode>(this ITree<TItem, TNode> tree)
            where TNode : INode<TItem>
        {
            Assert(tree != null);
            Assert(!tree.IsEmpty);

            if (tree.Root.IsLeaf)
                return 0;

            var queue = new Queue<(TNode, int)>();
            int len = 0;

            queue.Enqueue((tree.Root, 0));

            while (queue.Count > 0)
            {
                (TNode node, int h) = queue.Dequeue();
                len += h;

                foreach (TNode child in node.Children)
                    if (!child.IsLeaf)
                        queue.Enqueue((child, h + 1));
            }

            return len;
        }

        public static int ExternalPathLength<TItem, TNode>(this ITree<TItem, TNode> tree)
            where TNode : INode<TItem>
        {
            Assert(tree != null);
            Assert(!tree.IsEmpty);

            if (tree.Root.IsLeaf)
                return 0;

            var queue = new Queue<(TNode, int)>();
            int len = 0;

            queue.Enqueue((tree.Root, 0));

            while (queue.Count > 0)
            {
                (TNode node, int h) = queue.Dequeue();

                if (node.IsLeaf)
                    len += h;
                else
                    foreach (TNode child in node.Children)
                        queue.Enqueue((child, h + 1));
            }

            System.Diagnostics.Debug.WriteLine($"len = {len}");
            return len;
        }


        public static int WeightedExternalPathLength<TItem, TNode>(this ITree<TItem, TNode> tree, Func<TNode, int> leafWeight)
            where TNode : INode<TItem>
        {
            Assert(tree != null);
            Assert(!tree.IsEmpty);
            Assert(leafWeight != null);

            if (tree.Root.IsLeaf)
                return 0;

            var queue = new Queue<(TNode, int)>();
            int len = 0;

            queue.Enqueue((tree.Root, 0));

            while (queue.Count > 0)
            {
                (TNode node, int h) = queue.Dequeue();

                if (node.IsLeaf)
                    len += h * leafWeight(node);
                else
                    foreach (TNode child in node.Children)
                        queue.Enqueue((child, h + 1));
            }

            System.Diagnostics.Debug.WriteLine($"len = {len}");
            return len;
        }

        public static IEnumerable<TNode> Enumerate<TItem, TNode>(this ITree<TItem, TNode> tree, TraversalOrder order)
            where TNode : INode<TItem>
        {
            Assert(tree != null);
            Assert(Enum.IsDefined(typeof(TraversalOrder), order));

            if (tree.IsEmpty)
                return Enumerable.Empty<TNode>();

            TNode root = tree.Root;

            if (root.IsLeaf)
                return Enumerable.Repeat(root, 1);


            Func<TNode, IEnumerable<TNode>> iter = null;

            switch (order)
            {
                case TraversalOrder.PreOrder:
                    iter = PreOrderTraversal<TNode, TItem>;
                    break;

                case TraversalOrder.PostOrder:
                    iter = PostOrderTraversal<TNode, TItem>;
                    break;

                case TraversalOrder.InOrder:
                    iter = InOrderTraversal<TNode, TItem>;
                    break;

                case TraversalOrder.BreadthFirst:
                    iter = LevelOrderTraversal<TNode, TItem>;
                    break;
            }

            return iter(root);
        }


        //private:
        static IEnumerable<TNode> PreOrderTraversal<TNode, TItem>(TNode root)
            where TNode : INode<TItem>
        {
            IEnumerable<TNode> res = Enumerable.Repeat(root, 1);
            uint childCount = root.Degree;

            if (childCount == 1)
                res = res.Concat(PreOrder((TNode)root.Children.Single()));
            else
            {
                var seqs = new IEnumerable<TNode>[childCount];

                Parallel.ForEach(root.Children, (node, _, ndx)
                    => seqs[ndx] = PreOrder((TNode)node));


                foreach (IEnumerable<TNode> seq in seqs)
                    res = res.Concat(seq);
            }

            return res;

            //--------------

            IEnumerable<TNode> PreOrder(TNode node)
            {
                var queue = new Queue<TNode>();
                PushNodes(queue, node);

                return queue;
            }

            void PushNodes(Queue<TNode> queue, TNode node)
            {
                queue.Enqueue(node);
                foreach (TNode nd in node.Children)
                    PushNodes(queue, nd);
            }
        }

        static IEnumerable<TNode> PostOrderTraversal<TNode, TItem>(TNode root)
            where TNode : INode<TItem>
        {
            IEnumerable<TNode> res;
            uint childCount = root.Degree;

            if (childCount == 1)
                res = PostOrder((TNode)root.Children.Single());
            else
            {
                var seqs = new IEnumerable<TNode>[childCount];

                Parallel.ForEach(root.Children, (node, _, ndx)
                    => seqs[ndx] = PostOrder((TNode)node));

                res = Enumerable.Empty<TNode>();
                foreach (IEnumerable<TNode> seq in seqs)
                    res = res.Concat(seq);
            }

            res = res.Concat(Enumerable.Repeat(root, 1));
            return res;

            //--------
            IEnumerable<TNode> PostOrder(TNode node)
            {
                var queue = new Queue<TNode>();
                PushNodes(queue, node);

                return queue;
            }

            void PushNodes(Queue<TNode> queue, TNode node)
            {
                foreach (TNode nd in node.Children)
                    PushNodes(queue, nd);

                queue.Enqueue(node);
            }
        }

        static IEnumerable<TNode> InOrderTraversal<TNode, TItem>(TNode root)
            where TNode : INode<TItem>
        {
            IEnumerable<TNode> rootOnly = Enumerable.Repeat(root, 1);
            IEnumerable<TNode> res;
            uint childCount = root.Degree;

            if (childCount == 1)
                res = InOrder((TNode)root.Children.Single()).Concat(rootOnly);
            else
            {
                var seqs = new IEnumerable<TNode>[childCount];

                Parallel.ForEach(root.Children, (node, _, ndx)
                    => seqs[ndx] = InOrder((TNode)node));

                res = seqs[0].Concat(rootOnly);

                for (int i = 1; i < seqs.Length; ++i)
                    res = res.Concat(seqs[i]);
            }

            return res;

            //---------------

            IEnumerable<TNode> InOrder(TNode node)
            {
                var queue = new Queue<TNode>();
                PushNodes(queue, node);

                return queue;
            }


            void PushNodes(Queue<TNode> queue, TNode node)
            {
                if (!node.IsLeaf)
                {
                    var firstChild = (TNode)node.Children.First();
                    PushNodes(queue, firstChild);

                    queue.Enqueue(node);

                    foreach (TNode nd in node.Children.Skip(1))
                        PushNodes(queue, nd);
                }
                else
                    queue.Enqueue(node);
            }
        }

        static IEnumerable<TNode> LevelOrderTraversal<TNode, TItem>(TNode root)
            where TNode : INode<TItem>
        {
            var queue = new Queue<TNode>();

            queue.Enqueue(root);

            TNode node;
            while (queue.Count > 0)
            {
                node = queue.Dequeue();
                yield return node;

                foreach (TNode child in node.Children)
                    queue.Enqueue(child);
            }
        }

    }
}
