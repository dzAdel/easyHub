using easyLib;
using easyLib.Test;
using System.Linq;

namespace TestApp
{
    //Assume SampleFactoryTest ok
    
    class MultiByteCodecTest : UnitTest
    {
        public MultiByteCodecTest() :
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
            var seq = from short sh in SampleFactory.CreateShorts().Take(RandByte)
                      let bytes = MultiByteCodec.GetBytes(sh)
                      select (sh, bytes);

            Ensure(seq.All(item => MultiByteCodec.GetShort(item.bytes) == item.sh));
        }

        void MBUShortTest()
        {
            var seq = from ushort ush in SampleFactory.CreateUShorts().Take(RandByte)
                      let bytes = MultiByteCodec.GetBytes(ush)
                      select (ush, bytes);

            Ensure(seq.All(item => MultiByteCodec.GetUShort(item.bytes) == item.ush));
        }

        void MBIntTest()
        {
            var seq = from int n in SampleFactory.CreateInts().Take(RandByte)
                      let bytes = MultiByteCodec.GetBytes(n)
                      select (n, bytes);

            Ensure(seq.All(item => MultiByteCodec.GetInt(item.bytes) == item.n));
        }

        void MBUIntTest()
        {
            var seq = from uint ui in SampleFactory.CreateUInts().Take(RandByte)
                      let bytes = MultiByteCodec.GetBytes(ui)
                      select (ui, bytes);

            Ensure(seq.All(item => MultiByteCodec.GetUInt(item.bytes) == item.ui));
        }

        void MBLongTest()
        {
            var seq = from long l in SampleFactory.CreateLongs().Take(RandByte)
                      let bytes = MultiByteCodec.GetBytes(l)
                      select (l, bytes);

            Ensure(seq.All(item => MultiByteCodec.GetLong(item.bytes) == item.l));
        }

        void MBULongTest()
        {
            var seq = from ulong ul in SampleFactory.CreateULongs().Take(RandByte)
                      let bytes = MultiByteCodec.GetBytes(ul)
                      select (ul, bytes);

            Ensure(seq.All(item => MultiByteCodec.GetULong(item.bytes) == item.ul));
        }

        static byte RandByte => SampleFactory.CreateBytes().First();
    }
}
