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

            MainV2.config["theme_bg"] = colorDialog1.Color.ToArgb().ToString();
        }

        private void BUT_ctlbg_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = TXT_ctlbg.BackColor;

            colorDialog1.ShowDialog();

            TXT_ctlbg.BackColor = colorDialog1.Color;

            MainV2.config["theme_ctlbg"] = colorDialog1.Color.ToArgb().ToString();
        }

        private void BUT_text_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = TXT_text.BackColor;

            colorDialog1.ShowDialog();

            TXT_text.BackColor = colorDialog1.Color;

            MainV2.config["theme_text"] = colorDialog1.Color.ToArgb().ToString();
        }

        private void BUT_butbg_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = TXT_butbg.BackColor;

            colorDialog1.ShowDialog();

            TXT_butbg.BackColor = colorDialog1.Color;

            MainV2.config["theme_butbg"] = colorDialog1.Color.ToArgb().ToString();
        }

        private void BUT_butbord_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = TXT_butbord.BackColor;

            colorDialog1.ShowDialog();

            TXT_butbord.BackColor = colorDialog1.Color;

            MainV2.config["theme_butbord"] = colorDialog1.Color.ToArgb().ToString();
        }

        private void BUT_done_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ThemeColors_Load(object sender, EventArgs e)
        {
            try
            {
                TXT_bg.BackColor = Color.FromArgb(int.Parse(MainV2.config["theme_bg"].ToString()));
                TXT_ctlbg.BackColor = Color.FromArgb(int.Parse(MainV2.config["theme_ctlbg"].ToString()));
                TXT_text.BackColor = Color.FromArgb(int.Parse(MainV2.config["theme_text"].ToString()));
                TXT_butbg.BackColor = Color.FromArgb(int.Parse(MainV2.config["theme_butbg"].ToString()));
                TXT_butbord.BackColor = Color.FromArgb(int.Parse(MainV2.config["theme_butbord"].ToString()));
            }
            catch
            {
            }
        }
    }
}