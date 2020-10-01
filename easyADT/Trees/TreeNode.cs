using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static easyLib.DebugHelper;


namespace easyLib.ADT.Trees
{
    public interface ITreeNode<T>
    {
        bool IsRoot { get; }
        bool IsLeaf { get; }
        ITreeNode<T> Parent { get; }
        IEnumerable<ITreeNode<T>> Children { get; }
        bool IsDescendant(ITreeNode<T> node);
        int Degree { get; }
        public IEnumerable<ITreeNode<T>> GetPath();
        int GetDepth();
        int GetDescendantCount();
        T Item { get; }
    }
    //------------------------------------------------

    public abstract class TreeNode<T>
    {
        public T Item { get; set; }
        public bool IsRoot => GetParent() == null;
        public bool IsLeaf => Degree == 0;
        public int Degree => GetChildCount();
        public int GetDepth() => GetNodePath().Count() - 1;
        public bool IsAncestor(TreeNode<T> node)
        {
            Assert(node != null);
            TreeNode<T> nd = this;

            do
            {
                if (node == nd)
                    return true;

                nd = nd.GetParent();

            } while (nd != null);

            return false;
        }
        public int GetDescendantCount()
        {
            int childCount = GetChildCount();

            if (childCount == 0)
                return 1;

            var nbers = new int[childCount];

            Parallel.ForEach(GetChildren(), (node, _, ndx)
                => nbers[ndx] = CountDescendants(node));

            return nbers.Aggregate((total, sz) => total += sz) + 1;


            int CountDescendants(TreeNode<T> node)
            {
                int n = 1;
                foreach (TreeNode<T> child in node.GetChildren())
                    n += CountDescendants(child);

                return n;
            }
        }


        //protected:
        protected TreeNode(T item)
        {
            Item = item;
        }

        protected IEnumerable<TreeNode<T>> GetNodePath()
        {
            var stack = new Stack<TreeNode<T>>();
            TreeNode<T> node = this;

            do
            {
                stack.Push(node);
                node = node.GetParent();

            } while (node != null);

            return stack;
        }
        protected abstract TreeNode<T> GetParent();
        protected abstract IEnumerable<TreeNode<T>> GetChildren();
        protected abstract int GetChildCount();

        protected virtual bool ClassInvariant =>
                (IsLeaf == (Degree == 0)) &&
                (IsRoot || GetParent().GetChildren().Contains(this)) &&
                (GetNodePath().Last() == this);
    }

}
