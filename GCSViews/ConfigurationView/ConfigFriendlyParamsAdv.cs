using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigFriendlyParamsAdv : ConfigFriendlyParams
    {
        [System.Obsolete]
        public ConfigFriendlyParamsAdv()
        {
            ParameterMode = ParameterMode = ParameterMetaDataConstants.Advanced;
        }
    }
}