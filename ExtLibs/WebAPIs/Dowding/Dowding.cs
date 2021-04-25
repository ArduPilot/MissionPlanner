using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dowding.Api;
using Dowding.Client;
using Dowding.Model;
using Newtonsoft.Json;
using WebSocket4Net;

namespace MissionPlanner.WebAPIs
{
    public class Dowding
    {
        private AuthenticationApi auth;
        private AuthToken token;
        public string URL { get; set; } = "https://test.dowding.cuas.dds.mil/api/1.0";
        public string WS { get; set; } = "wss://test.dowding.cuas.dds.mil/ws";

        public async Task Auth(string email, string password)
        {
            auth = new AuthenticationApi(URL);

            token = await auth.AuthenticationLoginPostAsync(new LoginDto(email, password));
        }

        public void SetToken(string customtoken)
        {
            token = new AuthToken(customtoken);
        }

        public async Task<List<VehicleTick>> GetVehicles()
        {
            var vehicle = new VehicleTickApi(URL);
            var list = await vehicle.VehicleTickGetAsync();
            return list;
        }

        public async Task<WebSocket> StartWS<T>(string type = "vehicle_ticks")
        {
            var ws = new WebSocket(WS);
            await ws.OpenAsync();
            var connectmsg = JsonConvert.SerializeObject(new
            {
                @event = type,
                data = new
                {
                    headers = new
                    {
                        authorization = "Bearer " + token?.Token
                    }
                }
            });

            ws.Opened += (sender, args) => { ws.Send(connectmsg); };

            ws.MessageReceived += (sender, args) =>
            {
                var item = JsonConvert.DeserializeObject<T>(args.Message);
            };

            ws.Closed += (sender, args) => { };

            ws.Error += (sender, args) => { };

            return ws;
        }
    }
}
