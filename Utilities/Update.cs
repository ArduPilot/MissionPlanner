using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using log4net;

namespace MissionPlanner.Utilities
{
    class Update
    {
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static bool MONO = false;
        public static bool dobeta = false;

        public static void updateCheckMain(ProgressReporterDialogue frmProgressReporter)
        {
            var t = Type.GetType("Mono.Runtime");
            MONO = (t != null);

            try
            {
                if (dobeta)
                {
                    CheckMD5(frmProgressReporter, ConfigurationManager.AppSettings["BetaUpdateLocationMD5"].ToString());
                }
                else
                {
                    CheckMD5(frmProgressReporter, ConfigurationManager.AppSettings["UpdateLocationMD5"].ToString());
                }

                var process = new Process();
                string exePath = Path.GetDirectoryName(Application.ExecutablePath);
                if (MONO)
                {
                    process.StartInfo.FileName = "mono";
                    process.StartInfo.Arguments = " \"" + exePath + Path.DirectorySeparatorChar + "Updater.exe\"" +
                                                  "  \"" + Application.ExecutablePath + "\"";
                }
                else
                {
                    process.StartInfo.FileName = exePath + Path.DirectorySeparatorChar + "Updater.exe";
                    process.StartInfo.Arguments = Application.ExecutablePath;
                }

                try
                {
                    foreach (string newupdater in Directory.GetFiles(exePath, "Updater.exe*.new"))
                    {
                        File.Copy(newupdater, newupdater.Remove(newupdater.Length - 4), true);
                        File.Delete(newupdater);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Exception during update", ex);
                }
                if (frmProgressReporter != null)
                    frmProgressReporter.UpdateProgressAndStatus(-1, "Starting Updater");
                log.Info("Starting new process: " + process.StartInfo.FileName + " with " + process.StartInfo.Arguments);
                process.Start();
                log.Info("Quitting existing process");

                frmProgressReporter.BeginInvoke((Action) delegate { Application.Exit(); });
            }
            catch (Exception ex)
            {
                log.Error("Update Failed", ex);
                CustomMessageBox.Show("Update Failed " + ex.Message);
            }
        }

        public static void CheckForUpdate(bool NotifyNoUpdate = false)
        {
            var baseurl = ConfigurationManager.AppSettings["UpdateLocationVersion"];

            if (dobeta)
                baseurl = ConfigurationManager.AppSettings["BetaUpdateLocationVersion"];

            if (baseurl == "")
                return;

            string path = Path.GetDirectoryName(Application.ExecutablePath);

            path = path + Path.DirectorySeparatorChar + "version.txt";

            ServicePointManager.ServerCertificateValidationCallback =
                new System.Net.Security.RemoteCertificateValidationCallback(
                    (sender, certificate, chain, policyErrors) => { return true; });

            log.Debug(path);

            // Create a request using a URL that can receive a post. 
            string requestUriString = baseurl + Path.GetFileName(path);
            log.Info("Checking for update at: " + requestUriString);
            var webRequest = WebRequest.Create(requestUriString);
            webRequest.Timeout = 5000;

            // Set the Method property of the request to POST.
            webRequest.Method = "GET";

            // ((HttpWebRequest)webRequest).IfModifiedSince = File.GetLastWriteTimeUtc(path);

            bool updateFound = false;

            // Get the response.
            using (var response = webRequest.GetResponse())
            {
                // Display the status.
                log.Debug("Response status: " + ((HttpWebResponse) response).StatusDescription);
                // Get the stream containing content returned by the server.

                if (File.Exists(path))
                {
                    var fi = new FileInfo(path);

                    Version LocalVersion = new Version();
                    Version WebVersion = new Version();

                    if (File.Exists(path))
                    {
                        using (Stream fs = File.OpenRead(path))
                        {
                            using (StreamReader sr = new StreamReader(fs))
                            {
                                LocalVersion = new Version(sr.ReadLine());
                            }
                        }
                    }

                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        WebVersion = new Version(sr.ReadLine());
                    }

                    log.Info("New file Check: local " + LocalVersion + " vs Remote " + WebVersion);

                    if (LocalVersion < WebVersion)
                    {
                        updateFound = true;
                    }
                }
                else
                {
                    updateFound = true;
                    log.Info("File does not exist: Getting " + path);
                    // get it
                }
            }

            if (updateFound)
            {
                // do the update in the main thread
                MainV2.instance.Invoke((MethodInvoker) delegate
                {
                    string extra = "";

                    if (dobeta)
                        extra = "BETA ";

                    DialogResult dr = DialogResult.Cancel;


                    dr = CustomMessageBox.Show(extra + Strings.UpdateFound + baseurl + "/ChangeLog.txt;ChangeLog]",
                        Strings.UpdateNow, MessageBoxButtons.YesNo);

                    if (dr == DialogResult.Yes)
                    {
                        DoUpdate();
                    }
                    else
                    {
                        return;
                    }
                });
            }
            else if (NotifyNoUpdate)
            {
                CustomMessageBox.Show(Strings.UpdateNotFound);
            }
        }

        public static void DoUpdate()
        {
            ProgressReporterDialogue frmProgressReporter = new ProgressReporterDialogue()
            {
                Text = "Check for Updates",
                StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            };

            ThemeManager.ApplyThemeTo(frmProgressReporter);

            frmProgressReporter.DoWork += new ProgressReporterDialogue.DoWorkEventHandler(DoUpdateWorker_DoWork);

            frmProgressReporter.UpdateProgressAndStatus(-1, "Checking for Updates");

            frmProgressReporter.RunBackgroundOperationAsync();

            frmProgressReporter.Dispose();
        }

        static void CheckMD5(ProgressReporterDialogue frmProgressReporter, string url)
        {
            var baseurl = ConfigurationManager.AppSettings["UpdateLocation"];

            if (dobeta)
            {
                baseurl = ConfigurationManager.AppSettings["BetaUpdateLocation"];
            }

            L10N.ReplaceMirrorUrl(ref baseurl);

            WebRequest request = WebRequest.Create(url);
            request.Timeout = 10000;
            // Set the Method property of the request to POST.
            request.Method = "GET";
            // Get the request stream.
            Stream dataStream; //= request.GetRequestStream();
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            log.Info(((HttpWebResponse) response).StatusDescription);
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();

            Regex regex = new Regex(@"([^\s]+)\s+upgrade/(.*)", RegexOptions.IgnoreCase);

            if (regex.IsMatch(responseFromServer))
            {
                MatchCollection matchs = regex.Matches(responseFromServer);
                for (int i = 0; i < matchs.Count; i++)
                {
                    string hash = matchs[i].Groups[1].Value.ToString();
                    string file = matchs[i].Groups[2].Value.ToString();

                    if (file.ToLower().EndsWith(".etag"))
                    {
                        try
                        {
                            // remove all etags
                            File.Delete(file);
                        }
                        catch
                        {
                        }
                        continue;
                    }

                    // check if existing matchs hash
                    if (!MD5File(file, hash))
                    {
                        log.Info("Newer File " + file);

                        // check is we have already downloaded and matchs hash
                        if (!MD5File(file + ".new", hash))
                        {
                            if (frmProgressReporter != null)
                                frmProgressReporter.UpdateProgressAndStatus(-1, Strings.Getting + file);

                            string subdir = Path.GetDirectoryName(file) + Path.DirectorySeparatorChar;

                            GetNewFile(frmProgressReporter, baseurl + subdir.Replace('\\', '/'), subdir,
                                Path.GetFileName(file));

                            // check the new downloaded file matchs hash
                            if (!MD5File(file + ".new", hash))
                            {
                                throw new Exception("File downloaded does not match hash: " + file);
                            }
                        }
                        else
                        {
                            log.Info("already got new File " + file);
                        }
                    }
                    else
                    {
                        log.Info("Same File " + file);

                        if (frmProgressReporter != null)
                            frmProgressReporter.UpdateProgressAndStatus(-1, Strings.Checking + file);
                    }
                }
            }
        }

        static bool MD5File(string filename, string hash)
        {
            try
            {
                using (var md5 = MD5.Create())
                {
                    if (!File.Exists(filename))
                        return false;

                    using (var stream = File.OpenRead(filename))
                    {
                        return hash == BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Info("md5 fail " + ex.ToString());
            }

            return false;
        }

        static void GetNewFile(ProgressReporterDialogue frmProgressReporter, string baseurl, string subdir, string file)
        {
            // create dest dir
            string dir = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + subdir;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            // get dest path
            string path = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + subdir +
                          file;

            Exception fail = null;
            int attempt = 0;

            // attempt to get file
            while (attempt < 2)
            {
                // check if user canceled
                if (frmProgressReporter.doWorkArgs.CancelRequested)
                {
                    frmProgressReporter.doWorkArgs.CancelAcknowledged = true;
                    throw new Exception("Cancel");
                }

                try
                {
                    string url = baseurl + file + "?" + new Random().Next();
                    // Create a request using a URL that can receive a post. 
                    WebRequest request = WebRequest.Create(url);
                    log.Info("GetNewFile " + url);
                    // Set the Method property of the request to GET.
                    request.Method = "GET";
                    // Allow compressed content
                    ((HttpWebRequest) request).AutomaticDecompression = DecompressionMethods.GZip |
                                                                        DecompressionMethods.Deflate;
                    // tell server we allow compress content
                    request.Headers.Add("Accept-Encoding", "gzip,deflate");
                    // Get the response.
                    using (WebResponse response = request.GetResponse())
                    {
                        // Display the status.
                        log.Info(((HttpWebResponse) response).StatusDescription);
                        // Get the stream containing content returned by the server.
                        Stream dataStream = response.GetResponseStream();

                        // update status
                        if (frmProgressReporter != null)
                            frmProgressReporter.UpdateProgressAndStatus(-1, Strings.Getting + file);

                        // from head
                        long bytes = response.ContentLength;

                        long contlen = bytes;

                        byte[] buf1 = new byte[4096];

                        // if the file doesnt exist. just save it inplace
                        string fn = path + ".new";

                        using (FileStream fs = new FileStream(fn, FileMode.Create))
                        {
                            DateTime dt = DateTime.Now;

                            while (dataStream.CanRead)
                            {
                                try
                                {
                                    if (dt.Second != DateTime.Now.Second)
                                    {
                                        if (frmProgressReporter != null)
                                            frmProgressReporter.UpdateProgressAndStatus(
                                                (int) (((double) (contlen - bytes)/(double) contlen)*100),
                                                Strings.Getting + file + ": " +
                                                (((double) (contlen - bytes)/(double) contlen)*100).ToString("0.0") +
                                                "%"); //+ Math.Abs(bytes) + " bytes");
                                        dt = DateTime.Now;
                                    }
                                }
                                catch
                                {
                                }
                                log.Debug(file + " " + bytes);
                                int len = dataStream.Read(buf1, 0, buf1.Length);
                                if (len == 0)
                                    break;
                                bytes -= len;
                                fs.Write(buf1, 0, len);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    fail = ex;
                    attempt++;
                    continue;
                }

                // break if we have no exception
                break;
            }

            if (attempt == 2)
            {
                throw fail;
            }
        }

        static void DoUpdateWorker_DoWork(object sender, ProgressWorkerEventArgs e, object passdata = null)
        {
            // TODO: Is this the right place?

            #region Fetch Parameter Meta Data

            var progressReporterDialogue = ((ProgressReporterDialogue) sender);
            progressReporterDialogue.UpdateProgressAndStatus(-1, "Getting Updated Parameters");

            try
            {
                ParameterMetaDataParser.GetParameterInformation();
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
                CustomMessageBox.Show("Error getting Parameter Information");
            }

            #endregion Fetch Parameter Meta Data

            progressReporterDialogue.UpdateProgressAndStatus(-1, "Getting Base URL");
            // check for updates
            //  if (Debugger.IsAttached)
            {
                //      log.Info("Skipping update test as it appears we are debugging");
            }
            //  else
            {
                updateCheckMain(progressReporterDialogue);
            }
        }

        private static bool updateCheck(ProgressReporterDialogue frmProgressReporter, string baseurl, string subdir)
        {
            bool update = false;
            List<string> files = new List<string>();

            // Create a request using a URL that can receive a post. 
            log.Info(baseurl);
            WebRequest request = WebRequest.Create(baseurl);
            request.Timeout = 10000;
            // Set the Method property of the request to POST.
            request.Method = "GET";
            // Get the request stream.
            Stream dataStream; //= request.GetRequestStream();
            // Get the response.
            using (WebResponse response = request.GetResponse())
            {
                // Display the status.
                log.Info(((HttpWebResponse) response).StatusDescription);
                // Get the stream containing content returned by the server.
                using (dataStream = response.GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.
                    using (StreamReader reader = new StreamReader(dataStream))
                    {
                        // Read the content.
                        string responseFromServer = reader.ReadToEnd();
                        // Display the content.
                        Regex regex = new Regex("href=\"([^\"]+)\"", RegexOptions.IgnoreCase);

                        Uri baseuri = new Uri(baseurl, UriKind.Absolute);

                        if (regex.IsMatch(responseFromServer))
                        {
                            MatchCollection matchs = regex.Matches(responseFromServer);
                            for (int i = 0; i < matchs.Count; i++)
                            {
                                if (matchs[i].Groups[1].Value.ToString().Contains(".."))
                                    continue;
                                if (matchs[i].Groups[1].Value.ToString().Contains("http"))
                                    continue;
                                if (matchs[i].Groups[1].Value.ToString().StartsWith("?"))
                                    continue;
                                if (matchs[i].Groups[1].Value.ToString().ToLower().Contains(".etag"))
                                    continue;

                                //                     
                                {
                                    string url = System.Web.HttpUtility.UrlDecode(matchs[i].Groups[1].Value.ToString());
                                    Uri newuri = new Uri(baseuri, url);
                                    files.Add(baseuri.MakeRelativeUri(newuri).ToString());
                                }


                                // dirs
                                if (matchs[i].Groups[1].Value.ToString().Contains("tree/master/"))
                                {
                                    string url =
                                        System.Web.HttpUtility.UrlDecode(matchs[i].Groups[1].Value.ToString()) + "/";
                                    Uri newuri = new Uri(baseuri, url);
                                    files.Add(baseuri.MakeRelativeUri(newuri).ToString());
                                }
                                // files
                                if (matchs[i].Groups[1].Value.ToString().Contains("blob/master/"))
                                {
                                    string url = System.Web.HttpUtility.UrlDecode(matchs[i].Groups[1].Value.ToString());
                                    Uri newuri = new Uri(baseuri, url);
                                    files.Add(
                                        System.Web.HttpUtility.UrlDecode(newuri.Segments[newuri.Segments.Length - 1]));
                                }
                            }
                        }

                        //Console.WriteLine(responseFromServer);
                        // Clean up the streams.
                    }
                }
            }

            string dir = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + subdir;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            foreach (string file in files)
            {
                if (frmProgressReporter.doWorkArgs.CancelRequested)
                {
                    frmProgressReporter.doWorkArgs.CancelAcknowledged = true;
                    throw new Exception("Cancel");
                }


                if (file.Equals("/") || file.Equals("") || file.StartsWith("../"))
                {
                    continue;
                }
                if (file.EndsWith("/"))
                {
                    update =
                        updateCheck(frmProgressReporter, baseurl + file,
                            subdir.Replace('/', Path.DirectorySeparatorChar) + file) && update;
                    continue;
                }
                if (frmProgressReporter != null)
                    frmProgressReporter.UpdateProgressAndStatus(-1, "Checking " + file);

                string path = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + subdir +
                              file;

                //   baseurl = baseurl.Replace("//github.com", "//raw.github.com");
                //   baseurl = baseurl.Replace("/tree/", "/");

                Exception fail = null;
                int attempt = 0;

                while (attempt < 2)
                {
                    try
                    {
                        // Create a request using a URL that can receive a post. 
                        request = WebRequest.Create(baseurl + file);
                        log.Info(baseurl + file + " ");
                        // Set the Method property of the request to POST.
                        request.Method = "GET";

                        ((HttpWebRequest) request).AutomaticDecompression = DecompressionMethods.GZip |
                                                                            DecompressionMethods.Deflate;

                        request.Headers.Add("Accept-Encoding", "gzip,deflate");

                        // Get the response.
                        using (WebResponse response = request.GetResponse())
                        {
                            // Display the status.
                            log.Info(((HttpWebResponse) response).StatusDescription);
                            // Get the stream containing content returned by the server.
                            using (dataStream = response.GetResponseStream())
                            {
                                // Open the stream using a StreamReader for easy access.

                                bool updateThisFile = false;

                                if (File.Exists(path))
                                {
                                    FileInfo fi = new FileInfo(path);

                                    //log.Info(response.Headers[HttpResponseHeader.ETag]);
                                    string CurrentEtag = "";

                                    if (File.Exists(path + ".etag"))
                                    {
                                        using (Stream fs = File.OpenRead(path + ".etag"))
                                        {
                                            using (StreamReader sr = new StreamReader(fs))
                                            {
                                                CurrentEtag = sr.ReadLine();
                                            }
                                        }
                                    }

                                    log.Debug("New file Check: " + fi.Length + " vs " + response.ContentLength + " " +
                                              response.Headers[HttpResponseHeader.ETag] + " vs " + CurrentEtag);

                                    if (fi.Length != response.ContentLength ||
                                        response.Headers[HttpResponseHeader.ETag] != CurrentEtag)
                                    {
                                        using (StreamWriter sw = new StreamWriter(path + ".etag.new"))
                                        {
                                            sw.WriteLine(response.Headers[HttpResponseHeader.ETag]);
                                        }
                                        updateThisFile = true;
                                        log.Info("NEW FILE " + file);
                                    }
                                }
                                else
                                {
                                    updateThisFile = true;
                                    log.Info("NEW FILE " + file);
                                    using (StreamWriter sw = new StreamWriter(path + ".etag.new"))
                                    {
                                        sw.WriteLine(response.Headers[HttpResponseHeader.ETag]);
                                    }
                                    // get it
                                }

                                if (updateThisFile)
                                {
                                    if (!update)
                                    {
                                        //DialogResult dr = MessageBox.Show("Update Found\n\nDo you wish to update now?", "Update Now", MessageBoxButtons.YesNo);
                                        //if (dr == DialogResult.Yes)
                                        {
                                            update = true;
                                        }
                                        //else
                                        {
                                            //    return;
                                        }
                                    }
                                    if (frmProgressReporter != null)
                                        frmProgressReporter.UpdateProgressAndStatus(-1, "Getting " + file);

                                    // from head
                                    long bytes = response.ContentLength;

                                    long contlen = bytes;

                                    byte[] buf1 = new byte[4096];

                                    using (FileStream fs = new FileStream(path + ".new", FileMode.Create))
                                    {
                                        DateTime dt = DateTime.Now;

                                        //dataStream.ReadTimeout = 30000;

                                        while (dataStream.CanRead)
                                        {
                                            try
                                            {
                                                if (dt.Second != DateTime.Now.Second)
                                                {
                                                    if (frmProgressReporter != null)
                                                        frmProgressReporter.UpdateProgressAndStatus(
                                                            (int) (((double) (contlen - bytes)/(double) contlen)*100),
                                                            "Getting " + file + ": " +
                                                            (((double) (contlen - bytes)/(double) contlen)*100).ToString
                                                                ("0.0") + "%"); //+ Math.Abs(bytes) + " bytes");
                                                    dt = DateTime.Now;
                                                }
                                            }
                                            catch
                                            {
                                            }
                                            log.Debug(file + " " + bytes);
                                            int len = dataStream.Read(buf1, 0, buf1.Length);
                                            if (len == 0)
                                                break;
                                            bytes -= len;
                                            fs.Write(buf1, 0, len);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        fail = ex;
                        attempt++;
                        update = false;
                        continue;
                    }

                    // break if we have no exception
                    break;
                }

                if (attempt == 2)
                {
                    throw fail;
                }
            }


            //P.StartInfo.CreateNoWindow = true;
            //P.StartInfo.RedirectStandardOutput = true;
            return update;
        }
    }
}