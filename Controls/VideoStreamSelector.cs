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
        public (uint System, byte Component)? CameraAddress { get; private set; }

        private sealed class StreamChoice
        {
            public string Name { get; set; }
            public mavlink_video_stream_information_t Stream { get; set; }
            public (uint System, byte Component) Camera { get; set; }
        }
        public VideoStreamSelector()
        {
            InitializeComponent();

            // Populate the combobox with the available video streams
            cmb_detectedstreams.DisplayMember = "Name";
            cmb_detectedstreams.ValueMember = "Stream";
            cmb_detectedstreams.DataSource = CameraProtocol.VideoStreams.Select(x => new StreamChoice
            {
                Name = System.Text.Encoding.UTF8.GetString(x.Value.name).Split('\0')[0],
                Stream = x.Value,
                Camera = (x.Key.Item1, x.Key.Item2)
            }).ToList();

            Utilities.ThemeManager.ApplyThemeTo(this);
        }

        private void but_launch_Click(object sender, EventArgs e)
        {
            if(txt_gstreamraw.Text != "")
            {
                gstreamer_pipeline = txt_gstreamraw.Text;
                if (cmb_detectedstreams.SelectedItem is StreamChoice choice && gstreamer_pipeline == CameraProtocol.GStreamerPipeline(choice.Stream))
                    CameraAddress = choice.Camera;
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
