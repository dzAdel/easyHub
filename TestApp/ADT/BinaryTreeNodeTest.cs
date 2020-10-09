using easyLib.ADT.Trees;
using easyLib.Test;
using System;
using System.Linq;

namespace TestApp.ADT
{
    class BinaryTreeNodeTest : UnitTest
    {
        public BinaryTreeNodeTest() :
            base("BinaryTreeNode Test")
        { }


        //protected:
        protected override void Start()
        {
            ConstructionTest();
            PathTest();
            InsertionRemovalTest();
        }

        //private:
        void InsertionRemovalTest()
        {
            var node = TreeFactory.CreateBinaryNode<int>();
            var left = TreeFactory.CreateBinaryNode<int>();

            node.LeftChild = left;
            Ensure(node.LeftChild == left);
            Ensure(left.Parent == node);
            Ensure(node.Children.First() == left);

            var right = TreeFactory.CreateBinaryNode<int>();
            node.RightChild = right;
            Ensure(node.LeftChild == left);
            Ensure(node.RightChild == right);
            Ensure(right.Parent == node);
            Ensure(node.Children.Last() == right);
            Ensure(node.GetDescendantCount() == left.GetDescendantCount() + right.GetDescendantCount() + 1);


            node.LeftChild = null;
            Ensure(node.LeftChild == null);
            Ensure(left.Parent == null);

            node.RightChild = null;
            Ensure(node.RightChild == null);
            Ensure(right.Parent == null);
            Ensure(!node.Children.Any());
        }

        void ConstructionTest()
        {
            var node = new BinaryTree<int>.Node(default);
            Ensure(node.Parent == null);
            Ensure(node.Item == default);
            Ensure(node.LeftChild == null);
            Ensure(node.RightChild == null);
            Ensure(node.IsLeaf);
            Ensure(node.Degree == 0);
            Ensure(!node.Children.Any());

            do
                node = TreeFactory.CreateBinaryNode<int>();
            while (node.Degree != 2);

            Ensure(node.LeftChild != null);
            Ensure(node.RightChild != null);
            Ensure(node.LeftChild.Parent == node);
            Ensure(node.RightChild.Parent == node);
            Ensure(node.RightChild != node.LeftChild);
            Ensure(node.Children.Count() == node.Degree);
        }

        void PathTest()
        {
            var node = TreeFactory.CreateBinaryNode<int>();

            Ensure(node.Children.All(nd => nd.GetPath().First() == node));
            Ensure(node.Children.All(nd => nd.GetPath().Last() == nd));
            Ensure(node.Children.All(nd => nd.GetPath().Reverse().ElementAt(1) == nd.Parent));
            Ensure(node.Children.All(nd => nd.IsDescendantOf(node)));
            Ensure(node.Children.All(nd => node.IsAncestorOf(nd)));
        }
    }
}
