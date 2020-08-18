using System.Collections.Generic;
using System.IO;
using static easyLib.DebugHelper;


namespace easyLib
{
    public static class MultiByteCodec
    {
        public static byte[] GetBytes(short s) => GetBytes((ulong)(ushort)s);

        public static short GetShort(IEnumerable<byte> bytes)
        {
            Assert(bytes != null);
            return (short)Decode(bytes, sizeof(short));
        }

        public static byte[] GetBytes(ushort s) => GetBytes((ulong)s);

        public static ushort GetUShort(IEnumerable<byte> bytes)
        {
            Assert(bytes != null);

            return (ushort)Decode(bytes, sizeof(ushort));
        }

        public static byte[] GetBytes(int n) => GetBytes((ulong)(uint)n);

        public static int GetInt(IEnumerable<byte> bytes)
        {
            Assert(bytes != null);

            return (int)Decode(bytes, sizeof(int));
        }

        public static byte[] GetBytes(uint u) => GetBytes((ulong)u);

        public static uint GetUInt(IEnumerable<byte> bytes)
        {
            Assert(bytes != null);

            return (uint)Decode(bytes, sizeof(uint));
        }

        public static byte[] GetBytes(long l) => GetBytes((ulong)l);

        public static long GetLong(IEnumerable<byte> bytes)
        {
            Assert(bytes != null);

            return (long)Decode(bytes, sizeof(long));
        }

        public static byte[] GetBytes(ulong ul)
        {
            if (ul <= 0x7F)
                return new byte[] { (byte)ul };


            const int maxBytes = 9;
            var bytes = new List<byte>(maxBytes);

            do
            {
                byte b = (byte)(ul & 0x7F);

                if ((ul >>= 7) != 0)
                    b |= 0b10000000;

                bytes.Add(b);

                if (bytes.Count == maxBytes - 1)
                {
                    Assert(ul <= byte.MaxValue);

                    bytes.Add((byte)(ul));
                    break;
                }


            } while (ul != 0);

            return bytes.ToArray();
        }

        public static ulong GetULong(IEnumerable<byte> bytes)
        {
            Assert(bytes != null);

            return Decode(bytes, sizeof(ulong));
        }


        //private:
        static ulong Decode(IEnumerable<byte> bytes, int szDatum)
        {
            int msbMaxValue = (1 << szDatum) - 1;
            int ndxByte = 0;
            ulong result = 0;

            foreach (byte b in bytes)
            {
                if (ndxByte == szDatum)
                    if (b > msbMaxValue)
                        break;
                    else
                        return result | ((ulong)b << (szDatum * 7));


                result |= (ulong)(b & 0b01111111) << (ndxByte * 7);

                if ((b & 0b10000000) == 0)
                    return result;

                ++ndxByte;
            }

            throw new InvalidDataException();
        }
    }
}
