using easyLib.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using easyLib.Extensions;

namespace TestApp.Extensions
{
    public class ListExTest: UnitTest
    {
        public ListExTest():
            base("ListEx Test")
        { }


        //protected:
        protected override void Start()
        {
            var strs = SampleFactory.CreateStrings().Take(SampleFactory.CreateBytes(min: 1).First()).Distinct().ToList();

            for (int i = 0; i < strs.Count; ++i)
                if (!Ensure(strs.IndexOf(strs[i], i) == i))
                    break;

            var strs2 = strs.Concat(strs).ToList();
            Ensure(strs2.All(s => strs[strs2.IndexOf(s, strs.Count) - strs.Count] == s));
        }
    }
}
