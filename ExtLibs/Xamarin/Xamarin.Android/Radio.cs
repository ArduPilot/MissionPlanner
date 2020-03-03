using System;
using Android.Content;
using Android.Runtime;
using Android.Telephony;
using Android.Util;

namespace Xamarin.Droid
{
    public class Radio : IRadio 
    {
        static bool _radiostate = true;
        public void Toggle()
        {
            //https://android.googlesource.com/platform/frameworks/base/+/73cdcf57877f94cefb76d2b1d160f59a2ce82df6/telephony/java/android/telephony/TelephonyManager.java#4463
            try
            {
                var manager =
                    (TelephonyManager) global::Android.App.Application.Context.GetSystemService(
                        Context.TelephonyService);
                IntPtr TelephonyManager_getITelephony = JNIEnv.GetMethodID(
                    manager.Class.Handle,
                    "getITelephony",
                    "()Lcom/android/internal/telephony/ITelephony;");

                IntPtr telephony = JNIEnv.CallObjectMethod(manager.Handle, TelephonyManager_getITelephony);
                IntPtr ITelephony_class = JNIEnv.GetObjectClass(telephony);
                IntPtr ITelephony_setRadioPower = JNIEnv.GetMethodID(
                    ITelephony_class,
                    "setRadioPower",
                    "(Z)Z");
                JNIEnv.CallBooleanMethod(telephony, ITelephony_setRadioPower, new JValue(!_radiostate));
                JNIEnv.DeleteLocalRef(telephony);
                JNIEnv.DeleteLocalRef(ITelephony_class);

                _radiostate = !_radiostate;

            }
            catch (Exception e)
            {
                Log.Info("Radio", e.ToString());
            }
        }
    }
}