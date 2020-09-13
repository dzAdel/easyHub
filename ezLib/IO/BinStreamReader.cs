using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static easyLib.DebugHelper;


namespace easyLib.IO
{
    public interface IBinStreamReader : IEnumerable<byte>
    {
        int Read(byte[] buffer, int count, int offset = 0);
        byte ReadByte();
        byte[] ReadBytes(int count);       
        sbyte ReadSByte();
        bool ReadBool();
        char ReadChar();
        short ReadShort();
        ushort ReadUShort();
        int ReadInt();
        uint ReadUInt();
        long ReadLong();
        ulong ReadULong();
        float ReadFloat();
        double ReadDouble();
        decimal ReadDecimal();
        string ReadString();
        DateTime ReadTime();
    }
    //------------------------------------------------------------------------------

    public partial class BinStreamReader : IBinStreamReader
    {
        readonly Stream m_inStream;
        byte[] m_buffer = new byte[sizeof(int)];
        UTF8Encoding m_encoding;
        bool m_needReorder;
        ByteOrder m_endianess;

        public BinStreamReader(Stream srcStream, ByteOrder endianess = ByteOrder.System)
        {
            Assert(srcStream != null);
            Assert(srcStream.CanRead);
            Assert(Enum.IsDefined(typeof(ByteOrder), endianess));

            m_inStream = srcStream;
            ByteOrder = endianess;
        }


        public ByteOrder ByteOrder
        {
            get => m_endianess;

            set
            {
                Assert(Enum.IsDefined(typeof(ByteOrder), value));

                m_endianess = value.Normalize();
                m_needReorder = BitConverter.IsLittleEndian != (m_endianess == ByteOrder.LittleEndian);
            }
        }

        public DateTime ReadTime() => new DateTime(ReadLong());
        public uint ReadUInt() => (uint)ReadInt();
        public ulong ReadULong() => (ulong)ReadLong();
        public ushort ReadUShort() => (ushort)ReadShort();
        public sbyte ReadSByte() => (sbyte)ReadByte();
        public IEnumerator<byte> GetEnumerator() => new ByteEnumerator(this);

        public bool ReadBool()
        {
            byte b = ReadByte();

            if (b > 1)
                throw new CorruptedStreamException();

            return b == 0 ? false : true;            
        }

        public byte ReadByte()
        {
            int n = m_inStream.ReadByte();

            if (n < 0)
                throw new EndOfStreamException();

            return (byte)n;
        }

        public byte[] ReadBytes(int count)
        {
            Assert(count >= 0);

            var bytes = new byte[count];

            if (Read(bytes, count) != count)
                throw new EndOfStreamException();

            return bytes;
        }

        public int Read(byte[] buffer, int count, int offset = 0)
        {
            Assert(buffer != null);
            Assert(count >= 0);
            Assert(offset >= 0);
            Assert(count <= buffer.Length - offset);

            int nRead = 0;

            while (nRead != count)
            {
                int n = m_inStream.Read(buffer, nRead, count - nRead);

                if (n == 0)
                    break;

                nRead += n;
            }

            return nRead;
        }

        public char ReadChar()
        {
            if (LoadBytes(sizeof(char)) != sizeof(char))
                throw new EndOfStreamException();

            if (m_needReorder)
                m_buffer.ReverseBytes(sizeof(char), 1);

            return BitConverter.ToChar(m_buffer, 0);
        }

        public decimal ReadDecimal()
        {
            const int sz = sizeof(decimal) / sizeof(int);

            var data = new int[sz];

            for (int i = 0; i < data.Length; ++i)
                data[i] = ReadInt();


            try
            {
                return new decimal(data);
            }
            catch (ArgumentException ex)
            {
                throw new CorruptedStreamException(innerException: ex);
            }
        }

        public double ReadDouble()
        {
            if (LoadBytes(sizeof(double)) != sizeof(double))
                throw new EndOfStreamException();

            if (m_needReorder)
                m_buffer.ReverseBytes(sizeof(double), 1);

            return BitConverter.ToDouble(m_buffer, 0);
        }

        public float ReadFloat()
        {
            if (LoadBytes(sizeof(float)) != sizeof(float))
                throw new EndOfStreamException();

            if (m_needReorder)
                m_buffer.ReverseBytes(sizeof(float), 1);

            return BitConverter.ToSingle(m_buffer, 0);
        }

        public int ReadInt()
        {
            if (LoadBytes(sizeof(int)) != sizeof(int))
                throw new EndOfStreamException();

            if (m_needReorder)
                m_buffer.ReverseBytes(sizeof(int), 1);

            return BitConverter.ToInt32(m_buffer, 0);
        }

        public long ReadLong()
        {
            if (LoadBytes(sizeof(long)) != sizeof(long))
                throw new EndOfStreamException();

            if (m_needReorder)
                m_buffer.ReverseBytes(sizeof(long), 1);

            return BitConverter.ToInt64(m_buffer, 0);
        }

        public short ReadShort()
        {
            if (LoadBytes(sizeof(short)) != sizeof(short))
                throw new EndOfStreamException();

            if (m_needReorder)
                m_buffer.ReverseBytes(sizeof(short), 1);

            return BitConverter.ToInt16(m_buffer, 0);
        }

        public string ReadString()
        {
            int sz = MultiByteCodec.GetInt(this);

            if (sz == 0)
                return "";

            byte[] bytes = ReadBytes(sz);
            return Decoder.GetString(bytes);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        //protected:
        protected Stream InputStream => m_inStream;


        //private:
        UTF8Encoding Decoder
        {
            get
            {
                if (m_encoding == null)
                    m_encoding = new UTF8Encoding(false, true);

                return m_encoding;
            }
        }

        int LoadBytes(int count)
        {
            if (m_buffer.Length < count)
                m_buffer = new byte[count];

            return m_inStream.Read(m_buffer, 0, count);
        }
    }
}
