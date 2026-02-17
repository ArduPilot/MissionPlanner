/*  WinUSBNet library
 *  (C) 2010 Thomas Bleeker (www.madwizard.org)
 *
 *  Licensed under the MIT license, see license.txt or:
 *  http://www.opensource.org/licenses/mit-license.php
 */

namespace Nefarius.Drivers.WinUSB.API;

internal struct DeviceDetails
{
    public string DevicePath;
    public string Manufacturer;
    public string DeviceDescription;
    public ushort VID;
    public ushort PID;
}
