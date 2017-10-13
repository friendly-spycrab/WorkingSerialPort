using System;
using System.Collections.Generic;
using System.Text;

namespace SerialPort.Structs
{
    public struct DCB
    {
        public uint DCBlength;
        public uint BaudRate;
        public uint fBinary  ;
        public uint fParity  ;
        public uint fOutxCtsFlow  ;
        public uint fOutxDsrFlow  ;
        public uint fDtrControl ;
        public uint fDsrSensitivity  ;
        public uint fTXContinueOnXoff  ;
        public uint fOutX  ;
        public uint fInX  ;
        public uint fErrorChar ;
        public uint fNull ;
        public uint fRtsControl  ;
        public uint fAbortOnError;
        public uint fDummy2  ;
        public ushort wReserved;
        public ushort XonLim;
        public ushort XoffLim;
        public byte byteSize;
        public byte Parity;
        public byte StopBits;
        public byte XonChar;
        public byte XoffChar;
        public byte ErrorChar;
        public byte EofChar;
        public byte EvtChar;
        public ushort wReserved1;
    }
}
