using System;
using Android.Content;
using Android.Telephony;

namespace Xamarin.Droid
{
    [BroadcastReceiver]
    public class SignalStrengthBroadcastReceiver : PhoneStateListener
    {
        public event Action<SignalStrength> SignalStrengthChanged;//No need to declare a delegate
        public override void OnSignalStrengthsChanged(SignalStrength signalStrength)
        {
            base.OnSignalStrengthsChanged(signalStrength);

            Console.WriteLine("signalStrength " + signalStrength);

            SignalStrengthChanged?.Invoke(signalStrength);
        }
    }
}