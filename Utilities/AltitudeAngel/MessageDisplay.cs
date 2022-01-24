using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AltitudeAngelWings;
using AltitudeAngelWings.Service.Messaging;
using Message = AltitudeAngelWings.Models.Message;

namespace MissionPlanner.Utilities.AltitudeAngel
{
    public class MessageDisplay : IMessageDisplay
    {
        private readonly Label[] _messagesLabels;
        private readonly Control _templateControl;
        private readonly IUiThreadInvoke _uiThreadInvoke;
        private readonly List<Message> _messages = new List<Message>();

        public MessageDisplay(Control[] parentControls, Control templateControl, IUiThreadInvoke uiThreadInvoke)
        {
            _templateControl = templateControl;
            _messagesLabels = new Label[parentControls.Length];
            for (var pos = 0; pos < _messagesLabels.Length; pos++)
            {
                _messagesLabels[pos] = new Label
                {
                    Name = "AA_MessageDisplay",
                    AutoSize = true,
                    ForeColor = _templateControl.ForeColor,
                    BackColor = _templateControl.BackColor,
                    Visible = false,
                    Dock = DockStyle.Bottom,
                    Margin = new Padding(_templateControl.Height),
                    Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold)
                };
            }
            _uiThreadInvoke = uiThreadInvoke;
            _uiThreadInvoke.FireAndForget(() =>
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
            _uiThreadInvoke.FireAndForget(() =>
            {
                foreach (var messagesLabel in _messagesLabels)
                {
                    if (!messagesLabel.Parent.Visible) continue;
                    messagesLabel.Text = messages;
                    messagesLabel.ForeColor = _templateControl.ForeColor;
                    messagesLabel.Visible = messages.Length > 0;
                }
            });
        }

        public void RemoveMessage(Message message)
        {
            _messages.Remove(message);
            var messages = FormatMessages();
            _uiThreadInvoke.FireAndForget(() =>
            {
                foreach (var messagesLabel in _messagesLabels)
                {
                    if (!messagesLabel.Parent.Visible) continue;
                    messagesLabel.Text = messages;
                    messagesLabel.Visible = messages.Length > 0;
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