using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.ADT
{
    interface INodeInfo<T>
    {
        T Data { get; set; }
        int Depth { get; }
        int DescendantCount { get; }
    }
    //-------------------------------------

    class NodeInfo<T> : INodeInfo<T>
    {
        public NodeInfo(int depth, T data = default)
        {
            Depth = depth;
            Data = data;
        }

        public T Data { get; set; }
        public int Depth { get; }
        public int DescendantCount { get; set; } = 1;
    }
}
