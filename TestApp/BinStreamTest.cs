﻿using easyLib.IO;
using easyLib.Test;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestApp
{
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
        }

        //private:
        void LittleEndianTest()
        {
            try
            {
                Setup(out TestData sample);

                var writer = new BinStreamWriter(m_stream, ByteOrder.LittleEndian);
                Write(writer, sample);

                m_stream.Position = 0;

                var reader = new BinStreamReader(m_stream, ByteOrder.LittleEndian);
                Verify(reader, sample);
            }
            finally
            {
                Clean();
            }
        }

        void BigEndianTest()
        {
            try
            {
                Setup(out TestData sample);

                var writer = new BinStreamWriter(m_stream, ByteOrder.BigEndian);
                Write(writer, sample);

                m_stream.Position = 0;

                var reader = new BinStreamReader(m_stream, ByteOrder.BigEndian);
                Verify(reader, sample);
            }
            finally
            {
                Clean();
            }
        }

        void MixedByteOrderTest()
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
                Verify(reader, sample);

                reader.ByteOrder = ByteOrder.LittleEndian;
                Verify(reader, sample);
            }
            finally
            {
                Clean();
            }
        }

        void Clean() => m_stream?.Dispose();

        void Setup(out TestData data)
        {
            m_stream = new MemoryStream();

            IEnumerable<int> seqLen = SampleFactory.CreateInts(0, ushort.MaxValue);

            data.BoolValue = SampleFactory.CreateBools().First();
            data.Buffer = SampleFactory.CreateBytes().Take(seqLen.First()).ToArray();
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

        void Verify(IBinStreamReader reader, in TestData sample)
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

            Ensure(data.Equals(sample));
        }


    }
}