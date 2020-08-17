using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easyLib.IO
{
    public interface ITextStreamReader: IEnumerable<string>
    {
        char ReadChar();
        char ReadChars(int count);
        string ReadString();
        string ReadLine();
    }
}
