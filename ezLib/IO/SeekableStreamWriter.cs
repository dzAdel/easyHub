using System;
using System.IO;
using static easyLib.DebugHelper;


namespace easyLib.IO
{
    public interface ISeekableStreamWriter : IBinStreamWriter
    {
        long Position { get; set; }
        long Length { get; }
    }
    //------------------------------------------------------------------

    public sealed class SeekableStreamWriter : BinStreamWriter, ISeekableStreamWriter
    {
        public SeekableStreamWriter(Stream destStream, ByteOrder endianess = ByteOrder.System) :
            base(destStream, endianess)
        {
            Assert(destStream != null);
            Assert(destStream.CanWrite);
            Assert(destStream.CanSeek);
            Assert(Enum.IsDefined(typeof(ByteOrder), endianess));
        }


        public long Length => OutputStream.Length;

        public long Position
        {
            get => OutputStream.Position;

            set
            {
                Assert(value >= 0);
                OutputStream.Position = value;
            }
        }
    }
}
