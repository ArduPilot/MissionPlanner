using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArdupilotMega.Utilities;
using System.Xml;
using System.IO;
using log4net;
using System.Reflection;
using System.Globalization;
using ArdupilotMega.Controls;

namespace ArdupilotMega.GCSViews.ConfigurationView
{
    public partial class ConfigSimplePidsV2: MyUserControl, IActivate
    {
        internal static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ConfigSimplePidsV2()
        {
            InitializeComponent();

        }

        public void Activate()
        {
        }

        private void TRK_Gain_Scroll(object sender, EventArgs e)
        {
            DoZieglerNichols();
        }

        private void TRK_damp_Scroll(object sender, EventArgs e)
        {
            DoZieglerNichols();
        }

        void DoZieglerNichols()
        {
            float Ku = TRK_Gain.Value;
            float Tu = TRK_damp.Value;

            float Kp = 1f * Ku;

            MainV2.comPort.setParam("RATE_RLL_P", Kp);
            MainV2.comPort.setParam("RATE_RLL_I", 1.2f * Kp / Tu);
            MainV2.comPort.setParam("RATE_RLL_D", Kp * Tu / 8);
            MainV2.comPort.setParam("RATE_PIT_P", Kp);
            MainV2.comPort.setParam("RATE_PIT_I", 1.2f * Kp / Tu);
            MainV2.comPort.setParam("RATE_PIT_D", Kp * Tu / 8);


        }
    }
}
