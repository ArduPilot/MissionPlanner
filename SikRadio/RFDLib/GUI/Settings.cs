using System;
using System.Windows.Forms;

namespace RFDLib.GUI.Settings
{
    public class TLabelEditorPair
    {
        public readonly Label L;
        public readonly Control Editor;

        public TLabelEditorPair(Label L, Control Editor)
        {
            this.L = L;
            this.Editor = Editor;
        }

        public virtual string[] SettingNames
        {
            get
            {
                return new string[] { Editor.Name };
            }
        }
    }

    public class TDynamicLabelEditorPair : TLabelEditorPair
    {
        public readonly TSettingNameLabelTextPair[] AlternateNames;

        public TDynamicLabelEditorPair(Label L, Control Editor, TSettingNameLabelTextPair[] AlternateNames)
            : base(L, Editor)
        {
            this.AlternateNames = AlternateNames;
        }

        public override string[] SettingNames
        {
            get
            {
                string[] Result = new string[AlternateNames.Length];

                for (int n = 0; n < AlternateNames.Length; n++)
                {
                    Result[n] = AlternateNames[n].SettingName;
                }

                return Result;
            }
        }

        public bool SetUpForSettingName(string SettingName)
        {
            foreach (var SNLTP in AlternateNames)
            {
                if (SNLTP.SettingName == SettingName)
                {
                    L.Text = SNLTP.LabelText;
                    return true;
                }
            }

            return false;
        }

        public class TSettingNameLabelTextPair
        {
            public readonly string SettingName;
            public readonly string LabelText;

            public TSettingNameLabelTextPair(string SettingName, string LabelText)
            {
                this.SettingName = SettingName;
                this.LabelText = LabelText;
            }
        }
    }

    public class TDynamicLabelEditorPairRegister
    {
        public readonly TDynamicLabelEditorPair[] Pairs;

        public TDynamicLabelEditorPairRegister(TDynamicLabelEditorPair[] Pairs)
        {
            this.Pairs = Pairs;
        }

        /// <summary>
        /// Try to find an editor control for the setting of the given name, and if found, set up its label.
        /// Returns the editor control if successful, otherwise returns null.
        /// </summary>
        /// <param name="SettingName">The setting name.  Must not be null.</param>
        /// <returns>Returns the editor control if successful, otherwise returns null.</returns>
        public Control FindAndSetUpEditorWithSettingName(string SettingName)
        {
            foreach (var Pair in Pairs)
            {
                if (Pair.SetUpForSettingName(SettingName))
                {
                    return Pair.Editor;
                }
            }
            return null;
        }
    }
}