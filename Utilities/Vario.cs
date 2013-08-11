using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.Utilities
{
    class Vario
    {
        public static int MidTone = 700;
        static float climbrate = 0;
        public static bool run = true;

        public static void SetValue(float climbrate)
        {
            Vario.climbrate = climbrate;
        }

        public static void mainloop(object o)
        {
            System.Threading.Thread.CurrentThread.IsBackground = true;

            while (run)
            {
                float note = climbrate *20 + MidTone;

                try
                {

                    if (Math.Abs(climbrate) > 0.5)
                    {
                        // freq , duration
                        if (climbrate > 0)
                        {
                            Console.Beep((int)note, 130 - (int)(climbrate * 5));
                        }
                        else
                        {
                            Console.Beep((int)note, 220 - (int)(climbrate * 5));
                        }
                    }

                }
                catch { }

                System.Threading.Thread.Sleep(20);
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
