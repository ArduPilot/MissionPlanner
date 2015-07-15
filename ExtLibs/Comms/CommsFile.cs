﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.IO;

namespace MissionPlanner.Comms
{
    public class CommsFile : CommsBase, ICommsSerial
    {
        // Methods
        public void Close() { BaseStream.Close(); }
        public void DiscardInBuffer() { }

        public int bps { get; set; }
        int currentbps = 0;
        int bytecredit = 0;
        DateTime lastread = DateTime.MinValue;
        int lastsecond = 0;

        int step = 0;

        //void DiscardOutBuffer();
        public void Open(string filename)
        {
            bps = 10000;
            PortName = filename;
            BaseStream = File.OpenRead(PortName);
        }

        public void Open()
        {
            bps = 10000;
            BaseStream = File.OpenRead(PortName);
        }
        public int Read(byte[] buffer, int offset, int count)
        {
            if (!IsOpen)
                throw new EndOfStreamException("File not open");

            while (true) 
            {
                // check if we have credit and continue
                if (count < bytecredit)
                {
                    bytecredit -= count;
                    break;
                }

                // get the time taken since last read in seconds
                double LapsedSinceLastRead = (DateTime.Now - lastread).TotalSeconds;

                // escape if we are out of range
                if (LapsedSinceLastRead < 0 || LapsedSinceLastRead > 2)
                    break;

                // get our target bps for this time slice.
                int targetbps = (int)(bps * LapsedSinceLastRead) + bytecredit;

                // check if out target+count is less than our required bps
                if (count < targetbps)
                {
                    bytecredit = targetbps - count;
                    break;
                }

                System.Threading.Thread.Sleep(1);      
            }

            lastread = DateTime.Now;

            if (lastsecond != DateTime.Now.Second)
            {
                Console.WriteLine("CommsFile Read bps {0}", currentbps);
                currentbps = 0;
                lastsecond = DateTime.Now.Second;
            }
            currentbps += count;
            int ret = BaseStream.Read(buffer, offset, count);

            if (buffer[0] != 254 && offset == 0)
                return 0;
            if (buffer[0] == 254 && offset == 1)
                step = buffer[1] + 5 + 2; // + header + checksum

            step -= ret;

            // read the timestamp
            if (step == 0)
                BaseStream.Read(new byte[8], 0, 8);

            return ret;
        }
        //int Read(char[] buffer, int offset, int count);
        public int ReadByte() { return BaseStream.ReadByte(); }
        public int ReadChar() { return BaseStream.ReadByte(); }
        public string ReadExisting() { return ""; }
        public string ReadLine() { return ""; }
        //string ReadTo(string value);
        public void Write(string text) { }
        public void Write(byte[] buffer, int offset, int count) { }
        //void Write(char[] buffer, int offset, int count);
        public void WriteLine(string text) { }

        public void toggleDTR() { }

        // Properties
        public Stream BaseStream { get; private set; }
        public int BaudRate { get; set; }
        public int BytesToRead { get { if (!BaseStream.CanRead) return 0; return (int)(BaseStream.Length - BaseStream.Position); } }
        public int BytesToWrite { get; set; }
        public int DataBits  { get; set; }
        public bool DtrEnable { get; set; }
        public bool IsOpen { get { if (BaseStream != null && BaseStream.CanRead) { return BaseStream.Position < BaseStream.Length; } return false; } }

        public Parity Parity { get; set; }

        public string PortName { get; set; }
        public int ReadBufferSize { get; set; }
        public int ReadTimeout { get; set; }
        public bool RtsEnable { get; set; }
        public StopBits StopBits { get; set; }
        public int WriteBufferSize { get; set; }
        public int WriteTimeout { get; set; }
    }
}
