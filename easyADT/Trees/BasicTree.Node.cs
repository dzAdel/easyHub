using System;
using System.Collections.Generic;
using System.Linq;
using static easyLib.DebugHelper;


namespace easyLib.ADT.Trees
{
    partial class BasicTree<T>
    {
        public sealed class Node : ITreeNode<T>
        {
            readonly List<Node> m_children = new List<Node>();
            Node m_parent;


            public Node(T item)
            {
                Item = item;

                Assert(ClassInvariant);
            }

            public Node(T item, IEnumerable<T> children) :
                this(item)
            {
                Assert(children != null);

                foreach (T child in children)
                    m_children.Add(new Node(child) { m_parent = this });

                Assert(ClassInvariant);
            }

            public T Item { get; set; }

            public bool IsLeaf => Degree == 0;

            public int Degree => m_children.Count;

            public Node Parent
            {
                get => m_parent;
                set
                {
                    Assert(value == null || !this.IsAncestorOf(value));

                    if (value == null)
                        Parent?.DetachChild(this);
                    else
                        value.AppendChild(this);

                    Assert(ClassInvariant);
                }
            }

            public IEnumerable<Node> Children => m_children;            

            public IEnumerable<Node> GetPath(Node ancestor = null)
            {
                //since ancestor is of type Node there is no way 
                //to add a node of other type in the path

                Assert(ancestor == null || this.IsDescendantOf(ancestor));

                return TreeNodes.GetPath(this, ancestor).Cast<Node>();
            }

            public Node PrependChild(T item)
            {
                var node = new Node(item);
                m_children.Insert(0, node);
                node.m_parent = this;

                Assert(ClassInvariant);
                return node;
            }

            public void PrependChild(Node node)
            {
                Assert(node != null);
                Assert(!this.IsDescendantOf(node));

                m_children.Insert(0, node);
                node.m_parent?.DetachChild(node);
                node.m_parent = this;

                Assert(ClassInvariant);
            }

            public Node AppendChild(T item)
            {
                var node = new Node(item);

                m_children.Add(node);
                node.m_parent = this;

                Assert(ClassInvariant);
                return node;
            }

            public void AppendChild(Node node)
            {
                Assert(node != null);
                Assert(!this.IsDescendantOf(node));

                m_children.Add(node);
                node.m_parent?.DetachChild(node);
                node.m_parent = this;

                Assert(ClassInvariant);
            }

            public Node InsertSibling(T item)
            {
                Assert(Parent != null);

                var node = new Node(item);

                int ndx = m_parent.m_children.IndexOf(this);
                m_parent.m_children.Insert(ndx + 1, node);
                node.m_parent = Parent;

                Assert(ClassInvariant);
                return node;
            }

            public void InsertSibling(Node node)
            {
                Assert(node != null);
                Assert(Parent != null);
                Assert(!this.IsDescendantOf(node));

                int ndx = m_parent.m_children.IndexOf(this);
                m_parent.m_children.Insert(ndx + 1, node);
                node.m_parent?.DetachChild(node);
                node.m_parent = Parent;

                Assert(ClassInvariant);
            }

            public void DetachChild(Node node)
            {
                Assert(node != null);
                Assert(Children.Contains(node));

                node.m_parent = null;
                m_children.Remove(node);

                Assert(ClassInvariant);
            }

            ITreeNode<T> ITreeNode<T>.Parent => Parent;
            IEnumerable<ITreeNode<T>> ITreeNode<T>.Children => Children;


            //private:           
            bool ClassInvariant =>
                    (IsLeaf == (Degree == 0)) &&
                    (Parent == null || Parent.Children.Contains(this)) &&
                    (GetPath().Last() == this);
        }
    }
}
