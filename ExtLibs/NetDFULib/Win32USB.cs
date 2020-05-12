/*
 * This module was originally posted on CodeProject in an article titled
 * ST Micro-electronics Device Firmware Upgrade (DFU/DfuSe) from C#
 * By Mark McLean (ExpElec), 4 Mar 2013 
 * http://www.codeproject.com/Tips/540963/ST-Micro-electronics-Device-Firmware-Upgrade-DFU-D
 * 
 * and is licensed under the The Code Project Open License (CPOL) 1.02,
 * which can be found here
 * http://www.codeproject.com/info/cpol10.aspx
*/
// From the "Working with USB devices in .NET and C#" tutorial by Ashley Deakin on
// http://www.developerfusion.com/article/84338/making-usb-c-friendly/
//
// Had to make a couple of changes for 64 bit compatibility

using System;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace NetDFULib
{
	/// <summary>
	/// Class that wraps USB API calls and structures
	/// </summary>
	[Serializable]
    public class Win32Usb
    {
        #region Structures
        /// <summary>
        /// An overlapped structure used for overlapped IO operations. The structure is
        /// only used by the OS to keep state on pending operations. You don't need to fill anything in if you
        /// unless you want a Windows event to fire when the operation is complete.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct Overlapped
        {
            public uint Internal;
            public uint InternalHigh;
            public uint Offset;
            public uint OffsetHigh;
            public IntPtr Event;
        }
		/// <summary>
		/// Provides details about a single USB device
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		protected struct DeviceInterfaceData
		{
			public int Size;
			public Guid InterfaceClassGuid;
			public int Flags;
			public IntPtr Reserved;
		//           public int Reserved; - int and IntPtr are the same thing in 32 bit, but NOT in 64bit !
		}
		/// <summary>
		/// Provides the capabilities of a HID device
		/// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct HidCaps
        {
            public short Usage;
            public short UsagePage;
            public short InputReportByteLength;
            public short OutputReportByteLength;
            public short FeatureReportByteLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            public short[] Reserved;
            public short NumberLinkCollectionNodes;
            public short NumberInputButtonCaps;
            public short NumberInputValueCaps;
            public short NumberInputDataIndices;
            public short NumberOutputButtonCaps;
            public short NumberOutputValueCaps;
            public short NumberOutputDataIndices;
            public short NumberFeatureButtonCaps;
            public short NumberFeatureValueCaps;
            public short NumberFeatureDataIndices;
        }
		/// <summary>
		/// Access to the path for a device
		/// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DeviceInterfaceDetailData
        {
            public int Size;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string DevicePath;
        }
		/// <summary>
		/// Used when registering a window to receive messages about devices added or removed from the system.
		/// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
        public class DeviceBroadcastInterface
        {
            public int Size;
            public int DeviceType;
            public int Reserved;
            public Guid ClassGuid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string Name;
        }		
        #endregion

        #region Constants
		/// <summary>Windows message sent when a device is inserted or removed</summary>
		public const int WM_DEVICECHANGE = 0x0219;
		/// <summary>WParam for above : A device was inserted</summary>
		public const int DEVICE_ARRIVAL = 0x8000;
		/// <summary>WParam for above : A device was removed</summary>
		public const int DEVICE_REMOVECOMPLETE = 0x8004;
		/// <summary>Used in SetupDiClassDevs to get devices present in the system</summary>
        protected const int DIGCF_PRESENT = 0x02;
		/// <summary>Used in SetupDiClassDevs to get device interface details</summary>
		protected const int DIGCF_DEVICEINTERFACE = 0x10;
		/// <summary>Used when registering for device insert/remove messages : specifies the type of device</summary>
		protected const int DEVTYP_DEVICEINTERFACE = 0x05;
		/// <summary>Used when registering for device insert/remove messages : we're giving the API call a window handle</summary>
		protected const int DEVICE_NOTIFY_WINDOW_HANDLE = 0;
        /// <summary>Purges Win32 transmit buffer by aborting the current transmission.</summary>
        protected const uint PURGE_TXABORT = 0x01;
        /// <summary>Purges Win32 receive buffer by aborting the current receive.</summary>
        protected const uint PURGE_RXABORT = 0x02;
        /// <summary>Purges Win32 transmit buffer by clearing it.</summary>
        protected const uint PURGE_TXCLEAR = 0x04;
        /// <summary>Purges Win32 receive buffer by clearing it.</summary>
        protected const uint PURGE_RXCLEAR = 0x08;
        /// <summary>CreateFile : Open file for read</summary>
        protected const uint GENERIC_READ = 0x80000000;
        /// <summary>CreateFile : Open file for write</summary>
        protected const uint GENERIC_WRITE = 0x40000000;
        /// <summary>CreateFile : Open handle for overlapped operations</summary>
        protected const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        /// <summary>CreateFile : Resource to be "created" must exist</summary>
        protected const uint OPEN_EXISTING = 3;
        /// <summary>ReadFile/WriteFile : Overlapped operation is incomplete.</summary>
		protected const uint ERROR_NO_MORE_ITEMS = 259;
		/// <summary>Infinite timeout</summary>
		protected const uint ERROR_IO_PENDING = 997;
		/// <summary>Infinite timeout</summary>
		protected const uint ERROR_INVALID_USER_BUFFER = 1784;
		/// <summary>Infinite timeout</summary>
		protected const uint INFINITE = 0xFFFFFFFF;
        /// <summary>Simple representation of a null handle : a closed stream will get this handle. Note it is public for comparison by higher level classes.</summary>
        public static IntPtr NullHandle = IntPtr.Zero;
        /// <summary>Simple representation of the handle returned when CreateFile fails.</summary>
        protected static IntPtr InvalidHandleValue = new IntPtr(-1);
        #endregion

        #region P/Invoke
		/// <summary>
		/// Gets the GUID that Windows uses to represent HID class devices
		/// </summary>
		/// <param name="gHid">An out parameter to take the Guid</param>
        [DllImport("hid.dll",      SetLastError = true)] protected static extern void HidD_GetHidGuid(out Guid gHid);
		/// <summary>
		/// Allocates an InfoSet memory block within Windows that contains details of devices.
		/// </summary>
		/// <param name="gClass">Class guid (e.g. HID guid)</param>
		/// <param name="strEnumerator">Not used</param>
		/// <param name="hParent">Not used</param>
		/// <param name="nFlags">Type of device details required (DIGCF_ constants)</param>
		/// <returns>A reference to the InfoSet</returns>
        [DllImport("setupapi.dll", SetLastError = true)] protected static extern IntPtr SetupDiGetClassDevs(ref Guid gClass, [MarshalAs(UnmanagedType.LPStr)] string strEnumerator, IntPtr hParent, uint nFlags);
		/// <summary>
		/// Frees InfoSet allocated in call to above.
		/// </summary>
		/// <param name="lpInfoSet">Reference to InfoSet</param>
		/// <returns>true if successful</returns>
		[DllImport("setupapi.dll", SetLastError = true)] protected static extern int SetupDiDestroyDeviceInfoList(IntPtr lpInfoSet);
		/// <summary>
		/// Gets the DeviceInterfaceData for a device from an InfoSet.
		/// </summary>
		/// <param name="lpDeviceInfoSet">InfoSet to access</param>
		/// <param name="nDeviceInfoData">Not used</param>
		/// <param name="gClass">Device class guid</param>
		/// <param name="nIndex">Index into InfoSet for device</param>
		/// <param name="oInterfaceData">DeviceInterfaceData to fill with data</param>
		/// <returns>True if successful, false if not (e.g. when index is passed end of InfoSet)</returns>
        [DllImport("setupapi.dll", SetLastError = true)] protected static extern bool SetupDiEnumDeviceInterfaces(IntPtr lpDeviceInfoSet, uint nDeviceInfoData, ref Guid gClass, uint nIndex, ref DeviceInterfaceData oInterfaceData);

		/// <summary>
		/// SetupDiGetDeviceInterfaceDetail - two of these, overloaded because they are used together in slightly different
		/// ways and the parameters have different meanings.
		/// Gets the interface detail from a DeviceInterfaceData. This is pretty much the device path.
		/// You call this twice, once to get the size of the struct you need to send (nDeviceInterfaceDetailDataSize=0)
		/// and once again when you've allocated the required space.
		/// </summary>
		/// <param name="lpDeviceInfoSet">InfoSet to access</param>
		/// <param name="oInterfaceData">DeviceInterfaceData to use</param>
		/// <param name="lpDeviceInterfaceDetailData">DeviceInterfaceDetailData to fill with data</param>
		/// <param name="nDeviceInterfaceDetailDataSize">The size of the above</param>
		/// <param name="nRequiredSize">The required size of the above when above is set as zero</param>
		/// <param name="lpDeviceInfoData">Not used</param>
		/// <returns></returns>
        [DllImport("setupapi.dll", SetLastError = true)] protected static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr lpDeviceInfoSet, ref DeviceInterfaceData oInterfaceData, IntPtr lpDeviceInterfaceDetailData, uint nDeviceInterfaceDetailDataSize, ref uint nRequiredSize, IntPtr lpDeviceInfoData);
        [DllImport("setupapi.dll", SetLastError = true)] protected static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr lpDeviceInfoSet, ref DeviceInterfaceData oInterfaceData, ref DeviceInterfaceDetailData oDetailData, uint nDeviceInterfaceDetailDataSize, ref uint nRequiredSize, IntPtr lpDeviceInfoData);

		/// <summary>
		/// Registers a window for device insert/remove messages
		/// </summary>
		/// <param name="hwnd">Handle to the window that will receive the messages</param>
		/// <param name="lpInterface">DeviceBroadcastInterrface structure</param>
		/// <param name="nFlags">set to DEVICE_NOTIFY_WINDOW_HANDLE</param>
		/// <returns>A handle used when unregistering</returns>
        [DllImport("user32.dll",   SetLastError = true)] protected static extern IntPtr RegisterDeviceNotification(IntPtr hwnd, DeviceBroadcastInterface oInterface, uint nFlags);

		/// <summary>
		/// Unregister from above.
		/// </summary>
		/// <param name="hHandle">Handle returned in call to RegisterDeviceNotification</param>
		/// <returns>True if success</returns>
        [DllImport("user32.dll",   SetLastError = true)] protected static extern bool UnregisterDeviceNotification(IntPtr hHandle);

		/// <summary>
		/// Gets details from an open device. Reserves a block of memory which must be freed.
		/// </summary>
		/// <param name="hFile">Device file handle</param>
		/// <param name="lpData">Reference to the preparsed data block</param>
		/// <returns></returns>
		[DllImport("hid.dll",      SetLastError = true)] protected static extern bool HidD_GetPreparsedData(IntPtr hFile, out IntPtr lpData);
		/// <summary>
		/// Frees the memory block reserved above.
		/// </summary>
		/// <param name="pData">Reference to preparsed data returned in call to GetPreparsedData</param>
		/// <returns></returns>
		[DllImport("hid.dll",      SetLastError = true)] protected static extern bool HidD_FreePreparsedData(ref IntPtr pData);
		/// <summary>
		/// Gets a device's capabilities from the preparsed data.
		/// </summary>
		/// <param name="lpData">Preparsed data reference</param>
		/// <param name="oCaps">HidCaps structure to receive the capabilities</param>
		/// <returns>True if successful</returns>
		[DllImport("hid.dll", SetLastError = true)] protected static extern int HidP_GetCaps(IntPtr lpData, out HidCaps oCaps);

		// Additional P/Invokes for device firmware update
		[DllImport("hid.dll", SetLastError = true)] 
		protected static extern bool HidD_GetFeature(IntPtr lpData, out byte lpReportBuffer, uint uBufferLength);

		[DllImport("hid.dll", SetLastError = true)]
		protected static extern bool HidD_SetFeature(SafeFileHandle hFile, IntPtr Buffer, uint uBufferLength);

		/// <summary>
		/// Set the size of the USB Report IN input buffer
		/// </summary>
		/// <param name="hFile">Handle to device</param>
		/// <param name="nBuffers">Required size of buffer in USB IN Reports</param>
		/// <returns>True if successful</returns>
		[DllImport("hid.dll", SetLastError = true)]
		protected static extern bool HidD_SetNumInputBuffers(IntPtr hFile, uint nBuffers);

		/// <summary>
		/// Get the indexed string. (MMc added)
		/// Remember to initialise the string first so that it has some size, e.g. StringBuilder myValue = new StringBuilder("", 256);
		/// </summary>
		/// <param name="hFile">Handle to HID device</param>
		/// <param name="ulStringIndex">Index for string</param>
		/// <param name="pvBuffer">Buffer for string</param>
		/// <param name="ulBufferLength">Size of buffer</param>
		/// <returns></returns>
		[DllImport("hid.dll", SetLastError = true)]
		protected static extern bool HidD_GetIndexedString(SafeFileHandle hFile, uint uStringIndex, IntPtr Buffer, uint uBufferLength );

		[DllImport("hid.dll", SetLastError = true)]
		protected static extern bool HidD_GetInputReport(IntPtr hFile, IntPtr Buffer, uint uBufferLength);


		[DllImport("hid.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		internal static extern bool HidD_GetProductString(IntPtr hDevice, IntPtr Buffer, uint BufferLength); 
	
		/// <summary>
		/// Creates/opens a file, serial port, USB device... etc
		/// </summary>
		/// <param name="strName">Path to object to open</param>
		/// <param name="nAccess">Access mode. e.g. Read, write</param>
		/// <param name="nShareMode">Sharing mode</param>
		/// <param name="lpSecurity">Security details (can be null)</param>
		/// <param name="nCreationFlags">Specifies if the file is created or opened</param>
		/// <param name="nAttributes">Any extra attributes? e.g. open overlapped</param>
		/// <param name="lpTemplate">Not used</param>
		/// <returns></returns>
		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		protected static extern SafeFileHandle CreateFile(string strName, uint nAccess, uint nShareMode, IntPtr lpSecurity, uint nCreationFlags, uint nAttributes, IntPtr lpTemplate);
		/// <summary>
		/// Closes a window handle. File handles, event handles, mutex handles... etc
		/// </summary>
		/// <param name="hFile">Handle to close</param>
		/// <returns>True if successful.</returns>
		[DllImport("kernel32.dll", SetLastError = true)] protected static extern int CloseHandle(IntPtr hFile);
        #endregion

		#region Public methods
		/// <summary>
		/// Registers a window to receive windows messages when a device is inserted/removed. Need to call this
		/// from a form when its handle has been created, not in the form constructor. Use form's OnHandleCreated override.
		/// </summary>
		/// <param name="hWnd">Handle to window that will receive messages</param>
		/// <param name="gClass">Class of devices to get messages for</param>
		/// <returns>A handle used when unregistering</returns>
        public static IntPtr RegisterForUsbEvents(IntPtr hWnd, Guid gClass)
        {
			DeviceBroadcastInterface oInterfaceIn = new DeviceBroadcastInterface();
            oInterfaceIn.Size = Marshal.SizeOf(oInterfaceIn);
			oInterfaceIn.ClassGuid = gClass;
			oInterfaceIn.DeviceType = DEVTYP_DEVICEINTERFACE;
            oInterfaceIn.Reserved = 0;
			return RegisterDeviceNotification(hWnd, oInterfaceIn, DEVICE_NOTIFY_WINDOW_HANDLE);
        }

		/// <summary>
		/// Unregisters notifications. Can be used in form dispose
		/// </summary>
		/// <param name="hHandle">Handle returned from RegisterForUSBEvents</param>
		/// <returns>True if successful</returns>
        public static bool UnregisterForUsbEvents(IntPtr hHandle)
        {
            return UnregisterDeviceNotification(hHandle);
        }

		/// <summary>
		/// Helper to get the HID guid.
		/// </summary>
        public static Guid HIDGuid
        {
            get
            {
                Guid gHid;
                HidD_GetHidGuid(out gHid);
                return gHid;
            }
        }
		#endregion
    }
} 