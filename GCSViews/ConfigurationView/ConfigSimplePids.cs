using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using log4net;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigSimplePids : MyUserControl, IActivate
    {
        internal static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private List<configitem> piditems = new List<configitem>();
        private int y = 10;

        public ConfigSimplePids()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            // prevent memory leak
            foreach (Control ctl in panel1.Controls)
            {
                ctl.Dispose();
            }

            y = 10;

            LoadXML(Settings.GetRunningDirectory() + "acsimplepids.xml");
        }

        private void ConfigSimplePids_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     The template xml for the screen
        /// </summary>
        /// <param name="FileName"></param>
        public void LoadXML(string FileName)
        {
            using (var reader = XmlReader.Create(FileName))
            {
                reader.Read();
                reader.ReadStartElement("simple");

                while (!reader.EOF)
                {
                    reader.ReadToFollowing("ac");

                    if (reader.Name == "")
                        break;

                    var acinner = reader.ReadSubtree();

                    while (acinner.ReadToFollowing("param"))
                    {
                        var inner = acinner.ReadSubtree();

                        var item = new configitem();

                        while (inner.Read())
                        {
                            inner.MoveToElement();
                            if (inner.IsStartElement())
                            {
                                switch (inner.Name.ToLower())
                                {
                                    case "title":
                                        item.title = inner.ReadString();
                                        break;
                                    case "desc":
                                        item.desc = inner.ReadString();
                                        break;
                                    case "name":
                                        item.paramname = inner.ReadString();
                                        break;
                                    case "value":
                                        item.paramvalue = float.Parse(inner.ReadString(), NumberFormatInfo.InvariantInfo);
                                        break;
                                    case "min":
                                        item.min = float.Parse(inner.ReadString(), NumberFormatInfo.InvariantInfo);
                                        break;
                                    case "max":
                                        item.max = float.Parse(inner.ReadString(), NumberFormatInfo.InvariantInfo);
                                        break;
                                    case "relation":
                                        var relation = reader.ReadSubtree();
                                        var relitem = new relationitem();
                                        while (relation.Read())
                                        {
                                            relation.MoveToElement();
                                            if (relation.Name == "param")
                                            {
                                                relitem.paramaname = inner.ReadString();
                                            }
                                            else if (relation.Name == "multiplier")
                                            {
                                                relitem.multiplier = float.Parse(inner.ReadString(),
                                                    NumberFormatInfo.InvariantInfo);
                                            }
                                        }

                                        item.relations.Add(relitem);
                                        break;
                                }
                            }
                        }
                        // process item to screen
                        ProcessItem(item);
                    }
                }
            }
        }

        private void ProcessItem(configitem item)
        {
            try
            {
                if (!MainV2.comPort.MAV.param.ContainsKey(item.paramname))
                    return;

                var value = (float) MainV2.comPort.MAV.param[item.paramname];

                if (value < item.min)
                    item.min = value;
                if (value > item.max)
                    item.max = value;

                var range = ParameterMetaDataRepository.GetParameterMetaData(item.paramname,
                    ParameterMetaDataConstants.Range, MainV2.comPort.MAV.cs.firmware.ToString());
                var increment = ParameterMetaDataRepository.GetParameterMetaData(item.paramname,
                    ParameterMetaDataConstants.Increment, MainV2.comPort.MAV.cs.firmware.ToString());

                var rangeopt = range.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                if (rangeopt.Length > 1)
                {
                    item.min = float.Parse(rangeopt[0], CultureInfo.InvariantCulture);
                    item.max = float.Parse(rangeopt[1], CultureInfo.InvariantCulture);
                }

                var incrementf = 0.01f;
                if (increment.Length > 0)
                    float.TryParse(increment, NumberStyles.Float, CultureInfo.InvariantCulture, out incrementf);

                var RNG = new RangeControl(item.paramname, item.desc, item.title, incrementf, 1, item.min, item.max,
                    value.ToString());
                RNG.Tag = item;

                RNG.Location = new Point(10, y);

                RNG.AttachEvents();

                RNG.ValueChanged += RNG_ValueChanged;

                ThemeManager.ApplyThemeTo(RNG);

                panel1.Controls.Add(RNG);

                y += RNG.Height;
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Failed to process " + item.paramname + "\n" + ex);
            }
        }

        private void RNG_ValueChanged(object sender, string Name, string Value)
        {
            TXT_info.Clear();

            if (Value.Contains(','))
                Value = Value.Replace(",", ".");

            var value = float.Parse(Value, CultureInfo.InvariantCulture);

            var rc = ((RangeControl) sender);
            log.Info(rc.Name + " " + rc.Value);

            var relitems = ((configitem) rc.Tag).relations;

            try
            {
                MainV2.comPort.setParam(rc.Name, value);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Failed to change setting " + ex.Message);
                return;
            }
            TXT_info.AppendText("set " + rc.Name + " " + rc.Value + "\r\n");

            foreach (var item in relitems)
            {
                try
                {
                    MainV2.comPort.setParam(item.paramaname, value*item.multiplier);
                    TXT_info.AppendText("set " + item.paramaname + " " + value*item.multiplier + "\r\n");
                }
                catch (Exception ex)
                {
                    CustomMessageBox.Show("Failed to change setting " + ex.Message);
                    return;
                }
            }
        }

        private class configitem : IDisposable
        {
            public readonly Label lbl_max = new Label();
            public readonly Label lbl_min = new Label();
            public readonly List<relationitem> relations = new List<relationitem>();
            public string desc;
            public float max;
            public float min;
            public string paramname;
            public float paramvalue;
            public string title;
            // use increments


            public void Dispose()
            {
                lbl_max.Dispose();
                lbl_min.Dispose();
            }
        }

        private class relationitem
        {
            public float multiplier;
            public string paramaname;
        }
    }
}