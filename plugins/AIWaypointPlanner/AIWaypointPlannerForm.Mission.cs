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
        private void InvalidateGeneratedResult(string statusKey)
        {
            currentResult = null;
            currentResultContext = null;
            lastResponseData = null;
            applyButton.Enabled = false;
            viewResponseButton.Enabled = false;
            confirmRequirementsCheckBox.Checked = false;
            confirmRequirementsCheckBox.Enabled = false;
            summaryTextBox.Clear();
            SetValidationStatus(statusKey);
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

        private void SetValidationStatus(string statusKey)
        {
            validationStatusKey = statusKey;
            validationTextBox.Text = string.IsNullOrWhiteSpace(statusKey)
                ? string.Empty
                : L(statusKey);
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

            bool attachmentOnlyTask = string.IsNullOrWhiteSpace(rawObjective);
            string visibleObjective = attachmentOnlyTask
                ? L("TaskSuggestions.FromFile")
                : rawObjective;
            AddConversationMessage(
                ConversationMessageRole.User,
                visibleObjective,
                false,
                attachmentOnlyTask ? (Func<string>)(() => L("TaskSuggestions.FromFile")) : null);
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
                    !result.Validation.IsValid,
                    delegate { return BuildAssistantMessage(result); });

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
                SetValidationStatus("Status.Cancelled");
                AddConversationMessage(
                    ConversationMessageRole.System,
                    L("Status.Cancelled"),
                    false,
                    delegate { return L("Status.Cancelled"); });
            }
            catch (Exception ex)
            {
                string errorDetail = ex.Message;
                AddConversationMessage(
                    ConversationMessageRole.Assistant,
                    L("Chat.MessageFailed") + Environment.NewLine + errorDetail,
                    true,
                    delegate { return L("Chat.MessageFailed") + Environment.NewLine + errorDetail; });
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
                    string hostTag = string.IsNullOrWhiteSpace(plannerItem.Description)
                        ? L("App.Name")
                        : plannerItem.Description;
                    int rowIndex = plugin.Host.AddWPtoList(
                        plannerItem.Command,
                        plannerItem.Param1,
                        plannerItem.Param2,
                        plannerItem.Param3,
                        plannerItem.Param4,
                        plannerItem.Longitude,
                        plannerItem.Latitude,
                        plannerItem.Altitude,
                        hostTag);

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
            SetActivityStatus("Status.AppliedLocally", false);
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
            validationStatusKey = null;
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
            summaryLines.Add(UiStrings.Format(languageCode, "Mission.TemplateAndCountFormat",
                GetMissionTypeDisplayName(missionType), itemCount));
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

        private string GetMissionTypeDisplayName(string missionType)
        {
            if (string.Equals(missionType, "relative_route", StringComparison.OrdinalIgnoreCase))
                return L("Mission.TypeRelativeRoute");
            if (string.Equals(missionType, "survey_polygon", StringComparison.OrdinalIgnoreCase))
                return L("Mission.TypeSurveyPolygon");
            if (string.Equals(missionType, "unsupported", StringComparison.OrdinalIgnoreCase))
                return L("Mission.TypeUnsupported");
            return L("Mission.TypeUnsupported");
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
    }
}
