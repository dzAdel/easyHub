using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    partial class BinStreamTest
    {
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
        struct TestData
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
        {
            public byte[] Buffer;
            public byte ByteValue;
            public sbyte SByteValue;
            public bool BoolValue;
            public char CharValue;
            public short ShortValue;
            public ushort UShortValue;
            public int IntValue;
            public uint UIntValue;
            public long LongValue;
            public ulong ULongValue;
            public float FloatValue;
            public double DoubleValue;
            public decimal DecimalValue;
            public string StringValue;
            public DateTime TimeValue;

            public override bool Equals(object obj)
            {
                var td = (TestData)obj;

                if (Buffer.Length != td.Buffer.Length)
                    return false;

                for (int i = 0; i < Buffer.Length; ++i)
                    if (Buffer[i] != td.Buffer[i])
                        return false;

                return ByteValue == td.ByteValue &&
                    SByteValue == td.SByteValue &&
                    BoolValue == td.BoolValue &&
                    CharValue == td.CharValue &&
                    ShortValue == td.ShortValue &&
                    UShortValue == td.UShortValue &&
                    IntValue == td.IntValue &&
                    UIntValue == td.UIntValue &&
                    LongValue == td.LongValue &&
                    ULongValue == td.ULongValue &&
                    FloatValue == td.FloatValue &&
                    DoubleValue == td.DoubleValue &&
                    DecimalValue == td.DecimalValue &&
                    StringValue == td.StringValue &&
                    TimeValue == td.TimeValue;
            }
        }
    }
}
