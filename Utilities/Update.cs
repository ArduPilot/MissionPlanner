using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using MissionPlanner.Controls;
using log4net;

namespace MissionPlanner.Utilities
{
    class Update
    {
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static bool MONO = false;
        public static bool dobeta = false;
        public static bool domaster = false;

        public static void updateCheckMain(IProgressReporterDialogue frmProgressReporter)
        {
            var t = Type.GetType("Mono.Runtime");
            MONO = (t != null);

            try
            {
                if (domaster)
                {
                    CheckMD5(frmProgressReporter,
                        ConfigurationManager.AppSettings["MasterUpdateLocationMD5"].ToString(),
                        ConfigurationManager.AppSettings["MasterUpdateLocationZip"]);
                }
                else if (dobeta)
                {
                    CheckMD5(frmProgressReporter, 
                        ConfigurationManager.AppSettings["BetaUpdateLocationMD5"].ToString(),
                        ConfigurationManager.AppSettings["BetaUpdateLocationZip"]);
                }
                else
                {
                    CheckMD5(frmProgressReporter, 
                        ConfigurationManager.AppSettings["UpdateLocationMD5"].ToString(),
                        ConfigurationManager.AppSettings["UpdateLocation"]);
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
                log.Info("Starting new process: " + process.StartInfo.FileName + " with " +
                         process.StartInfo.Arguments);
                process.Start();
                log.Info("Quitting existing process");

                if (frmProgressReporter != null)
                    frmProgressReporter.BeginInvoke((Action) delegate { Application.Exit(); });
            }
            catch (AggregateException ex)
            {
                log.Error("Update Failed", ex.InnerException);
                CustomMessageBox.Show("Update Failed " + ex.InnerException?.Message);
            }
            catch (Exception ex)
            {
                log.Error("Update Failed", ex);
                CustomMessageBox.Show("Update Failed " + ex.Message);
            }
        }

        public static void CheckForUpdate(bool NotifyNoUpdate = false)
        {
            if (Program.WindowsStoreApp)
            {
                return;
            }

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
            string requestUriString = baseurl;

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

                    var dr = CustomMessageBox.Show(
                        extra + Strings.UpdateFound + " [link;" + baseurl.Replace("version.txt", "ChangeLog.txt") + ";ChangeLog]",
                        Strings.UpdateNow, MessageBoxButtons.YesNo);

                    if (dr == (int)DialogResult.Yes)
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
            if (Program.WindowsStoreApp)
            {
                CustomMessageBox.Show(Strings.Not_available_when_used_as_a_windows_store_app);
                return;
            }

            IProgressReporterDialogue frmProgressReporter = new ProgressReporterDialogue()
            {
                Text = "Check for Updates",
                StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            };

            ThemeManager.ApplyThemeTo(frmProgressReporter);

            frmProgressReporter.DoWork += new DoWorkEventHandler(DoUpdateWorker_DoWork);

            frmProgressReporter.UpdateProgressAndStatus(-1, "Checking for Updates");

            frmProgressReporter.RunBackgroundOperationAsync();

            frmProgressReporter.Dispose();
        }

        static void CheckMD5(IProgressReporterDialogue frmProgressReporter, string md5url, string baseurl)
        {
            log.InfoFormat("get checksums {0} - base {1}", md5url, baseurl);

            string responseFromServer = "";

            WebRequest request = WebRequest.Create(md5url);
            request.Timeout = 10000;
            // Set the Method property of the request to POST.
            request.Method = "GET";
            // Get the response.
            // Get the stream containing content returned by the server.
            // Open the stream using a StreamReader for easy access.
            using (WebResponse response = request.GetResponse())
            using (Stream dataStream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(dataStream))
            {
                // Display the status.
                log.Info(((HttpWebResponse) response).StatusDescription);
                // Read the content.
                responseFromServer = reader.ReadToEnd();
            }

            Regex regex = new Regex(@"([^\s]+)\s+[^/]+/(.*)", RegexOptions.IgnoreCase);

            if (regex.IsMatch(responseFromServer))
            {
                if (frmProgressReporter != null)
                    frmProgressReporter.UpdateProgressAndStatus(-1,"Hashing Files");

                // cleanup dll's with the same exe name
                var dlls = Directory.GetFiles(Settings.GetRunningDirectory(), "*.dll", SearchOption.TopDirectoryOnly);
                var exes = Directory.GetFiles(Settings.GetRunningDirectory(), "*.exe", SearchOption.TopDirectoryOnly);
                List<string> files = new List<string>();

                // hash everything
                MatchCollection matchs = regex.Matches(responseFromServer);
                for (int i = 0; i < matchs.Count; i++)
                {
                    string hash = matchs[i].Groups[1].Value.ToString();
                    string file = matchs[i].Groups[2].Value.ToString();

                    files.Add(file);
                }

                // cleanup unused dlls and exes
                dlls.ForEach(dll =>
                {
                    try
                    {
                        var result = files.Any(task => Path.Combine(Settings.GetRunningDirectory(), task).ToLower().Equals(dll.ToLower()));

                        if (result == false)
                            File.Delete(dll);
                    }
                    catch { }
                });

                exes.ForEach(exe =>
                {
                    try
                    {
                        var result = files.Any(task => Path.Combine(Settings.GetRunningDirectory(), task).ToLower().Equals(exe.ToLower()));

                        if (result == false)
                            File.Delete(exe);
                    }
                    catch { }
                });

                // background md5
                List<Tuple<string, string, Task<bool>>> tasklist = new List<Tuple<string, string, Task<bool>>>();

                for (int i = 0; i < matchs.Count; i++)
                {
                    string hash = matchs[i].Groups[1].Value.ToString();
                    string file = matchs[i].Groups[2].Value.ToString();

                    Task<bool> ismatch = Task<bool>.Factory.StartNew(() => MD5File(file, hash));

                    tasklist.Add(new Tuple<string, string, Task<bool>>(file, hash, ismatch));
                }
                // get count and wait for all hashing to be done
                int count = tasklist.Count(a =>
                {
                    a.Item3.Wait();
                    return !a.Item3.Result;
                });

                // parallel download
                ParallelOptions opt = new ParallelOptions() { MaxDegreeOfParallelism = 3 };

                int done = 0;

                Parallel.ForEach(tasklist, opt, task =>
                    //foreach (var task in tasklist)
                {
                    string file = task.Item1;
                    string hash = task.Item2;
                    // check if existing matchs hash
                    task.Item3.Wait();
                    bool match = task.Item3.Result;

                    if (!match)
                    {
                        done++;
                        log.Info("Newer File " + file);

                        if (frmProgressReporter != null && frmProgressReporter.doWorkArgs.CancelRequested)
                        {
                            frmProgressReporter.doWorkArgs.CancelAcknowledged = true;
                            throw new Exception("User Request");
                        }

                        // check is we have already downloaded and matchs hash
                        if (!MD5File(file + ".new", hash))
                        {
                            if (frmProgressReporter != null)
                                frmProgressReporter.UpdateProgressAndStatus((int)((done/(double)count)*100),
                                    Strings.Getting + file + "\n" + done + " of " + count + " of total " +
                                    tasklist.Count);

                            string subdir = Path.GetDirectoryName(file) + Path.DirectorySeparatorChar;

                            subdir = subdir.Replace("" + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar,
                                "" + Path.DirectorySeparatorChar);

                            if (baseurl.ToLower().Contains(".zip"))
                            {
                                GetNewFileZip(frmProgressReporter, baseurl, subdir,
                                    Path.GetFileName(file));
                            }
                            else
                            {
                                GetNewFile(frmProgressReporter, baseurl + subdir.Replace('\\', '/'), subdir,
                                    Path.GetFileName(file));
                            }

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
                });
            }
        }

        static bool MD5File(string filename, string hash)
        {
            try
            {
                if (!File.Exists(filename))
                    return false;

                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(filename))
                    {
                        var answer = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();

                        log.Debug(filename + "," + hash + "," + answer);

                        return hash == answer;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Info("md5 fail " + ex.ToString());
            }

            return false;
        }

        static void GetNewFileZip(IProgressReporterDialogue frmProgressReporter, string baseurl, string subdir, string file)
        {          
            // create dest dir
            string dir = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + subdir;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            // get dest path
            string path = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + subdir +
                          file;

            DownloadStream ds = new DownloadStream(baseurl);

            ZipArchive zip = new ZipArchive(ds);

            var entry = zip.GetEntry((subdir.TrimStart('\\').Replace('\\', '/') + file));

            if (entry == null)
            {
                Console.WriteLine("{0} {1}", file, baseurl);
                return;
            }

            log.InfoFormat("unzip {0}", file);

            //entry.ExtractToFile(path + ".new", true);

            using (var fo = File.Open(path + ".new", FileMode.Create))
                entry.Open().CopyTo(fo);

            zip.Dispose();

            ds.Dispose();
        }

        static void GetNewFile(IProgressReporterDialogue frmProgressReporter, string baseurl, string subdir, string file)
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

                        // from head
                        long bytes = response.ContentLength;

                        long contlen = bytes;

                        byte[] buf1 = new byte[4096];

                        // if the file doesnt exist. just save it inplace
                        string fn = path + ".new";

                        using (FileStream fs = new FileStream(fn, FileMode.Create))
                        {
                            DateTime dt = DateTime.Now;

                            log.Debug("ContentLength: " + file + " " + bytes);

                            while (dataStream.CanRead)
                            {
                                try
                                {
                                    if (dt.Second != DateTime.Now.Second)
                                    {
                                        if (frmProgressReporter != null)
                                            frmProgressReporter.UpdateProgressAndStatus(
                                                (int) (((double) (contlen - bytes) / (double) contlen) * 100),
                                                Strings.Getting + file + ": " +
                                                (((double) (contlen - bytes) / (double) contlen) * 100)
                                                .ToString("0.0") +
                                                "%"); //+ Math.Abs(bytes) + " bytes");
                                        dt = DateTime.Now;
                                    }
                                }
                                catch
                                {
                                }

                                int len = dataStream.Read(buf1, 0, buf1.Length);
                                if (len == 0)
                                {
                                    log.Debug("GetNewFile: 0 byte read " + file);
                                    break;
                                }
                                bytes -= len;
                                fs.Write(buf1, 0, len);
                            }

                            log.Info("GetNewFile: " + file + " Done with length: " + fs.Length);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
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

        static void DoUpdateWorker_DoWork(IProgressReporterDialogue sender)
        {
            // TODO: Is this the right place?

            #region Fetch Parameter Meta Data

            var progressReporterDialogue = ((IProgressReporterDialogue) sender);
            progressReporterDialogue.UpdateProgressAndStatus(-1, "Getting updated parameter documentation");

            try
            {
                var dns = Dns.GetHostAddresses("github.com");
                var dns2 = Dns.GetHostAddresses("raw.githubusercontent.com");

                if (dns.Length != 0)
                {
                    if (MissionPlanner.Utilities.Update.dobeta)
                        ParameterMetaDataParser.GetParameterInformation(
                            ConfigurationManager.AppSettings["ParameterLocationsBleeding"], "ParameterMetaData.xml");
                    else
                        ParameterMetaDataParser.GetParameterInformation(
                            ConfigurationManager.AppSettings["ParameterLocations"], "ParameterMetaData.xml");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
                CustomMessageBox.Show("Error getting Parameter Information");
            }

            #endregion Fetch Parameter Meta Data

            progressReporterDialogue.UpdateProgressAndStatus(-1, "Getting Base URL");

            try
            {
                File.WriteAllText(
                    Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + "writetest.txt",
                    "this is a test");
            }
            catch (Exception ex)
            {
                log.Info("Write test failed");
                throw new Exception("Unable to write to the install directory", ex);
            }
            finally
            {
                try
                {
                    File.Delete(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar +
                                "writetest.txt");
                }
                catch
                {
                    log.Info("Write test cleanup failed");
                }
            }

            updateCheckMain(progressReporterDialogue);
        }
    }
}