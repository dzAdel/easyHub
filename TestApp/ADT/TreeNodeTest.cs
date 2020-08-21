using easyLib.ADT.Trees;
using easyLib.Test;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestApp.ADT
{

    class TreeNodeTest : UnitTest
    {
        public TreeNodeTest() :
            base("Tree<T>.Node Test")
        { }


        //protected:
        protected override void Start()
        {
            SingleNodeTest();
            TestMultiNode();
        }

        //private:

        void TestMultiNode()
        {
            var root = CreateNodes(2);

            Ensure(root.Degree == 2);
            Ensure(root.IsAncestor(root));            
            Ensure(root.GetDescendantCount() == 3);

            foreach (var node in root.Children)
            {
                Ensure(node.GetPath().First() == root);
                Ensure(node.GetPath().Last() == node);
                Ensure(node.IsLeaf);
                Ensure(node.GetDepth() == 1);
                Ensure(node.Parent == root);
                Ensure(node.IsAncestor(root));
            }

            var nd = CreateNodes(1);
            root.Children.First().PrependChild(nd);
            Ensure(root.Children.First().Children.First() == nd);
            Ensure(root.Children.First() == nd.Parent);
            Ensure(root.GetDescendantCount() == 4);
            Ensure(nd.IsAncestor(root));

            var path = nd.GetPath().ToArray();
            Ensure(path.Length == 3);
            Ensure(path[0] == root);
            Ensure(path[1] == root.Children.First());
            Ensure(path[2] == nd);

            nd = root.PrependChild(1);
            Ensure(root.GetDescendantCount() == 5);
            Ensure(root.Children.First() == nd);
            Ensure(nd.Parent == root);
            Ensure(nd.IsAncestor(root));

            nd = CreateNodes(1);
            root.AppendChild(nd);
            Ensure(root.GetDescendantCount() == 6);
            Ensure(root.Children.Last() == nd);
            Ensure(nd.Parent == root);
            Ensure(nd.IsAncestor(root));

            nd = root.Children.First().InsertSibling(1);
            Ensure(nd == root.Children.Skip(1).First());
            Ensure(nd.Parent == root);
            Ensure(nd.IsAncestor(root));

            nd = root.Children.First();
            root.Children.Last().InsertSibling(nd);
            Ensure(nd.Parent == root);
            Ensure(root.Children.First() != nd);

            nd = root.Children.First();
            root.DetachChild(nd);
            Ensure(!nd.IsAncestor(root));
            Ensure(nd.IsRoot);
            Ensure(!root.Children.Contains(nd));
        }

        void SingleNodeTest()
        {
            var node = new Tree<int>.Node(0);

            Ensure(node.IsLeaf);
            Ensure(node.IsRoot);
            Ensure(node.Children.Count() == 0);
            Ensure(node.Degree == 0);
            Ensure(node.GetDepth() == 0);
            Ensure(node.GetDescendantCount() == 1);
            Ensure(node.GetPath().Single() == node);
            Ensure(node.Item == 0);
            Ensure(node.Parent == null);
        }


        uint CountDescendant(Tree<int>.Node node)
        {
            if (node.IsLeaf)
                return 1;

            uint n = 1;
            foreach (Tree<int>.Node child in node.Children)
                n += CountDescendant(child);

            return n;
        }        

        Tree<int>.Node CreateNodes(int levels)
        {
            var root = new Tree<int>.Node(1);

            if (levels > 1)
                CreateLevel(root, 2);

            System.Diagnostics.Debug.WriteLine($"{CountDescendant(root)} nodes created.");

            return root;


            void CreateLevel(Tree<int>.Node parent, int lvlNber)
            {   
                for (int i = 0; i < lvlNber; ++i)
                {
                    var node = new Tree<int>.Node(lvlNber);
                    parent.AppendChild(node);
                }

                if (lvlNber < levels)
                    foreach (Tree<int>.Node child in parent.Children)
                        CreateLevel(child, lvlNber + 1);
            }
        }
    }
}
