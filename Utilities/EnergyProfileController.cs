using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using DotSpatial.Topology.Utilities;
using log4net.Filter;
using Microsoft.Scripting.AspNet.MembersInjectors;
using Org.BouncyCastle.Crypto.Tls;

namespace MissionPlanner.Utilities
{
    class EnergyProfileController : IConfigEnergyProfile, IEnergyConsumption
    {
        private string _energyProfilePath =
            Settings.GetUserDataDirectory() + "EnergyProfile" + Path.DirectorySeparatorChar;

        public enum PlotProfile
        {
            Current,
            Velocity
        }

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
                                chart.ChartAreas[0].AxisY.Minimum = findMinByValue.YValues[0] - 5.00f;
                            var findMaxByValue = maxCrnt.Points.FindMaxByValue();
                            if (findMaxByValue != null)
                                chart.ChartAreas[0].AxisY.Maximum = findMaxByValue.YValues[0] + 5.00f;
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
                ? CalculateClimbDistance(distance_Horizontal, angle)
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

            double flighttime_avr = CalculateFlghtTime(distance, speedAvrg);
            double flighttime_max = CalculateFlghtTime(distance, speedMax);
            double flighttime_min = CalculateFlghtTime(distance, speedMin);

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
                hoverEnergyConsumption = CalculateEnergyConsumption(EnergyProfileModel.CurrentHover.AverageCurrent, hoverTime);
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
        private double CalculateClimbDistance(double distance_Horizontal, double angle)
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
        private double CalculateFlghtTime(double distance, double speed)
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
            string path = @"c:\Users\kruets\Documents\Mission Planner\EnergyProfile\";
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
            var distance1 = CalculateClimbDistance(50, -90);
            var distance2 = CalculateClimbDistance(50, +90);
            var distance3 = CalculateClimbDistance(100, 0);
            var distance4 = CalculateClimbDistance(50, -37);
            var distance5 = CalculateClimbDistance(50, 18);

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
            var flighttime1 = CalculateFlghtTime(50, 0);
            var flighttime2 = CalculateFlghtTime(50, 7.95);
            var flighttime3 = CalculateFlghtTime(-10, 6.92);
            var flighttime4 = CalculateFlghtTime(50, -37);
            var flighttime5 = CalculateFlghtTime(-50, -37);

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
    }

}
