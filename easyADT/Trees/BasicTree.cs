using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static easyLib.DebugHelper;

namespace easyLib.ADT.Trees
{
    public sealed partial class BasicTree<T> : ITree<T, BasicTree<T>.Node>
    {
        public BasicTree(T item, IEnumerable<T> children = null) :
            this(children == null ? new Node(item) : new Node(item, children))
        { }

        public BasicTree(Node root = null)
        {
            Root = root;
        }

        public Node Root { get; set; }

        public IEnumerable<Node> Nodes => Enumerate(TraversalOrder.PreOrder);

        public IEnumerable<Node> Leaves => Nodes.Where(nd => nd.IsLeaf);

        public bool IsEmpty => Root == null;

        public int GetCount() => Root?.GetDescendantCount() ?? 0;

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var node in Nodes)
                yield return node.Item;
        }

        public IEnumerable<Node> Enumerate(TraversalOrder order) => Trees.Enumerate(this, order).Cast<Node>();
        
        public IEnumerable<Node> GetPath(Node node)
        {
            Assert(node != null);
            Assert(this.Contains(node));

            return node.GetPath(Root);
        }

        public void Clear() => Root = null;

        public IEnumerable<BasicTree<T>> SubTrees()
        {
            Assert(!IsEmpty);            

            foreach (var node in Root.Children)
                yield return new BasicTree<T>(node);
        }

        public static BasicTree<T> Merge(T item, params BasicTree<T>[] trees)
        {
            var root = new BasicTree<T>.Node(item);

            foreach (BasicTree<T> tree in trees)
                if (!tree.IsEmpty)
                    root.AppendChild(tree.Root);

            return new BasicTree<T>(root);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
