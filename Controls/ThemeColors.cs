using MissionPlanner.Utilities;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class ThemeColors : Form
    {
        public ThemeColors()
        {
            InitializeComponent();

            Utilities.ThemeManager.ApplyThemeTo(this);
        }

        private void BUT_bg_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = TXT_bg.BackColor;

            colorDialog1.ShowDialog();

            TXT_bg.BackColor = colorDialog1.Color;

            Settings.Instance["theme_bg"] = colorDialog1.Color.ToArgb().ToString();
        }

        private void BUT_ctlbg_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = TXT_ctlbg.BackColor;

            colorDialog1.ShowDialog();

            TXT_ctlbg.BackColor = colorDialog1.Color;

            Settings.Instance["theme_ctlbg"] = colorDialog1.Color.ToArgb().ToString();
        }

        private void BUT_text_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = TXT_text.BackColor;

            colorDialog1.ShowDialog();

            TXT_text.BackColor = colorDialog1.Color;

            Settings.Instance["theme_text"] = colorDialog1.Color.ToArgb().ToString();
        }

        private void BUT_butbg_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = TXT_butbg.BackColor;

            colorDialog1.ShowDialog();

            TXT_butbg.BackColor = colorDialog1.Color;

            Settings.Instance["theme_butbg"] = colorDialog1.Color.ToArgb().ToString();
        }

        private void BUT_butbord_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = TXT_butbord.BackColor;

            colorDialog1.ShowDialog();

            TXT_butbord.BackColor = colorDialog1.Color;

            Settings.Instance["theme_butbord"] = colorDialog1.Color.ToArgb().ToString();
        }

        private void BUT_done_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ThemeColors_Load(object sender, EventArgs e)
        {
            try
            {
                TXT_bg.BackColor = Color.FromArgb(Settings.Instance.GetInt32("theme_bg"));
                TXT_ctlbg.BackColor = Color.FromArgb(Settings.Instance.GetInt32("theme_ctlbg"));
                TXT_text.BackColor = Color.FromArgb(Settings.Instance.GetInt32("theme_text"));
                TXT_butbg.BackColor = Color.FromArgb(Settings.Instance.GetInt32("theme_butbg"));
                TXT_butbord.BackColor = Color.FromArgb(Settings.Instance.GetInt32("theme_butbord"));
            }
            catch
            {
            }
        }
    }
}