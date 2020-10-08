﻿using easyLib.ADT.Trees;
using System;
using System.Collections.Generic;
using static easyLib.DebugHelper;


namespace easyLib.ADT.Heaps
{
    public sealed class LinkedHeap<T> : Heap<T>
    {
        readonly BinaryTree<T> m_tree;
        readonly Func<T, T, bool> m_before;
        BinaryTree<T>.Node m_tail;
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


        //protected:
        protected override int GetItemCount() => m_count;

        protected override T PeekItem() => m_tree.Root.Item;

        protected override T PopItem()
        {
            T result = m_tree.Root.Item;


            if (--m_count != 0)
            {
                BinaryTree<T>.Node node = m_tail;

                Assert(m_tail != m_tree.Root);
                m_tail = Predecessor(m_tail);

                m_tree.Root.Item = node.Item;

                if (node.Parent.RightChild == node)
                    node.Parent.RightChild = null;
                else
                    node.Parent.LeftChild = null;

                Assert(node.IsLeaf && (node.Parent == null));


                node = m_tree.Root;

                while (!node.IsLeaf)
                {
                    BinaryTree<T>.Node child = node.RightChild == null ? node.LeftChild :
                        (m_before(node.LeftChild.Item, node.RightChild.Item) ? node.LeftChild : node.RightChild);

                    if (!m_before(child.Item, node.Item))
                        break;

                    (node.Item, child.Item) = (child.Item, node.Item);
                    node = child;
                }

                Assert(m_count <= 1 || !m_before(m_tree.Root.LeftChild.Item, m_tree.Root.Item));
                Assert(m_count <= 2 || !m_before(m_tree.Root.RightChild.Item, m_tree.Root.Item));
            }
            else
                m_tail = m_tree.Root = null;


            Assert(ClassInvariant);
            return result;
        }

        protected override void AddItem(T item)
        {
            if (IsEmpty)
                m_tree.Root = m_tail = new BinaryTree<T>.Node(item);
            else
            {
                var node = new BinaryTree<T>.Node(item);
                SetSuccessor(m_tail, node);
                m_tail = node;

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
        BinaryTree<T>.Node Predecessor(BinaryTree<T>.Node node)
        {
            BinaryTree<T>.Node p = node.Parent;

            if (p.RightChild == node)
                return p.LeftChild;

            if (p == m_tree.Root)
                return p;

            node = p.Parent;

            while (node != m_tree.Root && node.LeftChild == p)
            {
                p = node;
                node = node.Parent;
            }

            if (node.LeftChild != p)
                node = node.LeftChild;

            do
                node = node.RightChild;
            while (!node.IsLeaf);

            return node;
        }

        void SetSuccessor(BinaryTree<T>.Node node, BinaryTree<T>.Node successor)
        {
            if (node == m_tree.Root)
            {
                node.LeftChild = successor;
                return;
            }


            BinaryTree<T>.Node p = node.Parent;

            if (p.LeftChild == node)
            {
                p.RightChild = successor;
                return;
            }

            if (p == m_tree.Root)
            {
                p.LeftChild.LeftChild = successor;
                return;
            }


            node = p.Parent;

            while (node.RightChild == p && node != m_tree.Root)
            {
                p = node;
                node = node.Parent;
            }

            if (node.RightChild != p)
                node = node.RightChild;

            while (!node.IsLeaf)
                node = node.LeftChild;

            node.LeftChild = successor;
        }

        bool ClassInvariant => m_count == m_tree.GetCount() &&
            m_tree.IsEmpty || m_tree.IsComplete();
    }
}
