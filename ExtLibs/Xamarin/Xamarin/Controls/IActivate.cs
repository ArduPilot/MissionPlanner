using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.Controls
{
    /// <summary>
    /// The implementor executes some logic on activation, for e.g when moving 
    /// from not selected to selected in a tab control
    /// </summary>
    public interface IActivate
    {
        // Should be idempotent
        void Activate();
    }

    /// <summary>
    /// The implementor executes some logic on deactivation, for e.g when moving 
    /// from selected to not selected in a tab control
    /// </summary>
    public interface IDeactivate
    {
        // Should be idempotent
        void Deactivate();
    }
}
