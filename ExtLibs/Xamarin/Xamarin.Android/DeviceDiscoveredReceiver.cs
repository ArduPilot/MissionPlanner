using Android.App;
using Android.Bluetooth;
using Android.Content;

namespace MissionPlanner.Comms
{
    public class DeviceDiscoveredReceiver : BroadcastReceiver
    {
        Activity chatActivity;

        public DeviceDiscoveredReceiver(Activity chat)
        {
            this.chatActivity = chat;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            string action = intent.Action;

            // When discovery finds a device
            if (action == BluetoothDevice.ActionFound)
            {
                // Get the BluetoothDevice object from the Intent
                BluetoothDevice device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                // If it's already paired, skip it, because it's been listed already
                if (device.BondState != Bond.Bonded)
                {
                    //newDevicesArrayAdapter.Add(device.Name + "\n" + device.Address);
                }
                // When discovery is finished, change the Activity title
            }
            else if (action == BluetoothAdapter.ActionDiscoveryFinished)
            {
                //chatActivity.SetProgressBarIndeterminateVisibility(false);
                // chatActivity.SetTitle(Resource.String.select_device);
                // if (newDevicesArrayAdapter.Count == 0)
                {
                    //     var noDevices = chatActivity.Resources.GetText(Resource.String.none_found).ToString();
                    //     newDevicesArrayAdapter.Add(noDevices);

                }
            }
        }
    }
}