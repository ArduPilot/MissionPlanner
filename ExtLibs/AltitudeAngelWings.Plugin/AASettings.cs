using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AltitudeAngelWings.Clients.Api.Model;
using AltitudeAngelWings.Clients.Auth;
using AltitudeAngelWings.Clients.Auth.Model;
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
        private readonly IAuthClient _authClient;

        private static readonly object InstanceLock = new object();
        private static AASettings _instance;

        private AASettings()
            : this(ServiceLocator.GetService<ISettings>(),
                ServiceLocator.GetService<IAltitudeAngelService>(),
                ServiceLocator.GetService<IMissionPlanner>(),
                ServiceLocator.GetService<IAuthClient>())
        {
        }

        public static AASettings Instance
        {
            get
            {
                lock (InstanceLock)
                {
                    if (_instance == null || _instance.IsDisposed)
                    {
                        _instance = new AASettings();
                    }
                }
                return _instance;
            }
        }

        private AASettings(ISettings settings, IAltitudeAngelService altitudeAngelService, IMissionPlanner missionPlanner, IAuthClient authClient)
        {
            _settings = settings;
            _altitudeAngelService = altitudeAngelService;
            _missionPlanner = missionPlanner;
            _authClient = authClient;
            InitializeComponent();
            _altitudeAngelService.IsSignedIn.ObserveOn(MainV2.instance).SubscribeWithAsync(OnSignInChange);
            _missionPlanner.FlightDataMap.MapChanged.ObserveOn(MainV2.instance).Subscribe(OnMapChanged);
            _missionPlanner.FlightPlanningMap.MapChanged.ObserveOn(MainV2.instance).Subscribe(OnMapChanged);

            ThemeManager.ApplyThemeTo(this);
            Icon = Resources.AAIconBlack;

            // load settings
            chk_FlightPlansEnable.Checked = _settings.UseFlightPlans;
            chk_FlightsEnable.Checked = _settings.UseFlights;
            lst_FlightTelemetry.SelectedIndex = (int)_settings.SendFlightTelemetry;
            chk_UseExistingFlightPlanId.Checked = _settings.UseExistingFlightPlanId;
            txt_ExistingFlightPlanId.Text = _settings.ExistingFlightPlanId == Guid.Empty ? "" : _settings.ExistingFlightPlanId.ToString();
            txt_FlightPlanName.Text = _settings.FlightPlanName;
            txt_FlightPlanDescription.Text = _settings.FlightPlanDescription;
            txt_FlightPlanDuration.Text = ((int)_settings.FlightPlanTimeSpan.TotalMinutes).ToString();
            but_SignIn.Enabled = !_altitudeAngelService.IsSignedIn.Value;
            but_SignOut.Enabled = _altitudeAngelService.IsSignedIn.Value;
            chk_OverrideClientSettings.Checked = _settings.OverrideClientUrlSettings;
            txt_OverrideClientId.Text = _settings.OverrideClientId;
            txt_OverrideClientSecret.Text = _settings.OverrideClientSecret;
            txt_OverrideClientSuffix.Text = _settings.OverrideUrlDomainSuffix;
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
            trk_AltitudeFilter.Value = _settings.AltitudeFilter;
            ((SHDocVw.WebBrowser)web_About.ActiveXInstance).NewWindow3 +=
                (ref object o, ref bool b, uint u, string s, string url) =>
                {
                    Process.Start(url);
                };

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
            if (trv_MapLayers.Visible && trv_MapLayers.Focused)
            {
                ProcessMapsFromCache();
            }
        }

        private void txt_FlightPlanName_TextChanged(object sender, EventArgs e)
        {
            _settings.FlightPlanName = txt_FlightPlanName.Text;
            RefreshControlStates();
        }

        private void txt_FlightPlanDuration_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txt_FlightPlanDuration.Text, out var minutes))
            {
                _settings.FlightPlanTimeSpan = TimeSpan.FromMinutes(minutes);
            }
            RefreshControlStates();
        }

        private void chk_FlightPlansEnable_CheckedChanged(object sender, EventArgs e)
        {
            _settings.UseFlightPlans = chk_FlightPlansEnable.Checked;
            RefreshControlStates();
        }
        
        private void chk_FlightsEnable_CheckedChanged(object sender, EventArgs e)
        {
            _settings.UseFlights = chk_FlightsEnable.Checked;
            RefreshControlStates();
        }

        private void lst_FlightTelemetry_SelectedIndexChanged(object sender, EventArgs e)
        {
            _settings.SendFlightTelemetry = (FlightTelemetry)lst_FlightTelemetry.SelectedIndex;
            RefreshControlStates();
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

        private void txt_FlightPlanDescription_TextChanged(object sender, EventArgs e)
        {
            _settings.FlightPlanDescription = txt_FlightPlanDescription.Text;
            RefreshControlStates();
        }

        private void chk_OverrideClientSettings_CheckedChanged(object sender, EventArgs e)
        {
            _settings.OverrideClientUrlSettings = chk_OverrideClientSettings.Checked;
            RefreshControlStates();
        }

        private void txt_OverrideClientId_TextChanged(object sender, EventArgs e)
        {
            _settings.OverrideClientId = txt_OverrideClientId.Text;
            RefreshControlStates();
        }

        private void txt_OverrideClientSecret_TextChanged(object sender, EventArgs e)
        {
            _settings.OverrideClientSecret = txt_OverrideClientSecret.Text;
            RefreshControlStates();
        }

        private void txt_OverrideUrlSuffix_TextChanged(object sender, EventArgs e)
        {
            _settings.OverrideUrlDomainSuffix = txt_OverrideClientSuffix.Text;
            RefreshControlStates();
        }

        private void trk_OpacityAdjust_ValueChanged(object sender, EventArgs e)
        {
            _settings.MapOpacityAdjust = (float)trk_OpacityAdjust.Value / 100;
            RefreshControlStates(processMap: true);
        }


        private void trk_AltitudeFilter_ValueChanged(object sender, EventArgs e)
        {
            _settings.AltitudeFilter = trk_AltitudeFilter.Value;
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

        private async Task OnSignInChange(bool signedIn, CancellationToken cancellationToken)
        {
            lbl_UserDetails.Text = string.Empty;
            if (_settings.TokenResponse.IsValidForAuth())
            {
                var user = await _authClient.GetUserProfile(_settings.TokenResponse.AccessToken, cancellationToken);
                if (user != null)
                {
                    lbl_UserDetails.Text = signedIn
                        ? $"{user.FirstName} {user.LastName}\r\n{user.EmailAddress}\r\n{user.UserId}\r\n{string.Join("\r\n", _settings.TokenResponse.AccessTokenScopes())}"
                        : string.Empty;
                }
            }

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

        private void AddUpdateFilterInfoTree(TreeNode parent = null)
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
            AddUpdateFilterInfoTree();
            trv_MapLayers.ExpandAll();
            trv_MapLayers.EndUpdate();

            but_SignIn.Enabled = _settings.CheckEnableAltitudeAngel && !_altitudeAngelService.IsSignedIn.Value;
            but_SignOut.Enabled = _altitudeAngelService.IsSignedIn.Value;
            trv_MapLayers.Enabled = _altitudeAngelService.IsSignedIn.Value;
            trk_OpacityAdjust.Enabled = _altitudeAngelService.IsSignedIn.Value;
            chk_FlightPlansEnable.Enabled = _altitudeAngelService.IsSignedIn.Value;
            chk_FlightsEnable.Enabled = _altitudeAngelService.IsSignedIn.Value && chk_FlightPlansEnable.Checked;
            lst_FlightTelemetry.Enabled = _altitudeAngelService.IsSignedIn.Value && chk_FlightPlansEnable.Checked && chk_FlightsEnable.Checked;
            chk_UseExistingFlightPlanId.Enabled = _altitudeAngelService.IsSignedIn.Value && chk_FlightPlansEnable.Checked;
            txt_ExistingFlightPlanId.Enabled = _altitudeAngelService.IsSignedIn.Value && chk_UseExistingFlightPlanId.Checked;
            lbl_FlightPlanName.Enabled = _altitudeAngelService.IsSignedIn.Value && chk_FlightPlansEnable.Checked && !chk_UseExistingFlightPlanId.Checked;
            txt_FlightPlanName.Enabled = _altitudeAngelService.IsSignedIn.Value && chk_FlightPlansEnable.Checked && !chk_UseExistingFlightPlanId.Checked;
            lbl_FlightPlanDescription.Enabled = _altitudeAngelService.IsSignedIn.Value && chk_FlightPlansEnable.Checked && !chk_UseExistingFlightPlanId.Checked;
            txt_FlightPlanDescription.Enabled = _altitudeAngelService.IsSignedIn.Value && chk_FlightPlansEnable.Checked && !chk_UseExistingFlightPlanId.Checked;
            lbl_FlightPlanDuration.Enabled = _altitudeAngelService.IsSignedIn.Value && chk_FlightPlansEnable.Checked && !chk_UseExistingFlightPlanId.Checked;
            txt_FlightPlanDuration.Enabled = _altitudeAngelService.IsSignedIn.Value && chk_FlightPlansEnable.Checked && !chk_UseExistingFlightPlanId.Checked;
            chk_OverrideClientSettings.Enabled = _settings.CheckEnableAltitudeAngel && !_altitudeAngelService.IsSignedIn.Value;
            lbl_OverrideClientId.Enabled = _settings.OverrideClientUrlSettings && !_altitudeAngelService.IsSignedIn.Value;
            txt_OverrideClientId.Enabled = _settings.OverrideClientUrlSettings && !_altitudeAngelService.IsSignedIn.Value;
            lbl_OverrideClientSecret.Enabled = _settings.OverrideClientUrlSettings && !_altitudeAngelService.IsSignedIn.Value;
            txt_OverrideClientSecret.Enabled = _settings.OverrideClientUrlSettings && !_altitudeAngelService.IsSignedIn.Value;
            lbl_OverrideClientSuffix.Enabled = _settings.OverrideClientUrlSettings && !_altitudeAngelService.IsSignedIn.Value;
            txt_OverrideClientSuffix.Enabled = _settings.OverrideClientUrlSettings && !_altitudeAngelService.IsSignedIn.Value;
            chk_AllowSms.Enabled = _altitudeAngelService.IsSignedIn.Value && chk_FlightPlansEnable.Checked && !chk_UseExistingFlightPlanId.Checked;
            txt_ContactPhone.Enabled = _altitudeAngelService.IsSignedIn.Value && chk_FlightPlansEnable.Checked && !chk_UseExistingFlightPlanId.Checked;
            chk_IcaoAddress.Enabled = _altitudeAngelService.IsSignedIn.Value && chk_FlightPlansEnable.Checked && !chk_UseExistingFlightPlanId.Checked;
            txt_IcaoAddress.Enabled = _altitudeAngelService.IsSignedIn.Value && chk_FlightPlansEnable.Checked && !chk_UseExistingFlightPlanId.Checked && chk_IcaoAddress.Checked;
            chk_SerialNumber.Enabled = _altitudeAngelService.IsSignedIn.Value && chk_FlightPlansEnable.Checked && !chk_UseExistingFlightPlanId.Checked;
            txt_SerialNumber.Enabled = _altitudeAngelService.IsSignedIn.Value && chk_FlightPlansEnable.Checked && !chk_UseExistingFlightPlanId.Checked && chk_SerialNumber.Checked;
            trk_AltitudeFilter.Enabled = _altitudeAngelService.IsSignedIn.Value;
            lbl_AltitudeDisplay.Text = $"{_settings.AltitudeFilter}m ({_settings.AltitudeFilter*3.28084:F0}ft) AGL";

            SetTabVisibility(tabPageMap, _settings.CheckEnableAltitudeAngel && _altitudeAngelService.IsSignedIn.Value);
            SetTabVisibility(tabPageFlight, _settings.CheckEnableAltitudeAngel && _altitudeAngelService.IsSignedIn.Value && chk_FlightPlansEnable.Checked);
            ResumeLayout(true);
        }

        private void SetTabVisibility(TabPage tabPage, bool visible)
        {
            var exists = tabPages.TabPages.Contains(tabPage);
            switch (visible)
            {
                case true when !exists:
                    tabPages.TabPages.Insert(tabPages.TabPages.IndexOf(tabPageAbout), tabPage);
                    break;
                case false when exists:
                    tabPages.TabPages.Remove(tabPage);
                    break;
            }
        }

        private void web_About_NewWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}