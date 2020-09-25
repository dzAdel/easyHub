using System;
using System.Collections.Generic;
using System.Linq;
using static easyLib.DebugHelper;


namespace easyLib.ADT.Trees
{
    partial class BasicTree<T>
    {
        public sealed class Node : Node<T>, INode<T>
        {
            readonly List<Node> m_children = new List<Node>();

            public Node(T item) :
                base(item)
            {
                Assert(ClassInvariant);
            }

            public Node(T item, IEnumerable<T> children) :
                base(item)
            {
                Assert(children != null);

                foreach (T child in children)
                    m_children.Add(new Node(child) { Parent = this });

                Assert(ClassInvariant);
            }


            public Node Parent { get; private set; }

            public IEnumerable<Node> Children => m_children;

            public IEnumerable<Node> GetPath() => GetNodePath().Cast<Node>();

            public Node PrependChild(T item)
            {
                var node = new Node(item);

                m_children.Insert(0, node);
                node.Parent = this;

                Assert(ClassInvariant);
                return node;
            }

            public void PrependChild(Node node)
            {
                Assert(node != null);
                Assert(!IsAncestor(node));

                m_children.Insert(0, node);
                node.Parent?.DetachChild(node);
                node.Parent = this;

                Assert(ClassInvariant);
            }

            public Node AppendChild(T item)
            {
                var node = new Node(item);

                m_children.Add(node);
                node.Parent = this;

                Assert(ClassInvariant);
                return node;
            }

            public void AppendChild(Node node)
            {
                Assert(node != null);
                Assert(!IsAncestor(node));

                m_children.Add(node);
                node.Parent?.DetachChild(node);
                node.Parent = this;

                Assert(ClassInvariant);
            }

            public Node InsertSibling(T item)
            {
                Assert(!IsRoot);

                var node = new Node(item);

                int ndx = Parent.m_children.IndexOf(this);
                Parent.m_children.Insert(ndx + 1, node);
                node.Parent = Parent;

                Assert(ClassInvariant);
                return node;
            }

            public void InsertSibling(Node node)
            {
                Assert(node != null);
                Assert(!IsRoot);
                Assert(!IsAncestor(node));

                int ndx = Parent.m_children.IndexOf(this);
                Parent.m_children.Insert(ndx + 1, node);
                node.Parent?.DetachChild(node);
                node.Parent = Parent;

                Assert(ClassInvariant);
            }

            public void DetachChild(Node node)
            {
                Assert(node != null);
                Assert(Children.Contains(node));

                node.Parent = null;
                m_children.Remove(node);

                Assert(ClassInvariant);
            }

            public bool IsDescendant(Node node)
            {
                Assert(node != null);

                return node.IsAncestor(this);
            }


            INode<T> INode<T>.Parent => Parent;

            IEnumerable<INode<T>> INode<T>.Children => Children;

            IEnumerable<INode<T>> INode<T>.GetPath() => GetPath();

            bool INode<T>.IsDescendant(INode<T> node)
            {
                Assert(node != null);

                if (node is Node nd)
                    return IsDescendant(nd);

                return false;
            }


            //protected:
            protected override Node<T> GetParent() => Parent;

            protected override IEnumerable<Node<T>> GetChildren() => Children;

            protected override int GetChildCount() => m_children.Count;
        }
    }
}
