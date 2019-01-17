using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OSDConfigurator.Models
{
    [DebuggerDisplay("{Name} (enabled: {Enabled.Value})")]
    public class OSDItem
    {
        public ICollection<IOSDSetting> Options { get; }

        public string Name { get; set; }

        public IOSDSetting Enabled
        {
            get { return EndsWith("_EN"); }
        }

        public IOSDSetting X
        {
            get { return EndsWith("_X"); }
        }

        public IOSDSetting Y
        {
            get { return EndsWith("_Y"); }           
        }

        public OSDItem(string name, ICollection<IOSDSetting> options)
        {
            Name = name;
            Options = options;
        }

        public IOSDSetting EndsWith(string suffix)
        {
            return Options.First(o => o.Name.EndsWith($"{Name}{suffix}"));
        }
    }

    [DebuggerDisplay("{Name}, {Items.Count} item(s)")]
    public class OSDScreen
    {
        public string Name { get; }
        public ICollection<IOSDSetting> Options { get; }
        public ICollection<OSDItem> Items { get; }

        public OSDScreen(string name, ICollection<IOSDSetting> options, ICollection<OSDItem> items)
        {
            Options = options;
            Items = items;
            Name = name;
        }
    }
}
