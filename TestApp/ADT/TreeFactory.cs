using easyLib.ADT.Trees;
using easyLib.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using static easyLib.DebugHelper;


namespace TestApp.ADT
{
    static class TreeFactory
    {
        public static BasicTree<T>.Node CreateBasicNode<T>(int maxChildCount, int maxLevel, Func<BasicTree<T>.Node, T> itemProvider = null)
        {
            var node = new BasicTree<T>.Node(default);

            if (maxLevel == 0 || maxChildCount == 0)
            {
                if (itemProvider != null)
                    node.Item = itemProvider(node);

                return node;
            }


            var childCountSeq = SampleFactory.CreateInts(0, maxChildCount + 1);
            var maxLvl = SampleFactory.CreateInts(1, maxLevel + 1).First();

            CreateChild(node, 1);

            if (itemProvider != null)
            {
                node.Item = itemProvider(node);

                foreach (BasicTree<T>.Node child in EnumerateDescendant(node))
                    child.Item = itemProvider(child);
            }

            return node;

            //-----------
            void CreateChild(BasicTree<T>.Node parent, int lvl)
            {
                var child = new BasicTree<T>.Node(default);
                parent.AppendChild(child);

                if (++lvl < maxLvl)
                {
                    var childCount = childCountSeq.First();

                    for (int i = 0; i < childCount; ++i)
                        CreateChild(child, lvl);
                }
            }
        }

        public static BasicTree<T>.Node CreateBasicNode<T>(Func<BasicTree<T>.Node, T> itemProvider = null)
        {
            int maxChild = SampleFactory.CreateInts(0, 4).First();
            int maxLevel = SampleFactory.CreateInts(0, 16).First();

            return CreateBasicNode(maxChild, maxLevel, itemProvider);
        }

        public static BinaryTree<T>.Node CreateBinaryNode<T>(int maxLevel,
            Func<BinaryTree<T>.Node, T> itemProvider = null)
        {

            var node = new BinaryTree<T>.Node(default);

            if (maxLevel != 0)
            {
                Func<bool> hasLeft = () => SampleFactory.NextInt % 2 != 0;
                Func<bool> hasRight = hasLeft;
                var maxLvl = SampleFactory.CreateInts(1, maxLevel + 1).First();

                CreateChild(node, 1);

                //----
                void CreateChild(BinaryTree<T>.Node parent, int lvl)
                {
                    if (hasLeft())
                        parent.LeftChild = new BinaryTree<T>.Node(default);

                    if (hasRight())
                        parent.RightChild = new BinaryTree<T>.Node(default);

                    if (++lvl < maxLvl)
                    {
                        if (parent.LeftChild != null)
                            CreateChild(parent.LeftChild, lvl);

                        if (parent.RightChild != null)
                            CreateChild(parent.RightChild, lvl);
                    }
                }
            }


            if (itemProvider != null)
                foreach (BinaryTree<T>.Node nd in EnumerateDescendant(node))
                    nd.Item = itemProvider(nd);

            return node;
        }

        public static BinaryTree<T>.Node CreateBinaryNode<T>(Func<BinaryTree<T>.Node, T> itemProvider = null)
        {
            int maxLevel = SampleFactory.CreateInts(0, 16).First();

            return CreateBinaryNode<T>(maxLevel, itemProvider);
        }

        public static BasicTree<T> CreateBasicTree<T>(byte maxChildCount,
            byte maxLevel,
            Func<BasicTree<T>.Node, T> itemProvider = null)
        {
            var root = CreateBasicNode<T>(maxChildCount, maxLevel);

            if (itemProvider != null)
            {
                foreach (var node in EnumerateDescendant(root).Cast<BasicTree<T>.Node>())
                    node.Item = itemProvider(node);
            }

            return new BasicTree<T>(root);
        }

        public static BasicTree<T> CreateBasicTree<T>(Func<BasicTree<T>.Node, T> itemProvider = null)
        {
            byte lvl = SampleFactory.CreateBytes(0, 13).First();

            if (lvl == 0)
                return new BasicTree<T>();

            return CreateBasicTree<T>(4, (byte)(lvl - 1), itemProvider);
        }

        public static BinaryTree<T> CreateBinaryTree<T>(Func<BinaryTree<T>.Node, T> itemProvider = null)
        {
            byte lvl = SampleFactory.CreateBytes(0, 13).First();

            if (lvl == 0)
                return new BinaryTree<T>();

            return CreateBinaryTree<T>((byte)(lvl - 1), itemProvider);
        }

        public static BinaryTree<T> CreateBinaryTree<T>(byte maxLevel, Func<BinaryTree<T>.Node, T> itemProvider = null)
        {
            var root = CreateBinaryNode<T>(maxLevel);

            if (itemProvider != null)
            {
                foreach (var node in EnumerateDescendant(root).Cast<BinaryTree<T>.Node>())
                    node.Item = itemProvider(node);
            }

            return new BinaryTree<T>(root);
        }

        public static BinaryTree<T> CreateProperBinaryTree<T>(Func<BinaryTree<T>.Node, T> itemProvider = null)
        {
            byte lvl = SampleFactory.CreateBytes(0, 13).First();

            if (lvl == 0)
                return new BinaryTree<T>();

            return CreateProperBinaryTree<T>((byte)(lvl - 1), itemProvider);
        }

        public static BinaryTree<T> CreateProperBinaryTree<T>(byte maxLevel, Func<BinaryTree<T>.Node, T> itemProvider = null)
        {
            var tree = CreateBinaryTree<T>(maxLevel);
            var improperNodes = new Queue<BinaryTree<T>.Node>();

            foreach (var node in tree.Nodes)
                if (node.Degree == 1)
                    improperNodes.Enqueue(node);

            while (improperNodes.Count > 0)
            {
                var node = improperNodes.Dequeue();

                var child = new BinaryTree<T>.Node(default);

                if (node.LeftChild == null)
                    node.LeftChild = child;
                else
                    node.RightChild = child;
            }

            Assert(tree.Nodes.All(nd => nd.Degree != 1));

            if (itemProvider != null)
                foreach (var nd in tree.Nodes)
                    nd.Item = itemProvider(nd);

            return tree;
        }

        public static BinaryTree<T> CreateCompleteBinaryTree<T>(Func<BinaryTree<T>.Node, T> itemProvider = null)
        {
            int lvl = SampleFactory.CreateInts(-1, 13).First();

            if (lvl == 0)
                return new BinaryTree<T>(default(T));

            BinaryTree<T> tree;

            if (lvl == -1)
            {
                tree = new BinaryTree<T>(new BinaryTree<T>.Node(default));
                tree.Root.LeftChild = new BinaryTree<T>.Node(default);
            }
            else
                tree = CreateCompleteBinaryTree<T>((byte)(lvl));

            if (itemProvider != null)
                foreach (var nd in tree.Nodes)
                    nd.Item = itemProvider(nd);

            return tree;
        }

        public static BinaryTree<T> CreateCompleteBinaryTree<T>(byte maxLevel, Func<BinaryTree<T>.Node, T> itemProvider = null)
        {
            var tree = CreateProperBinaryTree<T>(maxLevel);

            int h = tree.GetHeight();

            if (h < 2)
                return tree;


            var badNodes = new Queue<(BinaryTree<T>.Node, int)>();

            foreach (var (node, lvl) in tree.LevelOrderTraversal())
                if (lvl != h && node.Degree != 2)
                    badNodes.Enqueue(((BinaryTree<T>.Node)node, lvl));

            var src = from node in tree.Nodes
                      where node.Degree != 2
                      where tree.GetDepth(node) != h
                      select node;

            var q = from p in badNodes
                    select p.Item1;

            Assert(src.All(nd => q.Contains(nd)));

            while (badNodes.Count > 0)
            {
                var (node, lvl) = badNodes.Dequeue();

                Assert(node.RightChild == null || node.LeftChild == null);


                if (node.LeftChild == null)
                {
                    node.LeftChild = new BinaryTree<T>.Node(default);

                    if (lvl + 1 < h)
                        badNodes.Enqueue((node.LeftChild, lvl + 1));
                }

                if (node.RightChild == null)
                {
                    node.RightChild = new BinaryTree<T>.Node(default);

                    if (lvl + 1 < h)
                        badNodes.Enqueue((node.RightChild, lvl + 1));
                }

                Assert(node.Degree == 2);
            }

            Assert(tree.Nodes.All(nd => nd.IsLeaf || nd.Degree == 2));
            Assert(tree.Nodes.All(nd => !nd.IsLeaf || tree.GetDepth(nd) == h));

            var leaves = tree.Leaves.ToList();


            leaves.Reverse();

            var n = SampleFactory.CreateInts(0, leaves.Count).First();

            for (int i = 0; i < n; ++i)
            {
                var parent = leaves[i].Parent;

                parent.RightChild = null;

                if (++i < n)
                    parent.LeftChild = null;
            }


            if (itemProvider != null)
                foreach (var nd in tree.Nodes)
                    nd.Item = itemProvider(nd);

            return tree;
        }


        //private:
        static IEnumerable<ITreeNode<T>> EnumerateDescendant<T>(ITreeNode<T> node)
        {
            var res = Enumerable.Repeat(node, 1);

            foreach (var child in node.Children)
                res = res.Concat(EnumerateDescendant(child));

            return res;
        }

        public static void Display<T, N>(this ITree<T, N> tree)
            where N : ITreeNode<T>
        {
            System.Diagnostics.Debug.WriteLine("------------------");
            DisplaySubTree(tree.Root, 0);
            System.Diagnostics.Debug.WriteLine("------------------");

            //------------
            void DisplaySubTree(ITreeNode<T> node, int ind)
            {
                string indent = new string(' ', ind);

                System.Diagnostics.Debug.WriteLine($"{indent}{node.Item}");
                foreach (var child in node.Children)
                    DisplaySubTree(child, ind + 4);
            }
        }

        public static void Display<T>(this BinaryTree<T> tree)
        {
            System.Diagnostics.Debug.WriteLine("------------------");
            DisplaySubTree(tree.Root, 0);
            System.Diagnostics.Debug.WriteLine("------------------");

            //------------
            void DisplaySubTree(BinaryTree<T>.Node node, int ind)
            {
                string indent = new string(' ', ind);
                System.Diagnostics.Debug.WriteLine($"{indent}{node.Item}");

                if (!node.IsLeaf)
                {
                    if (node.LeftChild == null)
                        System.Diagnostics.Debug.WriteLine($"{indent}    x");
                    else
                        DisplaySubTree(node.LeftChild, ind + 4);

                    if (node.RightChild == null)
                        System.Diagnostics.Debug.WriteLine($"{indent}    x");
                    else
                        DisplaySubTree(node.RightChild, ind + 4);
                }
            }
        }

        public static void Display<T>(this IBinaryTreeNode<T> root, Func<T, string> strSelector)
        {
            System.Diagnostics.Debug.WriteLine("------------------");
            DisplaySubTree(root, 0);
            System.Diagnostics.Debug.WriteLine("------------------");

            //------------
            void DisplaySubTree(IBinaryTreeNode<T> node, int ind)
            {
                string indent = new string(' ', ind);
                System.Diagnostics.Debug.WriteLine($"{indent}{strSelector(node.Item)}");

                if (!node.IsLeaf)
                {
                    if (node.LeftChild == null)
                        System.Diagnostics.Debug.WriteLine($"{indent}    x");
                    else
                        DisplaySubTree(node.LeftChild, ind + 4);

                    if (node.RightChild == null)
                        System.Diagnostics.Debug.WriteLine($"{indent}    x");
                    else
                        DisplaySubTree(node.RightChild, ind + 4);
                }
            }
        }
    }
}

