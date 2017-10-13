using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using SerialPort.Structs;
using SerialPort.Enums;

namespace SerialPort
{
    public class WorkingSerialPort
    {

        #region Properties
        public bool IsOpen {  get; private set; }
        public string Port {  get;  set; }
        public uint BaudRate { get; set; } = 9600;
        public byte ByteSize { get; set; } = 8;
        public Parity ParityType { get; set; } = Parity.NOPARITY;
        public StopBits StopBitType { get; set; } = StopBits.ONE5STOPBITS;
        #endregion

        private Regex IsValidPortName = new Regex("COM([0-9][0-9][0-9]|[0-9][0-9]|[0-9])$",RegexOptions.Compiled);

        //The createfile is used for opening the serial port.
        [DllImport("kernel32")]
        private static extern IntPtr CreateFile(
                                            string lpFileName,
                                            uint dwDesiredAccess,
                                            uint dwShareMode,
                                            LPSECURITY_ATTRIBUTES lpSecurityAttributes,
                                            uint dwCreationDisposition,
                                            uint dwFlagsAndAttributes,
                                            IntPtr hTemplateFile);
        [DllImport("kernel32")]
        private static extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, out UInt64 lpNumberOfBytesWritten,ref OVERLAPPED lpOverlapped);

        [DllImport("kernel32")]
        private static extern bool ReadFile(IntPtr hFile,byte[] lpBuffer, uint nNumberOfBytesToRead, out UInt64 lpNumberOfBytesRead, ref OVERLAPPED lpOverlapped);

        [DllImport("kernel32")]
        private static extern bool SetCommState(IntPtr hFile,DCB lpDCB);

        [DllImport("kernel32.dll")]
        static extern uint GetLastError();

        //Gives generic read access
        const uint GENERIC_READ = 0x80000000;

        //Gives generic write access
        const uint GENERIC_WRITE = 0x40000000;

        //Constant for opening an existing I/O device
        const uint OPEN_EXISTING = 3;

        const uint FILE_FLAG_OVERLAPPED = 0x40000000;

        IntPtr PortHandle;


        /// <summary>
        /// Open the serial port
        /// </summary>
        /// <returns></returns>
        public bool OpenPort()
        {
            if (IsValidPortName.Matches(Port).Count != 1)
                throw new ArgumentException("Invalid port name.");

            PortHandle = CreateFile(Port, GENERIC_READ | GENERIC_WRITE,0,new LPSECURITY_ATTRIBUTES(),OPEN_EXISTING, FILE_FLAG_OVERLAPPED,IntPtr.Zero);

            if ((long)PortHandle == -1)
                return false;

            SetCommState(PortHandle, new DCB
            {
                BaudRate = BaudRate,
                fBinary = 1,
                fParity = Convert.ToUInt32(ParityType != Parity.NOPARITY),
                Parity = (byte)ParityType,
                byteSize = ByteSize,
                StopBits = (byte)StopBitType,
                 
            });

            IsOpen = true;
            return true;
        }

        /// <summary>
        /// Write the data to the serial port.
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public ulong Write(byte[] Data)
        {
            ulong NumberOfBytesWritten = 0;

            if (!IsOpen)
                return 0;

            OVERLAPPED Overlapped = new OVERLAPPED();

            WriteFile(PortHandle,Data,(uint)Data.Length,out NumberOfBytesWritten,ref Overlapped);

            return NumberOfBytesWritten;
        }

        public ulong Read(byte[] buffer,uint NumberOfBytesToRead)
        {
            ulong NumberOfBytesWritten = 0;

            if (!IsOpen)
                return 0;

            OVERLAPPED Overlapped = new OVERLAPPED();

            ReadFile(PortHandle, buffer, NumberOfBytesToRead, out NumberOfBytesWritten, ref Overlapped);

            return NumberOfBytesWritten;
        }

    }


}
