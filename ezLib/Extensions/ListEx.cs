using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static easyLib.DebugHelper;


namespace easyLib.Extensions
{
    public static class ListEx
    {
        public static int IndexOf<T>(this IList<T> lst, T item, int ndxStart)
        {
            Assert(lst != null);
            Assert(ndxStart < lst.Count);

            if (lst is List<T> list)
                return list.IndexOf(item, ndxStart);



            for (int i = ndxStart; i < lst.Count; ++i)
                if (EqualityComparer<T>.Default.Equals(lst[i], item))
                    return i;

            return -1;
        }
    }
}
