#region netDxf library, Copyright (C) 2009-2018 Daniel Carvajal (haplokuon@gmail.com)

//                        netDxf library
// Copyright (C) 2009-2018 Daniel Carvajal (haplokuon@gmail.com)
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;

namespace netDxf.Objects
{
    /// <summary>
    /// Represents the plot settings of a layout.
    /// </summary>
    public class PlotSettings :
        ICloneable
    {
        #region private fields

        private string pageSetupName;
        private string plotterName;
        private string paperSizeName;
        private string viewName;
        private string currentStyleSheet;

        private PaperMargin paperMargin;
        private Vector2 paperSize;
        private Vector2 origin;
        private Vector2 windowUpRight;
        private Vector2 windowBottomLeft;

        private bool scaleToFit ;
        private double numeratorScale;
        private double denominatorScale;
        private PlotFlags flags;
        private PlotType plotType;

        private PlotPaperUnits paperUnits;
        private PlotRotation rotation;

        private ShadePlotMode shadePlotMode;
        private ShadePlotResolutionMode shadePlotResolutionMode;
        private short shadePlotDPI;
        private Vector2 paperImageOrigin;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of <c>PlotSettings</c>.
        /// </summary>
        public PlotSettings()
        {
            this.pageSetupName = string.Empty;
            this.plotterName = "none_device";
            this.paperSizeName = "ISO_A4_(210.00_x_297.00_MM)";
            this.viewName = string.Empty;
            this.currentStyleSheet = string.Empty;

            this.paperMargin = new PaperMargin(7.5, 20.0, 7.5, 20.0);

            this.paperSize = new Vector2(210.0, 297.0);
            this.origin = Vector2.Zero;
            this.windowUpRight = Vector2.Zero;
            this.windowBottomLeft = Vector2.Zero;

            this.scaleToFit = true;
            this.numeratorScale = 1.0;
            this.denominatorScale = 1.0;
            this.flags = PlotFlags.DrawViewportsFirst | PlotFlags.PrintLineweights | PlotFlags.PlotPlotStyles | PlotFlags.UseStandardScale;
            this.plotType = PlotType.DrawingExtents;

            this.paperUnits = PlotPaperUnits.Milimeters;
            this.rotation = PlotRotation.Degrees90;

            this.shadePlotMode = ShadePlotMode.AsDisplayed;
            this.shadePlotResolutionMode = ShadePlotResolutionMode.Normal;
            this.shadePlotDPI = 300;
            this.paperImageOrigin = Vector2.Zero;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the page setup name.
        /// </summary>
        public string PageSetupName
        {
            get { return this.pageSetupName; }
            set { this.pageSetupName = value; }
        }

        /// <summary>
        /// Gets or sets the name of system printer or plot configuration file.
        /// </summary>
        public string PlotterName
        {
            get { return this.plotterName; }
            set { this.plotterName = value; }
        }

        /// <summary>
        /// Gets or set the paper size name.
        /// </summary>
        public string PaperSizeName
        {
            get { return this.paperSizeName; }
            set { this.paperSizeName = value; }
        }

        /// <summary>
        /// Gets or sets the plot view name.
        /// </summary>
        public string ViewName
        {
            get { return this.viewName; }
            set { this.viewName = value; }
        }

        /// <summary>
        /// Gets or sets the current style sheet name.
        /// </summary>
        public string CurrentStyleSheet
        {
            get { return this.currentStyleSheet; }
            set { this.currentStyleSheet = value; }
        }

        /// <summary>
        /// Gets or set the size, in millimeters, of unprintable margins of paper.
        /// </summary>
        public PaperMargin PaperMargin
        {
            get { return this.paperMargin; }
            set { this.paperMargin = value; }
        }

        /// <summary>
        /// Gets or sets the plot paper size: physical paper width and height in millimeters.
        /// </summary>
        public Vector2 PaperSize
        {
            get { return this.paperSize; }
            set { this.paperSize = value; }
        }

        /// <summary>
        /// Gets or sets the plot origin in millimeters.
        /// </summary>
        public Vector2 Origin
        {
            get { return this.origin; }
            set { this.origin = value; }
        }

        /// <summary>
        /// Gets or sets the plot upper-right window corner.
        /// </summary>
        public Vector2 WindowUpRight
        {
            get { return this.windowUpRight; }
            set { this.windowUpRight = value; }
        }

        /// <summary>
        /// Gets or sets the plot lower-left window corner.
        /// </summary>
        public Vector2 WindowBottomLeft
        {
            get { return this.windowBottomLeft; }
            set { this.windowBottomLeft = value; }
        }

        /// <summary>
        /// Gets or sets if the plot scale will be automatically computed show the drawing fits the media.
        /// </summary>
        /// <remarks>
        /// If <c>ScaleToFit</c> is set to false the values specified by <c>PrintScaleNumerator</c> and <c>PrintScaleDenomiator</c> will be used.
        /// </remarks>
        public bool ScaleToFit
        {
            get { return this.scaleToFit; }
            set { this.scaleToFit = value; }
        }

        /// <summary>
        /// Gets or sets the numerator of custom print scale: real world paper units.
        /// </summary>
        /// <remarks>
        /// The paper units used are specified by the <c>PaperUnits</c> value.
        /// </remarks>
        public double PrintScaleNumerator
        {
            get { return this.numeratorScale; }
            set
            {
                if(value <= 0.0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The print scale numerator must be a number greater than zero.");
                this.numeratorScale = value;
            }
        }

        /// <summary>
        /// Gets or sets the denominator of custom print scale: drawing units.
        /// </summary>
        public double PrintScaleDenominator
        {
            get { return this.denominatorScale; }
            set
            {
                if (value <= 0.0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The print scale denominator must be a number greater than zero.");
                this.denominatorScale = value;
            }
        }

        /// <summary>
        /// Gets the scale factor.
        /// </summary>
        public double PrintScale
        {
            get { return this.numeratorScale / this.denominatorScale; }
        }

        /// <summary>
        /// Gets or sets the plot layout flags.
        /// </summary>
        public PlotFlags Flags
        {
            get { return this.flags; }
            set { this.flags = value; }
        }

        /// <summary>
        /// Gets or sets the portion of paper space to output to the media.
        /// </summary>
        public PlotType PlotType
        {
            get { return this.plotType; }
            set { this.plotType = value; }
        }

        /// <summary>
        /// Gets or sets the paper units.
        /// </summary>
        /// <remarks>This value is only applicable to the scale parameter <c>PrintScaleNumerator</c>.</remarks>
        public PlotPaperUnits PaperUnits
        {
            get { return this.paperUnits; }
            set { this.paperUnits = value; }
        }

        /// <summary>
        /// Gets or sets the paper rotation.
        /// </summary>
        public PlotRotation PaperRotation
        {
            get { return this.rotation; }
            set { this.rotation = value; }
        }

        /// <summary>
        /// Gets or sets the shade plot mode.
        /// </summary>
        public ShadePlotMode ShadePlotMode
        {
            get { return this.shadePlotMode; }
            set { this.shadePlotMode = value; }
        }

        /// <summary>
        /// Gets or sets the plot resolution mode.
        /// </summary>
        /// <remarks>
        /// if the <c>ShadePlotResolutionMode</c> is set to Custom the value specified by the <c>ShadPloDPI</c> will be used.
        /// </remarks>
        public ShadePlotResolutionMode ShadePlotResolutionMode
        {
            get { return this.shadePlotResolutionMode; }
            set { this.shadePlotResolutionMode = value; }
        }

        /// <summary>
        /// Gets or sets the shade plot custom DPI.
        /// </summary>
        public short ShadePlotDPI
        {
            get { return this.shadePlotDPI; }
            set
            {
                if(value <100 || value > 32767)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The valid shade plot DPI values range from 100 to 23767.");
                this.shadePlotDPI = value;
            }
        }

        /// <summary>
        /// Gets or sets the paper image origin.
        /// </summary>
        public Vector2 PaperImageOrigin
        {
            get { return this.paperImageOrigin; }
            set { this.paperImageOrigin = value; }
        }

        #endregion

        #region implements ICloneable

        /// <summary>
        /// Creates a new plot settings that is a copy of the current instance.
        /// </summary>
        /// <returns>A new plot settings that is a copy of this instance.</returns>
        public object Clone()
        {
            return new PlotSettings
            {
                PageSetupName = this.pageSetupName,
                PlotterName = this.plotterName,
                PaperSizeName = this.paperSizeName,
                ViewName = this.viewName,
                CurrentStyleSheet = this.currentStyleSheet,
                PaperMargin = this.PaperMargin,
                PaperSize = this.paperSize,
                Origin = this.origin,
                WindowUpRight = this.windowUpRight,
                WindowBottomLeft = this.windowBottomLeft,
                ScaleToFit = this.scaleToFit,
                PrintScaleNumerator = this.numeratorScale,
                PrintScaleDenominator = this.denominatorScale,
                Flags = this.flags,
                PlotType = this.plotType,
                PaperUnits = this.paperUnits,
                PaperRotation = this.rotation,
                ShadePlotMode = this.shadePlotMode,
                ShadePlotResolutionMode = this.shadePlotResolutionMode,
                ShadePlotDPI = this.shadePlotDPI,
                PaperImageOrigin = this.paperImageOrigin
            };
        }

        #endregion
    }
}