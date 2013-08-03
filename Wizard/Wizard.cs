using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArdupilotMega.Controls;

namespace ArdupilotMega.Wizard
{
    public partial class Wizard : Form
    {
        MainSwitcher wiz_main = null;

        List<string> history = new List<string>();

        internal static Hashtable config = new Hashtable();

        internal static Wizard instance;

        public Wizard()
        {
            instance = this;

            InitializeComponent();

            Utilities.ThemeManager.ApplyThemeTo(this);

            config.Clear();

            wiz_main = new MainSwitcher(this.panel1);

            wiz_main.ShowScreen("Intro");

            history.Add(wiz_main.current.Name);

            progressStep1.Maximum = wiz_main.screens.Count + 1;
            progressStep1.Step = 1;
        }

        public void GoNext(int progresspages, bool saveinhistory = true)
        {
            // show the next screen
            wiz_main.ShowScreen(wiz_main.screens[wiz_main.screens.IndexOf(wiz_main.current) + progresspages].Name);

            // display index 0 as 1
            progressStep1.Step = wiz_main.screens.IndexOf(wiz_main.current) + 1;

            // add a history line
            history.Add(wiz_main.current.Name);

            // enable the back button if we leave the start point
            if (wiz_main.screens.IndexOf(wiz_main.current) >= 1)
            {
                BUT_Back.Enabled = true;
            }

            if (wiz_main.current == wiz_main.screens.Last())
            {
                BUT_Next.Text = "Finish";
            }
            else
            {
                BUT_Next.Text = "NEXT >>";
            }
        }

        public void GoBack()
        {
            // show the last page in history
            wiz_main.ShowScreen(history[history.Count - 2]);

            // remove that entry
            history.RemoveAt(history.Count - 1);

            // display index 0 as 1
            progressStep1.Step = wiz_main.screens.IndexOf(wiz_main.current) + 1;

            // disable the back button if we go back to start
            if (wiz_main.screens.IndexOf(wiz_main.current) == 0)
            {
                BUT_Back.Enabled = false;
            }
        }

        private void BUT_Back_Click(object sender, EventArgs e)
        {
            GoBack();
        }

        private void BUT_Next_Click(object sender, EventArgs e)
        {
            int progresspages = 1;

            // do the current page validation.
            if (wiz_main.current.Control is IWizard)
            {
                progresspages = ((IWizard)(wiz_main.current.Control)).WizardValidate();
                if (progresspages == 0)
                {
                    return;
                }
            }

            GoNext(progresspages);
        }
    }
}