using System;
using System.Collections.Generic;
using System.Text;

namespace SerialPort.Events
{
    public delegate void OutputReceivedEventHandler(object sender,OutputReceivedArgs EventArgs);

    public class OutputReceivedArgs : EventArgs
    {
        public byte[] data;

    }
}
