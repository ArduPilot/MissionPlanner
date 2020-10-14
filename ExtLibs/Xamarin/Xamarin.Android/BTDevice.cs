using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Bluetooth;
using Android.Util;
using MissionPlanner.ArduPilot;

namespace Xamarin.Droid
{
    public class BTDevice : IBlueToothDevice
    {
        public Task<List<DeviceInfo>> GetDeviceInfoList()
        {
            // Get the local Bluetooth adapter
            var btAdapter = BluetoothAdapter.DefaultAdapter;

            // Get a set of currently paired devices
            var pairedDevices = btAdapter.BondedDevices;

            // If there are paired devices, add each on to the ArrayAdapter
            if (pairedDevices.Count > 0)
            {
                foreach (var device in pairedDevices)
                {
                    Log.Info("MP", "{0} {1} {2} {3} {4}", device.Name, device.Address, device.Type, device.Class,
                        device.BondState);

                }
            }

            return null;
        }
    }
}