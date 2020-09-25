using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static easyLib.DebugHelper;


namespace easyLib.ADT.Trees
{
    public interface INode<T>
    {
        bool IsRoot { get; }
        bool IsLeaf { get; }
        INode<T> Parent { get; }
        IEnumerable<INode<T>> Children { get; }
        bool IsDescendant(INode<T> node);
        int Degree { get; }
        public IEnumerable<INode<T>> GetPath();
        int GetDepth();
        int GetDescendantCount();
        T Item { get; }
    }
    //------------------------------------------------

    public abstract class Node<T>
    {
        public T Item { get; set; }
        public bool IsRoot => GetParent() == null;
        public bool IsLeaf => Degree == 0;
        public int Degree => GetChildCount();
        public int GetDepth() => GetNodePath().Count() - 1;
        public bool IsAncestor(Node<T> node)
        {
            Assert(node != null);
            Node<T> nd = this;

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


            int CountDescendants(Node<T> node)
            {
                int n = 1;
                foreach (Node<T> child in node.GetChildren())
                    n += CountDescendants(child);

                return n;
            }
        }


        //protected:
        protected Node(T item)
        {
            Item = item;
        }

        protected IEnumerable<Node<T>> GetNodePath()
        {
            var stack = new Stack<Node<T>>();
            Node<T> node = this;

            do
            {
                stack.Push(node);
                node = node.GetParent();

            } while (node != null);

            return stack;
        }
        protected abstract Node<T> GetParent();
        protected abstract IEnumerable<Node<T>> GetChildren();
        protected abstract int GetChildCount();

        protected virtual bool ClassInvariant =>
                (IsLeaf == (Degree == 0)) &&
                (IsRoot || GetParent().GetChildren().Contains(this)) &&
                (GetNodePath().Last() == this);
    }

}
