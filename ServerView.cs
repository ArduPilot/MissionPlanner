using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Plugin;

namespace MissionPlanner
{
    public partial class ServerView : Form
    {

        private static ServerView instance;

        public ServerView()
        {
            InitializeComponent();
        }


        private void chargeIP(object sender, EventArgs e)
        {
            try
            {
                TcpStream.InitTimer();
                TcpStream.setrealTime();
            }
            catch (Exception a)
            {
                Console.WriteLine(a.ToString());
            }

            try
            {
                TcpStream.serverPort = Int32.Parse(this.textBox3.Text);
                TcpStream.serverWindow = this;
                TcpStream.changeIP(this.textBox1.Text, this.textBox2.Text);
            }
            catch(Exception a)
            {
                Console.WriteLine(a.ToString());
            }
        }

        public Label getConnectionIndice()
        {
            return ConnectionIndice;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ServerView_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (TcpStream.IPLoaded)
            {
                try
                {
                    TcpStream.client.Close();
                }
                catch(Exception ex) { }
            }
            for (int i = Plugin.PluginLoader.Plugins.Count - 1; i >= 0; i--)
            {
                if (Plugin.PluginLoader.Plugins[i] is TcpStream)
                    Plugin.PluginLoader.Plugins.RemoveAt(i);
            }
        }
        
        private void label2_Click_1(object sender, EventArgs e)
        {

        }


        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }

        private void ServerView_MouseMove(object sender, MouseEventArgs e)
        {
            if(!TcpStream.IPLoaded)
            {
                ConnectionIndice.Text = "Disconnected";
                ConnectionIndice.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                ConnectionIndice.Text = "Connected";
                ConnectionIndice.ForeColor = System.Drawing.Color.Green;
            }
        }
        
    }
}
