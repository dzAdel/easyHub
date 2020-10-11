using easyLib.IO;
using easyLib.Test;
using System;
using System.IO;
using System.Linq;

namespace TestApp
{
    //Assume SampleFactoryTest, MultiByteCodecTest ok
    sealed partial class BinStreamTest : UnitTest
    {
        Stream m_stream;

        public BinStreamTest() :
            base("BinStreamReader/BinStreamWriter Test")
        { }


        //protected:
        protected override void Start()
        {
            LittleEndianTest();
            BigEndianTest();
            MixedByteOrderTest();
            SeekableStreamTest();
        }


        //private:
        void SeekableStreamTest()
        {
            Ensure(SeekTest());

            bool SeekTest()
            {
                try
                {
                    Setup(out TestData sample);
                    var writer = new SeekableStreamWriter(m_stream, ByteOrder.LittleEndian);
                    writer.Write(SampleFactory.CreateBytes().Take(RandNber()).ToArray());
                    long pos = writer.Position;
                    Write(writer, sample);
                                       

                    var reader = new SeekableStreamReader(m_stream, ByteOrder.LittleEndian);
                    reader.Position = pos;
                    return Verify(reader, sample);
                }
                finally
                {
                    Clean();
                }
            }

            byte RandNber() => SampleFactory.CreateBytes().First();
        }
      
        void LittleEndianTest()
        {
            Func<bool> tst = () =>
                {
                    try
                    {
                        Setup(out TestData sample);

                        var writer = new BinStreamWriter(m_stream, ByteOrder.LittleEndian);
                        Write(writer, sample);

                        m_stream.Position = 0;

                        var reader = new BinStreamReader(m_stream, ByteOrder.LittleEndian);
                        return Verify(reader, sample);
                    }
                    finally
                    {
                        Clean();
                    }
                };


            Ensure(tst());
        }

        void BigEndianTest()
        {
            Func<bool> tst = () =>
            {
                try
                {
                    Setup(out TestData sample);

                    var writer = new BinStreamWriter(m_stream, ByteOrder.BigEndian);
                    Write(writer, sample);

                    m_stream.Position = 0;

                    var reader = new BinStreamReader(m_stream, ByteOrder.BigEndian);
                    return Verify(reader, sample);
                }
                finally
                {
                    Clean();
                }
            };

            Ensure(tst());
        }

        void MixedByteOrderTest()
        {
            Func<bool> tst = () =>
            {
                try
                {
                    Setup(out TestData sample);

                    var writer = new BinStreamWriter(m_stream, ByteOrder.BigEndian);
                    Write(writer, sample);

                    writer.ByteOrder = ByteOrder.LittleEndian;
                    Write(writer, sample);

                    m_stream.Position = 0;

                    var reader = new BinStreamReader(m_stream, ByteOrder.BigEndian);
                    return Verify(reader, sample);
                }
                finally
                {
                    Clean();
                }
            };

            Ensure(tst());
        }

        void Clean() => m_stream?.Dispose();

        void Setup(out TestData data)
        {
            m_stream = new MemoryStream();

            data.BoolValue = SampleFactory.CreateBools().First();
            data.Buffer = SampleFactory.CreateBytes().Take(SampleFactory.CreateInts(0, ushort.MaxValue).First()).ToArray();
            data.ByteValue = SampleFactory.CreateBytes().First();
            data.CharValue = SampleFactory.CreateChars().First();
            data.DecimalValue = SampleFactory.CreateDecimals().First();
            data.DoubleValue = SampleFactory.CreateDoubles().First();
            data.FloatValue = SampleFactory.CreateFloats().First();
            data.IntValue = SampleFactory.CreateInts().First();
            data.LongValue = SampleFactory.CreateLongs().First();
            data.SByteValue = SampleFactory.CreateSBytes().First();
            data.ShortValue = SampleFactory.CreateShorts().First();
            data.StringValue = SampleFactory.CreateStrings().First();
            data.TimeValue = DateTime.Now;
            data.UIntValue = SampleFactory.CreateUInts().First();
            data.ULongValue = SampleFactory.CreateULongs().First();
            data.UShortValue = SampleFactory.CreateUShorts().First();
        }

        void Write(IBinStreamWriter writer, in TestData data)
        {
            writer.Write(data.BoolValue);
            writer.Write(data.Buffer);
            writer.Write(data.ByteValue);
            writer.Write(data.CharValue);
            writer.Write(data.DecimalValue);
            writer.Write(data.DoubleValue);
            writer.Write(data.FloatValue);
            writer.Write(data.IntValue);
            writer.Write(data.LongValue);
            writer.Write(data.SByteValue);
            writer.Write(data.ShortValue);
            writer.Write(data.StringValue);
            writer.Write(data.TimeValue);
            writer.Write(data.UIntValue);
            writer.Write(data.ULongValue);
            writer.Write(data.UShortValue);
        }

        bool Verify(IBinStreamReader reader, in TestData sample)
        {
            var data = new TestData();

            data.BoolValue = reader.ReadBool();
            data.Buffer = reader.ReadBytes(sample.Buffer.Length);
            data.ByteValue = reader.ReadByte();
            data.CharValue = reader.ReadChar();
            data.DecimalValue = reader.ReadDecimal();
            data.DoubleValue = reader.ReadDouble();
            data.FloatValue = reader.ReadFloat();
            data.IntValue = reader.ReadInt();
            data.LongValue = reader.ReadLong();
            data.SByteValue = reader.ReadSByte();
            data.ShortValue = reader.ReadShort();
            data.StringValue = reader.ReadString();
            data.TimeValue = reader.ReadTime();
            data.UIntValue = reader.ReadUInt();
            data.ULongValue = reader.ReadULong();
            data.UShortValue = reader.ReadUShort();

            return data.Equals(sample);
        }
    }
}
