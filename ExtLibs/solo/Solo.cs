using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Util;
using Renci.SshNet;

namespace solo
{
    public class Solo
    {
        public static string soloip = "10.1.1.10";
        public static string controllerip = "10.1.1.1";
        public static string username = "root";
        public static string password = "TjSDBkAu";

        public static bool is_solo_alive
        {
            get { return Ping("10.1.1.10"); }
        }

        public static bool is_controller_alive
        {
            get { return Ping("10.1.1.1"); }
        }

        public static List<string> GetLogNames()
        {
            var ans = new List<string>();

            ///log/dataflash

            if (is_solo_alive)
            {
                using (SshClient client = new SshClient(Solo.soloip, 22, Solo.username, Solo.password))
                {
                    client.KeepAliveInterval = TimeSpan.FromSeconds(5);
                    client.Connect();

                    if (!client.IsConnected)
                        throw new Exception("Failed to connect ssh");

                    var cmd = client.CreateCommand("ls -al /log/dataflash");
                    cmd.Execute();

                    Regex lsregex = new Regex(@"^[drwx-]+\s+[0-9]+\s+[^\s]+\s+[^\s]+\s+([0-9]+)\s+[^\s]+\s+[^\s]+\s+[^\s]+\s+([^\*\s]+).*$", RegexOptions.Multiline);

                    var matches = lsregex.Matches(cmd.Result);

                    foreach (Match match in matches)
                    {
                        var size = match.Groups[1].Value;
                        var name = match.Groups[2].Value;
                        if (name.ToLower().EndsWith(".bin"))
                            ans.Add(name);
                    }

                    /*
                    var files = cmd.Result.Split(new char[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var file in files)
                    {
                        if(file.ToLower().EndsWith(".bin"))
                            ans.Add(file);
                    }
                    */
                }
            }
            else
            {
                throw new Exception("Solo is not responding to pings");
            }

            return ans;
        }

        public static void DownloadDFLog(string source, string destination)
        {
            if (is_solo_alive)
            {
                using (SshClient client = new SshClient(Solo.soloip, 22, Solo.username, Solo.password))
                {
                    client.KeepAliveInterval = TimeSpan.FromSeconds(5);
                    client.Connect();

                    if (!client.IsConnected)
                        throw new Exception("Failed to connect ssh");
                    
                    using (ScpClient scpClient = new ScpClient(client.ConnectionInfo))
                    {
                        scpClient.Connect();

                        if (!scpClient.IsConnected)
                            throw new Exception("Failed to connect scp");

                        scpClient.Downloading += ScpClient_Downloading;

                        scpClient.Download("/log/dataflash/" + source, new FileInfo(destination));
                    }
                }
            }
            else
            {
                throw new Exception("Solo is not responding to pings");
            }
        }

        private static void ScpClient_Downloading(object sender, Renci.SshNet.Common.ScpDownloadEventArgs e)
        {
            Console.WriteLine("{0} - {1} {2}",e.Filename,e.Downloaded, (e.Downloaded/(double)e.Size)*100);
        }

        public static void flash_px4(string firmware_file)
        {
            if (is_solo_alive)
            {
                using (SshClient client = new SshClient(Solo.soloip, 22, Solo.username, Solo.password))
                {
                    client.KeepAliveInterval = TimeSpan.FromSeconds(5);
                    client.Connect();

                    if (!client.IsConnected)
                        throw new Exception("Failed to connect ssh");

                    var retcode = client.RunCommand("rm -rf /firmware/loaded");
                    
                    using (ScpClient scpClient = new ScpClient(client.ConnectionInfo))
                    {
                        scpClient.Connect();

                        if (!scpClient.IsConnected)
                            throw new Exception("Failed to connect scp");

                        scpClient.Upload(new FileInfo(firmware_file), "/firmware/" + Path.GetFileName(firmware_file));
                    }

                    var st = client.CreateShellStream("bash", 80, 24, 800, 600, 1024*8);

                    // wait for bash prompt
                    while (!st.DataAvailable)
                        System.Threading.Thread.Sleep(200);

                    st.WriteLine("loadPixhawk.py; exit;");
                    st.Flush();

                    StringBuilder output = new StringBuilder();

                    while (client.IsConnected)
                    {
                        var line = st.Read();
                        Console.Write(line);
                        output.Append(line);
                        System.Threading.Thread.Sleep(100);

                        if (output.ToString().Contains("logout"))
                            break;
                    }
                }
            }
            else
            {
                throw new Exception("Solo is not responding to pings");
            }
        }

        public static void flash(string firmware_file, string firmware_md5 = "", bool solo = true, bool clean = false)
        {
            var ip = Solo.soloip;
            if (solo == false)
                ip = Solo.controllerip;

            if ((is_solo_alive && solo) ||(is_controller_alive && !solo))
            {
                Console.WriteLine("About to connect "+ ip);
                using (SshClient client = new SshClient(ip, 22, Solo.username, Solo.password))
                {
                    client.KeepAliveInterval = TimeSpan.FromSeconds(5);
                    client.Connect();

                    if (!client.IsConnected)
                        throw new Exception("Failed to connect ssh");

                    Console.WriteLine("run update-prepare");
                    var retcode = client.RunCommand("sololink_config --update-prepare sololink");

                    if (retcode.ExitStatus != 0)
                    {
                        Console.WriteLine("run cleanup");
                        client.RunCommand("rm -rf /log/updates && mkdir -p /log/updates");
                    }

                    using (ScpClient scpClient = new ScpClient(client.ConnectionInfo))
                    {
                        scpClient.Connect();

                        if (!scpClient.IsConnected)
                            throw new Exception("Failed to connect scp");

                        if (firmware_md5 == "")
                        {
                            using (var md5 = MD5.Create())
                            using (var fs = File.OpenRead(firmware_file))
                            {
                                var hash = md5.ComputeHash(fs);

                                File.WriteAllText(firmware_file + ".md5",
                                    ByteArrayToString(hash) + "  " + Path.GetFileName(firmware_file) + "\n");
                            }

                            firmware_md5 = firmware_file + ".md5";
                        }

                        Console.WriteLine("upload firmware");
                        scpClient.Upload(new FileInfo(firmware_file), "/log/updates/" + Path.GetFileName(firmware_file));
                        scpClient.Upload(new FileInfo(firmware_md5), "/log/updates/" + Path.GetFileName(firmware_md5));
                    }

                    if (clean)
                    {
                        retcode = client.RunCommand("sololink_config --update-apply sololink --reset");
                    }
                    else
                    {
                        Console.WriteLine("update-apply");
                        retcode = client.RunCommand("sololink_config --update-apply sololink");
                    }

                    if (retcode.ExitStatus != 0)
                    {
                        if (clean)
                        {
                            retcode =
                                client.RunCommand(
                                    "touch /log/updates/UPDATE && touch /log/updates/RESETSETTINGS && shutdown -r now");
                        }
                        else
                        {
                            Console.WriteLine("reboot");
                            retcode = client.RunCommand("touch /log/updates/UPDATE && shutdown -r now");
                        }
                    }

                    client.Disconnect();
                }
            }
            else
            {
                throw new Exception("Solo is not responding to pings");
            }
        }

        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "").ToLower();
        }

        public static async Task<string> getFirmwareUrl()
        {
            const string SERVERADDR = "http://firmwarehouse.3dr.com/";
            const string TOKEN = "51fbe08cf5ef0800a07af051031a21d7f9f5438e";

            var releaseurl = SERVERADDR + "releases/";

            var results = new List<object>();

            while (true)
            {
                var list = await releaseurl.WithHeader("Authorization", ("Token " + TOKEN)).GetJsonAsync();

                IDictionary<string, object> kv = (IDictionary<string, object>)list;

                var test = kv["results"];

                results.AddRange((List<object>)test);

                if (kv.ContainsKey("next") && kv["next"] != null)
                {
                    releaseurl = kv["next"].ToString();
                }
                else
                {
                    break;
                }
            }

            // 1,10 controller
            // 2,9 drone

            // channel 1 = release

            results.Sort((o1, o2) =>
            {
                var ans1 = ((IDictionary<string, object>) o1);
                var ans2 = ((IDictionary<string, object>) o2);
                var suf1 = ans1["suffix"].ToString() == "" ? 0 : int.Parse(ans1["suffix"].ToString().TrimStart('-'));
                var suf2 = ans2["suffix"].ToString() == "" ? 0 : int.Parse(ans2["suffix"].ToString().TrimStart('-'));
                var version1 = new Version(int.Parse(ans1["major"].ToString()), int.Parse(ans1["minor"].ToString()), int.Parse(ans1["patch"].ToString()), suf1);
                var version2 = new Version(int.Parse(ans2["major"].ToString()), int.Parse(ans2["minor"].ToString()), int.Parse(ans2["patch"].ToString()), suf2);

                if (version1 > version2)
                    return 1;
                if (version1 < version2)
                    return -1;
                return 0;
            });

            foreach (var result in results)
            {
                var item = (IDictionary<string, object>) result;

                var version = new Version(int.Parse(item["major"].ToString()), int.Parse(item["minor"].ToString()), int.Parse(item["patch"].ToString()));

                Console.WriteLine(item["description"] + " " + item["file"] + " " + item["md5"] + " " + item["channel"] +" "+ item["product"] + " " + version);
            }

            return "";
        }

        public static bool Ping(string ip)
        {
            try
            {
                using (var p = new Ping())
                {
                    var options = new PingOptions();
                    options.DontFragment = true;
                    var data = "MissionPlanner";
                    var buffer = Encoding.ASCII.GetBytes(data);
                    var timeout = 2000;
                    var reply = p.Send(ip, timeout, buffer, options);
                    if (reply.Status == IPStatus.Success)
                        return true;
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}