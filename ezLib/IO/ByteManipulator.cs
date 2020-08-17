using System.Collections.Generic;
using static easyLib.DebugHelper;


namespace easyLib.IO
{
    public static class ByteManipulator
    {
        public const int SizeOfTime = sizeof(long);

        public static void ReverseBytes(this IList<byte> buffer, int szItem, int itemCount, int offset = 0)
        {
            Assert(buffer != null);
            Assert(szItem >= 0);
            Assert(itemCount >= 0);
            Assert(offset >= 0);
            Assert(itemCount * szItem >= 0);
            Assert(itemCount * szItem <= buffer.Count - offset);

            int halfSize = szItem >> 1;
            int range = itemCount * szItem + offset;

            for (int ndx = offset; ndx < range; ndx += szItem)
            {
                for (int j = 0; j < halfSize; ++j)
                {
                    int ndxByte0 = ndx + j;
                    int ndxByte1 = ndx + szItem - 1 - j;

                    byte tmp = buffer[ndxByte0];
                    buffer[ndxByte0] = buffer[ndxByte1];
                    buffer[ndxByte1] = tmp;
                }
            }
        }
    }
}
