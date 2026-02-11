using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using log4net;
using Nefarius.Drivers.WinUSB;

namespace MissionPlanner.Comms
{
    public class CommsWinUSB : CommsBase, ICommsSerial
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CommsWinUSB));

        // CDC Interface Class/Subclass codes
        private const byte CDC_COMM_CLASS = 0x02;       // Communications Interface Class
        private const byte CDC_ACM_SUBCLASS = 0x02;     // Abstract Control Model subclass
        private const byte CDC_DATA_CLASS = 0x0A;       // CDC Data Interface Class

        // CDC Class-Specific Request Codes
        private const byte CDC_SET_LINE_CODING = 0x20;
        private const byte CDC_GET_LINE_CODING = 0x21;
        private const byte CDC_SET_CONTROL_LINE_STATE = 0x22;
        private const byte CDC_SEND_BREAK = 0x23;

        // CDC Functional Descriptor Subtypes (within CS_INTERFACE descriptors)
        private const byte CDC_FUNC_HEADER = 0x00;
        private const byte CDC_FUNC_CALL_MGMT = 0x01;
        private const byte CDC_FUNC_ACM = 0x02;
        private const byte CDC_FUNC_UNION = 0x06;

        // USB Descriptor Types
        private const byte DESC_TYPE_CS_INTERFACE = 0x24;

        // WinUSB device and configuration
        private USBDevice usbDevice;
        private bool isOpen;
        private ushort vendorId;
        private ushort productId;
        private int miNumber = -1;  // MI_xx for composite device functions (-1 = not specified)
        private string devicePath = "";

        // CDC ACM interface tracking
        private int controlInterfaceNumber = -1;  // USB descriptor interface number for CDC Communication
        private int dataInterfaceNumber = -1;     // USB descriptor interface number for CDC Data
        private byte acmCapabilities = 0;         // ACM functional descriptor capabilities bitmap

        // USB endpoints
        private byte bulkInPipe = 0x81;
        private byte bulkOutPipe = 0x01;

        // Buffer management
        private MemoryStream readBuffer = new MemoryStream(1024 * 10);
        private readonly object readBufferLock = new object();

        // Background read thread
        private Thread readThread;
        private bool stopReadThread;

        // Pipe references for read/write
        private USBPipe inPipe;
        private USBPipe outPipe;

        // Line coding state for CDC ACM
        private int _baudRate = 115200;
        private byte _stopBits = 0;   // 0=1 stop bit, 1=1.5, 2=2
        private byte _parity = 0;     // 0=None, 1=Odd, 2=Even, 3=Mark, 4=Space
        private byte _dataBits = 8;

        // ICommsSerial properties
        public int BaudRate
        {
            get => _baudRate;
            set
            {
                _baudRate = value;
                if (isOpen && controlInterfaceNumber >= 0)
                    SendLineCoding();
            }
        }

        public int BytesToRead { get; private set; }
        public int BytesToWrite => 0;

        public int DataBits
        {
            get => _dataBits;
            set
            {
                _dataBits = (byte)value;
                if (isOpen && controlInterfaceNumber >= 0)
                    SendLineCoding();
            }
        }

        public bool DtrEnable { get; set; }
        public bool RtsEnable { get; set; }
        public bool IsOpen => isOpen && usbDevice != null;
        public int ReadBufferSize { get; set; } = 8192;
        public int WriteBufferSize { get; set; } = 8192;
        public int ReadTimeout { get; set; } = 500;
        public int WriteTimeout { get; set; } = 500;

        // Static constructor for device enumeration registration
        static CommsWinUSB()
        {
            try
            {
                SerialPort.GetCustomPorts -= SerialPort_GetCustomPorts;
                SerialPort.GetCustomPorts += SerialPort_GetCustomPorts;
            }
            catch (Exception ex)
            {
                log.Error("Failed to register WinUSB custom ports", ex);
            }
        }

        public CommsWinUSB()
        {
        }

        // WinUSB device interface GUIDs to search for devices
        // Composite device children register under these GUIDs (from .inf), not the generic USB GUID
        private static readonly string[] WinUsbGuids = new[]
        {
            "{88BAE032-5A81-49f0-BC3D-A4FF138216D6}", // Common WinUSB GUID
            "{DEE824EF-729B-4A0E-9C14-B7117D33A817}", // Alternative WinUSB GUID
        };

        // Device enumeration - finds WinUSB devices including composite device children
        public static List<string> SerialPort_GetCustomPorts()
        {
            var ports = new List<string>();

            // Method 1: Enumerate WinUSB device interfaces directly via known GUIDs.
            // This is the primary method and catches composite device children which
            // register under WinUSB-specific GUIDs, not the generic USB device GUID.
            try
            {
                foreach (var guidString in WinUsbGuids)
                {
                    try
                    {
                        var devices = USBDevice.GetDevices(guidString);
                        foreach (var devInfo in devices)
                        {
                            var vid = devInfo.VID.ToString("X4");
                            var pid = devInfo.PID.ToString("X4");

                            // Check device path for mi_xx to detect composite function
                            var miMatch = Regex.Match(devInfo.DevicePath,
                                @"mi_([0-9a-fA-F]{2})", RegexOptions.IgnoreCase);
                            var mi = miMatch.Success ? miMatch.Groups[1].Value.ToUpper() : "";

                            var portName = $"WINUSB_VID_{vid}_PID_{pid}";
                            if (!string.IsNullOrEmpty(mi))
                                portName += $"_MI_{mi}";

                            ports.Add(portName);
                        }
                    }
                    catch
                    {
                        // GUID not registered, skip
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Failed to enumerate WinUSB device interfaces", ex);
            }

            // Method 2: PnP enumeration via generic USB device interface GUID.
            // Catches devices with custom GUIDs not in our list above.
            try
            {
                var usbDeviceGuid = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED");

                var instance = 0;
                while (Nefarius.Utilities.DeviceManagement.PnP.Devcon.FindByInterfaceGuid(
                    usbDeviceGuid, out var path, out var instanceId, instance++))
                {
                    try
                    {
                        var device = Nefarius.Utilities.DeviceManagement.PnP.PnPDevice.GetDeviceByInstanceId(instanceId);

                        var hardwareIds = device.GetProperty<string[]>(
                            Nefarius.Utilities.DeviceManagement.PnP.DevicePropertyKey.Device_HardwareIds);

                        if (hardwareIds != null && hardwareIds.Length > 0)
                        {
                            var match = Regex.Match(hardwareIds[0],
                                @"USB\\VID_([0-9A-Fa-f]{4})&PID_([0-9A-Fa-f]{4})(?:&MI_([0-9A-Fa-f]{2}))?");

                            if (match.Success)
                            {
                                var vid = match.Groups[1].Value;
                                var pid = match.Groups[2].Value;
                                var mi = match.Groups[3].Success ? match.Groups[3].Value : "";

                                var service = device.GetProperty<string>(
                                    Nefarius.Utilities.DeviceManagement.PnP.DevicePropertyKey.Device_Service);

                                if (service != null && service.Equals("WinUSB", StringComparison.OrdinalIgnoreCase))
                                {
                                    var portName = $"WINUSB_VID_{vid}_PID_{pid}";
                                    if (!string.IsNullOrEmpty(mi))
                                        portName += $"_MI_{mi}";
                                    ports.Add(portName);
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Skip devices that can't be queried
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Failed to enumerate USB PnP devices", ex);
            }

            return ports.Distinct().ToList();
        }

        // PortName property - parses VID/PID/MI from port name string
        public string PortName
        {
            get
            {
                if (vendorId > 0 && productId > 0)
                {
                    var name = $"WINUSB_VID_{vendorId:X4}_PID_{productId:X4}";
                    if (miNumber >= 0)
                        name += $"_MI_{miNumber:X2}";
                    return name;
                }
                if (!string.IsNullOrEmpty(devicePath))
                    return devicePath;
                return "WINUSB";
            }
            set
            {
                if (value.StartsWith("WINUSB_VID_"))
                {
                    var match = Regex.Match(value,
                        @"VID_([0-9A-Fa-f]{4})_PID_([0-9A-Fa-f]{4})(?:_MI_([0-9A-Fa-f]{2}))?");
                    if (match.Success)
                    {
                        vendorId = ushort.Parse(match.Groups[1].Value,
                            System.Globalization.NumberStyles.HexNumber);
                        productId = ushort.Parse(match.Groups[2].Value,
                            System.Globalization.NumberStyles.HexNumber);
                        if (match.Groups[3].Success)
                            miNumber = int.Parse(match.Groups[3].Value,
                                System.Globalization.NumberStyles.HexNumber);
                        else
                            miNumber = -1;
                    }
                }
                else if (value.StartsWith(@"\\?\") || value.StartsWith(@"\\.\\"))
                {
                    devicePath = value;
                }
            }
        }

        // BaseStream property
        public Stream BaseStream => new WinUSBStream(this);

        // Open method
        public void Open()
        {
            if (isOpen)
            {
                log.Warn("WinUSB device already open");
                return;
            }

            try
            {
                // Get settings or prompt user
                string vidStr = vendorId > 0 ? vendorId.ToString("X4") : "";
                string pidStr = productId > 0 ? productId.ToString("X4") : "";

                // Prompt if not set
                if (string.IsNullOrEmpty(vidStr))
                {
                    if (OnInputBoxShow("USB Vendor ID", "Enter VID in hex (e.g., 2341)", ref vidStr) == inputboxreturn.Cancel)
                        throw new Exception("Canceled by user");
                }

                if (string.IsNullOrEmpty(pidStr))
                {
                    if (OnInputBoxShow("USB Product ID", "Enter PID in hex (e.g., 0043)", ref pidStr) == inputboxreturn.Cancel)
                        throw new Exception("Canceled by user");
                }

                vendorId = ushort.Parse(vidStr, System.Globalization.NumberStyles.HexNumber);
                productId = ushort.Parse(pidStr, System.Globalization.NumberStyles.HexNumber);

                // Find device by VID/PID/MI
                if (string.IsNullOrEmpty(devicePath))
                {
                    devicePath = FindDevicePathByVidPid(vendorId, productId, miNumber);
                }

                if (string.IsNullOrEmpty(devicePath))
                    throw new Exception($"WinUSB device VID:{vendorId:X4} PID:{productId:X4}" +
                        (miNumber >= 0 ? $" MI:{miNumber:X2}" : "") + " not found");

                log.InfoFormat("Opening WinUSB device: {0}", devicePath);

                usbDevice = USBDevice.GetSingleDeviceByPath(devicePath);

                if (usbDevice == null)
                    throw new Exception("Failed to open WinUSB device");

                // Scan interfaces for CDC ACM and configure endpoints
                ConfigureCdcAcm();

                // Start background read thread
                lock (readBufferLock)
                {
                    readBuffer.SetLength(0);
                    readBuffer.Position = 0;
                    BytesToRead = 0;
                }

                stopReadThread = false;
                readThread = new Thread(ReadThreadWorker)
                {
                    IsBackground = true,
                    Name = "WinUSB Read Thread"
                };
                readThread.Start();

                isOpen = true;
                log.Info("WinUSB device opened successfully");
            }
            catch (Exception ex)
            {
                log.Error("Failed to open WinUSB device", ex);
                Close();
                throw;
            }
        }

        // Find device path by VID/PID with optional MI filtering for composite devices
        private static string FindDevicePathByVidPid(ushort vid, ushort pid, int mi = -1)
        {
            try
            {
                var commonGuids = new[]
                {
                    "{88BAE032-5A81-49f0-BC3D-A4FF138216D6}", // Common WinUSB GUID
                    "{DEE824EF-729B-4A0E-9C14-B7117D33A817}", // Alternative WinUSB GUID
                };

                foreach (var guidString in commonGuids)
                {
                    try
                    {
                        var devices = USBDevice.GetDevices(guidString);

                        foreach (var devInfo in devices)
                        {
                            if (devInfo.VID != vid || devInfo.PID != pid)
                                continue;

                            // For composite devices, match MI in the device path
                            if (mi >= 0)
                            {
                                var miMatch = Regex.Match(devInfo.DevicePath,
                                    @"mi_([0-9a-fA-F]{2})", RegexOptions.IgnoreCase);
                                if (miMatch.Success)
                                {
                                    int pathMi = int.Parse(miMatch.Groups[1].Value,
                                        System.Globalization.NumberStyles.HexNumber);
                                    if (pathMi == mi)
                                        return devInfo.DevicePath;
                                }
                                continue;
                            }

                            return devInfo.DevicePath;
                        }
                    }
                    catch
                    {
                        // Continue to next GUID
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Error finding device VID:{vid:X4} PID:{pid:X4}" +
                    (mi >= 0 ? $" MI:{mi:X2}" : ""), ex);
            }

            return null;
        }

        /// <summary>
        /// Scans USB interfaces to find CDC ACM Communication and Data interface pair.
        /// Parses CDC functional descriptors, sets up bulk endpoints from the Data
        /// interface, and sends initial SET_LINE_CODING to the control interface.
        /// </summary>
        private void ConfigureCdcAcm()
        {
            USBInterface cdcControlIf = null;
            USBInterface cdcDataIf = null;

            log.Info("Scanning USB interfaces for CDC ACM...");
            foreach (var iface in usbDevice.Interfaces)
            {
                log.InfoFormat("  Interface {0}: Class=0x{1:X2} SubClass=0x{2:X2} Protocol=0x{3:X2}",
                    iface.Number, iface.ClassValue, iface.SubClass, iface.Protocol);

                foreach (var pipe in iface.Pipes)
                {
                    log.InfoFormat("    Pipe 0x{0:X2}: {1} {2} MaxPacket={3}",
                        pipe.Address,
                        pipe.IsIn ? "IN" : "OUT",
                        pipe.TransferType,
                        pipe.MaximumPacketSize);
                }

                // CDC Communication Class Interface (control, ACM subclass)
                if (iface.ClassValue == CDC_COMM_CLASS && iface.SubClass == CDC_ACM_SUBCLASS)
                {
                    if (cdcControlIf == null)
                    {
                        cdcControlIf = iface;
                        controlInterfaceNumber = iface.Number;
                        log.InfoFormat("  -> CDC ACM Control Interface: {0}", iface.Number);
                    }
                }
                // CDC Data Class Interface (bulk data endpoints)
                else if (iface.ClassValue == CDC_DATA_CLASS)
                {
                    if (cdcDataIf == null)
                    {
                        cdcDataIf = iface;
                        dataInterfaceNumber = iface.Number;
                        log.InfoFormat("  -> CDC ACM Data Interface: {0}", iface.Number);
                    }
                }
            }

            // Parse CDC functional descriptors from the configuration descriptor
            ParseCdcFunctionalDescriptors();

            // Get bulk IN/OUT endpoints from the Data interface
            if (cdcDataIf != null)
            {
                foreach (var pipe in cdcDataIf.Pipes)
                {
                    if (pipe.IsIn && pipe.TransferType == USBTransferType.Bulk && inPipe == null)
                    {
                        inPipe = pipe;
                        bulkInPipe = pipe.Address;
                    }
                    else if (pipe.IsOut && pipe.TransferType == USBTransferType.Bulk && outPipe == null)
                    {
                        outPipe = pipe;
                        bulkOutPipe = pipe.Address;
                    }
                }
            }

            // Fallback: if no CDC Data interface or missing bulk endpoints, scan all interfaces
            if (inPipe == null || outPipe == null)
            {
                log.Warn("CDC Data interface not found or incomplete, scanning all interfaces for bulk endpoints");
                foreach (var iface in usbDevice.Interfaces)
                {
                    foreach (var pipe in iface.Pipes)
                    {
                        if (pipe.IsIn && pipe.TransferType == USBTransferType.Bulk && inPipe == null)
                        {
                            inPipe = pipe;
                            bulkInPipe = pipe.Address;
                        }
                        else if (pipe.IsOut && pipe.TransferType == USBTransferType.Bulk && outPipe == null)
                        {
                            outPipe = pipe;
                            bulkOutPipe = pipe.Address;
                        }
                    }
                }
            }

            if (inPipe == null || outPipe == null)
                log.Warn("Could not find both bulk IN and OUT endpoints");

            // Send initial line coding and control line state
            if (controlInterfaceNumber >= 0)
            {
                SendLineCoding();
                SetControlLineState(DtrEnable, RtsEnable);
            }

            log.InfoFormat("CDC ACM configured: ControlIF={0} DataIF={1} IN=0x{2:X2} OUT=0x{3:X2} ACM_Caps=0x{4:X2}",
                controlInterfaceNumber, dataInterfaceNumber, bulkInPipe, bulkOutPipe, acmCapabilities);
        }

        /// <summary>
        /// Reads the USB configuration descriptor and parses CDC class-specific
        /// functional descriptors: Header, ACM, Union, and Call Management.
        /// </summary>
        private void ParseCdcFunctionalDescriptors()
        {
            try
            {
                // Read config descriptor header (9 bytes) to get wTotalLength
                var header = usbDevice.ControlIn(0x80, 0x06, 0x0200, 0, 9);
                if (header == null || header.Length < 4)
                {
                    log.Debug("Could not read configuration descriptor header");
                    return;
                }

                int totalLength = header[2] | (header[3] << 8);
                if (totalLength < 9 || totalLength > 4096)
                {
                    log.DebugFormat("Invalid configuration descriptor wTotalLength: {0}", totalLength);
                    return;
                }

                var configDesc = usbDevice.ControlIn(0x80, 0x06, 0x0200, 0, totalLength);
                if (configDesc == null || configDesc.Length < totalLength)
                {
                    log.Debug("Could not read full configuration descriptor");
                    return;
                }

                log.InfoFormat("Configuration descriptor: {0} bytes, {1} interfaces",
                    totalLength, configDesc[4]);

                int offset = 0;
                while (offset < configDesc.Length - 1)
                {
                    byte bLength = configDesc[offset];
                    if (bLength < 2) break;
                    if (offset + bLength > configDesc.Length) break;

                    byte bDescriptorType = configDesc[offset + 1];

                    // CDC Class-Specific Interface Descriptor (CS_INTERFACE = 0x24)
                    if (bDescriptorType == DESC_TYPE_CS_INTERFACE && bLength >= 3)
                    {
                        byte bDescriptorSubtype = configDesc[offset + 2];

                        switch (bDescriptorSubtype)
                        {
                            case CDC_FUNC_HEADER:
                                if (bLength >= 5)
                                {
                                    ushort cdcVersion = (ushort)(configDesc[offset + 3] | (configDesc[offset + 4] << 8));
                                    log.InfoFormat("  CDC Header: version {0}.{1}",
                                        (cdcVersion >> 8) & 0xFF, cdcVersion & 0xFF);
                                }
                                break;

                            case CDC_FUNC_ACM:
                                if (bLength >= 4)
                                {
                                    acmCapabilities = configDesc[offset + 3];
                                    log.InfoFormat("  CDC ACM: capabilities=0x{0:X2} (LineCoding={1} SendBreak={2})",
                                        acmCapabilities,
                                        (acmCapabilities & 0x02) != 0 ? "yes" : "no",
                                        (acmCapabilities & 0x04) != 0 ? "yes" : "no");
                                }
                                break;

                            case CDC_FUNC_UNION:
                                if (bLength >= 5)
                                {
                                    int masterIf = configDesc[offset + 3];
                                    log.InfoFormat("  CDC Union: controlling IF={0}", masterIf);

                                    for (int i = 4; i < bLength; i++)
                                    {
                                        int subIf = configDesc[offset + i];
                                        log.InfoFormat("    subordinate IF={0}", subIf);
                                        if (dataInterfaceNumber < 0)
                                            dataInterfaceNumber = subIf;
                                    }

                                    if (controlInterfaceNumber < 0)
                                        controlInterfaceNumber = masterIf;
                                }
                                break;

                            case CDC_FUNC_CALL_MGMT:
                                if (bLength >= 5)
                                {
                                    byte caps = configDesc[offset + 3];
                                    int dataIf = configDesc[offset + 4];
                                    log.InfoFormat("  CDC Call Management: caps=0x{0:X2}, data IF={1}", caps, dataIf);
                                }
                                break;
                        }
                    }

                    offset += bLength;
                }
            }
            catch (Exception ex)
            {
                log.Debug("CDC functional descriptor parsing failed (non-fatal)", ex);
            }
        }

        /// <summary>
        /// Sends SET_LINE_CODING (0x20) to the CDC control interface to configure
        /// baud rate, stop bits, parity, and data bits.
        /// </summary>
        private void SendLineCoding()
        {
            if (controlInterfaceNumber < 0 || usbDevice == null)
                return;

            try
            {
                var lineCoding = new byte[7];
                lineCoding[0] = (byte)(_baudRate & 0xFF);
                lineCoding[1] = (byte)((_baudRate >> 8) & 0xFF);
                lineCoding[2] = (byte)((_baudRate >> 16) & 0xFF);
                lineCoding[3] = (byte)((_baudRate >> 24) & 0xFF);
                lineCoding[4] = _stopBits;
                lineCoding[5] = _parity;
                lineCoding[6] = _dataBits;

                // bmRequestType: 0x21 = Class | Interface | Host-to-Device
                usbDevice.ControlOut(0x21, CDC_SET_LINE_CODING, 0, controlInterfaceNumber, lineCoding);

                log.InfoFormat("SET_LINE_CODING: baud={0} data={1} parity={2} stop={3}",
                    _baudRate, _dataBits, _parity, _stopBits);
            }
            catch (Exception ex)
            {
                log.Warn("SET_LINE_CODING failed (device may not support it)", ex);
            }
        }

        // Background read thread
        private void ReadThreadWorker()
        {
            log.Info("WinUSB read thread started");

            byte[] buffer = new byte[ReadBufferSize];

            while (!stopReadThread && isOpen)
            {
                try
                {
                    if (inPipe == null) continue;

                    // Read from USB bulk IN endpoint
                    int bytesRead = inPipe.Read(buffer);

                    if (bytesRead > 0)
                    {
                        lock (readBufferLock)
                        {
                            // Append to read buffer
                            if (readBuffer.Position == readBuffer.Length && readBuffer.Length > 0)
                            {
                                // Buffer fully consumed, reset
                                readBuffer.SetLength(0);
                                readBuffer.Write(buffer, 0, bytesRead);
                                readBuffer.Position = 0;
                            }
                            else
                            {
                                // Append to existing buffer
                                var pos = readBuffer.Position;
                                readBuffer.Seek(0, SeekOrigin.End);
                                readBuffer.Write(buffer, 0, bytesRead);
                                readBuffer.Seek(pos, SeekOrigin.Begin);
                            }

                            BytesToRead = (int)(readBuffer.Length - readBuffer.Position);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (isOpen)
                    {
                        log.Error("WinUSB read thread error", ex);
                        Thread.Sleep(100);
                    }
                }
            }

            log.Info("WinUSB read thread stopped");
        }

        // Read methods
        public int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            if (!isOpen) throw new InvalidOperationException("Device not open");

            var deadline = DateTime.Now.AddMilliseconds(ReadTimeout);

            // Wait for data with timeout
            while (BytesToRead < count && DateTime.Now < deadline)
            {
                Thread.Sleep(1);
            }

            if (ReadTimeout > 0 && BytesToRead == 0)
            {
                throw new TimeoutException("No data available");
            }

            var read = Math.Min(count, BytesToRead);

            lock (readBufferLock)
            {
                read = readBuffer.Read(buffer, offset, read);
                BytesToRead = (int)(readBuffer.Length - readBuffer.Position);
            }

            return read;
        }

        public int ReadByte()
        {
            var buffer = new byte[1];
            var count = Read(buffer, 0, 1);
            return count > 0 ? buffer[0] : -1;
        }

        public int ReadChar() => ReadByte();

        public string ReadExisting()
        {
            if (BytesToRead == 0) return "";

            StringBuilder builder = new StringBuilder();

            lock (readBufferLock)
            {
                var bytesToRead = BytesToRead;
                for (int i = 0; i < bytesToRead; i++)
                {
                    int b = readBuffer.ReadByte();
                    if (b >= 0)
                        builder.Append((char)b);
                }

                BytesToRead = (int)(readBuffer.Length - readBuffer.Position);
            }

            return builder.ToString();
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

                    if (letter == '\n')
                        break;

                    count++;
                    if (count >= temp.Length)
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

        // Write methods
        public void Write(byte[] buffer, int offset, int count)
        {
            if (!isOpen)
                throw new InvalidOperationException("Device not open");

            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            try
            {
                if (outPipe == null)
                    throw new InvalidOperationException("Output pipe not available");

                byte[] writeData = buffer;

                if (offset != 0 || count != buffer.Length)
                {
                    writeData = new byte[count];
                    Array.Copy(buffer, offset, writeData, 0, count);
                }

                outPipe.Write(writeData);
            }
            catch (Exception ex)
            {
                log.Error("WinUSB write error", ex);
                // Silent failure (matches other implementations)
            }
        }

        public void Write(string text)
        {
            var data = Encoding.ASCII.GetBytes(text);
            Write(data, 0, data.Length);
        }

        public void WriteLine(string text)
        {
            Write(text + "\n");
        }

        // DiscardInBuffer
        public void DiscardInBuffer()
        {
            lock (readBufferLock)
            {
                readBuffer.SetLength(0);
                readBuffer.Position = 0;
                BytesToRead = 0;
            }
        }

        // Close and Dispose
        public void Close()
        {
            log.Info("Closing WinUSB device");

            try
            {
                // Stop read thread
                stopReadThread = true;
                if (readThread != null && readThread.IsAlive)
                {
                    if (!readThread.Join(1000))
                    {
                        try { readThread.Abort(); }
                        catch { }
                    }
                }

                // Close USB device
                if (usbDevice != null)
                {
                    usbDevice.Dispose();
                    usbDevice = null;
                }

                inPipe = null;
                outPipe = null;
                isOpen = false;
                controlInterfaceNumber = -1;
                dataInterfaceNumber = -1;
            }
            catch (Exception ex)
            {
                log.Error("Error during WinUSB close", ex);
            }
        }

        public void Dispose()
        {
            Close();
        }

        // DTR toggle
        public void toggleDTR()
        {
            if (!isOpen) return;

            try
            {
                SetControlLineState(false, RtsEnable);
                Thread.Sleep(50);
                SetControlLineState(true, RtsEnable);
                Thread.Sleep(50);
            }
            catch (Exception ex)
            {
                log.Error("Failed to toggle DTR", ex);
            }
        }

        // Send SET_CONTROL_LINE_STATE to the CDC control interface
        private void SetControlLineState(bool dtr, bool rts)
        {
            int ifNum = controlInterfaceNumber >= 0 ? controlInterfaceNumber : 0;
            ushort controlBits = (ushort)((dtr ? 1 : 0) | (rts ? 2 : 0));

            try
            {
                // bmRequestType: 0x21 (Class, Interface, Host-to-Device)
                // wValue: control line bitmap (bit 0=DTR, bit 1=RTS)
                // wIndex: interface number
                usbDevice.ControlOut(0x21, CDC_SET_CONTROL_LINE_STATE, controlBits, ifNum);
            }
            catch (Exception ex)
            {
                log.Warn("Device may not support CDC control lines", ex);
            }
        }

        // WinUSBStream wrapper class for BaseStream
        private class WinUSBStream : Stream
        {
            private CommsWinUSB parent;
            private long position;

            public WinUSBStream(CommsWinUSB parent)
            {
                this.parent = parent;
            }

            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => true;
            public override long Length => throw new NotImplementedException();

            public override long Position
            {
                get => position;
                set => position = value;
            }

            public override void Flush() { }

            public override int Read(byte[] buffer, int offset, int count)
            {
                var read = parent.Read(buffer, offset, count);
                position += read;
                return read;
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                parent.Write(buffer, offset, count);
                position += count;
            }

            public override long Seek(long offset, SeekOrigin origin) => position;
            public override void SetLength(long value) { }
        }
    }
}
