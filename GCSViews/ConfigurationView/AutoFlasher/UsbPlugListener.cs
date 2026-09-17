using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews.ConfigurationView.AutoFlasher
{
    public sealed class UsbDeviceArrivedEventArgs : EventArgs
    {
        public UsbId Match { get; }
        public string ComPort { get; }
        public string DeviceId { get; }

        public UsbDeviceArrivedEventArgs(UsbId match, string com, string deviceId)
        {
            Match = match;
            ComPort = com;
            DeviceId = deviceId;
        }
    }

    /// <summary>
    /// Hybrid USB plug listener: native WM_DEVICECHANGE for instant arrival
    /// notification + 250 ms WMI safety poll for cases where the device was
    /// already enumerated before subscription. All callbacks are dispatched on
    /// the SynchronizationContext captured at construction (UI thread).
    /// </summary>
    public sealed class UsbPlugListener : IDisposable
    {
        private const int WM_DEVICECHANGE = 0x0219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVTYP_DEVICEINTERFACE = 0x00000005;

        private static readonly Guid GUID_DEVINTERFACE_USB_DEVICE =
            new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED");

        private readonly NotifyWindow _window;
        private readonly SynchronizationContext _sync;
        private readonly System.Threading.Timer _safetyPoll;
        private readonly object _gate = new object();
        private string _lastDeviceIdRaised;
        private bool _disposed;

        public event EventHandler<UsbDeviceArrivedEventArgs> DeviceArrived;

        public UsbPlugListener()
        {
            _sync = SynchronizationContext.Current ?? new SynchronizationContext();
            _window = new NotifyWindow(OnNativeDeviceChange);
            _window.CreateHandle(new CreateParams());
            try { RegisterForUsbNotifications(_window.Handle); }
            catch (Exception ex) { Debug.WriteLine($"[AutoFlasher] RegisterDeviceNotification failed: {ex.Message}"); }

            _safetyPoll = new System.Threading.Timer(_ => SafeScan(), null, 250, 250);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            try { _safetyPoll?.Dispose(); } catch { }
            try { _window?.DestroyHandle(); } catch { }
        }

        private void OnNativeDeviceChange(int wParam)
        {
            if (wParam != DBT_DEVICEARRIVAL) return;
            // Never block the message pump with WMI work.
            Task.Run(() => SafeScan());
        }

        private void SafeScan()
        {
            try
            {
                if (_disposed) return;
                ScanForBootloader();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AutoFlasher] scan error: {ex.Message}");
            }
        }

        private void ScanForBootloader()
        {
            using (var searcher = new ManagementObjectSearcher(
                "SELECT DeviceID FROM Win32_PnPEntity WHERE DeviceID LIKE 'USB%'"))
            using (var collection = searcher.Get())
            {
                foreach (ManagementObject mo in collection)
                {
                    var deviceId = mo["DeviceID"] as string ?? "";
                    var match = MatchVidPid(deviceId);
                    if (match == null) continue;

                    lock (_gate)
                    {
                        if (deviceId == _lastDeviceIdRaised) return;
                        _lastDeviceIdRaised = deviceId;
                    }

                    var com = TryGetComPort(deviceId);
                    RaiseArrived(match, com, deviceId);
                    return;
                }
            }
        }

        private static UsbId MatchVidPid(string deviceId)
        {
            var m = Regex.Match(deviceId, @"VID_([0-9A-F]{4})&PID_([0-9A-F]{4})",
                RegexOptions.IgnoreCase);
            if (!m.Success) return null;

            ushort vid = Convert.ToUInt16(m.Groups[1].Value, 16);
            ushort pid = Convert.ToUInt16(m.Groups[2].Value, 16);

            // FirstOrDefault — never First — to avoid "Sequence contains no elements".
            return BootloaderTargets.Known.FirstOrDefault(t => t.Vid == vid && t.Pid == pid);
        }

        private static string TryGetComPort(string deviceId)
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                    $@"SYSTEM\CurrentControlSet\Enum\{deviceId}\Device Parameters"))
                {
                    return key?.GetValue("PortName") as string;
                }
            }
            catch
            {
                return null;
            }
        }

        private void RaiseArrived(UsbId match, string com, string deviceId)
        {
            var handler = DeviceArrived;
            if (handler == null) return;
            _sync.Post(_ => handler(this, new UsbDeviceArrivedEventArgs(match, com, deviceId)), null);
        }

        private static void RegisterForUsbNotifications(IntPtr hwnd)
        {
            var dbi = new DEV_BROADCAST_DEVICEINTERFACE
            {
                dbcc_size = Marshal.SizeOf(typeof(DEV_BROADCAST_DEVICEINTERFACE)),
                dbcc_devicetype = DBT_DEVTYP_DEVICEINTERFACE,
                dbcc_classguid = GUID_DEVINTERFACE_USB_DEVICE
            };
            var buffer = Marshal.AllocHGlobal(dbi.dbcc_size);
            try
            {
                Marshal.StructureToPtr(dbi, buffer, true);
                RegisterDeviceNotification(hwnd, buffer, 0);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DEV_BROADCAST_DEVICEINTERFACE
        {
            public int dbcc_size;
            public int dbcc_devicetype;
            public int dbcc_reserved;
            public Guid dbcc_classguid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            public string dbcc_name;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient,
            IntPtr notificationFilter, int flags);

        private sealed class NotifyWindow : NativeWindow
        {
            private readonly Action<int> _onDeviceChange;
            public NotifyWindow(Action<int> onDeviceChange) { _onDeviceChange = onDeviceChange; }
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_DEVICECHANGE) _onDeviceChange((int)m.WParam);
                base.WndProc(ref m);
            }
        }
    }
}
