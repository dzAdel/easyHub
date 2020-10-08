using easyLib.ADT.Trees;
using easyLib.Test;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestApp.ADT
{

    class BasicTreeNodeTest : UnitTest
    {
        //Assume SampleFactoryTest ok
        public BasicTreeNodeTest() :
            base("BasicTree<>.Node Test")
        { }


        //protected:
        protected override void Start()
        {
            ConstructionTest();
            SettingParentTest();
            PathTest();
            InsertionTest();
            RemovalTest();
        }

        //private:
        void RemovalTest()
        {
            BasicTree<int>.Node node;

            do
                node = TreeFactory.CreateBasicNode<int>();
            while (node.IsLeaf);

            var n = node.GetDescendantCount();
            var nodes = Nodes(node);
            var ndx = SampleFactory.CreateInts(1, nodes.Count()).First();

            var child = nodes.ElementAt(ndx);
            child.Parent.DetachChild(child);

            Ensure(child.Parent == null);
            Ensure(!Nodes(node).Contains(child));
            Ensure(node.GetDescendantCount() + child.GetDescendantCount() == n);
            Ensure(!node.IsAncestorOf(child));
            Ensure(!child.IsDescendantOf(node));


            //-----------
            IEnumerable<BasicTree<int>.Node> Nodes(BasicTree<int>.Node node)
            {
                var res = Enumerable.Empty<BasicTree<int>.Node>().Append(node);

                foreach (var child in node.Children)
                    res = res.Concat(Nodes(child));

                return res;
            }
        }

        void InsertionTest()
        {
            var node = TreeFactory.CreateBasicNode<int>();
            var item = SampleFactory.NextInt;

            node.AppendChild(item);
            Ensure(node.Children.Last().Item == item);

            item = SampleFactory.NextInt;
            node.PrependChild(item);
            Ensure(node.Children.First().Item == item);

            item = SampleFactory.NextInt;
            var ndx = SampleFactory.CreateInts(0, node.Children.Count()).First();
            node.Children.ElementAt(ndx).InsertSibling(item);
            Ensure(node.Children.ElementAt(ndx + 1).Item == item);

            var child = new BasicTree<int>.Node(0);
            node.AppendChild(child);
            Ensure(node.Children.Last() == child);

            child = new BasicTree<int>.Node(0);
            node.PrependChild(child);
            Ensure(node.Children.First() == child);

            child = new BasicTree<int>.Node(0);
            ndx = SampleFactory.CreateInts(0, node.Children.Count()).First();
            node.Children.ElementAt(ndx).InsertSibling(child);
            Ensure(node.Children.ElementAt(ndx + 1) == child);
        }

        void PathTest()
        {
            var node = TreeFactory.CreateBasicNode<int>();

            Ensure(node.Children.All(nd => nd.GetPath().First() == node));
            Ensure(node.Children.All(nd => nd.GetPath().Last() == nd));
            Ensure(node.Children.All(nd => nd.GetPath().Reverse().ElementAt(1) == nd.Parent));
        }

        void SettingParentTest()
        {
            var node = TreeFactory.CreateBasicNode<int>();
            var child = TreeFactory.CreateBasicNode<int>();

            var nodeDescCount = node.GetDescendantCount();
            var childDescCount = child.GetDescendantCount();

            child.Parent = node;
            Ensure(child.Parent == node);
            Ensure(node.Children.Last() == child);
            Ensure(node.GetDescendantCount() == nodeDescCount + childDescCount);

            //...

            child.Parent = null;
            Ensure(child.Parent == null);
            Ensure(!node.Children.Contains(child));
            Ensure(node.GetDescendantCount() == nodeDescCount);
            Ensure(child.GetDescendantCount() == childDescCount);
        }

        void ConstructionTest()
        {

            var n = SampleFactory.NextByte;
            var node = new BasicTree<int>.Node(n);

            Ensure(node.Item == n);
            Ensure(node.IsLeaf);
            Ensure(node.Degree == 0);
            Ensure(node.Parent == null);
            Ensure(node.Children.Count() == 0);
            Ensure(node.GetDescendantCount() == 1);
            Ensure(node.GetPath().Single() == node);
            Ensure(node.IsDescendantOf(node));
            Ensure(node.IsAncestorOf(node));

            //....

            var children = SampleFactory.CreateInts().Take(SampleFactory.NextByte).ToArray();
            node = new BasicTree<int>.Node(n, children);

            Ensure(node.Item == n);
            Ensure(children.Length == 0 || !node.IsLeaf);
            Ensure(node.Degree == children.Length);
            Ensure(node.Parent == null);
            Ensure(node.Children.Count() == children.Length);
            Ensure(node.GetDescendantCount() == 1 + children.Length);
            Ensure(node.GetPath().First() == node);
            Ensure(node.Children.All(nd => nd.GetPath(node).SequenceEqual(new BasicTree<int>.Node[] { node, nd })));
            Ensure(node.Children.Select(nd => nd.Item).ToArray().SequenceEqual(children));
            Ensure(node.Children.All(nd => node.IsAncestorOf(nd)));
            Ensure(node.Children.All(nd => nd.IsDescendantOf(node)));
        }
    }
}
