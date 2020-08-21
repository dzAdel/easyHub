using System;
using System.Collections.Generic;
using System.Linq;
using static easyLib.DebugHelper;


namespace easyLib.Test
{
    public static class SampleFactory
    {

        public static IEnumerable<int> CreateInts(int min = int.MinValue, int limit = int.MaxValue)
        {
            var rand = new Random();
            while (true)
                yield return rand.Next(min, limit);
        }

        public static IEnumerable<short> CreateShorts(short min = short.MinValue,
            short limit = short.MaxValue)
        {
            var seq = from i in CreateInts(min, limit)
                      select (short)i;

            return seq;
        }

        public static IEnumerable<ushort> CreateUShorts(ushort min = ushort.MinValue,
            ushort limit = ushort.MaxValue)
        {
            var seq = from i in CreateInts(min, limit)
                      select (ushort)i;

            return seq;
        }

        public static IEnumerable<uint> CreateUInts(uint min = uint.MinValue,
            uint limit = uint.MaxValue)
        {
            foreach (int i in CreateInts())
            {
                var ui = (uint)i;

                if (ui < limit && min <= ui)
                    yield return ui;
            }
        }

        public static IEnumerable<long> CreateLongs(long min = long.MinValue,
            long limit = long.MaxValue)
        {
            var rand = new Random();
            var buffer = new byte[sizeof(long)];

            while (true)
            {
                rand.NextBytes(buffer);
                long l = BitConverter.ToInt64(buffer, 0);

                if (min <= l && l < limit)
                    yield return l;
            }
        }

        public static IEnumerable<ulong> CreateULongs(ulong min = ulong.MinValue,
            ulong limit = ulong.MaxValue)
        {
            foreach (long l in CreateLongs())
            {
                var ul = (ulong)l;

                if (min <= ul && ul < limit)
                    yield return ul;
            }
        }

        //Surrogate pairs start at U+D800
        public static IEnumerable<char> CreateChars(char min = char.MinValue, 
            char limit = '\ud800') =>  CreateUShorts(min, limit).Select(us => (char)us);
        
        public static IEnumerable<string> CreateStrings(int maxLen = byte.MaxValue)
        {
            Assert(maxLen >= 0);

            IEnumerable<char> charGen = CreateChars();
            IEnumerable<int> lenGen = CreateInts(0, maxLen);

            while(true)
            {
                int len = lenGen.First();
                char[] chars = charGen.Take(len).ToArray();

                yield return new string(chars);
            }
        }

        public static IEnumerable<byte> CreateBytes(byte min = byte.MinValue, 
            byte limit = byte.MaxValue)
        {
            var rand = new Random();
            const int SZ_BUFFER = 16;
            var buffer = new byte[SZ_BUFFER];

            while(true)
            {
                rand.NextBytes(buffer);

                foreach (byte b in buffer)
                    if (min <= b && b < limit)
                        yield return b;
            }
        }

        public static IEnumerable<sbyte> CreateSBytes(sbyte min = sbyte.MinValue,
            sbyte limit = sbyte.MaxValue)
        {
            foreach(byte b in CreateBytes())
            {
                var sb = (sbyte)b;

                if (min <= sb && sb < limit)
                    yield return sb;
            }    
        }

        public static IEnumerable<bool> CreateBools() => CreateInts().Select(i => i >= 0);

        public static IEnumerable<decimal> CreateDecimals(decimal min = decimal.MinValue,
            decimal limit = decimal.MaxValue)
        {
            IEnumerable<int> seqInts = CreateInts();
            IEnumerable<int> seqScaleFactor = CreateInts(0, 28);
            IEnumerable<bool> seqSign = CreateBools();

            while(true)
            {
                int[] ints = seqInts.Take(4).ToArray();
                int sf = seqScaleFactor.First();
                bool isNeg = seqSign.First();
                ints[3] = sf << 16;

                if(isNeg)
                    ints[3] |= 1 << 31;

                var d = new decimal(ints);

                if (min <= d && d < limit)
                    yield return d;
            }
        }

        public static IEnumerable<double> CreateDoubles(double min = double.MinValue,
            double limit = double.MaxValue)
        {
            var rand = new Random();

            while(true)
                yield return rand.NextDouble() * (limit - min) + min;
        }

        public static IEnumerable<float> CreateFloats(float min = float.MinValue,
            float limit = float.MaxValue) => CreateDoubles(min, limit).Select(d => (float)d);

    }
}
