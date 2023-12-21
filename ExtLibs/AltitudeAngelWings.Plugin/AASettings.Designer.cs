using System.Windows.Forms;
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
            this.lst_FlightTelemetry = new System.Windows.Forms.ComboBox();
            this.chk_FlightsEnable = new System.Windows.Forms.CheckBox();
            this.but_Disable = new MissionPlanner.Controls.MyButton();
            this.but_Enable = new MissionPlanner.Controls.MyButton();
            this.lbl_UserDetails = new System.Windows.Forms.Label();
            this.lbl_OverrideClientSuffix = new System.Windows.Forms.Label();
            this.txt_OverrideClientSuffix = new System.Windows.Forms.TextBox();
            this.lbl_OverrideClientSecret = new System.Windows.Forms.Label();
            this.txt_OverrideClientSecret = new System.Windows.Forms.TextBox();
            this.lbl_OverrideClientId = new System.Windows.Forms.Label();
            this.txt_OverrideClientId = new System.Windows.Forms.TextBox();
            this.chk_OverrideClientSettings = new System.Windows.Forms.CheckBox();
            this.chk_FlightPlansEnable = new System.Windows.Forms.CheckBox();
            this.but_SignOut = new MissionPlanner.Controls.MyButton();
            this.but_SignIn = new MissionPlanner.Controls.MyButton();
            this.tabPageMap = new System.Windows.Forms.TabPage();
            this.lbl_AltitudeDisplay = new System.Windows.Forms.Label();
            this.lbl_AltitudeFilter = new System.Windows.Forms.Label();
            this.trk_AltitudeFilter = new System.Windows.Forms.TrackBar();
            this.chk_EnablePlanMap = new System.Windows.Forms.CheckBox();
            this.chk_EnableDataMap = new System.Windows.Forms.CheckBox();
            this.btn_DefaultLayers = new MissionPlanner.Controls.MyButton();
            this.trv_MapLayers = new System.Windows.Forms.TreeView();
            this.lbl_OpacityAdjust = new System.Windows.Forms.Label();
            this.trk_OpacityAdjust = new System.Windows.Forms.TrackBar();
            this.tabPageFlight = new System.Windows.Forms.TabPage();
            this.lbl_FlightPlanDescription = new System.Windows.Forms.Label();
            this.txt_FlightPlanDescription = new System.Windows.Forms.TextBox();
            this.txt_SerialNumber = new System.Windows.Forms.TextBox();
            this.txt_IcaoAddress = new System.Windows.Forms.TextBox();
            this.txt_ContactPhone = new System.Windows.Forms.TextBox();
            this.chk_SerialNumber = new System.Windows.Forms.CheckBox();
            this.txt_ExistingFlightPlanId = new System.Windows.Forms.TextBox();
            this.chk_AllowSms = new System.Windows.Forms.CheckBox();
            this.chk_IcaoAddress = new System.Windows.Forms.CheckBox();
            this.chk_UseExistingFlightPlanId = new System.Windows.Forms.CheckBox();
            this.lbl_FlightPlanDuration = new System.Windows.Forms.Label();
            this.txt_FlightPlanDuration = new System.Windows.Forms.TextBox();
            this.lbl_ContactPhoneNumber = new System.Windows.Forms.Label();
            this.lbl_FlightPlanName = new System.Windows.Forms.Label();
            this.txt_FlightPlanName = new System.Windows.Forms.TextBox();
            this.tabPageAbout = new System.Windows.Forms.TabPage();
            this.web_About = new System.Windows.Forms.WebBrowser();
            this.tabPages.SuspendLayout();
            this.tabPageAccount.SuspendLayout();
            this.tabPageMap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trk_AltitudeFilter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trk_OpacityAdjust)).BeginInit();
            this.tabPageFlight.SuspendLayout();
            this.tabPageAbout.SuspendLayout();
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
            this.tabPages.Size = new System.Drawing.Size(452, 284);
            this.tabPages.TabIndex = 15;
            // 
            // tabPageAccount
            // 
            this.tabPageAccount.Controls.Add(this.lst_FlightTelemetry);
            this.tabPageAccount.Controls.Add(this.chk_FlightsEnable);
            this.tabPageAccount.Controls.Add(this.but_Disable);
            this.tabPageAccount.Controls.Add(this.but_Enable);
            this.tabPageAccount.Controls.Add(this.lbl_UserDetails);
            this.tabPageAccount.Controls.Add(this.lbl_OverrideClientSuffix);
            this.tabPageAccount.Controls.Add(this.txt_OverrideClientSuffix);
            this.tabPageAccount.Controls.Add(this.lbl_OverrideClientSecret);
            this.tabPageAccount.Controls.Add(this.txt_OverrideClientSecret);
            this.tabPageAccount.Controls.Add(this.lbl_OverrideClientId);
            this.tabPageAccount.Controls.Add(this.txt_OverrideClientId);
            this.tabPageAccount.Controls.Add(this.chk_OverrideClientSettings);
            this.tabPageAccount.Controls.Add(this.chk_FlightPlansEnable);
            this.tabPageAccount.Controls.Add(this.but_SignOut);
            this.tabPageAccount.Controls.Add(this.but_SignIn);
            this.tabPageAccount.Location = new System.Drawing.Point(4, 22);
            this.tabPageAccount.Name = "tabPageAccount";
            this.tabPageAccount.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAccount.Size = new System.Drawing.Size(444, 258);
            this.tabPageAccount.TabIndex = 2;
            this.tabPageAccount.Text = global::AltitudeAngelWings.Plugin.Properties.Resources.SettingsAccountTabText;
            this.tabPageAccount.UseVisualStyleBackColor = true;
            // 
            // lst_FlightTelemetry
            // 
            this.lst_FlightTelemetry.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lst_FlightTelemetry.FormattingEnabled = true;
            this.lst_FlightTelemetry.Items.AddRange(new object[] {
            "Do not send flight telemetry",
            "Use Telemetry (UDP)",
            "Use Surveillance (HTTPS)"});
            this.lst_FlightTelemetry.Location = new System.Drawing.Point(101, 59);
            this.lst_FlightTelemetry.MaxDropDownItems = 3;
            this.lst_FlightTelemetry.Name = "lst_FlightTelemetry";
            this.lst_FlightTelemetry.Size = new System.Drawing.Size(192, 21);
            this.lst_FlightTelemetry.TabIndex = 48;
            this.lst_FlightTelemetry.SelectedIndexChanged += new System.EventHandler(this.lst_FlightTelemetry_SelectedIndexChanged);
            // 
            // chk_FlightsEnable
            // 
            this.chk_FlightsEnable.AutoSize = true;
            this.chk_FlightsEnable.Location = new System.Drawing.Point(101, 35);
            this.chk_FlightsEnable.Name = "chk_FlightsEnable";
            this.chk_FlightsEnable.Size = new System.Drawing.Size(92, 17);
            this.chk_FlightsEnable.TabIndex = 47;
            this.chk_FlightsEnable.Text = "Enable Flights";
            this.chk_FlightsEnable.UseVisualStyleBackColor = true;
            this.chk_FlightsEnable.CheckedChanged += new System.EventHandler(this.chk_FlightsEnable_CheckedChanged);
            // 
            // but_Disable
            // 
            this.but_Disable.Location = new System.Drawing.Point(6, 229);
            this.but_Disable.Name = "but_Disable";
            this.but_Disable.Size = new System.Drawing.Size(75, 23);
            this.but_Disable.TabIndex = 46;
            this.but_Disable.Text = global::AltitudeAngelWings.Plugin.Properties.Resources.SettingsDisableText;
            this.but_Disable.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_Disable.UseVisualStyleBackColor = true;
            this.but_Disable.Click += new System.EventHandler(this.but_Disable_Click);
            // 
            // but_Enable
            // 
            this.but_Enable.Location = new System.Drawing.Point(6, 200);
            this.but_Enable.Name = "but_Enable";
            this.but_Enable.Size = new System.Drawing.Size(75, 23);
            this.but_Enable.TabIndex = 45;
            this.but_Enable.Text = global::AltitudeAngelWings.Plugin.Properties.Resources.SettingsEnableText;
            this.but_Enable.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_Enable.UseVisualStyleBackColor = true;
            this.but_Enable.Click += new System.EventHandler(this.but_Enable_Click);
            // 
            // lbl_UserDetails
            // 
            this.lbl_UserDetails.Location = new System.Drawing.Point(299, 6);
            this.lbl_UserDetails.Name = "lbl_UserDetails";
            this.lbl_UserDetails.Size = new System.Drawing.Size(140, 246);
            this.lbl_UserDetails.TabIndex = 40;
            // 
            // lbl_OverrideClientSuffix
            // 
            this.lbl_OverrideClientSuffix.AutoSize = true;
            this.lbl_OverrideClientSuffix.Location = new System.Drawing.Point(98, 206);
            this.lbl_OverrideClientSuffix.Name = "lbl_OverrideClientSuffix";
            this.lbl_OverrideClientSuffix.Size = new System.Drawing.Size(97, 13);
            this.lbl_OverrideClientSuffix.TabIndex = 38;
            this.lbl_OverrideClientSuffix.Text = "URL Domain Suffix";
            // 
            // txt_OverrideClientSuffix
            // 
            this.txt_OverrideClientSuffix.Location = new System.Drawing.Point(103, 222);
            this.txt_OverrideClientSuffix.Name = "txt_OverrideClientSuffix";
            this.txt_OverrideClientSuffix.Size = new System.Drawing.Size(190, 20);
            this.txt_OverrideClientSuffix.TabIndex = 37;
            this.txt_OverrideClientSuffix.TextChanged += new System.EventHandler(this.txt_OverrideUrlSuffix_TextChanged);
            // 
            // lbl_OverrideClientSecret
            // 
            this.lbl_OverrideClientSecret.AutoSize = true;
            this.lbl_OverrideClientSecret.Location = new System.Drawing.Point(98, 167);
            this.lbl_OverrideClientSecret.Name = "lbl_OverrideClientSecret";
            this.lbl_OverrideClientSecret.Size = new System.Drawing.Size(67, 13);
            this.lbl_OverrideClientSecret.TabIndex = 36;
            this.lbl_OverrideClientSecret.Text = "Client Secret";
            // 
            // txt_OverrideClientSecret
            // 
            this.txt_OverrideClientSecret.Location = new System.Drawing.Point(103, 183);
            this.txt_OverrideClientSecret.Name = "txt_OverrideClientSecret";
            this.txt_OverrideClientSecret.Size = new System.Drawing.Size(190, 20);
            this.txt_OverrideClientSecret.TabIndex = 35;
            this.txt_OverrideClientSecret.UseSystemPasswordChar = true;
            this.txt_OverrideClientSecret.TextChanged += new System.EventHandler(this.txt_OverrideClientSecret_TextChanged);
            // 
            // lbl_OverrideClientId
            // 
            this.lbl_OverrideClientId.AutoSize = true;
            this.lbl_OverrideClientId.Location = new System.Drawing.Point(98, 128);
            this.lbl_OverrideClientId.Name = "lbl_OverrideClientId";
            this.lbl_OverrideClientId.Size = new System.Drawing.Size(47, 13);
            this.lbl_OverrideClientId.TabIndex = 34;
            this.lbl_OverrideClientId.Text = "Client ID";
            // 
            // txt_OverrideClientId
            // 
            this.txt_OverrideClientId.Location = new System.Drawing.Point(103, 144);
            this.txt_OverrideClientId.Name = "txt_OverrideClientId";
            this.txt_OverrideClientId.Size = new System.Drawing.Size(190, 20);
            this.txt_OverrideClientId.TabIndex = 33;
            this.txt_OverrideClientId.TextChanged += new System.EventHandler(this.txt_OverrideClientId_TextChanged);
            // 
            // chk_OverrideClientSettings
            // 
            this.chk_OverrideClientSettings.AutoSize = true;
            this.chk_OverrideClientSettings.Location = new System.Drawing.Point(101, 108);
            this.chk_OverrideClientSettings.Name = "chk_OverrideClientSettings";
            this.chk_OverrideClientSettings.Size = new System.Drawing.Size(136, 17);
            this.chk_OverrideClientSettings.TabIndex = 32;
            this.chk_OverrideClientSettings.Text = global::AltitudeAngelWings.Plugin.Properties.Resources.SettingsOverrideClientSettingsText;
            this.chk_OverrideClientSettings.UseVisualStyleBackColor = true;
            this.chk_OverrideClientSettings.CheckedChanged += new System.EventHandler(this.chk_OverrideClientSettings_CheckedChanged);
            // 
            // chk_FlightPlansEnable
            // 
            this.chk_FlightPlansEnable.AutoSize = true;
            this.chk_FlightPlansEnable.Location = new System.Drawing.Point(101, 12);
            this.chk_FlightPlansEnable.Name = "chk_FlightPlansEnable";
            this.chk_FlightPlansEnable.Size = new System.Drawing.Size(116, 17);
            this.chk_FlightPlansEnable.TabIndex = 30;
            this.chk_FlightPlansEnable.Text = "Enable Flight Plans";
            this.chk_FlightPlansEnable.UseVisualStyleBackColor = true;
            this.chk_FlightPlansEnable.CheckedChanged += new System.EventHandler(this.chk_FlightPlansEnable_CheckedChanged);
            // 
            // but_SignOut
            // 
            this.but_SignOut.Location = new System.Drawing.Point(6, 37);
            this.but_SignOut.Name = "but_SignOut";
            this.but_SignOut.Size = new System.Drawing.Size(75, 23);
            this.but_SignOut.TabIndex = 3;
            this.but_SignOut.Text = global::AltitudeAngelWings.Plugin.Properties.Resources.SettingsSignOutText;
            this.but_SignOut.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_SignOut.UseVisualStyleBackColor = true;
            this.but_SignOut.Click += new System.EventHandler(this.but_SignOut_Click);
            // 
            // but_SignIn
            // 
            this.but_SignIn.Location = new System.Drawing.Point(6, 8);
            this.but_SignIn.Name = "but_SignIn";
            this.but_SignIn.Size = new System.Drawing.Size(75, 23);
            this.but_SignIn.TabIndex = 2;
            this.but_SignIn.Text = global::AltitudeAngelWings.Plugin.Properties.Resources.SettingsSignInText;
            this.but_SignIn.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_SignIn.UseVisualStyleBackColor = true;
            this.but_SignIn.Click += new System.EventHandler(this.but_SignIn_Click);
            // 
            // tabPageMap
            // 
            this.tabPageMap.Controls.Add(this.lbl_AltitudeDisplay);
            this.tabPageMap.Controls.Add(this.lbl_AltitudeFilter);
            this.tabPageMap.Controls.Add(this.trk_AltitudeFilter);
            this.tabPageMap.Controls.Add(this.chk_EnablePlanMap);
            this.tabPageMap.Controls.Add(this.chk_EnableDataMap);
            this.tabPageMap.Controls.Add(this.btn_DefaultLayers);
            this.tabPageMap.Controls.Add(this.trv_MapLayers);
            this.tabPageMap.Controls.Add(this.lbl_OpacityAdjust);
            this.tabPageMap.Controls.Add(this.trk_OpacityAdjust);
            this.tabPageMap.Location = new System.Drawing.Point(4, 22);
            this.tabPageMap.Name = "tabPageMap";
            this.tabPageMap.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMap.Size = new System.Drawing.Size(444, 258);
            this.tabPageMap.TabIndex = 0;
            this.tabPageMap.Text = global::AltitudeAngelWings.Plugin.Properties.Resources.SettingsMapLayersText;
            this.tabPageMap.UseVisualStyleBackColor = true;
            // 
            // lbl_AltitudeDisplay
            // 
            this.lbl_AltitudeDisplay.Location = new System.Drawing.Point(320, 145);
            this.lbl_AltitudeDisplay.Name = "lbl_AltitudeDisplay";
            this.lbl_AltitudeDisplay.Size = new System.Drawing.Size(118, 20);
            this.lbl_AltitudeDisplay.TabIndex = 49;
            this.lbl_AltitudeDisplay.Text = "altitude";
            this.lbl_AltitudeDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_AltitudeFilter
            // 
            this.lbl_AltitudeFilter.AutoSize = true;
            this.lbl_AltitudeFilter.Location = new System.Drawing.Point(349, 132);
            this.lbl_AltitudeFilter.Name = "lbl_AltitudeFilter";
            this.lbl_AltitudeFilter.Size = new System.Drawing.Size(66, 13);
            this.lbl_AltitudeFilter.TabIndex = 48;
            this.lbl_AltitudeFilter.Text = "Show Below";
            // 
            // trk_AltitudeFilter
            // 
            this.trk_AltitudeFilter.LargeChange = 100;
            this.trk_AltitudeFilter.Location = new System.Drawing.Point(320, 100);
            this.trk_AltitudeFilter.Maximum = 1000;
            this.trk_AltitudeFilter.Minimum = 10;
            this.trk_AltitudeFilter.Name = "trk_AltitudeFilter";
            this.trk_AltitudeFilter.Size = new System.Drawing.Size(118, 45);
            this.trk_AltitudeFilter.SmallChange = 10;
            this.trk_AltitudeFilter.TabIndex = 47;
            this.trk_AltitudeFilter.Text = "Ground Data";
            this.trk_AltitudeFilter.TickFrequency = 100;
            this.trk_AltitudeFilter.Value = 300;
            this.trk_AltitudeFilter.ValueChanged += new System.EventHandler(this.trk_AltitudeFilter_ValueChanged);
            // 
            // chk_EnablePlanMap
            // 
            this.chk_EnablePlanMap.AutoSize = true;
            this.chk_EnablePlanMap.Location = new System.Drawing.Point(343, 203);
            this.chk_EnablePlanMap.Name = "chk_EnablePlanMap";
            this.chk_EnablePlanMap.Size = new System.Drawing.Size(71, 17);
            this.chk_EnablePlanMap.TabIndex = 46;
            this.chk_EnablePlanMap.Text = global::AltitudeAngelWings.Plugin.Properties.Resources.SettingsPlanMapText;
            this.chk_EnablePlanMap.UseVisualStyleBackColor = true;
            this.chk_EnablePlanMap.CheckedChanged += new System.EventHandler(this.chk_EnablePlanMap_CheckedChanged);
            // 
            // chk_EnableDataMap
            // 
            this.chk_EnableDataMap.AutoSize = true;
            this.chk_EnableDataMap.Location = new System.Drawing.Point(343, 180);
            this.chk_EnableDataMap.Name = "chk_EnableDataMap";
            this.chk_EnableDataMap.Size = new System.Drawing.Size(73, 17);
            this.chk_EnableDataMap.TabIndex = 45;
            this.chk_EnableDataMap.Text = global::AltitudeAngelWings.Plugin.Properties.Resources.SettingsDataMapText;
            this.chk_EnableDataMap.UseVisualStyleBackColor = true;
            this.chk_EnableDataMap.CheckedChanged += new System.EventHandler(this.chk_EnableDataMap_CheckedChanged);
            // 
            // btn_DefaultLayers
            // 
            this.btn_DefaultLayers.Location = new System.Drawing.Point(341, 6);
            this.btn_DefaultLayers.Name = "btn_DefaultLayers";
            this.btn_DefaultLayers.Size = new System.Drawing.Size(75, 31);
            this.btn_DefaultLayers.TabIndex = 44;
            this.btn_DefaultLayers.Text = global::AltitudeAngelWings.Plugin.Properties.Resources.SettingsDefaultLayersText;
            this.btn_DefaultLayers.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.btn_DefaultLayers.UseVisualStyleBackColor = true;
            this.btn_DefaultLayers.Click += new System.EventHandler(this.btn_DefaultLayers_Click);
            // 
            // trv_MapLayers
            // 
            this.trv_MapLayers.CheckBoxes = true;
            this.trv_MapLayers.Location = new System.Drawing.Point(6, 6);
            this.trv_MapLayers.Name = "trv_MapLayers";
            this.trv_MapLayers.Size = new System.Drawing.Size(308, 246);
            this.trv_MapLayers.TabIndex = 18;
            this.trv_MapLayers.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.trv_MapLayers_AfterCheck);
            // 
            // lbl_OpacityAdjust
            // 
            this.lbl_OpacityAdjust.AutoSize = true;
            this.lbl_OpacityAdjust.Location = new System.Drawing.Point(341, 75);
            this.lbl_OpacityAdjust.Name = "lbl_OpacityAdjust";
            this.lbl_OpacityAdjust.Size = new System.Drawing.Size(75, 13);
            this.lbl_OpacityAdjust.TabIndex = 17;
            this.lbl_OpacityAdjust.Text = "Opacity Adjust";
            // 
            // trk_OpacityAdjust
            // 
            this.trk_OpacityAdjust.LargeChange = 80;
            this.trk_OpacityAdjust.Location = new System.Drawing.Point(320, 43);
            this.trk_OpacityAdjust.Maximum = 240;
            this.trk_OpacityAdjust.Minimum = 20;
            this.trk_OpacityAdjust.Name = "trk_OpacityAdjust";
            this.trk_OpacityAdjust.Size = new System.Drawing.Size(118, 45);
            this.trk_OpacityAdjust.SmallChange = 20;
            this.trk_OpacityAdjust.TabIndex = 15;
            this.trk_OpacityAdjust.Text = "Ground Data";
            this.trk_OpacityAdjust.TickFrequency = 20;
            this.trk_OpacityAdjust.Value = 100;
            this.trk_OpacityAdjust.ValueChanged += new System.EventHandler(this.trk_OpacityAdjust_ValueChanged);
            // 
            // tabPageFlight
            // 
            this.tabPageFlight.Controls.Add(this.lbl_FlightPlanDescription);
            this.tabPageFlight.Controls.Add(this.txt_FlightPlanDescription);
            this.tabPageFlight.Controls.Add(this.txt_SerialNumber);
            this.tabPageFlight.Controls.Add(this.txt_IcaoAddress);
            this.tabPageFlight.Controls.Add(this.txt_ContactPhone);
            this.tabPageFlight.Controls.Add(this.chk_SerialNumber);
            this.tabPageFlight.Controls.Add(this.txt_ExistingFlightPlanId);
            this.tabPageFlight.Controls.Add(this.chk_AllowSms);
            this.tabPageFlight.Controls.Add(this.chk_IcaoAddress);
            this.tabPageFlight.Controls.Add(this.chk_UseExistingFlightPlanId);
            this.tabPageFlight.Controls.Add(this.lbl_FlightPlanDuration);
            this.tabPageFlight.Controls.Add(this.txt_FlightPlanDuration);
            this.tabPageFlight.Controls.Add(this.lbl_ContactPhoneNumber);
            this.tabPageFlight.Controls.Add(this.lbl_FlightPlanName);
            this.tabPageFlight.Controls.Add(this.txt_FlightPlanName);
            this.tabPageFlight.Location = new System.Drawing.Point(4, 22);
            this.tabPageFlight.Name = "tabPageFlight";
            this.tabPageFlight.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageFlight.Size = new System.Drawing.Size(444, 258);
            this.tabPageFlight.TabIndex = 1;
            this.tabPageFlight.Text = "Flight Plans";
            this.tabPageFlight.UseVisualStyleBackColor = true;
            // 
            // lbl_FlightPlanDescription
            // 
            this.lbl_FlightPlanDescription.AutoSize = true;
            this.lbl_FlightPlanDescription.Location = new System.Drawing.Point(4, 91);
            this.lbl_FlightPlanDescription.Name = "lbl_FlightPlanDescription";
            this.lbl_FlightPlanDescription.Size = new System.Drawing.Size(112, 13);
            this.lbl_FlightPlanDescription.TabIndex = 34;
            this.lbl_FlightPlanDescription.Text = "Flight Plan Description";
            // 
            // txt_FlightPlanDescription
            // 
            this.txt_FlightPlanDescription.Location = new System.Drawing.Point(6, 110);
            this.txt_FlightPlanDescription.Name = "txt_FlightPlanDescription";
            this.txt_FlightPlanDescription.Size = new System.Drawing.Size(172, 20);
            this.txt_FlightPlanDescription.TabIndex = 33;
            this.txt_FlightPlanDescription.TextChanged += new System.EventHandler(this.txt_FlightPlanDescription_TextChanged);
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
            this.chk_SerialNumber.Text = global::AltitudeAngelWings.Plugin.Properties.Resources.SettingsSerialNumberText;
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
            this.chk_AllowSms.Text = global::AltitudeAngelWings.Plugin.Properties.Resources.SettingsAllowSMSContactText;
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
            this.chk_IcaoAddress.Text = global::AltitudeAngelWings.Plugin.Properties.Resources.SettingsICAOAddressText;
            this.chk_IcaoAddress.UseVisualStyleBackColor = true;
            this.chk_IcaoAddress.CheckedChanged += new System.EventHandler(this.chk_IcaoAddress_CheckedChanged);
            // 
            // chk_UseExistingFlightPlanId
            // 
            this.chk_UseExistingFlightPlanId.AutoSize = true;
            this.chk_UseExistingFlightPlanId.Enabled = false;
            this.chk_UseExistingFlightPlanId.Location = new System.Drawing.Point(6, 10);
            this.chk_UseExistingFlightPlanId.Name = "chk_UseExistingFlightPlanId";
            this.chk_UseExistingFlightPlanId.Size = new System.Drawing.Size(150, 17);
            this.chk_UseExistingFlightPlanId.TabIndex = 30;
            this.chk_UseExistingFlightPlanId.Text = global::AltitudeAngelWings.Plugin.Properties.Resources.SettingsUseExistingFlightPlanIDText;
            this.chk_UseExistingFlightPlanId.UseVisualStyleBackColor = true;
            this.chk_UseExistingFlightPlanId.CheckedChanged += new System.EventHandler(this.chk_UseExistingFlightPlanId_CheckedChanged);
            // 
            // lbl_FlightPlanDuration
            // 
            this.lbl_FlightPlanDuration.AutoSize = true;
            this.lbl_FlightPlanDuration.Location = new System.Drawing.Point(3, 133);
            this.lbl_FlightPlanDuration.Name = "lbl_FlightPlanDuration";
            this.lbl_FlightPlanDuration.Size = new System.Drawing.Size(144, 13);
            this.lbl_FlightPlanDuration.TabIndex = 28;
            this.lbl_FlightPlanDuration.Text = "Flight Plan Duration (minutes)";
            // 
            // txt_FlightPlanDuration
            // 
            this.txt_FlightPlanDuration.Location = new System.Drawing.Point(6, 149);
            this.txt_FlightPlanDuration.Name = "txt_FlightPlanDuration";
            this.txt_FlightPlanDuration.Size = new System.Drawing.Size(172, 20);
            this.txt_FlightPlanDuration.TabIndex = 26;
            this.txt_FlightPlanDuration.TextChanged += new System.EventHandler(this.txt_FlightPlanDuration_TextChanged);
            // 
            // lbl_ContactPhoneNumber
            // 
            this.lbl_ContactPhoneNumber.AutoSize = true;
            this.lbl_ContactPhoneNumber.Location = new System.Drawing.Point(226, 10);
            this.lbl_ContactPhoneNumber.Name = "lbl_ContactPhoneNumber";
            this.lbl_ContactPhoneNumber.Size = new System.Drawing.Size(118, 13);
            this.lbl_ContactPhoneNumber.TabIndex = 27;
            this.lbl_ContactPhoneNumber.Text = "Contact Phone Number";
            // 
            // lbl_FlightPlanName
            // 
            this.lbl_FlightPlanName.AutoSize = true;
            this.lbl_FlightPlanName.Location = new System.Drawing.Point(3, 52);
            this.lbl_FlightPlanName.Name = "lbl_FlightPlanName";
            this.lbl_FlightPlanName.Size = new System.Drawing.Size(87, 13);
            this.lbl_FlightPlanName.TabIndex = 27;
            this.lbl_FlightPlanName.Text = "Flight Plan Name";
            // 
            // txt_FlightPlanName
            // 
            this.txt_FlightPlanName.Location = new System.Drawing.Point(6, 68);
            this.txt_FlightPlanName.Name = "txt_FlightPlanName";
            this.txt_FlightPlanName.Size = new System.Drawing.Size(172, 20);
            this.txt_FlightPlanName.TabIndex = 25;
            this.txt_FlightPlanName.TextChanged += new System.EventHandler(this.txt_FlightPlanName_TextChanged);
            // 
            // tabPageAbout
            // 
            this.tabPageAbout.Controls.Add(this.web_About);
            this.tabPageAbout.Location = new System.Drawing.Point(4, 22);
            this.tabPageAbout.Name = "tabPageAbout";
            this.tabPageAbout.Size = new System.Drawing.Size(444, 258);
            this.tabPageAbout.TabIndex = 3;
            this.tabPageAbout.Text = global::AltitudeAngelWings.Plugin.Properties.Resources.SettingsAboutTabText;
            this.tabPageAbout.UseVisualStyleBackColor = true;
            // 
            // web_About
            // 
            this.web_About.AllowWebBrowserDrop = false;
            this.web_About.Dock = System.Windows.Forms.DockStyle.Fill;
            this.web_About.IsWebBrowserContextMenuEnabled = false;
            this.web_About.Location = new System.Drawing.Point(0, 0);
            this.web_About.MinimumSize = new System.Drawing.Size(20, 20);
            this.web_About.Name = "web_About";
            this.web_About.ScriptErrorsSuppressed = true;
            this.web_About.Size = new System.Drawing.Size(444, 258);
            this.web_About.TabIndex = 45;
            this.web_About.WebBrowserShortcutsEnabled = false;
            this.web_About.NewWindow += new System.ComponentModel.CancelEventHandler(this.web_About_NewWindow);
            // 
            // AASettings
            // 
            this.ClientSize = new System.Drawing.Size(476, 304);
            this.Controls.Add(this.tabPages);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AASettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Altitude Angel Settings";
            this.tabPages.ResumeLayout(false);
            this.tabPageAccount.ResumeLayout(false);
            this.tabPageAccount.PerformLayout();
            this.tabPageMap.ResumeLayout(false);
            this.tabPageMap.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trk_AltitudeFilter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trk_OpacityAdjust)).EndInit();
            this.tabPageFlight.ResumeLayout(false);
            this.tabPageFlight.PerformLayout();
            this.tabPageAbout.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl tabPages;
        private System.Windows.Forms.TabPage tabPageMap;
        private System.Windows.Forms.TabPage tabPageFlight;
        private System.Windows.Forms.TextBox txt_ExistingFlightPlanId;
        private System.Windows.Forms.CheckBox chk_UseExistingFlightPlanId;
        private System.Windows.Forms.Label lbl_FlightPlanDuration;
        private System.Windows.Forms.TextBox txt_FlightPlanDuration;
        private System.Windows.Forms.Label lbl_FlightPlanName;
        private System.Windows.Forms.TextBox txt_FlightPlanName;
        private System.Windows.Forms.TabPage tabPageAccount;
        private MyButton but_SignOut;
        private MyButton but_SignIn;
        private System.Windows.Forms.CheckBox chk_FlightPlansEnable;
        private System.Windows.Forms.Label lbl_FlightPlanDescription;
        private System.Windows.Forms.TextBox txt_FlightPlanDescription;
        private System.Windows.Forms.Label lbl_OverrideClientSuffix;
        private System.Windows.Forms.TextBox txt_OverrideClientSuffix;
        private System.Windows.Forms.Label lbl_OverrideClientSecret;
        private System.Windows.Forms.TextBox txt_OverrideClientSecret;
        private System.Windows.Forms.Label lbl_OverrideClientId;
        private System.Windows.Forms.TextBox txt_OverrideClientId;
        private System.Windows.Forms.CheckBox chk_OverrideClientSettings;
        private System.Windows.Forms.Label lbl_UserDetails;
        private System.Windows.Forms.TrackBar trk_OpacityAdjust;
        private System.Windows.Forms.Label lbl_OpacityAdjust;
        private System.Windows.Forms.TabPage tabPageAbout;
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
        private Label lbl_AltitudeDisplay;
        private Label lbl_AltitudeFilter;
        private TrackBar trk_AltitudeFilter;
        private MyButton but_Disable;
        private MyButton but_Enable;
        private CheckBox chk_FlightsEnable;
        private ComboBox lst_FlightTelemetry;
    }
}