extern alias SystemDrawing;

using System;
using System.Windows.Forms;
using Drawing = SystemDrawing::System.Drawing;

namespace MissionPlanner.AIWaypointPlanner
{
    public enum ConversationMessageRole
    {
        User,
        Assistant,
        System
    }

    public sealed class ConversationMessageControl : UserControl
    {
        private readonly Label roleLabel;
        private readonly Label messageLabel;
        private readonly ConversationMessageRole role;
        private readonly bool isError;
        private string languageCode;

        public ConversationMessageControl(
            ConversationMessageRole role,
            string message,
            string languageCode,
            bool isError)
        {
            this.role = role;
            this.isError = isError;
            this.languageCode = UiStrings.NormalizeLanguageCode(languageCode);

            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Margin = new Padding(0, 0, 0, 10);
            Padding = new Padding(14, 10, 14, 12);
            BorderStyle = BorderStyle.FixedSingle;

            var layout = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                RowCount = 2,
                Dock = DockStyle.Top,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            roleLabel = new Label
            {
                AutoSize = true,
                Font = new Drawing.Font(Font, Drawing.FontStyle.Bold),
                Margin = new Padding(0, 0, 0, 5)
            };
            messageLabel = new Label
            {
                AutoSize = true,
                Text = message ?? string.Empty,
                Margin = Padding.Empty,
                UseMnemonic = false
            };
            layout.Controls.Add(roleLabel, 0, 0);
            layout.Controls.Add(messageLabel, 0, 1);
            Controls.Add(layout);
            ApplyLanguage(this.languageCode);
            ApplyPluginTheme();
        }

        public void ApplyLanguage(string selectedLanguageCode)
        {
            languageCode = UiStrings.NormalizeLanguageCode(selectedLanguageCode);
            string key = role == ConversationMessageRole.User
                ? "Chat.UserLabel"
                : role == ConversationMessageRole.Assistant
                    ? "Chat.AssistantLabel"
                    : "Chat.SystemLabel";
            roleLabel.Text = UiStrings.Get(languageCode, key);
        }

        public void SetMessage(string message)
        {
            messageLabel.Text = message ?? string.Empty;
        }

        public void ApplyPluginTheme()
        {
            PluginTheme.Apply(this);
        }

        internal void ApplyPluginThemeColors()
        {
            BackColor = isError
                ? PluginTheme.ErrorMessage
                : role == ConversationMessageRole.User
                    ? PluginTheme.UserMessage
                    : PluginTheme.AssistantMessage;
            ForeColor = isError ? PluginTheme.ErrorText : PluginTheme.PrimaryText;
            BorderStyle = BorderStyle.FixedSingle;
            roleLabel.BackColor = Drawing.Color.Transparent;
            roleLabel.ForeColor = isError
                ? PluginTheme.ErrorText
                : role == ConversationMessageRole.User
                    ? Drawing.Color.FromArgb(166, 203, 255)
                    : PluginTheme.SecondaryText;
            messageLabel.BackColor = Drawing.Color.Transparent;
            messageLabel.ForeColor = isError ? PluginTheme.ErrorText : PluginTheme.PrimaryText;
        }

        public void SetAvailableWidth(int width)
        {
            int available = Math.Max(260, width);
            Width = available;
            MaximumSize = new Drawing.Size(available, 0);
            messageLabel.MaximumSize = new Drawing.Size(Math.Max(220, available - Padding.Horizontal), 0);
        }
    }
}
