using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using MissionPlanner;
using MissionPlanner.Plugin;
using MissionPlanner.Utilities;
using TrackerHomeGPS;

    public class TrackerHome : Plugin
    {
        private string _Name = "GPS Tracker Home Plugin";
        private string _Version = "1.0";
        private string _Author = "Will Bryan";

        public override string Name { get { return _Name; } }
        public override string Version { get { return _Version; } }
        public override string Author { get { return _Author; } }

        private bool _Available = false;

        public override bool Init() { loopratehz = 0.0f;  return true; }

        public override bool Loaded()
        {
            /* Register with Device Change event */
            Host.DeviceChanged += deviceChanged;
        /* Add to Flight Planner Map Menu */
        ToolStripMenuItem trkrHome = new ToolStripMenuItem(Strings.TrackerHome)
        {
            Name = "trkrHomeMenuItem"
        };
        ToolStripMenuItem obtainFrmMod = new ToolStripMenuItem(Strings.ObtainFromModule);
            obtainFrmMod.Click += setTrackerHomeFromModule;
            ToolStripMenuItem setAtLoc = new ToolStripMenuItem(Strings.SetHere);
            setAtLoc.Click += setFromPlannerLocation;

            trkrHome.DropDownItems.AddRange( new ToolStripItem[] { obtainFrmMod, setAtLoc } );

            ToolStripItemCollection col = Host.FPMenuMap.Items;
            int index = col.Count;
            foreach (ToolStripItem item in col)
            {
                if (item.Text.Equals(Strings.TrackerHome))
                {
                    index = col.IndexOf(item);
                    col.Remove(item);
                    break;
                }
            }
            if (index != col.Count) col.Insert(index, trkrHome);
            else col.Add(trkrHome);

            if (getDevice() != null) _Available = true;
            
            return true;
        }

        void deviceChanged(MainV2.WM_DEVICECHANGE_enum cause)
        {
            GPSDevice gpsModule = getDevice();
            if ((gpsModule != null))
            {
                if (_Available == false)
                {
                    _Available = true;
                    if (CustomMessageBox.Show("A GPS module was detected on your system. Would you like to use it to set your tracker home location?", "Tracker Home", MessageBoxButtons.YesNo) == (int)DialogResult.Yes)
                    {
                        GPSPosition pos = gpsModule.GetCoordinates();
                        double alt = getGEAlt(pos.Lat, pos.Lng);
                        Host.comPort.MAV.cs.TrackerLocation = new PointLatLngAlt(pos.Lat, pos.Lng, alt, "Tracker Home");
                    }
                }
            }
            else if (_Available == true) _Available = false;
        }

        public override bool Exit()
        {
            return true;
        }

        private GPSDevice getDevice()
        {
            if (GarminUSBGPS.DevicePresent())
            {
                return new GarminUSBGPS();
            }
            else
            {
                return null;
            }
        }

        void setFromPlannerLocation(object sender, EventArgs e)
        {
            Host.comPort.MAV.cs.TrackerLocation = new PointLatLngAlt(this.Host.FPMenuMapPosition);

            Host.comPort.MAV.cs.TrackerLocation.Alt = srtm.getAltitude(Host.comPort.MAV.cs.TrackerLocation.Lat, Host.comPort.MAV.cs.TrackerLocation.Lng).alt;
        }

        void setTrackerHomeFromModule(object sender, EventArgs e)
        {
            GPSDevice gpsModule = getDevice();
            if (gpsModule != null)
            {
                _Available = true;
                GPSPosition pos = gpsModule.GetCoordinates();
                double alt = getGEAlt(pos.Lat, pos.Lng);
                Host.comPort.MAV.cs.TrackerLocation = new PointLatLngAlt(pos.Lat, pos.Lng, alt, "Tracker Home");
            }
            else
            {
                _Available = false;
                CustomMessageBox.Show("No GPS Device connected. Please verify it is connected and try again.");
            }
        }

        double getGEAlt(double lat, double lng)
        {
            double alt = 0;
            //http://maps.google.com/maps/api/elevation/xml

            try
            {
                using (XmlTextReader xmlreader = new XmlTextReader("http://maps.google.com/maps/api/elevation/xml?locations=" + lat.ToString(new System.Globalization.CultureInfo("en-US")) + "," + lng.ToString(new System.Globalization.CultureInfo("en-US")) + "&sensor=true"))
                {
                    while (xmlreader.Read())
                    {
                        xmlreader.MoveToElement();
                        switch (xmlreader.Name)
                        {
                            case "elevation":
                                alt = double.Parse(xmlreader.ReadString(), new System.Globalization.CultureInfo("en-US"));
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch { }

            return alt * CurrentState.multiplierdist;
        }
    }