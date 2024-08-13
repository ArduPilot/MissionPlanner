using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using MissionPlanner.Controls;
using MissionPlanner.Plugin;
using MissionPlanner.Utilities;
using Newtonsoft.Json.Linq;

namespace Carbonix
{
    public partial class RecordsTab : UserControl
    {
        private readonly PluginHost Host;
        private static readonly ILog log = LogManager.GetLogger(typeof(RecordsTab));
        private readonly string weather_api_key;
        private Task<(bool, string)> metarTask = null;

        public RecordsTab(PluginHost Host, GeneralSettings settings, AircraftSettings aircraft_settings)
        {
            this.Host = Host;
            this.weather_api_key = settings.weather_api_key;
            InitializeComponent();
            foreach (var pilot in aircraft_settings.pilots)
            {
                cmb_pic.Items.Add(pilot);
                cmb_gso.Items.Add(pilot);
            }
            
            foreach (var payload in aircraft_settings.payloads)
            {
                cmb_payload.Items.Add(payload);
            }

            foreach (var location in settings.pilot_locations)
            {
                cmb_location.Items.Add(location);
            }

            // Hide the avionics battery row if the aircraft does not have one
            if (!aircraft_settings.has_avionics_battery)
            {
                num_avbatid.Visible = false;
                var rowIndex = tableLayoutPanelOuter.GetRow(num_avbatid);
                tableLayoutPanelOuter.RowStyles[rowIndex].SizeType = SizeType.Absolute;
                tableLayoutPanelOuter.RowStyles[rowIndex].Height = 0;
            }
            
        }

        private DateTime last_attempt = DateTime.MinValue;
        private DateTime last_success = DateTime.MinValue;
        private PointLatLngAlt last_location = PointLatLngAlt.Zero;

        /// <summary>
        /// Check if it is time to fetch the METAR for the closest air field. And update the METAR text box if needed.
        /// </summary>
        /// <returns></returns>
        public void UpdateMETAR()
        {
            // If the fetch task has completed, get the results
            if (metarTask?.IsCompleted == true)
            {
                bool success = false;
                string metar;

                // log any exceptions that occurred during the fetch
                if (metarTask.Exception != null)
                {
                    log.Error(metarTask.Exception);
                    metar = metarTask.Exception.InnerException.Message;
                }
                else
                {
                    (success, metar) = metarTask.Result;
                }
                metarTask = null;

                // If successful update the text box
                if (success)
                {
                    txt_metar.BeginInvokeIfRequired(() => txt_metar.Text = metar);
                    last_success = DateTime.UtcNow;
                    last_location = Host.cs.Location;
                }
                else
                {
                    log.Error(metar);
                }

                // If we haven't been successful in the last 30 minutes, write the error to the METAR box
                if (last_success.AddMinutes(30) < DateTime.UtcNow)
                {
                    txt_metar.BeginInvokeIfRequired(() => txt_metar.Text = metar);
                }
            }

            // Rate limit to once every 15 seconds
            if (last_attempt.AddSeconds(15) > DateTime.UtcNow)
            {
                return;
            }

            // If auto METAR is disabled, don't fetch
            if (!chk_auto_metar.Checked)
            {
                return;
            }

            // If we are not connected to the vehicle, or if we are armed, don't fetch
            if (Host.comPort?.BaseStream?.IsOpen == false || Host.cs.armed)
            {
                return;
            }

            // If we don't have a valid GPS fix, don't fetch
            if (Math.Max(Host.cs.gpsstatus, Host.cs.gpsstatus2) < (byte)MAVLink.GPS_FIX_TYPE._3D_FIX)
            {
                return;
            }

            // If we are at a drastically different location, or if the observation is stale, fetch
            if (last_location == PointLatLngAlt.Zero || Host.cs.Location.GetDistance(last_location) > 1000 || last_success.AddMinutes(5) < DateTime.UtcNow)
            {
                metarTask = FetchMETAR();
            }
        }

        int unauthorized_count = 0;
        /// <summary>
        /// Asynchonously fetch the METAR for the closest airfield
        /// </summary>
        /// <returns>success/error flag and string value or error message</returns>
        private async Task<(bool, string)> FetchMETAR()
        {
            last_attempt = DateTime.UtcNow;

            // If we have been unauthorized more than 5 times, it's not likely that will change
            if (unauthorized_count > 5)
            {
                return (false, "Invalid API Key");
            }

            var location = Host.cs.Location;
            var url = $"https://api.checkwx.com/metar/lat/{location.Lat:0.0000}/lon/{location.Lng:0.0000}";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-API-Key", weather_api_key);
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                unauthorized_count = 0;
                var content = await response.Content.ReadAsStringAsync();
                try
                {
                    var metar = JObject.Parse(content)["data"][0].ToString();
                    return (true, metar);
                }
                catch(Exception e)
                {
                    log.Error(e);
                    return (false, "Could not parse HTTP response");
                }
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    unauthorized_count++;
                }
                log.Error($"HTTP Error: {response.ReasonPhrase}");
                return (false, response.ReasonPhrase);
            }
        }

        /// <summary>
        /// Return a list of strings to send to the aircraft as statustext messages
        /// </summary>
        public List<string> GetRecords()
        {
            // Check if invoke is required, and call the delegate on the UI thread if needed
            if (InvokeRequired)
            {
                return (List<string>)Invoke(new Func<List<string>>(GetRecordsDelegate));
            }
            else
            {
                return GetRecordsDelegate();
            }
        }

        /// <summary>
        /// Delegate to get the records from the UI elements
        /// </summary>
        private List<string> GetRecordsDelegate()
        {
            List<string> messages = new List<string>()
            {
                $"PIC:{cmb_pic.Text}",
                $"GSO:{cmb_gso.Text}",
                $"LOC:{cmb_location.Text}",
                $"OP:{cmb_operation.Text}:{cmb_vlos.Text}",
                $"VTOLBAT:{num_vtolbatid.Value}",
            };

            if (num_avbatid.Visible)
            {
                messages.Add($"AVBAT:{num_avbatid.Value}");
            }

            if (txt_payload_serial.Text != "")
            {
                messages.Add($"PLD:{cmb_payload.Text}:{txt_payload_serial.Text}");
            }
            else
            {
                messages.Add($"PLD:{cmb_payload.Text}");
            }

            // For the METAR, break into max of 35 character lines, breaking on spaces in the text
            var metar = txt_metar.Text;
            int line_number = 1;
            string this_line = "";
            foreach (var word in metar.Split(' '))
            {
                if (this_line == "")
                {
                    this_line = $"WX{line_number++}:{word}";
                    continue;
                }
                if (this_line.Length + word.Length + 1 > 35)
                {
                    messages.Add(this_line);
                    this_line = $"WX{line_number++}:{word}";
                }
                else
                {
                    this_line += $" {word}";
                }
            }

            return messages;
        }

        private void RecordsTab_VisibleChanged(object sender, EventArgs e)
        {
            bool enabled = !Host.cs.armed || !Host.comPort.BaseStream.IsOpen;
            tableLayoutPanelOuter.Enabled = enabled;
        }

        private void chk_auto_metar_CheckedChanged(object sender, EventArgs e)
        {
            var chk = sender as CheckBox;
            txt_metar.ReadOnly = chk.Checked;
        }
    }
}
