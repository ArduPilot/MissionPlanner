using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using DotSpatial.Data;


namespace MissionPlanner.Utilities
{
    /// <summary>
    /// static methods for calculate the specific velocity, current and consumption
    /// </summary>
    public static class EnergyProfileModel
    {
        private static bool _enabled;
        private static double _percentDevCrnt;
        
        /// <summary>
        /// populate/initialize energyprofile
        /// </summary>
        public static void Initialize()
        {
            // ===========================
            // Init currentmodel for hover
            // ===========================
            CurrentHover = new CurrentModel(0f, 0f);
            // ===============
            // Init currentset
            // ===============

            // pattern for dev
            //CurrentSet.Add(1, new CurrentModel(-90f, 16.22f, 2.7f));
            //CurrentSet.Add(2, new CurrentModel(-72f, 17.91f, 0.67f));
            //CurrentSet.Add(3, new CurrentModel(-54f, 16.89f, 1.35f));
            //CurrentSet.Add(4, new CurrentModel(-36f, 17.23f, 1.35f));
            //CurrentSet.Add(5, new CurrentModel(-18f, 16.22f, 2.36f));
            //CurrentSet.Add(6, new CurrentModel(0.00f, 16.89f, 2.03f));
            //CurrentSet.Add(7, new CurrentModel(18f, 18.24f, 4.06f));
            //CurrentSet.Add(8, new CurrentModel(36f, 19.59f, 2.71f));
            //CurrentSet.Add(9, new CurrentModel(54f, 20.95f, 1.35f));
            //CurrentSet.Add(10, new CurrentModel(72f, 20.27f, 2.03f));
            //CurrentSet.Add(11, new CurrentModel(90f, 21.62f, 1.35f));

            // pattern for dev
            //CurrentSet.Add(1, new CurrentModel(-90f, 16.22f));
            //CurrentSet.Add(2, new CurrentModel(-72f, 17.91f));
            //CurrentSet.Add(3, new CurrentModel(-54f, 16.89f));
            //CurrentSet.Add(4, new CurrentModel(-36f, 17.23f));
            //CurrentSet.Add(5, new CurrentModel(-18f, 16.22f));
            //CurrentSet.Add(6, new CurrentModel(0.00f, 16.89f));
            //CurrentSet.Add(7, new CurrentModel(18f, 18.24f));
            //CurrentSet.Add(8, new CurrentModel(36f, 19.59f));
            //CurrentSet.Add(9, new CurrentModel(54f, 20.95f));
            //CurrentSet.Add(10, new CurrentModel(72f, 20.27f));
            //CurrentSet.Add(11, new CurrentModel(90f, 21.62f));

            // for release
            CurrentSet.Add(1, new CurrentModel(-90.0f, 0.0f));
            CurrentSet.Add(2, new CurrentModel(0.0f, 0.0f));
            CurrentSet.Add(3, new CurrentModel(0.0f, 0.0f));
            CurrentSet.Add(4, new CurrentModel(0.0f, 0.0f));
            CurrentSet.Add(5, new CurrentModel(0.0f, 0.0f));
            CurrentSet.Add(6, new CurrentModel(0.0f, 0.0f));
            CurrentSet.Add(7, new CurrentModel(0.0f, 0.0f));
            CurrentSet.Add(8, new CurrentModel(0.0f, 0.0f));
            CurrentSet.Add(9, new CurrentModel(0.0f, 0.0f));
            CurrentSet.Add(10, new CurrentModel(0.0f, 0.0f));
            CurrentSet.Add(11, new CurrentModel(90.0f, 0.0f));

            // =================
            // Init VelocitySet 
            // =================
            // pattern for dev
            //VelocitySet.Add(1, new VelocityModel(-90f, 2.22f, 0.7f));
            //VelocitySet.Add(2, new VelocityModel(-72f, 3.91f, 0.67f));
            //VelocitySet.Add(3, new VelocityModel(-54f, 4.89f, 0.35f));
            //VelocitySet.Add(4, new VelocityModel(-36f, 5.23f, 0.35f));
            //VelocitySet.Add(5, new VelocityModel(-18f, 6.22f, 0.36f));
            //VelocitySet.Add(6, new VelocityModel(0.00f, 8.89f, 0.03f));
            //VelocitySet.Add(7, new VelocityModel(18f, 6.24f, 0.06f));
            //VelocitySet.Add(8, new VelocityModel(36f, 5.59f, 0.71f));
            //VelocitySet.Add(9, new VelocityModel(54f, 4.95f, 0.35f));
            //VelocitySet.Add(10, new VelocityModel(72f, 3.27f, 0.03f));
            //VelocitySet.Add(11, new VelocityModel(90f, 2.62f, 0.35f));
            // for release
            VelocitySet.Add(1, new VelocityModel(-90f, 0f, 0f));
            VelocitySet.Add(2, new VelocityModel(0f, 0f, 0f));
            VelocitySet.Add(3, new VelocityModel(0f, 0f, 0f));
            VelocitySet.Add(4, new VelocityModel(0f, 0f, 0f));
            VelocitySet.Add(5, new VelocityModel(0f, 0f, 0f));
            VelocitySet.Add(6, new VelocityModel(0f, 0f, 0f));
            VelocitySet.Add(7, new VelocityModel(0f, 0f, 0f));
            VelocitySet.Add(8, new VelocityModel(0f, 0f, 0f));
            VelocitySet.Add(9, new VelocityModel(0f, 0f, 0f));
            VelocitySet.Add(10, new VelocityModel(0f, 0f, 0f));
            VelocitySet.Add(11, new VelocityModel(90f, 0f, 0f));

        }

        // Getter & Setter

        /// <summary>
        /// enable-flag
        /// </summary>
        public static bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (value == false)
                {
                    ClearProperties();
                }
                else
                {
                    Initialize();
                }
                _enabled = value;
            }
        }

        /// <summary>
        /// Clearing the static fields
        /// </summary>
        private static void ClearProperties()
        {
            CurrentSet.Clear();
            VelocitySet.Clear();
            CurrentHover = null;
            MinCurrentSplinePoints.Clear();
            AverageCurrentSplinePoints.Clear();
            MaxCurrentSplinePoints.Clear();
            MinVelocitySplinePoints.Clear();
            AverageVelocitySplinePoints.Clear();
            MaxVelocitySplinePoints.Clear();
            PercentDevCrnt = 0.0f;
        }

        /// <summary>
        /// static dictionary for current values
        /// </summary>
        public static Dictionary<string, double> Current { get; } = new Dictionary<string, double>();

        public static CurrentModel CurrentHover { get; set; }
        /// <summary>
        /// static dictionary for current values
        /// </summary>
        public static Dictionary<int, CurrentModel> CurrentSet { get; set; } = new Dictionary<int, CurrentModel>();

        /// <summary>
        /// static dictionary for velocity values
        /// </summary>
        public static Dictionary<int, VelocityModel> VelocitySet { get; set; } = new Dictionary<int, VelocityModel>();

        //Lists of CurrentSplinePoints
        public static List<PointF> MinCurrentSplinePoints { get; set; } = new List<PointF>();
        public static List<PointF> AverageCurrentSplinePoints { get; set; } = new List<PointF>();
        public static List<PointF> MaxCurrentSplinePoints { get; set; } = new List<PointF>();

        //Lists of VelocitySplinePoints
        public static List<PointF> MinVelocitySplinePoints { get; set; } = new List<PointF>();
        public static List<PointF> AverageVelocitySplinePoints { get; set; } = new List<PointF>();
        public static List<PointF> MaxVelocitySplinePoints { get; set; } = new List<PointF>();

        //gives the deviation from percent
        public static double PercentDevCrnt
        {
            get { return _percentDevCrnt; }
            set
            {
                _percentDevCrnt = Math.Round(value, 2);
            }
        }

        // list of items for dropdown in configview
        public static List<int> DeviationInPercentList { get;  } = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 , 9, 10 , 15, 20, 25};
    }

    // set datamodel for current
    public class CurrentModel
    {
        private double _angle;
        private double _averageCurrent;
        private double _deviation;
        private bool _percentDevFlag;

        public double PercentDev { get; set; } = 0.0f;

        public double Angle
        {
            get { return Math.Round(_angle, 0); }
            set { _angle = Math.Round(value, 0); }
        }

        public double AverageCurrent
        {
            get { return Math.Round(_averageCurrent, 2); }
            set { _averageCurrent = Math.Round(value, 2); }
        }

        public double Deviation
        {
            get
            {
                // CustomMessageBox.Show("get dev currentmodel--> " + _deviation);
                if (!_percentDevFlag)
                {
                    return Math.Round(_deviation, 2);
                }
                if (Math.Round(AverageCurrent * EnergyProfileModel.PercentDevCrnt, 2) == Math.Round(_deviation, 2))
                {
                    return _deviation;
                }
                _deviation = Math.Round(AverageCurrent * EnergyProfileModel.PercentDevCrnt, 2);
                return _deviation;
            }
            set
            {
                if (!_percentDevFlag)
                    _deviation = Math.Round(value, 2);
            }
        }

        public double MaxCurrent => Math.Round(AverageCurrent + _deviation, 2);

        public double MinCurrent => Math.Round(AverageCurrent - _deviation, 2);

        public CurrentModel(double angle, double averageCurrent, double deviation)
        {
            _angle = angle;
            _averageCurrent = averageCurrent;
            _deviation = deviation;
        }
        
        public CurrentModel(double angle, double averageCurrent)
        {
            _angle = angle;
            _averageCurrent = averageCurrent;
            _percentDevFlag = true; // set for a fix dev
        }
    }

    /// <summary>
    /// Model for Velocity
    /// </summary>
    public class VelocityModel
    {
        public double Angle
        {
            get { return Math.Round(_angle, 0); }
            set { _angle = Math.Round(value, 0); }
        }

        public double AverageVelocity
        {
            get { return Math.Round(_averageVelocity, 2); }
            set { _averageVelocity = Math.Round(value, 2); }
        }

        public double Deviation
        {
            get { return Math.Round(_deviation, 2); }
            set { _deviation = Math.Round(value, 2); }
        }

        private double _angle;
        private double _averageVelocity;
        private double _deviation;

        public VelocityModel(double angle, double averageVelocity, double deviation)
        {
            _angle = angle;
            _averageVelocity = averageVelocity;
            _deviation = deviation;
        }

        public double MaxVelocity => Math.Round(_averageVelocity + _deviation, 2);

        public double MinVelocity => Math.Round(_averageVelocity - _deviation, 2);
    }
}
