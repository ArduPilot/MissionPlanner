using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    public partial class Video : Form
    {
        public Video()
        {
            InitializeComponent();
        }

        private void Video_Load(object sender, EventArgs e)
        {
            CheckBox chk = new CheckBox();
            chk.Text = "Display External";
            flowLayoutPanel1.Controls.Add(chk);

            foreach (var zeroconfHost in ZeroConf.Hosts)
            {
                Label lbl = new Label();
                lbl.Text = zeroconfHost.Id;
                flowLayoutPanel1.Controls.Add(lbl);
                foreach (var service in zeroconfHost.Services)
                {
                    foreach (var valueProperty in service.Value.Properties)
                    {
                        foreach (var property in valueProperty)
                        {
                            var matchs = Regex.Match(property.Value, @"\s*(\w+)\(");

                            var resolutions = new Regex(@"(?:([0-9]+)x([0-9]+)[,\)])");

                            if (matchs.Length > 0)
                            {
                                var reslist = resolutions.Matches(property.Value);

                                foreach (Match match in reslist)
                                {
                                    var width = match.Groups[1].Value;
                                    var height = match.Groups[2].Value;
                                    MyButton but = new MyButton();
                                    but.Text = service.Value.Properties[0]["name"] + "     \n" +
                                               matchs.Groups[1].Value.ToString() + " - " + width + "x" + height;
                                    but.Size = TextRenderer.MeasureText(but.Text + "   ", but.Font);
                                    but.Click += delegate(object o, EventArgs args)
                                    {
                                        GStreamer.StartA(String.Format(
                                            "rtspsrc location=rtsp://{0}:{1}{2}?width={3}&height={4} ! application/x-rtp ! rtpjpegdepay ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink",
                                            zeroconfHost.IPAddress, service.Value.Port, service.Value.PTR, width,
                                            height));
                                    };
                                    flowLayoutPanel1.Controls.Add(but);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
