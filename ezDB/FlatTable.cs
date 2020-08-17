using easyLib.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using static easyLib.DebugHelper;


namespace easyLib.DB
{
    public interface IFlatTable : ITable
    {
        int RowSize { get; }        
    }
    //-----------------------------------------

    public interface IAsyncFlatTable : IAsyncTable
    {
        int RowSize { get; }        
    }
    //---------------------------------------------------

    public partial class FlatTable<T> : IFlatTable, IAsyncFlatTable
        where T: FlatTable<T>.IFlatTableHeader, new()
    {
        readonly object m_ioLock = new object();
        readonly T m_header = new T();
        readonly List<int> m_delFrames = new List<int>();
        FileStream m_fs;
        bool m_isDirty;

        public event Action<int, byte[]> RowAdded;
        public event Action<int> RowReplacing;
        public event Action<int, byte[]> RowReplaced;
        public event Action<int> RowDeleting;
        public event Action<int> RowDeleted;


        public string FilePath { get; }
        public ByteOrder ByteOrder { get; private set; }
        public int RowSize => m_header.RowSize;
        public int RowSizeLimit => RowSize;

        public int Count
        {
            get
            {
                lock(m_ioLock)
                    return m_header.FrameCount;
            }
        }
        
        public int Append(byte[] row)
        {
            Assert(row != null);
            Assert(row.Length <= RowSize);

            return AppendAsync(row).Result;
        }

        public Task<int> AppendAsync(byte[] row) => throw new NotImplementedException();
        private long RowIndexToFilePos(long rowCount)
        {
            throw new NotImplementedException();
        }

        public void Delete(int ndxRow)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int ndxRow)
        {
            throw new NotImplementedException();
        }

        public void Flush()
        {
            throw new NotImplementedException();
        }

        public Task FlushAsync()
        {
            throw new NotImplementedException();
        }

        public byte[] Get(int ndxRow)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> GetAsync(int ndxRow)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<byte[]> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int Replace(int ndxRow, byte[] row)
        {
            throw new NotImplementedException();
        }

        public Task<int> ReplaceAsync(int ndxRow, byte[] row)
        {
            throw new NotImplementedException();
        }

        public static FlatTable<T> Create(string filePath, int szRow, ByteOrder endianess = ByteOrder.System)
        {
            Assert(!string.IsNullOrEmpty(filePath));
            Assert(szRow > 0);
            Assert(Enum.IsDefined(typeof(ByteOrder), endianess));

            const int SZ_BUFFER = 1024 * 4;

            var tbl = new FlatTable<T>(filePath);

            tbl.m_fs = new FileStream(filePath,
                FileMode.Create,
                FileAccess.ReadWrite,
                FileShare.None,
                SZ_BUFFER,
                FileOptions.RandomAccess);

            tbl.ByteOrder = endianess.Normalize();

            tbl.m_header.RowSize = szRow;
            bool be = tbl.ByteOrder == ByteOrder.BigEndian;

            var writer = new SeekableStreamWriter(tbl.m_fs, endianess);

            try
            {
                writer.Write(be);
                tbl.m_header.Write(writer);
            }
            catch
            {
                tbl.m_fs.Dispose();
            }

            return tbl;
        }

        public static FlatTable<T> Open(string filePath)
        {
            Assert(!string.IsNullOrEmpty(filePath));

            const int SZ_BUFFER = 1024 * 4;
            var tbl = new FlatTable<T>(filePath);

            tbl.m_fs = new FileStream(filePath,
            FileMode.Open,
            FileAccess.ReadWrite,
            FileShare.None,
            SZ_BUFFER,
            FileOptions.RandomAccess);

            var reader = new SeekableStreamReader(tbl.m_fs);

            try
            {
                tbl.ByteOrder = reader.ReadBool() ? ByteOrder.BigEndian : ByteOrder.LittleEndian;
                reader.ByteOrder = tbl.ByteOrder;
                tbl.m_header.Read(reader);
            }
            catch
            {
                tbl.m_fs.Dispose();
            }

            return tbl;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        


        //protected:
        protected FlatTable(string filePath)
        {
            Assert(!string.IsNullOrWhiteSpace(filePath));

            FilePath = filePath;            
        }

        protected T Header => m_header;

        //private:
        int FrameSize => Math.Max(sizeof(int), m_header.RowSize);
        int RowIndexToFrameIndex(int ndxRow)
        {
            if (m_header.DelFrameCount == 0 || ndxRow < m_header.FirstDelFrameIndex)
                return ndxRow;

            if (m_header.DelFrameCount < m_header.LastDelFrameIndex)
                return ndxRow + m_header.DelFrameCount;


            int ndx;

            if (m_delFrames.Count != m_header.DelFrameCount)
            {
                int delListLen = m_delFrames.Count;

                if (delListLen == 0 || m_delFrames[delListLen - 1] < ndxRow)
                    LoadDelFrameList(ndxRow);
            }

            ndx = ~m_delFrames.BinarySearch(ndxRow);

            Assert(ndx > m_header.FirstDelFrameIndex);
            Assert(ndx < m_header.LastDelFrameIndex);

            return ndx + ndxRow;
        }

        void LoadDelFrameList(int ndxRow)
        {
            Assert(m_delFrames.Count == 0 || m_delFrames[m_delFrames.Count] < ndxRow);
        }
    }


}
