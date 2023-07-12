using System;
using System.Windows.Forms;

namespace AltitudeAngelWings.Plugin
{
    internal partial class WaitPanel : UserControl
    {
        public WaitPanel()
        {
            InitializeComponent();
            _btnCancel.UseWaitCursor = false;
            _btnCancel.Focus();
        }

        public event EventHandler<EventArgs> CancelClick;

        public string Operation
        {
            get => _lblOperation.Text;
            set => _lblOperation.Text = value;
        }

        protected override void OnLoad(EventArgs e)
        {
            SuspendLayout();
            _panel.Left = ClientSize.Width / 2 - _panel.Width / 2;
            _panel.Top = ClientSize.Height / 2 - _panel.Height / 2 - (Height - ClientSize.Height);
            ResumeLayout();
            base.OnLoad(e);
        }

        private void _btnCancel_Click(object sender, EventArgs e)
        {
            _btnCancel.Enabled = false;
            _btnCancel.Text = "Cancelling...";
            CancelClick?.Invoke(sender, e);
        }
    }
}
