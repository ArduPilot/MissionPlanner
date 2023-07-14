using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using AltitudeAngelWings.Service.Messaging;
using Message = AltitudeAngelWings.Models.Message;

namespace AltitudeAngelWings.Plugin
{
    public class MessageDisplay : IMessageDisplay
    {
        private readonly Label[] _messagesLabels;
        private readonly IUiThreadInvoke _uiThreadInvoke;
        private readonly List<Message> _messages = new List<Message>();

        public MessageDisplay(IReadOnlyList<Control> parentControls, IUiThreadInvoke uiThreadInvoke)
        {
            _messagesLabels = new Label[parentControls.Count];
            for (var pos = 0; pos < _messagesLabels.Length; pos++)
            {
                _messagesLabels[pos] = new Label
                {
                    Name = "AA_MessageDisplay",
                    AutoSize = true,
                    ForeColor = Color.White,
                    BackColor = Color.Black,
                    Visible = false,
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                    BorderStyle = BorderStyle.FixedSingle,
                    Padding = new Padding(5),
                    Font = new Font("Microsoft Sans Serif", 9, FontStyle.Regular)
                };
            }
            _uiThreadInvoke = uiThreadInvoke;
            _uiThreadInvoke.Invoke(() =>
            {
                for (var pos = 0; pos < _messagesLabels.Length; pos++)
                {
                    parentControls[pos].Controls.Add(_messagesLabels[pos]);
                    _messagesLabels[pos].BringToFront();
                }
            });
        }

        public void AddMessage(Message message)
        {
            _messages.Add(message);
            var messages = FormatMessages();
            _uiThreadInvoke.Invoke(() =>
            {
                foreach (var messagesLabel in _messagesLabels)
                {
                    if (!messagesLabel.Parent.Visible) continue;
                    messagesLabel.Parent.SuspendLayout();
                    messagesLabel.Text = messages;
                    messagesLabel.Visible = messages.Length > 0;
                    messagesLabel.Location = new Point(0, messagesLabel.Parent.Height - messagesLabel.Height);
                    messagesLabel.Parent.ResumeLayout();
                }
            });
        }

        public void RemoveMessage(Message message)
        {
            _messages.Remove(message);
            var messages = FormatMessages();
            _uiThreadInvoke.Invoke(() =>
            {
                foreach (var messagesLabel in _messagesLabels)
                {
                    if (!messagesLabel.Parent.Visible) continue;
                    messagesLabel.Parent.SuspendLayout();
                    messagesLabel.Text = messages;
                    messagesLabel.Visible = messages.Length > 0;
                    messagesLabel.Location = new Point(0, messagesLabel.Parent.Height - messagesLabel.Height);
                    messagesLabel.Parent.ResumeLayout();
                }
            });
        }

        private string FormatMessages()
        {
            var builder = new StringBuilder();
            foreach (var message in _messages)
            {
                builder.AppendLine(message.Content);
            }

            return builder.ToString(0, Math.Max(0, builder.Length - Environment.NewLine.Length));
        }
    }
}