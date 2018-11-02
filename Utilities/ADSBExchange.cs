using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionPlanner.Utilities
{
    public class Feed
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool polarPlot { get; set; }
    }

    public class AcList
    {
        public int Id { get; set; }
        public int Rcvr { get; set; }
        public bool HasSig { get; set; }
        public int Sig { get; set; }
        public string Icao { get; set; }
        public bool Bad { get; set; }
        public string Reg { get; set; }
        public DateTime FSeen { get; set; }
        public int TSecs { get; set; }
        public int CMsgs { get; set; }
        public int Alt { get; set; }
        public int GAlt { get; set; }
        public double InHg { get; set; }
        public int AltT { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public object PosTime { get; set; }
        public bool Mlat { get; set; }
        public bool Tisb { get; set; }
        public double Spd { get; set; }
        public double Trak { get; set; }
        public bool TrkH { get; set; }
        public string Type { get; set; }
        public string Mdl { get; set; }
        public string Man { get; set; }
        public string CNum { get; set; }
        public string Op { get; set; }
        public string Sqk { get; set; }
        public int Vsi { get; set; }
        public int VsiT { get; set; }
        public double Dst { get; set; }
        public double Brng { get; set; }
        public int WTC { get; set; }
        public int Species { get; set; }
        public string Engines { get; set; }
        public int EngType { get; set; }
        public int EngMount { get; set; }
        public bool Mil { get; set; }
        public string Cou { get; set; }
        public bool HasPic { get; set; }
        public bool Interested { get; set; }
        public int FlightsCount { get; set; }
        public bool Gnd { get; set; }
        public int SpdTyp { get; set; }
        public bool CallSus { get; set; }
        public int Trt { get; set; }
        public string Year { get; set; }
        public string Call { get; set; }
        public bool? Help { get; set; }
        public int? TAlt { get; set; }
        public bool? PosStale { get; set; }
        public string OpIcao { get; set; }
        public double? TTrk { get; set; }
        public string Tag { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public List<string> Stops { get; set; }
    }

    public class RootObject
    {
        public int src { get; set; }
        public List<Feed> feeds { get; set; }
        public int srcFeed { get; set; }
        public bool showSil { get; set; }
        public bool showFlg { get; set; }
        public bool showPic { get; set; }
        public int flgH { get; set; }
        public int flgW { get; set; }
        public List<AcList> acList { get; set; }
        public int totalAc { get; set; }
        public string lastDv { get; set; }
        public int shtTrlSec { get; set; }
        public long stm { get; set; }
    }
    static class ADSBExchange
    {

        private static string ldv { get; set; }
        private static System.Windows.Forms.Timer tmr = new System.Windows.Forms.Timer();
        private static System.Windows.Forms.Timer timerRenable = new System.Windows.Forms.Timer();

        public static event EventHandler<MissionPlanner.Utilities.adsb.PointLatLngAltHdg> UpdatePlanePosition;

        private static bool stop = false;

        public static void Stop()
        {
            stop = true;
        }

        public static void StartTimer()
        {
            if (tmr.Enabled || timerRenable.Enabled) return;

            stop = false;
            timerRenable.Interval = 60000;
            timerRenable.Tick += (snd, obj) =>
            { tmr.Enabled = true; timerRenable.Enabled = false; };

            tmr.Interval = 3000;
            tmr.Tick += (snd, obj) =>
            {
                //if (MainV2.comPort == null || !MainV2.comPort.BaseStream.IsOpen) return;
                if (MainV2.comPort.MAV == null || MainV2.comPort.MAV.cs == null) return;
                if (MainV2.comPort.MAV.cs.Location == null) return;
                if (MainV2.comPort.MAV.cs.Location.Lat == 0 && MainV2.comPort.MAV.cs.Location.Lng == 0) return;
                //if (MainV2.comPort.MAV.cs.gpsstatus <= 3) return;

                System.ComponentModel.BackgroundWorker bgWorker = new System.ComponentModel.BackgroundWorker();

                bool error = false;
                tmr.Enabled = timerRenable.Enabled = false;

                bgWorker.DoWork += (sender, obj2) =>
                {
                    try
                    {
                        //http://public-api.adsbexchange.com/VirtualRadar/AircraftList.json?lat=33.433638&lng=-112.008113&fDstL=0&fDstU=37.04
                        using (var client = new System.Net.WebClient())
                        {
                            string request = string.Format("http://public-api.adsbexchange.com/VirtualRadar/AircraftList.json?lat={0}&lng={1}&fDstL=0&fDstU=37.04", MainV2.comPort.MAV.cs.Location.Lat, MainV2.comPort.MAV.cs.Location.Lng);
                            if (!string.IsNullOrEmpty(ldv))
                                request = string.Format("http://public-api.adsbexchange.com/VirtualRadar/AircraftList.json?lat={0}&lng={1}&fDstL=0&fDstU=37.04&ldv={2}", MainV2.comPort.MAV.cs.Location.Lat, MainV2.comPort.MAV.cs.Location.Lng, ldv);
                            var json = client.DownloadString(request);
                            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                            RootObject adsbexchange = serializer.Deserialize<RootObject>(json);
                            // TODO: do something with the model
                            //System.Diagnostics.Debug.WriteLine("Found " + adsbexchange.acList.Count + " aircraft within 20nm");
                            if (adsbexchange == null) return;
                            if (!string.IsNullOrEmpty(adsbexchange.lastDv)) ldv = adsbexchange.lastDv;

                            lock (MainV2.instance.adsbPlanes)
                            {
                                float multi = Settings.Instance["distunits"] == "Feet" ? 3.28084f : 1f;
                                foreach (AcList vehicle in adsbexchange.acList)
                                {
                                    string id = vehicle.Id.ToString();
                                    //vehicle alt is always feet, if set to meters change
                                    //GAlt is altitude adjusted for local air pressure
                                    PointLatLngAlt vehicleplla = new PointLatLngAlt(vehicle.Lat, vehicle.Long, vehicle.GAlt * 3.2808399f);
                                    double distance = MainV2.comPort.MAV.cs.Location.GetDistance(vehicleplla) * 0.000539957;
                                    double altsep = vehicleplla.Alt - (MainV2.comPort.MAV.cs.altasl * multi);
                                    if (MainV2.instance.adsbPlanes.ContainsKey(id))
                                    {
                                        // update existing
                                        MainV2.instance.adsbPlanes[id].Lat = vehicle.Lat;
                                        MainV2.instance.adsbPlanes[id].Lng = vehicle.Long;
                                        MainV2.instance.adsbPlanes[id].Alt = vehicleplla.Alt;
                                        MainV2.instance.adsbPlanes[id].Heading = (float)vehicle.Trak;
                                        MainV2.instance.adsbPlanes[id].Time = DateTime.Now;
                                        MainV2.instance.adsbPlanes[id].CallSign = vehicle.Call;
                                        MainV2.instance.adsbPlanes[id].Tag = vehicle.Icao; // + Environment.NewLine + species + "/" + vehicle.Call + Environment.NewLine + vehicle.Mdl;
                                        MainV2.instance.adsbPlanes[id].Vsi = vehicle.Vsi;
                                        MainV2.instance.adsbPlanes[id].Species = (adsb.ADSBSpecies)vehicle.Species;
                                        MainV2.instance.adsbPlanes[id].type = vehicle.Type;
                                    }
                                    else
                                    {
                                        // create new plane
                                        MainV2.instance.adsbPlanes[id] =
                                            new adsb.PointLatLngAltHdg(vehicle.Lat, vehicle.Long,
                                                vehicleplla.Alt, (float)vehicle.Trak, id,
                                                DateTime.Now)
                                            {
                                                CallSign = vehicle.Call,
                                                Tag = vehicle.Icao,
                                                DisplayICAO = true,
                                                Vsi = vehicle.Vsi,
                                                Species = (adsb.ADSBSpecies)vehicle.Species,
                                                type = vehicle.Type
                                            };
                                    }


                                    if (distance <= 2 && altsep <= 152.4 && altsep >= -152.4) //closer than 2NM and verticle seperation <= 500 feet
                                        MainV2.instance.adsbPlanes[id].ThreatLevel = MAVLink.MAV_COLLISION_THREAT_LEVEL.HIGH;
                                    else if (distance <= 5 && altsep <= 457.2 && altsep >= -457.2) //greater than 2NM but less than 5NM and verticle seperation <= 1500 feet
                                        MainV2.instance.adsbPlanes[id].ThreatLevel = MAVLink.MAV_COLLISION_THREAT_LEVEL.LOW;
                                    else
                                        MainV2.instance.adsbPlanes[id].ThreatLevel = MAVLink.MAV_COLLISION_THREAT_LEVEL.NONE;

                                    try
                                    {
                                        //send adsb to mav
                                        if (MainV2.comPort.BaseStream.IsOpen)
                                        {
                                            MAVLink.mavlink_adsb_vehicle_t packet = new MAVLink.mavlink_adsb_vehicle_t();

                                            packet.altitude = (int)(MainV2.instance.adsbPlanes[id].Alt * 1000);
                                            packet.altitude_type = (byte)MAVLink.ADSB_ALTITUDE_TYPE.GEOMETRIC;
                                            packet.callsign = ASCIIEncoding.ASCII.GetBytes(MainV2.instance.adsbPlanes[id].CallSign == null ? MainV2.instance.adsbPlanes[id].Tag : MainV2.instance.adsbPlanes[id].CallSign);
                                            packet.emitter_type = (byte)MAVLink.ADSB_EMITTER_TYPE.NO_INFO;
                                            packet.heading = (ushort)(MainV2.instance.adsbPlanes[id].Heading * 100);
                                            packet.lat = (int)(MainV2.instance.adsbPlanes[id].Lat * 1e7);
                                            packet.lon = (int)(MainV2.instance.adsbPlanes[id].Lng * 1e7);
                                            packet.ICAO_address = uint.Parse(id, System.Globalization.NumberStyles.HexNumber);

                                            packet.flags = (ushort)(MAVLink.ADSB_FLAGS.VALID_ALTITUDE | MAVLink.ADSB_FLAGS.VALID_COORDS |
                                                MAVLink.ADSB_FLAGS.VALID_HEADING | MAVLink.ADSB_FLAGS.VALID_CALLSIGN);

                                            //send to current connected
                                            MainV2.comPort.sendPacket(packet, MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid);
                                        }
                                    }
                                    catch
                                    {

                                    }
                                }

                                foreach (var key in MainV2.instance.adsbPlanes.Keys)
                                {
                                    adsb.PointLatLngAltHdg tmp;
                                    if (!adsbexchange.acList.Any(x => x.Id.ToString() == key && x.Gnd == false))
                                        MainV2.instance.adsbPlanes.TryRemove(key, out tmp);
                                }
                            }
                        }
                    }
                    catch (System.Net.WebException webException)
                    {
                        error = true;
                        timerRenable.Enabled = true;
                    }
                    catch { }
                };

                if (!error && !stop) tmr.Enabled = true;
                bgWorker.RunWorkerAsync();
                
            };
            tmr.Enabled = true;
        }
    }
}
