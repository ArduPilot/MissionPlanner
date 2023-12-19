using Bulb;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RedundantLinkManager
{
    public partial class LinkStatus : UserControl
    {
        private readonly List<Label> LinkNames = new List<Label>();
        private readonly List<LedBulb> Bulbs = new List<LedBulb>();
        private readonly List<RadioButton> RadioButtons = new List<RadioButton>();

        private readonly RedundantLinkManager_Plugin Plugin;

        private readonly Dictionary<Link.Quality, Color> QualityColors = new Dictionary<Link.Quality, Color>()
        {
            { Link.Quality.Good, Color.LightGreen },
            { Link.Quality.Marginal, Color.Yellow },
            { Link.Quality.Critical, Color.Red },
            { Link.Quality.Off, Color.Gray },
        };

        public LinkStatus(RedundantLinkManager_Plugin plugin)
        {
            InitializeComponent();

            Plugin = plugin;
        }

        public void UpdateStatus()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateStatus));
                return;
            }

            // Check if links need to be added or removed
            if (Plugin.Links.Count != LinkNames.Count)
            {
                RebuildLayout();
            }

            // Update the status of each link
            for (int i = 0; i < Plugin.Links.Count; i++)
            {
                var link = Plugin.Links[i];
                LinkNames[i].Text = link.Name;
                var linkQuality = link.GetQuality();
                Bulbs[i].Color = QualityColors[linkQuality];
                Bulbs[i].On = linkQuality != Link.Quality.Off;
                RadioButtons[i].Checked = link.comPort == Plugin.Host.comPort;
            }
        }

        /// <summary>
        /// Constructs the table layout for the links
        /// </summary>
        private void RebuildLayout()
        {
            // Dispose of all the old controls
            foreach (var linkName in LinkNames)
            {
                linkName.Dispose();
            }
            LinkNames.Clear();
            foreach (var bulb in Bulbs)
            {
                bulb.Dispose();
            }
            Bulbs.Clear();
            foreach (var radioButton in RadioButtons)
            {
                radioButton.Dispose();
            }
            RadioButtons.Clear();

            foreach(var link in Plugin.Links)
            {
                LinkNames.Add(new Label()
                {
                    Anchor = label1.Anchor,
                    AutoEllipsis = label1.AutoEllipsis,
                    AutoSize = label1.AutoSize,
                    Margin = label1.Margin,
                    Size = label1.Size,
                    Text = link.Name,
                });
                Bulbs.Add(new LedBulb()
                {
                    Anchor = ledBulb1.Anchor,
                    Margin = ledBulb1.Margin,
                    Size = ledBulb1.Size,
                });
                var rad = new RadioButton()
                {
                    Anchor = radioButton1.Anchor,
                    Margin = radioButton1.Margin,
                    Size = radioButton1.Size,
                    Text = radioButton1.Text,
                };
                rad.CheckedChanged += Rad_CheckedChanged;
                RadioButtons.Add(rad);
            }

            // Add the controls to the table layout
            tbl_links.Controls.Clear();
            for (int i = 0; i < LinkNames.Count; i++)
            {
                int base_column = 3 * (i / 3);
                int row = i % 3;
                tbl_links.Controls.Add(RadioButtons[i], base_column + 0, row);
                tbl_links.Controls.Add(Bulbs[i], base_column + 1, row);
                tbl_links.Controls.Add(LinkNames[i], base_column + 2, row);
            }
        }

        private void Rad_CheckedChanged(object sender, EventArgs e)
        {
            // throw new NotImplementedException();
        }
    }
}
