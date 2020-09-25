using System.Collections.Generic;
using System.Linq;
using static easyLib.DebugHelper;


namespace easyLib.ADT.Trees
{
    partial class BinaryTree<T>
    {
        public sealed class Node : Node<T>, IBinaryTreeNode<T>
        {
            Node m_leftChild;
            Node m_rightChild;

            public Node(T item = default, Node leftChild = null, Node rightChild = null) :
                base(item)
            {
                Assert(leftChild == null || leftChild.IsRoot);
                Assert(rightChild == null || rightChild.IsRoot);

                m_leftChild = leftChild;

                if (m_leftChild != null)
                    m_leftChild.Parent = this;

                m_rightChild = rightChild;

                if (m_rightChild != null)
                    m_rightChild.Parent = this;

                Assert(ClassInvariant);
            }

            public Node Parent { get; private set; }

            public Node LeftChild
            {
                get => m_leftChild;
                set
                {
                    Assert(value == null || !IsAncestor(value));

                    if (m_leftChild != null)
                        m_leftChild.Parent = null;

                    if (value != null)
                    {
                        DetachNode(value);
                        value.Parent = this;
                    }

                    m_leftChild = value;

                    Assert(ClassInvariant);
                }
            }

            public Node RightChild
            {
                get => m_rightChild;
                set
                {
                    Assert(value == null || !IsAncestor(value));

                    if (m_rightChild != null)
                        m_rightChild.Parent = null;

                    if (value != null)
                    {
                        DetachNode(value);
                        value.Parent = this;
                    }

                    m_rightChild = value;

                    Assert(ClassInvariant);
                }
            }

            public IEnumerable<Node> Children
            {
                get
                {
                    if (m_leftChild != null)
                        yield return m_leftChild;

                    if (m_rightChild != null)
                        yield return m_rightChild;
                }
            }

            public IEnumerable<Node> GetPath() => GetNodePath().Cast<Node>();

            public bool IsDescendant(Node node)
            {
                Assert(node != null);

                return node.IsAncestor(this);
            }

            bool INode<T>.IsDescendant(INode<T> node)
            {
                Assert(node != null);

                if (node is Node nd)
                    return IsDescendant(nd);

                return false;
            }

            IEnumerable<INode<T>> INode<T>.GetPath() => GetPath();
            IBinaryTreeNode<T> IBinaryTreeNode<T>.LeftChild => LeftChild;
            IBinaryTreeNode<T> IBinaryTreeNode<T>.RightChild => RightChild;

            INode<T> INode<T>.Parent => Parent;

            IEnumerable<INode<T>> INode<T>.Children => Children;



            //protected:
            protected override int GetChildCount()
            {
                int count = m_leftChild == null ? 0 : 1;
                return m_rightChild == null ? count : count + 1;
            }

            protected override IEnumerable<Node<T>> GetChildren() => Children;

            protected override Node<T> GetParent() => Parent;

            protected override bool ClassInvariant =>
                base.ClassInvariant &&
                Degree <= 2 &&
                (LeftChild == null && RightChild == null) || (LeftChild != RightChild);


            //private:
            static void DetachNode(Node node)
            {
                Node parent = node.Parent;

                if (parent != null)
                {
                    if (parent.m_leftChild == node)
                        parent.m_leftChild = null;
                    else
                        parent.m_rightChild = null;

                    node.Parent = null;
                }
            }
        }
    }
}
