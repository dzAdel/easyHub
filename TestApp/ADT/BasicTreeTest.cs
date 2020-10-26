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
            ConstructionTest();
            PreOrderTest();
            PostOrderTest();
            InOrderTest();
            LevelOrderTest();
            PathTest();
            DepthTest();
            HeightTest();
            SubTreesTest();
            MergeTest();
            PathLengthTest();
            CommonAncestorTest();

        }

        //private:
        void CommonAncestorTest()
        {
            BasicTree<int> tree;

            do
                tree = TreeFactory.CreateBasicTree<int>();
            while (tree.GetCount() < 2);

            var indices = SampleFactory.CreateInts(0, tree.GetCount()).Take(2).ToArray();

            var node0 = tree.Nodes.ElementAt(indices[0]);
            var node1 = tree.Nodes.ElementAt(indices[1]);

            var fca = tree.GetFirstCommonAncestor(node0, node1);

            Ensure(fca.IsAncestorOf(node0));
            Ensure(fca.IsAncestorOf(node1));
            Ensure(node0.GetPath(fca).Intersect(node1.GetPath(fca)).SequenceEqual(Enumerable.Repeat(fca, 1)));
        }

        void MergeTest()
        {
            BasicTree<int> tree;

            int n = 0;
            Func<BasicTree<int>.Node, int> dataProvider = _ => n++;

            do
                tree = TreeFactory.CreateBasicTree(dataProvider);
            while (tree.GetCount() < 2);


            var data = tree.ToArray();
            var trees = tree.SubTrees().ToArray();
            var mergeData = BasicTree<int>.Merge(tree.Root.Item, trees).ToArray();

            Ensure(tree.Root.IsLeaf);
            Ensure(mergeData.SequenceEqual(data));
        }

        void SubTreesTest()
        {
            BasicTree<int> tree;

            do
                tree = TreeFactory.CreateBasicTree<int>();
            while (tree.IsEmpty);


            Ensure(tree.SubTrees().All(t => tree.Root.Children.Contains(t.Root)));
            Ensure(tree.SubTrees().Select(t => t.GetCount()).Sum() == tree.GetCount() - 1);

            var h = tree.GetHeight();
            Ensure(tree.SubTrees().All(t => t.GetHeight() == h - 1));

            var seq = from node in tree.Nodes.Skip(1)
                      from t in tree.SubTrees()
                      select new { Depth = tree.GetDepth(node), STDepth = t.GetDepth(node) };

            Ensure(seq.All(p => p.Depth == p.STDepth + 1));

            Ensure(tree.SubTrees().All(t => !t.ContainsNode(tree.Root)));

            var seq1 = from t in tree.SubTrees()
                       from node in t.Nodes
                       where t.GetPath(node).Contains(tree.Root)
                       select node;

            Ensure(!seq1.Any());
        }

        void HeightTest()
        {
            BasicTree<int> tree;


            Func<BasicTree<int>.Node, int> LevelProvider = nd =>
            {
                if (nd.Parent == null)
                    return 0;

                return nd.Parent.Item + 1;
            };

            do
                tree = TreeFactory.CreateBasicTree<int>(LevelProvider);
            while (tree.IsEmpty);


            int lvl = 0;
            foreach (var node in tree.Nodes)
                lvl = Math.Max(lvl, node.Item);

            var h = tree.GetHeight();
            Ensure(h == lvl);
            Ensure(tree.Nodes.All(nd => tree.GetDepth(nd) <= h));
        }

        void DepthTest()
        {
            BasicTree<int> tree;

            Func<BasicTree<int>.Node, int> DepthProvider = nd => nd.Parent == null ? 0 : nd.Parent.Item + 1;

            do
                tree = TreeFactory.CreateBasicTree<int>(DepthProvider);
            while (tree.IsEmpty);


            Ensure(tree.Nodes.All(nd => nd.Item == tree.GetDepth(nd)));
        }

        void PathTest()
        {
            BasicTree<int> tree;

            do
                tree = TreeFactory.CreateBasicTree<int>();
            while (tree.IsEmpty);


            Ensure(tree.Nodes.All(nd => tree.GetPath(nd).First() == tree.Root));
            Ensure(tree.Nodes.All(nd => tree.GetPath(nd).Last() == nd));
            Ensure(tree.Nodes.All(nd => tree.GetPath(nd).All(node => nd.IsDescendantOf(node))));
            Ensure(tree.Nodes.All(nd => tree.GetPath(nd).All(node => node.IsAncestorOf(nd))));

            var seq = from node in tree.Nodes
                      let path = tree.GetPath(node)
                      from nd in tree.Nodes
                      where nd.IsAncestorOf(node) && !path.Contains(nd)
                      select nd;

            Ensure(!seq.Any());
        }

        void LevelOrderTest()
        {
            BasicTree<int> tree;

            do
                tree = TreeFactory.CreateBasicTree<int>(ItemProvider);
            while (tree.IsEmpty);

            Ensure(tree.Root.Item == 0);
            Ensure(tree.Enumerate(TraversalOrder.BreadthFirst).
                Where(nd => nd != tree.Root).
                All(nd => nd.Item == nd.Parent.Item + 1));

            int lvl = 0;
            Func<BasicTree<int>.Node, bool> pred = nd =>
            {
                bool result = nd.Item >= lvl;
                lvl = nd.Item;

                return result;
            };

            Ensure(tree.Enumerate(TraversalOrder.BreadthFirst).All(pred));
            Ensure(tree.LevelOrderTraversal().All(pair => pair.level == pair.node.Item));
            Ensure(tree.LevelOrderTraversal().
                Select(pair => pair.node).
                SequenceEqual(tree.Enumerate(TraversalOrder.BreadthFirst)));


            //----
            int ItemProvider(BasicTree<int>.Node node)
            {
                if (node.Parent == null)
                    return 0;

                return node.Parent.Item + 1;
            }
        }

        void InOrderTest()
        {
            BasicTree<int> tree;

            do
                tree = TreeFactory.CreateBasicTree<int>();
            while (tree.IsEmpty);

            int n = 0;
            foreach (var node in tree.Enumerate(TraversalOrder.InOrder))
                node.Item = n++;

            Ensure(tree.Enumerate(TraversalOrder.InOrder).
                Where(nd => !nd.IsLeaf).
                All(nd => nd.Children.First().Item < nd.Item));

            Ensure(tree.Enumerate(TraversalOrder.InOrder).
                Where(nd => nd.Degree > 1).
                All(nd => nd.Children.Skip(1).All(child => child.Item > nd.Item)));
        }

        void PostOrderTest()
        {
            BasicTree<int> tree;

            do
                tree = TreeFactory.CreateBasicTree<int>();
            while (tree.IsEmpty);

            int n = 0;

            foreach (var node in tree.Enumerate(TraversalOrder.PostOrder))
                node.Item = n++;

            Ensure(tree.Root.Item == n - 1);
            Ensure(tree.Enumerate(TraversalOrder.PostOrder).SkipWhile(nd => nd != tree.Root).Count() == 1);
            Ensure(tree.Enumerate(TraversalOrder.PostOrder).TakeWhile(nd => nd != tree.Root).All(nd => nd.Item < nd.Parent.Item));
            Ensure(tree.Enumerate(TraversalOrder.PostOrder).Where(nd => nd.IsLeaf).SequenceEqual(tree.Leaves));
        }

        void PreOrderTest()
        {
            int ndx = 0;
            Func<BasicTree<int>.Node, int> provider = _ => ndx++;

            BasicTree<int> tree;

            do
                tree = TreeFactory.CreateBasicTree(provider);
            while (tree.IsEmpty);

            ndx = 0;
            Ensure(tree.Enumerate(TraversalOrder.PreOrder).All(nd => nd.Item == ndx++));
            Ensure(tree.Enumerate(TraversalOrder.PreOrder).First() == tree.Root);
            Ensure(tree.Nodes.SequenceEqual(tree.Enumerate(TraversalOrder.PreOrder)));
            Ensure(tree.SequenceEqual(tree.Enumerate(TraversalOrder.PreOrder).Select(nd => nd.Item)));
            Ensure(tree.Leaves.All(nd => nd.IsLeaf));
            Ensure(tree.Enumerate(TraversalOrder.PreOrder).Where(nd => nd.IsLeaf).SequenceEqual(tree.Leaves));
            Ensure(tree.Enumerate(TraversalOrder.PreOrder).SequenceEqual(tree.Nodes));
            Ensure(tree.Nodes.Count() == tree.GetCount());
            Ensure(tree.Nodes.All(nd => tree.ContainsNode(nd)));
        }

        void ConstructionTest()
        {
            BasicTree<int> tree = new BasicTree<int>();
            Ensure(tree.IsEmpty);
            Ensure(tree.Root == null);
            Ensure(tree.GetCount() == 0);

            var datum = SampleFactory.NextInt;
            tree = new BasicTree<int>(datum);
            Ensure(tree.Root.Item == datum);
            Ensure(!tree.IsEmpty);
            Ensure(tree.GetHeight() == 0);
            Ensure(tree.GetCount() == 1);

            datum = SampleFactory.NextInt;
            var node = new BasicTree<int>.Node(datum);
            tree = new BasicTree<int>(node);
            Ensure(tree.Root == node);
            Ensure(tree.Root.Item == datum);
            Ensure(tree.ContainsNode(node));
            Ensure(tree.GetHeight() == 0);
            Ensure(tree.GetCount() == 1);

            datum = SampleFactory.NextInt;
            var data = SampleFactory.CreateInts().Take(SampleFactory.CreateSBytes(min: 1).First()).ToArray();
            tree = new BasicTree<int>(datum, data);
            Ensure(!tree.IsEmpty);
            Ensure(tree.Root.Item == datum);
            Ensure(tree.Root.Children.Select(nd => nd.Item).SequenceEqual(data));
            Ensure(tree.GetCount() == data.Length + 1);
            Ensure(tree.Root.Children.All(nd => tree.ContainsNode(nd)));
            Ensure(tree.GetHeight() == 1);

            int n = 0;
            tree = TreeFactory.CreateBasicTree<int>(nd => ++n);
            Ensure(tree.GetCount() == n);
            Ensure(tree.IsEmpty || tree.GetCount() == tree.Root.GetDescendantCount());

            tree.Clear();
            Ensure(tree.IsEmpty);
        }


        void PathLengthTest()
        {
            BasicTree<int> tree;

            do
                tree = TreeFactory.CreateBasicTree<int>();
            while (tree.IsEmpty);

            foreach (var node in tree.Nodes)
                node.Item = tree.GetDepth(node);

            Ensure(tree.GetExternalPathLength() == ExternalPathLength(tree));

            Ensure(tree.GetInternalPathLength() == InternalPathLength(tree));

            int n = tree.Leaves.Count();
            var weights = SampleFactory.CreateInts().Take(n).ToArray();
            var dict = new Dictionary<BasicTree<int>.Node, int>();

            int ndx = 0;
            foreach (var node in tree.Leaves)
                dict[node] = weights[ndx++];

            Func<BasicTree<int>.Node, int> itemWeight = nd => dict[nd];

            Ensure(tree.GetWeightedExternalPathLength(itemWeight) == WeightedExternalPathLength(tree, dict));
        }

        int WeightedExternalPathLength(BasicTree<int> tree, Dictionary<BasicTree<int>.Node, int> dic)
        {
            var wl = 0;
            foreach (var node in tree.Nodes)
                if (node.IsLeaf)
                    wl += node.Item * dic[node];

            return wl;
        }

        int ExternalPathLength(BasicTree<int> tree)
        {
            int len = 0;
            foreach (var node in tree.Nodes)
                if (node.IsLeaf)
                    len += node.Item;

            return len;
        }

        int InternalPathLength(BasicTree<int> tree)
        {
            int len = 0;
            foreach (var node in tree.Nodes)
                if (!node.IsLeaf)
                    len += node.Item;

            return len;
        }
    }
}
