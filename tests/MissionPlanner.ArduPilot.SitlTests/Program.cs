using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MissionPlanner;
using MissionPlanner.Comms;

class Program
{
    // args[0]: host, args[1]: port, args[2]: oracle file path (WSL log via \\wsl$),
    // args[3]: "cancel" to also run the mid-download cancel check
    static async Task<int> Main(string[] args)
    {
        var host = args.Length > 0 ? args[0] : "127.0.0.1";
        var port = args.Length > 1 ? args[1] : "5760";
        var oracle = args.Length > 2 ? args[2] : null;
        var doCancel = args.Contains("cancel");

        var tcp = new TcpSerial { Host = host, Port = port, autoReconnect = false, retrys = 1 };
        tcp.Open();
        Console.WriteLine($"[harness] connected to {host}:{port}");

        var mav = new MAVLinkInterface();
        mav.BaseStream = tcp;

        var stop = new CancellationTokenSource();
        var pump = Task.Run(async () =>
        {
            while (!stop.IsCancellationRequested)
            {
                try
                {
                    // GetLogEntry reads packets itself - stand down like MainV2.SerialReader does
                    if (mav.giveComport)
                    {
                        await Task.Delay(5).ConfigureAwait(false);
                        continue;
                    }

                    await mav.readPacketAsync().ConfigureAwait(false);
                }
                catch
                {
                    await Task.Delay(5).ConfigureAwait(false);
                }
            }
        });

        // wait for a heartbeat so sysid/compid are known
        var deadline = DateTime.UtcNow.AddSeconds(20);
        while (mav.MAV.sysid == 0 && DateTime.UtcNow < deadline)
            await Task.Delay(100);
        if (mav.MAV.sysid == 0)
        {
            Console.WriteLine("[harness] FAIL no heartbeat within 20s");
            return 1;
        }

        Console.WriteLine($"[harness] heartbeat from sysid={mav.MAV.sysid} compid={mav.MAV.compid}");

        long packets = 0;
        mav.OnPacketReceived += (s, m) => Interlocked.Increment(ref packets);

        // let SITL finish sensor init before asking for the log list
        await Task.Delay(2000);

        // --- list logs ---
        System.Collections.Generic.List<MAVLink.mavlink_log_entry_t> entries = null;
        for (int attempt = 1; attempt <= 3; attempt++)
        {
            Console.WriteLine($"[harness] log list attempt {attempt}, packets seen so far: {Interlocked.Read(ref packets)}");
            try
            {
                entries = (await mav.GetLogEntry()).Values.OrderBy(a => a.id).ToList();
                break;
            }
            catch (TimeoutException) when (attempt < 3)
            {
                Console.WriteLine("[harness] log list timed out, retrying");
                await Task.Delay(1000);
            }
        }
        Console.WriteLine($"[harness] log list: {entries.Count} entries");
        foreach (var e in entries)
            Console.WriteLine($"[harness]   id={e.id} size={e.size} utc={e.time_utc}");
        if (entries.Count == 0)
        {
            Console.WriteLine("[harness] FAIL no logs on vehicle");
            return 1;
        }

        var entry = entries.First();

        // --- download ---
        var sw = Stopwatch.StartNew();
        var file = await mav.GetLog(mav.MAV.sysid, mav.MAV.compid, entry.id);
        sw.Stop();
        var bytes = File.ReadAllBytes(file);
        File.Delete(file);
        Console.WriteLine($"[harness] downloaded {bytes.Length} bytes in {sw.Elapsed.TotalSeconds:F2}s " +
                          $"({bytes.Length / Math.Max(sw.Elapsed.TotalSeconds, 0.001) / 1024:F0} KiB/s)");

        if (entry.size != 0 && bytes.Length != entry.size)
        {
            Console.WriteLine($"[harness] FAIL size mismatch: LOG_ENTRY said {entry.size}, got {bytes.Length}");
            return 1;
        }

        if (oracle != null)
        {
            var expected = File.ReadAllBytes(oracle);
            if (expected.Length != bytes.Length)
            {
                Console.WriteLine($"[harness] FAIL oracle length {expected.Length} != downloaded {bytes.Length}");
                return 1;
            }

            for (int i = 0; i < expected.Length; i++)
            {
                if (expected[i] != bytes[i])
                {
                    Console.WriteLine($"[harness] FAIL first byte difference at offset {i}");
                    return 1;
                }
            }

            Console.WriteLine($"[harness] PASS oracle byte-for-byte match ({expected.Length} bytes)");
        }

        if (doCancel)
        {
            using (var cts = new CancellationTokenSource(400))
            {
                try
                {
                    await mav.GetLog(mav.MAV.sysid, mav.MAV.compid, entry.id, cts.Token);
                    Console.WriteLine("[harness] FAIL cancel: download completed anyway");
                    return 1;
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("[harness] PASS cancel raised OperationCanceledException");
                }
            }

            // link must still be usable afterwards: list logs again.
            // a couple of LOG_DATA packets may still be in flight right after the
            // cancel, so allow a short settle + one retry
            await Task.Delay(500);
            int again = -1;
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                try
                {
                    again = (await mav.GetLogEntry()).Values.Count;
                    break;
                }
                catch (Exception ex) when (attempt < 3)
                {
                    Console.WriteLine($"[harness] post-cancel list attempt {attempt} failed: {ex.Message}");
                    await Task.Delay(1000);
                }
            }

            if (again < 0)
            {
                Console.WriteLine("[harness] FAIL link unusable after cancel");
                return 1;
            }

            Console.WriteLine($"[harness] PASS link usable after cancel (log list: {again} entries)");
        }

        stop.Cancel();
        tcp.Close();
        Console.WriteLine("[harness] DONE");
        return 0;
    }
}
