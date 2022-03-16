console.log('Hello World')

const url = require("url");
const http = require('http');
const WebSocket = require('ws');
const dgram = require('dgram');
const udpclient = dgram.createSocket('udp4');

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

  r2d = 57.2958;
    d2r = 0.0174533;
brng = 0;

const interval = setInterval(function ping() {
    brng += 10 * d2r;
    
    brng = brng % (360*d2r);

    //            var answer = new WebAPIs.Dowding.WSPackaging<VehicleTick>() { datatype = "vehicle_ticks", operation = "create", data = new VehicleTick(Ts: DateTime.Now.AddYears(1).toUnixTime() * 1000, CorrelationId: "iwashereagain", AgentId: Guid.NewGuid().ToString(), Lat: -35, Lon: 145, Hae: 40, Vn: (decimal)-1.7, Ve: (decimal)0.05, Vd: (decimal)0.019, Vendor: "DJI", Model: "mavic mini", Serial: "iwashere", Id: Guid.NewGuid().ToString(), ContactId: Guid.NewGuid().ToString()) }.ToJSON();


    
    time = Math.floor(new Date().getTime());
    console.log('interval');
 
    
    lat1 = -38.1705006099009 *d2r;
    lon1 = 144.543803930283 *d2r;
    homealt = 48;
    
    d= 50;
    R= 6378100.0;
    
    
    
    lat2 =Math.asin(Math.sin(lat1)*Math.cos(d/R) + Math.cos(lat1)*Math.sin(d/R)*Math.cos(brng));
    lon2 =lon1 + Math.atan2(Math.sin(brng)*Math.sin(d/R)*Math.cos(lat1), Math.cos(d/R)-Math.sin(lat1)*Math.sin(lat2));
    
    lat2 = lat2 *r2d;
    lon2 = lon2 *r2d;
    
    console.log(lat2 + ' ' + lon2 + ' ' + (homealt + d));
    
    wss.clients.forEach(function each(ws) {
        ws.send('{  "datatype": "vehicle_ticks",  "data": {    "ts": '+time+',    "correlation_id": "iwashereagain",    "agent_id": "025e98ab-3260-4144-918e-4858d0a7b1fe",    "lat": '+lat2+',    "lon": '+lon2+',    "hae": '+(homealt + d)+',    "vn": -1.7,    "ve": 0.05,    "vd": 0.019,    "vendor": "DJI",    "model": "mavic mini",    "serial": "iwashere",    "id": "4b31b0af-feec-4437-942e-29cccd54cbeb",    "contact_id": "9a64d68f-5f99-45e8-8bc5-939a5c395499"  },  "operation": "create"}');
    });

    lat2 =Math.asin(Math.sin(lat1)*Math.cos(d/R) + Math.cos(lat1)*Math.sin(d/R)*Math.cos(brng + 90*d2r));
    lon2 =lon1 + Math.atan2(Math.sin(brng+ 90*d2r)*Math.sin(d/R)*Math.cos(lat1), Math.cos(d/R)-Math.sin(lat1)*Math.sin(lat2));
    
    lat2 = lat2 *r2d;
    lon2 = lon2 *r2d;
    
    console.log(lat2 + ' ' + lon2 + ' ' + (homealt + (brng*r2d/6)));

    
    data = '<?xml version="1.0" standalone="yes"?><event version="2.0" uid="J-01334" type="a-h-A-M-F-U-M" time="'+new Date().toISOString()+'" start="2005-04-05T11:43:38.07Z" stale="2005-04-05T11:45:38.07Z" ><detail></detail><point lat="'+(lat2 + 0.00005)+'" lon="'+lon2+'" ce="45.3" hae="'+(homealt + d)+'" le="99.5" /></event>';

    udpclient.send(data, 0, data.length, 10011, '127.0.0.1');


  wss.clients.forEach(function each(ws) {
      ws.send('{  "datatype": "vehicle_ticks",  "data": {    "ts": '+time+',    "correlation_id": "iwashereagain1",    "agent_id": "025e98ab-3260-4144-918e-4858d0a7b1fe",    "lat": '+lat2+',    "lon": '+lon2+',    "hae": '+(homealt + (brng*r2d/6))+',    "vn": -1.7,    "ve": 0.05,    "vd": 0.019,    "vendor": "DJI",    "model": "mavic mini",    "serial": "iwashere2",    "id": "4b31b0af-feec-4437-942e-29cccd54cbe3",    "contact_id": "9a64d68f-5f99-45e8-8bc5-939a5c395494"  },  "operation": "create"}');
  });
}, 2000);


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
