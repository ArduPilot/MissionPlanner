using MissionPlanner;
using MissionPlanner.Comms;
using MissionPlanner.Plugin;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RedundantLinkManager
{
    public class RedundantLinkManager_Plugin : Plugin
    {
        public override string Name { get; } = "Redundant Link Manager";
        public override string Version { get; } = "0.1";
        public override string Author { get; } = "Bob Long";

        private readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // List of all links
        public readonly Dictionary<string, List<Link>> Presets;
        public string SelectedPreset;
        public BindingList<Link> Links = new BindingList<Link>();

        // Link status indicator
        public LinkStatus linkStatus;

        // Link manager form
        public RedundantLinkManager linkManager = null;

        // Task for the asynchronous connect function
        public Task connectTask = null;

        // Index of which link to autoconnect next
        // (initialized to -1 because we increment it before using it)
        public int autoConnectIndex = -1;

        /// <summary>
        /// Initializes the readonly Presets dictionary and the link status indicator
        /// </summary>
        public RedundantLinkManager_Plugin()
        {
            // Import json from RedundantLinks.json
            string filename = System.IO.Path.Combine(Settings.GetUserDataDirectory(), "RedundantLinks.json");
            // If the file doesn't exist, create a default one
            if (!System.IO.File.Exists(filename))
            {
                Link link = new Link
                {
                    Enabled = true,
                    Type = "UDP",
                    HostOrCom = "",
                    PortOrBaud = "14550",
                    Name = "UDP1",
                };
                Presets = new Dictionary<string, List<Link>>
                {
                    { "Default", new List<Link> { link } }
                };
            }
            else
            {
                string json = System.IO.File.ReadAllText(filename);
                Presets = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<Link>>>(json);
            }
        }

        public override bool Init() { return true; }

        /// <summary>
        /// Add the link manager button to the menu and load the selected preset
        /// </summary>
        /// <returns></returns>
        public override bool Loaded()
        {
            // Remove the stock connection control
            Host.MainForm.MainMenu.Items.RemoveByKey("toolStripConnectionControl");
            Host.MainForm.MainMenu.Items.RemoveByKey("MenuConnect");

            // Add the link manager button
            ToolStripButton linkManagerButton = new ToolStripButton
            {
                Name = "toolStripLinkManager",
                Size = new System.Drawing.Size(180, 22),
                Text = "Manage\nLinks",
                Alignment = ToolStripItemAlignment.Right
            };
            linkManagerButton.Click += new EventHandler(linkManagerButton_Click);

            ThemeManager.ApplyThemeTo(linkManagerButton);
            Host.MainForm.MainMenu.Items.Insert(0, linkManagerButton);

            SelectedPreset = Host.config["RedundantLinkManager_SelectedPreset", Presets.Keys.First()];
            LoadPreset(SelectedPreset);

            // Add the link status indicator to the top menu
            linkStatus = new LinkStatus(this);
            var controlHost = new ToolStripControlHost(linkStatus) { Alignment = ToolStripItemAlignment.Right };
            ThemeManager.ApplyThemeTo(controlHost);
            Host.MainForm.MainMenu.Items.Insert(1, controlHost);

            loopratehz = 1;

            return true;
        }

        /// <summary>
        /// Updates at 1Hz.
        /// Attempt to auto connect one link at a time if necessary.
        /// </summary>
        /// <returns>true</returns>
        public override bool Loop()
        {
            // Prune links from MainV2.Comports that are disabled or missing from Links
            foreach (var link in MainV2.Comports.Where(c => c.BaseStream.IsOpen && !Links.Any(l => l.comPort == c)))
            {
                // Closed ports get cleaned up and removed from MainV2.Comports automatically
                link.BaseStream.Close();
            }

            linkStatus.BeginInvoke((Action)(() => linkStatus.UpdateStatus()));

            AutoConnect();

            return true;
        }

        public override bool Exit() { return true; }

        /// <summary>
        /// Load or reload a preset
        /// </summary>
        /// <param name="presetName">Name of the preset to load</param>
        public void LoadPreset(string presetName)
        {
            SelectedPreset = presetName;
            Links.ForEach(l => l.Dispose());
            Links.Clear();
            foreach (var link in Presets[presetName])
            {
                Links.Add(link.Clone() as Link);
            }
            Host.config["RedundantLinkManager_SelectedPreset"] = presetName;
        }

        /// <summary>
        /// Save the current Links into the presets dictionary
        /// </summary>
        /// <param name="presetName"></param>
        public void SavePreset(string presetName)
        {
            // If this preset is empty, delete it instead
            if (Links.Count == 0)
            {
                Presets.Remove(presetName);
                Host.config.Remove("RedundantLinkManager_SelectedPreset");
            }
            else
            {
                Presets[presetName] = new List<Link>(Links.Select(l => l.Clone() as Link));
            }
            SaveSettings();
        }

        public void SwitchLink(int index)
        {
            if (index < 0 || index >= Links.Count ||
                Host.comPort == Links[index].comPort)
            {
                return;
            }

            CopyLinkData(Host.comPort, Links[index].comPort);
            // Switch the link over
            // Can't use Host.comPort because it has no setter function
            MainV2.comPort = Links[index].comPort;
        }

        /// <summary>
        /// If there are no running connection tasks, find the next link in the list, and start
        /// an asynchronous attempt to connect to it.
        /// </summary>
        public void AutoConnect()
        {
            // If all enabled links are connected, do nothing
            if (Links.Where(l => l.Enabled).All(l => l.comPort?.BaseStream?.IsOpen ?? false))
            {
                return;
            }

            // Log any exceptions from the connection thread
            if (connectTask?.IsFaulted ?? false)
            {
                log.Error("Error connecting link:", connectTask.Exception);
            }

            // If there is an existing connect task running, do nothing
            if (!(connectTask?.IsCompleted ?? true))
            {
                return;
            }

            // Find the first link that needs to be connected
            // (we know that at least one link needs to be connected, but we use a for loop just in case)
            for (int i = 0; i < Links.Count; i++)
            {
                autoConnectIndex = (autoConnectIndex + 1) % Links.Count;
                bool is_open = Links[autoConnectIndex].comPort?.BaseStream?.IsOpen ?? false;
                bool enabled = Links[autoConnectIndex].Enabled;
                if (enabled && !is_open)
                {
                    break;
                }
            }
            log.Info($"Attempting to connect link {Links[autoConnectIndex].Name}");
            connectTask = ConnectLinkAsync(Links[autoConnectIndex]);
        }

        /// <summary>
        /// Asynchronously attempt to connect to a link.
        /// </summary>
        /// <param name="link">Link class containing options and MAVLinkInterface</param>
        /// <returns></returns>
        private async Task ConnectLinkAsync(Link link)
        {
            link.Dispose();
            link.comPort = new MAVLinkInterface()
            {
                BaseStream = make_basestream(link),
                speechenabled = false,
            };

            // Attempt to open a connection, and listen to it for 2s
            link.comPort.BaseStream.Open();
            DateTime startTime = DateTime.Now;
            while (DateTime.Now - startTime < TimeSpan.FromSeconds(2))
            {
                if (link.comPort.BaseStream.BytesToRead > 0)
                {
                    break;
                }
                await Task.Delay(100);
            }

            // If we have data, attempt to connect
            if (link.comPort.BaseStream.BytesToRead > 0)
            {
                // If no links are connected, show the UI for the next connection
                var has_existing_connection = Host.comPort?.BaseStream?.IsOpen ?? false;

                MainV2.Comports.Add(link.comPort);
                MainV2.instance.doConnect(link.comPort, "preset", "", getparams: !has_existing_connection, showui: !has_existing_connection);

                // Copy over the params if we have another connection
                if(has_existing_connection)
                {
                    CopyLinkData(Host.comPort, link.comPort);
                }
            }
            // If we don't have data dispose and try again later
            else
            {
                link.Dispose();
            }
        }

        /// <summary>
        /// Create and open an ICommsSerial object given the options stored in "link".
        /// </summary>
        /// <param name="link">Link class containing options and MAVLinkInterface</param>
        /// <returns>ICommsSerial object</returns>
        /// <exception cref="ArgumentException">if link.Type is not a valid type</exception>
        /// <exception cref="Exception">most ICommsSerial.Open implementations can throw generic exceptions for a variety of reasons</exception>
        private ICommsSerial make_basestream(Link link)
        {
            ICommsSerial basestream;
            switch (link.Type)
            {
            case "Serial":
                basestream = new SerialPort(link.HostOrCom, int.Parse(link.PortOrBaud));
                basestream.Open();
                break;
            case "TCP":
                basestream = new TcpSerial() { Host = link.HostOrCom, Port = link.PortOrBaud };
                basestream.Open();
                break;
            case "UDP":
                basestream = new UdpSerial(new System.Net.Sockets.UdpClient(int.Parse(link.PortOrBaud)));
                basestream.Open();
                break;
            case "UDPCl":
                var temp_udpcl = new UdpSerialConnect();
                temp_udpcl.Open(link.HostOrCom, link.PortOrBaud);
                basestream = temp_udpcl;
                break;
            // Websocket is currently not possible to open without popups
            //case "WS":
            //    var temp_ws = new WebSocket();
            //    temp_ws.Open(host);
            //    basestream = temp_ws;
            //    break;
            default:
                throw new ArgumentException("Invalid port type selection");
            }

            return basestream;
        }

        /// <summary>
        /// Called when one link is switched to another. Copies all the parameters, waypoints, etc. from the old link to the new one.
        /// </summary>
        /// <param name="from_link">MAVLinkInterface of the link that is being switched from</param>
        /// <param name="to_link">MAVLinkInterface of the link that is being switched to</param>
        public void CopyLinkData(MAVLinkInterface from_link, MAVLinkInterface to_link)
        {
            if (from_link == null || to_link == null) return;

            // Don't copy to self
            if (from_link == to_link)
            {
                return;
            }

            foreach (var mav in from_link.MAVlist)
            {
                to_link.MAVlist[mav.sysid, mav.compid].param.Clear();
                to_link.MAVlist[mav.sysid, mav.compid].param.AddRange(mav.param);
                to_link.MAVlist[mav.sysid, mav.compid].param_types.Clear();
                foreach (var pt in mav.param_types)
                {
                    to_link.MAVlist[mav.sysid, mav.compid].param_types.TryAdd(pt.Key, pt.Value);
                }
                to_link.MAVlist[mav.sysid, mav.compid].wps.Clear();
                foreach (var wp in mav.wps)
                {
                    to_link.MAVlist[mav.sysid, mav.compid].wps.TryAdd(wp.Key, wp.Value);
                }
                to_link.MAVlist[mav.sysid, mav.compid].rallypoints.Clear();
                foreach (var rp in mav.rallypoints)
                {
                    to_link.MAVlist[mav.sysid, mav.compid].rallypoints.TryAdd(rp.Key, rp.Value);
                }
                to_link.MAVlist[mav.sysid, mav.compid].fencepoints.Clear();
                foreach (var fp in mav.fencepoints)
                {
                    to_link.MAVlist[mav.sysid, mav.compid].fencepoints.TryAdd(fp.Key, fp.Value);
                }
                to_link.MAVlist[mav.sysid, mav.compid].camerapoints.Clear();
                to_link.MAVlist[mav.sysid, mav.compid].camerapoints.AddRange(mav.camerapoints);
                to_link.MAVlist[mav.sysid, mav.compid].GuidedMode = mav.GuidedMode;
            }
        }

        /// <summary>
        /// Saves the current settings to RedundantLinks.json
        /// </summary>
        public void SaveSettings()
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(Presets, Newtonsoft.Json.Formatting.Indented);
            string filename = System.IO.Path.Combine(Settings.GetUserDataDirectory(), "RedundantLinks.json");
            System.IO.File.WriteAllText(filename, json);
        }

        /// <summary>
        /// Launches the link manager form if it isn't already open, or brings it to the front if it is.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkManagerButton_Click(object sender, EventArgs e)
        {
            if (linkManager?.IsDisposed ?? true)
            {
                linkManager = new RedundantLinkManager(this);
                linkManager.Show();
                return;
            }

            linkManager.BringToFront();
        }
    }
}