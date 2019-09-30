using System;
using System.Windows.Forms;

namespace Xamarin.Controls
{
    public class UserControl: Control
    {
        public object Invoke(Action p0)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread((Action)p0);
            return null;
        }
    }
}
