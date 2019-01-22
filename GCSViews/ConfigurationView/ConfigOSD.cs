using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MissionPlanner.Controls;
using OSDConfigurator.Models;
using System.Diagnostics;
using OSDConfigurator.GUI;

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

        private OSDSetting[] parameters;

        public ConfigOSD()
        {
            InitializeComponent();

            btnWrite.Click += (s, e) => WriteParameters(silent: false);
            btnDiscardChanges.Click += (s, e) => DiscardChanges();
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
            parameters = GetOSDSettings().ToArray();

            osdUserControl.ApplySettings(parameters);

            if (parameters.Any())
            {
                panel1.Enabled = true;             
            }
            else
            {
                panel1.Enabled = false;
                CustomMessageBox.Show("No Onboard OSD parameters found");                
            }

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
                foreach (var p in parameters)
                    p.DiscardChange();
            }
        }

        private void WriteParameters(bool silent)
        {
            if (!parameters.Any(o => o.Changed))
            {
                if (!silent)
                    CustomMessageBox.Show("No Changes to Write!");

                return;
            }

            if (MainV2.comPort.BaseStream == null || !MainV2.comPort.BaseStream.IsOpen)
            {
                if (!silent)
                    CustomMessageBox.Show("Your are not connected", Strings.ERROR);

                return;
            }

            List<string> failed = null;

            foreach (var p in parameters.Where(o => o.Changed))
            {
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

            if (!silent && null != failed)
            {
                var failedParamsEnum = string.Join(", ", failed.Take(3)) + (failed.Count > 3 ? "..." : "");
                CustomMessageBox.Show($"Write Failed for {failed.Count} params: {failedParamsEnum}");
            }       
            else if (!silent)
            {
                CustomMessageBox.Show("Parameters successfully saved.", "Saved");
            }
        }
    }
}
