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
        private void ObjectiveKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                GenerateMission(sender, EventArgs.Empty);
            }
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
            validationStatusKey = null;
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

        private void AddConversationMessage(
            ConversationMessageRole role,
            string message,
            bool isError,
            Func<string> localizer = null)
        {
            if (conversationPanel == null || string.IsNullOrWhiteSpace(message))
                return;

            ConversationMessageControl item = new ConversationMessageControl(role, message, languageCode, isError);
            item.ApplyPluginTheme();
            if (localizer != null)
                conversationMessageLocalizers[item] = localizer;
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
                BackColor = PluginTheme.RaisedSurface,
                Tag = "pending-status"
            };
            pendingStatusLabel = new Label
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                UseMnemonic = false
            };
            pendingStatusPanel.Controls.Add(pendingStatusLabel);
            PluginTheme.Apply(pendingStatusPanel);
            pendingStatusPanel.BackColor = PluginTheme.RaisedSurface;
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
                        ? "Attachment.SomeFailed"
                        : "Attachment.Ready");
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
            InvalidateGeneratedResult("Attachment.Removed");
        }

        private void ClearAttachments(object sender, EventArgs e)
        {
            if (attachments.Count == 0)
                return;
            attachments.Clear();
            RefreshAttachmentChips();
            InvalidateGeneratedResult("Attachment.Empty");
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
                        BackColor = PluginTheme.RaisedSurface,
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
                    PluginTheme.Apply(chip);
                    chip.BackColor = PluginTheme.RaisedSurface;
                    PluginTheme.ApplyButton(remove, PluginButtonStyle.Danger);
                    attachmentChipPanel.Controls.Add(chip);
                }

                if (attachments.Count == 0)
                {
                    var emptyLabel = new Label
                    {
                        AutoSize = true,
                        ForeColor = PluginTheme.SecondaryText,
                        Text = L("Attachment.Empty"),
                        Padding = new Padding(0, 5, 0, 0),
                        UseMnemonic = false
                    };
                    PluginTheme.Apply(emptyLabel);
                    emptyLabel.ForeColor = PluginTheme.SecondaryText;
                    attachmentChipPanel.Controls.Add(emptyLabel);
                }
                clearAttachmentsButton.Enabled = attachments.Count > 0;
            }
            finally
            {
                attachmentChipPanel.ResumeLayout(true);
            }
        }

        private static string FormatFileSize(long bytes)
        {
            if (bytes >= 1024L * 1024L)
                return (bytes / (1024.0 * 1024.0)).ToString("0.##") + " MB";
            if (bytes >= 1024L)
                return (bytes / 1024.0).ToString("0.##") + " KB";
            return bytes + " B";
        }
    }
}
