using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionPlanner.Utilities;
using MissionPlanner.Controls;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using MissionPlanner;
using System.Drawing;
using GMap.NET.WindowsForms;
using MissionPlanner.GCSViews;
using MissionPlanner.Maps;
using MissionPlanner.Comms;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Threading;

namespace donate
{
    public class Donate : MissionPlanner.Plugin.Plugin
    {
        public override string Name
        {
            get { return "Donate"; }
        }

        public override string Version
        {
            get { return "0.10"; }
        }

        public override string Author
        {
            get { return "Example"; }
        }

        public override bool Init()
        {
            return true;
        }

        public override bool Loaded()
        {
            return true;
            
            var MenuCustom = new System.Windows.Forms.ToolStripButton();
            MenuCustom.Name = "MenuCustom";
            MenuCustom.Margin = new System.Windows.Forms.Padding(0);
            MenuCustom.Size = new Size(56, 47);
            MenuCustom.Text = "DONATE";
            MenuCustom.TextAlign = ContentAlignment.BottomCenter;
            MenuCustom.TextImageRelation = TextImageRelation.ImageAboveText;
            var idx = MainV2.instance.MainMenu.Items.IndexOfKey("MenuHelp");
            MainV2.instance.MainMenu.Items.Insert(idx+1, MenuCustom);

            var custom = new Bitmap(47, 47);
            using (var g = Graphics.FromImage(custom))
            {
                g.FillRectangle(new SolidBrush(System.Drawing.ColorTranslator.FromHtml("#0057b7")), new Rectangle(0, 0, 47, 47 / 2));
                g.FillRectangle(new SolidBrush(System.Drawing.ColorTranslator.FromHtml("#ffd700")), new Rectangle(0, 47 / 2, 47, 47 / 2));
                g.Flush();
            }
            MenuCustom.Image = custom;
            MenuCustom.Click += (s, e) =>
            {
                System.Diagnostics.Process.Start("https://www.unicef.org.au/appeals/ukraine-emergency-appeal");
            };

            return true;
        }


        public override bool Loop()
        {
            return true;
        }

        public override bool Exit()
        {
            return true;
        }
    }
}