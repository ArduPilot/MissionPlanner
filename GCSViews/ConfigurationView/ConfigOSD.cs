using MissionPlanner.Controls;
using OSDConfigurator.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using MissionPlanner.Utilities;
using OSDConfigurator.GUI;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using System.Threading;
using static MAVLink;
using OSDConfigurator;
using OSDConfigurator.GUI.Osd56ItemsSetup;
using OSDConfigurator.Extensions;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigOSD : MyUserControl, IActivate, IDeactivate
    {
        [DebuggerDisplay("{Name}: {Value}")]
        public class OSDSetting : IOSDSetting
        {
            public event Action<IOSDSetting> Updated;

            private double value;
            private double originalValue;

            public string Name { get; private set; }

            public double Value
            {
                get { return value; }
                set { if (value != this.value) { this.value = value; OnUpdated(); } }
            }

            public bool Changed
            {
                get { return value != originalValue; }
            }

            public OSDSetting(string name, double value)
            {
                Name = name;
                this.value = this.originalValue = value;
            }

            private void OnUpdated()
            {
                Updated?.Invoke(this);
            }

            internal void ClearChanged()
            {
                originalValue = value;
            }

            internal void DiscardChange()
            {
                Value = originalValue;
            }
        }

        private IList<OSDSetting> osdSettings;

        public ConfigOSD()
        {
            InitializeComponent();

            btnWrite.Click += (s, e) => WriteParameters(silent: false);
            btnDiscardChanges.Click += (s, e) => DiscardChanges();
            btnRefreshParameters.Click += (s, e) => RefreshParameters();
            btnOsd56ItemsSetup.Click += (s, e) => ShowOsdTuningSlotSetupDialog();
        }

        private bool CheckConnected()
        {
            if (MainV2.comPort.BaseStream == null || !MainV2.comPort.BaseStream.IsOpen)
            {
                CustomMessageBox.Show("Your are not connected", Strings.ERROR);
                return false;
            }

            return true;
        }

        private void ShowOsdTuningSlotSetupDialog()
        {
            if (!CheckConnected())
                return;

            // Get Parameters Names assigned to OSD5/6 Items
            var assignedFunctions = GetOsdSlotInfoWithDialog();

            // Create a OSD5/6 config from parameters
            var parameters = GetOSDSettings().ToList<IOSDSetting>();
            var config = ConfigFactory.Create(parameters, Enumerable.Range(5, 2));

            // Create a names list of all available Mav parameters
            var allparameterNames = MainV2.comPort.MAV.param.Select(p => p.Name).ToList();
            allparameterNames.Sort();

            var dialog = new SetupDialog(allparameterNames.ToArray(), config.Screens, assignedFunctions);

            MissionPlanner.Utilities.ThemeManager.ApplyThemeTo(dialog);

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            var changes = dialog.GetChangedItems().ToArray();

            if (changes.Any())
            {
                UpdateOsdTuningSlots(changes);
                RefreshParameters();
            }
        }

        private void UpdateOsdTuningSlots(SetupDialog.Change[] changes)
        {
            if (!CheckConnected())
                return;

            var dialog = new ProgressReporterDialogue() { StartPosition = FormStartPosition.CenterScreen, Text = "Updating Parameters..." };

            var errors = new List<string>();
            int responseCount = 0;

            dialog.DoWork += (s) =>
            {
                using (var osdParamsProvider = new OsdTuningSlotProvider())
                {
                    var lastResponseTime = DateTime.Now;

                    osdParamsProvider.OnParamSetResponce += r =>
                    {
                        responseCount++;
                        lastResponseTime = DateTime.Now;

                        if (!r.Success)
                            errors.Add($"Screen {r.Screen} Slot {r.Index}: Update Failed");
                    };

                    foreach (var change in changes)
                    {
                        if (dialog.doWorkArgs.CancelRequested)
                            break;

                        osdParamsProvider.ParamSet(change.Screen, change.Index, change.ParamName,
                                                  (OSD_PARAM_CONFIG_TYPE)change.Type,
                                                  (float)change.Min, (float)change.Max, (float)change.Increment);
                    }

                    while (responseCount < changes.Length && (DateTime.Now - lastResponseTime).TotalSeconds < 10)
                    {
                        Thread.Sleep(100);
                    }
                }
            };

            dialog.RunBackgroundOperationAsync();

            if (dialog.doWorkArgs.CancelRequested)
                return;

            if (responseCount != changes.Length)
            {
                errors.Add($"Got {responseCount} responses of {changes.Length} requests");
            }

            if (errors.Any())
            {
                var errorMsg = string.Join(Environment.NewLine, errors);
                CustomMessageBox.Show(errorMsg);
            }
        }

        private ICollection<(byte Screen, byte Index, string Name)> GetOsdSlotInfoWithDialog()
        {
            if (!CheckConnected())
                return new List<(byte Screen, byte Index, string Name)>();

            ICollection <(byte Screen, byte Index, string Name)> result = null;

            var dialog = new ProgressReporterDialogue() { StartPosition = FormStartPosition.CenterScreen, Text = "Fetching Param Names" };

            CancellationTokenSource cts = new CancellationTokenSource();

            dialog.DoWork += (s) =>
            {
                result = OsdTuningSlotGetter.LoadAll(1, cts.Token);
            };

            dialog.doWorkArgs.CancelRequestChanged += (s, e) =>
            {
                if (dialog.doWorkArgs.CancelRequested)
                    cts.Cancel();
            };

            dialog.RunBackgroundOperationAsync();

            return result;
        }

        private static IEnumerable<OSDSetting> GetOSDSettings()
        {
            return MainV2.comPort.MAV.param
                   .Where(o => o.Name.StartsWith("OSD", StringComparison.OrdinalIgnoreCase))
                   .Select(o => new OSDSetting(o.Name, o.Value));
        }

        public static bool IsApplicable()
        {
            return GetOSDSettings().Any();
        }

        public void Activate()
        {
            osdSettings = GetOSDSettings().ToList();

            var osdTuningSlotCaptionsLazy
                = new Lazy<ICollection<(byte Screen, byte Index, string Name)>>(() => GetOsdSlotInfoWithDialog());

            osdUserControl.CaptionProvider = new OsdItemCaptionProvider(osdTuningSlotCaptionsLazy);

            osdUserControl.ApplySettings(osdSettings.ToList<IOSDSetting>());

            MissionPlanner.Utilities.ThemeManager.ApplyThemeTo(this);
        }

        public void Deactivate()
        {
            if (cbAutoWriteOnLeave.Checked)
                WriteParameters(silent: true);
        }

        private void DiscardChanges()
        {
            if ((int)DialogResult.OK == CustomMessageBox.Show("Are you sure?", MessageBoxButtons: MessageBoxButtons.OKCancel))
            {
                foreach (var p in osdSettings)
                    p.DiscardChange();
            }
        }

        private void WriteParameters(bool silent)
        {
            if (!osdSettings.Any(o => o.Changed))
            {
                if (!silent)
                    CustomMessageBox.Show("No Changes to Write!");

                return;
            }

            if (!CheckConnected())
                return;

            var dialog = new ProgressReporterDialogue() { StartPosition = FormStartPosition.CenterScreen, Text = "Writing changes..." };

            CancellationTokenSource cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;

            List<string> failed = null;

            dialog.DoWork += (s) =>
            {
                foreach (var p in osdSettings.Where(o => o.Changed))
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    try
                    {
                        MainV2.comPort.setParam(p.Name, p.Value);
                        p.ClearChanged();
                    }
                    catch
                    {
                        (failed ?? (failed = new List<string>())).Add(p.Name);
                    }
                }
            };

            dialog.doWorkArgs.CancelRequestChanged += (s, e) =>
            {
                if (dialog.doWorkArgs.CancelRequested)
                    cts.Cancel();
            };

            dialog.RunBackgroundOperationAsync();

            if (!silent && null != failed)
            {
                var failedParamsEnum = string.Join(", ", failed.Take(3)) + (failed.Count > 3 ? "..." : "");
                CustomMessageBox.Show($"Write Failed for {failed.Count} params: {failedParamsEnum}");
            }
        }

        private void RefreshParameters()
        {
            if (osdSettings.Any(o => o.Changed)
                && (int)DialogResult.No == CustomMessageBox.Show("This will reset your changes. Continue?", MessageBoxButtons: MessageBoxButtons.YesNo))
                return;

            if (!MainV2.comPort.BaseStream.IsOpen)
                return;

            if (!MainV2.comPort.MAV.cs.armed || (DialogResult.OK ==  Common.MessageShowAgain("Refresh Params", Strings.WarningUpdateParamList, true)))
            {
                this.Enabled = false;

                try
                {
                    MainV2.comPort.getParamList();
                }
                catch (Exception)
                {
                    CustomMessageBox.Show(Strings.ErrorReceivingParams, Strings.ERROR);
                }

                Activate();

                this.Enabled = true;
            }
        }
    }

    public class OsdItemCaptionProvider : IItemCaptionProvider
    {
        private readonly Lazy<ICollection<(byte Screen, byte Index, string Name)>> osdTuningSlotCaptionsLazy;

        public CaptionModes CaptionMode { get; set; }

        public OsdItemCaptionProvider(Lazy<ICollection<(byte Screen, byte Index, string Name)>> osdTuningSlotCaptionsLazy)
        {
            this.osdTuningSlotCaptionsLazy = osdTuningSlotCaptionsLazy;
        }

        public string GetItemCaption(OSDItem item, out int xOffset)
        {
            if (CaptionMode == CaptionModes.Names)
            {
                xOffset = 0;
                return item.Name;
            }

            if (item.Options.First().Name.TryParseScreenAndIndex(out byte screen, out byte index) && screen > 4)
            {
                xOffset = 0;

                return osdTuningSlotCaptionsLazy.Value.FirstOrDefault(o => o.Screen == screen && o.Index == index).Name
                       ?? $"{item.Name} (NOT SET)";
            }
            else
                return GetCommonCaption(item, out xOffset);
        }

        public string GetCommonCaption(OSDItem item, out int xOffset)
        {
            var caption = DoGetCaption(item, out xOffset);

            caption = Regex.Replace(caption, "(\\d)(\\.)(\\d)", DigitPointEvaluator);

            return caption;
        }

        private string DigitPointEvaluator(Match match)
        {
            const int SYM_NUM_WITH_DIGIT_AT_END = 192;
            const int SYM_NUM_WITH_DIGIT_AT_BEGIN = 208;

            char c1 = (char)(SYM_NUM_WITH_DIGIT_AT_END + int.Parse(match.Groups[1].Value));
            char c2 = (char)(SYM_NUM_WITH_DIGIT_AT_BEGIN + int.Parse(match.Groups[3].Value));

            return string.Concat(c1, c2);
        }

        private string DoGetCaption(OSDItem item, out int xOffset)
        {
            xOffset = 0;

            switch (item.Name)
            {
                case "ASPEED":
                    return $"{Symbols.SYM_ASPD}10{Symbols.SYM_MS}";

                case "ESCRPM":
                    return $"10{Symbols.SYM_KILO}{Symbols.SYM_RPM}";

                case "HDOP":
                    return $"{Symbols.SYM_HDOP_L}{Symbols.SYM_HDOP_R}10.2";

                case "ALTITUDE":
                    xOffset = -2;
                    return $"11{Symbols.SYM_ALT_M}";

                case "BAT_VOLT":
                case "BAT2_VLT":
                case "RESTVOLT":
                    return $"{(char)(Symbols.SYM_BATT_FULL + 1)}11.8{Symbols.SYM_VOLT}";

                case "RSSI":
                    return $"{Symbols.SYM_RSSI}93";

                case "CURRENT":
                case "CURRENT2":
                case "ESCAMPS":
                    xOffset = 0;
                    return $"8.3{Symbols.SYM_AMP}";

                case "FLTMODE":
                    return "STAB" + Symbols.SYM_DISARMED;

                case "SATS":
                    return $"{Symbols.SYM_SAT_L}{Symbols.SYM_SAT_R}13";

                case "BATUSED":
                case "BAT2USED":
                    xOffset = -1;
                    return $"125{Symbols.SYM_MAH}";

                case "HORIZON":
                    xOffset = 4;
                    var h = (char)(Symbols.SYM_AH_H_START + 4);
                    return $"{h}{h}{h}{Symbols.SYM_AH_CENTER_LINE_LEFT}{Symbols.SYM_AH_CENTER}{Symbols.SYM_AH_CENTER_LINE_RIGHT}{h}{h}{h}";

                case "COMPASS":
                    xOffset = 4;
                    return string.Concat(Symbols.SYM_HEADING_N, Symbols.SYM_HEADING_LINE, Symbols.SYM_HEADING_DIVIDED_LINE, Symbols.SYM_HEADING_LINE,
                                         Symbols.SYM_HEADING_E, Symbols.SYM_HEADING_LINE, Symbols.SYM_HEADING_DIVIDED_LINE, Symbols.SYM_HEADING_LINE,
                                         Symbols.SYM_HEADING_S);

                case "GPSLONG":
                    return $"{Symbols.SYM_GPS_LONG}  30.5003901";

                case "GPSLAT":
                    return $"{Symbols.SYM_GPS_LAT}  50.3534305";

                case "HOME":
                    return $"{Symbols.SYM_HOME}{Symbols.SYM_ARROW_START} 101{Symbols.SYM_M}";

                case "GSPEED":
                    return $"{Symbols.SYM_GSPD}{Symbols.SYM_ARROW_START} 17{Symbols.SYM_KMH}";

                case "PITCH":
                    return $"{Symbols.SYM_PTCHDWN} 10{Symbols.SYM_DEGR}";

                case "ROLL":
                    return $"{Symbols.SYM_ROLLL}  3{Symbols.SYM_DEGR}";

                case "VSPEED":
                    return $"{Symbols.SYM_UP} 0{Symbols.SYM_MS}";

                case "THROTTLE":
                    xOffset = -2;
                    return $"0{Symbols.SYM_PCNT}";

                case "HEADING":
                    xOffset = -1;
                    return $"32{Symbols.SYM_DEGR}";

                case "LINK_Q":
                    return $"99{Symbols.SYM_LQ}";

                case "RNGF":
                    return $"{Symbols.SYM_RNGFD} 1.23{Symbols.SYM_M}";

                case "FENCE":
                    return $"{Symbols.SYM_FENCE_ENABLED}";

                case "AVGCELLV":
                case "CELLVOLT":
                    return $"{(char)(Symbols.SYM_BATT_FULL + 1)}3.85{Symbols.SYM_VOLT}";

                case "VTX_PWR":
                    return $"25{Symbols.SYM_MW}";

                case "CALLSIGN":
                    return $"TOPGUN";

                case "PLUSCODE":
                    return $"MP97+Q6H";

                case "SIDEBARS":
                    xOffset = -4;
                    return $"{Symbols.SYM_SIDEBAR_A}           {Symbols.SYM_SIDEBAR_A}";

                case "CLK":
                    return $"{Symbols.SYM_CLK}12:00";

                case "ASPD1":
                case "ASPD2":
                    return $"{Symbols.SYM_ASPD}10{Symbols.SYM_MS}";

                case "ATEMP":
                case "BTEMP":
                case "TEMP":
                case "ESCTEMP":
                    return $"25{Symbols.SYM_DEGREES_C}";

                case "EFF":
                    return $"{Symbols.SYM_EFF}100{Symbols.SYM_MAH}";

                case "CLIMBEFF":
                    return $"{Symbols.SYM_PTCHUP}{Symbols.SYM_EFF}2.1{Symbols.SYM_M}";

                case "FLTIME":
                    return $"{Symbols.SYM_FLY}03:00";

                case "DIST":
                    return Symbols.SYM_DIST.ToString();

                case "XTRACK":
                    return Symbols.SYM_XERR.ToString();

                default:
                    return item.Name;
            }
        }
    }
}
