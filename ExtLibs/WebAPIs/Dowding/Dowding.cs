using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
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
        private Configuration Configuration;
        private string token;
        public string URL { get; set; } = "https://{0}/api/1.0";
        public string WS { get; set; } = "wss://{0}/ws";

        public static ConcurrentDictionary<string, VehicleTick> Vehicles = new ConcurrentDictionary<string, VehicleTick>();

        /// <summary>
        /// Auth then Start
        /// </summary>
        public Dowding()
        {
            Console.WriteLine("Dowding .ctor");
            Configuration.Default.DefaultHeader["User-Agent"] =
                "MissionPlanner " + Assembly.GetExecutingAssembly().GetName().Version;
        }

        public async Task Auth(string email, string password, string server)
        {
            // if port 80, change url to non secure
            if (server.EndsWith(":80"))
            {
                URL = "http://{0}/api/1.0";
                WS =  "ws://{0}/ws";
            }

            Console.WriteLine("Dowding Auth");
            Configuration = new Configuration(new ApiClient(String.Format(URL, server)));

            var auth = new AuthenticationApi(Configuration);

            var tokenres = await auth.AuthenticationLoginPostAsync(new LoginDto(email, password));

            SetToken(tokenres?.Token, server);
        }

        public void SetToken(string customtoken, string server)
        {
            Configuration = new Configuration(new ApiClient(String.Format(URL, server)));

            token = customtoken;

            Configuration.AddApiKey("Authorization", "Bearer " + customtoken);
        }

        public async Task<WebSocket> Start(string server)
        {
            Console.WriteLine("Dowding Start");
            // starting point - get last 120seconds with the very last point of each, max of 100 nodes
            var contacts =
                (await GetContact(minTs: DateTime.UtcNow.AddSeconds(-120).toUnixTime().ToString(), thin: true))
                .OrderByDescending(a => a.VehicleLastTs).Take(100).ToList();

            contacts.ForEach(a =>
                {
                    Vehicles[a.Serial ?? a.VehicleId] = new VehicleTick(Ts: a.VehicleLastTs, Lat: a.VehicleLastLat,
                        Lon: a.VehicleLastLon, Hae: a.VehicleLastHae, CorrelationId: a.Id, AgentId: "",
                        ContactId: a.Id, Id: a.VehicleId, Serial: a.Serial);
                }
            );

            var ws = await StartWS(server);

            ws.MessageReceived += (sender, args) =>
            {
                var tick = JsonConvert.DeserializeObject<WSPackaging<VehicleTick>>(args.Message);

                Vehicles[tick.data.Id] = tick.data;
            };

            // on fail, wait 60 seconds and reconnect
            ws.Closed += (sender, args) =>
            {
                Thread.Sleep(60000);
                ws.OpenAsync();
            };

            ws.Error += (sender, args) =>
            {
                Console.WriteLine(args.Exception);
            };

            return ws;
        }

        public async Task<List<AgentTick>> GetAgents()
        {
            var agent = new AgentTickApi(Configuration);
            var list = await agent.AgentTickGetAsync();
            return list;
        }

        public async Task<List<VehicleTick>> GetVehicle(string contactIds = null)
        {
            var vehicle = new VehicleTickApi(Configuration);
            var list = await vehicle.VehicleTickGetAsync(contactIds);
            return list;
        }

        public async Task<List<Contact>> GetContact(decimal? offset = null, decimal? limit = null, bool? thin = null,
            string format = null, string maxLon = null, string minLon = null, string maxLat = null,
            string minLat = null, string maxTs = null, string minTs = null)
        {
            var contact = new ContactApi(Configuration);
            var list = await contact.ContactGetAsync(offset, limit, thin, format, maxLon, minLon, maxLat, minLat, maxTs,
                minTs);
            return list;
        }

        public async Task<List<Zone>> GetZone()
        {
            var zone = new ZoneApi(Configuration);
            var list = await zone.ZoneGetAsync();
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">contacts/vehicle_ticks/operator_ticks/homepoints/events</param>
        /// <returns></returns>
        public async Task<WebSocket> StartWS(string server, string type = "vehicle_ticks")
        {
            Console.WriteLine("Dowding StartWS");
            var ws = new WebSocket(String.Format(WS, server));
            await Task.Run(async () => await ws.OpenAsync());
            var connectmsg = JsonConvert.SerializeObject(new
            {
                @event = type,
                data = new
                {
                    headers = new
                    {
                        authorization = "Bearer " + token
                    }
                }
            });

            ws.Opened += (sender, args) => { ws.Send(connectmsg); };

            ws.Closed += (sender, args) => { };

            ws.Error += (sender, args) => { };

            return ws;
        }

        public class WSPackaging<T>
        {
            public string datatype { get; set; }
            public T data { get; set; }
            public string operation { get; set; }
        }
    }
}