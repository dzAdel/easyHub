using System;
using static easyLib.DebugHelper;


namespace easyLib.IO
{
    public enum ByteOrder : byte
    {
        System,
        LittleEndian,
        BigEndian,
        Network = BigEndian
    }
    //------------------------------------

    public static class ByteOrderExtenssion
    {
        public static ByteOrder Normalize(this ByteOrder endianess)
        {
            Assert(Enum.IsDefined(typeof(ByteOrder), endianess));

            return endianess == ByteOrder.System ? 
                (BitConverter.IsLittleEndian ? ByteOrder.LittleEndian : ByteOrder.BigEndian) : endianess;
        }
    }
}
