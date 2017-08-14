using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;

namespace MissionPlanner.Utilities
{
    interface IConfigEnergyProfile
    {
        void ChangeDeviation(int dev);

        void LinearInterpolation();

        void PlotCurrent_Spline(Chart chart);

        void ExportProfile();

        void ImportProfile();
    }
    interface IEnergyConsumption
    {
        double CalculateEnergyConsumption(double accel_Horizontal, double accel_Vertical, double maxSpeed_Horizontal,
            double maxSpeed_Vertical_UP, double maxSpeed_Vertical_DN, double distance_Horizontal, double angle, double altitude, double hoverTime);
    }
}
