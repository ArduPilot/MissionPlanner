using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Runtime;
using Android.Util;
using BruTile.Wms;
using Java.Nio;
using Java.Util;
using MissionPlanner.ArduPilot;
using MissionPlanner.Comms;
using static Android.Renderscripts.Sampler;

namespace Xamarin.Droid
{
    public class BTDevice : IBlueToothDevice
    {
        public async Task<List<DeviceInfo>> GetDeviceInfoList()
        {
            var result = new List<DeviceInfo>();
            // Get the local Bluetooth adapter
            var btAdapter = BluetoothAdapter.DefaultAdapter;

            if (btAdapter != null && btAdapter.IsEnabled)
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
                        {
                            result.Add(new DeviceInfo() { board = "BLE_" + device.Name, hardwareid = device.Address});                            
                            continue;
                        }
                        result.Add(new DeviceInfo() {board = "BT_" + device.Name, hardwareid = device.Address});
                    }
                }
            }

            return result;
        }

        public async Task<ICommsSerial> GetBT(DeviceInfo first)
        {
            if (first.board.StartsWith("BLE_"))
            {
                return new BTSerialBLE(first);
            }
            else
            {
                return new BTSerial(first);
            }
        }
    }

    public class BTSerialBLE : Stream, ICommsSerial
    {
        MemoryStream readbuffer = new MemoryStream(1024 * 10);

        // nordic uart
        public static UUID CCCD = UUID.FromString("00002902-0000-1000-8000-00805f9b34fb");
        public static UUID RX_SERVICE_UUID = UUID.FromString("6e400001-b5a3-f393-e0a9-e50e24dcca9e");
        public static UUID RX_CHAR_UUID = UUID.FromString("6e400002-b5a3-f393-e0a9-e50e24dcca9e");
        public static UUID TX_CHAR_UUID = UUID.FromString("6e400003-b5a3-f393-e0a9-e50e24dcca9e");

        public BTSerialBLE(DeviceInfo first)
        {
            _first = first;
            PortName = first.name;
        }

        public Stream BaseStream => this;

        public int BaudRate { get; set; }

        public int BytesToRead { get; set; }

        public int BytesToWrite { get; set; }

        public int DataBits { get; set; }
        public bool DtrEnable { get; set; }

        private BluetoothGatt mBluetoothGatt;

        public bool IsOpen { get; private set; }

        private DeviceInfo _first;

        public string PortName { get; set; }
        public int ReadBufferSize { get; set; }
        public bool RtsEnable { get; set; }
        public int WriteBufferSize { get; set; }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length { get; }

        public override long Position {get;set; }
        public int ReadTimeout { get; set; }
        public int WriteTimeout { get; set; }

        public void DiscardInBuffer()
        {

        }

        public override void Flush()
        {

        }

        public void Open()
        {
            Log.Info("MP", "Open {0} {1}", _first.name, _first.hardwareid);
            BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            BluetoothDevice device = bluetoothAdapter.GetRemoteDevice(_first.hardwareid);

            mBluetoothGatt = device.ConnectGatt(null, false, new BluetoothGattCallback(this));

            Thread.Sleep(1000);

            IsOpen = true;
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
                build.Append((char)ReadByte());
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


        public override long Seek(long offset, SeekOrigin origin)
        {
            return BaseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            BaseStream.SetLength(value);
        }

        public void toggleDTR()
        {
            
        }


        public override void Write(byte[] buffer, int offset, int count)
        {
            // rx from nordic side
            BluetoothGattService RxService = mBluetoothGatt.GetService(RX_SERVICE_UUID);
            BluetoothGattCharacteristic RxChar = RxService.GetCharacteristic(RX_CHAR_UUID);

            RxChar.SetValue(buffer.Skip(offset).Take(count).ToArray());
            bool status = mBluetoothGatt.WriteCharacteristic(RxChar);

            Log.Debug("MP", "Write {0} {1} {2}", status, buffer.Length, count);
        }

        public void Write(string text)
        {
            Write(ASCIIEncoding.ASCII.GetBytes(text), 0, text.Length);
        }

        public void WriteLine(string text)
        {
            text += '\n';
            Write(ASCIIEncoding.ASCII.GetBytes(text), 0, text.Length);
        }

        public void Close()
        {
            mBluetoothGatt?.Close();
        }

        public void Dispose()
        {
            Close();
        }

        private class BluetoothGattCallback : global::Android.Bluetooth.BluetoothGattCallback
        {
            public BTSerialBLE BTSerialBLE { get; }

            public BluetoothGattCallback(BTSerialBLE bTSerialBLE)
            {
                BTSerialBLE = bTSerialBLE;
            }

            public override void OnConnectionStateChange(BluetoothGatt gatt, [GeneratedEnum] GattStatus status, [GeneratedEnum] ProfileState newState)
            {
                Log.Info("MP", "OnConnectionStateChange {0} - {1}", newState, status);
                base.OnConnectionStateChange(gatt, status, newState);

                if (newState == ProfileState.Connected)
                {
                    gatt.DiscoverServices();
                } 
                else if (newState == ProfileState.Disconnected)
                {
                    BTSerialBLE.IsOpen = false;
                }
            }

            public override void OnServicesDiscovered(BluetoothGatt gatt, [GeneratedEnum] GattStatus status)
            {
                Log.Info("MP", "OnServicesDiscovered {0}", status);
                base.OnServicesDiscovered(gatt, status);

                BluetoothGattService RxService = BTSerialBLE.mBluetoothGatt.GetService(RX_SERVICE_UUID);
                if (RxService != null)
                {
                    BluetoothGattCharacteristic TxChar = RxService.GetCharacteristic(TX_CHAR_UUID);
                    if (TxChar != null)
                    {
                        BTSerialBLE.mBluetoothGatt.SetCharacteristicNotification(TxChar, true);
                        BluetoothGattDescriptor descriptor = TxChar.GetDescriptor(CCCD);
                        descriptor.SetValue(BluetoothGattDescriptor.EnableNotificationValue.ToArray());
                        BTSerialBLE.mBluetoothGatt.WriteDescriptor(descriptor);
                    }
                    else
                        Log.Info("MP", "OnServicesDiscovered GetService fail");
                }
                else
                    Log.Info("MP", "OnServicesDiscovered GetCharacteristic fail");

                Log.Info("MP", "OnServicesDiscovered Done");
            }

            public override void OnCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, [GeneratedEnum] GattStatus status)
            {
                Log.Info("MP", "OnCharacteristicRead {0} {1}", characteristic.Uuid, status);
                base.OnCharacteristicRead(gatt, characteristic, status);

                update(characteristic);
            }

            private void update(BluetoothGattCharacteristic characteristic)
            {
                Log.Debug("MP", "update {0} {1}", characteristic.Uuid, TX_CHAR_UUID.Equals(characteristic.Uuid));
                if (TX_CHAR_UUID.Equals(characteristic.Uuid))
                {
                    lock (BTSerialBLE.readbuffer)
                    {
                        var buffer = characteristic.GetValue();
                        var len = buffer.Length;
                        if (BTSerialBLE.readbuffer.Position == BTSerialBLE.readbuffer.Length && BTSerialBLE.readbuffer.Length > 0)
                        {
                            //clear it
                            BTSerialBLE.readbuffer.SetLength(0);
                            // add data
                            BTSerialBLE.readbuffer.Write(buffer, 0, len);
                            // set readback start point
                            BTSerialBLE.readbuffer.Position = 0;
                            // update toread
                            BTSerialBLE.BytesToRead += len;
                            Log.Debug("BTSerial", "BytesToRead1 " + BTSerialBLE.BytesToRead);
                        }
                        else
                        {
                            var pos = BTSerialBLE.readbuffer.Position;
                            // goto end
                            BTSerialBLE.readbuffer.Seek(0, SeekOrigin.End);
                            //write
                            BTSerialBLE.readbuffer.Write(buffer, 0, len);
                            // seek back to readpos
                            BTSerialBLE.readbuffer.Seek(pos, SeekOrigin.Begin);
                            BTSerialBLE.BytesToRead += len;
                            Log.Debug("BTSerial", "BytesToRead2 " + BTSerialBLE.BytesToRead);
                        }
                    }
                }
            }

            public override void OnCharacteristicChanged(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic)
            {
                Log.Info("MP", "OnCharacteristicChanged {0}", characteristic.Uuid);
                base.OnCharacteristicChanged(gatt, characteristic);

                update(characteristic);
            }
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