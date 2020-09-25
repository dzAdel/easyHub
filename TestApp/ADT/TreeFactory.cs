﻿using easyLib.ADT.Trees;
using easyLib.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using static easyLib.DebugHelper;


namespace TestApp.ADT
{
    static class TreeFactory
    {
        public static BasicTree<INodeInfo<T>> CreateTree<T>(byte maxLevel, 
            Func<INode<INodeInfo<T>>, T> dataProvider = null,            
            byte minChildCount = 0,
            byte maxChildCount = 4)
        {
            if (dataProvider == null)
                dataProvider = _ => default(T);

            var ni = new NodeInfo<T>(0);
            var root = new BasicTree<INodeInfo<T>>.Node(ni);

            

            long nbNode = 1;

            if (maxLevel > 0)
            {            
                int nChild = SampleFactory.CreateInts(minChildCount,maxChildCount + 1).First();

                for (int i = 0; i < nChild; ++i)
                {
                    var nd = CreateChild(root, maxLevel - 1);
                    ni.DescendantCount += nd.Item.DescendantCount;            
                }
            }


            Assert(root.GetDescendantCount() == ni.DescendantCount);

            var tree = new BasicTree<INodeInfo<T>>(root);

            foreach (var node in PostOrderChildern(tree.Root))
                node.Item.Data = dataProvider(node);

            return tree;

            //-------------
            BasicTree<INodeInfo<T>>.Node CreateChild(BasicTree<INodeInfo<T>>.Node parent, int lvl)
            {
                var ni = new NodeInfo<T>(parent.Item.Depth + 1);
                ++nbNode;
                var node = new BasicTree<INodeInfo<T>>.Node(ni);
                parent.AppendChild(node);


                if (lvl > 0)
                {
                    int nChild = SampleFactory.CreateInts(minChildCount, maxChildCount + 1).First();
                    for (int i = 0; i < nChild; ++i)
                        ni.DescendantCount += CreateChild(node, lvl - 1).Item.DescendantCount;                       
                }

                

                return node;
            }
        }

        public static BasicTree<INodeInfo<T>> CreateTree<T>(Func<INode<INodeInfo<T>>, T> dataProvider = null)
        {
            const int maxLevel = 16;
            int lvl = SampleFactory.CreateInts(-1, maxLevel + 1).First();

            if (lvl < 0)
                return new BasicTree<INodeInfo<T>>();

            return CreateTree<T>(maxLevel: (byte)lvl, dataProvider);
        }

        public static BinaryTree<NodeInfo<T>> CreateBinaryTree<T>(Func<BinaryTree<NodeInfo<T>>.Node, T> dataProvider = null)
        {
            int lvl = SampleFactory.CreateInts(-1, 17).First();

            if (lvl < 0)
                return new BinaryTree<NodeInfo<T>>();

            return CreateBinaryTree<T>((byte)lvl, dataProvider);
        }

        public static BinaryTree<NodeInfo<T>> CreateBinaryTree<T>(byte maxLevel, 
            Func<BinaryTree<NodeInfo<T>>.Node, T> dataProvider = null)
        {
            if (dataProvider == null)
                dataProvider = _ => default(T);

            var root = new BinaryTree<NodeInfo<T>>.Node(new NodeInfo<T>(0));

            IEnumerable<byte> byteSeq = SampleFactory.CreateBytes(0, 3);

            if (maxLevel > 0)
            {
                byte childCount = byteSeq.First();

                for (byte i = 0; i < childCount; ++i)
                    CreateChildren(root, 1);

                foreach (var node in root.Children)
                    root.Item.DescendantCount += node.Item.DescendantCount;

            }

            var tree =  new BinaryTree<NodeInfo<T>>(root);
            foreach (var node in Trees.Enumerate(tree, TraversalOrder.PostOrder))
                node.Item.Data = dataProvider(root);

            return tree;

            //--------------

            void CreateChildren(BinaryTree<NodeInfo<T>>.Node parent, int lvl)
            {
                if (lvl == maxLevel)
                    return;

                var nodeInfo = new NodeInfo<T>(parent.Item.Depth + 1);
                var node = new BinaryTree<NodeInfo<T>>.Node(nodeInfo);

                if (parent.IsLeaf)
                    if (byteSeq.First() == 0)
                        parent.RightChild = node;
                    else
                        parent.LeftChild = node;
                else if (parent.LeftChild == null)
                    parent.LeftChild = node;
                else
                    parent.RightChild = node;

                byte nbChild = byteSeq.First();
                for (int i = 0; i < nbChild; ++i)
                    CreateChildren(node, lvl + 1);

                foreach (var child in node.Children)
                    nodeInfo.DescendantCount += child.Item.DescendantCount;
            }
        }

        public static BinaryTree<NodeInfo<T>> CreateProperBinaryTree<T>(byte maxLevel = 16, Func<BinaryTree<NodeInfo<T>>.Node, T> dataProvider = null)
        {
            var tree = CreateBinaryTree<T>(maxLevel, dataProvider);
            var improperNodes = new Queue<BinaryTree<NodeInfo<T>>.Node>();

            foreach (var node in tree.Nodes)
                if (node.Degree == 1)
                    improperNodes.Enqueue(node);

            while (improperNodes.Count > 0)
            {
                var node = improperNodes.Dequeue();
                var nodeInfo = new NodeInfo<T>(node.Item.Depth + 1);

                ++node.Item.DescendantCount;

                var child = new BinaryTree<NodeInfo<T>>.Node(nodeInfo);
                if (node.LeftChild == null)
                    node.LeftChild = child;
                else
                    node.RightChild = child;
            }

            return tree;
        }
           

        //private:
        static IEnumerable<INode<INodeInfo<T>>> PostOrderChildern<T>(BasicTree<INodeInfo<T>>.Node root)
        {
            var result = Enumerable.Empty<INode<INodeInfo<T>>>();

            if (root.Degree > 0)
                foreach (var child in root.Children)
                    result = result.Concat(PostOrderChildern(child));

            return result.Append(root);
        }
    }
}