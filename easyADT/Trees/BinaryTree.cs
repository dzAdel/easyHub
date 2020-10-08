using easyLib.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static easyLib.DebugHelper;

namespace easyLib.ADT.Trees
{
    public interface IBinaryTree<out TItem, out TNode> : ITree<TItem, TNode>
        where TNode : IBinaryTreeNode<TItem>
    {
        bool IsProper();
        bool IsComplete();
    }
    //---------------------------------------------------------------------

    public sealed partial class BinaryTree<T> : IBinaryTree<T, BinaryTree<T>.Node>
    {
        public BinaryTree(T item)
        {
            Root = new Node(item);
        }

        public BinaryTree(Node root = null)
        {
            Root = root;
        }

        public Node Root { get; set; }

        public bool IsEmpty => Root == null;

        public IEnumerable<Node> Nodes => Enumerate(TraversalOrder.PostOrder);

        public IEnumerable<Node> Leaves => Nodes.Where(node => node.IsLeaf);

        public int GetCount() => Root?.GetDescendantCount() ?? 0;

        public bool IsProper()
        {
            Assert(!IsEmpty);
            return BinaryTrees.IsProper(this);
        }

        public bool IsComplete()
        {
            Assert(!IsEmpty);
            return BinaryTrees.IsComplete(this);
        }

        public IEnumerable<Node> GetPath(Node node)
        {
            Assert(node != null);
            Assert(this.Contains(node));

            return node.GetPath(Root);
        }

        public IEnumerable<Node> Enumerate(TraversalOrder order) => Trees.Enumerate(this, order).Cast<Node>();

        public IEnumerator<T> GetEnumerator() => Nodes.Select(nd => nd.Item).GetEnumerator();

        public void Clear() => Root = null;

        public IEnumerable<BinaryTree<T>> SubTrees()
        {
            Assert(!IsEmpty);

            Node lChild = Root.LeftChild;
            Node rChild = Root.RightChild;

            yield return new BinaryTree<T>(lChild);
            yield return new BinaryTree<T>(rChild);
        }

        public static BinaryTree<T> Merge(T item,
            BinaryTree<T> lefSubTree = null,
            BinaryTree<T> rigthSubTree = null)
        {
            var bt = new BinaryTree<T>(item);

            if (lefSubTree != null && !lefSubTree.IsEmpty)
                bt.Root.LeftChild = lefSubTree.Root;

            if (rigthSubTree != null && !rigthSubTree.IsEmpty)
                bt.Root.RightChild = rigthSubTree.Root;

            return bt;
        }

        public static BinaryTree<T> BuildTree(IList<T> inOrderTraversal,
            IList<T> otherTraversal,
            TraversalOrder otherTraversalOrder)
        {
            Assert(inOrderTraversal != null);
            Assert(inOrderTraversal.Distinct().Count() == inOrderTraversal.Count);
            Assert(otherTraversal != null);
            Assert(inOrderTraversal.Count == otherTraversal.Count);
            Assert(inOrderTraversal.All(node => otherTraversal.Contains(node)));
            Assert(otherTraversalOrder == TraversalOrder.PostOrder || otherTraversalOrder == TraversalOrder.PreOrder);

            return BuildTree(inOrderTraversal, otherTraversal, x => x, otherTraversalOrder);
        }

        public static BinaryTree<T> BuildTree<U>(IList<U> inOrderTraversal,
            IList<U> otherTraversal,
            Func<U, T> selector,
            TraversalOrder otherTraversalOrder)
        {
            Assert(inOrderTraversal != null);
            Assert(inOrderTraversal.Distinct().Count() == inOrderTraversal.Count);
            Assert(otherTraversal != null);
            Assert(inOrderTraversal.Count == otherTraversal.Count);
            Assert(inOrderTraversal.All(node => otherTraversal.Contains(node)));
            Assert(selector != null);
            // assert selector thread safe
            Assert(otherTraversalOrder == TraversalOrder.PostOrder || otherTraversalOrder == TraversalOrder.PreOrder);

            int count = inOrderTraversal.Count;

            if (count == 0)
                return new BinaryTree<T>();

            if (count == 1)
                return new BinaryTree<T>(new Node(selector(inOrderTraversal[0])));

            U item = otherTraversalOrder == TraversalOrder.PostOrder ? otherTraversal[count - 1] :
                otherTraversal[0];

            var root = new Node(selector(item));
            int rootIndex = inOrderTraversal.IndexOf(item);
            Assert(rootIndex >= 0 && rootIndex < count);


            if (otherTraversalOrder == TraversalOrder.PostOrder)
            {
                Action leftAction = () => root.LeftChild = SubTreeFromPostOrder(0, 0, rootIndex);
                Action rightAction = () => root.RightChild = SubTreeFromPostOrder(rootIndex + 1, rootIndex, count - rootIndex - 1);

                Parallel.Invoke(leftAction, rightAction);
            }
            else
            {
                Action leftAction = () => root.LeftChild = SubTreeFromPreOrder(0, 1, rootIndex);
                Action rightAction = () => root.RightChild = SubTreeFromPreOrder(rootIndex + 1, 1 + rootIndex, count - rootIndex - 1);

                Parallel.Invoke(leftAction, rightAction);
            }

            return new BinaryTree<T>(root);

            //-----------
            Node SubTreeFromPostOrder(int ndxInOrder, int ndxPostOrder, int count)
            {
                if (count == 0)
                    return null;

                U rootData = otherTraversal[ndxPostOrder + count - 1];
                var rootNode = new Node(selector(rootData));

                if (count > 1)
                {

                    int ndxRoot = inOrderTraversal.IndexOf(rootData, ndxInOrder);
                    Assert(ndxRoot >= ndxInOrder && ndxRoot < ndxInOrder + count);

                    int inOrderStartLeft = ndxInOrder;
                    int postOrderStartLeft = ndxPostOrder;
                    int countLeft = ndxRoot - ndxInOrder;

                    int inOrderStartRight = ndxRoot + 1;
                    int postOrderStartRight = postOrderStartLeft + countLeft;
                    int countRight = ndxInOrder + count - ndxRoot - 1;

                    rootNode.LeftChild = SubTreeFromPostOrder(inOrderStartLeft, postOrderStartLeft, countLeft);
                    rootNode.RightChild = SubTreeFromPostOrder(inOrderStartRight, postOrderStartRight, countRight);
                }

                return rootNode;
            }


            Node SubTreeFromPreOrder(int ndxInOrder, int ndxPreOrder, int count)
            {
                if (count == 0)
                    return null;

                U rootData = otherTraversal[ndxPreOrder];

                var rootNode = new Node(selector(rootData));

                if (count > 1)
                {
                    int ndxRoot = inOrderTraversal.IndexOf(rootData, ndxInOrder);
                    Assert(ndxRoot >= ndxInOrder && ndxRoot < ndxInOrder + count);

                    int inOrderStartLeft = ndxInOrder;
                    int preOrderStartLeft = ndxPreOrder + 1;
                    int countLeft = ndxRoot - ndxInOrder;

                    int inOrderStartRight = ndxRoot + 1;
                    int preOrderStartRight = preOrderStartLeft + countLeft;
                    int countRight = ndxInOrder + count - ndxRoot - 1;

                    rootNode.LeftChild = SubTreeFromPreOrder(inOrderStartLeft, preOrderStartLeft, countLeft);
                    rootNode.RightChild = SubTreeFromPreOrder(inOrderStartRight, preOrderStartRight, countRight);
                }
                return rootNode;
            }
        }


        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    //--------------------------------------------------------------------------------------------------

    public static class BinaryTrees
    {
        public static bool IsProper<T, N>(this IBinaryTree<T, N> tree)
            where N : IBinaryTreeNode<T>
        {
            Assert(tree != null);
            Assert(!tree.IsEmpty);

            int childCouunt = tree.Root.Degree;

            if (childCouunt == 0)
                return true;

            bool improper = false;

            if (childCouunt == 1)
                LookupImproperNode(tree.Root.LeftChild ?? tree.Root.RightChild, null);
            else
                Parallel.ForEach(new IBinaryTreeNode<T>[] { tree.Root.LeftChild, tree.Root.RightChild },
                    LookupImproperNode);

            return !improper;


            //------------------
            void LookupImproperNode(IBinaryTreeNode<T> node, ParallelLoopState pls)
            {
                int degree = node.Degree;

                if ((pls != null && pls.IsStopped) || degree == 0)
                    return;

                if (degree == 1)
                {
                    improper = true;
                    pls?.Stop();
                }
                else
                {
                    LookupImproperNode(node.LeftChild, pls);
                    LookupImproperNode(node.RightChild, pls);
                }
            }

        }

        public static bool IsComplete<T, N>(this IBinaryTree<T, N> tree)
            where N : IBinaryTreeNode<T>
        {
            Assert(tree != null);
            Assert(!tree.IsEmpty);

            if (tree.Root.IsLeaf)
                return true;

            int breakLevel = -1;

            foreach (var (node, lvl) in tree.LevelOrderTraversal())
                switch (node.Degree)
                {
                    case 0:
                        if (breakLevel == -1)
                            breakLevel = lvl;

                        break;

                    case 1:
                        if (((IBinaryTreeNode<T>)node).LeftChild == null || breakLevel != -1)   // any descendant of an IBinaryTreeNode
                            return false;                                                       // is an IBinaryTreeNode

                        breakLevel = lvl;
                        break;

                    case 2:
                        if (breakLevel != -1)
                            return false;

                        break;
                }

            return true;

        }
    }
}
