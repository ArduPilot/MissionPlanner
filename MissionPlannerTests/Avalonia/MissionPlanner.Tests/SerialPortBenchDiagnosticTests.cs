using System.Diagnostics;
using MissionPlanner.Comms;
using Xunit.Abstractions;

namespace MissionPlanner.Tests;

/// <summary>
/// Opt-in measurement of the serial primitives the MAVLink reader relies on: set MP_LIVE_CAN_PORT
/// to a connected autopilot. On Linux every System.IO.Ports read costs about a millisecond, which
/// is why <c>WinSerialPort</c> buffers reads; the 1-byte read figure here should stay far below that.
/// </summary>
public class SerialPortBenchDiagnosticTests(ITestOutputHelper output) {
  [Fact]
  public async Task Serial_port_primitive_costs() {
    string? port = Environment.GetEnvironmentVariable("MP_LIVE_CAN_PORT");
    if (string.IsNullOrEmpty(port)) { output.WriteLine("skipped"); return; }
    var sp = new SerialPort { PortName = port, BaudRate = 115200 };
    sp.Open();
    sp.ReadTimeout = 1200;
    var buf = new byte[8192];
    await Task.Delay(500);
    var sw = Stopwatch.StartNew();
    for (int i = 0; i < 2000; i++) { _ = sp.BytesToRead; }
    output.WriteLine($"BytesToRead x2000: {sw.Elapsed.TotalMilliseconds / 2000 * 1000:F1} us each");
    sw.Restart();
    for (int i = 0; i < 2000; i++) { sp.ReadTimeout = 1200; }
    output.WriteLine($"ReadTimeout set x2000: {sw.Elapsed.TotalMilliseconds / 2000 * 1000:F1} us each");
    sw.Restart();
    for (int i = 0; i < 200; i++) { await Task.Delay(1); }
    output.WriteLine($"Task.Delay(1) x200: {sw.Elapsed.TotalMilliseconds / 200:F2} ms each");
    // single-byte reads while data is available
    int reads = 0; double waited = 0;
    sw.Restart();
    while (reads < 3000) {
      if (sp.BytesToRead <= 0) { var w = Stopwatch.StartNew(); await Task.Delay(1); waited += w.Elapsed.TotalMilliseconds; continue; }
      sp.Read(buf, 0, 1); reads++;
    }
    output.WriteLine($"1-byte Read x3000: total {sw.Elapsed.TotalMilliseconds:F0} ms, waiting {waited:F0} ms, per read {(sw.Elapsed.TotalMilliseconds - waited) / 3000 * 1000:F1} us");
    // throughput draining in chunks
    long bytes = 0; sw.Restart();
    while (sw.Elapsed.TotalSeconds < 3) {
      int n = sp.BytesToRead;
      if (n <= 0) { await Task.Delay(1); continue; }
      bytes += sp.Read(buf, 0, Math.Min(n, buf.Length));
    }
    output.WriteLine($"chunk drain: {bytes / 3.0:F0} B/s (telemetry only, no CAN forward)");
    sp.Close();
  }
}
