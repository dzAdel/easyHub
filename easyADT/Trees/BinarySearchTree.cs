using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static easyLib.DebugHelper;


namespace easyLib.ADT.Trees
{
    public interface IBinarySearchTree<T, K> : IBinaryTree<T, IBinaryTreeNode<T>>, ISearchTree<T, K, IBinaryTreeNode<T>>
    { }
    //-----------------------------------------------------------------------------------------

    public sealed class BinarySearchTree<T, K> : IBinarySearchTree<T, K>
    {
        BinaryTree<T>.Node m_root;

        public BinarySearchTree(Func<T, K> keySelector, Comparison<K> keyComparator = null)
        {
            Assert(keySelector != null);

            KeySelect = keySelector;
            KeyCompare = keyComparator ?? Comparer<K>.Default.Compare;
        }

        public Comparison<K> KeyCompare { get; private set; }

        public Func<T, K> KeySelect { get; private set; }

        public IBinaryTreeNode<T> Root => m_root;

        public int Count { get; private set; }

        public bool IsEmpty => Count == 0;

        public IEnumerable<K> Keys => Enumerate(TraversalOrder.InOrder).Select(node => KeySelect(node.Item));

        public IEnumerable<IBinaryTreeNode<T>> Nodes => Enumerate(TraversalOrder.InOrder);

        public IEnumerable<IBinaryTreeNode<T>> Leaves => Nodes.Where(node => node.IsLeaf);

        public IEnumerable<IBinaryTreeNode<T>> Enumerate(TraversalOrder order)
        {
            Assert(Enum.IsDefined(typeof(TraversalOrder), order));

            return BinaryTrees.Enumerate(this, order).Cast<IBinaryTreeNode<T>>();
        }

        public bool ContainsKey(K key) => Locate(key) != null;

        public IBinaryTreeNode<T> Locate(K key)
        {
            if (m_root == null)
                return null;

            BinaryTree<T>.Node node = LocateNode(key);
            return Compare(key, node.Item) == 0 ? node : null;
        }

        public bool TryAdd(T item, out IBinaryTreeNode<T> node)
        {
            if (m_root == null)
            {
                node = m_root = new BinaryTree<T>.Node(item);
                ++Count;
                return true;
            }

            K key = KeySelect(item);
            BinaryTree<T>.Node nd = LocateNode(key);
            int cmp = Compare(key, nd.Item);

            if(cmp == 0)
            {
                node = nd;
                return false;
            }

            if (cmp < 0)
                node = nd.LeftChild = new BinaryTree<T>.Node(item);
            else
                node = nd.RightChild = new BinaryTree<T>.Node(item);

            ++Count;
            return true;
        }

        public IBinaryTreeNode<T> Add(T item)
        {
            Assert(!ContainsKey(KeySelect(item)));

            if (TryAdd(item, out IBinaryTreeNode<T> result))
                return result;

            throw new ArgumentException();            
        }

        public IBinaryTreeNode<T> Floor(K key)
        {
            //the largest key less than or equal to "key"
            // x | x <= key and for all y <= key y <= x 
            
            if (m_root == null)
                return null;

            IBinaryTreeNode<T> floor = null;
            IBinaryTreeNode<T> node = m_root;

            while (node != null)
            {
                int cmp = Compare(key, node.Item);

                if (cmp == 0)
                {
                    floor = node;
                    break;
                }

                if (cmp < 0)
                    node = node.LeftChild;
                else // found a key that is less than "key"
                {
                    floor = node;
                    node = node.RightChild;                    
                }
            }

            return floor;
        }

        public IBinaryTreeNode<T> Ceiling(K key)
        {
            //the smaller key greater than or equal to "key"
            // x | x >= key  and for all y >= key y >= x 

            if (m_root == null)
                return null;

            IBinaryTreeNode<T> ceiling = null;
            IBinaryTreeNode<T> node = m_root;

            while (node != null)
            {
                int cmp = Compare(key, node.Item);

                if (cmp == 0)
                {
                    ceiling = node;
                    break;
                }

                if (cmp < 0) // found a key that is greater than "key"
                {
                    ceiling = node;
                    node = node.LeftChild;
                }
                else
                    node = node.RightChild;
            }

            return ceiling;
        }

        public IBinaryTreeNode<T> FirstCommonAncestor(IBinaryTreeNode<T> node0, IBinaryTreeNode<T> node1)
        {
            Assert(node0 != null);
            Assert(this.ContainsNode(node0));
            Assert(node1 != null);
            Assert(this.ContainsNode(node1));

            IBinaryTreeNode<T> node = m_root;
            K key0 = KeySelect(node0.Item);
            K key1 = KeySelect(node1.Item);
            

            while(true)
            {
                K key = KeySelect(node.Item);
                int cmp0 = KeyCompare(key, key0);
                int cmp1 = KeyCompare(key, key1);

                if (cmp0 < 0 && cmp1 < 0)
                    node = node.RightChild;
                else if (cmp0 > 0 && cmp1 > 0)
                    node = node.LeftChild;
                else
                    break;
            }

            return node;            
        }

        public IEnumerator<T> GetEnumerator() => Nodes.Select(nd => nd.Item).GetEnumerator();

        public IEnumerable<IBinaryTreeNode<T>> GetPath(IBinaryTreeNode<T> node)
        {
            Assert(node != null);
            Assert(this.ContainsNode(node));

            return node.GetPath(m_root).Cast<IBinaryTreeNode<T>>();
        }

        public IBinaryTreeNode<T> Min()
        {
            if (m_root == null)
                return null;

            IBinaryTreeNode<T> node = m_root;

            while (node.LeftChild != null)
                node = node.LeftChild;

            return node;
        }

        public IBinaryTreeNode<T> Max()
        {
            if (m_root == null)
                return null;

            IBinaryTreeNode<T> node = m_root;

            while (node.RightChild != null)
                node = node.RightChild;

            return node;
        }

        public IEnumerable<IBinaryTreeNode<T>> GetRange(K loKey, K hiKey)
        {
            Assert(KeyCompare(loKey, hiKey) <= 0);

            if (m_root == null)
                return Enumerable.Empty<IBinaryTreeNode<T>>();

            var queue = new Queue<IBinaryTreeNode<T>>();
            PushNodes(m_root);

            return queue;

            //-----------
            void PushNodes(IBinaryTreeNode<T> node)
            {
                int loCmp = Compare(loKey, node.Item);
                int hiCmp = Compare(hiKey, node.Item);

                if (loCmp < 0 && node.LeftChild != null)
                    PushNodes(node.LeftChild);

                if (loCmp <= 0 && hiCmp >= 0)
                    queue.Enqueue(node);

                if (hiCmp > 0 && node.RightChild != null)
                    PushNodes(node.RightChild);
            }
        }

        public IBinaryTreeNode<T> GetPredecessor(IBinaryTreeNode<T> node)
        {
            Assert(node != null);
            Assert(this.ContainsNode(node));

            return Predecessor(node as BinaryTree<T>.Node);
        }
        
        public IBinaryTreeNode<T> GetSuccessor(IBinaryTreeNode<T> node)
        {
            Assert(node != null);
            Assert(this.ContainsNode(node));

            return Successor(node as BinaryTree<T>.Node);
        }

        public void ReplaceItem(IBinaryTreeNode<T> node, T item)
        {
            Assert(node != null);
            Assert(this.ContainsNode(node));
            Assert(KeyCompare(KeySelect(node.Item), KeySelect(item)) == 0);

            ((BinaryTree<T>.Node)node).Item = item;
        }

        public void Remove(K key)
        {
            Assert(ContainsKey(key));

            BinaryTree<T>.Node node = LocateNode(key);
            RemoveNode(node);
            --Count;
        }

        public void Clear()
        {
            m_root = null;
            Count = 0;
        }


        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        int ITree<T, IBinaryTreeNode<T>>.GetCount() => Count;

        
        //private:        
        int Compare(K key, T item) => KeyCompare(key, KeySelect(item));

        BinaryTree<T>.Node LocateNode(K key)
        {
            BinaryTree<T>.Node node = m_root;

            while (true)
            {
                K nodekey = KeySelect(node.Item);
                int comp = KeyCompare(key, nodekey);

                if (comp == 0)
                    break;

                if (comp < 0)
                    if (node.LeftChild != null)
                        node = node.LeftChild;
                    else
                        break;
                else if (node.RightChild != null)
                    node = node.RightChild;
                else
                    break;
            }

            return node;
        }
        
        BinaryTree<T>.Node Predecessor(BinaryTree<T>.Node node)
        {
            if (node.LeftChild == null)
            {
                BinaryTree<T>.Node p = node.Parent;

                while (p != null && p.LeftChild == node)
                {
                    node = p;
                    p = p.Parent;
                }

                node = p;
            }
            else
            {
                node = node.LeftChild;

                while (node.RightChild != null)
                    node = node.RightChild;
            }

            return node;

        }

        BinaryTree<T>.Node Successor(BinaryTree<T>.Node node)
        {
            if(node.RightChild == null)
            {
                BinaryTree<T>.Node p = node.Parent;
                
                while(p != null && p.RightChild == node)
                {
                    node = p;
                    p = p.Parent;
                }

                node = p;
            }
            else
            {
                node = node.RightChild;

                while (node.LeftChild != null)
                    node = node.LeftChild;
            }

            return node;
        }

        void ReplaceNode(BinaryTree<T>.Node node, BinaryTree<T>.Node newNode)
        {
            System.Diagnostics.Debug.Assert(node.Degree < 2);

            if (node == m_root)
            {
                m_root.LeftChild = m_root.RightChild = null;
                m_root = newNode;
            }
            else
            {
                BinaryTree<T>.Node parent = node.Parent;
                
                if (parent.LeftChild == node)
                    parent.LeftChild = newNode;
                else
                    parent.RightChild = newNode;
            }
        }

        void RemoveNode(BinaryTree<T>.Node node)
        {
            if (node.IsLeaf)
                ReplaceNode(node, null);
            else if (node.LeftChild == null)
                ReplaceNode(node, node.RightChild);
            else if (node.RightChild == null)
                ReplaceNode(node, node.LeftChild);
            else
            {
                //the choice of using the successor is arbitrary and not symmetric. Why not use the predecessor? 
                //In practice, it is worthwhile to choose at random between the predecessor and the successor.
                // see Algorithms {Y2011, E4} P.410
                BinaryTree<T>.Node nd = Count % 2 == 0 ? Predecessor(node) : Successor(node);
                
                Assert(nd.Degree < 2);

                RemoveNode(nd);

                Assert(nd.IsLeaf);
                Assert(nd.Parent == null);

                node.Item = nd.Item;
            }
        }
    }
}
