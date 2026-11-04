using System.Diagnostics;
using System.Reflection;
using System.Text;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using MissionPlanner.Comms;
using MissionPlanner.ViewModels;
using MissionPlanner.ViewModels.GCSViews.ConfigurationView;
using Xunit.Abstractions;

namespace MissionPlanner.Tests;

/// <summary>
/// Opt-in trace against a real autopilot: MP_LIVE_CAN_PORT, MP_LIVE_CAN_BUS (default 2),
/// MP_LIVE_CAN_NODE (node to inspect, default 125). MP_LIVE_CAN_FLASH=1 installs the catalog build
/// named by MP_LIVE_CAN_CHANNEL (default stable); MP_LIVE_CAN_FILE flashes a local image instead;
/// MP_LIVE_CAN_BURST=1 measures the autopilot's queueing latency. Every CAN frame, MAVLink
/// sequence number, pump rate and status change is written to MP_LIVE_CAN_TRACE (default
/// dronecan-live-trace.log in the temp directory) and to the test output.
/// </summary>
public class DroneCanLiveFirmwareDiagnosticTests(ITestOutputHelper output) {
  [AvaloniaFact]
  public async Task Live_firmware_update_trace() {
    string? port = Environment.GetEnvironmentVariable("MP_LIVE_CAN_PORT");
    if (string.IsNullOrEmpty(port)) {
      output.WriteLine("MP_LIVE_CAN_PORT not set; skipped.");
      return;
    }
    int bus = int.TryParse(Environment.GetEnvironmentVariable("MP_LIVE_CAN_BUS"), out int b) ? b : 2;
    int node = int.TryParse(Environment.GetEnvironmentVariable("MP_LIVE_CAN_NODE"), out int n) ? n : 125;
    bool flash = Environment.GetEnvironmentVariable("MP_LIVE_CAN_FLASH") == "1";
    string? file = Environment.GetEnvironmentVariable("MP_LIVE_CAN_FILE");

    var watch = Stopwatch.StartNew();
    var log = new StringBuilder();
    object logLock = new();
    void Log(string line) {
      lock (logLock) { log.AppendLine($"{watch.Elapsed.TotalSeconds,8:F3} {line}"); }
    }
    var console = new CallbackWriter(line => Log("CON " + line));
    var previousOut = Console.Out;
    Console.SetOut(console);

    var link = new MAVLinkInterface { BaseStream = new SerialPort { PortName = port, BaudRate = 115200 } };
    await Task.Run(() => link.Open(getparams: true, skipconnectedcheck: true, showui: false));
    Assert.True(link.BaseStream.IsOpen, "MAVLink link did not open");
    Log($"Connected {link.sysidcurrent}:{link.compidcurrent}, {link.MAV.param.Count} params, version {link.MAV.VersionString}");
    foreach (var name in new[] { "CAN_P1_DRIVER", "CAN_P2_DRIVER", "CAN_D1_PROTOCOL", "CAN_D2_PROTOCOL", "CAN_D1_UC_NODE", "CAN_D2_UC_NODE", "CAN_D1_UC_OPTION", "CAN_P1_BITRATE", "CAN_P2_BITRATE", "CAN_SLCAN_CPORT" }) {
      if (link.MAV.param.ContainsKey(name)) Log($"  {name} = {link.MAV.param[name].Value}");
    }
    var target = new DroneCanSessionTarget(link, (byte)link.sysidcurrent, (byte)link.compidcurrent);
    using var pump = new CancellationTokenSource();
    long pumpPackets = 0, pumpCan = 0;
    var reader = Task.Run(async () => {
      while (!pump.IsCancellationRequested && link.BaseStream.IsOpen) {
        try {
          var m = await link.readPacketAsync().ConfigureAwait(false);
          if (m != null) { Interlocked.Increment(ref pumpPackets); if (m.msgid == (uint)MAVLink.MAVLINK_MSG_ID.CAN_FRAME) Interlocked.Increment(ref pumpCan); }
        } catch (Exception) when (!pump.IsCancellationRequested) { await Task.Delay(10); }
      }
    });
    var sampler = Task.Run(async () => {
      long lastP = 0, lastC = 0;
      while (!pump.IsCancellationRequested) {
        await Task.Delay(1000);
        long p = Interlocked.Read(ref pumpPackets), c = Interlocked.Read(ref pumpCan);
        int btr = -1; try { btr = link.BaseStream.BytesToRead; } catch { }
        Log($"PUMP packets/s={p - lastP} can/s={c - lastC} bytesToRead={btr}");
        lastP = p; lastC = c;
      }
    });
    try {
      using var viewModel = new ConfigDroneCanViewModel(() => target);
      viewModel.SelectedBusIndex = bus == 2 ? 1 : 0;
      viewModel.ToggleConnectCommand.Execute(null);
      await WaitForAsync(() => viewModel.IsConnected && !viewModel.IsBusy, 10);

      var can = (DroneCAN.DroneCAN)typeof(ConfigDroneCanViewModel).GetField("_can", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(viewModel)!;
      var transport = (MavlinkCanTransport)typeof(ConfigDroneCanViewModel).GetField("_transport", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(viewModel)!;
      int seqSub = link.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.CAN_FRAME, m => {
        var pkt = (MAVLink.mavlink_can_frame_t)m.data;
        Log($"SEQ {m.seq} id={pkt.id:X8} len={pkt.len} {Convert.ToHexString(pkt.data, 0, Math.Min((int)pkt.len, 8))}");
        return false;
      }, target.SystemId, target.ComponentId);
      can.FrameReceived += (frame, payload) => Log($"RX {frame.ToHex()} {payload.ToHex()} src={frame.SourceNode} svc={frame.IsServiceMsg} type={frame.MsgTypeID}" + (frame.IsServiceMsg ? $" dst={frame.SvcDestinationNode}" : ""));
      can.MessageReceived += (frame, msg, tid) => Log($"MSG {msg.GetType().Name} src={frame.SourceNode}" + (frame.IsServiceMsg ? $" dst={frame.SvcDestinationNode}" : "") + $" tid={tid}");
      transport.Port.WriteCallback += (s, bytes) => Log("TX " + Encoding.ASCII.GetString(bytes.ToArray()).Replace("\r", " "));

      await Task.Delay(5000);
      Dispatcher.UIThread.RunJobs();
      foreach (var item in viewModel.Nodes) {
        Log($"NODE {item.Id} {item.Name} sw {item.SoftwareVersion} crc {item.SoftwareCrc} hw {item.HardwareVersion} {item.Health} {item.Mode} up {item.Uptime} vsc {item.VendorSpecificCode}");
      }
      Log("STATUS " + viewModel.Status);
      foreach (var name in new[] { "SCHED_LOOP_RATE", "SERIAL0_BAUD", "SERIAL0_PROTOCOL", "CAN_D1_ESC_BM", "CAN_D1_ESC_OF", "SR0_EXTRA1", "SR0_RAW_SENS" }) {
        if (link.MAV.param.ContainsKey(name)) Log($"  {name} = {link.MAV.param[name].Value}");
      }
      if (Environment.GetEnvironmentVariable("MP_LIVE_CAN_BURST") == "1") {
        // Queueing latency through the autopilot: a burst of N frames to an absent node, then one
        // GetNodeInfo to the real node; the RTT grows with the autopilot's backlog.
        var reply = new System.Threading.ManualResetEventSlim(false);
        can.MessageReceived += (frame, msg, tid) => {
          if (msg is DroneCAN.DroneCAN.uavcan_protocol_GetNodeInfo_res && frame.SourceNode == node) reply.Set();
        };
        foreach (int burst in new[] { 0, 0, 25, 50, 100, 200, 400, 0 }) {
          var req = new DroneCAN.DroneCAN.uavcan_protocol_GetNodeInfo_req();
          var sw = Stopwatch.StartNew();
          for (int i = 0; i < burst; i++) {
            can.WriteToStreamSLCAN(can.PackageMessageSLCAN(126, 30, can.TransferID++, req));
          }
          double sent = sw.Elapsed.TotalMilliseconds;
          reply.Reset();
          can.WriteToStreamSLCAN(can.PackageMessageSLCAN((byte)node, 30, can.TransferID++, req));
          bool got = reply.Wait(15000);
          Log($"BURST n={burst} sent in {sent:F0} ms, GetNodeInfo RTT {(got ? sw.Elapsed.TotalMilliseconds - sent : -1):F0} ms");
          await Task.Delay(4000);
        }
      }
      var device = viewModel.Nodes.FirstOrDefault(item => item.Id == node);
      if (device == null) {
        Log($"node {node} not found");
      } else if (flash || file != null) {
        viewModel.SelectedNode = device;
        string lastStatus = "";
        using var statusPump = new CancellationTokenSource();
        var statusTask = Task.Run(async () => {
          while (!statusPump.IsCancellationRequested) {
            string s = viewModel.NodeStatus;
            if (s != lastStatus) { lastStatus = s; Log("NODESTATUS " + s); }
            await Task.Delay(100);
          }
        });
        Task operation;
        if (file != null) {
          Log("FILE " + file);
          operation = viewModel.UpdateFirmwareAsync(file);
        } else {
          Assert.True(await viewModel.CheckFirmwareUpdatesAsync(), viewModel.FirmwareCheckStatus);
          foreach (var option in viewModel.FirmwareOptions) Log($"OPTION {option.Label} {option.Url} installed={option.Installed}");
          string channel = Environment.GetEnvironmentVariable("MP_LIVE_CAN_CHANNEL") ?? "stable";
          var stable = viewModel.FirmwareOptions.First(option => option.Channel == channel && option.Source == "firmware.ardupilot.org");
          Log("INSTALL " + stable.Label);
          operation = viewModel.InstallFirmwareOptionAsync(stable);
        }
        var deadline = DateTime.UtcNow.AddSeconds(int.TryParse(Environment.GetEnvironmentVariable("MP_LIVE_CAN_DEADLINE"), out int d) ? d : 240);
        while (!operation.IsCompleted && DateTime.UtcNow < deadline) {
          Dispatcher.UIThread.RunJobs();
          await Task.Delay(50);
        }
        Dispatcher.UIThread.RunJobs();
        Log($"OPERATION completed={operation.IsCompleted} status={operation.Status} busy={viewModel.IsBusy}");
        if (operation.IsCompleted) await operation;
        await Task.Delay(8000);
        Dispatcher.UIThread.RunJobs();
        statusPump.Cancel();
        foreach (var item in viewModel.Nodes) {
          Log($"NODE {item.Id} {item.Name} sw {item.SoftwareVersion} crc {item.SoftwareCrc} hw {item.HardwareVersion} {item.Health} {item.Mode} up {item.Uptime} vsc {item.VendorSpecificCode}");
        }
      }
      Log($"frames rx={transport.ReceivedFrames} tx={transport.SentFrames}");
      link.UnSubscribeToPacketType(seqSub);
      viewModel.ToggleConnectCommand.Execute(null);
      await WaitForAsync(() => !viewModel.IsConnected, 5);
    } finally {
      pump.Cancel();
      try { link.Close(); } catch { }
      try { await reader.WaitAsync(TimeSpan.FromSeconds(5)); } catch { }
      Console.SetOut(previousOut);
      string path = Environment.GetEnvironmentVariable("MP_LIVE_CAN_TRACE") ?? Path.Combine(Path.GetTempPath(), "dronecan-live-trace.log");
      File.WriteAllText(path, log.ToString());
      output.WriteLine("trace written to " + path);
      output.WriteLine(log.ToString());
    }
  }

  private sealed class CallbackWriter(Action<string> onLine) : TextWriter {
    private readonly StringBuilder _buffer = new();
    public override Encoding Encoding => Encoding.UTF8;
    public override void Write(char value) {
      if (value == '\n') { onLine(_buffer.ToString().TrimEnd('\r')); _buffer.Clear(); } else { _buffer.Append(value); }
    }
    public override void Write(string? value) { foreach (char c in value ?? "") Write(c); }
  }

  private static async Task WaitForAsync(Func<bool> predicate, int seconds) {
    DateTime deadline = DateTime.UtcNow.AddSeconds(seconds);
    while (!predicate() && DateTime.UtcNow < deadline) {
      Dispatcher.UIThread.RunJobs();
      await Task.Delay(50);
    }
    Dispatcher.UIThread.RunJobs();
    Assert.True(predicate(), $"Timed out after {seconds} s waiting for the live DroneCAN state.");
  }
}
