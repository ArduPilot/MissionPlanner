using System;

namespace RFDLib.Config.GUI
{
    public class TGUIController
    {
        RFDLib.GUI.ConfigArray _Array;

        public TGUIController(RFDLib.GUI.ConfigArray Array)
        {
            _Array = Array;
        }
        
        public void AddSetting(RFDLib.Config.ISetting Setting)
        {
            if (Setting is RFDLib.Config.TSetting<int>)
            {
                AddIntSetting((RFDLib.Config.TSetting<int>)Setting);
            }
            else if (Setting is RFDLib.Config.TSetting<bool>)
            {
                AddBoolSetting((RFDLib.Config.TSetting<bool>)Setting);
            }
        }

        void AddButtonStringSetting(RFDLib.Config.TSetting<string> Setting)
        {
            //var TextBoxItem = _Array.AddTextItem
        }

        void AddIntSetting(RFDLib.Config.TSetting<int> Setting)
        {
            var TextItem = _Array.AddTextItem();

            TextItem.Text.Text = Setting.Value.ToString();
            TextItem.Tag = Setting;
        }

        void AddBoolSetting(RFDLib.Config.TSetting<bool> Setting)
        {
            var CheckBoxItem = _Array.AddCheckBoxItem();

            CheckBoxItem.Text.Text = Setting.GetDescriptor().Name;
            CheckBoxItem.Tag = Setting;
            CheckBoxItem.CB.Checked = Setting.Value;
        }

        void AddMultiChoiceSetting(RFDLib.Config.ISetting Setting)
        {
            RFDLib.Config.TMultiChoiceSettingDescriptor Desc = 
                (RFDLib.Config.TMultiChoiceSettingDescriptor)Setting.GetDescriptor();

            var ComboItem = _Array.AddComboItem();
            ComboItem.Text.Text = Setting.GetDescriptor().Name;
            ComboItem.Tag = Setting;
            foreach (var Option in Desc.GetOptions())
            {
                ComboItem.CB.Items.Add(Option);
                if (Setting.ToString() == Option.ToString())
                {
                    ComboItem.CB.SelectedItem = Option;
                }
            }
        }
    }

}