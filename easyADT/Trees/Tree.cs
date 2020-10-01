using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static easyLib.DebugHelper;


namespace easyLib.ADT.Trees
{

    public interface ITree<out TItem, TNode>
        where TNode : ITreeNode<TItem>
    {
        TNode Root { get; }
        IEnumerable<TNode> Nodes { get; }
        IEnumerable<TNode> Leaves { get; }
        IEnumerable<TItem> Items { get; }
        bool IsEmpty { get; }
        bool Contains(TNode node);
        int GetNodeCount();
        int GetHeight();
    }
    //-------------------------------------------------

    public abstract class Tree<TItem, TNode> : ITree<TItem, TNode>
        where TNode : class, ITreeNode<TItem>
    {
        TNode m_root;

        public TNode Root => m_root;
        public IEnumerable<TNode> Nodes => this.Enumerate(TraversalOrder.PreOrder);
        public IEnumerable<TNode> Leaves => this.Enumerate(TraversalOrder.PreOrder).Where(nd => nd.IsLeaf);
        public IEnumerable<TItem> Items => Nodes.Select(node => node.Item);
        public bool IsEmpty => m_root == null;

        public bool Contains(TNode node)
        {
            Assert(node != null);

            ITreeNode<TItem> nd = node;

            do
            {
                if (nd == m_root)
                    return true;

                nd = nd.Parent;

            } while (nd != null);

            return false;
        }

        public int GetHeight()
        {
            Assert(!IsEmpty);

            if (m_root.Degree == 0)
                return 0;

            var heights = new int[m_root.Degree];

            Parallel.ForEach(m_root.Children, (node, _, ndx)
                => heights[ndx] = GetHeight(node));

            return heights.Max() + 1;

            //---
            int GetHeight(ITreeNode<TItem> node)
            {
                int h = 0;
                foreach (TNode nd in node.Children)
                    h = Math.Max(h, GetHeight(nd) + 1);

                return h;
            }
        }

        public int GetNodeCount() => NodeCount();

        public void Clear()
        {
            m_root = null;

            Assert(ClassInvariant);
        }


        //protected:
        protected Tree(TNode root)
        {
            Assert(root == null || root.Parent == null);

            m_root = root;

            Assert(ClassInvariant);
        }

        protected virtual int NodeCount() => m_root?.GetDescendantCount() ?? 0;

        protected virtual bool ClassInvariant => (IsEmpty || Root.Parent == null);
    }
}
