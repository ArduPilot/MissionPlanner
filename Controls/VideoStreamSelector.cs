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
using static MAVLink;

namespace MissionPlanner.Controls
{
    public partial class VideoStreamSelector : Form
    {

        public string gstreamer_pipeline = "";
        public VideoStreamSelector()
        {
            InitializeComponent();

            // Populate the combobox with the available video streams
            cmb_detectedstreams.DisplayMember = "Key";
            cmb_detectedstreams.ValueMember = "Value";
            cmb_detectedstreams.DataSource = CameraProtocol.VideoStreams.Values.Select
            (
                x => new KeyValuePair<string, mavlink_video_stream_information_t>
                (
                    System.Text.Encoding.UTF8.GetString(x.name).Split('\0')[0], x
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
            if (cmb_detectedstreams.SelectedValue == null)
                return;
            txt_gstreamraw.Text = CameraProtocol.GStreamerPipeline((mavlink_video_stream_information_t)cmb_detectedstreams.SelectedValue);
        }
    }
}
