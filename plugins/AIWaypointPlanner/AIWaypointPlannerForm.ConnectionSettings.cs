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
        private ApiProfileRecord SelectedApiProfile
        {
            get { return savedProfileComboBox == null ? null : savedProfileComboBox.SelectedItem as ApiProfileRecord; }
        }

        private ApiReasoningLevel GetReasoningLevel()
        {
            if (reasoningComboBox == null || reasoningComboBox.SelectedIndex < 0)
                return ApiReasoningLevel.Off;

            switch (reasoningComboBox.SelectedIndex)
            {
                case 0: return ApiReasoningLevel.Off;
                case 1: return ApiReasoningLevel.Low;
                case 2: return ApiReasoningLevel.Medium;
                case 3: return ApiReasoningLevel.High;
                case 4: return ApiReasoningLevel.ExtraHigh;
                case 5: return ApiReasoningLevel.Ultra;
                default: return ApiReasoningLevel.Off;
            }
        }

        private static int GetReasoningIndex(ApiReasoningLevel level)
        {
            switch (level)
            {
                case ApiReasoningLevel.Off: return 0;
                case ApiReasoningLevel.Low: return 1;
                case ApiReasoningLevel.Medium: return 2;
                case ApiReasoningLevel.High: return 3;
                case ApiReasoningLevel.ExtraHigh: return 4;
                case ApiReasoningLevel.Ultra: return 5;
                default: return 0;
        }
        }

        private void ProviderSelectionChanged(object sender, EventArgs e)
        {
            if (loadingApiProfile)
                return;

            var preset = providerComboBox.SelectedItem as ApiProviderPreset;
            if (preset == null)
                return;

            loadingApiProfile = true;
            if (savedProfileComboBox != null)
                savedProfileComboBox.SelectedIndex = -1;
            loadingApiProfile = false;

            baseUrlTextBox.Text = preset.BaseUrl;
            protocolComboBox.SelectedIndex = preset.Protocol == ApiProtocol.Responses ? 0 : 1;
            authenticationComboBox.SelectedIndex = GetAuthenticationIndex(preset.AuthenticationMode);
            modelTextBox.Text = preset.Model;
            reasoningComboBox.SelectedIndex = GetReasoningIndex(preset.ReasoningLevel);
            projectTextBox.Clear();
            apiKeyTextBox.Clear();
            sessionCredentialScope = null;
            rememberKeyCheckBox.Checked = false;
            providerNoteLabel.Text = GetProviderNote(preset);
            UpdateAuthenticationControls();
        }

        private string GetProviderNote(ApiProviderPreset preset)
        {
            if (preset == null)
                return string.Empty;
            return string.IsNullOrWhiteSpace(preset.NoteKey)
                ? preset.Note
                : L(preset.NoteKey);
        }

        private void RefreshProviderNote()
        {
            if (providerNoteLabel == null)
                return;

            ApiProfileRecord profile = SelectedApiProfile;
            if (profile != null)
            {
                providerNoteLabel.Text = UiStrings.Format(
                    languageCode, "Api.ProfileLoadedFormat", profile.Name) +
                    Environment.NewLine + L("Settings.ProviderNote");
                return;
            }

            ApiProviderPreset preset = providerComboBox == null
                ? null
                : providerComboBox.SelectedItem as ApiProviderPreset;
            providerNoteLabel.Text = preset == null
                ? L("Settings.ProviderNote")
                : GetProviderNote(preset);
        }

        private void RefreshSavedProfileList()
        {
            if (savedProfileComboBox == null)
                return;

            loadingApiProfile = true;
            savedProfileComboBox.Items.Clear();
            List<ApiProfileRecord> ordered = savedApiProfiles
                .Where(profile => profile != null)
                .OrderByDescending(profile => profile.LastUsedUtc)
                .ThenBy(profile => profile.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
            foreach (ApiProfileRecord profile in ordered)
                savedProfileComboBox.Items.Add(profile);

            if (ordered.Count == 0)
            {
                savedProfileComboBox.SelectedIndex = -1;
                loadingApiProfile = false;
                if (providerComboBox != null && providerComboBox.Items.Count > 0)
                    providerComboBox.SelectedIndex = 0;
                return;
            }

            savedProfileComboBox.SelectedIndex = 0;
            loadingApiProfile = false;
            LoadApiProfile(ordered[0]);
        }

        private void SavedProfileSelectionChanged(object sender, EventArgs e)
        {
            if (loadingApiProfile)
                return;

            ApiProfileRecord profile = SelectedApiProfile;
            if (profile != null)
                LoadApiProfile(profile);
        }

        private void LoadApiProfile(ApiProfileRecord profile)
        {
            if (profile == null)
                return;

            loadingApiProfile = true;
            try
            {
                if (providerComboBox != null)
                    providerComboBox.SelectedIndex = -1;
                baseUrlTextBox.Text = profile.BaseUrl ?? string.Empty;
                protocolComboBox.SelectedIndex = profile.Protocol == ApiProtocol.ChatCompletions ? 1 : 0;
                authenticationComboBox.SelectedIndex = GetAuthenticationIndex(profile.AuthenticationMode);
                modelTextBox.Text = profile.Model ?? string.Empty;
                reasoningComboBox.SelectedIndex = GetReasoningIndex(profile.ReasoningLevel);
                projectTextBox.Text = profile.ProjectId ?? string.Empty;
                rememberKeyCheckBox.Checked = profile.RememberApiKey;
                apiKeyTextBox.Text = string.Empty;
                try
                {
                    if (profile.RememberApiKey)
                    {
                        string storedKey = new WindowsCredentialStore(
                            ApiProfileStore.CredentialTargetFor(profile)).Read();
                        if (!string.IsNullOrWhiteSpace(storedKey))
                            apiKeyTextBox.Text = storedKey;
                    }
                }
                catch (Exception ex)
                {
                    credentialStatusLabel.Text = UiStrings.Format(languageCode, "Api.CredentialReadFailedFormat", ex.Message);
                }

                sessionCredentialScope = string.IsNullOrWhiteSpace(apiKeyTextBox.Text)
                    ? null
                    : GetCurrentCredentialScope();

                RefreshProviderNote();
            }
            finally
            {
                loadingApiProfile = false;
            }

            UpdateAuthenticationControls();
            LoadCredentialStatus();
        }

        private ApiProfileRecord BuildCurrentProfileRecord(string name)
        {
            return new ApiProfileRecord
            {
                Name = name == null ? string.Empty : name.Trim(),
                BaseUrl = baseUrlTextBox.Text == null ? string.Empty : baseUrlTextBox.Text.Trim(),
                Protocol = protocolComboBox.SelectedIndex == 1 ? ApiProtocol.ChatCompletions : ApiProtocol.Responses,
                AuthenticationMode = GetAuthenticationMode(),
                Model = modelTextBox.Text == null ? string.Empty : modelTextBox.Text.Trim(),
                ProjectId = projectTextBox.Text == null ? string.Empty : projectTextBox.Text.Trim(),
                ReasoningLevel = GetReasoningLevel(),
                RememberApiKey = rememberKeyCheckBox.Checked,
                LastUsedUtc = DateTime.UtcNow
            };
        }

        private void SaveApiProfile(object sender, EventArgs e)
        {
            string defaultName = SelectedApiProfile == null ? string.Empty : SelectedApiProfile.Name;
            if (string.IsNullOrWhiteSpace(defaultName))
            {
                var preset = providerComboBox.SelectedItem as ApiProviderPreset;
                defaultName = preset == null ? L("App.Name") : preset.Name;
                if (preset != null && !string.IsNullOrWhiteSpace(preset.DisplayName))
                    defaultName = preset.DisplayName;
            }

            string name = PromptForText(L("Button.SaveProfile"), L("Api.ProfileNamePrompt"), defaultName);
            if (string.IsNullOrWhiteSpace(name))
                return;
            name = name.Trim();
            if (name.Length > 80)
            {
                ShowError(L("Api.ProfileNameTooLong"));
                return;
            }

            ApiProfileRecord profile = BuildCurrentProfileRecord(name);
            if (string.IsNullOrWhiteSpace(profile.BaseUrl) || string.IsNullOrWhiteSpace(profile.Model))
            {
                ShowError(L("Api.ProfileEndpointModelRequired"));
                return;
            }

            ApiProfileRecord existing = savedApiProfiles.FirstOrDefault(item =>
                string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));
            if (existing != null && !ApiProfileStore.MatchesConnection(
                existing,
                profile.BaseUrl,
                profile.Protocol,
                profile.AuthenticationMode,
                profile.Model))
            {
                TryDeleteCredential(ApiProfileStore.CredentialTargetFor(existing));
            }
            if (existing != null)
                savedApiProfiles.Remove(existing);
            savedApiProfiles.Add(profile);
            apiProfileStore.Save(savedApiProfiles);

            if (profile.AuthenticationMode != ApiAuthenticationMode.None &&
                profile.RememberApiKey && !string.IsNullOrWhiteSpace(apiKeyTextBox.Text))
            {
                new WindowsCredentialStore(ApiProfileStore.CredentialTargetFor(profile)).Write(apiKeyTextBox.Text);
                credentialStatusLabel.Text = L("Api.KeySaved");
            }
            else
            {
                if (!profile.RememberApiKey || profile.AuthenticationMode == ApiAuthenticationMode.None)
                    TryDeleteCredential(ApiProfileStore.CredentialTargetFor(profile));
                credentialStatusLabel.Text = UiStrings.Format(languageCode, "Api.ProfileSavedFormat", profile.Name);
            }
            TryDeleteCredential(ApiProfileStore.LegacyCredentialTargetForProfileName(profile.Name));

            loadingApiProfile = true;
            RefreshSavedProfileList();
            for (int i = 0; i < savedProfileComboBox.Items.Count; i++)
            {
                var item = savedProfileComboBox.Items[i] as ApiProfileRecord;
                if (item != null && string.Equals(item.Name, profile.Name, StringComparison.OrdinalIgnoreCase))
                {
                    savedProfileComboBox.SelectedIndex = i;
                    break;
                }
            }
            loadingApiProfile = false;
            LoadApiProfile(profile);
        }

        private void DeleteApiProfile(object sender, EventArgs e)
        {
            ApiProfileRecord profile = SelectedApiProfile;
            if (profile == null)
            {
                ShowError(L("Api.ProfileMissing"));
                return;
            }

            DialogResult result = MessageBox.Show(this,
                 UiStrings.Format(languageCode, "Dialog.ConfirmDeleteProfile") + Environment.NewLine + profile.Name,
                 L("Dialog.Warning"), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (result != DialogResult.OK)
                return;

            savedApiProfiles.RemoveAll(item => string.Equals(item.Name, profile.Name, StringComparison.OrdinalIgnoreCase));
            apiProfileStore.Save(savedApiProfiles);
            try
            {
                new WindowsCredentialStore(ApiProfileStore.CredentialTargetFor(profile)).Delete();
                new WindowsCredentialStore(
                    ApiProfileStore.LegacyCredentialTargetForProfileName(profile.Name)).Delete();
            }
            catch (Exception ex)
            {
                ShowError(L("Api.ProfileDeleted") + ": " + ex.Message);
            }
            RefreshSavedProfileList();
        }

        private string PromptForText(string title, string labelText, string initialValue)
        {
            using (var prompt = new Form())
            {
                prompt.Text = title;
                prompt.StartPosition = FormStartPosition.CenterParent;
                prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
                prompt.MinimizeBox = false;
                prompt.MaximizeBox = false;
                prompt.ShowInTaskbar = false;
                prompt.ClientSize = new Drawing.Size(440, 128);

                var label = new Label { Text = labelText, AutoSize = true, Location = new Drawing.Point(12, 14) };
                var textBox = new TextBox { Text = initialValue ?? string.Empty, Location = new Drawing.Point(12, 40), Width = 416 };
                var ok = new Button
                {
                    Text = L("Button.Confirm"), DialogResult = DialogResult.OK,
                    Location = new Drawing.Point(264, 80), AutoSize = true,
                    MinimumSize = new Drawing.Size(78, 28)
                };
                var cancel = new Button
                {
                    Text = L("Button.Cancel"), DialogResult = DialogResult.Cancel,
                    Location = new Drawing.Point(350, 80), AutoSize = true,
                    MinimumSize = new Drawing.Size(78, 28)
                };
                prompt.Controls.Add(label);
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(ok);
                prompt.Controls.Add(cancel);
                prompt.AcceptButton = ok;
                prompt.CancelButton = cancel;
                PluginTheme.Apply(prompt);
                PluginTheme.ApplyButton(ok, PluginButtonStyle.Primary);
                PluginTheme.ApplyButton(cancel, PluginButtonStyle.Secondary);
                prompt.Shown += delegate { textBox.SelectAll(); textBox.Focus(); };
                return prompt.ShowDialog(this) == DialogResult.OK ? textBox.Text : null;
            }
        }

        private void MarkCurrentProfileUsed()
        {
            ApiProfileRecord selected = SelectedApiProfile;
            if (selected == null)
                return;

            string sessionApiKey = apiKeyTextBox.Text;
            ApiProfileRecord updated = BuildCurrentProfileRecord(selected.Name);
            if (!ApiProfileStore.MatchesConnection(
                selected,
                updated.BaseUrl,
                updated.Protocol,
                updated.AuthenticationMode,
                updated.Model))
            {
                TryDeleteCredential(ApiProfileStore.CredentialTargetFor(selected));
            }
            savedApiProfiles.RemoveAll(item => string.Equals(item.Name, selected.Name, StringComparison.OrdinalIgnoreCase));
            savedApiProfiles.Add(updated);
            apiProfileStore.Save(savedApiProfiles);
            RefreshSavedProfileList();
            if (!updated.RememberApiKey && !string.IsNullOrWhiteSpace(sessionApiKey))
            {
                apiKeyTextBox.Text = sessionApiKey;
                LoadCredentialStatus();
            }
        }

        private void AuthenticationSelectionChanged(object sender, EventArgs e)
        {
            ConnectionSettingsChanged(sender, e);
            UpdateAuthenticationControls();
        }

        private void SessionApiKeyChanged(object sender, EventArgs e)
        {
            if (loadingApiProfile || apiKeyTextBox == null)
                return;

            sessionCredentialScope = string.IsNullOrWhiteSpace(apiKeyTextBox.Text)
                ? null
                : GetCurrentCredentialScope();
        }

        private void ConnectionSettingsChanged(object sender, EventArgs e)
        {
            if (loadingApiProfile || apiKeyTextBox == null)
                return;

            // A key loaded for one profile/endpoint must not remain in the
            // session field after the operator changes connection settings.
            // The user can still enter a new key immediately afterwards.
            if (!string.IsNullOrWhiteSpace(apiKeyTextBox.Text))
                apiKeyTextBox.Clear();
            sessionCredentialScope = null;

            if (authenticationComboBox != null &&
                GetAuthenticationMode() != ApiAuthenticationMode.None)
                LoadCredentialStatus();
        }

        private string GetCurrentCredentialScope()
        {
            string endpointTarget = ApiProfileStore.CredentialTargetForEndpoint(
                baseUrlTextBox == null ? string.Empty : baseUrlTextBox.Text,
                protocolComboBox != null && protocolComboBox.SelectedIndex == 1
                    ? ApiProtocol.ChatCompletions
                    : ApiProtocol.Responses,
                GetAuthenticationMode(),
                modelTextBox == null ? string.Empty : modelTextBox.Text);
            string profileName = SelectedApiProfile == null
                ? string.Empty
                : SelectedApiProfile.Name ?? string.Empty;
            string protocol = protocolComboBox == null
                ? ApiProtocol.Responses.ToString()
                : (protocolComboBox.SelectedIndex == 1
                    ? ApiProtocol.ChatCompletions
                    : ApiProtocol.Responses).ToString();
            string authentication = authenticationComboBox == null
                ? ApiAuthenticationMode.Bearer.ToString()
                : GetAuthenticationMode().ToString();
            return profileName + "\n" + endpointTarget + "\n" + protocol + "\n" + authentication;
        }

        private static void TryDeleteCredential(string target)
        {
            if (string.IsNullOrWhiteSpace(target))
                return;
            try
            {
                new WindowsCredentialStore(target).Delete();
            }
            catch
            {
                // Credential cleanup must not prevent saving a valid profile.
            }
        }

        private bool CurrentProfileMatchesConnection(ApiProfileRecord profile)
        {
            if (profile == null)
                return false;

            ApiProtocol protocol = protocolComboBox != null && protocolComboBox.SelectedIndex == 1
                ? ApiProtocol.ChatCompletions
                : ApiProtocol.Responses;
            return ApiProfileStore.MatchesConnection(
                profile,
                baseUrlTextBox == null ? string.Empty : baseUrlTextBox.Text,
                protocol,
                GetAuthenticationMode(),
                modelTextBox == null ? string.Empty : modelTextBox.Text);
        }

        private void UpdateAuthenticationControls()
        {
            if (authenticationComboBox == null || apiKeyTextBox == null)
                return;

            bool requiresKey = GetAuthenticationMode() != ApiAuthenticationMode.None;
            apiKeyTextBox.Enabled = requiresKey;
            rememberKeyCheckBox.Enabled = requiresKey;
            if (!requiresKey)
            {
                rememberKeyCheckBox.Checked = false;
                credentialStatusLabel.Text = L("Api.CredentialNone");
            }
            else
            {
                LoadCredentialStatus();
            }
        }

        private static int GetAuthenticationIndex(ApiAuthenticationMode mode)
        {
            if (mode == ApiAuthenticationMode.ApiKeyHeader)
                return 1;
            if (mode == ApiAuthenticationMode.None)
                return 2;
            return 0;
        }

        private ApiAuthenticationMode GetAuthenticationMode()
        {
            if (authenticationComboBox == null || authenticationComboBox.SelectedIndex < 0)
                return ApiAuthenticationMode.Bearer;
            if (authenticationComboBox.SelectedIndex == 1)
                return ApiAuthenticationMode.ApiKeyHeader;
            if (authenticationComboBox.SelectedIndex == 2)
                return ApiAuthenticationMode.None;
            return ApiAuthenticationMode.Bearer;
        }

        private ApiConnectionSettings BuildConnectionSettings(out string credentialSource)
        {
            ApiAuthenticationMode authenticationMode = GetAuthenticationMode();
            string apiKey = null;
            if (authenticationMode == ApiAuthenticationMode.None)
                credentialSource = L("Api.SourceNone");
            else
                apiKey = ResolveApiKey(out credentialSource);

            var settings = new ApiConnectionSettings
            {
                BaseUrl = baseUrlTextBox.Text,
                Protocol = protocolComboBox.SelectedIndex == 1
                    ? ApiProtocol.ChatCompletions
                    : ApiProtocol.Responses,
                AuthenticationMode = authenticationMode,
                Model = modelTextBox.Text,
                ApiKey = apiKey,
                ProjectId = projectTextBox.Text,
                ReasoningLevel = GetReasoningLevel(),
                DisplayLanguageCode = languageCode
            };
            settings.Validate();
            return settings;
        }

        private async void TestConnection(object sender, EventArgs e)
        {
            ApiConnectionSettings settings;
            string source;
            try
            {
                settings = BuildConnectionSettings(out source);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                return;
            }

            testConnectionButton.Enabled = false;
            SetBusy(true, "Status.TestingConnection");
            credentialStatusLabel.Text = L("Status.TestingConnection");
            bool connectionSucceeded = false;
            bool connectionCancelled = false;
            testingConnection = true;
            cancellation = new CancellationTokenSource();
            try
            {
                using (var client = new OpenAiResponsesClient(languageCode, ReportApiActivity))
                {
                    try
                    {
                        await client.TestConnectionAsync(settings, cancellation.Token);
                    }
                    finally
                    {
                        lastResponseData = client.LastResponseData;
                        viewResponseButton.Enabled = lastResponseData != null;
                    }
                }

                if (settings.AuthenticationMode != ApiAuthenticationMode.None && rememberKeyCheckBox.Checked)
                {
                    WriteCurrentCredential(settings.ApiKey);
                    credentialStatusLabel.Text = L("Api.KeySaved");
                }
                else
                {
                    credentialStatusLabel.Text = UiStrings.Format(
                        languageCode, "Api.TestSuccessFormat", source);
                }
                connectionSucceeded = true;
                MarkCurrentProfileUsed();
            }
            catch (OperationCanceledException)
            {
                connectionCancelled = true;
                credentialStatusLabel.Text = L("Status.Cancelled");
            }
            catch (Exception ex)
            {
                credentialStatusLabel.Text = L("Status.ConnectionFailed");
                ShowError(ex.Message);
            }
            finally
            {
                if (cancellation != null)
                {
                    cancellation.Dispose();
                    cancellation = null;
                }
                testingConnection = false;
                testConnectionButton.Enabled = true;
                CompleteBusyOperation(connectionCancelled
                    ? "Status.Cancelled"
                    : connectionSucceeded
                        ? "Status.ConnectionSucceeded"
                        : "Status.ConnectionFailed");
            }
        }

        private string ResolveApiKey(out string source)
        {
            if (!string.IsNullOrWhiteSpace(apiKeyTextBox.Text) &&
                (string.IsNullOrWhiteSpace(sessionCredentialScope) ||
                 string.Equals(sessionCredentialScope, GetCurrentCredentialScope(),
                     StringComparison.Ordinal)))
            {
                source = L("Api.SourceSession");
                return apiKeyTextBox.Text.Trim();
            }

            string storedKey = null;
            ApiProfileRecord profile = SelectedApiProfile;
            if (profile != null && profile.RememberApiKey &&
                CurrentProfileMatchesConnection(profile))
                storedKey = new WindowsCredentialStore(ApiProfileStore.CredentialTargetFor(profile)).Read();
            if ((profile == null || !CurrentProfileMatchesConnection(profile)) &&
                rememberKeyCheckBox != null && rememberKeyCheckBox.Checked)
            {
                storedKey = new WindowsCredentialStore(ApiProfileStore.CredentialTargetForEndpoint(
                    baseUrlTextBox == null ? string.Empty : baseUrlTextBox.Text,
                    protocolComboBox != null && protocolComboBox.SelectedIndex == 1
                        ? ApiProtocol.ChatCompletions
                        : ApiProtocol.Responses,
                    GetAuthenticationMode(),
                    modelTextBox == null ? string.Empty : modelTextBox.Text)).Read();
            }
            if (!string.IsNullOrWhiteSpace(storedKey))
            {
                source = L("Api.SourceWindows");
                return storedKey.Trim();
            }

            if (profile == null && IsOfficialOpenAiEndpoint())
            {
                string environmentKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
                if (!string.IsNullOrWhiteSpace(environmentKey))
                {
                    source = L("Api.SourceEnvironment");
                    return environmentKey.Trim();
                }

                storedKey = credentialStore.Read();
                if (!string.IsNullOrWhiteSpace(storedKey))
                {
                    source = L("Api.SourceWindows");
                    return storedKey.Trim();
                }
            }

            source = L("Api.SourceNone");
            return null;
        }

        private bool IsOfficialOpenAiEndpoint()
        {
            Uri endpoint;
            return baseUrlTextBox != null &&
                   Uri.TryCreate(baseUrlTextBox.Text, UriKind.Absolute, out endpoint) &&
                   string.Equals(endpoint.Host, "api.openai.com", StringComparison.OrdinalIgnoreCase);
        }

        private void WriteCurrentCredential(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                return;

            ApiProfileRecord profile = SelectedApiProfile;
            if (profile != null)
                new WindowsCredentialStore(ApiProfileStore.CredentialTargetFor(
                    BuildCurrentProfileRecord(profile.Name))).Write(apiKey);
            else
                new WindowsCredentialStore(ApiProfileStore.CredentialTargetForEndpoint(
                    baseUrlTextBox.Text,
                    protocolComboBox != null && protocolComboBox.SelectedIndex == 1
                        ? ApiProtocol.ChatCompletions
                        : ApiProtocol.Responses,
                    GetAuthenticationMode(),
                    modelTextBox.Text)).Write(apiKey);
        }

        private void LoadCredentialStatus()
        {
            if (authenticationComboBox != null && GetAuthenticationMode() == ApiAuthenticationMode.None)
            {
                credentialStatusLabel.Text = L("Api.CredentialNone");
                return;
            }

            try
            {
                string source;
                string apiKey = ResolveApiKey(out source);
                credentialStatusLabel.Text = string.IsNullOrWhiteSpace(apiKey)
                    ? L("Api.CredentialMissing")
                    : UiStrings.Format(languageCode, "Api.CredentialFoundFormat", source);
            }
            catch (Exception ex)
            {
                credentialStatusLabel.Text = UiStrings.Format(
                    languageCode, "Api.CredentialReadFailedFormat", ex.Message);
            }
        }

        private void DeleteStoredCredential(object sender, EventArgs e)
        {
            try
            {
                ApiProfileRecord profile = SelectedApiProfile;
                bool useProfileCredential = profile != null && CurrentProfileMatchesConnection(profile);
                bool deleted = !useProfileCredential
                    ? new WindowsCredentialStore(ApiProfileStore.CredentialTargetForEndpoint(
                        baseUrlTextBox.Text,
                        protocolComboBox != null && protocolComboBox.SelectedIndex == 1
                            ? ApiProtocol.ChatCompletions
                            : ApiProtocol.Responses,
                        GetAuthenticationMode(),
                        modelTextBox.Text)).Delete()
                    : new WindowsCredentialStore(ApiProfileStore.CredentialTargetFor(profile)).Delete();
                credentialStatusLabel.Text = deleted ? L("Api.KeyDeleted") : L("Api.NoSavedKey");
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
    }
}
