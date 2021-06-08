console.log('Hello World')

const url = require("url");
const http = require('http');
const WebSocket = require('ws');

const requestListener = function (req, res) {
    console.log('requestListener');
        const pathname = url.parse(req.url).pathname;
    if (pathname=== '/ws'){
        return;
    }

    res.setHeader("Content-Type", "application/json");
    res.writeHead(200);
    if (pathname=== '/api/1.0/authentication/login') {
        res.end('{"token":"test"}');
        return;
    }
    res.end('[]');
};


const server = http.createServer(requestListener);

const wss = new WebSocket.Server({ noServer: true });

wss.on('connection', function connection(ws) {
  ws.on('message', function incoming(message) {
    console.log('received: %s', message);
  });

});

const interval = setInterval(function ping() {
  wss.clients.forEach(function each(ws) {
      
//            var answer = new WebAPIs.Dowding.WSPackaging<VehicleTick>() { datatype = "vehicle_ticks", operation = "create", data = new VehicleTick(Ts: DateTime.Now.AddYears(1).toUnixTime() * 1000, CorrelationId: "iwashereagain", AgentId: Guid.NewGuid().ToString(), Lat: -35, Lon: 145, Hae: 40, Vn: (decimal)-1.7, Ve: (decimal)0.05, Vd: (decimal)0.019, Vendor: "DJI", Model: "mavic mini", Serial: "iwashere", Id: Guid.NewGuid().ToString(), ContactId: Guid.NewGuid().ToString()) }.ToJSON();

    time = Math.floor(new Date().getTime());
    console.log('interval');
    
    ws.send('{  "datatype": "vehicle_ticks",  "data": {    "ts": '+time+',    "correlation_id": "iwashereagain",    "agent_id": "025e98ab-3260-4144-918e-4858d0a7b1fe",    "lat": -38.1700188,    "lon": 144.5437986,    "hae": 0.0,    "vn": -1.7,    "ve": 0.05,    "vd": 0.019,    "vendor": "DJI",    "model": "mavic mini",    "serial": "iwashere",    "id": "4b31b0af-feec-4437-942e-29cccd54cbeb",    "contact_id": "9a64d68f-5f99-45e8-8bc5-939a5c395499"  },  "operation": "create"}');
    
    ws.send('{  "datatype": "vehicle_ticks",  "data": {    "ts": '+time+',    "correlation_id": "iwashereagain1",    "agent_id": "025e98ab-3260-4144-918e-4858d0a7b1fe",    "lat": -38.1704974,    "lon": 144.5448822,    "hae": 50.0,    "vn": -1.7,    "ve": 0.05,    "vd": 0.019,    "vendor": "DJI",    "model": "mavic mini",    "serial": "iwashere1",    "id": "4b31b0af-feec-4437-942e-29cccd54cbec",    "contact_id": "9a64d68f-5f99-45e8-8bc5-939a5c395498"  },  "operation": "create"}');
    
    ws.send('{  "datatype": "vehicle_ticks",  "data": {    "ts": '+time+',    "correlation_id": "iwashereagain2",    "agent_id": "025e98ab-3260-4144-918e-4858d0a7b1fe",    "lat": -38.1712060,    "lon": 144.5438120,    "hae": 100.0,    "vn": -1.7,    "ve": 0.05,    "vd": 0.019,    "vendor": "DJI",    "model": "mavic mini",    "serial": "iwashere2",    "id": "4b31b0af-feec-4437-942e-29cccd54cbed",    "contact_id": "9a64d68f-5f99-45e8-8bc5-939a5c395497"  },  "operation": "create"}');
    
    ws.send('{  "datatype": "vehicle_ticks",  "data": {    "ts": '+time+',    "correlation_id": "iwashereagain3",    "agent_id": "025e98ab-3260-4144-918e-4858d0a7b1fe",    "lat": -38.1705185,    "lon": 144.5429376,    "hae": 300.0,    "vn": -1.7,    "ve": 0.05,    "vd": 0.019,    "vendor": "DJI",    "model": "mavic mini",    "serial": "iwashere3",    "id": "4b31b0af-feec-4437-942e-29cccd54cbee",    "contact_id": "9a64d68f-5f99-45e8-8bc5-939a5c395496"  },  "operation": "create"}');
    

  });
}, 10000);


server.on('upgrade', function upgrade(request, socket, head) {
    console.log('upgrade');
  const pathname = url.parse(request.url).pathname;

  if (pathname === '/ws') {
    wss.handleUpgrade(request, socket, head, function done(ws) {
      wss.emit('connection', ws, request);
    });
  } else {
    socket.destroy();
  }
});

server.listen(80);
