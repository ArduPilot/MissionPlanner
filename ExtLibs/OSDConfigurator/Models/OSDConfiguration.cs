using OSDConfigurator.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSDConfigurator.Models
{
    public class OSDConfiguration
    {
        public IOSDSetting[] Options { get; private set; }
        public OSDScreen[] Screens { get; private set; }

        public OSDConfiguration(IOSDSetting[] options, OSDScreen[] screens)
        {
            Options = options;
            Screens = screens;
        }
    }
}
