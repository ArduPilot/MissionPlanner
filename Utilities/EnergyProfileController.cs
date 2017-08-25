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
using Microsoft.Scripting.AspNet.MembersInjectors;
using Org.BouncyCastle.Crypto.Tls;

namespace MissionPlanner.Utilities
{
    class EnergyProfileController : IConfigEnergyProfile, IEnergyConsumption
    {
        private string _energyProfilePath = Settings.GetUserDataDirectory() + "EnergyProfile" + Path.DirectorySeparatorChar;

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
        public void ImportProfile()
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
            }
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
        public void LinearInterpolation()
        {
            EnergyProfileModel.AverageCurrentSplinePoints.Clear();
            for (int points = 1; points < EnergyProfileModel.CurrentSet.Count; points++)
            {
                // set start and end parameters for each segment
                double x1 = EnergyProfileModel.CurrentSet[points].Angle;
                double x2 = EnergyProfileModel.CurrentSet[points + 1].Angle;
                double y1 = EnergyProfileModel.CurrentSet[points].AverageCurrent;
                double y2 = EnergyProfileModel.CurrentSet[points + 1].AverageCurrent;

                // set default values for interp
                double n = (x2 - x1);
                double x = x1;

                // set the first Item
                if (points == 1)
                    EnergyProfileModel.AverageCurrentSplinePoints.Add(new PointF((float)x1, (float)EnergyProfileModel.CurrentSet[points].AverageCurrent));
                for (int i = 0; i < n; i++)
                {
                    // for each degree
                    x++;
                    // formula for linear interpolation (https://en.wikipedia.org/wiki/Linear_interpolation)
                    var y = ((y2 - y1) / (x2 - x1)) * x + y1 - ((y2 - y1) / (x2 - x1)) * x1;
                    EnergyProfileModel.AverageCurrentSplinePoints.Add(new PointF((float)x, (float)y));
                }
                // fill the last parameter
                EnergyProfileModel.AverageCurrentSplinePoints.Add(new PointF((float)x2, (float)EnergyProfileModel.CurrentSet[points + 1].AverageCurrent));
            }
        }

        /// <summary>
        /// plot the current
        /// </summary>
        public void PlotCurrent_Spline(Chart chart)
        {
            try
            {
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
                    maxCrnt.Points.DataBindXY(xValue: EnergyProfileModel.CurrentSet.Values, xField: "Angle", yValue: EnergyProfileModel.CurrentSet.Values,
                        yFields: "MaxCurrent");
                    minCrnt.Points.DataBindXY(xValue: EnergyProfileModel.CurrentSet.Values, xField: "Angle", yValue: EnergyProfileModel.CurrentSet.Values,
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

                LinearInterpolation();
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
            XElement root = new XElement("CurrentSet", new XAttribute("DevPercent", EnergyProfileModel.PercentDev));
            var xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);

            XElement xCurrentModelHover = new XElement("CurrentModel",
                                                    new XAttribute("ID", "Hover"),
                                                    new XElement("AverageCurrent", EnergyProfileModel.CurrentHover.AverageCurrent),
                                                    new XElement("Deviation", EnergyProfileModel.CurrentHover.Deviation));
            root.Add(xCurrentModelHover);
            foreach (KeyValuePair<int, CurrentModel> currentModel in energyCurrentSet)
            {
                XElement xCurrentModel = new XElement("CurrentModel",
                                                    new XAttribute("ID", currentModel.Key),
                                                    new XElement("Angle", currentModel.Value.Angle),
                                                    new XElement("AverageCurrent", currentModel.Value.AverageCurrent),
                                                    new XElement("Deviation", currentModel.Value.Deviation));
                root.Add(xCurrentModel);
            }

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
            // load and set root element
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(path);
            XmlNode root = xdoc.SelectSingleNode("CurrentSet");
            if (root != null)
            {
                XmlNodeList currentModelList = root.SelectNodes("//CurrentModel");
                var attributePerc = root.Attributes?["DevPercent"];
                if (currentModelList != null)
                {
                    foreach (XmlNode currentmodel in currentModelList)
                    {
                        
                        if (currentmodel.Attributes != null)
                        {
                            var attributeID = currentmodel.Attributes["ID"];
                            var xmlElementAngle = currentmodel["Angle"];
                            var xmlElementCurrent = currentmodel["AverageCurrent"];
                            var xmlElementDeviation = currentmodel["Deviation"];

                            if (attributePerc != null)
                            {
                                EnergyProfileModel.PercentDev = Convert.ToDouble(attributePerc.Value.Replace(".", ","));

                                if (attributeID.Value == "Hover")
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
        }

        public void ChangeDeviation(int dev)
        {
            EnergyProfileModel.PercentDev = Math.Round(((double)dev / 100), 2);
        }

        /// <summary>
        /// Calculate the Energy-Consumption of actual part of moving
        /// </summary>
        /// <param name="accel_Horizontal">horizontal accelaration</param>
        /// <param name="accel_Vertical">vertical accelaration</param>
        /// <param name="maxSpeed_Horizontal">maximum horizontal speed</param>
        /// <param name="maxSpeed_Vertical_UP">maximum vertical upwards speed</param>
        /// <param name="maxSpeed_Vertical_DN">maximum vertical downwards speed</param>
        /// <param name="distance_Horizontal">distance (only horizontal) between two waypoints</param>
        /// <param name="angle">the angle between two waypoints</param>
        /// <param name="altitude">result of altitude WP1 and WP2</param>
        /// <returns>energy-consumption in mAh</returns>
        public double CalculateEnergyConsumption(double accel_Horizontal, double accel_Vertical, double maxSpeed_Horizontal,
            double maxSpeed_Vertical_UP, double maxSpeed_Vertical_DN, double distance_Horizontal, double angle, double altitude, double hoverTime)
        {
            double time;
            double timeToMaxSpeedHorizontal = TimeToMaxSpeed(accel_Horizontal, maxSpeed_Horizontal);
            double timeToMaxSpeedVertical_UP = TimeToMaxSpeed(accel_Vertical, maxSpeed_Vertical_UP);
            double timeToMaxSpeedVertical_DN = TimeToMaxSpeed(accel_Vertical, maxSpeed_Vertical_DN);
            angle = Math.Round(angle);

            // fly straight
            if (angle == 0)
            {
                // result distance for acceleration and deceleration
                var doubleDistanceToMaxSpeed = 2 * DistanceToMaxSpeed(accel_Horizontal, timeToMaxSpeedHorizontal);
                if (distance_Horizontal >= doubleDistanceToMaxSpeed)
                {
                    time = CalculateFlyTime(
                               distance_Horizontal - doubleDistanceToMaxSpeed,
                               maxSpeed_Horizontal) + 2 * timeToMaxSpeedHorizontal;
                }
                else
                {
                    time = 2 * Math.Sqrt(distance_Horizontal / accel_Horizontal);
                }
            }
            // fly vertical up
            else if (angle == 90)
            {
                // result distance for acceleration and deceleration
                var doubleDistanceToMaxSpeedUp = 2 * DistanceToMaxSpeed(accel_Vertical, timeToMaxSpeedVertical_UP);
                if (altitude >= doubleDistanceToMaxSpeedUp)
                {
                    time = CalculateFlyTime(altitude - doubleDistanceToMaxSpeedUp, maxSpeed_Vertical_UP) +
                           2 * timeToMaxSpeedVertical_UP;
                }
                else
                {
                    time = 2 * Math.Sqrt(altitude / accel_Vertical);
                }
            }
            // fly vertical down
            else if (angle == -90)
            {
                // result distance for acceleration and deceleration
                var doubleDistanceToMaxSpeedDown = 2 * DistanceToMaxSpeed(accel_Vertical, timeToMaxSpeedVertical_DN);
                if (altitude >= doubleDistanceToMaxSpeedDown)
                {
                    time = CalculateFlyTime(
                               altitude - 2 * DistanceToMaxSpeed(accel_Vertical, timeToMaxSpeedVertical_DN),
                               maxSpeed_Vertical_DN) + 2 * timeToMaxSpeedVertical_DN;
                }
                else
                {
                    time = 2 * Math.Sqrt(altitude / accel_Vertical);
                }
            }
            // other cases
            else
            {
                var maxClimbSpeed = angle < 0 ? Math.Sqrt(Math.Pow(maxSpeed_Horizontal, 2) + Math.Pow(maxSpeed_Vertical_DN, 2)) : Math.Sqrt(Math.Pow(maxSpeed_Horizontal, 2) + Math.Pow(maxSpeed_Vertical_UP, 2));
                var maxClimbAccel = Math.Sqrt(Math.Pow(accel_Horizontal, 2) + Math.Pow(accel_Vertical, 2));
                var climbTime = TimeToMaxSpeed(maxClimbAccel, maxClimbSpeed);
                var doubleMaxClimbDistance = 2 * DistanceToMaxSpeed(maxClimbAccel, climbTime);
                var climbDistance = CalculateClimbDistance(distance_Horizontal, angle);
                if (climbDistance >= doubleMaxClimbDistance)
                {
                    var constFlyTime = CalculateFlyTime(climbDistance - doubleMaxClimbDistance, maxClimbSpeed);
                    time = constFlyTime + 2 * climbTime;

                }
                else
                {
                    time = 2 * Math.Sqrt(doubleMaxClimbDistance / maxClimbAccel);
                }
            }
            // if delay from hover
            double hoverEnergyConsumption = 0;
            int index = EnergyProfileModel.AverageCurrentSplinePoints.FindIndex(a => a.X == Convert.ToDouble(angle));
            double current = Convert.ToDouble(EnergyProfileModel.AverageCurrentSplinePoints[index].Y);
            if (hoverTime > 0)
            {
                hoverEnergyConsumption = Math.Round(EnergyProfileModel.CurrentHover.AverageCurrent * hoverTime * 1000 / 3600, 1, MidpointRounding.AwayFromZero);
            }

            var energyConsumption = Math.Round(current * time * 1000 / 3600, 1, MidpointRounding.AwayFromZero);
            return energyConsumption + hoverEnergyConsumption;
        }
        /// <summary>
        /// calculate the climb distance ... like triangle
        /// </summary>
        /// <param name="distance_Horizontal">This is the distance from specific waypoint below FlightPlanner.</param>
        /// <param name="angle">This is the angle from specific waypoint below FlightPlanner.</param>
        /// <returns>Returns the diagonale-distance (DOUBLE)</returns>
        private double CalculateClimbDistance(double distance_Horizontal, double angle)
        {
            double distance = 0.0f;
            if (Math.Abs(angle) != 90 )
                distance = distance_Horizontal / Math.Cos(Math.PI/180*angle);
            return distance;
        }
        /// <summary>
        /// How much time passes to the maximum speed.
        /// </summary>
        /// <param name="accel">Const. acceleration</param>
        /// <param name="maxSpeed">maximum speed</param>
        /// <returns>Returns the time (DOUBLE)</returns>
        private double TimeToMaxSpeed(double accel, double maxSpeed)
        {
            double time = 0.0f;
            if (accel != 0)
                time = Math.Abs(maxSpeed / accel);
            return time;
        }
        /// <summary>
        /// Calculates the distance traveled to the maximum speed.
        /// </summary>
        /// <param name="accel">const acceleration</param>
        /// <param name="time">acceleration time to maximum speed</param>
        /// <returns></returns>
        private double DistanceToMaxSpeed(double accel, double time)
        {
            double distance = 0.0f;
            distance = 0.5 * accel * Math.Pow(time, 2);
            return distance;
        }
        /// <summary>
        /// Calculates the pure flight time without acceleration^.
        /// </summary>
        /// <param name="distance">const distance</param>
        /// <param name="maxSpeed">const maximum speed</param>
        /// <returns></returns>
        private double CalculateFlyTime(double distance, double maxSpeed)
        {
            double time = 0.0f;
            if (maxSpeed != 0)
                time = distance / maxSpeed;
            return time;
        }
    }

    /// <summary>
    /// UNITTEST AAA-Test
    /// UNITTEST AAA-Test
    /// </summary>
    public class TestEnergyProfile
    {
        public void TestEnergyConsumption()
        {
            var energyConsumption = new EnergyProfileController();

            var energy = Math.Round(energyConsumption.CalculateEnergyConsumption(1, 1, 5, 2.5f, 1.5f, 111, 18, 33, 0), 2);

            Assert.IsEquals(125.8, energy, "Wrong value of Energy-Consumption");
        }
    }
}
