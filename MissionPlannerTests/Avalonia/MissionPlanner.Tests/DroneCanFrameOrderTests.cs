using System.Text;

namespace MissionPlanner.Tests;

/// <summary>
/// The DroneCAN reassembler must cope with two consecutive frames of one transfer arriving
/// swapped: the AP_Periph bootloader does this under bus load while it pipelines file reads,
/// which otherwise loses every fourth block request and the node's own GetNodeInfo answer.
/// </summary>
public class DroneCanFrameOrderTests {
  private static List<(DroneCAN.CANFrame cf, DroneCAN.CANPayload payload)> Frames(
      DroneCAN.DroneCAN can, DroneCAN.IDroneCANSerialize msg, byte tid = 7) =>
      can.PackageMessage(127, 8, tid, msg);

  private static uint Id(DroneCAN.CANFrame cf) => BitConverter.ToUInt32(cf.packet_data, 0);

  private static List<object> Feed(DroneCAN.DroneCAN can, IEnumerable<(DroneCAN.CANFrame cf, DroneCAN.CANPayload payload)> frames) {
    var received = new List<object>();
    DroneCAN.DroneCAN.MessageRecievedDel handler = (frame, msg, tid) => received.Add(msg);
    can.MessageReceived += handler;
    foreach (var (cf, payload) in frames) {
      can.ProcessFrame(cf, Id(cf), payload);
    }
    can.MessageReceived -= handler;
    return received;
  }

  private static DroneCAN.DroneCAN.uavcan_protocol_file_Read_req ReadRequest(ulong offset) {
    byte[] path = Encoding.ASCII.GetBytes("fw.bin");
    return new DroneCAN.DroneCAN.uavcan_protocol_file_Read_req {
      offset = offset,
      path = new DroneCAN.DroneCAN.uavcan_protocol_file_Path { path = path, path_len = (byte)path.Length },
    };
  }

  private static DroneCAN.DroneCAN.uavcan_protocol_GetNodeInfo_res NodeInfo() {
    byte[] name = Encoding.ASCII.GetBytes("org.ardupilot.MatekL431-Periph");
    return new DroneCAN.DroneCAN.uavcan_protocol_GetNodeInfo_res {
      name = name,
      name_len = (byte)name.Length,
      status = new DroneCAN.DroneCAN.uavcan_protocol_NodeStatus { uptime_sec = 42 },
      software_version = new DroneCAN.DroneCAN.uavcan_protocol_SoftwareVersion { major = 4, minor = 7, vcs_commit = 0xfb773567 },
      hardware_version = new DroneCAN.DroneCAN.uavcan_protocol_HardwareVersion { major = 4, minor = 38, unique_id = new byte[16] },
    };
  }

  [Fact]
  public void Two_frame_request_with_swapped_frames_is_decoded() {
    var can = new DroneCAN.DroneCAN { SourceNode = 125 };
    var frames = Frames(can, ReadRequest(256));
    Assert.Equal(2, frames.Count);

    var received = Feed(can, new[] { frames[1], frames[0] });

    var request = Assert.IsType<DroneCAN.DroneCAN.uavcan_protocol_file_Read_req>(Assert.Single(received));
    Assert.Equal(256UL, request.offset);
    Assert.Equal("fw.bin", Encoding.ASCII.GetString(request.path.path, 0, request.path.path_len));
  }

  [Fact]
  public void Long_transfer_with_two_middle_frames_swapped_is_decoded() {
    var can = new DroneCAN.DroneCAN { SourceNode = 125 };
    var frames = Frames(can, NodeInfo());
    Assert.True(frames.Count > 6, "expected a multi-frame GetNodeInfo response");
    var swapped = frames.ToList();
    (swapped[2], swapped[3]) = (swapped[3], swapped[2]);

    var received = Feed(can, swapped);

    var info = Assert.IsType<DroneCAN.DroneCAN.uavcan_protocol_GetNodeInfo_res>(Assert.Single(received));
    Assert.Equal("org.ardupilot.MatekL431-Periph", Encoding.ASCII.GetString(info.name, 0, info.name_len));
    Assert.Equal(0xfb773567u, info.software_version.vcs_commit);
  }

  [Fact]
  public void In_order_transfer_still_decodes_and_a_lost_frame_still_fails() {
    var can = new DroneCAN.DroneCAN { SourceNode = 125 };
    var frames = Frames(can, NodeInfo());

    Assert.Single(Feed(can, frames));

    var missing = frames.ToList();
    missing.RemoveAt(3);
    Assert.Empty(Feed(can, missing));

    // A fresh transfer with the same id afterwards is unaffected by the held frame.
    Assert.Single(Feed(can, frames));
  }

  [Fact]
  public void Swapped_last_two_frames_are_decoded() {
    var can = new DroneCAN.DroneCAN { SourceNode = 125 };
    var frames = Frames(can, NodeInfo());
    var swapped = frames.ToList();
    int last = swapped.Count - 1;
    (swapped[last - 1], swapped[last]) = (swapped[last], swapped[last - 1]);

    Assert.Single(Feed(can, swapped));
  }

  [Theory]
  [InlineData(0, null)]
  [InlineData(10, "no application signature (image erased or incomplete)")]
  [InlineData(13, "image CRC mismatch (blocks were lost or written out of order)")]
  [InlineData(18, "verification failed")]
  [InlineData(21, null)]
  public void Bootloader_check_codes_map_to_reasons(int code, string? reason) {
    Assert.Equal(reason, DroneCAN.DroneCAN.FirmwareCheckReason((ushort)code));
  }
}
