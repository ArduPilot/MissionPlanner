using Flurl;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using System.Web;
using System.Windows.Forms;
using log4net;
using MissionPlanner.Controls;

namespace MissionPlanner.Utilities
{
    public class AirMarketUI: MyUserControl, IActivate
    {

        private FlowLayoutPanel flowLayoutPanel1;
        private TextBox txt_username;
        private Label myLabel3;
        private TextBox txt_password;
        private Label myLabel2;
        private ComboBox cmb_server;
        private MyButton but_verify;
        private Label lbl_header;
        private CheckBox chk_enable;
        private Label myLabel1;

        private void InitializeComponent()
        {
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lbl_header = new System.Windows.Forms.Label();
            this.myLabel1 = new System.Windows.Forms.Label();
            this.txt_username = new System.Windows.Forms.TextBox();
            this.myLabel3 = new System.Windows.Forms.Label();
            this.txt_password = new System.Windows.Forms.TextBox();
            this.myLabel2 = new System.Windows.Forms.Label();
            this.cmb_server = new System.Windows.Forms.ComboBox();
            this.but_verify = new MissionPlanner.Controls.MyButton();
            this.chk_enable = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.lbl_header);
            this.flowLayoutPanel1.Controls.Add(this.myLabel1);
            this.flowLayoutPanel1.Controls.Add(this.txt_username);
            this.flowLayoutPanel1.Controls.Add(this.myLabel3);
            this.flowLayoutPanel1.Controls.Add(this.txt_password);
            this.flowLayoutPanel1.Controls.Add(this.myLabel2);
            this.flowLayoutPanel1.Controls.Add(this.cmb_server);
            this.flowLayoutPanel1.Controls.Add(this.but_verify);
            this.flowLayoutPanel1.Controls.Add(this.chk_enable);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(68, 22);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(190, 136);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // lbl_header
            // 
            this.lbl_header.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_header.Location = new System.Drawing.Point(3, 0);
            this.lbl_header.Name = "lbl_header";
            this.lbl_header.Size = new System.Drawing.Size(181, 23);
            this.lbl_header.TabIndex = 7;
            this.lbl_header.Text = "AirMarket - FLYSAFE";
            // 
            // myLabel1
            // 
            this.myLabel1.Location = new System.Drawing.Point(3, 23);
            this.myLabel1.Name = "myLabel1";
            this.myLabel1.Size = new System.Drawing.Size(75, 23);
            this.myLabel1.TabIndex = 0;
            this.myLabel1.Text = "Username";
            // 
            // txt_username
            // 
            this.txt_username.Location = new System.Drawing.Point(84, 26);
            this.txt_username.Name = "txt_username";
            this.txt_username.Size = new System.Drawing.Size(100, 20);
            this.txt_username.TabIndex = 3;
            // 
            // myLabel3
            // 
            this.myLabel3.Location = new System.Drawing.Point(3, 49);
            this.myLabel3.Name = "myLabel3";
            this.myLabel3.Size = new System.Drawing.Size(75, 23);
            this.myLabel3.TabIndex = 2;
            this.myLabel3.Text = "Password";
            // 
            // txt_password
            // 
            this.txt_password.Location = new System.Drawing.Point(84, 52);
            this.txt_password.Name = "txt_password";
            this.txt_password.PasswordChar = '*';
            this.txt_password.Size = new System.Drawing.Size(100, 20);
            this.txt_password.TabIndex = 4;
            this.txt_password.UseSystemPasswordChar = true;
            // 
            // myLabel2
            // 
            this.myLabel2.Location = new System.Drawing.Point(3, 75);
            this.myLabel2.Name = "myLabel2";
            this.myLabel2.Size = new System.Drawing.Size(75, 23);
            this.myLabel2.TabIndex = 1;
            this.myLabel2.Text = "Server";
            // 
            // cmb_server
            // 
            this.cmb_server.FormattingEnabled = true;
            this.cmb_server.Items.AddRange(new object[] {
            "teck.airmarket.io",
            "flysafe-pl.airmarket.io"});
            this.cmb_server.Location = new System.Drawing.Point(84, 78);
            this.cmb_server.Name = "cmb_server";
            this.cmb_server.Size = new System.Drawing.Size(100, 21);
            this.cmb_server.TabIndex = 5;
            // 
            // but_verify
            // 
            this.but_verify.Location = new System.Drawing.Point(3, 105);
            this.but_verify.Name = "but_verify";
            this.but_verify.Size = new System.Drawing.Size(75, 23);
            this.but_verify.TabIndex = 6;
            this.but_verify.Text = "Verify";
            this.but_verify.UseVisualStyleBackColor = true;
            this.but_verify.Click += new System.EventHandler(this.but_verify_Click);
            // 
            // chk_enable
            // 
            this.chk_enable.AutoSize = true;
            this.chk_enable.Location = new System.Drawing.Point(84, 105);
            this.chk_enable.Name = "chk_enable";
            this.chk_enable.Size = new System.Drawing.Size(65, 17);
            this.chk_enable.TabIndex = 8;
            this.chk_enable.Text = "Enabled";
            this.chk_enable.UseVisualStyleBackColor = true;
            this.chk_enable.CheckedChanged += new System.EventHandler(this.chk_enable_CheckedChanged);
            // 
            // AirMarketUI
            // 
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "AirMarketUI";
            this.Size = new System.Drawing.Size(321, 195);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        public AirMarketUI()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            txt_username.Text = Settings.Instance["AirMarket_username"];
            txt_password.Text = Settings.Instance["AirMarket_password"];
            cmb_server.Text = Settings.Instance["AirMarket_server"];
            chk_enable.Checked = AirMarket.Enabled;
        }

        private async void but_verify_Click(object sender, EventArgs e)
        {
            var ans = await AirMarket.ValidateCredentials(txt_username.Text, txt_password.Text, cmb_server.Text).ConfigureAwait(true);
            if (ans == false)
            {
                CustomMessageBox.Show("Username or password invalid");
            }
            else
            {
                CustomMessageBox.Show("Success checking credentials");
            }
        }

        private void chk_enable_CheckedChanged(object sender, EventArgs e)
        {
            AirMarket.Enabled = chk_enable.Checked;
        }
    }

    public class AirMarket
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        private static HttpClient httpClient;

        private static string Url { get; set; } =
            "https://{0}/webservices/validateAccount.php?id={1}&pwd={2}";

        public static bool Enabled
        {
            get { return Settings.Instance.GetBoolean("AirMarket_enabled"); }
            set { Settings.Instance["AirMarket_enabled"] = value.ToString(); }
        }

        public static async Task<bool> ValidateCredentials(string username, string password, string server = "teck.airmarket.io")
        {
            var md5 = MD5.Create();

            var md5pass = md5.ComputeHash(password.Select(a => (byte) a).ToArray()).Select(a => a.ToString("x2"));
            
            httpClient = new HttpClient();

            var url = String.Format(Url, server, username,
                String.Join("", md5pass));

            var resp = await httpClient.GetAsync(url).ConfigureAwait(false);

            if (resp.StatusCode != System.Net.HttpStatusCode.OK)
                return false;

            Settings.Instance["AirMarket_username"] = username;
            Settings.Instance["AirMarket_password"] = String.Join("", md5pass);
            Settings.Instance["AirMarket_server"] = server;
            Settings.Instance.Save();

            return true;
        }

        /// <summary>
        /// scan for tlogs to upload
        /// </summary>
        /// <param name="logsafter">use  Settings.Instance["AirMarket_logdate"]</param>
        /// <returns></returns>
        public static async Task ScanTLogs(DateTime logsafter)
        {
            if(!Enabled)
                return;

            var username = Settings.Instance["AirMarket_username"];
            var password = Settings.Instance["AirMarket_password"];
            var server = Settings.Instance["AirMarket_server"];

            var logdir = Settings.GetDefaultLogDir();

            //

            var logstoupload = Directory.GetFiles(logdir, "*.tlog", SearchOption.AllDirectories).Select(a => new FileInfo(a)).Where(a => a.LastWriteTime > logsafter).AsParallel()
                .OrderBy(a => a.LastWriteTime).ToList();

            while (logstoupload.Count > 25)
                logstoupload.RemoveAt(0);

            foreach (var log in logstoupload)
            {
                Queue.Enqueue(log);
            }

            try
            {
                await StartUploader().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                log.Error(e);
            }
        }

        private static async Task StartUploader()
        {
            var username = Settings.Instance["AirMarket_username"];
            var password = Settings.Instance["AirMarket_password"];
            var server = Settings.Instance["AirMarket_server"];

            int a = 0;
            while (Queue.Count > 0)
            {
                var current = Queue.Peek();

                var resp = await ("https://" + server + "/flightImport/logFlightImporter.php").WithBasicAuth(username, password)
                    .PostMultipartAsync(mp =>
                        mp.AddFile("logFile", current.FullName)
                            .AddString("DlbUrl", "https://" + server)
                    ).ConfigureAwait(false);

                if (resp.StatusCode != 200)
                    throw new Exception(await resp.GetStringAsync().ConfigureAwait(false));

                Settings.Instance["AirMarket_logdate"] = current.LastWriteTime.ToString();
                Queue.Dequeue();
            }
        }


        public static Queue<FileInfo> Queue { get; } = new Queue<FileInfo>();
    }
}
