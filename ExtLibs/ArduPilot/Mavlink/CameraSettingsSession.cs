using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.ArduPilot.Mavlink
{
    /// <summary>Single-threaded PARAM_EXT transaction state, driven by the settings window.</summary>
    public sealed class CameraSettingsSession
    {
        private sealed class Request
        {
            public DateTime Deadline, Expires;
            public int Attempts;
            public object Value;
            public bool InProgress;
        }
        public CameraDefinition Definition { get; }
        public Dictionary<string, object> Values { get; } = new Dictionary<string, object>();
        public Dictionary<string, string> Errors { get; } = new Dictionary<string, string>();
        public string Status { get; private set; } = "Reading camera settings";
        private readonly Dictionary<string, Request> reads = new Dictionary<string, Request>();
        private readonly Dictionary<string, Request> writes = new Dictionary<string, Request>();
        private readonly HashSet<string> writeErrors = new HashSet<string>();
        private readonly Action<object> send;
        private readonly byte system, component;
        private DateTime nextBatch, lastPoll;
        private static readonly TimeSpan Retry = TimeSpan.FromSeconds(2);

        public CameraSettingsSession(CameraDefinition definition, byte system, byte component, Action<object> send)
        {
            Definition = definition;
            this.system = system;
            this.component = component;
            this.send = send;
        }

        public bool IsPending(string name) => writes.ContainsKey(name);
        public bool CanEdit(string name, IDictionary<string, List<CameraParameter.Option>> controls = null) => !Definition.Parameters[name].ReadOnly && !IsPending(name)
            && (Values.ContainsKey(name) || Definition.Parameters[name].WriteOnly) && (controls ?? Definition.Controls(Values)).ContainsKey(name);

        public void RequestAll(DateTime now)
        {
            reads.Clear();
            Errors.Clear();
            writeErrors.Clear();
            foreach (var p in Definition.Parameters.Values.Where(p => !p.WriteOnly))
                reads[p.Name] = new Request { Deadline = now + Retry, Attempts = 1 };
            lastPoll = now;
            send(new MAVLink.mavlink_param_ext_request_list_t(system, component));
        }

        private void Read(string name, DateTime now)
        {
            if (Definition.Parameters.TryGetValue(name, out var p) && !p.WriteOnly && !writes.ContainsKey(name) && !reads.ContainsKey(name))
                reads[name] = new Request { Deadline = now };
        }

        public void SetValue(string name, string text, DateTime now)
        {
            if (!CanEdit(name)) throw new ArgumentException("Setting is not currently editable");
            var p = Definition.Parameters[name];
            var value = p.Validate(text, Definition.Controls(Values)[name]);
            reads.Remove(name);
            Errors.Remove(name);
            writeErrors.Remove(name);
            writes[name] = new Request { Value = value, Attempts = 1, Deadline = now + Retry, Expires = now.AddSeconds(30) };
            Status = "Applying " + p.Description;
            SendValue(name);
        }

        private void SendValue(string name)
        {
            var p = Definition.Parameters[name];
            send(new MAVLink.mavlink_param_ext_set_t(system, component, p.Id, p.Encode(writes[name].Value), p.WireType));
        }

        private void Received(string name, object value, DateTime now, bool update)
        {
            bool changed = Values.TryGetValue(name, out var old) && !(old is byte[] oldBytes && value is byte[] newBytes
                ? oldBytes.SequenceEqual(newBytes) : Equals(old, value));
            Values[name] = value;
            reads.Remove(name);
            if (!writeErrors.Contains(name)) Errors.Remove(name);
            else if (Errors.TryGetValue(name, out var error))
                Errors[name] = error.Replace("camera state is unconfirmed", "current camera value has been read back");
            if (update || changed)
                foreach (var target in Definition.Parameters[name].Updates) Read(target, now.AddMilliseconds(500));
        }

        public void Receive(MAVLink.MAVLINK_MSG_ID id, object message, DateTime now)
        {
            string name;
            byte[] raw;
            byte type;
            byte result = 0;
            bool ack = id == MAVLink.MAVLINK_MSG_ID.PARAM_EXT_ACK;
            if (ack)
            {
                var m = (MAVLink.mavlink_param_ext_ack_t)message;
                name = Encoding.ASCII.GetString(m.param_id).TrimEnd('\0');
                raw = m.param_value; type = m.param_type; result = m.param_result;
            }
            else if (id == MAVLink.MAVLINK_MSG_ID.PARAM_EXT_VALUE)
            {
                var m = (MAVLink.mavlink_param_ext_value_t)message;
                name = Encoding.ASCII.GetString(m.param_id).TrimEnd('\0');
                raw = m.param_value; type = m.param_type;
            }
            else return;
            if (!Definition.Parameters.TryGetValue(name, out var p)) return;
            if (type != p.WireType)
            {
                Errors[name] = "Camera returned a different parameter type";
                return;
            }
            writes.TryGetValue(name, out var pending);
            if (!ack && pending != null) return; // A read response cannot confirm a write.
            if (ack && result == (byte)MAVLink.PARAM_ACK.IN_PROGRESS)
            {
                if (pending != null)
                {
                    pending.InProgress = true;
                    pending.Deadline = pending.Expires;
                }
                return;
            }
            if (ack && result != (byte)MAVLink.PARAM_ACK.ACCEPTED)
            {
                if (pending == null) return;
                writes.Remove(name);
                Values.Remove(name);
                writeErrors.Add(name);
                Errors[name] = "Camera rejected setting: " + (MAVLink.PARAM_ACK)result;
                Status = p.Description + ": " + Errors[name];
                Read(name, now);
                return;
            }
            object value;
            try { value = p.Decode(raw, type); }
            catch (ArgumentException ex)
            {
                Errors[name] = ex.Message;
                return;
            }
            bool adjusted = pending != null && !Equals(value, pending.Value);
            writes.Remove(name);
            if (ack) Errors.Remove(name);
            Received(name, value, now, ack);
            // Cameras can quantize a setting. Display their acknowledged value and confirm it by reading.
            if (adjusted) Read(name, now);
            if (reads.Count == 0 && writes.Count == 0)
                Status = Errors.Count > 0 ? "Some settings are unavailable or could not be applied" : "Camera settings up to date";
        }

        public void Tick(DateTime now)
        {
            int sent = 0;
            foreach (var pair in reads.ToArray())
            {
                if (now < pair.Value.Deadline || now < nextBatch || sent >= 5) continue;
                if (pair.Value.Attempts >= 3)
                {
                    reads.Remove(pair.Key);
                    if (!Errors.ContainsKey(pair.Key)) Errors[pair.Key] = "Camera did not respond";
                    Status = "Some camera settings did not respond";
                    continue;
                }
                send(new MAVLink.mavlink_param_ext_request_read_t(-1, system, component, Definition.Parameters[pair.Key].Id));
                pair.Value.Attempts++;
                pair.Value.Deadline = now + Retry;
                sent++;
            }
            if (sent > 0) nextBatch = now.AddMilliseconds(200);
            foreach (var pair in writes.ToArray())
            {
                var pending = pair.Value;
                if (now < pending.Deadline && now < pending.Expires) continue;
                if (pending.InProgress || pending.Attempts >= 3 || now >= pending.Expires)
                {
                    writes.Remove(pair.Key);
                    Values.Remove(pair.Key);
                    writeErrors.Add(pair.Key);
                    Errors[pair.Key] = "Write timed out; camera state is unconfirmed";
                    Status = pair.Key + ": write timed out";
                    Read(pair.Key, now);
                }
                else
                {
                    SendValue(pair.Key);
                    pending.Attempts++;
                    pending.Deadline = now + Retry;
                }
            }
            if (now - lastPoll > TimeSpan.FromSeconds(5) && reads.Count == 0 && writes.Count == 0)
            {
                foreach (var name in Values.Keys) Read(name, now);
                lastPoll = now;
            }
        }
    }
}
