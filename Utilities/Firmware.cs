﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using MissionPlanner.Arduino;
using MissionPlanner.Comms;
using log4net;
using px4uploader;
using System.Collections;
using System.Xml.Serialization;

namespace MissionPlanner.Utilities
{
    public class Firmware
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public event ProgressEventHandler Progress;

        string firmwareurl = "https://raw.github.com/diydrones/binary/master/Firmware/firmware2.xml";
		
		//string firmwareurl = "http://www.radionav.it/portale/MissionPlanner/firmware2.xml";

        // ap 2.5 - ac 2.7
        //"https://meee146-planner.googlecode.com/git-history/dfc5737c5efc1e7b78e908829a097624c273d9d7/Tools/MissionPlanner.lanner/Firmware/firmware2.xml";
        //"http://meee146-planner.googlecode.com/git/Tools/MissionPlanner.lanner/Firmware/AC2-Y6-1280.hex"
        //https://github.com/diydrones/binary/raw/f159deedbe4dee7134d711ed4390ea30be8b68e6/Firmware/AP-2560.size.txt
        readonly string gcoldurl = ("https://meee146-planner.googlecode.com/git-history/!Hash!/Tools/MissionPlanner.lanner/Firmware/firmware2.xml");
        readonly string gcoldfirmwareurl = ("https://meee146-planner.googlecode.com/git-history/!Hash!/Tools/MissionPlanner.lanner/Firmware/!Firmware!");
        string[] gcoldurls = new string[] { "76ff91fe7b2940a509ea7dfd728542491f480372", "bb5ee0e1c3e643e7e359ffb4c8bde34aa7d4f996", "55ec5eaf662a56044ea25c894d235d17185f0660", "cb5b736976c7ed791ea45675c31f588ecb8228d4", "bcd5239322df38db011f183e48d596f215803838", "8709cc418e00326295abc562530413c0089807a7", "06a64192df594b0f81233dfb1f0214aab2cb2603", "7853ef3fad98e5053f228b7c1748c76858c4d282", "abe930ce723267697542388ef181328f00371f40", "26305d5790333f730cd396afcd08c165cde33ed7", "bc1f26ca40b076e3d06f173adad772fb25aa6512", "dfc5737c5efc1e7b78e908829a097624c273d9d7", "682065db449b6c79d89717908ed8beea1ed6a03a", "b21116847d35472b9ab770408cbeb88ed2ed0a95", "511e00bc89a554aea8768a274bff28af532cd335", "1da56714aa1ed88dcdb078a90d33bcef4eb4315f", "8aa4c7a1ed07648f31335926cc6bcc06c87dc536" };
        readonly string gholdurl = ("https://github.com/diydrones/binary/raw/!Hash!/Firmware/firmware2.xml");
        readonly string gholdfirmwareurl = ("https://github.com/diydrones/binary/raw/!Hash!/Firmware/!Firmware!");
        string[] gholdurls = new string[] { };
        public List<KeyValuePair<string, string>> niceNames = new List<KeyValuePair<string, string>>();

        int ingetapmversion = 0;

        List<software> softwares = new List<software>();

        [Serializable]
        [XmlType(TypeName = "software")]
        public struct software
        {
            public string url;
            public string url2560;
            public string url2560_2;
            public string urlpx4v1;
            public string urlpx4v2;
            public string urlvrbrainv40;
            public string urlvrbrainv45;
            public string urlvrbrainv50;
            public string urlvrbrainv51;
            public string urlvrherov10;
            public string urlvrubrainv51;
            public string name;
            public string desc;
            public int k_format_version;
        }

        public string getUrl(string hash, string filename)
        {
            if (hash.ToLower().StartsWith("http"))
            {
                if (filename == "")
                    return hash;

                var url = new Uri(hash);
                return new Uri(url, filename).AbsoluteUri;
            }

            foreach (string x in gholdurls)
            {
                if (x == hash)
                {
                    if (filename == "")
                        return gholdurl.Replace("!Hash!", hash);
                    string fn = Path.GetFileName(filename);
                    filename = gholdfirmwareurl.Replace("!Hash!", hash);
                    filename = filename.Replace("!Firmware!", fn);
                    return filename;
                }
            }
            foreach (string x in gcoldurls)
            {
                if (x == hash)
                {
                    if (filename == "")
                        return gcoldurl.Replace("!Hash!", hash);
                    string fn = Path.GetFileName(filename);
                    filename = gcoldfirmwareurl.Replace("!Hash!", hash);
                    filename = filename.Replace("!Firmware!", fn);
                    return filename;
                }
            }
            return "";
        }



        /// <summary>
        /// Load firmware history from file
        /// </summary>
        public Firmware()
        {
            string file = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + "FirmwareHistory.txt";

            if (!File.Exists(file))
            {
                //CustomMessageBox.Show("Missing FirmwareHistory.txt file");
                return;
            }

            gholdurls = File.ReadAllLines(file);
            int a = 0;
            foreach (string gh in gholdurls)
            {
                if (gh.Length > 40)
                {
                    int index = gh.IndexOf(' ');

                    if (index >= 40)
                    {
                        gholdurls[a] = gh.Trim().Substring(0, index);
                    }
                    else
                    {
                        continue;
                    }

                    try
                    {
                        niceNames.Add(new KeyValuePair<string, string>(gholdurls[a], gh.Substring(index + 1).Trim()));
                    }
                    catch { niceNames.Add(new KeyValuePair<string, string>(gholdurls[a], gholdurls[a])); }

                    a++;
                }
            }
        }

        /// <summary>
        /// Load xml from internet based on firmwareurl, and return softwarelist
        /// </summary>
        /// <returns></returns>
        public List<software> getFWList(string firmwareurl = "")
        {
            if (firmwareurl == "")
                firmwareurl = this.firmwareurl;

            log.Info("getFWList");

            string url = "";
            string url2560 = "";
            string url2560_2 = "";
            string px4 = "";
            string px4v2 = "";
            string vrbrainv40 = "";
            string vrbrainv45 = "";
            string vrbrainv50 = "";
            string vrbrainv51 = "";
            string vrherov10 = "";
            string vrubrainv51 = "";
            string name = "";
            string desc = "";
            int k_format_version = 0;

            softwares.Clear();

            software temp = new software();

            // this is for mono to a ssl server
            //ServicePointManager.CertificatePolicy = new NoCheckCertificatePolicy(); 
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback((sender1, certificate, chain, policyErrors) => { return true; });

            updateProgress(-1, "Getting FW List");

            try
            {
                log.Info("url: " + firmwareurl);
                using (XmlTextReader xmlreader = new XmlTextReader(firmwareurl))
                {
                    while (xmlreader.Read())
                    {
                        xmlreader.MoveToElement();
                        switch (xmlreader.Name)
                        {
                            case "url":
                                url = xmlreader.ReadString();
                                break;
                            case "url2560":
                                url2560 = xmlreader.ReadString();
                                break;
                            case "url2560-2":
                                url2560_2 = xmlreader.ReadString();
                                break;
                            case "urlpx4":
                                px4 = xmlreader.ReadString();
                                break;
                            case "urlpx4v2":
                                px4v2 = xmlreader.ReadString();
                                break;
                            case "urlvrbrainv40":
                                vrbrainv40 = xmlreader.ReadString();
                                break;
                            case "urlvrbrainv45":
                                vrbrainv45 = xmlreader.ReadString();
                                break;
                            case "urlvrbrainv50":
                                vrbrainv50 = xmlreader.ReadString();
                                break;
                            case "urlvrbrainv51":
                                vrbrainv51 = xmlreader.ReadString();
                                break;
                            case "urlvrherov10":
                                vrherov10 = xmlreader.ReadString();
                                break;
                            case "urlvrubrainv51":
                                vrubrainv51 = xmlreader.ReadString();
                                break;
                            case "name":
                                name = xmlreader.ReadString();
                                break;
                            case "format_version":
                                k_format_version = int.Parse(xmlreader.ReadString());
                                break;
                            case "desc":
                                desc = xmlreader.ReadString();
                                break;
                            case "Firmware":
                                if (!url2560_2.Equals("") && !name.Equals("") && !desc.Equals("Please Update"))
                                {
                                    temp.desc = desc.Trim();
                                    temp.name = name;
                                    temp.url = url;
                                    temp.url2560 = url2560;
                                    temp.url2560_2 = url2560_2;
                                    temp.urlpx4v1 = px4;
                                    temp.urlpx4v2 = px4v2;
                                    temp.urlvrbrainv40 = vrbrainv40;
                                    temp.urlvrbrainv45 = vrbrainv45;
                                    temp.urlvrbrainv50 = vrbrainv50;
                                    temp.urlvrbrainv51 = vrbrainv51;
                                    temp.urlvrherov10 = vrherov10;
                                    temp.urlvrubrainv51 = vrubrainv51;
                                    temp.k_format_version = k_format_version;

                                    try
                                    {
                                        try
                                        {
                                            if (!url2560.Contains("github"))
                                            {
                                                //name = 

                                                lock (this)
                                                {
                                                    ingetapmversion++;
                                                }

                                                System.Threading.ThreadPool.QueueUserWorkItem(getAPMVersion, temp);

                                                //if (name != "")
                                                //temp.name = name;
                                            }
                                        }
                                        catch { }
                                    }
                                    catch { } // just in case

                                    softwares.Add(temp);
                                }
                                url = "";
                                url2560 = "";
                                url2560_2 = "";
                                px4 = "";
                                px4v2 = "";
                                vrbrainv40 = "";
                                vrbrainv45 = "";
                                vrbrainv50 = "";
                                vrbrainv51 = "";
                                vrherov10 = "";
                                vrubrainv51 = "";
                                name = "";
                                desc = "";
                                k_format_version = 0;
                                temp = new software();
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                //CustomMessageBox.Show("Failed to get Firmware List : " + ex.Message);
                throw;
            }

            while (ingetapmversion > 0)
                System.Threading.Thread.Sleep(100);

            log.Info("load done");

            updateProgress(-1, "Received List");

            return softwares;
        }

        public static void SaveSoftwares(List<software> list)
        {
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<software>), new Type[] { typeof(software) });

            using (StreamWriter sw = new StreamWriter(Application.StartupPath + Path.DirectorySeparatorChar + "fwversions.xml"))
            {
                writer.Serialize(sw, list);
            }
        }

        public static List<software> LoadSoftwares()
        {
            try
            {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<software>), new Type[] { typeof(software) });

                using (StreamReader sr = new StreamReader(Application.StartupPath + Path.DirectorySeparatorChar + "fwversions.xml"))
                {
                    return (List<software>)reader.Deserialize(sr);
                }
            }
            catch (Exception ex) 
            { 
                log.Error(ex);
            }

            return new List<software>();
        }

        void updateProgress(int percent, string status)
        {
            if (Progress != null)
                Progress(percent, status);
        }

        /// <summary>
        /// Get fw version from firmeware.diydrones.com
        /// </summary>
        /// <param name="fwurl"></param>
        /// <returns></returns>
        void getAPMVersion(object tempin)
        {
            try
            {

                software temp = (software)tempin;

                Uri url = new Uri(new Uri(temp.url2560_2), "git-version.txt");

                log.Info("Get url " + url.ToString());

                updateProgress(-1, "Getting FW Version");

                WebRequest wr = WebRequest.Create(url);
                WebResponse wresp = wr.GetResponse();

                StreamReader sr = new StreamReader(wresp.GetResponseStream());

                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    if (line.Contains("APMVERSION:"))
                    {
                        log.Info(line);

                        // get index
                        var index = softwares.IndexOf(temp);
                        // get item to modify
                        var item = softwares[index];
                        // change name
                        item.name = line.Substring(line.IndexOf(':') + 2);
                        // save back to list
                        softwares[index] = item;

                        return;
                    }
                }

                log.Info("no answer");
            }
            catch (Exception ex) { log.Error(ex); }
            finally
            {
                lock (this)
                {
                    ingetapmversion--;
                }
            }
        }

        /// <summary>
        /// Do full update - get firmware from internet
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="historyhash"></param>
        public bool update(string comport, software temp, string historyhash)
        {
            BoardDetect.boards board = BoardDetect.boards.none;

            try
            {
                updateProgress(-1, "Detecting Board Version");

                board = BoardDetect.DetectBoard(comport);

                if (board == BoardDetect.boards.none)
                {
                    CustomMessageBox.Show("Cant detect your Board version. Please check your cabling");
                    return false;
                }

                int apmformat_version = -1; // fail continue

                if (board != BoardDetect.boards.px4 && board != BoardDetect.boards.px4v2 && board != BoardDetect.boards.vrbrainv40 && board != BoardDetect.boards.vrbrainv45 && board != BoardDetect.boards.vrbrainv50 && board != BoardDetect.boards.vrbrainv51 && board != BoardDetect.boards.vrherov10 && board != BoardDetect.boards.vrubrainv51)
                {
                    try
                    {

                        apmformat_version = BoardDetect.decodeApVar(comport, board);
                    }
                    catch { }

                    if (apmformat_version != -1 && apmformat_version != temp.k_format_version)
                    {
                        if (DialogResult.No == CustomMessageBox.Show("Epprom changed, all your setting will be lost during the update,\nDo you wish to continue?", "Epprom format changed (" + apmformat_version + " vs " + temp.k_format_version + ")", MessageBoxButtons.YesNo))
                        {
                            CustomMessageBox.Show("Please connect and backup your config in the configuration tab.");
                            return false;
                        }
                    }
                }


                log.Info("Detected a " + board);

                updateProgress(-1, "Detected a " + board);

                string baseurl = "";
                if (board == BoardDetect.boards.b2560)
                {
                    baseurl = temp.url2560.ToString();
                }
                else if (board == BoardDetect.boards.b1280)
                {
                    baseurl = temp.url.ToString();
                }
                else if (board == BoardDetect.boards.b2560v2)
                {
                    baseurl = temp.url2560_2.ToString();
                }
                else if (board == BoardDetect.boards.px4)
                {
                    baseurl = temp.urlpx4v1.ToString();
                }
                else if (board == BoardDetect.boards.px4v2)
                {
                    baseurl = temp.urlpx4v2.ToString();
                }
                else if (board == BoardDetect.boards.vrbrainv40)
                {
                    baseurl = temp.urlvrbrainv40.ToString();
                }
                else if (board == BoardDetect.boards.vrbrainv45)
                {
                    baseurl = temp.urlvrbrainv45.ToString();
                }
                else if (board == BoardDetect.boards.vrbrainv50)
                {
                    baseurl = temp.urlvrbrainv50.ToString();
                }
                else if (board == BoardDetect.boards.vrbrainv51)
                {
                    baseurl = temp.urlvrbrainv51.ToString();
                }
                else if (board == BoardDetect.boards.vrherov10)
                {
                    baseurl = temp.urlvrherov10.ToString();
                }
                else if (board == BoardDetect.boards.vrubrainv51)
                {
                    baseurl = temp.urlvrubrainv51.ToString();
                }
                else
                {
                    CustomMessageBox.Show("Invalid Board Type");
                    return false;
                }

                if (historyhash != "")
                    baseurl = getUrl(historyhash, baseurl);

                log.Info("Using " + baseurl);

                // Create a request using a URL that can receive a post. 
                WebRequest request = WebRequest.Create(baseurl);
                request.Timeout = 10000;
                // Set the Method property of the request to POST.
                request.Method = "GET";
                // Get the request stream.
                Stream dataStream; //= request.GetRequestStream();
                // Get the response.
                WebResponse response = request.GetResponse();
                // Display the status.
                log.Info(((HttpWebResponse)response).StatusDescription);
                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();

                long bytes = response.ContentLength;
                long contlen = bytes;

                byte[] buf1 = new byte[1024];

                FileStream fs = new FileStream(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"firmware.hex", FileMode.Create);

                updateProgress(0, "Downloading from Internet");

                dataStream.ReadTimeout = 30000;

                while (dataStream.CanRead)
                {
                    try
                    {
                        updateProgress(50, "Downloading from Internet");
                    }
                    catch { }
                    int len = dataStream.Read(buf1, 0, 1024);
                    if (len == 0)
                        break;
                    bytes -= len;
                    fs.Write(buf1, 0, len);
                }

                fs.Close();
                dataStream.Close();
                response.Close();

                updateProgress(100, "Downloaded from Internet");
                log.Info("Downloaded");
            }
            catch (Exception ex) { updateProgress(50, "Failed download"); CustomMessageBox.Show("Failed to download new firmware : " + ex.ToString()); return false; }

            MissionPlanner.Utilities.Tracking.AddFW(temp.name, board.ToString());

            return UploadFlash(comport, Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"firmware.hex", board);
        }

        void apmtype(object temp)
        {
            try
            {
                // Create a request using a URL that can receive a post. 
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://vps.oborne.me/axs/ax.pl?" + (string)temp);
                //request.AllowAutoRedirect = true;
                request.UserAgent = MainV2.instance.Text + " (res" + Screen.PrimaryScreen.Bounds.Width + "x" + Screen.PrimaryScreen.Bounds.Height + "; " + Environment.OSVersion.VersionString + "; cores " + Environment.ProcessorCount + ")";
                request.Timeout = 10000;
                // Set the Method property of the request to POST.
                request.Method = "GET";
                // Get the request stream.
                // Get the response.
                WebResponse response = request.GetResponse();
            }
            catch { }
        }

        /// <summary>
        /// upload to px4 standalone
        /// </summary>
        /// <param name="filename"></param>
        public bool UploadPX4(string filename)
        {
            Uploader up;
            updateProgress(0, "Reading Hex File");
            px4uploader.Firmware fw;
            try
            {
                fw = px4uploader.Firmware.ProcessFirmware(filename);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error loading firmware file\n\n" + ex.ToString(), "Error");
                return false;
            }

            try
            {
                // check if we are seeing heartbeats
                MainV2.comPort.BaseStream.Open();
                MainV2.comPort.giveComport = true;

                if (MainV2.comPort.getHeartBeat().Length > 0)
                {
                    MainV2.comPort.doReboot(true);
                    MainV2.comPort.Close();
                }
                else
                {
                    MainV2.comPort.BaseStream.Close();
                    CustomMessageBox.Show("Please unplug the board, and then press OK and plug back in.\nMission Planner will look for 30 seconds to find the board");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                CustomMessageBox.Show("Please unplug the board, and then press OK and plug back in.\nMission Planner will look for 30 seconds to find the board");
            }

            DateTime DEADLINE = DateTime.Now.AddSeconds(30);

            updateProgress(0, "Scanning comports");

            while (DateTime.Now < DEADLINE)
            {
                string[] allports = SerialPort.GetPortNames();

                foreach (string port in allports)
                {
                    log.Info(DateTime.Now.Millisecond + " Trying Port " + port);

                    updateProgress(-1, "Connecting");

                    try
                    {
                        up = new Uploader(port, 115200);
                    }
                    catch (Exception ex)
                    {
                        //System.Threading.Thread.Sleep(50);
                        Console.WriteLine(ex.Message);
                        continue;
                    }

                    try
                    {
                        up.identify();
                        updateProgress(-1, "Identify");
                        log.InfoFormat("Found board type {0} boardrev {1} bl rev {2} fwmax {3} on {4}", up.board_type, up.board_rev, up.bl_rev, up.fw_maxsize, port);

                        up.ProgressEvent += new Uploader.ProgressEventHandler(up_ProgressEvent);
                        up.LogEvent += new Uploader.LogEventHandler(up_LogEvent);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Not There..");
                        //Console.WriteLine(ex.Message);
                        up.close();
                        continue;
                    }

                    // test if pausing here stops - System.TimeoutException: The write timed out.
                    System.Threading.Thread.Sleep(500);

                    try
                    {
                        up.verifyotp();
                    }
                    catch (Org.BouncyCastle.Security.InvalidKeyException ex) 
                    {
                        log.Error(ex);
                        CustomMessageBox.Show("You are using unsupported hardware.\nThis board does not contain a valid certificate of authenticity.\nPlease contact your hardware vendor about signing your hardware.", "Invalid Cert"); 
                        up.skipotp = true;
                    }
                    catch (FormatException ex)
                    {
                        log.Error(ex);
                        CustomMessageBox.Show("You are using unsupported hardware.\nThis board does not contain a valid certificate of authenticity.\nPlease contact your hardware vendor about signing your hardware.", "Invalid Cert");
                        up.skipotp = true;
                    }
                    catch (IOException ex) 
                    {
                        log.Error(ex);
                        CustomMessageBox.Show("lost communication with the board.", "lost comms");
                        up.close();
                        return false;
                    }
                    catch (TimeoutException ex)
                    {
                        log.Error(ex);
                        CustomMessageBox.Show("lost communication with the board.", "lost comms");
                        up.close();
                        return false;
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        CustomMessageBox.Show("lost communication with the board. " + ex.ToString(), "lost comms");
                        up.close();
                        return false;
                    }

                    try
                    {
                        up.currentChecksum(fw);
                    }
                    catch (IOException ex)
                    {
                        log.Error(ex);
                        CustomMessageBox.Show("lost communication with the board.", "lost comms");
                        up.close();
                        return false;
                    }
                    catch
                    {
                        up.__reboot();
                        up.close();
                        CustomMessageBox.Show("No need to upload. already on the board");
                        return true;
                    }

                    try
                    {
                        updateProgress(0, "Upload");
                        up.upload(fw);
                        updateProgress(100, "Upload Done");
                    }
                    catch (Exception ex)
                    {
                        updateProgress(0, "ERROR: " + ex.Message);
                        log.Info(ex);
                        Console.WriteLine(ex.ToString());
                        return false;
                    }
                    finally
                    {
                        up.close();
                    }

                    // wait for IO firmware upgrade and boot to a mavlink state
                    CustomMessageBox.Show("Please wait for the musical tones to finish before clicking OK");

                    return true;
                }
            }

            updateProgress(0, "ERROR: No Response from board");
            return false;
        }

        /// <summary>
        /// upload to vrbrain standalone
        /// </summary>
        /// <param name="filename"></param>
        public bool UploadVRBRAIN(string filename)
        {

            px4uploader.Uploader up;
            updateProgress(0, "Reading Hex File");
            px4uploader.Firmware fw;
            try
            {
                fw = px4uploader.Firmware.ProcessFirmware(filename);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error loading firmware file\n\n" + ex.ToString(), "Error");
                return false;
            }

            try
            {
                // check if we are seeing heartbeats
                MainV2.comPort.BaseStream.Open();
                MainV2.comPort.giveComport = true;

                if (MainV2.comPort.getHeartBeat().Length > 0)
                {
                    MainV2.comPort.doReboot(true);
                    MainV2.comPort.Close();
                }
                else
                {
                    MainV2.comPort.BaseStream.Close();
                    CustomMessageBox.Show("Please unplug the board, and then press OK and plug back in.\nMission Planner will look for 30 seconds to find the board");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                CustomMessageBox.Show("Please unplug the board, and then press OK and plug back in.\nMission Planner will look for 30 seconds to find the board");
            }

            DateTime DEADLINE = DateTime.Now.AddSeconds(30);

            updateProgress(0, "Scanning comports");

            while (DateTime.Now < DEADLINE)
            {
                string[] allports = SerialPort.GetPortNames();

                foreach (string port in allports)
                {
                    log.Info(DateTime.Now.Millisecond + " Trying Port " + port);

                    updateProgress(-1, "Connecting");

                    try
                    {
                        up = new px4uploader.Uploader(port, 115200);
                    }
                    catch (Exception ex)
                    {
                        //System.Threading.Thread.Sleep(50);
                        Console.WriteLine(ex.Message);
                        continue;
                    }

                    try
                    {
                        up.identify();
                        updateProgress(-1, "Identify");
                        log.InfoFormat("Found board type {0} boardrev {1} bl rev {2} fwmax {3} on {4}", up.board_type, up.board_rev, up.bl_rev, up.fw_maxsize, port);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Not There..");
                        //Console.WriteLine(ex.Message);
                        up.close();
                        continue;
                    }

                    up.skipotp = true;

                    try
                    {
                        up.currentChecksum(fw);
                    }
                    catch
                    {
                        up.__reboot();
                        up.close();
                        CustomMessageBox.Show("No need to upload. already on the board");
                        return true;
                    }

                    try
                    {
                        up.ProgressEvent += new px4uploader.Uploader.ProgressEventHandler(up_ProgressEvent);
                        up.LogEvent += new px4uploader.Uploader.LogEventHandler(up_LogEvent);

                        updateProgress(0, "Upload");
                        up.upload(fw);
                        updateProgress(100, "Upload Done");
                    }
                    catch (Exception ex)
                    {
                        updateProgress(0, "ERROR: " + ex.Message);
                        log.Info(ex);
                        Console.WriteLine(ex.ToString());
                        return false;
                    }
                    finally
                    {
                        up.close();
                    }

                    // wait for IO firmware upgrade and boot to a mavlink state
                    CustomMessageBox.Show("Please wait for the musical tones to finish before clicking OK");

                    return true;
                }
            }

            updateProgress(0, "ERROR: No Response from board");
            return false;
        }

        string _message = "";

        void up_LogEvent(string message, int level = 0)
        {
            log.Debug(message);

            _message = message;
            updateProgress(-1, message);
        }

        void up_ProgressEvent(double completed)
        {
            updateProgress((int)completed, _message);
        }

        /// <summary>
        /// upload to arduino standalone
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="board"></param>
        public bool UploadFlash(string comport, string filename, BoardDetect.boards board)
        {
            if (board == BoardDetect.boards.px4 || board == BoardDetect.boards.px4v2)
            {
                return UploadPX4(filename);
            }

            if (board == BoardDetect.boards.vrbrainv40 || board == BoardDetect.boards.vrbrainv45 || board == BoardDetect.boards.vrbrainv50 || board == BoardDetect.boards.vrbrainv51 || board == BoardDetect.boards.vrherov10 || board == BoardDetect.boards.vrubrainv51)
            {
                return UploadVRBRAIN(filename);
            }

            byte[] FLASH = new byte[1];
            try
            {
                updateProgress(0, "Reading Hex File");
                using (StreamReader sr = new StreamReader(filename))
                {
                    FLASH = readIntelHEXv2(sr);
                }
                log.InfoFormat("\n\nSize: {0}\n\n", FLASH.Length);
            }
            catch (Exception ex)
            {
                updateProgress(0, "Failed read HEX");
                CustomMessageBox.Show("Failed to read firmware.hex : " + ex.Message);
                return false;
            }
            IArduinoComms port = new ArduinoSTK();

            if (board == BoardDetect.boards.b1280)
            {
                if (FLASH.Length > 126976)
                {
                    CustomMessageBox.Show("Firmware is to big for a 1280, Please upgrade your hardware!!");
                    return false;
                }
                //port = new ArduinoSTK();
                port.BaudRate = 57600;
            }
            else if (board == BoardDetect.boards.b2560 || board == BoardDetect.boards.b2560v2)
            {
                port = new ArduinoSTKv2
                {
                    BaudRate = 115200
                };
            }
            port.DataBits = 8;
            port.StopBits = System.IO.Ports.StopBits.One;
            port.Parity = System.IO.Ports.Parity.None;
            port.DtrEnable = true;

            try
            {
                port.PortName = comport;

                port.Open();

                if (port.connectAP())
                {
                    log.Info("starting");
                    updateProgress(0, "Uploading " + FLASH.Length + " bytes to Board: " + board);

                    // this is enough to make ap_var reset
                    //port.upload(new byte[256], 0, 2, 0);

                    port.Progress += updateProgress;

                    if (!port.uploadflash(FLASH, 0, FLASH.Length, 0))
                    {
                        if (port.IsOpen)
                            port.Close();
                        throw new Exception("Upload failed. Lost sync. Try Arduino!!");
                    }

                    port.Progress -= updateProgress;

                    updateProgress(100, "Upload Complete");

                    log.Info("Uploaded");

                    int start = 0;
                    short length = 0x100;

                    byte[] flashverify = new byte[FLASH.Length + 256];

                    updateProgress(0, "Verify Firmware");

                    while (start < FLASH.Length)
                    {
                        updateProgress((int)((start / (float)FLASH.Length) * 100), "Verify Firmware");
                        port.setaddress(start);
                        //log.Info("Downloading " + length + " at " + start);
                        port.downloadflash(length).CopyTo(flashverify, start);
                        start += length;
                    }

                    for (int s = 0; s < FLASH.Length; s++)
                    {
                        if (FLASH[s] != flashverify[s])
                        {
                            CustomMessageBox.Show("Upload succeeded, but verify failed: exp " + FLASH[s].ToString("X") + " got " + flashverify[s].ToString("X") + " at " + s);
                            port.Close();
                            return false;
                        }
                    }

                    updateProgress(100, "Verify Complete");
                }
                else
                {
                    updateProgress(0, "Failed upload");
                    CustomMessageBox.Show("Communication Error - no connection");
                }
                port.Close();

                try
                {
                    ((SerialPort)port).Open();
                }
                catch { }

                //CustomMessageBox.Show("1. If you are updating your firmware from a previous version, please verify your parameters are appropriate for the new version.\n2. Please ensure your accelerometer is calibrated after installing or re-calibrated after updating the firmware.");

                try
                {
                    ((SerialPort)port).Close();
                }
                catch { }

                updateProgress(100, "Done");
            }
            catch (Exception ex)
            {
                updateProgress(0, "Failed upload");
                CustomMessageBox.Show("Check port settings or Port in use? " + ex);
                try
                {
                    port.Close();
                }
                catch { }
                return false;
            }
            MainV2.comPort.giveComport = false;
            return true;
        }

        /// <summary>
        /// Read intel hex file
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        byte[] readIntelHEXv2(StreamReader sr)
        {
            byte[] FLASH = new byte[1024 * 1024];

            int optionoffset = 0;
            int total = 0;
            bool hitend = false;

            while (!sr.EndOfStream)
            {
                updateProgress((int)(((float)sr.BaseStream.Position / (float)sr.BaseStream.Length) * 100), "Reading Hex");

                string line = sr.ReadLine();

                if (line.StartsWith(":"))
                {
                    int length = Convert.ToInt32(line.Substring(1, 2), 16);
                    int address = Convert.ToInt32(line.Substring(3, 4), 16);
                    int option = Convert.ToInt32(line.Substring(7, 2), 16);
                    // log.InfoFormat("len {0} add {1} opt {2}", length, address, option);

                    if (option == 0)
                    {
                        string data = line.Substring(9, length * 2);
                        for (int i = 0; i < length; i++)
                        {
                            byte byte1 = Convert.ToByte(data.Substring(i * 2, 2), 16);
                            FLASH[optionoffset + address] = byte1;
                            address++;
                            if ((optionoffset + address) > total)
                                total = optionoffset + address;
                        }
                    }
                    else if (option == 2)
                    {
                        optionoffset = (int)Convert.ToUInt16(line.Substring(9, 4), 16) << 4;
                    }
                    else if (option == 1)
                    {
                        hitend = true;
                    }
                    int checksum = Convert.ToInt32(line.Substring(line.Length - 2, 2), 16);

                    byte checksumact = 0;
                    for (int z = 0; z < ((line.Length - 1 - 2) / 2); z++) // minus 1 for : then mins 2 for checksum itself
                    {
                        checksumact += Convert.ToByte(line.Substring(z * 2 + 1, 2), 16);
                    }
                    checksumact = (byte)(0x100 - checksumact);

                    if (checksumact != checksum)
                    {
                        CustomMessageBox.Show("The hex file loaded is invalid, please try again.");
                        throw new Exception("Checksum Failed - Invalid Hex");
                    }
                }
                //Regex regex = new Regex(@"^:(..)(....)(..)(.*)(..)$"); // length - address - option - data - checksum
            }

            if (!hitend)
            {
                CustomMessageBox.Show("The hex file did no contain an end flag. aborting");
                throw new Exception("No end flag in file");
            }

            Array.Resize<byte>(ref FLASH, total);

            return FLASH;
        }
    }
}