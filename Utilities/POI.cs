using GMap.NET.WindowsForms;
using log4net;
using MissionPlanner.Controls;
using MissionPlanner.Maps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Utilities
{
    public class POI
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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

                // the first subscription reads the saved file and redraws every page
                if (LoadSaved())
                    return;

                // a page subscribing later (Plan opens after Data) still has to draw the POIs
                try
                {
                    value(null, null);
                }
                catch (Exception ex)
                {
                    log.Error("A page failed to draw the POIs when it subscribed", ex);
                }
            }
            remove { _POIModified -= value; }
        }

        private static string filename = Settings.GetUserDataDirectory() + "poi.txt";
        private static bool loading;
        // the saved file has been read, or there is none; until then it is not written
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
                // before the saved file has been read, saving would replace it with a list that
                // is missing its POIs
                if (loading || !loaded)
                    return;
                SaveFile(filename);
            }
            catch { }
        }

        /// <summary>
        /// Read the saved file into the list. Several pages subscribe (Data, Plan); loading it once
        /// per subscriber duplicated every POI on each start, and the next save made the duplicates
        /// permanent. A read that fails, for example while another program holds the file, is
        /// retried on the next subscription.
        /// </summary>
        /// <returns>true when the list was read now and the pages have been redrawn</returns>
        private static bool LoadSaved()
        {
            if (loaded)
                return false;

            int duplicates = 0;
            try
            {
                if (File.Exists(filename))
                    duplicates = LoadFile(filename);
            }
            catch (Exception ex)
            {
                // read again when the next page subscribes; until then the file is not written
                log.Error("Failed to read " + filename + ", will try again on the next subscription", ex);
                return false;
            }

            loaded = true;

            try
            {
                // write the file back without its duplicates, which repairs files that grew
                // before the list was loaded once
                if (duplicates > 0)
                    SaveFile(filename);

                // redraw now
                if (_POIModified != null)
                    _POIModified(null, null);
            }
            catch (Exception ex)
            {
                log.Error("Failed to rewrite or redraw the POIs after loading " + filename, ex);
            }

            return true;
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
                    string line = CoordText(item.Lat) + "\t" + CoordText(item.Lng) + "\t" +
                                  item.Tag.Substring(0, item.Tag.IndexOf('\n')) + "\r\n";
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

                    // redraw now
                    if (_POIModified != null)
                        _POIModified(null, null);
                }
            }
        }

        /// <summary>
        /// Merge the POIs in <paramref name="fileName"/> into the list. An entry with the same
        /// position and name as one already held is skipped, so loading a file twice, or a file
        /// that already contains duplicates, never piles markers on top of each other.
        /// </summary>
        /// <returns>the number of entries skipped as duplicates</returns>
        private static int LoadFile(string fileName)
        {
            var held = new HashSet<string>(POIs.Select(pnt => Key(pnt.Lat, pnt.Lng, NameOf(pnt))));
            int skipped = 0;
            loading = true;
            try
            {
                // read only, and let other programs (virus scanners, sync clients) keep it open
                using (Stream file = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(file))
                    {
                        while (!sr.EndOfStream)
                        {
                            string[] items = sr.ReadLine().Split('\t');

                            if (items.Length < 3)
                                continue;

                            double lat, lng;
                            if (!double.TryParse(items[0], NumberStyles.Float, CultureInfo.InvariantCulture, out lat) ||
                                !double.TryParse(items[1], NumberStyles.Float, CultureInfo.InvariantCulture, out lng))
                                continue;

                            if (!held.Add(Key(lat, lng, items[2])))
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

            return skipped;
        }

        /// <summary>
        /// Identifies a POI when skipping duplicates: the position exactly as SaveFile writes it,
        /// and the name. Comparing the written text rather than the doubles lets a point placed on
        /// the map match the same point read back from a saved file.
        /// </summary>
        private static string Key(double lat, double lng, string name)
        {
            return CoordText(lat) + "\t" + CoordText(lng) + "\t" + name;
        }

        /// <summary>
        /// A latitude or longitude as the file stores it. SaveFile and Key share this, so the key is
        /// the text the file holds whatever digits the runtime's default double format keeps (15
        /// significant on .NET Framework, the shortest round trip on newer runtimes).
        /// </summary>
        private static string CoordText(double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
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