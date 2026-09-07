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
    public sealed partial class AIWaypointPlannerForm : Form
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
        private readonly Dictionary<ConversationMessageControl, Func<string>> conversationMessageLocalizers;

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
        private string validationStatusKey;
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
            conversationMessageLocalizers = new Dictionary<ConversationMessageControl, Func<string>>();

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
            ApplyPluginTheme();
            ApplySafetyNoticeStyle();
            UpdateSafetyNoticeLayout();
            UpdateSuggestionButtonLayouts();
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
            return string.IsNullOrWhiteSpace(activeStatusKey)
                ? (activeStatusText ?? string.Empty)
                : L(activeStatusKey);
        }

        private void ViewModelResponse(object sender, EventArgs e)
        {
            if (lastResponseData == null)
                return;

            using (var dialog = new ModelResponseDialog(lastResponseData, languageCode))
            {
                ThemeManager.ApplyThemeTo(dialog);
                dialog.ApplyPluginTheme();
                dialog.ShowDialog(this);
            }
        }

        private void ShowError(string message)
        {
            validationStatusKey = null;
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
