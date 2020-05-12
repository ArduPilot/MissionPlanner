/*
 * This module was originally posted on CodeProject in an article titled
 * ST Micro-electronics Device Firmware Upgrade (DFU/DfuSe) from C#
 * By Mark McLean (ExpElec), 4 Mar 2013 
 * http://www.codeproject.com/Tips/540963/ST-Micro-electronics-Device-Firmware-Upgrade-DFU-D
 * 
 * and is licensed under the The Code Project Open License (CPOL) 1.02,
 * which can be found here
 * http://www.codeproject.com/info/cpol10.aspx
 * 
 * Revision History:
 * 12/10/2013 Brien Schultz: Added support for my STM32F405VGT6
 * 12/10/2013 Brien Schultz: Added the "Exit DFU Mode" option to the UpdateFirmware method.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace NetDFULib
{
    public class FirmwareUpdate : Win32Usb, IFirmwareUpdate
	{
        UInt16 HID_VID = 0x1234;
        UInt16 HID_PID = 0x5678;
		const byte HID_DETACH_REPORT_ID = 0x80;
		const byte USAGE_DETACH = 0x55;
		const uint STDFU_ERROR_OFFSET = 0x12340000;
		const uint STDFU_NOERROR = STDFU_ERROR_OFFSET + 0;

		const byte STATE_IDLE = 0x00;
		const byte STATE_DETACH = 0x01;
		const byte STATE_DFU_IDLE = 0x02;
		const byte STATE_DFU_DOWNLOAD_SYNC = 0x03;
		const byte STATE_DFU_DOWNLOAD_BUS = 0x04;
		const byte STATE_DFU_DOWNLOAD_IDLE = 0x05;
		const byte STATE_DFU_MANIFEST_SYNC = 0x06;
		const byte STATE_DFU_MANIFEST = 0x07;
		const byte STATE_DFU_MANIFEST_WAIT_RESET = 0x08;
		const byte STATE_DFU_UPLOAD_IDLE = 0x09;
		const byte STATE_DFU_ERROR = 0x0A;
		const byte STATE_DFU_UPLOAD_SYNC = 0x91;
		const byte STATE_DFU_UPLOAD_BUSY = 0x92;

		IntPtr INVALID_HANDLE_VALUE = (System.IntPtr)(-1);
		
		Guid GUID_DFU = new Guid( 0x3fe809ab, 0xfb91, 0x4cb5, 0xa6, 0x43, 0x69, 0x67, 0x0d, 0x52, 0x36, 0x6e );
		Guid GUID_APP = new Guid( 0xcb979912, 0x5029, 0x420a, 0xae, 0xb1, 0x34, 0xfc, 0x0a, 0x7d, 0x57, 0x26 );

		/// <summary>Thread to run the firmware update process</summary>
		private Thread thread = null;

		/// <summary>Event handler for firmware update progress event</summary>
		public event FirmwareUpdateProgressEventHandler OnFirmwareUpdateProgress;

		private String DFU_FilePath = "";

        private String DFU_DevicePath = "";

        /// <summary>Handle to open HID device, if any</summary>
        SafeFileHandle _ParentHandle = null;

		/// <summary>List of classes describing each programmable sector of the micro-controller</summary>
		private List<MappingSector> Sectors;

		/// <summary>Maximum size of a block of data for writing, this is set depending on the bootloader version</summary>
		private UInt16 MaxWriteBlockSize = 1024;

		/// <summary>Set true to request that the firmware update process start with a mass erase of the micro-controller
		/// This is not suitable unless the DFU module is in ROM because it would erase the bootloader itself.</summary>
		private bool DoMassErase;

        /// <summary>True to exit DFU mode after the update.</summary>
        public bool ExitDFUMode = false;

		public void SetVIDPIDforHID ( UInt16 VID, UInt16 PID )
		{
			HID_VID = VID;
			HID_PID = PID;
		}
		public bool IsDFUDeviceFound()
		{
			List<MappingSector> sectors;
			ushort nMaxBlkSize;
			IntPtr hDevice = IntPtr.Zero;
			hDevice = OpenDFU_Device(out sectors, out nMaxBlkSize);		//XXX does this hDevice leak?
			return IntPtr.Zero != hDevice;
		}

		/// <summary>
		/// Update the device firmware with the referenced DFU file data
		/// </summary>
		/// <param name="DFU_FilePath">Full path of DFU file for new firmware</param>
		/// <param name="DoMassErase">True to mass erase the device, false to erase by sectors.
		/// Unless the DFU module is in ROM, the device should be erased by sectors or else the DFU module will be erased</param>
        /// <param name="ExitDFUMode">True to exit DFU mode after the update.</param>
		public void UpdateFirmware(String DFU_FilePath, bool DoMassErase, bool ExitDFUMode)
		{
            if (thread != null)
            {
                throw new Exception("Firmware Update Thread already running.");
            }
			this.DFU_FilePath = DFU_FilePath;
			this.DoMassErase = DoMassErase;
            this.ExitDFUMode = ExitDFUMode;
			thread =  new Thread( DoFirmwareUpdate );
			thread.Name = "Firmware update thread";
			thread.Start();
		}

		/// <summary>
		/// Mass erase a device.  
		/// Useful for a device which has been read protected, otherwise it can't be programmed with a ULink.
		/// Obviously erases the DFU module unless it is in ROM
		/// Assumes the device is already in DFU mode.
		/// </summary>
		public void MassErase()
		{
			IntPtr hDevice = IntPtr.Zero;

			try
			{
				if (OnFirmwareUpdateProgress != null) OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(0, "Detecting device", false));
				FindAndDetachHID();
				if (OnFirmwareUpdateProgress != null) OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(10, "Opening the device", false));
				hDevice = OpenDFU_Device(out Sectors, out MaxWriteBlockSize);		//XXX does this hDevice leak?
				if( OnFirmwareUpdateProgress != null ) OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(20, "Performing mass erase", false));
				MassErase(hDevice);
				if( OnFirmwareUpdateProgress != null ) OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(100, "Mass erase complete", false));

			}
			catch (Exception ex)
			{
				if( OnFirmwareUpdateProgress != null ) OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(100, ex.Message, true));
			}
		}

		/// <summary>
		/// Function to sequence the firmware update
		/// </summary>
		private void DoFirmwareUpdate()
		{
			int i;
			byte[] FileData = new byte[0];
			IntPtr hDevice = IntPtr.Zero;

			//XXX we are going to assume always target 0; support picking
			//target (a 'target' is for making an uber dfu, presumably
			//supporting multiple boards, like Netduino 2, Netduino Plus 2,
			//Netduino Go, Netduino Shield Base, etc) in the same DFU.
			int nIdxTarget = 0;	//could parameterize, but really, I think
			//the best thing would be to separate the DFU parsing out of this
			//function; then the UI can present all the DFU details, which
			//the user can then selectively pick, like 'only program this
			//element' or whatnot.

			try
			{
				if( OnFirmwareUpdateProgress != null ) OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(0, "Detecting device", false));

				FindAndDetachHID();

				if( OnFirmwareUpdateProgress != null ) OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(2, "Loading the DFU file", false));
				DFUFile dfuFile;
				LoadDFU_File(FileData, out dfuFile);
				//LoadDFU_File( FileData, out ElementData, out ElementAddress, out ElementSize);
				if( OnFirmwareUpdateProgress != null ) OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(3, "Opening the device", false));
				hDevice = OpenDFU_Device(out Sectors, out MaxWriteBlockSize);		//XXX does this hDevice leak?
				if (DoMassErase)
				{
					if( OnFirmwareUpdateProgress != null ) OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(4, "Performing mass erase", false));
					MassErase(hDevice);
					if( OnFirmwareUpdateProgress != null ) OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(5, "Mass erase complete", false));
				}
				else
				{
					if( OnFirmwareUpdateProgress != null ) OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(4, "Performing partial erase", false));
					//erase only the sections we will program
					//XXX support more selective burn options, like only programming specific sections (elements)
					DFUTarget dfuTarErase = dfuFile.atargets[nIdxTarget];
					for (int nIdxElem = 0; nIdxElem < dfuTarErase.aelements.Length; ++nIdxElem)
					{
						DFUElement dfuElem = dfuTarErase.aelements[nIdxElem];
						//XXX break Sectors out of this object; give UI ability to present to user in interesting ways
						PartialErase(hDevice, dfuElem.dwAddr, (uint)dfuElem.abyData.Length, Sectors);
					}
					if( OnFirmwareUpdateProgress != null ) OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(5, "Partial erase complete", false));
				}
				System.Threading.Thread.Sleep(500);

				//XXX support more selective burn options, like only programming specific sections (elements)
				DFUTarget dfuTarBurn = dfuFile.atargets[nIdxTarget];
				for (int nIdxElem = 0; nIdxElem < dfuTarBurn.aelements.Length; ++nIdxElem)
				{
					DFUElement dfuElem = dfuTarBurn.aelements[nIdxElem];

					// Write the data in MaxWriteBlockSize (2048byte) blocks
					for (UInt32 BlockNumber = 0; BlockNumber <= (uint)dfuElem.abyData.Length / MaxWriteBlockSize; BlockNumber++)
					{
						if( OnFirmwareUpdateProgress != null ) OnFirmwareUpdateProgress(this,
								new FirmwareUpdateProgressEventArgs(10 + (80 * BlockNumber * MaxWriteBlockSize / dfuElem.abyData.Length),
								"Writing block " + BlockNumber.ToString(), false));
						// Get the data for write and massage it into a 2048 byte block
						byte[] Block = dfuElem.abyData.Skip((int)(MaxWriteBlockSize * BlockNumber)).Take((int)MaxWriteBlockSize).ToArray();
						if (Block.Length < MaxWriteBlockSize)
						{
							i = Block.Length;
							Array.Resize(ref Block, (int)MaxWriteBlockSize);
							// Pad with 0xFF so our CRC matches the ST Bootloader and the ULink's CRC
							for (; i < MaxWriteBlockSize; i++)
							{
								Block[i] = 0xFF;
							}
						}
						WriteBlock(hDevice, dfuElem.dwAddr, Block, BlockNumber);
					}
				}
                  
				if ( OnFirmwareUpdateProgress != null ) OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(95, "Restarting", false));
			
	            if (ExitDFUMode)
                {
                    //XXX support doing this optionally; also, we are going to assume the 'jump to' address is that of the clr. hmmm.  there must be a better, fixed, address.
				        Detach(hDevice, 0x08020000);	//this makes me feel too dirty, so I'm turning it off for now
                }

				if ( OnFirmwareUpdateProgress != null ) OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(100, "Programming complete", false));
			}
			catch (Exception ex)
			{
				if( OnFirmwareUpdateProgress != null ) OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(100, ex.Message, true));
			}
            thread = null;
		}

		/// <summary>
		/// Look for a HID device with the expected VID and PID, if found, send the detach message
		/// </summary>
		private void FindAndDetachHID()
		{
			int i;

			// If we are connected to a device with the expected VID and PID, send the detach message
			if (FindDevice(HID_VID, HID_PID))
			{
				//byte[] Feature = new byte[65];

				// Assign a buffer in unmananged memory
				IntPtr Feature = Marshal.AllocHGlobal(65);
				Marshal.WriteByte(Feature, 0, HID_DETACH_REPORT_ID);
				Marshal.WriteByte(Feature, 1, USAGE_DETACH);
				for (i = 2; i < 65; i++)
				{
					Marshal.WriteByte(Feature, i, 0);
				}
				if (HidD_SetFeature(_ParentHandle, Feature, 65))
				{
					if (OnFirmwareUpdateProgress != null) OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(1, "HID detach success", false));
				}
				else
				{
					// SensiPOD usually replies to this with 31, ERROR_GEN_FAILURE, but it still works...
					if (OnFirmwareUpdateProgress != null) OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(1, "HID detach error = " + (Marshal.GetLastWin32Error()).ToString(), false));
					//throw (new Exception("HID detach failed"));
				}
				Marshal.FreeHGlobal(Feature);
				System.Threading.Thread.Sleep(5000);
			}
		}

		/// <summary>
		/// Function to find a device with a given VID and PID
		/// </summary>
		/// <param name="nVid">VID to search for</param>
		/// <param name="nPid">PID to search for</param>
		/// <returns>True if matching device is found</returns>
		public bool FindDevice(UInt16 nVid, UInt16 nPid)
		{
			String strDevicePath;
			string strPath = string.Empty;
			string strSearch = string.Format("vid_{0:x4}&pid_{1:x4}", nVid, nPid); // first, build the path search string
			Guid gHid;
			HidD_GetHidGuid(out gHid);	// next, get the GUID from Windows that it uses to represent the HID USB interface
			IntPtr hInfoSet = SetupDiGetClassDevs(ref gHid, null, IntPtr.Zero, DIGCF_DEVICEINTERFACE | DIGCF_PRESENT);	// this gets a list of all HID devices currently connected to the computer (InfoSet)
			try
			{
				DeviceInterfaceData oInterface = new DeviceInterfaceData();	// build up a device interface data block
				oInterface.Size = Marshal.SizeOf(oInterface);				// 28 in 32bit or 32 in 64bit mode, 
				// Now iterate through the InfoSet memory block assigned within Windows in the call to SetupDiGetClassDevs
				// to get device details for each device connected
				int nIndex = 0;
				while (SetupDiEnumDeviceInterfaces(hInfoSet, 0, ref gHid, (uint)nIndex, ref oInterface))	// this gets the device interface information for a device at index 'nIndex' in the memory block
				{
					strDevicePath = GetDevicePath(hInfoSet, ref oInterface);	// get the device path (see helper method 'GetDevicePath')
					if (strDevicePath.IndexOf(strSearch) >= 0)	// do a string search, if we find the VID/PID string then we found our device!
					{
						return true;
					}
					nIndex++;	// if we get here, we didn't find our device. So move on to the next one.
				}
				if (0 != Marshal.GetLastWin32Error())
				{
					if (ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error())
					{
						// Do nothing, this just means the ARM_Board wasn't found
					}
					else if (ERROR_INVALID_USER_BUFFER == Marshal.GetLastWin32Error())
					{
						throw new Exception("Size member of hInfoSet is not set correctly (5 for 32bit or 8 for 64bit)");
					}
					else
					{
						throw new Exception("SetupDiEnumDeviceInterfaces returned error " + Marshal.GetLastWin32Error().ToString());
					}
				}
			}
			finally
			{
				// Before we go, we have to free up the InfoSet memory reserved by SetupDiGetClassDevs
				SetupDiDestroyDeviceInfoList(hInfoSet);
			}
			return false;	// oops, didn't find our device
		}

		/// <summary>
		/// Helper method to return the device path given a DeviceInterfaceData structure and an InfoSet handle.
		/// Used in 'FindDevice' so check that method out to see how to get an InfoSet handle and a DeviceInterfaceData.
		/// </summary>
		/// <param name="hInfoSet">Handle to the InfoSet</param>
		/// <param name="oInterface">DeviceInterfaceData structure</param>
		/// <returns>The device path or null if there was some problem</returns>
		private string GetDevicePath(IntPtr hInfoSet, ref DeviceInterfaceData oInterface)
		{
			uint nRequiredSize = 0;
			// Get the device interface details
			if (!SetupDiGetDeviceInterfaceDetail(hInfoSet, ref oInterface, IntPtr.Zero, 0, ref nRequiredSize, IntPtr.Zero))
			{
				DeviceInterfaceDetailData oDetail = new DeviceInterfaceDetailData();
				if (IntPtr.Size == 8)	// If we are compiled as 64bit
				{
					oDetail.Size = 8;
				}
				else if (IntPtr.Size == 4) // If we are compiled as 32 bit
				{
					oDetail.Size = 5;
				}
				else
				{
					throw new Exception("Operating system is neither 32 nor 64 bits!");
				}
				if (SetupDiGetDeviceInterfaceDetail(hInfoSet, ref oInterface, ref oDetail, nRequiredSize, ref nRequiredSize, IntPtr.Zero))
				{
					return oDetail.DevicePath;
				}
			}
			return null;
		}



		#region DFU File Parsing

		#region DFU on-disk structures
		//The DFU format is fairly simple, consisting of a sequence of structs;
		//DFUPrefix{1}
		//DFUImage{*}, consisting of:
		//  DFUImageTargetPrefix{1}
		//  DFUImageImageElement{*}
		//DFUSuffix{1}
		//the doc claims (or seems to claim) the structs are bigendian, but they
		//sure look little endian to me!

		//generic method to marshal a section of a byte array into a struct.
		//too bad C# does not have a cast, haha.
		static void StructFromBytes<T>(ref T pstruct, byte[] abyBuff, int nOffset)
		{
			int nLen = Marshal.SizeOf(pstruct);
			IntPtr pbyMarsh = Marshal.AllocHGlobal(nLen);
			Marshal.Copy(abyBuff, nOffset, pbyMarsh, nLen);
			pstruct = (T)Marshal.PtrToStructure(pbyMarsh, pstruct.GetType());
			Marshal.FreeHGlobal(pbyMarsh);
		}

		//start of a DFU file contains this general information
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		unsafe struct DFUPrefix_ods
		{
			public fixed byte abySig[5];			//"DfuSe"
			public byte byVersion;					//how to interpret structs?  0x01
			public UInt32 dwDFUImageSize;			//sum of the image sections between this and the suffix
			public byte byNumTargets;				//number of 'targets' that follows
		}

		//start of a 'target' has this info, which describes the target itself and
		//the elements that follow.
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		unsafe struct DFUImageTarget_ods
		{
			public fixed byte abySig[6];			//"Target"
			public byte byAltSetting;				//que?
			public UInt32 bIsNamed;					//if the following is valid
			public fixed byte abyTargetName[255];	//optional target name
			public UInt32 dwTargetSize;				//length of image elements (after this header), in toto
			public UInt32 dwNumElements;			//number of image elements that follows
		}

		//a block of data to be written
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		unsafe struct DFUImageElement_ods
		{
			public UInt32 dwAddress;				//start address
			public UInt32 dwSize;					//size of this block
			// ... byte[abySize];				//the data follows this struct
		}

		//this is always at the end of the file, so you can seek there and work backwards
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		unsafe struct DFUSuffix_ods
		{
			public UInt16 wVer;				//firmware version
			public UInt16 wPID;				//PID
			public UInt16 wVID;				//VID
			public UInt16 wDFUVer;			//DFU version, fixed at 011a
			public fixed byte abySig[3];	//'U'55 'F'46 'D'44
			public byte byLen;				//length of this suffix, always 16
			public UInt32 dwCRC;			//CRC of entire file, exluding this itself
		}

		#endregion

		#region DFU in-memory descriptors
		//while parsing the on-disk structures, we build a dotnet-y description of the image,
		//which just has the stuff we're interested in.
		class DFUFile
		{
			public UInt16 VID = 0;
			public UInt16 PID = 0;
			public UInt16 Version = 0;
			public DFUTarget[] atargets = null;
		}
		class DFUTarget
		{
			public String strName = null;
			public DFUElement[] aelements = null;
		}
		class DFUElement
		{
			public UInt32 dwAddr = 0xffffffff;
			public byte[] abyData = null;
		}
		#endregion


		/// <summary>
		/// Scan a DFU file to get the VID, PID and file version
		/// It is recommended that the VID and PID are checked before proceeding with the firmware update
		/// </summary>
		/// <param name="Filepath">DFU filepath</param>
		/// <param name="VID">Vendor Identifier</param>
		/// <param name="PID">Product Identifier</param>
		/// <param name="Version">DFU file firmware version</param>
		/// <returns>true if success, else false</returns>
		public bool ParseDFU_File(String Filepath, out UInt16 VID, out UInt16 PID, out UInt16 Version)
		{
			byte[] FileData;
			UInt32 CRC = 0;
			bool Retval = true;

			try
			{
				// Read the file into memory
				FileData = System.IO.File.ReadAllBytes(Filepath);

				// Check the prefix
				if (Encoding.UTF8.GetString(FileData, 0, 5) != "DfuSe")
				{
					throw new Exception("File signature error");
				}
				if (FileData[5] != 1)
				{
					throw new Exception("DFU file version must be 1");
				}
				// Check the suffix
				if ((Encoding.UTF8.GetString(FileData, FileData.Length - 8, 3) != "UFD")
					|| (FileData[FileData.Length - 5] != 16)
					|| (FileData[FileData.Length - 10] != 0x1A)
					|| (FileData[FileData.Length - 9] != 0x01))
				{
					throw new Exception("File suffix error");
				}
				// Check the CRC
				CRC = BitConverter.ToUInt32(FileData, FileData.Length - 4);
				if (CRC != CalculateCRC(FileData))
				{
					throw new Exception("File CRC error");
				}
				// Get VID, PID and version number
				VID = BitConverter.ToUInt16(FileData, FileData.Length - 12);
				PID = BitConverter.ToUInt16(FileData, FileData.Length - 14);
				Version = BitConverter.ToUInt16(FileData, FileData.Length - 16);
			}
			catch
			{
				VID = 0;
				PID = 0;
				Version = 0;
				Retval = false;
			}
			return (Retval);
		}



		/// <summary>
		/// Load a DFU file and pull out the actual firmware image
		/// </summary>
		/// <param name="FileData">Complete file as a byte array</param>
		/// <param name="ElementData">Image data only</param>
		/// <param name="ElementAddress">Starting address in the micro-controller for the image</param>
		/// <param name="ElementSize">Size of the image</param>
		private unsafe void LoadDFU_File(byte[] FileData, out DFUFile dfuFile)
		{
			dfuFile = new DFUFile();

			try
			{
				// Read the file into memory
				FileData = System.IO.File.ReadAllBytes(DFU_FilePath);

				//deperist the prefix, which is at the beginning of the file
				DFUPrefix_ods dfuPfxODS = new DFUPrefix_ods();
				StructFromBytes ( ref dfuPfxODS, FileData, 0);

				//(in)sanity check the prefix
				if (dfuPfxODS.abySig[0] == (byte)'D' &&
						dfuPfxODS.abySig[1] == (byte)'f' &&
						dfuPfxODS.abySig[2] == (byte)'u' &&
						dfuPfxODS.abySig[3] == (byte)'S' &&
						dfuPfxODS.abySig[4] == (byte)'e'
						)
				{}	//sane
				else
				{	//insane
					throw new Exception("File signature error");
				}
				
				//check DFU file format version
				if (dfuPfxODS.byVersion != 1)
				{	//insane
					throw new Exception("DFU file version must be 1");
				}
				if ( 0 != dfuPfxODS.byNumTargets )
				{}	//sane
				else
				{	//insane
					throw new Exception("File has "+dfuPfxODS.byNumTargets+" targets");
				}
				

				//depersist the suffix, which is at the end of the file
				DFUSuffix_ods dfuSfxODS = new DFUSuffix_ods();
				StructFromBytes(ref dfuSfxODS, FileData, FileData.Length - Marshal.SizeOf(dfuSfxODS));
				
				// Check the suffix
				if (dfuSfxODS.abySig[0] == (byte)'U' &&
						dfuSfxODS.abySig[1] == (byte)'F' &&
						dfuSfxODS.abySig[2] == (byte)'D'
						)
				{ }	//sane
				else
				{	//insane
					throw new Exception("File suffix signature error");
				}
				if ( 16 != dfuSfxODS.byLen ||
						0x011a != dfuSfxODS.wDFUVer )
				{	//insane
					throw new Exception("File suffix size/version error");
				}
				// Check the CRC
				if (dfuSfxODS.dwCRC != CalculateCRC(FileData))
				{	//insane
					throw new Exception("File CRC error");
				}


				// Get VID, PID and version number
				dfuFile.VID = dfuSfxODS.wVID;
				dfuFile.PID = dfuSfxODS.wPID;
				dfuFile.Version = dfuSfxODS.wVer;

				//make the targets array
				dfuFile.atargets = new DFUTarget[dfuPfxODS.byNumTargets];

				//scroll through each target, then scroll through each image element
				int nIdxTargetCursor = Marshal.SizeOf(typeof(DFUPrefix_ods));
				for (int nIdxTarget = 0; nIdxTarget < dfuFile.atargets.Length; ++nIdxTarget)
				{
					dfuFile.atargets[nIdxTarget] = new DFUTarget();	//make the object
					DFUTarget dfuTar = dfuFile.atargets[nIdxTarget];	//this target
					//depersist the target structure
					DFUImageTarget_ods dfuTarODS = new DFUImageTarget_ods();
					StructFromBytes(ref dfuTarODS, FileData, nIdxTargetCursor);
					//sanity check on the 'target' struct
					if (dfuTarODS.abySig[0] == (byte)'T' &&
							dfuTarODS.abySig[1] == (byte)'a' &&
							dfuTarODS.abySig[2] == (byte)'r' &&
							dfuTarODS.abySig[3] == (byte)'g' &&
							dfuTarODS.abySig[4] == (byte)'e' &&
							dfuTarODS.abySig[5] == (byte)'t'
							)
					{}	//good
					else
					{
						throw new Exception("Target "+nIdxTarget+" at loc "+nIdxTargetCursor+" failed sanity check");
					}

					dfuTar.strName = "---";	//XXX until I figure out how to convert dfuTar.abyTargetName
					//string str = Encoding.ASCII.GetString ( dfuTar.abyTargetName, 0, dfuTar.len )
					if (0 == dfuTarODS.dwNumElements)
					{	//insane
						throw new Exception("target " + nIdxTarget + " has " + dfuTarODS.dwNumElements + " elements");
					}
					else
					{	//sane
						dfuTar.aelements = new DFUElement[dfuTarODS.dwNumElements];	//make elements
						//scroll through each element
						int nIdxElementCursor = nIdxTargetCursor + Marshal.SizeOf(typeof(DFUImageTarget_ods));
						for (int nIdxElement = 0; nIdxElement < dfuTar.aelements.Length; ++nIdxElement)
						{
							dfuTar.aelements[nIdxElement] = new DFUElement();	//make the object
							DFUElement dfuElem = dfuTar.aelements[nIdxElement];	//this element
							//depersist the element structure
							DFUImageElement_ods dfuElemODS = new DFUImageElement_ods();
							StructFromBytes(ref dfuElemODS, FileData, nIdxElementCursor);
							//sanity check on the 'element' struct
							//(XXX there is no sanity check)
							dfuElem.dwAddr = dfuElemODS.dwAddress;
							dfuElem.abyData = FileData.Skip(nIdxElementCursor + Marshal.SizeOf(typeof(DFUImageElement_ods))).Take((int)dfuElemODS.dwSize).ToArray();
							nIdxElementCursor += Marshal.SizeOf(typeof(DFUImageElement_ods)) + (int)dfuElemODS.dwSize;	//next element...
						}
					}
					nIdxTargetCursor += Marshal.SizeOf(typeof(DFUImageTarget_ods)) + (int)dfuTarODS.dwTargetSize;	//next target...
				}

				if (OnFirmwareUpdateProgress != null) OnFirmwareUpdateProgress(this,
							new FirmwareUpdateProgressEventArgs(5, "DFU file parsed with " + dfuFile.atargets.Length + " elements", false));
			}
			catch (Exception ex)
			{
				throw new Exception("DFU file read failed. " + ex.Message);
			}
		}



		/// <summary>
		/// Helper function to calculate the CRC
		/// POSIX.2 checksum
		/// </summary>
		/// <param name="FileData">Data to calculate the CRC for</param>
		/// <returns>CRC</returns>
		private UInt32 CalculateCRC(byte[] FileData)
		{
			UInt32 Retval = 0xFFFFFFFF;
			int i;

			for (i = 0; i < FileData.Length - 4; i++)
			{
				Retval = CrcTable[((Retval) ^ (FileData[i])) & 0xff] ^ ((Retval) >> 8);
			}

			return (Retval);
		}

		#endregion

		/// <summary>
		/// Open the connection to the single DFU device which is attached to this computer.  Also parse the sector descriptions to build a list of sectors.
		/// </summary>
		/// <returns>Handle to the device</returns>
		private IntPtr OpenDFU_Device(out List<MappingSector> Sectors, out UInt16 MaxWriteBlockSize)
		{
			int i = 10;
			uint Index = 0;
			Guid GUID = GUID_DFU;
			DeviceInterfaceData ifData = new DeviceInterfaceData();
			ifData.Size = Marshal.SizeOf(ifData);
			DeviceInterfaceDetailData ifDetail = new DeviceInterfaceDetailData();
			UInt32 Size = 0;
			IntPtr hInfoSet = SetupDiGetClassDevs(ref GUID, null, IntPtr.Zero, DIGCF_DEVICEINTERFACE | DIGCF_PRESENT);	// this gets a list of all DFU devices currently connected to the computer (InfoSet)
			IntPtr hDevice = IntPtr.Zero;

			Sectors = null;
			MaxWriteBlockSize = 1024;

			try
			{
				if (hInfoSet == INVALID_HANDLE_VALUE)
				{
					throw new Exception("SetupDiGetClassDevs returned error=" + Marshal.GetLastWin32Error().ToString()); 
				}

				// Loop ten times hoping to find exactly one DFU device
				while (i-- > 0)
				{
					Index = 0;
					while (SetupDiEnumDeviceInterfaces(hInfoSet, 0, ref GUID, Index, ref ifData))
					{
						Index++;
					}
					if (0 == Index)
					{
						System.Threading.Thread.Sleep(500);
					}
					else
					{
						break;
					}
				}

				if (1 == Index)
				{
					SetupDiEnumDeviceInterfaces(hInfoSet, 0, ref GUID, 0, ref ifData);
					SetupDiGetDeviceInterfaceDetail(hInfoSet, ref ifData, IntPtr.Zero, 0, ref Size, IntPtr.Zero);
					//					ifDetail.Size = (int)Size;
					if (IntPtr.Size == 8)	// If we are compiled as 64bit
					{
						ifDetail.Size = 8;
					}
					else if (IntPtr.Size == 4) // If we are compiled as 32 bit
					{
						ifDetail.Size = 5;
					}
					else
					{
						throw new Exception("Operating system is neither 32 nor 64 bits!");
					}
					if( Marshal.SizeOf(ifDetail) < Size )
					{
						throw new Exception("ifDetail too small");
					}
					if (true == SetupDiGetDeviceInterfaceDetail(hInfoSet, ref ifData, ref ifDetail, Size, ref Size, IntPtr.Zero))
					{
						DFU_DevicePath = ifDetail.DevicePath.ToUpper();
						if (STDFU_NOERROR == STDFU_Open(DFU_DevicePath, out hDevice))
						{
							if( OnFirmwareUpdateProgress != null ) OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(10, "DFU device opened OK", false));
							USB_DeviceDescriptor Descriptor = new USB_DeviceDescriptor();
							if (STDFU_NOERROR == STDFU_GetDeviceDescriptor(ref hDevice, ref Descriptor))
							{
								switch (Descriptor.bcdDevice)
								{
									case 0x011A:
									case 0x0200:
										MaxWriteBlockSize = 1024;
                                        break;
                                    //-------------------------------------------------------
                                    // 12/10/2013 Brien Schultz: Added support for my STM32F405VGT
                                    //-------------------------------------------------------
                                    case 0x2200:
                                        MaxWriteBlockSize = 2048;
                                        break;
                                    //-------------------------------------------------------
									case 0x02100:
										MaxWriteBlockSize = 2048;
										break;
									default:
										throw new Exception("Unsupported bootloader version=" + Descriptor.bcdDevice.ToString("X4"));
								}

								UInt32 Dummy1 = 0;
								UInt32 Dummy2 = 0;
								DFU_FunctionalDescriptor CurrentDeviceDescriptor = new DFU_FunctionalDescriptor();
								if (STDFU_NOERROR == STDFU_GetDFUDescriptor(ref hDevice, ref Dummy1, ref Dummy2, ref CurrentDeviceDescriptor))
								{
									if( OnFirmwareUpdateProgress != null ) OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(14, "Got DFU Descriptor " + CurrentDeviceDescriptor.bcdDFUVersion.ToString(), false));
									Sectors = CreateMappingFromDevice(hDevice);
								}
								else
								{
									throw new Exception("STDFU_GetDFUDescriptor failed, error code=" + Marshal.GetLastWin32Error().ToString());
								}
							}
							else
							{
								throw new Exception("STDFU_GetDeviceDescriptor failed, error code=" + Marshal.GetLastWin32Error().ToString());
							}
						}
						else
						{
							throw new Exception("STDFU_Open failed, error code=" + Marshal.GetLastWin32Error().ToString());
						}
					}
				}
				else
				{
					throw new Exception("No devices in DFU mode were found.");
				}
			}
			catch( Exception ex )
			{
				throw new Exception(ex.Message);
//				del(100, ex.Message, true);
			}
			finally
			{
				// Before we go, we have to free up the InfoSet memory reserved by SetupDiGetClassDevs
				SetupDiDestroyDeviceInfoList(hInfoSet);
			}
			return (hDevice);
		}


		/// <summary>
		/// Mass erase the micro-controller
		/// </summary>
		/// <param name="hDevice">Handle to the USB connection to the micro-controller</param>
		private void MassErase(IntPtr hDevice)
		{
			DFU_Status dfuStatus = new DFU_Status();
			UInt32 Result = 0;
			byte[] EraseCommand = { 0x41, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

			if (STDFU_NOERROR == (Result = STDFU_SelectCurrentConfiguration( ref hDevice, 0, 0, 1)))
			{
				STDFU_GetStatus(ref hDevice, ref dfuStatus);
				while (dfuStatus.bState != STATE_DFU_IDLE)
				{
					STDFU_ClrStatus(ref hDevice);
					STDFU_GetStatus(ref hDevice, ref dfuStatus);
				}
				if (STDFU_NOERROR == (Result = STDFU_Dnload(ref hDevice, EraseCommand, 1, 0)))
				{
					STDFU_GetStatus(ref hDevice, ref dfuStatus);
					while (dfuStatus.bState != STATE_DFU_IDLE)
					{
						STDFU_ClrStatus(ref hDevice);
						STDFU_GetStatus(ref hDevice, ref dfuStatus);
					}
				}
				else
				{
					throw new Exception("STDFU_Dnload returned " + Result.ToString("X8"));
				}
			}
			else
			{
				throw new Exception("STDFU_SelectCurrentConfiguration returned " + Result.ToString("X8"));
			}
		}

		private void PartialErase(IntPtr hDevice, UInt32 StartAddress, UInt32 Size, List<MappingSector> SectorList)
		{
			foreach (MappingSector s in SectorList)
			{
				if ((StartAddress < s.dwStartAddress + s.dwSectorSize) && (StartAddress + Size > s.dwStartAddress))
				{
					EraseSector(hDevice, s.dwStartAddress);
				}
			}
		}

		/// <summary>
		/// Erase a sector
		/// </summary>
		/// <param name="hDevice">Device handle</param>
		/// <param name="Address">Start address of sector for erase</param>
		private void EraseSector(IntPtr hDevice, UInt32 Address)
		{
			DFU_Status dfuStatus = new DFU_Status();
			UInt32 Result = 0;
			byte[] Command = { 0x41, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

			Command[1] = (byte)(Address & 0xFF);
			Command[2] = (byte)((Address >> 8) & 0xFF);
			Command[3] = (byte)((Address >> 16) & 0xFF);
			Command[4] = (byte)((Address >> 24) & 0xFF);

			if (STDFU_NOERROR == (Result = STDFU_SelectCurrentConfiguration(ref hDevice, 0, 0, 0)))
			{
				STDFU_GetStatus(ref hDevice, ref dfuStatus);
				while (dfuStatus.bState != STATE_DFU_IDLE)
				{
					STDFU_ClrStatus(ref hDevice);
					STDFU_GetStatus(ref hDevice, ref dfuStatus);
				}
				if (STDFU_NOERROR == (Result = STDFU_Dnload(ref hDevice, Command, 5, 0)))
				{
					STDFU_GetStatus(ref hDevice, ref dfuStatus);
					while (dfuStatus.bState != STATE_DFU_IDLE)
					{
						STDFU_ClrStatus(ref hDevice);
						STDFU_GetStatus(ref hDevice, ref dfuStatus);
					}
				}
				else
				{
					throw new Exception("STDFU_Dnload returned " + Result.ToString("X8"));
				}
			}
			else
			{
				throw new Exception("STDFU_SelectCurrentConfiguration returned " + Result.ToString("X8"));
			}
		}

		/// <summary>
		/// Write a block of data to the micro-controller
		/// </summary>
		/// <param name="hDevice">Handle to the USB connection to the micro-controller</param>
		/// <param name="Address">Address to write the data to</param>
		/// <param name="Data">Data for write</param>
		/// <param name="BlockNumber">Data block number</param>
		private void WriteBlock(IntPtr hDevice, UInt32 Address, byte[] Data, UInt32 BlockNumber)
		{
			byte[] Command = new byte[5];
			DFU_Status dfuStatus = new DFU_Status();

			if (Data.Length > MaxWriteBlockSize)
			{
				throw new Exception("Block size too big (" + Data.Length.ToString() + ")");
			}

			if (0 == BlockNumber)
			{
				SetAddressPointer(hDevice, Address);
			}

			STDFU_GetStatus(ref hDevice, ref dfuStatus);
			while (dfuStatus.bState != STATE_DFU_IDLE)
			{
				STDFU_ClrStatus(ref hDevice);
				STDFU_GetStatus(ref hDevice, ref dfuStatus);
			}

			STDFU_Dnload(ref hDevice, Data, (UInt32)Data.Length, (UInt16)(BlockNumber + 2));

			STDFU_GetStatus(ref hDevice, ref dfuStatus);
			while (dfuStatus.bState != STATE_DFU_IDLE)
			{
				STDFU_ClrStatus(ref hDevice);
				STDFU_GetStatus(ref hDevice, ref dfuStatus);
			}
		}

		/// <summary>
		/// Set the address pointer used in the DFU module within the micro-controller
		/// </summary>
		/// <param name="hDevice">Handle to the USB connection to the micro-controller</param>
		/// <param name="Address">Value to set address pointer to</param>
		private void SetAddressPointer(IntPtr hDevice, UInt32 Address)
		{
			byte[] Command = new byte[5];
			DFU_Status dfuStatus = new DFU_Status();

			STDFU_GetStatus(ref hDevice, ref dfuStatus);
			while (dfuStatus.bState != STATE_DFU_IDLE)
			{
				STDFU_ClrStatus(ref hDevice);
				STDFU_GetStatus(ref hDevice, ref dfuStatus);
			} 
			Command[0] = 0x21;
			Command[1] = (byte)(Address & 0xFF);
			Command[2] = (byte)((Address >> 8) & 0xFF);
			Command[3] = (byte)((Address >> 16) & 0xFF);
			Command[4] = (byte)((Address >> 24) & 0xFF);

			STDFU_Dnload(ref hDevice, Command, 5, 0);

			STDFU_GetStatus(ref hDevice, ref dfuStatus);
			while (dfuStatus.bState != STATE_DFU_IDLE)
			{
				STDFU_ClrStatus(ref hDevice);
				STDFU_GetStatus(ref hDevice, ref dfuStatus);
			}
		}

		/// <summary>
		/// Disconnect the DFU mode USB connection and request the micro-controller to jump to a given address
		/// Used after firmware update has completed to run the new firmware.
		/// </summary>
		/// <param name="hDevice">Handle to the USB connection to the micro-controller</param>
		/// <param name="Address">Address to jump to</param>
		private void Detach(IntPtr hDevice, UInt32 Address)
		{
			byte[] Command = new byte[5];
			DFU_Status dfuStatus = new DFU_Status();

			STDFU_GetStatus(ref hDevice, ref dfuStatus);
			while (dfuStatus.bState != STATE_DFU_IDLE)
			{
				STDFU_ClrStatus(ref hDevice);
				STDFU_GetStatus(ref hDevice, ref dfuStatus);
			}
			Command[0] = 0x21;
			Command[1] = (byte)(Address & 0xFF);
			Command[2] = (byte)((Address >> 8) & 0xFF);
			Command[3] = (byte)((Address >> 16) & 0xFF);
			Command[4] = (byte)((Address >> 24) & 0xFF);

			// Set the command pointer to the new application base address
			STDFU_Dnload(ref hDevice, Command, 5, 0);

			STDFU_GetStatus(ref hDevice, ref dfuStatus);
			while (dfuStatus.bState != STATE_DFU_IDLE)
			{
				STDFU_ClrStatus(ref hDevice);
				STDFU_GetStatus(ref hDevice, ref dfuStatus);
			}

			// Issue the DFU detach command
			STDFU_Dnload(ref hDevice, Command, 0, 0);

			STDFU_GetStatus(ref hDevice, ref dfuStatus);
			STDFU_ClrStatus(ref hDevice);
			STDFU_GetStatus(ref hDevice, ref dfuStatus);
		}

		/// <summary>
		/// Create a list of the programmable sectors within the micro-controller, based on the string(s) which we read from the device
		/// </summary>
		/// <param name="hDevice">Handle to the USB connection to the micro-controller</param>
		/// <returns>List of sector classes</returns>
		private List<MappingSector> CreateMappingFromDevice(IntPtr hDevice)
		{
			List<MappingSector> Sectors = new List<MappingSector>();
			UInt32 Result = 0;
			UInt32 InterfaceIndex = 0;
			UInt32 NumberOfAlternates = 0;
			DFU_FunctionalDescriptor dfuDescriptor = new DFU_FunctionalDescriptor();
			USB_InterfaceDescriptor usbDescriptor = new USB_InterfaceDescriptor();
			uint i = 0;
			IntPtr StringBuffer = Marshal.AllocHGlobal(512);
			String MapDesc;

			if (STDFU_NOERROR == (Result = STDFU_GetDFUDescriptor(ref hDevice, ref InterfaceIndex, ref NumberOfAlternates, ref dfuDescriptor)))
			{
				// Loop thru Internal FLASH, Option bytes, OTP and Device Feature
				for (i = 0; i < NumberOfAlternates; i++)
				{
					if (STDFU_NOERROR == (Result = STDFU_GetInterfaceDescriptor(ref hDevice, 0, InterfaceIndex, i, ref usbDescriptor)))
					{
						if (0 == usbDescriptor.iInterface)
						{
							throw new Exception("STDFU_GetInterfaceDescriptor bad value in iInterface");
						}
						if (STDFU_NOERROR == (Result = STDFU_GetStringDescriptor(ref hDevice, usbDescriptor.iInterface, StringBuffer, 512)))
						{
						//	ByteArray = Marshal.ReadByte(
							UInt32 StartAddress;
							UInt16 NumberOfSectors = 0;
							UInt32 SectorSize = 0;
							UInt16 j = 0;
							MappingSector.SectorType SType = MappingSector.SectorType.Other;
							String SectorName;
							String SectorDescription;
							String SectorN;

							MapDesc = Marshal.PtrToStringAnsi(StringBuffer);
							if ('@' != MapDesc[0])
							{
								throw new Exception("STDFU_GetStringDescriptor bad value in MapDesc, i=" + i.ToString());
							}
							SectorName = MapDesc.Substring(1, MapDesc.IndexOf('/') - 1);
							SectorName = SectorName.TrimEnd(' ');
							if (SectorName.Equals("Internal Flash"))
							{
								SType = MappingSector.SectorType.InternalFLASH;
							}
							else if (SectorName.Equals("Option Bytes"))
							{
								SType = MappingSector.SectorType.OptionBytes;
							}
							else if (SectorName.Equals("OTP Memory"))
							{
								SType = MappingSector.SectorType.OTP;
							}
							else if (SectorName.Equals("Device Feature"))
							{
								SType = MappingSector.SectorType.DeviceFeature;
							}
							else
							{
								SType = MappingSector.SectorType.Other;
							}
							StartAddress = UInt32.Parse(MapDesc.Substring(MapDesc.IndexOf('/') + 3, 8), System.Globalization.NumberStyles.HexNumber);
							SectorDescription = MapDesc;
							while (SectorDescription.IndexOf('*') >= 0)
							{
								SectorN = SectorDescription.Substring(SectorDescription.IndexOf('*') - 3, 3);
								if (char.IsDigit(SectorN[0]))
								{
									NumberOfSectors = UInt16.Parse(SectorN);
								}
								else
								{
									NumberOfSectors = UInt16.Parse(SectorN.Substring(1));
								}
								SectorSize = UInt16.Parse(SectorDescription.Substring(SectorDescription.IndexOf('*') + 1, 3));
								if ('k' == char.ToLower(SectorDescription[SectorDescription.IndexOf('*') + 4]))
								{
									SectorSize *= 1024;
								}
								for (j = 0; j < NumberOfSectors; j++)
								{
									Sectors.Add(new MappingSector(SectorName, SType, StartAddress, SectorSize, j));
									StartAddress += SectorSize;
								}
								SectorDescription = SectorDescription.Substring(SectorDescription.IndexOf('*') + 1);
							}
						}
						else
						{
							break;
						}
					}
					else
					{
						throw new Exception("STDFU_GetInterfaceDescriptor returned " + Result.ToString());
					}
				}
			}
			else
			{
				throw new Exception("STDFU_GetDFUDescriptor returned " + Result.ToString());
			}

			return (Sectors);
		}

		#region CrcTable
		private UInt32[] CrcTable =	{
		0x00000000, 0x77073096, 0xee0e612c, 0x990951ba, 0x076dc419, 0x706af48f,
		0xe963a535, 0x9e6495a3, 0x0edb8832, 0x79dcb8a4, 0xe0d5e91e, 0x97d2d988,
		0x09b64c2b, 0x7eb17cbd, 0xe7b82d07, 0x90bf1d91, 0x1db71064, 0x6ab020f2,
		0xf3b97148, 0x84be41de, 0x1adad47d, 0x6ddde4eb, 0xf4d4b551, 0x83d385c7,
		0x136c9856, 0x646ba8c0, 0xfd62f97a, 0x8a65c9ec, 0x14015c4f, 0x63066cd9,
		0xfa0f3d63, 0x8d080df5, 0x3b6e20c8, 0x4c69105e, 0xd56041e4, 0xa2677172,
		0x3c03e4d1, 0x4b04d447, 0xd20d85fd, 0xa50ab56b, 0x35b5a8fa, 0x42b2986c,
		0xdbbbc9d6, 0xacbcf940, 0x32d86ce3, 0x45df5c75, 0xdcd60dcf, 0xabd13d59,
		0x26d930ac, 0x51de003a, 0xc8d75180, 0xbfd06116, 0x21b4f4b5, 0x56b3c423,
		0xcfba9599, 0xb8bda50f, 0x2802b89e, 0x5f058808, 0xc60cd9b2, 0xb10be924,
		0x2f6f7c87, 0x58684c11, 0xc1611dab, 0xb6662d3d, 0x76dc4190, 0x01db7106,
		0x98d220bc, 0xefd5102a, 0x71b18589, 0x06b6b51f, 0x9fbfe4a5, 0xe8b8d433,
		0x7807c9a2, 0x0f00f934, 0x9609a88e, 0xe10e9818, 0x7f6a0dbb, 0x086d3d2d,
		0x91646c97, 0xe6635c01, 0x6b6b51f4, 0x1c6c6162, 0x856530d8, 0xf262004e,
		0x6c0695ed, 0x1b01a57b, 0x8208f4c1, 0xf50fc457, 0x65b0d9c6, 0x12b7e950,
		0x8bbeb8ea, 0xfcb9887c, 0x62dd1ddf, 0x15da2d49, 0x8cd37cf3, 0xfbd44c65,
		0x4db26158, 0x3ab551ce, 0xa3bc0074, 0xd4bb30e2, 0x4adfa541, 0x3dd895d7,
		0xa4d1c46d, 0xd3d6f4fb, 0x4369e96a, 0x346ed9fc, 0xad678846, 0xda60b8d0,
		0x44042d73, 0x33031de5, 0xaa0a4c5f, 0xdd0d7cc9, 0x5005713c, 0x270241aa,
		0xbe0b1010, 0xc90c2086, 0x5768b525, 0x206f85b3, 0xb966d409, 0xce61e49f,
		0x5edef90e, 0x29d9c998, 0xb0d09822, 0xc7d7a8b4, 0x59b33d17, 0x2eb40d81,
		0xb7bd5c3b, 0xc0ba6cad, 0xedb88320, 0x9abfb3b6, 0x03b6e20c, 0x74b1d29a,
		0xead54739, 0x9dd277af, 0x04db2615, 0x73dc1683, 0xe3630b12, 0x94643b84,
		0x0d6d6a3e, 0x7a6a5aa8, 0xe40ecf0b, 0x9309ff9d, 0x0a00ae27, 0x7d079eb1,
		0xf00f9344, 0x8708a3d2, 0x1e01f268, 0x6906c2fe, 0xf762575d, 0x806567cb,
		0x196c3671, 0x6e6b06e7, 0xfed41b76, 0x89d32be0, 0x10da7a5a, 0x67dd4acc,
		0xf9b9df6f, 0x8ebeeff9, 0x17b7be43, 0x60b08ed5, 0xd6d6a3e8, 0xa1d1937e,
		0x38d8c2c4, 0x4fdff252, 0xd1bb67f1, 0xa6bc5767, 0x3fb506dd, 0x48b2364b,
		0xd80d2bda, 0xaf0a1b4c, 0x36034af6, 0x41047a60, 0xdf60efc3, 0xa867df55,
		0x316e8eef, 0x4669be79, 0xcb61b38c, 0xbc66831a, 0x256fd2a0, 0x5268e236,
		0xcc0c7795, 0xbb0b4703, 0x220216b9, 0x5505262f, 0xc5ba3bbe, 0xb2bd0b28,
		0x2bb45a92, 0x5cb36a04, 0xc2d7ffa7, 0xb5d0cf31, 0x2cd99e8b, 0x5bdeae1d,
		0x9b64c2b0, 0xec63f226, 0x756aa39c, 0x026d930a, 0x9c0906a9, 0xeb0e363f,
		0x72076785, 0x05005713, 0x95bf4a82, 0xe2b87a14, 0x7bb12bae, 0x0cb61b38,
		0x92d28e9b, 0xe5d5be0d, 0x7cdcefb7, 0x0bdbdf21, 0x86d3d2d4, 0xf1d4e242,
		0x68ddb3f8, 0x1fda836e, 0x81be16cd, 0xf6b9265b, 0x6fb077e1, 0x18b74777,
		0x88085ae6, 0xff0f6a70, 0x66063bca, 0x11010b5c, 0x8f659eff, 0xf862ae69,
		0x616bffd3, 0x166ccf45, 0xa00ae278, 0xd70dd2ee, 0x4e048354, 0x3903b3c2,
		0xa7672661, 0xd06016f7, 0x4969474d, 0x3e6e77db, 0xaed16a4a, 0xd9d65adc,
		0x40df0b66, 0x37d83bf0, 0xa9bcae53, 0xdebb9ec5, 0x47b2cf7f, 0x30b5ffe9,
		0xbdbdf21c, 0xcabac28a, 0x53b39330, 0x24b4a3a6, 0xbad03605, 0xcdd70693,
		0x54de5729, 0x23d967bf, 0xb3667a2e, 0xc4614ab8, 0x5d681b02, 0x2a6f2b94,
		0xb40bbe37, 0xc30c8ea1, 0x5a05df1b, 0x2d02ef8d	};
		#endregion

		/// <summary>
		/// Class to encapsulate information about a programmable sector of memory within the micro-controller
		/// </summary>
		public class MappingSector
		{
			public enum SectorType { InternalFLASH, OptionBytes, OTP, DeviceFeature, Other };
			public SectorType Type;
			public String Name;
			public UInt32 dwStartAddress;
			public UInt32 dwSectorIndex;
			public UInt32 dwSectorSize;

			public MappingSector(String Name, SectorType SType, UInt32 StartAddress, UInt32 Size, UInt16 SectorIndex)
			{
				this.Name = Name;
				this.Type = SType;
				this.dwStartAddress = StartAddress;
				this.dwSectorSize = Size;
				this.dwSectorIndex = SectorIndex;
			}
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct USB_DeviceDescriptor // Taken from usb100.h
		{
			public byte bLength;
			public byte bDescriptorType;
			public ushort bcdUSB;
			public byte bDeviceClass;
			public byte bDeviceSubClass;
			public byte bDeviceProtocol;
			public byte bMaxPacketSize0;
			public ushort idVendor;
			public ushort idProduct;
			public ushort bcdDevice;
			public byte iManufacturer;
			public byte iProduct;
			public byte iSerialNumber;
			public byte bNumConfigurations;
		};

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct DFU_FunctionalDescriptor
		{
			public byte bLength;
			public byte bDescriptorType; // Should be 0x21
			public byte bmAttributes;
			public UInt16 wDetachTimeOut;
			public UInt16 wTransfertSize;
			public UInt16 bcdDFUVersion;
		};

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct  DFU_Status
		{
			public byte bStatus;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
			public byte[] bwPollTimeout;
			public byte bState;
			public byte iString;
		};

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct USB_InterfaceDescriptor {
			public byte bLength;
			public byte bDescriptorType;
			public byte bInterfaceNumber;
			public byte bAlternateSetting;
			public byte bNumEndpoints;
			public byte bInterfaceClass;
			public byte bInterfaceSubClass;
			public byte bInterfaceProtocol;
			public byte iInterface;
		};

		/// <summary>
		/// Open the DFU driver, get the handle
		/// </summary>
		/// <param name="szDevicePath">Device path string</param>
		/// <param name="hDevice">Device handle, populated by this call</param>
		/// <returns>STDFU_NOERROR if successful, else error codes</returns>
		[DllImport("STDFU.dll", EntryPoint = "STDFU_Open", CharSet = CharSet.Ansi)]
		public static extern UInt32 STDFU_Open([MarshalAs(UnmanagedType.LPStr)]String szDevicePath, out IntPtr hDevice);

		[DllImport("STDFU.dll", EntryPoint = "STDFU_SelectCurrentConfiguration", CharSet = CharSet.Ansi)]
		public static extern UInt32 STDFU_SelectCurrentConfiguration(ref IntPtr hDevice, UInt32 ConfigIndex, UInt32 InterfaceIndex, UInt32 AlternateSetIndex );

		[DllImport("STDFU.dll", EntryPoint = "STDFU_GetDeviceDescriptor", CharSet = CharSet.Auto)]
		public static extern UInt32 STDFU_GetDeviceDescriptor(ref IntPtr handle, ref USB_DeviceDescriptor descriptor);

		[DllImport("STDFU.dll", EntryPoint = "STDFU_GetDFUDescriptor", CharSet = CharSet.Auto)]
		public static extern UInt32 STDFU_GetDFUDescriptor(ref IntPtr handle, ref uint DFUInterfaceNum, ref uint NBOfAlternates, ref DFU_FunctionalDescriptor dfuDescriptor);

		[DllImport("STDFU.dll", EntryPoint = "STDFU_GetInterfaceDescriptor", CharSet = CharSet.Auto)]
		public static extern UInt32 STDFU_GetInterfaceDescriptor(ref IntPtr handle, UInt32 ConfigIndex, UInt32 InterfaceIndex, UInt32 AlternateIndex, ref USB_InterfaceDescriptor usbDescriptor);

		/// <summary>
		/// Get a string descriptor from the DFU driver
		/// </summary>
		/// <param name="handle">Handle to DFU device</param>
		/// <param name="Index">Index of desired string, if this is not valid the function will return an error</param>
		/// <param name="StringBuffer">Buffer for the string to be copied to</param>
		/// <param name="BufferSize">Size of buffer</param>
		/// <returns>STDEVICE_NOERROR or error code</returns>
		[DllImport("STDFU.dll", EntryPoint = "STDFU_GetStringDescriptor", CharSet = CharSet.Auto)]
		public static extern UInt32 STDFU_GetStringDescriptor(ref IntPtr handle, UInt32 Index, IntPtr StringBuffer, UInt32 BufferSize);

		[DllImport("STDFU.dll", EntryPoint = "STDFU_Dnload", CharSet = CharSet.Ansi)]
		public static extern UInt32 STDFU_Dnload(ref IntPtr hDevice, [MarshalAs(UnmanagedType.LPArray)]byte[] pBuffer, UInt32 nBytes, UInt16 nBlocks);

		[DllImport("STDFU.dll", EntryPoint = "STDFU_Getstatus", CharSet = CharSet.Ansi)]
		public static extern UInt32 STDFU_GetStatus(ref IntPtr hDevice, ref DFU_Status dfuStatus);

		[DllImport("STDFU.dll", EntryPoint = "STDFU_Clrstatus", CharSet = CharSet.Ansi)]
		public static extern UInt32 STDFU_ClrStatus(ref IntPtr hDevice); 

        
	}
} 