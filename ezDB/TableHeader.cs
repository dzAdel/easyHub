using easyLib.IO;
using System;
using static easyLib.DebugHelper;

namespace easyLib.DB
{
    public interface ITableHeader
    {
        DateTime CreationTime { get; }
        ulong Generation { get; set; }        
        DateTime LastWriteTime { get; set; }
        int FrameCount { get; set; }
        int DelFrameCount { get; set; }
        int FirstDelFrameIndex { get; set; }
        int LastDelFrameIndex { get; set; }


        void Flush(ISeekableStreamWriter writer);
        void Read(ISeekableStreamReader reader);
        void Write(ISeekableStreamWriter writer);
    }
    //---------------------------------------------------

    public abstract class TableHeader : ITableHeader
    {
        long m_mutableDataOffset;
        int m_frmCount;
        int m_delFrmCount;
        int m_ndxFirstDelFrm;
        int m_ndxLastDelFrm;

        public const int NULL_INDEX = int.MaxValue;

        public DateTime CreationTime { get; private set; } = DateTime.Now;
        public int FrameCount
        {
            get => m_frmCount;
            set
            {
                Assert(value >= 0);
                m_frmCount = value;

                Assert(ClassInvariant);
            }
        }

        public int DelFrameCount
        {
            get => m_delFrmCount;
            set
            {
                Assert(value >= 0);
                Assert(value <= FrameCount);

                m_delFrmCount = value;

                Assert(ClassInvariant);
            }
        }

        public int FirstDelFrameIndex
        {
            get => m_ndxFirstDelFrm;
            set
            {
                Assert(value >= 0);
                Assert(value <= LastDelFrameIndex);

                m_ndxFirstDelFrm = value;

                Assert(ClassInvariant);
            }
        }

        public int LastDelFrameIndex
        {
            get => m_ndxLastDelFrm;
            set
            {
                Assert(value >= FirstDelFrameIndex);
                Assert(value < FrameCount);

                m_ndxLastDelFrm = value;

                Assert(ClassInvariant);
            }
        }
        public DateTime LastWriteTime { get; set; }
        public ulong Generation { get; set; }

        public void Read(ISeekableStreamReader reader)
        {
            Assert(reader != null);

            ReadImmutableData(reader);
            m_mutableDataOffset = reader.Position;
            ReadMutableData(reader);

            Assert(ClassInvariant);

            if (!ClassInvariant)
                throw new CorruptedStreamException();
        }

        public void Write(ISeekableStreamWriter writer)
        {
            Assert(writer != null);

            WriteImmutableData(writer);
            m_mutableDataOffset = writer.Position;
            WriteMutableData(writer);
        }

        public void Flush(ISeekableStreamWriter writer)
        {
            Assert(writer != null);

            if (m_mutableDataOffset == 0)
                Write(writer);
            else
            {
                writer.Position = m_mutableDataOffset;
                WriteMutableData(writer);
            }
        }

        //protected:
        protected abstract byte[] Signature { get; }

        protected virtual void WriteImmutableData(IBinStreamWriter writer)
        {
            writer.Write(Signature);
            writer.Write(CreationTime);
        }

        protected virtual void WriteMutableData(IBinStreamWriter writer)
        {
            writer.Write(m_frmCount);
            writer.Write(LastWriteTime);
            writer.Write(Generation);
            writer.Write(m_delFrmCount);
            writer.Write(m_ndxFirstDelFrm);
            writer.Write(m_ndxLastDelFrm);            
        }

        protected virtual void ReadImmutableData(IBinStreamReader reader)
        {
            byte[] sign = Signature;
            var bytes = reader.ReadBytes(sign.Length);

            for (int i = 0; i < sign.Length; ++i)
                if (sign[i] != bytes[i])
                    throw new CorruptedStreamException();

            CreationTime = reader.ReadTime();
        }

        protected virtual void ReadMutableData(IBinStreamReader reader)
        {
            m_frmCount = reader.ReadInt();

            LastWriteTime = reader.ReadTime();
            Generation = reader.ReadULong();

            m_delFrmCount = reader.ReadInt();
            m_ndxFirstDelFrm = reader.ReadInt();
            m_ndxLastDelFrm = reader.ReadInt();            
        }

        protected virtual bool ClassInvariant => DelFrameCount >= 0 &&
            FrameCount >= DelFrameCount &&
            DelFrameCount == 0 || LastDelFrameIndex <= FrameCount &&
            DelFrameCount == 0 || FirstDelFrameIndex <= LastDelFrameIndex;
    }
}
