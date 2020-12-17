using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RFDLib.GUI
{
    /// <summary>
    /// A GUI user control which is an array of settings controls.
    /// </summary>
    public partial class ConfigArray : UserControl
    {
        List<TItem> _Items = new List<TItem>();
        int _RowHeight = 30;

        public ConfigArray()
        {
            InitializeComponent();
        }

        public void ClearItems()
        {
            foreach (var I in _Items)
            {
                I.RemoveFromControls(Controls);
            }
            _Items.Clear();
        }

        public TCheckBoxItem AddCheckBoxItem()
        {
            int Top = (_Items.Count) * _RowHeight;
            Label L = new Label();
            L.AutoSize = false;
            L.Top = Top;
            L.Left = 0;
            L.Width = this.Width / 2;
            L.Height = _RowHeight;
            L.TextAlign = ContentAlignment.MiddleRight;
            L.BackColor = ((_Items.Count & 0x01) != 0) ? Color.White : Color.LightGray;
            Controls.Add(L);
            CheckBox PB = new CheckBox();
            PB.Top = Top;
            PB.Left = L.Width;
            PB.Width = this.Width / 2;
            PB.Height = _RowHeight;
            PB.BackColor = L.BackColor;
            Controls.Add(PB);
            TCheckBoxItem I = new TCheckBoxItem(L, PB);
            _Items.Add(I);
            return I;
        }

        public TComboItem AddComboItem()
        {
            int Top = (_Items.Count) * _RowHeight;
            Label L = new Label();
            L.AutoSize = false;
            L.Top = Top;
            L.Left = 0;
            L.Width = this.Width / 2;
            L.Height = _RowHeight;
            L.TextAlign = ContentAlignment.MiddleRight;
            L.BackColor = ((_Items.Count & 0x01) != 0) ? Color.White : Color.LightGray;
            Controls.Add(L);
            CBWithBackground CBWB = new CBWithBackground();
            ComboBox PB = CBWB.CMB;
            CBWB.Top = Top;
            CBWB.Left = L.Width;
            CBWB.Width = this.Width / 2;
            CBWB.Height = _RowHeight;
            PB.DropDownStyle = ComboBoxStyle.DropDownList;
            CBWB.BackColor = L.BackColor;
            Controls.Add(CBWB);
            TComboItem I = new TComboItem(L, CBWB);
            _Items.Add(I);
            return I;
        }

        public TTextItem AddTextItem()
        {
            int Top = (_Items.Count) * _RowHeight;
            Label L = new Label();
            L.AutoSize = false;
            L.Top = Top;
            L.Left = 0;
            L.Width = this.Width / 2;
            L.Height = _RowHeight;
            L.TextAlign = ContentAlignment.MiddleRight;
            L.BackColor = ((_Items.Count & 0x01) != 0) ? Color.White : Color.LightGray;
            Controls.Add(L);
            TextBox TB = new TextBox();
            TB.Top = Top;
            TB.Left = L.Width;
            TB.Width = this.Width / 2;
            TB.Height = _RowHeight;
            TB.BackColor = L.BackColor;
            Controls.Add(TB);
            TTextItem I = new TTextItem(L, TB);
            _Items.Add(I);
            return I;
        }

        public TTextAndButtonItem AddTextAndButtonItem()
        {
            int Top = (_Items.Count) * _RowHeight;
            Label L = new Label();
            L.AutoSize = false;
            L.Top = Top;
            L.Left = 0;
            L.Width = this.Width / 2;
            L.Height = _RowHeight;
            L.TextAlign = ContentAlignment.MiddleRight;
            L.BackColor = ((_Items.Count & 0x01) != 0) ? Color.White : Color.LightGray;
            Controls.Add(L);
            TextBox TB = new TextBox();
            TB.Top = Top;
            TB.Left = L.Width;
            TB.Width = this.Width / 4;
            TB.Height = _RowHeight;
            TB.BackColor = L.BackColor;
            Controls.Add(TB);
            Button B = new Button();
            B.Top = Top;
            B.Left = TB.Right + 1;
            B.Width = this.Width / 4;
            B.Height = _RowHeight;
            B.BackColor = L.BackColor;
            Controls.Add(B);
            TTextAndButtonItem I = new TTextAndButtonItem(L, TB, B);
            _Items.Add(I);
            return I;
        }

        public TFilePathItem AddFilePathItem(string FilterPattern, bool Save)
        {
            int Top = (_Items.Count) * _RowHeight;
            Label L = new Label();
            L.AutoSize = false;
            L.Top = Top;
            L.Left = 0;
            L.Width = this.Width / 2;
            L.Height = _RowHeight;
            L.TextAlign = ContentAlignment.MiddleRight;
            L.BackColor = ((_Items.Count & 0x01) != 0) ? Color.White : Color.LightGray;
            Controls.Add(L);
            TextBox TB = new TextBox();
            TB.Top = Top;
            TB.Left = L.Width;
            TB.Width = this.Width / 3;
            TB.Height = _RowHeight;
            TB.BackColor = L.BackColor;
            Controls.Add(TB);
            Button B = new Button();
            B.Text = "Browse...";
            B.Top = Top;
            B.Left = TB.Right + 1;
            B.Width = this.Width / 6;
            B.Height = _RowHeight;
            B.BackColor = L.BackColor;
            Controls.Add(B);
            TFilePathItem I = new TFilePathItem(L, TB, B, Save ? (FileDialog)dlgSave : (FileDialog)dlgOpen, FilterPattern);
            _Items.Add(I);
            return I;
        }

        public TItem GetItem(int Index)
        {
            if (_Items.Count >= Index)
            {
                return null;
            }
            else
            {
                return _Items[Index];
            }
        }

        public List<TItem> Items
        {
            get
            {
                return _Items;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            foreach (var i in _Items)
            {
                i.Resize(this);
            }
        }

        public class TItem
        {
            Label _L;
            public object Tag;

            public TItem(Label L)
            {
                _L = L;
            }

            public Label Text
            {
                get
                {
                    return _L;
                }
            }

            public virtual void RemoveFromControls(ControlCollection CC)
            {
                CC.Remove(_L);
            }

            public virtual void Resize(Control Container)
            {
                Text.Width = Container.Width / 2;
            }
        }

        public class TTextItem : TItem
        {
            TextBox _TB;

            public TTextItem(Label L, TextBox TB)
                : base(L)
            {
                _TB = TB;
            }

            public TextBox TB
            {
                get
                {
                    return _TB;
                }
            }

            public override void RemoveFromControls(ControlCollection CC)
            {
                base.RemoveFromControls(CC);
                CC.Remove(_TB);
            }

            public override void Resize(Control Container)
            {
                base.Resize(Container);
                TB.Width = Container.Width / 2;
                TB.Left = Text.Width;
            }
        }

        public class TTextAndButtonItem : TTextItem
        {
            Button _Button;

            public TTextAndButtonItem(Label L, TextBox TB, Button B)
                : base(L, TB)
            {
                _Button = B;
            }

            public Button B
            {
                get
                {
                    return _Button;
                }
            }

            public override void RemoveFromControls(ControlCollection CC)
            {
                base.RemoveFromControls(CC);
                CC.Remove(_Button);
            }

            public override void Resize(Control Container)
            {
                base.Resize(Container);
                TB.Width = Container.Width / 4;
                TB.Left = Text.Width;
                B.Width = Container.Width / 4;
                B.Left = TB.Right + 1;
            }
        }

        public class TFilePathItem : TTextItem
        {
            Button _B;
            FileDialog _dlgOpen;
            string _FilterPattern;

            public TFilePathItem(Label L, TextBox TB, Button B, FileDialog dlgOpen, string FilterPattern)
                : base(L, TB)
            {
                _B = B;
                _B.Click += BClickEvtHdlr;
                _dlgOpen = dlgOpen;
                _FilterPattern = FilterPattern;
            }

            void BClickEvtHdlr(object sender, EventArgs e)
            {
                _dlgOpen.FileName = TB.Text;
                _dlgOpen.Filter = _FilterPattern;
                if (_dlgOpen.ShowDialog() == DialogResult.OK)
                {
                    TB.Text = _dlgOpen.FileName;
                }
            }

            public override void RemoveFromControls(ControlCollection CC)
            {
                base.RemoveFromControls(CC);
                CC.Remove(_B);
            }

            public override void Resize(Control Container)
            {
                base.Resize(Container);
                TB.Width = Container.Width / 3;
                TB.Left = Text.Width;
                _B.Width = Container.Width / 6;
                _B.Left = TB.Right + 1;
            }
        }

        public class TComboItem : TItem
        {
            ComboBox _CB;
            CBWithBackground _CBWB;

            public TComboItem(Label L, CBWithBackground CBWB)
                : base(L)
            {
                _CBWB = CBWB;
            }

            public ComboBox CB
            {
                get
                {
                    return _CBWB.CMB;
                }
            }

            public override void RemoveFromControls(ControlCollection CC)
            {
                base.RemoveFromControls(CC);
                CC.Remove(_CBWB);
            }

            public override void Resize(Control Container)
            {
                base.Resize(Container);
                _CBWB.Width = Container.Width / 2;
                _CBWB.Left = Text.Width;
            }
        }

        public class TCheckBoxItem : TItem
        {
            CheckBox _CB;

            public TCheckBoxItem(Label L, CheckBox CB)
                : base(L)
            {
                _CB = CB;
            }

            public CheckBox CB
            {
                get
                {
                    return _CB;
                }
            }

            public override void RemoveFromControls(ControlCollection CC)
            {
                base.RemoveFromControls(CC);
                CC.Remove(_CB);
            }

            public override void Resize(Control Container)
            {
                base.Resize(Container);
                CB.Width = Container.Width / 2;
                CB.Left = Text.Width;
            }
        }
    }
}
