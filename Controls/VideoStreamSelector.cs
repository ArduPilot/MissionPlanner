using MissionPlanner.ArduPilot.Mavlink;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class VideoStreamSelector : Form
    {

        public string gstreamer_pipeline = "";
        public VideoStreamSelector()
        {
            InitializeComponent();

            // Populate the combobox with the available video streams
            // use the "name" as the display value and the "url" as the value
            cmb_detectedstreams.DisplayMember = "Key";
            cmb_detectedstreams.ValueMember = "Value";
            cmb_detectedstreams.DataSource = CameraProtocol.VideoStreams.Values.Select
            (
                x => new KeyValuePair<string, string>
                (
                    System.Text.Encoding.UTF8.GetString(x.name).Split('\0')[0],
                    System.Text.Encoding.UTF8.GetString(x.uri).Split('\0')[0]
                )
            ).ToList();

            Utilities.ThemeManager.ApplyThemeTo(this);
        }

        private void but_launch_Click(object sender, EventArgs e)
        {
            if(txt_gstreamraw.Text != "")
            {
                gstreamer_pipeline = txt_gstreamraw.Text;
                DialogResult = DialogResult.OK;
            }
            
            Close();
        }

        private void cmb_detectedstreams_SelectedIndexChanged(object sender, EventArgs e)
        {
            var uri = cmb_detectedstreams.SelectedValue.ToString();
            if (uri.StartsWith("rtsp://"))
            {
                txt_gstreamraw.Text = $"rtspsrc location={uri} latency=41 udp-reconnect=1 timeout=0 do-retransmission=false ! application/x-rtp ! decodebin3 ! queue leaky=2 ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink sync=false";
            }
            else if (uri.StartsWith("gst://"))
            {
                txt_gstreamraw.Text = uri.Substring("gst://".Length);
            }
        }
    }
}
