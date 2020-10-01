using easyLib.ADT.Trees;
using easyLib.Test;
using System;
using System.Collections.Generic;
using System.Linq;


namespace TestApp.ADT
{
    class BasicTreeTest : UnitTest
    {
        //Assume BasicTreeNodeTest ok
        public BasicTreeTest() :
            base("BasicTree<> Test")
        { }


        //protected:
        protected override void Start()
        {
            //assume SampleFactoryTest ok
            var emptyTree = new BasicTree<int>();
            Ensure(emptyTree.IsEmpty);
            Ensure(emptyTree.Root == null);

            var randByte = SampleFactory.CreateBytes().First();

            var tree = TreeFactory.CreateTree<int>(nd => nd.Item.Data = nd.Item.GetHashCode());

            Ensure(tree.Items.All(item => item.Data == item.GetHashCode()));
            Ensure(tree.GetNodeCount() == (tree.IsEmpty ? 0 : tree.Root.Item.DescendantCount));
            Ensure(tree.Nodes.All(nd => tree.Contains(nd)));
            Ensure(tree.Leaves.All(nd => nd.IsLeaf));
            Ensure(tree.Nodes.Except(tree.Leaves).All(nd => !nd.IsLeaf));

            while (tree.IsEmpty)
                tree = TreeFactory.CreateTree<int>();

            var roots = new List<ITreeNode<INodeInfo<int>>>();

            roots.Add(tree.Root);
            roots.AddRange(tree.Root.Children);


            var trees = BasicTree<INodeInfo<int>>.Split(tree).ToList();

            Ensure(roots.Count == trees.Count);
            Ensure(trees.All(t => roots.Contains(t.Root)));

            tree = TreeFactory.CreateTree<int>();
            tree.Clear();

            Ensure(tree.IsEmpty);

            PostOrderTest();
            PreOrderTest();
            InOrderTest();
            BreadthFirstTest();
            GetHeightTest();
            PathLengthTest();
        }

        //private:

        void PathLengthTest()
        {
            //assume SampleTestFactory(); PreOrderTest() ok
            var tree = TreeFactory.CreateTree<int>();

            while (tree.IsEmpty)
                tree = TreeFactory.CreateTree<int>();

            Trace("PathLengthTest()\nTree properties:",
                 $"Descendants count: {tree.Root?.Item.DescendantCount ?? 0}");


            GetExternalPathLengthTest(tree);
            GetInternalPathLengthTest(tree);
        }

        void GetExternalPathLengthTest(BasicTree<INodeInfo<int>> tree)
        {
            int len = 0;
            foreach (var node in tree.Nodes)
                if (node.IsLeaf)
                    len += node.Item.Depth;

            Ensure(len == tree.GetExternalPathLength());
        }

        void GetInternalPathLengthTest(BasicTree<INodeInfo<int>> tree)
        {
            int len = 0;
            foreach (var node in tree.Nodes)
                if (!node.IsLeaf)
                    len += node.Item.Depth;

            Ensure(len == tree.GetInternalPathLength());
        }

        void PostOrderTest()
        {
            //assume SampleFactoryTest() ok
            var tree = CreatePostOrderTree();

            Trace("PostOrderTest()\nTree properties:",
                $"IsEmpty: {tree.IsEmpty}",
                $"Descendants count: {tree.Root?.Item.DescendantCount ?? 0}");


            Ensure(tree.Enumerate(TraversalOrder.PostOrder).Count() == tree.GetNodeCount());

            if (tree.IsEmpty)
            {
                int ndx = 0;
                Ensure(tree.Enumerate(TraversalOrder.PostOrder).All(node => node.Item.Data == ndx++));
            }
        }

        void InOrderTest()
        {
            //assume SampleFactoryTest() ok
            var tree = CreateInOrderTree();
            Trace("InOrderTest()\nTree properties:",
                $"IsEmpty: {tree.IsEmpty}",
                $"Descendants count: {tree.Root?.Item.DescendantCount ?? 0}");

            Ensure(tree.Enumerate(TraversalOrder.InOrder).Count() == tree.GetNodeCount());

            if (tree.IsEmpty)
            {
                int ndx = 0;
                Ensure(tree.Enumerate(TraversalOrder.InOrder).All(node => node.Item.Data == ndx++));
            }
        }

        void PreOrderTest()
        {
            //assume SampleFactoryTest() ok
            var tree = CreatePreOrderTree();
            Trace("PretOrderTest()\nTree properties:",
                $"IsEmpty: {tree.IsEmpty}",
                $"Descendants count: {tree.Root?.Item.DescendantCount ?? 0}");

            Ensure(tree.Enumerate(TraversalOrder.PreOrder).Count() == tree.GetNodeCount()); ;

            if (tree.IsEmpty)
            {
                int ndx = 0;
                Ensure(tree.Enumerate(TraversalOrder.PreOrder).All(node => node.Item.Data == ndx++));
            }
        }

        void BreadthFirstTest()
        {
            //assume SampleFactoryTest, InOrderTest() ok
            var tree = CreateLevelOrderTree();

            Trace("BreadthFirstTest()\nTree properties:",
                $"IsEmpty: {tree.IsEmpty}",
                $"Descendants count: {tree.Root?.Item.DescendantCount ?? 0}");

            Ensure(tree.Enumerate(TraversalOrder.BreadthFirst).Count() == tree.GetNodeCount());
            Ensure(tree.LevelOrderTraversal().Count() == tree.GetNodeCount());

            if (!tree.IsEmpty)
            {
                int ndx = 0;
                Ensure(tree.Enumerate(TraversalOrder.BreadthFirst).All(node => node.Item.Data == ndx++));
                Ensure(tree.LevelOrderTraversal().All(pair => pair.level == pair.node.Item.Depth));
            }

            
        }

        BasicTree<INodeInfo<int>> CreateLevelOrderTree()
        {
            var tree = TreeFactory.CreateTree<int>();

            if (!tree.IsEmpty)
            {
                int maxLevel = TagLevelNode(tree.Root, 0);
                int prevTag = (maxLevel << 1) - 1;

                for (int lvl = 0; lvl <= maxLevel; ++lvl)
                    foreach (var node in tree.Nodes.Where(nd => nd.Item.Data == lvl))
                        node.Item.Data = ++prevTag;


                foreach (var node in tree.Nodes)
                    node.Item.Data -= maxLevel << 1;
            }

            return tree;
        }

        void GetHeightTest()
        {
            //assume SampleFactoryTest ok
            var tree = TreeFactory.CreateTree<int>();

            while (tree.IsEmpty)
                tree = TreeFactory.CreateTree<int>();

            Trace("GetHeightTest()\nTree properties:",
                $"Descendants count: {tree.Root?.Item.DescendantCount ?? 0}");

            int maxLevel = TagLevelNode(tree.Root, 0);
            Ensure(tree.GetHeight() == maxLevel);
        }

        static BasicTree<INodeInfo<int>> CreatePostOrderTree()
        {
            int tag = 0;

            Func<ITreeNode<INodeInfo<int>>, int> tagger = node => node.Item.Data = tag++;

            return TreeFactory.CreateTree<int>(tagger);
        }

        static BasicTree<INodeInfo<int>> CreatePreOrderTree()
        {
            var tree = TreeFactory.CreateTree<int>();

            if (!tree.IsEmpty)
                TagNode(tree.Root);

            return tree;

            //---------
            void TagNode(ITreeNode<INodeInfo<int>> node)
            {
                if (node.IsRoot)
                    node.Item.Data = 0;
                else
                    node.Item.Data = Pred(node).Item.Data + 1;


                foreach (var child in node.Children)
                    TagNode(child);
            }

            ITreeNode<INodeInfo<int>> Pred(ITreeNode<INodeInfo<int>> node)
            {
                if (node.IsRoot)
                    return null;

                if (node.Parent.Children.First() == node)
                    return node.Parent;

                var pred = PrevSibling(node);

                while (!pred.IsLeaf)
                    pred = pred.Children.Last();

                return pred;
            }
        }

        static BasicTree<INodeInfo<int>> CreateInOrderTree()
        {
            var tree = TreeFactory.CreateTree<int>(_ => -1);
            ITreeNode<INodeInfo<int>> pred = null;

            if (!tree.IsEmpty)
                TagNode(tree.Root);

            return tree;

            //---------
            void TagNode(ITreeNode<INodeInfo<int>> node)
            {
                if (node.IsLeaf)
                {
                    node.Item.Data = pred == null ? 0 : pred.Item.Data + 1;
                    pred = node;
                }
                else
                {
                    TagNode(node.Children.First());
                    node.Item.Data = pred.Item.Data + 1;
                    pred = node;

                    foreach (var child in node.Children.Skip(1))
                        TagNode(child);
                }
            }
        }

        static ITreeNode<INodeInfo<int>> PrevSibling(ITreeNode<INodeInfo<int>> node)
        {
            ITreeNode<INodeInfo<int>> prevSibling = null;

            foreach (var child in node.Parent.Children)
                if (child != node)
                    prevSibling = child;
                else
                    break;

            return prevSibling;
        }

        int TagLevelNode(ITreeNode<INodeInfo<int>> node, int lvl)
        {
            int lvlMax = lvl;
            node.Item.Data = lvl;

            foreach (var child in node.Children)
                lvlMax = Math.Max(lvlMax, TagLevelNode(child, lvl + 1));

            return lvlMax;
        }
    }
}
