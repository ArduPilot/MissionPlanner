using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using log4net;
using MissionPlanner.Comms;
using MissionPlanner.Controls;
using MissionPlanner.MsgBox;
using MissionPlanner.Radio;
using MissionPlanner.Utilities;
using uploader;
using Microsoft.VisualBasic;

namespace MissionPlanner.Radio
{
    public partial class Sikradio : UserControl, SikRadio.ISikRadioForm
    {
        public delegate void LogEventHandler(string message, int level = 0);

        public delegate void ProgressEventHandler(double completed);

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private bool beta;

        private string firmwarefile = Path.GetTempFileName();
        private Dictionary<Control, bool> _DefaultLocalEnabled = new Dictionary<Control, bool>();
        private Dictionary<ComboBox, object> _DefaultCBObjects = new Dictionary<ComboBox, object>();
        RFD.RFD900.TSession _Session;
        ExtraParamControlsSet _LocalExtraParams;
        ExtraParamControlsSet _RemoteExtraParams;

        RFDLib.GUI.Settings.TDynamicLabelEditorPairRegister _DynamicLabelEditorPairRegister;

        RFDLib.GUI.Settings.TLabelEditorPairRegister _LocalLabelEditorPairs = new RFDLib.GUI.Settings.TLabelEditorPairRegister();
        RFDLib.GUI.Settings.TLabelEditorPairRegister _RemoteLabelEditorPairs = new RFDLib.GUI.Settings.TLabelEditorPairRegister();
        Dictionary<string, string> _KnownNameDescriptions = new Dictionary<string, string>()
        {
            {"RSSI_IN_DBM", "RSSI in dBm"},
            {"AUXSER_SPEED", "Aux Baud"},
            {"AIR_FRAMELEN", "Air Frame Length"},
        };

        public event Action DoDisconnectReconnect;

        /*
ATI5
S0: FORMAT=25
S1: SERIAL_SPEED=57
S2: AIR_SPEED=64
S3: NETID=40
S4: TXPOWER=30
S5: ECC=1
S6: MAVLINK=1
S7: OPPRESEND=1
S8: MIN_FREQ=915000
S9: MAX_FREQ=928000
S10: NUM_CHANNELS=50
S11: DUTY_CYCLE=100
S12: LBT_RSSI=0
S13: MANCHESTER=0
S14: RTSCTS=0
S15: MAX_WINDOW=131
         */

        public Sikradio()
        {
            InitializeComponent();

            // hide advanced view
            //SPLIT_local.Panel2Collapsed = true;
            //SPLIT_remote.Panel2Collapsed = true;

            _LocalExtraParams = new ExtraParamControlsSet(lblNODEID, NODEID,
                lblDESTID, DESTID, lblTX_ENCAP_METHOD, TX_ENCAP_METHOD, lblRX_ENCAP_METHOD, RX_ENCAP_METHOD,
                lblMAX_DATA, MAX_DATA, lblMAX_RETRIES, MAX_RETRIES,
                new Control[] {
                lblGLOBAL_RETRIES, GLOBAL_RETRIES, lblSER_BRK_DETMS, SER_BRK_DETMS}, false);

            _RemoteExtraParams = new ExtraParamControlsSet(lblRNODEID, RNODEID,
                lblRDESTID, RDESTID, lblRTX_ENCAP_METHOD, RTX_ENCAP_METHOD, lblRRX_ENCAP_METHOD, RRX_ENCAP_METHOD,
                lblRMAX_DATA, RMAX_DATA, lblRMAX_RETRIES, RMAX_RETRIES,
                new Control[] {
                lblRGLOBAL_RETRIES, RGLOBAL_RETRIES, lblRSER_BRK_DETMS, RSER_BRK_DETMS}, true);

            // setup netid
            NETID.DataSource = Enumerable.Range(0, 500).ToArray();
            RNETID.DataSource = Enumerable.Range(0, 500).ToArray();

            MAVLINK.DisplayMember = "Value";
            MAVLINK.ValueMember = "Key";
            SetupComboForMavlink(MAVLINK, false);
            RMAVLINK.DisplayMember = "Value";
            RMAVLINK.ValueMember = "Key";
            SetupComboForMavlink(RMAVLINK, false);

            MAX_WINDOW.DataSource = Enumerable.Range(33, 131 - 32).ToArray();
            RMAX_WINDOW.DataSource = Enumerable.Range(33, 131 - 32).ToArray();

            foreach (Control C in groupBoxLocal.Controls)
            {
                _DefaultLocalEnabled[C] = C.Enabled;
            }
            foreach (Control C in groupBoxRemote.Controls)
            {
                _DefaultLocalEnabled[C] = C.Enabled;
            }

            SaveDefaultCBObjects(SERIAL_SPEED);
            SaveDefaultCBObjects(RSERIAL_SPEED);

            SaveDefaultCBObjects(AIR_SPEED);
            SaveDefaultCBObjects(RAIR_SPEED);

            SaveDefaultCBObjects(NETID);
            SaveDefaultCBObjects(RNETID);

            SaveDefaultCBObjects(NUM_CHANNELS);
            SaveDefaultCBObjects(RNUM_CHANNELS);

            SaveDefaultCBObjects(MAX_WINDOW);
            SaveDefaultCBObjects(RMAX_WINDOW);

            RFDLib.GUI.Settings.TDynamicLabelEditorPair SBUSIN = new RFDLib.GUI.Settings.TDynamicLabelEditorPair(lblSBUSIN, GPO1_3SBUSIN,
                new RFDLib.GUI.Settings.TDynamicLabelEditorPair.TSettingNameLabelTextPair[]
                {
                    new RFDLib.GUI.Settings.TDynamicLabelEditorPair.TSettingNameLabelTextPair("GPO1_3SBUSIN", "GPO1_3SBUSIN"),
                    new RFDLib.GUI.Settings.TDynamicLabelEditorPair.TSettingNameLabelTextPair("GPO1_1SBUSIN", "GPO1_1SBUSIN"),
                });
            RFDLib.GUI.Settings.TDynamicLabelEditorPair RSBUSIN = new RFDLib.GUI.Settings.TDynamicLabelEditorPair(lblRSBUSIN, RGPO1_3SBUSIN,
                new RFDLib.GUI.Settings.TDynamicLabelEditorPair.TSettingNameLabelTextPair[]
                {
                    new RFDLib.GUI.Settings.TDynamicLabelEditorPair.TSettingNameLabelTextPair("RGPO1_3SBUSIN", "GPO1_3SBUSIN"),
                    new RFDLib.GUI.Settings.TDynamicLabelEditorPair.TSettingNameLabelTextPair("RGPO1_1SBUSIN", "GPO1_1SBUSIN"),
                });
            RFDLib.GUI.Settings.TDynamicLabelEditorPair SBUSOUT = new RFDLib.GUI.Settings.TDynamicLabelEditorPair(lblSBUSOUT, GPO1_3SBUSOUT,
                new RFDLib.GUI.Settings.TDynamicLabelEditorPair.TSettingNameLabelTextPair[]
                {
                    new RFDLib.GUI.Settings.TDynamicLabelEditorPair.TSettingNameLabelTextPair("GPO1_3SBUSOUT", "GPO1_3SBUSOUT"),
                    new RFDLib.GUI.Settings.TDynamicLabelEditorPair.TSettingNameLabelTextPair("GPO1_1SBUSOUT", "GPO1_1SBUSOUT"),
                });
            RFDLib.GUI.Settings.TDynamicLabelEditorPair RSBUSOUT = new RFDLib.GUI.Settings.TDynamicLabelEditorPair(lblRSBUSOUT, RGPO1_3SBUSOUT,
                new RFDLib.GUI.Settings.TDynamicLabelEditorPair.TSettingNameLabelTextPair[]
                {
                    new RFDLib.GUI.Settings.TDynamicLabelEditorPair.TSettingNameLabelTextPair("RGPO1_3SBUSOUT", "GPO1_3SBUSOUT"),
                    new RFDLib.GUI.Settings.TDynamicLabelEditorPair.TSettingNameLabelTextPair("RGPO1_1SBUSOUT", "GPO1_1SBUSOUT"),
                });
            _DynamicLabelEditorPairRegister = new RFDLib.GUI.Settings.TDynamicLabelEditorPairRegister(new RFDLib.GUI.Settings.TDynamicLabelEditorPair[]
                {
                    SBUSIN, RSBUSIN, SBUSOUT, RSBUSOUT,
                });

            _LocalLabelEditorPairs.Add(lblNODEID, NODEID);
            _LocalLabelEditorPairs.Add(lblDESTID, DESTID);
            _LocalLabelEditorPairs.Add(lblTX_ENCAP_METHOD, TX_ENCAP_METHOD);
            _LocalLabelEditorPairs.Add(lblRX_ENCAP_METHOD, RX_ENCAP_METHOD);
            _LocalLabelEditorPairs.Add(lblMAX_DATA, MAX_DATA);
            _LocalLabelEditorPairs.Add(lblMAX_RETRIES, MAX_RETRIES);
            _LocalLabelEditorPairs.Add(lblGLOBAL_RETRIES, GLOBAL_RETRIES);
            _LocalLabelEditorPairs.Add(lblSER_BRK_DETMS, SER_BRK_DETMS);
            _LocalLabelEditorPairs.Add(label54, FSFRAMELOSS);
            _LocalLabelEditorPairs.Add(lblNETID, NETID);
            _LocalLabelEditorPairs.Add(lblTXPOWER, TXPOWER);
            _LocalLabelEditorPairs.Add(lblECC, ECC);
            _LocalLabelEditorPairs.Add(lblMAVLINK, MAVLINK);
            _LocalLabelEditorPairs.Add(lblOPPRESEND, OPPRESEND);
            _LocalLabelEditorPairs.Add(lblGPI1_1R_CIN, GPI1_1R_CIN);
            _LocalLabelEditorPairs.Add(lblGPO1_1R_COUT, GPO1_1R_COUT);
            _LocalLabelEditorPairs.Add(lblGPO1_3STATLED, GPO1_3STATLED);
            _LocalLabelEditorPairs.Add(lblGPI1_2AUXIN, GPI1_2AUXIN);
            _LocalLabelEditorPairs.Add(lblGPO1_3AUXOUT, GPO1_3AUXOUT);
            _LocalLabelEditorPairs.Add(lblMIN_FREQ, MIN_FREQ);
            _LocalLabelEditorPairs.Add(lblMAX_FREQ, MAX_FREQ);
            _LocalLabelEditorPairs.Add(lblNUM_CHANNELS, NUM_CHANNELS);
            _LocalLabelEditorPairs.Add(lblDUTY_CYCLE, DUTY_CYCLE);
            _LocalLabelEditorPairs.Add(lblLBT_RSSI, LBT_RSSI);
            _LocalLabelEditorPairs.Add(lblRTSCTS, RTSCTS);
            _LocalLabelEditorPairs.Add(lblMAX_WINDOW, MAX_WINDOW);
            _LocalLabelEditorPairs.Add(lblENCRYPTION_LEVEL, ENCRYPTION_LEVEL);
            _LocalLabelEditorPairs.Add(lblGPO1_0TXEN485, GPO1_0TXEN485);
            _LocalLabelEditorPairs.Add(lblGPIO1_1FUNC, GPIO1_1FUNC);

            _RemoteLabelEditorPairs.Add(lblRNODEID, RNODEID);
            _RemoteLabelEditorPairs.Add(lblRDESTID, RDESTID);
            _RemoteLabelEditorPairs.Add(lblRTX_ENCAP_METHOD, RTX_ENCAP_METHOD);
            _RemoteLabelEditorPairs.Add(lblRRX_ENCAP_METHOD, RRX_ENCAP_METHOD);
            _RemoteLabelEditorPairs.Add(lblRMAX_DATA, RMAX_DATA);
            _RemoteLabelEditorPairs.Add(lblRMAX_RETRIES, RMAX_RETRIES);
            _RemoteLabelEditorPairs.Add(lblRGLOBAL_RETRIES, RGLOBAL_RETRIES);
            _RemoteLabelEditorPairs.Add(lblRSER_BRK_DETMS, RSER_BRK_DETMS);
            _RemoteLabelEditorPairs.Add(lblRFSFRAMELOSS, RFSFRAMELOSS);
            _RemoteLabelEditorPairs.Add(lblRNETID, RNETID);
            _RemoteLabelEditorPairs.Add(lblRTXPOWER, RTXPOWER);
            _RemoteLabelEditorPairs.Add(lblRECC, RECC);
            _RemoteLabelEditorPairs.Add(lblRMAVLINK, RMAVLINK);
            _RemoteLabelEditorPairs.Add(lblROPPRESEND, ROPPRESEND);
            _RemoteLabelEditorPairs.Add(lblRGPI1_1R_CIN, RGPI1_1R_CIN);
            _RemoteLabelEditorPairs.Add(lblRGPO1_1R_COUT, RGPO1_1R_COUT);
            _RemoteLabelEditorPairs.Add(lblRGPO1_3STATLED, RGPO1_3STATLED);
            _RemoteLabelEditorPairs.Add(lblRGPI1_2AUXIN, RGPI1_2AUXIN);
            _RemoteLabelEditorPairs.Add(lblRGPO1_3AUXOUT, RGPO1_3AUXOUT);
            _RemoteLabelEditorPairs.Add(lblRMIN_FREQ, RMIN_FREQ);
            _RemoteLabelEditorPairs.Add(lblRMAX_FREQ, RMAX_FREQ);
            _RemoteLabelEditorPairs.Add(lblRNUM_CHANNELS, RNUM_CHANNELS);
            _RemoteLabelEditorPairs.Add(lblRDUTY_CYCLE, RDUTY_CYCLE);
            _RemoteLabelEditorPairs.Add(lblRLBT_RSSI, RLBT_RSSI);
            _RemoteLabelEditorPairs.Add(lblRRTSCTS, RRTSCTS);
            _RemoteLabelEditorPairs.Add(lblRMAX_WINDOW, RMAX_WINDOW);
            _RemoteLabelEditorPairs.Add(lblRENCRYPTION_LEVEL, RENCRYPTION_LEVEL);
            _RemoteLabelEditorPairs.Add(lblRGPO1_0TXEN485, RGPO1_0TXEN485);
            _RemoteLabelEditorPairs.Add(lblRGPIO1_1FUNC, RGPIO1_1FUNC);

            this.Disposed += DisposedEvtHdlr;
        }

        public void Connect()
        {
            var S = GetSession();
        }

        public void Disconnect()
        {
            var S = _Session;
            if ((S != null) && S.Port.IsOpen)
            {
                S.PutIntoTransparentMode();
            }
            EndSession();
        }

        void DisposedEvtHdlr(object sender, EventArgs e)
        {
            Disconnect();
        }

        private void SaveDefaultCBObjects(ComboBox CB)
        {
            if (CB.DataSource == null)
            {
                List<object> LO = new List<object>();
                foreach (var O in CB.Items)
                {
                    LO.Add(O);
                }
                _DefaultCBObjects[CB] = LO;
            }
            else
            {
                _DefaultCBObjects[CB] = CB.DataSource;
            }
        }

        private void RestoreAllDefaultCBObjects()
        {
            foreach (var kvp in _DefaultCBObjects)
            {
                if (kvp.Value is List<object>)
                {
                    kvp.Key.DataSource = null;
                    kvp.Key.Items.Clear();
                    List<object> LO = (List<object>)kvp.Value;
                    foreach (var O in LO)
                    {
                        kvp.Key.Items.Add(O);
                    }
                }
                else
                {
                    kvp.Key.DataSource = kvp.Value;
                }
            }
        }

        private bool getFirmware(Uploader.Board device, RFD.RFD900.RFD900 RFD900, bool custom = false)
        {
            if (custom)
            {
                return getFirmwareLocal(RFD900);
            }
            
            firmwarefile = Path.GetTempFileName();

            if (device == Uploader.Board.DEVICE_ID_HM_TRP)
            {
                if (beta)
                {
                    return Download.getFilefromNet("http://firmware.ardupilot.org/SiK/beta/radio~hm_trp.ihx", firmwarefile);
                }
                return Download.getFilefromNet("http://firmware.ardupilot.org/SiK/stable/radio~hm_trp.ihx",
                    firmwarefile);
            }
            if (device == Uploader.Board.DEVICE_ID_RFD900)
            {
                if (beta)
                {
                    return Download.getFilefromNet("http://firmware.ardupilot.org/SiK/beta/radio~rfd900.ihx", firmwarefile);
                }
                return Download.getFilefromNet("http://firmware.ardupilot.org/SiK/stable/radio~rfd900.ihx", firmwarefile);
            }
            if (device == Uploader.Board.DEVICE_ID_RFD900A)
            {
                if (beta)
                {
                    return Download.getFilefromNet("http://firmware.ardupilot.org/SiK/beta/radio~rfd900a.ihx",
                        firmwarefile);
                }
                return Download.getFilefromNet("http://firmware.ardupilot.org/SiK/stable/radio~rfd900a.ihx", firmwarefile);
            }
            if (device == Uploader.Board.DEVICE_ID_RFD900U)
            {
                if (beta)
                {
                    return Download.getFilefromNet("http://files.rfdesign.com.au/Files/firmware/MPSiK%20V2.6%20rfd900u.ihx", firmwarefile);
                }
                return Download.getFilefromNet("http://files.rfdesign.com.au/Files/firmware/RFDSiK%20V1.9%20rfd900u.ihx", firmwarefile);
            }
            if (device == Uploader.Board.DEVICE_ID_RFD900P)
            {
                if (beta)
                {
                    return Download.getFilefromNet("http://files.rfdesign.com.au/Files/firmware/MPSiK%20V2.6%20rfd900p.ihx", firmwarefile);
                }
                return Download.getFilefromNet("http://files.rfdesign.com.au/Files/firmware/RFDSiK%20V1.9%20rfd900p.ihx", firmwarefile);
            }
            if (device == Uploader.Board.DEVICE_ID_RFD900X)
            {
                return Download.getFilefromNet("http://files.rfdesign.com.au/Files/firmware/RFDSiK%20V2.60%20rfd900x.bin", firmwarefile);
            }
            return false;
        }

        /// <summary>
        /// Loads a local firmware file after prompting user for file.
        /// </summary>
        /// <returns></returns>
        private bool getFirmwareLocal(RFD.RFD900.RFD900 RFD900)
        {
            using (var openFileDialog1 = new OpenFileDialog())
            {
                string Filter = "Firmware|";
                string[] Exts = RFD900.FirmwareFileNameExtensions;
                for (int n = 0; n < Exts.Length; n++)
                {
                    if (n != 0)
                    {
                        Filter += ";";
                    }
                    Filter += "*." + Exts[n];
                }

                openFileDialog1.Filter = Filter;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.Multiselect = false;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        firmwarefile = openFileDialog1.FileName;
                        //File.Copy(openFileDialog1.FileName, firmwarefile, true);
                    }
                    catch (Exception ex)
                    {
                        MsgBox.CustomMessageBox.Show("Error copying file\n" + ex, "ERROR");
                        return false;
                    }
                    return true;
                }

                return false;
            }
        }

        private void Sleep(int mstimeout, ICommsSerial comPort = null)
        {
            var endtime = DateTime.Now.AddMilliseconds(mstimeout);

            while (DateTime.Now < endtime)
            {
                Thread.Sleep(1);
                Application.DoEvents();

                // prime the mavlinkserial loop with data.
                if (comPort != null)
                {
                    var test = comPort.BytesToRead;
                    test++;
                }
            }
        }

        private void BUT_upload_Click(object sender, EventArgs e)
        {
            ProgramFirmware(false);
        }

        private void iHex_ProgressEvent(double completed)
        {
            try
            {
                Progressbar.Value = (int) (completed*100);
                Application.DoEvents();
            }
            catch
            {
            }
        }

        private void uploader_LogEvent(string message, int level = 0)
        {
            try
            {
                if (level == 0)
                {
                    Console.Write(message);
                    lbl_status.Text = message;
                    log.Info(message);
                    Application.DoEvents();
                }
                else if (level < 5) // 5 = byte data
                {
                    log.Debug(message);
                }
            }
            catch
            {
            }
        }

        private void iHex_LogEvent(string message, int level = 0)
        {
            try
            {
                if (level == 0)
                {
                    lbl_status.Text = message;
                    Console.WriteLine(message);
                    log.Info(message);
                    Application.DoEvents();
                }
            }
            catch
            {
            }
        }

        private void uploader_ProgressEvent(double completed)
        {
            try
            {
                Progressbar.Value = (int)Math.Min (completed*100,100);
                Application.DoEvents();
            }
            catch
            {
            }
        }

        string GetParamNumber(string Part1)
        {
            Part1 = Part1.Trim();
            int S = Part1.IndexOf('S');
            return Part1.Substring(S + 1);
        }

        void SaveSettingsFromGroupBox(string answer, GroupBox GB, bool Remote, ICommsSerial Port, List<Control> EnabledControls)
        {
            string CommandPrefix = Remote ? "R" : "A";

            var items = answer.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in items)
            {
                if (item.Contains("ANT_MODE"))
                {
                    System.Diagnostics.Debug.WriteLine("Ant mode");
                }

                //if (item.StartsWith("S"))
                {
                    string[] values;

                    if (Remote)
                    {
                        int multipoint_fix = -1;
                        if (item.StartsWith("["))
                        {
                            multipoint_fix = item.IndexOf(']') + 1;
                        }
                        var mod_item = item;
                        if (multipoint_fix > 0 && item.Length > multipoint_fix)
                        {
                            mod_item = item.Substring(multipoint_fix).Trim();
                        }
                        values = mod_item.Split(':', '=');
                    }
                    else
                    {
                        values = item.Split(':', '=');
                    }

                    if (values.Length == 3)
                    {
                        values[1] = values[1].Replace("/", "_");

                        var control = FindControlInGroupBox(GB, (Remote ? "R" : "") + values[1].Trim());

                        if ((control != null) && EnabledControls.Contains(control))
                        {
                            if (control.GetType() == typeof(CheckBox))
                            {
                                var value = ((CheckBox)control).Checked ? "1" : "0";

                                if (value != values[2].Trim())
                                {
                                    var cmdanswer = doCommand(Port,
                                        CommandPrefix + "TS" + GetParamNumber(values[0]) + "=" + value);

                                    if (cmdanswer.Contains("OK"))
                                    {
                                        if (control.Name.Contains("GPO1_1R_COUT") ||
                                            control.Name.Contains("GPO1_3SBUSOUT"))
                                        {
                                            if (((CheckBox)control).Checked)
                                            {
                                                //Also need to set RTPO.
                                                cmdanswer = doCommand(Port,
                                                    CommandPrefix + "TPO=1");
                                            }
                                            else
                                            {
                                                cmdanswer = doCommand(Port,
                                                    CommandPrefix + "TPI=1");
                                            }
                                        }
                                        else if (control.Name.Contains("GPI1_1R_CIN") ||
                                            control.Name.Contains("GPO1_3SBUSIN"))
                                        {
                                            //Also need to set RTPI.
                                            cmdanswer = doCommand(Port,
                                                CommandPrefix + "TPI=1");
                                        }
                                        if (!cmdanswer.Contains("OK"))
                                        {
                                            MsgBox.CustomMessageBox.Show("Set Command error");
                                        }

                                    }
                                    else
                                    {
                                        if (Remote && (values[1] == "ENCRYPTION_LEVEL"))
                                        {
                                            // set this on the local radio as well.
                                            doCommand(Port, "ATS" + values[0].Trim().TrimStart('S') + "=" + value);
                                            // both radios should now be using the default key
                                        }
                                        else
                                        {
                                            MsgBox.CustomMessageBox.Show("Set Command error");
                                        }
                                    }
                                }
                            }
                            else if (control is TextBox)
                            {
                                if (Remote && !control.Name.Contains("FORMAT"))
                                {
                                    if (control.Text != values[2].Trim())
                                    {
                                        var cmdanswer = doCommand(Port,
                                            CommandPrefix + "TS" + values[0].Trim().TrimStart('S') + "=" + control.Text);

                                        if (cmdanswer.Contains("OK"))
                                        {
                                        }
                                        else
                                        {
                                            MsgBox.CustomMessageBox.Show("Set Command error");
                                        }
                                    }
                                }
                            }
                            else if (control is ComboBox)
                            {
                                string CBValue = GetCBValue((ComboBox)control);
                                if (CBValue != values[2].Trim())
                                {
                                    var cmdanswer = doCommand(Port,
                                        CommandPrefix + "TS" + GetParamNumber(values[0]) + "=" + CBValue);

                                    if (cmdanswer.Contains("OK"))
                                    {
                                    }
                                    else
                                    {
                                        MsgBox.CustomMessageBox.Show("Set Command error");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns whether encryption is enabled, as per the given combo box.
        /// </summary>
        /// <param name="CB">The combo box which is used to set encryption level.  Must not be null.</param>
        /// <returns>true if enabled, otherwise false.</returns>
        bool GetIsEncryptionEnabled(ComboBox CB)
        {
            return GetEncryptionLevelValue(CB) != 0;
        }

        /// <summary>
        /// Get the max encryption key length in hex numerals.
        /// </summary>
        /// <param name="CB">The encryption level setting combo box.  Must not be null.</param>
        /// <returns>The max encryption key length, in hex numerals.</returns>
        int GetEncryptionMaxKeyLength(ComboBox CB)
        {
            if (CB.Text.Contains("b"))
            {
                var Parts = CB.Text.Split('b');
                if (Parts.Length >= 1)
                {
                    int QTYBits;

                    if (int.TryParse(Parts[0], out QTYBits))
                    {
                        return QTYBits / 4;
                    }
                }

                return 0;
            }
            else
            {
                if (CB.Text == "0")
                {
                    return 0;
                }
                else
                {
                    return 32;
                }
            }
        }

        /// <summary>
        /// Get the encryption level setting number value.
        /// </summary>
        /// <param name="CB">The combo box to get number value for.  Must not be null.</param>
        /// <returns>The level setting number value.</returns>
        int GetEncryptionLevelValue(ComboBox CB)
        {
            if (CB.Tag != null)
            {
                RFD.RFD900.TSetting Setting = (RFD.RFD900.TSetting)CB.Tag;

                if (Setting.Options != null)
                {
                    foreach (var O in Setting.Options)
                    {
                        if (O.OptionName == CB.Text)
                        {
                            return O.Value;
                        }
                    }
                }
            }

            int Result;
            if (int.TryParse(CB.Text, out Result))
            {
                return Result;
            }
            return 0;
        }

        private void BUT_savesettings_Click(object sender, EventArgs e)
        {
            //EndSession();
            var Session = GetSession();
            
            if (Session == null)
            {
                return;
            }

            List<Control> EnabledControls = new List<Control>();
            foreach (Control C in groupBoxLocal.Controls)
            {
                if (C.Enabled)
                {
                    EnabledControls.Add(C);
                }
            }
            foreach (Control C in groupBoxRemote.Controls)
            {
                if (C.Enabled)
                {
                    EnabledControls.Add(C);
                }
            }

            EnableConfigControls(false, false);
            EnableProgrammingControls(false);
            lbl_status.Text = "Connecting";

            if (Session.PutIntoATCommandMode() == RFD.RFD900.TSession.TMode.AT_COMMAND)
            {
                // cleanup
                doCommand(Session.Port, "AT&T", false, 1);

                Session.Port.DiscardInBuffer();

                lbl_status.Text = "Doing Command";


                if (RTI.Text != "")
                {
                    // remote
                    var answer = doCommand(Session.Port, "RTI5", true);

                    SaveSettingsFromGroupBox(answer, groupBoxRemote, true, Session.Port, EnabledControls);

                    Sleep(100);
                }

                Session.Port.DiscardInBuffer();
                {
                    //local
                    string answer = "";
                    for (int n = 0; n < 5; n++)
                    {
                        answer = doCommand(Session.Port, "ATI5", true);
                        if (answer.Length != 0)
                        {
                            break;
                        }
                    }

                    SaveSettingsFromGroupBox(answer, groupBoxLocal, false, Session.Port, EnabledControls);

                    // set encryption keys at the same time, so if we are enabled we dont lose comms.
                    // we have set encryption to on for both radios, they will be using the default key atm
                    if (GetIsEncryptionEnabled(RENCRYPTION_LEVEL))
                    {
                        int MaxKeyLength = GetEncryptionMaxKeyLength(RENCRYPTION_LEVEL);

                        if (System.Text.RegularExpressions.Regex.IsMatch(txt_Raeskey.Text, @"\A\b[0-9a-fA-F]+\b\Z")
                            && (txt_Raeskey.Text.Length <= MaxKeyLength))
                        {
                            doCommand(Session.Port, "RT&E=" + txt_Raeskey.Text.PadRight(MaxKeyLength, '0'), true);
                        }
                        else
                        {
                            //Complain that encryption key invalid.
                            lbl_status.Text = "Fail";
                            MsgBox.CustomMessageBox.Show("Encryption key not valid hex number <= " + MaxKeyLength.ToString() + " hex numerals");
                        }
                    }
                    if (GetIsEncryptionEnabled(ENCRYPTION_LEVEL))
                    {
                        int MaxKeyLength = GetEncryptionMaxKeyLength(ENCRYPTION_LEVEL);

                        if (System.Text.RegularExpressions.Regex.IsMatch(txt_aeskey.Text, @"\A\b[0-9a-fA-F]+\b\Z")
                            && (txt_aeskey.Text.Length <= MaxKeyLength))
                        {
                            doCommand(Session.Port, "AT&E=" + txt_aeskey.Text.PadRight(MaxKeyLength, '0'), true);
                        }
                        else
                        {
                            //Complain that encryption key invalid.
                            lbl_status.Text = "Fail";
                            MsgBox.CustomMessageBox.Show("Encryption key not valid hex number <= " + MaxKeyLength.ToString() + " hex numerals");
                        }
                    }


                    if (RTI.Text != "")
                    {
                        // write it
                        doCommand(Session.Port, "RT&W");

                        // return to normal mode
                        doCommand(Session.Port, "RTZ");
                    }

                    // write it
                    var cmdwriteanswer = doCommand(Session.Port, "AT&W");
                    if (!cmdwriteanswer.Contains("OK"))
                    {
                        MsgBox.CustomMessageBox.Show("Failed to save parameters");
                    }

                    // return to normal mode
                    doCommand(Session.Port, "ATZ");
                }

                lbl_status.Text = "Done";
                EnableConfigControls(true, true);
            }
            else
            {
                // return to normal mode
                doCommand(Session.Port, "ATZ");

                lbl_status.Text = "Fail";
                MsgBox.CustomMessageBox.Show("Failed to enter command mode");
                EnableConfigControls(true, false);
            }

            //Need to do this because modem rebooted.
            Session.PutIntoATCommandModeAssumingInTransparentMode();

            EnableProgrammingControls(true);

            UpdateSetPPMFailSafeButtons();
        }

        /// <summary>
        /// Return an array of ints in a linear progression, but end is always included as the end.
        /// </summary>
        /// <param name="start">The start</param>
        /// <param name="step">The step</param>
        /// <param name="end">The end, always included</param>
        /// <returns>The array of ints, never null</returns>
        public static IEnumerable<int> Range(int start, int step, int end)
        {
            bool GotEnd = false;
            /*
             * To speed things up, might be best to use array and calculate
             * length ahead of time.
             * 
             * length = ((end - start - 1) / step) + 2
             * 
             * 1, 13, 3 = 5
             * 1, 14, 3 = 6
             * 1, 15, 3 = 6
             * 
             */

            try
            {

                int[] list;
                int index = 0;

                if (start == end)
                {
                    list = new int[1];
                }
                else
                {
                    list = new int[((end - start - 1) / step) + 2];
                }

                for (var a = start; a <= end; a += step)
                {
                    if (a == end)
                    {
                        GotEnd = true;
                    }
                    list[index++] = a;
                }

                if (!GotEnd)
                {
                    list[index++] = end;
                }

                return list;
            }
            catch (Exception e)
            {
                //Console.WriteLine();

                throw e;
            }
        }

        private void SetupCBWithDefaultEncryptionOptions(ComboBox CB)
        {
            CB.Tag = null;
            CB.DataSource = Range(0, 1, 1);
            CB.Text = "0";
        }

        /// <summary>
        /// Set the given combo box to use the corresponding setting in the given dictionary of
        /// settings.  If no corresponding setting exists in that dictionary, just set it to the 
        /// given value.
        /// </summary>
        /// <param name="CB">The combo box.  Must not be null.</param>
        /// <param name="Settings">The settings.  Must not be null.</param>
        /// <param name="Value">The string value representation to set it to.  Must not be null.</param>
        /// <param name="Remote">true if it is a remote modem setting, false if local.</param>
        /// <returns></returns>
        private bool SetupCBWithSetting(ComboBox CB, Dictionary<string, RFD.RFD900.TSetting> Settings,
            string Value, bool Remote, string SettingName)
        {
            foreach (var kvp in Settings)
            {
                string CBName = kvp.Value.Name.Replace('/', '_');

                if (CBName == SettingName)
                {
                    var Setting = kvp.Value;
                    if (Setting.Options != null)
                    {
                        //Use options.
                        string[] OptionNames = Setting.GetOptionNames();
                        string OptionName = Setting.GetOptionNameForValue(Value);
                        if (OptionName == null)
                        {
                            Array.Resize(ref OptionNames, OptionNames.Length + 1);
                            OptionNames[OptionNames.Length - 1] = Value;
                            OptionName = Value;
                        }

                        CB.DataSource = OptionNames;
                        CB.Text = OptionName;
                        CB.Tag = Setting;
                        return true;
                    }
                    if (Setting.Range != null)
                    {
                        CB.DataSource = Setting.Range.GetOptionsIncludingValue(Setting.Value);
                        CB.Text = Value;
                        CB.Tag = null;
                        return true;
                    }
                }
            }
            
            CB.Tag = null;
            CB.Text = Value;
            return false;
        }

        private string GetCBValue(ComboBox CB)
        {
            if (CB.Tag != null)
            {
                RFD.RFD900.TSetting Setting = (RFD.RFD900.TSetting)CB.Tag;
                foreach (var O in Setting.Options)
                {
                    if (O.OptionName == CB.Text)
                    {
                        return O.Value.ToString();
                    }
                }
            }

            //If got to here, and it's a MAVLink setting, revert back to the old method...
            if (CB.Name.Contains("MAVLINK"))
            {
                var Value = CB.SelectedValue;
                if (Value != null)
                {
                    return Value.ToString();
                }
            }

            //If got here, just return the text.
            return CB.Text;
        }

        void SetupComboForMavlink(ComboBox CB,  bool Simple)
        {
            Dictionary<int, string> dict;
            if (Simple)
            {
                dict = Enum.GetValues(typeof(mavlink_option_simple))
                    .Cast<mavlink_option>()
                    .ToDictionary(t => (int)t, t => t.ToString());
            }
            else
            {
                dict = Enum.GetValues(typeof(mavlink_option))
                    .Cast<mavlink_option>()
                    .ToDictionary(t => (int)t, t => t.ToString());
            }

            CB.DataSource = dict.ToArray();
        }

        /// <summary>
        /// Given an array of lines returned from ATI5 command from a modem,
        /// remove the "[n]" from the start of the lines.  The "[n]" is returned
        /// from a modem at the start of the lines if it is running multipoint firmware.
        /// </summary>
        /// <param name="items">The raw lines returned from the modem.  Must not be null.</param>
        /// <param name="multipoint_fix">The character index into the string immediately after
        /// the initial "[n]", or -1 if not multipoint.</param>
        /// <returns>The modified lines.  Never null.</returns>
        string[] ModifyReturnedStringsForMultipoint(string[] items, int multipoint_fix)
        {
            string[] Result = new string[items.Length];

            for (int n = 0; n < items.Length; n++)
            {
                Result[n] = items[n];
                if (multipoint_fix > 0 && items[n].Length > multipoint_fix)
                {
                    Result[n] = items[n].Substring(multipoint_fix).Trim();
                }
            }

            return Result;
        }

        Control FindControlInGroupBox(GroupBox GB, string Name)
        {
            Control Result = _DynamicLabelEditorPairRegister.FindAndSetUpEditorWithSettingName(Name);
            if (Result == null)
            {
                var Array = GB.Controls.Find(Name, true);

                if (Array.Length == 0)
                {
                    return null;
                }
                else
                {
                    return Array[0];
                }
            }
            else
            {
                return Result;
            }
        }

        /// <summary>
        /// Returns whether it can be determined that the setting (from the given Settings) with the given
        /// SettingName only has one value/option available (i.e. setting can't be changed).
        /// </summary>
        /// <param name="Settings">The dictionary of settings for the modem.  Must not be null.</param>
        /// <param name="SettingName">The setting name.  Must not be null.</param>
        /// <returns>Returns true if it can be determined that the setting can only have one value, otherwise false.</returns>
        bool GetDoesCheckboxHaveOnlyOneOption(Dictionary<string, RFD.RFD900.TSetting> Settings, string SettingName)
        {
            if (Settings.ContainsKey(SettingName))
            {
                var Setting = Settings[SettingName];

                if (Setting.Options != null)
                {
                    if (Setting.Options.Length == 1)
                    {
                        return true;
                    }
                }
                if (Setting.Range != null)
                {
                    if (Setting.Range.GetOptions().Length == 1)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Configure and return a spare editor if one is available.
        /// </summary>
        /// <param name="ThisSetting">The name of the setting to get the editor for.  Must not be null.</param>
        /// <param name="Setting">The setting to get the editor for.  Must not be null.</param>
        /// <param name="Remote">Whether it is the remote set of settings.</param>
        /// <returns>A control to use, or null if none available.</returns>
        Control GetSpareEditor(string ThisSetting, RFD.RFD900.TSetting Setting, bool Remote)
        {
            RFDLib.GUI.Settings.TLabelEditorPairRegister Reg = Remote ? _RemoteLabelEditorPairs : _LocalLabelEditorPairs;
            Control Result = null;
            string Description = Remote ? ThisSetting.Substring(1) : ThisSetting;

            if (_KnownNameDescriptions.ContainsKey(Description))
            {
                Description = _KnownNameDescriptions[Description];
            }

            if (Setting.GetIsFlag())
            {
                Result = Reg.GetSpareCheckbox(ThisSetting, Description);
            }

            if (Result == null)
            {
                Result = Reg.GetSpareComboBox(ThisSetting, Description);
            }

            return Result;
        }

        /// <summary>
        /// Given a groupbox containing a set of controls, load them with the given values and settings.
        /// </summary>
        /// <param name="GB">The groupbox.  Must not be null.</param>
        /// <param name="Remote">true if it is the remote groupbox, false if it is the local groupbox.</param>
        /// <param name="items">The lines returned by ATI5 command.  Must not be null.</param>
        /// <param name="Settings">The settings parsed from the modem.  Must not be null.</param>
        /// <returns>true if at least one setting had an invalid value, otherwise false.</returns>
        bool SetUpControlsWithValues(GroupBox GB, bool Remote, string[] items, Dictionary<string, RFD.RFD900.TSetting> Settings)
        {
            /*RFD900Tools.GUI.frmCfgTest frm = new RFD900Tools.GUI.frmCfgTest();
            RFD900Tools.GUI.TConfig Cfg = new RFD900Tools.GUI.TConfig(frm.GetControl());
            Cfg.Update(Settings, items);
            frm.Show();*/

            _LocalLabelEditorPairs.SetUp(new List<string>(Settings.Keys));
            _RemoteLabelEditorPairs.SetUp(new List<string>(
                RFDLib.Array.CherryPickArray(Settings.Keys.ToArray(), 
                    (n) => "R" + n)));

            bool SomeSettingsInvalid = false;

            foreach (var item in items)
            {
                var values = item.Split(new char[] { ':', '=' }, StringSplitOptions.RemoveEmptyEntries);

                if (item.Contains("ANT_MODE"))
                {
                    System.Diagnostics.Debug.WriteLine("Ant mode");
                }

                if (values.Length == 3)
                {
                    string SettingName = values[1].Replace("/", "_").Trim();

                    var control = FindControlInGroupBox(GB, (Remote ? "R" : "") + SettingName);

                    if (control == null)
                    {
                        if (Settings.ContainsKey(SettingName))
                        {
                            var Setting = Settings[SettingName];
                            if (Setting.Range != null || Setting.Options != null)
                            {
                                control = GetSpareEditor((Remote ? "R" : "") + SettingName, Setting, Remote);
                            }
                        }
                    }

                    if (control != null)
                    {
                        GB.Enabled = true;
                        control.Parent.Enabled = true;
                        control.Enabled = true;

                        if (control is CheckBox)
                        {
                            ((CheckBox)control).Checked = values[2].Trim() == "1";
                            //If the setting can only have one option/value...
                            if (GetDoesCheckboxHaveOnlyOneOption(Settings, values[1].Trim()))
                            {
                                //Disable the control
                                control.Enabled = false;
                            }
                        }
                        else if (control is TextBox)
                        {
                            ((TextBox)control).Text = values[2].Trim();
                        }
                        else if (control.Name.Contains("MAVLINK") && !(Settings.ContainsKey("MAVLINK") && (Settings["MAVLINK"].Options != null))) //
                        {
                            var ans = Enum.Parse(typeof(mavlink_option), values[2].Trim());
                            ((ComboBox)control).Text = ans.ToString();
                        }
                        else if (control is ComboBox)
                        {
                            if (!SetupCBWithSetting((ComboBox)control, Settings,
                                values[2].Trim(), Remote, SettingName))
                            {
                                ((ComboBox)control).Text = values[2].Trim();
                                if (((ComboBox)control).Text != values[2].Trim())
                                {
                                    SomeSettingsInvalid = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    log.Info("Odd config line :" + item);
                }
            }

            return SomeSettingsInvalid;
        }

        /// <summary>
        /// Get the country code from the modem as a string, or "--" if unknown or not locked to country.
        /// </summary>
        /// <param name="Session">The session.  Must not be null.</param>
        /// <param name="GetCC">The function to get the country code, given the modem object.  Must not be null.</param>
        /// <returns>The country code string.</returns>
        string GetCountryCodeFromSession(RFD.RFD900.TSession Session, 
            Func<RFD.RFD900.RFD900xuxRevN, RFD.RFD900.RFD900xux.TCountry> GetCC)
        {
            RFD.RFD900.RFD900xux.TCountry CC;
            var Mdm = Session.GetModemObject();

            if (Mdm == null || !(Mdm is RFD.RFD900.RFD900xuxRevN) ||
                !RFD.RFD900.RFD900xux.GetIsCountryLocked(CC = GetCC((RFD.RFD900.RFD900xuxRevN)Mdm)))
            {
                return "--";
            }
            else
            {
                return CC.ToString();
            }
        }

        /// <summary>
        /// Load settings button evt hdlr
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BUT_getcurrent_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Stopwatch SW = new System.Diagnostics.Stopwatch();
            //SW.Start();
            //EndSession();
            var Session = GetSession();

            if (Session == null)
            {
                return;
            }

            _AlreadyInEncCheckChangedEvtHdlr = true;

            ResetAllControls(groupBoxLocal);
            ResetAllControls(groupBoxRemote);
            SetupCBWithDefaultEncryptionOptions(ENCRYPTION_LEVEL);
            SetupCBWithDefaultEncryptionOptions(RENCRYPTION_LEVEL);

            EnableConfigControls(false, false);
            EnableProgrammingControls(false);
            lbl_status.Text = "Connecting";

            //System.Diagnostics.Debug.WriteLine(SW.ElapsedMilliseconds.ToString() + ":  Putting into AT CMD mode");

            try
            {
                if (Session.PutIntoATCommandMode() == RFD.RFD900.TSession.TMode.AT_COMMAND)
                {
                    //System.Diagnostics.Debug.WriteLine(SW.ElapsedMilliseconds.ToString() + ":  Done putting into AT CMD mode");

                    bool SomeSettingsInvalid = false;
                    // cleanup
                    doCommand(Session.Port, "AT&T", false, 1);
                    /*Session.ATCClient.Timeout = 100;
                    Session.ATCClient.DoCommand("AT&T");
                    Session.ATCClient.Timeout = 1000;*/
                    //System.Diagnostics.Debug.WriteLine(SW.ElapsedMilliseconds.ToString() + ":  Done AT&T cmd");

                    Session.Port.DiscardInBuffer();

                    lbl_status.Text = "Doing Command ATI & RTI";

                    //Set the text box to show the radio version
                    int multipoint_fix = -1;    //If this radio has multipoint firmware, the index within returned strings to use for returned values, otherwise -1.
                    var ati_str = doCommand(Session.Port, "ATI").Trim(); //Session.ATCClient.DoQuery("ATI", true);// doCommand(Session.Port, "ATI").Trim();
                    //System.Diagnostics.Debug.WriteLine(SW.ElapsedMilliseconds.ToString() + ":  Done ATI cmd");

                    if (ati_str.StartsWith("["))
                    {
                        multipoint_fix = ati_str.IndexOf(']') + 1;
                    }
                    ATI.Text = ati_str;

                    string LocalFWVer = RFD.RFD900.RFD900.ATIResponseToFWVersion(ati_str);

                    NumberStyles style = NumberStyles.Any;

                    //Get the board frequency.
                    var freqstring = doCommand(Session.Port, "ATI3").Trim(); //Session.ATCClient.DoQuery("ATI3", true);// doCommand(Session.Port, "ATI3").Trim();
                    //System.Diagnostics.Debug.WriteLine(SW.ElapsedMilliseconds.ToString() + ":  Done ATI3 cmd");

                    //Some multipoint firmware versions don't reply to ATI command with [n] at start of reply, but they do for ATI3 command, so check for [n] again...
                    if (multipoint_fix < 0 && freqstring.StartsWith("["))
                    {
                        multipoint_fix = freqstring.IndexOf(']') + 1;
                    }

                    if (multipoint_fix > 0)
                    {
                        freqstring = freqstring.Substring(multipoint_fix).Trim();
                    }

                    if (freqstring.ToLower().Contains('x'))
                        style = NumberStyles.AllowHexSpecifier;

                    var freq =
                        (Uploader.Frequency)
                            Enum.Parse(typeof (Uploader.Frequency),
                                int.Parse(freqstring.ToLower().Replace("x", ""), style).ToString());

                    ATI3.Text = freq.ToString();

                    style = NumberStyles.Any;

                    var boardstring = doCommand(Session.Port, "ATI2").Trim(); //Session.ATCClient.DoQuery("ATI2", true);// doCommand(Session.Port, "ATI2").Trim();
                    //System.Diagnostics.Debug.WriteLine(SW.ElapsedMilliseconds.ToString() + ":  Done ATI2 cmd");

                    if (multipoint_fix > 0)
                    {
                        boardstring = boardstring.Substring(multipoint_fix).Trim();
                    }

                    if (boardstring.ToLower().Contains('x'))
                        style = NumberStyles.AllowHexSpecifier;

                    Session.Board =
                        (Uploader.Board)
                            Enum.Parse(typeof (Uploader.Board),
                                int.Parse(boardstring.ToLower().Replace("x", ""), style).ToString());

                    txtCountry.Text = GetCountryCodeFromSession(Session,
                                    (m) => m.GetCountryCode());

                    ATI2.Text = Session.Board.ToString();

                    if (Session.Board == Uploader.Board.DEVICE_ID_RFD900X)
                    {
                        //RFD900x has a new set of acceptable settings ranges...
                        SERIAL_SPEED.DataSource = new int[] { 1, 2, 4, 9, 19, 38, 57, 115, 230, 460 };
                        RSERIAL_SPEED.DataSource = new int[] { 1, 2, 4, 9, 19, 38, 57, 115, 230, 460 };

                        AIR_SPEED.DataSource = new int[] { 4, 64, 125, 250, 500 };
                        RAIR_SPEED.DataSource = new int[] { 4, 64, 125, 250, 500 };

                        if (ATI.Text.Contains("ASYNC"))
                        {
                            NETID.DataSource = Range(0, 1, 255);
                            RNETID.DataSource = Range(0, 1, 255);
                        }
                        else
                        {
                            NETID.DataSource = Range(0, 1, 65535);
                            RNETID.DataSource = Range(0, 1, 65535);
                        }

                        MIN_FREQ.DataSource = Range(902000, 1000, 927000);
                        RMIN_FREQ.DataSource = Range(902000, 1000, 927000);

                        MAX_FREQ.DataSource = Range(903000, 1000, 928000);
                        RMAX_FREQ.DataSource = Range(903000, 1000, 928000);

                        NUM_CHANNELS.DataSource = Range(1, 1, 50);
                        RNUM_CHANNELS.DataSource = Range(1, 1, 50);

                        MAX_WINDOW.DataSource = Range(20, 1, 400);
                        RMAX_WINDOW.DataSource = Range(20, 1, 400);

                    }
                    else
                    {
                        RestoreAllDefaultCBObjects();
                    }

                    // 8 and 9
                    if (freq == Uploader.Frequency.FREQ_915)
                    {
                        MIN_FREQ.DataSource = Range(902000, 1000, 927000);
                        RMIN_FREQ.DataSource = Range(902000, 1000, 927000);

                        MAX_FREQ.DataSource = Range(903000, 1000, 928000);
                        RMAX_FREQ.DataSource = Range(903000, 1000, 928000);
                    }
                    else if (freq == Uploader.Frequency.FREQ_433)
                    {
                        MIN_FREQ.DataSource = Range(414000, 50, 460000);
                        RMIN_FREQ.DataSource = Range(414000, 50, 460000);

                        MAX_FREQ.DataSource = Range(414000, 50, 460000);
                        RMAX_FREQ.DataSource = Range(414000, 50, 460000);
                    }
                    else if (freq == Uploader.Frequency.FREQ_868)
                    {
                        MIN_FREQ.DataSource = Range(849000, 1000, 889000);
                        RMIN_FREQ.DataSource = Range(849000, 1000, 889000);

                        MAX_FREQ.DataSource = Range(849000, 1000, 889000);
                        RMAX_FREQ.DataSource = Range(849000, 1000, 889000);
                    }

                    if (Session.Board == Uploader.Board.DEVICE_ID_RFD900 ||
                            Session.Board == Uploader.Board.DEVICE_ID_RFD900A
                            || Session.Board == Uploader.Board.DEVICE_ID_RFD900P ||
                            Session.Board == Uploader.Board.DEVICE_ID_RFD900X)
                    {
                        TXPOWER.DataSource = Range(0, 1, 30);
                        RTXPOWER.DataSource = Range(0, 1, 30);
                    }
                    else
                    {
                        TXPOWER.DataSource = Range(0, 1, 20);
                        RTXPOWER.DataSource = Range(0, 1, 20);
                    }

                    if (Session.Board == Uploader.Board.DEVICE_ID_RFD900X)
                    {
                        LBT_RSSI.DataSource = Range(0, 25, 220);
                        RLBT_RSSI.DataSource = Range(0, 25, 220);
                    }
                    else
                    {
                        LBT_RSSI.DataSource = Range(0, 1, 1);
                        RLBT_RSSI.DataSource = Range(0, 1, 1);
                    }

                    if (multipoint_fix == -1)
                    {
                        var AESKey = doCommand(Session.Port, "AT&E?").Trim(); //Session.ATCClient.DoQuery("AT&E?", true);// doCommand(Session.Port, "AT&E?").Trim();
                        if (AESKey.Contains("ERROR"))
                        {
                            txt_aeskey.Text = "";
                            txt_aeskey.Enabled = false;
                        }
                        else
                        {
                            txt_aeskey.Text = AESKey;
                            txt_aeskey.Enabled = true;
                        }
                        SetupComboForMavlink(MAVLINK, false);
                    }
                    else
                    {
                        txt_aeskey.Text = "";
                        txt_aeskey.Enabled = false;
                        SetupComboForMavlink(MAVLINK, true);
                    }

                    RSSI.Text = doCommand(Session.Port, "ATI7").Trim(); //Session.ATCClient.DoQuery("ATI7", true);// doCommand(Session.Port, "ATI7").Trim();
                    //System.Diagnostics.Debug.WriteLine(SW.ElapsedMilliseconds.ToString() + ":  Done ATI7 cmd");


                    lbl_status.Text = "Doing Command ATI5";

                    var answer = doCommand(Session.Port, "ATI5", true); //Session.ATCClient.DoQueryWithMultiLineResponse("ATI5");// doCommand(Session.Port, "ATI5", true);

                    bool Junk;

                    //System.Diagnostics.Debug.WriteLine(SW.ElapsedMilliseconds.ToString() + ":  Parsing local settings");

                    var Settings = Session.GetSettings(false,
                        Session.Board, answer, null, out Junk);

                    DisableRFD900xControls();

                    var items = answer.Split('\n');

                    foreach (var kvp in _DefaultLocalEnabled)
                    {
                        kvp.Key.Enabled = kvp.Value;
                    }

                    _LocalLabelEditorPairs.Reset();

                    if (ATI.Text.Contains("ASYNC"))
                    {
                        _LocalExtraParams.SetModel(Model.ASYNC, Settings);
                    }
                    else
                    {
                        if (ATI.Text.Contains("MP on") && (Session.Board == Uploader.Board.DEVICE_ID_RFD900X))
                        {
                            //This is multipoint firmware.
                            _LocalExtraParams.SetModel(Model.MULTIPOINT_X, Settings);
                        }
                        else if ((items.Length > 0) && items[0].StartsWith("["))
                        {
                            _LocalExtraParams.SetModel(Model.MULTIPOINT, Settings);
                        }
                        else
                        {
                            //This is p2p firmware.
                            _LocalExtraParams.SetModel(Model.P2P, Settings);
                        }
                    }

                    //System.Diagnostics.Debug.WriteLine(SW.ElapsedMilliseconds.ToString() + ":  Done getting info out of local modem");

                    //For each of the settings returned by the radio...
                    SetUpControlsWithValues(groupBoxLocal, false, ModifyReturnedStringsForMultipoint(items, multipoint_fix), Settings);

                    btnRandom.Enabled = GetIsEncryptionEnabled(ENCRYPTION_LEVEL);
                    //System.Diagnostics.Debug.WriteLine(SW.ElapsedMilliseconds.ToString() + ":  Done setting up controls for local modem");

                    // remote
                    foreach (Control ctl in groupBoxRemote.Controls)
                    {
                        if ((ctl.Name != "RSSI")&& (!(ctl is Label)) && !(ctl is Button))
                        {
                            ctl.ResetText();
                        }
                    }

                    Session.Port.DiscardInBuffer();

                    string RTIText = doCommand(Session.Port, "RTI");

                    if ((RTIText.Length < 5) || RTIText.Substring(0, 5) == "ERROR")
                    {
                        RTI.Text = "";
                    }
                    else
                    {
                        RTI.Text = RTIText;
                    }          

                    string RemoteFWVer = RFD.RFD900.RFD900.ATIResponseToFWVersion(RTI.Text);

                    txtRCountry.Text = GetCountryCodeFromSession(Session, (m) => m.GetRemoteCountryCode());

                    if (RTI.Text != "")
                    {

                        try
                        {
                            var resp = doCommand(Session.Port, "RTI2");
                            if (resp.Trim() != "")
                                RTI2.Text =
                                    ((Uploader.Board)Enum.Parse(typeof(Uploader.Board), resp)).ToString();
                        }
                        catch
                        {
                        }

                        if (multipoint_fix == -1)
                        {
                            var AESKey = doCommand(Session.Port, "RT&E?").Trim();
                            if (AESKey.Contains("ERROR"))
                            {
                                txt_Raeskey.Text = "";
                                txt_Raeskey.Enabled = false;
                            }
                            else
                            {
                                txt_Raeskey.Text = AESKey;
                                txt_Raeskey.Enabled = true;
                            }
                            SetupComboForMavlink(RMAVLINK, false);
                        }
                        else
                        {
                            txt_Raeskey.Text = "";
                            txt_Raeskey.Enabled = false;
                            SetupComboForMavlink(RMAVLINK, true);
                        }

                        lbl_status.Text = "Doing Command RTI5";

                        answer = doCommand(Session.Port, "RTI5", true);

                        bool UsedAltRanges;

                        var RemoteSettings = Session.GetSettings(true, Session.Board, answer, (LocalFWVer == RemoteFWVer) ? Settings : null, out UsedAltRanges);

                        if ((RemoteFWVer != null) &&  (LocalFWVer != RemoteFWVer) && UsedAltRanges)
                        {
                            MsgBox.CustomMessageBox.Show("The ranges and options shown for the remote modem may not be accurate.  To ensure accurate, use the same firmware version in both the local and remote modems");
                        }

                        items = answer.Split('\n');
                        _RemoteLabelEditorPairs.Reset();

                        if (RTI.Text.Contains("ASYNC"))
                        {
                            _RemoteExtraParams.SetModel(Model.ASYNC, RemoteSettings);
                        }
                        else
                        {
                            if (RTI.Text.Contains("MP on") && (Session.Board == Uploader.Board.DEVICE_ID_RFD900X))
                            {
                                _RemoteExtraParams.SetModel(Model.MULTIPOINT_X, RemoteSettings);
                            }
                            else if ((items.Length > 0) && items[0].StartsWith("["))
                            {
                                //This is multipoint firmware.
                                _RemoteExtraParams.SetModel(Model.MULTIPOINT, RemoteSettings);
                            }
                            else
                            {
                                //This is 2-point firmware.
                                _RemoteExtraParams.SetModel(Model.P2P, RemoteSettings);
                            }
                        }

                        //System.Diagnostics.Debug.WriteLine(SW.ElapsedMilliseconds.ToString() + ":  Done getting info from remote modem");

                        SomeSettingsInvalid |= SetUpControlsWithValues(groupBoxRemote, true, items, RemoteSettings);

                        //System.Diagnostics.Debug.WriteLine(SW.ElapsedMilliseconds.ToString() + ":  Done setting up controls for remote modem");

                    }

                    // off hook
                    Session.PutIntoTransparentMode();

                    if (SomeSettingsInvalid)
                    {
                        lbl_status.Text = "Done.  Some settings in modem were invalid.";
                    }
                    else
                    {
                        lbl_status.Text = "Done";
                    }
                    EnableConfigControls(true, true);


                }
                else
                {
                    // off hook
                    Session.PutIntoTransparentMode();

                    lbl_status.Text = "Fail";
                    MsgBox.CustomMessageBox.Show("Failed to enter command mode.  Try power-cycling modem.");
                    EnableConfigControls(true, false);
                }

                //BUT_Syncoptions.Enabled = true;

                //BUT_savesettings.Enabled = true;

                //BUT_SetPPMFailSafe.Enabled = true;
                
                EnableProgrammingControls(true);
            }
            catch (Exception ex)
            {
                lbl_status.Text = "Error";
                MsgBox.CustomMessageBox.Show("Error during read " + ex);
            }
            _AlreadyInEncCheckChangedEvtHdlr = false;

            UpdateSetPPMFailSafeButtons();
        }

        void UpdateSetPPMFailSafeButtons()
        {
            BUT_SetPPMFailSafe.Enabled = GPO1_1R_COUT.Enabled && GPO1_1R_COUT.Checked;
            BUT_SetPPMFailSafeRemote.Enabled = RGPO1_1R_COUT.Enabled && RGPO1_1R_COUT.Checked;
        }

        private string Serial_ReadLine(ICommsSerial comPort)
        {
            var sb = new StringBuilder();
            var Deadline = DateTime.Now.AddMilliseconds(comPort.ReadTimeout);

            while (DateTime.Now < Deadline)
            {
                if (comPort.BytesToRead > 0)
                {
                    var data = (byte) comPort.ReadByte();
                    sb.Append((char) data);
                    if (data == '\n')
                        break;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Send a command to the radio, wait for a response.
        /// </summary>
        /// <param name="comPort"></param>
        /// <param name="cmd">The command</param>
        /// <param name="multiLineResponce"></param>
        /// <param name="level"></param>
        /// <returns>The response</returns>
        public string doCommand(ICommsSerial comPort, string cmd, bool multiLineResponce = false, int level = 0)
        {
            if (!comPort.IsOpen)
                return "";

            comPort.DiscardInBuffer();

            lbl_status.Text = "Doing Command " + cmd;
            log.Info("Doing Command " + cmd);
            comPort.ReadTimeout = 1000;

            comPort.Write("\r\n");
            Serial_ReadLine(comPort);
            Thread.Sleep(50);
            comPort.Write(cmd + "\r\n");

            comPort.ReadTimeout = 1000;

            // command echo
            var cmdecho = Serial_ReadLine(comPort);

            if (cmdecho.Contains(cmd))
            {
                var value = "";

                if (multiLineResponce)
                {
                    var deadline = DateTime.Now.AddMilliseconds(1000);
                    while (comPort.BytesToRead > 0 || DateTime.Now < deadline)
                    {
                        try
                        {
                            value = value + Serial_ReadLine(comPort);
                        }
                        catch
                        {
                            value = value + comPort.ReadExisting();
                        }
                    }
                }
                else
                {
                    value = Serial_ReadLine(comPort);

                    if (value == "" && level == 0)
                    {
                        return doCommand(comPort, cmd, multiLineResponce, 1);
                    }
                }

                log.Info(value.Replace('\0', ' '));

                return value;
            }

            comPort.DiscardInBuffer();

            // try again
            if (level == 0)
                return doCommand(comPort, cmd, multiLineResponce, 1);

            return "";
        }

        /// <summary>
        /// Tries to put the radio into AT command mode.
        /// </summary>
        /// <param name="comPort"></param>
        /// <returns></returns>
        public bool doConnect(ICommsSerial comPort)
        {
            try
            {
                Console.WriteLine("doConnect");

                var trys = 1;

                // setup a known enviroment
                comPort.Write("ATO\r\n");

                retry:

                // wait
                Sleep(1500, comPort);
                comPort.DiscardInBuffer();
                // send config string
                comPort.Write("+");
                Sleep(200, comPort);
                comPort.Write("+");
                Sleep(200, comPort);
                comPort.Write("+");
                Sleep(1500, comPort);
                // check for config response "OK"
                log.Info("Connect btr " + comPort.BytesToRead + " baud " + comPort.BaudRate);
                // allow time for data/response

                if (comPort.BytesToRead == 0 && trys <= 3)
                {
                    trys++;
                    log.Info("doConnect retry");
                    goto retry;
                }

                var buffer = new byte[20];
                var len = comPort.Read(buffer, 0, buffer.Length);
                var conn = Encoding.ASCII.GetString(buffer, 0, len);
                log.Info("Connect first response " + conn.Replace('\0', ' ') + " " + conn.Length);
                if (conn.Contains("OK"))
                {
                    //return true;
                }
                else
                {
                    // cleanup incase we are already in cmd mode
                    comPort.Write("\r\n");
                }

                doCommand(comPort, "AT&T", false, 1);

                var version = doCommand(comPort, "ATI");

                log.Info("Connect Version: " + version.Trim() + "\n");

                var regex = new Regex(@"SiK\s+(.*)\s+on\s+(.*)");

                if (regex.IsMatch(version))
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private void BUT_Syncoptions_Click(object sender, EventArgs e)
        {
            RAIR_SPEED.Text = AIR_SPEED.Text;
            RNETID.Text = NETID.Text;
            RECC.Checked = ECC.Checked;
            RMAVLINK.Text = MAVLINK.Text;
            RMIN_FREQ.Text = MIN_FREQ.Text;
            RMAX_FREQ.Text = MAX_FREQ.Text;
            RNUM_CHANNELS.Text = NUM_CHANNELS.Text;
            RMAX_WINDOW.Text = MAX_WINDOW.Text;
            RENCRYPTION_LEVEL.SelectedIndex = ENCRYPTION_LEVEL.SelectedIndex;
            txt_Raeskey.Text = txt_aeskey.Text;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MsgBox.CustomMessageBox.Show(@"The Sik Radios have 2 status LEDs, one red and one green.
green LED blinking - searching for another radio 
green LED solid - link is established with another radio 
red LED flashing - transmitting data 
red LED solid - in firmware update mode");
        }

        void DoCommandShowErrorIfNotOK(ICommsSerial Port, string cmd, string ErrorMsg)
        {
            string Result = doCommand(Port, cmd);
            if (!Result.Contains("OK"))
            {
                MsgBox.CustomMessageBox.Show(ErrorMsg);
            }
        }

        private void BUT_resettodefault_Click(object sender, EventArgs e)
        {
            //EndSession();
            var Session = GetSession();
            if (Session == null)
            {
                return;
            }

            lbl_status.Text = "Connecting";

            if (Session.PutIntoATCommandMode() == RFD.RFD900.TSession.TMode.AT_COMMAND)
            {
                // cleanup
                if (RTI.Text != "")
                {
                    doCommand(Session.Port, "RT&T");

                    Session.Port.DiscardInBuffer();

                    lbl_status.Text = "Doing Command RTI & AT&F";

                    doCommand(Session.Port, "RT&F");

                    doCommand(Session.Port, "RT&W");

                    lbl_status.Text = "Reset";

                    doCommand(Session.Port, "RTZ");

                    doCommand(Session.Port, "RT&T");
                }

                doCommand(Session.Port, "AT&T", false, 1);

                Session.Port.DiscardInBuffer();

                lbl_status.Text = "Doing Command ATI & AT&F";

                DoCommandShowErrorIfNotOK(Session.Port, "AT&F", "Failed to reset parameters to factory defaults");

                DoCommandShowErrorIfNotOK(Session.Port, "AT&W", "Failed to write parameters to EEPROM");

                lbl_status.Text = "Reset";
                doCommand(Session.Port, "ATZ");

                //Session must be ended because modem rebooted.
                Session.PutIntoATCommandModeAssumingInTransparentMode();
            }
            else
            {
                // off hook
                Session.PutIntoTransparentMode();

                lbl_status.Text = "Fail";
                MsgBox.CustomMessageBox.Show("Failed to enter command mode.  Try power-cycling modem.");
            }
        }

        void UpdateStatus(string Status)
        {
            //lblStatus.Text = Status;
            lbl_status.Text = Status;
            Application.DoEvents();
        }

        void UpdateStatusCallback(string Status, double Progress)
        {
            if (Status != null)
            {
                UpdateStatus(Status);
            }
            if (!double.IsNaN(Progress))
            {
                ProgressEvtHdlr(Progress);
            }
        }

        void EnableConfigControls(bool Enable, bool SaveSettingsEnable)
        {
            groupBoxLocal.Enabled = SaveSettingsEnable;
            groupBoxRemote.Enabled = SaveSettingsEnable;
            BUT_Syncoptions.Enabled = Enable;
            BUT_SetPPMFailSafe.Enabled = Enable;
            BUT_getcurrent.Enabled = Enable;
            BUT_savesettings.Enabled = SaveSettingsEnable;
            BUT_resettodefault.Enabled = Enable;
        }
        public static void ResetAllControls(Control form)
        {
            {
                foreach (Control control in form.Controls)
                {
                    control.Enabled = false;
                    if (control is TextBox)
                    {
                        TextBox textBox = (TextBox)control;
                        textBox.Text = null;
                    }

                    if (control is ComboBox)
                    {
                        ComboBox comboBox = (ComboBox)control;
                        if (comboBox.Items.Count > 0)
                            comboBox.SelectedIndex = 0;
                    }

                    if (control is CheckBox)
                    {
                        CheckBox checkBox = (CheckBox)control;
                        checkBox.Checked = false;
                    }

                    if (control is ListBox)
                    {
                        ListBox listBox = (ListBox)control;
                        listBox.ClearSelected();
                    }
                }
            }
        }

        void EnableProgrammingControls(bool Enable)
        {
            BUT_loadcustom.Enabled = Enable;
        }

        void DisableRFD900xControls()
        {
            GPI1_1R_CIN.Enabled = false;
            RGPI1_1R_CIN.Enabled = false;
            GPO1_1R_COUT.Enabled = false;
            RGPO1_1R_COUT.Enabled = false;
        }

        private void BUT_loadcustom_Click(object sender, EventArgs e)
        {
            ProgramFirmware(true);
        }

        void ProgramFirmware(bool Custom)
        {
            EnableProgrammingControls(false);
            EnableConfigControls(false, false);

            try
            {
                //EndSession();
                var Session = GetSession();
                UpdateStatus("Determining mode...");
                var Mode = Session.GetMode();
                UpdateStatus("Mode is " + Mode.ToString());
                //Console.WriteLine("Mode is " + Mode.ToString());
                //port.Close();

                RFD.RFD900.RFD900 RFD900 = _Session.GetModemObject();

                if (RFD900 == null)
                {
                    UpdateStatus("Unknown modem");
                    MsgBox.CustomMessageBox.Show("Couldn't communicate with modem.  Try power-cycling modem.");
                    EndSession();
                }
                else
                {
                    if (Custom)
                    {
                        UpdateStatus("Asking user for firmware file");
                    }
                    else
                    {
                        UpdateStatus("Getting firmware from internet");
                    }
                    if (getFirmware(RFD900.Board, RFD900, Custom))
                    {
                        UpdateStatus("Programming firmware into device");
                        if (RFD900.ProgramFirmware(firmwarefile, UpdateStatusCallback))
                        {
                            UpdateStatus("Programmed firmware into device");
                        }
                        else
                        {
                            UpdateStatus("Programming failed.  (Try again?)");
                        }
                        EndSession();
                    }
                    else
                    {
                        UpdateStatus("Firmware file selection cancelled");
                    }
                }
            }
            catch
            {
                try
                {
                    UpdateStatus("Programming failed.  (Try again?)");
                    EndSession();
                }
                catch
                {
                }
            }
            EnableProgrammingControls(true);
            EnableConfigControls(true, false);
            //UploadFW(true);
        }

        void ProgressEvtHdlr(double Completed)
        {
            try
            {
                Progressbar.Minimum = 0;
                Progressbar.Maximum = 100;
                Progressbar.Value = Math.Min((int)(Completed * 100F), 100);
                Application.DoEvents();
                
            }
            catch
            {
                //Console.WriteLine("Failed");
            }
        }

        private void Progressbar_Click(object sender, EventArgs e)
        {
            beta = !beta;
            MsgBox.CustomMessageBox.Show("Beta set to " + beta);
        }

        private void linkLabel_mavlink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MAVLINK.SelectedValue = 1;
            MAX_WINDOW.Text = 131.ToString();

            RMAVLINK.SelectedValue = 1;
            RMAX_WINDOW.Text = 131.ToString();
        }

        private void linkLabel_lowlatency_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MAVLINK.SelectedValue = 2;
            MAX_WINDOW.Text = 33.ToString();

            RMAVLINK.SelectedValue = 2;
            RMAX_WINDOW.Text = 33.ToString();
        }

        public enum mavlink_option
        {
            RawData = 0,
            Mavlink = 1,
            LowLatency = 2
        }

        private enum mavlink_option_simple
        {
            RawData = 0,
            Mavlink = 1,
        }

        private void txt_aeskey_TextChanged(object sender, EventArgs e)
        {
            var txt = (TextBox)sender;

            string item = txt.Text;
            if (!(Regex.IsMatch(item, "^[0-9a-fA-F]+$")))
            {
                if(item.Length != 0)
                    txt.Text = item.Remove(item.Length - 1, 1);
                txt.SelectionStart = txt.Text.Length;
            }
        }

        private void SetPPMFailSafe(string SetCmd, string SaveCmd)
        {
            lbl_status.Text = "Connecting";
            RFD.RFD900.TSession Session = GetSession();

            if (Session == null)
            {
                return;
            }

            if (Session.PutIntoATCommandMode() == RFD.RFD900.TSession.TMode.AT_COMMAND)
            {
                // cleanup
                //Session.Port.DiscardInBuffer();
                //doCommand(Session.Port, "AT&T", false, 1);

                lbl_status.Text = "Doing Command";

                Session.Port.DiscardInBuffer();
                bool Result = Session.ATCClient.DoCommand(SetCmd);

                Session.Port.DiscardInBuffer();
                Session.ATCClient.DoCommand(SaveCmd);

                // off hook
                //doCommand(Session.Port, "ATO");

                if (Result)
                {
                    lbl_status.Text = "Done";
                }
                else
                {
                    lbl_status.Text = "Fail";
                }
            }
            else
            {
                // off hook
                //doCommand(Session.Port, "ATO");

                lbl_status.Text = "Fail";
                MsgBox.CustomMessageBox.Show("Failed to enter command mode");
            }
        }


        private void BUT_SetPPMFailSafe_Click(object sender, EventArgs e)
        {
            SetPPMFailSafe("AT&R", "AT&W");
        }

        RFD.RFD900.TSession GetSession()
        {
            if (_Session == null)
            {
                try
                {
                    if (SikRadio.Config.comPort != null)
                    {
                        _Session = new RFD.RFD900.TSession(SikRadio.Config.comPort, MainV2.comPort.BaseStream.BaudRate);
                    }
                }
                catch
                {
                    MsgBox.CustomMessageBox.Show("Invalid ComPort or in use");
                    return null;
                }
            }
            else if (_Session.Port.BaudRate != MainV2.comPort.BaseStream.BaudRate ||
                (MainV2.comPort.BaseStream.PortName != "TCP" && (_Session.Port.PortName != MainV2.comPort.BaseStream.PortName)))
            {
                _Session.Dispose();
                _Session = null;
                GetSession();
            }
            return _Session;
        }

        private void EndSession()
        {
            if (_Session != null)
            {
                _Session.Dispose();
                _Session = null;
            }
            if (DoDisconnectReconnect != null)
            {
                DoDisconnectReconnect();
            }
        }

        bool SetSetting(string Designator, int Value, bool Remote)
        {
            var Session = GetSession();

            if (Session == null)
            {
                return false;
            }
            else
            {
                var answer = doCommand(Session.Port, (Remote ? "RT" : "AT")+Designator+"="+Value.ToString(), false);
                return answer.Contains("OK");
            }
        }

        /// <summary>
        /// Handles a change of the local encryption level check box.
        /// </summary>
        /// <param name="sender">ignored</param>
        /// <param name="e">ignored</param>
        private void ENCRYPTION_LEVEL_CheckedChanged(object sender, EventArgs e)
        {
            if (ENCRYPTION_LEVEL.Enabled)
            {
                EncryptionCheckChangedEvtHdlr(ENCRYPTION_LEVEL, "ATI5", "AT&E?", txt_aeskey, false, "ATI5");
            }
            btnRandom.Enabled = GetIsEncryptionEnabled(ENCRYPTION_LEVEL);
        }

        /// <summary>
        /// Handles a change of the remote encryption level check box.
        /// </summary>
        /// <param name="sender">ignored</param>
        /// <param name="e">ignored</param>
        private void RENCRYPTION_LEVEL_CheckedChanged(object sender, EventArgs e)
        {
            if (RENCRYPTION_LEVEL.Enabled)
            {
                EncryptionCheckChangedEvtHdlr(RENCRYPTION_LEVEL, "RTI5", "RT&E?", txt_Raeskey, true, "RTI5");
            }
        }

        bool _AlreadyInEncCheckChangedEvtHdlr = false;

        string RemoveMultiPointLocalNodeID(string ATCReply)
        {
            ATCReply = ATCReply.Trim();

            if ((ATCReply.Length > 1) && (ATCReply[0] == '[') && ATCReply.Contains(']'))
            {
                int StartIndex = ATCReply.IndexOf(']') + 1;

                return ATCReply.Substring(StartIndex);
            }
            else
            {
                return ATCReply;
            }
        }

        /// <summary>
        /// Handles a change of an encryption level check box.
        /// </summary>
        /// <param name="CB">The checkbox which was changed.  Must not be null.</param>
        /// <param name="ATCommand">The AT command to use to get the settings
        /// from the relevant modem.  Must not be null.</param>
        void EncryptionCheckChangedEvtHdlr(ComboBox CB, string ATCommand, string EncKeyQuery,
            TextBox EncKeyTextBox, bool Remote, string ATI5Command)
        {
            if (_AlreadyInEncCheckChangedEvtHdlr)
            {
                return;
            }
            _AlreadyInEncCheckChangedEvtHdlr = true;
            try
            {
                //Write setting to radio now.
                var Session = GetSession();

                if (Session == null)
                {
                    return;
                }
                Session.PutIntoATCommandMode();
                var ATI5answer = doCommand(Session.Port, ATI5Command, true);

                bool Junk;

                var Settings = Session.GetSettings(Remote, Session.Board, ATI5answer, null, out Junk);
                if (Settings.ContainsKey("ENCRYPTION_LEVEL"))
                {
                    var Setting = Settings["ENCRYPTION_LEVEL"];
                    if (!SetSetting(Setting.Designator, GetEncryptionLevelValue(CB), Remote))
                    {
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Something wrong here");
                }

                //Read AES key back out of modem and display it.  
                //BUT_getcurrent_Click(this, null);
                //txt_aeskey.Text = doCommand(Session.Port, "AT&E?").Trim();
                EncKeyTextBox.Text = RemoveMultiPointLocalNodeID(doCommand(Session.Port, EncKeyQuery).Trim());
                lbl_status.Text = "Done.";
            }
            finally
            {
                _AlreadyInEncCheckChangedEvtHdlr = false;
            }
        }

        /// <summary>
        /// Get a random key as a hex numeral string, with the given QTY of hex numerals.
        /// </summary>
        /// <param name="QTYHexDigits">The QTY of hex numerals.</param>
        /// <returns>The key.  Never null.</returns>
        string GetRandomKey(int QTYHexDigits)
        {
            string Result = "";

            System.Random R = new Random((int)(System.DateTime.Now.Ticks & 0xFFFFFFFF));

            for (; QTYHexDigits > 0; QTYHexDigits--)
            {
                Result += (((UInt32)R.Next()) & 0xF).ToString("X1");
            }

            return Result;
        }

        private void btnRandom_Click(object sender, EventArgs e)
        {
            txt_aeskey.Text = GetRandomKey(GetEncryptionMaxKeyLength(ENCRYPTION_LEVEL));
            txt_Raeskey.Text = txt_aeskey.Text;
        }

        private void btnCommsLog_Click(object sender, EventArgs e)
        {
            
        }

        private void BUT_SetPPMFailSafeRemote_Click(object sender, EventArgs e)
        {
            SetPPMFailSafe("RT&R", "RT&W");
        }

        public string Header
        {
            get
            {
                return "Settings";
            }
        }

    }
}