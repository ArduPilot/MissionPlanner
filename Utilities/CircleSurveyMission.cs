using MissionPlanner.Controls;
using MissionPlanner.GCSViews;

namespace MissionPlanner.Utilities
{
    public class CircleSurveyMission
    {
        public static void createGrid(PointLatLngAlt centerPoint)
        {
            int startalt = 10;
            int endalt = 20;
            int seperation = 2;
            int radius = 5;
            int photos = 50;
            int startheading = 0;

            InputBox.Show("", "startalt", ref startalt);
            InputBox.Show("", "endalt", ref endalt);
            InputBox.Show("", "seperation", ref seperation);
            InputBox.Show("", "radius", ref radius);
            InputBox.Show("", "photos", ref photos);
            InputBox.Show("", "start heading", ref startheading);

            MainV2.instance.FlightPlanner.quickadd = true;

            // set roi centerpoint
            FlightPlanner.instance.AddCommand(MAVLink.MAV_CMD.DO_SET_ROI, 0, 0, 0, 0, centerPoint.Lng, centerPoint.Lat,
                centerPoint.Alt);

            // alts
            for (int alt = startalt; alt <= endalt; alt+=seperation)
            {
                // headings
                for (int heading = startheading; heading <= startheading+360; heading+=360/photos)
                {
                    MainV2.instance.FlightPlanner.quickadd = true;
                    var newpoint = centerPoint.newpos(heading, radius);
                    // add wp
                    FlightPlanner.instance.AddCommand(MAVLink.MAV_CMD.WAYPOINT, 2, 0, 0, 0, newpoint.Lng,
                        newpoint.Lat, alt);
                    // trigger camera
                    FlightPlanner.instance.AddCommand(MAVLink.MAV_CMD.DO_DIGICAM_CONTROL, 0, 0, 0, 0, 0, 1, 0);
                }
            }

            MainV2.instance.FlightPlanner.quickadd = false;

            MainV2.instance.FlightPlanner.writeKML();
        }
    }
}
