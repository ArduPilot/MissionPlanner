using System;
using System.Windows.Forms;

namespace MissionPlanner.AIWaypointPlanner
{
    public sealed class AIWaypointPlannerPlugin : MissionPlanner.Plugin.Plugin
    {
        private ToolStripMenuItem menuItem;
        private string languageCode;

        public override string Name
        {
            get { return UiStrings.Get(GetEffectiveLanguageCode(), "App.Name"); }
        }

        public override string Version { get { return PluginIdentity.Version; } }
        public override string Author { get { return "Alico12315"; } }

        public override bool Init()
        {
            return true;
        }

        public override bool Loaded()
        {
            languageCode = LoadSavedLanguageCode();
            menuItem = new ToolStripMenuItem();
            ApplyLanguage(languageCode);
            menuItem.Click += OpenPlanner;

            ToolStripItemCollection items = Host.FPMenuMap.Items;
            foreach (ToolStripItem item in items)
            {
                if (string.Equals(item.Name, "autoWPToolStripMenuItem", StringComparison.Ordinal) &&
                    item is ToolStripMenuItem)
                {
                    ((ToolStripMenuItem)item).DropDownItems.Add(menuItem);
                    return true;
                }
            }

            items.Add(menuItem);
            return true;
        }

        private void OpenPlanner(object sender, EventArgs e)
        {
            using (var form = new AIWaypointPlannerForm(this))
            {
                MissionPlanner.Utilities.ThemeManager.ApplyThemeTo(form);
                form.ApplyPluginTheme();
                form.ShowDialog(Host.MainForm);
            }
        }

        public void ApplyLanguage(string selectedLanguageCode)
        {
            languageCode = UiStrings.NormalizeLanguageCode(selectedLanguageCode);
            if (menuItem == null || menuItem.IsDisposed)
                return;

            menuItem.Text = UiStrings.Get(languageCode, "App.Name");
            menuItem.ToolTipText = UiStrings.Get(languageCode, "App.MenuTooltip");
        }

        private string GetEffectiveLanguageCode()
        {
            return string.IsNullOrWhiteSpace(languageCode)
                ? LoadSavedLanguageCode()
                : UiStrings.NormalizeLanguageCode(languageCode);
        }

        private static string LoadSavedLanguageCode()
        {
            try
            {
                return UiStrings.NormalizeLanguageCode(
                    new PluginPreferencesStore().Load().LanguageCode);
            }
            catch
            {
                return UiStrings.DefaultLanguageCode;
            }
        }

        public override bool Exit()
        {
            if (menuItem != null)
            {
                menuItem.Click -= OpenPlanner;
                if (menuItem.Owner != null)
                    menuItem.Owner.Items.Remove(menuItem);
                menuItem.Dispose();
                menuItem = null;
            }
            return true;
        }
    }
}
