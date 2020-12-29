using AppKit;
using Foundation;
using Xamarin.Forms.Platform.MacOS;
using Xamarin.Forms;

namespace Xamarin.MacOS
{
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate
    {
        public AppDelegate()
        {
            var style = NSWindowStyle.Closable | NSWindowStyle.Resizable | NSWindowStyle.Titled;
            var rect = new CoreGraphics.CGRect(200,200,1024,768);
            mainWindow = new NSWindow(rect, style, NSBackingStore.Buffered, false);
            mainWindow.Title = "Xamarin.Forms on Mac!";
            mainWindow.TitleVisibility = NSWindowTitleVisibility.Hidden;
        }

        private  NSWindow mainWindow;

        public override NSWindow MainWindow { get => mainWindow; }

        public override void DidFinishLaunching(NSNotification notification)
        {
            // Insert code here to initialize your application
            Forms.Forms.Init();
            LoadApplication(new Xamarin.App());// new App());
            base.DidFinishLaunching(notification);
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }
    }
}