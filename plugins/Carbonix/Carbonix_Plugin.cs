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
using System.Linq;

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

        // Reference to Records tab, so its data can be accessed by Loop()
        public RecordsTab tabRecords;

        // List of pilots for the dropdowns on the Records tab
        public List<string> pilots = new List<string>();

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

            // Remove unnecessary UI Elements
            CleanUI();
            
            // Add extra options to FlightPlanner (like landing planner)
            AddPlanningOptions();

            // Change HUD bottom color to a lighter brown color than stock
            Host.MainForm.FlightData.Load += new EventHandler(ForceHUD);

            // Refresh the waypoints after refreshing params
            Host.comPort.ParamListChanged += Refresh_CS_WPs;

            loopratehz = 1;

            return true;
        }

        public override bool Exit() { return true; }

        bool last_arm_state = false; // Used to detect rising edge from disarm to arm
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

            // If the aircraft has just been armed, send a message to the autopilot to
            // capture the pilots and battery set in the log.
            if (Host.comPort.BaseStream.IsOpen && Host.cs.armed && !last_arm_state)
            {
                string pic = "";
                string gso = "";
                int avionics_batid = 0;
                int vtol_batid = 0;
                tabRecords.Invoke((MethodInvoker)delegate
                {
                    pic = tabRecords.cmb_pic.Text;
                    gso = tabRecords.cmb_gso.Text;
                    avionics_batid = (int)tabRecords.num_avbatid.Value;
                    vtol_batid = (int)tabRecords.num_vtolbatid.Value;

                });
                Host.comPort.send_text((byte)MAVLink.MAV_SEVERITY.INFO, "PIC: " + pic);
                Host.comPort.send_text((byte)MAVLink.MAV_SEVERITY.INFO, "GSO: " + gso);
                Host.comPort.send_text((byte)MAVLink.MAV_SEVERITY.INFO, "AVBAT: " + avionics_batid.ToString());
                Host.comPort.send_text((byte)MAVLink.MAV_SEVERITY.INFO, "VTOLBAT: " + vtol_batid.ToString());
                last_arm_state = true;
            }
            else if (!Host.comPort.BaseStream.IsOpen)
            {
                last_arm_state = false;
            }

            return true;
        }

        private void LoadSettings()
        {
            string settings_file = Path.Combine(Settings.GetUserDataDirectory(), "CarbonixSettings.json");
            bool needs_new_file = true;
            bool overwrite_pilots_file = false;
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
            overwrite_pilots_file = true;
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

            string pilots_file = Path.Combine(Settings.GetUserDataDirectory(), "pilots.json");
            if (!File.Exists(pilots_file) || overwrite_pilots_file)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream("Carbonix.VolantiPilots.json"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string s = reader.ReadToEnd();
                    File.WriteAllText(pilots_file, s);
                }
            }
            try
            {
                pilots = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(pilots_file));
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error reading pilots.json: " + ex.Message);
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

            // Add takeoff/land tab
            TabPage tabPageTakeoff = new TabPage
            {
                Text = "Takeoff/Land",
                Name = "tabTakeoff"
            };
            TakeoffTab tabTakeoff = new TakeoffTab(Host, settings) { Dock = DockStyle.Fill };
            tabPageTakeoff.Controls.Add(tabTakeoff);
            Host.MainForm.FlightData.TabListOriginal.Insert(1, tabPageTakeoff);

            // Add "records" tab, to record the current operators and batteries
            TabPage tabPageRecords = new TabPage
            {
                Text = "Records",
                Name = "tabRecords"
            };
            tabRecords = new RecordsTab(Host, pilots) { Dock = DockStyle.Fill };
            tabPageRecords.Controls.Add(tabRecords);
            Host.MainForm.FlightData.TabListOriginal.Insert(2, tabPageRecords);

            // Add emergencies tab, to access dangerous modes and settings in emergency
            TabPage tabPageEmergency = new TabPage
            {
                Text = "Emergency",
                Name = "tabEmergency"
            };
            EmergencyTab tabEmergency = new EmergencyTab(Host, settings) { Dock = DockStyle.Fill };
            tabPageEmergency.Controls.Add(tabEmergency);
            Host.MainForm.FlightData.TabListOriginal.Insert(3, tabPageEmergency);

            // refilter the display list based on user selection
            Host.MainForm.FlightData.loadTabControlActions();
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

        private void CleanUI()
        {
            // ---------------------
            // Clean FlightData menu
            // ---------------------
            Host.FDGMapControl.BeginInvokeIfRequired(() =>
            {
                string[] allowlist = settings["fdmap_menu_allow"].Split(',');

                PruneMenu(Host.FDMenuMap.Items, allowlist);
            });

            // ------------------------
            // Clean FlightPlanner Menu
            // ------------------------
            Host.FPGMapControl.BeginInvokeIfRequired(() =>
            {
                var items = Host.FPMenuMap.Items;

                // Prune the map context menu
                string[] allowlist = settings["fpmap_menu_allow"].Split(',');
                PruneMenu(Host.FPMenuMap.Items, allowlist);

                // Add another separator before Clear Mission
                Host.FPMenuMap.Items.Insert(Host.FPMenuMap.Items.IndexOfKey("clearMissionToolStripMenuItem"), new ToolStripSeparator());

                // Prune the Auto WP sub-menu
                allowlist = settings["fpmap_menu_autowp_allow"].Split(',');
                PruneMenu(
                    ((ToolStripMenuItem)Host.FPMenuMap.Items["autoWPToolStripMenuItem"]).DropDownItems,
                    allowlist
                );

                // Prune the Map Tool sub-menu
                allowlist = settings["fpmap_menu_maptool_allow"].Split(',');
                PruneMenu(
                    ((ToolStripMenuItem)Host.FPMenuMap.Items["mapToolToolStripMenuItem"]).DropDownItems,
                    allowlist
                );

                // Replace Clear Mission handler to ask for confirmation
                Host.FPMenuMap.Items["clearMissionToolStripMenuItem"].Click -= Host.MainForm.FlightPlanner.clearMissionToolStripMenuItem_Click;
                ((ToolStripMenuItem)Host.FPMenuMap.Items["clearMissionToolStripMenuItem"]).Click += (s, e) =>
                {
                    if (CustomMessageBox.Show("Are you sure you want to clear the mission?", "Clear Mission", MessageBoxButtons.YesNo) == (int)DialogResult.Yes)
                    {
                        Host.MainForm.FlightPlanner.clearMissionToolStripMenuItem_Click(s, e);
                    }
                };

            });
            // Rename some Loiter right-click entries
            // Rename "Forever" to "Unlimited"
            ((ToolStripMenuItem)Host.FPMenuMap.Items["loiterToolStripMenuItem"]).DropDownItems["loiterForeverToolStripMenuItem"].Text = "Unlimited";
            // Rename "Circles to "Turns"
            ((ToolStripMenuItem)Host.FPMenuMap.Items["loiterToolStripMenuItem"]).DropDownItems["loitercirclesToolStripMenuItem"].Text = "Turns";
            // Rebind the handler for "Time" to a better popup
            ((ToolStripMenuItem)Host.FPMenuMap.Items["loiterToolStripMenuItem"]).DropDownItems["loitertimeToolStripMenuItem"].Click -= Host.MainForm.FlightPlanner.loitertimeToolStripMenuItem_Click;
            ((ToolStripMenuItem)Host.FPMenuMap.Items["loiterToolStripMenuItem"]).DropDownItems["loitertimeToolStripMenuItem"].Click += (o, e) =>
            {
                LoiterTimeDialog dialog = new LoiterTimeDialog();

                // Show the form and wait for the user to click OK
                var response = dialog.ShowDialog();
                // Add the loiter command to the mission
                if (response == DialogResult.OK)
                {
                    PointLatLngAlt pnt = Host.FPMenuMapPosition;
                    pnt.Alt = double.Parse(Host.MainForm.FlightPlanner.TXT_DefaultAlt.Text);
                    double seconds = (dialog.dateTimePicker1.Value - dialog.dateTimePicker1.MinDate).TotalSeconds;
                    Host.AddWPtoList(MAVLink.MAV_CMD.LOITER_TIME, seconds, 0, 0, 0, pnt.Lng, pnt.Lat, pnt.Alt);
                }
            };
            // Move click handler from jump to wp to the jump menu item
            ((ToolStripMenuItem)Host.FPMenuMap.Items["jumpToolStripMenuItem"]).Click += Host.MainForm.FlightPlanner.jumpwPToolStripMenuItem_Click;
            // Clear all subitems from jump
            ((ToolStripMenuItem)Host.FPMenuMap.Items["jumpToolStripMenuItem"]).DropDownItems.Clear();
            // Clear all subitems from insert
            ((ToolStripMenuItem)Host.FPMenuMap.Items["insertWpToolStripMenuItem"]).DropDownItems.Clear();

            // -------------------------
            // Remove write-fast button
            // -------------------------
            Host.MainForm.FlightData.BeginInvokeIfRequired(() =>
            {
                Host.MainForm.FlightPlanner.but_writewpfast.Visible = false;
            });
            // Shrink the panel containing the write-fast button
            Host.MainForm.FlightData.BeginInvokeIfRequired(() =>
            {
                Host.MainForm.FlightPlanner.but_writewpfast.Parent.Height -= Host.MainForm.FlightPlanner.but_writewpfast.Height + 5;
            });

            // ------------------------------------
            // Remove WP progress bar on FlightData
            // ------------------------------------
            // Hacky, but gets the job done
            // (the control is private, and seems to resist attempts to set Visible to false)
            foreach (Control c in Host.FDGMapControl.Parent.Controls)
            {
                if (c.Name == "distanceBar1")
                {
                    c.Size = new System.Drawing.Size(0, 0);
                    break;
                }
            }

            // -----------------------------
            // Remove buttons on top menu bar
            // ------------------------------
            Host.MainForm.MainMenu.Items.RemoveByKey("MenuSimulation");
            Host.MainForm.MainMenu.Items.RemoveByKey("MenuHelp");

        }

        private void PruneMenu(ToolStripItemCollection collection, string[] allowlist)
        {
            // Loop over the collection and remove any items not in the allowlist.
            // Doing this backward so we can remove items without messing up the index.
            for (int i = collection.Count - 1; i >= 0; i--)
            {
                if (!allowlist.Contains(collection[i].Name))
                {
                    collection.RemoveAt(i);
                }
            }
        }

        // Add additional options to the FlightPlanner context menu.
        // For example, the landing planner and loiter-to-alt
        private void AddPlanningOptions()
        {
            // Add a takeoff command to the context menu
            var takeoffitem = new ToolStripMenuItem("Takeoff");
            takeoffitem.Click += (o, e) =>
            {
                // Prompt user for altitude
                int altitude = MissionPlanner.CurrentState.AltUnit == "m" ? 30 : 100;
                var result = InputBox.Show("Takeoff", "Enter takeoff altitude in " + MissionPlanner.CurrentState.AltUnit, ref altitude);
                if (result != DialogResult.OK)
                {
                    return;
                }
                // Prompt user for direction
                int direction = 90;
                result = InputBox.Show("Takeoff", "Enter takeoff direction in degrees", ref direction);
                // Add the takeoff command to the mission
                if (result == DialogResult.OK)
                {
                    PointLatLngAlt pnt = new PointLatLngAlt(0, 0, altitude);
                    Host.AddWPtoList(MAVLink.MAV_CMD.VTOL_TAKEOFF, 0, 0, 0, 0, pnt.Lng, pnt.Lat, altitude);
                    pnt = Host.cs.PlannedHomeLocation;
                    pnt.Alt = altitude;
                    // Add a little extra altitude during the transition
                    pnt.Alt += MissionPlanner.CurrentState.AltUnit == "m" ? 10 : 30;
                    // Give 500m to transition
                    pnt = pnt.newpos(direction, 500);
                    Host.AddWPtoList(MAVLink.MAV_CMD.WAYPOINT, 0, 0, 0, 0, pnt.Lng, pnt.Lat, altitude);
                }

            };

            // Create a LandingPlanUI object and add it to the FlightPlanner's context menu
            var landitem = new ToolStripMenuItem("Land");
            landitem.Click += (o, e) =>
            {
                using (Form landing_form = new LandingPlanForm(this, "VolantiLanding.json"))
                {
                    ThemeManager.ApplyThemeTo(landing_form);
                    landing_form.ShowDialog();
                }
            };
            // Insert the takeoff and landing items after the Jump submenu
            ToolStripItemCollection items = Host.FPMenuMap.Items;
            int index = items.Count;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Name == "jumpToolStripMenuItem")
                {
                    index = i + 1;
                    break;
                }
            }
            items.Insert(index, landitem);
            items.Insert(index, takeoffitem);

            // Add Loiter-to-Alt
            landitem = new ToolStripMenuItem("To Altitude");
            landitem.Click += (o, e) =>
            {
                PointLatLngAlt pnt = Host.FPMenuMapPosition;
                pnt.Alt = double.Parse(Host.MainForm.FlightPlanner.TXT_DefaultAlt.Text);
                Host.AddWPtoList(MAVLink.MAV_CMD.LOITER_TO_ALT, 0, 0, 0, 0, pnt.Lng, pnt.Lat, pnt.Alt);
            };
            // Insert the new entry at the top of the loiter submenu
            ((ToolStripMenuItem)Host.FPMenuMap.Items["loiterToolStripMenuItem"]).DropDownItems.Insert(0, landitem);
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
