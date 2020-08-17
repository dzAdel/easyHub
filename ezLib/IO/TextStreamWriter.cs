using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easyLib.IO
{
    public interface ITextStreamWriter
    {
        void Write(char c);
        void Write(char[] chars);

        void Write(string s);
        void WriteLine(string s);
    }
}
