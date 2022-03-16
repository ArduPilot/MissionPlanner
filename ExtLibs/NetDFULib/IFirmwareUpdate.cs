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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// See ST's C++ demo application, UM0412: http://www.st.com/internet/com/SOFTWARE_RESOURCES/SW_COMPONENT/SW_DEMO/um0412.zip
// Drivers and required DLLs are part of this package
// See ST's USB DFU protocol document, AN3156: http://www.st.com/internet/com/TECHNICAL_RESOURCES/TECHNICAL_LITERATURE/APPLICATION_NOTE/CD00264379.pdf
// See ST's STDFU library specification, UM0384 (part of the demo package)
// See ST's DfuSe file format specification, UM0391 (part of the demo package)

namespace NetDFULib
{
	public class FirmwareUpdateProgressEventArgs : EventArgs
	{
		/// <summary>
		/// Constructor for the firmware update progress event args.  This is an event raised during at intervals
		/// during the firmware update process to allow the client to indicate the progress or failure.
		/// </summary>
		/// <param name="Percentage">0-100% progress value</param>
		/// <param name="Message">Text message</param>
		/// <param name="Failed">Set true if the process has failed, otherwise false</param>
		public FirmwareUpdateProgressEventArgs(float Percentage, String Message, bool Failed)
		{
			this.Percentage = Percentage;
			this.Message = Message;
			this.Failed = Failed;
		}

		public float Percentage;
		public String Message;
		public bool Failed;
	}

	/// <summary>
	/// Handler for the firmware update progress event.  This is an event raised during at intervals
	/// during the firmware update process to allow the client to indicate the progress or failure.
	/// </summary>
	/// <param name="sender">IARM_Board</param>
	/// <param name="e">Event args</param>
	public delegate void FirmwareUpdateProgressEventHandler(object sender, FirmwareUpdateProgressEventArgs e);

	/// <summary>
	/// Interface for the FirmwareUpdate class, which handles updating the embedded's firmware
	/// </summary>
	public interface IFirmwareUpdate
	{
		/// <summary>
		/// Update the device firmware with the referenced DFU file data
		/// Checks that the VID and PID match those expected before doing the update
		/// </summary>
		/// <param name="DFU_FilePath">Full path of DFU file for new firmware</param>
		/// <param name="Del">Delegate for progress updates</param>
		/// <param name="DoMassErase">True to mass erase the device, false to erase by sectors.
        /// Unless the DFU module is in ROM, the device should be erased by sectors or else the DFU module will be erased</param>
        /// <param name="ExitDFUMode">True to exit DFU mode after the update.</param>
        void UpdateFirmware(String DFU_FilePath, bool DoMassErase, bool ExitDFUMode);

		/// <summary>
		/// Mass erase a device.  
		/// Useful for a device which has been read protected, otherwise it can't be programmed with a ULink.
        /// Obviously erases the DFU module unless it is in ROM
        /// Assumes the device is already in DFU mode.
		/// </summary>
		void MassErase();

		/// <summary>
		/// Scan a DFU file to get the VID, PID and file version
		/// It is recommended that the VID and PID are checked before proceeding with the firmware update
		/// </summary>
		/// <param name="Filepath">DFU filepath</param>
		/// <param name="VID">Vendor Identifier</param>
		/// <param name="PID">Product Identifier</param>
		/// <param name="Version">DFU file firmware version</param>
		/// <returns>true if success, else false</returns>
		bool ParseDFU_File(String Filepath, out UInt16 VID, out UInt16 PID, out UInt16 Version);

		/// <summary>Event handler for firmware update progress event</summary>
		event FirmwareUpdateProgressEventHandler OnFirmwareUpdateProgress;
	}
} 