using System;
using System.Collections.Generic;
using System.Text;

namespace SerialPort.Structs
{
    public struct LPSECURITY_ATTRIBUTES
    {
        int nLength;
        IntPtr lpSecurityDescriptor;
        bool bInheritHandle;
    }
}
