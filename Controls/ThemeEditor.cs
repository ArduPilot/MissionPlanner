using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    public partial class ThemeEditor : Form
    {
        public ThemeEditor()
        {
            InitializeComponent();
            Utilities.ThemeManager.ApplyThemeTo(this);

            listboxThemeItems.Items.Clear();
            foreach (ThemeColor coloritem in ThemeManager.thmColor.colors)
            {
                listboxThemeItems.Items.Add(coloritem.strColorItemName);
            }
            listboxThemeItems.SelectedIndex = 0;

            lblThemeName.Text = ThemeManager.thmColor.strThemeName;

            if (ThemeManager.thmColor.iconSet == ThemeColorTable.IconSet.HighContrastIconSet)
            {
                cbIconSet.Checked = true;
            } else
            {
                cbIconSet.Checked = false;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listboxThemeItems_SelectedIndexChanged(object sender, EventArgs e)
        {

            lblItemName.Text = listboxThemeItems.Items[listboxThemeItems.SelectedIndex].ToString();
            lblColorName.Text = ThemeManager.thmColor.colors.Find(x => x.strColorItemName.Contains(lblItemName.Text)).clrColor.ToString() ;
            colorPatch.BackColor = ThemeManager.thmColor.colors.Find(x => x.strColorItemName.Contains(lblItemName.Text)).clrColor;

        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            //Create a new theme based on the current theme. Nem theme will allways be a .mpusertheme
            string name = Path.GetFileNameWithoutExtension(ThemeManager.thmColor.strThemeName);

            if (DialogResult.Cancel == InputBox.Show("Create Theme Copy", "Enter nem theme name (without extension)", ref name))
                return;

            name = name + ".mpusertheme";
            var match = ThemeManager.ThemeNames.FirstOrDefault(stringToCheck => stringToCheck.Equals(name,StringComparison.OrdinalIgnoreCase));
            if (match != null)
            {
                CustomMessageBox.Show("User theme named " + Path.GetFileNameWithoutExtension(name) + " exists.");
                return;
            }

            string strFiletoSave = Settings.GetUserDataDirectory() + name;
            string tmpThemeName = ThemeManager.thmColor.strThemeName;
            try
            {
                ThemeManager.thmColor.strThemeName = name;
                ThemeManager.WriteToXmlFile<ThemeColorTable>(strFiletoSave,ThemeManager.thmColor);
                lblThemeName.Text = ThemeManager.thmColor.strThemeName;
            }
            catch
            {
                ThemeManager.thmColor.strThemeName = tmpThemeName;
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            ThemeManager.thmColor.SetTheme();
            ThemeManager.ApplyThemeTo(MainV2.instance);

        }

        private void colorPatch_dblclick(object sender, MouseEventArgs e)
        {
            colorSelectDialog.Color = colorPatch.BackColor;
            colorSelectDialog.ShowDialog();
            colorPatch.BackColor = colorSelectDialog.Color;
            ThemeManager.thmColor.colors.Find(x => x.strColorItemName.Contains(lblItemName.Text)).clrColor = colorSelectDialog.Color;
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            //Restore the theme from disk
            ThemeManager.LoadTheme(ThemeManager.thmColor.strThemeName);
            ThemeManager.ApplyThemeTo(MainV2.instance);
        }

        private void btnSaveApply_Click(object sender, EventArgs e)
        {

            if (ThemeManager.thmColor.strThemeName.Contains(".mpsystheme"))
                return;

            string strFiletoSave = Settings.GetUserDataDirectory() + ThemeManager.thmColor.strThemeName;
            try
            {
                ThemeManager.WriteToXmlFile<ThemeColorTable>(strFiletoSave, ThemeManager.thmColor);
                Settings.Instance["theme"] = ThemeManager.thmColor.strThemeName;
            }
            catch
            {
            }

            ThemeManager.LoadTheme(ThemeManager.thmColor.strThemeName);
            ThemeManager.ApplyThemeTo(MainV2.instance);
            CustomMessageBox.Show("You may need to select another tab or restart to see the full effect.");
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //Get current theme and reapply
            ThemeManager.LoadTheme(Settings.Instance["theme"]);
            ThemeManager.ApplyThemeTo(MainV2.instance);
        }

        private void cbIconSet_CheckedChanged(object sender, EventArgs e)
        {
            if (cbIconSet.Checked)
            {
                ThemeManager.thmColor.iconSet = ThemeColorTable.IconSet.HighContrastIconSet; 
            }
            else
            {
                ThemeManager.thmColor.iconSet = ThemeColorTable.IconSet.BurnKermitIconSet;
            }
        }
    }
    
}
