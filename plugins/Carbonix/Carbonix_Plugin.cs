using log4net;
﻿using MissionPlanner.Plugin;
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

    }
}
