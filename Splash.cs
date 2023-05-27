using System;
using System.Reflection;
using System.Windows.Forms;

namespace MissionPlanner
{
    public partial class Splash : Form
    {
        public Splash()
        {
            InitializeComponent();

            string strVersion = typeof(Splash).GetType().Assembly.GetName().Version.ToString();

            TXT_version.Text = "Version: Titan " + Application.ProductVersion; // +" Build " + strVersion;

            Console.WriteLine(strVersion);

            Console.WriteLine("Splash .ctor");
        }
    }
}