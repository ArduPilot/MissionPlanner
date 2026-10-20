using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner;
using MissionPlanner.ArduPilot.Mavlink;
using MissionPlanner.Comms;

namespace MissionPlannerTests.Mavlink
{
    [TestClass]
    public class CameraSettingsTests
    {
        private const string Xml = @"<mavlinkcamera><definition version='1'><model>Test Camera</model></definition><parameters>
<parameter name='MODE' type='uint32' control='0'><options>
 <option name='Auto' value='0'><exclusions><exclude>GAIN</exclude></exclusions></option>
 <option name='Manual' value='1'><parameterranges><parameterrange parameter='ISO' condition='GAIN&gt;=2 AND ENABLED=1'><roption name='Low' value='100'/></parameterrange></parameterranges></option>
</options><updates><update>GAIN</update><update>ISO</update></updates></parameter>
<parameter name='ENABLED' type='bool'/>
<parameter name='GAIN' type='float' min='0' max='10' step='0.5'/>
<parameter name='ISO' type='uint32'><options><option name='Low' value='100'/><option name='High' value='200'/></options></parameter>
<parameter name='SERIAL' type='uint64' readonly='1'/>
<parameter name='RESET' type='uint8' writeonly='1'/>
<parameter name='VENDOR' type='custom' default='opaque'/>
</parameters></mavlinkcamera>";
        private static CameraDefinition Definition(string xml = Xml) => new CameraDefinition(Encoding.UTF8.GetBytes(xml));
        private static readonly DateTime Start = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static void Value(CameraSettingsSession session, string name, string text, DateTime? now = null)
        {
            var p = session.Definition.Parameters[name];
            session.Receive(MAVLink.MAVLINK_MSG_ID.PARAM_EXT_VALUE,
                new MAVLink.mavlink_param_ext_value_t(7, 0, p.Id, p.Encode(p.ConvertValue(text)), p.WireType), now ?? Start);
        }
        private static void Ack(CameraSettingsSession session, string name, string text, MAVLink.PARAM_ACK result, DateTime? now = null)
        {
            var p = session.Definition.Parameters[name];
            session.Receive(MAVLink.MAVLINK_MSG_ID.PARAM_EXT_ACK,
                new MAVLink.mavlink_param_ext_ack_t(p.Id, p.Encode(p.ConvertValue(text)), p.WireType, (byte)result), now ?? Start);
        }

        [TestMethod]
        public void BinaryEncodingPreservesAllNumericTypesAndFullWidthIntegers()
        {
            string[] types = { "uint8", "int8", "uint16", "int16", "uint32", "int32", "uint64", "int64", "float", "double" };
            string[] values = { "255", "-128", "65535", "-32768", "4294967295", "-2147483648", "18446744073709551615", "-9223372036854775808", "0.1", "1.2345678901234567" };
            for (int i = 0; i < types.Length; i++)
            {
                var p = Definition("<mavlinkcamera><definition/><parameters><parameter name='X' type='" + types[i] + "'/></parameters></mavlinkcamera>").Parameters["X"];
                var value = p.ConvertValue(values[i]);
                var bytes = p.Encode(value);
                Assert.AreEqual(128, bytes.Length);
                Assert.AreEqual(value, p.Decode(bytes, (byte)(i + 1)));
            }
            var gain = Definition().Parameters["GAIN"];
            CollectionAssert.AreEqual(new byte[] { 0, 0, 0xc0, 0x3f }, gain.Encode(1.5f).Take(4).ToArray());
            Assert.ThrowsException<ArgumentException>(() => gain.Decode(gain.Encode(1f), 5));
            Assert.ThrowsException<ArgumentException>(() => gain.ConvertValue("NaN"));
        }

        [TestMethod]
        public void XmlRulesUseCurrentValuesAndRefreshDependentOptions()
        {
            var definition = Definition();
            var values = new Dictionary<string, object> { ["MODE"] = 0u, ["GAIN"] = 3f, ["ENABLED"] = (byte)1 };
            Assert.IsFalse(definition.Controls(values).ContainsKey("GAIN"));
            Assert.IsFalse(definition.Controls(values).ContainsKey("MODE"));
            Assert.IsFalse(definition.Controls(values).ContainsKey("VENDOR"));
            values["MODE"] = 1u;
            Assert.IsTrue(definition.Controls(values).ContainsKey("GAIN"));
            Assert.AreEqual(1, definition.Controls(values)["ISO"].Count);
            Assert.ThrowsException<ArgumentException>(() => definition.Parameters["ISO"].Validate("200", definition.Controls(values)["ISO"]));
            values["ENABLED"] = (byte)0;
            Assert.AreEqual(2, definition.Controls(values)["ISO"].Count);
            CollectionAssert.AreEqual(new[] { "GAIN", "ISO" }, definition.Parameters["MODE"].Updates);
        }

        [TestMethod]
        public void NumericLimitsStepsAndBooleanValuesAreValidated()
        {
            var p = Definition().Parameters["GAIN"];
            Assert.AreEqual(2.5f, p.Validate("2.5", p.Options));
            Assert.ThrowsException<ArgumentException>(() => p.Validate("2.6", p.Options));
            Assert.ThrowsException<ArgumentException>(() => p.Validate("11", p.Options));
            Assert.ThrowsException<ArgumentException>(() => Definition().Parameters["ENABLED"].ConvertValue("2"));
            var wide = Definition("<mavlinkcamera><definition/><parameters><parameter name='W' type='uint64' min='18446744073709551610' max='18446744073709551615' step='2'/></parameters></mavlinkcamera>").Parameters["W"];
            Assert.AreEqual(18446744073709551614ul, wide.Validate("18446744073709551614", wide.Options));
            Assert.ThrowsException<ArgumentException>(() => wide.Validate("18446744073709551615", wide.Options));
        }

        [TestMethod]
        public void WritesRequireCameraValuesAndAcknowledgements()
        {
            var sent = new List<object>();
            var session = new CameraSettingsSession(Definition(), 7, 100, sent.Add);
            Assert.IsFalse(session.CanEdit("GAIN"));
            Assert.IsFalse(session.CanEdit("SERIAL"));
            Assert.IsTrue(session.CanEdit("RESET"));
            Value(session, "MODE", "1"); Value(session, "GAIN", "1");
            session.SetValue("GAIN", "2.5", Start);
            var set = (MAVLink.mavlink_param_ext_set_t)sent.Single();
            Assert.AreEqual((byte)7, set.target_system); Assert.AreEqual((byte)100, set.target_component);
            Assert.AreEqual((byte)9, set.param_type);
            CollectionAssert.AreEqual(new byte[] { 0, 0, 0x20, 0x40 }, set.param_value.Take(4).ToArray());
            Value(session, "GAIN", "2.5"); // A read cannot complete a write.
            Assert.IsTrue(session.IsPending("GAIN")); Assert.AreEqual(1f, session.Values["GAIN"]);
            Ack(session, "GAIN", "2.5", MAVLink.PARAM_ACK.ACCEPTED);
            Assert.IsFalse(session.IsPending("GAIN")); Assert.AreEqual(2.5f, session.Values["GAIN"]);
        }

        [TestMethod]
        public void AcceptedCameraAdjustedValuesAreDisplayedAndReadBack()
        {
            var sent = new List<object>();
            var session = new CameraSettingsSession(Definition(), 7, 100, sent.Add);
            Value(session, "MODE", "1"); Value(session, "GAIN", "1");
            session.SetValue("GAIN", "2.5", Start);
            Ack(session, "GAIN", "2", MAVLink.PARAM_ACK.ACCEPTED);
            Assert.IsFalse(session.IsPending("GAIN"));
            Assert.AreEqual(2f, session.Values["GAIN"]);
            session.Tick(Start);
            Assert.IsTrue(sent.Last() is MAVLink.mavlink_param_ext_request_read_t);
        }

        [TestMethod]
        public void InProgressStopsResendsAndHasAnAbsoluteDeadline()
        {
            var sent = new List<object>();
            var session = new CameraSettingsSession(Definition(), 7, 100, sent.Add);
            session.SetValue("RESET", "1", Start);
            Ack(session, "RESET", "0", MAVLink.PARAM_ACK.IN_PROGRESS, Start.AddSeconds(1));
            session.Tick(Start.AddSeconds(10));
            Assert.IsTrue(session.IsPending("RESET"));
            Assert.AreEqual(1, sent.Count);
            Ack(session, "RESET", "0", MAVLink.PARAM_ACK.IN_PROGRESS, Start.AddSeconds(29));
            session.Tick(Start.AddSeconds(31));
            Assert.IsFalse(session.IsPending("RESET"));
            StringAssert.Contains(session.Errors["RESET"], "timed out");
        }

        [TestMethod]
        public void RejectedAndLostWritesReadBackStateAndKeepTheErrorVisible()
        {
            var sent = new List<object>();
            var session = new CameraSettingsSession(Definition(), 7, 100, sent.Add);
            Value(session, "MODE", "1"); Value(session, "GAIN", "1");
            session.SetValue("GAIN", "2", Start);
            Ack(session, "GAIN", "1", MAVLink.PARAM_ACK.FAILED);
            Assert.IsFalse(session.CanEdit("GAIN"));
            session.Tick(Start);
            Assert.IsTrue(sent.Last() is MAVLink.mavlink_param_ext_request_read_t);
            Value(session, "GAIN", "1");
            Assert.IsTrue(session.CanEdit("GAIN")); StringAssert.Contains(session.Errors["GAIN"], "rejected");
            session.SetValue("GAIN", "3", Start);
            session.Tick(Start.AddSeconds(2)); session.Tick(Start.AddSeconds(4)); session.Tick(Start.AddSeconds(6));
            Assert.IsFalse(session.IsPending("GAIN")); Assert.IsFalse(session.CanEdit("GAIN"));
            StringAssert.Contains(session.Errors["GAIN"], "timed out");
            Assert.AreEqual(4, sent.OfType<MAVLink.mavlink_param_ext_set_t>().Count());
            Value(session, "GAIN", "3", Start.AddSeconds(7));
            Assert.IsTrue(session.CanEdit("GAIN"));
            StringAssert.Contains(session.Errors["GAIN"], "current camera value has been read back");
        }

        [TestMethod]
        public void LostReadsArePacedRetriedAndReported()
        {
            var sent = new List<object>();
            var session = new CameraSettingsSession(Definition(), 7, 100, sent.Add);
            session.RequestAll(Start);
            Assert.AreEqual(1, sent.Count);
            Value(session, "MODE", "1");
            session.Tick(Start.AddSeconds(2));
            var reads = sent.OfType<MAVLink.mavlink_param_ext_request_read_t>().ToList();
            Assert.IsTrue(reads.Count <= 5);
            Assert.IsFalse(reads.Any(r => Encoding.ASCII.GetString(r.param_id).TrimEnd('\0') == "MODE"));
            Assert.IsFalse(reads.Any(r => Encoding.ASCII.GetString(r.param_id).TrimEnd('\0') == "RESET"));
            for (int i = 3; i <= 10; i++) session.Tick(Start.AddSeconds(i));
            Assert.IsTrue(session.Errors.ContainsKey("GAIN"));
            Assert.IsFalse(session.CanEdit("GAIN"));
        }

        [TestMethod]
        public void ChangingAParameterReadsItsDependencies()
        {
            var sent = new List<object>();
            var session = new CameraSettingsSession(Definition(), 7, 100, sent.Add);
            Value(session, "MODE", "0");
            Value(session, "MODE", "1");
            session.Tick(Start.AddSeconds(1));
            var names = sent.OfType<MAVLink.mavlink_param_ext_request_read_t>().Select(r => Encoding.ASCII.GetString(r.param_id).TrimEnd('\0')).ToArray();
            CollectionAssert.AreEquivalent(new[] { "GAIN", "ISO" }, names);
        }

        [TestMethod]
        public void IdenticalCustomValuesDoNotRepeatedlyReadDependencies()
        {
            var definition = Definition(Xml.Replace("type='custom' default='opaque'/>", "type='custom'><updates><update>GAIN</update></updates></parameter>"));
            var sent = new List<object>();
            var session = new CameraSettingsSession(definition, 7, 100, sent.Add);
            var p = definition.Parameters["VENDOR"];
            var data = new byte[128];
            session.Receive(MAVLink.MAVLINK_MSG_ID.PARAM_EXT_VALUE, new MAVLink.mavlink_param_ext_value_t(7, 0, p.Id, data, 11), Start);
            session.Receive(MAVLink.MAVLINK_MSG_ID.PARAM_EXT_VALUE, new MAVLink.mavlink_param_ext_value_t(7, 0, p.Id, (byte[])data.Clone(), 11), Start);
            session.Tick(Start.AddSeconds(1));
            Assert.AreEqual(0, sent.Count);
            data[0] = 1;
            session.Receive(MAVLink.MAVLINK_MSG_ID.PARAM_EXT_VALUE, new MAVLink.mavlink_param_ext_value_t(7, 0, p.Id, data, 11), Start.AddSeconds(1));
            session.Tick(Start.AddSeconds(2));
            Assert.IsTrue(sent.Single() is MAVLink.mavlink_param_ext_request_read_t);
        }

        [TestMethod]
        public void DefinitionRejectsDtdDuplicatesAndOversizedInput()
        {
            Assert.ThrowsException<System.Xml.XmlException>(() => Definition("<!DOCTYPE mavlinkcamera [<!ENTITY x SYSTEM 'file:///etc/passwd'>]><mavlinkcamera><definition/></mavlinkcamera>"));
            Assert.ThrowsException<InvalidDataException>(() => Definition(Xml.Replace("name='GAIN' type='float'", "name='MODE' type='float'")));
            Assert.ThrowsException<InvalidDataException>(() => new CameraDefinition(new byte[CameraDefinition.MaxSize + 1]));
            using (var stream = new MemoryStream(new byte[CameraDefinition.MaxSize + 1]))
                Assert.ThrowsException<InvalidDataException>(() => CameraDefinition.ReadLimited(stream));
        }

        [TestMethod]
        public void XzCompressedDefinitionsAreDecoded()
        {
            var data = Convert.FromBase64String("/Td6WFoAAATm1rRGAgAhARYAAAB0L+Wj4ABcAEFdAB4bSCeZ16KIhYkKUZWWc6ppnTBIm0GdYyH9Kqe98DP5Nry4q9sMG6jm2kH5I+aWzcYHV/tr6Jd2aOvA89QdC5IAAAAAALwYQAsD6KLGAAFdXXu7SToftvN9AQAAAAAEWVo=");
            Assert.AreEqual("XZ Camera", new CameraDefinition(data).Model);
        }

        [TestMethod]
        public void NonModalInformationRequestsDoNotWaitForCommandAcknowledgements()
        {
            using (var link = new MAVLinkInterface())
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.MAVlist[7, 100].mavlinkv2 = true;
                wire.WriteCallback = (sender, bytes) =>
                {
                    var message = new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() };
                    var command = (MAVLink.mavlink_command_long_t)message.data;
                    wire.AppendBuffer(new MAVLink.MavlinkParse().GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.COMMAND_ACK,
                        new MAVLink.mavlink_command_ack_t { command = command.command, result = 0 }, false, 7, 100));
                };
                var camera = new CameraProtocol();
                camera.StartID(link.MAVlist[7, 100]).GetAwaiter().GetResult();
                wire.DiscardInBuffer();
                var commands = new List<MAVLink.mavlink_command_long_t>();
                wire.WriteCallback = (sender, bytes) => commands.Add((MAVLink.mavlink_command_long_t)new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() }.data);
                Assert.IsTrue(camera.RequestCameraInformationAsync(requireAck: false).Wait(1000));
                Assert.AreEqual(3, commands.Count);
                Assert.IsTrue(commands.All(c => c.target_system == 7 && c.target_component == 100));
                Assert.AreEqual(1f, commands.Single(c => c.command == (ushort)MAVLink.MAV_CMD.REQUEST_CAMERA_INFORMATION).param1);
                Assert.IsFalse(link.giveComport, "A non-modal information request must leave the telemetry reader running");
            }
        }

        [TestMethod]
        public void MavFtpUrisPreserveCameraTargetsAndDecodeFilenames()
        {
            var address = CameraDefinitionLoader.ParseFtpAddress("mftp://[;comp=101]/camera%20settings.xml", uint.MaxValue, 100);
            Assert.AreEqual(uint.MaxValue, address.System); Assert.AreEqual((byte)101, address.Component);
            Assert.AreEqual("/camera settings.xml", address.Path);
            address = CameraDefinitionLoader.ParseFtpAddress("mavftp://42:102/camera.xml", 7, 100);
            Assert.AreEqual(42u, address.System); Assert.AreEqual((byte)102, address.Component);
            address = CameraDefinitionLoader.ParseFtpAddress("mftp:///camera.xml", 7, 100);
            Assert.AreEqual(7u, address.System); Assert.AreEqual((byte)100, address.Component);
            Assert.ThrowsException<ArgumentException>(() => CameraDefinitionLoader.ParseFtpAddress("mftp://[;comp=0]/camera.xml", 7, 100));
            Assert.ThrowsException<ArgumentException>(() => CameraDefinitionLoader.ParseFtpAddress("mftp:///bad%00.xml", 7, 100));
        }

        [TestMethod]
        public void XmlOptionsUseExactFloatWireValues()
        {
            var p = Definition("<mavlinkcamera><definition/><parameters><parameter name='F' type='float'><options><option name='A' value='1'/><option name='B' value='1.00000011920928955078125'/></options></parameter></parameters></mavlinkcamera>").Parameters["F"];
            Assert.AreNotEqual(p.Options[0].Value, p.Options[1].Value);
            Assert.AreEqual(p.Options[1].Value, p.Validate("1.00000011920928955078125", p.Options));
        }

        [TestMethod]
        public void MavFtpDownloadTargetsCameraAndRejectsOversizedFilesBeforeReading()
        {
            foreach (uint system in new uint[] { 7, uint.MaxValue })
            foreach (int failure in new[] { 0, 1, 2, 3, 4 })
            foreach (byte ftpSession in new byte[] { 0, 42 })
            using (var link = new MAVLinkInterface())
            using (var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
            {
                var wire = new CommsInjection();
                bool oversized = failure == 1, noSessions = failure == 2, cancelOnOpen = failure == 3, loseFirstAck = failure == 4;
                link.BaseStream = wire;
                link.MAVlist[system, 100].mavlinkv2 = true;
                var data = Encoding.UTF8.GetBytes(Xml);
                int reads = 0, wrongTargets = 0, resets = 0, wrongSessions = 0, opens = 0, terminates = 0;
                if (cancelOnOpen)
                    link.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL, message =>
                    {
                        var reply = ((MAVLink.mavlink_file_transfer_protocol_t)message.data).payload;
                        if (reply[3] == 128 && reply[5] == 4) cancel.Cancel();
                        return true;
                    }, system, 100);
                wire.WriteCallback += (sender, bytes) =>
                {
                    var message = new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() };
                    if (message.GetTargetSystem() != system || message.GetTargetComponent() != 100) wrongTargets++;
                    var payload = ((MAVLink.mavlink_file_transfer_protocol_t)message.data).payload;
                    byte opcode = payload[3];
                    if (opcode == 2) resets++;
                    if (opcode == 1) terminates++;
                    if ((opcode == 1 || opcode == 5) && payload[2] != ftpSession) wrongSessions++;
                    // Construct replies independently of MAVFtp's payload conversion helper.
                    var response = new byte[251];
                    BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(payload, 0) + 1)).CopyTo(response, 0);
                    response[3] = 128; // ACK
                    response[2] = ftpSession;
                    response[5] = opcode;
                    if (opcode == 4) // OpenFileRO
                    {
                        opens++;
                        if (loseFirstAck && opens == 1) return;
                        if (noSessions)
                        {
                            response[3] = 129; // NAK: no free sessions
                            response[4] = 1;
                            response[12] = 5;
                        }
                        else
                        {
                            response[4] = 4;
                            BitConverter.GetBytes(oversized ? CameraDefinition.MaxSize + 1 : data.Length).CopyTo(response, 12);
                        }
                    }
                    else if (opcode == 5) // ReadFile
                    {
                        reads++;
                        int offset = (int)BitConverter.ToUInt32(payload, 8);
                        int count = Math.Min(payload[4], data.Length - offset);
                        response[4] = (byte)count;
                        BitConverter.GetBytes(offset).CopyTo(response, 8);
                        Array.Copy(data, offset, response, 12, count);
                    }
                    wire.AppendBuffer(new MAVLink.MavlinkParse().GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL,
                        new MAVLink.mavlink_file_transfer_protocol_t { payload = response }, false, system, 100));
                };
                var load = Task.Run(() => CameraDefinitionLoader.Load("mftp://[;comp=100]/camera.xml", link.MAVlist[system, 100], cancel));
                var deadline = DateTime.UtcNow.AddSeconds(10);
                while (!load.IsCompleted && DateTime.UtcNow < deadline)
                {
                    if (wire.BytesToRead > 0) link.readPacketAsync().GetAwaiter().GetResult();
                    else Thread.Sleep(1);
                }
                Assert.AreEqual(0, wrongTargets, "FTP open/read/terminate must all target the advertised component");
                Assert.AreEqual(0, wrongSessions, "Read and terminate must use the camera's allocated session");
                Assert.AreEqual(0, resets, "Loading a definition must not close other tools' sessions");
                Assert.IsTrue(load.IsCompleted, "Definition transfer exceeded the test deadline");
                Assert.AreEqual(noSessions ? 0 : 1, terminates, "Every identified session must be closed, including cancelled opens");
                if (cancelOnOpen)
                {
                    Assert.ThrowsException<OperationCanceledException>(() => load.GetAwaiter().GetResult());
                    Assert.AreEqual(0, reads);
                }
                else if (noSessions)
                {
                    Assert.ThrowsException<IOException>(() => load.GetAwaiter().GetResult());
                    Assert.AreEqual(0, reads);
                }
                else if (oversized)
                {
                    Assert.ThrowsException<InvalidDataException>(() => load.GetAwaiter().GetResult());
                    Assert.AreEqual(0, reads);
                }
                else
                {
                    Assert.AreEqual("Test Camera", load.GetAwaiter().GetResult().Model);
                    Assert.IsTrue(reads > 0);
                    if (loseFirstAck) Assert.AreEqual(2, opens, "A lost open ACK must be retried");
                }
            }
        }
    }
}
