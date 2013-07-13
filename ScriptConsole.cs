using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArdupilotMega
{
    public partial class ScriptConsole : Form
    {
        private static int consoleNumber = 0;
        private Script readingScript;

        public ScriptConsole()
        {
            InitializeComponent();
        }

        private void ScriptConsole_Load(object sender, EventArgs e)
        {
            //Make the title unique
            //in the future it might be nice to show if the script is running
            //or stopped in the title too.
            Text = Text + " - Run " + consoleNumber++;
        }

        /// <summary>
        /// Set the script this console should display the output
        /// to. The script must have been set to "redirect"
        /// </summary>
        /// <param name="script">the python script instance</param>
        public void SetScript(Script script)
        {
            readingScript = script;
            Utilities.StringRedirectWriter writer = readingScript.OutputWriter;
            if (writer != null)
            {
                //Subscribe to this event so it will be called
                //for every string written
                writer.StringWritten += new Utilities.StringWrittenEvent(output_called);
            }
        }

        private void BUT_clear_Click(object sender, EventArgs e)
        {
            textOutput.Text = "";
        }

        private void output_called(object sender, string writtenText)
        {
            textOutput.AppendText(writtenText);
            if (autoscrollCheckbox.Checked)
            {
                textOutput.SelectionStart = textOutput.Text.Length;
                textOutput.ScrollToCaret();
            }
                
        }
    }
}
