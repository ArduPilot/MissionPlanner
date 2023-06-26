using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Services
{
    public class DatagramSender : IDatagramSender
    {


        public DatagramSender(string hostName, int portNumber)
        {

            this.PortNumber = portNumber;
            this.Host = hostName;
        }

        public string Host
        {
            get; set;
        }

        public string IPAddress
        {
            get
            {
                return this.IPAddressObject.ToString();
            }
            set
            {
                this.Host = value;
            }
        }

        public int PortNumber
        {
            get; set;
        }

        private IPAddress IPAddressObject
        {
            get
            {
                return Dns.GetHostEntry(this.Host).AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
            }
        }

        private IPEndPoint IPEndPoint => new IPEndPoint(this.IPAddressObject, this.PortNumber);

        public void SendToServer(byte[] data)
        {
            using (UdpClient client = new UdpClient(GetAvailablePort()))
            {
                client.Send(data, data.Length, this.IPEndPoint);
            }
        }

        /// <summary>
        /// checks for used ports and retrieves the first free port
        /// </summary>
        /// <returns>the free port or 0 if it did not find a free port</returns>
        /// <remarks>https://gist.github.com/jrusbatch/4211535</remarks>
        private int GetAvailablePort(int startingPort = 0)
        {
            IPEndPoint[] endPoints;
            List<int> portArray = new List<int>();

            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();

            //getting active connections
            TcpConnectionInformation[] connections = properties.GetActiveTcpConnections();
            portArray.AddRange(from n in connections
                               where n.LocalEndPoint.Port >= startingPort
                               select n.LocalEndPoint.Port);

            //getting active udp listeners
            endPoints = properties.GetActiveUdpListeners();
            portArray.AddRange(from n in endPoints
                               where n.Port >= startingPort
                               select n.Port);

            portArray.Sort();

            for (int i = startingPort; i < ushort.MaxValue; i++)
            {
                if (!portArray.Contains(i))
                {
                    return i;
                }
            }

            return 0;
        }

    }
}
