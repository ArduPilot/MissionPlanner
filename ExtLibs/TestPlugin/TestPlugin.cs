using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionPlanner.Plugin;
using MissionPlanner;
using System.Windows.Forms;
using System.IO;
using MissionPlanner.Utilities;
using MQTTnet.Client.Receiving;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class TestPlugin : Plugin
{
    string _Name = "Test Plugin";
    string _Version = "0.1";
    string _Author = "Michael Oborne";
    private TelstraUTM UTM;
    private JArray UAVS;
    private string clientid = "michael";

    public override string Name
    {
        get { return _Name; }
    }

    public override string Version
    {
        get { return _Version; }
    }

    public override string Author
    {
        get { return _Author; }
    }

    public override bool Init()
    {
        loopratehz = 1f;
        UTM = new TelstraUTM();
        return true;
    }

    public override bool Loaded()
    {
        UTM.Auth();

        UAVS = UTM.GetUAVs();

        UTM.StartMQTT(clientid, @"C:\Users\michael\Desktop\Hex\laam-mqtt-control\Hardware\pi_zero\certs\ca.crt",
            @"C:\Users\michael\Desktop\Hex\laam-mqtt-control\Hardware\pi_zero\certs\client.crt",
            @"C:\Users\michael\Desktop\Hex\laam-mqtt-control\Hardware\pi_zero\certs\client.key");

        UTM.MQTTClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(e =>
        {
            Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
            Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
            Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
            Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");

            var json = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

            var obj = JsonConvert.DeserializeObject<JObject>(json);

            var item = obj["telemetry"]["altitude"];
        });

        return true;
    }

    public override bool Loop()
    {
        UTM.Telemetry(clientid);
        return true;
    }


    public override bool Exit()
    {
        return true;
    }
}

