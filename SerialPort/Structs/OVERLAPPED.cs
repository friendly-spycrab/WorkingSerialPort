using System;
using System.Collections.Generic;
using System.Text;

namespace SerialPort.Structs
{
    public struct OVERLAPPED
    {
        public UIntPtr Internal;
        public UIntPtr InternalHigh;
        public uint Offset;
        public uint OffsetHigh;
        public IntPtr EventHandle;
    }

}
