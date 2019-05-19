using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Scripting.Utils;
using MissionPlanner.ArduPilot;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MissionPlanner.test
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FirmwareSelection : ContentPage, IClose
    {
        public FirmwareSelection(List<APFirmware.FirmwareInfo> fwitems)
        {
            InitializeComponent();

            FWList = fwitems;

            OnSelectedIndexChanged(null, null);
        }

        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var FWList = this.FWList;//APFirmware.Manifest.Firmware.AsEnumerable();

            if (board_id.SelectedItem != null && board_id.SelectedItem != "Ignore")
            {
                FWList = FWList.Where(a => a.BoardId.ToString() == (string) board_id.SelectedItem);
            }

            if (mavtype.SelectedItem != null && mavtype.SelectedItem != "Ignore")
            {
                FWList = FWList.Where(a => a.MavType == (string) mavtype.SelectedItem);
            }

            if (versiontype.SelectedItem != null && versiontype.SelectedItem != "Ignore")
            {
                FWList = FWList.Where(a => a.MavFirmwareVersionType == (string) versiontype.SelectedItem);
            }

            if (format.SelectedItem != null && format.SelectedItem != "Ignore")
            {
                FWList = FWList.Where(a => a.Format == (string) format.SelectedItem);
            }

            if (platform.SelectedItem != null && platform.SelectedItem != "Ignore")
            {
                FWList = FWList.Where(a => a.Platform == (string) platform.SelectedItem);
            }

            if (latest.SelectedItem != null && latest.SelectedItem != "Ignore")
            {
                FWList = FWList.Where(a => a.Latest.ToString() == (string) latest.SelectedItem);
            }

            if (version.SelectedItem != null && version.SelectedItem != "Ignore")
            {
                FWList = FWList.Where(a => a.MavFirmwareVersion.ToString() == (string) version.SelectedItem);
            }

            if (USBID.SelectedItem != null && USBID.SelectedItem != "Ignore")
            {
                FWList = FWList.Where(a => a.Usbid.Any(b => b.Contains((string) USBID.SelectedItem)));
            }

            if (bootloader_str.SelectedItem != null && bootloader_str.SelectedItem != "Ignore")
            {
                FWList = FWList.Where(a => a.BootloaderStr.Any(b => b.Contains((string) bootloader_str.SelectedItem)));
            }

            Result.Items.Clear();
            if (FWList.Count() < 100)
            {
                Result.Items.AddRange(
                    FWList.Select(a => a.Url.AbsoluteUri));
                if (FWList.Count() == 1)
                    Result.SelectedIndex = 0;
            }
            else
            {
                Result.Items.Add("To many options - apply more filters - " + FWList.Count());
                Result.SelectedIndex = 0;
            }

            if (FWList.Count() == 0)
            {
                Result.Items.Add("No options to show");
                Result.SelectedIndex = 0;
                return;
            }


            PopulatePicker(board_id,
                FWList.Select(a => a.BoardId.ToString()).Distinct().OrderBy(a => int.Parse(a)));

            PopulatePicker(mavtype, FWList.Select(a => a.MavType.ToString()).Distinct().OrderBy(a => a));

            PopulatePicker(versiontype, FWList.Select(a => a.MavFirmwareVersionType.ToString()).Distinct().OrderBy(a => a));

            PopulatePicker(format, FWList.Select(a => a.Format.ToString()).Distinct().OrderBy(a => a));

            PopulatePicker(platform, FWList.Select(a => a.Platform.ToString()).Distinct().OrderBy(a => a));

            PopulatePicker(latest, FWList.Select(a => a.Latest.ToString()).Distinct().OrderBy(a => a));

            PopulatePicker(version, FWList.Select(a => a.MavFirmwareVersion.ToString()).Distinct().OrderBy(a => a));


            PopulatePicker(USBID,
                FWList.Where(a => a.Usbid?.Length > 0).SelectMany(a => a.Usbid, (info, s) => s).Distinct()
                    .OrderBy(a => a));

            PopulatePicker(bootloader_str,
                FWList.Where(a => a.BootloaderStr?.Length > 0).SelectMany(info => info.BootloaderStr, (info, s) => s)
                    .Distinct().OrderBy(a => a));
        }

        public IEnumerable<APFirmware.FirmwareInfo> FWList { get; set; }

        private void PopulatePicker(Picker picker, IEnumerable<string> list)
        {
            Console.WriteLine("PopulatePicker " + picker.Title);
            try
            {
                if (picker.SelectedItem == null)
                {
                    var pick = list.ToList();
                    pick.Add("Ignore");
                    picker.ItemsSource = pick;
                }

                // select it if its the only item (and Ignore)
                if (list.Count() == 1 && picker.SelectedIndex == -1)
                    picker.SelectedIndex = 0;

                if (picker.SelectedItem == "Ignore")
                    picker.SelectedItem = null;
            }
            catch
            {
            }
        }

        private void Result_OnSelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
            FinalResult = Result.SelectedItem.ToString();

            CloseAction?.Invoke();
        }

        public string FinalResult { get; set; }

        public Action CloseAction { get; set; }
    }

    public interface IClose
    {
        Action CloseAction { get; set; }
    }
}