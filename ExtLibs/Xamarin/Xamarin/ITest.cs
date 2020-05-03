using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MissionPlanner.ArduPilot;
using MissionPlanner.Comms;

namespace Xamarin
{
    public class Test
    {
        public static IUSBDevices UsbDevices { get; set; }
        public static IRadio Radio { get; set; }
    }

    public interface IRadio
    {
        void Toggle();
    }
    public interface IUSBDevices
    {
        /// <summary>
        /// Turn a native type into a DeviceInfo
        /// </summary>
        /// <param name="devicein"></param>
        /// <returns></returns>
        DeviceInfo GetDeviceInfo(object devicein);

        /// <summary>
        /// Turn a generic deviceinfo into a icommsserial
        /// </summary>
        /// <param name="di"></param>
        /// <returns></returns>
        Task<ICommsSerial> GetUSB(DeviceInfo di);

        /// <summary>
        /// Get a list of all devices
        /// </summary>
        /// <returns></returns>
        Task<List<DeviceInfo>> GetDeviceInfoList();

        /// <summary>
        ///  Called when a device is plugged or unplugged
        /// </summary>
        void USBEventCallBack(object usbDeviceReceiver, object device);

        event EventHandler<object> USBEvent;
    }
}
