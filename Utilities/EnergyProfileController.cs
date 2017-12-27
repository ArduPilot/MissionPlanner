using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using DotSpatial.Data.Forms;
using DotSpatial.Topology.Utilities;
using log4net.Filter;
using Microsoft.Scripting.AspNet.MembersInjectors;
using MissionPlanner.Controls;
using MissionPlanner.Log;
using Org.BouncyCastle.Crypto.Tls;
using ZedGraph;
using Chart = System.Windows.Forms.DataVisualization.Charting.Chart;
using ILog = log4net.ILog;
using System.Reflection;
using Core.ExtendedObjects;
using DirectShowLib;
using DotSpatial.Symbology;
using MissionPlanner.GCSViews;
using MissionPlanner.GCSViews.ConfigurationView;
using Org.BouncyCastle.Asn1.Ocsp;

namespace MissionPlanner.Utilities
{
    class EnergyProfileController : IConfigEnergyProfile, IEnergyConsumption
    {
        // ------------------------------------------
        //  variables and enums for energyconsumption
        // ------------------------------------------

        private string _energyProfilePath =
            Settings.GetUserDataDirectory() + "EnergyProfile" + Path.DirectorySeparatorChar;

        public enum ECID
        {
            AverageCurrent,
            MaxCurrent,
            MinCurrent,
            AverageVelocity,
            MaxVelocity,
            MinVelocity,
            FlightTime,
            avrCur_avrTime, //Current-Time-Combi 1
            maxCur_avrTime //Current-Time-Combi 2
        }

        /// <summary>
        /// XML-Tags for XML-Database
        /// </summary>
        enum XMLTag
        {
            Sets,
            CurrentSet,
            DevPercent,
            CurrentModel,
            ID,
            Hover,
            AverageCurrent,
            Deviation,
            Angle,
            VelocitySet,
            VelocityModel,
            AverageVelocity
        }


        // -------------------------------------
        //  variables and enums for LogAnalyzer
        // -------------------------------------

        private int validMinValues;
        private int transitionDelayTime;
        private int ampfactor = 100;
        LogAnalizerModel logAnalizerModel;
        private bool validAnalysis;
        private int cmdFlightTime;

        private StatisticModus statsmodus = StatisticModus.None;

        /// <summary>
        /// Enum for different statistic modi
        /// </summary>
        enum StatisticModus
        {
            GPS_Speed,
            Curr,
            Both,
            None
        }

        /// <summary>
        /// Enum for different Plotpfofiles
        /// </summary>
        public enum PlotProfile
        {
            Current,
            Velocity
        }

        /// <summary>
        /// Enum for each relavant rowtype
        /// </summary>
        enum RowType
        {
            FMT,
            GPS,
            MODE,
            CURR,
            CMD
        }

        /// <summary>
        /// Enum for CMD-Frame
        /// </summary>
        enum CMDFrame
        {
            CNum,
            CId,
            Prm1,
            Prm2,
            Prm3,
            Prm4,
            Lat,
            Lng,
            Alt,
            None
        }

        /// <summary>
        /// Enum for GPS-Frame
        /// </summary>
        enum GPSFrame
        {
            HDop,
            Lat,
            Lng,
            RelAlt,
            RAlt,
            None
        }

        /// <summary>
        /// Enum for CURR-Frame
        /// </summary>
        enum CURRFrame
        {
            Curr,
            Volt,
            CurrTot,
            None
        }

        /// <summary>
        /// set the flightmode interpretation
        /// </summary>
        enum FlightMode
        {
            Hover,
            Delay_at_Waypoint,
            StraigtForward,
            StraightUP,
            StraightDOWN,
            Climb,
            Descent,
            Warning
        }

        /// <summary>
        /// Enum for different states.
        /// </summary>
        enum ESearchFlag
        {
            First,
            Start,
            End,
        }


        // ----------------------------------------------
        //  methods for energy-consumption and -profile
        // ----------------------------------------------

        /// <summary>
        /// Export the energy-profile in a xml-file.
        /// </summary>
        public void ExportProfile()
        {
            int serialNumber = 0;

            // check if dirfectory exist
            if (!(Directory.Exists(_energyProfilePath)))
            {
                Directory.CreateDirectory(_energyProfilePath);
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                InitialDirectory = _energyProfilePath,
                FileName = serialNumber
                    .ToString(),
                AddExtension = true,
                Filter = @"Energyprofile settings (*.xml)|*.xml",
                DefaultExt = ".xml"
            };
            //file is saved with the id to allow automatic loading of values if id is set in copter

            if (DialogResult.OK == sfd.ShowDialog())
            {
                WriteXML(sfd.FileName);
            }

        }

        /// <summary>
        /// Import the energy-profile from xml-file.
        /// </summary>
        public bool ImportProfile()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                InitialDirectory = _energyProfilePath,
                Multiselect = false,
                Filter = @"Energyprofile settings (*.xml)|*.xml",
                DefaultExt = ".xml"
            };

            if (DialogResult.OK == ofd.ShowDialog())
            {
                ReadXML(ofd.FileName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// This should be a spline interpolation for the chart nodes.
        /// http://siegert.f2.htw-berlin.de/Buero/Arblaetter/arblSPLINE.html
        /// The gaussian algorithm code has parts from 
        /// https://trainyourprogrammer.de/csharp-122-loesen-eines-linearen-gleichungssystems.html
        /// </summary>
        /// <param name="_x">This are the supporting values for X</param>
        /// <param name="_y">This are the supporting values for Y</param>
        /// <param name="_steps">This is the steps for each angle</param>
        /// <returns>A PointList of all angles</returns>
        private List<PointF> SplineInterpolation(double[] _x, double[] _y, double _steps)
        {
            List<PointF> interpPoints = new List<PointF>();
            int n = _x.Length - 1;
            double[] h = new double[n];
            double[,] matrix = new Double[n - 1, n];

            // 1. calculate the c-moments
            for (int i = 0; i < _x.Length - 1; i++)
            {
                h[i] = _x[i + 1] - _x[i];
            }

            for (int i = 0; i < n - 1; i++)
            {
                for (int col = 0; col < n; col++)
                {
                    if (col.Equals(i))
                    {
                        matrix[i, col] = Math.Round(2 * (h[i + 1] + h[i]), 2);
                    }
                    else if (col.Equals(i - 1))
                    {
                        matrix[i, col] = Math.Round(h[i], 2);
                    }
                    else if (col.Equals(i + 1) && !col.Equals(n - 1))
                    {
                        matrix[i, col] = Math.Round(h[i + 1], 2);
                    }
                    else if (col.Equals(h.Length - 1))
                    {
                        double r = Math.Round(6 * ((_y[i + 2] - _y[i + 1]) / h[i + 1] - (_y[i + 1] - _y[i]) / h[i]), 2);

                        matrix[i, col] = r;
                    }
                    else
                    {
                        matrix[i, col] = 0;
                    }
                }
            }
            // testing gaussian algorithm
            TestGauss();
            Double[] coeffs = Gauss(matrix);

            double[] c = new double[n + 1];

            for (int i = 0; i <= n; i++)
            {
                if (i.Equals(0) || i.Equals(n))
                {
                    c[i] = 0;
                }
                else
                {
                    c[i] = coeffs[i - 1];
                }
            } // --> End of calculating c-moments

        
            // 2. calculate the a, b and d values
            double[] a = new double[n];
            double[] b = new double[n];
            double[] d = new double[n];

            for (int i = 0; i < n; i++)
            {
                a[i] = _y[i];
                d[i] = Math.Round((c[i + 1] - c[i]) / h[i], 2);
                b[i] = Math.Round((_y[i + 1] - _y[i]) / h[i] - (double)Decimal.Divide(1, 2) * c[i] * h[i] - (double)Decimal.Divide(1, 6) * d[i] * Math.Pow(h[i], 2), 2);
            } // --> End calculating a,b,d - values

            // calculate result 
            for (int i = 0; i < n; i++)
            {
                for (double x = Math.Round(_x[i], 2); x < Math.Round(_x[i + 1], 2); x = Math.Round(x + _steps, 2))
                {
                    var y = Math.Round(a[i] + b[i] * (x - _x[i]) + c[i] / 2 * Math.Pow(x - _x[i], 2) +
                                              d[i] / 6 * Math.Pow(x - _x[i], 3), 2);
                    interpPoints.Add(new PointF((float)x, (float)y));
                }
                // insert last point
                if (i.Equals(n - 1))
                {
                    interpPoints.Add(new PointF((float)Math.Round(_x[n], 2), (float)Math.Round(_y[n], 2)));
                }
            }
            return interpPoints;
        }

        /// <summary>
        /// This is the Gauss-Elimination-Algorithm. (code-idea from eulerscheZhl in "Train Your Programmer")
        /// https://trainyourprogrammer.de/csharp-A122-L2-loesen-eines-linearen-gleichungssystems.html
        /// </summary>
        /// <param name="_matrix">coefficients matrix</param>
        /// <returns>coefficients as array</returns>
        static double[] Gauss(double[,] _matrix)
        {
            for (int line = 0; line < _matrix.GetLength(0); line++)
            {
                // change lines if necessary
                try
                {
                    if (_matrix[line, line].Equals(0))
                    {
                        for (int tmpline = line + 1; tmpline < _matrix.GetLength(0) - 1; tmpline++)
                        {
                            if (!_matrix[tmpline, line].Equals(0))
                            {
                                for (int x = 0; x < _matrix.GetLength(1); x++)
                                {
                                    double tmp = _matrix[x, line];
                                    _matrix[x, line] = _matrix[x, tmpline];
                                    _matrix[x, tmpline] = tmp;
                                }
                                break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                if (_matrix[line, line].Equals(0))
                    throw new DivideByZeroException("no clear solution");
                // subtract lines
                for (int line2 = line + 1; line2 < _matrix.GetLength(0); line2++)
                {
                    if (_matrix[line2, line].Equals(0))
                        continue;
                    //double factor = Math.Round(_matrix[line2, line] / _matrix[line, line], 1);
                    double factor = _matrix[line2, line] / _matrix[line, line];
                    for (int x = line; x < _matrix.GetLength(1); x++)
                    {
                        _matrix[line2, x] -= Math.Round(factor * _matrix[line, x], 1);
                    }
                }
            }
            // equation-matrix is finish
            double[] result = new double[_matrix.GetLength(0)];
            for (int line = _matrix.GetLength(0) - 1; line >= 0; line--)
            {
                result[line] = _matrix[line, _matrix.GetLength(1) - 1];
                for (int x = line + 1; x < result.Length; x++)
                {
                    result[line] -= Math.Round(result[x] * _matrix[line, x], 1);
                }
                try
                {
                    result[line] = Math.Round(result[line] / _matrix[line, line], 1);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

            }
            return result;
        }

        /// <summary>
        /// // formula for linear interpolation (https://en.wikipedia.org/wiki/Linear_interpolation)
        /// </summary>
        /// <param name="_x">This are the supporting values for X</param>
        /// <param name="_y">This are the supporting values for Y</param>
        /// <param name="_steps">This is the steps for each angle</param>
        /// <returns></returns>
        private List<PointF> LinearInterpolation(double[] _x, double[] _y, double _steps)
        {
            double n = _x.Length - 1;
            List<PointF> interpPoints = new List<PointF>();
            for (int i = 0; i < n; i++)
            {
                for (double x = Math.Round(_x[i], 2); x < Math.Round(_x[i + 1], 2); x = Math.Round(x + _steps, 2))
                {
                    var y = Math.Round(((_y[i + 1] - _y[i]) / (_x[i + 1] - _x[i])) * x + _y[i] - ((_y[i + 1] - _y[i]) / (_x[i + 1] - _x[i])) * _x[i], 2);
                    interpPoints.Add(new PointF((float)x, (float)Math.Round(y, 2)));
                }
                // last point
                if (i >= n - 1)
                {
                    interpPoints.Add(new PointF((float)Math.Round(_x[i + 1], 2), (float)Math.Round(_y[i + 1], 2)));
                }
            }
            return interpPoints;
        }

        /// <summary>
        /// This methos fill PointList with default-values
        /// </summary>
        /// <returns></returns>
        private List<PointF> InterpListNull()
        {
            List<PointF> interpPoints = new List<PointF>();
            for (int i = -90; i <= 181; i++)
            {
                interpPoints.Add(new PointF(i, 0));
            }
            return interpPoints;
        }

        /// <summary>
        /// This method interpolate points between two nodes. It fills a list with new interpolated spline-points.
        /// </summary>
        public bool Interpolation(PlotProfile profile)
        {
            bool validValues = true;
            switch (profile)
            {
                case PlotProfile.Current:
                    EnergyProfileModel.AverageCurrentSplinePoints.Clear();
                    EnergyProfileModel.MaxCurrentSplinePoints.Clear();
                    EnergyProfileModel.MinCurrentSplinePoints.Clear();
                    double[] angels2curr = new double[EnergyProfileModel.CurrentSet.Count];
                    double[] currents = new double[EnergyProfileModel.CurrentSet.Count];
                    double[] maxcurrents = new double[EnergyProfileModel.CurrentSet.Count];
                    double[] mincurrents = new double[EnergyProfileModel.CurrentSet.Count];
                    for (int i = 1; i < EnergyProfileModel.CurrentSet.Count + 1; i++)
                    {
                        if (EnergyProfileModel.CurrentSet[i].Angle.Equals(0) && !i.Equals(6))
                        {
                            validValues = false;
                            EnergyProfileModel.InterpModeCurr = EnergyProfileModel.InterpolationMode.None;
                            break;
                        }
                        angels2curr[i - 1] = Math.Round(EnergyProfileModel.CurrentSet[i].Angle, 2);
                        currents[i - 1] = Math.Round(EnergyProfileModel.CurrentSet[i].AverageCurrent, 2);
                        maxcurrents[i - 1] = Math.Round(EnergyProfileModel.CurrentSet[i].MaxCurrent, 2);
                        mincurrents[i - 1] = Math.Round(EnergyProfileModel.CurrentSet[i].MinCurrent, 2);
                    }

                    switch (EnergyProfileModel.InterpModeCurr)
                    {
                        case EnergyProfileModel.InterpolationMode.LinearInterp:
                            EnergyProfileModel.AverageCurrentSplinePoints =
                                LinearInterpolation(angels2curr, currents, 1);
                            EnergyProfileModel.MaxCurrentSplinePoints =
                                LinearInterpolation(angels2curr, maxcurrents, 1);
                            EnergyProfileModel.MinCurrentSplinePoints =
                                LinearInterpolation(angels2curr, mincurrents, 1);
                            break;
                        case EnergyProfileModel.InterpolationMode.CubicSpline:
                            EnergyProfileModel.AverageCurrentSplinePoints =
                                SplineInterpolation(angels2curr, currents, 1);
                            EnergyProfileModel.MaxCurrentSplinePoints =
                                SplineInterpolation(angels2curr, maxcurrents, 1);
                            EnergyProfileModel.MinCurrentSplinePoints =
                                SplineInterpolation(angels2curr, mincurrents, 1);
                            break;
                        case EnergyProfileModel.InterpolationMode.None:
                            EnergyProfileModel.AverageCurrentSplinePoints = InterpListNull();
                            EnergyProfileModel.MaxCurrentSplinePoints = InterpListNull();
                            EnergyProfileModel.MinCurrentSplinePoints = InterpListNull();
                            break;
                    }

                    break;
                case PlotProfile.Velocity:
                    EnergyProfileModel.AverageVelocitySplinePoints.Clear();
                    EnergyProfileModel.MaxVelocitySplinePoints.Clear();
                    EnergyProfileModel.MinVelocitySplinePoints.Clear();
                    double[] angels2vel = new double[EnergyProfileModel.VelocitySet.Count];
                    double[] velocitys = new double[EnergyProfileModel.VelocitySet.Count];
                    double[] maxvelocitys = new double[EnergyProfileModel.VelocitySet.Count];
                    double[] minvelocitys = new double[EnergyProfileModel.VelocitySet.Count];
                    for (int i = 1; i < EnergyProfileModel.VelocitySet.Count + 1; i++)
                    {
                        if (EnergyProfileModel.VelocitySet[i].Angle.Equals(0) && !i.Equals(6))
                        {
                            validValues = false;
                            EnergyProfileModel.InterpModeVel = EnergyProfileModel.InterpolationMode.None;
                            break;
                        }
                        angels2vel[i - 1] = Math.Round(EnergyProfileModel.VelocitySet[i].Angle, 2);
                        velocitys[i - 1] = Math.Round(EnergyProfileModel.VelocitySet[i].AverageVelocity, 2);
                        //maxvelocitys[i - 1] = Math.Round(EnergyProfileModel.VelocitySet[i].MaxVelocity, 2);
                        //minvelocitys[i - 1] = Math.Round(EnergyProfileModel.VelocitySet[i].MinVelocity, 2);
                    }
                    switch (EnergyProfileModel.InterpModeVel)
                    {
                        case EnergyProfileModel.InterpolationMode.LinearInterp:
                            EnergyProfileModel.AverageVelocitySplinePoints =
                                LinearInterpolation(angels2vel, velocitys, 1);
                            EnergyProfileModel.MaxVelocitySplinePoints =
                                LinearInterpolation(angels2vel, maxvelocitys, 1);
                            EnergyProfileModel.MinVelocitySplinePoints =
                                LinearInterpolation(angels2vel, minvelocitys, 1);
                            break;
                        case EnergyProfileModel.InterpolationMode.CubicSpline:
                            EnergyProfileModel.AverageVelocitySplinePoints =
                                SplineInterpolation(angels2vel, velocitys, 1);
                            EnergyProfileModel.MaxVelocitySplinePoints =
                                SplineInterpolation(angels2vel, maxvelocitys, 1);
                            EnergyProfileModel.MinVelocitySplinePoints =
                                SplineInterpolation(angels2vel, minvelocitys, 1);
                            break;
                        case EnergyProfileModel.InterpolationMode.None:
                            EnergyProfileModel.AverageVelocitySplinePoints = InterpListNull();
                            EnergyProfileModel.MaxVelocitySplinePoints = InterpListNull();
                            EnergyProfileModel.MinVelocitySplinePoints = InterpListNull();
                            break;
                    }

                    break;
            }
            // write interp-points into txt file
            WritePlotLogfile(profile);
            return validValues;
        }

        /// <summary>
        /// plot the current
        /// </summary>
        public void Plot(Chart chart, PlotProfile profile)
        {
            try
            {

                switch (profile)
                {
                    case PlotProfile.Current:
                        // init series

                        if (chart.Series != null)
                        {
                            Series avrgCrnt = chart.Series["AverageCurrent"];
                            Series minCrnt = chart.Series["MinCurrent"];
                            Series maxCrnt = chart.Series["MaxCurrent"];
                            Series range = chart.Series["Range"];

                            // clear series
                            avrgCrnt.Points.Clear();
                            maxCrnt.Points.Clear();
                            minCrnt.Points.Clear();
                            range.Points.Clear();

                            switch (EnergyProfileModel.InterpModeCurr)
                            {
                                case EnergyProfileModel.InterpolationMode.LinearInterp:
                                    avrgCrnt.ChartType = SeriesChartType.Line;
                                    maxCrnt.ChartType = SeriesChartType.Line;
                                    minCrnt.ChartType = SeriesChartType.Line;
                                    range.ChartType = SeriesChartType.Range;
                                    break;
                                case EnergyProfileModel.InterpolationMode.CubicSpline:
                                    avrgCrnt.ChartType = SeriesChartType.Spline;
                                    maxCrnt.ChartType = SeriesChartType.Spline;
                                    minCrnt.ChartType = SeriesChartType.Spline;
                                    range.ChartType = SeriesChartType.SplineRange;
                                    break;
                            }


                            // set new series
                            range.Points.DataBindXY(xValue: EnergyProfileModel.CurrentSet.Values, xField: "Angle",
                                yValue: EnergyProfileModel.CurrentSet.Values, yFields: "MaxCurrent,MinCurrent");
                            avrgCrnt.Points.DataBindXY(xValue: EnergyProfileModel.CurrentSet.Values,
                                xField: "Angle",
                                yValue: EnergyProfileModel.CurrentSet.Values, yFields: "AverageCurrent");
                            maxCrnt.Points.DataBindXY(xValue: EnergyProfileModel.CurrentSet.Values, xField: "Angle",
                                yValue: EnergyProfileModel.CurrentSet.Values,
                                yFields: "MaxCurrent");
                            minCrnt.Points.DataBindXY(xValue: EnergyProfileModel.CurrentSet.Values, xField: "Angle",
                                yValue: EnergyProfileModel.CurrentSet.Values,
                                yFields: "MinCurrent");

                            //scale graph
                            var findMinByValue = minCrnt.Points.FindMinByValue();
                            if (findMinByValue != null)
                                chart.ChartAreas[0].AxisY.Minimum = findMinByValue.YValues[0] - 2.00f;
                            var findMaxByValue = maxCrnt.Points.FindMaxByValue();
                            if (findMaxByValue != null)
                                chart.ChartAreas[0].AxisY.Maximum = findMaxByValue.YValues[0] + 2.00f;
                        }
                        else
                            CustomMessageBox.Show("Error: Empty Chart.");


                        break;
                    case PlotProfile.Velocity:
                        // init series

                        if (chart.Series != null)
                        {
                            Series avrgVel = chart.Series["AverageVelocity"];
                            Series minVel = chart.Series["MinVelocity"];
                            Series maxVel = chart.Series["MaxVelocity"];
                            Series range = chart.Series["Range"];

                            // clear series
                            avrgVel.Points.Clear();
                            maxVel.Points.Clear();
                            minVel.Points.Clear();
                            range.Points.Clear();

                            switch (EnergyProfileModel.InterpModeVel)
                            {
                                case EnergyProfileModel.InterpolationMode.LinearInterp:
                                    avrgVel.ChartType = SeriesChartType.Line;
                                    maxVel.ChartType = SeriesChartType.Line;
                                    minVel.ChartType = SeriesChartType.Line;
                                    range.ChartType = SeriesChartType.Range;
                                    break;
                                case EnergyProfileModel.InterpolationMode.CubicSpline:
                                    avrgVel.ChartType = SeriesChartType.Spline;
                                    maxVel.ChartType = SeriesChartType.Spline;
                                    minVel.ChartType = SeriesChartType.Spline;
                                    range.ChartType = SeriesChartType.SplineRange;
                                    break;
                            }

                            // set new series
                            range.Points.DataBindXY(xValue: EnergyProfileModel.VelocitySet.Values, xField: "Angle",
                                yValue: EnergyProfileModel.VelocitySet.Values, yFields: "MaxVelocity,MinVelocity");
                            avrgVel.Points.DataBindXY(xValue: EnergyProfileModel.VelocitySet.Values,
                                xField: "Angle",
                                yValue: EnergyProfileModel.VelocitySet.Values, yFields: "AverageVelocity");
                            maxVel.Points.DataBindXY(xValue: EnergyProfileModel.VelocitySet.Values, xField: "Angle",
                                yValue: EnergyProfileModel.VelocitySet.Values,
                                yFields: "MaxVelocity");
                            minVel.Points.DataBindXY(xValue: EnergyProfileModel.VelocitySet.Values, xField: "Angle",
                                yValue: EnergyProfileModel.VelocitySet.Values,
                                yFields: "MinVelocity");

                            //scale graph
                            var findMinByValue = minVel.Points.FindMinByValue();
                            if (findMinByValue != null)
                                chart.ChartAreas[0].AxisY.Minimum = findMinByValue.YValues[0] - 1.00f;
                            var findMaxByValue = maxVel.Points.FindMaxByValue();
                            if (findMaxByValue != null)
                            {
                                chart.ChartAreas[0].AxisY.Maximum = findMaxByValue.YValues[0] + 2.00f;
                            }
                        }
                        else
                            CustomMessageBox.Show("Error: Empty Chart.");


                        break;
                }
            }
            catch (Exception e)
            {
                CustomMessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// This methode are writing the xml-file.
        /// </summary>
        /// <param name="path">This is the path to save the xml-file</param>
        private void WriteXML(string path)
        {
            Dictionary<int, CurrentModel> energyCurrentSet = EnergyProfileModel.CurrentSet;
            Dictionary<int, VelocityModel> energyVelocitySet = EnergyProfileModel.VelocitySet;
            XElement root = new XElement(XMLTag.Sets.ToString());
            XElement CrntSet = new XElement(XMLTag.CurrentSet.ToString(),
                new XAttribute(XMLTag.DevPercent.ToString(), EnergyProfileModel.PercentDevCrnt));

            var xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);

            XElement xCurrentModelHover = new XElement(XMLTag.CurrentModel.ToString(),
                new XAttribute(XMLTag.ID.ToString(), XMLTag.Hover.ToString()),
                new XElement(XMLTag.AverageCurrent.ToString(), EnergyProfileModel.CurrentHover.AverageCurrent),
                new XElement(XMLTag.Deviation.ToString(), EnergyProfileModel.CurrentHover.Deviation));
            CrntSet.Add(xCurrentModelHover);
            foreach (KeyValuePair<int, CurrentModel> currentModel in energyCurrentSet)
            {
                XElement xCurrentModel = new XElement(XMLTag.CurrentModel.ToString(),
                    new XAttribute(XMLTag.ID.ToString(), currentModel.Key),
                    new XElement(XMLTag.Angle.ToString(), currentModel.Value.Angle),
                    new XElement(XMLTag.AverageCurrent.ToString(), currentModel.Value.AverageCurrent),
                    new XElement(XMLTag.Deviation.ToString(), currentModel.Value.Deviation));
                CrntSet.Add(xCurrentModel);
            }
            root.Add(CrntSet);

            XElement VelSet = new XElement(XMLTag.VelocitySet.ToString());
            foreach (KeyValuePair<int, VelocityModel> velocityModel in energyVelocitySet)
            {
                XElement xVelocitytModel = new XElement(XMLTag.VelocityModel.ToString(),
                    new XAttribute(XMLTag.ID.ToString(), velocityModel.Key),
                    new XElement(XMLTag.Angle.ToString(), velocityModel.Value.Angle),
                    new XElement(XMLTag.AverageVelocity.ToString(), velocityModel.Value.AverageVelocity),
                    new XElement(XMLTag.Deviation.ToString(), velocityModel.Value.Deviation));
                VelSet.Add(xVelocitytModel);
            }
            root.Add(VelSet);

            xDoc.Save(path);
        }

        /// <summary>
        /// Reading the xml-file
        /// </summary>
        /// <param name="path">This is the path from xml-file.</param>
        private void ReadXML(string path)
        {
            // clear Dictionary Energy-CurrentSet 
            EnergyProfileModel.CurrentSet.Clear();
            EnergyProfileModel.VelocitySet.Clear();
            // load and set root element
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(path);
            XmlNode root = xdoc.SelectSingleNode("Sets");
            if (root != null)
            {
                XmlNode CrntSet = root.SelectSingleNode(XMLTag.CurrentSet.ToString());
                if (CrntSet != null)
                {
                    XmlNodeList currentModelList = CrntSet.SelectNodes("//" + XMLTag.CurrentModel);
                    var attributePerc = CrntSet.Attributes?[XMLTag.DevPercent.ToString()];
                    if (currentModelList != null)
                    {
                        foreach (XmlNode currentmodel in currentModelList)
                        {
                            if (currentmodel.Attributes != null)
                            {
                                var attributeID = currentmodel.Attributes[XMLTag.ID.ToString()];
                                var xmlElementAngle = currentmodel[XMLTag.Angle.ToString()];
                                var xmlElementCurrent = currentmodel[XMLTag.AverageCurrent.ToString()];
                                var xmlElementDeviation = currentmodel[XMLTag.Deviation.ToString()];

                                if (attributePerc != null)
                                {
                                    EnergyProfileModel.PercentDevCrnt =
                                        Convert.ToDouble(attributePerc.Value.Replace(".", ","));

                                    if (attributeID.Value == XMLTag.Hover.ToString())
                                    {
                                        if (xmlElementCurrent != null)
                                            EnergyProfileModel.CurrentHover =
                                                new CurrentModel(0,
                                                    Convert.ToDouble(xmlElementCurrent.InnerText.Replace(".", ",")));
                                    }
                                    else
                                    {
                                        if (xmlElementAngle != null && xmlElementCurrent != null)
                                            EnergyProfileModel.CurrentSet.Add(Convert.ToInt16(attributeID.Value),
                                                new CurrentModel(Convert.ToDouble(xmlElementAngle.InnerText),
                                                    Convert.ToDouble(xmlElementCurrent.InnerText.Replace(".", ","))));
                                    }
                                }
                                else
                                {
                                    if (xmlElementAngle != null &&
                                        (xmlElementCurrent != null && xmlElementDeviation != null))
                                        EnergyProfileModel.CurrentSet.Add(Convert.ToInt16(attributeID.Value),
                                            new CurrentModel(Convert.ToDouble(xmlElementAngle.InnerText),
                                                Convert.ToDouble(xmlElementCurrent.InnerText.Replace(".", ",")),
                                                Convert.ToDouble(xmlElementDeviation.InnerText.Replace(".", ","))));
                                }
                            }
                        }
                    }
                }
                XmlNode velSet = root.SelectSingleNode(XMLTag.VelocitySet.ToString());
                // ReSharper disable once UseNullPropagation
                if (velSet != null)
                {
                    XmlNodeList velModelList = velSet.SelectNodes("//" + XMLTag.VelocityModel);

                    if (velModelList != null)
                        foreach (XmlNode velocitymodel in velModelList)
                        {
                            if (velocitymodel.Attributes != null)
                            {
                                var attributeID = velocitymodel.Attributes[XMLTag.ID.ToString()];
                                var xmlElementAngle = velocitymodel[XMLTag.Angle.ToString()];
                                var xmlElementVelocity = velocitymodel[XMLTag.AverageVelocity.ToString()];
                                var xmlElementDeviation = velocitymodel[XMLTag.Deviation.ToString()];
                                if (xmlElementAngle != null &&
                                    (xmlElementVelocity != null && xmlElementDeviation != null))
                                    EnergyProfileModel.VelocitySet.Add(Convert.ToInt16(attributeID.Value),
                                        new VelocityModel(Convert.ToDouble(xmlElementAngle.InnerText),
                                            Convert.ToDouble(xmlElementVelocity.InnerText.Replace(".", ",")),
                                            Convert.ToDouble(xmlElementDeviation.InnerText.Replace(".", ","))));
                            }
                        }
                }
            }
        }

        /// <summary>
        /// changing the fix deviation
        /// </summary>
        /// <param name="dev"></param>
        public void ChangeDeviation(int dev)
        {
            EnergyProfileModel.PercentDevCrnt = Math.Round(((double)dev / 100), 2);
        }

        /// <summary>
        /// Calculate the Energy-Consumption of actual part of moving
        /// </summary>
        /// <param name="dist">distance (only horizontal) between two waypoints</param>
        /// <param name="angle">the angle between two waypoints</param>
        /// <param name="altitudeDiff">result of altitude WP1 and WP2</param>
        /// <param name="hoverTime">the delay time btw the loiter_Time</param>
        /// <returns>energy-consumption in mAh</returns>
        public Dictionary<ECID, double> EnergyConsumption(double dist, double angle,
            double altitudeDiff, double hoverTime)
        {
            // testing the calculating methods
            TestCalculateFlightTime();
            TestCalculateEnergyConsumption();

            angle = Math.Round(angle);
            var distance = dist;

            if (Math.Abs(distance) <= 0.001)
                distance = altitudeDiff;

            // search current values and add in dict
            double currentAvrg = (from p in EnergyProfileModel.AverageCurrentSplinePoints
                                  where Math.Abs(p.X - angle) < 0.6
                                  select p.Y).FirstOrDefault();
            double currentMax = (from p in EnergyProfileModel.MaxCurrentSplinePoints
                                 where Math.Abs(p.X - angle) < 0.6
                                 select p.Y).FirstOrDefault();
            double currentMin = (from p in EnergyProfileModel.MinCurrentSplinePoints
                                 where Math.Abs(p.X - angle) < 0.6
                                 select p.Y).FirstOrDefault();

            Dictionary<ECID, double> current =
                new Dictionary<ECID, double>
                {
                    {ECID.AverageCurrent, currentAvrg},
                    {ECID.MaxCurrent, currentMax},
                    {ECID.MinCurrent, currentMin}
                };

            // search for velocity values and calculate the flight times an add in dict
            double speedAvrg = (from p in EnergyProfileModel.AverageVelocitySplinePoints
                                where Math.Abs(p.X - angle) < 0.6
                                select p.Y).FirstOrDefault();
            double speedMax = (from p in EnergyProfileModel.MaxVelocitySplinePoints
                               where Math.Abs(p.X - angle) < 0.6
                               select p.Y).FirstOrDefault();
            double speedMin = (from p in EnergyProfileModel.MinVelocitySplinePoints
                               where Math.Abs(p.X - angle) < 0.6
                               select p.Y).FirstOrDefault();

            double flighttime_avr = CalcFlightTime(distance, speedAvrg);
            double flighttime_max = CalcFlightTime(distance, speedMax);
            double flighttime_min = CalcFlightTime(distance, speedMin);

            Dictionary<ECID, double> flightTimes =
                new Dictionary<ECID, double>
                {
                    {ECID.AverageVelocity, flighttime_avr},
                    {ECID.MaxVelocity, flighttime_max},
                    {ECID.MinVelocity, flighttime_min}
                };
            // if delay from hover
            double hoverEnergyConsumption = 0;
            if (hoverTime > 0)
            {
                hoverEnergyConsumption =
                    CalculateEnergyConsumption(EnergyProfileModel.CurrentHover.AverageCurrent, hoverTime);
            }

            double energyConsumptionAvrg = 0f;
            double energyConsumptionMax = 0f;
            double flightTimeAvrg = 0f;

            if (flightTimes.ContainsKey(ECID.AverageVelocity))
            {
                flightTimeAvrg = flightTimes[ECID.AverageVelocity];

                if (current.ContainsKey(ECID.AverageCurrent))
                {
                    energyConsumptionAvrg =
                        CalculateEnergyConsumption(current[ECID.AverageCurrent], flightTimes[ECID.AverageVelocity]);
                }
                if (current.ContainsKey(ECID.MaxCurrent))
                    energyConsumptionMax =
                        CalculateEnergyConsumption(current[ECID.MaxCurrent], flightTimes[ECID.AverageVelocity]);
            }

            Dictionary<ECID, double> returnValues =
                new Dictionary<ECID, double>
                {
                    {ECID.avrCur_avrTime, energyConsumptionAvrg + hoverEnergyConsumption},
                    {ECID.maxCur_avrTime, energyConsumptionMax + hoverEnergyConsumption},
                    {ECID.FlightTime, hoverTime + flightTimeAvrg}
                };
            return returnValues;
        }

        /// <summary>
        /// calculate the climb distance ... like triangle
        /// </summary>
        /// <param name="distance_Horizontal">This is the distance from specific waypoint below FlightPlanner.</param>
        /// <param name="angle">This is the angle from specific waypoint below FlightPlanner.</param>
        /// <returns>Returns the diagonale-distance (DOUBLE)</returns>
        private double CalcClimbDistance(double distance_Horizontal, double angle)
        {
            double distance = 0.00f;
            if ((Math.Abs(angle) < 90))
            {
                distance = Math.Round(distance_Horizontal / Math.Cos(Math.PI / 180 * angle), 2);
            }
            return distance;
        }

        /// <summary>
        /// Calculates the pure flight time without acceleration^.
        /// </summary>
        /// <param name="distance">const distance in [m]</param>
        /// <param name="speed">velocity in [m/s]</param>
        /// <returns>fligh time in [s]</returns>
        private double CalcFlightTime(double distance, double speed)
        {
            double flighttime = 0.0f;
            if (speed > 0 && distance > 0)
                flighttime = Math.Round(distance / speed);
            return flighttime;
        }

        /// <summary>
        /// Calculate the needed energy from WP1 to WP2
        /// </summary>
        /// <param name="current">The current at specific angle.</param>
        /// <param name="flighttime">The flight time between 2 waypoints.</param>
        /// <returns>The consumed energy of flight between 2 waypoints</returns>
        private double CalculateEnergyConsumption(double current, double flighttime)
        {
            double ec = 0.00f;
            if (current > 0 && flighttime > 0)
            {
                ec = Math.Round(current * flighttime * 1000 / 3600, 1,
                    MidpointRounding.AwayFromZero); // conversion from [As] in [mAh]
            }
            return ec;
        }

        /// <summary>
        /// Write the Splinepoints into a text-file
        /// </summary>
        /// <param name="profile">Current or Velocity</param>
        void WritePlotLogfile(PlotProfile profile)
        {
            string path = _energyProfilePath + @"InterpProfile\";
            // create dir if not exist
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            List<PointF> splineListAvrg = new List<PointF>();
            List<PointF> splineListMax = new List<PointF>();
            List<PointF> splineListMin = new List<PointF>();
            switch (profile)
            {
                case PlotProfile.Current:
                    splineListAvrg = EnergyProfileModel.AverageCurrentSplinePoints;
                    splineListMax = EnergyProfileModel.MaxCurrentSplinePoints;
                    splineListMin = EnergyProfileModel.MinCurrentSplinePoints;
                    break;
                case PlotProfile.Velocity:
                    splineListAvrg = EnergyProfileModel.AverageVelocitySplinePoints;
                    splineListMax = EnergyProfileModel.MaxVelocitySplinePoints;
                    splineListMin = EnergyProfileModel.MinVelocitySplinePoints;
                    break;
            }
            StreamWriter file_Avrg = new StreamWriter(path + "Avrg_InterpPoints" + profile + ".txt");
            foreach (PointF point in splineListAvrg)
            {
                file_Avrg.WriteLine(point.X + " --> " + point.Y);
            }
            file_Avrg.Close();
            StreamWriter file_Max = new StreamWriter(path + "Max_InterpPoints" + profile + ".txt");
            foreach (PointF point in splineListMax)
            {
                file_Max.WriteLine(point.X + " --> " + point.Y);
            }
            file_Max.Close();
            StreamWriter file_Min = new StreamWriter(path + "Min_InterpPoints" + profile + ".txt");
            foreach (PointF point in splineListMin)
            {
                file_Min.WriteLine(point.X + " --> " + point.Y);
            }
            file_Min.Close();
        }

        /// <summary>
        /// Test for calculate climb distance
        /// </summary>
        private void TestCalculateClimbDistance()
        {
            var distance1 = CalcClimbDistance(50, -90);
            var distance2 = CalcClimbDistance(50, +90);
            var distance3 = CalcClimbDistance(100, 0);
            var distance4 = CalcClimbDistance(50, -37);
            var distance5 = CalcClimbDistance(50, 18);

            Assert.IsEquals((double)0, distance1, "Wrong value (distance1) of CalculateClimbDistance");
            Assert.IsEquals((double)0, distance2, "Wrong value (distance2) of CalculateClimbDistance");
            Assert.IsEquals((double)100, distance3, "Wrong value (distance3) of CalculateClimbDistance");
            Assert.IsEquals(62.61, distance4, "Wrong value (distance4) of CalculateClimbDistance");
            Assert.IsEquals(52.57, distance5, "Wrong value (distance5) of CalculateClimbDistance");
        }

        /// <summary>
        /// Test for calculate flight time
        /// </summary>
        private void TestCalculateFlightTime()
        {
            var flighttime1 = CalcFlightTime(50, 0);
            var flighttime2 = CalcFlightTime(50, 7.95);
            var flighttime3 = CalcFlightTime(-10, 6.92);
            var flighttime4 = CalcFlightTime(50, -37);
            var flighttime5 = CalcFlightTime(-50, -37);

            Assert.IsEquals((double)0, flighttime1, "--> Wrong value (flighttime1) of CalculateClimbDistance");
            Assert.IsEquals((double)6, flighttime2, "--> Wrong value (flighttime2) of CalculateClimbDistance");
            Assert.IsEquals((double)0, flighttime3, "--> Wrong value (flighttime3) of CalculateClimbDistance");
            Assert.IsEquals((double)0, flighttime4, "--> Wrong value (flighttime4) of CalculateClimbDistance");
            Assert.IsEquals((double)0, flighttime5, "--> Wrong value (flighttime5) of CalculateClimbDistance");
        }

        /// <summary>
        /// Test for calculate coeff matrix over gaussian formula
        /// </summary>
        private void TestGauss()
        {
            double[,] testmatrix = {{2, 0.5, 0, -1.2}, { 0.5, 2, 0.5, -4.8 } , { 0, 0.5, 2, -1.2 } };

            double[] coeff = Gauss(testmatrix);

            Assert.IsEquals((double)0, coeff[0], "--> Wrong coeff[0] from gaussian algorithm.");
            Assert.IsEquals((double)-2.4, coeff[1], "--> Wrong coeff[1] from gaussian algorithm.");
            Assert.IsEquals((double)0, coeff[2], "--> Wrong coeff[2] from gaussian algorithm.");
        }


        /// <summary>
        /// Test for calculate consumed energy
        /// </summary>
        private void TestCalculateEnergyConsumption()
        {
            var ec1 = CalculateEnergyConsumption(12.12, -6);
            var ec2 = CalculateEnergyConsumption(-12.12, 6);
            var ec3 = CalculateEnergyConsumption(-12.12, -6);
            var ec4 = CalculateEnergyConsumption(12.12, 6);
            var ec5 = CalculateEnergyConsumption(12.12, 0);

            Assert.IsEquals((double)0, ec1, "Wrong value (ec1) of CalculateClimbDistance");
            Assert.IsEquals((double)0, ec2, "Wrong value (ec2) of CalculateClimbDistance");
            Assert.IsEquals((double)0, ec3, "Wrong value (ec3) of CalculateClimbDistance");
            Assert.IsEquals(20.2, ec4, "Wrong value (ec4) of CalculateClimbDistance");
            Assert.IsEquals((double)0, ec5, "Wrong value (ec5) of CalculateClimbDistance");
        }

        // -------------------------------------
        //  methods for LogAnalyzer
        // -------------------------------------

        /// <summary>
        /// Parsing the Logfile for all relevant values for calculate the energyprofile and save it in the energylogfilemodel and return it
        /// </summary>
        /// <param name="logData">CollectionBuffer of actual parsing logfile</param>
        /// <returns>EnergyLogFileModel</returns>
        private EnergyLogFileModel LogDatas(CollectionBuffer logData)
        {
            Dictionary<string, string[]> Columns_Data = new Dictionary<string, string[]>();
            int timedelay;
            if (statsmodus.Equals(StatisticModus.GPS_Speed) || statsmodus.Equals(StatisticModus.GPS_Speed))
            {
                timedelay = transitionDelayTime;
            }
            else
            {
                timedelay = 0;
            }
            // find the Headers of DataFrames "FMT"
            foreach (var item in logData.GetEnumeratorType(RowType.FMT.ToString()))
            {

                if (item.items[3].Replace(" ", "").Equals(RowType.GPS.ToString()))
                {
                    Columns_Data.Add(RowType.GPS.ToString(), item.items.Skip(5).Take(item.items.Length).ToArray());
                }
                else if (item.items[3].Replace(" ", "").Equals(RowType.CMD.ToString()))
                {
                    Columns_Data.Add(RowType.CMD.ToString(), item.items.Skip(5).Take(item.items.Length).ToArray());
                }
                else if (item.items[3].Replace(" ", "").Equals(RowType.MODE.ToString()))
                {
                    Columns_Data.Add(RowType.MODE.ToString(), item.items.Skip(5).Take(item.items.Length).ToArray());
                }
                else if (item.items[3].Replace(" ", "").Equals(RowType.CURR.ToString()))
                {
                    Columns_Data.Add(RowType.CURR.ToString(), item.items.Skip(5).Take(item.items.Length).ToArray());
                }
            }

            EnergyLogFileModel energyLogfileModel = new EnergyLogFileModel();

            foreach (var modeitem in logData.GetEnumeratorType(RowType.MODE.ToString()))
            {
                string[] x = modeitem.items; //tempArray
                // zipping 2 list to 1 dict
                var dict = Columns_Data[RowType.MODE.ToString()].Zip(x.Skip(1), (s, i) => new { s, i })
                    .ToDictionary(itemt => itemt.s, itemt => itemt.i);
                //dict.Add("timeinms", item.timems.ToString());
                //dict.Add("msgtype", item.msgtype);
                //mode_datas.Add(dict);

                // set Start from first CMD ... the space in the string is necessary
                if (dict["Mode"].Equals(" Auto") && energyLogfileModel.StartTime.Equals(0))
                {
                    energyLogfileModel.StartTime = Convert.ToInt32(modeitem.timems - 1);
                }

                // create current-object and add in list
                energyLogfileModel.MODE_Lines.Add(new MODE_Model(modeitem.timems, dict["Mode"], dict["ModeNum"]));
            }
            foreach (var cmditem in logData.GetEnumeratorType(RowType.CMD.ToString()))
            {
                string[] x = cmditem.items; //tempArray

                // zipping 2 list to 1 dict
                var dict = Columns_Data[RowType.CMD.ToString()].Zip(x.Skip(1), (s, i) => new { s, i }).ToDictionary(itemt => itemt.s, itemt => itemt.i);

                int time = cmditem.timems; // time in ms
                string[] paras = new string[4]; // 4 command-parameters
                paras[0] = dict.ContainsKey(CMDFrame.Prm1.ToString()) ? dict[CMDFrame.Prm1.ToString()].Replace('.', ',') : "0"; // parameter 1
                paras[1] = dict.ContainsKey(CMDFrame.Prm2.ToString()) ? dict[CMDFrame.Prm2.ToString()].Replace('.', ',') : "0"; // parameter 2
                paras[2] = dict.ContainsKey(CMDFrame.Prm3.ToString()) ? dict[CMDFrame.Prm3.ToString()].Replace('.', ',') : "0"; // parameter 3
                paras[3] = dict.ContainsKey(CMDFrame.Prm4.ToString()) ? dict[CMDFrame.Prm4.ToString()].Replace('.', ',') : "0"; // parameter 4
                int cnum;
                try
                {
                    cnum = dict.ContainsKey(CMDFrame.CNum.ToString()) ? Convert.ToInt32(dict[CMDFrame.CNum.ToString()]) : -1337; // command number
                }
                catch
                {
                    cnum = -1337;
                }

                if (!cnum.Equals(-1337) && energyLogfileModel.CMD_Lines.Any(cmdlines => cmdlines.CNum.Equals(Convert.ToInt32(cnum))) ||
                    cmditem.timems <= energyLogfileModel.StartTime && energyLogfileModel.CMD_Lines.Count > 0) // continue if cnum != -1337 && cnum exists or cmdtime is lesser than starttime 
                    continue;

                var cid = dict.ContainsKey(CMDFrame.CId.ToString()) ? dict[CMDFrame.CId.ToString()] : CMDFrame.None.ToString(); // command id
                var lat = dict.ContainsKey(CMDFrame.Lat.ToString()) ? dict[CMDFrame.Lat.ToString()].Replace('.', ',') : CMDFrame.None.ToString(); // command latitude
                var lng = dict.ContainsKey(CMDFrame.Lng.ToString()) ? dict[CMDFrame.Lng.ToString()].Replace('.', ',') : CMDFrame.None.ToString(); // command longitude
                var alt = dict.ContainsKey(CMDFrame.Alt.ToString()) ? dict[CMDFrame.Alt.ToString()].Replace('.', ',') : CMDFrame.None.ToString(); // command altitude

                // create current-object and add in list in energylogfilemodel
                energyLogfileModel.CMD_Lines.Add(new CMD_Model(time, Convert.ToInt32(cnum), cid, paras, lat, lng, alt));
            }
            foreach (var curritem in logData.GetEnumeratorType(RowType.CURR.ToString()))
            {
                string[] x = curritem.items; //tempArray
                // zipping 2 list to 1 dict
                var dict = Columns_Data[RowType.CURR.ToString()].Zip(x.Skip(1), (s, i) => new { s, i })
                    .ToDictionary(itemt => itemt.s, itemt => itemt.i);

                int time = curritem.timems; // time in ms
                var curr = dict.ContainsKey(CURRFrame.Curr.ToString()) ? dict[CURRFrame.Curr.ToString()].Replace('.', ',') : CURRFrame.None.ToString(); // current drawns from battery in amps * 100
                var volt = dict.ContainsKey(CURRFrame.Volt.ToString()) ? dict[CURRFrame.Volt.ToString()].Replace('.', ',') : CURRFrame.None.ToString(); // actual battery voltage in volts * 100
                var currTot = dict.ContainsKey(CURRFrame.CurrTot.ToString()) ? dict[CURRFrame.CurrTot.ToString()].Replace('.', ',') : CURRFrame.None.ToString(); // actual battery voltage in volts * 100

                // create current-object and add in list
                energyLogfileModel.CURR_Lines.Add(new CURR_Model(time, volt, curr, currTot));
            }

            ESearchFlag searchflag = ESearchFlag.Start; //set init flag
            int cmdindex = 0;
            int gpsindex = 0;
            // fill datas from lines to specific headers "GPS"
            foreach (var gpsitem in logData.GetEnumeratorType(RowType.GPS.ToString()))
            {
                string[] x = gpsitem.items; //tempArray
                // zipping 2 list to 1 dict
                var dict = Columns_Data[RowType.GPS.ToString()].Zip(x.Skip(1), (s, i) => new { s, i })
                    .ToDictionary(itemt => itemt.s, itemt => itemt.i);

                int time = gpsitem.timems; // time in ms
                var hdop = dict.ContainsKey(GPSFrame.HDop.ToString()) ? dict[GPSFrame.HDop.ToString()].Replace('.', ',') : GPSFrame.None.ToString(); // gps hdop - gps precision < 1 - 1.5
                var lat = dict.ContainsKey(GPSFrame.Lat.ToString()) ? dict[GPSFrame.Lat.ToString()].Replace('.', ',') : GPSFrame.None.ToString(); // gps latitude
                var lng = dict.ContainsKey(GPSFrame.Lng.ToString()) ? dict[GPSFrame.Lng.ToString()].Replace('.', ',') : GPSFrame.None.ToString(); // gps longitude
                string alt;
                if (dict.ContainsKey(GPSFrame.RelAlt.ToString()))
                    alt = dict[GPSFrame.RelAlt.ToString()].Replace('.', ',');
                else if (dict.ContainsKey(GPSFrame.RAlt.ToString()))
                    alt = dict[GPSFrame.RAlt.ToString()].Replace('.', ',');
                else
                    alt = GPSFrame.None.ToString(); // gps altitude

                switch (searchflag)
                {
                    case ESearchFlag.First:
                        //energyLogfileModel.GPS_Lines.Add(new GPS_Model(time, hdop, lat, lng, alt, false, false));
                        searchflag = ESearchFlag.Start;
                        cmdindex++;
                        break;
                    case ESearchFlag.Start: // loiter time in param1 + cmd_time + timedelay
                        int param1; // for cmd waypoint delay prm1

                        try
                        {
                            // convert only if cmd => Waypoint (16) or Loiter_Time (19)
                            param1 = Convert.ToInt16(energyLogfileModel.CMD_Lines[cmdindex].Param[0]);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(@"Convert-Error in EnergyProfileController --> LogDatas(): " + e);
                            throw;
                        }
                        if (param1 > 0 && Convert.ToByte(energyLogfileModel.CMD_Lines[cmdindex].CmdId).Equals(16)) //convert in milliseconds
                        {
                            param1 = param1 * 1000; // [ms]
                        }
                        if (gpsitem.timems >= param1 + energyLogfileModel.CMD_Lines[cmdindex].Time_ms + timedelay)
                        {
                            energyLogfileModel.CMD_Lines[cmdindex].GPS_START_Index = gpsindex;
                            //energyLogfileModel.GPS_Lines.Add(new GPS_Model(time, hdop, lat, lng, alt, true, false));
                            cmdindex++;
                            searchflag = ESearchFlag.End;
                        }
                        break;
                    case ESearchFlag.End:
                        if (!energyLogfileModel.CMD_Lines.Count.Equals(cmdindex) && gpsitem.timems >= energyLogfileModel.CMD_Lines[cmdindex].Time_ms - timedelay) // all cmd_line except last cmd_line ... in general is this the return_to_launch 
                        {
                            energyLogfileModel.CMD_Lines[cmdindex - 1].GPS_END_Index = gpsindex;
                            //energyLogfileModel.GPS_Lines.Add(new GPS_Model(time, hdop, lat, lng, alt, false, true));
                            searchflag = ESearchFlag.Start;
                        }
                        else if (energyLogfileModel.CMD_Lines.Count.Equals(cmdindex))
                        {
                            //energyLogfileModel.GPS_Lines.Add(new GPS_Model(time, hdop, lat, lng, alt, false, false));
                        }
                        break;
                }
                // create current-object and add in list
                energyLogfileModel.GPS_Lines.Add(new GPS_Model(time, hdop, lat, lng, alt, false, false));
                gpsindex++;

            }
            return energyLogfileModel;
        }

        /// <summary>
        /// Analyze the logfiles from listbox
        /// </summary>
        public bool AnalyzeLogs(List<string> filenames, int gvalues, int transtime)
        {
            // init values
            validMinValues = gvalues;
            transitionDelayTime = transtime;
            validAnalysis = false;
            GC.Collect();
            Loading.ShowLoading("Analyze Logfiles ..."); // show loading window
            // make a threadppol
            using (var finished = new CountdownEvent(1))
            {
                foreach (var filename in filenames)
                {
                    finished.AddCount(); // Indicate that there is another work item.
                    ThreadPool.QueueUserWorkItem(
                        (state) =>
                        {
                            try
                            {
                                LoadLogfile(filename.ToString());
                            }
                            finally
                            {
                                // ReSharper disable once AccessToDisposedClosure
                                finished.Signal(); // Signal that the work item is complete.
                            }
                        }, null);
                }
                finished.Signal(); // Signal that queueing is complete.
                finished.Wait(); // Wait for all work items to complete.
            }

            // calculate values
            CalcCMDValues();
            CalcMeanCurrentAndSpeed();

            // set values in table
            ClearModelSets();
            SelectAngleCurrentValues();
            SelectAngleSpeedValues();
            SelectHoverValue();

            // print values as CSV for better offline statistics
            //PrintCSV(logAnalizerModel.Angle_MeanCurrent, SectionType.Current);
            //PrintCSV(logAnalizerModel.Angle_MeanSpeed, SectionType.Speed);
            //PrintCSV(logAnalizerModel.AngleSection, SectionType.Current);
            //PrintCSV(logAnalizerModel.AngleSection, SectionType.Speed);
            //if (logAnalizerModel.HoverCurrentList.Count > 0)
            //    PrintCSV(logAnalizerModel.HoverCurrentList, SectionType.Hover);

            Loading.Close(); // close loading window after analyze

            // Giv the user a feedback after analyze the logfiles
            Feedback();

            // clean memory
            logAnalizerModel = null;
            GC.Collect();

            // return weather the analyis was successful
            return validAnalysis;
        }

        /// <summary>
        /// Check if all necessary data are in the tables of energyprofilemodel. 
        /// </summary>
        /// <returns>A dictionary with counts of missing values of current, speed aqnd hover. Best Case if all type has count = 0.</returns>
        private Dictionary<SectionType, int> CheckValidValues()
        {
            // check currentset
            Dictionary<SectionType, int> missingvaluesdict = new Dictionary<SectionType, int>();
            int missingvalues = 0;
            foreach (KeyValuePair<int, CurrentModel> keyValuePair in EnergyProfileModel.CurrentSet)
            {
                if (keyValuePair.Value.AverageCurrent.Equals(0) || (keyValuePair.Value.Angle.Equals(0) && !keyValuePair.Key.Equals(6)))
                {
                    missingvalues++;
                }
            }
            missingvaluesdict.Add(SectionType.Current, missingvalues);

            // check velocityset
            missingvalues = 0;
            foreach (KeyValuePair<int, VelocityModel> keyValuePair in EnergyProfileModel.VelocitySet)
            {
                if (keyValuePair.Value.AverageVelocity.Equals(0) || (keyValuePair.Value.Angle.Equals(0) && !keyValuePair.Key.Equals(6)))
                {
                    missingvalues++;
                }
            }
            missingvaluesdict.Add(SectionType.Speed, missingvalues);

            // check hover
            missingvalues = 0;
            if (EnergyProfileModel.CurrentHover.AverageCurrent.Equals(0))
            {
                missingvalues++;
            }
            missingvaluesdict.Add(SectionType.Hover, missingvalues);
            return missingvaluesdict;
        }


        /// <summary>
        /// Create a CustomMessageBox with a feedback-string for user.
        /// In this string stands the result angle --> meancurrent/meanspeed and the samples of each current/speed to specific angle.
        /// </summary>
        private void Feedback()
        {
            // start of the string, if all necessary values would be found or not.
            string feedback;
            Dictionary<SectionType, int> missingdict = CheckValidValues();
            if (!missingdict[SectionType.Current].Equals(0) || !missingdict[SectionType.Speed].Equals(0) ||
                !missingdict[SectionType.Hover].Equals(0))
            {
                feedback = "At least one valid value is missing: (type : missing counts)" + Environment.NewLine;
                feedback += SectionType.Current.ToString() + "   : " + missingdict[SectionType.Current] +
                            Environment.NewLine;
                feedback += SectionType.Speed.ToString() + "    : " + missingdict[SectionType.Speed] +
                            Environment.NewLine;
                feedback += SectionType.Hover.ToString() + "     : " + missingdict[SectionType.Hover] +
                            Environment.NewLine;
            }
            else
            {
                feedback = "Sufficient values for analysis were found." + Environment.NewLine;
                validAnalysis = true;
            }
            // string for angle to current and the samples
            var keysindict = logAnalizerModel.Angle_MeanCurrent_SampleCounts.Keys.ToList();
            keysindict.Sort();
            string current = Environment.NewLine + "This values are found for current: angle [°] --> meanvalue of current [A] (samples)";
            if (keysindict.Count > 0)
            {
                foreach (var key in keysindict)
                {
                    current += Environment.NewLine + key.ToString("00") + "     -->     " +
                               logAnalizerModel.Angle_MeanCurrent[key].ToString("0.00").Replace(".", ",") + " ( " +
                               logAnalizerModel.Angle_MeanCurrent_SampleCounts[key] + " ) ";

                }
            }
            else
            {
                current += Environment.NewLine + "nothing found";
            }
            // string for angle to speed and the samples
            keysindict = logAnalizerModel.Angle_MeanSpeed_SampleCounts.Keys.ToList();
            keysindict.Sort();
            string speed = Environment.NewLine + Environment.NewLine + "This values are found for speed: angle [°] --> meanvalue of speed [m/s] (samples)";
            if (keysindict.Count > 0)
            {
                foreach (var key in keysindict)
                {
                    speed += Environment.NewLine + key.ToString("00") + "     -->     " +
                             logAnalizerModel.Angle_MeanSpeed[key].ToString("0.00").Replace(".", ",") + " ( " +
                             logAnalizerModel.Angle_MeanSpeed_SampleCounts[key] + " ) ";
                }
            }
            else
            {
                speed += Environment.NewLine + "nothing found";
            }
            // string for hover samples
            string hover = Environment.NewLine + Environment.NewLine + "This values are found for hover: meanvalue of current (samples)";
            if (logAnalizerModel.Hover_SampleCounts.Equals(0))
            {
                hover += Environment.NewLine + "nothing found";
            }
            else
            {
                hover += Environment.NewLine + logAnalizerModel.MeanCurrent_Hover + " ( " + logAnalizerModel.Hover_SampleCounts + " ) ";
            }
            // link to one string
            feedback += current;
            feedback += speed;
            feedback += hover;
            CustomMessageBox.Show(feedback);
        }


        /// <summary>
        /// Load each logfile and fill it into LogDictionary 
        /// </summary>
        /// <param name="FileName"></param>
        public void LoadLogfile(string FileName)
        {
            if (logAnalizerModel == null)
            {
                //initialize the LogAnalizerModel
                logAnalizerModel = new LogAnalizerModel();
            }
            try
            {
                Stream stream = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                CollectionBuffer logdata = new CollectionBuffer(stream);
                // fill all datas to specific logfile into a dict 
                logAnalizerModel.AllLogfiles.Add(Path.GetFileName(FileName), LogDatas(logdata));
                logdata.Clear();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Failed to read File: " + ex);
            }
        }

        /// <summary>
        /// This methods calculate some important values from CMD-Values and GPS-Values and save it in CMD-Model
        /// </summary>
        private void CalcCMDValues()
        {
            int currentsTransitionDelayTime = 0;
            int gpsTransitionDelayTime = 0;
            if (statsmodus.Equals(StatisticModus.Curr) ||
                statsmodus.Equals(StatisticModus.Both))
            {
                currentsTransitionDelayTime = transitionDelayTime;
            }
            if (statsmodus.Equals(StatisticModus.GPS_Speed) ||
                statsmodus.Equals(StatisticModus.Both))
            {
                gpsTransitionDelayTime = transitionDelayTime;
            }
            foreach (var logfilemodel in logAnalizerModel.AllLogfiles)
            {

                for (int cmdindex = 0; cmdindex < logfilemodel.Value.CMD_Lines.Count; cmdindex++)
                {
                    // init actual position points

                    if (cmdindex > 0)
                    {
                        // set act position points
                        double lat_actpos = Convert.ToDouble(logfilemodel.Value.CMD_Lines[cmdindex - 1].Latitude);
                        double lng_actpos = Convert.ToDouble(logfilemodel.Value.CMD_Lines[cmdindex - 1].Longitude);
                        var alt_actpos = cmdindex.Equals(1) ? 0 : Convert.ToDouble(logfilemodel.Value.CMD_Lines[cmdindex - 1].Altitude);
                        // set post position points
                        double lat_postpos = Convert.ToDouble(logfilemodel.Value.CMD_Lines[cmdindex].Latitude);
                        double lng_postpos = Convert.ToDouble(logfilemodel.Value.CMD_Lines[cmdindex].Longitude);
                        double alt_postpos = Convert.ToDouble(logfilemodel.Value.CMD_Lines[cmdindex].Altitude);
                        double distance_on_ground_2_postpos =
                            CalcGeoDistance(lat_actpos, lat_postpos, lng_actpos,
                                lng_postpos); // geodistance btw. airline in meters
                        double alt_diff = alt_postpos - alt_actpos;

                        // save the real distance
                        logfilemodel.Value.CMD_Lines[cmdindex].Distance = distance_on_ground_2_postpos.Equals(0)
                            ? Math.Abs(alt_diff)
                            : Math.Round(Math.Sqrt(Math.Pow(distance_on_ground_2_postpos, 2) + Math.Pow(alt_diff, 2)),
                                1);
                        // save the climbing or descent angle
                        logfilemodel.Value.CMD_Lines[cmdindex].Angle =
                            alt_diff.Equals(0)
                                ? 0
                                : CalcAngle(distance_on_ground_2_postpos, alt_diff); // CalcAngle(distance, alt_diff)

                        // set FlightModes in CMD
                        if (logfilemodel.Value.CMD_Lines[cmdindex].Angle < 0 &&
                            logfilemodel.Value.CMD_Lines[cmdindex].Distance.Equals(0))
                        {
                            logfilemodel.Value.CMD_Lines[cmdindex].FlightMode = FlightMode.StraightDOWN.ToString();
                        }
                        else if (logfilemodel.Value.CMD_Lines[cmdindex].Angle < 0 &&
                                 !logfilemodel.Value.CMD_Lines[cmdindex].Distance.Equals(0))
                        {
                            logfilemodel.Value.CMD_Lines[cmdindex].FlightMode = FlightMode.Descent.ToString();
                        }
                        else if (logfilemodel.Value.CMD_Lines[cmdindex].Angle > 0 &&
                                 logfilemodel.Value.CMD_Lines[cmdindex].Distance.Equals(0))
                        {
                            logfilemodel.Value.CMD_Lines[cmdindex].FlightMode = FlightMode.StraightUP.ToString();
                        }
                        else if (logfilemodel.Value.CMD_Lines[cmdindex].Angle > 0 &&
                                 !logfilemodel.Value.CMD_Lines[cmdindex].Distance.Equals(0))
                        {
                            logfilemodel.Value.CMD_Lines[cmdindex].FlightMode = FlightMode.Climb.ToString();
                        }
                        else if (logfilemodel.Value.CMD_Lines[cmdindex].Angle.Equals(0) &&
                                 !logfilemodel.Value.CMD_Lines[cmdindex].Distance.Equals(0))
                        {
                            logfilemodel.Value.CMD_Lines[cmdindex].FlightMode = FlightMode.StraigtForward.ToString();
                        }
                        else if (logfilemodel.Value.CMD_Lines[cmdindex].Angle.Equals(0) &&
                                 !logfilemodel.Value.CMD_Lines[cmdindex].Distance.Equals(0))
                        {
                            logfilemodel.Value.CMD_Lines[cmdindex].FlightMode = FlightMode.Warning.ToString();
                        }
                        if (cmdindex < logfilemodel.Value.CMD_Lines.Count - 1)
                        {
                            //if (!logfilemodel.Value.CMD_Lines[cmdindex].FlightMode.Equals(FlightMode.Hover.ToString()))
                            if (Convert.ToByte(logfilemodel.Value.CMD_Lines[cmdindex].CmdId).Equals(16))
                            {
                                logfilemodel.Value.CMD_Lines[cmdindex].FlightMode = FlightMode.Delay_at_Waypoint.ToString();
                                // hover from delay - param1
                                int delaytime = Convert.ToInt16(logfilemodel.Value.CMD_Lines[cmdindex].Param[0]) *
                                                1000; // in milliseconds
                                if (delaytime > 0) // set only if delaytime is set
                                {
                                    // save all values for calculate energy in hoverstate
                                    logfilemodel.Value.CMD_Lines[cmdindex].currentHoverList = GetCurrentList(
                                        logfilemodel.Value.CURR_Lines, logfilemodel.Value.CMD_Lines[cmdindex].Time_ms,
                                        logfilemodel.Value.CMD_Lines[cmdindex].Time_ms + delaytime);

                                    //// save all values for calculate energy after hoverstate
                                    //logfilemodel.Value.CMD_Lines[cmdindex].currentList = GetCurrentList(
                                    //    logfilemodel.Value.CURR_Lines,
                                    //    logfilemodel.Value.CMD_Lines[cmdindex].Time_ms + delaytime + delaytime + currentsTransitionDelayTime,
                                    //    logfilemodel.Value.CMD_Lines[cmdindex + 1].Time_ms - currentsTransitionDelayTime);

                                }

                                // set all qualified currents in list
                                logfilemodel.Value.CMD_Lines[cmdindex].currentList = GetCurrentList(
                                    logfilemodel.Value.CURR_Lines, logfilemodel.Value.CMD_Lines[cmdindex].Time_ms + delaytime + currentsTransitionDelayTime,
                                    logfilemodel.Value.CMD_Lines[cmdindex + 1].Time_ms - currentsTransitionDelayTime);

                                logfilemodel.Value.CMD_Lines[cmdindex].CurrentCount =
                                    logfilemodel.Value.CMD_Lines[cmdindex].currentList.Count;

                                // calculate gps_speed and save it

                                // first way for calculate speed
                                //logfilemodel.Value.CMD_Lines[cmdindex].Speed = CalcSpeed(
                                //    logfilemodel.Value.GPS_Lines
                                //        [logfilemodel.Value.CMD_Lines[cmdindex].GPS_START_Index],
                                //    logfilemodel.Value.GPS_Lines[
                                //        logfilemodel.Value.CMD_Lines[cmdindex].GPS_END_Index],
                                //    logfilemodel.Value.CMD_Lines[cmdindex].Angle, alt_diff);

                                // second way for calculate speed
                                logfilemodel.Value.CMD_Lines[cmdindex].Speed = CalcGPSSpeed(
                                    logfilemodel.Value.GPS_Lines, logfilemodel.Value.CMD_Lines[cmdindex].Time_ms + gpsTransitionDelayTime,
                                    logfilemodel.Value.CMD_Lines[cmdindex + 1].Time_ms - gpsTransitionDelayTime);

                                // ToDo: set EnergyTime and TotalEnergy
                                logfilemodel.Value.EnergyDatas[cmdindex] = GetEnergyValues(
                                    logfilemodel.Value.CURR_Lines, logfilemodel.Value.CMD_Lines[cmdindex].Time_ms,
                                    logfilemodel.Value.CMD_Lines[cmdindex + 1].Time_ms); // only for one Command

                            }
                            // calculate values in "Loiter_Time" only for hover
                            else if (Convert.ToByte(logfilemodel.Value.CMD_Lines[cmdindex].CmdId).Equals(19)) // only values for Hover in Loiter_Time
                            {
                                logfilemodel.Value.CMD_Lines[cmdindex].FlightMode = FlightMode.Hover.ToString();
                                int delaytime = Convert.ToInt16(logfilemodel.Value.CMD_Lines[cmdindex].Param[0]) *
                                                1000; // in milliseconds
                                if (delaytime > 0)
                                {
                                    logfilemodel.Value.CMD_Lines[cmdindex].currentHoverList = GetCurrentList(
                                        logfilemodel.Value.CURR_Lines,
                                        logfilemodel.Value.CMD_Lines[cmdindex + 1].Time_ms - delaytime,
                                        logfilemodel.Value.CMD_Lines[cmdindex + 1].Time_ms);
                                }
                            }

                            // add hovercurrent
                            if (logfilemodel.Value.CMD_Lines[cmdindex].currentHoverList.Count > 0)
                            {
                                logAnalizerModel.HoverCurrentList.AddRange(logfilemodel.Value.CMD_Lines[cmdindex].currentHoverList);
                            }

                            // add angle/speed
                            if (logAnalizerModel.AngleSection.ContainsKey(logfilemodel.Value.CMD_Lines[cmdindex].Angle))
                            {
                                logAnalizerModel.AngleSection[logfilemodel.Value.CMD_Lines[cmdindex].Angle][SectionType.Speed].Add(logfilemodel.Value.CMD_Lines[cmdindex].Speed);
                                logAnalizerModel.AngleSection[logfilemodel.Value.CMD_Lines[cmdindex].Angle][SectionType.Current].AddRange(logfilemodel.Value.CMD_Lines[cmdindex].currentList);
                            }
                            else
                            {
                                logAnalizerModel.AngleSection.Add(logfilemodel.Value.CMD_Lines[cmdindex].Angle,
                                    new Dictionary<SectionType, List<double>>
                                    {
                                        {
                                            SectionType.Speed,
                                            new List<double> {logfilemodel.Value.CMD_Lines[cmdindex].Speed}

                                        },
                                        {
                                            SectionType.Current,
                                            logfilemodel.Value.CMD_Lines[cmdindex].currentList
                                        }
                                    });
                            }
                        }
                    }
                }
                // For better statistic overview ... in release is that outcomment
                //PrintStatisticEnergyValues(logfilemodel.Value, logfilemodel.Key);
            }
        }

        /// <summary>
        /// Get a list of energy values from beginning of a command until next comand
        /// </summary>
        /// <param name="currList">This is the all over current list.</param>
        /// <param name="timestart">This is the timestamp from beginning command.</param>
        /// <param name="timeend">This is the time from the ending command.</param>
        /// <returns>Returns a list of double values: [0]=energytotaltime [s], [1]=energydiff, [2]=firstenergysample (CurrTot), [3]=lastenergysample (CurrTot) </returns>
        private double[] GetEnergyValues(List<CURR_Model> currList, int timestart, int timeend)
        {
            List<CURR_Model> tempCurrList = new List<CURR_Model>();
            double energytot = 0;
            double energy1 = 0;
            double energy2 = 0;
            var energytime = Convert.ToDouble(timeend - timestart) / 1000; // [s]
            if (energytime > cmdFlightTime)
            {
                foreach (var currModel in currList)
                {
                    if (currModel.Time_ms >= timestart && currModel.Time_ms <= timeend)
                    {
                        tempCurrList.Add(currModel);
                    }
                }
                var energyfirst = tempCurrList.First().TotalCurrennt;
                var energylast = tempCurrList.Last().TotalCurrennt;
                energy1 = Convert.ToDouble(energyfirst);
                energy2 = Convert.ToDouble(energylast);
                energytot = Convert.ToDouble(energylast) - Convert.ToDouble(energyfirst);
            }
            return new[] { energytime, energytot, energy1, energy2 };
        }

        /// <summary>
        /// Calculate the GPS_Speed
        /// </summary>
        /// <param name="gpslist">Full list of all GPS_Samples of one Logfile</param>
        /// <param name="timestart">Start_Time for first GPS_Sample</param>
        /// <param name="timeend">END_Time for last GPS_Sample</param>
        /// <returns>Get the GPS_Speed.</returns>
        private double CalcGPSSpeed(List<GPS_Model> gpslist, int timestart, int timeend)
        {
            double speed = 0;
            bool start = false;
            double startLong = 0;
            double startLat = 0;
            double startAlt = 0;
            double endLong = 0;
            double endLat = 0;
            double endAlt = 0;
            double gpsstarttime = 0;
            double gpsendtime = 0;
            double distance = 0;
            bool convertValid = true;
            foreach (var gpsModel in gpslist)
            {
                if (gpsModel.Time_ms >= timestart && !start)
                {
                    try
                    {
                        if (!gpsModel.Longitude.Equals(GPSFrame.None.ToString()))
                        {
                            startLong = Convert.ToDouble(gpsModel.Longitude);
                        }
                        else
                        {
                            convertValid = false;
                            break;
                        }
                        if (!gpsModel.Latitude.Equals(GPSFrame.None.ToString()))
                        {
                            startLat = Convert.ToDouble(gpsModel.Latitude);
                        }
                        else
                        {
                            convertValid = false;
                            break;
                        }
                        if (!gpsModel.Altitude.Equals(GPSFrame.None.ToString()))
                        {
                            startAlt = Convert.ToDouble(gpsModel.Altitude);
                        }
                        else
                        {
                            convertValid = false;
                            break;
                        }
                        gpsstarttime = Convert.ToDouble(gpsModel.Time_ms);

                        start = true;
                    }
                    catch (Exception e)
                    {
                        convertValid = false;
                        CustomMessageBox.Show(e.ToString());
                        break;
                    }
                }
                else if (gpsModel.Time_ms >= timeend && start)
                {
                    try
                    {
                        if (!gpsModel.Longitude.Equals(GPSFrame.None.ToString()))
                        {
                            endLong = Convert.ToDouble(gpsModel.Longitude);
                        }
                        else
                        {
                            convertValid = false;
                            break;
                        }
                        if (!gpsModel.Latitude.Equals(GPSFrame.None.ToString()))
                        {
                            endLat = Convert.ToDouble(gpsModel.Latitude);
                        }
                        else
                        {
                            convertValid = false;
                            break;
                        }
                        if (!gpsModel.Altitude.Equals(GPSFrame.None.ToString()))
                        {
                            endAlt = Convert.ToDouble(gpsModel.Altitude);
                        }
                        else
                        {
                            convertValid = false;
                            break;
                        }
                        gpsendtime = Convert.ToDouble((gpsModel.Time_ms));
                        break;
                    }
                    catch (Exception e)
                    {
                        convertValid = false;
                        CustomMessageBox.Show(e.ToString());
                        break;
                    }
                }
            }
            if (convertValid)
            {
                var time = (gpsendtime - gpsstarttime) / 1000;
                var alt_diff = Math.Abs(endAlt - startAlt);
                if (time < 0 || time < cmdFlightTime)
                {
                    return speed;
                }
                double distance_on_ground = CalcGeoDistance(startLat, endLat, startLong, endLong);

                if (alt_diff.Equals(0) && distance_on_ground > 0)
                {
                    distance = distance_on_ground;
                }
                else if (alt_diff > 0 && distance_on_ground.Equals(0))
                {
                    distance = alt_diff;
                }
                else if (alt_diff > 0 && distance_on_ground > 0)
                {

                    distance = Math.Sqrt(Math.Pow(distance_on_ground, 2) + Math.Pow(alt_diff, 2));
                }

                speed = distance / time;
            }
            return speed;
        }

        /// <summary>
        /// Calculation the distance on ground btw. airline. [meters]
        /// https://www.kompf.de/gps/distcalc.html --> better method
        /// </summary>
        /// <param name="lat1">Latitude from Point 1</param>
        /// <param name="lat2">Latitude from Point 2</param>
        /// <param name="lng1">Longitude from Point 1</param>
        /// <param name="lng2">Longitude from Point 2</param>
        /// <param name="kilometers">set true if you want the distance in kilometers, default: meters</param>
        /// <returns>airline as straight distance on ground</returns>
        private double CalcGeoDistance(double lat1, double lat2, double lng1, double lng2, bool kilometers = false)
        {
            //// distance Calculation
            double lat = ((lat1 + lat2) / 2) * (Math.PI / 180); //  1° = π/180 rad ≈ 0.01745 calc to rad
            double dx = 111.3 * Math.Cos(lat) * (lng1 - lng2); // "111.3 * cos(lat)" is the distance between 2 longitudes and is variable
            double dy = 111.3 * (lat1 - lat2); // distance between 2 latitudes is constant = 111.3 km
            int factor = 1000;
            if (kilometers)
                factor = 1;
            double airline =
                Math.Round((Math.Sqrt(dx * dx + dy * dy)) * factor, 1); // distance in meters or kilometers, rounded by 1
            return airline;
        }

        /// <summary>
        /// Calculate the climb or descent angle from flightpath.
        /// </summary>
        /// <param name="distance">The airline/distance on ground.</param>
        /// <param name="alt">The difference from altitude in pos1 to pos2.</param>
        /// <returns>The climb or descent angle</returns>
        private double CalcAngle(double distance, double alt)
        {
            double angle = 0;
            // angle calculation
            if (alt < 0 && distance.Equals(0))
                angle = -90;
            else if (alt > 0 && distance.Equals(0))
                angle = +90;
            else if (distance > 0)
            {
                double radians = Math.Atan(alt / distance);
                angle = radians * (180 / Math.PI); // calc to degree
            }
            return Math.Round(angle, 0);
        }

        /// <summary>
        /// Calculate the climb, descent or straight-forward speed.
        /// </summary>
        /// <param name="startmodel">The first marked GPS-Model.</param>
        /// <param name="endmodel">The last marked GPS-Model</param>
        /// <param name="angle">The climb or descent angle.</param>
        /// <param name="altitude">The difference of altitude from pos1 to pos2.</param>
        /// <returns>The result speed.</returns>
        private double CalcSpeed2(GPS_Model startmodel, GPS_Model endmodel, double angle, double altitude)
        {
            double distance;
            double speed;
            // if distance != 0 and angle not 90 or -90 degree
            if (!Math.Abs(angle).Equals(90))
            {
                double distance_on_ground = CalcGeoDistance(Convert.ToDouble(startmodel.Latitude),
                    Convert.ToDouble(endmodel.Latitude),
                    Convert.ToDouble(startmodel.Longitude), Convert.ToDouble(endmodel.Longitude));
                distance = angle.Equals(0) ? distance_on_ground : Math.Sqrt(Math.Pow(distance_on_ground, 2) + Math.Pow(altitude, 2));
            }
            else
            {
                distance = Math.Abs(altitude);
            }
            var time = (endmodel.Time_ms - startmodel.Time_ms) / 1000; // from ms to s
            if (statsmodus.Equals(StatisticModus.GPS_Speed) || statsmodus.Equals(StatisticModus.Both))
            {
                if (time > transitionDelayTime / 1000 && !Math.Round(distance).Equals(0)) //set speed only if time larger than transitiontime --> discuss
                {
                    speed = Math.Round(distance / time, 1);
                }
                else
                    speed = 0;
            }
            else
            {
                if (!time.Equals(0) && !Math.Round(distance).Equals(0))
                {
                    speed = Math.Round(distance / time, 1);
                }
                else
                {
                    speed = 0;
                }
            }
            return speed;
        }

        /// <summary>
        /// Calculate the mean-current.
        /// </summary>
        /// <param name="Curr_List">All values for calculate the mean.</param>
        /// <param name="starttime">Starttime for begin the interval.</param>
        /// <param name="endtime">Endtime for end of interval.</param>
        /// <param name="valid_min_values">Minimum of samples for validation.</param>
        /// <returns>The mean-current value</returns>
        private double CalcMeanCurrent(List<CURR_Model> Curr_List, int starttime, int endtime, int valid_min_values)
        {
            double mean_current = 0;
            int valueCount = 0;
            foreach (var current in Curr_List)
            {
                if (current.Time_ms >= starttime && current.Time_ms <= endtime)
                {
                    mean_current += Convert.ToDouble(current.Current);
                    valueCount++;
                }
            }
            if (valueCount >= valid_min_values)
            {
                mean_current = mean_current / valueCount;
            }
            else
            {
                mean_current = 0;
            }

            return mean_current; // in Logs 
        }

        /// <summary>
        /// Returns a specific list of current values.
        /// </summary>
        /// <param name="allCurrentLines">List of all CURR-Models.</param>
        /// <param name="starttime">Set start of interval.</param>
        /// <param name="endtime">Set end of interval.</param>
        /// <returns>A result list of all values in time-interval.</returns>
        private List<double> GetCurrentList(List<CURR_Model> allCurrentLines, int starttime, int endtime)
        {
            List<double> actCurrList = new List<double>();
            int time = endtime - starttime;
            if (time > cmdFlightTime)
            {
                foreach (var line in allCurrentLines)
                {

                    if (line.Time_ms >= starttime && line.Time_ms <= endtime)
                    {
                        actCurrList.Add(Convert.ToDouble(line.Current));
                    }
                }
            }
            return actCurrList;
        }

        /// <summary>
        /// Calculate at the End of parsing logfiles the meancurrent und meanspeed of all founding angles
        /// Important: Here is a choice of valid values. 
        /// </summary>
        private void CalcMeanCurrentAndSpeed()
        {
            // calc mean_current of each angle
            foreach (var anglepair in logAnalizerModel.AngleSection)
            {
                foreach (var sectiontype in anglepair.Value)
                {
                    double sectionValue = 0;
                    double mean = 0;
                    int count = 0;
                    switch (sectiontype.Key)
                    {
                        case SectionType.Speed: // only section speed
                            foreach (var speedvalue in sectiontype.Value)
                            {
                                if (speedvalue > 0)
                                {
                                    count++;
                                    sectionValue += speedvalue;
                                }
                            }
                            if (count > 0)
                            {
                                mean = Math.Round(sectionValue / count, 2);
                                logAnalizerModel.Angle_MeanSpeed_SampleCounts.Add(anglepair.Key, count);
                            }
                            logAnalizerModel.Angle_MeanSpeed.Add(anglepair.Key, mean);
                            break;
                        case SectionType.Current: // only current section
                            if (sectiontype.Value.Count > validMinValues)
                            {
                                foreach (var currentvalue in sectiontype.Value)
                                {
                                    if (currentvalue > 0)
                                    {
                                        count++;
                                        sectionValue += currentvalue;
                                    }
                                }
                                if (count > 0)
                                {
                                    mean = Math.Round(sectionValue / (count * ampfactor), 2);
                                    logAnalizerModel.Angle_MeanCurrent_SampleCounts.Add(anglepair.Key, count);
                                }
                                logAnalizerModel.Angle_MeanCurrent.Add(anglepair.Key, mean);
                            }
                            break;
                    }
                }
            }

            // calculate mean current for Hover
            if (logAnalizerModel.HoverCurrentList.Count > 0)
            {
                double value = 0;
                logAnalizerModel.Hover_SampleCounts = logAnalizerModel.HoverCurrentList.Count;
                foreach (var current in logAnalizerModel.HoverCurrentList)
                {
                    value += current;
                }

                logAnalizerModel.MeanCurrent_Hover = Math.Round(value / (logAnalizerModel.HoverCurrentList.Count * ampfactor), 2);
            }
        }

        /// <summary>
        /// Fill the static dict "CurrentSet" in "EnergyProfileModel" with optimated values.
        /// </summary>
        private void SelectAngleCurrentValues()
        {
            foreach (KeyValuePair<double, double> anglepair in logAnalizerModel.Angle_MeanCurrent)
            {
                if (!anglepair.Value.Equals(0))
                {
                    // only for values at -90 degrees --> straight down
                    if (anglepair.Key.Equals(-90))
                    {
                        if (!EnergyProfileModel.CurrentSet[1].Equals(null))
                        {
                            EnergyProfileModel.CurrentSet[1].AverageCurrent = anglepair.Value;
                        }
                        else
                        {
                            EnergyProfileModel.CurrentSet[1] = new CurrentModel(-90, anglepair.Value);
                        }
                    }
                    // only values between -90 and -63 degrees and set best value of near at -72 degree
                    else if (anglepair.Key > -90 && anglepair.Key <= -63)
                    {
                        if (!EnergyProfileModel.CurrentSet[2].Equals(null))
                        {
                            if (Math.Abs(Math.Abs(EnergyProfileModel.CurrentSet[2].Angle) - 72) >
                                Math.Abs(Math.Abs(anglepair.Key) - 72))
                            {
                                EnergyProfileModel.CurrentSet[2].Angle = anglepair.Key;
                                EnergyProfileModel.CurrentSet[2].AverageCurrent = anglepair.Value;
                            }
                        }
                        else
                        {
                            EnergyProfileModel.CurrentSet[2] = new CurrentModel(anglepair.Key, anglepair.Value);
                        }
                    }
                    // only values between -62 and -45 degrees and set best value of near at -54 degree
                    else if (anglepair.Key > -63 && anglepair.Key <= -45)
                    {
                        if (!EnergyProfileModel.CurrentSet[3].Equals(null))
                        {
                            if (Math.Abs(Math.Abs(EnergyProfileModel.CurrentSet[3].Angle) - 54) >
                                Math.Abs(Math.Abs(anglepair.Key) - 54))
                            {
                                EnergyProfileModel.CurrentSet[3].Angle = anglepair.Key;
                                EnergyProfileModel.CurrentSet[3].AverageCurrent = anglepair.Value;
                            }
                        }
                        else
                        {
                            EnergyProfileModel.CurrentSet[3] = new CurrentModel(anglepair.Key, anglepair.Value);
                        }
                    }
                    // only values between -44 and -27 degrees and set best value of near at -36 degree
                    else if (anglepair.Key > -45 && anglepair.Key <= -27)
                    {
                        if (!EnergyProfileModel.CurrentSet[4].Equals(null))
                        {
                            if (Math.Abs(Math.Abs(EnergyProfileModel.CurrentSet[4].Angle) - 36) >
                                Math.Abs(Math.Abs(anglepair.Key) - 36))
                            {
                                EnergyProfileModel.CurrentSet[4].Angle = anglepair.Key;
                                EnergyProfileModel.CurrentSet[4].AverageCurrent = anglepair.Value;
                            }
                        }
                        else
                        {
                            EnergyProfileModel.CurrentSet[4] = new CurrentModel(anglepair.Key, anglepair.Value);
                        }

                    }
                    // only values between -26 and 0 degrees and set best value of near at -18 degree
                    else if (anglepair.Key > -27 && anglepair.Key < 0)
                    {
                        if (!EnergyProfileModel.CurrentSet[5].Equals(null))
                        {
                            if (Math.Abs(Math.Abs(EnergyProfileModel.CurrentSet[5].Angle) - 18) >
                                Math.Abs(Math.Abs(anglepair.Key) - 18))
                            {
                                EnergyProfileModel.CurrentSet[5].Angle = anglepair.Key;
                                EnergyProfileModel.CurrentSet[5].AverageCurrent = anglepair.Value;
                            }
                        }
                        else
                        {
                            EnergyProfileModel.CurrentSet[5] = new CurrentModel(anglepair.Key, anglepair.Value);
                        }
                    }
                    // set value at 0 degree --> straight forward
                    else if (anglepair.Key.Equals(0))
                    {
                        if (!EnergyProfileModel.CurrentSet[6].Equals(null))
                        {
                            EnergyProfileModel.CurrentSet[6].AverageCurrent = anglepair.Value;

                        }
                        else
                        {
                            EnergyProfileModel.CurrentSet[6] = new CurrentModel(0, anglepair.Value);
                        }
                    }
                    // only values between 1 and -27 degrees and set best value of near at 18 degree
                    else if (anglepair.Key > 0 && anglepair.Key < 27)
                    {
                        if (!EnergyProfileModel.CurrentSet[7].Equals(null))
                        {
                            if (Math.Abs(EnergyProfileModel.CurrentSet[7].Angle - 18) > Math.Abs(anglepair.Key - 18))
                            {
                                EnergyProfileModel.CurrentSet[7].Angle = anglepair.Key;
                                EnergyProfileModel.CurrentSet[7].AverageCurrent = anglepair.Value;
                            }
                        }
                        else
                        {
                            EnergyProfileModel.CurrentSet[7] = new CurrentModel(anglepair.Key, anglepair.Value);
                        }
                    }
                    // only values between 28 and 45 degrees and set best value of near at 36 degree
                    else if (anglepair.Key >= 27 && anglepair.Key < 45)
                    {
                        if (!EnergyProfileModel.CurrentSet[8].Equals(null))
                        {
                            if (Math.Abs(EnergyProfileModel.CurrentSet[8].Angle - 36) > Math.Abs(anglepair.Key - 36))
                            {
                                EnergyProfileModel.CurrentSet[8].Angle = anglepair.Key;
                                EnergyProfileModel.CurrentSet[8].AverageCurrent = anglepair.Value;
                            }
                        }
                        else
                        {
                            EnergyProfileModel.CurrentSet[8] = new CurrentModel(anglepair.Key, anglepair.Value);
                        }
                    }
                    // only values between 46 and 63 degrees and set best value of near at 54 degree
                    else if (anglepair.Key >= 45 && anglepair.Key < 63)
                    {
                        if (!EnergyProfileModel.CurrentSet[9].Equals(null))
                        {
                            if (Math.Abs(EnergyProfileModel.CurrentSet[9].Angle - 54) > Math.Abs(anglepair.Key - 54))
                            {
                                EnergyProfileModel.CurrentSet[9].Angle = anglepair.Key;
                                EnergyProfileModel.CurrentSet[9].AverageCurrent = anglepair.Value;
                            }
                        }
                        else
                        {
                            EnergyProfileModel.CurrentSet[9] = new CurrentModel(anglepair.Key, anglepair.Value);
                        }
                    }
                    // only values between 64 and 90 degrees and set best value of near at 72 degree
                    else if (anglepair.Key >= 63 && anglepair.Key < 90)
                    {
                        if (!EnergyProfileModel.CurrentSet[10].Equals(null))
                        {
                            if (Math.Abs(EnergyProfileModel.CurrentSet[10].Angle - 72) > Math.Abs(anglepair.Key - 72))
                            {
                                EnergyProfileModel.CurrentSet[10].Angle = anglepair.Key;
                                EnergyProfileModel.CurrentSet[10].AverageCurrent = anglepair.Value;
                            }
                        }
                        else
                        {
                            EnergyProfileModel.CurrentSet[10] = new CurrentModel(anglepair.Key, anglepair.Value);
                        }
                    }
                    // only values at 90 degree --> straight up
                    else if (anglepair.Key.Equals(90))
                    {
                        if (!EnergyProfileModel.CurrentSet[11].Equals(null))
                        {
                            EnergyProfileModel.CurrentSet[11].AverageCurrent = anglepair.Value;
                        }
                        else
                        {
                            EnergyProfileModel.CurrentSet[11] = new CurrentModel(90, anglepair.Value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Clear the existing models.
        /// </summary>
        private void ClearModelSets()
        {
            EnergyProfileModel.VelocitySet.Clear();
            EnergyProfileModel.CurrentSet.Clear();
            EnergyProfileModel.CurrentHover = new CurrentModel(0f, 0f);

            EnergyProfileModel.VelocitySet.Add(1, new VelocityModel(-90f, 0f));
            EnergyProfileModel.VelocitySet.Add(2, new VelocityModel(0f, 0f));
            EnergyProfileModel.VelocitySet.Add(3, new VelocityModel(0f, 0f));
            EnergyProfileModel.VelocitySet.Add(4, new VelocityModel(0f, 0f));
            EnergyProfileModel.VelocitySet.Add(5, new VelocityModel(0f, 0f));
            EnergyProfileModel.VelocitySet.Add(6, new VelocityModel(0f, 0f));
            EnergyProfileModel.VelocitySet.Add(7, new VelocityModel(0f, 0f));
            EnergyProfileModel.VelocitySet.Add(8, new VelocityModel(0f, 0f));
            EnergyProfileModel.VelocitySet.Add(9, new VelocityModel(0f, 0f));
            EnergyProfileModel.VelocitySet.Add(10, new VelocityModel(0f, 0f));
            EnergyProfileModel.VelocitySet.Add(11, new VelocityModel(90f, 0f));

            EnergyProfileModel.CurrentSet.Add(1, new CurrentModel(-90.0f, 0.0f));
            EnergyProfileModel.CurrentSet.Add(2, new CurrentModel(0.0f, 0.0f));
            EnergyProfileModel.CurrentSet.Add(3, new CurrentModel(0.0f, 0.0f));
            EnergyProfileModel.CurrentSet.Add(4, new CurrentModel(0.0f, 0.0f));
            EnergyProfileModel.CurrentSet.Add(5, new CurrentModel(0.0f, 0.0f));
            EnergyProfileModel.CurrentSet.Add(6, new CurrentModel(0.0f, 0.0f));
            EnergyProfileModel.CurrentSet.Add(7, new CurrentModel(0.0f, 0.0f));
            EnergyProfileModel.CurrentSet.Add(8, new CurrentModel(0.0f, 0.0f));
            EnergyProfileModel.CurrentSet.Add(9, new CurrentModel(0.0f, 0.0f));
            EnergyProfileModel.CurrentSet.Add(10, new CurrentModel(0.0f, 0.0f));
            EnergyProfileModel.CurrentSet.Add(11, new CurrentModel(90.0f, 0.0f));
        }

        /// <summary>
        /// Set the static value in EnergyProfileModel for hover.
        /// </summary>
        private void SelectHoverValue()
        {
            EnergyProfileModel.CurrentHover.AverageCurrent = logAnalizerModel.MeanCurrent_Hover;
        }

        /// <summary>
        /// Set best values for speed to specific angle into "VelocitySet" in "EnergyProfileModel".
        /// </summary>
        private void SelectAngleSpeedValues()
        {
            foreach (KeyValuePair<double, double> anglepair in logAnalizerModel.Angle_MeanSpeed)
            {
                if (!anglepair.Value.Equals(0))
                {
                    // only values at -90 degree --> straight down
                    if (anglepair.Key.Equals(-90))
                    {
                        if (!EnergyProfileModel.VelocitySet[1].Equals(null))
                        {
                            EnergyProfileModel.VelocitySet[1].AverageVelocity = anglepair.Value;
                        }
                        else
                        {
                            EnergyProfileModel.VelocitySet[1] = new VelocityModel(-90, anglepair.Value);
                        }
                    }
                    // only values between -90 and -63 degrees and set best value of near at -72 degree
                    else if (anglepair.Key > -90 && anglepair.Key <= -63)
                    {
                        if (!EnergyProfileModel.VelocitySet[2].Equals(null))
                        {
                            if (Math.Abs(Math.Abs(EnergyProfileModel.VelocitySet[2].Angle) - 72) >
                                Math.Abs(Math.Abs(anglepair.Key) - 72))
                            {
                                EnergyProfileModel.VelocitySet[2].Angle = anglepair.Key;
                                EnergyProfileModel.VelocitySet[2].AverageVelocity = anglepair.Value;
                            }
                        }
                        else
                        {
                            EnergyProfileModel.VelocitySet[2] = new VelocityModel(anglepair.Key, anglepair.Value);
                        }
                    }
                    // only values between -62 and -45 degrees and set best value of near at -54 degree
                    else if (anglepair.Key > -63 && anglepair.Key <= -45)
                    {
                        if (!EnergyProfileModel.VelocitySet[3].Equals(null))
                        {
                            if (Math.Abs(Math.Abs(EnergyProfileModel.VelocitySet[3].Angle) - 54) >
                                Math.Abs(Math.Abs(anglepair.Key) - 54))
                            {
                                EnergyProfileModel.VelocitySet[3].Angle = anglepair.Key;
                                EnergyProfileModel.VelocitySet[3].AverageVelocity = anglepair.Value;
                            }
                        }
                        else
                        {
                            EnergyProfileModel.VelocitySet[3] = new VelocityModel(anglepair.Key, anglepair.Value);
                        }
                    }
                    // only values between -44 and -27 degrees and set best value of near at -36 degree
                    else if (anglepair.Key > -45 && anglepair.Key <= -27)
                    {
                        if (!EnergyProfileModel.VelocitySet[4].Equals(null))
                        {
                            if (Math.Abs(Math.Abs(EnergyProfileModel.VelocitySet[4].Angle) - 36) >
                                Math.Abs(Math.Abs(anglepair.Key) - 36))
                            {
                                EnergyProfileModel.VelocitySet[4].Angle = anglepair.Key;
                                EnergyProfileModel.VelocitySet[4].AverageVelocity = anglepair.Value;
                            }
                        }
                        else
                        {
                            EnergyProfileModel.VelocitySet[4] = new VelocityModel(anglepair.Key, anglepair.Value);
                        }

                    }
                    // only values between -26 and 0 degrees and set best value of near at -18 degree
                    else if (anglepair.Key > -27 && anglepair.Key < 0)
                    {
                        if (!EnergyProfileModel.VelocitySet[5].Equals(null))
                        {
                            if (Math.Abs(Math.Abs(EnergyProfileModel.VelocitySet[5].Angle) - 18) >
                                Math.Abs(Math.Abs(anglepair.Key) - 18))
                            {
                                EnergyProfileModel.VelocitySet[5].Angle = anglepair.Key;
                                EnergyProfileModel.VelocitySet[5].AverageVelocity = anglepair.Value;
                            }
                        }
                        else
                        {
                            EnergyProfileModel.VelocitySet[5] = new VelocityModel(anglepair.Key, anglepair.Value);
                        }
                    }
                    // set value at 0 degree --> straight forward
                    else if (anglepair.Key.Equals(0))
                    {
                        if (!EnergyProfileModel.VelocitySet[6].Equals(null))
                        {
                            EnergyProfileModel.VelocitySet[6].AverageVelocity = anglepair.Value;

                        }
                        else
                        {
                            EnergyProfileModel.VelocitySet[6] = new VelocityModel(0, anglepair.Value);
                        }
                    }
                    // only values between 1 and -27 degrees and set best value of near at 18 degree
                    else if (anglepair.Key > 0 && anglepair.Key < 27)
                    {
                        if (!EnergyProfileModel.VelocitySet[7].Equals(null))
                        {
                            if (Math.Abs(EnergyProfileModel.VelocitySet[7].Angle - 18) > Math.Abs(anglepair.Key - 18))
                            {
                                EnergyProfileModel.VelocitySet[7].Angle = anglepair.Key;
                                EnergyProfileModel.VelocitySet[7].AverageVelocity = anglepair.Value;
                            }
                        }
                        else
                        {
                            EnergyProfileModel.VelocitySet[7] = new VelocityModel(anglepair.Key, anglepair.Value);
                        }
                    }
                    // only values between 28 and 45 degrees and set best value of near at 36 degree
                    else if (anglepair.Key >= 27 && anglepair.Key < 45)
                    {
                        if (!EnergyProfileModel.VelocitySet[8].Equals(null))
                        {
                            if (Math.Abs(EnergyProfileModel.VelocitySet[8].Angle - 36) > Math.Abs(anglepair.Key - 36))
                            {
                                EnergyProfileModel.VelocitySet[8].Angle = anglepair.Key;
                                EnergyProfileModel.VelocitySet[8].AverageVelocity = anglepair.Value;
                            }
                        }
                        else
                        {
                            EnergyProfileModel.VelocitySet[8] = new VelocityModel(anglepair.Key, anglepair.Value);
                        }
                    }
                    // only values between 46 and 63 degrees and set best value of near at 54 degree
                    else if (anglepair.Key >= 45 && anglepair.Key < 63)
                    {
                        if (!EnergyProfileModel.VelocitySet[9].Equals(null))
                        {
                            if (Math.Abs(EnergyProfileModel.VelocitySet[9].Angle - 54) > Math.Abs(anglepair.Key - 54))
                            {
                                EnergyProfileModel.VelocitySet[9].Angle = anglepair.Key;
                                EnergyProfileModel.VelocitySet[9].AverageVelocity = anglepair.Value;
                            }
                        }
                        else
                        {
                            EnergyProfileModel.VelocitySet[9] = new VelocityModel(anglepair.Key, anglepair.Value);
                        }
                    }
                    // only values between 64 and 90 degrees and set best value of near at 72 degree
                    else if (anglepair.Key >= 63 && anglepair.Key < 90)
                    {
                        if (!EnergyProfileModel.VelocitySet[10].Equals(null))
                        {
                            if (Math.Abs(EnergyProfileModel.VelocitySet[10].Angle - 72) > Math.Abs(anglepair.Key - 72))
                            {
                                EnergyProfileModel.VelocitySet[10].Angle = anglepair.Key;
                                EnergyProfileModel.VelocitySet[10].AverageVelocity = anglepair.Value;
                            }
                        }
                        else
                        {
                            EnergyProfileModel.VelocitySet[10] = new VelocityModel(anglepair.Key, anglepair.Value);
                        }
                    }
                    // only values at 90 degree --> straight up
                    else if (anglepair.Key.Equals(90))
                    {
                        if (!EnergyProfileModel.VelocitySet[11].Equals(null))
                        {
                            EnergyProfileModel.VelocitySet[11].AverageVelocity = anglepair.Value;
                        }
                        else
                        {
                            EnergyProfileModel.VelocitySet[11] = new VelocityModel(90, anglepair.Value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This is for better statistic analisis.
        /// Write the values of a specific model in a csv-file and save it with a timestamp.
        /// </summary>
        /// <param name="model">Specific model of Current or Speed</param>
        /// <param name="sectiontype">Current, Speed or Hover</param>
        private void PrintCSV(object model, SectionType sectiontype)
        {
            string path = Settings.Instance.LogDir + @"\CSV\";
            DateTime date = DateTime.Now;
            string filename = date.ToString("yyyy_MM_dd",CultureInfo.CurrentCulture).Replace(":", "_");

            // check if dirfectory exist
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (model.GetType() == typeof(Dictionary<double, double>))
            {
                using (StreamWriter file =
                new StreamWriter(path + "Angle_Mean" + sectiontype + "_" + filename + ".csv"))
                {

                    foreach (var line in (Dictionary<double, double>)model)
                    {
                        file.WriteLine(line.Key + "; " + line.Value);
                    }
                }

            }
            else if (model.GetType() == typeof(Dictionary<double, Dictionary<SectionType, List<double>>>))
            {

                using (StreamWriter file =
                    new StreamWriter(path + "Angle_" + sectiontype + "_" + filename + ".csv"))
                {
                    file.WriteLine("angle; " + sectiontype);
                    foreach (var anglepair in (Dictionary<double, Dictionary<SectionType, List<double>>>)model)
                    {
                        string line = "";
                        foreach (var sectionpair in anglepair.Value)
                        {
                            if (sectionpair.Key.Equals(sectiontype))
                            {
                                foreach (var section in sectionpair.Value)
                                {
                                    line += section + "; ";

                                }
                            }
                        }
                        file.WriteLine(anglepair.Key + "; " + line);
                    }
                }

            }
            else if (model.GetType() == typeof(List<double>))
            {
                using (StreamWriter file =
                    new StreamWriter(path + "_" + sectiontype + "_Current_" + filename + ".csv"))
                {

                    foreach (var line in (List<double>)model)
                    {
                        file.WriteLine(line);
                    }
                }
            }
        }

        /// <summary>
        /// For statistic and value validation. Print the values of selected model in a seperate textfile.
        /// </summary>
        /// <param name="logmodel">specific logfilemodel</param>
        /// <param name="logname">String of the logfilename</param>
        private void PrintStatisticEnergyValues(EnergyLogFileModel logmodel, string logname)
        {
            if (logmodel != null)
            {
                string path = Settings.Instance.LogDir + @"\CSV\";
                DateTime date = DateTime.Now;
                string filename = "EnergyStatistic_" + logname;
                using (StreamWriter file =
                    new StreamWriter(path + filename + ".txt"))
                {
                    foreach (var line in logmodel.EnergyDatas)
                    {
                        file.WriteLine("CMD --> " + line.Key + " || Time --> " + line.Value[0] + " || Energy --> " + line.Value[1] + " || E1 --> " + line.Value[2] + " || E2 --> " + line.Value[3]);
                    }
                }
            }
        }

        /// <summary>
        /// Set the specific trigger of minimum flighttime and transition time
        /// </summary>
        /// <param name="currenttransstate">Flag for current vakues</param>
        /// <param name="speedtransstate">Flag for speed values</param>
        /// <param name="cmdflighttime">value for minimum flighttime</param>
        public void SetTransitionState(bool currenttransstate, bool speedtransstate, int cmdflighttime)
        {
            if (cmdflighttime > 0)
            {
                cmdFlightTime = cmdflighttime;
            }
            if (currenttransstate && speedtransstate)
            {
                statsmodus = StatisticModus.Both;
            }
            else if (!currenttransstate && speedtransstate)
            {
                statsmodus = StatisticModus.GPS_Speed;
            }
            else if (!speedtransstate && currenttransstate)
            {
                statsmodus = StatisticModus.Curr;
            }
            else
            {
                statsmodus = StatisticModus.None;
            }
        }
    }
}
