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

        /// <summary>
        /// called as the plugin is loaded.
        /// </summary>
        /// <returns></returns>
        bool Init();

        void Activate();
        void DeActivate();
        void Unload();

        void ExecuteOption(int option);

        bool Loaded();
    }

    public interface IPluginHost
    {
        MAVLink MAVInterface { get; }
    }
}
