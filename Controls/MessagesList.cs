using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static MAVLink;

namespace MissionPlanner.Controls
{
    public class MessagesList : UserControl
    {
        private Panel containerPanel;
        private VScrollBar scrollBar;
        private List<(DateTime time, string message, byte severity)> messages = new List<(DateTime, string, byte)>();
        private int itemHeight = 26;
        private int lastMessageCount = 0;
        private bool autoScroll = true;
        private Font displayFont;

        public MessagesList()
        {
            InitializeComponent();
            displayFont = new Font("Segoe UI", 9.5f, FontStyle.Regular);
        }

        private void InitializeComponent()
        {
            this.containerPanel = new DoubleBufferedPanel();
            this.scrollBar = new VScrollBar();
            this.SuspendLayout();

            // scrollBar - add first so it docks on right
            this.scrollBar.Dock = DockStyle.Right;
            this.scrollBar.Name = "scrollBar";
            this.scrollBar.Scroll += ScrollBar_Scroll;

            // containerPanel
            this.containerPanel.Dock = DockStyle.Fill;
            this.containerPanel.Name = "containerPanel";
            this.containerPanel.BackColor = Color.FromArgb(30, 30, 30);
            this.containerPanel.Paint += ContainerPanel_Paint;
            this.containerPanel.MouseWheel += ContainerPanel_MouseWheel;
            this.containerPanel.MouseClick += ContainerPanel_MouseClick;
            this.containerPanel.MouseEnter += ContainerPanel_MouseEnter;

            // MessagesList - add scrollbar first, then panel
            this.Controls.Add(this.scrollBar);
            this.Controls.Add(this.containerPanel);
            this.Name = "MessagesList";
            this.Size = new Size(400, 300);
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ResumeLayout(false);
        }

        private void ContainerPanel_MouseEnter(object sender, EventArgs e)
        {
            // Focus the panel to receive mouse wheel events
            if (!containerPanel.Focused)
                containerPanel.Focus();
        }

        private void ContainerPanel_MouseClick(object sender, MouseEventArgs e)
        {
            // Focus the panel to receive mouse wheel events
            containerPanel.Focus();
        }

        private void ContainerPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!scrollBar.Enabled)
                return;

            int delta = e.Delta > 0 ? -3 : 3;
            int newValue = scrollBar.Value + delta;
            newValue = Math.Max(scrollBar.Minimum, Math.Min(scrollBar.Maximum - scrollBar.LargeChange + 1, newValue));

            if (newValue != scrollBar.Value)
            {
                scrollBar.Value = newValue;
                autoScroll = (scrollBar.Value >= scrollBar.Maximum - scrollBar.LargeChange);
                containerPanel.Invalidate();
            }
        }

        private void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            autoScroll = (scrollBar.Value >= scrollBar.Maximum - scrollBar.LargeChange);
            containerPanel.Invalidate();
        }

        public void UpdateMessages(List<(DateTime time, string message, byte severity)> newMessages)
        {
            if (newMessages == null)
                return;

            if (newMessages.Count == lastMessageCount)
                return;

            messages = new List<(DateTime, string, byte)>(newMessages);
            lastMessageCount = messages.Count;

            UpdateScrollBar();

            if (autoScroll && messages.Count > 0)
            {
                int maxScroll = Math.Max(0, scrollBar.Maximum - scrollBar.LargeChange + 1);
                if (scrollBar.Value != maxScroll)
                    scrollBar.Value = maxScroll;
            }

            containerPanel.Invalidate();
        }

        private void UpdateScrollBar()
        {
            int visibleItems = Math.Max(1, containerPanel.Height / itemHeight);

            if (messages.Count <= visibleItems)
            {
                scrollBar.Enabled = false;
                scrollBar.Value = 0;
                scrollBar.Maximum = 0;
            }
            else
            {
                scrollBar.Enabled = true;
                scrollBar.Minimum = 0;
                scrollBar.Maximum = messages.Count - 1;
                scrollBar.LargeChange = Math.Max(1, visibleItems);
                scrollBar.SmallChange = 1;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateScrollBar();
            containerPanel.Invalidate();
        }

        private void ContainerPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(containerPanel.BackColor);

            if (messages.Count == 0)
            {
                // Draw placeholder text
                using (var brush = new SolidBrush(Color.Gray))
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString("No messages", displayFont, brush, containerPanel.ClientRectangle, sf);
                }
                return;
            }

            int visibleItems = (containerPanel.Height / itemHeight) + 2;
            int scrollOffset = scrollBar.Enabled ? scrollBar.Value : 0;

            // Draw messages from newest (bottom) to oldest (top)
            // Messages list is oldest first, so we iterate from end
            int y = 0;
            for (int i = 0; i < visibleItems && i + scrollOffset < messages.Count; i++)
            {
                int msgIndex = messages.Count - 1 - scrollOffset - i;
                if (msgIndex < 0)
                    break;

                var msg = messages[msgIndex];
                DrawMessageRow(g, msg, y);
                y += itemHeight;

                if (y > containerPanel.Height)
                    break;
            }
        }

        private void DrawMessageRow(Graphics g, (DateTime time, string message, byte severity) msg, int y)
        {
            Color bgColor = GetSeverityBackgroundColor(msg.severity);
            Color textColor = GetSeverityTextColor(msg.severity);
            string severityText = GetSeverityText(msg.severity);

            // Draw background
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, 0, y, containerPanel.Width, itemHeight - 1);
            }

            // Draw separator line
            using (var pen = new Pen(Color.FromArgb(50, 50, 50)))
            {
                g.DrawLine(pen, 0, y + itemHeight - 1, containerPanel.Width, y + itemHeight - 1);
            }

            int padding = 4;
            int textY = y + (itemHeight - displayFont.Height) / 2;

            // Draw timestamp
            string timeStr = msg.time.ToString("HH:mm:ss");
            using (var brush = new SolidBrush(Color.FromArgb(160, 160, 160)))
            {
                g.DrawString(timeStr, displayFont, brush, padding, textY);
            }

            // Draw severity badge
            int severityX = 70;
            Color badgeColor = GetSeverityBadgeColor(msg.severity);
            using (var brush = new SolidBrush(badgeColor))
            {
                var badgeRect = new Rectangle(severityX, y + 4, 50, itemHeight - 8);
                g.FillRectangle(brush, badgeRect);
            }
            using (var brush = new SolidBrush(Color.White))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                var badgeRect = new RectangleF(severityX, y, 50, itemHeight);
                using (var smallFont = new Font(displayFont.FontFamily, displayFont.Size * 0.8f, FontStyle.Bold))
                {
                    g.DrawString(severityText, smallFont, brush, badgeRect, sf);
                }
            }

            // Draw message
            int messageX = 130;
            using (var brush = new SolidBrush(textColor))
            {
                var messageRect = new RectangleF(messageX, textY, containerPanel.Width - messageX - padding, displayFont.Height);
                using (var sf = new StringFormat { Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap })
                {
                    g.DrawString(msg.message, displayFont, brush, messageRect, sf);
                }
            }
        }

        private Color GetSeverityBackgroundColor(byte severity)
        {
            switch ((MAV_SEVERITY)severity)
            {
                case MAV_SEVERITY.EMERGENCY:
                case MAV_SEVERITY.ALERT:
                case MAV_SEVERITY.CRITICAL:
                    return Color.FromArgb(60, 20, 20);
                case MAV_SEVERITY.ERROR:
                    return Color.FromArgb(50, 25, 25);
                case MAV_SEVERITY.WARNING:
                    return Color.FromArgb(50, 45, 20);
                case MAV_SEVERITY.NOTICE:
                    return Color.FromArgb(30, 40, 50);
                case MAV_SEVERITY.INFO:
                    return Color.FromArgb(35, 35, 35);
                case MAV_SEVERITY.DEBUG:
                    return Color.FromArgb(30, 30, 40);
                default:
                    return Color.FromArgb(35, 35, 35);
            }
        }

        private Color GetSeverityTextColor(byte severity)
        {
            switch ((MAV_SEVERITY)severity)
            {
                case MAV_SEVERITY.EMERGENCY:
                case MAV_SEVERITY.ALERT:
                case MAV_SEVERITY.CRITICAL:
                    return Color.FromArgb(255, 120, 120);
                case MAV_SEVERITY.ERROR:
                    return Color.FromArgb(255, 150, 150);
                case MAV_SEVERITY.WARNING:
                    return Color.FromArgb(255, 230, 130);
                case MAV_SEVERITY.NOTICE:
                    return Color.FromArgb(130, 200, 255);
                case MAV_SEVERITY.INFO:
                    return Color.FromArgb(230, 230, 230);
                case MAV_SEVERITY.DEBUG:
                    return Color.FromArgb(170, 170, 200);
                default:
                    return Color.FromArgb(230, 230, 230);
            }
        }

        private Color GetSeverityBadgeColor(byte severity)
        {
            switch ((MAV_SEVERITY)severity)
            {
                case MAV_SEVERITY.EMERGENCY:
                    return Color.FromArgb(180, 0, 0);
                case MAV_SEVERITY.ALERT:
                    return Color.FromArgb(200, 50, 0);
                case MAV_SEVERITY.CRITICAL:
                    return Color.FromArgb(180, 30, 30);
                case MAV_SEVERITY.ERROR:
                    return Color.FromArgb(160, 50, 50);
                case MAV_SEVERITY.WARNING:
                    return Color.FromArgb(180, 140, 0);
                case MAV_SEVERITY.NOTICE:
                    return Color.FromArgb(0, 100, 150);
                case MAV_SEVERITY.INFO:
                    return Color.FromArgb(70, 70, 70);
                case MAV_SEVERITY.DEBUG:
                    return Color.FromArgb(80, 80, 100);
                default:
                    return Color.FromArgb(70, 70, 70);
            }
        }

        private string GetSeverityText(byte severity)
        {
            switch ((MAV_SEVERITY)severity)
            {
                case MAV_SEVERITY.EMERGENCY:
                    return "EMERG";
                case MAV_SEVERITY.ALERT:
                    return "ALERT";
                case MAV_SEVERITY.CRITICAL:
                    return "CRIT";
                case MAV_SEVERITY.ERROR:
                    return "ERROR";
                case MAV_SEVERITY.WARNING:
                    return "WARN";
                case MAV_SEVERITY.NOTICE:
                    return "NOTICE";
                case MAV_SEVERITY.INFO:
                    return "INFO";
                case MAV_SEVERITY.DEBUG:
                    return "DEBUG";
                default:
                    return "INFO";
            }
        }

        // Double buffered panel to prevent flickering
        private class DoubleBufferedPanel : Panel
        {
            public DoubleBufferedPanel()
            {
                this.DoubleBuffered = true;
                this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
                this.UpdateStyles();
            }
        }
    }
}
