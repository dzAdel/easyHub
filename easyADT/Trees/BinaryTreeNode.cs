namespace easyLib.ADT.Trees
{
    public interface IBinaryTreeNode<T> : INode<T>
    {
        IBinaryTreeNode<T> LeftChild { get; }
        IBinaryTreeNode<T> RightChild { get; }
    }

}
