using easyLib.ADT.Trees;
using easyLib.Test;
using System.Linq;


namespace TestApp.ADT
{
    class BasicTreeTest : UnitTest
    {
        public BasicTreeTest():
            base("Tree<T> Test")
        { }


        //protected:
        protected override void Start()
        {
            var tree = new BasicTree<int>();
            Ensure(tree.IsEmpty);
            Ensure(tree.Root == null);

            PreOrderTest();
            PostOrderTest();
            LevelOrderTest();
            InOrderTest();
            TestHeight();
            InternalPathLengthTest();
            ExternalPathLengthTest();
        }

        //private:

        void InternalPathLengthTest()
        {
            var root = new BasicTree<int>.Node(0);
            root.AppendChild(new BasicTree<int>.Node(1));

            var node = new BasicTree<int>.Node(1, Enumerable.Repeat(2, 2));
            root.AppendChild(node);

            var node2 = new BasicTree<int>.Node(2, Enumerable.Repeat(3, 1));
            node = new BasicTree<int>.Node(1);
            node.AppendChild(node2);
            root.AppendChild(node);

            var tree = new BasicTree<int>(root);

            Ensure(tree.InternalPathLength() == 4);
        }

        void ExternalPathLengthTest()
        {
            var root = new BasicTree<int>.Node(0);
            root.AppendChild(new BasicTree<int>.Node(1));

            var node = new BasicTree<int>.Node(1, Enumerable.Repeat(2, 2));
            root.AppendChild(node);

            var node2 = new BasicTree<int>.Node(2, Enumerable.Repeat(3, 1));
            node = new BasicTree<int>.Node(1);
            node.AppendChild(node2);
            root.AppendChild(node);

            var tree = new BasicTree<int>(root);


            Ensure(tree.ExternalPathLength() == 8);
        }

        void PreOrderTest()
        {
            var root = new BasicTree<int>.Node(0);
            var node = new BasicTree<int>.Node(1, new int[] { 2, 3, 4 });
            root.AppendChild(node);

            node = new BasicTree<int>.Node(5, new int[] { 6, 7 });
            root.AppendChild(node);

            node = new BasicTree<int>.Node(8, new int[] { 9 });
            root.AppendChild(node);

            var tree = new BasicTree<int>(root);

            var nodes = tree.Enumerate(TraversalOrder.PreOrder).ToArray();

            for (int ndx = 0; ndx < nodes.Length; ++ndx)
                Ensure(ndx == nodes[ndx].Item);
        }

        void PostOrderTest()
        {
            var root = new BasicTree<int>.Node(9);
            var node = new BasicTree<int>.Node(3, new int[] { 0, 1, 2 });
            root.AppendChild(node);

            node = new BasicTree<int>.Node(6, new int[] { 4, 5 });
            root.AppendChild(node);

            node = new BasicTree<int>.Node(8, new int[] { 7 });
            root.AppendChild(node);

            var tree = new BasicTree<int>(root);

            var nodes = tree.Enumerate(TraversalOrder.PostOrder).ToArray();

            for (int ndx = 0; ndx < nodes.Length; ++ndx)
                Ensure(ndx == nodes[ndx].Item);
        }

        void LevelOrderTest()
        {
            var root = new BasicTree<int>.Node(0);
            var node = new BasicTree<int>.Node(1, new int[] { 4, 5, 6 });
            root.AppendChild(node);

            node = new BasicTree<int>.Node(2, new int[] { 7, 8 });
            root.AppendChild(node);

            node = new BasicTree<int>.Node(3, new int[] { 9 });
            root.AppendChild(node);

            var tree = new BasicTree<int>(root);

            var nodes = tree.Enumerate(TraversalOrder.BreadthFirst).ToArray();

            for (int ndx = 0; ndx < nodes.Length; ++ndx)
                Ensure(ndx == nodes[ndx].Item);
        }

        void InOrderTest()
        {
            var root = new BasicTree<int>.Node(4);
            var node = new BasicTree<int>.Node(1, new int[] { 0, 2, 3 });
            root.AppendChild(node);

            node = new BasicTree<int>.Node(6, new int[] { 5, 7 });
            root.AppendChild(node);

            node = new BasicTree<int>.Node(9, new int[] { 8 });
            root.AppendChild(node);

            var tree = new BasicTree<int>(root);

            var nodes = tree.Enumerate(TraversalOrder.InOrder).ToArray();

            for (int ndx = 0; ndx < nodes.Length; ++ndx)
                Ensure(ndx == nodes[ndx].Item);
        }

        void TestHeight()
        {
            var tree = CreateTree(8);
            Ensure(tree.GetHeight() == 7);
        }

        BasicTree<int> CreateTree(int levels)
        {
            var root = new BasicTree<int>.Node(1);

            if (levels > 1)
                CreateLevel(root, 2);

            return new BasicTree<int>(root);


            void CreateLevel(BasicTree<int>.Node parent, int lvlNber)
            {
                for (int i = 0; i < lvlNber; ++i)
                {
                    var node = new BasicTree<int>.Node(lvlNber);
                    parent.AppendChild(node);
                }

                if (lvlNber < levels)
                    foreach (BasicTree<int>.Node child in parent.Children)
                        CreateLevel(child, lvlNber + 1);
            }
        }


    }
}
