using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AltitudeAngelWings.Service.Messaging;
using Message = AltitudeAngelWings.Models.Message;

namespace AltitudeAngelWings.Plugin
{
    public class MessageDisplay : IMessageDisplay
    {
        private const int LeftOffset = 5;

        private readonly Control _parent;
        private readonly IUiThreadInvoke _uiThreadInvoke;
        private readonly int _bottomOffset;

        public MessageDisplay(Control parent, IUiThreadInvoke uiThreadInvoke, int bottomOffset)
        {
            _parent = parent;
            _uiThreadInvoke = uiThreadInvoke;
            _bottomOffset = bottomOffset;
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
                LayoutLabels(GetMessageLabels());
                _parent.ResumeLayout();
            });
        }

        public void RemoveMessage(Message message)
        {
            _uiThreadInvoke.Invoke(() =>
            {
                if (!_parent.Visible) return;
                _parent.SuspendLayout();
                var labels = GetMessageLabels();
                foreach (var label in labels)
                {
                    if (label.Tag != message) continue;
                    _parent.Controls.Remove(label);
                    labels.Remove(label);
                    label.Dispose();
                    break;
                }
                LayoutLabels(labels);
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
                Padding = new Padding(5, 0, 5, 1),
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

        private void LayoutLabels(ICollection<Label> labels)
        {
            var totalHeight = 0;
            foreach (var label in labels)
            {
                totalHeight += label.Height;
                label.Location = new Point(LeftOffset, _parent.Height - totalHeight - _bottomOffset);
            }
        }

        private IList<Label> GetMessageLabels() => _parent.Controls.OfType<Label>().Where(l => l.Tag is Message).ToList();
    }
}