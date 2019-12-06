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