using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.Bluetooth;
using Android.Util;
using Java.Util;
using MissionPlanner.ArduPilot;
using MissionPlanner.Comms;

namespace Xamarin.Droid
{
    public class BTDevice : IBlueToothDevice
    {
        public async Task<List<DeviceInfo>> GetDeviceInfoList()
        {
            var result = new List<DeviceInfo>();
            // Get the local Bluetooth adapter
            var btAdapter = BluetoothAdapter.DefaultAdapter;

            if (btAdapter != null)
            {
                // Get a set of currently paired devices
                var pairedDevices = btAdapter.BondedDevices;

                // If there are paired devices, add each on to the ArrayAdapter
                if (pairedDevices.Count > 0)
                {
                    foreach (var device in pairedDevices)
                    {
                        Log.Info("MP", "{0} {1} {2} {3} {4}", device.Name, device.Address, device.Type, device.Class,
                            device.BondState);
                        if (device.Type == BluetoothDeviceType.Le)
                            continue;
                        result.Add(new DeviceInfo() {board = "BT_" + device.Name, hardwareid = device.Address});
                    }
                }
            }

            return result;
        }

        public async Task<ICommsSerial> GetBT(DeviceInfo first)
        {
            return new BTSerial(first);
        }
    }

    public class BTSerial: Stream, ICommsSerial
    {
        private readonly DeviceInfo _first;
        MemoryStream readbuffer = new MemoryStream(1024*10);
        private BluetoothSocket socket;

        public BTSerial(DeviceInfo first)
        {
            _first = first;
            PortName = first.name;
        }

        public override void Flush()
        {
            Thread.Sleep(1);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public int BaudRate { get; set; }
        public int BytesToRead { get; internal set; }

        public int BytesToWrite => 0;
        public int DataBits { get; set; } = 8;
        public bool DtrEnable { get; set; }
        public bool IsOpen
        {
            get
            {
                if (socket == null) return false;
                return socket.IsConnected;
            }
        }
        public string PortName
        {
            get => _first.name;
            set { }
        }

        public override int ReadTimeout { get; set; }

        public override int WriteTimeout { get; set; }
        public int ReadBufferSize { get; set; }
        public bool RtsEnable { get; set; }
        public int WriteBufferSize { get; set; }
        public void Close()
        {
            if (socket != null && socket.IsConnected)
                socket.Close();
        }

        public void DiscardInBuffer()
        {
            lock (readbuffer)
            {
                readbuffer.SetLength(0);
                readbuffer.Position = 0;
                BytesToRead = 0;
            }
        }

        public async void Open()
        {
            try
            {
                UUID BLUETOOTH_SPP = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");

                BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
                BluetoothDevice device = bluetoothAdapter.GetRemoteDevice(_first.hardwareid);

                socket = device.CreateRfcommSocketToServiceRecord(BLUETOOTH_SPP);
                await socket.ConnectAsync();

                Task.Run(async () =>
                {
                    byte[] buffer = new byte[1024];
                    while (socket.IsConnected)
                    {
                        var len = await socket.InputStream.ReadAsync(buffer);

                        lock(readbuffer)
                        {
                            if (readbuffer.Position == readbuffer.Length && readbuffer.Length > 0)
                            {
                                //clear it
                                readbuffer.SetLength(0);
                                // add data
                                readbuffer.Write(buffer, 0, len);
                                // set readback start point
                                readbuffer.Position = 0;
                                // update toread
                                BytesToRead += len;
                                Log.Debug("BTSerial", "BytesToRead1 " + BytesToRead);
                            }
                            else
                            {
                                var pos = readbuffer.Position;
                                // goto end
                                readbuffer.Seek(0, SeekOrigin.End);
                                //write
                                readbuffer.Write(buffer, 0, len);
                                // seek back to readpos
                                readbuffer.Seek(pos, SeekOrigin.Begin);
                                BytesToRead += len;
                                Log.Debug("BTSerial", "BytesToRead2 " + BytesToRead);
                            }
                        }
                    }
                });
            }
            catch
            {
                throw;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            var deadline = DateTime.Now.AddMilliseconds(ReadTimeout);
            while (BytesToRead < count && DateTime.Now < deadline)
            {
                Thread.Sleep(1);
            }

            if (ReadTimeout > 0 && BytesToRead == 0)
            {
                throw new TimeoutException("No data in serial buffer");
            }

            var read = Math.Min(count, BytesToRead);

            lock (readbuffer)
            {
                read = readbuffer.Read(buffer, offset, read);
                BytesToRead -= read;
            }

            return read;
        }

        public override int ReadByte()
        {
            var ans = new byte[] { 0 };
            var count = Read(ans, 0, 1);
            return count > 0 ? ans[0] : -1;
        }

        public int ReadChar()
        {
            return ReadByte();
        }

        public string ReadExisting()
        {
            StringBuilder build = new StringBuilder();
            for (int a = 0; a < BytesToRead; a++)
                build.Append((char) ReadByte());
            return build.ToString();
        }

        public string ReadLine()
        {
            var temp = new byte[4000];
            var count = 0;
            var timeout = 0;

            while (timeout <= 100)
            {
                if (!IsOpen) break;
                if (BytesToRead > 0)
                {
                    var letter = (byte)ReadByte();

                    temp[count] = letter;

                    if (letter == '\n') // normal line
                        break;

                    count++;
                    if (count == temp.Length)
                        break;
                    timeout = 0;
                }
                else
                {
                    timeout++;
                    Thread.Sleep(5);
                }
            }

            return Encoding.ASCII.GetString(temp, 0, count + 1);
        }

        public void Write(string text)
        {
            Write(ASCIIEncoding.ASCII.GetBytes(text), 0, text.Length);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            socket.OutputStream.Write(buffer, offset, count);
        }
        
        public void WriteLine(string text)
        {
            text += '\n';
            Write(ASCIIEncoding.ASCII.GetBytes(text), 0, text.Length);
        }

        public void toggleDTR()
        {
        }


        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => 0;
        public override long Position
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public Stream BaseStream => this;
        public void Dispose()
        {
            try
            {
                socket.Close();
            } catch {}
        }
    }
}