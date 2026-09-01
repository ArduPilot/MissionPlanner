extern alias SystemDrawing;

using System;
using System.Collections.Generic;
using System.Linq;
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

        private TextBox objectiveTextBox;
        private TextBox summaryTextBox;
        private TextBox validationTextBox;
        private DataGridView candidateGrid;
        private DataGridView attachmentGrid;
        private Button addAttachmentButton;
        private Button removeAttachmentButton;
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
        private ApiResponseData lastResponseData;
        private bool loadingApiProfile;

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

            BuildInterface();
            LoadCredentialStatus();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && cancellation != null)
            {
                cancellation.Cancel();
                cancellation.Dispose();
                cancellation = null;
            }
            base.Dispose(disposing);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            ApplySafetyNoticeStyle();
        }

        private void BuildInterface()
        {
            Text = "AI 航点规划 v1.4.2 - 文件辅助候选任务生成器";
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Drawing.Size(1080, 760);
            Size = new Drawing.Size(1280, 900);
            Font = new Drawing.Font("Microsoft YaHei UI", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point);

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(12)
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
            Controls.Add(root);

            safetyNotice = new Label
            {
                Dock = DockStyle.Fill,
                AutoSize = false,
                Padding = new Padding(12, 8, 12, 8),
                TextAlign = Drawing.ContentAlignment.MiddleLeft,
                Text = "安全边界：GPT 仅生成受限任务参数。插件只追加到本地 Flight Planner 列表，不会上传任务、改变模式、解锁、起飞或发送 RC/PWM。"
            };
            ApplySafetyNoticeStyle();
            root.Controls.Add(safetyNotice, 0, 0);

            var tabs = new TabControl { Dock = DockStyle.Fill };
            tabs.TabPages.Add(BuildMissionTab());
            tabs.TabPages.Add(BuildApiTab());
            root.Controls.Add(tabs, 0, 1);

            var bottom = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 10, 0, 0),
                WrapContents = false
            };
            closeButton = new Button { Text = "关闭", AutoSize = true, Height = 32 };
            closeButton.Click += delegate { Close(); };
            applyButton = new Button
            {
                Text = "应用到飞行计划",
                AutoSize = true,
                Height = 32,
                Enabled = false
            };
            applyButton.Click += ApplyMission;
            viewResponseButton = new Button
            {
                Text = "查看模型返回数据",
                AutoSize = true,
                Height = 32,
                Enabled = false
            };
            viewResponseButton.Click += ViewModelResponse;
            activityLabel = new Label
            {
                AutoSize = true,
                Padding = new Padding(0, 8, 16, 0),
                Text = "等待输入任务目标"
            };
            bottom.Controls.Add(closeButton);
            bottom.Controls.Add(applyButton);
            bottom.Controls.Add(viewResponseButton);
            bottom.Controls.Add(activityLabel);
            root.Controls.Add(bottom, 0, 2);
        }

        private TabPage BuildMissionTab()
        {
            var tab = new TabPage("任务规划") { Padding = new Padding(8) };
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 135F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 145F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 155F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 115F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tab.Controls.Add(layout);

            var objectiveGroup = new GroupBox { Text = "自然语言任务目标", Dock = DockStyle.Fill };
            var objectiveLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(8)
            };
            objectiveLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            objectiveLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            objectiveTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                AcceptsReturn = true,
                Text = "从计划 Home 起飞，起飞任务高度 80 米，以 120 米相对高度、18 米每秒速度先向东飞行 1000 米，再向北飞行 1000 米，完成后返航。"
            };
            var actionPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(8, 0, 0, 0),
                WrapContents = false
            };
            generateButton = new Button { Text = "生成候选任务", Width = 140, Height = 36 };
            generateButton.Click += GenerateMission;
            cancelButton = new Button { Text = "取消请求", Width = 140, Height = 32, Enabled = false };
            cancelButton.Click += delegate { if (cancellation != null) cancellation.Cancel(); };
            actionPanel.Controls.Add(generateButton);
            actionPanel.Controls.Add(cancelButton);
            objectiveLayout.Controls.Add(objectiveTextBox, 0, 0);
            objectiveLayout.Controls.Add(actionPanel, 1, 0);
            objectiveGroup.Controls.Add(objectiveLayout);
            layout.Controls.Add(objectiveGroup, 0, 0);

            var attachmentGroup = new GroupBox { Text = "任务资料附件（最多 6 个）", Dock = DockStyle.Fill };
            var attachmentLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(8)
            };
            attachmentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            attachmentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            attachmentGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoGenerateColumns = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true
            };
            attachmentGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "文件名",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 200
            });
            attachmentGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "类型", Width = 155 });
            attachmentGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "大小", Width = 85 });
            attachmentGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "处理状态", Width = 260 });
            var attachmentActions = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(8, 0, 0, 0),
                WrapContents = false
            };
            addAttachmentButton = new Button { Text = "添加文件", Width = 140, Height = 32 };
            addAttachmentButton.Click += AddAttachments;
            removeAttachmentButton = new Button { Text = "移除所选", Width = 140, Height = 32 };
            removeAttachmentButton.Click += RemoveSelectedAttachments;
            clearAttachmentsButton = new Button { Text = "清空", Width = 140, Height = 32 };
            clearAttachmentsButton.Click += ClearAttachments;
            attachmentActions.Controls.Add(addAttachmentButton);
            attachmentActions.Controls.Add(removeAttachmentButton);
            attachmentActions.Controls.Add(clearAttachmentsButton);
            attachmentLayout.Controls.Add(attachmentGrid, 0, 0);
            attachmentLayout.Controls.Add(attachmentActions, 1, 0);
            attachmentGroup.Controls.Add(attachmentLayout);
            layout.Controls.Add(attachmentGroup, 0, 1);

            var summaryGroup = new GroupBox { Text = "AI 对任务要求的理解（应用前须人工确认）", Dock = DockStyle.Fill };
            var summaryLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            summaryLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            summaryLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            summaryTextBox = CreateReadOnlyTextBox();
            confirmRequirementsCheckBox = new CheckBox
            {
                AutoSize = true,
                Enabled = false,
                Padding = new Padding(4, 4, 0, 0),
                Text = "我已核对并确认上述任务理解与要求"
            };
            confirmRequirementsCheckBox.CheckedChanged += ConfirmRequirementsChanged;
            summaryLayout.Controls.Add(summaryTextBox, 0, 0);
            summaryLayout.Controls.Add(confirmRequirementsCheckBox, 0, 1);
            summaryGroup.Controls.Add(summaryLayout);
            layout.Controls.Add(summaryGroup, 0, 2);

            var validationGroup = new GroupBox { Text = "本地校验结果与提示", Dock = DockStyle.Fill };
            validationTextBox = CreateReadOnlyTextBox();
            validationGroup.Controls.Add(validationTextBox);
            layout.Controls.Add(validationGroup, 0, 3);

            var candidateGroup = new GroupBox { Text = "候选任务项（尚未应用、尚未上传）", Dock = DockStyle.Fill };
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
            candidateGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "序号", Width = 54 });
            candidateGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "命令", Width = 180 });
            candidateGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "纬度", Width = 110 });
            candidateGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "经度", Width = 110 });
            candidateGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "高度(m)", Width = 82 });
            candidateGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "参数1", Width = 72 });
            candidateGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "参数2", Width = 72 });
            candidateGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "说明",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 180
            });
            candidateGroup.Controls.Add(candidateGrid);
            layout.Controls.Add(candidateGroup, 0, 4);
            return tab;
        }

        private TabPage BuildApiTab()
        {
            var tab = new TabPage("AI API 设置") { Padding = new Padding(8) };
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
                ColumnCount = 3,
                RowCount = 13,
                Padding = new Padding(0, 0, 12, 8)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190F));

            providerComboBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            savedProfileComboBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            protocolComboBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            protocolComboBox.Items.AddRange(new object[]
            {
                "Responses API (/responses)",
                "Chat Completions (/chat/completions)"
            });
            authenticationComboBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            authenticationComboBox.Items.AddRange(new object[]
            {
                "Bearer API Key",
                "api-key 请求头",
                "无需鉴权（本机网关）"
            });
            baseUrlTextBox = new TextBox { Dock = DockStyle.Fill };
            modelTextBox = new TextBox { Dock = DockStyle.Fill, Text = "gpt-5.6-sol" };
            projectTextBox = new TextBox { Dock = DockStyle.Fill };
            apiKeyTextBox = new TextBox { Dock = DockStyle.Fill, UseSystemPasswordChar = true };
            rememberKeyCheckBox = new CheckBox
            {
                AutoSize = true,
                Text = "保存到 Windows 凭据管理器",
                Padding = new Padding(0, 6, 0, 0)
            };
            credentialStatusLabel = new Label
            {
                AutoSize = true,
                Padding = new Padding(0, 8, 0, 0),
                Text = "尚未检查凭据"
            };
            testConnectionButton = new Button { Text = "测试连接", Width = 120, Height = 32 };
            testConnectionButton.Click += TestConnection;
            deleteCredentialButton = new Button { Text = "删除已保存密钥", Width = 150, Height = 32 };
            deleteCredentialButton.Click += DeleteStoredCredential;
            providerNoteLabel = new Label
            {
                AutoSize = true,
                MaximumSize = new Drawing.Size(1020, 0),
                Padding = new Padding(0, 12, 0, 4)
            };

            AddSettingRow(layout, 0, "服务预设", providerComboBox, "选择后自动填写；下方字段仍可编辑。", null);
            AddSettingRow(layout, 1, "已保存组合", savedProfileComboBox, "可重复保存和读取；插件启动时自动加载最近使用的组合。", null);
            AddSettingRow(layout, 2, "API Base URL", baseUrlTextBox, "远程接口必须使用 HTTPS；本机回环地址可用 HTTP。", null);
            AddSettingRow(layout, 3, "API 协议", protocolComboBox, "支持 Responses 与 Chat Completions。", null);
            AddSettingRow(layout, 4, "鉴权方式", authenticationComboBox, "由网关管理上游凭据时选择无需鉴权。", null);
            AddSettingRow(layout, 5, "模型 ID", modelTextBox, "须支持 JSON Schema 结构化输出。", null);
            AddSettingRow(layout, 6, "OpenAI Project ID", projectTextBox, "可选；主要用于 OpenAI 官方多项目账号。", null);
            AddSettingRow(layout, 7, "API 密钥", apiKeyTextBox, "密钥按组合单独保存到 Windows 凭据管理器，不写入配置文件。", null);
            AddSettingRow(layout, 8, "凭据保存", rememberKeyCheckBox, "启用后保存当前组合对应的密钥；不勾选则仅保存非敏感配置。", null);
            AddSettingRow(layout, 9, "当前状态", credentialStatusLabel, string.Empty, null);

            var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = false };
            buttons.Controls.Add(testConnectionButton);
            buttons.Controls.Add(deleteCredentialButton);
            saveProfileButton = new Button { Text = "保存当前组合", Width = 130, Height = 32 };
            saveProfileButton.Click += SaveApiProfile;
            deleteProfileButton = new Button { Text = "删除组合", Width = 100, Height = 32 };
            deleteProfileButton.Click += DeleteApiProfile;
            buttons.Controls.Add(saveProfileButton);
            buttons.Controls.Add(deleteProfileButton);
            layout.Controls.Add(new Label { Text = "连接操作", AutoSize = true, Padding = new Padding(0, 8, 0, 0) }, 0, 10);
            layout.Controls.Add(buttons, 1, 10);
            layout.SetColumnSpan(buttons, 2);

            layout.Controls.Add(providerNoteLabel, 0, 12);
            layout.SetColumnSpan(providerNoteLabel, 3);
            scrollPanel.Controls.Add(layout);
            tab.Controls.Add(scrollPanel);

            foreach (ApiProviderPreset preset in ApiProviderPreset.CreateDefaults())
                providerComboBox.Items.Add(preset);
            providerComboBox.SelectedIndexChanged += ProviderSelectionChanged;
            savedProfileComboBox.SelectedIndexChanged += SavedProfileSelectionChanged;
            authenticationComboBox.SelectedIndexChanged += AuthenticationSelectionChanged;
            providerComboBox.SelectedIndex = 0;
            RefreshSavedProfileList();
            return tab;
        }

        private static void AddSettingRow(
            TableLayoutPanel layout,
            int row,
            string label,
            Control control,
            string help,
            EventHandler handler)
        {
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            var labelControl = new Label { Text = label, AutoSize = true, Padding = new Padding(0, 8, 0, 0) };
            var helpControl = new Label
            {
                Text = help,
                AutoSize = true,
                Padding = new Padding(10, 8, 0, 0),
                ForeColor = Drawing.SystemColors.GrayText
            };
            if (handler != null)
                control.Click += handler;
            layout.Controls.Add(labelControl, 0, row);
            layout.Controls.Add(control, 1, row);
            layout.Controls.Add(helpControl, 2, row);
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
            projectTextBox.Clear();
            providerNoteLabel.Text = preset.Note + Environment.NewLine +
                "插件仅调用 OpenAI 兼容接口，不会读取 CC Switch、Codex、ChatGPT 或其他软件保存的密钥/OAuth。";
            UpdateAuthenticationControls();
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
                baseUrlTextBox.Text = profile.BaseUrl ?? string.Empty;
                protocolComboBox.SelectedIndex = profile.Protocol == ApiProtocol.ChatCompletions ? 1 : 0;
                authenticationComboBox.SelectedIndex = GetAuthenticationIndex(profile.AuthenticationMode);
                modelTextBox.Text = profile.Model ?? string.Empty;
                projectTextBox.Text = profile.ProjectId ?? string.Empty;
                rememberKeyCheckBox.Checked = profile.RememberApiKey;
                apiKeyTextBox.Text = string.Empty;
                try
                {
                    if (profile.RememberApiKey)
                    {
                        string storedKey = new WindowsCredentialStore(
                            ApiProfileStore.CredentialTargetFor(profile.Name)).Read();
                        if (!string.IsNullOrWhiteSpace(storedKey))
                            apiKeyTextBox.Text = storedKey;
                    }
                }
                catch (Exception ex)
                {
                    credentialStatusLabel.Text = "组合凭据读取失败：" + ex.Message;
                }

                providerNoteLabel.Text = "已加载组合：" + profile.Name + Environment.NewLine +
                    "插件仅调用 OpenAI 兼容接口，不会读取 CC Switch、Codex、ChatGPT 或其他软件保存的密钥/OAuth。";
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
                defaultName = preset == null ? "自定义 API" : preset.Name;
            }

            string name = PromptForText("保存 API 配置组合", "组合名称：", defaultName);
            if (string.IsNullOrWhiteSpace(name))
                return;
            name = name.Trim();
            if (name.Length > 80)
            {
                ShowError("组合名称不能超过 80 个字符。");
                return;
            }

            ApiProfileRecord profile = BuildCurrentProfileRecord(name);
            if (string.IsNullOrWhiteSpace(profile.BaseUrl) || string.IsNullOrWhiteSpace(profile.Model))
            {
                ShowError("请先填写 API Base URL 和模型 ID。");
                return;
            }

            ApiProfileRecord existing = savedApiProfiles.FirstOrDefault(item =>
                string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
                savedApiProfiles.Remove(existing);
            savedApiProfiles.Add(profile);
            apiProfileStore.Save(savedApiProfiles);

            if (profile.AuthenticationMode != ApiAuthenticationMode.None &&
                profile.RememberApiKey && !string.IsNullOrWhiteSpace(apiKeyTextBox.Text))
            {
                new WindowsCredentialStore(ApiProfileStore.CredentialTargetFor(profile.Name)).Write(apiKeyTextBox.Text);
                credentialStatusLabel.Text = "组合已保存；密钥已保存到 Windows 凭据管理器";
            }
            else
            {
                credentialStatusLabel.Text = "组合已保存；仅保存非敏感配置";
            }

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
                ShowError("请先选择要删除的配置组合。");
                return;
            }

            DialogResult result = MessageBox.Show(this,
                "确定删除配置组合“" + profile.Name + "”及其对应的已保存密钥吗？",
                "删除配置组合", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (result != DialogResult.OK)
                return;

            savedApiProfiles.RemoveAll(item => string.Equals(item.Name, profile.Name, StringComparison.OrdinalIgnoreCase));
            apiProfileStore.Save(savedApiProfiles);
            try
            {
                new WindowsCredentialStore(ApiProfileStore.CredentialTargetFor(profile.Name)).Delete();
            }
            catch (Exception ex)
            {
                ShowError("组合已删除，但删除对应凭据失败：" + ex.Message);
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
                var ok = new Button { Text = "确定", DialogResult = DialogResult.OK, Location = new Drawing.Point(264, 80), Width = 78 };
                var cancel = new Button { Text = "取消", DialogResult = DialogResult.Cancel, Location = new Drawing.Point(350, 80), Width = 78 };
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

            ApiProfileRecord updated = BuildCurrentProfileRecord(selected.Name);
            savedApiProfiles.RemoveAll(item => string.Equals(item.Name, selected.Name, StringComparison.OrdinalIgnoreCase));
            savedApiProfiles.Add(updated);
            apiProfileStore.Save(savedApiProfiles);
            RefreshSavedProfileList();
        }

        private void AuthenticationSelectionChanged(object sender, EventArgs e)
        {
            UpdateAuthenticationControls();
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
                credentialStatusLabel.Text = "无需插件密钥；凭据由本机网关管理";
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
                credentialSource = "无需鉴权";
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
                ProjectId = projectTextBox.Text
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

        private void AddAttachments(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog
            {
                Title = "选择任务资料",
                Multiselect = true,
                CheckFileExists = true,
                Filter = "支持的任务资料|*.pdf;*.docx;*.png;*.jpg;*.jpeg;*.webp;*.gif;*.txt;*.md;*.csv;*.tsv;*.json;*.xml;*.kml;*.gpx;*.yaml;*.yml;*.html;*.htm;*.log;*.ini;*.cfg|PDF 文件|*.pdf|Word 文档|*.docx|图像文件|*.png;*.jpg;*.jpeg;*.webp;*.gif|文本与数据文件|*.txt;*.md;*.csv;*.tsv;*.json;*.xml;*.kml;*.gpx;*.yaml;*.yml;*.html;*.htm;*.log;*.ini;*.cfg|所有文件|*.*"
            })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;

                var errors = new List<string>();
                foreach (string path in dialog.FileNames)
                {
                    try
                    {
                        attachments.Add(attachmentProcessor.Load(path, attachments));
                    }
                    catch (Exception ex)
                    {
                        errors.Add(ex.Message);
                    }
                }
                RefreshAttachmentGrid();
                InvalidateGeneratedResult("附件已变化，请重新生成并确认任务要求。");
                if (errors.Count > 0)
                    MessageBox.Show(this, string.Join(Environment.NewLine, errors), "部分文件未添加",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void RemoveSelectedAttachments(object sender, EventArgs e)
        {
            int[] indexes = attachmentGrid.SelectedRows.Cast<DataGridViewRow>()
                .Select(row => row.Index)
                .Where(index => index >= 0 && index < attachments.Count)
                .Distinct()
                .OrderByDescending(index => index)
                .ToArray();
            if (indexes.Length == 0)
                return;
            foreach (int index in indexes)
                attachments.RemoveAt(index);
            RefreshAttachmentGrid();
            InvalidateGeneratedResult("附件已变化，请重新生成并确认任务要求。");
        }

        private void ClearAttachments(object sender, EventArgs e)
        {
            if (attachments.Count == 0)
                return;
            attachments.Clear();
            RefreshAttachmentGrid();
            InvalidateGeneratedResult("附件已清空，请重新生成并确认任务要求。");
        }

        private void RefreshAttachmentGrid()
        {
            attachmentGrid.Rows.Clear();
            foreach (MissionAttachment attachment in attachments)
            {
                attachmentGrid.Rows.Add(
                    attachment.DisplayName,
                    attachment.MediaType,
                    FormatFileSize(attachment.SizeBytes),
                    attachment.Status);
            }
        }

        private void InvalidateGeneratedResult(string status)
        {
            currentResult = null;
            lastResponseData = null;
            applyButton.Enabled = false;
            viewResponseButton.Enabled = false;
            confirmRequirementsCheckBox.Checked = false;
            confirmRequirementsCheckBox.Enabled = false;
            summaryTextBox.Clear();
            validationTextBox.Text = status;
            candidateGrid.Rows.Clear();
            activityLabel.Text = "等待重新生成";
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
            if (string.IsNullOrWhiteSpace(objectiveTextBox.Text) && attachments.Count == 0)
            {
                MessageBox.Show(this, "请先输入任务目标或添加包含任务要求的文件。", "缺少任务资料", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            SetBusy(true, "正在请求 GPT 并执行本地校验...");
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

            try
            {
                MissionContext context = CaptureMissionContext();
                TaskSpec spec;
                using (var client = new OpenAiResponsesClient())
                {
                    try
                    {
                        spec = await client.GenerateTaskSpecAsync(
                            string.IsNullOrWhiteSpace(objectiveTextBox.Text) ? "请依据附件理解并整理任务要求。" : objectiveTextBox.Text,
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

                var result = new MissionGenerationResult { Spec = spec };
                result.Validation.Merge(validator.ValidateSpec(spec, context));
                ValidateAttachmentAcknowledgement(result.Validation, spec);
                if (result.Validation.IsValid)
                {
                    try
                    {
                        result.Mission = compiler.Compile(spec, context);
                        result.Validation.Merge(validator.ValidateMission(result.Mission, context.Home));
                    }
                    catch (Exception ex)
                    {
                        result.Validation.Errors.Add("本地任务编译失败：" + ex.Message);
                    }
                }

                currentResult = result;
                DisplayResult(result);

                if (settings.AuthenticationMode != ApiAuthenticationMode.None && rememberKeyCheckBox.Checked)
                {
                    WriteCurrentCredential(settings.ApiKey);
                    credentialStatusLabel.Text = "已保存到 Windows 凭据管理器";
                }
                else
                {
                    credentialStatusLabel.Text = "本次使用：" + credentialSource;
                }
                MarkCurrentProfileUsed();
            }
            catch (OperationCanceledException)
            {
                validationTextBox.Text = "请求已取消。";
                activityLabel.Text = "请求已取消";
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                if (cancellation != null)
                {
                    cancellation.Dispose();
                    cancellation = null;
                }
                SetBusy(false, currentResult != null && currentResult.Validation.IsValid
                    ? "候选任务已通过本地校验"
                    : "未生成可应用的候选任务");
            }
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
            credentialStatusLabel.Text = "正在测试连接...";
            try
            {
                using (var client = new OpenAiResponsesClient())
                {
                    try
                    {
                        await client.TestConnectionAsync(settings, CancellationToken.None);
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
                    credentialStatusLabel.Text = "连接成功；密钥已保存到 Windows 凭据管理器";
                }
                else
                {
                    credentialStatusLabel.Text = "连接成功；凭据来源：" + source;
                }
                MarkCurrentProfileUsed();
            }
            catch (Exception ex)
            {
                credentialStatusLabel.Text = "连接失败";
                ShowError(ex.Message);
            }
            finally
            {
                testConnectionButton.Enabled = true;
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
                    "当前 Flight Planner 高度模式不是 Relative。请切换为相对 Home 高度后重新检查候选任务。",
                    "高度模式不匹配", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsStandardMissionType())
            {
                MessageBox.Show(this,
                    "当前 Flight Planner 不是普通 Mission 任务类型。请切换到 Mission 后再应用候选任务。",
                    "任务类型不匹配", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PointLatLngAlt currentHome = CaptureMissionContext().Home;
            ValidationResult revalidation = validator.ValidateMission(currentResult.Mission, currentHome);
            if (!revalidation.IsValid)
            {
                validationTextBox.Text = string.Join(Environment.NewLine,
                    revalidation.Errors.Select(error => "错误：" + error));
                applyButton.Enabled = false;
                MessageBox.Show(this, "任务状态已变化，重新校验未通过；未应用任何任务项。",
                    "校验失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int existing = GetExistingMissionItemCount();
            string existingWarning = existing > 0
                ? "当前飞行计划已有 " + existing + " 个任务项，本插件将追加而不会替换。\r\n\r\n"
                : string.Empty;
            DialogResult confirmation = MessageBox.Show(this,
                existingWarning +
                "确认把 " + currentResult.Mission.Items.Count + " 个候选任务项追加到本地 Flight Planner 列表？\r\n" +
                "此操作不会上传到飞控，之后仍须人工逐项检查并手动写入。",
                "确认应用候选任务", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
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
                        throw new InvalidOperationException(
                            "Mission Planner 当前编辑模式改变了任务命令，请关闭样条航点等自动转换后重试。");
                    }
                }
            }
            catch (Exception ex)
            {
                RollBackAppendedRows(originalMissionItemCount);
                MessageBox.Show(this,
                    "追加任务项时发生错误，本次已添加的行已撤回。请检查 Flight Planner：\r\n" + ex.Message,
                    "应用失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                plugin.Host.MainForm.FlightPlanner.CHK_verifyheight.Checked = previousVerifyHeight;
                plugin.Host.MainForm.FlightPlanner.quickadd = previousQuickAdd;
                plugin.Host.MainForm.FlightPlanner.writeKML();
            }

            applyButton.Enabled = false;
            activityLabel.Text = "已追加到本地飞行计划，尚未上传";
            MessageBox.Show(this,
                "候选任务已追加到本地 Flight Planner 列表。请人工检查航线、高度、空域、返航与失效保护设置，再决定是否手动写入飞控。",
                "已应用到本地计划", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            string missionType = result.Spec == null ? "未知" : result.Spec.mission_type;
            int itemCount = result.Mission == null ? 0 : result.Mission.Items.Count;
            TaskSpec spec = result.Spec;
            var summaryLines = new List<string>();
            if (spec != null)
            {
                summaryLines.Add("理解摘要：" + (spec.source_summary ?? string.Empty));
                summaryLines.Add("确认要求：");
                if (spec.confirmed_requirements != null)
                {
                    summaryLines.AddRange(spec.confirmed_requirements
                        .Where(requirement => !string.IsNullOrWhiteSpace(requirement))
                        .Select(requirement => "  - " + requirement.Trim()));
                }
                string files = spec.source_files_used == null
                    ? string.Empty
                    : string.Join("、", spec.source_files_used.Where(name => !string.IsNullOrWhiteSpace(name)));
                summaryLines.Add("已使用文件：" + (string.IsNullOrWhiteSpace(files) ? "无" : files));
                summaryLines.Add("候选任务：" + (spec.summary ?? string.Empty));
            }
            summaryLines.Add("模板：" + missionType + "；候选任务项：" + itemCount);
            summaryTextBox.Text = string.Join(Environment.NewLine, summaryLines);

            var lines = result.Validation.Errors.Select(error => "错误：" + error)
                .Concat(result.Validation.Warnings.Select(warning => "提示：" + warning))
                .ToArray();
            validationTextBox.Text = lines.Length == 0 ? "本地校验通过。" : string.Join(Environment.NewLine, lines);

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

        private void ValidateAttachmentAcknowledgement(ValidationResult validation, TaskSpec spec)
        {
            if (validation == null || spec == null)
                return;

            string[] usedFiles = (spec.source_files_used ?? new List<string>())
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(name => name.Trim())
                .ToArray();
            if (attachments.Count > 0 && usedFiles.Length == 0)
                validation.Errors.Add("AI 未确认使用任何已添加文件，请重新生成或检查模型的文件理解能力。");
            if (attachments.Count == 0 && usedFiles.Length > 0)
                validation.Errors.Add("AI 声称使用了并未添加的文件，结果不可信。请重新生成。");

            var available = new HashSet<string>(attachments.Select(item => item.DisplayName), StringComparer.OrdinalIgnoreCase);
            foreach (string usedFile in usedFiles)
            {
                if (!available.Contains(usedFile))
                    validation.Errors.Add("AI 声称使用了未知文件：" + usedFile);
            }
        }

        private string ResolveApiKey(out string source)
        {
            if (!string.IsNullOrWhiteSpace(apiKeyTextBox.Text))
            {
                source = "会话输入";
                return apiKeyTextBox.Text.Trim();
            }

            string environmentKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            if (!string.IsNullOrWhiteSpace(environmentKey))
            {
                source = "OPENAI_API_KEY";
                return environmentKey.Trim();
            }

            string storedKey = null;
            ApiProfileRecord profile = SelectedApiProfile;
            if (profile != null && profile.RememberApiKey)
                storedKey = new WindowsCredentialStore(ApiProfileStore.CredentialTargetFor(profile.Name)).Read();
            if (string.IsNullOrWhiteSpace(storedKey))
                storedKey = credentialStore.Read();
            if (!string.IsNullOrWhiteSpace(storedKey))
            {
                source = "Windows 凭据管理器";
                return storedKey.Trim();
            }

            source = "无";
            return null;
        }

        private void WriteCurrentCredential(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                return;

            ApiProfileRecord profile = SelectedApiProfile;
            if (profile == null)
                credentialStore.Write(apiKey);
            else
                new WindowsCredentialStore(ApiProfileStore.CredentialTargetFor(profile.Name)).Write(apiKey);
        }

        private void LoadCredentialStatus()
        {
            if (authenticationComboBox != null && GetAuthenticationMode() == ApiAuthenticationMode.None)
            {
                credentialStatusLabel.Text = "无需插件密钥；凭据由本机网关管理";
                return;
            }

            try
            {
                string source;
                string apiKey = ResolveApiKey(out source);
                credentialStatusLabel.Text = string.IsNullOrWhiteSpace(apiKey)
                    ? "未发现可用 API 密钥"
                    : "已发现凭据：" + source + "（密钥内容不显示）";
            }
            catch (Exception ex)
            {
                credentialStatusLabel.Text = "凭据检查失败：" + ex.Message;
            }
        }

        private void DeleteStoredCredential(object sender, EventArgs e)
        {
            try
            {
                ApiProfileRecord profile = SelectedApiProfile;
                bool deleted = profile == null
                    ? credentialStore.Delete()
                    : new WindowsCredentialStore(ApiProfileStore.CredentialTargetFor(profile.Name)).Delete();
                credentialStatusLabel.Text = deleted ? "已删除 Windows 凭据" : "Windows 凭据管理器中没有已保存密钥";
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void SetBusy(bool busy, string status)
        {
            generateButton.Enabled = !busy;
            cancelButton.Enabled = busy;
            testConnectionButton.Enabled = !busy;
            deleteCredentialButton.Enabled = !busy;
            closeButton.Enabled = !busy;
            objectiveTextBox.Enabled = !busy;
            attachmentGrid.Enabled = !busy;
            addAttachmentButton.Enabled = !busy;
            removeAttachmentButton.Enabled = !busy;
            clearAttachmentsButton.Enabled = !busy;
            providerComboBox.Enabled = !busy;
            protocolComboBox.Enabled = !busy;
            authenticationComboBox.Enabled = !busy;
            baseUrlTextBox.Enabled = !busy;
            modelTextBox.Enabled = !busy;
            projectTextBox.Enabled = !busy;
            apiKeyTextBox.Enabled = !busy && GetAuthenticationMode() != ApiAuthenticationMode.None;
            rememberKeyCheckBox.Enabled = !busy && GetAuthenticationMode() != ApiAuthenticationMode.None;
            activityLabel.Text = status;
            Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
        }

        private void ViewModelResponse(object sender, EventArgs e)
        {
            if (lastResponseData == null)
                return;

            using (var dialog = new ModelResponseDialog(lastResponseData))
            {
                ThemeManager.ApplyThemeTo(dialog);
                dialog.ShowDialog(this);
            }
        }

        private void ShowError(string message)
        {
            validationTextBox.Text = "错误：" + message;
            activityLabel.Text = "发生错误";
            string responseHint = lastResponseData == null
                ? string.Empty
                : "\r\n\r\n可点击“查看模型返回数据”查看 HTTP 状态、原始响应和完整诊断。";
            MessageBox.Show(this, message + responseHint, "AI 航点规划", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
