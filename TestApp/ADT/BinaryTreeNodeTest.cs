using easyLib.ADT.Trees;
using easyLib.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.ADT
{
    class BinaryTreeNodeTest: UnitTest
    {
        public BinaryTreeNodeTest():
            base("BinaryTreeNode test")
        { }


        //protected:
        protected override void Start()
        {
            byte rootData = SampleFactory.NextByte;
            byte leftData = SampleFactory.NextByte;
            byte rightData = SampleFactory.NextByte;

            var leftChild = new BinaryTree<NodeInfo<int>>.Node(new NodeInfo<int>(1, leftData));
            var rightChild = new BinaryTree<NodeInfo<int>>.Node(new NodeInfo<int>(1, rightData));
            var root = new BinaryTree<NodeInfo<int>>.Node(new NodeInfo<int>(0, rootData), leftChild, rightChild);

            Ensure(root.Item.Data == rootData);
            Ensure(root.LeftChild.Item.Data == leftData);
            Ensure(root.RightChild.Item.Data == rightData);
            Ensure(root.LeftChild.Parent == root);
            Ensure(root.RightChild.Parent == root);
            Ensure(root.IsDescendant(leftChild));
            Ensure(root.IsDescendant(rightChild));
            Ensure(root.LeftChild.IsAncestor(root));
            Ensure(root.RightChild.IsAncestor(root));
            Ensure(root.Children.All(nd => nd == root.LeftChild || nd == root.RightChild));
            Ensure(root.Children.Count() == root.Degree);

            GetPathTest();
        }

        //private:
        void GetPathTest()
        {
            //assume SampleFactoryTest, BasicTreeTest ok
            var binTree = TreeFactory.CreateBinaryTree<int>();

            while(binTree.IsEmpty)
                binTree = TreeFactory.CreateBinaryTree<int>();

            Tree<NodeInfo<int>, BinaryTree<NodeInfo<int>>.Node> tree = binTree;
            
            Ensure(tree.Nodes.
                    All(node => node.GetPath().All(nd => node.IsAncestor(nd))));            
        }
    }
}
