using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AltitudeAngelWings.Service.Messaging;
using Message = AltitudeAngelWings.Models.Message;

namespace AltitudeAngelWings.Plugin
{
    public class MessageDisplay : IMessageDisplay
    {
        private const int Offset = 5;

        private readonly Control _parent;
        private readonly IUiThreadInvoke _uiThreadInvoke;

        public MessageDisplay(Control parent, IUiThreadInvoke uiThreadInvoke)
        {
            _parent = parent;
            _uiThreadInvoke = uiThreadInvoke;
        }

        public void AddMessage(Message message)
        {
            var label = CreateLabel(message);
            _uiThreadInvoke.Invoke(() =>
            {
                if (!_parent.Visible) return;
                _parent.SuspendLayout();
                _parent.Controls.Add(label);
                label.BringToFront();
                LayoutLabels();
                _parent.ResumeLayout();
            });
        }

        public void RemoveMessage(Message message)
        {
            _uiThreadInvoke.Invoke(() =>
            {
                if (!_parent.Visible) return;
                _parent.SuspendLayout();
                foreach (var label in _parent.Controls.OfType<Label>().Where(l => l.Tag is Message))
                {
                    if (label.Tag != message) continue;
                    _parent.Controls.Remove(label);
                    label.Dispose();
                    break;
                }
                LayoutLabels();
                _parent.ResumeLayout();
            });
        }

        private static Label CreateLabel(Message message)
        {
            var label = new Label
            {
                Tag = message,
                Text = message.Content,
                AutoSize = true,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Visible = true,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 2, 5, 2),
                Font = new Font("Microsoft Sans Serif", 9, FontStyle.Regular)
            };
            if (message.OnClick != null)
            {
                label.Click += (sender, e) =>
                {
                    message.OnClick();
                };
            }
            return label;
        }

        private void LayoutLabels()
        {
            var totalHeight = 0;
            foreach (var label in _parent.Controls.OfType<Label>().Where(l => l.Tag is Message))
            {
                totalHeight += label.Height;
                label.Location = new Point(Offset, _parent.Height - totalHeight - Offset);
            }
        }
    }
}