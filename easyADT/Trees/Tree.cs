using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        IEnumerable<ITree<TItem, TNode>> SubTrees { get; }
    }



    public sealed partial class Tree<T>: ITree<T, Tree<T>.Node>
    {
        public Tree() => throw new NotImplementedException();
        public Tree(T root) => throw new NotImplementedException();
        public Tree(Node root, IEnumerable<Node> children = null) => throw new NotImplementedException();

        public Node Root => throw new NotImplementedException();               
        public bool IsEmpty => throw new NotImplementedException();
        public IEnumerable<Node> Nodes => throw new NotImplementedException();
        public IEnumerable<T> Items => throw new NotImplementedException();
        public IEnumerable<Tree<T>> SubTrees => throw new NotImplementedException();        

        public bool Contains(Node node) => throw new NotImplementedException();
        public uint GetNodeCount() => throw new NotImplementedException();        
        public uint GetHeight() => throw new NotImplementedException();

        public void Clear() => throw new NotImplementedException();
        
        public static Tree<T> Merge(params Tree<T>[] trees) => throw new NotImplementedException();
        public static Tree<T> Merge(IEnumerable<Tree<T>> trees) => throw new NotImplementedException();
        public static IEnumerable<Tree<T>> Split(Tree<T> tree) => throw new NotImplementedException();

        IEnumerable<ITree<T, Node>> ITree<T, Node>.SubTrees => throw new NotImplementedException();
    }
}
