using System;
using System.Drawing;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.Plugin;

namespace GridFlight
{
    /// <summary>
    /// Añade el botón "Write and Verify" al panel de acciones rápidas de FlightPlanner.
    ///
    /// Al pulsarlo ejecuta dos operaciones en secuencia:
    ///   1. BUT_write_Click — envía la misión al dron.
    ///   2. BUT_read_Click  — lee la misión de vuelta para confirmar que fue
    ///                        almacenada correctamente.
    ///
    /// Por qué aquí y no en FlightPlanner.Designer.cs + FlightPlanner.cs:
    ///   - Host.MainForm.FlightPlanner es un campo public (MainV2.cs:582).
    ///   - FlightPlanner ya está instanciado cuando Loaded() corre: el objeto se
    ///     crea en MainV2.cs línea 855 y LoadAll() se llama en línea 3199 (después).
    ///   - panel5, BUT_write y BUT_read son public (FlightPlanner.Designer.cs:1609-1612).
    ///   - BUT_write_Click y BUT_read_Click son public (FlightPlanner.cs:645, 606),
    ///     lo cual también está documentado en PluginHost.GetWPs() (Plugin.cs:248).
    ///   Con estos accesos no hace falta tocar ningún archivo fuente de ArduPilot.
    /// </summary>
    public class WriteVerifyPlugin : MissionPlanner.Plugin.Plugin
    {
        public override string Name    => "GridFlight - Write and Verify";
        public override string Version => "1.0";
        public override string Author  => "GridFlight";

        public override bool Init() => true;

        public override bool Loaded()
        {
            AddWriteVerifyButton();
            return false; // Sin loop periódico
        }

        public override bool Exit() => true;

        /// <summary>
        /// Crea el botón con las mismas propiedades que tenía en FlightPlanner.resx
        /// (Location 3,90 · Size 115,23 · TabIndex 10) y lo añade a panel5.
        /// </summary>
        private void AddWriteVerifyButton()
        {
            var fp  = Host.MainForm.FlightPlanner;
            var btn = new MyButton
            {
                Name                    = "btnWriteVerify",
                Text                    = "Write and Verify",
                Location                = new Point(3, 90),
                Size                    = new Size(115, 23),
                TabIndex                = 10,
                TextColorNotEnabled     = Color.FromArgb(64, 87, 4),
                UseVisualStyleBackColor = true,
            };

            btn.Click += OnWriteVerifyClick;
            fp.panel5.Controls.Add(btn);
        }

        /// <summary>
        /// Escribe la misión al dron y la lee de vuelta para verificar la recepción.
        /// Delega directamente en los métodos públicos de FlightPlanner en lugar de
        /// simular clics de botón, evitando dependencia en controles de UI internos.
        /// </summary>
        private void OnWriteVerifyClick(object sender, EventArgs e)
        {
            var fp = Host.MainForm.FlightPlanner;
            try
            {
                fp.BUT_write_Click(sender, e);
                fp.BUT_read_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error de comunicación: " + ex.Message,
                    "Write and Verify",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
