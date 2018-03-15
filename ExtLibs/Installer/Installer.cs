using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using MissionPlanner.Utilities;
using System.Configuration;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Net;
using System.Net.Mime;

namespace Installer
{
    public partial class Installer : Form
    {
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string installlocation = @"C:\Program Files (x86)\Mission Planner";

        private string md5s = "";
        private string zip = "";

        public Installer()
        {
            InitializeComponent();

            textBox1.Text = installlocation;

            radioButton1.Checked = true;
        }

        private async void but_Start_Click(object sender, EventArgs e)
        {
            this.Enabled = false;

            await Task.Run(delegate
            {
                this.Invoke((Action) delegate() { but_Start.Enabled = false; });
                start_dowork(UpdateText);
                this.Invoke((Action) delegate() { but_Start.Enabled = true; });
            });

            this.Enabled = true;
        }

        async void start_dowork(Action<string> UpdateText)
        {
            Directory.CreateDirectory(installlocation);

            var tmp = installlocation + Path.DirectorySeparatorChar;
            var checkfile = tmp + "checksums.txt";

            Regex regex = new Regex(@"([^\s]+)\s+[^/]+/(.*)", RegexOptions.IgnoreCase);
            
            if (Download.getFilefromNet(md5s, checkfile))
            {
                List<string> filestoget = new List<string>();
                var sums = File.ReadAllLines(checkfile);
                Parallel.ForEach(sums, sum =>
                {
                    var match = regex.Match(sum);
                    if (match.Success)
                    {
                        var file = tmp + match.Groups[2].Value;
                        if (File.Exists(file))
                        {
                            if (!MD5File(file, match.Groups[1].Value))
                            {
                                lock (filestoget)
                                    filestoget.Add(match.Groups[2].Value);
                            }
                        }
                        else
                        {
                            lock (filestoget)
                                filestoget.Add(match.Groups[2].Value);
                        }
                    }
                });

                DownloadStream ds = new DownloadStream(zip);

                // length / 100 = part size
                // part size rounded to closest 100kb
                ds.chunksize = (int) (Math.Floor((ds.Length/100.0) / 100000.0) * 100000.0);

                Console.WriteLine("chunk size {0}", ds.chunksize);

                int got = 0;
                using (ZipArchive zip = new ZipArchive(ds))
                {
                    foreach (var file in filestoget)
                    {
                        var entry = zip.GetEntry(file);
                        UpdateText(String.Format("Getting {0}\nFile {1} of {2}\nCompressed size {3}\nSize {4}", file, got, filestoget.Count,
                            entry?.CompressedLength, entry?.Length));
                        var output = tmp + file.Replace('/', Path.DirectorySeparatorChar);
                        var dir = Path.GetDirectoryName(output);
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);

                        if(Application.ExecutablePath.ToLower() == output.ToLower())
                            continue;

                        await Task.Run(() => { entry.ExtractToFile(output, true); });
                        got++;
                    }
                }

                UpdateText("Done");
            }
            else
            {
                MessageBox.Show("Failed to get checksum file");
            }
        }

        void UpdateText(string input)
        {
            if (InvokeRequired)
            {
                Invoke((Action) delegate() { UpdateText(input); });
            }
            else
            {
                label1.Text = input;
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

        private void but_folder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            fbd.SelectedPath = textBox1.Text;
            fbd.ShowDialog();

            textBox1.Text = fbd.SelectedPath;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            md5s = ConfigurationManager.AppSettings["UpdateLocationMD5"];
            zip = ConfigurationManager.AppSettings["UpdateLocationZip"];
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            md5s = ConfigurationManager.AppSettings["BetaUpdateLocationMD5"];
            zip = ConfigurationManager.AppSettings["BetaUpdateLocationZip"];
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            md5s = ConfigurationManager.AppSettings["MasterUpdateLocationMD5"];
            zip = ConfigurationManager.AppSettings["MasterUpdateLocationZip"];
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            installlocation = textBox1.Text;
        }
    }
}
