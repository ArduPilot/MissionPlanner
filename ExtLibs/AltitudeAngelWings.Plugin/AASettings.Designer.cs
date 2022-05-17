using System.Windows.Forms;
using AltitudeAngelWings.Plugin.Properties;
using MissionPlanner.Controls;

namespace AltitudeAngelWings.Plugin
{
    internal partial class AASettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabPages = new System.Windows.Forms.TabControl();
            this.tabPageAccount = new System.Windows.Forms.TabPage();
            this.lbl_UserDetails = new System.Windows.Forms.Label();
            this.lbl_OverrideClientSuffix = new System.Windows.Forms.Label();
            this.txt_OverrideUrlSuffix = new System.Windows.Forms.TextBox();
            this.lbl_OverrideClientSecret = new System.Windows.Forms.Label();
            this.txt_OverrideClientSecret = new System.Windows.Forms.TextBox();
            this.lbl_OverrideClientId = new System.Windows.Forms.Label();
            this.txt_OverrideClientId = new System.Windows.Forms.TextBox();
            this.chk_OverrideClientSettings = new System.Windows.Forms.CheckBox();
            this.lbl_FlightReportWhat = new System.Windows.Forms.LinkLabel();
            this.chk_FlightReportEnable = new System.Windows.Forms.CheckBox();
            this.but_SignOut = new MissionPlanner.Controls.MyButton();
            this.but_SignIn = new MissionPlanner.Controls.MyButton();
            this.tabPageMap = new System.Windows.Forms.TabPage();
            this.chk_EnablePlanMap = new System.Windows.Forms.CheckBox();
            this.chk_EnableDataMap = new System.Windows.Forms.CheckBox();
            this.btn_DefaultLayers = new MissionPlanner.Controls.MyButton();
            this.trv_MapLayers = new System.Windows.Forms.TreeView();
            this.lbl_OpacityAdjust = new System.Windows.Forms.Label();
            this.trk_OpacityAdjust = new System.Windows.Forms.TrackBar();
            this.tabPageFlight = new System.Windows.Forms.TabPage();
            this.lbl_FlightReportDescription = new System.Windows.Forms.Label();
            this.txt_FlightReportDescription = new System.Windows.Forms.TextBox();
            this.chk_FlightReportLocalScope = new System.Windows.Forms.CheckBox();
            this.txt_SerialNumber = new System.Windows.Forms.TextBox();
            this.txt_IcaoAddress = new System.Windows.Forms.TextBox();
            this.txt_ContactPhone = new System.Windows.Forms.TextBox();
            this.chk_SerialNumber = new System.Windows.Forms.CheckBox();
            this.txt_ExistingFlightPlanId = new System.Windows.Forms.TextBox();
            this.chk_AllowSms = new System.Windows.Forms.CheckBox();
            this.chk_IcaoAddress = new System.Windows.Forms.CheckBox();
            this.chk_UseExistingFlightPlanId = new System.Windows.Forms.CheckBox();
            this.chk_FlightReportCommercial = new System.Windows.Forms.CheckBox();
            this.lbl_FlightReportDuration = new System.Windows.Forms.Label();
            this.txt_FlightReportDuration = new System.Windows.Forms.TextBox();
            this.lbl_ContactPhoneNumber = new System.Windows.Forms.Label();
            this.lbl_FlightReportName = new System.Windows.Forms.Label();
            this.txt_FlightReportName = new System.Windows.Forms.TextBox();
            this.tabPageAbout = new System.Windows.Forms.TabPage();
            this.but_Disable = new MissionPlanner.Controls.MyButton();
            this.but_Enable = new MissionPlanner.Controls.MyButton();
            this.pic_AboutLogo = new System.Windows.Forms.PictureBox();
            this.web_About = new System.Windows.Forms.WebBrowser();
            this.tabPages.SuspendLayout();
            this.tabPageAccount.SuspendLayout();
            this.tabPageMap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trk_OpacityAdjust)).BeginInit();
            this.tabPageFlight.SuspendLayout();
            this.tabPageAbout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_AboutLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // tabPages
            // 
            this.tabPages.Controls.Add(this.tabPageAccount);
            this.tabPages.Controls.Add(this.tabPageMap);
            this.tabPages.Controls.Add(this.tabPageFlight);
            this.tabPages.Controls.Add(this.tabPageAbout);
            this.tabPages.Location = new System.Drawing.Point(12, 12);
            this.tabPages.Name = "tabPages";
            this.tabPages.SelectedIndex = 0;
            this.tabPages.Size = new System.Drawing.Size(453, 284);
            this.tabPages.TabIndex = 15;
            // 
            // tabPageAccount
            // 
            this.tabPageAccount.Controls.Add(this.lbl_UserDetails);
            this.tabPageAccount.Controls.Add(this.lbl_OverrideClientSuffix);
            this.tabPageAccount.Controls.Add(this.txt_OverrideUrlSuffix);
            this.tabPageAccount.Controls.Add(this.lbl_OverrideClientSecret);
            this.tabPageAccount.Controls.Add(this.txt_OverrideClientSecret);
            this.tabPageAccount.Controls.Add(this.lbl_OverrideClientId);
            this.tabPageAccount.Controls.Add(this.txt_OverrideClientId);
            this.tabPageAccount.Controls.Add(this.chk_OverrideClientSettings);
            this.tabPageAccount.Controls.Add(this.lbl_FlightReportWhat);
            this.tabPageAccount.Controls.Add(this.chk_FlightReportEnable);
            this.tabPageAccount.Controls.Add(this.but_SignOut);
            this.tabPageAccount.Controls.Add(this.but_SignIn);
            this.tabPageAccount.Location = new System.Drawing.Point(4, 22);
            this.tabPageAccount.Name = "tabPageAccount";
            this.tabPageAccount.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAccount.Size = new System.Drawing.Size(445, 258);
            this.tabPageAccount.TabIndex = 2;
            this.tabPageAccount.Text = Resources.SettingsAccountTabText;
            this.tabPageAccount.UseVisualStyleBackColor = true;
            // 
            // lbl_UserDetails
            // 
            this.lbl_UserDetails.Location = new System.Drawing.Point(299, 6);
            this.lbl_UserDetails.Name = "lbl_UserDetails";
            this.lbl_UserDetails.Size = new System.Drawing.Size(140, 142);
            this.lbl_UserDetails.TabIndex = 40;
            // 
            // lbl_OverrideClientSuffix
            // 
            this.lbl_OverrideClientSuffix.AutoSize = true;
            this.lbl_OverrideClientSuffix.Location = new System.Drawing.Point(98, 156);
            this.lbl_OverrideClientSuffix.Name = "lbl_OverrideClientSuffix";
            this.lbl_OverrideClientSuffix.Size = new System.Drawing.Size(97, 13);
            this.lbl_OverrideClientSuffix.TabIndex = 38;
            this.lbl_OverrideClientSuffix.Text = Resources.SettingsURLDomainSuffixText;
            // 
            // txt_OverrideUrlSuffix
            // 
            this.txt_OverrideUrlSuffix.Location = new System.Drawing.Point(103, 172);
            this.txt_OverrideUrlSuffix.Name = "txt_OverrideUrlSuffix";
            this.txt_OverrideUrlSuffix.Size = new System.Drawing.Size(172, 20);
            this.txt_OverrideUrlSuffix.TabIndex = 37;
            this.txt_OverrideUrlSuffix.TextChanged += new System.EventHandler(this.txt_OverrideUrlSuffix_TextChanged);
            // 
            // lbl_OverrideClientSecret
            // 
            this.lbl_OverrideClientSecret.AutoSize = true;
            this.lbl_OverrideClientSecret.Location = new System.Drawing.Point(98, 117);
            this.lbl_OverrideClientSecret.Name = "lbl_OverrideClientSecret";
            this.lbl_OverrideClientSecret.Size = new System.Drawing.Size(67, 13);
            this.lbl_OverrideClientSecret.TabIndex = 36;
            this.lbl_OverrideClientSecret.Text = Resources.SettingsClientSecretText;
            // 
            // txt_OverrideClientSecret
            // 
            this.txt_OverrideClientSecret.Location = new System.Drawing.Point(103, 133);
            this.txt_OverrideClientSecret.Name = "txt_OverrideClientSecret";
            this.txt_OverrideClientSecret.Size = new System.Drawing.Size(172, 20);
            this.txt_OverrideClientSecret.TabIndex = 35;
            this.txt_OverrideClientSecret.UseSystemPasswordChar = true;
            this.txt_OverrideClientSecret.TextChanged += new System.EventHandler(this.txt_OverrideClientSecret_TextChanged);
            // 
            // lbl_OverrideClientId
            // 
            this.lbl_OverrideClientId.AutoSize = true;
            this.lbl_OverrideClientId.Location = new System.Drawing.Point(98, 78);
            this.lbl_OverrideClientId.Name = "lbl_OverrideClientId";
            this.lbl_OverrideClientId.Size = new System.Drawing.Size(47, 13);
            this.lbl_OverrideClientId.TabIndex = 34;
            this.lbl_OverrideClientId.Text = Resources.SettingsClientIDText;
            // 
            // txt_OverrideClientId
            // 
            this.txt_OverrideClientId.Location = new System.Drawing.Point(103, 94);
            this.txt_OverrideClientId.Name = "txt_OverrideClientId";
            this.txt_OverrideClientId.Size = new System.Drawing.Size(172, 20);
            this.txt_OverrideClientId.TabIndex = 33;
            this.txt_OverrideClientId.TextChanged += new System.EventHandler(this.txt_OverrideClientId_TextChanged);
            // 
            // chk_OverrideClientSettings
            // 
            this.chk_OverrideClientSettings.AutoSize = true;
            this.chk_OverrideClientSettings.Location = new System.Drawing.Point(101, 58);
            this.chk_OverrideClientSettings.Name = "chk_OverrideClientSettings";
            this.chk_OverrideClientSettings.Size = new System.Drawing.Size(136, 17);
            this.chk_OverrideClientSettings.TabIndex = 32;
            this.chk_OverrideClientSettings.Text = Resources.SettingsOverrideClientSettingsText;
            this.chk_OverrideClientSettings.UseVisualStyleBackColor = true;
            this.chk_OverrideClientSettings.CheckedChanged += new System.EventHandler(this.chk_OverrideClientSettings_CheckedChanged);
            // 
            // lbl_FlightReportWhat
            // 
            this.lbl_FlightReportWhat.AutoSize = true;
            this.lbl_FlightReportWhat.Location = new System.Drawing.Point(101, 30);
            this.lbl_FlightReportWhat.Name = "lbl_FlightReportWhat";
            this.lbl_FlightReportWhat.Size = new System.Drawing.Size(68, 13);
            this.lbl_FlightReportWhat.TabIndex = 31;
            this.lbl_FlightReportWhat.TabStop = true;
            this.lbl_FlightReportWhat.Text = Resources.SettingsWhatIsThisText;
            this.lbl_FlightReportWhat.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbl_FlightReportWhat_LinkClicked);
            // 
            // chk_FlightReportEnable
            // 
            this.chk_FlightReportEnable.AutoSize = true;
            this.chk_FlightReportEnable.Location = new System.Drawing.Point(101, 12);
            this.chk_FlightReportEnable.Name = "chk_FlightReportEnable";
            this.chk_FlightReportEnable.Size = new System.Drawing.Size(136, 17);
            this.chk_FlightReportEnable.TabIndex = 30;
            this.chk_FlightReportEnable.Text = Resources.SettingsEnableFlightReportingText;
            this.chk_FlightReportEnable.UseVisualStyleBackColor = true;
            this.chk_FlightReportEnable.CheckedChanged += new System.EventHandler(this.chk_FlightReportEnable_CheckedChanged);
            // 
            // but_SignOut
            // 
            this.but_SignOut.Location = new System.Drawing.Point(6, 37);
            this.but_SignOut.Name = "but_SignOut";
            this.but_SignOut.Size = new System.Drawing.Size(75, 23);
            this.but_SignOut.TabIndex = 3;
            this.but_SignOut.Text = Resources.SettingsSignOutText;
            this.but_SignOut.UseVisualStyleBackColor = true;
            this.but_SignOut.Click += new System.EventHandler(this.but_SignOut_Click);
            // 
            // but_SignIn
            // 
            this.but_SignIn.Location = new System.Drawing.Point(6, 8);
            this.but_SignIn.Name = "but_SignIn";
            this.but_SignIn.Size = new System.Drawing.Size(75, 23);
            this.but_SignIn.TabIndex = 2;
            this.but_SignIn.Text = Resources.SettingsSignInText;
            this.but_SignIn.UseVisualStyleBackColor = true;
            this.but_SignIn.Click += new System.EventHandler(this.but_SignIn_Click);
            // 
            // tabPageMap
            // 
            this.tabPageMap.Controls.Add(this.chk_EnablePlanMap);
            this.tabPageMap.Controls.Add(this.chk_EnableDataMap);
            this.tabPageMap.Controls.Add(this.btn_DefaultLayers);
            this.tabPageMap.Controls.Add(this.trv_MapLayers);
            this.tabPageMap.Controls.Add(this.lbl_OpacityAdjust);
            this.tabPageMap.Controls.Add(this.trk_OpacityAdjust);
            this.tabPageMap.Location = new System.Drawing.Point(4, 22);
            this.tabPageMap.Name = "tabPageMap";
            this.tabPageMap.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMap.Size = new System.Drawing.Size(445, 258);
            this.tabPageMap.TabIndex = 0;
            this.tabPageMap.Text = Resources.SettingsMapLayersText;
            this.tabPageMap.UseVisualStyleBackColor = true;
            // 
            // chk_EnablePlanMap
            // 
            this.chk_EnablePlanMap.AutoSize = true;
            this.chk_EnablePlanMap.Location = new System.Drawing.Point(343, 134);
            this.chk_EnablePlanMap.Name = "chk_EnablePlanMap";
            this.chk_EnablePlanMap.Size = new System.Drawing.Size(71, 17);
            this.chk_EnablePlanMap.TabIndex = 46;
            this.chk_EnablePlanMap.Text = Resources.SettingsPlanMapText;
            this.chk_EnablePlanMap.UseVisualStyleBackColor = true;
            this.chk_EnablePlanMap.CheckedChanged += new System.EventHandler(this.chk_EnablePlanMap_CheckedChanged);
            // 
            // chk_EnableDataMap
            // 
            this.chk_EnableDataMap.AutoSize = true;
            this.chk_EnableDataMap.Location = new System.Drawing.Point(343, 111);
            this.chk_EnableDataMap.Name = "chk_EnableDataMap";
            this.chk_EnableDataMap.Size = new System.Drawing.Size(73, 17);
            this.chk_EnableDataMap.TabIndex = 45;
            this.chk_EnableDataMap.Text = Resources.SettingsDataMapText;
            this.chk_EnableDataMap.UseVisualStyleBackColor = true;
            this.chk_EnableDataMap.CheckedChanged += new System.EventHandler(this.chk_EnableDataMap_CheckedChanged);
            // 
            // btn_DefaultLayers
            // 
            this.btn_DefaultLayers.Location = new System.Drawing.Point(348, 6);
            this.btn_DefaultLayers.Name = "btn_DefaultLayers";
            this.btn_DefaultLayers.Size = new System.Drawing.Size(75, 31);
            this.btn_DefaultLayers.TabIndex = 44;
            this.btn_DefaultLayers.Text = Resources.SettingsDefaultLayersText;
            this.btn_DefaultLayers.UseVisualStyleBackColor = true;
            this.btn_DefaultLayers.Click += new System.EventHandler(this.btn_DefaultLayers_Click);
            // 
            // trv_MapLayers
            // 
            this.trv_MapLayers.CheckBoxes = true;
            this.trv_MapLayers.Location = new System.Drawing.Point(6, 6);
            this.trv_MapLayers.Name = "trv_MapLayers";
            this.trv_MapLayers.Size = new System.Drawing.Size(321, 246);
            this.trv_MapLayers.TabIndex = 18;
            this.trv_MapLayers.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.trv_MapLayers_AfterCheck);
            // 
            // lbl_OpacityAdjust
            // 
            this.lbl_OpacityAdjust.AutoSize = true;
            this.lbl_OpacityAdjust.Location = new System.Drawing.Point(349, 82);
            this.lbl_OpacityAdjust.Name = "lbl_OpacityAdjust";
            this.lbl_OpacityAdjust.Size = new System.Drawing.Size(75, 13);
            this.lbl_OpacityAdjust.TabIndex = 17;
            this.lbl_OpacityAdjust.Text = Resources.SettingsOpacityAdjustText;
            // 
            // trk_OpacityAdjust
            // 
            this.trk_OpacityAdjust.LargeChange = 80;
            this.trk_OpacityAdjust.Location = new System.Drawing.Point(343, 49);
            this.trk_OpacityAdjust.Maximum = 240;
            this.trk_OpacityAdjust.Minimum = 20;
            this.trk_OpacityAdjust.Name = "trk_OpacityAdjust";
            this.trk_OpacityAdjust.Size = new System.Drawing.Size(87, 45);
            this.trk_OpacityAdjust.SmallChange = 20;
            this.trk_OpacityAdjust.TabIndex = 15;
            this.trk_OpacityAdjust.Text = Resources.SettingsGroundDataText;
            this.trk_OpacityAdjust.TickFrequency = 20;
            this.trk_OpacityAdjust.Value = 100;
            this.trk_OpacityAdjust.ValueChanged += new System.EventHandler(this.trk_OpacityAdjust_ValueChanged);
            // 
            // tabPageFlight
            // 
            this.tabPageFlight.Controls.Add(this.lbl_FlightReportDescription);
            this.tabPageFlight.Controls.Add(this.txt_FlightReportDescription);
            this.tabPageFlight.Controls.Add(this.chk_FlightReportLocalScope);
            this.tabPageFlight.Controls.Add(this.txt_SerialNumber);
            this.tabPageFlight.Controls.Add(this.txt_IcaoAddress);
            this.tabPageFlight.Controls.Add(this.txt_ContactPhone);
            this.tabPageFlight.Controls.Add(this.chk_SerialNumber);
            this.tabPageFlight.Controls.Add(this.txt_ExistingFlightPlanId);
            this.tabPageFlight.Controls.Add(this.chk_AllowSms);
            this.tabPageFlight.Controls.Add(this.chk_IcaoAddress);
            this.tabPageFlight.Controls.Add(this.chk_UseExistingFlightPlanId);
            this.tabPageFlight.Controls.Add(this.chk_FlightReportCommercial);
            this.tabPageFlight.Controls.Add(this.lbl_FlightReportDuration);
            this.tabPageFlight.Controls.Add(this.txt_FlightReportDuration);
            this.tabPageFlight.Controls.Add(this.lbl_ContactPhoneNumber);
            this.tabPageFlight.Controls.Add(this.lbl_FlightReportName);
            this.tabPageFlight.Controls.Add(this.txt_FlightReportName);
            this.tabPageFlight.Location = new System.Drawing.Point(4, 22);
            this.tabPageFlight.Name = "tabPageFlight";
            this.tabPageFlight.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageFlight.Size = new System.Drawing.Size(445, 258);
            this.tabPageFlight.TabIndex = 1;
            this.tabPageFlight.Text = Resources.SettingsFlightReportingTabText;
            this.tabPageFlight.UseVisualStyleBackColor = true;
            // 
            // lbl_FlightReportDescription
            // 
            this.lbl_FlightReportDescription.AutoSize = true;
            this.lbl_FlightReportDescription.Location = new System.Drawing.Point(4, 91);
            this.lbl_FlightReportDescription.Name = "lbl_FlightReportDescription";
            this.lbl_FlightReportDescription.Size = new System.Drawing.Size(123, 13);
            this.lbl_FlightReportDescription.TabIndex = 34;
            this.lbl_FlightReportDescription.Text = Resources.SettingsFlightReportDescriptionText;
            // 
            // txt_FlightReportDescription
            // 
            this.txt_FlightReportDescription.Location = new System.Drawing.Point(9, 107);
            this.txt_FlightReportDescription.Name = "txt_FlightReportDescription";
            this.txt_FlightReportDescription.Size = new System.Drawing.Size(172, 20);
            this.txt_FlightReportDescription.TabIndex = 33;
            this.txt_FlightReportDescription.TextChanged += new System.EventHandler(this.txt_FlightReportDescription_TextChanged);
            // 
            // chk_FlightReportLocalScope
            // 
            this.chk_FlightReportLocalScope.AutoSize = true;
            this.chk_FlightReportLocalScope.Location = new System.Drawing.Point(7, 193);
            this.chk_FlightReportLocalScope.Name = "chk_FlightReportLocalScope";
            this.chk_FlightReportLocalScope.Size = new System.Drawing.Size(146, 17);
            this.chk_FlightReportLocalScope.TabIndex = 32;
            this.chk_FlightReportLocalScope.Text = Resources.SettingsUseLocalConflictScopeText;
            this.chk_FlightReportLocalScope.UseVisualStyleBackColor = true;
            this.chk_FlightReportLocalScope.CheckedChanged += new System.EventHandler(this.chk_FlightReportLocalScope_CheckedChanged);
            // 
            // txt_SerialNumber
            // 
            this.txt_SerialNumber.Enabled = false;
            this.txt_SerialNumber.Location = new System.Drawing.Point(229, 159);
            this.txt_SerialNumber.Name = "txt_SerialNumber";
            this.txt_SerialNumber.Size = new System.Drawing.Size(172, 20);
            this.txt_SerialNumber.TabIndex = 31;
            this.txt_SerialNumber.TextChanged += new System.EventHandler(this.txt_SerialNumber_TextChanged);
            // 
            // txt_IcaoAddress
            // 
            this.txt_IcaoAddress.Enabled = false;
            this.txt_IcaoAddress.Location = new System.Drawing.Point(229, 110);
            this.txt_IcaoAddress.Name = "txt_IcaoAddress";
            this.txt_IcaoAddress.Size = new System.Drawing.Size(172, 20);
            this.txt_IcaoAddress.TabIndex = 31;
            this.txt_IcaoAddress.TextChanged += new System.EventHandler(this.txt_IcaoAddress_TextChanged);
            // 
            // txt_ContactPhone
            // 
            this.txt_ContactPhone.Enabled = false;
            this.txt_ContactPhone.Location = new System.Drawing.Point(229, 29);
            this.txt_ContactPhone.Name = "txt_ContactPhone";
            this.txt_ContactPhone.Size = new System.Drawing.Size(172, 20);
            this.txt_ContactPhone.TabIndex = 31;
            this.txt_ContactPhone.TextChanged += new System.EventHandler(this.txt_ContactPhone_TextChanged);
            // 
            // chk_SerialNumber
            // 
            this.chk_SerialNumber.AutoSize = true;
            this.chk_SerialNumber.Enabled = false;
            this.chk_SerialNumber.Location = new System.Drawing.Point(229, 136);
            this.chk_SerialNumber.Name = "chk_SerialNumber";
            this.chk_SerialNumber.Size = new System.Drawing.Size(92, 17);
            this.chk_SerialNumber.TabIndex = 30;
            this.chk_SerialNumber.Text = Resources.SettingsSerialNumberText;
            this.chk_SerialNumber.UseVisualStyleBackColor = true;
            this.chk_SerialNumber.CheckedChanged += new System.EventHandler(this.chk_SerialNumber_CheckedChanged);
            // 
            // txt_ExistingFlightPlanId
            // 
            this.txt_ExistingFlightPlanId.Enabled = false;
            this.txt_ExistingFlightPlanId.Location = new System.Drawing.Point(6, 29);
            this.txt_ExistingFlightPlanId.Name = "txt_ExistingFlightPlanId";
            this.txt_ExistingFlightPlanId.Size = new System.Drawing.Size(172, 20);
            this.txt_ExistingFlightPlanId.TabIndex = 31;
            this.txt_ExistingFlightPlanId.TextChanged += new System.EventHandler(this.txt_ExistingFlightPlanId_TextChanged);
            // 
            // chk_AllowSms
            // 
            this.chk_AllowSms.AutoSize = true;
            this.chk_AllowSms.Enabled = false;
            this.chk_AllowSms.Location = new System.Drawing.Point(229, 55);
            this.chk_AllowSms.Name = "chk_AllowSms";
            this.chk_AllowSms.Size = new System.Drawing.Size(117, 17);
            this.chk_AllowSms.TabIndex = 30;
            this.chk_AllowSms.Text = Resources.SettingsAllowSMSContactText;
            this.chk_AllowSms.UseVisualStyleBackColor = true;
            this.chk_AllowSms.CheckedChanged += new System.EventHandler(this.chk_AllowSms_CheckedChanged);
            // 
            // chk_IcaoAddress
            // 
            this.chk_IcaoAddress.AutoSize = true;
            this.chk_IcaoAddress.Enabled = false;
            this.chk_IcaoAddress.Location = new System.Drawing.Point(229, 87);
            this.chk_IcaoAddress.Name = "chk_IcaoAddress";
            this.chk_IcaoAddress.Size = new System.Drawing.Size(92, 17);
            this.chk_IcaoAddress.TabIndex = 30;
            this.chk_IcaoAddress.Text = Resources.SettingsICAOAddressText;
            this.chk_IcaoAddress.UseVisualStyleBackColor = true;
            this.chk_IcaoAddress.CheckedChanged += new System.EventHandler(this.chk_IcaoAddress_CheckedChanged);
            // 
            // chk_UseExistingFlightPlanId
            // 
            this.chk_UseExistingFlightPlanId.AutoSize = true;
            this.chk_UseExistingFlightPlanId.Enabled = false;
            this.chk_UseExistingFlightPlanId.Location = new System.Drawing.Point(6, 6);
            this.chk_UseExistingFlightPlanId.Name = "chk_UseExistingFlightPlanId";
            this.chk_UseExistingFlightPlanId.Size = new System.Drawing.Size(150, 17);
            this.chk_UseExistingFlightPlanId.TabIndex = 30;
            this.chk_UseExistingFlightPlanId.Text = Resources.SettingsUseExistingFlightPlanIDText;
            this.chk_UseExistingFlightPlanId.UseVisualStyleBackColor = true;
            this.chk_UseExistingFlightPlanId.CheckedChanged += new System.EventHandler(this.chk_UseExistingFlightPlanId_CheckedChanged);
            // 
            // chk_FlightReportCommercial
            // 
            this.chk_FlightReportCommercial.AutoSize = true;
            this.chk_FlightReportCommercial.Location = new System.Drawing.Point(7, 133);
            this.chk_FlightReportCommercial.Name = "chk_FlightReportCommercial";
            this.chk_FlightReportCommercial.Size = new System.Drawing.Size(108, 17);
            this.chk_FlightReportCommercial.TabIndex = 24;
            this.chk_FlightReportCommercial.Text = Resources.SettingsCommercialFlightText;
            this.chk_FlightReportCommercial.UseVisualStyleBackColor = true;
            this.chk_FlightReportCommercial.CheckedChanged += new System.EventHandler(this.chk_FlightReportCommercial_CheckedChanged);
            // 
            // lbl_FlightReportDuration
            // 
            this.lbl_FlightReportDuration.AutoSize = true;
            this.lbl_FlightReportDuration.Location = new System.Drawing.Point(3, 151);
            this.lbl_FlightReportDuration.Name = "lbl_FlightReportDuration";
            this.lbl_FlightReportDuration.Size = new System.Drawing.Size(140, 13);
            this.lbl_FlightReportDuration.TabIndex = 28;
            this.lbl_FlightReportDuration.Text = Resources.SettingsFlightReportDurationText;
            // 
            // txt_FlightReportDuration
            // 
            this.txt_FlightReportDuration.Location = new System.Drawing.Point(8, 167);
            this.txt_FlightReportDuration.Name = "txt_FlightReportDuration";
            this.txt_FlightReportDuration.Size = new System.Drawing.Size(172, 20);
            this.txt_FlightReportDuration.TabIndex = 26;
            this.txt_FlightReportDuration.TextChanged += new System.EventHandler(this.txt_FlightReportDuration_TextChanged);
            // 
            // lbl_ContactPhoneNumber
            // 
            this.lbl_ContactPhoneNumber.AutoSize = true;
            this.lbl_ContactPhoneNumber.Location = new System.Drawing.Point(226, 10);
            this.lbl_ContactPhoneNumber.Name = "lbl_ContactPhoneNumber";
            this.lbl_ContactPhoneNumber.Size = new System.Drawing.Size(118, 13);
            this.lbl_ContactPhoneNumber.TabIndex = 27;
            this.lbl_ContactPhoneNumber.Text = Resources.SettingsContactPhoneNumberText;
            // 
            // lbl_FlightReportName
            // 
            this.lbl_FlightReportName.AutoSize = true;
            this.lbl_FlightReportName.Location = new System.Drawing.Point(3, 52);
            this.lbl_FlightReportName.Name = "lbl_FlightReportName";
            this.lbl_FlightReportName.Size = new System.Drawing.Size(98, 13);
            this.lbl_FlightReportName.TabIndex = 27;
            this.lbl_FlightReportName.Text = Resources.SettingsFlightReportNameText;
            // 
            // txt_FlightReportName
            // 
            this.txt_FlightReportName.Location = new System.Drawing.Point(8, 68);
            this.txt_FlightReportName.Name = "txt_FlightReportName";
            this.txt_FlightReportName.Size = new System.Drawing.Size(172, 20);
            this.txt_FlightReportName.TabIndex = 25;
            this.txt_FlightReportName.TextChanged += new System.EventHandler(this.txt_FlightReportName_TextChanged);
            // 
            // tabPageAbout
            // 
            this.tabPageAbout.Controls.Add(this.web_About);
            this.tabPageAbout.Controls.Add(this.but_Disable);
            this.tabPageAbout.Controls.Add(this.but_Enable);
            this.tabPageAbout.Controls.Add(this.pic_AboutLogo);
            this.tabPageAbout.Location = new System.Drawing.Point(4, 22);
            this.tabPageAbout.Name = "tabPageAbout";
            this.tabPageAbout.Size = new System.Drawing.Size(445, 258);
            this.tabPageAbout.TabIndex = 3;
            this.tabPageAbout.Text = Resources.SettingsAboutTabText;
            this.tabPageAbout.UseVisualStyleBackColor = true;
            // 
            // but_Disable
            // 
            this.but_Disable.Location = new System.Drawing.Point(334, 180);
            this.but_Disable.Name = "but_Disable";
            this.but_Disable.Size = new System.Drawing.Size(75, 23);
            this.but_Disable.TabIndex = 44;
            this.but_Disable.Text = Resources.SettingsDisableText;
            this.but_Disable.UseVisualStyleBackColor = true;
            this.but_Disable.Click += new System.EventHandler(this.but_Disable_Click);
            // 
            // but_Enable
            // 
            this.but_Enable.Location = new System.Drawing.Point(334, 151);
            this.but_Enable.Name = "but_Enable";
            this.but_Enable.Size = new System.Drawing.Size(75, 23);
            this.but_Enable.TabIndex = 43;
            this.but_Enable.Text = Resources.SettingsEnableText;
            this.but_Enable.UseVisualStyleBackColor = true;
            this.but_Enable.Click += new System.EventHandler(this.but_Enable_Click);
            // 
            // pic_AboutLogo
            // 
            this.pic_AboutLogo.Location = new System.Drawing.Point(302, 3);
            this.pic_AboutLogo.Name = "pic_AboutLogo";
            this.pic_AboutLogo.Size = new System.Drawing.Size(140, 100);
            this.pic_AboutLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic_AboutLogo.TabIndex = 40;
            this.pic_AboutLogo.TabStop = false;
            // 
            // web_About
            // 
            this.web_About.AllowNavigation = false;
            this.web_About.AllowWebBrowserDrop = false;
            this.web_About.IsWebBrowserContextMenuEnabled = false;
            this.web_About.Location = new System.Drawing.Point(3, 3);
            this.web_About.MinimumSize = new System.Drawing.Size(20, 20);
            this.web_About.Name = "web_About";
            this.web_About.Size = new System.Drawing.Size(293, 250);
            this.web_About.TabIndex = 45;
            this.web_About.WebBrowserShortcutsEnabled = false;
            // 
            // AASettings
            // 
            this.ClientSize = new System.Drawing.Size(476, 304);
            this.Controls.Add(this.tabPages);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AASettings";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = Resources.SettingsWindowTitleText;
            this.tabPages.ResumeLayout(false);
            this.tabPageAccount.ResumeLayout(false);
            this.tabPageAccount.PerformLayout();
            this.tabPageMap.ResumeLayout(false);
            this.tabPageMap.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trk_OpacityAdjust)).EndInit();
            this.tabPageFlight.ResumeLayout(false);
            this.tabPageFlight.PerformLayout();
            this.tabPageAbout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pic_AboutLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl tabPages;
        private System.Windows.Forms.TabPage tabPageMap;
        private System.Windows.Forms.TabPage tabPageFlight;
        private System.Windows.Forms.CheckBox chk_FlightReportLocalScope;
        private System.Windows.Forms.TextBox txt_ExistingFlightPlanId;
        private System.Windows.Forms.CheckBox chk_UseExistingFlightPlanId;
        private System.Windows.Forms.CheckBox chk_FlightReportCommercial;
        private System.Windows.Forms.Label lbl_FlightReportDuration;
        private System.Windows.Forms.TextBox txt_FlightReportDuration;
        private System.Windows.Forms.Label lbl_FlightReportName;
        private System.Windows.Forms.TextBox txt_FlightReportName;
        private System.Windows.Forms.TabPage tabPageAccount;
        private MyButton but_SignOut;
        private MyButton but_SignIn;
        private System.Windows.Forms.LinkLabel lbl_FlightReportWhat;
        private System.Windows.Forms.CheckBox chk_FlightReportEnable;
        private System.Windows.Forms.Label lbl_FlightReportDescription;
        private System.Windows.Forms.TextBox txt_FlightReportDescription;
        private System.Windows.Forms.Label lbl_OverrideClientSuffix;
        private System.Windows.Forms.TextBox txt_OverrideUrlSuffix;
        private System.Windows.Forms.Label lbl_OverrideClientSecret;
        private System.Windows.Forms.TextBox txt_OverrideClientSecret;
        private System.Windows.Forms.Label lbl_OverrideClientId;
        private System.Windows.Forms.TextBox txt_OverrideClientId;
        private System.Windows.Forms.CheckBox chk_OverrideClientSettings;
        private System.Windows.Forms.Label lbl_UserDetails;
        private System.Windows.Forms.TrackBar trk_OpacityAdjust;
        private System.Windows.Forms.Label lbl_OpacityAdjust;
        private System.Windows.Forms.TabPage tabPageAbout;
        private MyButton but_Enable;
        private System.Windows.Forms.PictureBox pic_AboutLogo;
        private MyButton but_Disable;
        private System.Windows.Forms.TreeView trv_MapLayers;
        private MyButton btn_DefaultLayers;
        private System.Windows.Forms.CheckBox chk_EnablePlanMap;
        private System.Windows.Forms.CheckBox chk_EnableDataMap;
        private System.Windows.Forms.TextBox txt_SerialNumber;
        private System.Windows.Forms.TextBox txt_IcaoAddress;
        private System.Windows.Forms.TextBox txt_ContactPhone;
        private System.Windows.Forms.CheckBox chk_SerialNumber;
        private System.Windows.Forms.CheckBox chk_AllowSms;
        private System.Windows.Forms.CheckBox chk_IcaoAddress;
        private System.Windows.Forms.Label lbl_ContactPhoneNumber;
        private System.Windows.Forms.WebBrowser web_About;
    }
}