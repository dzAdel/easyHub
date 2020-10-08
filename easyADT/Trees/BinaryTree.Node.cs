using System.Collections.Generic;
using System.Linq;
using static easyLib.DebugHelper;


namespace easyLib.ADT.Trees
{
    partial class BinaryTree<T>
    {
        public sealed class Node : IBinaryTreeNode<T>
        {
            Node m_parent;
            Node m_leftChild;
            Node m_rightChild;

            public Node(T item, Node leftChild = null, Node rightChild = null)
            {
                Item = item;

                if (leftChild != null)
                {
                    m_leftChild = leftChild;
                    m_leftChild.Parent = this;
                }


                if (rightChild != null)
                {
                    m_rightChild = rightChild;
                    m_rightChild.Parent = this;
                }

                Assert(ClassInvariant);
            }

            public Node Parent { get; private set; }

            public T Item { get; set; }

            public Node LeftChild
            {
                get => m_leftChild;
                set
                {
                    Assert(value == null || !value.IsAncestorOf(this));

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
                    Assert(value == null || !value.IsAncestorOf(this));

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

            public bool IsLeaf => Degree == 0;

            public int Degree
            {
                get
                {
                    int count = m_leftChild == null ? 0 : 1;
                    return m_rightChild == null ? count : count + 1;
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

            public IEnumerable<Node> GetPath(Node ancestor)
            {
                //since ancestor is of type Node, there is no way to insert a node of any other type
                Assert(ancestor != null);
                Assert(ancestor.IsAncestorOf(this));

                return TreeNodes.GetPath(this, ancestor).Cast<Node>();
            }


            IBinaryTreeNode<T> IBinaryTreeNode<T>.LeftChild => LeftChild;
            IBinaryTreeNode<T> IBinaryTreeNode<T>.RightChild => RightChild;
            ITreeNode<T> ITreeNode<T>.Parent => Parent;
            IEnumerable<ITreeNode<T>> ITreeNode<T>.Children => Children;


            //private:
            bool ClassInvariant =>
                Degree <= 2 &&
                (LeftChild == null && RightChild == null) || (LeftChild != RightChild);

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
