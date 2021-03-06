﻿using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using SerialPort.Structs;
using SerialPort.Enums;
using SerialPort.Events;
using System.Collections.Generic;

namespace SerialPort
{
    public class WorkingSerialPort
    {
        public event OutputReceivedEventHandler OutputReceived;

        #region Properties
        public bool IsOpen {  get; private set; }
        public string Port {  get;  set; }
        public uint BaudRate { get; set; } = 9600;
        public byte ByteSize { get; set; } = 8;
        public Parity ParityType { get; set; } = Parity.NOPARITY;
        public StopBits StopBitType { get; set; } = StopBits.ONE5STOPBITS;
        #endregion

        private Regex IsValidPortName = new Regex("^COM[0-9]{1,3}$", RegexOptions.Compiled);

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

        [DllImport("kernel32.dll")]
        static extern bool SetCommMask(IntPtr hFile,uint dwEvtMask);

        [DllImport("kernel32.dll")]
        static extern bool WaitCommEvent(IntPtr hFile, ulong lpEvtMask, OVERLAPPED lpOverlapped);


        //Gives generic read access
        const uint GENERIC_READ = 0x80000000;

        //Gives generic write access
        const uint GENERIC_WRITE = 0x40000000;

        //Constant for opening an existing I/O device
        const uint OPEN_EXISTING = 3;

        const uint FILE_FLAG_OVERLAPPED = 0x40000000;

        const uint EV_RXCHAR = 0x0001;

        const uint EV_TXEMPTY = 0x0004;

        const ulong INFINITE = 0xffffffffL;
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

            SetCommMask(PortHandle,EV_RXCHAR|EV_TXEMPTY);

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
            ulong NumberOfBytesWritten;

            if (!IsOpen)
                return 0;

            OVERLAPPED Overlapped = new OVERLAPPED();

            WriteFile(PortHandle,Data,(uint)Data.Length,out NumberOfBytesWritten,ref Overlapped);

            return NumberOfBytesWritten;
        }

        /// <summary>
        /// Reads data from the serial port
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="NumberOfBytesToRead"></param>
        /// <returns></returns>
        public ulong Read(byte[] buffer,uint NumberOfBytesToRead)
        {
            ulong NumberOfBytesRead;

            if (!IsOpen)
                return 0;

            OVERLAPPED Overlapped = new OVERLAPPED();

            ReadFile(PortHandle, buffer, NumberOfBytesToRead, out NumberOfBytesRead, ref Overlapped);

            return NumberOfBytesRead;
        }

        private void CheckForOutputLoop()
        {
            while (IsOpen)
            {
                if(WaitCommEvent(PortHandle,EV_RXCHAR|EV_TXEMPTY,new OVERLAPPED() { EventHandle = (IntPtr)INFINITE}))
                {
                    byte[] Buffer = new byte[256];
                    ulong NumberOfBytesRead = Read(Buffer, (uint)256);
                    if (NumberOfBytesRead > 0)
                    {
                        List<byte> ByteList = new List<byte>(Buffer);
                        OnOutputReceived(ByteList.GetRange(0, (int)NumberOfBytesRead).ToArray());
                    }

                }
            }

        }

        protected virtual void OnOutputReceived(byte[] output)
        {
            OutputReceived?.Invoke(this,new OutputReceivedArgs() { data = output });

        }

        

    }


}
