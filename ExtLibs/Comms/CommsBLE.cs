using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using int16_t = System.Int16;
using uint32_t = System.UInt32;
using size_t = System.Int64;

using simpleble_adapter_t = System.IntPtr;
using simpleble_peripheral_t = System.IntPtr;
using static MissionPlanner.Comms.CommsBLE.NativeMethods;
using System.Linq;
using System.Threading;
using System.IO;
using System.Threading.Tasks;

#pragma warning disable IDE1006 // Naming Styles

namespace MissionPlanner.Comms
{    
    public unsafe class CommsBLE : Stream, ICommsSerial
    {
        static MemoryStream readbuffer = new MemoryStream(1024 * 10);

        static CommsBLE() 
        {
            SerialPort.GetCustomPorts -= SerialPort_GetCustomPorts;
            SerialPort.GetCustomPorts += SerialPort_GetCustomPorts;          
        }

        public Stream BaseStream => this;

        public int BaudRate { get; set; }

        public int BytesToRead { get; set; }

        public int BytesToWrite { get; set; }

        public int DataBits { get; set; }
        public bool DtrEnable { get; set; }

        public bool IsOpen { get; private set; }

        public string PortName { get; set; }
        public int ReadBufferSize { get; set; }
        public bool RtsEnable { get; set; }
        public int WriteBufferSize { get; set; }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length { get; }

        public override long Position { get; set; }
        public int ReadTimeout { get; set; }
        public int WriteTimeout { get; set; }

        public void DiscardInBuffer()
        {
            readbuffer.SetLength(0);
        }

        public override void Flush()
        {

        }


        public void Open()
        {
            if (!simpleble_adapter_is_bluetooth_enabled())
            {
                return;
            }

            if (IsOpen)
                return;

            // only one instance of the scan
            if (Monitor.TryEnter(locker))
            {
                try
                {
                    if (adapter == null)
                        adapter = simpleble_adapter_get_handle(0);

                    if (adapter == null)
                    {
                        throw new Exception("No adapter was found.\n");
                        return;
                    }

                    simpleble_adapter_set_callback_on_scan_start(adapter, adapter_on_scan_start, null);
                    simpleble_adapter_set_callback_on_scan_stop(adapter, adapter_on_scan_stop, null);
                    simpleble_adapter_set_callback_on_scan_found(adapter, adapter_on_scan_found, null);
                    simpleble_adapter_set_callback_on_scan_updated(adapter, adapter_on_scan_updated, null);

                    simpleble_adapter_scan_for(adapter, 5000);

                    simpleble_adapter_set_callback_on_scan_start(adapter, null, null);
                    simpleble_adapter_set_callback_on_scan_stop(adapter, null, null);
                    simpleble_adapter_set_callback_on_scan_found(adapter, null, null);
                    simpleble_adapter_set_callback_on_scan_updated(adapter, null, null);

                    size_t useindex = -1;

                    size_t peripheral_count = simpleble_adapter_scan_get_results_count(adapter);
                    for (size_t peripheral_index = 0; peripheral_index < peripheral_count; peripheral_index++)
                    {
                        var peripheral = simpleble_adapter_scan_get_results_handle(adapter, peripheral_index);

                        string peripheral_identifier = simpleble_peripheral_identifier(peripheral);
                        string peripheral_address = simpleble_peripheral_address(peripheral);

                        bool peripheral_connectable = false;
                        simpleble_peripheral_is_connectable(peripheral, out peripheral_connectable);

                        int16_t peripheral_rssi = simpleble_peripheral_rssi(peripheral);

                        printf("[{0}] {1} [{2}] {3} dBm {4}\n", peripheral_index, peripheral_identifier, peripheral_address,
                               peripheral_rssi, peripheral_connectable ? "Connectable" : "Non-Connectable");

                        if (PortName.Contains(peripheral_address.ToLower().Replace(":", "")))
                        {
                            size_t services_count = simpleble_peripheral_services_count(peripheral);
                            for (size_t service_index = 0; service_index < services_count; service_index++)
                            {
                                simpleble_service_t service = default;
                                simpleble_peripheral_services_get(peripheral, service_index, ref service);

                                printf("    Service UUID: {0}\n", service.uuid.valueuuid);
                                printf("    Service data: ");
                                print_buffer_hex(service.data, service.data_length, true);

                                useindex = peripheral_index;
                            }
                        }

                        size_t manufacturer_data_count = simpleble_peripheral_manufacturer_data_count(peripheral);
                        for (size_t manuf_data_index = 0; manuf_data_index < manufacturer_data_count; manuf_data_index++)
                        {
                            simpleble_manufacturer_data_t manuf_data = default;
                            simpleble_peripheral_manufacturer_data_get(peripheral, manuf_data_index, ref manuf_data);
                            printf("    Manufacturer ID: {0}\n", manuf_data.manufacturer_id);
                            printf("    Manufacturer data: ");
                            print_buffer_hex(manuf_data.data, manuf_data.data_length, true);
                        }

                        // Let's not forget to release the associated handles and memory
                        simpleble_peripheral_release_handle(peripheral);
                        peripheral = IntPtr.Zero;
                        //simpleble_free(peripheral_address);
                        //simpleble_free(peripheral_identifier);
                    }

                    if (useindex == -1)
                    {
                        throw new Exception("Could not find device");
                    }

                    {
                        peripheral = simpleble_adapter_scan_get_results_handle(adapter, useindex);
                        string peripheral_address = simpleble_peripheral_address(peripheral);
                        bleperiph = this;

                        _callbackperipheral_on_connected = new MyCallbackp2(peripheral_on_connected);
                        _callbackperipheral_on_disconnected = new MyCallbackp2(peripheral_on_disconnected);

                        simpleble_peripheral_set_callback_on_connected(peripheral, _callbackperipheral_on_connected, null);
                        simpleble_peripheral_set_callback_on_disconnected(peripheral, _callbackperipheral_on_disconnected, null);


                        var err_code = simpleble_peripheral_connect(peripheral);
                        if (err_code != simpleble_err_t.SIMPLEBLE_SUCCESS)
                        {
                            throw new Exception("Failed to connect.\n");
                            return;
                        }

                        size_t services_count = simpleble_peripheral_services_count(peripheral);
                        printf("Successfully connected, listing {0} services.\n", services_count);

                        for (size_t i = 0; i < services_count; i++)
                        {
                            simpleble_service_t service = default;
                            err_code = simpleble_peripheral_services_get(peripheral, i, ref service);

                            if (err_code != simpleble_err_t.SIMPLEBLE_SUCCESS)
                            {
                                throw new Exception("Failed to get service.\n");
                                return;
                            }

                            printf("Service: {0} - ({1} characteristics)\n", service.uuid.valueuuid, service.characteristic_count);
                            for (size_t j = 0; j < service.characteristic_count; j++)
                            {
                                printf("  Characteristic: {0} - ({1} descriptors)\n", service.characteristics[j].uuid.valueuuid,
                                       service.characteristics[j].descriptor_count);
                                for (size_t k = 0; k < service.characteristics[j].descriptor_count; k++)
                                {
                                    printf("    Descriptor: {0}\n", service.characteristics[j].descriptors[k].uuid.valueuuid);
                                }
                            }
                        }

                        bool paired = false;
                        //while (!paired)
                        {
                            mtu = simpleble_peripheral_mtu(peripheral);
                            Console.WriteLine("MTU: " + mtu);
                            simpleble_peripheral_is_connected(peripheral, out paired);
                            //simpleble_peripheral_is_paired(peripheral, out paired);
                            Thread.Sleep(1000);
                            Console.WriteLine("Waiting for pairing/connected");
                        }

                        IsOpen = true;
                    }
                }
                finally
                {
                    Monitor.Exit(locker);
                }
            }
        }

        static object locker = new object();

        public static List<string> SerialPort_GetCustomPorts()
        {
            if (!System.Environment.Is64BitOperatingSystem)
                return ans;

            if (!simpleble_adapter_is_bluetooth_enabled())
            {
                return new List<string>();
            }

            // run scan in the background - and only one
            Task.Run(() =>
            {
                if (Monitor.TryEnter(locker))
                {
                    try
                    {
                        simpleble_adapter_t adapter = IntPtr.Zero;

                        if (adapter == IntPtr.Zero)
                            adapter = simpleble_adapter_get_handle(0);
                        if (adapter == null)
                        {
                            Console.WriteLine("No adapter was found.\n");
                            return;
                        }

                        if (simpleble_adapter_get_paired_peripherals_handle(adapter, 0) != IntPtr.Zero)
                        {
                            return;
                        }

                        simpleble_adapter_set_callback_on_scan_start(adapter, adapter_on_scan_start, null);
                        simpleble_adapter_set_callback_on_scan_stop(adapter, adapter_on_scan_stop, null);
                        simpleble_adapter_set_callback_on_scan_found(adapter, adapter_on_scan_found, null);
                        simpleble_adapter_set_callback_on_scan_updated(adapter, adapter_on_scan_updated, null);

                        simpleble_adapter_scan_for(adapter, 5000);

                        simpleble_adapter_set_callback_on_scan_start(adapter, null, null);
                        simpleble_adapter_set_callback_on_scan_stop(adapter, null, null);
                        simpleble_adapter_set_callback_on_scan_found(adapter, null, null);
                        simpleble_adapter_set_callback_on_scan_updated(adapter, null, null);

                        size_t peripheral_count = simpleble_adapter_scan_get_results_count(adapter);
                        for (size_t peripheral_index = 0; peripheral_index < peripheral_count; peripheral_index++)
                        {
                            var peripheral = simpleble_adapter_scan_get_results_handle(adapter, peripheral_index);

                            string peripheral_identifier = simpleble_peripheral_identifier(peripheral);
                            string peripheral_address = simpleble_peripheral_address(peripheral);

                            bool peripheral_connectable = false;
                            simpleble_peripheral_is_connectable(peripheral, out peripheral_connectable);

                            int16_t peripheral_rssi = simpleble_peripheral_rssi(peripheral);

                            printf("[{0}] {1} [{2}] {3} dBm {4}\n", peripheral_index, peripheral_identifier, peripheral_address,
                                   peripheral_rssi, peripheral_connectable ? "Connectable" : "Non-Connectable");

                            size_t services_count = simpleble_peripheral_services_count(peripheral);
                            for (size_t service_index = 0; service_index < services_count; service_index++)
                            {
                                simpleble_service_t service = default;
                                simpleble_peripheral_services_get(peripheral, service_index, ref service);

                                printf("    Service UUID: {0}\n", service.uuid.valueuuid);
                                printf("    Service data: ");
                                print_buffer_hex(service.data, service.data_length, true);

                                // nordic uart
                                if (service.uuid.valueuuid.Equals("6e400001-b5a3-f393-e0a9-e50e24dcca9e"))
                                {
                                    ans.Add("BLE_" + peripheral_identifier + "_" + peripheral_address.Replace(":", ""));
                                }
                            }

                            simpleble_peripheral_release_handle(peripheral);
                            peripheral = IntPtr.Zero;
                        }

                        simpleble_adapter_release_handle(adapter);
                        adapter = IntPtr.Zero;
                    }
                    catch { }
                    finally
                    {
                        Monitor.Exit(locker);
                    }
                }
            });

            return ans;
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
            Send(peripheral, buffer, offset, count);
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
            simpleble_peripheral_disconnect(peripheral);
            string peripheral_address = simpleble_peripheral_address(peripheral);
            IsOpen = false;
        }

        public void Dispose()
        {
            Close();
        }











        static void print_buffer_hex(uint8_t[] buf, size_t len, bool newline)
        {
            for (size_t i = 0; i < len; i++)
            {
                printf("{0:X2}", buf[i]);

                if (i < (len - 1))
                {
                    printf(" ");
                }
            }

            if (newline)
            {
                printf("\n");
            }
        }

        static void Main(string[] args)
        {
            new CommsBLE().Open();

            Thread.Sleep(1000 * 30);

            return;

            if (!simpleble_adapter_is_bluetooth_enabled())
            {
                return;
            }
            simpleble_adapter_t adapter;
            size_t adapter_count = simpleble_adapter_get_count();
            for (size_t i = 0; i < adapter_count; i++)
            {
                adapter = simpleble_adapter_get_handle(i);
                string identifier = simpleble_adapter_identifier(adapter);
                string address = simpleble_adapter_address(adapter);
                Console.WriteLine("Adapter: {0} {1}", identifier, address);
                simpleble_adapter_release_handle(adapter);
            }

            adapter = simpleble_adapter_get_handle(0);
            if (adapter == null)
            {
                Console.WriteLine("No adapter was found.\n");
                return ;
            }

            simpleble_adapter_set_callback_on_scan_start(adapter, adapter_on_scan_start, null);
            simpleble_adapter_set_callback_on_scan_stop(adapter, adapter_on_scan_stop, null);
            simpleble_adapter_set_callback_on_scan_found(adapter, adapter_on_scan_found, null);
            simpleble_adapter_set_callback_on_scan_updated(adapter, adapter_on_scan_updated, null);

            simpleble_adapter_scan_for(adapter, 5000);

            Thread.Sleep(1000);

            size_t useindex = -1;

            size_t peripheral_count = simpleble_adapter_scan_get_results_count(adapter);
            for (size_t peripheral_index = 0; peripheral_index < peripheral_count; peripheral_index++)
            {
                var peripheral = simpleble_adapter_scan_get_results_handle(adapter, peripheral_index);

                string peripheral_identifier = simpleble_peripheral_identifier(peripheral);
                string peripheral_address = simpleble_peripheral_address(peripheral);

                bool peripheral_connectable = false;
                simpleble_peripheral_is_connectable(peripheral,  out peripheral_connectable);

                int16_t peripheral_rssi = simpleble_peripheral_rssi(peripheral);

                printf("[{0}] {1} [{2}] {3} dBm {4}\n", peripheral_index, peripheral_identifier, peripheral_address,
                       peripheral_rssi, peripheral_connectable ? "Connectable" : "Non-Connectable");

                size_t services_count = simpleble_peripheral_services_count(peripheral);
                for (size_t service_index = 0; service_index < services_count; service_index++)
                {
                    simpleble_service_t service = default;
                    simpleble_peripheral_services_get(peripheral, service_index, ref service);

                    printf("    Service UUID: {0}\n", service.uuid.valueuuid);
                    printf("    Service data: ");
                    print_buffer_hex(service.data, service.data_length, true);

                    if (service.uuid.valueuuid.Equals("6e400001-b5a3-f393-e0a9-e50e24dcca9e")) 
                    {
                        useindex = peripheral_index;
                    }

                }

                size_t manufacturer_data_count = simpleble_peripheral_manufacturer_data_count(peripheral);
                for (size_t manuf_data_index = 0; manuf_data_index < manufacturer_data_count; manuf_data_index++)
                {
                    simpleble_manufacturer_data_t manuf_data = default;
                    simpleble_peripheral_manufacturer_data_get(peripheral, manuf_data_index, ref manuf_data);
                    printf("    Manufacturer ID: {0}\n", manuf_data.manufacturer_id);
                    printf("    Manufacturer data: ");
                    print_buffer_hex(manuf_data.data, manuf_data.data_length, true);
                }

                // Let's not forget to release the associated handles and memory
                simpleble_peripheral_release_handle(peripheral);
                //simpleble_free(peripheral_address);
                //simpleble_free(peripheral_identifier);
            }

            {
                var peripheral = simpleble_adapter_scan_get_results_handle(adapter, useindex);

                simpleble_peripheral_set_callback_on_connected(peripheral, peripheral_on_connected, null);
                simpleble_peripheral_set_callback_on_disconnected(peripheral, peripheral_on_disconnected, null);


                var err_code = simpleble_peripheral_connect(peripheral);
                if (err_code != simpleble_err_t.SIMPLEBLE_SUCCESS)
                {
                    printf("Failed to connect.\n");
                    return;
                }

                size_t services_count = simpleble_peripheral_services_count(peripheral);
                printf("Successfully connected, listing {0} services.\n", services_count);

                for (size_t i = 0; i < services_count; i++)
                {
                    simpleble_service_t service = default;
                    err_code = simpleble_peripheral_services_get(peripheral, i, ref service);

                    if (err_code != simpleble_err_t.SIMPLEBLE_SUCCESS)
                    {
                        printf("Failed to get service.\n");
                        return;
                    }

                    printf("Service: {0} - ({1} characteristics)\n", service.uuid.valueuuid, service.characteristic_count);
                    for (size_t j = 0; j < service.characteristic_count; j++)
                    {
                        printf("  Characteristic: {0} - ({1} descriptors)\n", service.characteristics[j].uuid.valueuuid,
                               service.characteristics[j].descriptor_count);
                        for (size_t k = 0; k < service.characteristics[j].descriptor_count; k++)
                        {
                            printf("    Descriptor: {0}\n", service.characteristics[j].descriptors[k].uuid.valueuuid);
                        }
                    }

                    simpleble_peripheral_read(peripheral, service.uuid, service.characteristics[0].uuid, null, null);
                }

                bool paired = false;
                while (!paired)
                {
                    simpleble_peripheral_is_paired(peripheral, out paired);
                    Thread.Sleep(1000);
                    Console.WriteLine("Waiting for pairing");
                }


                Thread.Sleep(1000*30);

                simpleble_peripheral_disconnect(peripheral);
            }
            
        }

        private static void peripheral_on_disconnected(simpleble_peripheral_t peripheral, void* userdata)
        {
            string peripheral_address = simpleble_peripheral_address(peripheral);

            Console.WriteLine("Disconnected from " + peripheral_address);

            bleperiph.Close();
        }

        private static void peripheral_on_connected(simpleble_peripheral_t peripheral, void* userdata)
        {
            string peripheral_address = simpleble_peripheral_address(peripheral);

            Console.WriteLine("Connected to " + peripheral_address + " at " + String.Format("{0:X}", peripheral));

            _callbackperipheral_on_notify = new NativeMethods.MyCallbackp(peripheral_on_notify);

            // rx data from notify
            simpleble_peripheral_notify(peripheral, RX_SERVICE_UUID, TX_CHAR_UUID, _callbackperipheral_on_notify, null);
        }

        private static void peripheral_on_notify(/*simpleble_peripheral_t handle,*/ simpleble_uuid_t service, simpleble_uuid_t characteristic, byte* data, long data_length, void* userdata)
        {
            var buffer = new byte[data_length];
            var len = (int)data_length;
            Marshal.Copy((IntPtr)data, buffer, 0, (int)data_length);

            //if (!bleperiph.ContainsKey(peripheral_address))
            {
                //return;
            }

            var ble = bleperiph;
            //var readbuffer = ble.readbuffer;

            if (TX_CHAR_UUID.ToString() == characteristic.ToString())
            {
                lock (readbuffer)
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
                        ble.BytesToRead = len;
                        //Log.Debug("BTSerial", "BytesToRead1 " + BytesToRead);
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
                        ble.BytesToRead += len;
                        //Log.Debug("BTSerial", "BytesToRead2 " + BytesToRead);
                    }
                }
            }
        }

        static byte[] GUIDtoByteArray(string guid)
        {
            var guidbyte = guid.Select(c => Convert.ToByte(c)).Append((byte)0).ToArray();

            return guidbyte;
        }

        public static simpleble_uuid_t CCCD = new simpleble_uuid_t() { value = GUIDtoByteArray("00002902-0000-1000-8000-00805f9b34fb") };
        public static simpleble_uuid_t RX_SERVICE_UUID = new simpleble_uuid_t() { value = GUIDtoByteArray("6e400001-b5a3-f393-e0a9-e50e24dcca9e") };
        public static simpleble_uuid_t RX_CHAR_UUID = new simpleble_uuid_t() { value = GUIDtoByteArray("6e400002-b5a3-f393-e0a9-e50e24dcca9e") };
        public static simpleble_uuid_t TX_CHAR_UUID = new simpleble_uuid_t() { value = GUIDtoByteArray("6e400003-b5a3-f393-e0a9-e50e24dcca9e") };
        private simpleble_peripheral_t peripheral;
        private static CommsBLE bleperiph;
        private static List<string> ans = new List<string>();
        private static MyCallbackp _callbackperipheral_on_notify;
        private MyCallbackp2 _callbackperipheral_on_connected;
        private MyCallbackp2 _callbackperipheral_on_disconnected;
        private ushort mtu;
        private simpleble_adapter_t adapter;

        private static void Send(simpleble_peripheral_t peripheral, byte[] data, int offset, int length) 
        {
            fixed (byte* ptr = data)
            {
                simpleble_peripheral_write_request(peripheral, RX_SERVICE_UUID,  RX_CHAR_UUID, ptr, length);
            }
            //Console.WriteLine("TX: " + BitConverter.ToString(data));
        }

        private static void adapter_on_scan_updated(simpleble_peripheral_t adapter, simpleble_peripheral_t peripheral, void* userdata)
        {
            var adapter_identifier = simpleble_adapter_identifier(adapter);
            var peripheral_identifier = simpleble_peripheral_identifier(peripheral);
            var peripheral_address = simpleble_peripheral_address(peripheral);

            if (adapter_identifier == null || peripheral_identifier == null || peripheral_address == null)
            {
                return;
            }

            printf("Adapter {0} updated device: {1} [{2}]\n", adapter_identifier, peripheral_identifier, peripheral_address);

            // Let's not forget to release the associated handles and memory
            simpleble_peripheral_release_handle(peripheral);
            //simpleble_free(peripheral_address);
            //simpleble_free(peripheral_identifier);
        }

        private static void adapter_on_scan_found(simpleble_peripheral_t adapter, simpleble_peripheral_t peripheral, void* userdata)
        {
            var adapter_identifier = simpleble_adapter_identifier(adapter);
            var peripheral_identifier = simpleble_peripheral_identifier(peripheral);
            var peripheral_address = simpleble_peripheral_address(peripheral);

            if (adapter_identifier == null || peripheral_identifier == null || peripheral_address == null)
            {
                return;
            }

            printf("Adapter {0} found device: {1} [{2}]\n", adapter_identifier, peripheral_identifier, peripheral_address);

            // Let's not forget to release the associated handles and memory
            simpleble_peripheral_release_handle(peripheral);
            //simpleble_free(peripheral_address);
            //simpleble_free(peripheral_identifier);
        }

        private static void adapter_on_scan_stop(simpleble_peripheral_t adapter, void* userdata)
        {
           var identifier = simpleble_adapter_identifier(adapter);

            if (identifier == null)
            {
                return;
            }

            printf("Adapter {0} stopped scanning.\n", identifier);

            // Let's not forget to clear the allocated memory.
            //simpleble_free(identifier);
        }

        private static void adapter_on_scan_start(simpleble_peripheral_t adapter, void* userdata)
        {
           var identifier = simpleble_adapter_identifier(adapter);

            if (identifier == null)
            {
                return;
            }

            printf("Adapter {0} started scanning.\n", identifier);

            // Let's not forget to clear the allocated memory.
            //simpleble_free(identifier);
        }

        public static void printf(string v, params object[] values)
        {
            Console.Write(v, values);
        }

        public class NativeMethods
        {
            const string SimpleBleExtLibrary = "simpleble-c.dll";

            const int SIMPLEBLE_UUID_STR_LEN = 37; // 36 characters + null terminator
            const int SIMPLEBLE_CHARACTERISTIC_MAX_COUNT = 16;
            const int SIMPLEBLE_DESCRIPTOR_MAX_COUNT = 16;

            // TODO: Add proper error codes.
            public enum simpleble_err_t
            {
                SIMPLEBLE_SUCCESS = 0,
                SIMPLEBLE_FAILURE = 1,
            }
 ;
            [StructLayout(LayoutKind.Sequential)]
            public struct simpleble_uuid_t            {
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = SIMPLEBLE_UUID_STR_LEN)]
                public byte[] value;// [SIMPLEBLE_UUID_STR_LEN];
                public string valueuuid { get { return Encoding.ASCII.GetString(value,0, SIMPLEBLE_UUID_STR_LEN-1); } }
                override public string ToString()
                {
                    return "simpleble_uuid_t: " + valueuuid + " " + BitConverter.ToString(value);
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct simpleble_descriptor_t
            {
                public simpleble_uuid_t uuid;

                public override string ToString()
                {
                    return "simpleble_descriptor_t: " +uuid.ToString();
                }
            }

            [Flags]
            public enum characteristic_can : UInt64
            {
                can_read = 1,
                can_write_request,
                can_write_command,
                can_notify,
                can_indicate                
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct simpleble_characteristic_t
            {
                public simpleble_uuid_t uuid;
                public characteristic_can can;
                public size_t descriptor_count;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = SIMPLEBLE_DESCRIPTOR_MAX_COUNT, ArraySubType = UnmanagedType.LPStruct)]
                public simpleble_descriptor_t[] descriptors;// [SIMPLEBLE_DESCRIPTOR_MAX_COUNT];
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct simpleble_service_t
            {
                public simpleble_uuid_t uuid;
                public size_t data_length;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 27)]
                public uint8_t[] data;
                // Note: The maximum length of a BLE advertisement is 31 bytes.
                // The first byte will be the length of the field,
                // the second byte will be the type of the field,
                // the next two bytes will be the service UUID,
                // and the remaining 27 bytes are the manufacturer data.
                public size_t characteristic_count;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = SIMPLEBLE_CHARACTERISTIC_MAX_COUNT, ArraySubType = UnmanagedType.LPStruct)]
                public simpleble_characteristic_t[] characteristics;// [SIMPLEBLE_CHARACTERISTIC_MAX_COUNT];
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct simpleble_manufacturer_data_t
            {
                public uint16_t manufacturer_id;
                public size_t data_length;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 27)]
                public uint8_t[] data;
                // Note: The maximum length of a BLE advertisement is 31 bytes.
                // The first byte will be the length of the field,
                // the second byte will be the type of the field (0xFF for manufacturer data),
                // the next two bytes will be the manufacturer ID,
                // and the remaining 27 bytes are the manufacturer data.
            }




            public enum simpleble_os_t
            {
                SIMPLEBLE_OS_WINDOWS = 0,
                SIMPLEBLE_OS_MACOS = 1,
                SIMPLEBLE_OS_LINUX = 2,
            }


            public enum simpleble_address_type_t
            {
                SIMPLEBLE_ADDRESS_TYPE_PUBLIC = 0,
                SIMPLEBLE_ADDRESS_TYPE_RANDOM = 1,
                SIMPLEBLE_ADDRESS_TYPE_UNSPECIFIED = 2,
            }


            /**
             * @brief
             *
             * @return bool
             */

            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)] public static extern bool simpleble_adapter_is_bluetooth_enabled();

            /**
             * @brief
             *
             * @return size_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)] public static extern size_t simpleble_adapter_get_count();

            /**
             * @brief
             *
             * @note The user is responsible for freeing the returned adapter object
             *       by calling `simpleble_adapter_release_handle`.
             *
             * @param index
             * @return simpleble_adapter_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)] public static extern simpleble_adapter_t simpleble_adapter_get_handle(size_t index);

            /**
             * @brief Releases all memory and resources consumed by the specific
             *        instance of simpleble_adapter_t.
             *
             * @param handle
             */

            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)] public static extern void simpleble_adapter_release_handle(simpleble_adapter_t handle);


            /**
             * @brief Returns the identifier of a given adapter.
             *
             * @note The user is responsible for freeing the returned value.
             *
             * @param handle
             * @return char*
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)][return: MarshalAs(UnmanagedType.LPStr)] public static extern string simpleble_adapter_identifier(simpleble_adapter_t handle);

            /**
             * @brief Returns the MAC address of a given adapter.
             *
             * @note The user is responsible for freeing the returned value.
             *
             * @param handle
             * @return char*
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)][return: MarshalAs(UnmanagedType.LPStr)] public static extern string simpleble_adapter_address(simpleble_adapter_t handle);

            /**
             * @brief
             *
             * @param handle
             * @return simpleble_err_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)] public static extern simpleble_err_t simpleble_adapter_scan_start(simpleble_adapter_t handle);

            /**
             * @brief
             *
             * @param handle
             * @return simpleble_err_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)] public static extern simpleble_err_t simpleble_adapter_scan_stop(simpleble_adapter_t handle);

            /**
             * @brief
             *
             * @param handle
             * @param active
             * @return simpleble_err_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)] public static extern simpleble_err_t simpleble_adapter_scan_is_active(simpleble_adapter_t handle, bool* active);

            /**
             * @brief
             *
             * @param handle
             * @param timeout_ms
             * @return simpleble_err_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)] public static extern simpleble_err_t simpleble_adapter_scan_for(simpleble_adapter_t handle, int timeout_ms);

            /**
             * @brief
             *
             * @param handle
             * @return size_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)] public static extern size_t simpleble_adapter_scan_get_results_count(simpleble_adapter_t handle);

            /**
             * @brief
             *
             * @note The user is responsible for freeing the returned peripheral object
             *       by calling `simpleble_peripheral_release_handle`.
             *
             * @param handle
             * @param index
             * @return simpleble_peripheral_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern simpleble_peripheral_t simpleble_adapter_scan_get_results_handle(simpleble_adapter_t handle,
                                                                                              size_t index);

            /**
             * @brief
             *
             * @param handle
             * @return size_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)] public static extern size_t simpleble_adapter_get_paired_peripherals_count(simpleble_adapter_t handle);

            /**
             * @brief
             *
             * @note The user is responsible for freeing the returned peripheral object
             *       by calling `simpleble_peripheral_release_handle`.
             *
             * @param handle
             * @param index
             * @return simpleble_peripheral_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern simpleble_peripheral_t simpleble_adapter_get_paired_peripherals_handle(simpleble_adapter_t handle,
                                                                                                    size_t index);

            /**
             * @brief
             *
             * @param handle
             * @param callback
             * @return simpleble_err_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern simpleble_err_t simpleble_adapter_set_callback_on_scan_start(
                simpleble_adapter_t handle, [MarshalAs(UnmanagedType.FunctionPtr)] MyCallback func, void* userdata);

            /**
             * @brief
             *
             * @param handle
             * @param callback
             * @return simpleble_err_t
             */
            public delegate void MyCallback(simpleble_adapter_t adapter, void* userdata);
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern simpleble_err_t simpleble_adapter_set_callback_on_scan_stop(
                simpleble_adapter_t handle, [MarshalAs(UnmanagedType.FunctionPtr)] MyCallback func, void* userdata);

            /**
             * @brief
             *
             * @param handle
             * @param callback
             * @return simpleble_err_t
             */
            public delegate void MyCallback2(simpleble_adapter_t adapter, simpleble_peripheral_t peripheral, void* userdata);
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern simpleble_err_t simpleble_adapter_set_callback_on_scan_updated(
                simpleble_adapter_t handle,
                [MarshalAs(UnmanagedType.FunctionPtr)] MyCallback2 func, void* userdata);

            /**
             * @brief
             *
             * @param handle
             * @param callback
             * @return simpleble_err_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern simpleble_err_t simpleble_adapter_set_callback_on_scan_found(
                simpleble_adapter_t handle,
                [MarshalAs(UnmanagedType.FunctionPtr)] MyCallback2 func, void* userdata);







            /**
             * @brief Releases all memory and resources consumed by the specific
             *        instance of simpleble_peripheral_t.
             *
             * @param handle
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)] public static extern void simpleble_peripheral_release_handle(simpleble_peripheral_t handle);

            /**
             * @brief
             *
             * @param handle
             * @return char*
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)][return: MarshalAs(UnmanagedType.LPStr)] public static extern string simpleble_peripheral_identifier(simpleble_peripheral_t handle);

            /**
             * @brief
             *
             * @param handle
             * @return char*
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)][return: MarshalAs(UnmanagedType.LPStr)] public static extern string simpleble_peripheral_address(simpleble_peripheral_t handle);

            /**
             * @brief
             *
             * @param handle
             * @return simpleble_address_type_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)] public static extern simpleble_address_type_t simpleble_peripheral_address_type(simpleble_peripheral_t handle);

            /**
             * @brief
             *
             * @param handle
             * @return int16_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)] public static extern int16_t simpleble_peripheral_rssi(simpleble_peripheral_t handle);

            /**
             * @brief
             *
             * @param handle
             * @return int16_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)] public static extern int16_t simpleble_peripheral_tx_power(simpleble_peripheral_t handle);

            /**
             * @brief
             *
             * @param handle
             * @return uint16_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)] public static extern uint16_t simpleble_peripheral_mtu(simpleble_peripheral_t handle);

            /**
             * @brief
             *
             * @param handle
             * @return simpleble_err_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)] public static extern simpleble_err_t simpleble_peripheral_connect(simpleble_peripheral_t handle);

            /**
             * @brief
             *
             * @param handle
             * @return simpleble_err_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)] public static extern simpleble_err_t simpleble_peripheral_disconnect(simpleble_peripheral_t handle);

            /**
             * @brief
             *
             * @param handle
             * @param connected
             * @return simpleble_err_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)] public static extern simpleble_err_t simpleble_peripheral_is_connected(simpleble_peripheral_t handle, out bool connected);

            /**
             * @brief
             *
             * @param handle
             * @param connectable
             * @return simpleble_err_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)] public static extern simpleble_err_t simpleble_peripheral_is_connectable(simpleble_peripheral_t handle, out bool connectable);

            /**
             * @brief
             *
             * @param handle
             * @param paired
             * @return simpleble_err_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)] public static extern simpleble_err_t simpleble_peripheral_is_paired(simpleble_peripheral_t handle, out bool paired);

            /**
             * @brief
             *
             * @param handle
             * @return simpleble_err_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)] public static extern simpleble_err_t simpleble_peripheral_unpair(simpleble_peripheral_t handle);

            /**
             * @brief
             *
             * @param handle
             * @return size_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)] public static extern size_t simpleble_peripheral_services_count(simpleble_peripheral_t handle);

            /**
             * @brief
             *
             * @param handle
             * @param index
             * @param services
             * @return simpleble_err_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern simpleble_err_t simpleble_peripheral_services_get(simpleble_peripheral_t handle, size_t index,
                                                                              ref simpleble_service_t services);

            /**
             * @brief
             *
             * @param handle
             * @return size_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)] public static extern size_t simpleble_peripheral_manufacturer_data_count(simpleble_peripheral_t handle);

            /**
             * @brief
             *
             * @param handle
             * @param index
             * @param manufacturer_data
             * @return simpleble_err_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern simpleble_err_t simpleble_peripheral_manufacturer_data_get(
                simpleble_peripheral_t handle, size_t index, ref simpleble_manufacturer_data_t manufacturer_data);

            /**
             * @brief
             *
             * @note The user is responsible for freeing the pointer returned in data.
             *
             * @param handle
             * @param service
             * @param characteristic
             * @param data
             * @param data_length
             * @return simpleble_err_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern simpleble_err_t simpleble_peripheral_read(simpleble_peripheral_t handle, simpleble_uuid_t service,
                                                                       simpleble_uuid_t characteristic, uint8_t** data,
                                                                       size_t* data_length);

            /**
             * @brief
             *
             * @param handle
             * @param service
             * @param characteristic
             * @param data
             * @param data_length
             * @return simpleble_err_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern simpleble_err_t simpleble_peripheral_write_request(simpleble_peripheral_t handle,
                                                                                simpleble_uuid_t service,
                                                                                simpleble_uuid_t characteristic,
                                                                    uint8_t* data, size_t data_length);

            /**
             * @brief
             *
             * @param handle
             * @param service
             * @param characteristic
             * @param data
             * @param data_length
             * @return simpleble_err_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern simpleble_err_t simpleble_peripheral_write_command(simpleble_peripheral_t handle,
                                                                               simpleble_uuid_t service,
                                                                               simpleble_uuid_t characteristic,
                                                                               uint8_t* data, size_t data_length);

            /**
             * @brief
             *
             * @param handle
             * @param service
             * @param characteristic
             * @param callback
             * @return simpleble_err_t
             */
            public delegate void MyCallbackp(/*simpleble_peripheral_t handle,*/ simpleble_uuid_t service, simpleble_uuid_t characteristic,
                                                        uint8_t* data, size_t data_length, void* userdata);

            /*
simpleble_err_t simpleble_peripheral_notify(
    simpleble_peripheral_t handle, simpleble_uuid_t service, simpleble_uuid_t characteristic,
    void (*callback)(simpleble_peripheral_t,simpleble_uuid_t, simpleble_uuid_t, const uint8_t*, size_t, void*), void* userdata) {
 

//SIMPLEBLE_EXPORT simpleble_err_t simpleble_peripheral_notify(simpleble_peripheral_t handle, simpleble_uuid_t service, simpleble_uuid_t characteristic, void (*callback)(simpleble_uuid_t service, simpleble_uuid_t characteristic, const uint8_t* data, size_t data_length, void* userdata), void* userdata);
function SimpleBlePeripheralNotify(Handle: TSimpleBlePeripheral; service: TSimpleBleUuid; Characteristic: TSimpleBleUuid; Callback: TSimpleBleCallbackNotify; UserData: PPointer): TSimpleBleErr; Cdecl; external SimpleBleExtLibrary name 'simpleble_peripheral_notify';

  //SIMPLEBLE_EXPORT simpleble_err_t simpleble_peripheral_notify(simpleble_peripheral_t handle, simpleble_uuid_t service, simpleble_uuid_t characteristic, void (*callback)(simpleble_uuid_t service, simpleble_uuid_t characteristic, const uint8_t* data, size_t data_length, void* userdata), void* userdata);
  TSimpleBleCallbackNotify = procedure(Service: TSimpleBleUuid; Characteristic: TSimpleBleUuid; Data: PByte; DataLength: NativeUInt; UserData: PPointer);

//SIMPLEBLE_EXPORT simpleble_err_t simpleble_peripheral_notify(simpleble_peripheral_t handle, simpleble_uuid_t service, simpleble_uuid_t characteristic, void (*callback)(simpleble_uuid_t service, simpleble_uuid_t characteristic, const uint8_t* data, size_t data_length, void* userdata), void* userdata);
function SimpleBlePeripheralNotify(Handle: TSimpleBlePeripheral; service: TSimpleBleUuid; Characteristic: TSimpleBleUuid; Callback: TSimpleBleCallbackNotify; UserData: PPointer): TSimpleBleErr; cdecl; external SimpleBleExtLibrary name 'simpleble_peripheral_notify';

             */

            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern simpleble_err_t
           simpleble_peripheral_notify(simpleble_peripheral_t handle, simpleble_uuid_t service, simpleble_uuid_t characteristic,
                                    [MarshalAs(UnmanagedType.FunctionPtr)] MyCallbackp func,
                                       void* userdata);

            /**
             * @brief
             *
             * @param handle
             * @param service
             * @param characteristic
             * @param callback
             * @return simpleble_err_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern simpleble_err_t
simpleble_peripheral_indicate(simpleble_peripheral_t handle, simpleble_uuid_t service, simpleble_uuid_t characteristic,
                              [MarshalAs(UnmanagedType.FunctionPtr)] MyCallbackp func,
                              void* userdata);

            /**
             * @brief
             *
             * @param handle
             * @param service
             * @param characteristic
             * @return simpleble_err_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern simpleble_err_t simpleble_peripheral_unsubscribe(simpleble_peripheral_t handle,
                                                                              simpleble_uuid_t service,
                                                                              simpleble_uuid_t characteristic);

            /**
             * @brief
             *
             * @param handle
             * @param service
             * @param characteristic
             * @param descriptor
             * @param data
             * @param data_length
             * @return simpleble_err_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern simpleble_err_t simpleble_peripheral_read_descriptor(simpleble_peripheral_t handle,
                                                                                  simpleble_uuid_t service,
                                                                                  simpleble_uuid_t characteristic,
                                                                                  simpleble_uuid_t descriptor, uint8_t** data,
                                                                                  size_t* data_length);

            /**
             * @brief
             *
             * @param handle
             * @param service
             * @param characteristic
             * @param descriptor
             * @param data
             * @param data_length
             * @return simpleble_err_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern simpleble_err_t simpleble_peripheral_write_descriptor(simpleble_peripheral_t handle,
                                                                                   simpleble_uuid_t service,
                                                                                   simpleble_uuid_t characteristic,
                                                                                   simpleble_uuid_t descriptor, uint8_t* data,
                                                                                   size_t data_length);

            /**
             * @brief
             *
             * @param handle
             * @param callback
             * @return simpleble_err_t
             */
            public delegate void MyCallbackp2(simpleble_peripheral_t peripheral, void* userdata);
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern simpleble_err_t simpleble_peripheral_set_callback_on_connected(
               simpleble_peripheral_t handle, [MarshalAs(UnmanagedType.FunctionPtr)] MyCallbackp2 func, void* userdata);

            /**
             * @brief
             *
             * @param handle
             * @param callback
             * @return simpleble_err_t
             */
            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern simpleble_err_t simpleble_peripheral_set_callback_on_disconnected(
                simpleble_peripheral_t handle, [MarshalAs(UnmanagedType.FunctionPtr)] MyCallbackp2 func, void* userdata);

            [DllImport(SimpleBleExtLibrary, CallingConvention = CallingConvention.Cdecl)] public static extern void simpleble_free(void* handle);
        }
    }
}
#pragma warning restore IDE1006 // Naming Styles