using GMap.NET.MapProviders;
using System;
using System.Globalization;
using System.Windows.Forms;

namespace GMap.NET
{
    /// <summary>
    /// form helping to prefetch tiles on local db
    /// </summary>
    public partial class TilePrefetcherMenu : Form
    {
        public int Minimum { get; set; } = 1;
        public int Maximum { get; set; } = 20;

        private RectLatLng area;
        private GMapProvider provider;
        private bool isInitialized = false;

        public TilePrefetcherMenu(int min, int max, RectLatLng area, GMapProvider provider)
        {
            InitializeComponent();
            this.area = area;
            this.provider = provider;
            numericUpDownMinZoom.Minimum = numericUpDownMaxZoom.Minimum = trackBarMinZoom.Minimum = trackBarMaxZoom.Minimum = min;
            numericUpDownMinZoom.Maximum = numericUpDownMaxZoom.Maximum = trackBarMinZoom.Maximum = trackBarMaxZoom.Maximum = max;
            isInitialized = true;
            UpdateTilesCount();
        }

        private void UpdateTilesCount()
        {
            if (!isInitialized)
                return;

            long count = 0;
            string results = "";
            long total = 0;
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
            for (int i = Minimum; i <= Maximum; i++)
            {
                count = provider.Projection.GetAreaTileNumber(area, i, 0);

                results += "Zoom " + i + " : " + count.ToString("#,0", nfi);
                if (i < Maximum)
                    results += Environment.NewLine;
                total += count;
            }
            textBoxTile.Text = results;

            long averageTileSize = 15000; // byte
            labelTotal.Text = "Estimated: " + total.ToString("#,0", nfi) + " tile" + (total > 1 ? "s" : "") + " for " + sizeConverter(total * averageTileSize);
        }
        private string sizeConverter(float bytes)
        {
            try
            {
                float gb = 1024 * 1024 * 1024;
                float mb = 1024 * 1024;
                float kb = 1024;
                float result = 0;
                string ext = "";
                if (bytes >= gb)
                {
                    result = bytes / gb;
                    ext = " GB ";
                }
                else if (bytes >= mb)
                {
                    result = bytes / mb;
                    ext = " MB ";
                }
                else if (bytes >= kb)
                {
                    result = bytes / kb;
                    ext = " KB ";
                }
                else
                {
                    result = bytes;
                    ext = " B ";
                }
                string ret = (result).ToString("N2") + ext;
                return ret;
            }
            catch
            {

            }
            return "";
        }

        private void numericUpDownMinZoom_ValueChanged(object sender, EventArgs e)
        {
            if (Minimum != (int)numericUpDownMinZoom.Value)
            {
                if ((int)numericUpDownMinZoom.Value <= (int)numericUpDownMaxZoom.Value)
                {
                    Minimum = (int)numericUpDownMinZoom.Value;
                    trackBarMinZoom.Value = Minimum;
                    UpdateTilesCount();
                }
                else
                {
                    numericUpDownMinZoom.Value = Minimum;
                }
            }
        }

        private void trackBarMinZoom_ValueChanged(object sender, EventArgs e)
        {
            if (Minimum != (int)trackBarMinZoom.Value)
            {
                if ((int)trackBarMinZoom.Value <= (int)trackBarMaxZoom.Value)
                {
                    Minimum = (int)trackBarMinZoom.Value;
                    numericUpDownMinZoom.Value = Minimum;
                    UpdateTilesCount();
                }
                else
                {
                    trackBarMinZoom.Value = Minimum;
                }
            }
        }

        private void numericUpDownMaxZoom_ValueChanged(object sender, EventArgs e)
        {
            if (Maximum != (int)numericUpDownMaxZoom.Value)
            {
                if ((int)numericUpDownMinZoom.Value <= (int)numericUpDownMaxZoom.Value)
                {
                    Maximum = (int)numericUpDownMaxZoom.Value;
                    trackBarMaxZoom.Value = Maximum;
                    UpdateTilesCount();
                }
                else
                {
                    numericUpDownMaxZoom.Value = Maximum;
                }
            }
        }

        private void trackBarMaxZoom_ValueChanged(object sender, EventArgs e)
        {
            if (Maximum != (int)trackBarMaxZoom.Value)
            {
                if ((int)trackBarMinZoom.Value <= (int)trackBarMaxZoom.Value)
                {
                    Maximum = (int)trackBarMaxZoom.Value;
                    numericUpDownMaxZoom.Value = Maximum;
                    UpdateTilesCount();
                }
                else
                {
                    trackBarMaxZoom.Value = Maximum;
                }
            }
        }
    }
}
