using System;
using System.Collections.Generic;


namespace MissionPlanner.Utilities
{
    /// <summary>
    /// static methods for calculate the specific velocity, current and consumption
    /// </summary>
    public class EnergyProfile
    {
        /// <summary>
        /// returns the velocity to a specific angle
        /// </summary>
        /// <param name="dAngle">specific angle</param>
        /// <returns>velocity [m/s]</returns>
        public static double PolyValV(double dAngle)
        {
            //Scaling factors for curvature and gradient
            double gauss = (Velocity["Amplitude"] - Velocity["LowerBound"]) *
                           Math.Exp(-0.5 * Math.Pow(((dAngle - Velocity["AmpPosition"]) / Velocity["Variance"]), 2));
            gauss += (Velocity["Gradient"] / 100) * dAngle + Velocity["LowerBound"];

            return gauss;
        }

        /// <summary>
        /// returns the velocity to specific parameters
        /// </summary>
        /// <param name="dAngle">specific angle</param>
        /// <param name="dAmp">amplitude</param>
        /// <param name="dAngAmp">position of amplitude</param>
        /// <param name="dVarAmp">variance</param>
        /// <param name="dGradient">gradient</param>
        /// <param name="dLowerBound">lower bound</param>
        /// <returns>velocity [m/s]</returns>
        public static double PolyValV(double dAngle, double dAmp, double dAngAmp,
            double dVarAmp, double dGradient, double dLowerBound)
        {
            //Scaling factors for curvature and gradient
            double gauss = (dAmp - dLowerBound) * Math.Exp(-0.5 * Math.Pow(((dAngle - dAngAmp) / dVarAmp), 2));
            gauss += (dGradient / 100) * dAngle + dLowerBound;

            return gauss;
        }

        /// <summary>
        /// returns the current plus deviation to a specific angle
        /// </summary>
        /// <param name="dAngle">specific angle</param>
        /// <param name="dDeviation">deviation</param>
        /// <returns>current plus deviation [A]</returns>
        public static double PolyValI(double dAngle, double dDeviation = 0)
        {
            double gauss = (Current["NegativeAmplitude"] - Current["LowerLimit"]) *
                           Math.Exp((-0.5 * Math.Pow((dAngle - Current["NegativeAmpAngle"]), 2)) /
                                    Math.Pow(Current["NegativeVariance"], 2));
            gauss += (Current["PositiveAmplitude"] - Current["LowerLimit"]) *
                     Math.Exp((-0.5 * Math.Pow((dAngle - Current["PositiveAmpAngle"]), 2)) /
                              Math.Pow(Current["PositiveVariance"], 2));
            gauss += Current["LowerLimit"];

            return gauss + dDeviation;
        }

        /// <summary>
        /// returns the current plus deviation to a specific angle and other parameters
        /// </summary>
        /// <param name="dAngle">specific angle</param>
        /// <param name="dPosAmp">position of amplitude</param>
        /// <param name="dAngPos">positive angle of amplitude</param>
        /// <param name="dVarPos">positive variance</param>
        /// <param name="dNegAmp">negetive amplitude</param>
        /// <param name="dAngNeg">negative angle of amplitude</param>
        /// <param name="dVarNeg">negative variance</param>
        /// <param name="dLowerLimit">lower limit</param>
        /// <param name="dDeviation">deviation</param>
        /// <returns>current plus deviation [A]</returns>
        public static double PolyValI(double dAngle, double dPosAmp, double dAngPos,
            double dVarPos, double dNegAmp, double dAngNeg, double dVarNeg, double dLowerLimit, double dDeviation = 0)
        {
            double gauss = (dNegAmp - dLowerLimit) *
                           Math.Exp((-0.5 * Math.Pow((dAngle - dAngNeg), 2)) / Math.Pow(dVarNeg, 2));
            gauss += (dPosAmp - dLowerLimit) *
                     Math.Exp((-0.5 * Math.Pow((dAngle - dAngPos), 2)) / Math.Pow(dVarPos, 2));
            gauss += dLowerLimit;

            return gauss + dDeviation;
        }

        /// <summary>
        /// Calculates the energyconsumption in [mAh] for movement.
        /// Given: Current in [A], Velocity in [m/s], Distance in [m]
        /// </summary>
        /// <param name="dCurrent">current [A]</param>
        /// <param name="dVelocity">velocity [m/s]</param>
        /// <param name="dDistance">distance [m]</param>
        /// <returns>value of energy consumption [mAh]</returns>
        public static double CalculateEnergyConsumption(double dCurrent,
            double dVelocity, double dDistance)
        {
            var dTime = dDistance / 3600 * dVelocity; // [h]
            dCurrent *= 1000; //[A] => [mA]
            return Math.Round(dCurrent * dTime, 1, MidpointRounding.AwayFromZero); //[mA] * [h] => [mAh]
        }

        /// <summary>
        /// populate/initialize energyprofile
        /// </summary>
        public static void Initialize()
        {
            //current
            Current["NegativeAmplitude"] = 0.0f;
            Current["NegativeAmpAngle"] = 0.0f;
            Current["NegativeVariance"] = 0.0f;
            Current["PositiveAmplitude"] = 0.0f;
            Current["PositiveAmpAngle"] = 0.0f;
            Current["PositiveVariance"] = 0.0f;
            Current["MaxDeviation"] = 0.0f;
            Current["MinDeviation"] = 0.0f;
            Current["LowerLimit"] = 0.0f;
            Current["Hover"] = 0.0f;

            //velocity
            Velocity["Amplitude"] = 0.0f;
            Velocity["AmpPosition"] = 0.0f;
            Velocity["Variance"] = 0.0f;
            Velocity["LowerBound"] = 0.0f;
            Velocity["Gradient"] = 0.0f;

            Initialized = true;
        }

        // Getter & Setter

        /// <summary>
        /// initialize-flag 
        /// </summary>
        public static bool Initialized { get; set; } //Dictionaries populated?

        /// <summary>
        /// enable-flag
        /// </summary>
        public static bool Enabled { get; set; }

        /// <summary>
        /// static dictionary for current values
        /// </summary>
        public static Dictionary<string, double> Current { get; } = new Dictionary<string, double>();

        /// <summary>
        /// static dictionary for velocity values
        /// </summary>
        public static Dictionary<string, double> Velocity { get; } = new Dictionary<string, double>();
    }
}
