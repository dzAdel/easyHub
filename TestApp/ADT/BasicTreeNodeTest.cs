using easyLib.ADT.Trees;
using easyLib.Test;
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
            SingleNodePropertiesTest();
            MultiNodePropertiesTest();
            AddingNodeTest();
            RemovingNodeTest();
        }

        //private:
        void SingleNodePropertiesTest()
        {
            var node = TreeFactory.CreateTree<int>(maxLevel: 0).Root;

            Ensure(node.IsLeaf);
            Ensure(node.IsRoot);
            Ensure(node.Children.Count() == 0);
            Ensure(node.Degree == 0);
            Ensure(node.GetDepth() == 0);
            Ensure(node.GetDescendantCount() == 1);
            Ensure(node.GetPath().Single() == node);
            Ensure(node.Parent == null);
        }

        void MultiNodePropertiesTest()
        {
            var node = TreeFactory.CreateTree<int>(maxLevel: SampleFactory.CreateBytes(1, 9).First()).Root;

            Trace("MultiNodePropertiesTest()\nNode properties:",
                $"Degree: {node.Degree}",
                $"IsLeaf: {node.IsLeaf}",
                $"IsRoot: {node.IsRoot}",
                $"DescendantCount: {node.Item.DescendantCount}");


            EnumerateDescendant(node).All(nd => Ensure(nd.Item.Depth == nd.GetDepth()));
            EnumerateDescendant(node).All(nd => Ensure(nd.Degree == nd.Children.Count()));
            EnumerateDescendant(node).All(nd => Ensure(nd.GetDepth() == nd.Item.Depth));
            EnumerateDescendant(node).All(nd => nd.GetPath().Contains(node));
            EnumerateDescendant(node).All(nd => nd.GetPath().Contains(nd));
            EnumerateDescendant(node).All(nd => node.IsAncestor(nd));
            EnumerateDescendant(node).All(nd => nd.IsDescendant(node));
            EnumerateDescendant(node).All(nd => nd.GetDescendantCount() == nd.Item.DescendantCount);
        }

        void AddingNodeTest()
        {
            var root = TreeFactory.CreateTree<int>(maxLevel: SampleFactory.CreateBytes(limit: 9).First()).Root;

            var ndx = SampleFactory.CreateInts(0, root.Item.DescendantCount).First();
            var parent = EnumerateDescendant(root).ElementAt(ndx);

            Trace("AddingNodeTest()\nNodes properties:",
                        $"Root.DescendantCount: {root.Item.DescendantCount}",
                        $"Parent.Degree: {parent.Degree}",
                        $"Parent.IsRoot: {parent.IsRoot}",
                        $"Parent.DescendantCount: {parent.Item.DescendantCount}");


            var ni = new NodeInfo<int>(parent.Item.Depth + 1);

            var newNode0 = parent.AppendChild(ni);
            Ensure(newNode0.Parent == parent);
            Ensure(newNode0.GetDepth() == newNode0.Item.Depth);
            Ensure(root.IsDescendant(newNode0));
            Ensure(newNode0.IsAncestor(root));
            Ensure(parent.GetDescendantCount() == parent.Item.DescendantCount + 1);
            Ensure(parent.Children.Last() == newNode0);

            ni = new NodeInfo<int>(parent.Item.Depth + 1);
            int count = SampleFactory.CreateBytes(min: 1).First();
            var newNode1 = new BasicTree<INodeInfo<int>>.Node(ni, Enumerable.
                Repeat<NodeInfo<int>>(new NodeInfo<int>(ni.Depth + 1), count));

            parent.AppendChild(newNode1);
            Ensure(newNode1.Parent == parent);
            Ensure(newNode1.GetDepth() == newNode1.Item.Depth);
            Ensure(root.IsDescendant(newNode1));
            Ensure(newNode1.IsAncestor(root));
            Ensure(parent.GetDescendantCount() == parent.Item.DescendantCount + 1 + 1 + count);
            Ensure(parent.Children.Last() == newNode1);
            newNode1.Children.All(nd => Ensure(nd.Parent == newNode1));
            newNode1.Children.All(nd => Ensure(nd.GetDepth() == nd.Item.Depth));

            ni = new NodeInfo<int>(parent.Item.Depth + 1);
            var newNode2 = parent.PrependChild(ni);
            Ensure(newNode2.Parent == parent);
            Ensure(parent.Children.First() == newNode2);
            Ensure(parent.GetDescendantCount() == parent.Item.DescendantCount + 1 + 1 + count + 1);


            ni = new NodeInfo<int>(parent.Item.Depth + 1);
            var newNode3 = new BasicTree<INodeInfo<int>>.Node(ni, Enumerable.
                Repeat<NodeInfo<int>>(new NodeInfo<int>(ni.Depth + 1), count));
            parent.PrependChild(newNode3);
            Ensure(newNode3.Parent == parent);
            Ensure(newNode3.Degree == count);
            Ensure(parent.Children.First() == newNode3);
            Ensure(parent.GetDescendantCount() == parent.Item.DescendantCount + 1 + 1 + count + 1 + 1 + count);


            var sibling = parent.Children.ElementAt(SampleFactory.CreateInts(0, parent.Degree - 1).First());
            ni = new NodeInfo<int>(parent.Item.Depth + 1);
            var newNode4 = sibling.InsertSibling(ni);
            Ensure(newNode4.Parent == sibling.Parent);
            Ensure(parent.GetDescendantCount() == parent.Item.DescendantCount + 1 + 1 + count + 1 + 1 + count + 1);
            Ensure(parent.Children.SkipWhile(nd => nd != sibling).ElementAt(1) == newNode4);

            ni = new NodeInfo<int>(parent.Item.Depth + 1);
            var newNode5 = new BasicTree<INodeInfo<int>>.Node(ni, Enumerable.
                Repeat<NodeInfo<int>>(new NodeInfo<int>(ni.Depth + 1), count));
            sibling.InsertSibling(newNode5);
            Ensure(newNode5.Parent == sibling.Parent);
            Ensure(parent.GetDescendantCount() == parent.Item.DescendantCount + 1 + 1 + count + 1 + 1 + count + 1 + 1 + count);
            Ensure(parent.Children.SkipWhile(nd => nd != sibling).ElementAt(1) == newNode5);
        }

        void RemovingNodeTest()
        {
            var root = TreeFactory.CreateTree<int>(maxLevel: SampleFactory.CreateBytes(1, 9).First()).Root;

            while (root.Degree == 0)
                root = TreeFactory.CreateTree<int>(maxLevel: SampleFactory.CreateBytes(1, 9).First()).Root;

            var ndx = SampleFactory.CreateInts(0, root.Item.DescendantCount).First();
            var parent = EnumerateDescendant(root).ElementAt(ndx);

            while (parent.Degree == 0)
            {
                ndx = SampleFactory.CreateInts(0, root.Item.DescendantCount).First();
                parent = EnumerateDescendant(root).ElementAt(ndx);
            }

            Trace("RemovingNodeTest()\nNodes properties:",
                        $"Root.DescendantCount: {root.Item.DescendantCount}",
                        $"Parent.Degree: {parent.Degree}",
                        $"Parent.IsRoot: {parent.IsRoot}",
                        $"Parent.DescendantCount: {parent.Item.DescendantCount}");

            ndx = SampleFactory.CreateInts(0, parent.Degree).First();
            var node = parent.Children.ElementAt(ndx);

            parent.DetachChild(node);
            Ensure(node.Parent == null);
            Ensure(!parent.Children.Contains(node));
            Ensure(!root.IsDescendant(node));
            Ensure(!node.IsAncestor(root));
            Ensure(root.GetDescendantCount() == root.Item.DescendantCount - node.GetDescendantCount());
        }

        IEnumerable<BasicTree<T>.Node> EnumerateDescendant<T>(BasicTree<T>.Node node)
        {
            IEnumerable<BasicTree<T>.Node> result = Enumerable.Empty<BasicTree<T>.Node>();

            if (node != null)
            {
                result = result.Append(node);

                foreach (var child in node.Children)
                    result = result.Append(child).Concat(EnumerateDescendant(child));
            }

            return result;
        }
    }
}
