using System;
using System.Collections.Generic;
using static easyLib.DebugHelper;


namespace easyLib.Test
{
    public sealed class RTExceptionInfo : FailureInfo
    {
        readonly Exception m_ex;

        public RTExceptionInfo(Exception ex) :
            base("Unasserted exception catched")
        {
            Assert(ex != null);
            m_ex = ex;
        }

        //protected:
        protected override IEnumerable<string> FailureLog
        {
            get
            {
                yield return $"Exception: {m_ex.GetType()}";
                yield return $"Target site: {m_ex.TargetSite}";

                if (m_ex.Source != null)
                    yield return $"Source: {m_ex.Source}";

                yield return m_ex.Message;
            }
        }
    }
}
