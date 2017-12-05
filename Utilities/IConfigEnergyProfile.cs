using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Windows.Forms.DataVisualization.Charting;
using Transitions;

namespace MissionPlanner.Utilities
{
    interface IConfigEnergyProfile
    {
        void ChangeDeviation(int dev);

        bool Interpolation(EnergyProfileController.PlotProfile profile);

        void Plot(Chart chart, EnergyProfileController.PlotProfile profile);

        void ExportProfile();

        bool ImportProfile();

        bool AnalyzeLogs(List<string> filenames, int minval, int transtime);

        //only for Dev --> statistic results
        void SetTransitionState(bool currentstate, bool speedstate, int cmdflighttime);

    }
    interface IEnergyConsumption
    {
        Dictionary<EnergyProfileController.ECID, double> EnergyConsumption(double distance_Horizontal, double angle, double altitudeDiff, double hoverTime);
    }

}
