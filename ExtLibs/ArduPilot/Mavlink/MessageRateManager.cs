using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using static MAVLink;

namespace MissionPlanner.ArduPilot.Mavlink
{
    /// <summary>
    /// Represents a disposable lease on a MAVLink message rate.
    /// </summary>
    public sealed class MessageRateLease : IDisposable
    {
        internal readonly uint MsgId;
        internal readonly byte SysId;
        internal readonly byte CompId;
        internal readonly double Hz;
        internal readonly string Owner;
        internal int Released;

        private readonly MessageRateManager _manager;

        internal MessageRateLease(MessageRateManager manager, uint msgId, byte sysid,
            byte compid, double hz, string owner)
        {
            _manager = manager;
            MsgId = msgId;
            SysId = sysid;
            CompId = compid;
            Hz = hz;
            Owner = owner ?? "";
        }

        public void Dispose()
        {
            _manager.Release(this);
        }
    }

    /// <summary>
    /// Provides lease-based management of per-message MAVLink streaming rates.
    /// </summary>
    /// <remarks>
    /// One instance per <see cref="MAVLinkInterface"/>. The fastest active lease wins.
    /// When all leases for a message are released, SET_MESSAGE_INTERVAL(0) restores the
    /// autopilot's default rate.
    /// </remarks>
    public class MessageRateManager : IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(
            MethodBase.GetCurrentMethod().DeclaringType);

        private readonly MAVLinkInterface _port;
        private readonly object _lock = new object();

        private readonly Dictionary<(uint msgId, byte sysid, byte compid), List<MessageRateLease>> _leases
            = new Dictionary<(uint, byte, byte), List<MessageRateLease>>();

        private readonly HashSet<(uint msgId, byte sysid, byte compid)> _nackedMessages
            = new HashSet<(uint, byte, byte)>();

        // Messages awaiting a confirmed SET_MESSAGE_INTERVAL(0) to restore default rate.
        // Added on last-lease release, removed once the SET-0 is ACKed.
        private readonly HashSet<(uint msgId, byte sysid, byte compid)> _pendingRestores
            = new HashSet<(uint, byte, byte)>();

        // Persistent MESSAGE_INTERVAL subscriptions per target, for detecting
        // unsupported messages (interval_us == 0) after fire-and-forget GETs.
        private readonly Dictionary<(byte sysid, byte compid), int> _intervalSubs
            = new Dictionary<(byte, byte), int>();

        // Per-message packet counting for accurate rate measurement.
        // SubscribeToPacketType increments the counter; Tick snapshots it.
        private readonly Dictionary<(uint msgId, byte sysid, byte compid), long> _packetCounts
            = new Dictionary<(uint, byte, byte), long>();
        private readonly Dictionary<(uint msgId, byte sysid, byte compid), int> _packetSubs
            = new Dictionary<(uint, byte, byte), int>();
        private readonly Dictionary<(uint msgId, byte sysid, byte compid), (long count, long ticks)> _tickSnapshots
            = new Dictionary<(uint, byte, byte), (long, long)>();

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private Task _loop;
        private bool _drainingRestores;
        private int _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRateManager"/> class.
        /// </summary>
        /// <param name="port">The MAVLink interface to manage rates on.</param>
        public MessageRateManager(MAVLinkInterface port)
        {
            _port = port;
        }

        /// <summary>
        /// Requests a message at a minimum rate.
        /// </summary>
        /// <param name="sysid">Target system ID.</param>
        /// <param name="compid">Target component ID.</param>
        /// <param name="msgId">MAVLink message ID.</param>
        /// <param name="hz">Desired minimum rate in Hz.</param>
        /// <param name="owner">Diagnostic label for logging.</param>
        /// <returns>A lease that must be disposed when the rate is no longer needed.</returns>
        public MessageRateLease Subscribe(byte sysid, byte compid,
            MAVLINK_MSG_ID msgId, double hz, string owner = null)
        {
            var id = (uint)msgId;
            if (id > ushort.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(msgId),
                    $"Message ID {id} exceeds MESSAGE_INTERVAL's uint16 range");

            var lease = new MessageRateLease(this, id, sysid, compid, hz, owner);

            if (sysid == 0 || compid == 0)
                return lease;

            var key = (id, sysid, compid);

            int? desiredUs;

            lock (_lock)
            {
                if (_nackedMessages.Contains(key))
                {
                    log.InfoFormat("RateManager: {0} msg {1} ({2},{3}) previously NACKed, skipping",
                        owner ?? "", id, sysid, compid);
                    return lease;
                }

                if (!_leases.TryGetValue(key, out var list))
                {
                    list = new List<MessageRateLease>();
                    _leases[key] = list;
                }

                _pendingRestores.Remove(key);
                list.Add(lease);
                desiredUs = FastestLeaseIntervalUs(key);
                EnsurePacketCounter(id, sysid, compid);
            }

            EnsureSubMessageInterval(sysid, compid);
            if (desiredUs.HasValue)
                FireAndForgetSet(id, sysid, compid, desiredUs.Value);

            // If the message has never arrived, GET to learn whether the AP
            // supports it (the MESSAGE_INTERVAL callback handles interval_us == 0).
            if (!HasEverReceived(id, sysid, compid))
                FireAndForgetGet(id, sysid, compid);

            log.InfoFormat("RateManager: {0} subscribed msg {1} ({2},{3}) at {4:F1} Hz",
                owner ?? "", id, sysid, compid, hz);

            EnsureLoopStarted();
            return lease;
        }

        /// <summary>
        /// Releases a lease and adjusts the active rate accordingly.
        /// </summary>
        internal void Release(MessageRateLease lease)
        {
            if (Interlocked.CompareExchange(ref lease.Released, 1, 0) != 0)
                return;

            if (lease.SysId == 0 || lease.CompId == 0)
                return;

            var key = (lease.MsgId, lease.SysId, lease.CompId);
            int? newDesiredUs;

            lock (_lock)
            {
                if (!_leases.TryGetValue(key, out var list))
                    return;

                list.Remove(lease);

                if (list.Count == 0)
                {
                    _leases.Remove(key);
                    RemovePacketCounter(lease.MsgId, lease.SysId, lease.CompId);
                    newDesiredUs = 0;
                }
                else
                {
                    newDesiredUs = FastestLeaseIntervalUs(key);
                }
            }

            if (newDesiredUs == 0)
            {
                lock (_lock)
                {
                    _pendingRestores.Add(key);
                }
                // Kick off an attempt to restore.
                _ = RetryPendingRestoresAsync();
            }
            else if (newDesiredUs.HasValue)
            {
                FireAndForgetSet(lease.MsgId, lease.SysId, lease.CompId, newDesiredUs.Value);
            }

            log.InfoFormat("RateManager: {0} released msg {1} ({2},{3}){4}",
                lease.Owner, lease.MsgId, lease.SysId, lease.CompId,
                newDesiredUs == 0 ? " -- restoring default" : "");

            TryRemoveSubMessageInterval(lease.SysId, lease.CompId);
        }

        /// <summary>
        /// Resets cached state and re-sends rates for all active leases.
        /// </summary>
        /// <remarks>
        /// Call after connection negotiation completes.
        /// </remarks>
        public void OnConnectionOpen()
        {
            List<(uint msgId, byte sysid, byte compid)> keys;

            lock (_lock)
            {
                _nackedMessages.Clear();
                _pendingRestores.Clear();
                _tickSnapshots.Clear();
                keys = _leases.Keys.ToList();
            }

            foreach (var key in keys)
            {
                EnsureSubMessageInterval(key.sysid, key.compid);

                int? desiredUs;
                lock (_lock)
                {
                    desiredUs = FastestLeaseIntervalUs(key);
                }
                if (desiredUs.HasValue)
                    FireAndForgetSet(key.msgId, key.sysid, key.compid, desiredUs.Value);
            }

            EnsureLoopStarted();
        }

        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _disposed, 1, 0) != 0)
                return;

            _cts.Cancel();

            lock (_lock)
            {
                foreach (var sub in _intervalSubs.Values)
                {
                    try { _port.UnSubscribeToPacketType(sub); }
                    catch { }
                }
                _intervalSubs.Clear();

                foreach (var sub in _packetSubs.Values)
                {
                    try { _port.UnSubscribeToPacketType(sub); }
                    catch { }
                }
                _packetSubs.Clear();
                _packetCounts.Clear();
                _tickSnapshots.Clear();
            }

            _cts.Dispose();
        }

        // --- Tick loop ---

        private async Task RunLoopAsync()
        {
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(30_000, _cts.Token).ConfigureAwait(false);
                    Tick();
                    await RetryPendingRestoresAsync().ConfigureAwait(false);
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    log.Error("RateManager: tick failed", ex);
                }
            }
        }

        private void EnsureLoopStarted()
        {
            lock (_lock)
            {
                if (_loop == null || _loop.IsCompleted)
                    _loop = Task.Run(() => RunLoopAsync());
            }
        }

        /// <summary>
        /// Re-sends SET_MESSAGE_INTERVAL for any tracked message whose observed rate is too low.
        /// </summary>
        private void Tick()
        {
            List<(uint msgId, byte sysid, byte compid)> keys;
            lock (_lock)
            {
                keys = _leases.Keys.ToList();
            }

            foreach (var key in keys)
            {
                if (_cts.IsCancellationRequested)
                    return;

                try
                {
                    int? desiredUs;
                    lock (_lock)
                    {
                        if (_nackedMessages.Contains(key))
                            continue;
                        desiredUs = FastestLeaseIntervalUs(key);
                    }
                    if (!desiredUs.HasValue)
                        continue;

                    bool satisfied = IsRateSatisfied(key, desiredUs.Value);

                    // Reset snapshot so next tick measures a fresh window.
                    lock (_lock)
                    {
                        if (_packetCounts.TryGetValue(key, out var count))
                            _tickSnapshots[key] = (count, Stopwatch.GetTimestamp());
                    }

                    if (satisfied)
                        continue;

                    FireAndForgetSet(key.msgId, key.sysid, key.compid, desiredUs.Value);

                    // If the message has never arrived, retry the GET -- the initial
                    // response may have been lost.
                    if (!HasEverReceived(key.msgId, key.sysid, key.compid))
                        FireAndForgetGet(key.msgId, key.sysid, key.compid);
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("RateManager: tick failed for msg {0} ({1},{2})",
                        key.msgId, key.sysid, key.compid), ex);
                }
            }
        }

        /// <summary>
        /// Determines whether the observed packet rate meets the desired rate.
        /// </summary>
        /// <remarks>
        /// Uses count-based measurement and scales observed rate by
        /// <c>1 / linkqualitygcs</c> to estimate the autopilot's actual send rate.
        /// </remarks>
        private bool IsRateSatisfied((uint msgId, byte sysid, byte compid) key, int desiredUs)
        {
            long currentCount;
            (long count, long ticks) snapshot;

            lock (_lock)
            {
                if (!_packetCounts.TryGetValue(key, out currentCount))
                    return false;

                if (!_tickSnapshots.TryGetValue(key, out snapshot))
                    return false; // Should not happen -- created in EnsurePacketCounter.
            }

            double elapsed = (double)(Stopwatch.GetTimestamp() - snapshot.ticks) / Stopwatch.Frequency;
            if (elapsed < 1.0)
                return true; // Too soon to judge; don't re-send.

            long delta = currentCount - snapshot.count;
            if (delta <= 0)
                return false; // No packets at all since last snapshot.

            double observedHz = delta / elapsed;
            double desiredHz = IntervalUsToHz(desiredUs);

            // Compensate for link quality: if we're only receiving 80% of packets,
            // the autopilot is likely sending 1/0.8 = 1.25x what we see.
            // Cap the multiplier at 2x so bad LQ doesn't make us accept anything.
            double lqFraction = GetLinkQuality(key.sysid, key.compid);
            double lqMultiplier = lqFraction > 0.5 ? 1.0 / lqFraction : 2.0;
            double estimatedSendHz = observedHz * lqMultiplier;

            return estimatedSendHz >= desiredHz - 1.0;
        }

        /// <summary>
        /// Returns link quality as a fraction (0.0-1.0) for the given target.
        /// </summary>
        /// <remarks>
        /// Falls back to 1.0 (perfect) if unavailable.
        /// </remarks>
        private double GetLinkQuality(byte sysid, byte compid)
        {
            try
            {
                int lq = _port.MAVlist[sysid, compid].cs.linkqualitygcs;
                return lq > 0 ? lq / 100.0 : 1.0;
            }
            catch { return 1.0; }
        }

        private bool HasEverReceived(uint msgId, byte sysid, byte compid)
        {
            try
            {
                return _port.MAVlist[sysid, compid].packetspersecondbuild.ContainsKey(msgId);
            }
            catch { return false; }
        }

        // --- MESSAGE_INTERVAL subscription ---

        /// <summary>
        /// Subscribes to MESSAGE_INTERVAL from the specified target if not already subscribed.
        /// </summary>
        private void EnsureSubMessageInterval(byte sysid, byte compid)
        {
            var targetKey = (sysid, compid);

            lock (_lock)
            {
                if (_intervalSubs.ContainsKey(targetKey))
                    return;

                byte s = sysid, c = compid;
                var sub = _port.SubscribeToPacketType(
                    MAVLINK_MSG_ID.MESSAGE_INTERVAL,
                    msg =>
                    {
                        var data = msg.ToStructure<mavlink_message_interval_t>();
                        OnMessageInterval(data.message_id, s, c, data.interval_us);
                        return true;
                    },
                    sysid, compid);

                _intervalSubs[targetKey] = sub;
            }
        }

        /// <summary>
        /// Removes the MESSAGE_INTERVAL subscription if no leases remain for this target.
        /// </summary>
        private void TryRemoveSubMessageInterval(byte sysid, byte compid)
        {
            var targetKey = (sysid, compid);

            lock (_lock)
            {
                bool hasLeases = _leases.Keys.Any(k => k.sysid == sysid && k.compid == compid);
                if (hasLeases)
                    return;

                if (!_intervalSubs.TryGetValue(targetKey, out var sub))
                    return;

                _intervalSubs.Remove(targetKey);
                try { _port.UnSubscribeToPacketType(sub); }
                catch { }
            }
        }

        /// <summary>
        /// Marks a message as unsupported when a MESSAGE_INTERVAL response reports zero.
        /// </summary>
        private void OnMessageInterval(ushort messageId, byte sysid, byte compid, int intervalUs)
        {
            if (intervalUs != 0)
                return;

            var key = ((uint)messageId, sysid, compid);

            lock (_lock)
            {
                if (!_leases.ContainsKey(key))
                    return;

                _nackedMessages.Add(key);
                _leases.Remove(key);
                RemovePacketCounter((uint)messageId, sysid, compid);
            }

            log.WarnFormat("RateManager: msg {0} ({1},{2}) not available (interval_us=0)",
                messageId, sysid, compid);

            TryRemoveSubMessageInterval(sysid, compid);
        }

        // --- Packet counting ---

        /// <summary>
        /// Subscribes to a message type for counting if not already subscribed.
        /// </summary>
        /// <remarks>
        /// Caller must hold <see cref="_lock"/>. Calls into
        /// <see cref="MAVLinkInterface.SubscribeToPacketType"/> under lock --
        /// safe because that method does not call back into this class.
        /// </remarks>
        private void EnsurePacketCounter(uint msgId, byte sysid, byte compid)
        {
            var key = (msgId, sysid, compid);
            if (_packetSubs.ContainsKey(key))
                return;

            _packetCounts[key] = 0;
            _tickSnapshots[key] = (0, Stopwatch.GetTimestamp());
            var sub = _port.SubscribeToPacketType(
                (MAVLINK_MSG_ID)msgId,
                msg =>
                {
                    lock (_lock) { _packetCounts[key]++; }
                    return true;
                },
                sysid, compid);
            _packetSubs[key] = sub;
        }

        /// <summary>
        /// Removes the packet counter subscription for a message.
        /// </summary>
        /// <remarks>
        /// Caller must hold <see cref="_lock"/>. Calls into
        /// <see cref="MAVLinkInterface.UnSubscribeToPacketType"/> under lock --
        /// safe because that method does not call back into this class.
        /// </remarks>
        private void RemovePacketCounter(uint msgId, byte sysid, byte compid)
        {
            var key = (msgId, sysid, compid);
            if (!_packetSubs.TryGetValue(key, out var sub))
                return;

            _packetSubs.Remove(key);
            _packetCounts.Remove(key);
            _tickSnapshots.Remove(key);
            try { _port.UnSubscribeToPacketType(sub); }
            catch { }
        }

        // --- Command helpers ---

        /// <summary>
        /// Sends SET_MESSAGE_INTERVAL(0) with acknowledgment and removes the pending restore on success.
        /// </summary>
        private async Task RestoreDefaultAsync(uint msgId, byte sysid, byte compid)
        {
            var key = (msgId, sysid, compid);
            // Wait briefly for any in-progress requireAck exchange to finish
            // so we don't fight over COMMAND_ACK packets.
            for (int i = 0; i < 5 && _port.giveComport; i++)
                await Task.Delay(1000, _cts.Token).ConfigureAwait(false);
            if (_port.giveComport)
                return;
            try
            {
                bool acked = await _port.doCommandAsync(sysid, compid,
                    MAV_CMD.SET_MESSAGE_INTERVAL,
                    (float)msgId, 0,
                    0, 0, 0, 0, 0,
                    true).ConfigureAwait(false);

                if (acked)
                {
                    lock (_lock) { _pendingRestores.Remove(key); }
                    log.InfoFormat("RateManager: msg {0} ({1},{2}) default restored (ACKed)",
                        msgId, sysid, compid);
                }
            }
            catch (Exception ex)
            {
                log.WarnFormat("RateManager: msg {0} ({1},{2}) default restore failed: {3}",
                    msgId, sysid, compid, ex.Message);
            }
        }

        /// <summary>
        /// Retries any pending default-rate restores that have not yet been acknowledged.
        /// </summary>
        private async Task RetryPendingRestoresAsync()
        {
            lock (_lock)
            {
                if (_pendingRestores.Count == 0)
                    return;
                // Prevent concurrent runs, since we can't tell which ACK corresponds to which message.
                if (_drainingRestores)
                    return;
                _drainingRestores = true;
            }

            try
            {
                List<(uint msgId, byte sysid, byte compid)> pending;
                lock (_lock)
                {
                    pending = _pendingRestores.ToList();
                }

                foreach (var key in pending)
                {
                    if (_cts.IsCancellationRequested)
                        return;

                    await RestoreDefaultAsync(key.msgId, key.sysid, key.compid)
                        .ConfigureAwait(false);
                }
            }
            finally
            {
                lock (_lock) { _drainingRestores = false; }
            }
        }

        // --- Fire-and-forget command helpers ---

        private void FireAndForgetSet(uint msgId, byte sysid, byte compid, int intervalUs)
        {
            // Reset the snapshot so IsRateSatisfied measures from now, not from
            // before the rate change.
            var key = (msgId, sysid, compid);
            lock (_lock)
            {
                if (_packetCounts.TryGetValue(key, out var count))
                    _tickSnapshots[key] = (count, Stopwatch.GetTimestamp());
            }

            try
            {
                _port.doCommandAsync(sysid, compid,
                    MAV_CMD.SET_MESSAGE_INTERVAL,
                    (float)msgId, intervalUs,
                    0, 0, 0, 0, 0,
                    false)
                    .ContinueWith(t => log.Debug("RateManager: fire-and-forget SET failed: "
                        + t.Exception?.InnerException?.Message), TaskContinuationOptions.OnlyOnFaulted);
            }
            catch (Exception ex)
            {
                log.Debug("RateManager: fire-and-forget SET failed: " + ex.Message);
            }
        }

        private void FireAndForgetGet(uint msgId, byte sysid, byte compid)
        {
            try
            {
                _port.doCommandAsync(sysid, compid,
                    MAV_CMD.GET_MESSAGE_INTERVAL,
                    (float)msgId,
                    0, 0, 0, 0, 0, 0,
                    false)
                    .ContinueWith(t => log.Debug("RateManager: fire-and-forget GET failed: "
                        + t.Exception?.InnerException?.Message), TaskContinuationOptions.OnlyOnFaulted);
            }
            catch (Exception ex)
            {
                log.Debug("RateManager: fire-and-forget GET failed: " + ex.Message);
            }
        }

        // --- Helpers ---

        /// <summary>
        /// Returns the interval in microseconds of the fastest active lease for a key.
        /// </summary>
        /// <remarks>Caller must hold <see cref="_lock"/>.</remarks>
        private int? FastestLeaseIntervalUs((uint, byte, byte) key)
        {
            if (!_leases.TryGetValue(key, out var list) || list.Count == 0)
                return null;

            int min = int.MaxValue;
            foreach (var lease in list)
            {
                int us = HzToIntervalUs(lease.Hz);
                if (us > 0 && us < min)
                    min = us;
            }
            return min == int.MaxValue ? (int?)null : min;
        }

        private static int HzToIntervalUs(double hz)
        {
            return hz > 0 ? (int)(1e6 / hz) : 0;
        }

        private static double IntervalUsToHz(int intervalUs)
        {
            return intervalUs > 0 ? 1e6 / intervalUs : 0;
        }
    }
}
