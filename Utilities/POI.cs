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
        /// retried on the next subscription. Only a file that does not exist counts as nothing to
        /// read; File.Exists cannot tell that apart from a file that cannot be read.
        /// </summary>
        /// <returns>true when the list was read now and the pages have been redrawn</returns>
        private static bool LoadSaved()
        {
            if (loaded)
                return false;

            int duplicates = 0;
            try
            {
                duplicates = LoadFile(filename);
            }
            catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException)
            {
                // no saved file yet; the first save creates it
            }
            catch (Exception ex)
            {
                // read again when the next page subscribes; until then the file is not written
                log.Error("Failed to read " + filename + ", will try again on the next subscription", ex);
                return false;
            }

            loaded = true;

            // write the file back without its duplicates, which repairs files that grew before the
            // list was loaded once. A failed write must not stop the redraw below: the caller is
            // told the pages have been drawn, and draws nothing itself.
            if (duplicates > 0)
            {
                try
                {
                    SaveFile(filename);
                }
                catch (Exception ex)
                {
                    log.Error("Failed to rewrite " + filename + " without its duplicate POIs", ex);
                }
            }

            // redraw now
            try
            {
                if (_POIModified != null)
                    _POIModified(null, null);
            }
            catch (Exception ex)
            {
                log.Error("A page failed to draw the POIs after loading " + filename, ex);
            }

            return true;
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

        /// <summary>
        /// The index of the POI a marker was drawn from, which UpdateOverlay puts in the marker's
        /// Tag, or -1 when the list no longer holds it. Found by reference: two POIs with the same
        /// position, altitude and name are equal to IndexOf, and two at the same position are
        /// equal to a position match, so either would find the first of them.
        /// </summary>
        private static int IndexOfMarker(GMapMarkerPOI marker)
        {
            if (marker?.Tag is PointLatLngAlt pnt)
            {
                for (int a = 0; a < POIs.Count; a++)
                {
                    if (ReferenceEquals(POIs[a], pnt))
                        return a;
                }
            }

            return -1;
        }

        public static void POIDelete(GMapMarkerPOI Point)
        {
            var a = IndexOfMarker(Point);
            if (a < 0)
                return;

            POI.POIs.RemoveAt(a);
            if (_POIModified != null)
                _POIModified(null, null);
        }

        public static void POIEdit(GMapMarkerPOI Point)
        {
            if (Point == null)
                return;

            string output = "";

            if (DialogResult.OK != InputBox.Show("POI", "Enter ID", ref output))
                return;

            var a = IndexOfMarker(Point);
            if (a < 0)
                return;

            POI.POIs[a].Tag = output;
            if (_POIModified != null)
                _POIModified(null, null);
        }

        public static void POIMove(GMapMarkerPOI Point)
        {
            // the marker's Position is already where it was dropped
            var a = IndexOfMarker(Point);
            if (a >= 0)
            {
                POIs[a].Lat = Point.Position.Lat;
                POIs[a].Lng = Point.Position.Lng;
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
                    string line = CoordText(item.Lat) + "\t" + CoordText(item.Lng) + "\t" + NameOf(item) + "\t" +
                                  item.Alt.ToString(CultureInfo.InvariantCulture) + "\r\n";
                    // UTF-8 so a name outside ASCII survives a save. The reader (StreamReader)
                    // already decodes UTF-8, and files written as ASCII are valid UTF-8 too.
                    byte[] buffer = Encoding.UTF8.GetBytes(line);
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

                            double alt = 0;
                            if (items.Length > 3)
                                double.TryParse(items[3], NumberStyles.Float, CultureInfo.InvariantCulture, out alt);

                            if (!held.Add(Key(lat, lng, items[2])))
                            {
                                skipped++;
                                continue;
                            }

                            POIAdd(new PointLatLngAlt(lat, lng, alt), items[2]);
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