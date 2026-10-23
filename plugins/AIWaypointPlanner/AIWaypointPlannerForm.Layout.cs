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
        public void ApplyPluginTheme()
        {
            if (IsDisposed || Disposing)
                return;

            SuspendLayout();
            try
            {
                PluginTheme.Apply(this);

                if (conversationPanel != null)
                    conversationPanel.BackColor = PluginTheme.InputBackground;
                if (activityLabel != null)
                    activityLabel.ForeColor = PluginTheme.SecondaryText;
                if (providerNoteLabel != null)
                    providerNoteLabel.ForeColor = PluginTheme.SecondaryText;

                foreach (KeyValuePair<Control, string> entry in localizationKeys.ToArray())
                {
                    if (entry.Key == null || entry.Key.IsDisposed)
                        continue;
                    if (entry.Key is Label && IsMutedLocalizationKey(entry.Value))
                        entry.Key.ForeColor = PluginTheme.SecondaryText;
                }

                if (conversationPanel != null)
                {
                    foreach (ConversationMessageControl message in conversationPanel.Controls
                        .OfType<ConversationMessageControl>())
                        message.ApplyPluginTheme();
                }

                if (pendingStatusPanel != null && !pendingStatusPanel.IsDisposed)
                {
                    PluginTheme.Apply(pendingStatusPanel);
                    pendingStatusPanel.BackColor = PluginTheme.RaisedSurface;
                }

                if (attachmentChipPanel != null)
                {
                    foreach (Control control in attachmentChipPanel.Controls)
                    {
                        PluginTheme.Apply(control);
                        if (control is FlowLayoutPanel chip && chip.Tag is MissionAttachment)
                            chip.BackColor = PluginTheme.RaisedSurface;
                        foreach (Button remove in control.Controls.OfType<Button>())
                            PluginTheme.ApplyButton(remove, PluginButtonStyle.Danger);
                    }
                }

                PluginTheme.ApplyButton(generateButton, PluginButtonStyle.Primary);
                PluginTheme.ApplyButton(applyButton, PluginButtonStyle.Primary);
                PluginTheme.ApplyButton(cancelButton, PluginButtonStyle.Danger);
                PluginTheme.ApplyButton(deleteCredentialButton, PluginButtonStyle.Danger);
                PluginTheme.ApplyButton(deleteProfileButton, PluginButtonStyle.Danger);
                ApplySafetyNoticeStyle();
            }
            finally
            {
                ResumeLayout(true);
                Invalidate(true);
            }
        }

        private static bool IsMutedLocalizationKey(string key)
        {
            return !string.IsNullOrWhiteSpace(key) &&
                   (key.EndsWith("Help", StringComparison.Ordinal) ||
                    string.Equals(key, "Api.RemoteHttpsRequired", StringComparison.Ordinal));
        }

        private void BuildInterface()
        {
            Text = L("App.Title");
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Drawing.Size(820, 600);
            Size = new Drawing.Size(1180, 760);
            Font = Drawing.SystemFonts.MessageBoxFont;
            AutoScaleMode = AutoScaleMode.Font;
            KeyPreview = true;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(10)
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            Controls.Add(root);

            safetyNotice = new Label
            {
                Dock = DockStyle.Top,
                AutoSize = false,
                MinimumSize = new Drawing.Size(0, 50),
                MaximumSize = new Drawing.Size(760, 0),
                Padding = new Padding(12, 9, 12, 9),
                TextAlign = Drawing.ContentAlignment.MiddleLeft,
                UseMnemonic = false
            };
            Track(safetyNotice, "Safety.Banner");
            ApplySafetyNoticeStyle();
            root.Controls.Add(safetyNotice, 0, 0);
            root.SizeChanged += delegate { UpdateSafetyNoticeLayout(); };

            workspaceTabs = new PluginTabControl
            {
                Dock = DockStyle.Fill,
                Padding = new Drawing.Point(16, 6)
            };
            chatTab = BuildChatTab();
            reviewTab = BuildReviewTab();
            settingsTab = BuildSettingsTab();
            workspaceTabs.TabPages.Add(chatTab);
            workspaceTabs.TabPages.Add(reviewTab);
            workspaceTabs.TabPages.Add(settingsTab);
            root.Controls.Add(workspaceTabs, 0, 1);

            statusAnimationTimer = new System.Windows.Forms.Timer { Interval = 420 };
            statusAnimationTimer.Tick += AnimateStatus;
        }

        private TabPage BuildChatTab()
        {
            var tab = new TabPage { Padding = new Padding(0) };
            Track(tab, "Nav.Chat");
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(12)
            };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tab.Controls.Add(layout);

            var header = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0, 0, 0, 8)
            };
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            activityLabel = new Label
            {
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Padding = new Padding(2, 8, 12, 0)
            };
            activityLabel.Text = L("Status.Idle");
            var headerActions = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Anchor = AnchorStyles.Right
            };
            newChatButton = Track(new Button { AutoSize = true, Height = 32 }, "Button.NewChat");
            newChatButton.Click += StartNewConversation;
            chatReviewButton = Track(new Button { AutoSize = true, Height = 32, Enabled = false }, "Button.ReviewMission");
            chatReviewButton.Click += delegate { workspaceTabs.SelectedTab = reviewTab; };
            chatDiagnosticsButton = Track(new Button { AutoSize = true, Height = 32, Enabled = false }, "Nav.Diagnostics");
            chatDiagnosticsButton.Click += ViewModelResponse;
            settingsShortcutButton = Track(new Button { AutoSize = true, Height = 32 }, "Button.Settings");
            settingsShortcutButton.Click += delegate { workspaceTabs.SelectedTab = settingsTab; };
            closeButton = Track(new Button { AutoSize = true, Height = 32 }, "Button.Close");
            closeButton.Click += delegate { Close(); };
            headerActions.Controls.Add(newChatButton);
            headerActions.Controls.Add(chatReviewButton);
            headerActions.Controls.Add(chatDiagnosticsButton);
            headerActions.Controls.Add(settingsShortcutButton);
            headerActions.Controls.Add(closeButton);
            header.Controls.Add(activityLabel, 0, 0);
            header.Controls.Add(headerActions, 1, 0);
            layout.Controls.Add(header, 0, 0);

            conversationPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(12),
                BackColor = PluginTheme.InputBackground
            };
            conversationPanel.ClientSizeChanged += delegate { ResizeConversationItems(); };
            conversationPanel.Controls.Add(BuildWelcomePanel());
            layout.Controls.Add(conversationPanel, 0, 1);

            var attachmentStrip = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 3,
                RowCount = 1,
                Padding = new Padding(4, 5, 4, 5),
                Margin = new Padding(0, 8, 0, 6)
            };
            attachmentStrip.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            attachmentStrip.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            attachmentStrip.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            attachmentHeaderLabel = Track(new Label
            {
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Padding = new Padding(0, 7, 10, 0)
            }, "Attachment.Title");
            attachmentChipPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Margin = Padding.Empty
            };
            clearAttachmentsButton = Track(new Button { AutoSize = true, Height = 30, Enabled = false }, "Button.Clear");
            clearAttachmentsButton.Click += ClearAttachments;
            attachmentStrip.Controls.Add(attachmentHeaderLabel, 0, 0);
            attachmentStrip.Controls.Add(attachmentChipPanel, 1, 0);
            attachmentStrip.Controls.Add(clearAttachmentsButton, 2, 0);
            layout.Controls.Add(attachmentStrip, 0, 2);

            var composer = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(8),
                BackColor = PluginTheme.Surface
            };
            composer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            composer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            objectiveTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                AcceptsReturn = true,
                MinimumSize = new Drawing.Size(320, 86)
            };
            objectiveTextBox.KeyDown += ObjectiveKeyDown;
            var composerActions = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(8, 0, 0, 0)
            };
            addAttachmentButton = Track(new Button { AutoSize = true, MinimumSize = new Drawing.Size(118, 32) }, "Button.Attach");
            addAttachmentButton.Click += AddAttachments;
            generateButton = Track(new Button { AutoSize = true, MinimumSize = new Drawing.Size(118, 36) }, "Button.Send");
            generateButton.Click += GenerateMission;
            cancelButton = Track(new Button { AutoSize = true, MinimumSize = new Drawing.Size(118, 32), Enabled = false }, "Button.Stop");
            cancelButton.Click += delegate { if (cancellation != null) cancellation.Cancel(); };
            composerActions.Controls.Add(addAttachmentButton);
            composerActions.Controls.Add(generateButton);
            composerActions.Controls.Add(cancelButton);
            composer.Controls.Add(objectiveTextBox, 0, 0);
            composer.Controls.Add(composerActions, 1, 0);
            layout.Controls.Add(composer, 0, 3);
            return tab;
        }

        private Control BuildWelcomePanel()
        {
            var panel = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(18, 22, 18, 18),
                Margin = new Padding(0, 0, 0, 12),
                BackColor = PluginTheme.Surface
            };
            var title = Track(new Label
            {
                AutoSize = true,
                Font = new Drawing.Font(Font.FontFamily, 15F, Drawing.FontStyle.Bold),
                Margin = new Padding(0, 0, 0, 8)
            }, "Chat.WelcomeTitle");
            var body = Track(new Label
            {
                AutoSize = true,
                MaximumSize = new Drawing.Size(760, 0),
                Margin = new Padding(0, 0, 0, 14),
                UseMnemonic = false
            }, "Chat.WelcomeBody");
            var suggestions = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Margin = Padding.Empty
            };
            var relative = Track(CreateSuggestionButton(), "TaskSuggestions.RelativeRoute");
            relative.Click += delegate { SetSuggestedTask("relative"); };
            var survey = Track(CreateSuggestionButton(), "TaskSuggestions.SurveyPolygon");
            survey.Click += delegate { SetSuggestedTask("survey"); };
            var fromFile = Track(CreateSuggestionButton(), "TaskSuggestions.FromFile");
            fromFile.Click += delegate { SetSuggestedTask("file"); };
            UpdateSuggestionButtonLayout(relative);
            UpdateSuggestionButtonLayout(survey);
            UpdateSuggestionButtonLayout(fromFile);
            suggestions.Controls.Add(relative);
            suggestions.Controls.Add(survey);
            suggestions.Controls.Add(fromFile);
            panel.Controls.Add(title, 0, 0);
            panel.Controls.Add(body, 0, 1);
            panel.Controls.Add(suggestions, 0, 2);
            PluginTheme.Apply(panel);
            panel.BackColor = PluginTheme.Surface;
            return panel;
        }

        private static Button CreateSuggestionButton()
        {
            return new Button
            {
                AutoSize = false,
                Size = new Drawing.Size(310, 60),
                MinimumSize = new Drawing.Size(180, 44),
                MaximumSize = new Drawing.Size(310, 110),
                Padding = new Padding(10, 5, 10, 5),
                TextAlign = Drawing.ContentAlignment.MiddleLeft,
                UseMnemonic = false
            };
        }

        private void UpdateSuggestionButtonLayouts()
        {
            foreach (KeyValuePair<Control, string> entry in localizationKeys.ToArray())
            {
                Button button = entry.Key as Button;
                if (button == null || button.IsDisposed ||
                    !entry.Value.StartsWith("TaskSuggestions.", StringComparison.Ordinal))
                    continue;
                UpdateSuggestionButtonLayout(button);
            }
        }

        private static void UpdateSuggestionButtonLayout(Button button)
        {
            if (button == null)
                return;

            int width = button.MaximumSize.Width > 0
                ? button.MaximumSize.Width
                : Math.Max(button.MinimumSize.Width, button.Width);
            int textWidth = Math.Max(80, width - button.Padding.Horizontal - 8);
            Drawing.Size measured = TextRenderer.MeasureText(
                button.Text ?? string.Empty,
                button.Font,
                new Drawing.Size(textWidth, int.MaxValue),
                TextFormatFlags.WordBreak | TextFormatFlags.Left |
                TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding);
            int height = Math.Max(button.MinimumSize.Height,
                measured.Height + button.Padding.Vertical + 12);
            if (button.MaximumSize.Height > 0)
                height = Math.Min(height, button.MaximumSize.Height);
            button.Size = new Drawing.Size(width, height);
        }

        private TabPage BuildReviewTab()
        {
            var tab = new TabPage { Padding = new Padding(10) };
            Track(tab, "Nav.MissionReview");
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 18F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 52F));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tab.Controls.Add(layout);

            var summaryGroup = Track(new GroupBox { Dock = DockStyle.Fill }, "Mission.Understanding");
            summaryTextBox = CreateReadOnlyTextBox();
            summaryGroup.Controls.Add(summaryTextBox);
            layout.Controls.Add(summaryGroup, 0, 0);

            var validationGroup = Track(new GroupBox { Dock = DockStyle.Fill }, "Mission.Validation");
            validationTextBox = CreateReadOnlyTextBox();
            validationGroup.Controls.Add(validationTextBox);
            layout.Controls.Add(validationGroup, 0, 1);

            var candidateGroup = Track(new GroupBox { Dock = DockStyle.Fill }, "Mission.Items");
            candidateGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoGenerateColumns = false,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            candidateGrid.Columns.Add(new DataGridViewTextBoxColumn { Width = 48, HeaderText = L("Mission.Sequence") });
            candidateGrid.Columns.Add(new DataGridViewTextBoxColumn { Width = 150, HeaderText = L("Mission.Command") });
            candidateGrid.Columns.Add(new DataGridViewTextBoxColumn { Width = 100, HeaderText = L("Mission.Latitude") });
            candidateGrid.Columns.Add(new DataGridViewTextBoxColumn { Width = 100, HeaderText = L("Mission.Longitude") });
            candidateGrid.Columns.Add(new DataGridViewTextBoxColumn { Width = 78, HeaderText = L("Mission.Altitude") });
            candidateGrid.Columns.Add(new DataGridViewTextBoxColumn { Width = 68, HeaderText = L("Mission.Parameter1") });
            candidateGrid.Columns.Add(new DataGridViewTextBoxColumn { Width = 68, HeaderText = L("Mission.Parameter2") });
            candidateGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 130,
                HeaderText = L("Mission.Description")
            });
            candidateGroup.Controls.Add(candidateGrid);
            layout.Controls.Add(candidateGroup, 0, 2);

            var reviewActions = new TableLayoutPanel
            {
                AutoSize = true,
                Dock = DockStyle.Top,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(4, 8, 4, 2)
            };
            confirmRequirementsCheckBox = Track(new CheckBox
            {
                AutoSize = true,
                Enabled = false,
                Padding = new Padding(4),
                UseMnemonic = false
            }, "Mission.ConfirmCheck");
            confirmRequirementsCheckBox.CheckedChanged += ConfirmRequirementsChanged;
            var buttons = new FlowLayoutPanel
            {
                AutoSize = true,
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = true
            };
            applyButton = Track(new Button { AutoSize = true, Height = 34, Enabled = false }, "Button.ApplyToPlan");
            applyButton.Click += ApplyMission;
            viewResponseButton = Track(new Button { AutoSize = true, Height = 34, Enabled = false }, "Chat.ViewDiagnostics");
            viewResponseButton.Click += ViewModelResponse;
            var back = Track(new Button { AutoSize = true, Height = 34 }, "Button.BackToChat");
            back.Click += delegate { workspaceTabs.SelectedTab = chatTab; };
            buttons.Controls.Add(applyButton);
            buttons.Controls.Add(viewResponseButton);
            buttons.Controls.Add(back);
            reviewActions.Controls.Add(confirmRequirementsCheckBox, 0, 0);
            reviewActions.Controls.Add(buttons, 0, 1);
            layout.Controls.Add(reviewActions, 0, 3);
            return tab;
        }

        private TabPage BuildSettingsTab()
        {
            var tab = new TabPage { Padding = new Padding(8) };
            Track(tab, "Nav.Settings");
            var scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(8)
            };
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 0,
                Padding = new Padding(4, 2, 18, 12)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            AddSettingsSection(layout, "Settings.Appearance");
            languageComboBox = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList };
            loadingLanguage = true;
            foreach (UiLanguageOption option in UiStrings.CreateLanguageOptions())
                languageComboBox.Items.Add(option);
            for (int i = 0; i < languageComboBox.Items.Count; i++)
            {
                var option = languageComboBox.Items[i] as UiLanguageOption;
                if (option != null && option.LanguageCode == languageCode)
                {
                    languageComboBox.SelectedIndex = i;
                    break;
                }
            }
            if (languageComboBox.SelectedIndex < 0 && languageComboBox.Items.Count > 0)
                languageComboBox.SelectedIndex = 0;
            loadingLanguage = false;
            languageComboBox.SelectedIndexChanged += LanguageSelectionChanged;
            AddSettingRow(layout, "Settings.Language", languageComboBox, "Settings.LanguageHelp");

            AddSettingsSection(layout, "Settings.Api");

            providerComboBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            savedProfileComboBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            protocolComboBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            authenticationComboBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            reasoningComboBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            baseUrlTextBox = new TextBox { Dock = DockStyle.Fill };
            modelTextBox = new TextBox { Dock = DockStyle.Fill, Text = "gpt-5.6-sol" };
            projectTextBox = new TextBox { Dock = DockStyle.Fill };
            apiKeyTextBox = new TextBox { Dock = DockStyle.Fill, UseSystemPasswordChar = true };
            baseUrlTextBox.TextChanged += ConnectionSettingsChanged;
            modelTextBox.TextChanged += ConnectionSettingsChanged;
            protocolComboBox.SelectedIndexChanged += ConnectionSettingsChanged;
            apiKeyTextBox.TextChanged += SessionApiKeyChanged;
            rememberKeyCheckBox = new CheckBox
            {
                AutoSize = true,
                Padding = new Padding(0, 6, 0, 0)
            };
            credentialStatusLabel = new Label
            {
                AutoSize = true,
                Padding = new Padding(0, 8, 0, 0)
            };
            testConnectionButton = Track(new Button { AutoSize = true, Height = 32 }, "Button.TestConnection");
            testConnectionButton.Click += TestConnection;
            deleteCredentialButton = Track(new Button { AutoSize = true, Height = 32 }, "Button.DeleteSavedKey");
            deleteCredentialButton.Click += DeleteStoredCredential;
            providerNoteLabel = new Label
            {
                AutoSize = true,
                MaximumSize = new Drawing.Size(920, 0),
                Padding = new Padding(6, 12, 6, 8),
                ForeColor = PluginTheme.SecondaryText,
                UseMnemonic = false
            };

            AddSettingRow(layout, "Settings.Provider", providerComboBox, string.Empty);
            AddSettingRow(layout, "Settings.SavedProfile", savedProfileComboBox, string.Empty);
            AddSettingRow(layout, "Settings.BaseUrl", baseUrlTextBox, "Api.RemoteHttpsRequired");
            AddSettingRow(layout, "Settings.Protocol", protocolComboBox, string.Empty);
            AddSettingRow(layout, "Settings.Authentication", authenticationComboBox, string.Empty);
            AddSettingRow(layout, "Settings.ModelId", modelTextBox, string.Empty);
            AddSettingRow(layout, "Settings.Reasoning", reasoningComboBox, "Settings.ReasoningHelp");
            AddSettingRow(layout, "Settings.ProjectId", projectTextBox, string.Empty);
            AddSettingRow(layout, "Settings.ApiKey", apiKeyTextBox, string.Empty);
            AddSettingRow(layout, "Settings.RememberKey", rememberKeyCheckBox, string.Empty);
            AddSettingRow(layout, "Settings.CredentialStatus", credentialStatusLabel, string.Empty);

            var buttons = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = true };
            buttons.Controls.Add(testConnectionButton);
            buttons.Controls.Add(deleteCredentialButton);
            saveProfileButton = Track(new Button { AutoSize = true, Height = 32 }, "Button.SaveProfile");
            saveProfileButton.Click += SaveApiProfile;
            deleteProfileButton = Track(new Button { AutoSize = true, Height = 32 }, "Button.DeleteProfile");
            deleteProfileButton.Click += DeleteApiProfile;
            buttons.Controls.Add(saveProfileButton);
            buttons.Controls.Add(deleteProfileButton);
            AddSettingRow(layout, "Settings.ConnectionActions", buttons, string.Empty);
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(providerNoteLabel, 0, layout.RowCount++);
            scrollPanel.Controls.Add(layout);
            tab.Controls.Add(scrollPanel);

            foreach (ApiProviderPreset preset in ApiProviderPreset.CreateDefaults())
            {
                preset.DisplayName = string.IsNullOrWhiteSpace(preset.NameKey)
                    ? preset.Name
                    : L(preset.NameKey);
                providerComboBox.Items.Add(preset);
            }
            providerComboBox.SelectedIndexChanged += ProviderSelectionChanged;
            savedProfileComboBox.SelectedIndexChanged += SavedProfileSelectionChanged;
            authenticationComboBox.SelectedIndexChanged += AuthenticationSelectionChanged;
            RefreshLocalizedOptionLists();
            RefreshProviderPresetNames();
            providerComboBox.SelectedIndex = 0;
            RefreshSavedProfileList();
            return tab;
        }

        private void AddSettingsSection(TableLayoutPanel layout, string key)
        {
            var heading = Track(new Label
            {
                AutoSize = true,
                Font = new Drawing.Font(Font, Drawing.FontStyle.Bold),
                Padding = new Padding(2, 16, 2, 7)
            }, key);
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(heading, 0, layout.RowCount++);
        }

        private void AddSettingRow(TableLayoutPanel layout, string labelKey, Control control, string helpKey)
        {
            var row = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0, 0, 0, 7),
                Padding = new Padding(2)
            };
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250F));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            var label = Track(new Label
            {
                AutoSize = true,
                MaximumSize = new Drawing.Size(240, 0),
                Padding = new Padding(0, 7, 12, 0),
                UseMnemonic = false
            }, labelKey);
            var value = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = string.IsNullOrEmpty(helpKey) ? 1 : 2,
                Margin = Padding.Empty
            };
            control.Dock = DockStyle.Top;
            value.Controls.Add(control, 0, 0);
            if (!string.IsNullOrEmpty(helpKey))
            {
                var help = Track(new Label
                {
                    AutoSize = true,
                    ForeColor = PluginTheme.SecondaryText,
                    Padding = new Padding(0, 4, 0, 2),
                    UseMnemonic = false
                }, helpKey);
                value.Controls.Add(help, 0, 1);
                row.SizeChanged += delegate { ConstrainSettingHelpLabel(row, value, help); };
                ConstrainSettingHelpLabel(row, value, help);
            }
            row.Controls.Add(label, 0, 0);
            row.Controls.Add(value, 1, 0);
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(row, 0, layout.RowCount++);
        }

        private static void ConstrainSettingHelpLabel(
            TableLayoutPanel row, TableLayoutPanel value, Label help)
        {
            if (row == null || value == null || help == null || help.IsDisposed)
                return;

            int labelColumnWidth = row.ColumnStyles.Count > 0
                ? (int)Math.Ceiling(row.ColumnStyles[0].Width)
                : 0;
            int availableWidth = Math.Max(120,
                row.ClientSize.Width - row.Padding.Horizontal - labelColumnWidth -
                value.Margin.Horizontal - value.Padding.Horizontal - help.Margin.Horizontal);
            if (help.MaximumSize.Width != availableWidth)
                help.MaximumSize = new Drawing.Size(availableWidth, 0);
        }

        private void ApplySafetyNoticeStyle()
        {
            if (safetyNotice == null)
                return;

            safetyNotice.BackColor = PluginTheme.SafetyBanner;
            safetyNotice.ForeColor = Drawing.Color.White;
            if (!safetyNotice.Font.Bold)
                safetyNotice.Font = new Drawing.Font(safetyNotice.Font, Drawing.FontStyle.Bold);
        }

        private void UpdateSafetyNoticeLayout()
        {
            if (safetyNotice == null || safetyNotice.Parent == null)
                return;

            int availableWidth = Math.Max(260,
                safetyNotice.Parent.ClientSize.Width - safetyNotice.Parent.Padding.Horizontal -
                safetyNotice.Margin.Horizontal);
            int textWidth = Math.Max(120, availableWidth - safetyNotice.Padding.Horizontal);
            Drawing.Size measured = TextRenderer.MeasureText(
                safetyNotice.Text ?? string.Empty,
                safetyNotice.Font,
                new Drawing.Size(textWidth, int.MaxValue),
                TextFormatFlags.WordBreak | TextFormatFlags.Left | TextFormatFlags.NoPadding);
            safetyNotice.MaximumSize = new Drawing.Size(availableWidth, 0);
            safetyNotice.Width = availableWidth;
            int desiredHeight = Math.Max(50, measured.Height + safetyNotice.Padding.Vertical);
            safetyNotice.Height = desiredHeight;

            TableLayoutPanel parent = safetyNotice.Parent as TableLayoutPanel;
            if (parent != null)
            {
                int rowIndex = parent.GetRow(safetyNotice);
                if (rowIndex >= 0 && rowIndex < parent.RowStyles.Count)
                {
                    RowStyle rowStyle = parent.RowStyles[rowIndex];
                    rowStyle.SizeType = SizeType.Absolute;
                    float requiredHeight = desiredHeight + safetyNotice.Margin.Vertical;
                    if (Math.Abs(rowStyle.Height - requiredHeight) > 0.5F)
                    {
                        rowStyle.Height = requiredHeight;
                        parent.PerformLayout();
                    }
                }
            }
        }

        private static TextBox CreateReadOnlyTextBox()
        {
            return new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = PluginTheme.InputBackground,
                ForeColor = PluginTheme.PrimaryText,
                BorderStyle = BorderStyle.FixedSingle
            };
        }
    }
}
