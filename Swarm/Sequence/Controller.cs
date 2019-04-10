using log4net;
using System.Threading;

namespace MissionPlanner.Swarm.Sequence
{
    public class Controller
    {
        private static readonly ILog log =    LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DroneGroup DG;

        bool threadrun;
        private Thread thread;

        public void Start()
        {
            if (threadrun == true)
            {
                return;
            }

            DG = new DroneGroup();

            foreach (var port in MainV2.Comports)
            {
                foreach (var MAV in port.MAVlist)
                {
                    log.Debug("Add Drone " + MAV);
                    DG.Drones.Add(new Drone() { MavState = MAV });
                }
            }
            
            thread = new Thread(mainloop) {IsBackground = true};
            thread.Start();

            DG.CurrentMode = DroneGroup.Mode.idle;
        }

        public void Stop()
        {
            threadrun = false;
            if(thread != null)
                thread.Join();
            thread = null;
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
