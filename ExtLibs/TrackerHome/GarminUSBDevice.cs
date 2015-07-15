using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace TrackerHomeGPS
{
    public partial class GarminUSBGPS : GPSDevice
    {
        protected static SafeFileHandle gHandle;
        protected static long gUSBPacketSize = 0;

        private static GarminProductData _pdata = new GarminProductData();
        private static ProtocolDataPacket _protocols = new ProtocolDataPacket();

        private Queue<Packet> _pqueue = new Queue<Packet>();

        new public static bool DevicePresent()
        {
            return Device.DevicePresent(DeviceGuid);
        }

        public override bool IsAvailable()
        {
            return ((gHandle != null) && (!gHandle.IsInvalid));
        }

        public override string DeviceName()
        {
            if (_pdata.productDescription != null)
                return _pdata.productDescription;
            else return "";
        }

        public GarminUSBGPS()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (!DevicePresent()) throw new DeviceNotReadyException("Device could not be detected in the system.");

            if ((gHandle != null) && (!gHandle.IsInvalid)) gHandle.Close();

            if (!(_obtainDeviceHandle())) throw new DeviceNotReadyException("The device could not be initialized, it may not be connected to the system.");
            if (!_startSession()) throw new DeviceNotReadyException("There was an unexpected error when starting the session with the device.");

            _getProductData();
        }

        public override GPSPosition GetCoordinates()
        {
            if (!IsAvailable()) Initialize();

            byte[] theData = new byte[2] {
				(byte) ((ushort)GarminAppProtocol010CommandType.Cmnd_Transfer_Pos & 0x00FF),
				(byte) (((ushort)GarminAppProtocol010CommandType.Cmnd_Transfer_Pos & 0xFF00) >> 8)
			};

            Packet theGetPositionPacket = new Packet(false, (short)GarminLinkProtocol1ID.Pid_Command_Data, 2, theData);
            Packet responsePacket;

            GarminRadianPosition ret = new GarminRadianPosition();

            _sendPacket(theGetPositionPacket);

            for (; ; )
            {
                responsePacket = _getPacket();
                if ((responsePacket.mPacketId == (short)GarminLinkProtocol1ID.Pid_Position_Data) && (responsePacket.mPacketType == Pid_Application_Layer))
                {
                    ret = (GarminRadianPosition)responsePacket;
                    return (GPSPosition)ret;
                }
            }
        }

        private void _sendPacket(Packet packet)
        {
            UInt32 theBytesToWrite = (UInt32)(12 + packet.mDataSize);
            UInt32 theBytesReturned = 0;

            byte[] tmp = new byte[theBytesToWrite];
            IntPtr tmpPtr = Marshal.AllocHGlobal((int)theBytesToWrite);
            Marshal.StructureToPtr(packet, tmpPtr, false);
            Marshal.Copy(packet.mData, 0, new IntPtr(tmpPtr.ToInt64() + 12), packet.mDataSize);

            WriteFile(gHandle, tmpPtr, theBytesToWrite, ref theBytesReturned, IntPtr.Zero);

            if (theBytesToWrite % gUSBPacketSize == 0)
            {
                WriteFile(gHandle, IntPtr.Zero, 0, ref theBytesReturned, IntPtr.Zero);
            }

            if (!tmpPtr.Equals(IntPtr.Zero)) Marshal.FreeHGlobal(tmpPtr);
        }

        private Packet _getPacket()
        {
            Packet thePacket = new Packet();
            uint theBufferSize = 0;
            byte[] theBuffer = new byte[0];

            for (; ; )
            {
                IntPtr theTempBuffer = Marshal.AllocHGlobal(ASYNC_DATA_SIZE);
                byte[] theNewBuffer;
                UInt32 theBytesReturned = 0;
                DeviceIoControl(gHandle, IOCTL_ASYNC_IN, IntPtr.Zero, 0, theTempBuffer, (uint)ASYNC_DATA_SIZE, ref theBytesReturned, IntPtr.Zero);
                theBufferSize += ASYNC_DATA_SIZE;
                theNewBuffer = new byte[theBufferSize];
                theBuffer.CopyTo(theNewBuffer, 0);
                Marshal.Copy(theTempBuffer, theNewBuffer, (Int32)theBufferSize - ASYNC_DATA_SIZE, ASYNC_DATA_SIZE);
                theBuffer = theNewBuffer;

                if (!theTempBuffer.Equals(IntPtr.Zero)) Marshal.FreeHGlobal(theTempBuffer);

                if (theBytesReturned != ASYNC_DATA_SIZE)
                {
                    IntPtr packetPtr = Marshal.AllocHGlobal(theBuffer.Length);
                    Marshal.Copy(theBuffer, 0, packetPtr, theBuffer.Length);

                    thePacket = _marshalPacket(packetPtr, theBytesReturned, 0);

                    if (!packetPtr.Equals(IntPtr.Zero)) Marshal.FreeHGlobal(packetPtr);
                    break;
                }
            }

            if ((thePacket.mPacketType == Pid_USB_Protocol_Layer) && (thePacket.mPacketId == Pid_Data_Available))
            {
                IntPtr packetBuffer = Marshal.AllocHGlobal(MAX_BUFFER_SIZE);
                UInt32 packetBufferSize = 0;

                IntPtr tmpBuffer = Marshal.AllocHGlobal(MAX_BUFFER_SIZE);
                UInt32 theBytesReturned = 1;

                int offset = 0;

                while (theBytesReturned > 0)
                {
                    ReadFile(gHandle, tmpBuffer, MAX_BUFFER_SIZE, ref theBytesReturned, IntPtr.Zero);
                    packetBufferSize += theBytesReturned;
                    for (int i = 0; i < theBytesReturned; i++)
                        Marshal.WriteByte(packetBuffer, (Int32)packetBufferSize, Marshal.ReadByte(tmpBuffer, i));

                    thePacket = _marshalPacket(packetBuffer, packetBufferSize, offset);
                    offset += thePacket.mDataSize;
                    _pqueue.Enqueue(thePacket);
                }

                if (!packetBuffer.Equals(IntPtr.Zero)) Marshal.FreeHGlobal(packetBuffer);
            }
            else _pqueue.Enqueue(thePacket);

            return _pqueue.Dequeue();
        }

        private Packet _marshalPacket(IntPtr ptrPacket, UInt32 bytesInPtr, int offset)
        {
            Packet ret = new Packet();

            if (bytesInPtr - offset < 12) return new Packet();

            ret.mPacketType = Marshal.ReadByte(ptrPacket, offset);
            ret.mReserved1 = Marshal.ReadByte(ptrPacket, offset + 1);
            ret.mReserved2 = Marshal.ReadInt16(ptrPacket, offset + 2);
            ret.mPacketId = Marshal.ReadInt16(ptrPacket, offset + 4);
            ret.mReserved3 = Marshal.ReadInt16(ptrPacket, offset + 6);
            ret.mDataSize = Marshal.ReadInt32(ptrPacket, offset + 8);
            ret.mData = new byte[ret.mDataSize];
            for (int i = 0; i < ret.mDataSize; i++)
                ret.mData[i] = Marshal.ReadByte(ptrPacket, offset + 12 + i);

            return ret;
        }

        private void _getProductData()
        {
            Packet theProductDataPacket = new Packet(false, Pid_Product_Rqst, 0, new byte[0] { });
            Packet responsePacket;

            _sendPacket(theProductDataPacket);

            for (; ; )
            {
                responsePacket = _getPacket();

                if ((responsePacket.mPacketType == Pid_Application_Layer) && (responsePacket.mPacketId == Pid_Product_Data))
                {
                    _pdata = (GarminProductData)responsePacket;
                    break;
                }
            }

            for (; ; )
            {
                responsePacket = _getPacket();

                if ((responsePacket.mPacketType == Pid_Application_Layer) && (responsePacket.mPacketId == Pid_Protocol_Array))
                {
                    _protocols = (ProtocolDataPacket)responsePacket;
                    break;
                }
            }

        }

        private bool _startSession()
        {
            Packet theStartSessionPacket = new Packet(true, Pid_Start_Session, 0, new byte[0] { });

            Packet responsePacket;

            _sendPacket(theStartSessionPacket);

            for (; ; )
            {
                responsePacket = _getPacket();

                if (responsePacket.mPacketType == Pid_USB_Protocol_Layer && responsePacket.mPacketId == Pid_Session_Started)
                {
                    break;
                }
            }

            return true;
        }

        private void _endSession()
        {
            gHandle.Close();
            gHandle = null;
            _pqueue = new Queue<Packet>();
        }

        private bool _obtainDeviceHandle()
        {
            try
            {
                // Used to capture how many bytes are returned by system calls
                UInt32 theBytesReturned = 0;

                // SetupAPI32.DLL Data Structures
                SP_DEVICE_INTERFACE_DETAIL_DATA theDevDetailData = new SP_DEVICE_INTERFACE_DETAIL_DATA();
                SP_DEVINFO_DATA theDevInfoData = new SP_DEVINFO_DATA();
                theDevInfoData.cbSize = Marshal.SizeOf(theDevInfoData);
                IntPtr theDevInfo = SetupDiGetClassDevs(ref DeviceGuid, IntPtr.Zero, IntPtr.Zero, (int)(DiGetClassFlags.DIGCF_PRESENT | DiGetClassFlags.DIGCF_DEVICEINTERFACE));
                SP_DEVICE_INTERFACE_DATA theInterfaceData = new SP_DEVICE_INTERFACE_DATA();
                theInterfaceData.cbSize = Marshal.SizeOf(theInterfaceData);

                // Check for a Garmin Device
                if (!SetupDiEnumDeviceInterfaces(theDevInfo, IntPtr.Zero, ref DeviceGuid, 0, ref theInterfaceData) && GetLastError() == ERROR_NO_MORE_ITEMS)
                {
                    gHandle = null;
                    return false;
                }

                // Get the device's file path
                SetupDiGetDeviceInterfaceDetail(theDevInfo, ref theInterfaceData, IntPtr.Zero, 0, ref theBytesReturned, IntPtr.Zero);

                if (theBytesReturned <= 0)
                {
                    gHandle = null;
                    return false;
                }

                IntPtr tmpBuffer = Marshal.AllocHGlobal((int)theBytesReturned);
                if (IntPtr.Size == 4) Marshal.WriteInt32(tmpBuffer, 4 + Marshal.SystemDefaultCharSize);
                else Marshal.WriteInt32(tmpBuffer, 8);

                theDevDetailData.cbSize = Marshal.SizeOf(theDevDetailData);
                SetupDiGetDeviceInterfaceDetail(theDevInfo, ref theInterfaceData, tmpBuffer, theBytesReturned, IntPtr.Zero, ref theDevInfoData);

                IntPtr pDevicePathName = new IntPtr(tmpBuffer.ToInt64() + 4);
                String devicePathName = Marshal.PtrToStringAuto(pDevicePathName);

                // Create a handle to the device
                gHandle = CreateFile(devicePathName, ((UInt32)(GenericAccessRights.GenericRead | GenericAccessRights.GenericWrite)), 0, IntPtr.Zero, (UInt32)FileCreationDisposition.OpenExisting, (UInt32)FileAttributes.Normal, IntPtr.Zero);

                // Get the driver's asynchronous packet size
                if (tmpBuffer.Equals(IntPtr.Zero)) Marshal.FreeHGlobal(tmpBuffer);
                tmpBuffer = Marshal.AllocHGlobal(Marshal.SizeOf(gUSBPacketSize));

                DeviceIoControl(gHandle, IOCTL_USB_PACKET_SIZE, IntPtr.Zero, 0, tmpBuffer, (UInt32)Marshal.SizeOf(gUSBPacketSize), ref theBytesReturned, IntPtr.Zero);

                switch (theBytesReturned)
                {
                    case 2:
                        gUSBPacketSize = Marshal.ReadInt16(tmpBuffer);
                        break;
                    case 4:
                        gUSBPacketSize = Marshal.ReadInt32(tmpBuffer);
                        break;
                    case 8:
                        gUSBPacketSize = Marshal.ReadInt64(tmpBuffer);
                        break;
                }
                if (!tmpBuffer.Equals(IntPtr.Zero)) Marshal.FreeHGlobal(tmpBuffer);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
