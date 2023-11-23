using MissionPlanner.Plugin;

namespace Carbonix
{
    public class CarbonixPlugin : Plugin
    {
        public override string Name { get; } = "Carbonix Addons";
        public override string Version { get; } = "0.1";
        public override string Author { get; } = "Carbonix";

        public override bool Init() { return true; }

        public override bool Loaded()
        {
            return true;
        }

        public override bool Exit() { return true; }
    }
}
