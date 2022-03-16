using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Org.Freedesktop.Gstreamer.Tutorials.Tutorial_5;

namespace AndroidApp1
{
    [Activity(Label = "MainActivity", MainLauncher = true)]
    public class MainActivity : Tutorial5 //Activity, ISurfaceHolderCallback
    {
        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            //nativeSurfaceInit(holder.Surface);
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
           
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            //nativeSurfaceFinalize();
        }
        /*
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            //SetContentView(Resource.Layout.activity_main);

            JavaSystem.LoadLibrary("gstreamer_android");
            JavaSystem.LoadLibrary("tutorial-5");

            // java init
            Org.Freedesktop.Gstreamer.GStreamer.Init(this.ApplicationContext);


            //Create a layout---------------  
            LinearLayout linearLayout = new LinearLayout(this);
            
            var gs  = new GStreamerSurfaceView(this.ApplicationContext);
            gs.Holder.AddCallback(this);
            linearLayout.AddView(gs);

            //---Create a layout param for the layout-----------------  
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(ActionBar.LayoutParams.FillParent, ActionBar.LayoutParams.FillParent);
            this.AddContentView(linearLayout, layoutParams);


            //StartActivity(typeof(Tutorial5));

            
          //  var tut = new Org.Freedesktop.Gstreamer.Tutorials.Tutorial_5.Tutorial5();

          

        }
        */
    }
}