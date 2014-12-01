﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Utilities;
using System.Xml;
using System.IO;
using log4net;
using System.Reflection;
using System.Globalization;
using MissionPlanner.Controls;
using System.Collections;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigSimplePids: MyUserControl, IActivate
    {
        internal static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        List<configitem> piditems = new List<configitem>();

        int y = 10;

        class configitem: IDisposable
        {
            public string title;
            public string desc;

            public string paramname;
            public float paramvalue;
            public float min;
            public float max;
            public List<relationitem> relations = new List<relationitem>();

            public Label lbl_min = new Label();
            public Label lbl_max = new Label();
            // use increments


            public void Dispose()
            {
                lbl_max.Dispose();
                lbl_min.Dispose();
            }
        }

        class relationitem
        {
            public string paramaname;
            public float multiplier;
        }

        public ConfigSimplePids()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            // prevent memory leak
            foreach (Control ctl in this.panel1.Controls)
            {
                ctl.Dispose();
            }

            y = 10;

            LoadXML(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + "acsimplepids.xml");


        }



        private void ConfigSimplePids_Load(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// The template xml for the screen
        /// </summary>
        /// <param name="FileName"></param>
        public void LoadXML(string FileName)
        {
            using (XmlReader reader = XmlReader.Create(FileName))
            {
                reader.Read();
                reader.ReadStartElement("simple");

                while (!reader.EOF)
                {
                    reader.ReadToFollowing("ac");

                    if (reader.Name == "")
                        break;

                    XmlReader acinner = reader.ReadSubtree();

                    while (acinner.ReadToFollowing("param"))
                    {
                        XmlReader inner = acinner.ReadSubtree();

                        configitem item = new configitem();

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
                                        item.paramvalue = float.Parse(inner.ReadString(), System.Globalization.NumberFormatInfo.InvariantInfo);
                                        break;
                                    case "min":
                                        item.min = float.Parse(inner.ReadString(), System.Globalization.NumberFormatInfo.InvariantInfo);
                                        break;
                                    case "max":
                                        item.max = float.Parse(inner.ReadString(), System.Globalization.NumberFormatInfo.InvariantInfo);
                                        break;
                                    case "relation":
                                        XmlReader relation = reader.ReadSubtree();
                                        relationitem relitem = new relationitem();
                                        while (relation.Read())
                                        {
                                            relation.MoveToElement();
                                            if (relation.Name == "param")
                                            {
                                                relitem.paramaname = inner.ReadString();
                                            }
                                            else if (relation.Name == "multiplier")
                                            {
                                                relitem.multiplier = float.Parse(inner.ReadString(), System.Globalization.NumberFormatInfo.InvariantInfo);
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

        void ProcessItem(configitem item)
        {
            try
            {
                if (!MainV2.comPort.MAV.param.ContainsKey(item.paramname))
                    return;

                float value = (float)MainV2.comPort.MAV.param[item.paramname];

                if (value < item.min)
                    item.min = value;
                if (value > item.max)
                    item.max = value;

                string range = ParameterMetaDataRepository.GetParameterMetaData(item.paramname, ParameterMetaDataConstants.Range, MainV2.comPort.MAV.cs.firmware.ToString());
                string increment = ParameterMetaDataRepository.GetParameterMetaData(item.paramname, ParameterMetaDataConstants.Increment, MainV2.comPort.MAV.cs.firmware.ToString());

                string[] rangeopt = range.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (rangeopt.Length > 1)
                {
                    item.min = float.Parse(rangeopt[0], System.Globalization.CultureInfo.InvariantCulture);
                    item.max = float.Parse(rangeopt[1], System.Globalization.CultureInfo.InvariantCulture);
                }

                float incrementf = 0.01f;
                if (increment.Length > 0)
                    float.TryParse(increment,NumberStyles.Float, CultureInfo.InvariantCulture, out incrementf);

                Controls.RangeControl RNG = new Controls.RangeControl(item.paramname, item.desc, item.title, incrementf, 1, item.min, item.max, value.ToString());
                RNG.Tag = item;

                RNG.Location = new Point(10, y);

                RNG.AttachEvents();

                RNG.ValueChanged += RNG_ValueChanged;

                ThemeManager.ApplyThemeTo(RNG);

                panel1.Controls.Add(RNG);

                y += RNG.Height;
            }
            catch (Exception ex) { CustomMessageBox.Show("Failed to process " + item.paramname + "\n" + ex.ToString()); }
        }

        void RNG_ValueChanged(object sender, string Name, string Value)
        {
            TXT_info.Clear();

            if (Value.Contains(','))
                Value = Value.Replace(",",".");

            float value = float.Parse(Value, System.Globalization.CultureInfo.InvariantCulture);

            Controls.RangeControl rc = ((Controls.RangeControl)sender);
            log.Info(rc.Name + " " + rc.Value);

            List<relationitem> relitems = ((configitem)rc.Tag).relations;

            try
            {
                MainV2.comPort.setParam(rc.Name, value);
            }
            catch (Exception ex) { CustomMessageBox.Show("Failed to change setting " + ex.Message ); return; }
            TXT_info.AppendText("set " + rc.Name + " " + rc.Value + "\r\n");

            foreach (var item in relitems)
            {
                try
                {
                    MainV2.comPort.setParam(item.paramaname, (float)(value * item.multiplier));
                    TXT_info.AppendText("set " + item.paramaname + " " + (float)(value * item.multiplier) + "\r\n");
                }
                catch (Exception ex) { CustomMessageBox.Show("Failed to change setting " + ex.Message); return; }
            }
        }
    }
}
