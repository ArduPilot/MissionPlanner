using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.Utilities
{
    public class Vario
    {
        public static int MidTone = 700;
        static float climbrate = 0;
        public static bool run = false;

        public static string Running
        {
            get { if (run) return "Stop Vario"; return "Start Vario"; }
        }

        public static void SetValue(float climbrate)
        {
            Vario.climbrate = climbrate;
        }

        public static void mainloop(object o)
        {
            System.Threading.Thread.CurrentThread.IsBackground = true;

            while (run)
            {
                float note = climbrate *30 + MidTone;

                try
                {

                    if (Math.Abs(climbrate) > 0.3)
                    {
                        // freq , duration
                        if (climbrate > 0)
                        {
                            Console.Beep((int)note, 300 - (int)(climbrate * 5));
                            System.Threading.Thread.Sleep(20);
                        }
                        else
                        {
                            Console.Beep((int)note - 50, 600);
                        }
                    }
                    else
                    {
                        // sleep when there is no sound required
                        System.Threading.Thread.Sleep(100);
                    }

                }
                catch { }

            }
        }

        public static void Start()
        {
            run = true;
            System.Threading.ThreadPool.QueueUserWorkItem(mainloop);
        }

        public static void Stop()
        {
            run = false;
        }
    }
}
