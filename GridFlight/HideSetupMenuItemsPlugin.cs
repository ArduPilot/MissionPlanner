using MissionPlanner;
using MissionPlanner.Utilities;

// ============================================================
// Plugin: GridFlight - Ocultar ítems avanzados del Setup
// ============================================================
// PROPÓSITO:
//   Oculta del menú "Mandatory Hardware" (Initial Setup) los ítems
//   FailSafe, HW ID y ADSB, que no son relevantes para el flujo
//   operativo estándar de GridFlight.
//
// CÓMO FUNCIONA:
//   Mission Planner escanea el propio ensamblado (exe) buscando
//   clases que hereden de Plugin ("self" init, línea 308 de
//   PluginLoader.cs). Este mecanismo es SÍNCRONO, a diferencia de
//   los .cs del directorio /plugins que se compilan en background.
//   Init() se ejecuta antes de que el usuario pueda abrir cualquier
//   pestaña, garantizando que los flags estén aplicados cuando
//   HardwareConfig_Load() construye el menú lateral.
//
// PARA RESTAURAR UN ÍTEM:
//   Pon su flag a true aquí, o elimina este archivo del proyecto.
//
// FLAGS DISPONIBLES en DisplayView (ExtLibs/Utilities/DisplayView.cs):
//   displayFailSafe, displayHWIDs, displayADSB
// ============================================================

namespace GridFlight
{
    public class HideSetupMenuItemsPlugin : MissionPlanner.Plugin.Plugin
    {
        public override string Name    => "GridFlight - Hide Setup Items";
        public override string Version => "1.0";
        public override string Author  => "GridFlight";

        /// <summary>
        /// Se ejecuta síncronamente durante el arranque de la aplicación,
        /// después de que DisplayConfiguration es cargado desde settings.xml
        /// y antes de que el usuario pueda navegar a ninguna pestaña.
        /// Es el punto correcto para sobrescribir flags de visibilidad.
        /// </summary>
        public override bool Init()
        {
            // FailSafe: comportamiento ante pérdida de señal RC/telemetría.
            // Oculto para simplificar el flujo del operador en campo.
            MainV2.DisplayConfiguration.displayFailSafe = false;

            // HW ID: identificador de hardware del controlador de vuelo.
            // No es necesario en operación normal.
            MainV2.DisplayConfiguration.displayHWIDs = false;

            // ADSB: sistema anticolisión por transponder ADS-B.
            // GridFlight gestiona la separación a nivel de flota.
            MainV2.DisplayConfiguration.displayADSB = false;

            return true; // Continuar al ciclo Loaded()
        }

        /// <summary>
        /// Los flags ya están aplicados desde Init().
        /// Devolvemos false para no registrarnos en el loop activo
        /// de plugins — no necesitamos ejecución periódica.
        /// </summary>
        public override bool Loaded()
        {
            return false;
        }

        public override bool Exit()
        {
            return true;
        }
    }
}
