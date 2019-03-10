using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.SerialCommunication;
using Windows.Devices.Enumeration;
using MissionPlanner.Comms;
using System.IO;
using System.IO.Ports;

namespace Xamarin.UWP
{
    public class Serial : ICommsSerial
    {

        public Serial()
        {
            var device = SerialDevice.GetDeviceSelector("com11");
        }
        public Stream BaseStream => throw new NotImplementedException();

        public int BaudRate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int BytesToRead => throw new NotImplementedException();

        public int BytesToWrite => throw new NotImplementedException();

        public int DataBits { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool DtrEnable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsOpen => throw new NotImplementedException();

        public Parity Parity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string PortName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ReadBufferSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ReadTimeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool RtsEnable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public StopBits StopBits { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int WriteBufferSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int WriteTimeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void DiscardInBuffer()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            throw new NotImplementedException();
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public int ReadByte()
        {
            throw new NotImplementedException();
        }

        public int ReadChar()
        {
            throw new NotImplementedException();
        }

        public string ReadExisting()
        {
            throw new NotImplementedException();
        }

        public string ReadLine()
        {
            throw new NotImplementedException();
        }

        public void toggleDTR()
        {
            throw new NotImplementedException();
        }

        public void Write(string text)
        {
            throw new NotImplementedException();
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(string text)
        {
            throw new NotImplementedException();
        }
    }
}
