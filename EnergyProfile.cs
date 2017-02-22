using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.Utilities
{
    public static class EnergyProfile
    {
        public static double PolyValV(double _dAngle)
        {
            //(a1 - a2) ℯ^(-0.5*((x - b1) / c1)²) + a2 ℯ^(-0.5*((x - b2) / c2)²) + t x / 1000
            //Scaling factors for curvature and gradient
            double gauss = (EnergyProfile.Velocity["Amplitude"] - EnergyProfile.Velocity["LowerAmplitude"]) * Math.Exp(-0.5 * Math.Pow(((_dAngle - EnergyProfile.Velocity["Angle"]) / EnergyProfile.Velocity["Variance"]), 2));
            gauss += EnergyProfile.Velocity["LowerAmplitude"] * Math.Exp(-Math.Pow(0.5 * (_dAngle / (EnergyProfile.Velocity["Curvature"])), 2) + EnergyProfile.Velocity["Gradient"] * _dAngle / 1000);

            return gauss;
        }

        public static double PolyValI(double _dAngle, double _dDeviation = 0)
        {
            double gauss = (EnergyProfile.Current["NegativeAmplitude"] - EnergyProfile.Current["LowerLimit"]) * Math.Exp((-0.5 * Math.Pow((_dAngle - EnergyProfile.Current["NegativeAmpAngle"]), 2)) / Math.Pow(EnergyProfile.Current["NegativeVariance"], 2));
            gauss += (EnergyProfile.Current["PositiveAmplitude"] - EnergyProfile.Current["LowerLimit"]) * Math.Exp((-0.5 * Math.Pow((_dAngle - EnergyProfile.Current["PositiveAmpAngle"]), 2)) / Math.Pow(EnergyProfile.Current["PositiveVariance"], 2));
            gauss += EnergyProfile.Current["LowerLimit"];

            return gauss + _dDeviation;
        }

        public static string EnergyProfilePath
        {
            get;
            set;
        }

        public static Dictionary<string, double> Current
        {
            get { return m_CurrentDict; }
        }

        public static Dictionary<string, double> Velocity
        {
            get { return m_VelocityDict; }
        }

        private static Dictionary<string, double> m_CurrentDict = new Dictionary<string, double>();
        private static Dictionary<string, double> m_VelocityDict = new Dictionary<string, double>();
    }
}
