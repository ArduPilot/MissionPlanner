using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Managed.Adb {
	/// <summary>
	/// 
	/// </summary>
	public class PackageManager {
		private const String PM_LIST_FULL = "pm list packages -f";
		/// <summary>
		/// Initializes a new instance of the <see cref="PackageManager"/> class.
		/// </summary>
		/// <param name="device">The device.</param>
		public PackageManager ( Device device ) {
			Device = device;
			Packages = new Dictionary<string, FileEntry> ( );
		}

		/// <summary>
		/// Gets or sets the packages.
		/// </summary>
		/// <value>
		/// The packages.
		/// </value>
		public Dictionary<String,   FileEntry> Packages { get; set; }

		/// <summary>
		/// Refreshes the packages.
		/// </summary>
		public void RefreshPackages ( ) {
			if ( this.Device.IsOffline ) {
				throw new IOException ( "Device is offline" );
			}

			PackageManagerReceiver pmr = new PackageManagerReceiver ( this.Device, this );
			this.Device.ExecuteShellCommand ( PM_LIST_FULL, pmr );

		}

		/// <summary>
		/// Existses the specified package.
		/// </summary>
		/// <param name="package">The package.</param>
		/// <returns></returns>
		public bool Exists ( String package ) {
			try {
				return GetApkFileEntry ( package ) != null;
			} catch ( FileNotFoundException) {
				return false;
			}
		}

		/// <summary>
		/// Gets the apk file entry.
		/// </summary>
		/// <param name="package">The package.</param>
		/// <returns></returns>
		public FileEntry GetApkFileEntry ( String package ) {
			return FileEntry.Find ( this.Device, GetApkPath ( package ) );
		}

		/// <summary>
		/// Gets the apk path.
		/// </summary>
		/// <param name="package">The package.</param>
		/// <returns></returns>
		public String GetApkPath ( String package ) {

			if ( this.Device.IsOffline ) {
				throw new IOException ( "Device is offline" );
			}

			PackageManagerPathReceiver receiver = new PackageManagerPathReceiver();
			this.Device.ExecuteShellCommand ( "pm path {0}", receiver, package );
			if ( !String.IsNullOrEmpty ( receiver.Path ) ) {
				return receiver.Path;
			} else {
				throw new FileNotFoundException ( String.Format ( "The package '{0}' is not installed on the device: {1}", package, Device.SerialNumber ) );
			}

		}

		/// <summary>
		/// Gets or sets the device.
		/// </summary>
		/// <value>
		/// The device.
		/// </value>
		private Device Device { get; set; }

		/// <summary>
		/// 
		/// </summary>
		private class PackageManagerPathReceiver : MultiLineReceiver {
			/// <summary>
			/// Pattern to parse the output of the 'pm path &lt;package&gt;' command.
			/// The output format looks like:
			/// /data/app/myapp.apk=com.mypackage.myapp
			/// </summary>
			public const String PM_PATH_PATTERN = "^package:(.+?)$";

			/// <summary>
			/// Initializes a new instance of the <see cref="PackageManagerPathReceiver"/> class.
			/// </summary>
			public PackageManagerPathReceiver ( ) {
				Path = null;
			}

			/// <summary>
			/// Gets or sets the path.
			/// </summary>
			/// <value>
			/// The path.
			/// </value>
			public String Path { get; private set; }

			/// <summary>
			/// Processes the new lines.
			/// </summary>
			/// <param name="lines">The lines.</param>
			protected override void ProcessNewLines ( string[] lines ) {
				foreach ( String line in lines ) {
					if ( !String.IsNullOrEmpty ( line ) && !line.StartsWith ( "#" ) ) {
						// get the filepath and package from the line
						Match m = Regex.Match ( line, PM_PATH_PATTERN, RegexOptions.Compiled );
						if ( m.Success ) {
							Path = m.Groups[1].Value;
						}
					}
				}
			}
		}
	}
}
