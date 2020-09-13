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
        uint Degree { get; }
        public IEnumerable<INode<T>> GetPath();
        uint GetDepth();
        uint GetDescendantCount();
        T Item { get; }
    }
    //------------------------------------------------

    public abstract class Node<T>
    {
        public T Item { get; set; }
        public bool IsRoot => GetParent() == null;
        public bool IsLeaf => Degree == 0;
        public uint Degree => GetChildCount();
        public uint GetDepth() => (uint)GetNodePath().Count() - 1;
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
        public uint GetDescendantCount()
        {
            uint childCount = GetChildCount();

            if (childCount == 0)
                return 1;

            var nbers = new uint[childCount];

            Parallel.ForEach(GetChildren(), (node, _, ndx)
                => nbers[ndx] = CountDescendants(node));

            return nbers.Aggregate((total, sz) => total += sz) + 1;


            uint CountDescendants(Node<T> node)
            {
                uint n = 1;
                foreach (Node<T> child in node.GetChildren())
                    n += CountDescendants(child);

                return n;
            }
        }


        //protected:
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
        protected abstract uint GetChildCount();

        protected virtual bool ClassInvariant =>
                (IsLeaf == (Degree == 0)) &&
                (IsRoot || GetParent().GetChildren().Contains(this)) &&
                (GetNodePath().Last() == this);


    }

}
