using System;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Com.Android.Internal.Telephony.D2d;
using MissionPlanner.Utilities;
using Object = Java.Lang.Object;
using String = System.String;

namespace Xamarin.Droid
{
    public class D2DService : Object, IServiceConnection
    {
        private static String TAG = "D2DServiceX";
        private bool mBound;
        private ID2DService mD2DService;
        private ID2DInfoListener mD2DInfoListener = new D2DInfoListener();

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            Log.Verbose(TAG, "D2DService connected");
            mD2DService = (ID2DService)ID2DServiceStub.AsInterface(service);
            try
            {
                mD2DService.RegisterForD2dInfoChanged(mD2DInfoListener);
            }
            catch (RemoteException e)
            {
                Console.WriteLine(e.ToString());
            }
            mBound = true;
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            Log.Verbose(TAG, "D2DService disconnected");
            mD2DService = null;
            mBound = false;
        }
    }



}

namespace Android.Telephony
{
    public class ServiceStateStub
    {
        public static ServiceState AsInterface(IBinder obj)
        {
            Console.WriteLine(obj.ToJSON());
            return new ServiceState();
        }
    }

    public class SignalStrengthStub
    {
        public static SignalStrength AsInterface(IBinder obj)
        {
            Console.WriteLine(obj.ToJSON());
            return null;
        }
    }
}