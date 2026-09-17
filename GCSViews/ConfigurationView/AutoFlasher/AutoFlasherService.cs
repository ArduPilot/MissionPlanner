using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using px4uploader;

namespace MissionPlanner.GCSViews.ConfigurationView.AutoFlasher
{
    /// <summary>
    /// Orchestrates: pick firmware -> wait for USB plug -> race to open the
    /// virtual COM port and run the px4 bootloader sync before the user
    /// firmware boots and steals the port.
    /// </summary>
    public sealed class AutoFlasherService : IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AutoFlasherService));

        private readonly string _firmwarePath;
        private UsbPlugListener _listener;
        private CancellationTokenSource _cts;
        private int _busy;

        public event Action<string> Status;
        public event Action<double> Progress; // 0..100
        public event Action<bool, string> Completed;

        public AutoFlasherService(string firmwarePath)
        {
            if (string.IsNullOrWhiteSpace(firmwarePath))
                throw new ArgumentException("firmware path required", nameof(firmwarePath));
            if (!File.Exists(firmwarePath))
                throw new FileNotFoundException("Firmware file not found", firmwarePath);
            _firmwarePath = firmwarePath;
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            _listener = new UsbPlugListener();
            _listener.DeviceArrived += OnArrived;
            Status?.Invoke("En attente du branchement USB de la carte…");
        }

        public void Cancel()
        {
            try { _cts?.Cancel(); } catch { }
            try { _listener?.Dispose(); } catch { }
        }

        public void Dispose() => Cancel();

        private async void OnArrived(object sender, UsbDeviceArrivedEventArgs e)
        {
            // Single-shot guard: many WMI/WM_DEVICECHANGE events can fire in a row.
            if (Interlocked.CompareExchange(ref _busy, 1, 0) != 0) return;

            try
            {
                Status?.Invoke($"Détecté : {e.Match.Description}. Tentative d'interception bootloader…");
                log.Info($"AutoFlasher arrived: {e.Match} comPort={e.ComPort} deviceId={e.DeviceId}");

                var token = _cts?.Token ?? CancellationToken.None;
                await Task.Run(() => RunFlash(e, token), token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                Completed?.Invoke(false, "Annulé.");
            }
            catch (TimeoutException)
            {
                Completed?.Invoke(false,
                    "Le bootloader n'a pas répondu à temps. Débranche, attends 2 s, rebranche.");
            }
            catch (UnauthorizedAccessException)
            {
                Completed?.Invoke(false,
                    "Le port est déjà ouvert par une autre application. Ferme toute connexion MAVLink puis réessaie.");
            }
            catch (Exception ex)
            {
                log.Error("AutoFlasher fatal", ex);
                Completed?.Invoke(false, $"Erreur : {ex.Message}");
            }
            finally
            {
                try { _listener?.Dispose(); } catch { }
            }
        }

        private void RunFlash(UsbDeviceArrivedEventArgs e, CancellationToken ct)
        {
            Firmware fw = Firmware.ProcessFirmware(_firmwarePath);

            // Window in which the bootloader is reachable on the virtual COM port
            // before user firmware boots and possibly hangs the link.
            var deadline = DateTime.UtcNow.AddSeconds(8);
            Uploader uploader = null;
            string winningPort = null;

            while (DateTime.UtcNow < deadline)
            {
                ct.ThrowIfCancellationRequested();

                var ports = SerialPort.GetPortNames();
                Array.Sort(ports);

                // Prefer the port pre-resolved from the registry if present.
                if (!string.IsNullOrEmpty(e.ComPort))
                {
                    if (TryIdentify(e.ComPort, out uploader))
                    {
                        winningPort = e.ComPort;
                        break;
                    }
                }

                foreach (var p in ports)
                {
                    if (string.Equals(p, e.ComPort, StringComparison.OrdinalIgnoreCase)) continue;
                    ct.ThrowIfCancellationRequested();
                    if (TryIdentify(p, out uploader))
                    {
                        winningPort = p;
                        break;
                    }
                }

                if (uploader != null) break;
                Thread.Sleep(150);
            }

            if (uploader == null)
                throw new TimeoutException("Bootloader unreachable.");

            using (uploader)
            {
                Status?.Invoke($"Bootloader synchronisé sur {winningPort} (board_id={uploader.board_type}). Flash en cours…");

                if (uploader.board_type != fw.board_id)
                {
                    if (!(uploader.board_type == 33 && fw.board_id == 9))
                    {
                        throw new InvalidOperationException(
                            $"Firmware incompatible : board={uploader.board_type}, fw={fw.board_id}");
                    }
                }

                uploader.ProgressEvent += pct => Progress?.Invoke(pct * 100.0);
                uploader.LogEvent += (msg, lvl) => log.Info($"[uploader] {msg}");
                uploader.ConfirmEvent += msg => true; // auto-confirm in autoflasher mode

                uploader.upload(fw);

                Completed?.Invoke(true, $"Flash terminé sur {winningPort}.");
            }
        }

        private static bool TryIdentify(string portName, out Uploader uploader)
        {
            uploader = null;
            try
            {
                var up = new Uploader(portName, 115200);
                try
                {
                    up.identify();
                    uploader = up;
                    return true;
                }
                catch
                {
                    try { up.close(); } catch { }
                    try { up.Dispose(); } catch { }
                    return false;
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Port grabbed by Windows; let the bootloader window proceed and retry.
                return false;
            }
            catch (IOException)
            {
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AutoFlasher] identify failed on {portName}: {ex.Message}");
                return false;
            }
        }
    }
}
