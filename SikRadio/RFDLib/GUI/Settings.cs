using System;
using System.Collections.Generic;
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

    /// <summary>
    /// A TLabelEditorPair which remembers the original name and label.
    /// </summary>
    public class TOrigLabelEditorPair : TLabelEditorPair
    {
        public readonly string OriginalLabel;
        public readonly string OriginalName;
        public readonly bool OrigVisible;
        public readonly ToolTip OrigToolTip;
        public readonly string OrigToolTipText;

        public TOrigLabelEditorPair(Label L, Control Editor, ToolTip TT)
            : base(L, Editor)
        {
            this.OriginalLabel = L.Text;
            this.OriginalName = Editor.Name;
            this.OrigVisible = Editor.Visible;
            this.OrigToolTip = TT;
            this.OrigToolTipText = TT.GetToolTip(Editor);
        }

        /// <summary>
        /// Reset to original values.
        /// </summary>
        public void Reset()
        {
            L.Text = OriginalLabel;
            Editor.Name = OriginalName;
            L.Visible = OrigVisible;
            Editor.Visible = OrigVisible;
            OrigToolTip.SetToolTip(Editor, OrigToolTipText);
        }
    }

    /// <summary>
    /// This is for dynamically re-using spare label-editor pairs for which there is no setting in the modem.
    /// </summary>
    public class TLabelEditorPairRegister
    {
        Dictionary<string, TOrigLabelEditorPair> _Pairs = new Dictionary<string, TOrigLabelEditorPair>();
        Dictionary<string, TOrigLabelEditorPair> _Spare = new Dictionary<string, TOrigLabelEditorPair>();

        /// <summary>
        /// Add a label-editor pair.
        /// </summary>
        /// <param name="Pair"></param>
        public void Add(TOrigLabelEditorPair Pair)
        {
            _Pairs[Pair.OriginalName] = Pair;
        }

        /// <summary>
        /// Add a label-editor pair.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="Editor"></param>
        public void Add(Label L, Control Editor, ToolTip TT)
        {
            Add(new TOrigLabelEditorPair(L, Editor, TT));
        }

        /// <summary>
        /// Reset all label-editor pairs to their original labels and names.
        /// </summary>
        public void Reset()
        {
            foreach (var P in _Pairs.Values)
            {
                P.Reset();
            }

            _Spare.Clear();
        }

        /// <summary>
        /// Call this when the list of setting names has been read from the modem, so the spare
        /// label-editor pairs can be put into a separate list.
        /// </summary>
        /// <param name="SettingNamesSet">The list of setting names read from the modem.  Must not be null.</param>
        public void SetUp(List<string> SettingNamesSet)
        {
            foreach (var kvp in _Pairs)
            {
                if (!SettingNamesSet.Contains(kvp.Value.Editor.Name))
                {
                    _Spare[kvp.Key] = kvp.Value;
                }
            }
        }

        /// <summary>
        /// Get a spare editor control for an unexpected setting read from the modem.  Set the editor's label
        /// to the setting name, if a spare editor was found.
        /// </summary>
        /// <param name="SettingName">The setting name.  Must not be null.</param>
        /// <param name="Condition">The condition the editor control must meet.  Must not be null.</param>
        /// <returns></returns>
        Control GetSpare(string SettingName, Func<Control, bool> Condition, string Description)
        {
            TOrigLabelEditorPair Pair = null;

            foreach (var kvp in _Spare)
            {
                if (Condition(kvp.Value.Editor))
                {
                    //Assign to a variable and break, instead of processing here, because
                    //part of the processing is removing it from _Spare, but _Spare is being iterated here.
                    Pair = kvp.Value;
                    break;
                }
            }

            if (Pair == null)
            {
                return null;
            }
            else
            {
                Pair.Editor.Name = SettingName;
                Pair.L.Text = Description;
                if (Pair.L.Bottom > Pair.Editor.Top)
                {
                    while (Pair.L.Width > (Pair.Editor.Left - Pair.L.Left) && Pair.L.Font.Size > 1)
                    {
                        Pair.L.Font = new System.Drawing.Font(Pair.L.Font.FontFamily, Pair.L.Font.Size - 1);
                    }
                }
                Pair.L.Visible = true;
                Pair.Editor.Visible = true;
                Pair.OrigToolTip.SetToolTip(Pair.Editor, Description);
                _Spare.Remove(Pair.OriginalName);
                return Pair.Editor;
            }
        }

        /// <summary>
        /// Return a spare combobox if there is one, otherwise return null.
        /// </summary>
        /// <param name="SettingName"></param>
        /// <returns></returns>
        public ComboBox GetSpareComboBox(string SettingName, string Description)
        {
            return (ComboBox)GetSpare(SettingName, (C) => C is ComboBox, Description);
        }

        /// <summary>
        /// Return a spare checkbox if there is one, otherwise return null.
        /// </summary>
        /// <param name="SettingName"></param>
        /// <returns></returns>
        public CheckBox GetSpareCheckbox(string SettingName, string Description)
        {
            return (CheckBox)GetSpare(SettingName, (C) => C is CheckBox, Description);
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