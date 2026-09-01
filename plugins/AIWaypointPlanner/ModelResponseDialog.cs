extern alias SystemDrawing;

using System;
using System.Text;
using System.Windows.Forms;
using Drawing = SystemDrawing::System.Drawing;

namespace MissionPlanner.AIWaypointPlanner
{
    public sealed class ModelResponseDialog : Form
    {
        private readonly TabControl tabs;

        public ModelResponseDialog(ApiResponseData data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Text = "模型返回数据与请求诊断";
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Drawing.Size(760, 520);
            Size = new Drawing.Size(980, 700);
            Font = new Drawing.Font("Microsoft YaHei UI", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point);

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(12)
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 116F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            Controls.Add(root);

            root.Controls.Add(new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                BackColor = Drawing.SystemColors.Window,
                Text = BuildMetadata(data)
            }, 0, 0);

            tabs = new TabControl { Dock = DockStyle.Fill };
            tabs.TabPages.Add(CreateTextPage("模型原始响应", data.RawResponse,
                data.HasServerResponse ? "服务已响应，但响应正文为空。" : "请求未到达模型服务，因此没有模型返回数据。"));
            tabs.TabPages.Add(CreateTextPage("结构化任务数据", data.StructuredOutput,
                "尚未提取出结构化任务数据。请查看模型原始响应和诊断信息。"));
            tabs.TabPages.Add(CreateTextPage("诊断信息", data.Diagnostic,
                "本次请求没有记录错误。"));
            root.Controls.Add(tabs, 0, 1);

            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 8, 0, 0),
                WrapContents = false
            };
            var close = new Button { Text = "关闭", Width = 100, Height = 32, DialogResult = DialogResult.OK };
            var copy = new Button { Text = "复制当前页", Width = 120, Height = 32 };
            copy.Click += CopyCurrentPage;
            buttons.Controls.Add(close);
            buttons.Controls.Add(copy);
            root.Controls.Add(buttons, 0, 2);
            AcceptButton = close;
        }

        private static TabPage CreateTextPage(string title, string content, string emptyMessage)
        {
            var page = new TabPage(title) { Padding = new Padding(8) };
            page.Controls.Add(new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                WordWrap = false,
                BackColor = Drawing.SystemColors.Window,
                Font = new Drawing.Font("Consolas", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point),
                Text = string.IsNullOrWhiteSpace(content) ? emptyMessage : content
            });
            return page;
        }

        private static string BuildMetadata(ApiResponseData data)
        {
            var builder = new StringBuilder();
            builder.AppendLine("请求时间（本地）：" + data.RequestedAtUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            builder.AppendLine("请求：" + (data.Method ?? string.Empty) + " " + (data.Endpoint ?? string.Empty));
            builder.AppendLine("协议 / 模型：" + (data.Protocol ?? string.Empty) + " / " + (data.Model ?? string.Empty));
            builder.Append("HTTP 状态：");
            builder.Append(data.HttpStatusCode.HasValue
                ? data.HttpStatusCode.Value + " " + (data.HttpReasonPhrase ?? string.Empty)
                : "未收到 HTTP 响应");
            if (!string.IsNullOrWhiteSpace(data.RequestId))
                builder.Append("；请求 ID：" + data.RequestId);
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
