using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static easyLib.DebugHelper;


namespace easyLib.ADT.Trees
{

    public interface ITree<out TItem, TNode>
        where TNode : INode<TItem>
    {
        TNode Root { get; }
        IEnumerable<TNode> Nodes { get; }
        IEnumerable<TItem> Items { get; }
        bool IsEmpty { get; }
        bool Contains(TNode node);
        uint GetNodeCount();
        uint GetHeight();
    }
    //-------------------------------------------------

    public abstract class Tree<TItem, TNode> : ITree<TItem, TNode>
        where TNode : class, INode<TItem>
    {
        TNode m_root;

        public TNode Root => m_root;
        public IEnumerable<TNode> Nodes => this.Enumerate(TraversalOrder.PreOrder);
        public IEnumerable<TItem> Items => Nodes.Select(node => node.Item);
        public bool IsEmpty => m_root == null;

        public bool Contains(TNode node)
        {
            Assert(node != null);

            INode<TItem> nd = node;

            do
            {
                if (nd == m_root)
                    return true;

                nd = nd.Parent;

            } while (nd != null);

            return false;
        }

        public uint GetHeight()
        {
            Assert(!IsEmpty);

            if (m_root.Degree == 0)
                return 0;

            var heights = new uint[m_root.Degree];

            Parallel.ForEach(m_root.Children, (node, _, ndx)
                => heights[ndx] = GetHeight(node));

            return heights.Max() + 1;

            //---
            uint GetHeight(INode<TItem> node)
            {
                uint h = 0;
                foreach (TNode nd in node.Children)
                    h = Math.Max(h, GetHeight(nd) + 1);

                return h;
            }
        }

        public uint GetNodeCount() => m_root?.GetDescendantCount() ?? 0;

        public void Clear()
        {
            m_root = null;

            Assert(ClassInvariant);
        }


        //protected:
        protected Tree(TNode root = null)
        {
            Assert(root == null || root.Parent == null);

            m_root = root;

            Assert(ClassInvariant);
        }


        protected virtual bool ClassInvariant => (IsEmpty || Root.Parent == null);
    }
}
