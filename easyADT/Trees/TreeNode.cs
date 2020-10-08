using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static easyLib.DebugHelper;


namespace easyLib.ADT.Trees
{
    public interface ITreeNode<out T>
    {
        T Item { get; }
        bool IsLeaf { get; }
        ITreeNode<T> Parent { get; }
        IEnumerable<ITreeNode<T>> Children { get; }
        int Degree { get; }        
    }
    //-------------------------------------------------------

    public static class TreeNodes
    {
        public static bool IsDescendantOf<T>(this ITreeNode<T> node, ITreeNode<T> ancestor)
        {
            Assert(node != null);
            Assert(ancestor != null);

            do
            {
                if (node == ancestor)
                    return true;

                node = node.Parent;

            } while (node != null);

            return false;
        }

        public static bool IsAncestorOf<T>(this ITreeNode<T> node, ITreeNode<T> descendant)
        {
            Assert(node != null);
            Assert(descendant != null);

            return descendant.IsDescendantOf(node);
        }

        public static int GetDescendantCount<T>(this ITreeNode<T> node)
        {
            Assert(node != null);

            int childCount = node.Degree;

            if (childCount == 0)
                return 1;

            if (childCount == 1)
                return CountDescendants(node.Children.Single()) + 1;


            var nbers = new int[childCount];

            Parallel.ForEach(node.Children, (node, _, ndx)
                => nbers[ndx] = CountDescendants(node));

            return nbers.Aggregate((total, sz) => total += sz) + 1;

            //-------------
            int CountDescendants(ITreeNode<T> child)
            {
                int n = 1;
                foreach (var nd in child.Children)
                    n += CountDescendants(nd);

                return n;
            }
        }

        public static IEnumerable<ITreeNode<T>> GetPath<T>(this ITreeNode<T> node,  ITreeNode<T> ancestor = null)
        {
            Assert(node != null);
            Assert(ancestor == null || node.IsDescendantOf(ancestor));

            var stack = new Stack<ITreeNode<T>>();
            
            ITreeNode<T> endNode = ancestor?.Parent;

            do
            {
                stack.Push(node);
                node = node.Parent;

            } while (node != endNode);

            return stack;
        }
    }

}
