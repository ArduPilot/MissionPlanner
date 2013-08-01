using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArdupilotMega.Plugin
{
    public interface IPlugin
    {
        IPluginHost Host { get; set; }

        string Name { get; }
        string Version { get; }
        string Author { get; }

        bool Loaded();

        bool Init();

        bool SetupUI();

        bool Loop();

        int loopratehz { get; set; }

        bool Exit();

    }

    public interface IPluginHost
    {
        MainV2 Planner { get; }
    }
}
