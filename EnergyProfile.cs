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
            //(a1 - a2) e^(-0.5*((x - b1) / c1)²) + a2 e^(-0.5*((x - b2) / c2)²) + t x / 1000

            //Scaling factors for curvature and gradient
            double gauss = (EnergyProfile.Velocity["Amplitude"] - EnergyProfile.Velocity["LowerBound"]) * Math.Exp(-0.5 * Math.Pow(((_dAngle - EnergyProfile.Velocity["AmpPosition"]) / EnergyProfile.Velocity["Variance"]), 2));
            gauss += EnergyProfile.Velocity["LowerBound"] * Math.Exp(-Math.Pow(0.5 * (_dAngle / (EnergyProfile.Velocity["Curvature"])), 2) + EnergyProfile.Velocity["Gradient"] * _dAngle / 1000);
            
            return gauss;
        }

        public static double PolyValI(double _dAngle, double _dDeviation = 0)
        {
			//(ANeg - t)*e^((-0.5*((x-muNeg)^2)/(varNeg^2) + (APos - t)*e^(-0.5*((x-muPos)^2)/(varPos^2)) + t
            double gauss = (EnergyProfile.Current["NegativeAmplitude"] - EnergyProfile.Current["LowerLimit"]) * Math.Exp((-0.5 * Math.Pow((_dAngle - EnergyProfile.Current["NegativeAmpAngle"]), 2)) / Math.Pow(EnergyProfile.Current["NegativeVariance"], 2));
            gauss += (EnergyProfile.Current["PositiveAmplitude"] - EnergyProfile.Current["LowerLimit"]) * Math.Exp((-0.5 * Math.Pow((_dAngle - EnergyProfile.Current["PositiveAmpAngle"]), 2)) / Math.Pow(EnergyProfile.Current["PositiveVariance"], 2));
            gauss += EnergyProfile.Current["LowerLimit"];
            
            return gauss + _dDeviation;
        }
        /// <summary>
        /// Calculates the energyconsumption in [mAh].
        /// Given: Current in [A], Velocity in [m/s]
        /// </summary>
        /// <param name="_dCurrent"></param>
        /// <param name="_dVelocity"></param>
        /// <param name="_dDistance"></param>
        /// <returns></returns>
        public static double CalculateEnergyConsumption(double _dCurrent, double _dVelocity, double _dDistance)
        {
            double dTime = 0.0f;
            //dVelocity = _dDistance / _dTime;
            //convert Velocity => m/s to m/h
            _dVelocity *= 3600;
            _dCurrent *= 1000;
            dTime = _dDistance / _dVelocity;   //m/(m/h) = h

            return Math.Round(_dCurrent * dTime, 2, MidpointRounding.AwayFromZero);
        }

        public static string EnergyProfilePath { get; set; }

        public static Dictionary<string, double> Current
        {
            get { return m_CurrentDict; }
        }

        public static Dictionary<string, double> Velocity
        {
            get { return m_VelocityDict; }
        }

        public static int CopterID { get; set; }
        public static bool Enabled { get; set; }

        private static Dictionary<string, double> m_CurrentDict = new Dictionary<string, double>();
        private static Dictionary<string, double> m_VelocityDict = new Dictionary<string, double>();
    }
}
