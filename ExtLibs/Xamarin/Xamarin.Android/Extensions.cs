using Android.OS;
using Android.Telephony;
using Java.Lang;

namespace Android.Telephony
{
    public static class Extensions
    {
        public static IBinder AsBinder(this ServiceState ss)
        {
            return null;
        }

        public static IBinder AsBinder(this SignalStrength ss)
        {
            return null;
        }
    }
}

namespace Xamarin.Droid
{
    public static class Extensions
    {
  
        public static int getState(this ServiceState ss)
        {
            if (ss == null) return 0;
            return (int)ss.State;
        }
        public static int getLteRsrq(this SignalStrength ss)
        {
            if (ss == null)
                return 0;
            return ss.CdmaDbm;
        }
        public static void setData(this Message msg, Bundle args)
        {
            msg.Data = args;
        }
        public static void putInt(this Bundle bund, string name, int value)
        {
            bund.PutInt(name, value);
        }
        public static void post(this Handler hand, Runnable run)
        {
            if(run != null)
                hand.Post(run);
        }
        public static void sendMessage(this Handler hand, Message msg)
        {
            //hand.SendMessage(msg);
        }
        public static Message obtainMessage(this Handler hand, int eventConfigD2DBwDone)
        {
            return hand.ObtainMessage(eventConfigD2DBwDone);
        }
    }
}