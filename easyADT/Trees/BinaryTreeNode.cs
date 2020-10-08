namespace easyLib.ADT.Trees
{
    public interface IBinaryTreeNode<out T> : ITreeNode<T>
    {
        IBinaryTreeNode<T> LeftChild { get; }
        IBinaryTreeNode<T> RightChild { get; }
    }

}
