using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class LLMConfigDialog : Form
    {
        private TextBox txtApiKey;
        private TextBox txtApiUrl;
        private TextBox txtModel;
        private TextBox txtTemperature;
        private Button btnSave;
        private Button btnCancel;
        private LinkLabel lnkGetKey;
        private Label lblStatus;
        private CheckBox chkEnableTranslation;

        // 新增控件
        private TextBox txtSystemPrompt;
        private Button btnResetPrompt;

        private static readonly ComponentResourceManager resources =
            new ComponentResourceManager(typeof(LLMConfigDialog));

        // 读取本地化字符串；缺失时回退到内置英文
        private static string L(string key, string fallback)
        {
            try
            {
                var value = resources.GetString(key);
                return string.IsNullOrEmpty(value) ? fallback : value;
            }
            catch (Exception)
            {
                return fallback;
            }
        }

        // 默认系统提示
        private const string DEFAULT_SYSTEM_PROMPT =
            "You are a professional ardupilot  technical document translator. " +
            "Translate the following ArduPilot parameter description from English to Simplified Chinese. " +
            "Keep technical terms accurate. Output ONLY the Chinese translation, no explanation.";

        public LLMConfigDialog()
        {
            InitializeComponent();
            SetupUI();

            if (!IsInDesignMode())
                LoadSettings();
        }

        private static bool IsInDesignMode()
        {
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
        }

        private void LLMConfigDialog_Load(object sender, EventArgs e)
        {
        }

        private void SetupUI()
        {
            this.Controls.Clear();
            this.Name = "LLMConfigDialog";
            this.Text = L("$this.Text", "LLM Translation Settings");
            this.Size = new Size(580, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = SystemColors.Window;

            int currentY = 20;

            // 启用翻译复选框
            chkEnableTranslation = new CheckBox()
            {
                Name = "chkEnableTranslation",
                Text = L("chkEnableTranslation.Text", "Enable LLM translation"),
                Location = new Point(20, currentY),
                Size = new Size(200, 25),
                Checked = true
            };
            currentY += 35;

            // API Key 标签和文本框
            var lblKey = new Label()
            {
                Name = "lblKey",
                Text = L("lblKey.Text", "API Key:"),
                Location = new Point(20, currentY),
                Size = new Size(80, 25)
            };

            txtApiKey = new TextBox()
            {
                Location = new Point(110, currentY),
                Size = new Size(430, 25),
                PasswordChar = '*'
            };
            currentY += 35;

            // API URL 标签和文本框
            var lblUrl = new Label()
            {
                Name = "lblUrl",
                Text = L("lblUrl.Text", "API URL:"),
                Location = new Point(20, currentY),
                Size = new Size(80, 25)
            };

            txtApiUrl = new TextBox()
            {
                Location = new Point(110, currentY),
                Size = new Size(430, 25)
            };
            txtApiUrl.Text = LLMTranslationService.DefaultApiUrl;
            currentY += 35;

            var lblModel = new Label()
            {
                Name = "lblModel",
                Text = L("lblModel.Text", "Model:"),
                Location = new Point(20, currentY),
                Size = new Size(80, 25)
            };

            txtModel = new TextBox()
            {
                Location = new Point(110, currentY),
                Size = new Size(430, 25)
            };
            txtModel.Text = "";
            currentY += 35;

            var lblTemperature = new Label()
            {
                Name = "lblTemperature",
                Text = L("lblTemperature.Text", "Temperature:"),
                Location = new Point(20, currentY),
                Size = new Size(80, 25)
            };

            txtTemperature = new TextBox()
            {
                Location = new Point(110, currentY),
                Size = new Size(120, 25),
                Text = "1.3"
            };

            var lblTemperatureHint = new Label()
            {
                Name = "lblTemperatureHint",
                Text = L("lblTemperatureHint.Text", "Suggested 1.3 (range 0-2)"),
                Location = new Point(240, currentY),
                Size = new Size(300, 25),
                ForeColor = Color.Gray
            };
            currentY += 35;

            // 获取 API Key 链接
            lnkGetKey = new LinkLabel()
            {
                Text = L("lnkGetKey.Text", "Get an API key"),
                Location = new Point(110, currentY),
                Size = new Size(200, 25),
                LinkColor = Color.Blue
            };
            lnkGetKey.LinkClicked += (s, e) =>
            {
                try
                {
                    System.Diagnostics.Process.Start("https://platform.openai.com/api-keys");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        string.Format(L("msg.cannotOpenBrowser", "Could not open the browser: {0}"), ex.Message),
                        L("msg.error", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            currentY += 35;

            // ---- 新增：自定义系统提示词区域 ----
            var lblSystemPrompt = new Label()
            {
                Name = "lblSystemPrompt",
                Text = L("lblSystemPrompt.Text", "Custom system prompt (System Prompt):"),
                Location = new Point(20, currentY),
                Size = new Size(250, 25),
                Font = new Font(this.Font, FontStyle.Bold)
            };
            currentY += 25;

            txtSystemPrompt = new TextBox()
            {
                Location = new Point(20, currentY),
                Size = new Size(520, 100),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                WordWrap = true,
                Font = new Font("Consolas", 9)
            };
            currentY += 110;

            btnResetPrompt = new Button()
            {
                Name = "btnResetPrompt",
                Text = L("btnResetPrompt.Text", "Reset to default"),
                Location = new Point(430, currentY),
                Size = new Size(110, 28),
                FlatStyle = FlatStyle.Standard
            };
            btnResetPrompt.Click += BtnResetPrompt_Click;
            currentY += 40;

            // 使用说明标签
            var lblHint = new Label()
            {
                Name = "lblHint",
                Text = L("lblHint.Text", "Tip: changing the prompt alters the translation style or output language."),
                Location = new Point(20, currentY),
                Size = new Size(540, 25),
                ForeColor = Color.Gray,
                Font = new Font(this.Font, FontStyle.Italic)
            };
            currentY += 30;

            // 状态标签
            lblStatus = new Label()
            {
                Location = new Point(20, currentY),
                Size = new Size(520, 70),
                ForeColor = Color.Gray,
                Name = "lblStatus",
                Text = L("lblStatus.Text", "Waiting for configuration..."),
                Font = new Font(this.Font, FontStyle.Regular)
            };
            currentY += 80;

            // 按钮面板
            var panel = new Panel()
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = SystemColors.Control
            };

            btnSave = new Button()
            {
                Name = "btnSave",
                Text = L("btnSave.Text", "Save and test"),
                Location = new Point(panel.Width - 180, 10),
                Size = new Size(90, 30),
                BackColor = Color.LightGreen,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button()
            {
                Name = "btnCancel",
                Text = L("btnCancel.Text", "Cancel"),
                Location = new Point(panel.Width - 85, 10),
                Size = new Size(80, 30),
                DialogResult = DialogResult.Cancel,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            panel.Controls.AddRange(new Control[] { btnSave, btnCancel });

            // 将所有控件添加到窗体
            this.Controls.AddRange(new Control[] {
                chkEnableTranslation, lblKey, txtApiKey, lblUrl, txtApiUrl,
                lblModel, txtModel,
                lblTemperature, txtTemperature, lblTemperatureHint,
                lnkGetKey, lblSystemPrompt, txtSystemPrompt, btnResetPrompt,
                lblHint, lblStatus, panel
            });

            // 文本已由上面的 L(...) 从 LLMConfigDialog.resx 读取，不再走 ApplyResource 递归
            LLMTranslationService.OriginalTextLabel = L("translation.originalLabel", "Original: ");
        }

        private void BtnResetPrompt_Click(object sender, EventArgs e)
        {
            txtSystemPrompt.Text = DEFAULT_SYSTEM_PROMPT;
            // 仅重置文本框内容，不自动保存。用户需点击“保存并测试”才会持久化。
        }

        private void LoadSettings()
        {
            try
            {
                txtApiKey.Text = GetSetting("LLM_API_KEY", "");
                string savedUrl = GetSetting("LLM_API_URL", "");
                if (!string.IsNullOrEmpty(savedUrl))
                    txtApiUrl.Text = savedUrl;

                txtModel.Text = GetSetting("LLM_MODEL", "");
                txtTemperature.Text = GetSetting("LLM_TEMPERATURE", "1.3");

                bool enable;
                if (!bool.TryParse(GetSetting("LLM_ENABLE", "true"), out enable))
                    enable = true;
                chkEnableTranslation.Checked = enable;

                // 加载自定义系统提示词（若无则使用默认）
                string savedPrompt = GetSetting("LLM_SYSTEM_PROMPT", null);
                if (string.IsNullOrEmpty(savedPrompt))
                    txtSystemPrompt.Text = DEFAULT_SYSTEM_PROMPT;
                else
                    txtSystemPrompt.Text = savedPrompt;

                // 同步到服务
                LLMTranslationService.SetSystemPrompt(txtSystemPrompt.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load LLM settings: {ex.Message}");
            }

            UpdateStatus();
        }

        private void SaveSettings()
        {
            SaveSetting("LLM_API_KEY", txtApiKey.Text);
            SaveSetting("LLM_API_URL", txtApiUrl.Text);
            SaveSetting("LLM_MODEL", txtModel.Text);
            SaveSetting("LLM_TEMPERATURE", txtTemperature.Text);
            SaveSetting("LLM_ENABLE", chkEnableTranslation.Checked.ToString());
            SaveSetting("LLM_SYSTEM_PROMPT", txtSystemPrompt.Text);

            // 更新服务中的自定义提示词
            LLMTranslationService.SetSystemPrompt(txtSystemPrompt.Text);
        }

        private string GetSetting(string key, string defaultValue)
        {
            return Settings.Instance.GetString(key, defaultValue);
        }

        private void SaveSetting(string key, string value)
        {
            Settings.Instance[key] = value;
        }

        private void UpdateStatus()
        {
            if (!chkEnableTranslation.Checked)
            {
                lblStatus.Text = L("status.disabled", "✗ Translation is disabled");
                lblStatus.ForeColor = Color.Gray;
                return;
            }

            if (!string.IsNullOrEmpty(txtApiKey.Text))
            {
                lblStatus.Text = L("status.configured",
                    "✓ API configured\r\n\r\nUsage:\r\n" +
                    "• right-click a parameter row → Translate, or use the \"Translate selected/all\" toolbar buttons\r\n");
                lblStatus.ForeColor = Color.Green;
            }
            else
            {
                lblStatus.Text = L("status.noKey",
                    "⚠ No API key configured\r\n\r\nPlease provide an API key.");
                lblStatus.ForeColor = Color.Orange;
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            double temperature;
            if (!double.TryParse((txtTemperature.Text ?? string.Empty).Trim(), NumberStyles.Float,
                CultureInfo.InvariantCulture, out temperature) || temperature < 0 || temperature > 2)
            {
                MessageBox.Show(
                    L("msg.badTemperature", "Temperature must be a number between 0 and 2 (1.3 is suggested for translation)."),
                    L("msg.badParameter", "Invalid value"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            SaveSettings();   // 保存所有设置（包括提示词）

            if (chkEnableTranslation.Checked && !string.IsNullOrEmpty(txtApiKey.Text))
            {
                LLMTranslationService.Configure(txtApiKey.Text, txtApiUrl.Text, txtModel.Text, temperature);

                btnSave.Text = L("btnSave.testing", "Testing...");
                btnSave.Enabled = false;

                bool testResult = await LLMTranslationService.TestConnection();

                btnSave.Text = L("btnSave.Text", "Save and test");
                btnSave.Enabled = true;

                if (testResult)
                {
                    MessageBox.Show(
                        L("msg.saveSuccess", "✓ API configured successfully!\r\n\r\nThe translation feature is ready; use it from the parameter table."),
                        L("msg.successTitle", "Configuration saved"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show(
                        L("msg.testFailed", "✗ API connection test failed\r\n\r\nCheck the API key, the API URL and your network connection.\r\n\r\nDebug info:\r\n") +
                        LLMTranslationService.LastDebugMessage,
                        L("msg.failureTitle", "Configuration failed"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.DialogResult = DialogResult.None;
                    return;
                }
            }
            else if (!chkEnableTranslation.Checked)
            {
                LLMTranslationService.Configure(string.Empty, txtApiUrl.Text, txtModel.Text, temperature);
                MessageBox.Show(L("msg.disabledSaved", "Settings saved. Translation is disabled."),
                    L("msg.savedTitle", "Saved"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show(L("msg.needKey", "Please enter an API key."),
                    L("msg.noticeTitle", "Notice"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            UpdateStatus();
            this.Close();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(580, 700);
            this.Name = "LLMConfigDialog";
            this.ResumeLayout(false);
        }

        private void LLMConfigDialog_Load_1(object sender, EventArgs e)
        {

        }
    }
}