using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using webapi;
using System.Net;
using System.Net.Sockets;
using System.IO;
using ProtoBuf;

namespace MissionPlanner.Utilities.DroneApi
{
    public class DroneProto
    {
        TcpClient client = null;
        BufferedStream st = null;

        DateTime start = DateTime.MinValue;

        public bool connect()
        {
            client = new TcpClient(APIConstants.DEFAULT_SERVER, APIConstants.DEFAULT_TCP_PORT);

            client.NoDelay = true;

            start = DateTime.Now;

            st = new BufferedStream(client.GetStream(), 8000);

            Envelope ping = new Envelope()
            {
                ping = new PingMsg() {nonce = new Random().Next(int.MaxValue)},
                type = Envelope.MsgCode.PingMsgCode
            };

            DateTime deadline = DateTime.Now.AddMilliseconds(3000);
            while (DateTime.Now < deadline)
            {
                send(ping);

                System.Threading.Thread.Sleep(500);

                var pingresp = receive(0);

                if (pingresp != null && pingresp.pingResponse != null)
                    return true;
            }

            throw new IOException("No Response");
        }

        public bool loginUser(string user, string password)
        {
            LoginMsg m = new LoginMsg() {username = user, password = password, code = LoginRequestCode.LOGIN};

            Envelope msg = new Envelope() {login = m, type = Envelope.MsgCode.LoginMsgCode};

            send(msg);

            var loginresp = receive(0);

            if (loginresp != null && loginresp.loginResponse != null)
            {
                Console.WriteLine(loginresp.loginResponse.message + " " + loginresp.loginResponse.code);
                if (loginresp.loginResponse.code == LoginResponseMsg.ResponseCode.OK)
                    return true;

                throw new Exception("bad auth");
            }
            throw new IOException("no login response");
        }

        public bool startMission()
        {
            StartMissionMsg m = new StartMissionMsg() {uuid = Guid.NewGuid().ToString(), keep = false};

            Envelope msg = new Envelope() {startMission = m, type = Envelope.MsgCode.StartMissionMsgCode};

            send(msg);

            return true;
        }

        public bool setVechileId(string guid, int gcsinterface, int sysid, bool canAcceptCommands = true)
        {
            SenderIdMsg m = new SenderIdMsg()
            {
                vehicleUUID = guid,
                gcsInterface = gcsinterface,
                sysId = sysid,
                canAcceptCommands = canAcceptCommands
            };

            Envelope msg = new Envelope() {setSender = m, type = Envelope.MsgCode.SenderIdMsgCode};

            send(msg);

            return true;
        }

        public void SendMavlink(byte[] data, int gcsinterface)
        {
            var mavmsg = new MavlinkMsg();

            mavmsg.packet.Add(data);
            mavmsg.srcInterface = gcsinterface;
            //mavmsg.deltaT = (long)((DateTime.Now - start).TotalMilliseconds * 1000);

            Envelope msg = new Envelope() {mavlink = mavmsg, type = Envelope.MsgCode.MavlinkMsgCode};

            send(msg);
        }

        void send(Envelope msg)
        {
            if (client == null)
                return;

            // write the data to the stream
            Serializer.SerializeWithLengthPrefix<Envelope>(st, msg, PrefixStyle.Base128);

            st.Flush();
        }

        Envelope receive(int timeout)
        {
            // read data from the stream
            Envelope env = Serializer.DeserializeWithLengthPrefix<Envelope>(st, PrefixStyle.Base128);

            return env;
        }

        public void close()
        {
            if (st != null)
                st.Close();

            if (client != null)
                client.Close();

            client = null;
            st = null;
        }
    }
}