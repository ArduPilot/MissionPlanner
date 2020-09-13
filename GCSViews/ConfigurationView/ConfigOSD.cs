using MissionPlanner.Controls;
using OSDConfigurator.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using MissionPlanner.Utilities;

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
        private OSD_Param osd_params;

        public ConfigOSD()
        {
            InitializeComponent();

            btnWrite.Click += (s, e) => WriteParameters(silent: false);
            btnDiscardChanges.Click += (s, e) => DiscardChanges();
            btnRefreshParameters.Click += (s, e) => RefreshParameters();
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

                osd_params = new OSD_Param();
                for (byte a = 0; a < 10; a++)
                {
                    osd_params.show(5, a);
                    osd_params.show(6, a);
                }
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

        private void RefreshParameters()
        {
            if (parameters.Any(o => o.Changed)
                && (int)DialogResult.No == CustomMessageBox.Show("This will reset your changes. Continue?", MessageBoxButtons: MessageBoxButtons.YesNo))
                return;

            if (!MainV2.comPort.BaseStream.IsOpen)
                return;

            if (!MainV2.comPort.MAV.cs.armed || (int)DialogResult.OK ==
                CustomMessageBox.Show(Strings.WarningUpdateParamList, Strings.ERROR, MessageBoxButtons.OKCancel))
            {
                this.Enabled = false;

                try
                {
                    MainV2.comPort.getParamList();
                }
                catch (Exception ex)
                {
                    CustomMessageBox.Show(Strings.ErrorReceivingParams, Strings.ERROR);
                }

                Activate();

                this.Enabled = true;
            }
        }

        public class OSD_Param
        {
            static uint request_id = 0;
            private KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<MAVLink.MAVLinkMessage, bool>> sub1;
            private KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<MAVLink.MAVLinkMessage, bool>> sub2;

            List<object> @params = new List<object>();
            
            public OSD_Param()
            {
                sub1 = MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.OSD_PARAM_CONFIG_REPLY,
                    PacketResponse);
                sub2 = MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.OSD_PARAM_CONFIG_REPLY,
                    PacketResponse);
            }

            private bool PacketResponse(MAVLink.MAVLinkMessage arg)
            {
                if (arg.msgid == (uint) MAVLink.MAVLINK_MSG_ID.OSD_PARAM_CONFIG_REPLY)
                {
                    var rep = (MAVLink.mavlink_osd_param_config_reply_t) arg.data;
                    if (rep.result != (byte)MAVLink.OSD_PARAM_CONFIG_ERROR.OSD_PARAM_SUCCESS)
                    {
                        CustomMessageBox.Show("OSD Config set Error", Strings.ERROR);
                    }
                }
                else if(arg.msgid == (uint)MAVLink.MAVLINK_MSG_ID.OSD_PARAM_SHOW_CONFIG_REPLY)
                {
                    var rep = (MAVLink.mavlink_osd_param_show_config_reply_t) arg.data;
                    if (rep.result != (byte)MAVLink.OSD_PARAM_CONFIG_ERROR.OSD_PARAM_SUCCESS)
                    {
                        CustomMessageBox.Show("OSD Config show Error", Strings.ERROR);
                    }
                    else
                    {
                        var param = (rep.param_id, rep.config_type, rep.min_value, rep.max_value, rep.increment);
                        @params.Add(param);
                    }
                }

                return true;
            }

            ~OSD_Param()
            {
                MainV2.comPort.UnSubscribeToPacketType(sub1);
                MainV2.comPort.UnSubscribeToPacketType(sub2);
            }

            public void show(byte osd_screen, byte osd_index)
            {
                request_id++;
                MainV2.comPort.sendPacket(new MAVLink.mavlink_osd_param_show_config_t(request_id,
                        (byte) MainV2.comPort.sysidcurrent,
                        (byte) MainV2.comPort.compidcurrent, osd_screen, osd_index), (byte) MainV2.comPort.sysidcurrent,
                    (byte) MainV2.comPort.compidcurrent);
            }

            public void set(byte osd_screen, byte osd_index, string name, MAVLink.OSD_PARAM_CONFIG_TYPE type, float min, float max, float increment)
            {
                request_id++;
                MainV2.comPort.sendPacket(new MAVLink.mavlink_osd_param_config_t(request_id, min, max, increment,
                        (byte) MainV2.comPort.sysidcurrent,
                        (byte) MainV2.comPort.compidcurrent, osd_screen, osd_index, name.ToCharArray().ToByteArray(),
                        (byte)MAVLink.OSD_PARAM_CONFIG_TYPE.OSD_PARAM_NONE), (byte) MainV2.comPort.sysidcurrent,
                    (byte) MainV2.comPort.compidcurrent);
            }
        }
    }
}
