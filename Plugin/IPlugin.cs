using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArdupilotMega.Plugin
{
    public interface IPlugin
    {
        MainV2 Host { get; set; }

        string Name { get; }
        string Version { get; }
        string Author { get; }

        DateTime NextRun { get; set; }

        bool Loaded();

        bool Init();

        bool SetupUI();

        bool Loop();

        int loopratehz { get; set; }

        bool Exit();

    }
}
