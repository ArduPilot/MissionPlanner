using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class LogAnalyzer : Form
    {
        public LogAnalyzer(MissionPlanner.Utilities.LogAnalyzer.analysis analysis)
        {
            InitializeComponent();

            textBox1.Text = String.Format(@"Log File {0}
Size (kb) {1}
No of lines {2}
Duration {3}
Vehicletype {4}
Firmware Version {5}
Firmware Hash {6}
Hardware Type {7}
Free Mem {8}
Skipped Lines {9}

", analysis.logfile, analysis.sizekb, analysis.sizelines, analysis.duration, analysis.vehicletype,
                analysis.firmwareversion, analysis.firmwarehash, analysis.hardwaretype, analysis.freemem,
                analysis.skippedlines);

            foreach (var item in analysis.results)
            {
                textBox1.Text += "Test: " + item.name + " = " + item.status + " - " + item.message + "\r\n";
            }
        }
    }
}