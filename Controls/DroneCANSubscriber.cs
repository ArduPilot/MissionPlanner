using System.Collections.Generic;
using System.Windows.Forms;
using DroneCAN;
using MissionPlanner.Utilities;
using System.Linq;
using DroneCAN;

namespace MissionPlanner.Controls
{
    public class DroneCANSubscriber: MyUserControl, IActivate, IDeactivate
    {
        private ComboBox cmb_msg;
        private NumericUpDown num_lines;
        private TextBox txt_packet;
        private string selectedmsgid;
        private DroneCAN.DroneCAN can;
        private string targettype;

        public DroneCANSubscriber(DroneCAN.DroneCAN can, string selectedmsgid)
        {
            this.selectedmsgid = selectedmsgid;
            this.can=can;
            InitializeComponent();
        }



        private void InitializeComponent()
        {
            this.cmb_msg = new System.Windows.Forms.ComboBox();
            this.num_lines = new System.Windows.Forms.NumericUpDown();
            this.txt_packet = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.num_lines)).BeginInit();
            this.SuspendLayout();
            // 
            // cmb_msg
            // 
            this.cmb_msg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmb_msg.FormattingEnabled = true;
            this.cmb_msg.Location = new System.Drawing.Point(124, 3);
            this.cmb_msg.Name = "cmb_msg";
            this.cmb_msg.Size = new System.Drawing.Size(221, 21);
            this.cmb_msg.TabIndex = 0;
            this.cmb_msg.SelectedIndexChanged += new System.EventHandler(this.cmb_msg_SelectedIndexChanged);
            // 
            // num_lines
            // 
            this.num_lines.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.num_lines.Location = new System.Drawing.Point(351, 4);
            this.num_lines.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.num_lines.Name = "num_lines";
            this.num_lines.Size = new System.Drawing.Size(61, 20);
            this.num_lines.TabIndex = 1;
            this.num_lines.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // txt_packet
            // 
            this.txt_packet.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_packet.Location = new System.Drawing.Point(3, 30);
            this.txt_packet.Multiline = true;
            this.txt_packet.Name = "txt_packet";
            this.txt_packet.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txt_packet.Size = new System.Drawing.Size(409, 299);
            this.txt_packet.TabIndex = 2;
            // 
            // UAVCANSubscriber
            // 
            this.Controls.Add(this.txt_packet);
            this.Controls.Add(this.num_lines);
            this.Controls.Add(this.cmb_msg);
            this.Name = "UAVCANSubscriber";
            this.Size = new System.Drawing.Size(419, 337);
            ((System.ComponentModel.ISupportInitialize)(this.num_lines)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void cmb_msg_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            targettype = cmb_msg.Text;
        }

        public void Activate()
        {
            cmb_msg.Items.Clear();

            can.MessageReceived += Can_MessageReceived;

            var tim = new Timer() { Interval = 1000, Enabled = true };
   
            tim.Tick += (sender, e)=> 
            {          
                foreach (var item in msgtypes)
                if (!cmb_msg.Items.Contains(item))
                {
                    cmb_msg.Items.Add(item);
                }
            };
        }

        List<string> msgtypes = new List<string>();

        private void Can_MessageReceived(CANFrame frame, object msg, byte transferID)
        {
            if(msg.GetType().Name == targettype)
            {
                this.BeginInvokeIfRequired(()=>{
                    var item = msg.ToJSON(Newtonsoft.Json.Formatting.Indented);

                    var lines = item.Split(new char[] {'\n' ,'\r'}, System.StringSplitOptions.RemoveEmptyEntries);

                    var newlines = txt_packet.Lines.OfType<string>().ToList();
                    newlines.AddRange(lines);

                    if(newlines.Count > num_lines.Value)
                    {
                        txt_packet.Lines = newlines.Skip(txt_packet.Lines.Length - (int)num_lines.Value).ToArray();
                    } else
                    {
                        txt_packet.Lines = newlines.ToArray();
                    }
                });
            }

            if (!msgtypes.Contains(msg.GetType().Name))
            {
                msgtypes.Add(msg.GetType().Name);
            }
        }

        public void Deactivate()
        {           
            can.MessageReceived -= Can_MessageReceived;
        }
    }
}