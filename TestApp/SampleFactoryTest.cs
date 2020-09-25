using easyLib.Test;
using System;
using System.Linq;

namespace TestApp
{
    class SampleFactoryTest : UnitTest
    {
        public SampleFactoryTest() :
            base("SampleFactory Test")
        { }


        //protected:
        protected override void Start()
        {
            CreateBytesTest();
            CreateIntsTest();
            CreateSByteTest();
            CreateBoolsTest();
            CreateShortsTest();
            CreateUShortsTest();
            CreateULongsTest();
            CreateUIntsTest();
            CreateLongsTest();
            CreateCharsTest();
            CreateStringsTest();
            CreateDecimalsTest();
            CreateDoublesTest();
            CreateFloatsTest();
        }

        //private:
        void CreateFloatsTest()
        {
            //Assume CreateBytesTest(), CreateDoublesTest() ok
            float min = BitConverter.ToSingle(SampleFactory.CreateBytes().Take(sizeof(float)).ToArray());

            while (float.IsNaN(min) || float.IsInfinity(min))
                min = BitConverter.ToSingle(SampleFactory.CreateBytes().Take(sizeof(float)).ToArray());


            float lim = BitConverter.ToSingle(SampleFactory.CreateBytes().Take(sizeof(float)).ToArray());

            while (float.IsNaN(lim) || float.IsInfinity(lim))
                lim = BitConverter.ToSingle(SampleFactory.CreateBytes().Take(sizeof(float)).ToArray());


            const float epsilon = 1e-22f;

            if (Math.Abs(lim - min) < epsilon)
                min /= 2;

            if (lim < min)
                (min, lim) = (lim, min);


            Ensure(SampleFactory.CreateFloats(min, lim).
                Take(RandByte).
                All(d => min <= d & d < lim));
        }

        void CreateDoublesTest()
        {
            //Assume CreateBytesTest() ok
            double min = BitConverter.ToDouble(SampleFactory.CreateBytes().Take(sizeof(double)).ToArray());

            while (double.IsNaN(min) || double.IsInfinity(min))
                min = BitConverter.ToDouble(SampleFactory.CreateBytes().Take(sizeof(double)).ToArray());


            double lim = BitConverter.ToDouble(SampleFactory.CreateBytes().Take(sizeof(double)).ToArray());

            while (double.IsNaN(lim) || double.IsInfinity(lim))
                lim = BitConverter.ToDouble(SampleFactory.CreateBytes().Take(sizeof(double)).ToArray());


            const double epsilon = 1e-162;


            if (Math.Abs(lim - min) < epsilon)
                min /= 2;

            if (lim < min)
                (min, lim) = (lim, min);


            Ensure(SampleFactory.CreateDoubles(min, lim).
                Take(RandByte).
                All(d => min <= d & d < lim));
        }

        void CreateDecimalsTest()
        {
            //assume CreateBytesTest(), CreateIntsTest(), CreateBoolsTest() ok
            int lo = SampleFactory.CreateInts().First();
            int mid = SampleFactory.CreateInts().First();
            int hi = SampleFactory.CreateInts().First();
            byte sf = SampleFactory.CreateBytes(limit: 29).First();
            bool isNeg = SampleFactory.CreateBools().First();
            var lim = new Decimal(lo, mid, hi, isNeg, sf);

            lo = SampleFactory.CreateInts().First();
            mid = SampleFactory.CreateInts().First();
            hi = SampleFactory.CreateInts().First();
            isNeg = SampleFactory.CreateBools().First();
            sf = SampleFactory.CreateBytes(limit: 29).First();
            var min = new Decimal(lo, mid, hi, isNeg, sf);

            if (lim == min)
                min /= 2;

            if (lim < min)
                (min, lim) = (lim, min);


            Ensure(SampleFactory.CreateDecimals(min, lim).
                Take(RandByte).
                All(m => min <= m && m < lim));
        }

        void CreateStringsTest()
        {
            //Assume CreateBytesTest(), CreateCharsTest() ok
            var len = RandByte;

            Ensure(SampleFactory.CreateStrings(len).
                Take(RandByte).
                All(str => str.Length <= len));

            Ensure(SampleFactory.CreateStrings(0).
                Take(RandByte).
                All(str => str.Length == 0));

        }
       
        void CreateCharsTest()
        {
            //assume CreateBytesTest(), CreateUShortsTest() ok
            var min = (char)SampleFactory.CreateUShorts().First();
            var max = (char)SampleFactory.CreateUShorts().First();

            if (max < min)
                (min, max) = (max, min);

            Ensure(SampleFactory.CreateChars(min, (char)(max + 1)).
                Take(RandByte).
                All(c => min <= c && c <= max));

            Ensure(SampleFactory.CreateChars(char.MinValue, (char)(char.MinValue + 1)).
                Take(RandByte).
                All(c => c == char.MinValue));

            Ensure(SampleFactory.CreateChars((char)(char.MaxValue - 1), char.MaxValue).
                Take(RandByte).
                All(c => c == (char)(char.MaxValue - 1)));
        }

        void CreateLongsTest()
        {
            //Assume CreateBytesTest() ok
            long min = BitConverter.ToInt64(SampleFactory.CreateBytes().
                Take(sizeof(long)).
                ToArray());

            long max = BitConverter.ToInt64(SampleFactory.CreateBytes().
                Take(sizeof(long)).
                ToArray());

            if (max < min)
                (min, max) = (max, min);


            Ensure(SampleFactory.CreateLongs(min, max + 1).
                Take(RandByte).
                All(l => min <= l && l <= max));



            Ensure(SampleFactory.CreateLongs(long.MinValue, long.MinValue + 1).
                Take(RandByte).
                All(l => l == long.MinValue));

            Ensure(SampleFactory.CreateLongs(long.MaxValue - 1, long.MaxValue).
                Take(RandByte).
                All(l => l == long.MaxValue - 1));
        }

        void CreateBoolsTest()
        {
            //assume CreateIntsTest() ok

            //if random 255 bools all have the same value => the algorith is bad.
            Ensure(SampleFactory.CreateBools().Take(byte.MaxValue).Any(b => b == false));
            Ensure(SampleFactory.CreateBools().Take(byte.MaxValue).Any(b => b == true));
        }

        void CreateSByteTest()
        {
            //assume CreateIntsTest() ok

            var intSeq = SampleFactory.CreateInts(0, byte.MaxValue + 1);
            sbyte min = (sbyte)intSeq.Where(n => n < sbyte.MaxValue && sbyte.MinValue <= n).First();
            sbyte max = (sbyte)intSeq.Where(n => n < sbyte.MaxValue && sbyte.MinValue <= n).First();

            if (max < min)
                (min, max) = (max, min);

            SampleFactory.CreateSBytes(min, (sbyte)(max + 1)).
                Take(RandByte).
                All(sb => Ensure(min <= sb && sb <= max));

            SampleFactory.CreateSBytes(sbyte.MinValue, sbyte.MinValue + 1).
                Take(RandByte).
                All(sb => Ensure(sb == sbyte.MinValue));

            SampleFactory.CreateSBytes(sbyte.MaxValue - 1, sbyte.MaxValue).
                Take(RandByte).
                All(sb => Ensure(sb == sbyte.MaxValue - 1));

        }

        void CreateBytesTest()
        {
            var rand = new Random();
            byte min = (byte)rand.Next(byte.MaxValue);
            byte max = (byte)rand.Next(byte.MaxValue);

            if (max < min)
                (min, max) = (max, min);

            SampleFactory.CreateBytes(min, (byte)(max + 1)).
                Take(rand.Next(byte.MaxValue)).
                All(n => Ensure(min <= n && n <= max));

            SampleFactory.CreateBytes(byte.MinValue, byte.MinValue + 1).
                Take(rand.Next(byte.MaxValue)).
                All(n => Ensure(n == byte.MinValue));

            SampleFactory.CreateBytes(byte.MaxValue - 1, byte.MaxValue).
                Take(rand.Next(byte.MaxValue)).
                All(n => Ensure(n == byte.MaxValue - 1));
        }

        void CreateULongsTest()
        {
            //Assume CreateBytesTest() ok

            var bytes = SampleFactory.CreateBytes().Take(sizeof(ulong)).ToArray();
            ulong min = BitConverter.ToUInt64(bytes, 0);

            if (min == ulong.MaxValue)
                --min;


            bytes = SampleFactory.CreateBytes().Take(sizeof(ulong)).ToArray();
            ulong max = BitConverter.ToUInt64(bytes, 0);

            if (max == ulong.MaxValue)
                --max;

            if (max < min)
                (min, max) = (max, min);

            SampleFactory.CreateULongs(min, max + 1).
                Take(RandByte).
                All(ul => Ensure(min <= ul && ul <= max));

            SampleFactory.CreateULongs(ulong.MinValue, ulong.MinValue + 1).
                Take(RandByte).
                All(ul => Ensure(ul == ulong.MinValue));

            SampleFactory.CreateULongs(ulong.MaxValue - 1, ulong.MaxValue).
                Take(RandByte).
                All(ul => Ensure(ul == ulong.MaxValue - 1));
        }

        void CreateIntsTest()
        {
            //Assume CreateBytesTest() ok
            var rand = new Random();
            int min = rand.Next(int.MinValue, int.MaxValue);
            int max = rand.Next(int.MinValue, int.MaxValue);

            if (max < min)
                (min, max) = (max, min);

            SampleFactory.CreateInts(min, max + 1).
                Take(RandByte).
                All(n => Ensure(min <= n && n <= max));

            SampleFactory.CreateInts(int.MinValue, int.MinValue + 1).
                Take(RandByte).
                All(n => Ensure(n == int.MinValue));

            SampleFactory.CreateInts(int.MaxValue - 1, int.MaxValue).
                Take(RandByte).
                All(n => Ensure(n == int.MaxValue - 1));
        }

        void CreateShortsTest()
        {
            //assume CreateBytesTest() ok
            //assume CreateIntsTest() ok
            var intSeq = SampleFactory.CreateInts(short.MinValue, short.MaxValue);
            short min = (short)intSeq.First();
            short max = (short)intSeq.First();

            if (max < min)
                (min, max) = (max, min);

            SampleFactory.CreateShorts(min, (short)(max + 1)).
                Take(RandByte).
                All(sh => Ensure(min <= sh && sh <= max));

            SampleFactory.CreateShorts(short.MinValue, short.MinValue + 1).
                Take(RandByte).
                All(sh => Ensure(sh == short.MinValue));

            SampleFactory.CreateShorts(short.MaxValue - 1, short.MaxValue).
                Take(RandByte).
                All(sh => Ensure(sh == short.MaxValue - 1));
        }

        void CreateUShortsTest()
        {
            //Assume CreateIntsTest() ok
            //Assume CreateBytesTest() ok

            var intSeq = SampleFactory.CreateInts(ushort.MinValue, ushort.MaxValue);
            ushort min = (ushort)intSeq.First();
            ushort max = (ushort)intSeq.First();

            if (max < min)
                (min, max) = (max, min);

            SampleFactory.CreateUShorts(min, (ushort)(max + 1)).
                Take(RandByte).
                All(ush => Ensure(min <= ush && ush <= max));

            SampleFactory.CreateUShorts(ushort.MinValue, ushort.MinValue + 1).
                Take(RandByte).
                All(ush => Ensure(ush == ushort.MinValue));

            SampleFactory.CreateUShorts(ushort.MaxValue - 1, ushort.MaxValue).
                Take(RandByte).
                All(ush => Ensure(ush == ushort.MaxValue - 1));
        }

        void CreateUIntsTest()
        {
            //assume CreateULongsTest(), CreateBytesTest() ok
            var min = (uint)SampleFactory.CreateULongs(0, uint.MaxValue).First();
            var max = (uint)SampleFactory.CreateULongs(0, uint.MaxValue).First();

            if (max < min)
                (min, max) = (max, min);

            Ensure(SampleFactory.CreateUInts(min, max + 1).
                Take(RandByte).
                All(ui => ui <= max && min <= ui));

            Ensure(SampleFactory.CreateUInts(uint.MinValue, uint.MinValue + 1).
                Take(RandByte).
                All(ui => ui == uint.MinValue));

            Ensure(SampleFactory.CreateUInts(uint.MaxValue - 1, uint.MaxValue).
                Take(RandByte).
                All(ui => ui == uint.MaxValue - 1));
        }

        //private:
        static byte RandByte => SampleFactory.CreateBytes().First(); //Assume CreateBytesTest() ok
    }
}
