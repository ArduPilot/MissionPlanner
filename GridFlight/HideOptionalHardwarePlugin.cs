using System;
using System.Linq;
using System.Windows.Forms;
using MissionPlanner;
using MissionPlanner.GCSViews;
using MissionPlanner.Plugin;

namespace GridFlight
{
    /// <summary>
    /// Oculta todos los ítems de "Optional Hardware" (Setup → Optional Hardware)
    /// excepto "Motor Test", que es el único relevante para el flujo operativo de GridFlight.
    ///
    /// ─────────────────────────────────────────────────────────────────────────────
    /// TÉCNICA USADA — DOS MECANISMOS COMPLEMENTARIOS
    /// ─────────────────────────────────────────────────────────────────────────────
    ///
    /// 1. FLAGS DE DisplayConfiguration (en Init())
    ///    La mayoría de ítems en Optional Hardware se añaden condicionalmente en
    ///    InitialSetup.HardwareConfig_Load() chequeando MainV2.DisplayConfiguration.*
    ///    (ExtLibs/Utilities/DisplayView.cs). Init() corre ANTES de que el usuario
    ///    pueda navegar al tab Setup, por lo que poner estos flags a false garantiza
    ///    que nunca aparezcan en el menú lateral.
    ///
    ///    Ítems controlados por flags (todos → false):
    ///      displayRTKInject      → RTK/GPS Inject
    ///      displaySikRadio       → Sik Radio
    ///      displayGPSOrder       → CAN GPS Order
    ///      displayBattMonitor    → Battery Monitor + Battery Monitor 2 (mismo flag)
    ///      displayCAN            → DroneCAN/UAVCAN
    ///      displayJoystick       → Joystick
    ///      displayCompassMotorCalib → Compass/Motor Calibration
    ///      displayRangeFinder    → Range Finder
    ///      displayAirSpeed       → Airspeed
    ///      displayPx4Flow        → PX4Flow
    ///      displayOpticalFlow    → Optical Flow
    ///      displayOsd            → OSD
    ///      displayCameraGimbal   → Camera Gimbal
    ///      displayAntennaTracker → Antenna Tracker (aparece dos veces en el menú)
    ///      displayBluetooth      → Bluetooth Setup
    ///      displayParachute      → Parachute
    ///      displayEsp            → ESP8266 Setup
    ///      displayFFTSetup       → FFT Setup
    ///
    ///    displayMotorTest se deja en true (no se toca).
    ///
    /// 2. BACKSTAGEBVIEWPAGE.SHOW = false (en Loop())
    ///    "CubeID Update" NO tiene flag en DisplayView — se añade incondicionalmente
    ///    en InitialSetup.cs línea 254. Para ocultarlo se usa la propiedad Show de
    ///    BackstageViewPage (ExtLibs/Controls/BackstageView/BackstageViewPage.cs:25),
    ///    que el BackstageView ya comprueba en su render (BackstageView.cs:86, 219).
    ///
    ///    InitialSetup es lazy-loaded: MainSwitcher la instancia solo cuando el
    ///    usuario hace clic en SETUP (MainSwitcher.cs:152, 167). Por eso no se puede
    ///    acceder en Init() ni Loaded(). La solución es Loop() a 1 Hz:
    ///    - Busca el screen "HWConfig" en Host.MainForm.MyView.screens (public).
    ///    - Cuando Screen.Control es un InitialSetup con páginas ya cargadas,
    ///      invoca en el hilo UI para poner Show=false y refrescar el backstageView.
    ///    - Inmediatamente pone loopratehz = 0 para desactivar el loop sin trabajo.
    ///
    ///    InitialSetup.backstageView es "internal" (InitialSetup.Designer.cs:293),
    ///    accesible porque este plugin compila dentro del mismo ensamblado
    ///    (MissionPlanner.exe — InitPlugin("self"), PluginLoader.cs:308).
    ///
    /// ─────────────────────────────────────────────────────────────────────────────
    /// PARA RESTAURAR UN ÍTEM
    /// ─────────────────────────────────────────────────────────────────────────────
    ///    Flags: pon el flag correspondiente a true en Init() o elimina la línea.
    ///    CubeID: elimina el bloque de HideCubeIDPage o cambia la condición en Loop.
    /// </summary>
    public class HideOptionalHardwarePlugin : MissionPlanner.Plugin.Plugin
    {
        public override string Name    => "GridFlight - Hide Optional Hardware";
        public override string Version => "1.0";
        public override string Author  => "GridFlight";

        // ── Configuración del loop ───────────────────────────────────────────────
        // Se usa solo para detectar cuándo InitialSetup ha sido creado y así poder
        // ocultar CubeID Update (que no tiene flag en DisplayConfiguration).
        // Se desactiva tan pronto como el trabajo está hecho (loopratehz = 0).
        private const float _checkRateHz = 1.0f;

        /// <summary>
        /// Aplica los flags de visibilidad ANTES de que el usuario navegue al tab
        /// Setup. HardwareConfig_Load() leerá estos valores al construir el menú.
        /// </summary>
        public override bool Init()
        {
            var cfg = MainV2.DisplayConfiguration;

            // ── Items de Optional Hardware sin interés operativo ────────────────
            cfg.displayRTKInject        = false; // RTK/GPS Inject
            cfg.displaySikRadio         = false; // Sik Radio
            cfg.displayGPSOrder         = false; // CAN GPS Order
            cfg.displayBattMonitor      = false; // Battery Monitor + Battery Monitor 2
            cfg.displayCAN              = false; // DroneCAN/UAVCAN
            cfg.displayJoystick         = false; // Joystick
            cfg.displayCompassMotorCalib = false; // Compass/Motor Calibration
            cfg.displayRangeFinder      = false; // Range Finder
            cfg.displayAirSpeed         = false; // Airspeed
            cfg.displayPx4Flow          = false; // PX4Flow
            cfg.displayOpticalFlow      = false; // Optical Flow
            cfg.displayOsd              = false; // OSD
            cfg.displayCameraGimbal     = false; // Camera Gimbal
            cfg.displayAntennaTracker   = false; // Antenna Tracker (dos entradas)
            cfg.displayBluetooth        = false; // Bluetooth Setup
            cfg.displayParachute        = false; // Parachute
            cfg.displayEsp              = false; // ESP8266 Setup
            cfg.displayFFTSetup         = false; // FFT Setup

            // Motor Test: SE MANTIENE VISIBLE — es el único ítem requerido
            // cfg.displayMotorTest = true; (valor por defecto, no es necesario asignarlo)

            return true;
        }

        /// <summary>
        /// Activa el loop a 1 Hz para detectar cuándo InitialSetup es instanciada
        /// y poder ocultar CubeID Update, que no tiene flag en DisplayConfiguration.
        /// </summary>
        public override bool Loaded()
        {
            loopratehz = _checkRateHz;
            return true; // true = registrar en el loop activo de plugins
        }

        /// <summary>
        /// Comprueba (a 1 Hz) si InitialSetup ya ha sido creada y cargada.
        /// Cuando la encuentra, oculta CubeID Update en el hilo UI y desactiva el loop.
        ///
        /// Loop() corre en un hilo de fondo (ver MainV2.cs:2497-2537).
        /// Toda operación de UI se delega a BeginInvoke para ser seguro con los hilos.
        /// La condición loopratehz > 0 (MainV2.cs:2507) es lo que activa esta llamada.
        /// </summary>
        public override bool Loop()
        {
            var hwConfigScreen = Host.MainForm.MyView.screens
                .FirstOrDefault(s => s.Name == "HWConfig");

            if (hwConfigScreen?.Control is InitialSetup setup
                && setup.backstageView?.Pages.Count > 0)
            {
                // Desactivar el loop ANTES del BeginInvoke para evitar re-entradas
                loopratehz = 0;

                Host.MainForm.BeginInvoke((MethodInvoker)(() => HideCubeIDPage(setup)));
            }

            return true;
        }

        public override bool Exit() => true;

        /// <summary>
        /// Busca la página "CubeID Update" en el backstageView de InitialSetup y la
        /// oculta poniendo Show = false. BackstageView comprueba esta propiedad en su
        /// ciclo de pintado (BackstageView.cs:86 y 219), por lo que el botón desaparece
        /// en el siguiente repintado.
        /// Debe llamarse desde el hilo UI.
        /// </summary>
        /// <param name="setup">Instancia de InitialSetup ya activada.</param>
        private void HideCubeIDPage(InitialSetup setup)
        {
            foreach (var page in setup.backstageView.Pages)
            {
                if (page.LinkText == "CubeID Update")
                {
                    page.Show = false;
                    setup.backstageView.Refresh();
                    break;
                }
            }
        }
    }
}
