using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MissionPlanner.ArduPilot.Mavlink
{
    /// <summary>
    /// Rebuilds STATUSTEXT messages that the sender had to split up.
    ///
    /// A STATUSTEXT packet only carries 50 bytes of text. A longer message is sent as a run of
    /// packets that share a non-zero id and are numbered by chunk_seq, the last one being the
    /// chunk with a null in its text. An id of zero is a whole message on its own, which is also
    /// what a MAVLink 1 link delivers as it cannot carry the id and chunk_seq extension fields.
    /// </summary>
    public sealed class StatusTextAssembler
    {
        /// <summary>size of the STATUSTEXT text field, only the last chunk of a message is shorter</summary>
        public const int ChunkSize = 50;

        /// <summary>how long to wait for the rest of a message after its most recent chunk</summary>
        public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(1);

        private static readonly byte[] Gap = Encoding.UTF8.GetBytes(" ... ");

        private sealed class Pending
        {
            public byte Severity;
            public DateTime LastChunk;
            // number of chunks in the message, not known until the last chunk has been seen
            public int Expected = -1;
            public readonly SortedDictionary<byte, byte[]> Chunks = new SortedDictionary<byte, byte[]>();
        }

        private readonly Dictionary<(uint sysid, byte compid, ushort id), Pending> pending =
            new Dictionary<(uint sysid, byte compid, ushort id), Pending>();

        private volatile bool hasPending;

        /// <summary>true while a message is waiting on more chunks, ie TakeExpired needs polling</summary>
        public bool HasPending => hasPending;

        public string Add(uint sysid, byte compid, MAVLink.mavlink_statustext_t msg)
        {
            return Add(sysid, compid, msg, DateTime.UtcNow);
        }

        /// <summary>Add a received STATUSTEXT</summary>
        /// <returns>the whole message once all of it has arrived, otherwise null</returns>
        public string Add(uint sysid, byte compid, MAVLink.mavlink_statustext_t msg, DateTime now)
        {
            byte[] text = TrimAtNull(msg.text);

            // not chunked
            if (msg.id == 0)
                return Encoding.UTF8.GetString(text);

            lock (pending)
            {
                var key = (sysid, compid, msg.id);

                if (!pending.TryGetValue(key, out var item))
                    pending[key] = item = new Pending();

                item.Severity = msg.severity;
                item.LastChunk = now;
                item.Chunks[msg.chunk_seq] = text;

                if (text.Length < ChunkSize)
                    item.Expected = msg.chunk_seq + 1;

                if (item.Chunks.Count != item.Expected)
                {
                    hasPending = true;
                    return null;
                }

                pending.Remove(key);
                hasPending = pending.Count > 0;

                return Join(item);
            }
        }

        public List<(uint sysid, byte compid, byte severity, string text)> TakeExpired()
        {
            return TakeExpired(DateTime.UtcNow);
        }

        /// <summary>
        /// Remove and return the messages that have gone longer than Timeout without a new chunk,
        /// oldest first. A " ... " marks where chunks ahead of one that did arrive are missing.
        /// </summary>
        public List<(uint sysid, byte compid, byte severity, string text)> TakeExpired(DateTime now)
        {
            var expired = new List<(uint sysid, byte compid, byte severity, string text)>();

            lock (pending)
            {
                foreach (var item in pending.Where(a => now - a.Value.LastChunk > Timeout)
                             .OrderBy(a => a.Value.LastChunk).ToList())
                {
                    pending.Remove(item.Key);
                    expired.Add((item.Key.sysid, item.Key.compid, item.Value.Severity, Join(item.Value)));
                }

                hasPending = pending.Count > 0;
            }

            return expired;
        }

        private static string Join(Pending item)
        {
            using (var data = new MemoryStream())
            {
                int next = 0;

                foreach (var chunk in item.Chunks)
                {
                    if (chunk.Key != next)
                        data.Write(Gap, 0, Gap.Length);

                    data.Write(chunk.Value, 0, chunk.Value.Length);
                    next = chunk.Key + 1;
                }

                // decode once joined, a multi-byte character can straddle two chunks
                return Encoding.UTF8.GetString(data.ToArray());
            }
        }

        private static byte[] TrimAtNull(byte[] text)
        {
            if (text == null)
                return new byte[0];

            int length = Array.IndexOf(text, (byte)0);
            if (length == -1)
                length = text.Length;

            var ans = new byte[length];
            Array.Copy(text, ans, length);
            return ans;
        }
    }
}
