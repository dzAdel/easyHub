using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static easyLib.DebugHelper;


namespace easyLib.Test
{
    public static class SampleFactory
    {
        static Random m_rand = new Random();

        public static byte NextByte => CreateBytes().First();
        public static int NextInt => CreateInts().First();

        public static IEnumerable<byte> CreateBytes(byte min = byte.MinValue, byte limit = byte.MaxValue)
        {
            Assert(min < limit);

            const int SZ_BUFFER = 16;
            var buffer = new byte[SZ_BUFFER];

            while (true)
            {
                lock (m_rand)
                    m_rand.NextBytes(buffer);

                foreach (byte b in buffer)
                    if (min <= b && b < limit)
                        yield return b;
            }
        }

        public static IEnumerable<sbyte> CreateSBytes(sbyte min = sbyte.MinValue, sbyte limit = sbyte.MaxValue)
        {
            Assert(min < limit);

            return CreateInts(min, limit).Select(n => (sbyte)n);
        }

        public static IEnumerable<bool> CreateBools() => CreateInts(0, 2).Select(i => i == 1);

        public static IEnumerable<short> CreateShorts(short min = short.MinValue, short limit = short.MaxValue)
        {
            Assert(min < limit);

            return CreateInts(min, limit).Select(n => (short)n);
        }

        public static IEnumerable<ushort> CreateUShorts(ushort min = ushort.MinValue, ushort limit = ushort.MaxValue)
        {
            Assert(min < limit);

            return CreateInts(min, limit).Select(n => (ushort)n);
        }

        public static IEnumerable<int> CreateInts(int min = int.MinValue, int limit = int.MaxValue)
        {
            Assert(min < limit);

            while (true)
            {
                int value;

                lock (m_rand)
                    value = m_rand.Next(min, limit);

                yield return value;
            }
        }

        public static IEnumerable<uint> CreateUInts(uint min = uint.MinValue, uint limit = uint.MaxValue)
        {
            Assert(min < limit);
            return CreateULongs(min, limit).Select(ul => (uint)ul);
        }

        public static IEnumerable<long> CreateLongs(long min = long.MinValue, long limit = long.MaxValue)
        {
            Assert(min < limit);

            long delta = Math.Abs(limit - min);

            while (true)
            {
                double d;

                lock (m_rand)
                    d = m_rand.NextDouble();

                yield return (long)(d * delta) + min;
            }
        }

        public static IEnumerable<ulong> CreateULongs(ulong min = ulong.MinValue, ulong limit = ulong.MaxValue)
        {
            Assert(min < limit);

            while (true)
            {
                double d;

                lock (m_rand)
                    d = m_rand.NextDouble();

                yield return (ulong)(d * (limit - min)) + min;
            }
        }

        //Surrogate pairs start at U+D800
        public static IEnumerable<char> CreateChars(char min = char.MinValue, char limit = '\ud800')
        {
            Assert(min < limit);

            return CreateUShorts(min, limit).Select(us => (char)us);
        }

        public static IEnumerable<string> CreateStrings(int maxLen = byte.MaxValue)
        {
            Assert(maxLen >= 0);

            if (maxLen == 0)
                while (true)
                    yield return string.Empty;


            while (true)
            {
                int len = CreateInts(0, maxLen).First();
                char[] chars = CreateChars().Take(len).ToArray();

                yield return new string(chars);
            }
        }

        public static IEnumerable<decimal> CreateDecimals(decimal min = decimal.MinValue / 10, decimal limit = decimal.MaxValue / 10)
        {
            Assert(min < limit);

            var bigMin = new BigInteger(min);
            var bigLim = new BigInteger(limit);

            if ((BigInteger)decimal.MaxValue < BigInteger.Abs((bigLim - bigMin)))
                return RunAsBI(bigMin, bigLim).Where(m => min <= m && m < limit);

            return RunAsDecimal(min, limit);

            //----------------------
            IEnumerable<decimal> RunAsDecimal(decimal minVal, decimal limVal)
            {

                decimal delta = Math.Abs(limVal - minVal);

                while (true)
                {
                    decimal d;

                    lock (m_rand)
                        d = (decimal)m_rand.NextDouble();

                    decimal m = d * delta + minVal;

                    while (m < minVal || limVal <= m)
                    {
                        lock (m_rand)
                            d = (decimal)m_rand.NextDouble();

                        m = d * delta + minVal;
                    }

                    yield return m;
                }
            }


            IEnumerable<decimal> RunAsBI(BigInteger minVal, BigInteger limVal)
            {

                BigInteger diff = BigInteger.Abs(limVal - minVal);

                while (true)
                {
                    BigInteger bi;

                    lock (m_rand)
                        bi = (BigInteger)m_rand.NextDouble();

                    BigInteger val = bi * diff + minVal;

                    while (val < minVal || limVal <= val)
                    {
                        lock (m_rand)
                            bi = (BigInteger)m_rand.NextDouble();

                        val = bi * diff + minVal;
                    }

                    yield return (decimal)val;
                }
            }
        }

        public static IEnumerable<float> CreateFloats(float min = float.MinValue, float limit = float.MaxValue)
        {
            Assert(!float.IsNaN(min));
            Assert(!float.IsInfinity(min));
            Assert(!float.IsNaN(limit));
            Assert(!float.IsInfinity(limit));
            Assert(min < limit);

            return CreateDoubles(min, limit).Select(d => (float)d).Where(f => f < limit && min <= f);
        }

        public static IEnumerable<double> CreateDoubles(double min = double.MinValue, double limit = double.MaxValue)
        {
            Assert(!double.IsNaN(min));
            Assert(!double.IsInfinity(min));
            Assert(!double.IsNaN(limit));
            Assert(!double.IsInfinity(limit));
            Assert(min < limit);



            while (true)
            {
                double value;

                lock (m_rand)
                    value = m_rand.NextDouble();

                yield return value * (limit - min) + min;
            }
        }
    }
}
