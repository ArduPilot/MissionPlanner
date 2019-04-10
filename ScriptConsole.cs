using System;
using System.Windows.Forms;

namespace MissionPlanner
{
    public partial class ScriptConsole : Form
    {
        private static int consoleNumber = 0;
        private Script readingScript;
        Utilities.StringRedirectWriter writer;

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
            writer = readingScript.OutputWriter;
            //Only enable if writer is not null
            updateoutput.Enabled = (writer != null); //*/
        }

        private void BUT_clear_Click(object sender, EventArgs e)
        {
            textOutput.Text = "";
        }


        private void updateoutput_Tick(object sender, EventArgs e)
        {
            if (autoscrollCheckbox.Checked)
                textOutput.AppendText(writer.RetrieveWrittenString());
            else
                textOutput.Text += writer.RetrieveWrittenString();
        }
    }
}