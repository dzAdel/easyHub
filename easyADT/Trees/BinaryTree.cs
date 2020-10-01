using easyLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static easyLib.DebugHelper;

namespace easyLib.ADT.Trees
{
    public interface IBinaryTree<TItem, TNode> : ITree<TItem, TNode>
        where TNode : IBinaryTreeNode<TItem>
    {
        bool IsProper();
        bool IsComplete();
    }
    //---------------------------------------------------------------------

    public sealed partial class BinaryTree<T> : Tree<T, BinaryTree<T>.Node>, IBinaryTree<T, BinaryTree<T>.Node>
    {
        public BinaryTree(T item) :
            base(new Node(item))
        {
            Assert(ClassInvariant);
        }

        public BinaryTree(Node root = null) :
            base(root)
        {
            Assert(root == null || root.IsRoot);

            Assert(ClassInvariant);
        }

        public bool IsProper()
        {
            Assert(!IsEmpty);

            int childCouunt = Root.Degree;

            if (childCouunt == 0)
                return true;

            bool improper = false;
            Parallel.ForEach(Root.Children, LookupImproperNode);

            return !improper;


            //------------------
            void LookupImproperNode(Node node, ParallelLoopState pls)
            {
                int degree = node.Degree;

                if (pls.IsStopped || degree == 0)
                    return;

                if (degree == 1)
                {
                    improper = true;
                    pls.Stop();
                }
                else
                {
                    LookupImproperNode(node.LeftChild, pls);
                    LookupImproperNode(node.RightChild, pls);
                }
            }
        }

        public bool IsComplete()
        {
            Assert(!IsEmpty);

            if (Root.IsLeaf)
                return true;

            int breakLevel = -1;

            foreach (var (node, lvl) in this.LevelOrderTraversal())
                switch (node.Degree)
                {
                    case 0:
                        if (breakLevel == -1)
                            breakLevel = lvl;

                        break;

                    case 1:
                        if (node.LeftChild == null || breakLevel != -1)
                            return false;

                        breakLevel = lvl;
                        break;

                    case 2:
                        if (breakLevel != -1)
                            return false;
                        
                        break;
                }

            return true;
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
    }
}
