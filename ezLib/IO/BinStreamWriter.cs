using System;
using System.IO;
using System.Text;
using static easyLib.DebugHelper;


namespace easyLib.IO
{
    public interface IBinStreamWriter
    {
        void Write(byte b);
        void Write(byte[] bytes);
        void Write(byte[] buffer, int count, int offset = 0);

        void Write(sbyte sb);
        void Write(bool b);
        void Write(char c);
        void Write(short s);
        void Write(ushort us);
        void Write(int i);
        void Write(uint ui);
        void Write(long l);
        void Write(ulong ul);
        void Write(float f);
        void Write(double d);
        void Write(decimal d);
        void Write(string s);
        void Write(DateTime t);
    }
    //-----------------------------------------------------------

    public class BinStreamWriter : IBinStreamWriter
    {
        readonly Stream m_outStream;
        UTF8Encoding m_encoding;
        ByteOrder m_endianess;
        bool m_needReorder;

        public BinStreamWriter(Stream destStream, ByteOrder endianess = ByteOrder.System)
        {
            Assert(destStream != null);
            Assert(destStream.CanWrite);
            Assert(Enum.IsDefined(typeof(ByteOrder), endianess));

            m_outStream = destStream;
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


        public void Write(byte b) => m_outStream.WriteByte(b);

        public void Write(byte[] bytes)
        {
            Assert(bytes != null);

            m_outStream.Write(bytes, 0, bytes.Length);
        }

        public void Write(byte[] buffer, int count, int offset = 0)
        {
            Assert(buffer != null);
            Assert(count >= 0);
            Assert(offset >= 0);
            Assert(buffer.Length - offset >= count);

            m_outStream.Write(buffer, offset, count);
        }

        public void Write(sbyte sb) => Write((byte)sb);

        public void Write(bool b) => m_outStream.WriteByte((byte)(b ? 1 : 0));

        public void Write(char c)
        {
            byte[] bytes = BitConverter.GetBytes(c);

            if (m_needReorder)
                bytes.ReverseBytes(sizeof(char), 1);

            m_outStream.Write(bytes, 0, sizeof(char));
        }

        public void Write(short s)
        {
            byte[] bytes = BitConverter.GetBytes(s);

            if (m_needReorder)
                bytes.ReverseBytes(sizeof(short), 1);

            m_outStream.Write(bytes, 0, sizeof(short));
        }

        public void Write(ushort us) => Write((short)us);

        public void Write(int i)
        {
            byte[] bytes = BitConverter.GetBytes(i);

            if (m_needReorder)
                bytes.ReverseBytes(sizeof(int), 1);

            m_outStream.Write(bytes, 0, sizeof(int));
        }

        public void Write(uint ui) => Write((int)ui);

        public void Write(long l)
        {
            byte[] bytes = BitConverter.GetBytes(l);

            if (m_needReorder)
                bytes.ReverseBytes(sizeof(long), 1);

            m_outStream.Write(bytes, 0, sizeof(long));
        }

        public void Write(ulong ul) => Write((long)ul);

        public void Write(float f)
        {
            byte[] bytes = BitConverter.GetBytes(f);

            if (m_needReorder)
                bytes.ReverseBytes(sizeof(float), 1);

            m_outStream.Write(bytes, 0, sizeof(float));
        }

        public void Write(double d)
        {
            byte[] bytes = BitConverter.GetBytes(d);

            if (m_needReorder)
                bytes.ReverseBytes(sizeof(double), 1);

            m_outStream.Write(bytes, 0, sizeof(double));
        }

        public void Write(decimal d)
        {
            int[] data = decimal.GetBits(d);

            for (int i = 0; i < data.Length; ++i)
                Write(data[i]);
        }

        public void Write(string s)
        {
            Assert(s != null);

            byte[] bytes = Encoder.GetBytes(s);
            byte[] sz = MultiByteCodec.GetBytes(bytes.Length);

            Write(sz);
            Write(bytes);
        }

        public void Write(DateTime t) => Write(t.Ticks);


        //protected:
        protected Stream OutputStream => m_outStream;

        //private:
        UTF8Encoding Encoder
        {
            get
            {
                if (m_encoding == null)
                    m_encoding = new UTF8Encoding(false, true);

                return m_encoding;
            }
        }
    }


}
