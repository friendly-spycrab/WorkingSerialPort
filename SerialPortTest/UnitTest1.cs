using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using SerialPort;
using System.Text;

namespace SerialPortTest
{
    [TestClass]
    public class SerialPortTester
    {
        [TestMethod]
        public void TestPortSend()
        {
            WorkingSerialPort SP = new WorkingSerialPort();
            SP.Port = "COM1";
            SP.BaudRate = 115200;
            SP.OpenPort();

            while (true)
            {
                Thread.Sleep(500);
                SP.Write(Encoding.ASCII.GetBytes("Test"));
            }
        }

        [TestMethod]
        public void TestPortRecieve()
        {
            
        }
    }
}
