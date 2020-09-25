using easyLib.ADT.Trees;
using easyLib.Test;
using System;
using System.Linq;
using static easyLib.DebugHelper;


namespace TestApp.ADT
{
    class BinaryTreeTest : UnitTest
    {
        public BinaryTreeTest() :
            base("BinaryTree<> test")
        { }


        //protected:
        protected override void Start()
        {
            BTTest();
            ProperBTTest();
            ImproperBTTest();
            BTMathPropertiesTest();
            ProperBTMathPropertiesTest();
            BuildTreeTest();
        }

        //private:
        void BuildTreeTest()
        {
            var srcTree = TreeFactory.CreateBinaryTree<int>();
            var inOrderSeq = srcTree.Enumerate(TraversalOrder.InOrder).Select(node => node.Item);
            var postOrderSeq = srcTree.Enumerate(TraversalOrder.PostOrder).Select(node => node.Item);
            var preOrderSeq = srcTree.Enumerate(TraversalOrder.PreOrder).Select(node => node.Item);
            var tree = BinaryTree<NodeInfo<int>>.BuildTree(inOrderSeq.ToList(), postOrderSeq.ToList(), TraversalOrder.PostOrder);

            var seq = tree.Items.Select((item, ndx) => new { Item0 = item, Item1 = srcTree.Items.ElementAt(ndx) });
            Ensure(seq.All(pair => pair.Item0.Equals(pair.Item1)));

            tree = BinaryTree<NodeInfo<int>>.BuildTree(inOrderSeq.ToList(), preOrderSeq.ToList(), TraversalOrder.PreOrder);
            seq = tree.Items.Select((item, ndx) => new { Item0 = item, Item1 = srcTree.Items.ElementAt(ndx) });
            Ensure(seq.All(pair => pair.Item0.Equals(pair.Item1)));
        }

        void ProperBTMathPropertiesTest()
        {
            var bt = TreeFactory.CreateProperBinaryTree<int>();

            while (bt.IsEmpty)
                bt = TreeFactory.CreateProperBinaryTree<int>();

            int h = bt.GetHeight();
            int n = bt.GetNodeCount();

            Ensure(2 * h + 1 <= n && n <= Math.Pow(2, h + 1) - 1);

            var (ni, ne) = CountNode(bt);
            Ensure(h + 1 <= ne && ne <= Math.Pow(2, h));
            Ensure(h <= ni && ni <= Math.Pow(2, h) - 1);
            Ensure(Math.Log2(n + 1) - 1 <= h && h <= (n - 1) / 2);
            Ensure(ne == ni + 1);
        }

        void BTMathPropertiesTest()
        {
            var bt = TreeFactory.CreateBinaryTree<int>();

            while (bt.IsEmpty)
                bt = TreeFactory.CreateBinaryTree<int>();

            int h = bt.GetHeight();
            int n = bt.GetNodeCount();

            Ensure(h + 1 <= n && n <= Math.Pow(2, h + 1) - 1);

            var (ni, ne) = CountNode(bt);

            Ensure(1 <= ne && ne <= Math.Pow(2, h));
            Ensure(h <= ni && ni <= Math.Pow(2, h) - 1);
            Ensure(Math.Log2(n + 1) - 1 <= h && h <= n - 1);
        }

        void BTTest()
        {
            byte level = SampleFactory.CreateBytes(limit: 17).First();
            var bt = TreeFactory.CreateBinaryTree<int>(level);


            Trace("BTTest()\nBinaryTree properties:",
                $"Max Level: {level}",
                $"Nber of nodes: {bt.GetNodeCount()}");

            Ensure(bt.GetNodeCount() == bt.Nodes.Count());
            Ensure(bt.GetHeight() <= level);

            foreach (var node in bt.Nodes)
            {
                Ensure(bt.Contains(node));
                Ensure(node.Item.Depth == node.GetDepth());
                Ensure(node.Item.DescendantCount == node.GetDescendantCount());
            }

            bt.Clear();
            Ensure(bt.IsEmpty);
        }


        void ProperBTTest()
        {
            var bt = TreeFactory.CreateProperBinaryTree<int>();

            Trace("ProperBTTest()\nBinaryTree properties:",
                $"Height: {bt.GetHeight()}",
                $"Nber of nodes: {bt.GetNodeCount()}");


            Ensure(bt.IsProper());
        }

        void ImproperBTTest()
        {
            BinaryTree<NodeInfo<int>> bt = TreeFactory.CreateProperBinaryTree<int>();

            while (bt.IsEmpty || bt.GetHeight() < 4)
                bt = TreeFactory.CreateProperBinaryTree<int>();

            Trace("ImproperBTTest()\nBinaryTree properties:",
                $"Nber of nodes: {bt.GetNodeCount()}");

            var deepestNode = bt.Root;

            foreach (var node in bt.Nodes)
                if (node.Item.Depth >= deepestNode.Item.Depth)
                    deepestNode = node;

            var improperNode = deepestNode.GetPath().Skip(deepestNode.Item.Depth / 2).First();
            Assert(improperNode.Degree == 2);

            if (SampleFactory.CreateBools().First())
                improperNode.LeftChild = null;
            else
                improperNode.RightChild = null;

            Ensure(!bt.IsProper());
        }

        static (int interCount, int exterCount) CountNode<T>(BinaryTree<T> tree)
        {
            int intCount = 0;
            int extCount = 0;

            foreach (var node in tree.Nodes)
                if (node.IsLeaf)
                    ++extCount;
                else
                    ++intCount;

            return (intCount, extCount);

        }
    }
}
