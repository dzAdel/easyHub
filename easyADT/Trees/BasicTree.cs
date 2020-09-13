using System.Collections.Generic;
using System.Linq;
using static easyLib.DebugHelper;

namespace easyLib.ADT.Trees
{
    public sealed partial class BasicTree<T> : Tree<T, BasicTree<T>.Node>
    {
        Node m_root;


        public BasicTree(T item, IEnumerable<T> children = null) :
            this(children == null ? new Node(item) : new Node(item, children))
        { }

        public BasicTree(Node root = null) :
            base(root)
        {
            Assert(root == null || root.Parent == null);
        }


        public static IEnumerable<BasicTree<T>> Split(BasicTree<T> tree)
        {
            Assert(tree != null);
            Assert(!tree.IsEmpty);

            Node[] nodes = tree.m_root.Children.ToArray();

            for (int i = nodes.Length - 1; i >= 0; --i)
                tree.m_root.DetachChild(nodes[i]);

            for (int i = 0; i < nodes.Length; ++i)
                yield return new BasicTree<T>(nodes[i]);
        }
    }
}
