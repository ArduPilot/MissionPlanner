using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public static class ConnectionMethods
    {
        public static ConnectionType GetConnectionType(string connectionType)
        {
            if (connectionType.Equals("TCP")) return ConnectionType.TCP;
            else if (connectionType.Equals("UDP")) return ConnectionType.UDP;
            else if (connectionType.Equals("UDPCl")) return ConnectionType.UDPCl;
            else if (connectionType.Equals("AUTO")) return ConnectionType.AUTO;
            else return ConnectionType.COM;
        }
    }

    public enum ConnectionType
    {
        TCP,
        UDP,
        UDPCl,
        AUTO,
        COM
    }

    public interface IConnectionConfig
    {
        ConnectionType ConnectionType { get; }
        CheckState AutoReconnect { get; }
        decimal AutoReconnectTimeout { get; }
    }
}
