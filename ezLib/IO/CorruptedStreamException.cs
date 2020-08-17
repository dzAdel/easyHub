using System;
using System.IO;
using System.Runtime.Serialization;

namespace easyLib.IO
{
    public class CorruptedStreamException : IOException
    {
        const string DEFAULT_MSG = "Corrupted stream.";

        public CorruptedStreamException(string message = null, Exception innerException = null) :
            base(message ?? DEFAULT_MSG, innerException)
        { }

        public CorruptedStreamException(string message, int hresult) :
            base(message, hresult)
        { }



        //protected:
        protected CorruptedStreamException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        { }
    }
}
