namespace MissionPlanner.Swarm.SRB
{
    public class Controller
    {
        public DroneGroup DG = new DroneGroup();
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
