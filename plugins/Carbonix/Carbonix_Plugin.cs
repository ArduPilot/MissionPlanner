using log4net;
using MissionPlanner.Plugin;
using MissionPlanner.ArduPilot.Mavlink;
using MissionPlanner.ArduPilot;
using MissionPlanner.Joystick;
using System.Reflection;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System;
using MissionPlanner.Utilities;
using System.Windows.Forms;
using MissionPlanner.Controls;
using System.Threading.Tasks;

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

        // Reference to controller menu button so text can be changed to indicate connection status
        ToolStripButton controllerMenu;

        // Time to attempt autoconnect of joystick, 5 seconds after plugin load
        DateTime controller_autoconnect_time = DateTime.MaxValue;

        public override bool Init() { return true; }

        public override bool Loaded()
        {
            // Load settings dictionary json file
            LoadSettings();

            // Add custom actions/data tabs and panel
            LoadTabs();

            // Add custom controller menu button to the main menu
            if (settings.ContainsKey("controller") && settings["controller"] != "")
            {
                SetupController();

                // Can't autoconnect in SetupController() because it causes a deadlock for some reason
                // TODO: Figure out why, and fix that instead of this hack
                controller_autoconnect_time = DateTime.Now + TimeSpan.FromSeconds(5);
            }

            // Change HUD bottom color to a lighter brown color than stock
            Host.MainForm.FlightData.Load += new EventHandler(ForceHUD);

            // Refresh the waypoints after refreshing params
            Host.comPort.ParamListChanged += Refresh_CS_WPs;

            loopratehz = 1;

            return true;
        }

        public override bool Exit() { return true; }

        bool last_controller_state = false; // Used to detect change in controller connection
        DateTime last_rchealthy = DateTime.MinValue; // Used to detect long periods of bad RC Receiver health
        DateTime last_rcwarning = DateTime.MinValue;

        public override bool Loop()
        {
            // See if it's time to autoconnect the joystick
            bool controller_state = (MissionPlanner.MainV2.joystick?.enabled == true);
            if (DateTime.Now > controller_autoconnect_time)
            {
                controller_autoconnect_time = DateTime.MaxValue;
                if (!controller_state)
                {
                    var joy = JoystickBase.Create(() => Host.comPort);

                    if (joy.start(settings["controller"]))
                    {
                        MissionPlanner.MainV2.joystick = joy;
                        MissionPlanner.MainV2.joystick.enabled = true;
                    }
                    else
                    {
                        CustomMessageBox.Show("Failed to start " + settings["controller"]);
                    }
                }
            }

            if (bool.Parse(Settings.Instance["norcreceiver", "false"]))
            {
                // Check for RC Receiver health
                // If RC shows unhealthy for 5 seconds straight, or if the joystick is disconnected, then trigger a warning
                if (Host.comPort.BaseStream.IsOpen && !Host.cs.sensors_health.rc_receiver && Host.cs.sensors_enabled.rc_receiver && Host.cs.sensors_present.rc_receiver)
                {
                    if (DateTime.UtcNow - last_rchealthy > TimeSpan.FromSeconds(5) || !controller_state)
                    {
                        // Trigger a warning every 10 seconds
                        if (DateTime.UtcNow - last_rcwarning > TimeSpan.FromSeconds(10))
                        {
                            last_rcwarning = DateTime.UtcNow;
                            Host.cs.messageHigh = "No Controller";
                        }
                    }
                }
                else
                {
                    last_rchealthy = DateTime.UtcNow;
                }
            }

            // Update the controller menu button text to indicate whether the controller is connected
            if (!last_controller_state && controller_state)
            {
                Host.MainForm.MainMenu.Invoke((MethodInvoker)delegate
                {
                    controllerMenu.Text = "Controller\nConnected";
                    controllerMenu.Invalidate();
                });
                last_controller_state = true;
            }
            else if (last_controller_state && !controller_state)
            {
                Host.MainForm.MainMenu.Invoke((MethodInvoker)delegate
                {
                    controllerMenu.Text = "Controller\nDisconnected";
                    controllerMenu.Invalidate();
                });
                last_controller_state = false;
            }

            return true;
        }

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

        private void SetupController()
        {
            // Add controller menu button to top menu
            controllerMenu = new ToolStripButton("Controller\nDisconnected");
            controllerMenu.Click += (s, e) =>
            {
                new JoystickSetup().ShowUserControl();
            };
            controllerMenu.Font = new System.Drawing.Font(controllerMenu.Font.FontFamily, 12);
            controllerMenu.AutoSize = false;
            controllerMenu.Height = Host.MainForm.MainMenu.Height;
            controllerMenu.Width = 120;
            ThemeManager.ApplyThemeTo(controllerMenu);

            Host.MainForm.MainMenu.Items.Add(controllerMenu);

            // Use some hacky BS to hide the "disable joystick" button
            // This stupid button pops up under the wind indicator, and simply hiding it is unreliable,
            // because it gets unhidden when you enter and leave the embedded FlightPlanView from the
            // right-click menu. This button really should get removed from stock Mission Planner.
            var but_disablejoystick = Host.MainForm.FlightData.GetType().GetField("but_disablejoystick", BindingFlags.NonPublic | BindingFlags.Instance);
            if (but_disablejoystick?.GetValue(Host.MainForm.FlightData) is MyButton button)
            {
                button.Width = 0;
            }

            // Disable No RC Receiver messages when using joystick
            // We will do some additional handling of that message ourselves
            Host.config["norcreceiver"] = "true";
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
