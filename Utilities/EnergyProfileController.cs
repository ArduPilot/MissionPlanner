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

        private int guiltyValues;
        private int transitionTime;
        private int ampfactor = 100;
        LogAnalizerModel logAnalizerModel;

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
            STRT,
            GPS,
            MODE,
            CURR,
            CMD,
            None
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
            Alt,
            None
        }

        /// <summary>
        /// Enum for CURR-Frame
        /// </summary>
        enum CURRFrame
        {
            Curr,
            Volt,
            None
        }

        /// <summary>
        /// set the flightmode interpretation
        /// </summary>
        enum FlightMode
        {
            Hover,
            StraigtForward,
            StraightUP,
            StraightDOWN,
            Climb,
            Descent,
            Warning,
            None
        }

        enum ESearchFlag
        {
            First,
            Start,
            End,
            None,
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
        /// </summary>
        public void SplineInterpolation()
        {
            //TODO: make a spline interpolation
        }

        /// <summary>
        /// This method interpolate points between two nodes. It fills a list with new interpolated spline-points.
        /// </summary>
        public void LinearInterpolation(PlotProfile profile)
        {
            switch (profile)
            {
                case PlotProfile.Current:
                    EnergyProfileModel.AverageCurrentSplinePoints.Clear();
                    EnergyProfileModel.MaxCurrentSplinePoints.Clear();
                    EnergyProfileModel.MinCurrentSplinePoints.Clear();
                    for (int points = 1; points < EnergyProfileModel.CurrentSet.Count; points++)
                    {
                        // set start and end parameters for each segment
                        double x1 = EnergyProfileModel.CurrentSet[points].Angle;
                        double x2 = EnergyProfileModel.CurrentSet[points + 1].Angle;
                        double y1 = EnergyProfileModel.CurrentSet[points].AverageCurrent;
                        double y2 = EnergyProfileModel.CurrentSet[points + 1].AverageCurrent;
                        double y1_max = EnergyProfileModel.CurrentSet[points].MaxCurrent;
                        double y2_max = EnergyProfileModel.CurrentSet[points + 1].MaxCurrent;
                        double y1_min = EnergyProfileModel.CurrentSet[points].MinCurrent;
                        double y2_min = EnergyProfileModel.CurrentSet[points + 1].MinCurrent;


                        // set default values for interp
                        double n = (x2 - x1);
                        double x = x1;

                        // set the first Item
                        if (points == 1)
                        {
                            EnergyProfileModel.AverageCurrentSplinePoints.Add(new PointF((float)x1,
                                (float)EnergyProfileModel.CurrentSet[points].AverageCurrent));
                            EnergyProfileModel.MaxCurrentSplinePoints.Add(new PointF((float)x1,
                                (float)EnergyProfileModel.CurrentSet[points].MaxCurrent));
                            EnergyProfileModel.MinCurrentSplinePoints.Add(new PointF((float)x1,
                                (float)EnergyProfileModel.CurrentSet[points].MinCurrent));
                        }
                        for (int i = 0; i < n - 1; i++)
                        {
                            // for each degree
                            x++;
                            // formula for linear interpolation (https://en.wikipedia.org/wiki/Linear_interpolation)
                            var y = ((y2 - y1) / (x2 - x1)) * x + y1 - ((y2 - y1) / (x2 - x1)) * x1;
                            var y_max = ((y2_max - y1_max) / (x2 - x1)) * x + y1_max -
                                        ((y2_max - y1_max) / (x2 - x1)) * x1;
                            var y_min = ((y2_min - y1_min) / (x2 - x1)) * x + y1_min -
                                        ((y2_min - y1_min) / (x2 - x1)) * x1;
                            EnergyProfileModel.AverageCurrentSplinePoints.Add(new PointF((float)x,
                                (float)Math.Round(y, 2)));
                            EnergyProfileModel.MaxCurrentSplinePoints.Add(new PointF((float)x,
                                (float)Math.Round(y_max, 2)));
                            EnergyProfileModel.MinCurrentSplinePoints.Add(new PointF((float)x,
                                (float)Math.Round(y_min, 2)));
                        }
                        // fill the last parameter
                        EnergyProfileModel.AverageCurrentSplinePoints.Add(new PointF((float)x2,
                            (float)EnergyProfileModel.CurrentSet[points + 1].AverageCurrent));
                        EnergyProfileModel.MaxCurrentSplinePoints.Add(new PointF((float)x2,
                            (float)EnergyProfileModel.CurrentSet[points + 1].MaxCurrent));
                        EnergyProfileModel.MinCurrentSplinePoints.Add(new PointF((float)x2,
                            (float)EnergyProfileModel.CurrentSet[points + 1].MinCurrent));

                    }
                    break;
                case PlotProfile.Velocity:
                    EnergyProfileModel.AverageVelocitySplinePoints.Clear();
                    EnergyProfileModel.MaxVelocitySplinePoints.Clear();
                    EnergyProfileModel.MinVelocitySplinePoints.Clear();
                    for (int points = 1; points < EnergyProfileModel.VelocitySet.Count; points++)
                    {
                        // set start and end parameters for each segment
                        double x1 = EnergyProfileModel.VelocitySet[points].Angle;
                        double x2 = EnergyProfileModel.VelocitySet[points + 1].Angle;
                        double y1 = EnergyProfileModel.VelocitySet[points].AverageVelocity;
                        double y2 = EnergyProfileModel.VelocitySet[points + 1].AverageVelocity;
                        double y1_max = EnergyProfileModel.VelocitySet[points].MaxVelocity;
                        double y2_max = EnergyProfileModel.VelocitySet[points + 1].MaxVelocity;
                        double y1_min = EnergyProfileModel.VelocitySet[points].MinVelocity;
                        double y2_min = EnergyProfileModel.VelocitySet[points + 1].MinVelocity;

                        // set default values for interp
                        double n = (x2 - x1);
                        double x = x1;

                        // set the first Item
                        if (points == 1)
                        {
                            EnergyProfileModel.AverageVelocitySplinePoints.Add(new PointF((float)x1,
                                (float)EnergyProfileModel.VelocitySet[points].AverageVelocity));
                            EnergyProfileModel.MaxVelocitySplinePoints.Add(new PointF((float)x1,
                                (float)EnergyProfileModel.VelocitySet[points].MaxVelocity));
                            EnergyProfileModel.MinVelocitySplinePoints.Add(new PointF((float)x1,
                                (float)EnergyProfileModel.VelocitySet[points].MinVelocity));
                        }
                        for (int i = 0; i < n - 1; i++)
                        {
                            // for each degree
                            x++;
                            // formula for linear interpolation (https://en.wikipedia.org/wiki/Linear_interpolation)
                            var y = ((y2 - y1) / (x2 - x1)) * x + y1 - ((y2 - y1) / (x2 - x1)) * x1;
                            var y_max = ((y2_max - y1_max) / (x2 - x1)) * x + y1_max -
                                        ((y2_max - y1_max) / (x2 - x1)) * x1;
                            var y_min = ((y2_min - y1_min) / (x2 - x1)) * x + y1_min -
                                        ((y2_min - y1_min) / (x2 - x1)) * x1;
                            EnergyProfileModel.AverageVelocitySplinePoints.Add(new PointF((float)x,
                                (float)Math.Round(y, 2)));
                            EnergyProfileModel.MaxVelocitySplinePoints.Add(new PointF((float)x,
                                (float)Math.Round(y_max, 2)));
                            EnergyProfileModel.MinVelocitySplinePoints.Add(new PointF((float)x,
                                (float)Math.Round(y_min, 2)));
                        }
                        // fill the last parameter
                        EnergyProfileModel.AverageVelocitySplinePoints.Add(new PointF((float)x2,
                            (float)EnergyProfileModel.VelocitySet[points + 1].AverageVelocity));
                        EnergyProfileModel.MaxVelocitySplinePoints.Add(new PointF((float)x2,
                            (float)EnergyProfileModel.VelocitySet[points + 1].MaxVelocity));
                        EnergyProfileModel.MinVelocitySplinePoints.Add(new PointF((float)x2,
                            (float)EnergyProfileModel.VelocitySet[points + 1].MinVelocity));
                    }
                    break;
            }
            WritePlotLogfile(profile);
        }

        /// <summary>
        /// plot the current
        /// </summary>
        public void Plot_Spline(Chart chart, PlotProfile profile)
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

                            // set new series
                            range.Points.DataBindXY(xValue: EnergyProfileModel.CurrentSet.Values, xField: "Angle",
                                yValue: EnergyProfileModel.CurrentSet.Values, yFields: "MaxCurrent,MinCurrent");
                            avrgCrnt.Points.DataBindXY(xValue: EnergyProfileModel.CurrentSet.Values, xField: "Angle",
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

                            // set new series
                            range.Points.DataBindXY(xValue: EnergyProfileModel.VelocitySet.Values, xField: "Angle",
                                yValue: EnergyProfileModel.VelocitySet.Values, yFields: "MaxVelocity,MinVelocity");
                            avrgVel.Points.DataBindXY(xValue: EnergyProfileModel.VelocitySet.Values, xField: "Angle",
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
                LinearInterpolation(profile);

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

        public void ChangeDeviation(int dev)
        {
            EnergyProfileModel.PercentDevCrnt = Math.Round(((double)dev / 100), 2);
        }

        /// <summary>
        /// Calculate the Energy-Consumption of actual part of moving
        /// </summary>
        /// <param name="distance_Horizontal">distance (only horizontal) between two waypoints</param>
        /// <param name="angle">the angle between two waypoints</param>
        /// <param name="altitudeDiff">result of altitude WP1 and WP2</param>
        /// <param name="hoverTime">the delay time btw the loiter_Time</param>
        /// <returns>energy-consumption in mAh</returns>
        public Dictionary<ECID, double> EnergyConsumption(double distance_Horizontal, double angle,
            double altitudeDiff, double hoverTime)
        {
            // testing the calculating methods
            TestCalculateClimbDistance();
            TestCalculateFlightTime();
            TestCalculateEnergyConsumption();

            angle = Math.Round(angle);

            var distance = Math.Abs(angle) > 0
                ? CalcClimbDistance(distance_Horizontal, angle)
                : distance_Horizontal;

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
            string path = _energyProfilePath + @"PlotProfile\";
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

            Assert.IsEquals((double)0, flighttime1, "Wrong value (flighttime1) of CalculateClimbDistance");
            Assert.IsEquals((double)6, flighttime2, "Wrong value (flighttime2) of CalculateClimbDistance");
            Assert.IsEquals((double)0, flighttime3, "Wrong value (flighttime3) of CalculateClimbDistance");
            Assert.IsEquals((double)0, flighttime4, "Wrong value (flighttime4) of CalculateClimbDistance");
            Assert.IsEquals((double)0, flighttime5, "Wrong value (flighttime5) of CalculateClimbDistance");
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
            RowType type = RowType.None;

            int timedelay = transitionTime;

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
                var dict = Columns_Data[RowType.CMD.ToString()].Zip(x.Skip(1), (s, i) => new { s, i })
                    .ToDictionary(itemt => itemt.s, itemt => itemt.i);

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

                // create current-object and add in list
                energyLogfileModel.CURR_Lines.Add(new CURR_Model(time, volt, curr));
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
                var alt = dict.ContainsKey(GPSFrame.RelAlt.ToString()) ? dict[GPSFrame.RelAlt.ToString()].Replace('.', ',') : GPSFrame.None.ToString(); // gps altitude

                switch (searchflag)
                {
                    case ESearchFlag.First:
                        //energyLogfileModel.GPS_Lines.Add(new GPS_Model(time, hdop, lat, lng, alt, false, false));
                        searchflag = ESearchFlag.Start;
                        cmdindex++;
                        break;
                    case ESearchFlag.Start: // loiter time in param1 + cmd_time + timedelay
                        int param1 = 0; // for cmd waypoint delay prm1

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
                        if (!energyLogfileModel.CMD_Lines.Count.Equals(cmdindex) && gpsitem.timems >= energyLogfileModel.CMD_Lines[cmdindex].Time_ms - timedelay)
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
        public void AnalyzeLogs(List<string> filenames, int gvalues, int transtime)
        {
            // init values
            guiltyValues = gvalues;
            transitionTime = transtime;
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

            // print values as CSV
            PrintCSV(logAnalizerModel.Angle_MeanCurrent, SectionType.Current);
            PrintCSV(logAnalizerModel.Angle_MeanSpeed, SectionType.Speed);
            PrintCSV(logAnalizerModel.AngleSection, SectionType.Current);
            PrintCSV(logAnalizerModel.AngleSection, SectionType.Speed);
            if (logAnalizerModel.HoverCurrentList.Count > 0)
                PrintCSV(logAnalizerModel.HoverCurrentList, SectionType.Hover);

            Loading.Close(); // close loading window after analyze

            // clean memory
            logAnalizerModel = null;
            GC.Collect();
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
                //    Loading.ShowLoading("LOGANALZYER");

                // fill all datas to specific logfile into a dict 
                logAnalizerModel.AllLogfiles.Add(System.IO.Path.GetFileName(FileName), LogDatas(logdata));
                logdata.Clear();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Failed to read File: " + ex.ToString());
                return;
            }
            //Loading.Close();
        }

        /// <summary>
        /// This methods calculate some important values from CMD-Values and GPS-Values and save it in CMD-Model
        /// </summary>
        private void CalcCMDValues()
        {
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
                        double alt_actpos;
                        if (cmdindex.Equals(1))
                            alt_actpos = 0;
                        else
                        {
                            alt_actpos = Convert.ToDouble(logfilemodel.Value.CMD_Lines[cmdindex - 1].Altitude);
                        }
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
                                 logfilemodel.Value.CMD_Lines[cmdindex].Distance.Equals(0))
                        {
                            logfilemodel.Value.CMD_Lines[cmdindex].FlightMode = FlightMode.Hover.ToString(); // ToDo
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
                                // hover from delay - param1
                                int delaytime = Convert.ToInt16(logfilemodel.Value.CMD_Lines[cmdindex].Param[0]) *
                                                1000; // in milliseconds
                                if (delaytime > 0) // set only if delaytime is set
                                {
                                    // save all values for calculate energy in hoverstate
                                    logfilemodel.Value.CMD_Lines[cmdindex].currentHoverList = GetCurrentList(
                                        logfilemodel.Value.CURR_Lines, logfilemodel.Value.CMD_Lines[cmdindex].Time_ms,
                                        delaytime);
                                    // save all values for calculate energy after hoverstate
                                    logfilemodel.Value.CMD_Lines[cmdindex].currentList = GetCurrentList(
                                        logfilemodel.Value.CURR_Lines, logfilemodel.Value.CMD_Lines[cmdindex].Time_ms,
                                        logfilemodel.Value.CMD_Lines[cmdindex + 1].Time_ms);
                                }
                                else // save speed and currentvalues 
                                {
                                    logfilemodel.Value.CMD_Lines[cmdindex].Speed = CalcSpeed(
                                        logfilemodel.Value.GPS_Lines
                                            [logfilemodel.Value.CMD_Lines[cmdindex].GPS_START_Index],
                                        logfilemodel.Value.GPS_Lines[
                                            logfilemodel.Value.CMD_Lines[cmdindex].GPS_END_Index],
                                        logfilemodel.Value.CMD_Lines[cmdindex].Angle, alt_diff);

                                    logfilemodel.Value.CMD_Lines[cmdindex].CurrentMean = CalcMeanCurrent(
                                        logfilemodel.Value.CURR_Lines, logfilemodel.Value.CMD_Lines[cmdindex].Time_ms,
                                        logfilemodel.Value.CMD_Lines[cmdindex + 1].Time_ms,
                                        guiltyValues); // only for one Command

                                    logfilemodel.Value.CMD_Lines[cmdindex].currentList = GetCurrentList(
                                        logfilemodel.Value.CURR_Lines, logfilemodel.Value.CMD_Lines[cmdindex].Time_ms,
                                        logfilemodel.Value.CMD_Lines[cmdindex + 1].Time_ms);

                                    logfilemodel.Value.CMD_Lines[cmdindex].CurrentCount =
                                        logfilemodel.Value.CMD_Lines[cmdindex].currentList.Count;
                                }
                            }
                            else if (Convert.ToByte(logfilemodel.Value.CMD_Lines[cmdindex].CmdId).Equals(19)) // only values for Hover in Loiter_Time
                            {
                                int delaytime = Convert.ToInt16(logfilemodel.Value.CMD_Lines[cmdindex + 1].Param[0]) *
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
            }
            //logAnalizerModel.LogDictionary.GetEnumerator(RowType.CMD.ToString());
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
        private double CalcSpeed(GPS_Model startmodel, GPS_Model endmodel, double angle, double altitude)
        {
            double distance;
            double speed;
            // if distance != 0 and angle not 90 or -90 degree
            if (!Math.Abs(angle).Equals(90))
            {
                double distance_on_ground = CalcGeoDistance(Convert.ToDouble(startmodel.Latitude),
                    Convert.ToDouble(endmodel.Latitude),
                    Convert.ToDouble(startmodel.Longitude), Convert.ToDouble(endmodel.Longitude));
                double rad = angle * Math.PI / 180;
                distance = distance_on_ground / Math.Cos(rad);
            }
            else
            {
                distance = Math.Abs(altitude);
            }
            var time = (endmodel.Time_ms - startmodel.Time_ms) / 1000; // from ms to s
            if (time > transitionTime / 1000 && !Math.Round(distance).Equals(0))
            {
                speed = Math.Round(distance / time, 1);
            }
            else
                speed = 0;
            return speed;
        }

        /// <summary>
        /// Calculate the mean-current.
        /// </summary>
        /// <param name="Curr_List">All values for calculate the mean.</param>
        /// <param name="starttime">Starttime for begin the interval.</param>
        /// <param name="endtime">Endtime for end of interval.</param>
        /// <param name="guilty_Values">Calculate only if even more guilty values</param>
        /// <returns>The mean-current value</returns>
        private double CalcMeanCurrent(List<CURR_Model> Curr_List, int starttime, int endtime, int guilty_Values)
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
            if (valueCount >= guilty_Values)
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
            foreach (var line in allCurrentLines)
            {
                if (line.Time_ms >= starttime && line.Time_ms <= endtime)
                {
                    actCurrList.Add(Convert.ToDouble(line.Current));
                }
            }
            return actCurrList;
        }

        /// <summary>
        /// Calculate at the End of parsing logfiles the meancurrent und meanspeed of all founding angles
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
                                mean = sectionValue / count;
                            logAnalizerModel.Angle_MeanSpeed.Add(anglepair.Key, mean);
                            break;
                        case SectionType.Current: // only current section
                            if (sectiontype.Value.Count > guiltyValues)
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
                                    mean = Math.Round(sectionValue / (count * ampfactor), 2);
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

                foreach (var current in logAnalizerModel.HoverCurrentList)
                {
                    value += current;
                }

                logAnalizerModel.Angle_MeanCurrent_Hover = Math.Round(value / (logAnalizerModel.HoverCurrentList.Count * ampfactor), 2);
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
        /// lear the existing models.
        /// </summary>
        private void ClearModelSets()
        {
            EnergyProfileModel.VelocitySet.Clear();
            EnergyProfileModel.CurrentSet.Clear();
            EnergyProfileModel.CurrentHover = new CurrentModel(0f, 0f);

            EnergyProfileModel.VelocitySet.Add(1, new VelocityModel(-90f, 0f, 0f));
            EnergyProfileModel.VelocitySet.Add(2, new VelocityModel(0f, 0f, 0f));
            EnergyProfileModel.VelocitySet.Add(3, new VelocityModel(0f, 0f, 0f));
            EnergyProfileModel.VelocitySet.Add(4, new VelocityModel(0f, 0f, 0f));
            EnergyProfileModel.VelocitySet.Add(5, new VelocityModel(0f, 0f, 0f));
            EnergyProfileModel.VelocitySet.Add(6, new VelocityModel(0f, 0f, 0f));
            EnergyProfileModel.VelocitySet.Add(7, new VelocityModel(0f, 0f, 0f));
            EnergyProfileModel.VelocitySet.Add(8, new VelocityModel(0f, 0f, 0f));
            EnergyProfileModel.VelocitySet.Add(9, new VelocityModel(0f, 0f, 0f));
            EnergyProfileModel.VelocitySet.Add(10, new VelocityModel(0f, 0f, 0f));
            EnergyProfileModel.VelocitySet.Add(11, new VelocityModel(90f, 0f, 0f));

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
            EnergyProfileModel.CurrentHover.AverageCurrent = logAnalizerModel.Angle_MeanCurrent_Hover;
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

        private void PrintCSV(object model, SectionType sectiontype)
        {
            string path = Settings.Instance.LogDir + @"\CSV\";
            DateTime date = DateTime.Now;
            string filename = date.ToString(CultureInfo.CurrentCulture).Replace(":", "_");

            // check if dirfectory exist
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (model.GetType() == typeof(Dictionary<double, double>))
            {
                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(path + filename + "_Angle_Mean" + sectiontype + ".csv"))
                {

                    foreach (var line in (Dictionary<double, double>)model)
                    {
                        file.WriteLine(line.Key + "; " + line.Value);
                    }
                }

            }
            else if (model.GetType() == typeof(Dictionary<double, Dictionary<SectionType, List<double>>>))
            {

                using (System.IO.StreamWriter file =
                    new System.IO.StreamWriter(path + filename + "_Angle_" + sectiontype + ".csv"))
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
                using (System.IO.StreamWriter file =
                    new System.IO.StreamWriter(path + filename + "_" + sectiontype + "_Current" + ".csv"))
                {

                    foreach (var line in (List<double>)model)
                    {
                        file.WriteLine(line);
                    }
                }
            }
        }

        // todo for testing 

        private void SetNewValuesInModel(SectionType section)
        {
            Dictionary<double, double> sectionDict = new Dictionary<double, double>();
            object energydict;
            Type castType;
            switch (section)
            {
                case SectionType.Current:
                    sectionDict = logAnalizerModel.Angle_MeanCurrent;
                    //energydict = EnergyProfileModel.CurrentSet;
                    break;
                case SectionType.Speed:
                    sectionDict = logAnalizerModel.Angle_MeanSpeed;
                    castType = EnergyProfileModel.VelocitySet.GetType();
                    energydict = new Dictionary<int, VelocityModel>(EnergyProfileModel.VelocitySet);

                    break;
                case SectionType.Hover:
                    EnergyProfileModel.CurrentHover.AverageCurrent = logAnalizerModel.Angle_MeanCurrent_Hover;
                    return;
            }

            foreach (KeyValuePair<double, double> anglepair in sectionDict)
            {
                if (!anglepair.Value.Equals(0))
                {
                    // only values at -90 degree --> straight down
                    if (anglepair.Key.Equals(-90))
                    {
                        //if (!energydict[1].Equals(null))
                        //{
                        //    (Dictionary<int, VelocityModel>)energydict[1].AverageVelocity = anglepair.Value;
                        //}
                        //else
                        //{
                        //    EnergyProfileModel.VelocitySet[1] = new VelocityModel(-90, anglepair.Value);
                        //}
                    }
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

    }
}
