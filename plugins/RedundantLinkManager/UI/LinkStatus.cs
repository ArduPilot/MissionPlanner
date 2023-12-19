using Bulb;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RedundantLinkManager
{
    public partial class LinkStatus : UserControl
    {
        private readonly List<Label> LinkNames = new List<Label>();
        private readonly List<LedBulb> Bulbs = new List<LedBulb>();
        private readonly List<RadioButton> RadioButtons = new List<RadioButton>();

        // Prevents programatic changes from triggering events
        private bool SuppressEvents = false;

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
            chk_autoswitch.Checked = bool.Parse(Plugin.Host.config["RedundantLinkManager_AutoSwitch", "true"]);
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

            // Track indices and qualities for auto switching
            int firstBest = 0;  // Index of the first link with the best quality
            int selected = 0;   // Index of the currently selected link
            Link.Quality bestQuality = Link.Quality.Off;  // Quality of the best link
            Link.Quality selectedQuality = Link.Quality.Off;  // Quality of the selected link

            // Update the status of each link
            for (int i = 0; i < Plugin.Links.Count; i++)
            {
                var link = Plugin.Links[i];
                LinkNames[i].Text = link.Name;
                var linkQuality = link.GetQuality();
                Bulbs[i].Color = QualityColors[linkQuality.CurrentQuality];
                Bulbs[i].On = linkQuality.CurrentQuality != Link.Quality.Off;

                if (link.comPort == Plugin.Host.comPort)
                {
                    SuppressEvents = true;
                    RadioButtons[i].Checked = true;
                    SuppressEvents = false;
                    selected = i;
                    selectedQuality = linkQuality.CurrentQuality;
                }

                if(linkQuality.CurrentQuality > bestQuality)
                {
                    firstBest = i;
                    bestQuality = linkQuality.CurrentQuality;
                }

                // Update the tooltip
                toolTip1.SetToolTip(LinkNames[i], linkQuality.Reasons);
                toolTip1.SetToolTip(Bulbs[i], linkQuality.Reasons);
                toolTip1.SetToolTip(RadioButtons[i], linkQuality.Reasons);
            }

            // Autoswitch
            if (chk_autoswitch.Checked && bestQuality > selectedQuality)
            {
                RadioButtons[firstBest].Checked = true;
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
            if (SuppressEvents) return;
            
            // Do nothing when "unchecking"
            var sender_rad = sender as RadioButton;
            var index = RadioButtons.IndexOf(sender_rad);
            if (!sender_rad.Checked) return;

            if(index >= 0)
            {
                Plugin.SwitchLink(index);
            }
        }

        private void chk_autoswitch_CheckedChanged(object sender, EventArgs e)
        {
            Plugin.Host.config["RedundantLinkManager_AutoSwitch"] = ((CheckBox)sender).Checked.ToString();
        }
    }
}
