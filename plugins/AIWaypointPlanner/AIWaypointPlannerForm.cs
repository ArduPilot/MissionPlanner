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
    public sealed class AIWaypointPlannerForm : Form
    {
        private readonly AIWaypointPlannerPlugin plugin;
        private readonly MissionValidator validator;
        private readonly MissionCompiler compiler;
        private readonly WindowsCredentialStore credentialStore;
        private readonly ApiProfileStore apiProfileStore;
        private readonly List<ApiProfileRecord> savedApiProfiles;
        private readonly AttachmentProcessor attachmentProcessor;
        private readonly List<MissionAttachment> attachments;
        private readonly PluginPreferencesStore preferencesStore;
        private readonly MissionConversationSession conversationSession;
        private readonly Dictionary<Control, string> localizationKeys;

        private PluginPreferences preferences;
        private string languageCode;
        private bool loadingLanguage;

        private TabControl workspaceTabs;
        private TabPage chatTab;
        private TabPage reviewTab;
        private TabPage settingsTab;
        private FlowLayoutPanel conversationPanel;
        private FlowLayoutPanel attachmentChipPanel;
        private Panel pendingStatusPanel;
        private Label pendingStatusLabel;
        private Label attachmentHeaderLabel;
        private Button newChatButton;
        private Button settingsShortcutButton;
        private Button chatReviewButton;
        private Button chatDiagnosticsButton;
        private System.Windows.Forms.Timer statusAnimationTimer;
        private string activeStatusKey;
        private string activeStatusText;
        private int activeStatusAttempt;
        private int activeStatusMaximumAttempts;
        private int activeStatusDelayMilliseconds;
        private int statusAnimationFrame;

        private TextBox objectiveTextBox;
        private TextBox summaryTextBox;
        private TextBox validationTextBox;
        private DataGridView candidateGrid;
        private Button addAttachmentButton;
        private Button clearAttachmentsButton;
        private CheckBox confirmRequirementsCheckBox;
        private Button generateButton;
        private Button cancelButton;
        private Button applyButton;
        private Button viewResponseButton;
        private Button closeButton;
        private Label safetyNotice;
        private ComboBox providerComboBox;
        private ComboBox savedProfileComboBox;
        private Button saveProfileButton;
        private Button deleteProfileButton;
        private ComboBox protocolComboBox;
        private ComboBox authenticationComboBox;
        private ComboBox reasoningComboBox;
        private ComboBox languageComboBox;
        private TextBox baseUrlTextBox;
        private TextBox modelTextBox;
        private TextBox projectTextBox;
        private TextBox apiKeyTextBox;
        private CheckBox rememberKeyCheckBox;
        private Label credentialStatusLabel;
        private Button testConnectionButton;
        private Button deleteCredentialButton;
        private Label providerNoteLabel;
        private Label activityLabel;

        private CancellationTokenSource cancellation;
        private MissionGenerationResult currentResult;
        private MissionContext currentResultContext;
        private ApiResponseData lastResponseData;
        private bool loadingApiProfile;
        private bool closeWhenIdle;
        private bool testingConnection;
        // The key in the session field is deliberately scoped to the connection
        // settings that were visible when it was entered or loaded.  If an
        // operator edits the endpoint/model/protocol/authentication, the old
        // value is cleared and can never silently cross service boundaries.
        private string sessionCredentialScope;

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

        private ApiProfileRecord SelectedApiProfile
        {
            get { return savedProfileComboBox == null ? null : savedProfileComboBox.SelectedItem as ApiProfileRecord; }
        }

        public AIWaypointPlannerForm(AIWaypointPlannerPlugin plugin)
        {
            this.plugin = plugin ?? throw new ArgumentNullException("plugin");
            validator = new MissionValidator();
            compiler = new MissionCompiler();
            credentialStore = new WindowsCredentialStore();
            apiProfileStore = new ApiProfileStore();
            savedApiProfiles = new List<ApiProfileRecord>(apiProfileStore.Load());
            attachmentProcessor = new AttachmentProcessor();
            attachments = new List<MissionAttachment>();
            preferencesStore = new PluginPreferencesStore();
            preferences = preferencesStore.Load();
            languageCode = UiStrings.NormalizeLanguageCode(preferences.LanguageCode);
            conversationSession = new MissionConversationSession();
            localizationKeys = new Dictionary<Control, string>();

            BuildInterface();
            LoadCredentialStatus();
            ApplyLocalization();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && cancellation != null)
            {
                cancellation.Cancel();
                cancellation.Dispose();
                cancellation = null;
            }
            if (disposing && statusAnimationTimer != null)
            {
                statusAnimationTimer.Stop();
                statusAnimationTimer.Dispose();
                statusAnimationTimer = null;
            }
            base.Dispose(disposing);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (cancellation != null)
            {
                closeWhenIdle = true;
                cancellation.Cancel();
                e.Cancel = true;
                return;
            }

            preferences.LanguageCode = languageCode;
            try
            {
                preferencesStore.Save(preferences);
            }
            catch
            {
                // Closing Mission Planner must not be blocked by a preferences write failure.
            }
            base.OnFormClosing(e);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            ApplySafetyNoticeStyle();
            UpdateSafetyNoticeLayout();
            UpdateSuggestionButtonLayouts();
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
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
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

            workspaceTabs = new TabControl { Dock = DockStyle.Fill, Padding = new Drawing.Point(16, 6) };
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
                BackColor = Drawing.SystemColors.Window
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
                BackColor = Drawing.SystemColors.ControlLight
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
                BackColor = Drawing.SystemColors.Window
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
                ForeColor = Drawing.SystemColors.GrayText,
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
                    ForeColor = Drawing.SystemColors.GrayText,
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

            safetyNotice.BackColor = Drawing.Color.FromArgb(176, 0, 32);
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
            safetyNotice.Height = Math.Max(50, measured.Height + safetyNotice.Padding.Vertical);
        }

        private void ApplyLocalization()
        {
            languageCode = UiStrings.NormalizeLanguageCode(languageCode);
            validator.LanguageCode = languageCode;
            compiler.LanguageCode = languageCode;
            Text = L("App.Title");

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
                    message.ApplyLanguage(languageCode);
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

            if (providerNoteLabel != null && providerComboBox != null &&
                providerComboBox.SelectedItem is ApiProviderPreset preset)
            {
                providerNoteLabel.Text = GetProviderNote(preset);
            }
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

            languageCode = UiStrings.NormalizeLanguageCode(option.LanguageCode);
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

        private void ObjectiveKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                GenerateMission(sender, EventArgs.Empty);
            }
        }

        private void AnimateStatus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(activeStatusKey) || activityLabel == null)
                return;

            statusAnimationFrame = (statusAnimationFrame + 1) % 4;
            string suffix = new string('.', statusAnimationFrame);
            string statusText = string.IsNullOrWhiteSpace(activeStatusText)
                ? L(activeStatusKey)
                : activeStatusText;
            activityLabel.Text = statusText + suffix;
            if (pendingStatusLabel != null && !pendingStatusLabel.IsDisposed)
                pendingStatusLabel.Text = statusText + suffix;
        }

        private void StartNewConversation(object sender, EventArgs e)
        {
            if (conversationPanel != null && conversationPanel.Controls.Count > 1)
            {
                DialogResult result = MessageBox.Show(this,
                    L("Chat.ClearConversationPrompt"),
                    L("Dialog.ConfirmClear"),
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question);
                if (result != DialogResult.OK)
                    return;
            }

            if (cancellation != null)
                cancellation.Cancel();
            conversationSession.Clear();
            attachments.Clear();
            currentResult = null;
            currentResultContext = null;
            lastResponseData = null;
            objectiveTextBox.Clear();
            summaryTextBox.Clear();
            validationTextBox.Clear();
            candidateGrid.Rows.Clear();
            confirmRequirementsCheckBox.Checked = false;
            confirmRequirementsCheckBox.Enabled = false;
            applyButton.Enabled = false;
            viewResponseButton.Enabled = false;
            chatReviewButton.Enabled = false;
            chatDiagnosticsButton.Enabled = false;
            RefreshAttachmentChips();
            RemovePendingStatus();
            foreach (Control control in conversationPanel.Controls.Cast<Control>().ToArray())
            {
                ForgetLocalizationTree(control);
                control.Dispose();
            }
            conversationPanel.Controls.Clear();
            conversationPanel.Controls.Add(BuildWelcomePanel());
            UpdateSuggestionButtonLayouts();
            activityLabel.Text = L("Status.Idle");
            SetBusy(false, "Status.Idle");
        }

        private void ForgetLocalizationTree(Control control)
        {
            if (control == null)
                return;

            foreach (Control child in control.Controls.Cast<Control>().ToArray())
                ForgetLocalizationTree(child);
            localizationKeys.Remove(control);
        }

        private void SetSuggestedTask(string suggestionKey)
        {
            if (objectiveTextBox == null)
                return;

            objectiveTextBox.Text = L("TaskSuggestions." +
                (suggestionKey == "survey" ? "SurveyPolygon" :
                 suggestionKey == "file" ? "FromFile" : "RelativeRoute"));
            objectiveTextBox.SelectionStart = objectiveTextBox.Text.Length;
            objectiveTextBox.Focus();
        }

        private void ResizeConversationItems()
        {
            if (conversationPanel == null)
                return;

            int width = Math.Max(260, conversationPanel.ClientSize.Width -
                conversationPanel.Padding.Horizontal - SystemInformation.VerticalScrollBarWidth - 4);
            foreach (Control control in conversationPanel.Controls)
            {
                ConversationMessageControl message = control as ConversationMessageControl;
                if (message != null)
                    message.SetAvailableWidth(width);
                else if (control is Panel panel && panel.Tag is string && panel.Tag.ToString() == "pending-status")
                    panel.Width = width;
            }
        }

        private void AddConversationMessage(ConversationMessageRole role, string message, bool isError)
        {
            if (conversationPanel == null || string.IsNullOrWhiteSpace(message))
                return;

            ConversationMessageControl item = new ConversationMessageControl(role, message, languageCode, isError);
            conversationPanel.Controls.Add(item);
            ResizeConversationItems();
            conversationPanel.ScrollControlIntoView(item);
        }

        private void ShowPendingStatus(string statusKey)
        {
            if (conversationPanel == null)
                return;

            RemovePendingStatus();
            pendingStatusPanel = new Panel
            {
                AutoSize = false,
                Height = 32,
                Margin = new Padding(0, 0, 0, 10),
                Padding = new Padding(14, 7, 14, 6),
                BackColor = Drawing.Color.FromArgb(247, 247, 247),
                Tag = "pending-status"
            };
            pendingStatusLabel = new Label
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                UseMnemonic = false
            };
            pendingStatusPanel.Controls.Add(pendingStatusLabel);
            conversationPanel.Controls.Add(pendingStatusPanel);
            activeStatusKey = statusKey;
            activeStatusText = L(statusKey);
            activeStatusAttempt = 0;
            activeStatusMaximumAttempts = 0;
            activeStatusDelayMilliseconds = 0;
            statusAnimationFrame = 0;
            pendingStatusLabel.Text = activeStatusText;
            statusAnimationTimer.Start();
            ResizeConversationItems();
            conversationPanel.ScrollControlIntoView(pendingStatusPanel);
        }

        private void ShowPendingStatusText(string statusKey, string text)
        {
            ShowPendingStatus(statusKey);
            activeStatusText = text;
            if (pendingStatusLabel != null)
                pendingStatusLabel.Text = text;
            if (activityLabel != null)
                activityLabel.Text = text;
        }

        private void RemovePendingStatus()
        {
            if (pendingStatusPanel != null && conversationPanel != null &&
                conversationPanel.Controls.Contains(pendingStatusPanel))
            {
                conversationPanel.Controls.Remove(pendingStatusPanel);
                pendingStatusPanel.Dispose();
            }
            pendingStatusPanel = null;
            pendingStatusLabel = null;
            statusAnimationTimer.Stop();
            activeStatusKey = null;
            activeStatusText = null;
            activeStatusAttempt = 0;
            activeStatusMaximumAttempts = 0;
            activeStatusDelayMilliseconds = 0;
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

                providerNoteLabel.Text = UiStrings.Format(languageCode, "Api.ProfileLoadedFormat", profile.Name) +
                    Environment.NewLine + L("Settings.ProviderNote");
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

        private static TextBox CreateReadOnlyTextBox()
        {
            return new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Drawing.SystemColors.Window
            };
        }

        private async void AddAttachments(object sender, EventArgs e)
        {
            string[] selectedPaths;
            using (var dialog = new OpenFileDialog
            {
                Title = L("Attachment.SelectDialogTitle"),
                Multiselect = true,
                CheckFileExists = true,
                Filter = BuildAttachmentFilter()
            })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;
                selectedPaths = dialog.FileNames.ToArray();
            }

            var errors = new List<string>();
            int addedCount = 0;
            bool operationCancelled = false;
            cancellation = new CancellationTokenSource();
            SetBusy(true, "Status.ReadingFiles");
            ShowPendingStatus("Status.ReadingFiles");
            try
            {
                foreach (string path in selectedPaths)
                {
                    cancellation.Token.ThrowIfCancellationRequested();
                    try
                    {
                        MissionAttachment[] existing = attachments.ToArray();
                        string selectedLanguage = languageCode;
                        MissionAttachment attachment = await Task.Run(
                            () => attachmentProcessor.Load(
                                path, existing, selectedLanguage, cancellation.Token),
                            cancellation.Token);
                        attachments.Add(attachment);
                        addedCount++;
                        RefreshAttachmentChips();
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        errors.Add(ex.Message);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                operationCancelled = true;
            }
            finally
            {
                if (cancellation != null)
                {
                    cancellation.Dispose();
                    cancellation = null;
                }

                if (addedCount > 0)
                {
                    RefreshAttachmentChips();
                    InvalidateGeneratedResult(errors.Count > 0
                        ? L("Attachment.SomeFailed")
                        : L("Attachment.Ready"));
                }
                if (errors.Count > 0)
                {
                    MessageBox.Show(this, string.Join(Environment.NewLine, errors), L("Attachment.SomeFailed"),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                CompleteBusyOperation(operationCancelled ? "Status.Cancelled" : "Status.Idle");
            }
        }

        private string BuildAttachmentFilter()
        {
            const string allSupported = "*.pdf;*.doc;*.docx;*.rtf;*.odt;*.ppt;*.pptx;*.xls;*.xlsx;*.png;*.jpg;*.jpeg;*.webp;*.gif;*.txt;*.md;*.csv;*.tsv;*.json;*.xml;*.kml;*.gpx;*.yaml;*.yml;*.html;*.htm;*.log;*.ini;*.cfg";
            const string documents = "*.pdf;*.doc;*.docx;*.rtf;*.odt;*.ppt;*.pptx;*.xls;*.xlsx";
            const string images = "*.png;*.jpg;*.jpeg;*.webp;*.gif";
            const string textData = "*.txt;*.md;*.csv;*.tsv;*.json;*.xml;*.kml;*.gpx;*.yaml;*.yml;*.html;*.htm;*.log;*.ini;*.cfg";
            return L("Attachment.FilterMissionReferences") + "|" + allSupported + "|" +
                   L("Attachment.FilterDocuments") + "|" + documents + "|" +
                   L("Attachment.FilterImages") + "|" + images + "|" +
                   L("Attachment.FilterTextData") + "|" + textData + "|" +
                   L("Attachment.FilterAllFiles") + "|*.*";
        }

        private void RemoveSelectedAttachments(object sender, EventArgs e)
        {
            Button removeButton = sender as Button;
            MissionAttachment attachment = removeButton == null ? null : removeButton.Tag as MissionAttachment;
            if (attachment == null)
                return;
            attachments.Remove(attachment);
            RefreshAttachmentChips();
            InvalidateGeneratedResult(L("Attachment.Removed"));
        }

        private void ClearAttachments(object sender, EventArgs e)
        {
            if (attachments.Count == 0)
                return;
            attachments.Clear();
            RefreshAttachmentChips();
            InvalidateGeneratedResult(L("Attachment.Empty"));
        }

        private void RefreshAttachmentGrid()
        {
            RefreshAttachmentChips();
        }

        private void RefreshAttachmentChips()
        {
            if (attachmentChipPanel == null)
                return;

            attachmentChipPanel.SuspendLayout();
            try
            {
                foreach (Control control in attachmentChipPanel.Controls.Cast<Control>().ToArray())
                {
                    ForgetLocalizationTree(control);
                    control.Dispose();
                }
                attachmentChipPanel.Controls.Clear();

                foreach (MissionAttachment attachment in attachments)
                {
                    var chip = new FlowLayoutPanel
                    {
                        AutoSize = true,
                        WrapContents = false,
                        FlowDirection = FlowDirection.LeftToRight,
                        Margin = new Padding(0, 0, 6, 4),
                        Padding = new Padding(7, 4, 4, 4),
                        BackColor = Drawing.Color.FromArgb(235, 240, 246),
                        Tag = attachment
                    };
                    var label = new Label
                    {
                        AutoSize = true,
                        MaximumSize = new Drawing.Size(360, 0),
                        Text = attachment.DisplayName + " · " + FormatFileSize(attachment.SizeBytes),
                        Padding = new Padding(0, 3, 6, 0),
                        UseMnemonic = false
                    };
                    var remove = Track(new Button
                    {
                        AutoSize = true,
                        Height = 25,
                        Padding = new Padding(5, 1, 5, 1),
                        Tag = attachment
                    }, "Button.Remove");
                    remove.Click += RemoveSelectedAttachments;
                    chip.Controls.Add(label);
                    chip.Controls.Add(remove);
                    attachmentChipPanel.Controls.Add(chip);
                }

                if (attachments.Count == 0)
                {
                    attachmentChipPanel.Controls.Add(new Label
                    {
                        AutoSize = true,
                        ForeColor = Drawing.SystemColors.GrayText,
                        Text = L("Attachment.Empty"),
                        Padding = new Padding(0, 5, 0, 0),
                        UseMnemonic = false
                    });
                }
                clearAttachmentsButton.Enabled = attachments.Count > 0;
            }
            finally
            {
                attachmentChipPanel.ResumeLayout(true);
            }
        }

        private void InvalidateGeneratedResult(string status)
        {
            currentResult = null;
            currentResultContext = null;
            lastResponseData = null;
            applyButton.Enabled = false;
            viewResponseButton.Enabled = false;
            confirmRequirementsCheckBox.Checked = false;
            confirmRequirementsCheckBox.Enabled = false;
            summaryTextBox.Clear();
            validationTextBox.Text = status;
            candidateGrid.Rows.Clear();
            chatReviewButton.Enabled = false;
            chatDiagnosticsButton.Enabled = false;
            activeStatusKey = "Status.Idle";
            activeStatusText = L("Status.Idle");
            activeStatusAttempt = 0;
            activeStatusMaximumAttempts = 0;
            activeStatusDelayMilliseconds = 0;
            activityLabel.Text = L("Status.Idle");
        }

        private static string FormatFileSize(long bytes)
        {
            if (bytes >= 1024L * 1024L)
                return (bytes / (1024.0 * 1024.0)).ToString("0.##") + " MB";
            if (bytes >= 1024L)
                return (bytes / 1024.0).ToString("0.##") + " KB";
            return bytes + " B";
        }

        private void ConfirmRequirementsChanged(object sender, EventArgs e)
        {
            applyButton.Enabled = confirmRequirementsCheckBox.Checked &&
                                  currentResult != null &&
                                  currentResult.Mission != null &&
                                  currentResult.Validation.IsValid;
        }

        private async void GenerateMission(object sender, EventArgs e)
        {
            string rawObjective = objectiveTextBox.Text == null ? string.Empty : objectiveTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(rawObjective) && attachments.Count == 0)
            {
                MessageBox.Show(this, L("Chat.EmptyObjective"), L("Dialog.Information"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ApiConnectionSettings settings;
            string credentialSource;
            try
            {
                settings = BuildConnectionSettings(out credentialSource);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                return;
            }

            string visibleObjective = string.IsNullOrWhiteSpace(rawObjective)
                ? L("TaskSuggestions.FromFile")
                : rawObjective;
            AddConversationMessage(ConversationMessageRole.User, visibleObjective, false);
            string requestObjective = conversationSession.BuildObjective(
                string.IsNullOrWhiteSpace(rawObjective)
                    ? "Use the attached files to identify and clarify the mission requirements."
                    : rawObjective,
                languageCode);
            conversationSession.RecordUserTask(visibleObjective);

            SetBusy(true, "Status.ReadingFiles");
            ShowPendingStatus("Status.ReadingFiles");
            currentResult = null;
            candidateGrid.Rows.Clear();
            applyButton.Enabled = false;
            confirmRequirementsCheckBox.Checked = false;
            confirmRequirementsCheckBox.Enabled = false;
            summaryTextBox.Clear();
            validationTextBox.Clear();
            lastResponseData = null;
            viewResponseButton.Enabled = false;
            cancellation = new CancellationTokenSource();
            bool operationCancelled = false;

            try
            {
                ShowPendingStatus("Status.PreparingRequest");
                MissionContext context = CaptureMissionContext();
                TaskSpec spec;
                ShowPendingStatus("Status.Connecting");
                using (var client = new OpenAiResponsesClient(languageCode, ReportApiActivity))
                {
                    try
                    {
                        ShowPendingStatus("Status.Thinking");
                        spec = await client.GenerateTaskSpecAsync(
                            requestObjective,
                            settings,
                            context,
                            attachments.ToArray(),
                            cancellation.Token);
                    }
                    finally
                    {
                        lastResponseData = client.LastResponseData;
                        viewResponseButton.Enabled = lastResponseData != null;
                    }
                }

                ShowPendingStatus("Status.Validating");
                var result = new MissionGenerationResult { Spec = spec };
                result.Validation.Merge(validator.ValidateSpec(spec, context));
                ValidateAttachmentAcknowledgement(result.Validation, spec);
                if (result.Validation.IsValid)
                {
                    ShowPendingStatus("Status.Compiling");
                    try
                    {
                        result.Mission = compiler.Compile(spec, context);
                        result.Validation.Merge(validator.ValidateMission(result.Mission, context.Home));
                    }
                    catch (Exception ex)
                    {
                        result.Validation.Errors.Add(UiStrings.Format(
                            languageCode, "Mission.CompilationFailedFormat", ex.Message));
                    }
                }

                currentResult = result;
                currentResultContext = context;
                DisplayResult(result);
                conversationSession.RecordStructuredTaskData(BuildConversationSummary(spec));
                AddConversationMessage(
                    ConversationMessageRole.Assistant,
                    BuildAssistantMessage(result),
                    !result.Validation.IsValid);

                if (settings.AuthenticationMode != ApiAuthenticationMode.None && rememberKeyCheckBox.Checked)
                {
                    WriteCurrentCredential(settings.ApiKey);
                    credentialStatusLabel.Text = L("Api.KeySaved");
                }
                else
                {
                    credentialStatusLabel.Text = UiStrings.Format(
                        languageCode, "Api.CredentialFoundFormat", credentialSource);
                }
                MarkCurrentProfileUsed();
            }
            catch (OperationCanceledException)
            {
                operationCancelled = true;
                validationTextBox.Text = L("Status.Cancelled");
                AddConversationMessage(ConversationMessageRole.System, L("Status.Cancelled"), false);
            }
            catch (Exception ex)
            {
                AddConversationMessage(ConversationMessageRole.Assistant,
                    L("Chat.MessageFailed") + Environment.NewLine + ex.Message,
                    true);
                ShowError(ex.Message);
            }
            finally
            {
                if (cancellation != null)
                {
                    cancellation.Dispose();
                    cancellation = null;
                }
                string completionStatus = currentResult == null
                    ? operationCancelled ? "Status.Cancelled" : "Status.Failed"
                    : currentResult.Spec != null && currentResult.Spec.requires_clarification
                        ? "Status.NeedsClarification"
                        : currentResult.Validation.IsValid
                            ? "Status.ReadyForReview"
                            : "Status.Failed";
                CompleteBusyOperation(completionStatus);
            }
        }

        private static string BuildConversationSummary(TaskSpec spec)
        {
            if (spec == null)
                return string.Empty;

            var builder = new StringBuilder();
            builder.AppendLine(spec.summary ?? string.Empty);
            builder.AppendLine(spec.source_summary ?? string.Empty);
            if (spec.requires_clarification)
                builder.AppendLine(spec.clarification_question ?? string.Empty);
            return builder.ToString().Trim();
        }

        private string BuildAssistantMessage(MissionGenerationResult result)
        {
            if (result == null || result.Spec == null)
                return L("Mission.NoCandidate");

            TaskSpec spec = result.Spec;
            if (spec.requires_clarification)
                return L("Chat.ClarificationTitle") + Environment.NewLine +
                    (spec.clarification_question ?? string.Empty);

            string summary = string.IsNullOrWhiteSpace(spec.summary)
                ? L("Mission.CandidateSummary")
                : spec.summary.Trim();
            string title = result.Validation != null && result.Validation.IsValid && result.Mission != null
                ? L("Chat.CandidateReadyTitle")
                : L("Chat.CandidateInvalidTitle");
            return title + Environment.NewLine + summary;
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

        private void ApplyMission(object sender, EventArgs e)
        {
            if (currentResult == null || currentResult.Mission == null || !currentResult.Validation.IsValid ||
                !confirmRequirementsCheckBox.Checked)
                return;

            if (!IsRelativeAltitudeMode())
            {
                MessageBox.Show(this,
                    L("Mission.RelativeAltitudeRequired"),
                    L("Dialog.Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsStandardMissionType())
            {
                MessageBox.Show(this,
                    L("Mission.StandardMissionRequired"),
                    L("Dialog.Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PointLatLngAlt currentHome = CaptureMissionContext().Home;
            ValidationResult revalidation = validator.ValidateMission(currentResult.Mission, currentHome);
            if (!revalidation.IsValid)
            {
                validationTextBox.Text = string.Join(Environment.NewLine,
                    revalidation.Errors.Select(error =>
                        UiStrings.Format(languageCode, "Validation.ErrorPrefix", error)));
                applyButton.Enabled = false;
                MessageBox.Show(this, L("Mission.RevalidationFailed"),
                    L("Dialog.Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int existing = GetExistingMissionItemCount();
            string existingWarning = existing > 0
                ? UiStrings.Format(languageCode, "Mission.ExistingItemsWarningFormat", existing) +
                  Environment.NewLine + Environment.NewLine
                : string.Empty;
            DialogResult confirmation = MessageBox.Show(this,
                existingWarning +
                UiStrings.Format(languageCode, "Mission.ApplyConfirmFormat", currentResult.Mission.Items.Count) +
                Environment.NewLine + Environment.NewLine + L("Mission.LocalOnlyNotice"),
                L("Dialog.ConfirmApply"), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (confirmation != DialogResult.OK)
                return;

            int originalMissionItemCount = existing;
            bool previousQuickAdd = plugin.Host.MainForm.FlightPlanner.quickadd;
            bool previousVerifyHeight = plugin.Host.MainForm.FlightPlanner.CHK_verifyheight.Checked;
            try
            {
                plugin.Host.MainForm.FlightPlanner.quickadd = true;
                plugin.Host.MainForm.FlightPlanner.CHK_verifyheight.Checked = false;
                foreach (CandidateMissionItem item in currentResult.Mission.Items)
                {
                    CandidateMissionItem plannerItem = ConvertToPlannerDisplayUnits(item);
                    int rowIndex = plugin.Host.AddWPtoList(
                        plannerItem.Command,
                        plannerItem.Param1,
                        plannerItem.Param2,
                        plannerItem.Param3,
                        plannerItem.Param4,
                        plannerItem.Longitude,
                        plannerItem.Latitude,
                        plannerItem.Altitude,
                        "AIWaypointPlanner");

                    object appliedCommand = plugin.Host.MainForm.FlightPlanner.Commands.Rows[rowIndex]
                        .Cells["Command"].Value;
                    if (appliedCommand == null ||
                        !string.Equals(Convert.ToString(appliedCommand), item.Command.ToString(), StringComparison.Ordinal))
                    {
                        throw new InvalidOperationException(L("Mission.CommandChanged"));
                    }
                }
            }
            catch (Exception ex)
            {
                RollBackAppendedRows(originalMissionItemCount);
                MessageBox.Show(this,
                    UiStrings.Format(languageCode, "Mission.AppendFailedFormat", ex.Message),
                    L("Dialog.Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                plugin.Host.MainForm.FlightPlanner.CHK_verifyheight.Checked = previousVerifyHeight;
                plugin.Host.MainForm.FlightPlanner.quickadd = previousQuickAdd;
                plugin.Host.MainForm.FlightPlanner.writeKML();
            }

            applyButton.Enabled = false;
            activityLabel.Text = L("Status.AppliedLocally");
            MessageBox.Show(this,
                L("Mission.Applied"),
                L("Dialog.Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private MissionContext CaptureMissionContext()
        {
            PointLatLngAlt plannedHome = plugin.Host.cs.PlannedHomeLocation;
            var context = new MissionContext
            {
                Home = plannedHome == null ? null : new PointLatLngAlt(plannedHome)
            };

            var polygon = plugin.Host.FPDrawnPolygon;
            if (polygon != null)
            {
                foreach (var point in polygon.Points)
                    context.Polygon.Add(new PointLatLngAlt(point));
            }
            return context;
        }

        private bool IsRelativeAltitudeMode()
        {
            object selected = plugin.Host.MainForm.FlightPlanner.CMB_altmode.SelectedValue;
            if (selected == null)
                return false;
            return Convert.ToInt32(selected) == (int)FlightPlanner.altmode.Relative;
        }

        private bool IsStandardMissionType()
        {
            object selected = plugin.Host.MainForm.FlightPlanner.cmb_missiontype.SelectedValue;
            if (selected == null)
                return false;
            return Convert.ToInt32(selected) == (int)MAVLink.MAV_MISSION_TYPE.MISSION;
        }

        private int GetExistingMissionItemCount()
        {
            int count = 0;
            foreach (DataGridViewRow row in plugin.Host.MainForm.FlightPlanner.Commands.Rows)
            {
                if (!row.IsNewRow)
                    count++;
            }
            return count;
        }

        private static CandidateMissionItem ConvertToPlannerDisplayUnits(CandidateMissionItem item)
        {
            var converted = new CandidateMissionItem
            {
                Command = item.Command,
                Param1 = item.Param1,
                Param2 = item.Param2,
                Param3 = item.Param3,
                Param4 = item.Param4,
                Longitude = item.Longitude,
                Latitude = item.Latitude,
                Altitude = item.Altitude * CurrentState.multiplieralt,
                Description = item.Description
            };

            if (item.Command == MAVLink.MAV_CMD.DO_CHANGE_SPEED)
                converted.Param2 = item.Param2 * CurrentState.multiplierspeed;

            return converted;
        }

        private void RollBackAppendedRows(int originalCount)
        {
            DataGridViewRowCollection rows = plugin.Host.MainForm.FlightPlanner.Commands.Rows;
            while (GetExistingMissionItemCount() > originalCount)
            {
                int index = rows.Count - 1;
                while (index >= 0 && rows[index].IsNewRow)
                    index--;
                if (index < 0)
                    break;
                rows.RemoveAt(index);
            }
        }

        private void DisplayResult(MissionGenerationResult result)
        {
            string missionType = result.Spec == null ? string.Empty : result.Spec.mission_type;
            int itemCount = result.Mission == null ? 0 : result.Mission.Items.Count;
            TaskSpec spec = result.Spec;
            var summaryLines = new List<string>();
            if (spec != null)
            {
                summaryLines.Add(L("Mission.Understanding") + ": " + (spec.source_summary ?? string.Empty));
                summaryLines.Add(L("Mission.Requirements") + ":");
                if (spec.confirmed_requirements != null)
                {
                    summaryLines.AddRange(spec.confirmed_requirements
                        .Where(requirement => !string.IsNullOrWhiteSpace(requirement))
                        .Select(requirement => "  - " + requirement.Trim()));
                }
                string files = spec.source_files_used == null
                    ? string.Empty
                    : string.Join(", ", spec.source_files_used.Where(name => !string.IsNullOrWhiteSpace(name)));
                summaryLines.Add(L("Mission.UsedFiles") + ": " +
                    (string.IsNullOrWhiteSpace(files) ? L("Mission.NoFiles") : files));
                summaryLines.Add(L("Mission.CandidateSummary") + ": " + (spec.summary ?? string.Empty));
                if (spec.requires_clarification && !string.IsNullOrWhiteSpace(spec.clarification_question))
                    summaryLines.Add(L("Chat.ClarificationTitle") + ": " + spec.clarification_question);
            }
            summaryLines.Add(UiStrings.Format(languageCode, "Mission.TemplateAndCountFormat", missionType, itemCount));
            summaryTextBox.Text = string.Join(Environment.NewLine, summaryLines);

            var lines = result.Validation.Errors.Select(error =>
                    UiStrings.Format(languageCode, "Validation.ErrorPrefix", error))
                .Concat(result.Validation.Warnings.Select(warning =>
                    UiStrings.Format(languageCode, "Validation.WarningPrefix", warning)))
                .ToArray();
            validationTextBox.Text = lines.Length == 0
                ? L("Validation.Passed")
                : string.Join(Environment.NewLine, lines);

            candidateGrid.Rows.Clear();
            if (result.Mission != null)
            {
                for (int i = 0; i < result.Mission.Items.Count; i++)
                {
                    CandidateMissionItem item = result.Mission.Items[i];
                    candidateGrid.Rows.Add(
                        i + 1,
                        item.Command.ToString(),
                        item.Latitude == 0.0 ? string.Empty : item.Latitude.ToString("F7"),
                        item.Longitude == 0.0 ? string.Empty : item.Longitude.ToString("F7"),
                        item.Altitude == 0.0 ? string.Empty : item.Altitude.ToString("F1"),
                        item.Param1 == 0.0 ? string.Empty : item.Param1.ToString("0.###"),
                        item.Param2 == 0.0 ? string.Empty : item.Param2.ToString("0.###"),
                        item.Description);
                }
            }

            confirmRequirementsCheckBox.Enabled = result.Validation.IsValid && result.Mission != null;
            confirmRequirementsCheckBox.Checked = false;
            applyButton.Enabled = false;
        }

        private void RelocalizeCurrentResult()
        {
            if (currentResult == null || currentResult.Spec == null)
                return;

            if (currentResultContext != null)
            {
                var validation = new ValidationResult();
                validation.Merge(validator.ValidateSpec(currentResult.Spec, currentResultContext));
                ValidateAttachmentAcknowledgement(validation, currentResult.Spec);
                CandidateMission mission = null;
                if (validation.IsValid)
                {
                    try
                    {
                        mission = compiler.Compile(currentResult.Spec, currentResultContext);
                        validation.Merge(validator.ValidateMission(mission, currentResultContext.Home));
                    }
                    catch (Exception ex)
                    {
                        validation.Errors.Add(UiStrings.Format(
                            languageCode, "Mission.CompilationFailedFormat", ex.Message));
                    }
                }
                currentResult.Mission = mission;
                currentResult.Validation = validation;
            }

            DisplayResult(currentResult);
        }

        private void ValidateAttachmentAcknowledgement(ValidationResult validation, TaskSpec spec)
        {
            if (validation == null || spec == null)
                return;

            string[] usedFiles = (spec.source_files_used ?? new List<string>())
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(name => name.Trim())
                .ToArray();
            if (attachments.Count > 0 && usedFiles.Length == 0)
                validation.Errors.Add(L("Attachment.NoUsedFiles"));
            if (attachments.Count == 0 && usedFiles.Length > 0)
                validation.Errors.Add(L("Attachment.UnrequestedFiles"));

            var available = new HashSet<string>(attachments.Select(item => item.DisplayName), StringComparer.OrdinalIgnoreCase);
            foreach (string usedFile in usedFiles)
            {
                if (!available.Contains(usedFile))
                    validation.Errors.Add(UiStrings.Format(
                        languageCode, "Attachment.UnknownUsedFileFormat", usedFile));
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

        private void SetBusy(bool busy, string status)
        {
            if (!busy)
                RemovePendingStatus();

            generateButton.Enabled = !busy;
            cancelButton.Enabled = busy;
            testConnectionButton.Enabled = !busy;
            deleteCredentialButton.Enabled = !busy;
            saveProfileButton.Enabled = !busy;
            deleteProfileButton.Enabled = !busy;
            closeButton.Enabled = !busy;
            objectiveTextBox.Enabled = !busy;
            attachmentChipPanel.Enabled = !busy;
            addAttachmentButton.Enabled = !busy;
            clearAttachmentsButton.Enabled = !busy && attachments.Count > 0;
            settingsShortcutButton.Enabled = !busy;
            newChatButton.Enabled = !busy;
            chatReviewButton.Enabled = !busy && currentResult != null;
            chatDiagnosticsButton.Enabled = !busy && lastResponseData != null;
            providerComboBox.Enabled = !busy;
            savedProfileComboBox.Enabled = !busy;
            protocolComboBox.Enabled = !busy;
            authenticationComboBox.Enabled = !busy;
            reasoningComboBox.Enabled = !busy;
            languageComboBox.Enabled = !busy;
            baseUrlTextBox.Enabled = !busy;
            modelTextBox.Enabled = !busy;
            projectTextBox.Enabled = !busy;
            apiKeyTextBox.Enabled = !busy && GetAuthenticationMode() != ApiAuthenticationMode.None;
            rememberKeyCheckBox.Enabled = !busy && GetAuthenticationMode() != ApiAuthenticationMode.None;
            applyButton.Enabled = !busy && confirmRequirementsCheckBox.Checked &&
                                  currentResult != null && currentResult.Validation.IsValid;
            activeStatusKey = null;
            activeStatusText = null;
            activeStatusAttempt = 0;
            activeStatusMaximumAttempts = 0;
            activeStatusDelayMilliseconds = 0;
            statusAnimationTimer.Stop();
            if (!string.IsNullOrWhiteSpace(status) && status.StartsWith("Status.", StringComparison.Ordinal))
            {
                activeStatusKey = status;
                activeStatusText = L(status);
                activeStatusAttempt = 0;
                activeStatusMaximumAttempts = 0;
                activeStatusDelayMilliseconds = 0;
                statusAnimationFrame = 0;
                activityLabel.Text = activeStatusText;
                if (busy)
                    statusAnimationTimer.Start();
            }
            else
            {
                activityLabel.Text = status ?? string.Empty;
            }
            Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
        }

        private void CompleteBusyOperation(string status)
        {
            SetBusy(false, status);
            if (!closeWhenIdle)
                return;

            closeWhenIdle = false;
            BeginInvoke(new Action(Close));
        }

        private void ReportApiActivity(ApiClientActivity activity)
        {
            if (activity == null || IsDisposed || Disposing || !IsHandleCreated)
                return;
            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke(new Action<ApiClientActivity>(ReportApiActivity), activity);
                }
                catch (InvalidOperationException)
                {
                    // The form may be closing while an in-flight request reports its final state.
                }
                return;
            }

            string statusKey = activity.Kind == ApiClientActivityKind.Reconnecting
                ? "Status.ReconnectingFormat"
                : activity.Kind == ApiClientActivityKind.WaitingForModel && !testingConnection
                    ? "Status.Thinking"
                    : activity.Kind == ApiClientActivityKind.WaitingForModel
                        ? "Status.WaitingForModel"
                        : "Status.Connecting";

            string text = activity.Kind == ApiClientActivityKind.Reconnecting
                ? UiStrings.Format(languageCode, statusKey, activity.Attempt, activity.MaximumAttempts)
                : L(statusKey);
            ShowPendingStatusText(statusKey, text);
            activeStatusAttempt = activity.Attempt;
            activeStatusMaximumAttempts = activity.MaximumAttempts;
            activeStatusDelayMilliseconds = activity.DelayMilliseconds;
            activeStatusText = text;
        }

        private string GetActiveStatusText()
        {
            if (activeStatusKey == "Status.ReconnectingFormat" && activeStatusMaximumAttempts > 0)
                return UiStrings.Format(languageCode, activeStatusKey,
                    activeStatusAttempt, activeStatusMaximumAttempts);
            return string.IsNullOrWhiteSpace(activeStatusText)
                ? L(activeStatusKey)
                : activeStatusText;
        }

        private void ViewModelResponse(object sender, EventArgs e)
        {
            if (lastResponseData == null)
                return;

            using (var dialog = new ModelResponseDialog(lastResponseData, languageCode))
            {
                ThemeManager.ApplyThemeTo(dialog);
                dialog.ShowDialog(this);
            }
        }

        private void ShowError(string message)
        {
            validationTextBox.Text = UiStrings.Format(languageCode, "Validation.ErrorPrefix", message);
            SetActivityStatus("Status.Failed", false);
            string responseHint = lastResponseData == null
                ? string.Empty
                : Environment.NewLine + Environment.NewLine + L("Chat.ViewDiagnostics");
            MessageBox.Show(this, message + responseHint, L("Dialog.Error"),
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void SetActivityStatus(string statusKey, bool animate)
        {
            if (string.IsNullOrWhiteSpace(statusKey))
                return;

            activeStatusKey = statusKey;
            activeStatusText = L(statusKey);
            activeStatusAttempt = 0;
            activeStatusMaximumAttempts = 0;
            activeStatusDelayMilliseconds = 0;
            statusAnimationFrame = 0;
            if (activityLabel != null)
                activityLabel.Text = activeStatusText;
            if (pendingStatusLabel != null && !pendingStatusLabel.IsDisposed)
                pendingStatusLabel.Text = activeStatusText;
            if (animate)
                statusAnimationTimer.Start();
            else
                statusAnimationTimer.Stop();
        }
    }
}
