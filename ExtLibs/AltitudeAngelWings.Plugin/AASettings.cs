using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Forms;
using AltitudeAngelWings.ApiClient.Client;
using AltitudeAngelWings.Extra;
using AltitudeAngelWings.Plugin.Properties;
using AltitudeAngelWings.Service;
using MissionPlanner;
using MissionPlanner.Utilities;

namespace AltitudeAngelWings.Plugin
{
    internal partial class AASettings : Form
    {
        private readonly ISettings _settings;
        private readonly IAltitudeAngelService _altitudeAngelService;
        private readonly IMissionPlanner _missionPlanner;

        public AASettings()
            : this(ServiceLocator.GetService<ISettings>(),
                ServiceLocator.GetService<IAltitudeAngelService>(),
                ServiceLocator.GetService<IMissionPlanner>())
        {
        }

        private AASettings(ISettings settings, IAltitudeAngelService altitudeAngelService, IMissionPlanner missionPlanner)
        {
            _settings = settings;
            _altitudeAngelService = altitudeAngelService;
            _missionPlanner = missionPlanner;
            InitializeComponent();
            _altitudeAngelService.IsSignedIn.ObserveOn(MainV2.instance).Subscribe(OnSignInChange);
            _missionPlanner.FlightDataMap.MapChanged.ObserveOn(MainV2.instance).Subscribe(OnMapChanged);
            _missionPlanner.FlightPlanningMap.MapChanged.ObserveOn(MainV2.instance).Subscribe(OnMapChanged);

            ThemeManager.ApplyThemeTo(this);
            pic_AboutLogo.Image = Image.FromStream(new MemoryStream(Resources.AALogo));

            // load settings
            chk_FlightReportEnable.Checked = _settings.FlightReportEnable;
            chk_UseExistingFlightPlanId.Checked = _settings.UseExistingFlightPlanId;
            txt_ExistingFlightPlanId.Text = _settings.ExistingFlightPlanId == Guid.Empty ? "" : _settings.ExistingFlightPlanId.ToString();
            txt_FlightReportName.Text = _settings.FlightReportName;
            txt_FlightReportDescription.Text = _settings.FlightReportDescription;
            chk_FlightReportCommercial.Checked = _settings.FlightReportCommercial;
            chk_FlightReportLocalScope.Checked = _settings.UseFlightPlanLocalScope;
            txt_FlightReportDuration.Text = ((int)_settings.FlightReportTimeSpan.TotalMinutes).ToString();
            but_SignIn.Enabled = !_altitudeAngelService.IsSignedIn;
            but_SignOut.Enabled = _altitudeAngelService.IsSignedIn;
            chk_OverrideClientSettings.Checked = _settings.OverrideClientUrlSettings;
            txt_OverrideClientId.Text = _settings.OverrideClientId;
            txt_OverrideClientSecret.Text = _settings.OverrideClientSecret;
            txt_OverrideUrlSuffix.Text = _settings.OverrideUrlDomainSuffix;
            var opacityAdjust = (int)_settings.MapOpacityAdjust * 100;
            if (opacityAdjust >= trk_OpacityAdjust.Minimum && opacityAdjust <= trk_OpacityAdjust.Maximum)
            {
                trk_OpacityAdjust.Value = opacityAdjust;
            }
            chk_EnableDataMap.Checked = _settings.EnableDataMap;
            chk_EnablePlanMap.Checked = _settings.EnablePlanMap;
            txt_ContactPhone.Text = _settings.FlightPhoneNumber;
            chk_AllowSms.Checked = _settings.FlightAllowSms;
            chk_IcaoAddress.Checked = _settings.FlightIdentifierIcao;
            txt_IcaoAddress.Text = _settings.FlightIdentifierIcaoAddress;
            chk_SerialNumber.Checked = _settings.FlightIdentifierSerial;
            txt_SerialNumber.Text = _settings.FlightIdentifierSerialNumber;
            web_About.DocumentText = Resources.About;

            RefreshControlStates();
        }

        protected override void OnLoad(EventArgs e)
        {
            if (Owner != null)
            {
                Location = new Point(
                    Owner.Location.X + Owner.Width / 2 - Width / 2,
                    Owner.Location.Y + Owner.Height / 2 - Height / 2);
            }
            base.OnLoad(e);
        }

        private void but_SignIn_Click(object sender, EventArgs e)
        {
            _altitudeAngelService.SignInAsync();
            RefreshControlStates();
        }

        private void but_SignOut_Click(object sender, EventArgs e)
        {
            _altitudeAngelService.DisconnectAsync();
            RefreshControlStates();
        }

        private void trv_MapLayers_AfterCheck(object sender, TreeViewEventArgs e)
        {
            var item = (FilterInfoDisplay)e.Node.Tag;
            item.Visible = e.Node.Checked;
            ProcessMapsFromCache();
        }

        private void txt_FlightReportName_TextChanged(object sender, EventArgs e)
        {
            _settings.FlightReportName = txt_FlightReportName.Text;
            RefreshControlStates();
        }

        private void txt_FlightReportDuration_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txt_FlightReportDuration.Text, out var minutes))
            {
                _settings.FlightReportTimeSpan = TimeSpan.FromMinutes(minutes);
            }
            RefreshControlStates();
        }

        private void chk_FlightReportCommercial_CheckedChanged(object sender, EventArgs e)
        {
            _settings.FlightReportCommercial = chk_FlightReportCommercial.Checked;
            RefreshControlStates();
        }

        private void chk_FlightReportEnable_CheckedChanged(object sender, EventArgs e)
        {
            _settings.FlightReportEnable = chk_FlightReportEnable.Checked;
            RefreshControlStates();
        }

        private void lbl_FlightReportWhat_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://bit.ly/aamissionplanner1");
        }

        private void chk_UseExistingFlightPlanId_CheckedChanged(object sender, EventArgs e)
        {
            _settings.UseExistingFlightPlanId = chk_UseExistingFlightPlanId.Checked;
            RefreshControlStates();
        }

        private void txt_ExistingFlightPlanId_TextChanged(object sender, EventArgs e)
        {
            _settings.ExistingFlightPlanId = Guid.TryParse(txt_ExistingFlightPlanId.Text, out var id) ? id : Guid.Empty;
            RefreshControlStates();
        }

        private void chk_FlightReportLocalScope_CheckedChanged(object sender, EventArgs e)
        {
            _settings.UseFlightPlanLocalScope = chk_FlightReportLocalScope.Checked;
            RefreshControlStates();
        }

        private void txt_FlightReportDescription_TextChanged(object sender, EventArgs e)
        {
            _settings.FlightReportDescription = txt_FlightReportDescription.Text;
            RefreshControlStates();
        }

        private void chk_OverrideClientSettings_CheckedChanged(object sender, EventArgs e)
        {
            var changed = _settings.OverrideClientUrlSettings != chk_OverrideClientSettings.Checked;
            _settings.OverrideClientUrlSettings = chk_OverrideClientSettings.Checked;
            RefreshControlStates(changed);
        }

        private void txt_OverrideClientId_TextChanged(object sender, EventArgs e)
        {
            var changed = _settings.OverrideClientId != txt_OverrideClientId.Text;
            _settings.OverrideClientId = txt_OverrideClientId.Text;
            RefreshControlStates(changed);
        }

        private void txt_OverrideClientSecret_TextChanged(object sender, EventArgs e)
        {
            var changed = _settings.OverrideClientSecret != txt_OverrideClientSecret.Text;
            _settings.OverrideClientSecret = txt_OverrideClientSecret.Text;
            RefreshControlStates(changed);
        }

        private void txt_OverrideUrlSuffix_TextChanged(object sender, EventArgs e)
        {
            var changed = _settings.OverrideUrlDomainSuffix != txt_OverrideUrlSuffix.Text;
            _settings.OverrideUrlDomainSuffix = txt_OverrideUrlSuffix.Text;
            RefreshControlStates(changed);
        }

        private void trk_OpacityAdjust_ValueChanged(object sender, EventArgs e)
        {
            _settings.MapOpacityAdjust = (float)trk_OpacityAdjust.Value / 100;
            RefreshControlStates(processMap: true);
        }

        private void but_Enable_Click(object sender, EventArgs e)
        {
            _settings.CheckEnableAltitudeAngel = true;
            RefreshControlStates();
            tabPages.SelectTab(tabPageAccount);
            but_SignIn_Click(sender, e);
        }

        private void but_Disable_Click(object sender, EventArgs e)
        {
            _settings.CheckEnableAltitudeAngel = false;
            RefreshControlStates(true);
        }

        private void btn_DefaultLayers_Click(object sender, EventArgs e)
        {
            RefreshControlStates(processMap: true, resetFilters: true);
        }

        private void chk_EnableDataMap_CheckedChanged(object sender, EventArgs e)
        {
            _settings.EnableDataMap = chk_EnableDataMap.Checked;
            RefreshControlStates(processMap: true);
        }

        private void chk_EnablePlanMap_CheckedChanged(object sender, EventArgs e)
        {
            _settings.EnablePlanMap = chk_EnablePlanMap.Checked;
            RefreshControlStates(processMap: true);
        }

        private void txt_ContactPhone_TextChanged(object sender, EventArgs e)
        {
            _settings.FlightPhoneNumber = txt_ContactPhone.Text;
            RefreshControlStates();
        }

        private void chk_AllowSms_CheckedChanged(object sender, EventArgs e)
        {
            _settings.FlightAllowSms = chk_AllowSms.Checked;
            RefreshControlStates();
        }

        private void chk_IcaoAddress_CheckedChanged(object sender, EventArgs e)
        {
            _settings.FlightIdentifierIcao = chk_IcaoAddress.Checked;
            RefreshControlStates();
        }

        private void txt_IcaoAddress_TextChanged(object sender, EventArgs e)
        {
            _settings.FlightIdentifierIcaoAddress = txt_IcaoAddress.Text;
            RefreshControlStates();
        }

        private void chk_SerialNumber_CheckedChanged(object sender, EventArgs e)
        {
            _settings.FlightIdentifierSerial = chk_SerialNumber.Checked;
            RefreshControlStates();
        }

        private void txt_SerialNumber_TextChanged(object sender, EventArgs e)
        {
            _settings.FlightIdentifierSerialNumber = txt_SerialNumber.Text;
            RefreshControlStates();
        }

        private void OnSignInChange(bool signedIn)
        {
            var user = _altitudeAngelService.CurrentUser;
            lbl_UserDetails.Text = signedIn
                ? $"{user.FirstName} {user.LastName}\r\n{user.EmailAddress}\r\n{user.UserId}": string.Empty;
            RefreshControlStates();
        }

        private void OnMapChanged(Unit unit)
        {
            RefreshControlStates();
        }

        private void ProcessMapsFromCache(bool resetFilters = false)
        {
            _altitudeAngelService.ProcessAllFromCache(_missionPlanner.FlightDataMap, resetFilters);
            _altitudeAngelService.ProcessAllFromCache(_missionPlanner.FlightPlanningMap, resetFilters);
        }

        private void AddUpdateFilterInfoTree(TreeNode parent)
        {
            foreach (var item in _altitudeAngelService
                .FilterInfoDisplay
                .Where(i => i.ParentName == parent?.Name))
            {
                var children = parent == null ? trv_MapLayers.Nodes : parent.Nodes;
                var node = children[item.Name] ?? children.Add(item.Name, item.Name);
                node.Tag = item;
                if (node.Checked != item.Visible)
                {
                    node.Checked = item.Visible;
                }
                AddUpdateFilterInfoTree(node);
            }
        }

        private void RefreshControlStates(bool signOut = false, bool processMap = false, bool resetFilters = false)
        {
            SuspendLayout();
            but_Enable.Enabled = !_settings.CheckEnableAltitudeAngel;
            but_Disable.Enabled = _settings.CheckEnableAltitudeAngel;

            if (signOut)
            {
                _altitudeAngelService.DisconnectAsync();
            }

            if (processMap)
            {
                ProcessMapsFromCache(resetFilters);
            }

            trv_MapLayers.BeginUpdate();
            AddUpdateFilterInfoTree(null);
            trv_MapLayers.ExpandAll();
            trv_MapLayers.EndUpdate();

            but_SignIn.Enabled = !_altitudeAngelService.IsSignedIn;
            but_SignOut.Enabled = _altitudeAngelService.IsSignedIn;
            trv_MapLayers.Enabled = _altitudeAngelService.IsSignedIn;
            trk_OpacityAdjust.Enabled = _altitudeAngelService.IsSignedIn;
            chk_FlightReportEnable.Enabled = _altitudeAngelService.IsSignedIn;
            chk_UseExistingFlightPlanId.Enabled = _altitudeAngelService.IsSignedIn && chk_FlightReportEnable.Checked;
            txt_ExistingFlightPlanId.Enabled = _altitudeAngelService.IsSignedIn && chk_UseExistingFlightPlanId.Checked;
            lbl_FlightReportName.Enabled = _altitudeAngelService.IsSignedIn && chk_FlightReportEnable.Checked && !chk_UseExistingFlightPlanId.Checked;
            txt_FlightReportName.Enabled = _altitudeAngelService.IsSignedIn && chk_FlightReportEnable.Checked && !chk_UseExistingFlightPlanId.Checked;
            lbl_FlightReportDescription.Enabled = _altitudeAngelService.IsSignedIn && chk_FlightReportEnable.Checked && !chk_UseExistingFlightPlanId.Checked;
            txt_FlightReportDescription.Enabled = _altitudeAngelService.IsSignedIn && chk_FlightReportEnable.Checked && !chk_UseExistingFlightPlanId.Checked;
            lbl_FlightReportDuration.Enabled = _altitudeAngelService.IsSignedIn && chk_FlightReportEnable.Checked && !chk_UseExistingFlightPlanId.Checked;
            txt_FlightReportDuration.Enabled = _altitudeAngelService.IsSignedIn && chk_FlightReportEnable.Checked && !chk_UseExistingFlightPlanId.Checked;
            chk_FlightReportCommercial.Enabled = _altitudeAngelService.IsSignedIn && chk_FlightReportEnable.Checked && !chk_UseExistingFlightPlanId.Checked;
            chk_FlightReportLocalScope.Enabled = _altitudeAngelService.IsSignedIn && chk_FlightReportEnable.Checked && !chk_UseExistingFlightPlanId.Checked;
            lbl_OverrideClientId.Enabled = _settings.OverrideClientUrlSettings;
            txt_OverrideClientId.Enabled = _settings.OverrideClientUrlSettings;
            lbl_OverrideClientSecret.Enabled = _settings.OverrideClientUrlSettings;
            txt_OverrideClientSecret.Enabled = _settings.OverrideClientUrlSettings;
            lbl_OverrideClientSuffix.Enabled = _settings.OverrideClientUrlSettings;
            txt_OverrideUrlSuffix.Enabled = _settings.OverrideClientUrlSettings;
            chk_AllowSms.Enabled = _altitudeAngelService.IsSignedIn && chk_FlightReportEnable.Checked && !chk_UseExistingFlightPlanId.Checked;
            txt_ContactPhone.Enabled = _altitudeAngelService.IsSignedIn && chk_FlightReportEnable.Checked && !chk_UseExistingFlightPlanId.Checked;
            chk_IcaoAddress.Enabled = _altitudeAngelService.IsSignedIn && chk_FlightReportEnable.Checked && !chk_UseExistingFlightPlanId.Checked;
            txt_IcaoAddress.Enabled = _altitudeAngelService.IsSignedIn && chk_FlightReportEnable.Checked && !chk_UseExistingFlightPlanId.Checked && chk_IcaoAddress.Checked;
            chk_SerialNumber.Enabled = _altitudeAngelService.IsSignedIn && chk_FlightReportEnable.Checked && !chk_UseExistingFlightPlanId.Checked;
            txt_SerialNumber.Enabled = _altitudeAngelService.IsSignedIn && chk_FlightReportEnable.Checked && !chk_UseExistingFlightPlanId.Checked && chk_SerialNumber.Checked;

            SetTabVisibility(tabPageAccount, _settings.CheckEnableAltitudeAngel);
            SetTabVisibility(tabPageMap, _settings.CheckEnableAltitudeAngel && _altitudeAngelService.IsSignedIn);
            SetTabVisibility(tabPageFlight, _settings.CheckEnableAltitudeAngel && _altitudeAngelService.IsSignedIn && chk_FlightReportEnable.Checked);
            ResumeLayout(true);
        }

        private void SetTabVisibility(TabPage tabPage, bool visible)
        {
            var exists = tabPages.TabPages.Contains(tabPage);
            if (visible && !exists)
            {
                tabPages.TabPages.Insert(tabPages.TabPages.IndexOf(tabPageAbout), tabPage);
            }
            if (!visible && exists)
            {
                tabPages.TabPages.Remove(tabPage);
            }
        }
    }
}