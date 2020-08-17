using easyLib.IO;
using System.Text;
using static easyLib.DebugHelper;

namespace easyLib.DB
{
    partial class FlatTable<T>
    {
        public interface IFlatTableHeader : ITableHeader
        {
            int RowSize { get; set; }
        }
        //---------------------------------------------------

        protected class FlatTableHeader : TableHeader, IFlatTableHeader
        {
            const string TBL_SIGNATURE = "EZTBLFLAT0";

            public int RowSize { get; set; }


            //protected:
            protected override byte[] Signature => Encoding.UTF8.GetBytes(TBL_SIGNATURE);
            protected override void ReadImmutableData(IBinStreamReader reader)
            {
                base.ReadImmutableData(reader);
                RowSize = reader.ReadInt();
            }

            protected override void WriteImmutableData(IBinStreamWriter writer)
            {
                Assert(RowSize > 0);

                base.WriteImmutableData(writer);
                writer.Write(RowSize);
            }

            protected override bool ClassInvariant => base.ClassInvariant && RowSize >= 0;
        }
    }
}
