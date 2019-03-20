using System;
using System.Collections.Generic;
using SharpDX.DirectInput;

namespace MissionPlanner.Swarm.TD
{
    public class Controller
    {
        public DroneGroup DG = new DroneGroup();

        // get device list
        public static DirectInput directInput = new DirectInput();

        // map of devices to drones
        public static List<SharpDX.DirectInput.Joystick> Joysticks = new List<SharpDX.DirectInput.Joystick>();

        bool threadrun;

        public void Start()
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var MAV in port.MAVlist)
                {
                    DG.Drones.Add(new Drone() { MavState = MAV });
                }
            }

            foreach (var device in directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly))
            {
                var joystick = new SharpDX.DirectInput.Joystick(directInput, device.InstanceGuid);

                joystick.Acquire();

                joystick.Poll();

                Console.WriteLine(joystick.Information.ProductName);

                Joysticks.Add(joystick);
            }

            if (threadrun == true)
            {
                threadrun = false;
                return;
            }


            new System.Threading.Thread(mainloop) {IsBackground = true}.Start();

            DG.CurrentMode = DroneGroup.Mode.idle;
        }

        public void Stop()
        {
            threadrun = false;
        }

        private void mainloop()
        {
            threadrun = true;

            while (threadrun)
            {
                DG.UpdatePositions();

                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
