using GMap.NET.WindowsForms;
using MissionPlanner.Controls;
using MissionPlanner.Maps;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Utilities
{
    public class POI
    {
        /// <summary>
        /// Store points of interest
        /// </summary>
        static ObservableCollection<PointLatLngAlt> POIs = new ObservableCollection<PointLatLngAlt>();

        private static EventHandler _POIModified;

        public static event EventHandler POIModified
        {
            add
            {
                _POIModified += value;
                try
                {
                    // Several pages subscribe (Data, Plan). The saved file is loaded on the
                    // first subscription only; loading it once per subscriber duplicated every
                    // POI on each start, and the next save made the duplicates permanent.
                    if (!loaded)
                    {
                        loaded = true;
                        if (File.Exists(filename))
                            LoadFile(filename);
                    }
                }
                catch
                {
                }
            }
            remove { _POIModified -= value; }
        }

        private static string filename = Settings.GetUserDataDirectory() + "poi.txt";
        private static bool loading;
        private static bool loaded;

        /// <summary>The POI name: the first line of Tag, which also carries the position text.</summary>
        private static string NameOf(PointLatLngAlt pnt)
        {
            var tag = pnt.Tag ?? "";
            var nl = tag.IndexOf('\n');
            return nl >= 0 ? tag.Substring(0, nl) : tag;
        }

        static POI()
        {
            POIs.CollectionChanged += POIs_CollectionChanged;
        }

        private static void POIs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (loading)
                    return;
                SaveFile(filename);
            }
            catch { }
        }

        public static void POIAdd(PointLatLngAlt Point, string tag)
        {
            // local copy
            PointLatLngAlt pnt = Point;

            pnt.Tag = tag + "\n" + pnt.ToString();

            POI.POIs.Add(pnt);

            if (_POIModified != null && !loading)
                _POIModified(null, null);
        }

        public static void POIAdd(PointLatLngAlt Point)
        {
            if (Point == null)
                return;

            PointLatLngAlt pnt = Point;

            string output = "";

            if (DialogResult.OK != InputBox.Show("POI", "Enter ID", ref output))
                return;

            POIAdd(Point, output);
        }

        public static void POIDelete(GMapMarkerPOI Point)
        {
            if (Point == null)
                return;

            for (int a = 0; a < POI.POIs.Count; a++)
            {
                if (POI.POIs[a].Point() == Point.Position)
                {
                    POI.POIs.RemoveAt(a);
                    if (_POIModified != null)
                        _POIModified(null, null);
                    return;
                }
            }
        }

        public static void POIEdit(GMapMarkerPOI Point)
        {
            if (Point == null)
                return;

            string output = "";

            if (DialogResult.OK != InputBox.Show("POI", "Enter ID", ref output))
                return;

            for (int a = 0; a < POI.POIs.Count; a++)
            {
                if (POI.POIs[a].Point() == Point.Position)
                {
                    POI.POIs[a].Tag = output + "\n" + Point.Position.ToString();
                    if (_POIModified != null)
                        _POIModified(null, null);
                    return;
                }
            }
        }

        public static void POIMove(GMapMarkerPOI Point)
        {
            for (int a = 0; a < POI.POIs.Count; a++)
            {
                if (POIs[a].Tag == Point.ToolTipText)
                {
                    POIs[a].Lat = Point.Position.Lat;
                    POIs[a].Lng = Point.Position.Lng;
                    POIs[a].Tag = POIs[a].Tag.Substring(0, POIs[a].Tag.IndexOf('\n')) + "\n" + Point.Position.ToString();
                    break;
                }
            }

            if (_POIModified != null)
                _POIModified(null, null);
        }

        public static void POISave()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Poi File|*.txt";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    SaveFile(sfd.FileName);
                }
            }
        }

        private static void SaveFile(string fileName)
        {
            using (Stream file = File.Open(fileName, FileMode.Create))
            {
                foreach (var item in POI.POIs)
                {
                    string line = item.Lat.ToString(CultureInfo.InvariantCulture) + "\t" +
                                  item.Lng.ToString(CultureInfo.InvariantCulture) + "\t" + item.Tag.Substring(0, item.Tag.IndexOf('\n')) + "\r\n";
                    byte[] buffer = ASCIIEncoding.ASCII.GetBytes(line);
                    file.Write(buffer, 0, buffer.Length);
                }
            }
        }


        public static void POILoad()
        {
            using (OpenFileDialog sfd = new OpenFileDialog())
            {
                sfd.Filter = "Poi File|*.txt";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    LoadFile(sfd.FileName);
                }
            }
        }

        /// <summary>
        /// Merge the POIs in <paramref name="fileName"/> into the list. An entry with the same
        /// position and name as one already held is skipped, so loading a file twice, or a file
        /// that already contains duplicates, never piles markers on top of each other. When
        /// anything was skipped the saved file is rewritten without the duplicates.
        /// </summary>
        private static void LoadFile(string fileName)
        {
            int skipped = 0;
            loading = true;
            try
            {
                using (Stream file = File.Open(fileName, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(file))
                    {
                        while (!sr.EndOfStream)
                        {
                            string[] items = sr.ReadLine().Split('\t');

                            if (items.Count() < 3)
                                continue;

                            double lat, lng;
                            if (!double.TryParse(items[0], NumberStyles.Float, CultureInfo.InvariantCulture, out lat) ||
                                !double.TryParse(items[1], NumberStyles.Float, CultureInfo.InvariantCulture, out lng))
                                continue;

                            if (Contains(lat, lng, items[2]))
                            {
                                skipped++;
                                continue;
                            }

                            POIAdd(new PointLatLngAlt(lat, lng), items[2]);
                        }
                    }
                }
            }
            finally
            {
                loading = false;
            }

            if (skipped > 0)
            {
                try
                {
                    SaveFile(filename);
                }
                catch
                {
                }
            }

            // redraw now
            if (_POIModified != null)
                _POIModified(null, null);
        }

        /// <summary>True if a POI with this position and name is already in the list.</summary>
        private static bool Contains(double lat, double lng, string name)
        {
            foreach (var pnt in POIs)
            {
                if (pnt.Lat == lat && pnt.Lng == lng &&
                    string.Equals(NameOf(pnt), name, StringComparison.Ordinal))
                    return true;
            }
            return false;
        }

        public static void UpdateOverlay(GMap.NET.WindowsForms.GMapOverlay poioverlay)
        {
            if (poioverlay == null)
                return;

            poioverlay.Clear();

            foreach (var pnt in POIs)
            {
                poioverlay.Markers.Add(new GMapMarkerPOI(pnt)
                {
                    ToolTipMode = MarkerTooltipMode.OnMouseOver,
                    ToolTipText = pnt.Tag
                });
            }
        }
    }
}