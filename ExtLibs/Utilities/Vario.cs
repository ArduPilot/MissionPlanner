using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionPlanner.Utilities
{
    public class Vario
    {
        public static int MidTone = 700;
        static float climbrate = 0;
        public static bool run = false;

        public static string Running
        {
            get
            {
                if (run) return "Stop Vario";
                return "Start Vario";
            }
        }

        public static void SetValue(float climbrate)
        {
            Vario.climbrate = climbrate;
        }

        public static Action<int, int> Beep = (note, durationms) => { Console.Beep(note, durationms); };

        public static async void mainloop(object o)
        {
            while (run)
            {
                float note = climbrate * 30 + MidTone;

                try
                {

                    if (Math.Abs(climbrate) > 0.3)
                    {
                        // freq , duration
                        if (climbrate > 0)
                        {
                            Beep((int)note, 300 - (int)(climbrate * 5));
                            await Task.Delay(20).ConfigureAwait(false);
                        }
                        else
                        {
                            Beep((int)note - 50, 600);
                        }
                    }
                    else
                    {
                        // sleep when there is no sound required
                        await Task.Delay(100).ConfigureAwait(false);
                    }

                }
                catch
                {
                }
            }
        }

        public static void Start()
        {
            run = true;
            Task.Run(() =>
            {
                mainloop(null);
            });
        }

        public static void Stop()
        {
            run = false;
        }
    }
}
