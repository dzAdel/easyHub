using easyLib;
using easyTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TestApp
{
    public class MultiByteCodecTest : UnitTest
    {
        public MultiByteCodecTest():
            base("MultiByteCodec Test")
        { }

        //protected:
        protected override void Start()
        {
            MBShortTest();
            MBUShortTest();
            MBIntTest();
            MBUIntTest();
            MBLongTest();
            MBULongTest();
        }


        //private:
        void MBShortTest()
        {
            short sh = SampleFactory.CreateShorts().First();

            byte[] bytes = MultiByteCodec.GetBytes(sh);
            short val = MultiByteCodec.GetShort(bytes);

            Ensure(val == sh);
        }

        void MBUShortTest()
        {
            ushort sample = SampleFactory.CreateUShorts().First();
            byte[] bytes = MultiByteCodec.GetBytes(sample);
            ushort us = MultiByteCodec.GetUShort(bytes);

            Ensure(sample == us);
        }

        void MBIntTest()
        {
            int sample = SampleFactory.CreateInts().First();
            byte[] bytes = MultiByteCodec.GetBytes(sample);
            int i = MultiByteCodec.GetInt(bytes);

            Ensure(sample == i);
        }

        void MBUIntTest()
        {
            uint sample = SampleFactory.CreateUInts().First();
            byte[] bytes = MultiByteCodec.GetBytes(sample);
            uint ui = MultiByteCodec.GetUInt(bytes);

            Ensure(ui == sample);
        }

        void MBLongTest()
        {
            long sample = SampleFactory.CreateLongs().First();
            byte[] bytes = MultiByteCodec.GetBytes(sample);
            long l = MultiByteCodec.GetLong(bytes);

            Ensure(l == sample);
        }

        void MBULongTest()
        {
            ulong sample = SampleFactory.CreateULongs().First();
            byte[] bytes = MultiByteCodec.GetBytes(sample);
            ulong ul = MultiByteCodec.GetULong(bytes);

            Ensure(ul == sample);
        }
    }
}
