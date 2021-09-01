using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xamarin.Droid
{
    using System;


    // Get access to the hidden SDK class SystemProperties.  The SystemProperties
    // class provides access to the System Properties store.  This store contains
    // a list of key-value pairs
    // See https://android.googlesource.com/platform/frameworks/base/+/master/core/java/android/os/SystemProperties.java
    public static class SysProp
    {
        // Lazy load the SystemProperties class
        private static Lazy<Java.Lang.Class> _class =
            new Lazy<Java.Lang.Class>(() =>
                Java.Lang.Class.ForName("android.os.SystemProperties")
            );

        // Get the set method when we need it
        private static Lazy<Java.Lang.Reflect.Method> _SetMethod =
            new Lazy<Java.Lang.Reflect.Method>(() =>
                _class.Value.GetDeclaredMethod("set",
                    Java.Lang.Class.FromType(typeof(Java.Lang.String)),
                    Java.Lang.Class.FromType(typeof(Java.Lang.String)))
            );

        // Get the get method when we need it
        private static Lazy<Java.Lang.Reflect.Method> _GetMethod =
            new Lazy<Java.Lang.Reflect.Method>(() =>
                _class.Value.GetDeclaredMethod("get",
                    Java.Lang.Class.FromType(typeof(Java.Lang.String)))
            );

        private static Java.Lang.Reflect.Method SetMethod
        {
            get { return _SetMethod.Value; }
        }

        private static Java.Lang.Reflect.Method GetMethod
        {
            get { return _GetMethod.Value; }
        }

        /// <summary>
        /// Calls the get method of the android.os.SystemProperties class
        /// </summary>
        /// <param name="PropertyName">The name of the system property to get the value for</param>
        /// <returns>The value of the specified property or null if it does not exists</returns>
        public static string GetProp(string PropertyName)
        {
            // Invoking a static method, first parameter is null
            var r = GetMethod.Invoke(null, new Java.Lang.String(PropertyName));

            return r.ToString();
        }

        /// <summary>
        /// Calls the set method of the android.os.SystemProperties class
        /// </summary>
        /// <param name="PropertyName">The name of the system property to get the value for</param>
        /// <param name="PropertyValue">The value to set for the system property</param>
        /// <returns>The previous value of the specified property or null if it does not exists</returns>
        public static string SetProp(string PropertyName, string PropertyValue)
        {
            // Invoking a static method, first parameter is null
            var r = SetMethod.Invoke(null,
                new Java.Lang.String(PropertyName),
                new Java.Lang.String(PropertyValue));

            return r.ToString();
        }

    }
}