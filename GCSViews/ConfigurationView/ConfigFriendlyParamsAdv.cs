using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using log4net;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigFriendlyParamsAdv : ConfigFriendlyParams
    {
        public ConfigFriendlyParamsAdv()
        {
            ParameterMode = ParameterMode = ParameterMetaDataConstants.Advanced;
        }
    }
}