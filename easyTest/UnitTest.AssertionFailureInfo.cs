using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easyTest
{
    partial class UnitTest
    {
        class AssertionFailureInfo : FailureInfo
        {
            public AssertionFailureInfo():
                base("Assertion failure")
            { }

            public string Expression { get; set; }

            //protected:
            protected override IEnumerable<string> FailureLog
            {
                get
                {
                    if (!string.IsNullOrWhiteSpace(Expression))
                        yield return $"Expression: {Expression}";
                }
            }
        }
    }
}
