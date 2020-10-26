using easyLib.ADT.Trees;
using easyLib.Test;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestApp.ADT
{
    public class BinarySearchTreeTest : UnitTest
    {
        public BinarySearchTreeTest() :
            base("BinarySearchTree<,> Test")
        { }


        protected override void Start()
        {
            ConstructionTest();
            FloorTest();
            CeilingTest();
            FirstCommonAncestorTest();
            PathTest();
            MinMaxTest();
            GetRangeTest();
            ReplaceItemTest();
            GetSuccessorTest();
            GetPredecessorTest();
            RemoveTest();
        }

        //private: 
        void RemoveTest()
        {
            var bst = CreateTree(SampleFactory.NextByte + 1);
            int count = bst.Count;
            int ndx = SampleFactory.CreateInts(0, count).First();
            var node = bst.Nodes.ElementAt(ndx);
            int len = node.Item.Length;
            bst.Remove(len);
            
            Ensure(!bst.ContainsKey(len));
            Ensure(bst.Count == count - 1);
            Ensure(IsSorted(bst.Keys.ToArray()));

            while (--count > 0)
            {
                ndx = SampleFactory.CreateInts(0, count).First();
                bst.Remove(bst.Keys.ElementAt(ndx));

                Ensure(IsSorted(bst.Keys.ToArray()));
                Ensure(bst.Count == count - 1);
            }


            bst = CreateTree();
            bst.Clear();
            Ensure(bst.IsEmpty);
            Ensure(bst.Root == null);
        }

        void GetPredecessorTest()
        {
            var bst = CreateTree(SampleFactory.NextByte + 1);
            int ndx = SampleFactory.CreateInts(0, bst.Count).First();

            var node = bst.Nodes.ElementAt(ndx);
            var predcessor = bst.GetPredecessor(node);

            Ensure(predcessor != null || node == bst.Min());
            Ensure(predcessor == null || node.Item.Length >= predcessor.Item.Length);
            Ensure(predcessor == null || bst.GetRange(predcessor.Item.Length, node.Item.Length).
                SequenceEqual(new IBinaryTreeNode<string>[] { predcessor, node })); 

            Ensure(bst.Nodes.Reverse().SequenceEqual(GetNodes()));

            //________________

            IEnumerable<IBinaryTreeNode<string>> GetNodes()
            {
                var node = bst.Max();

                while (node != null)
                {
                    yield return node;

                    node = bst.GetPredecessor(node);
                }

            }
        }

        void GetSuccessorTest()
        {
            var bst = CreateTree(SampleFactory.NextByte + 1);
            int ndx = SampleFactory.CreateInts(0, bst.Count).First();

            var node = bst.Nodes.ElementAt(ndx);
            var successor = bst.GetSuccessor(node);

            Ensure(successor != null || node == bst.Max());            
            Ensure(successor == null || node.Item.Length <= successor.Item.Length);
            Ensure(successor == null || bst.GetRange(node.Item.Length, successor.Item.Length).
                SequenceEqual(new IBinaryTreeNode<string>[] { node, successor }));

            Ensure(bst.Nodes.SequenceEqual(GetNodes()));

            //________________

            IEnumerable<IBinaryTreeNode<string>> GetNodes()
            {
                var node = bst.Min();

                while(node != null)
                {
                    yield return node;

                    node = bst.GetSuccessor(node);
                }

            }
        }

        void ReplaceItemTest()
        {
            var bst = CreateTree(SampleFactory.NextByte + 1);
            var ndx = SampleFactory.CreateInts(0, bst.Count).First();

            var node = bst.Nodes.ElementAt(ndx);
            var str = new string(node.Item.Reverse().ToArray());

            bst.ReplaceItem(node, str);

            Ensure(node.Item == str);
            Ensure(bst.Nodes.ElementAt(ndx) == node);
        }

        void GetRangeTest()
        {
            var bst = CreateTree(SampleFactory.NextByte + 2);
            int minLen = bst.Min().Item.Length;
            int maxLen = bst.Max().Item.Length;
            var seq = SampleFactory.CreateInts(minLen, maxLen + 1).Distinct().Take(2).ToArray();
            var len0 = seq.First();
            var len1 = seq.Last();

            if (len1 < len0)
                (len0, len1) = (len1, len0);

            var range = bst.GetRange(len0, len1).OrderBy(nd => nd.Item.Length);

            Ensure(!range.Any() || len0 <= range.First().Item.Length);
            Ensure(!range.Any() || range.Last().Item.Length <= len1);

            var outRange = bst.Nodes.Except(range);
            Ensure(outRange.All(nd => nd.Item.Length < len0 || nd.Item.Length > len1));
        }

        void MinMaxTest()
        {

            var bst = CreateTree(0);
            Ensure(bst.Min() == null);
            Ensure(bst.Max() == null);

            bst = CreateTree(SampleFactory.NextByte + 2);
            var min = bst.Min();
            var max = bst.Max();

            Ensure(bst.Keys.Min() == min.Item.Length);
            Ensure(bst.Keys.Max() == max.Item.Length);
        }

        void PathTest()
        {
            var bst = CreateTree();
            
            Ensure(bst.Nodes.All(nd => bst.GetPath(nd).First() == bst.Root));
            Ensure(bst.Nodes.All(nd => bst.GetPath(nd).Last() == nd));
            
            if (!bst.IsEmpty)
            {
                int h = bst.GetHeight();

                Ensure(bst.Nodes.All(nd => bst.GetPath(nd).Count() - 1 <= h));            
            }
        }

        void FirstCommonAncestorTest()
        {
            var bst = CreateTree(SampleFactory.NextByte + 3);
            var ints = SampleFactory.CreateInts(0, bst.Count);
            var node0 = bst.Nodes.ElementAt(ints.First());
            var node1 = bst.Nodes.ElementAt(ints.First());

            var fca = bst.FirstCommonAncestor(node0, node1);

            Ensure(fca.IsAncestorOf(node0));
            Ensure(fca.IsAncestorOf(node1));
            Ensure(node0.GetPath(fca).Intersect(node1.GetPath(fca)).SequenceEqual(Enumerable.Repeat(fca, 1)));
        }

        void CeilingTest()
        {
            var bst = CreateTree();
            var len = SampleFactory.NextByte;
            var node = bst.Ceiling(len);

            Ensure(!bst.IsEmpty || node == null);

            if (!bst.IsEmpty)
            {
                Ensure(node != null || bst.Keys.Last() < len);
                Ensure(node == null || node.Item.Length >= len);
                Ensure(node == null || bst.Nodes.First(nd => nd.Item.Length >= len) == node);
            }
        }


        void FloorTest()
        {
            var bst = CreateTree();
            var len = SampleFactory.NextByte;
            var node = bst.Floor(len);

            Ensure(!bst.IsEmpty || node == null);

            if(!bst.IsEmpty)
            {
                Ensure(node != null || len < bst.Keys.First());
                Ensure(node == null || node.Item.Length <= len);
                Ensure(node == null || bst.Nodes.Last(nd => nd.Item.Length <= len) == node);
            }
        }

        void ConstructionTest()
        {
            var count = SampleFactory.NextByte * 2;
            var bst = CreateTree(count);

            Ensure(bst.Count == count);
            Ensure(bst.Nodes.All(nd => nd.LeftChild == null || nd.LeftChild.Item.Length < nd.Item.Length));
            Ensure(bst.Nodes.All(nd => nd.RightChild == null || nd.Item.Length < nd.RightChild.Item.Length));
            Ensure(IsSorted(bst.Keys.ToArray()));
            Ensure(IsSorted(bst.Nodes.Select(nd => nd.Item.Length).ToArray()));
        }

        BinarySearchTree<string, int> CreateTree(int nodeCount)
        {
            var tree = new BinarySearchTree<string, int>(s => s.Length);

            int count = 0;
            var seq = SampleFactory.CreateStrings(nodeCount * 2);

            while(count < nodeCount)
            {
                var str = seq.First();

                if (tree.TryAdd(str,out _))
                    ++count;                
            }

            return tree;
        }

        BinarySearchTree<string, int> CreateTree() => CreateTree(SampleFactory.NextByte * 2);        

        bool IsSorted(int[] items)
        {
            if (items.Length == 0)
                return true;

            int last = items[0];
            for(int i = 1; i < items.Length; ++i)
            {
                if (last > items[i])
                    return false;

                last = items[i];
            }

            return true;
        }
    }
}
