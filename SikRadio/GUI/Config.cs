using System;
using System.Collections.Generic;

namespace RFD900Tools.GUI
{

    public class TConfig
    {
        RFDLib.GUI.ConfigArray _GUI;
        Dictionary<string, TParamInfo> _ParamInfo = new Dictionary<string, TParamInfo>();

        public TConfig(RFDLib.GUI.ConfigArray GUI)
        {
            _GUI = GUI;

            _ParamInfo["FORMAT"] = new TParamInfo("Format", false, false);
            _ParamInfo["RESERVED"] = new TParamInfo("Reserved", false, false);
            _ParamInfo["RSV"] = new TParamInfo("Reserved", false, false);
            _ParamInfo["RSVD"] = new TParamInfo("Reserved", false, false);
            _ParamInfo["MAVLINK"] = new TParamInfo("Mavlink", 
                EnumTypeToComboOptions(typeof(MissionPlanner.Radio.Sikradio.mavlink_option)));
            _ParamInfo["AIR_SPEED"] = new TParamInfo("Air Speed",
                IntsToComboOptions(new int[] { 250, 192, 128, 96, 64, 48, 32, 24, 19, 16, 8, 4, 2 }));
            _ParamInfo["SERIAL_SPEED"] = new TParamInfo("Baud",
                IntsToComboOptions(new int[] { 115, 111, 57, 38, 19, 9, 4, 2, 1 }));
            _ParamInfo["NETID"] = new TParamInfo("Net ID", IntRangeToComboOptions(1, 30));
        }

        TComboOption[] EnumTypeToComboOptions(Type EnumType)
        {
            return RFDLib.Array.CherryPickArray(RFDLib.Utils.EnumToStrings(EnumType),
                (kvp) => new TComboOption(kvp.Key, kvp.Value));
        }

        TComboOption[] IntsToComboOptions(int[] Ints)
        {
            return RFDLib.Array.CherryPickArray(Ints, (i) => new TComboOption(i.ToString(), i));
        }

        TComboOption[] IntRangeToComboOptions(int First, int Last)
        {
            TComboOption[] Result = new TComboOption[Last - First + 1];

            for (int n = First; n <= Last; n++)
            {
                Result[n - First] = new TComboOption(n.ToString(), n);
            }

            return Result;
        }

        bool IsSettingBoolean(Dictionary<string, RFD.RFD900.TSetting> Settings, string Designator)
        {
            if (Settings.ContainsKey(Designator))
            {
                var S = Settings[Designator];

                if (S.Options != null)
                {
                    if (S.Options.Length == 2)
                    {
                        if (S.Options[0].Value == 0 && S.Options[1].Value == 1)
                        {
                            return true;
                        }
                    }
                }
                if (S.Range != null)
                {
                    var RO = S.Range.GetOptions();

                    if (RO.Length == 2)
                    {
                        if (RO[0] == 0 && RO[1] == 1)
                        {
                            return true;
                        }
                    }
                }
            }

            return _ParamInfo.ContainsKey(Designator) && _ParamInfo[Designator].DefaultToBoolean;
        }

        bool IsSettingTextAndButton(Dictionary<string, RFD.RFD900.TSetting> Settings, string Designator)
        {
            return false;
        }

        TComboOption[] GetComboOptions(Dictionary<string, RFD.RFD900.TSetting> Settings, string Designator)
        {
            if (Settings.ContainsKey(Designator) && Settings[Designator].Options != null)
            {
                return RFDLib.Array.CherryPickArray(Settings[Designator].Options, (o) => new TComboOption(o.OptionName, o.Value));
            }
            else
            {
                return _ParamInfo.ContainsKey(Designator) ? _ParamInfo[Designator].DefaultComboOptions : null;
            }
        }

        TComboOption FindOptionWithValue(TComboOption[] Options, int Value)
        {
            foreach (var o in Options)
            {
                if (o.Value == Value)
                {
                    return o;
                }
            }

            return null;
        }

        TComboOption GetSelected(TComboOption[] Options, Dictionary<string, RFD.RFD900.TSetting> Settings,
            string Designator, string Value)
        {
            //if (Designator.Contains("MAVLINK") && !(Settings.ContainsKey("MAVLINK") && (Settings["MAVLINK"].Options != null))) //
            {
                int n;
                TComboOption Option;

                if (int.TryParse(Value, out n) && (Option = FindOptionWithValue(Options, n)) != null)
                {
                    return Option;
                }
                else
                {
                    return Options[0];
                }
            }
        }

        string GetBetterTextDesc(string Designator)
        {
            return _ParamInfo.ContainsKey(Designator) ? _ParamInfo[Designator].Description : Designator;
        }

        public bool Update(Dictionary<string, RFD.RFD900.TSetting> Settings, string[] items)
        {
            _GUI.ClearItems();

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
                    //values[1] = values[1].Replace("/", "_");

                    //var control = FindControlInGroupBox(GB, (Remote ? "R" : "") + values[1].Trim());
                    var Designator = values[1].Trim();
                    var Value = values[2].Trim();

                    //if (control != null)
                    if (!_ParamInfo.ContainsKey(Designator) || _ParamInfo[Designator].Show)
                    {
                        //GB.Enabled = true;
                        //control.Parent.Enabled = true;
                        //control.Enabled = true;
                        TComboOption[] ComboOptions;

                        if (IsSettingBoolean(Settings, Designator))
                        {
                            var CB = _GUI.AddCheckBoxItem();
                            CB.Tag = Designator;
                            CB.Text.Text = GetBetterTextDesc(Designator);
                            CB.CB.Checked = Value == "1";
                        }
                        else if (IsSettingTextAndButton(Settings, Designator))
                        {
                            var TAB = _GUI.AddTextAndButtonItem();
                            TAB.Tag = Designator;
                            TAB.Text.Text = GetBetterTextDesc(Designator);
                            TAB.TB.Text = Value;
                            TAB.B.Text = "Random";
                        }
                        else if ((ComboOptions = GetComboOptions(Settings, Designator)) != null)
                        {
                            var cmb = _GUI.AddComboItem();
                            cmb.Tag = Designator;
                            cmb.Text.Text = GetBetterTextDesc(Designator);
                            cmb.CB.Items.AddRange(RFDLib.Array.CherryPickArray<TComboOption, object>(ComboOptions, (CO) => CO));
                            cmb.CB.SelectedItem = GetSelected(ComboOptions, Settings, Designator, Value);
                        }
                    }
                }
                else
                {
                    //log.Info("Odd config line :" + item);
                }
            }

            return SomeSettingsInvalid;
        }

        class TComboOption
        {
            string _Description;
            public readonly int Value;

            public TComboOption(string Description, int Value)
            {
                _Description = Description;
                this.Value = Value;
            }

            public override string ToString()
            {
                return _Description;
            }
        }

        class TParamInfo
        {
            public readonly string Description;
            public readonly TComboOption[] DefaultComboOptions;
            public readonly bool Show = true;
            public readonly bool DefaultToBoolean = false;

            public TParamInfo(string Description)
            {
                this.Description = Description;
            }

            public TParamInfo(string Description, bool DefaultToBoolean)
                : this(Description)
            {
                this.DefaultToBoolean = DefaultToBoolean;
            }

            public TParamInfo(string Description, bool DefaultToBoolean, bool Show)
                : this(Description, DefaultToBoolean)
            {
                this.Show = Show;
            }

            public TParamInfo(string Description, TComboOption[] DefaultComboOptions)
                : this(Description)
            {
                this.DefaultComboOptions = DefaultComboOptions;
            }
        }
    }
}
