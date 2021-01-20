﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MissionPlanner.ArduPilot;
using MissionPlanner.Comms;
using MissionPlanner.Utilities;

namespace Xamarin
{
    public class Test
    {
        public static IBlueToothDevice BlueToothDevice { get; set; }
        public static IUSBDevices UsbDevices { get; set; }
        public static IRadio Radio { get; set; }

        public static IGPS GPS { get; set; }
    }

    public interface IGPS
    {
        Task<(double lat,double lng,double alt)> GetPosition();
    }

    public interface IBlueToothDevice
    {
        Task<List<DeviceInfo>> GetDeviceInfoList();
        Task<ICommsSerial> GetBT(DeviceInfo first);
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

        event EventHandler<DeviceInfo> USBEvent;
    }
}
