using System;
using System.Collections;
using System.Collections.Generic;

namespace easyLib.IO
{
    partial class BinStreamReader
    {
        struct ByteEnumerator : IEnumerator<byte>
        {
            readonly IBinStreamReader m_reader;
            int m_current;
            bool m_disposed;

            public ByteEnumerator(IBinStreamReader reader)
            {
                m_reader = reader;
                m_current = -1;
                m_disposed = false;
            }

            public byte Current
            {
                get
                {
                    if (m_current < 0 || m_disposed)
                        throw new InvalidOperationException();

                    return (byte)m_current;
                }
            }

            object IEnumerator.Current => Current;

            public void Dispose() => m_disposed = true;

            public bool MoveNext()
            {
                if (m_disposed)
                    throw new InvalidOperationException();

                m_current = m_reader.ReadByte();
                return m_current >= 0;
            }

            public void Reset() => throw new NotSupportedException();
        }
    }
}
