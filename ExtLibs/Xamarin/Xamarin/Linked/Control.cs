using Xamarin.Controls;

namespace System.Windows.Forms
{
    
    public class Control: MySKCanvasView
    {
        public bool InvokeRequired { get; internal set; }

    }
}