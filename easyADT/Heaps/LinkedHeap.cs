using easyLib.ADT.Trees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static easyLib.DebugHelper;


namespace easyLib.ADT.Heaps
{
    public sealed class LinkedHeap<T> : Heap<T>, IBinaryTree<T>
    {
        readonly BinaryTree<T> m_tree;
        readonly Func<T, T, bool> m_before;
        int m_count;

        public LinkedHeap(Func<T, T, bool> before = null)
        {           
            m_tree = new BinaryTree<T>();
            m_before = before ?? ((a, b) => Comparer<T>.Default.Compare(a, b) < 0);
        }

        public LinkedHeap(Comparison<T> comparison)
        {
            Assert(comparison != null);

            m_tree = new BinaryTree<T>();
            m_before = (a, b) => comparison(a, b) < 0;
        }

        public IBinaryTreeNode<T> Root => m_tree.Root;

        public IEnumerable<IBinaryTreeNode<T>> Nodes => m_tree.Nodes;

        public IEnumerable<IBinaryTreeNode<T>> Leaves => m_tree.Leaves;

        public int GetHeight()
        {
            Assert(!IsEmpty);

            return m_tree.GetHeight();
        }

        public int GetCount() => m_count;

        public bool IsComplete()
        {
            Assert(IsEmpty);

            return true;
        }

        public bool IsProper()
        {
            Assert(!IsEmpty);

            return m_tree.IsProper();
        }


        //protected:
        protected override int GetItemCount() => m_count;

        protected override IEnumerator<T> GetItemEnumerator() => m_tree.GetEnumerator();

        protected override T PeekItem() => m_tree.Root.Item;

        protected override T PopItem()
        {
            T result = m_tree.Root.Item;

            BinaryTree<T>.Node node = m_tree.Enumerate(TraversalOrder.BreadthFirst).Last(); //todo: a revoir

            m_tree.Root.Item = node.Item;

            if (node.Parent.LeftChild == node)
                node.Parent.LeftChild = null;
            else
                node.Parent.RightChild = null;

            Assert(node.IsLeaf && (node.Parent == null));


            node = m_tree.Root;

            while (!node.IsLeaf)
            {
                BinaryTree<T>.Node child = node.LeftChild == null? node.RightChild :
                    node.RightChild != null ? (m_before(node.LeftChild.Item, node.RightChild.Item)? node.LeftChild : 
                    node.RightChild): null;

                if (child == null || !m_before(child.Item, node.Item))
                    break;

                (node.Item, child.Item) = (child.Item, node.Item);
                node = child;
            }

            --m_count;

            Assert(ClassInvariant);
            return result;
        }

        protected override void AddItem(T item)
        {
            if (IsEmpty)
                m_tree.Root = new BinaryTree<T>.Node(item);
            else
            {
                BinaryTree<T>.Node parent = m_tree.Enumerate(TraversalOrder.BreadthFirst).Where(nd => nd.Degree < 2).First();
                var node = new BinaryTree<T>.Node(item);

                if (parent.LeftChild == null)
                    parent.LeftChild = node;
                else
                    parent.RightChild = node;

                do
                {
                    if (!m_before(node.Item, node.Parent.Item))
                        break;

                    (node.Item, node.Parent.Item) = (node.Parent.Item, node.Item);
                    node = node.Parent;

                } while (node != m_tree.Root);
            }

            ++m_count;

            Assert(ClassInvariant);            
        }

        //private:
        bool ClassInvariant => m_count == m_tree.GetCount();
    }
}
