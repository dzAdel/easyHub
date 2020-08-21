using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static easyLib.DebugHelper;


namespace easyLib.ADT.Trees
{
    partial class Tree<T>
    {
        public sealed class Node : INode<T>
        {
            readonly List<Node> m_children = new List<Node>();

            public Node(T item) //O(1)
            {
                Item = item;

                Assert(ClassInvariant);
            }

            public Node(T item, IEnumerable<T> children)    //O(childen.Count)
            {
                Assert(children != null);

                Item = item;

                foreach (T child in children)
                    m_children.Add(new Node(child) { Parent = this });
            }

            public T Item { get; set; } //O(1)
            public Node Parent { get; private set; }  //Amortizd O(1)

            public IEnumerable<Node> Children => m_children; //O(N)
            public bool IsRoot => Parent == null; //O(1)
            public bool IsLeaf => m_children.Count == 0; //O(1)
            public uint Degree => (uint)m_children.Count; //O(1)

            public uint GetDescendantCount()    //O(N)
            {
                if (m_children.Count == 0)
                    return 1;

                var nbers = new uint[m_children.Count];

                Parallel.ForEach(m_children, (node, _, ndx)
                    => nbers[ndx] = CountDescendants(node));

                return nbers.Aggregate((total, sz) => total += sz) + 1;


                uint CountDescendants(Node node)
                {
                    uint n = 1;
                    foreach (Node child in node.m_children)
                        n += CountDescendants(child);

                    return n;
                }
            }

            public IEnumerable<Node> GetPath()  //O(N)
            {
                var stack = new Stack<Node>();
                Node node = this;

                do
                {
                    stack.Push(node);
                    node = node.Parent;

                } while (node != null);

                return stack;
            }

            public uint GetDepth() => (uint)GetPath().Count() - 1;  //O(N)

            public bool IsAncestor(Node node)
            {
                Assert(node != null);
                Node nd = this;

                do
                {
                    if (node == nd)
                        return true;

                    nd = nd.Parent;

                } while (nd != null);

                return false;
            }

            public Node PrependChild(T item)    //Amortized O(1)
            {
                var node = new Node(item);

                m_children.Insert(0, node);
                node.Parent = this;

                Assert(ClassInvariant);
                return node;
            }

            public void PrependChild(Node node) //O(N)
            {
                Assert(node != null);
                Assert(!IsAncestor(node));

                m_children.Insert(0, node);
                node.Parent?.DetachChild(node);
                node.Parent = this;

                Assert(ClassInvariant);
            }

            public Node AppendChild(T item) //Amortized O(1)
            {
                var node = new Node(item);

                m_children.Add(node);
                node.Parent = this;

                Assert(ClassInvariant);
                return node;
            }

            public void AppendChild(Node node)  //O(N)
            {
                Assert(node != null);
                Assert(!IsAncestor(node));

                m_children.Add(node);
                node.Parent?.DetachChild(node);
                node.Parent = this;

                Assert(ClassInvariant);
            }

            public Node InsertSibling(T item)   //O(N)
            {
                Assert(!IsRoot);

                var node = new Node(item);

                int ndx = Parent.m_children.IndexOf(this);
                Parent.m_children.Insert(ndx + 1, node);
                node.Parent = Parent;

                Assert(ClassInvariant);
                return node;
            }

            public void InsertSibling(Node node)    //O(N)
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

            public void DetachChild(Node node)  //O(N)
            {
                Assert(node != null);
                Assert(Children.Contains(node));

                node.Parent = null;
                m_children.Remove(node);

                Assert(ClassInvariant);
            }

            IEnumerable<INode<T>> INode<T>.GetPath() => GetPath();  //O(N)
            INode<T> INode<T>.Parent => Parent; //O(1)
            IEnumerable<INode<T>> INode<T>.Children => Children;    //O(N)


            //private:
            bool ClassInvariant =>
                (IsLeaf == (Degree == 0)) &&
                (IsRoot || Parent.Children.Contains(this)) &&
                (GetPath().Last() == this);
        }
    }
}
