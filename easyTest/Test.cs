﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easyTest
{
    public interface ITest
    {
        string Name { get; }
        IEnumerable<ITestResult> Run();
    }
}