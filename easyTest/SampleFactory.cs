using System;
using System.Collections.Generic;
using System.Linq;

namespace easyTest
{
    public static class SampleFactory
    {

        public static IEnumerable<int> CreateInts(int min = int.MinValue, int max = int.MaxValue)
        {
            var rand = new Random();
            while (true)
                yield return rand.Next(min, max);
        }

        public static IEnumerable<short> CreateShorts(short min = short.MinValue,
            short max = short.MaxValue)
        {
            var seq = from i in CreateInts(min, max)
                      select (short)i;

            return seq;
        }

        public static IEnumerable<ushort> CreateUShorts(ushort min = ushort.MinValue,
            ushort max = ushort.MaxValue)
        {
            var seq = from i in CreateInts(min, max)
                      select (ushort)i;

            return seq;
        }

        public static IEnumerable<uint> CreateUInts(uint min = uint.MinValue,
            uint max = uint.MaxValue)
        {
            foreach (int i in CreateInts())
            {
                var ui = (uint)i;

                if (ui <= max && min <= ui)
                    yield return ui;
            }
        }

        public static IEnumerable<long> CreateLongs(long min = long.MinValue,
            long max = long.MaxValue)
        {
            var rand = new Random();
            var buffer = new byte[sizeof(long)];

            while (true)
            {
                rand.NextBytes(buffer);
                long l = BitConverter.ToInt64(buffer, 0);

                if (min <= l && l <= max)
                    yield return l;
            }
        }

        public static IEnumerable<ulong> CreateULongs(ulong min = ulong.MinValue,
            ulong max = ulong.MaxValue)
        {
            foreach(long l in CreateLongs())
            {
                var ul = (ulong)l;

                if (min <= ul && ul <= max)
                    yield return ul;
            }
        }
    }
}
