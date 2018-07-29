using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public enum ConnectionTypes
    {
        TCP,
        UDP,
        UDPCl,
        AUTO,
        COM
    }

    public interface IConnectionConfig
    {
        ConnectionTypes ConnectionType { get; }
        CheckState AutoReconnect { get; set; }
        decimal AutoReconnectTimeout { get; set; }
    }
}
