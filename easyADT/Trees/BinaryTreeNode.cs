namespace easyLib.ADT.Trees
{
    public interface IBinaryTreeNode<T> : ITreeNode<T>
    {
        IBinaryTreeNode<T> LeftChild { get; }
        IBinaryTreeNode<T> RightChild { get; }
    }

}
