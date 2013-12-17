using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionPlanner;
using MissionPlanner.Controls;
using int8_t = System.SByte;
using Vector3f = MissionPlanner.HIL.Vector3;
using uint32_t = System.UInt32;
using uint8_t = System.Byte;

namespace MissionPlanner
{
    public class MagMotor
    {
        const float COMPASS_MAGFIELD_EXPECTED   =   530 ;

        public enum comptype: sbyte
        {
            Disabled = 0,
            Throttle = 1,
            Current = 2            
        }

        public void StartCalibration()
        {
            Controls.ProgressReporterDialogue prd = new Controls.ProgressReporterDialogue()
            {
                StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen,
                Text = "Compass Mot"
            };

            prd.DoWork += DoCalibration;

            prd.RunBackgroundOperationAsync();
        }

        void DoCalibration(object sender, ProgressWorkerEventArgs e, object passdata = null)
        {
            var prd = ((ProgressReporterDialogue)sender);

            prd.UpdateProgressAndStatus(-1, "Starting Compass Mot");

            int8_t comp_type;                 // throttle or current based compensation
            Vector3f compass_base = new Vector3f();              // compass vector when throttle is zero
            Vector3f motor_impact = new Vector3f();              // impact of motors on compass vector
            Vector3f motor_impact_scaled;       // impact of motors on compass vector scaled with throttle
            Vector3f motor_compensation;        // final compensation to be stored to eeprom
            float throttle_pct;              // throttle as a percentage 0.0 ~ 1.0
            float throttle_pct_max = 0.0f;   // maximum throttle reached (as a percentage 0~1.0)
            float current_amps_max = 0.0f;   // maximum current reached
            float interference_pct;          // interference as a percentage of total mag field (for reporting purposes only)
            //uint32_t last_run_time;
            uint8_t print_counter = 49;
            bool updated = false;           // have we updated the compensation vector at least once

            if ((float)MainV2.comPort.MAV.param["BATT_MONITOR"] == 4f) // volt and current
            {
                comp_type = (sbyte)comptype.Current;
                prd.UpdateProgressAndStatus(-1, "Compass Mot using current");
            }
            else
            {
                comp_type = (sbyte)comptype.Throttle;
                prd.UpdateProgressAndStatus(-1, "Compass Mot using throttle");
            }

            if ((float)MainV2.comPort.MAV.param["COMPASS_USE"] != 1)
            {
                e.ErrorMessage = "Compass is disabled";
                return;
            }

            // request streams
            MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.RC_CHANNELS, 10); // rc out
            MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.RAW_SENSORS, 50); // mag out

            // reset compass mot
            MainV2.comPort.setParam("COMPASS_MOTCT ", 0.0f);

            MainV2.comPort.setParam("COMPASS_MOT_X", 0.000000f);
            MainV2.comPort.setParam("COMPASS_MOT_Y", 0.000000f);
            MainV2.comPort.setParam("COMPASS_MOT_Z", 0.000000f);

            // store initial x,y,z compass values
            compass_base.x = MainV2.comPort.MAV.cs.mx;
            compass_base.y = MainV2.comPort.MAV.cs.my;
            compass_base.z = MainV2.comPort.MAV.cs.mz;

            // initialise motor compensation
            motor_compensation = new Vector3f(0, 0, 0);

            int magseen = MainV2.comPort.MAV.packetseencount[(byte)MAVLink.MAVLINK_MSG_ID.RAW_IMU];
            int rcseen = MainV2.comPort.MAV.packetseencount[(byte)MAVLink.MAVLINK_MSG_ID.HIL_RC_INPUTS_RAW];
            DateTime deadline = DateTime.Now.AddSeconds(10);

            prd.UpdateProgressAndStatus(-1, "Waiting for Mag and RC data");

            while (true)
            {
                if (magseen > (magseen + 100) && rcseen > (rcseen + 20))
                {
                    break;
                }

                if (e.CancelRequested)
                {
                    e.CancelAcknowledged = true;
                    return;
                }

                if (DateTime.Now > deadline)
                {
                    e.ErrorMessage = "Not enough packets where received\n" + magseen + " mag " + rcseen + " rc";
                    return;
                }
            }

            while (true)
            {
                if (prd.doWorkArgs.CancelRequested)
                {
                    prd.doWorkArgs.CancelAcknowledged = true;
                    break;
                }

                // radio

                // passthorugh - cant do.

                // compass read

                // battery read

                // calculate scaling for throttle
                int checkme;
                throttle_pct = (float)MainV2.comPort.MAV.cs.ch3percent / 100.0f;
                throttle_pct = constrain_float(throttle_pct, 0.0f, 1.0f);

                // if throttle is zero, update base x,y,z values
                if (throttle_pct == 0.0f)
                {
                    compass_base.x = compass_base.x * 0.99f + (float)MainV2.comPort.MAV.cs.mx * 0.01f;
                    compass_base.y = compass_base.y * 0.99f + (float)MainV2.comPort.MAV.cs.my * 0.01f;
                    compass_base.z = compass_base.z * 0.99f + (float)MainV2.comPort.MAV.cs.mz * 0.01f;

                    // causing printing to happen as soon as throttle is lifted
                    print_counter = 49;
                }
                else
                {

                    // calculate diff from compass base and scale with throttle
                    motor_impact.x = MainV2.comPort.MAV.cs.mx - compass_base.x;
                    motor_impact.y = MainV2.comPort.MAV.cs.my - compass_base.y;
                    motor_impact.z = MainV2.comPort.MAV.cs.mz - compass_base.z;

                    // throttle based compensation
                    if (comp_type == (byte)comptype.Throttle)
                    {
                        // scale by throttle
                        motor_impact_scaled = motor_impact / throttle_pct;

                        // adjust the motor compensation to negate the impact
                        motor_compensation = motor_compensation * 0.99f - motor_impact_scaled * 0.01f;
                        updated = true;
                    }
                    else
                    {
                        // current based compensation if more than 3amps being drawn
                        motor_impact_scaled = motor_impact / MainV2.comPort.MAV.cs.current;

                        // adjust the motor compensation to negate the impact if drawing over 3amps
                        if (MainV2.comPort.MAV.cs.current >= 3.0f)
                        {
                            motor_compensation = motor_compensation * 0.99f - motor_impact_scaled * 0.01f;
                            updated = true;
                        }
                    }

                    // record maximum throttle and current
                    throttle_pct_max = max(throttle_pct_max, throttle_pct);
                    current_amps_max = max(current_amps_max, MainV2.comPort.MAV.cs.current);

                    // display output at 1hz if throttle is above zero
                    print_counter++;
                    if (print_counter >= 50)
                    {
                        print_counter = 0;
                        var line = String.Format("thr:%d cur:%4.2f mot x:%4.1f y:%4.1f z:%4.1f  comp x:%4.2f y:%4.2f z:%4.2f\n", (int)MainV2.comPort.MAV.cs.ch3percent, (float)MainV2.comPort.MAV.cs.current, (float)motor_impact.x, (float)motor_impact.y, (float)motor_impact.z, (float)motor_compensation.x, (float)motor_compensation.y, (float)motor_compensation.z);
                        Console.Write(line);
                        prd.UpdateProgressAndStatus(-1, line);
                    }
                }
            }

            MainV2.comPort.doARM(false);

            // set and save motor compensation
            if (updated)
            {
                MainV2.comPort.setParam("COMPASS_MOTCT ", comp_type);

                MainV2.comPort.setParam("COMPASS_MOT_X", (float)motor_compensation.x);
                MainV2.comPort.setParam("COMPASS_MOT_Y", (float)motor_compensation.y);
                MainV2.comPort.setParam("COMPASS_MOT_Z", (float)motor_compensation.z);

                // calculate and display interference compensation at full throttle as % of total mag field
                if (comp_type == (byte)comptype.Throttle)
                {
                    // interference is impact@fullthrottle / mag field * 100
                    interference_pct = (float)motor_compensation.length() / (float)COMPASS_MAGFIELD_EXPECTED * 100.0f;
                }
                else
                {
                    // interference is impact/amp * (max current seen / max throttle seen) / mag field * 100
                    interference_pct = (float)motor_compensation.length() * (current_amps_max / throttle_pct_max) / (float)COMPASS_MAGFIELD_EXPECTED * 100.0f;
                }
                string line = String.Format("\nInterference at full throttle is {0}% of mag field\n\n", (int)interference_pct);
                Console.Write(line);
                prd.UpdateProgressAndStatus(100, line);
            }
            else
            {
                prd.UpdateProgressAndStatus(100, "Failed");
            }

            MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.ALL, 2);
        }

        private float max(float throttle_pct_max, float throttle_pct)
        {
            return Math.Max(throttle_pct_max,throttle_pct);
        }

        private float constrain_float(float throttle_pct, float p1, float p2)
        {
            if (throttle_pct > p2)
                return p2;
            if (throttle_pct < p1)
                return p1;
            return throttle_pct;
        }
    }
}
