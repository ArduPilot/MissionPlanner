using MissionPlanner.Controls;
using System;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigFirmwareDisabled : MyUserControl, IActivate
    {
        public ConfigFirmwareDisabled()
        {
            InitializeComponent();
        }

        public void Activate()
        {
        }

        private void but_bootloaderupdate_Click(object sender, System.EventArgs e)
        {
            var mav = MainV2.comPort;

            if (!mav.BaseStream.IsOpen)
                return;

            if (CustomMessageBox.Show("Are you sure you want to upgrade the bootloader? This can brick your board",
                    "BL Update", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == (int)DialogResult.Yes)
                if (CustomMessageBox.Show(
                        "Are you sure you want to upgrade the bootloader? This can brick your board, Please allow 5 mins for this process",
                        "BL Update", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == (int)DialogResult.Yes)
                    if (mav.doCommand(MAVLink.MAV_CMD.FLASH_BOOTLOADER, 0, 0, 0, 0, 290876, 0, 0))
                    {
                        CustomMessageBox.Show("Upgraded bootloader");
                    }
                    else
                    {
                        CustomMessageBox.Show("Failed to upgrade bootloader");
                    }
        }
    }
}