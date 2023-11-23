using log4net;
using MissionPlanner.Plugin;
using System.Reflection;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System;
using MissionPlanner.Utilities;
using System.Windows.Forms;
using MissionPlanner.Controls;
using System.Threading.Tasks;
using MissionPlanner.ArduPilot.Mavlink;
using MissionPlanner.ArduPilot;

namespace Carbonix
{
    public class CarbonixPlugin : Plugin
    {
        public override string Name { get; } = "Carbonix Addons";
        public override string Version { get; } = "0.1";
        public override string Author { get; } = "Carbonix";

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Dictionary<string, string> settings;
        // Used to remove old settings json files automatically
        // Must be incremented any time a field is added to the settings
        public int settings_version = 2;

        public override bool Init() { return true; }

        public override bool Loaded()
        {
            // Load settings dictionary json file
            LoadSettings();

            // Add custom actions/data tabs and panel
            LoadTabs();

            // Change HUD bottom color to a lighter brown color than stock
            Host.MainForm.FlightData.Load += new EventHandler(ForceHUD);

            // Refresh the waypoints after refreshing params
            Host.comPort.ParamListChanged += Refresh_CS_WPs;

            return true;
        }

        public override bool Exit() { return true; }
        
        private void LoadSettings()
        {
            string settings_file = Path.Combine(Settings.GetUserDataDirectory(), "CarbonixSettings.json");
            bool needs_new_file = true;
            if (File.Exists(settings_file))
            {
                settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(settings_file));
                needs_new_file = Convert.ToInt32(settings["settings_version"]) < settings_version;
                if (needs_new_file)
                {
                    int i = 1;
                    string newFilename;
                    do
                    {
                        newFilename = settings_file + "." + i + ".bak";
                        i++;
                    } while (File.Exists(newFilename));
                    File.Move(settings_file, newFilename);
                }
            }

#if DEBUG
            // For debugging, always load settings from embedded file
            // This is so the json file in the git repo always matches what I am testing
            needs_new_file = true;
#endif

            if (needs_new_file)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream("Carbonix.CarbonixSettings.json"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string s = reader.ReadToEnd();
                    settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(s);
                    File.WriteAllText(settings_file, s);
                }
            }
        }

        private void LoadTabs()
        {
            // Add persistant actions panel
            ActionsControl actionsPanel = new ActionsControl(Host, settings);
            Host.MainForm.FlightData.panel_persistent.Controls.Add(actionsPanel);
            actionsPanel.Size = Host.MainForm.FlightData.panel_persistent.Size;
            actionsPanel.Anchor = AnchorStyles.Left | AnchorStyles.Right;

            // This addition makes us tight on space. Make the tabs smaller.
            Host.MainForm.FlightData.tabControlactions.ItemSize = new System.Drawing.Size(Host.MainForm.FlightData.tabControlactions.ItemSize.Width, 20);
        }

        void ForceHUD(object sender, EventArgs e)
        {
            try
            {
                MissionPlanner.GCSViews.FlightData.myhud.groundColor1 = System.Drawing.ColorTranslator.FromHtml(settings["hud_groundcolor1"]);
                MissionPlanner.GCSViews.FlightData.myhud.groundColor2 = System.Drawing.ColorTranslator.FromHtml(settings["hud_groundcolor2"]);
            }
            catch
            {
                // ignore any parsing errors
            }

            // Remove the ground color option from the right-click menu
            Host.FDMenuHud.Items.RemoveByKey("groundColorToolStripMenuItem");
        }

        void Refresh_CS_WPs(object sender, EventArgs e)
        {
            if (!Host.comPort.BaseStream.IsOpen || (Host.cs.capabilities & (int)MAVLink.MAV_PROTOCOL_CAPABILITY.FTP) == 0)
                return;

            // If we haven't read params yet, don't read the mission
            if (Host.comPort.MAV.param.Count < 100)
                return;

            IProgressReporterDialogue frmProgressReporter = new ProgressReporterDialogue
            {
                StartPosition = FormStartPosition.CenterScreen,
                Text = "Receiving WP's"
            };

            frmProgressReporter.DoWork += GetWPs;
            frmProgressReporter.UpdateProgressAndStatus(-1, "Receiving WP's");

            ThemeManager.ApplyThemeTo(frmProgressReporter);

            frmProgressReporter.RunBackgroundOperationAsync();

            frmProgressReporter.Dispose();
        }

        private void GetWPs(IProgressReporterDialogue sender)
        {
            try
            {
                var paramfileTask = Task.Run<MemoryStream>(() =>
                {
                    var ftp = new MAVFtp(Host.comPort, Host.comPort.MAV.sysid, Host.comPort.MAV.compid);
                    ftp.Progress += (status, percent) => { sender.UpdateProgressAndStatus((int)(percent), status); };
                    return ftp.GetFile("@MISSION/mission.dat", null, true, 80);
                });
                var (wps, _, _) = missionpck.unpack(paramfileTask.GetAwaiter().GetResult().ToArray());
                Host.comPort.MAV.wps.Clear();
                wps.ForEach(wp => Host.comPort.MAV.wps[wp.seq] = wp);
                return;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
