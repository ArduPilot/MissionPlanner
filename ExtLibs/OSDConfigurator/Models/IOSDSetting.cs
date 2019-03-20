using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSDConfigurator.Models
{    
    public interface IOSDSetting
    {
        event Action<IOSDSetting> Updated;

        string Name { get; }
        double Value { get; set; }
    }
}
