using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionPlanner.Utilities;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterDetailPage1Detail : ContentPage
    {
        public MasterDetailPage1Detail()
        {
            InitializeComponent();

            string url =
                "rtspsrc location=rtsp://192.168.0.10:8554/fpv_stream latency=41 udp-reconnect=1 timeout=0 do-retransmission=false ! application/x-rtp ! rtph264depay ! h264parse ! queue ! avdec_h264 ! video/x-raw,format=BGRx ! appsink name=outsink";

            GStreamer.StartA(url);
        }
    }
}