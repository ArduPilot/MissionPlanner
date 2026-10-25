extern alias SystemDrawing;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MissionPlanner.GCSViews;
using MissionPlanner.Utilities;
using Drawing = SystemDrawing::System.Drawing;

namespace MissionPlanner.AIWaypointPlanner
{
    public sealed partial class AIWaypointPlannerForm
    {
        private string L(string key)
        {
            return UiStrings.Get(languageCode, key);
        }

        private T Track<T>(T control, string key) where T : Control
        {
            if (control == null)
                throw new ArgumentNullException("control");

            if (!string.IsNullOrWhiteSpace(key))
            {
                localizationKeys[control] = key;
                control.Text = L(key);
            }
            return control;
        }

        private void ApplyLocalization()
        {
            languageCode = UiStrings.NormalizeLanguageCode(languageCode);
            validator.LanguageCode = languageCode;
            compiler.LanguageCode = languageCode;
            Text = L("App.Title");
            if (!string.IsNullOrWhiteSpace(activeStatusKey))
                activeStatusText = GetActiveStatusText();

            foreach (KeyValuePair<Control, string> entry in localizationKeys.ToArray())
            {
                if (entry.Key == null)
                    continue;
                if (entry.Key.IsDisposed)
                {
                    localizationKeys.Remove(entry.Key);
                    continue;
                }
                entry.Key.Text = L(entry.Value);
            }

            if (conversationPanel != null)
            {
                foreach (ConversationMessageControl message in conversationPanel.Controls
                    .OfType<ConversationMessageControl>())
                {
                    message.ApplyLanguage(languageCode);
                    Func<string> localizer;
                    if (conversationMessageLocalizers.TryGetValue(message, out localizer) &&
                        localizer != null)
                    {
                        message.SetMessage(localizer());
                    }
                }
            }
            if (pendingStatusLabel != null && !pendingStatusLabel.IsDisposed &&
                !string.IsNullOrWhiteSpace(activeStatusKey))
                pendingStatusLabel.Text = GetActiveStatusText();

            if (objectiveTextBox != null && string.IsNullOrWhiteSpace(objectiveTextBox.Text))
                objectiveTextBox.Text = string.Empty;

            RefreshLocalizedOptionLists();
            RefreshProviderPresetNames();
            if (candidateGrid != null && candidateGrid.Columns.Count >= 8)
            {
                string[] headers =
                {
                    L("Mission.Sequence"), L("Mission.Command"), L("Mission.Latitude"),
                    L("Mission.Longitude"), L("Mission.Altitude"), L("Mission.Parameter1"),
                    L("Mission.Parameter2"), L("Mission.Description")
                };
                for (int i = 0; i < headers.Length; i++)
                    candidateGrid.Columns[i].HeaderText = headers[i];
            }
            ApplySafetyNoticeStyle();
            UpdateSafetyNoticeLayout();
            UpdateSuggestionButtonLayouts();
            RefreshAttachmentChips();
            ResizeConversationItems();
            UpdateAuthenticationControls();
            RelocalizeCurrentResult();
            if (activityLabel != null)
                activityLabel.Text = string.IsNullOrWhiteSpace(activeStatusKey)
                    ? L("Status.Idle")
                    : GetActiveStatusText();

            RefreshProviderNote();
            plugin.ApplyLanguage(languageCode);
            if (currentResult == null && validationTextBox != null &&
                !string.IsNullOrWhiteSpace(validationStatusKey))
                validationTextBox.Text = L(validationStatusKey);
            ApplyPluginTheme();
        }

        private void LanguageSelectionChanged(object sender, EventArgs e)
        {
            if (loadingLanguage)
                return;

            UiLanguageOption option = languageComboBox == null
                ? null
                : languageComboBox.SelectedItem as UiLanguageOption;
            if (option == null)
                return;

            string selectedLanguageCode = UiStrings.NormalizeLanguageCode(option.LanguageCode);
            if (string.Equals(languageCode, selectedLanguageCode, StringComparison.Ordinal))
                return;

            bool resetLanguageSensitiveContent = HasLanguageSensitiveContent();
            languageCode = selectedLanguageCode;
            preferences.LanguageCode = languageCode;
            try
            {
                preferencesStore.Save(preferences);
            }
            catch
            {
                // The choice remains active for this session and is retried on close.
            }
            ApplyLocalization();
            if (resetLanguageSensitiveContent)
                ResetLanguageSensitiveContentForLanguageChange();
        }

        private bool HasLanguageSensitiveContent()
        {
            return currentResult != null ||
                   lastResponseData != null ||
                   (conversationPanel != null && conversationPanel.Controls
                       .OfType<ConversationMessageControl>().Any());
        }

        private void ResetLanguageSensitiveContentForLanguageChange()
        {
            conversationSession.Clear();
            InvalidateGeneratedResult("Language.GeneratedContentReset");
            RemovePendingStatus();

            if (conversationPanel == null)
                return;

            foreach (Control control in conversationPanel.Controls.Cast<Control>().ToArray())
            {
                ForgetLocalizationTree(control);
                control.Dispose();
            }
            conversationPanel.Controls.Clear();
            conversationPanel.Controls.Add(BuildWelcomePanel());
            AddConversationMessage(
                ConversationMessageRole.System,
                L("Language.GeneratedContentReset"),
                false,
                () => L("Language.GeneratedContentReset"));
            UpdateSuggestionButtonLayouts();
            ApplyPluginTheme();
        }

        private void RefreshLocalizedOptionLists()
        {
            if (protocolComboBox == null || authenticationComboBox == null || reasoningComboBox == null)
                return;

            int protocolIndex = protocolComboBox.SelectedIndex;
            int authenticationIndex = authenticationComboBox.SelectedIndex;
            ApiReasoningLevel reasoning = GetReasoningLevel();

            loadingApiProfile = true;
            try
            {
                protocolComboBox.Items.Clear();
                protocolComboBox.Items.Add(L("Protocol.Responses"));
                protocolComboBox.Items.Add(L("Protocol.ChatCompletions"));
                protocolComboBox.SelectedIndex = protocolIndex < 0 ? 0 : Math.Min(protocolIndex, 1);

                authenticationComboBox.Items.Clear();
                authenticationComboBox.Items.Add(L("Authentication.Bearer"));
                authenticationComboBox.Items.Add(L("Authentication.ApiKeyHeader"));
                authenticationComboBox.Items.Add(L("Authentication.None"));
                authenticationComboBox.SelectedIndex = authenticationIndex < 0
                    ? 0
                    : Math.Min(authenticationIndex, 2);

                reasoningComboBox.Items.Clear();
                reasoningComboBox.Items.Add(L("Reasoning.ProviderDefault"));
                reasoningComboBox.Items.Add(L("Reasoning.Low"));
                reasoningComboBox.Items.Add(L("Reasoning.Medium"));
                reasoningComboBox.Items.Add(L("Reasoning.High"));
                reasoningComboBox.Items.Add(L("Reasoning.VeryHigh"));
                reasoningComboBox.Items.Add(L("Reasoning.Ultra"));
                reasoningComboBox.SelectedIndex = GetReasoningIndex(reasoning);
            }
            finally
            {
                loadingApiProfile = false;
            }

            UpdateAuthenticationControls();
        }

        private void RefreshProviderPresetNames()
        {
            if (providerComboBox == null)
                return;

            int selectedIndex = providerComboBox.SelectedIndex;
            foreach (ApiProviderPreset preset in providerComboBox.Items.OfType<ApiProviderPreset>())
            {
                preset.DisplayName = string.IsNullOrWhiteSpace(preset.NameKey)
                    ? preset.Name
                    : L(preset.NameKey);
            }
            providerComboBox.Refresh();
            if (selectedIndex >= 0 && selectedIndex < providerComboBox.Items.Count)
                providerComboBox.SelectedIndex = selectedIndex;
        }

        private void ForgetLocalizationTree(Control control)
        {
            if (control == null)
                return;

            foreach (Control child in control.Controls.Cast<Control>().ToArray())
                ForgetLocalizationTree(child);
            localizationKeys.Remove(control);
            ConversationMessageControl message = control as ConversationMessageControl;
            if (message != null)
                conversationMessageLocalizers.Remove(message);
        }
    }
}
