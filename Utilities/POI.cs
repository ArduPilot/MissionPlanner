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
        /// Store points of interest. Tag holds the name only; Alt is metres above sea level.
        /// The map label is built when drawn so it follows the selected coordinate frame.
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
                    if (File.Exists(filename))
                        LoadFile(filename);
                }
                catch
                {
                }
            }
            remove { _POIModified -= value; }
        }

        private static string filename = Settings.GetUserDataDirectory() + "poi.txt";
        private static bool loading;

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

        /// <summary>
        /// The POI name. Older versions stored "name\nlat,lng,..." in Tag, so only the first
        /// line counts.
        /// </summary>
        private static string NameOf(PointLatLngAlt pnt)
        {
            var tag = pnt.Tag ?? "";
            var nl = tag.IndexOf('\n');
            return nl >= 0 ? tag.Substring(0, nl) : tag;
        }

        /// <summary>
        /// Text shown on the map marker: name, position in the given coordinate frame, altitude.
        /// </summary>
        /// <param name="coordSystem">a Coords.CoordsSystems name, null for lat/long</param>
        public static string Label(PointLatLngAlt pnt, string coordSystem)
        {
            var coords = "";

            if (coordSystem != null && coordSystem != Coords.CoordsSystems.GEO.ToString())
                coords = CoordsInputBox.Format(coordSystem, pnt.Lat, pnt.Lng);

            // GEO, or outside the UTM/MGRS grid
            if (coords == "")
                coords = pnt.Lat.ToString("0.0000000", CultureInfo.InvariantCulture) + ", " +
                         pnt.Lng.ToString("0.0000000", CultureInfo.InvariantCulture);

            var alt = (pnt.Alt * CurrentState.multiplieralt).ToString("0") + CurrentState.AltUnit;

            return NameOf(pnt) + "\n" + coords + "\n" + alt;
        }

        public static void POIAdd(PointLatLngAlt Point, string tag)
        {
            // local copy
            PointLatLngAlt pnt = Point;

            pnt.Tag = tag;

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
                    POI.POIs[a].Tag = output;
                    if (_POIModified != null)
                        _POIModified(null, null);
                    return;
                }
            }
        }

        public static void POIMove(GMapMarkerPOI Point)
        {
            // the marker carries the POI it was drawn from (see UpdateOverlay); the marker's
            // Position is already where it was dropped
            if (Point?.Tag is PointLatLngAlt pnt)
            {
                var idx = POIs.IndexOf(pnt);
                if (idx >= 0)
                {
                    POIs[idx].Lat = Point.Position.Lat;
                    POIs[idx].Lng = Point.Position.Lng;
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
                    // lat, lng, name, alt (metres). Older files have no alt column.
                    string line = item.Lat.ToString(CultureInfo.InvariantCulture) + "\t" +
                                  item.Lng.ToString(CultureInfo.InvariantCulture) + "\t" + NameOf(item) + "\t" +
                                  item.Alt.ToString(CultureInfo.InvariantCulture) + "\r\n";
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

        private static void LoadFile(string fileName)
        {
            loading = true;
            using (Stream file = File.Open(fileName, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    while (!sr.EndOfStream)
                    {
                        string[] items = sr.ReadLine().Split('\t');

                        if (items.Count() < 3)
                            continue;

                        double alt = 0;
                        if (items.Length > 3)
                            double.TryParse(items[3], NumberStyles.Float, CultureInfo.InvariantCulture, out alt);

                        POIAdd(new PointLatLngAlt(double.Parse(items[0], CultureInfo.InvariantCulture)
                            , double.Parse(items[1], CultureInfo.InvariantCulture), alt), items[2]);
                    }
                }
            }
            loading = false;
            // redraw now
            if (_POIModified != null)
                _POIModified(null, null);
        }

        /// <summary>
        /// Redraw the POI markers.
        /// </summary>
        /// <param name="coordSystem">Coords.CoordsSystems name used for the marker labels (null = lat/long)</param>
        public static void UpdateOverlay(GMap.NET.WindowsForms.GMapOverlay poioverlay, string coordSystem = null)
        {
            if (poioverlay == null)
                return;

            poioverlay.Clear();

            foreach (var pnt in POIs)
            {
                poioverlay.Markers.Add(new GMapMarkerPOI(pnt)
                {
                    ToolTipMode = MarkerTooltipMode.OnMouseOver,
                    ToolTipText = Label(pnt, coordSystem),
                    Tag = pnt
                });
            }
        }
    }
}
