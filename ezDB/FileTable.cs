using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static easyLib.DebugHelper;
using System.Collections;
using System.IO;
using easyLib.IO;

namespace easyLib.DB
{       
    //public abstract partial class FileTable : ITable, IAsyncTable, IDisposable
    //{
    //    readonly object m_ioLock = new object();
    //    FileStream m_fs;
    //    long m_dataOffset;
    //    bool m_isDirty;

    //    public event Action<long, byte[]> RowAdded;
    //    public event Action<long> RowReplacing;
    //    public event Action<long, byte[]> RowReplaced;
    //    public event Action<long> RowDeleting;
    //    public event Action<long> RowDeleted;


    //    public string FilePath { get; }

    //    public long Count
    //    {
    //        get
    //        {
    //            lock (m_ioLock)
    //                return Header.RowCount;
    //        }
    //    }

    //    public ByteOrder ByteOrder
    //    {
    //        get
    //        {
    //            lock (m_ioLock)
    //                return Header.IsBigEndian ? ByteOrder.BigEndian : ByteOrder.LittleEndian;
    //        }
    //    }

    //    public int RowSizeLimit => GetRowSizeLimit();

    //    public bool IsDisposed { get; private set; }

    //    public long Append(byte[] row)
    //    {
    //        Assert(row != null);
    //        Assert(row.Length <= RowSizeLimit);

    //        return AppendAsync(row).Result;
    //    }

    //    public async Task<long> AppendAsync(byte[] row)
    //    {
    //        Assert(row != null);
    //        Assert(row.Length <= RowSizeLimit);

    //        Func<long> fn = () =>
    //        {
    //            lock (m_ioLock)
    //            {
    //                long l = AppendRow(row);
    //                ++Header.RowCount;
    //                ++Header.Generation;
    //                Header.LastWriteTime = DateTime.Now;
    //                m_isDirty = true;

    //                return l;
    //            }
    //        };


    //        long ndx = await Task.Run(fn);
    //        RowAdded?.Invoke(ndx, row);

    //        Assert(Count > 0);
    //        Assert(ndx < Count);

    //        return ndx;
    //    }

    //    public void Delete(long ndxRow)
    //    {
    //        Assert(ndxRow < Count);

    //        try
    //        {
    //            DeleteAsync(ndxRow).Wait();
    //        }
    //        catch (AggregateException ex)
    //        {
    //            throw ex.InnerException;
    //        }
    //    }

    //    public async Task DeleteAsync(long ndxRow)
    //    {
    //        Assert(ndxRow < Count);

    //        Action act = () =>
    //        {
    //            lock (m_ioLock)
    //            {
    //                DeleteRow(ndxRow);
    //                --Header.RowCount;
    //                ++Header.Generation;
    //                Header.LastWriteTime = DateTime.Now;
                    
    //                m_isDirty = true;
    //            }
    //        };

    //        RowDeleting?.Invoke(ndxRow);
    //        await Task.Run(act);
    //        RowDeleted?.Invoke(ndxRow);
    //    }

    //    public void Dispose()
    //    {
    //        if (!IsDisposed)
    //        {
    //            Flush();
    //            DoDispose();
    //            IsDisposed = true;
    //        }
    //    }

    //    public void Flush()
    //    {
    //        if (m_isDirty)
    //            try
    //            {
    //                FlushAsync().Wait();
    //            }
    //            catch (AggregateException ex)
    //            {
    //                throw ex.InnerException;
    //            }
    //    }

    //    public Task FlushAsync()
    //    {
    //        Action act = () =>
    //        {
    //            lock (m_ioLock)
    //            {
    //                var writer = new SeekableStreamWriter(m_fs, ByteOrder);
    //                Header.Flush(writer);
    //                DoFlush();
    //                m_isDirty = false;
    //            }
    //        };

    //        return m_isDirty ? Task.Run(act) : Task.CompletedTask;
    //    }

    //    public byte[] Get(long ndxRow)
    //    {
    //        Assert(ndxRow < Count);

    //        return GetAsync(ndxRow).Result;
    //    }

    //    public async Task<byte[]> GetAsync(long ndxRow)
    //    {
    //        Assert(ndxRow < Count);

    //        Func<byte[]> fn = () =>
    //        {
    //            lock (m_ioLock)
    //                return ReadRow(ndxRow);
    //        };

    //        byte[] row = await Task.Run(fn);

    //        Assert(row.Length <= RowSizeLimit);

    //        return row;
    //    }

    //    public IEnumerator<byte[]> GetEnumerator()
    //    {
    //        long ndx = 0;
    //        byte[] row;

    //        while (true)
    //        {
    //            lock (m_ioLock)
    //            {
    //                if (ndx == RowCount)
    //                    yield break;

    //                row = Get(ndx++);
    //            }

    //            yield return row;
    //        }
    //    }

    //    public long Replace(long ndxRow, byte[] row)
    //    {
    //        Assert(ndxRow < Count);
    //        Assert(row != null);
    //        Assert(row.Length <= RowSizeLimit);

    //        return ReplaceAsync(ndxRow, row).Result;
    //    }

    //    public async Task<long> ReplaceAsync(long ndxRow, byte[] row)
    //    {
    //        Assert(ndxRow < Count);
    //        Assert(row != null);
    //        Assert(row.Length <= RowSizeLimit);

    //        Func<long> fn = () =>
    //        {
    //            lock (m_ioLock)
    //            {
    //                long l = ReplaceRow(ndxRow, row);
    //                m_isDirty = true;
    //                return l;
    //            }
    //        };

    //        RowReplacing?.Invoke(ndxRow);
    //        long ndx = await Task.Run(fn);
    //        RowReplaced?.Invoke(ndx, row);
    //        return ndx;
    //    }

    //    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    //    //protected:
    //    protected FileTable(string filePath)
    //    {
    //        Assert(!string.IsNullOrWhiteSpace(filePath));
    //        FilePath = filePath;
    //    }

    //    protected abstract TableHeader Header { get; }
    //    protected abstract long RowCount { get; set; }        
    //    protected abstract long AppendRow(byte[] row);
    //    protected abstract void DeleteRow(long ndxRow);
    //    protected abstract void DoDispose();
    //    protected abstract void DoFlush();
    //    protected abstract byte[] ReadRow(long ndxRow);
    //    protected abstract long ReplaceRow(long ndxRow, byte[] row);
    //    protected virtual int GetRowSizeLimit() => int.MaxValue;
    //    protected virtual void CreateFile(ByteOrder endianess)
    //    {            
    //        Assert(!IsDisposed);            

    //        const int SZ_BUFFER = 1024 * 4;

    //        m_fs = new FileStream(FilePath,
    //            FileMode.Create,
    //            FileAccess.ReadWrite,
    //            FileShare.None,
    //            SZ_BUFFER,
    //            FileOptions.RandomAccess);            

    //        try
    //        {
    //            Header.IsBigEndian = endianess.Normalize() == ByteOrder.BigEndian;
    //            var writer = new SeekableStreamWriter(m_fs, endianess);

    //            Header.Write(writer);
    //        }
    //        catch 
    //        {
    //            m_fs.Dispose();
    //            throw;
    //        }
    //    }
    //    protected virtual void OpenFile() => throw new NotImplementedException();

    //}

}
