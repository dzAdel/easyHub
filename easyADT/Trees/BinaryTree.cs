using System.Threading.Tasks;
using static easyLib.DebugHelper;



namespace easyLib.ADT.Trees
{
    public interface IBinaryTree<TItem, TNode> : ITree<TItem, TNode>
        where TNode : IBinaryTreeNode<TItem>
    {
        bool IsProper();
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
    }
}
