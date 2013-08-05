using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Collections;

namespace ArdupilotMega
{
    public partial class CameraPlanner : Form
    {
        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        Dictionary<string, camerainfo> cameraplanners = new Dictionary<string, camerainfo>();

        public camerainfo camera;

        public struct camerainfo
        {
            public string name;
            public int flyalt;
            public float focallen;
            public float sensorwidth;
            public float sensorheight;
            public float sensorres;
            public int overlap;
            public int sidelap;
            public int angle;
            public bool orient;
            public bool drawinterior;
            public bool done;
        }

        public CameraPlanner()
        {
            InitializeComponent();

            doCalc();
        }

        void doCalc()
        {
            try
            {
                // entered values
                float focallen      = (float)num_focallength.Value;
                int flyalt          = (int)num_agl.Value;
                float sensorwidth   = (float)num_senswidth.Value;
                float sensorheight  = (float)num_sensheight.Value;
                float sensorres     = (float)num_sensres.Value;
                int overlap         = (int)num_overlap.Value;
                int sidelap         = (int)num_sidelap.Value;
                int angle           = (int)fp_angle.Value;
                bool orient         = (bool)radio_camdir.Checked;
                bool drawinterior   = (bool)CHKFill.Checked;
                bool done           = false;

                float sensorlat;   // sensor size across the direction of travel [mm]
                float sensorlong;  // sensor size along the direction of travel [mm]

                switch (orient)
                {
                    //Portrait=true, Landscape=false
                    case true:
                        sensorlat = sensorheight;
                        sensorlong = sensorwidth;
                        break;
                    case false:
                        sensorlat = sensorwidth;
                        sensorlong = sensorheight;
                        break;
                    default:
                        sensorlat = sensorheight;
                        sensorlong = sensorwidth;
                        break;
                }
                float sensorfplat = (flyalt*sensorlat/(focallen)); // lateral sensor footprint on the ground [meters]
                float sensorfplong = (flyalt * sensorlong / (focallen)); // longitudinal sensor footprint on the ground[meters]
                float sensorfpres = sensorfplat*sensorfplong/(sensorres*1000); // square centimeters per pixel
                double picevery = sensorfplong*(1 - overlap * .01);

                TXT_sensor_fplat.Text = sensorfplat.ToString("#");
                TXT_sensor_fplong.Text = sensorfplong.ToString("#");
                TXT_fp_res.Text = sensorfpres.ToString("#.#");
                TXT_picevery.Text = picevery.ToString("#");

                if (radio_camdir.Checked)
                {
                    orient = true;
                }
                else
                {
                    orient = false;
                }

                camera.flyalt = flyalt;
                camera.focallen = focallen;
                camera.sensorwidth = sensorwidth;
                camera.sensorheight = sensorheight;
                camera.sensorres = sensorres;
                camera.overlap = overlap;
                camera.sidelap = sidelap;
                camera.angle = angle;
                camera.orient = orient;
                camera.drawinterior = drawinterior;
                camera.done = done;

            }
            catch { return; }
        }

        private void num_agl_ValueChanged_1(object sender, EventArgs e)
        {
            doCalc();
        }

        private void num_focallength_ValueChanged_1(object sender, EventArgs e)
        {
            doCalc();
        }

        private void num_senswidth_ValueChanged(object sender, EventArgs e)
        {
            doCalc();
        }

        private void num_sensheight_ValueChanged(object sender, EventArgs e)
        {
            doCalc();
        }

        private void num_sensres_ValueChanged(object sender, EventArgs e)
        {
            doCalc();
        }

        private void num_overlap_ValueChanged_1(object sender, EventArgs e)
        {
            doCalc();
        }

        private void num_sidelap_ValueChanged_1(object sender, EventArgs e)
        {
            doCalc();
        }

        private void fp_angle_ValueChanged(object sender, EventArgs e)
        {
            doCalc();
        }

        private void CHK_camdirection_CheckedChanged(object sender, EventArgs e)
        {
            doCalc();
        }
        
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            doCalc();
        }

        private void radio_camdir_CheckedChanged(object sender, EventArgs e)
        {
            doCalc();
        }

        private void xmlcamera(bool write)
        {
            string filename = "cameraplanners.xml";

            if (write || !File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + filename))
            {
                try
                {
                    XmlTextWriter xmlwriter = new XmlTextWriter(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + filename, Encoding.ASCII);
                    xmlwriter.Formatting = Formatting.Indented;

                    xmlwriter.WriteStartDocument();

                    xmlwriter.WriteStartElement("CameraPlanners");

                    foreach (string key in cameraplanners.Keys)
                    {
                        try
                        {
                            if (key == "")
                                continue;
                            xmlwriter.WriteStartElement("CameraPlanner");
                            xmlwriter.WriteElementString("name", cameraplanners[key].name);
                            xmlwriter.WriteElementString("flen", cameraplanners[key].focallen.ToString(new System.Globalization.CultureInfo("en-US")));
                            xmlwriter.WriteElementString("flalt", cameraplanners[key].flyalt.ToString(new System.Globalization.CultureInfo("en-US")));
                            xmlwriter.WriteElementString("senh", cameraplanners[key].sensorheight.ToString(new System.Globalization.CultureInfo("en-US")));
                            xmlwriter.WriteElementString("senw", cameraplanners[key].sensorwidth.ToString(new System.Globalization.CultureInfo("en-US")));
                            xmlwriter.WriteElementString("senr", cameraplanners[key].sensorres.ToString(new System.Globalization.CultureInfo("en-US")));
                            xmlwriter.WriteElementString("ol", cameraplanners[key].overlap.ToString(new System.Globalization.CultureInfo("en-US")));
                            xmlwriter.WriteElementString("sl", cameraplanners[key].sidelap.ToString(new System.Globalization.CultureInfo("en-US")));
                            xmlwriter.WriteEndElement();
                        }
                        catch { }
                    }

                    xmlwriter.WriteEndElement();

                    xmlwriter.WriteEndDocument();
                    xmlwriter.Close();

                }
                catch (Exception ex) { CustomMessageBox.Show(ex.ToString()); }
            }
            else
            {
                try
                {
                    using (XmlTextReader xmlreader = new XmlTextReader(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + filename))
                    {
                        while (xmlreader.Read())
                        {
                            xmlreader.MoveToElement();
                            try
                            {
                                switch (xmlreader.Name)
                                {
                                    case "CameraPlanner":
                                        {
                                            camerainfo camera = new camerainfo();

                                            while (xmlreader.Read())
                                            {
                                                bool dobreak = false;
                                                xmlreader.MoveToElement();
                                                switch (xmlreader.Name)
                                                {
                                                    case "name":
                                                        camera.name = xmlreader.ReadString();
                                                        break;
                                                    case "flen":
                                                        camera.focallen = float.Parse(xmlreader.ReadString(), new System.Globalization.CultureInfo("en-US"));
                                                        break;
                                                    case "flalt":
                                                        camera.flyalt = int.Parse(xmlreader.ReadString(), new System.Globalization.CultureInfo("en-US"));
                                                        break;
                                                    case "senw":
                                                        camera.sensorwidth = float.Parse(xmlreader.ReadString(), new System.Globalization.CultureInfo("en-US"));
                                                        break;
                                                    case "senh":
                                                        camera.sensorheight = float.Parse(xmlreader.ReadString(), new System.Globalization.CultureInfo("en-US"));
                                                        break;
                                                    case "senr":
                                                        camera.sensorres = float.Parse(xmlreader.ReadString(), new System.Globalization.CultureInfo("en-US"));
                                                        break;
                                                    case "ol":
                                                        camera.overlap = int.Parse(xmlreader.ReadString(), new System.Globalization.CultureInfo("en-US"));
                                                        break;
                                                    case "sl":
                                                        camera.sidelap = int.Parse(xmlreader.ReadString(), new System.Globalization.CultureInfo("en-US"));
                                                        break;
                                                    case "CameraPlanner":
                                                        cameraplanners.Add(camera.name, camera);
                                                        CMB_camera.Items.Add(camera.name);
                                                        dobreak = true;
                                                        break;
                                                }
                                                if (dobreak)
                                                    break;
                                            }
                                            string temp = xmlreader.ReadString();
                                        }
                                        break;
                                    case "Config":
                                        break;
                                    case "xml":
                                        break;
                                    default:
                                        if (xmlreader.Name == "") // line feeds
                                            break;
                                        //config[xmlreader.Name] = xmlreader.ReadString();
                                        break;
                                }
                            }
                            catch (Exception ee) { Console.WriteLine(ee.Message); } // silent fail on bad entry
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("Bad Camera File: " + ex.ToString()); } // bad config file
            }
        }

        private void Camera_Load(object sender, EventArgs e)
        {
            xmlcamera(false);
        }

        private void CameraPlanner_Load(object sender, EventArgs e)
        {
            xmlcamera(false);
        }

        private void OokButton_Click(object sender, EventArgs e)
        {
            doCalc();
            OokButton.DialogResult = DialogResult.OK;
            camera.done = true;
            //return;
        }

        private void BUT_Ssave_Click(object sender, EventArgs e)
        {
            camerainfo camera = new camerainfo();

            // check if camera exists alreay
            if (cameraplanners.ContainsKey(CMB_camera.Text))
            {
                camera = cameraplanners[CMB_camera.Text];
            }
            else
            {
                cameraplanners.Add(CMB_camera.Text, camera);
            }

            try
            {
                camera.name = CMB_camera.Text;
                camera.focallen = (float)num_focallength.Value;
                camera.flyalt = (int)num_agl.Value;
                camera.sensorheight = (float)num_sensheight.Value;
                camera.sensorwidth = (float)num_senswidth.Value;
                camera.sensorres = (float)num_sensres.Value;
                camera.overlap = (int)num_overlap.Value;
                camera.sidelap = (int)num_sidelap.Value;
            }
            catch { CustomMessageBox.Show("One of your entries is not a valid number"); return; }

            cameraplanners[CMB_camera.Text] = camera;

            xmlcamera(true);
        }

        private void CMB_camera_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (cameraplanners.ContainsKey(CMB_camera.Text))
            {
                camerainfo camera = cameraplanners[CMB_camera.Text];

                num_focallength.Value = (decimal)camera.focallen;
                num_agl.Value = (decimal)camera.flyalt;
                num_sensheight.Value = (decimal)camera.sensorheight;
                num_senswidth.Value = (decimal)camera.sensorwidth;
                num_sensres.Value = (decimal)camera.sensorres;
                num_overlap.Value = (decimal)camera.overlap;
                num_sidelap.Value = (decimal)camera.sidelap;
            }
            doCalc();
        }
    }
}