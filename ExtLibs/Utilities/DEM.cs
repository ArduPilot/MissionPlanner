using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib.Zip;
using System.Threading;
using System.Collections;
using log4net;
using System.Xml;
using MissionPlanner.Utilities; // GE xml alt reader

namespace MissionPlanner.Utilities
{
    public class DEM 
    {
        public static PointLatLngAlt getAltitude(PointLatLngAlt location )
        {
            double alt = 0;
            double lat = 0;
            double lng = 0;

            int pos = 0;

            PointLatLngAlt answer = new PointLatLngAlt();

            //http://code.google.com/apis/maps/documentation/elevation/
            //http://maps.google.com/maps/api/elevation/xml
            string coords = "";



                coords = coords + location.Lat.ToString(new System.Globalization.CultureInfo("en-US")) + "," +
                         location.Lng.ToString(new System.Globalization.CultureInfo("en-US")) + "|";


            coords = coords.Remove(coords.Length - 1);

            try
            {
                using (
                    XmlTextReader xmlreader =
                        new XmlTextReader("http://maps.google.com/maps/api/elevation/xml?path=" + coords + "&samples=" +
                                          (0).ToString(new System.Globalization.CultureInfo("en-US")) +
                                          "&sensor=false"))
                {
                    while (xmlreader.Read())
                    {
                        xmlreader.MoveToElement();
                        switch (xmlreader.Name)
                        {
                            case "elevation":
                                alt = double.Parse(xmlreader.ReadString(), new System.Globalization.CultureInfo("en-US"));
                                Console.WriteLine("DO it " + lat + " " + lng + " " + alt);
                                PointLatLngAlt loc = new PointLatLngAlt(lat, lng, alt, "");
                                answer = loc;
                                pos++;
                                break;
                            case "lat":
                                lat = double.Parse(xmlreader.ReadString(), new System.Globalization.CultureInfo("en-US"));
                                break;
                            case "lng":
                                lng = double.Parse(xmlreader.ReadString(), new System.Globalization.CultureInfo("en-US"));
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch
            {
                //CustomMessageBox.Show("Error getting GE data", Strings.ERROR);
            }

            return answer;
        }
    }
}
