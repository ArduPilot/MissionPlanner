using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionPlanner.HIL;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls.Waypoints
{
    public class Spline: MissionPlanner.HIL.Utils
    {
        float spline_t;
        Vector3 current_position;
        Vector3 spline_start_point;
        Vector3 spline_target;
        Vector3 spline_p0;
        Vector3 spline_p0_prime;
        Vector3 spline_p1;
        Vector3 spline_p1_prime;
        Vector3 spline_p2;
        Vector3 spline_a;
        Vector3 spline_b;
        private float SPLINE_TENSION = 1.6f;
        private float spline_t_sq;

        List<PointLatLngAlt> wplist;
        int wpindex = 0;

        public List<PointLatLngAlt> calcspline(PointLatLngAlt currentpos, PointLatLngAlt p1, PointLatLngAlt p2)
        {
            List<PointLatLngAlt> answer = new List<PointLatLngAlt>();

            spline_t = spline_t_sq = 0.0f;

            spline_p0 = currentpos;
            spline_p1 = p1;
            spline_p2 = p2;
            spline_p0_prime = new Vector3();
            double yaw = (currentpos.GetBearing(p1) + 360) % 360;
            spline_p0_prime.x = .000001 * Math.Sin(yaw * deg2rad);
            spline_p0_prime.y = .000001 * Math.Cos(yaw * deg2rad);
            spline_p0_prime.z = spline_p1.z - spline_p0.z;

            initialize_spline_segment();

            int steps = 30;

            foreach (var step in range(steps +1))
            {
                spline_t = (1f / steps) * step;
                spline_t_sq = spline_t * spline_t;
                evaluate_spline();
                answer.Add(new PointLatLngAlt(spline_target.x, spline_target.y, spline_target.z, ""));
            }

            return answer;
        }

        public List<PointLatLngAlt> doit(List<PointLatLngAlt> wplist, int steps, double entryyaw, bool staticstart)
        {
            this.wplist = wplist;

            SPLINE_TENSION = 1f;

            List<PointLatLngAlt> answer = new List<PointLatLngAlt>();

            entryyaw = (entryyaw + 360) % 360;

            spline_t = spline_t_sq = 0.0f;

            current_position = wplist[0];

            // set spline p0, p1, p2, and p0 prime
            spline_p0 = spline_start_point = current_position;
            spline_p1 = next_spline_waypoint();
            spline_p2 = next_spline_waypoint();
            spline_p0_prime = new Vector3();
            if (!staticstart)
            {
                // we are in lat long units.
                spline_p0_prime.x = .001 * Math.Cos(entryyaw * deg2rad);
                spline_p0_prime.y = .001 * Math.Sin(entryyaw * deg2rad);
            }
            spline_p0_prime.z = spline_p1.z - spline_p0.z;

            initialize_spline_segment();

            foreach (int p in  range(wplist.Count-1)) 
            {
                foreach (var step in range(steps)) 
                {
                    spline_t = (1f / steps) * step;
                    spline_t_sq = spline_t * spline_t;
                    evaluate_spline();
                    answer.Add(new PointLatLngAlt(spline_target.x,spline_target.y,spline_target.z,""));
                }
                next_spline_segment();  
            }


            answer.Add(wplist[wplist.Count - 1]);



            return answer;
        }

        // perform initialization in preparation for the new spline segment
        void initialize_spline_segment()
        {
            // derivative of spline at p1 is based on difference of previous and next points
            spline_p1_prime = (spline_p2 - spline_p0) / SPLINE_TENSION;


            // compute a and b vectors used in spline formula
            spline_a = spline_p0 * 2.0f - spline_p1 * 2.0f + spline_p0_prime + spline_p1_prime;
            spline_b = spline_p1 * 3.0f - spline_p0 * 3.0f - spline_p0_prime * 2.0f - spline_p1_prime;
        }

        // continue to the next spline segment
        void next_spline_segment()
        {
            // start t back at near the beginning of the new segment
            spline_t -= 1.0f;


            // p1 becomes the next p01, p2 the next p1, etc.
            spline_p0 = spline_p1;
            spline_p1 = spline_p2;
            spline_p0_prime = spline_p1_prime;
            spline_p2 = next_spline_waypoint();


            initialize_spline_segment();
        }

        private Vector3 next_spline_waypoint()
        {
            wpindex++;

            if (wpindex >= wplist.Count)
            {
                return wplist[wplist.Count - 1];
            }

            return wplist[wpindex];
        }


        //evaluate spline formula at point t
        void evaluate_spline()
        {
            // evaluate spline t cubed
            float t_cu = spline_t_sq * spline_t;

            spline_target = spline_a * t_cu + spline_b * spline_t_sq + spline_p0_prime * spline_t + spline_p0;
        }
    }
}