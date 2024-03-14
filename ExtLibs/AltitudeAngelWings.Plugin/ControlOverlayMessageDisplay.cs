using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AltitudeAngelWings.Model;
using AltitudeAngelWings.Service.Messaging;
using Message = AltitudeAngelWings.Model.Message;

namespace AltitudeAngelWings.Plugin
{
    public class ControlOverlayMessageDisplay : IMessageDisplay
    {
        private const int LeftOffset = 5;

        private readonly Control _parent;
        private readonly IUiThreadInvoke _uiThreadInvoke;
        private readonly int _bottomOffset;

        public ControlOverlayMessageDisplay(Control parent, IUiThreadInvoke uiThreadInvoke, int bottomOffset)
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
                var labels = GetMessageLabels();
                if (!string.IsNullOrEmpty(message.Key))
                {
                    var matchingKeys = labels.Where(l => l.Name == message.Key).ToList();
                    foreach (var remove in matchingKeys)
                    {
                        _parent.Controls.Remove(remove);
                        labels.Remove(remove);
                        remove.Dispose();
                    }
                }
                _parent.Controls.Add(label);
                label.BringToFront();
                labels.Add(label);
                LayoutLabels(labels);
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
                var removed = false;
                foreach (var label in labels)
                {
                    if (label.Tag != message) continue;
                    _parent.Controls.Remove(label);
                    labels.Remove(label);
                    label.Dispose();
                    removed = true;
                    break;
                }

                if (removed)
                {
                    LayoutLabels(labels);
                }

                _parent.ResumeLayout();
            });
        }

        private Label CreateLabel(Message message)
        {
            var label = new Label
            {
                Tag = message,
                Text = message.Content,
                Name = message.Key ?? "",
                AutoSize = true,
                ForeColor = GetColorForMessage(message),
                BackColor = Color.Transparent,
                Visible = true,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 5, 1),
                Font = new Font("Microsoft Sans Serif", 9, FontStyle.Regular)
            };
            if (message.OnClick == null)
            {
                return label;
            }

            label.Cursor = Cursors.Hand;
            label.Click += (sender, e) =>
            {
                message.OnClick();
                RemoveMessage(message);
            };
            return label;
        }

        private static Color GetColorForMessage(Message message)
        {
            switch (message.Type)
            {
                case MessageType.Error:
                    return Color.Red;
                default:
                    return message.OnClick == null ? Color.White : Color.LawnGreen;
            }
        }

        private void LayoutLabels(IEnumerable<Label> labels)
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
