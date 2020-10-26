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
            base("BinaryTree<> Test")
        { }


        //protected:
        protected override void Start()
        {
            ConstructionTest();
            PreOrderTest();
            PostOrderTest();
            BreadthFirstTest();
            LevelOrderTest();
            MathPropertiesTest();
            ProperBTTest();
            ImproperBTTest();
            ProperBTMathPropertiesTest();
            CompleteBTTest();
            IncompleteBTTest();
            SubTreesMergeBTTest();
            BuildTreeTest();
        }


        //private:
        void ConstructionTest()
        {
            var bt = new BinaryTree<int>();
            Ensure(bt.IsEmpty);
            Ensure(bt.Root == null);
            Ensure(!bt.Nodes.Any());
            Ensure(bt.GetCount() == 0);

            bt = new BinaryTree<int>(default(int));
            Ensure(!bt.IsEmpty);
            Ensure(bt.Root.Item == default);
            Ensure(bt.Nodes.Single() == bt.Root);
            Ensure(bt.GetCount() == 1);

            bt.Root = null;
            Ensure(bt.IsEmpty);
            Ensure(bt.Root == null);
            Ensure(!bt.Nodes.Any());
            Ensure(bt.GetCount() == 0);

            bt.Root = new BinaryTree<int>.Node(default);
            Ensure(!bt.IsEmpty);
            Ensure(bt.Root.Item == default);
            Ensure(bt.Nodes.Single() == bt.Root);
            Ensure(bt.GetCount() == 1);

            bt.Clear();
            Ensure(bt.IsEmpty);
        }

        void PreOrderTest()
        {
            int ndx = 0;
            Func<BinaryTree<int>.Node, int> itemProvider = _ => ndx++;

            BinaryTree<int> bt;

            do
                bt = TreeFactory.CreateBinaryTree<int>();
            while (bt.IsEmpty);

            foreach (var nd in bt.Enumerate(TraversalOrder.PreOrder))
                nd.Item = itemProvider(nd);

            Ensure(bt.Enumerate(TraversalOrder.PreOrder).First() == bt.Root);
            Ensure(bt.Enumerate(TraversalOrder.PreOrder).Select(nd => nd.Item).Min() == bt.Root.Item);
            Ensure(bt.Enumerate(TraversalOrder.PreOrder).
                Skip(1).
                All(nd => nd.Item > nd.Parent.Item));

            Ensure(bt.Enumerate(TraversalOrder.PreOrder).
                Where(nd => nd.Degree == 2).
                All(nd => nd.LeftChild.Item < nd.RightChild.Item));
        }

        void PostOrderTest()
        {
            int ndx = 0;
            Func<BinaryTree<int>.Node, int> itemProvider = _ => ndx++;
            BinaryTree<int> bt;

            do
                bt = TreeFactory.CreateBinaryTree<int>();
            while (bt.IsEmpty);

            foreach (var nd in bt.Enumerate(TraversalOrder.PostOrder))
                nd.Item = itemProvider(nd);

            Ensure(bt.Enumerate(TraversalOrder.PostOrder).Last() == bt.Root);
            Ensure(bt.Enumerate(TraversalOrder.PostOrder).Select(nd => nd.Item).Max() == bt.Root.Item);

            Ensure(bt.Enumerate(TraversalOrder.PostOrder).
                Where(nd => nd != bt.Root).
                All(nd => nd.Item < nd.Parent.Item));

            Ensure(bt.Enumerate(TraversalOrder.PreOrder).
                Where(nd => nd.Degree == 2).
                All(nd => nd.LeftChild.Item < nd.RightChild.Item));
        }

        void BreadthFirstTest()
        {
            int ndx = 0;
            Func<BinaryTree<int>.Node, int> itemProvider = _ => ndx++;
            BinaryTree<int> bt;

            do
                bt = TreeFactory.CreateBinaryTree<int>();
            while (bt.IsEmpty);

            foreach (var nd in bt.Enumerate(TraversalOrder.BreadthFirst))
                nd.Item = itemProvider(nd);

            Ensure(bt.Enumerate(TraversalOrder.BreadthFirst).First() == bt.Root);
            Ensure(bt.Enumerate(TraversalOrder.BreadthFirst).Select(nd => nd.Item).Min() == bt.Root.Item);

            Ensure(bt.Enumerate(TraversalOrder.BreadthFirst).
                Where(nd => nd != bt.Root).
                All(nd => nd.Item > nd.Parent.Item));

            Ensure(bt.Enumerate(TraversalOrder.BreadthFirst).
                Where(nd => nd.Degree == 2).
                All(nd => nd.LeftChild.Item + 1 == nd.RightChild.Item));
        }

        void LevelOrderTest()
        {
            int ndx = 0;
            Func<BinaryTree<int>.Node, int> itemProvider = _ => ndx++;
            BinaryTree<int> bt;

            do
                bt = TreeFactory.CreateBinaryTree<int>();
            while (bt.IsEmpty);

            foreach (var nd in bt.Enumerate(TraversalOrder.BreadthFirst))
                nd.Item = itemProvider(nd);

            Ensure(bt.LevelOrderTraversal().All(p => bt.GetDepth(p.node) == p.level));
            Ensure(bt.LevelOrderTraversal().First().level == 0);
            Ensure(bt.LevelOrderTraversal().Last().level == bt.GetHeight());
            Ensure(bt.LevelOrderTraversal().Select(p => p.node.Item).
                SequenceEqual(bt.Enumerate(TraversalOrder.BreadthFirst).Select(nd => nd.Item)));
        }

        void MathPropertiesTest()
        {
            BinaryTree<int> bt;

            do
                bt = TreeFactory.CreateBinaryTree<int>();
            while (bt.IsEmpty);


            int h = bt.GetHeight();
            int n = bt.GetCount();

            Ensure(h + 1 <= n && n <= Math.Pow(2, h + 1) - 1);

            var (ni, ne) = CountNode(bt);

            Ensure(1 <= ne && ne <= Math.Pow(2, h));
            Ensure(h <= ni && ni <= Math.Pow(2, h) - 1);
            Ensure(Math.Log2(n + 1) - 1 <= h && h <= n - 1);
        }

        void ProperBTTest()
        {
            BinaryTree<int> bt;

            do
                bt = TreeFactory.CreateProperBinaryTree<int>();
            while (bt.IsEmpty);

            Ensure(bt.IsProper());
        }

        void ImproperBTTest()
        {
            BinaryTree<int> bt;

            do
                bt = TreeFactory.CreateProperBinaryTree<int>();
            while (bt.IsEmpty || bt.GetHeight() < 4);

            var deepestNode = bt.Root;
            var dp = 0;

            foreach (var node in bt.Nodes)
            {
                var d = bt.GetDepth(node);

                if (d >= dp)
                {
                    deepestNode = node;
                    dp = d;
                }
            }


            var improperNode = bt.GetPath(deepestNode).Skip(dp / 2).First();
            Assert(improperNode.Degree == 2);

            if (SampleFactory.CreateBools().First())
                improperNode.LeftChild = null;
            else
                improperNode.RightChild = null;

            Ensure(!bt.IsProper());
        }

        void ProperBTMathPropertiesTest()
        {
            BinaryTree<int> bt;

            do
                bt = TreeFactory.CreateProperBinaryTree<int>();
            while (bt.IsEmpty);


            int h = bt.GetHeight();
            int n = bt.GetCount();

            Ensure(2 * h + 1 <= n && n <= Math.Pow(2, h + 1) - 1);

            var (ni, ne) = CountNode(bt);
            Ensure(h + 1 <= ne && ne <= Math.Pow(2, h));
            Ensure(h <= ni && ni <= Math.Pow(2, h) - 1);
            Ensure(Math.Log2(n + 1) - 1 <= h && h <= (n - 1) / 2);
            Ensure(ne == ni + 1);
        }

        void CompleteBTTest()
        {
            BinaryTree<int> bt;

            do
                bt = TreeFactory.CreateCompleteBinaryTree<int>();
            while (bt.IsEmpty);


            Ensure(bt.IsComplete());
        }

        void IncompleteBTTest()
        {
            BinaryTree<int> bt;

            do
                bt = TreeFactory.CreateCompleteBinaryTree<int>();
            while (bt.IsEmpty || bt.GetHeight() < 4);

            var node = bt.Leaves.First();
            node.Parent.RightChild = node;

            Ensure(!bt.IsComplete());

            do
                bt = TreeFactory.CreateCompleteBinaryTree<int>();
            while (bt.IsEmpty || bt.GetHeight() < 4);

            bt.Root.RightChild = null;
            Ensure(!bt.IsComplete());

            do
                bt = TreeFactory.CreateCompleteBinaryTree<int>();
            while (bt.IsEmpty || bt.GetHeight() < 2);

            var array = bt.Enumerate(TraversalOrder.BreadthFirst).Skip(1).ToArray();
            var ndx = SampleFactory.CreateInts(0, array.Length - 1).First();
            var p = array[ndx].Parent;

            if (p.LeftChild == array[ndx])
                p.LeftChild = null;
            else
                p.RightChild = null;

            Assert(bt.GetCount() > 1);

            Ensure(!bt.IsComplete());
        }

        void SubTreesMergeBTTest()
        {
            byte level = SampleFactory.CreateBytes(limit: 32).First();
            int ndx = 0;
            Func<BinaryTree<int>.Node, int> itemProvider = _ => ndx++;
            var bt = TreeFactory.CreateBinaryTree<int>(level, itemProvider);

            var subTrees = bt.SubTrees().ToArray();
            Ensure(subTrees.Length == 2);
            Ensure(subTrees[0]?.Root == bt.Root.LeftChild);
            Ensure(subTrees[1]?.Root == bt.Root.RightChild);
            Ensure(subTrees[0].All(nd => bt.Contains(nd)));
            Ensure(subTrees[1].All(nd => bt.Contains(nd)));
            Ensure(!subTrees[0].ContainsNode(bt.Root));
            Ensure(!subTrees[1].ContainsNode(bt.Root));

            var btItems = bt.ToArray();

            var bt1 = BinaryTree<int>.Merge(bt.Root.Item, subTrees[0], subTrees[1]);
            var bt1Items = bt1.ToArray();

            Ensure(bt1Items.SequenceEqual(btItems));
        }

        void BuildTreeTest()
        {
            int ndx = 0;
            Func<BinaryTree<int>.Node, int> itemProvider = _ => ndx++;
            var srcTree = TreeFactory.CreateBinaryTree<int>(itemProvider);
            var inOrderSeq = srcTree.Enumerate(TraversalOrder.InOrder).Select(node => node.Item);
            var postOrderSeq = srcTree.Enumerate(TraversalOrder.PostOrder).Select(node => node.Item);
            var preOrderSeq = srcTree.Enumerate(TraversalOrder.PreOrder).Select(node => node.Item);

            var tree = BinaryTree<int>.BuildTree(inOrderSeq.ToList(), postOrderSeq.ToList(), TraversalOrder.PostOrder);
            Ensure(srcTree.SequenceEqual(tree));

            tree = BinaryTree<int>.BuildTree(inOrderSeq.ToList(), preOrderSeq.ToList(), TraversalOrder.PreOrder);
            Ensure(srcTree.SequenceEqual(tree));
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
