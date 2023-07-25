using System;
using System.Windows.Forms;
using Exception = System.Exception;

namespace AltitudeAngelWings.Plugin
{
    internal partial class WaitPanel : UserControl
    {
        private Exception _exception = null;

        public WaitPanel()
        {
            InitializeComponent();
            _btnCancel.UseWaitCursor = false;
            _btnCancel.Focus();
        }

        public event EventHandler<EventArgs> CancelClick;
        public event EventHandler<EventArgs> OkClick;

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
            if (_exception == null)
            {
                _btnCancel.Enabled = false;
                _btnCancel.Text = "Cancelling...";
                CancelClick?.Invoke(sender, e);
            }
            else
            {
                OkClick?.Invoke(sender, e);
            }
        }

        public Exception Exception
        {
            get => _exception;
            set
            {
                _exception = value;
                ShowException();
            }
        }

        private void ShowException()
        {
            _picLogo.Visible = false;
            _lblOperation.Top = _picLogo.Top;
            _lblOperation.Height = _btnCancel.Top;
            _lblOperation.Text = _exception.ToDisplayedException();
            _btnCancel.Enabled = true;
            _btnCancel.Text = "OK";
        }
    }
}
