using System;
using MissionPlanner.Plugin;
using MissionPlanner.Utilities;

namespace RedundantLinkManager
{
    public class RedundantLinkManager_Plugin : Plugin
    {
        public override string Name { get; } = "Redundant Link Manager";
        public override string Version { get; } = "0.1";
        public override string Author { get; } = "Bob Long";

        public override bool Init() { return true; }

        public override bool Loaded()
        {
            // Remove the stock connection control
            //Host.MainForm.MainMenu.Items.RemoveByKey("toolStripConnectionControl");
            //Host.MainForm.MainMenu.Items.RemoveByKey("MenuConnect");

            // Add the link manager button
            var linkManagerButton = new System.Windows.Forms.ToolStripButton
            {
                Name = "toolStripLinkManager",
                Size = new System.Drawing.Size(180, 22),
                Text = "Manage\nLinks",
                Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
            };
            linkManagerButton.Click += new EventHandler(this.linkManagerButton_Click);

            ThemeManager.ApplyThemeTo(linkManagerButton);
            Host.MainForm.MainMenu.Items.Insert(0, linkManagerButton);

            return true;
        }

        private void linkManagerButton_Click(object sender, EventArgs e)
        {
            var linkManager = new RedundantLinkManager();
            linkManager.Show();
        }

        public override bool Exit() { return true; }
    }
}