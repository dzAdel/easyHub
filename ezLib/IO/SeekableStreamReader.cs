using System.IO;
using static easyLib.DebugHelper;


namespace easyLib.IO
{
    public interface ISeekableStreamReader : IBinStreamReader
    {
        long Position { get; set; }
        long Length { get; }
    }
    //-------------------------------------------------------------

    public sealed class SeekableStreamReader : BinStreamReader, ISeekableStreamReader
    {
        public SeekableStreamReader(Stream srcStream, ByteOrder endianess = ByteOrder.System) :
            base(srcStream, endianess)
        {
            Assert(srcStream != null);
            Assert(srcStream.CanRead);
            Assert(srcStream.CanSeek);
        }


        public long Length => InputStream.Length;

        public long Position
        {
            get => InputStream.Position;

            set
            {
                Assert(value >= 0);
                InputStream.Position = value;
            }
        }
    }
}
