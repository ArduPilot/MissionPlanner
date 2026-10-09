extern alias SystemDrawing;

using System;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using Drawing = SystemDrawing::System.Drawing;

namespace MissionPlanner.AIWaypointPlanner
{
    public sealed class ModelResponseDialog : Form
    {
        private readonly TabControl tabs;
        private readonly string languageCode;

        public ModelResponseDialog(ApiResponseData data)
            : this(data, UiStrings.DefaultLanguageCode)
        {
        }

        public ModelResponseDialog(ApiResponseData data, string languageCode)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            this.languageCode = UiStrings.NormalizeLanguageCode(languageCode);
            Text = UiStrings.Get(this.languageCode, "Diagnostics.Title");
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Drawing.Size(760, 520);
            Size = new Drawing.Size(980, 700);
            Font = Drawing.SystemFonts.MessageBoxFont;
            AutoScaleMode = AutoScaleMode.Font;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(12)
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 132F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            Controls.Add(root);

            root.Controls.Add(new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                BackColor = PluginTheme.InputBackground,
                ForeColor = PluginTheme.PrimaryText,
                ScrollBars = ScrollBars.Vertical,
                Text = BuildMetadata(data)
            }, 0, 0);

            tabs = new PluginTabControl { Dock = DockStyle.Fill };
            tabs.TabPages.Add(CreateTextPage(
                UiStrings.Get(this.languageCode, "Diagnostics.RawResponse"),
                data.RawResponse,
                UiStrings.Get(this.languageCode, "Diagnostics.NoRawResponse")));
            tabs.TabPages.Add(CreateTextPage(
                UiStrings.Get(this.languageCode, "Diagnostics.StructuredData"),
                data.StructuredOutput,
                UiStrings.Get(this.languageCode, "Diagnostics.NoStructuredData")));
            tabs.TabPages.Add(CreateTextPage(
                UiStrings.Get(this.languageCode, "Diagnostics.DiagnosticInfo"),
                data.Diagnostic,
                UiStrings.Get(this.languageCode, "Diagnostics.NoError")));
            root.Controls.Add(tabs, 0, 1);

            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 8, 0, 0),
                WrapContents = false
            };
            var close = new Button
            {
                Text = UiStrings.Get(this.languageCode, "Button.Close"),
                AutoSize = true,
                MinimumSize = new Drawing.Size(100, 32),
                Height = 32,
                DialogResult = DialogResult.OK
            };
            var copy = new Button
            {
                Text = UiStrings.Get(this.languageCode, "Button.CopyCurrentPage"),
                AutoSize = true,
                MinimumSize = new Drawing.Size(140, 32),
                Height = 32
            };
            copy.Click += CopyCurrentPage;
            buttons.Controls.Add(close);
            buttons.Controls.Add(copy);
            root.Controls.Add(buttons, 0, 2);
            AcceptButton = close;
            PluginTheme.ApplyButton(close, PluginButtonStyle.Primary);
            PluginTheme.ApplyButton(copy, PluginButtonStyle.Secondary);
            ApplyPluginTheme();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            ApplyPluginTheme();
        }

        public void ApplyPluginTheme()
        {
            PluginTheme.Apply(this);
        }

        private TabPage CreateTextPage(string title, string content, string emptyMessage)
        {
            var page = new TabPage(title)
            {
                Padding = new Padding(8),
                BackColor = PluginTheme.WindowBackground,
                ForeColor = PluginTheme.PrimaryText
            };
            page.Controls.Add(new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                WordWrap = false,
                BackColor = PluginTheme.InputBackground,
                ForeColor = PluginTheme.PrimaryText,
                Font = new Drawing.Font("Consolas", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point),
                Text = string.IsNullOrWhiteSpace(content) ? emptyMessage : content
            });
            return page;
        }

        private string BuildMetadata(ApiResponseData data)
        {
            var builder = new StringBuilder();
            builder.AppendLine(UiStrings.Get(languageCode, "Diagnostics.RequestMetadata"));
            builder.AppendLine(UiStrings.Format(
                languageCode,
                "Diagnostics.RequestedAtFormat",
                data.RequestedAtUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)));
            builder.AppendLine(UiStrings.Format(
                languageCode,
                "Diagnostics.RequestFormat",
                data.Method ?? string.Empty,
                data.Endpoint ?? string.Empty));
            builder.AppendLine(UiStrings.Format(
                languageCode,
                "Diagnostics.ProtocolModelFormat",
                data.Protocol ?? string.Empty,
                data.Model ?? string.Empty));

            string httpStatus = data.HttpStatusCode.HasValue
                ? data.HttpStatusCode.Value + " " + (data.HttpReasonPhrase ?? string.Empty)
                : UiStrings.Get(languageCode, "Diagnostics.NoHttpResponse");
            builder.AppendLine(UiStrings.Format(languageCode, "Diagnostics.HttpStatusFormat", httpStatus));
            if (!string.IsNullOrWhiteSpace(data.RequestId))
            {
                builder.AppendLine(UiStrings.Format(
                    languageCode,
                    "Diagnostics.RequestIdFormat",
                    data.RequestId));
            }
            builder.AppendLine(UiStrings.Format(languageCode, "Diagnostics.AttemptsFormat", data.AttemptCount));
            builder.Append(UiStrings.Format(languageCode, "Diagnostics.ReconnectsFormat", data.RetryCount));
            return builder.ToString();
        }

        private void CopyCurrentPage(object sender, EventArgs e)
        {
            if (tabs.SelectedTab == null || tabs.SelectedTab.Controls.Count == 0)
                return;
            var textBox = tabs.SelectedTab.Controls[0] as TextBox;
            if (textBox != null && !string.IsNullOrEmpty(textBox.Text))
                Clipboard.SetText(textBox.Text);
        }
    }
}
