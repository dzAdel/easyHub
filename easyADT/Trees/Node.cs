using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easyLib.ADT.Trees
{
    public interface INode<out T>
    {
        bool IsRoot { get; }
        bool IsLeaf { get; }
        INode<T> Parent { get; }
        IEnumerable<INode<T>> Children { get; }        
        uint Degree { get; }
        uint GetDepth();
        public IEnumerable<INode<T>> GetPath();
    }
}
