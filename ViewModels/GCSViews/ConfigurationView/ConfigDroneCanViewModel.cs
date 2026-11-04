using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DroneCAN;
using MissionPlanner;
using MissionPlanner.ArduPilot;
using MissionPlanner.Comms;
using MissionPlanner.Services;
using MissionPlanner.Utilities;

namespace MissionPlanner.ViewModels.GCSViews.ConfigurationView;

public partial class ConfigDroneCanViewModel : ViewModelBase, IDisposable {
  private const string _favoritesKey = "dronecan_fav_params";
  private const string _slcanPortKey = "dronecan_slcan_port";
  private const string _slcanBaudKey = "dronecan_slcan_baud";
  private const string _multicastInterfaceKey = "dronecan_mcast_interface";
  private readonly Func<DroneCanSessionTarget?> _activeTarget;
  private readonly Func<string[]> _serialPortNames;
  private readonly Func<string, int, ICommsSerial> _serialPortFactory;
  private readonly Func<IReadOnlyList<DroneCanNetworkInterfaceOption>> _networkInterfaces;
  private readonly Func<DroneCanNetworkInterfaceOption, byte, IDroneCanMulticastSession>
      _multicastSessionFactory;
  private readonly Func<DroneCAN.DroneCAN> _canFactory;
  private readonly bool _subscribedToAppState;
  private DroneCAN.DroneCAN? _can;
  private MavlinkCanTransport? _transport;
  private readonly ConcurrentDictionary<byte, DateTime> _nodeInfoRequests = new();
  private ICommsSerial? _directPort;
  private IDroneCanMulticastSession? _multicastSession;
  private DroneCanSessionTarget? _observedTarget;
  private DroneCanSessionTarget? _sessionTarget;
  private CancellationTokenSource? _operationCancellation;
  private bool _mavlinkCanRun;
  private bool _sessionRequiresVehicleTarget;
  private volatile bool _targetInvalidated;
  private long _targetRevision;
  private long _nodeRevision;
  private byte _busInUse;
  private int? _autopilotNodeId;
  private (byte Id, string Name)? _firmwareOptionsFor;
  private static readonly HttpClient _firmwareHttp = CreateFirmwareHttp();
  private bool _disposed;

  public ConfigDroneCanViewModel()
      : this(CaptureAppStateTarget, subscribeToAppState: true) {
  }

  internal ConfigDroneCanViewModel(
      Func<DroneCanSessionTarget?> activeTarget,
      bool subscribeToAppState = false,
      Func<string[]>? serialPortNames = null,
      Func<string, int, ICommsSerial>? serialPortFactory = null,
      Func<IReadOnlyList<DroneCanNetworkInterfaceOption>>? networkInterfaces = null,
      Func<DroneCanNetworkInterfaceOption, byte, IDroneCanMulticastSession>?
          multicastSessionFactory = null,
      Func<DroneCAN.DroneCAN>? canFactory = null) {
    _activeTarget = activeTarget;
    _serialPortNames = serialPortNames ?? SerialPort.GetPortNames;
    _serialPortFactory = serialPortFactory ?? ((port, baud) => new SerialPort {
      PortName = port,
      BaudRate = baud,
    });
    _networkInterfaces = networkInterfaces ?? DroneCanMulticastSession.GetAvailableInterfaces;
    _multicastSessionFactory = multicastSessionFactory ??
        ((networkInterface, bus) => new DroneCanMulticastSession(networkInterface, bus));
    _canFactory = canFactory ?? (() => new DroneCAN.DroneCAN());
    _subscribedToAppState = subscribeToAppState;
    _observedTarget = CaptureActiveTarget();
    LoadDirectSlcanSettings();
    RefreshSerialPorts();
    RefreshNetworkInterfaces();
    if (_subscribedToAppState) {
      AppState.ConnectionChanged += OnConnectionChanged;
    }
  }

  public ObservableCollection<DroneCanNode> Nodes { get; } = new();

  public ObservableCollection<DroneCanParam> NodeParams { get; } = new();

  /// <summary>Firmware builds found for the selected node by <see cref="CheckFirmwareUpdatesAsync"/>.</summary>
  public ObservableCollection<DroneCanFirmwareOption> FirmwareOptions { get; } = new();

  private readonly List<DroneCanParam> _allNodeParams = new();

  public ObservableCollection<DroneCanLog> DebugLog { get; } = new();

  public ObservableCollection<string> SerialPorts { get; } = new();

  public ObservableCollection<DroneCanNetworkInterfaceOption> NetworkInterfaces { get; } = new();

  public string[] BusOptions { get; } = {
    "MAVLink-CAN1", "MAVLink-CAN2", "SLCAN", "Multicast-CAN1", "Multicast-CAN2",
  };

  public int[] SerialBaudOptions { get; } = {
    1200, 2400, 4800, 9600, 19200, 38400, 57600, 111100, 115200, 230400,
    460800, 500000, 625000, 921600, 1000000, 1500000,
  };

  [ObservableProperty]
  private int _selectedBusIndex;

  [ObservableProperty]
  private string? _selectedSerialPort;

  [ObservableProperty]
  private int _selectedSerialBaud = 115200;

  [ObservableProperty]
  private DroneCanNetworkInterfaceOption? _selectedNetworkInterface;

  [ObservableProperty]
  private bool _exitSlcanOnLeave = true;

  [ObservableProperty]
  private bool _logToFile;

  [ObservableProperty]
  private bool _statsLogging;

  [ObservableProperty]
  private string _status = "Select MAVLink CAN, direct SLCAN or multicast to enumerate DroneCAN / UAVCAN nodes.";

  [ObservableProperty]
  private bool _isConnected;

  [ObservableProperty]
  private DroneCanNode? _selectedNode;

  [ObservableProperty]
  private string _nodeStatus = "Select a node, then Get Parameters / Restart / Update Firmware.";

  [ObservableProperty]
  private DroneCanFirmwareOption? _selectedFirmwareOption;

  [ObservableProperty]
  private string _firmwareCheckStatus = "";

  [ObservableProperty]
  private bool _isBusy;

  [ObservableProperty]
  private string _parameterSearch = "";

  [ObservableProperty]
  private bool _showModifiedParametersOnly;

  partial void OnSelectedNodeChanged(DroneCanNode? value) {
    Interlocked.Increment(ref _nodeRevision);
    CancelSafely(Volatile.Read(ref _operationCancellation));
    _allNodeParams.Clear();
    NodeParams.Clear();
    NodeStatus = value == null
        ? "Select a node, then Get Parameters / Restart / Update Firmware."
        : $"Node {value.Id} ({value.Name}) selected.";
  }

  partial void OnParameterSearchChanged(string value) => ApplyParameterFilter();

  partial void OnShowModifiedParametersOnlyChanged(bool value) => ApplyParameterFilter();

  partial void OnSelectedBusIndexChanged(int value) {
    OnPropertyChanged(nameof(ShowDirectSerialOptions));
    OnPropertyChanged(nameof(ShowMulticastOptions));
    OnPropertyChanged(nameof(CanEditDirectSerial));
    OnPropertyChanged(nameof(CanEditNetworkInterface));
  }

  partial void OnSelectedSerialPortChanged(string? value) {
    if (!string.IsNullOrWhiteSpace(value)) {
      Settings.Instance[_slcanPortKey] = value;
    }
  }

  partial void OnSelectedSerialBaudChanged(int value) {
    if (value > 0) {
      Settings.Instance[_slcanBaudKey] = value.ToString(CultureInfo.InvariantCulture);
    }
  }

  partial void OnSelectedNetworkInterfaceChanged(DroneCanNetworkInterfaceOption? value) {
    if (value != null) {
      Settings.Instance[_multicastInterfaceKey] = value.Id;
    }
  }

  public string ConnectLabel => IsConnected ? "Disconnect" : "Connect";
  public bool CanChangeInterface => !IsConnected && !IsBusy;
  public bool CanToggleConnection => IsConnected || !IsBusy;
  public bool ShowDirectSerialOptions => SelectedBusIndex == 2;
  public bool ShowMulticastOptions => SelectedBusIndex is 3 or 4;
  public bool CanEditDirectSerial => ShowDirectSerialOptions && CanChangeInterface;
  public bool CanEditNetworkInterface => ShowMulticastOptions && CanChangeInterface;
  public bool CanFilterFrames => IsConnected && _sessionRequiresVehicleTarget;

  partial void OnIsConnectedChanged(bool value) {
    OnPropertyChanged(nameof(ConnectLabel));
    OnPropertyChanged(nameof(CanChangeInterface));
    OnPropertyChanged(nameof(CanToggleConnection));
    OnPropertyChanged(nameof(CanEditDirectSerial));
    OnPropertyChanged(nameof(CanEditNetworkInterface));
    OnPropertyChanged(nameof(CanFilterFrames));
  }

  partial void OnIsBusyChanged(bool value) {
    OnPropertyChanged(nameof(CanChangeInterface));
    OnPropertyChanged(nameof(CanToggleConnection));
    OnPropertyChanged(nameof(CanEditDirectSerial));
    OnPropertyChanged(nameof(CanEditNetworkInterface));
  }

  partial void OnLogToFileChanged(bool value) {
    if (_can == null) {
      return;
    }

    try {
      _can.LogFile = value ? BuildLogPath() : null;
    } catch {
    }
  }

  private static string BuildLogPath() {
    string dir;
    try {
      dir = MissionPlanner.Utilities.Settings.Instance.LogDir;
    } catch {
      dir = Path.GetTempPath();
    }

    if (string.IsNullOrEmpty(dir)) {
      dir = Path.GetTempPath();
    }

    return Path.Combine(dir, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".can");
  }

  private void LoadDirectSlcanSettings() {
    try {
      SelectedSerialPort = Settings.Instance[_slcanPortKey];
      SelectedSerialBaud = Settings.Instance.GetInt32(_slcanBaudKey, 115200);
      if (SelectedSerialBaud <= 0) {
        SelectedSerialBaud = 115200;
      }
    } catch {
      SelectedSerialBaud = 115200;
    }
  }

  [RelayCommand]
  private void RefreshSerialPorts() {
    string? selected = SelectedSerialPort;
    string[] discovered;
    try {
      discovered = _serialPortNames();
    } catch {
      discovered = [];
    }

    SerialPorts.Clear();
    foreach (string port in discovered
                 .Where(port => !string.IsNullOrWhiteSpace(port))
                 .Distinct(OperatingSystem.IsWindows()
                     ? StringComparer.OrdinalIgnoreCase
                     : StringComparer.Ordinal)
                 .OrderBy(port => port, StringComparer.OrdinalIgnoreCase)) {
      SerialPorts.Add(port);
    }

    // Preserve a configured removable adapter while it is unplugged. Connect still reports the
    // native open error, but the operator does not have to re-enter a stable /dev/serial/by-id path.
    if (!string.IsNullOrWhiteSpace(selected) && !SerialPorts.Contains(selected)) {
      SerialPorts.Insert(0, selected);
    }
    SelectedSerialPort = !string.IsNullOrWhiteSpace(selected)
        ? selected
        : SerialPorts.FirstOrDefault();
  }

  [RelayCommand]
  private void RefreshNetworkInterfaces() {
    string? selectedId = SelectedNetworkInterface?.Id;
    if (string.IsNullOrWhiteSpace(selectedId)) {
      try {
        selectedId = Settings.Instance[_multicastInterfaceKey];
      } catch {
      }
    }

    IReadOnlyList<DroneCanNetworkInterfaceOption> discovered;
    try {
      discovered = _networkInterfaces();
    } catch {
      discovered = [];
    }

    NetworkInterfaces.Clear();
    foreach (DroneCanNetworkInterfaceOption option in discovered
                 .GroupBy(item => item.Id, StringComparer.Ordinal)
                 .Select(group => group.First())) {
      NetworkInterfaces.Add(option);
    }
    SelectedNetworkInterface = NetworkInterfaces.FirstOrDefault(
        item => string.Equals(item.Id, selectedId, StringComparison.Ordinal))
        ?? NetworkInterfaces.FirstOrDefault();
  }

  private void StartDirectSlcan() {
    string? portName = SelectedSerialPort?.Trim();
    int baud = SelectedSerialBaud;
    if (string.IsNullOrWhiteSpace(portName)) {
      Status = "Select the serial port of the SLCAN adapter first.";
      return;
    }
    if (baud <= 0) {
      Status = "Select a valid positive serial baud rate for the SLCAN adapter.";
      return;
    }

    string? conflictingEndpoint = FindOpenConnectionUsingPort(portName);
    if (conflictingEndpoint != null) {
      Status = $"{portName} is still used by MAVLink connection {conflictingEndpoint}. "
          + "Disconnect that link after switching the autopilot/adapter to SLCAN, then connect here.";
      return;
    }

    ICommsSerial directPort;
    try {
      directPort = _serialPortFactory(portName, baud);
    } catch (Exception ex) {
      Status = "Unable to create the SLCAN serial transport: " + ex.Message;
      return;
    }

    _observedTarget = CaptureActiveTarget();
    _sessionTarget = null;
    _sessionRequiresVehicleTarget = false;
    _targetInvalidated = false;
    long revision = Interlocked.Increment(ref _targetRevision);
    _mavlinkCanRun = true;
    _directPort = directPort;
    _can = CreateCanProtocol();
    IsConnected = true;
    IsBusy = true;
    Status = $"Opening direct SLCAN adapter {portName} at {baud} baud…";

    StartCanProtocol(
        _can, directPort, null, revision,
        $"Listening for DroneCAN nodes on direct SLCAN adapter {portName} at {baud} baud.",
        openTransport: true);
  }

  private void StartMulticast() {
    DroneCanNetworkInterfaceOption? networkInterface = SelectedNetworkInterface;
    if (networkInterface == null || !NetworkInterfaces.Contains(networkInterface)) {
      Status = "Select an active multicast-capable IPv4 network interface first.";
      return;
    }

    byte bus = SelectedBusIndex == 4 ? (byte)1 : (byte)0;
    IDroneCanMulticastSession session;
    try {
      session = _multicastSessionFactory(networkInterface, bus);
    } catch (Exception ex) {
      Status = "Unable to create the DroneCAN multicast transport: " + ex.Message;
      return;
    }

    _observedTarget = CaptureActiveTarget();
    _sessionTarget = null;
    _sessionRequiresVehicleTarget = false;
    _targetInvalidated = false;
    long revision = Interlocked.Increment(ref _targetRevision);
    _mavlinkCanRun = true;
    _busInUse = bus;
    _multicastSession = session;
    var can = CreateCanProtocol();
    _can = can;
    IsConnected = true;
    IsBusy = true;
    Status = $"Joining DroneCAN multicast {session.Endpoint} on {networkInterface.DisplayName}…";

    session.TransportFailed += ex => Dispatcher.UIThread.Post(() => {
      if (ReferenceEquals(_multicastSession, session) &&
          IsCanSessionCurrent(can, null, revision)) {
        Disconnect("DroneCAN multicast transport failed: " + ex.Message);
      }
    });

    try {
      session.Start();
    } catch (Exception ex) {
      Disconnect("Unable to join DroneCAN multicast: " + ex.Message);
      return;
    }

    StartCanProtocol(
        can, session.Serial, null, revision,
        $"Listening for DroneCAN nodes on {session.Endpoint} through "
            + $"{networkInterface.DisplayName}.",
        openTransport: false);
  }

  [RelayCommand]
  private async Task PrepareAutopilotSlcan() {
    if (IsConnected || IsBusy) {
      Status = "Disconnect the current DroneCAN session before preparing an autopilot SLCAN port.";
      return;
    }

    DroneCanSessionTarget? target = CaptureActiveTarget();
    if (target == null) {
      Status = "Connect to the autopilot over MAVLink before preparing its SLCAN port.";
      return;
    }
    if (target.Link.MAV.cs.armed) {
      Status = "SLCAN preparation is blocked while the selected vehicle is armed.";
      return;
    }

    long revision = Volatile.Read(ref _targetRevision);
    bool confirmed = await Services.Dialogs.Confirm(
        "Prepare ArduPilot SLCAN",
        "This writes the official Mission Planner CAN1 SLCAN settings to the selected "
            + $"vehicle {target.SystemId}:{target.ComponentId}. Its current MAVLink serial/USB "
            + "connection may stop responding after the SLCAN timeout. Continue?");
    if (!confirmed) {
      return;
    }
    if (!ShouldAcceptSessionResult(
            false, revision, Volatile.Read(ref _targetRevision), target, CaptureActiveTarget())) {
      Status = TargetChangedMessage;
      return;
    }

    IsBusy = true;
    Status = "Writing ArduPilot CAN1 SLCAN settings…";
    try {
      SlcanPreparationResult result = await Task.Run(() => PrepareAutopilotSlcan(target));
      if (!ShouldAcceptSessionResult(
              false, revision, Volatile.Read(ref _targetRevision), target, CaptureActiveTarget())) {
        return;
      }
      Status = result.Message;
      if (result.Success && string.IsNullOrWhiteSpace(SelectedSerialPort)) {
        string? activePort = target.Link.BaseStream?.PortName;
        if (!string.IsNullOrWhiteSpace(activePort)) {
          if (!SerialPorts.Contains(activePort)) {
            SerialPorts.Insert(0, activePort);
          }
          SelectedSerialPort = activePort;
          if (target.Link.BaseStream!.BaudRate > 0) {
            SelectedSerialBaud = target.Link.BaseStream.BaudRate;
          }
        }
      }
    } catch (Exception ex) {
      if (ShouldAcceptSessionResult(
              false, revision, Volatile.Read(ref _targetRevision), target, CaptureActiveTarget())) {
        Status = "Unable to prepare SLCAN: " + ex.Message;
      }
    } finally {
      if (revision == Volatile.Read(ref _targetRevision)) {
        IsBusy = false;
      }
    }
  }

  private static SlcanPreparationResult PrepareAutopilotSlcan(
      DroneCanSessionTarget target) {
    MAVLink.MAVLinkParam? cport = target.Link.MAVlist[
        target.SystemId, target.ComponentId].param["CAN_SLCAN_CPORT"];
    if (cport == null) {
      return new SlcanPreparationResult(false,
          "CAN_SLCAN_CPORT is not available on the selected firmware/vehicle.");
    }

    bool hadDisabledCport = Math.Abs((double)cport.Value) < double.Epsilon;
    if (!target.Link.setParam(
            target.SystemId, target.ComponentId, "CAN_SLCAN_CPORT", 1, true)) {
      return new SlcanPreparationResult(false, "Writing CAN_SLCAN_CPORT failed.");
    }
    if (hadDisabledCport) {
      return new SlcanPreparationResult(true,
          "CAN_SLCAN_CPORT was enabled. Reboot the autopilot, reconnect over MAVLink, "
              + "then run Prepare autopilot SLCAN again to finish the remaining settings.");
    }

    string[] required = ["CAN_SLCAN_TIMOUT", "CAN_P1_DRIVER"];
    foreach (string name in required) {
      if (target.Link.MAVlist[target.SystemId, target.ComponentId].param[name] == null) {
        return new SlcanPreparationResult(false,
            $"{name} is not available on the selected firmware/vehicle.");
      }
    }
    if (!target.Link.setParam(
            target.SystemId, target.ComponentId, "CAN_SLCAN_TIMOUT", 2, true)) {
      return new SlcanPreparationResult(false, "Writing CAN_SLCAN_TIMOUT failed.");
    }
    if (!target.Link.setParam(
            target.SystemId, target.ComponentId, "CAN_P1_DRIVER", 1, true)) {
      return new SlcanPreparationResult(false, "Writing CAN_P1_DRIVER failed.");
    }

    // Current ArduPilot exposes this parameter when an SLCAN serial number can be selected. The
    // official Mission Planner path writes zero (USB); older targets without it remain usable.
    if (target.Link.MAVlist[target.SystemId, target.ComponentId]
            .param["CAN_SLCAN_SERNUM"] != null &&
        !target.Link.setParam(
            target.SystemId, target.ComponentId, "CAN_SLCAN_SERNUM", 0, true)) {
      return new SlcanPreparationResult(false, "Writing CAN_SLCAN_SERNUM failed.");
    }

    return new SlcanPreparationResult(true,
        "ArduPilot CAN1 SLCAN is prepared with the official two-second timeout. "
            + "Disconnect MAVLink, wait at least two seconds, select its serial port above and Connect.");
  }

  private string? FindOpenConnectionUsingPort(string selectedPort) {
    try {
      return AppState.Connections.Snapshot()
          .Where(connection => connection.IsOpen)
          .FirstOrDefault(connection => PortsIdentifySameDevice(
              selectedPort, connection.Link.BaseStream?.PortName))?.Endpoint;
    } catch {
      return null;
    }
  }

  internal static bool PortsIdentifySameDevice(string? first, string? second) {
    if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(second)) {
      return false;
    }
    string left = ResolveSerialDevice(first.Trim());
    string right = ResolveSerialDevice(second.Trim());
    return string.Equals(left, right, OperatingSystem.IsWindows()
        ? StringComparison.OrdinalIgnoreCase
        : StringComparison.Ordinal);
  }

  private static string ResolveSerialDevice(string path) {
    try {
      if (Path.IsPathFullyQualified(path)) {
        return File.ResolveLinkTarget(path, returnFinalTarget: true)?.FullName
            ?? Path.GetFullPath(path);
      }
    } catch {
    }
    return path;
  }

  [RelayCommand]
  private void ToggleConnect() {
    if (_disposed) {
      return;
    }
    if (IsConnected) {
      Disconnect("Disconnected.");
      return;
    }
    if (IsBusy) {
      return;
    }

    ClearVehicleState();
    if (SelectedBusIndex == 2) {
      StartDirectSlcan();
      return;
    }
    if (SelectedBusIndex is 3 or 4) {
      StartMulticast();
      return;
    }
    if (SelectedBusIndex is < 0 or > 4) {
      Status = "Select a supported DroneCAN interface first.";
      return;
    }

    DroneCanSessionTarget? target = CaptureActiveTarget();
    if (target == null) {
      Status = "Not connected — open the MAVLink link first.";
      return;
    }

    byte bus = (byte)(SelectedBusIndex == 1 ? 2 : 1);
    _observedTarget = target;
    _sessionTarget = target;
    _sessionRequiresVehicleTarget = true;
    _targetInvalidated = false;
    long revision = Interlocked.Increment(ref _targetRevision);
    IsConnected = true;
    if (StartMavlinkCAN(target, bus, revision)) {
      Status = $"Starting MAVLink CAN{bus} for "
          + $"{target.SystemId}:{target.ComponentId} on the selected modem…";
    }
  }

  [RelayCommand]
  private void Filter() {
    if (IsConnected && !_sessionRequiresVehicleTarget) {
      Status = "Frame filtering is available for MAVLink-CAN1/CAN2; direct SLCAN and multicast "
          + "use their complete bus streams.";
      return;
    }
    var can = _can;
    DroneCanSessionTarget? target = _sessionTarget;
    long revision = Volatile.Read(ref _targetRevision);
    if (!IsConnected || can == null || target == null ||
        !IsCanSessionCurrent(can, target, revision)) {
      Status = "Connect first to configure frame filtering.";
      return;
    }
    byte busInUse = _busInUse;

    var defaultFilter = new List<ushort> {
      (ushort)0,
      DroneCAN.DroneCAN.uavcan_protocol_NodeStatus.UAVCAN_PROTOCOL_NODESTATUS_DT_ID,
      DroneCAN.DroneCAN.uavcan_protocol_GetNodeInfo_req.UAVCAN_PROTOCOL_GETNODEINFO_REQ_DT_ID,
      DroneCAN.DroneCAN.uavcan_protocol_RestartNode_req.UAVCAN_PROTOCOL_RESTARTNODE_REQ_DT_ID,
      DroneCAN.DroneCAN.uavcan_protocol_param_GetSet_req.UAVCAN_PROTOCOL_PARAM_GETSET_REQ_DT_ID,
      DroneCAN.DroneCAN.uavcan_protocol_param_ExecuteOpcode_req
          .UAVCAN_PROTOCOL_PARAM_EXECUTEOPCODE_REQ_DT_ID,
      DroneCAN.DroneCAN.uavcan_protocol_file_BeginFirmwareUpdate_req
          .UAVCAN_PROTOCOL_FILE_BEGINFIRMWAREUPDATE_REQ_DT_ID,
      DroneCAN.DroneCAN.uavcan_protocol_file_Read_req.UAVCAN_PROTOCOL_FILE_READ_REQ_DT_ID,
      DroneCAN.DroneCAN.uavcan_protocol_file_GetInfo_req.UAVCAN_PROTOCOL_FILE_GETINFO_REQ_DT_ID,
      DroneCAN.DroneCAN.uavcan_protocol_dynamic_node_id_Allocation
          .UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_ALLOCATION_DT_ID,
      DroneCAN.DroneCAN.uavcan_protocol_debug_LogMessage.UAVCAN_PROTOCOL_DEBUG_LOGMESSAGE_DT_ID,
    };

    void SendFilter(byte numIds) {
      var filter = new MAVLink.mavlink_can_filter_modify_t(
          defaultFilter.ToArray().MakeSize(16), target.SystemId,
          target.ComponentId, busInUse,
          (byte)MAVLink.CAN_FILTER_OP.CAN_FILTER_REPLACE, numIds);

      if (!IsCanSessionCurrent(can, target, revision)) {
        return;
      }

      try {
        target.Link.sendPacket(filter, target.SystemId, target.ComponentId);
      } catch (Exception ex) {
        Console.WriteLine(ex.ToString());
      }
    }

    var panel = new StackPanel { Margin = new Thickness(8) };

    var all = new CheckBox { Content = "ALL" };
    all.IsCheckedChanged += (_, _) => SendFilter(0);
    panel.Children.Add(all);

    foreach (var msg in DroneCAN.DroneCAN.MSG_INFO
                 .Select(a => (a.msgid, a.type.Name)).OrderBy(a => a.Name.ToLower())) {
      var msgid = msg.msgid;
      var cb = new CheckBox { Content = msg.Name, IsChecked = defaultFilter.Contains(msgid) };
      cb.IsCheckedChanged += (_, _) => {
        if (cb.IsChecked == true) {
          if (!defaultFilter.Contains(msgid)) {
            defaultFilter.Add(msgid);
          }
        } else {
          defaultFilter.Remove(msgid);
        }

        SendFilter((byte)defaultFilter.Count);
      };
      panel.Children.Add(cb);
    }

    var window = new Window {
      Title = "DroneCAN Messages",
      Width = 360,
      Height = 600,
      Background = new SolidColorBrush(Color.Parse("#434445")),
      WindowStartupLocation = WindowStartupLocation.CenterOwner,
      Content = new ScrollViewer { Content = panel },
    };

    var owner = Services.Dialogs.Owner;
    if (owner != null) {
      window.Show(owner);
    } else {
      window.Show();
    }

    Status = "Frame filter open — toggling a message updates the CAN acceptance filter.";
  }

  [RelayCommand]
  private void Stats() {
    if (!HasCurrentCanSession()) {
      NodeStatus = "Connect first to capture node statistics.";
      return;
    }

    StatsLogging = !StatsLogging;
    NodeStatus = StatsLogging
        ? "Logging DroneCAN node statistics to the message grid…"
        : "Stopped logging node statistics.";
  }

  [RelayCommand]
  private void SelectNode(DroneCanNode? node) {
    if (node != null && Nodes.Contains(node) && HasCurrentCanSession()) {
      SelectedNode = node;
    }
  }

  [RelayCommand]
  private void Refresh() {
    if (IsBusy) {
      Status = "Wait for the current DroneCAN operation to finish before refreshing nodes.";
      return;
    }
    if (!HasCurrentCanSession()) {
      Status = "Connect first to refresh the node list.";
      return;
    }

    _can?.NodeList.Clear();
    _can?.NodeInfo.Clear();
    SelectedNode = null;
    Nodes.Clear();
    Status = "Waiting for fresh DroneCAN node status…";
  }

  [RelayCommand]
  private async Task GetParameters() {
    DroneCanOperation? operation = TryBeginNodeOperation();
    if (operation == null) {
      return;
    }

    NodeStatus = $"Requesting parameters from node {operation.NodeId}…";
    try {
      var response = await Task.Run(() => {
        operation.Cancellation.Token.ThrowIfCancellationRequested();
        var result = operation.Can.GetParameters(
            operation.NodeId, out bool receivedResponse);
        operation.Cancellation.Token.ThrowIfCancellationRequested();
        return (Parameters: result, ReceivedResponse: receivedResponse);
      });
      if (!IsOperationCurrent(operation)) {
        return;
      }

      _allNodeParams.Clear();
      NodeParams.Clear();
      bool hasDedicatedFavorites = Settings.Instance.ContainsKey(_favoritesKey);
      var favs = Settings.Instance.GetList(
          hasDedicatedFavorites ? _favoritesKey : "fav_params").ToHashSet();
      foreach (var p in response.Parameters) {
        var name = Encoding.ASCII.GetString(p.name, 0, p.name_len);
        if (string.IsNullOrEmpty(name)) {
          continue;
        }

        _allNodeParams.Add(new DroneCanParam {
          Name = name,
          Value = Convert.ToString(p.value.GetValue(), CultureInfo.InvariantCulture) ?? "",
          OriginalValue = Convert.ToString(p.value.GetValue(), CultureInfo.InvariantCulture) ?? "",
          Min = Convert.ToString(p.min_value.GetValue(), CultureInfo.InvariantCulture) ?? "",
          Max = Convert.ToString(p.max_value.GetValue(), CultureInfo.InvariantCulture) ?? "",
          Default = Convert.ToString(p.default_value.GetValue(), CultureInfo.InvariantCulture) ?? "",
          IsString = p.value.uavcan_protocol_param_Value_type ==
              DroneCAN.DroneCAN.uavcan_protocol_param_Value.uavcan_protocol_param_Value_type_t
                  .UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_STRING_VALUE,
          IsFav = favs.Contains(name),
        });
      }

      if (!hasDedicatedFavorites) {
        // Early Avalonia builds accidentally shared fav_params with the vehicle parameter page.
        // Copy only names that exist on this node, without modifying the vehicle favourites.
        Settings.Instance.SetList(_favoritesKey,
            _allNodeParams.Where(parameter => parameter.IsFav).Select(parameter => parameter.Name));
      }

      SortAndFilterParameters();
      NodeStatus = !response.ReceivedResponse
          ? $"Node {operation.NodeId} did not respond to uavcan.protocol.param.GetSet; "
              + "the parameter service may not be supported."
          : _allNodeParams.Count == 0
              ? $"Node {operation.NodeId} responded but exposes no configurable parameters."
              : $"Loaded {_allNodeParams.Count} parameters from node {operation.NodeId}.";
    } catch (OperationCanceledException) when (operation.Cancellation.IsCancellationRequested) {
      // A modem/vehicle change reports the stronger session warning from the invalidation path.
    } catch (Exception ex) {
      if (IsOperationCurrent(operation)) {
        NodeStatus = "Error getting parameters: " + ex.Message;
      }
    } finally {
      CompleteOperation(operation);
    }
  }

  [RelayCommand]
  private async Task WriteParameters() {
    DroneCanOperation? operation = TryBeginNodeOperation();
    if (operation == null) {
      return;
    }

    var changed = _allNodeParams.Where(p => p.IsDirty)
        .Select(parameter => (Parameter: parameter, Value: parameter.Value)).ToList();
    if (changed.Count == 0) {
      NodeStatus = "No modified parameters to write.";
      CompleteOperation(operation);
      return;
    }

    NodeStatus = $"Writing {changed.Count} parameter(s) to node {operation.NodeId}…";
    try {
      var result = await Task.Run(() => {
        int failed = 0;
        var written = new List<DroneCanParam>();
        foreach (var item in changed) {
          if (!IsOperationCurrent(operation)) {
            return (Stale: true, Failed: failed, Written: written);
          }
          try {
            if (!TryConvertParameterValue(item.Parameter, item.Value, out object value)) {
              failed++;
              continue;
            }
            if (!operation.Can.SetParameter(
                    operation.NodeId, item.Parameter.Name, value)) {
              failed++;
            } else {
              written.Add(item.Parameter);
            }
          } catch {
            failed++;
          }
        }

        if (!IsOperationCurrent(operation)) {
          return (Stale: true, Failed: failed, Written: written);
        }
        try {
          operation.Can.SaveConfig(operation.NodeId);
        } catch {
        }
        return (Stale: false, Failed: failed, Written: written);
      });
      if (result.Stale || !IsOperationCurrent(operation)) {
        return;
      }

      foreach (var parameter in result.Written) {
        parameter.AcceptValue();
      }
      ApplyParameterFilter();

      NodeStatus = result.Failed == 0
          ? $"Wrote {changed.Count} parameters and saved to flash."
          : $"Wrote parameters with {result.Failed} failure(s); saved to flash.";
    } catch (OperationCanceledException) when (operation.Cancellation.IsCancellationRequested) {
    } catch (Exception ex) {
      if (IsOperationCurrent(operation)) {
        NodeStatus = "Parameter write failed: " + ex.Message;
      }
    } finally {
      CompleteOperation(operation);
    }
  }

  [RelayCommand]
  private void ToggleParameterFavorite(DroneCanParam? parameter) {
    if (parameter == null || !_allNodeParams.Contains(parameter) || !HasCurrentCanSession()) {
      return;
    }

    parameter.IsFav = !parameter.IsFav;
    var favs = _allNodeParams.Where(p => p.IsFav).Select(p => p.Name);
    Settings.Instance.SetList(_favoritesKey, favs);
    SortAndFilterParameters();
  }

  [RelayCommand]
  private async Task ImportParameters() {
    DroneCanSelection? selection = CaptureSelection();
    var owner = Services.Dialogs.Owner;
    if (owner == null || _allNodeParams.Count == 0 || selection == null) {
      NodeStatus = "Get node parameters before importing a .param file.";
      return;
    }

    var files = await owner.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions {
      Title = "Import DroneCAN parameters",
      AllowMultiple = false,
      FileTypeFilter = new[] {
        new FilePickerFileType("Parameter files") { Patterns = new[] { "*.param", "*.parm" } },
        new FilePickerFileType("All files") { Patterns = new[] { "*" } },
      },
    });
    var path = files.FirstOrDefault()?.TryGetLocalPath();
    if (path == null) {
      return;
    }
    if (!IsSelectionCurrent(selection)) {
      NodeStatus = TargetChangedMessage;
      return;
    }

    Dictionary<string, string> fileParams;
    try {
      fileParams = ParamFile.LoadTextParamFile(path);
    } catch (Exception ex) {
      NodeStatus = "Parameter import failed: " + ex.Message;
      return;
    }
    if (!IsSelectionCurrent(selection)) {
      NodeStatus = TargetChangedMessage;
      return;
    }

    int matched = 0;
    int invalid = 0;
    foreach (var parameter in _allNodeParams) {
      if (fileParams.TryGetValue(parameter.Name, out var value)) {
        if (!TryConvertParameterValue(parameter, value, out _)) {
          invalid++;
          continue;
        }
        parameter.Value = value;
        matched++;
      }
    }
    ApplyParameterFilter();
    NodeStatus = $"Imported {matched} matching value(s)"
        + (invalid > 0 ? $"; ignored {invalid} invalid numeric value(s)" : "")
        + "; review them, then press Write.";
  }

  [RelayCommand]
  private async Task ExportParameters() {
    DroneCanSelection? selection = CaptureSelection();
    var owner = Services.Dialogs.Owner;
    if (owner == null || _allNodeParams.Count == 0 || selection == null) {
      NodeStatus = "Get node parameters before exporting.";
      return;
    }

    var file = await owner.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions {
      Title = "Export DroneCAN parameters",
      SuggestedFileName = SelectedNode == null ? "dronecan.param" : $"dronecan-node-{SelectedNode.Id}.param",
      DefaultExtension = "param",
      FileTypeChoices = new[] {
        new FilePickerFileType("Parameter files") { Patterns = new[] { "*.param" } },
      },
    });
    var path = file?.TryGetLocalPath();
    if (path == null) {
      return;
    }
    if (!IsSelectionCurrent(selection)) {
      NodeStatus = TargetChangedMessage;
      return;
    }
    if (!await Services.Dialogs.ConfirmDangerous(
            "Export DroneCAN parameters",
            Services.Dialogs.SensitiveExportWarning,
            "EXPORT PARAMETERS")) {
      NodeStatus = "Parameter export cancelled.";
      return;
    }
    if (!IsSelectionCurrent(selection)) {
      NodeStatus = TargetChangedMessage;
      return;
    }

    try {
      if (!IsSelectionCurrent(selection)) {
        NodeStatus = TargetChangedMessage;
        return;
      }
      ParamFile.SaveTextParamFile(path,
          _allNodeParams.Select(parameter =>
              new KeyValuePair<string, string>(parameter.Name, parameter.Value)));
      NodeStatus = $"Exported {_allNodeParams.Count} parameter(s) to {Path.GetFileName(path)}.";
    } catch (Exception ex) {
      NodeStatus = "Parameter export failed: " + ex.Message;
    }
  }

  private void SortAndFilterParameters() {
    _allNodeParams.Sort((a, b) => a.IsFav != b.IsFav
        ? b.IsFav.CompareTo(a.IsFav)
        : string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
    ApplyParameterFilter();
  }

  internal static bool TryConvertParameterValue(
      DroneCanParam parameter, string text, out object value) {
    if (parameter.IsString) {
      value = text;
      return true;
    }
    if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture,
            out double number)) {
      if (double.TryParse(parameter.Min, NumberStyles.Any, CultureInfo.InvariantCulture,
              out double minimum) && number < minimum) {
        value = 0d;
        return false;
      }
      if (double.TryParse(parameter.Max, NumberStyles.Any, CultureInfo.InvariantCulture,
              out double maximum) && number > maximum) {
        value = 0d;
        return false;
      }
      value = number;
      return true;
    }
    value = 0d;
    return false;
  }

  private void ApplyParameterFilter() {
    var search = ParameterSearch.Trim();
    NodeParams.Clear();
    foreach (var parameter in _allNodeParams) {
      if (ShowModifiedParametersOnly && !parameter.IsDirty) {
        continue;
      }
      if (search.Length >= 2 &&
          !parameter.Name.Contains(search, StringComparison.OrdinalIgnoreCase) &&
          !parameter.Value.Contains(search, StringComparison.OrdinalIgnoreCase) &&
          !parameter.Default.Contains(search, StringComparison.OrdinalIgnoreCase)) {
        continue;
      }
      NodeParams.Add(parameter);
    }
  }

  [RelayCommand]
  private async Task SaveConfig() {
    DroneCanOperation? operation = TryBeginNodeOperation();
    if (operation == null) {
      return;
    }

    try {
      bool ok = await Task.Run(() => {
        operation.Cancellation.Token.ThrowIfCancellationRequested();
        return operation.Can.SaveConfig(operation.NodeId);
      });
      if (IsOperationCurrent(operation)) {
        NodeStatus = ok
            ? "Parameters committed to non-volatile memory."
            : "Failed to save parameters.";
      }
    } catch (OperationCanceledException) when (operation.Cancellation.IsCancellationRequested) {
    } catch (Exception ex) {
      if (IsOperationCurrent(operation)) {
        NodeStatus = "Failed to save parameters: " + ex.Message;
      }
    } finally {
      CompleteOperation(operation);
    }
  }

  [RelayCommand]
  private async Task EraseConfig() {
    DroneCanOperation? operation = TryBeginNodeOperation();
    if (operation == null) {
      return;
    }

    try {
      bool ok = await Task.Run(() => {
        operation.Cancellation.Token.ThrowIfCancellationRequested();
        return operation.Can.ExecuteOpCode(operation.NodeId,
            (byte)DroneCAN.DroneCAN.uavcan_protocol_param_ExecuteOpcode_req
                .UAVCAN_PROTOCOL_PARAM_EXECUTEOPCODE_REQ_OPCODE_ERASE);
      });
      if (IsOperationCurrent(operation)) {
        NodeStatus = ok
            ? "Erased parameters to defaults (node restart may be required)."
            : "Failed to erase parameters.";
      }
    } catch (OperationCanceledException) when (operation.Cancellation.IsCancellationRequested) {
    } catch (Exception ex) {
      if (IsOperationCurrent(operation)) {
        NodeStatus = "Failed to erase parameters: " + ex.Message;
      }
    } finally {
      CompleteOperation(operation);
    }
  }

  [RelayCommand]
  private async Task RestartNode() {
    DroneCanOperation? operation = TryBeginNodeOperation();
    if (operation == null) {
      return;
    }

    try {
      bool ok = await Task.Run(() => {
        operation.Cancellation.Token.ThrowIfCancellationRequested();
        return operation.Can.RestartNode(operation.NodeId);
      });
      if (IsOperationCurrent(operation)) {
        NodeStatus = ok
            ? $"Node {operation.NodeId} restart requested."
            : $"Node {operation.NodeId} did not acknowledge restart.";
      }
    } catch (OperationCanceledException) when (operation.Cancellation.IsCancellationRequested) {
    } catch (Exception ex) {
      if (IsOperationCurrent(operation)) {
        NodeStatus = "Node restart failed: " + ex.Message;
      }
    } finally {
      CompleteOperation(operation);
    }
  }

  public async Task UpdateFirmwareAsync(string firmwarePath) {
    DroneCanOperation? operation = TryBeginNodeOperation();
    if (operation == null) {
      return;
    }

    if (string.IsNullOrEmpty(firmwarePath) || !File.Exists(firmwarePath)) {
      NodeStatus = "Firmware file not found.";
      CompleteOperation(operation);
      return;
    }

    try {
      await FlashFirmwareAsync(operation, firmwarePath);
    } finally {
      CompleteOperation(operation);
    }
  }

  private static HttpClient CreateFirmwareHttp() {
    var client = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
    try {
      client.DefaultRequestHeaders.UserAgent.ParseAdd("MissionPlanner10");
    } catch {
    }
    return client;
  }

  /// <summary>
  /// Looks up AP_Periph builds for the selected node in the ArduPilot manifest (and the CubePilot
  /// server for its boards) and fills <see cref="FirmwareOptions"/>. Returns true when at least one
  /// build is available; <see cref="FirmwareCheckStatus"/> explains the outcome either way.
  /// </summary>
  public async Task<bool> CheckFirmwareUpdatesAsync() {
    DroneCanOperation? operation = TryBeginNodeOperation();
    if (operation == null) {
      return false;
    }

    DroneCanNode? node = SelectedNode;
    FirmwareOptions.Clear();
    SelectedFirmwareOption = null;
    _firmwareOptionsFor = null;
    if (node == null || node.Name == "?" || node.Name.Length == 0) {
      FirmwareCheckStatus = NodeStatus =
          "The node has not reported its name yet; wait for node info before checking firmware.";
      CompleteOperation(operation);
      return false;
    }

    string name = node.Name;
    uint vcs = node.SoftwareVcsCommit;
    string running = $"{node.SoftwareVersion.Split('.').Take(2).Aggregate((a, b) => a + "." + b)}"
        + (vcs != 0 ? $" ({vcs:X8})" : "");
    NodeStatus = $"Checking firmware for node {node.Id} {name}…";
    try {
      var options = await Task.Run(() => {
        operation.Cancellation.Token.ThrowIfCancellationRequested();
        APFirmware.GetList();
        var found = DroneCanFirmwareCatalog.Match(APFirmware.Manifest?.Firmware, name, vcs);
        if (!double.TryParse(node.HardwareVersion, NumberStyles.Float, CultureInfo.InvariantCulture,
                out double hardware)) {
          hardware = 0;
        }
        foreach (bool beta in new[] { false, true }) {
          string vendor = operation.Can.LookForUpdate(name, hardware, beta);
          if (!string.IsNullOrEmpty(vendor) && Uri.TryCreate(vendor, UriKind.Absolute, out var url)) {
            found.Add(new DroneCanFirmwareOption(beta ? "beta" : "stable", "vendor build", "", url, null, url.Host));
          }
        }
        return found;
      }, operation.Cancellation.Token);
      await DroneCanFirmwareCatalog.FillPublishedDatesAsync(_firmwareHttp, options, operation.Cancellation.Token);
      if (!IsOperationCurrent(operation)) {
        return false;
      }
      foreach (var option in options) {
        FirmwareOptions.Add(option);
      }
      SelectedFirmwareOption = options.FirstOrDefault(option => !option.Installed) ?? options.FirstOrDefault();
      _firmwareOptionsFor = (node.Id, name);
      string platform = DroneCanFirmwareCatalog.PlatformOf(name) ?? name;
      FirmwareCheckStatus = options.Count == 0
          ? $"No firmware for {platform} in the ArduPilot manifest. Node {node.Id} runs {running}; "
            + "use Update Firmware… with a file from the vendor."
          : $"Node {node.Id} {name} (hardware {node.HardwareVersion}) runs {running}. "
            + $"{options.Count} build(s) available for {platform}"
            + (options.Any(option => option.Installed) ? "; one matches the running commit." : ".");
      NodeStatus = FirmwareCheckStatus;
      return options.Count != 0;
    } catch (OperationCanceledException) when (operation.Cancellation.IsCancellationRequested) {
      return false;
    } catch (Exception ex) {
      if (IsOperationCurrent(operation)) {
        FirmwareCheckStatus = NodeStatus = "Firmware check failed: " + ex.Message;
      }
      return false;
    } finally {
      CompleteOperation(operation);
    }
  }

  /// <summary>Downloads the chosen build and sends it to the node the options were built for.</summary>
  public async Task InstallFirmwareOptionAsync(DroneCanFirmwareOption option) {
    DroneCanOperation? operation = TryBeginNodeOperation();
    if (operation == null) {
      return;
    }

    DroneCanNode? node = SelectedNode;
    if (_firmwareOptionsFor is not { } built || node == null || built.Id != node.Id || built.Name != node.Name
        || !FirmwareOptions.Contains(option)) {
      NodeStatus = "The firmware list belongs to another node; run Check for Updates again.";
      CompleteOperation(operation);
      return;
    }

    string directory = Path.Combine(Path.GetTempPath(), "missionplanner-dronecan-firmware");
    string? image = null;
    try {
      NodeStatus = $"Downloading {option.Channel} {option.Version} from {option.Source}…";
      var progress = new Progress<double>(percent => {
        if (IsOperationCurrent(operation)) {
          NodeStatus = $"Downloading {option.Channel} {option.Version}: {percent:0}%";
        }
      });
      image = await DroneCanFirmwareCatalog.DownloadAsync(
          _firmwareHttp, option, directory, progress, operation.Cancellation.Token);
      if (!IsOperationCurrent(operation)) {
        return;
      }
      await FlashFirmwareAsync(operation, image);
    } catch (OperationCanceledException) when (operation.Cancellation.IsCancellationRequested) {
    } catch (Exception ex) {
      if (IsOperationCurrent(operation)) {
        NodeStatus = "Firmware download failed: " + ex.Message;
      }
    } finally {
      if (image != null) {
        try {
          File.Delete(image);
        } catch {
        }
      }
      CompleteOperation(operation);
    }
  }

  private async Task FlashFirmwareAsync(DroneCanOperation operation, string firmwarePath) {
    // The bootloader pipelines several block reads, so the served offset is not monotonic; show
    // the furthest block instead of a percentage that steps backwards.
    double furthest = 0;
    DroneCAN.DroneCAN.FileSendProgressArgs progress = (n, f, p) => {
      if (IsOperationCurrent(operation)) {
        Dispatcher.UIThread.Post(() => {
          if (IsOperationCurrent(operation)) {
            furthest = Math.Max(furthest, p);
            NodeStatus = $"Firmware {f}: {furthest:0}%";
          }
        });
      }
    };
    DroneCAN.DroneCAN.FileSendCompleteArgs complete = (n, f) => {
      if (IsOperationCurrent(operation)) {
        Dispatcher.UIThread.Post(() => {
          if (IsOperationCurrent(operation)) {
            NodeStatus = "Firmware send complete.";
          }
        });
      }
    };

    operation.Can.FileSendProgress += progress;
    operation.Can.FileSendComplete += complete;

    string? temporaryFirmware = null;
    try {
      (string deviceName, bool uploaded) = await Task.Run(() => {
        operation.Cancellation.Token.ThrowIfCancellationRequested();
        string name = operation.Can.GetNodeName(operation.NodeId);
        var file = firmwarePath;

        if (file.ToLowerInvariant().EndsWith(".apj")) {
          var fw = px4uploader.Firmware.ProcessFirmware(file);
          temporaryFirmware = Path.GetTempFileName();
          File.WriteAllBytes(temporaryFirmware, fw.imagebyte);
          file = temporaryFirmware;
        }

        operation.Cancellation.Token.ThrowIfCancellationRequested();
        bool sent = operation.Can.Update(
            operation.NodeId, name, 0, file, operation.Cancellation.Token);
        operation.Cancellation.Token.ThrowIfCancellationRequested();
        return (name, sent);
      });
      if (IsOperationCurrent(operation)) {
        NodeStatus = uploaded
            ? $"Firmware installed on node {operation.NodeId} ({deviceName}); the node restarted."
            : $"Node {operation.NodeId} ({deviceName}) already runs this image; nothing was sent.";
      }
    } catch (OperationCanceledException) when (operation.Cancellation.IsCancellationRequested) {
    } catch (Exception ex) {
      if (IsOperationCurrent(operation)) {
        NodeStatus = "Firmware update failed: " + ex.Message;
      }
    } finally {
      operation.Can.FileSendProgress -= progress;
      operation.Can.FileSendComplete -= complete;
      if (temporaryFirmware != null) {
        try {
          File.Delete(temporaryFirmware);
        } catch {
        }
      }
    }
  }

  private bool StartMavlinkCAN(
      DroneCanSessionTarget target, byte bus, long revision) {
    _busInUse = bus;
    _mavlinkCanRun = true;
    _can = CreateCanProtocol();
    _autopilotNodeId = AutopilotNodeId(target, bus);
    var can = _can;

    // One tunnel per link: the transport renews MAV_CMD_CAN_FORWARD, converts frames both ways and
    // never echoes received frames back to the vehicle.
    MavlinkCanTransport? transport = MavlinkCanTransport.Open(
        target.Link, target.SystemId, target.ComponentId, bus,
        () => IsCanSessionCurrent(can, target, revision), out string? conflict);
    if (transport == null) {
      _can = null;
      Disconnect(conflict ?? "The MAVLink CAN tunnel is busy.");
      return false;
    }
    _transport = transport;
    ScheduleBusHint(can, target, revision, bus);
    StartCanProtocol(
        can, transport.Port, target, revision,
        $"Listening for nodes on MAVLink CAN{bus} from "
            + $"{target.SystemId}:{target.ComponentId} on the selected modem…",
        openTransport: false);
    return true;
  }

  /// <summary>
  /// The autopilot forwards only the selected physical port. After a few seconds, tell the operator
  /// when nothing but the autopilot's own node answered, which is what a device wired to the other
  /// connector looks like.
  /// </summary>
  private void ScheduleBusHint(
      DroneCAN.DroneCAN can, DroneCanSessionTarget target, long revision, byte bus) {
    _ = Task.Run(async () => {
      await Task.Delay(TimeSpan.FromSeconds(6)).ConfigureAwait(false);
      PostForSession(can, target, revision, () => {
        byte other = (byte)(bus == 1 ? 2 : 1);
        int? autopilotNode = AutopilotNodeId(target, bus);
        if (Nodes.Count == 0) {
          Status = $"No DroneCAN traffic on MAVLink CAN{bus} yet. Check CAN_P{bus}_DRIVER and the "
              + $"wiring, or try MAVLink-CAN{other}: the autopilot forwards only the selected port.";
        } else if (autopilotNode != null && Nodes.All(node => node.Id == autopilotNode)) {
          Status = $"Only the autopilot itself (node {autopilotNode}) is visible on MAVLink CAN{bus}. "
              + $"A device wired to the other CAN connector appears only under MAVLink-CAN{other}.";
        }
      });
    });
  }

  internal static int? AutopilotNodeId(DroneCanSessionTarget target, byte bus) {
    try {
      var param = target.Link.MAVlist[target.SystemId, target.ComponentId].param;
      int driver = param.ContainsKey($"CAN_P{bus}_DRIVER") ? (int)param[$"CAN_P{bus}_DRIVER"].Value : 1;
      string name = $"CAN_D{driver}_UC_NODE";
      return param.ContainsKey(name) ? (int)param[name].Value : null;
    } catch {
      return null;
    }
  }

  /// <summary>
  /// Asks a node that is still shown as "?" for its name and versions as soon as it announces itself,
  /// like the native page does, instead of waiting for the stack's one-node-per-ten-seconds sweep.
  /// </summary>
  private void RequestNodeInfoIfUnknown(DroneCAN.DroneCAN can, byte node) {
    if (node == can.SourceNode || can.NodeInfo.ContainsKey(node)) {
      return;
    }
    DateTime now = DateTime.UtcNow;
    DateTime last = _nodeInfoRequests.GetOrAdd(node, DateTime.MinValue);
    if (now - last < TimeSpan.FromSeconds(2)) {
      return;
    }
    _nodeInfoRequests[node] = now;
    try {
      can.WriteToStreamSLCAN(can.PackageMessageSLCAN(
          node, 30, can.TransferID++, new DroneCAN.DroneCAN.uavcan_protocol_GetNodeInfo_req()));
    } catch {
    }
  }

  private void StartCanProtocol(
      DroneCAN.DroneCAN can,
      ICommsSerial transport,
      DroneCanSessionTarget? target,
      long revision,
      string connectedStatus,
      bool openTransport) {
    can.NodeAdded += (id, msg) => {
      // The stack reports its own outgoing NodeStatus as well; this application is not a bus node to configure.
      if (id == can.SourceNode) {
        return;
      }
      PostForSession(can, target, revision, () => {
        if (Nodes.Any(n => n.Id == id)) {
          return;
        }

        Nodes.Add(new DroneCanNode {
          Id = id,
          Name = "?",
          IsAutopilot = id == _autopilotNodeId,
          Health = HealthString(msg.health),
          Mode = ModeString(msg.mode),
          Uptime = TimeSpan.FromSeconds(msg.uptime_sec),
        });
      });
    };

    can.MessageReceived += (frame, msg, transferID) => {
      if (msg is DroneCAN.DroneCAN.uavcan_protocol_NodeStatus ns) {
        if (frame.SourceNode == can.SourceNode) {
          return;
        }
        RequestNodeInfoIfUnknown(can, frame.SourceNode);
        PostForSession(can, target, revision, () => {
          UpsertNodeStatus(frame.SourceNode, ns);
        });
      } else if (msg is DroneCAN.DroneCAN.uavcan_protocol_GetNodeInfo_res gnires) {
        PostForSession(can, target, revision, () => {
          foreach (var item in Nodes.Where(n => n.Id == frame.SourceNode)) {
            item.Name = Encoding.ASCII.GetString(gnires.name, 0, gnires.name_len);
            item.SoftwareVersion = gnires.software_version.major + "." + gnires.software_version.minor +
                                   "." + gnires.software_version.vcs_commit.ToString("X");
            item.SoftwareCrc = gnires.software_version.image_crc.ToString("X");
            item.SoftwareVcsCommit = gnires.software_version.vcs_commit;
            item.HardwareVersion = gnires.hardware_version.major + "." + gnires.hardware_version.minor;
            item.HardwareUid = string.Join(" ",
                gnires.hardware_version.unique_id.Select(b => b.ToString("X2")));
            item.VendorSpecificCode = gnires.status.vendor_specific_status_code.ToString();
          }
        });
      } else if (msg is DroneCAN.DroneCAN.uavcan_protocol_debug_LogMessage dbg) {
        PostForSession(can, target, revision, () => {
          DebugLog.Insert(0, new DroneCanLog {
            Node = frame.SourceNode.ToString(),
            Level = dbg.level.value.ToString(),
            Source = Encoding.ASCII.GetString(dbg.source, 0, dbg.source_len),
            Text = Encoding.ASCII.GetString(dbg.text, 0, dbg.text_len),
          });
          while (DebugLog.Count > 100) {
            DebugLog.RemoveAt(DebugLog.Count - 1);
          }
        });
      } else if (msg is DroneCAN.DroneCAN.dronecan_protocol_Stats st) {
        PostForSession(can, target, revision, () => {
          if (StatsLogging) {
            AppendStat(frame.SourceNode,
                $"tx={st.tx_frames} txerr={st.tx_errors} rx={st.rx_frames} crc_err={st.rx_error_bad_crc}");
          }
        });
      } else if (msg is DroneCAN.DroneCAN.dronecan_protocol_CanStats cs) {
        PostForSession(can, target, revision, () => {
          if (StatsLogging) {
            AppendStat(frame.SourceNode,
                $"if{cs.@interface} tx_req={cs.tx_requests} tx_ok={cs.tx_success} "
                + $"rx={cs.rx_received} busoff={cs.busoff_errors}");
          }
        });
      }
    };

    if (LogToFile) {
      try {
        can.LogFile = BuildLogPath();
      } catch {
      }
    }

    _ = Task.Run(() => {
      try {
        if (openTransport && !transport.IsOpen) {
          transport.Open();
        }
        if (!IsCanSessionCurrent(can, target, revision)) {
          try {
            transport.Close();
          } catch {
          }
          return;
        }
        can.StartSLCAN(transport.BaseStream);
        if (!IsCanSessionCurrent(can, target, revision)) {
          return;
        }
        can.SetupFileServer();
        can.SetupDynamicNodeAllocator();
        PostForSession(can, target, revision, () => {
          IsBusy = false;
          Status = connectedStatus;
        });
      } catch (Exception ex) {
        PostForSession(can, target, revision,
            () => Disconnect((openTransport ? "SLCAN start failed: " : "CAN start failed: ")
                + ex.Message));
      }
    });
  }

  private DroneCAN.DroneCAN CreateCanProtocol() {
    DroneCAN.DroneCAN can = _canFactory();
    can.SourceNode = 127;
    _autopilotNodeId = null;
    return can;
  }

  private void UpsertNodeStatus(
      byte id, DroneCAN.DroneCAN.uavcan_protocol_NodeStatus status) {
    DroneCanNode? node = Nodes.FirstOrDefault(item => item.Id == id);
    if (node == null) {
      Nodes.Add(new DroneCanNode {
        Id = id,
        Name = "?",
        IsAutopilot = id == _autopilotNodeId,
        Health = HealthString(status.health),
        Mode = ModeString(status.mode),
        Uptime = TimeSpan.FromSeconds(status.uptime_sec),
      });
      return;
    }

    node.Health = HealthString(status.health);
    node.Mode = ModeString(status.mode);
    node.Uptime = TimeSpan.FromSeconds(status.uptime_sec);
  }

  private void PostForSession(
      DroneCAN.DroneCAN can,
      DroneCanSessionTarget? target,
      long revision,
      Action action) {
    Dispatcher.UIThread.Post(() => {
      if (IsCanSessionCurrent(can, target, revision)) {
        action();
      }
    });
  }

  private void AppendStat(byte node, string text) {
    DebugLog.Insert(0, new DroneCanLog {
      Node = node.ToString(),
      Level = "STAT",
      Source = "stats",
      Text = text,
    });
    while (DebugLog.Count > 100) {
      DebugLog.RemoveAt(DebugLog.Count - 1);
    }
  }

  private static string HealthString(byte health) {
    return health switch {
      (byte)DroneCAN.DroneCAN.uavcan_protocol_NodeStatus.UAVCAN_PROTOCOL_NODESTATUS_HEALTH_OK => "OK",
      (byte)DroneCAN.DroneCAN.uavcan_protocol_NodeStatus.UAVCAN_PROTOCOL_NODESTATUS_HEALTH_WARNING => "WARNING",
      (byte)DroneCAN.DroneCAN.uavcan_protocol_NodeStatus.UAVCAN_PROTOCOL_NODESTATUS_HEALTH_ERROR => "ERROR",
      (byte)DroneCAN.DroneCAN.uavcan_protocol_NodeStatus.UAVCAN_PROTOCOL_NODESTATUS_HEALTH_CRITICAL => "CRITICAL",
      _ => health.ToString(),
    };
  }

  private static string ModeString(byte mode) {
    return mode switch {
      (byte)DroneCAN.DroneCAN.uavcan_protocol_NodeStatus.UAVCAN_PROTOCOL_NODESTATUS_MODE_OPERATIONAL => "OPERATIONAL",
      (byte)DroneCAN.DroneCAN.uavcan_protocol_NodeStatus.UAVCAN_PROTOCOL_NODESTATUS_MODE_INITIALIZATION => "INITIALIZATION",
      (byte)DroneCAN.DroneCAN.uavcan_protocol_NodeStatus.UAVCAN_PROTOCOL_NODESTATUS_MODE_MAINTENANCE => "MAINTENANCE",
      (byte)DroneCAN.DroneCAN.uavcan_protocol_NodeStatus.UAVCAN_PROTOCOL_NODESTATUS_MODE_SOFTWARE_UPDATE => "SOFTWARE_UPDATE",
      (byte)DroneCAN.DroneCAN.uavcan_protocol_NodeStatus.UAVCAN_PROTOCOL_NODESTATUS_MODE_OFFLINE => "OFFLINE",
      _ => mode.ToString(),
    };
  }

  private void Disconnect(string reason) {
    Interlocked.Increment(ref _targetRevision);
    _mavlinkCanRun = false;
    CancelCurrentOperation();

    DroneCAN.DroneCAN? can = _can;
    MavlinkCanTransport? transport = _transport;
    ICommsSerial? directPort = _directPort;
    IDroneCanMulticastSession? multicastSession = _multicastSession;
    _can = null;
    _transport = null;
    _directPort = null;
    _multicastSession = null;
    _sessionTarget = null;
    _sessionRequiresVehicleTarget = false;
    _nodeInfoRequests.Clear();

    // Unsubscribes from CAN_FRAME and releases the forwarding lease before the stack stops.
    try {
      transport?.Dispose();
    } catch {
    }

    // Stop UDP receive/send before stopping DroneCAN. This prevents its final virtual SLCAN close
    // command from being interpreted as network traffic and guarantees that the shared port is free.
    try {
      multicastSession?.Stop();
    } catch {
    }

    try {
      can?.Stop(ExitSlcanOnLeave);
    } catch {
    }

    // If Exit SLCAN is off, Stop(false) deliberately leaves the adapter in SLCAN mode. We still
    // release our host serial handle so another process or a later Mission Planner session can use it.
    try {
      directPort?.Close();
    } catch {
    }
    try {
      (directPort as IDisposable)?.Dispose();
    } catch {
    }
    try {
      multicastSession?.Dispose();
    } catch {
    }

    IsConnected = false;
    IsBusy = false;
    ClearVehicleState();
    Status = reason;
    NodeStatus = reason;
  }

  private void ClearVehicleState() {
    SelectedNode = null;
    Nodes.Clear();
    _allNodeParams.Clear();
    NodeParams.Clear();
    DebugLog.Clear();
  }

  private DroneCanOperation? TryBeginNodeOperation() {
    if (IsBusy) {
      NodeStatus = "Another DroneCAN operation is already running.";
      return null;
    }

    DroneCAN.DroneCAN? can = _can;
    DroneCanNode? node = SelectedNode;
    DroneCanSessionTarget? target = _sessionTarget;
    long revision = Volatile.Read(ref _targetRevision);
    if (can == null || node == null ||
        !IsCanSessionCurrent(can, target, revision)) {
      NodeStatus = _targetInvalidated ? TargetChangedMessage :
          "Connect and select a node first.";
      return null;
    }

    var cancellation = new CancellationTokenSource();
    CancellationTokenSource? previous =
        Interlocked.Exchange(ref _operationCancellation, cancellation);
    CancelSafely(previous);
    var operation = new DroneCanOperation(
        can, node.Id, Volatile.Read(ref _nodeRevision), target, revision, cancellation);
    if (!IsOperationCurrent(operation)) {
      CompleteOperation(operation);
      NodeStatus = TargetChangedMessage;
      return null;
    }
    IsBusy = true;
    return operation;
  }

  private bool IsOperationCurrent(DroneCanOperation operation) =>
      !operation.Cancellation.IsCancellationRequested &&
      ReferenceEquals(Volatile.Read(ref _operationCancellation), operation.Cancellation) &&
      ReferenceEquals(_can, operation.Can) && _mavlinkCanRun &&
      IsSessionCurrent(operation.Target, operation.Revision) &&
      operation.NodeRevision == Volatile.Read(ref _nodeRevision);

  private void CompleteOperation(DroneCanOperation operation) {
    bool current = ReferenceEquals(
        Interlocked.CompareExchange(
            ref _operationCancellation, null, operation.Cancellation),
        operation.Cancellation);
    if (current && !_disposed) {
      IsBusy = false;
    }
    operation.Cancellation.Dispose();
  }

  private void CancelCurrentOperation() {
    CancellationTokenSource? cancellation =
        Interlocked.Exchange(ref _operationCancellation, null);
    CancelSafely(cancellation);
  }

  private static void CancelSafely(CancellationTokenSource? cancellation) {
    try {
      cancellation?.Cancel();
    } catch (ObjectDisposedException) {
    }
  }

  private bool HasCurrentCanSession() {
    DroneCAN.DroneCAN? can = _can;
    DroneCanSessionTarget? target = _sessionTarget;
    return IsConnected && can != null &&
        IsCanSessionCurrent(can, target, Volatile.Read(ref _targetRevision));
  }

  private DroneCanSelection? CaptureSelection() {
    DroneCanSessionTarget? target = _sessionTarget;
    DroneCanNode? node = SelectedNode;
    long revision = Volatile.Read(ref _targetRevision);
    return node != null && HasCurrentCanSession() && _can != null
        ? new DroneCanSelection(
            _can, target, revision, node.Id, Volatile.Read(ref _nodeRevision))
        : null;
  }

  private bool IsSelectionCurrent(DroneCanSelection selection) =>
      ReferenceEquals(_can, selection.Can) &&
      IsSessionCurrent(selection.Target, selection.Revision) &&
      selection.NodeRevision == Volatile.Read(ref _nodeRevision);

  private bool IsCanSessionCurrent(
      DroneCAN.DroneCAN can,
      DroneCanSessionTarget? target,
      long revision) =>
      ReferenceEquals(_can, can) && IsSessionCurrent(target, revision);

  private bool IsSessionCurrent(DroneCanSessionTarget? target, long revision) {
    if (_disposed || !_mavlinkCanRun || _targetInvalidated ||
        revision != Volatile.Read(ref _targetRevision) ||
        !TargetsMatch(_sessionTarget, target)) {
      return false;
    }
    return !_sessionRequiresVehicleTarget ||
        (target != null && TargetsMatch(CaptureActiveTarget(), target));
  }

  internal static bool TargetsMatch(
      DroneCanSessionTarget? expected, DroneCanSessionTarget? current) => expected == current;

  internal static bool ShouldAcceptSessionResult(
      bool invalidated,
      long capturedRevision,
      long currentRevision,
      DroneCanSessionTarget? expected,
      DroneCanSessionTarget? current) =>
      !invalidated && capturedRevision == currentRevision && expected != null &&
      TargetsMatch(expected, current);

  internal static bool ShouldAcceptNodeBoundResult(
      bool invalidated,
      long capturedTargetRevision,
      long currentTargetRevision,
      DroneCanSessionTarget? expected,
      DroneCanSessionTarget? current,
      long capturedNodeRevision,
      long currentNodeRevision) =>
      ShouldAcceptSessionResult(
          invalidated, capturedTargetRevision, currentTargetRevision, expected, current) &&
      capturedNodeRevision == currentNodeRevision;

  private static DroneCanSessionTarget? CaptureAppStateTarget() {
    MAVLinkInterface link = AppState.comPort;
    return link.BaseStream?.IsOpen == true
        ? new DroneCanSessionTarget(link, link.MAV.sysid, link.MAV.compid)
        : null;
  }

  private DroneCanSessionTarget? CaptureActiveTarget() => _activeTarget();

  private void OnConnectionChanged() {
    if (_disposed) {
      return;
    }
    DroneCanSessionTarget? current = CaptureActiveTarget();
    if (TargetsMatch(_observedTarget, current)) {
      return;
    }

    _observedTarget = current;
    if (IsConnected && !_sessionRequiresVehicleTarget) {
      // A direct SLCAN adapter is independent of whichever MAVLink modem/vehicle is selected.
      return;
    }
    _targetInvalidated = true;
    Interlocked.Increment(ref _targetRevision);
    _mavlinkCanRun = false;
    CancelCurrentOperation();
    if (Dispatcher.UIThread.CheckAccess()) {
      InvalidateForTargetChange();
    } else {
      Dispatcher.UIThread.Post(InvalidateForTargetChange);
    }
  }

  private void InvalidateForTargetChange() {
    if (_disposed) {
      return;
    }
    Disconnect(TargetChangedMessage);
  }

  internal void SynchronizeActiveTarget() => OnConnectionChanged();

  public void Dispose() {
    if (_disposed) {
      return;
    }
    if (_subscribedToAppState) {
      AppState.ConnectionChanged -= OnConnectionChanged;
    }
    Disconnect("Disconnected.");
    _disposed = true;
  }

  private const string TargetChangedMessage =
      "The active modem or vehicle changed. DroneCAN was disconnected and all old nodes, "
      + "parameters, logs and late operation results were cleared.";
}

internal sealed record DroneCanSessionTarget(
    MAVLinkInterface Link, byte SystemId, byte ComponentId);

internal sealed record DroneCanOperation(
    DroneCAN.DroneCAN Can,
    byte NodeId,
    long NodeRevision,
    DroneCanSessionTarget? Target,
    long Revision,
    CancellationTokenSource Cancellation);

internal sealed record DroneCanSelection(
    DroneCAN.DroneCAN Can,
    DroneCanSessionTarget? Target,
    long Revision,
    byte NodeId,
    long NodeRevision);

internal sealed record SlcanPreparationResult(bool Success, string Message);

public partial class DroneCanNode : ObservableObject {
  [ObservableProperty]
  private byte _id;

  [ObservableProperty]
  private string _name = "?";

  [ObservableProperty]
  private string _health = "";

  [ObservableProperty]
  private string _mode = "";

  [ObservableProperty]
  private TimeSpan _uptime;

  [ObservableProperty]
  private string _hardwareVersion = "";

  [ObservableProperty]
  private string _softwareVersion = "";

  [ObservableProperty]
  private string _softwareCrc = "";

  [ObservableProperty]
  private string _hardwareUid = "";

  [ObservableProperty]
  private string _vendorSpecificCode = "";

  /// <summary>Raw vcs_commit from GetNodeInfo, compared with manifest git hashes.</summary>
  public uint SoftwareVcsCommit { get; set; }

  /// <summary>True when this id is the autopilot's own DroneCAN node (CAN_Dx_UC_NODE) on the tunnelled bus.</summary>
  [ObservableProperty]
  private bool _isAutopilot;

  /// <summary>Grid text: the autopilot is named even before (or without) a GetNodeInfo answer.</summary>
  public string DisplayName => IsAutopilot
      ? (Name == "?" ? "autopilot (no node info on this port)" : Name + " (autopilot)")
      : Name;

  partial void OnNameChanged(string value) => OnPropertyChanged(nameof(DisplayName));

  partial void OnIsAutopilotChanged(bool value) => OnPropertyChanged(nameof(DisplayName));
}

public partial class DroneCanLog : ObservableObject {
  [ObservableProperty]
  private string _node = "";

  [ObservableProperty]
  private string _level = "";

  [ObservableProperty]
  private string _source = "";

  [ObservableProperty]
  private string _text = "";
}

public partial class DroneCanParam : ObservableObject {
  [ObservableProperty]
  private string _name = "";

  [ObservableProperty]
  private string _value = "";

  [ObservableProperty]
  private string _min = "";

  [ObservableProperty]
  private string _max = "";

  [ObservableProperty]
  private string _default = "";

  [ObservableProperty]
  private bool _isFav;

  public bool IsString { get; init; }

  public string OriginalValue { get; set; } = "";

  public bool IsDirty => !string.Equals(Value, OriginalValue, StringComparison.Ordinal);

  public string FavoriteMarker => IsFav ? "★" : "☆";

  partial void OnValueChanged(string value) => OnPropertyChanged(nameof(IsDirty));

  partial void OnIsFavChanged(bool value) => OnPropertyChanged(nameof(FavoriteMarker));

  public void AcceptValue() {
    OriginalValue = Value;
    OnPropertyChanged(nameof(IsDirty));
  }
}
