using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner
{
    public class EnergyProfile
    {
        public static double PolyValV(double _dAngle)
        {
            //(a1 - t)*e^(-0.5*((x - b1) / c1)²) + kx / 100 + t

            //Scaling factors for curvature and gradient
            double gauss = (EnergyProfile.Velocity["Amplitude"] - EnergyProfile.Velocity["LowerBound"]) * Math.Exp(-0.5 * Math.Pow(((_dAngle - EnergyProfile.Velocity["AmpPosition"]) / EnergyProfile.Velocity["Variance"]), 2));
            gauss += (EnergyProfile.Velocity["Gradient"] / 100) * _dAngle + EnergyProfile.Velocity["LowerBound"];

            return gauss;
        }
        
        public static double PolyValV(double _dAngle, double _dAmp, double _dAngAmp, double _dVarAmp, double _dGradient, double _dLowerBound)
        {
            //(a1 - t)*e^(-0.5*((x - b1) / c1)²) + kx / 100 + t

            //Scaling factors for curvature and gradient
            double gauss = (_dAmp - _dLowerBound) * Math.Exp(-0.5 * Math.Pow(((_dAngle - _dAngAmp) / _dVarAmp), 2));
            gauss += (_dGradient / 100) * _dAngle + _dLowerBound;

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

        public static double PolyValI(double _dAngle, double _dPosAmp, double _dAngPos, double _dVarPos, double _dNegAmp, double _dAngNeg, double _dVarNeg, double _dLowerLimit, double _dDeviation = 0)
        {
            //(ANeg - t)*e^((-0.5*((x-muNeg)^2)/(varNeg^2) + (APos - t)*e^(-0.5*((x-muPos)^2)/(varPos^2)) + t
            double gauss = (_dNegAmp - _dLowerLimit) * Math.Exp((-0.5 * Math.Pow((_dAngle - _dAngNeg), 2)) / Math.Pow(_dVarNeg, 2));
            gauss += (_dPosAmp - _dLowerLimit) * Math.Exp((-0.5 * Math.Pow((_dAngle - _dAngPos), 2)) / Math.Pow(_dVarPos, 2));
            gauss += _dLowerLimit;

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

            //v = d/t ==> t = d/v in [s]

            dTime = _dDistance / _dVelocity;    //[s]
            dTime *= 3600;  //[h]

            _dCurrent *= 1000;  //[A] => [mA]

            return Math.Round(_dCurrent * dTime, 1, MidpointRounding.AwayFromZero); //[mA] * [h] => [mAh]
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

        public static void Initialize() //populate/initialize energyprofile
        {
            //current
            m_CurrentDict["NegativeAmplitude"] = 0.0f;
            m_CurrentDict["NegativeAmpAngle"] = 0.0f;
            m_CurrentDict["NegativeVariance"] = 0.0f;
            m_CurrentDict["PositiveAmplitude"] = 0.0f;
            m_CurrentDict["PositiveAmpAngle"] = 0.0f;
            m_CurrentDict["PositiveVariance"] = 0.0f;
            m_CurrentDict["MaxDeviation"] = 0.0f;
            m_CurrentDict["MinDeviation"] = 0.0f;
            m_CurrentDict["LowerLimit"] = 0.0f;
            m_CurrentDict["Hover"] = 0.0f;

            //velocity
            m_VelocityDict["Amplitude"] = 0.0f;
            m_VelocityDict["AmpPosition"] = 0.0f;
            m_VelocityDict["Variance"] = 0.0f;
            m_VelocityDict["LowerBound"] = 0.0f;
            m_VelocityDict["Gradient"] = 0.0f;

            Initialized = true;
        }

        public static bool Initialized { get; set; }    //Dictionaries populated?
        public static double CopterID { get; set; }
        public static bool Enabled { get; set; }

        private static Dictionary<string, double> m_CurrentDict = new Dictionary<string, double>();
        private static Dictionary<string, double> m_VelocityDict = new Dictionary<string, double>();
    }
}
