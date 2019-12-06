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
using System.Text;
using netDxf.Tables;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a multiline text <see cref="EntityObject">entity</see>.
    /// </summary>
    /// <remarks>
    /// Formatting codes for MText, you can use them directly while setting the text value or use the Write() method.<br />
    /// \L Start underline<br />
    /// \l Stop underline<br />
    /// \O Start overstrike<br />
    /// \o Stop overstrike<br />
    /// \K Start strike-through<br />
    /// \k Stop strike-through<br />
    /// \P New paragraph (new line)<br />
    /// \pxi Control codes for bullets, numbered paragraphs and columns<br />
    /// \X Paragraph wrap on the dimension line (only in dimensions)<br />
    /// \Q Slanting (obliquing) text by angle - e.g. \Q30;<br />
    /// \H Text height - e.g. \H3x;<br />
    /// \W Text width - e.g. \W0.8x;<br />
    /// \F Font selection<br />
    /// <br />
    /// e.g. \Fgdt;o - GDT-tolerance<br />
    /// e.g. \Fkroeger|b0|i0|c238|p10; - font Kroeger, non-bold, non-italic, code page 238, pitch 10<br />
    /// <br />
    /// \S Stacking, fractions<br />
    /// <br />
    /// e.g. \SA^B;<br />
    /// A<br />
    /// B<br />
    /// e.g. \SX/Y<br />
    /// X<br />
    /// -<br />
    /// Y<br />
    /// e.g. \S1#4;<br />
    /// 1/4<br />
    /// <br />
    /// \A Alignment<br />
    /// \A0; = bottom<br />
    /// \A1; = center<br />
    /// \A2; = top<br />
    /// <br />
    /// \C Color change<br />
    /// \C1; = red<br />
    /// \C2; = yellow<br />
    /// \C3; = green<br />
    /// \C4; = cyan<br />
    /// \C5; = blue<br />
    /// \C6; = magenta<br />
    /// \C7; = white<br />
    /// <br />
    /// \T Tracking, char.spacing - e.g. \T2;<br />
    /// \~ Non-wrapping space, hard space<br />
    /// {} Braces - define the text area influenced by the code<br />
    /// \ Escape character - e.g. \\ = "\", \{ = "{"<br />
    /// <br />
    /// Codes and braces can be nested up to 8 levels deep.<br />
    /// </remarks>
    public class MText :
        EntityObject
    {
        #region delegates and events

        public delegate void TextStyleChangedEventHandler(MText sender, TableObjectChangedEventArgs<TextStyle> e);

        public event TextStyleChangedEventHandler TextStyleChanged;

        protected virtual TextStyle OnTextStyleChangedEvent(TextStyle oldTextStyle, TextStyle newTextStyle)
        {
            TextStyleChangedEventHandler ae = this.TextStyleChanged;
            if (ae != null)
            {
                TableObjectChangedEventArgs<TextStyle> eventArgs = new TableObjectChangedEventArgs<TextStyle>(oldTextStyle, newTextStyle);
                ae(this, eventArgs);
                return eventArgs.NewValue;
            }
            return newTextStyle;
        }

        #endregion

        #region private fields

        private Vector3 position;
        private double rectangleWidth;
        private double height;
        private double rotation;
        private double lineSpacing;
        private double paragraphHeightFactor;
        private MTextLineSpacingStyle lineSpacingStyle;
        private MTextDrawingDirection drawingDirection;
        private MTextAttachmentPoint attachmentPoint;
        private TextStyle style;
        private string text;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>MText</c> class.
        /// </summary>
        public MText()
            : this(string.Empty, Vector3.Zero, 1.0, 0.0, TextStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>MText</c> class.
        /// </summary>
        /// <param name="text">Text string.</param>
        public MText(string text)
            : this(text, Vector3.Zero, 1.0, 0.0, TextStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>MText</c> class.
        /// </summary>
        /// <param name="position">Text <see cref="Vector2">position</see> in world coordinates.</param>
        /// <param name="height">Text height.</param>
        /// <param name="rectangleWidth">Reference rectangle width.</param>
        public MText(Vector3 position, double height, double rectangleWidth)
            : this(string.Empty, position, height, rectangleWidth, TextStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>MText</c> class.
        /// </summary>
        /// <param name="position">Text <see cref="Vector2">position</see> in world coordinates.</param>
        /// <param name="height">Text height.</param>
        /// <param name="rectangleWidth">Reference rectangle width.</param>
        public MText(Vector2 position, double height, double rectangleWidth)
            : this(string.Empty, new Vector3(position.X, position.Y, 0.0), height, rectangleWidth, TextStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>MText</c> class.
        /// </summary>
        /// <param name="position">Text <see cref="Vector2">position</see> in world coordinates.</param>
        /// <param name="height">Text height.</param>
        /// <param name="rectangleWidth">Reference rectangle width.</param>
        /// <param name="style">Text <see cref="TextStyle">style</see>.</param>
        public MText(Vector3 position, double height, double rectangleWidth, TextStyle style)
            : this(string.Empty, position, height, rectangleWidth, style)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>MText</c> class.
        /// </summary>
        /// <param name="position">Text <see cref="Vector2">position</see> in world coordinates.</param>
        /// <param name="height">Text height.</param>
        /// <param name="rectangleWidth">Reference rectangle width.</param>
        /// <param name="style">Text <see cref="TextStyle">style</see>.</param>
        public MText(Vector2 position, double height, double rectangleWidth, TextStyle style)
            : this(string.Empty, new Vector3(position.X, position.Y, 0.0), height, rectangleWidth, style)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>MText</c> class.
        /// </summary>
        /// <param name="text">Text string.</param>
        /// <param name="position">Text <see cref="Vector2">position</see> in world coordinates.</param>
        /// <param name="height">Text height.</param>
        /// <param name="rectangleWidth">Reference rectangle width.</param>
        /// <param name="style">Text <see cref="TextStyle">style</see>.</param>
        public MText(string text, Vector2 position, double height, double rectangleWidth, TextStyle style)
            : this(text, new Vector3(position.X, position.Y, 0.0), height, rectangleWidth, style)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>MText</c> class.
        /// </summary>
        /// <param name="text">Text string.</param>
        /// <param name="position">Text <see cref="Vector2">position</see> in world coordinates.</param>
        /// <param name="height">Text height.</param>
        /// <param name="rectangleWidth">Reference rectangle width.</param>
        public MText(string text, Vector2 position, double height, double rectangleWidth)
            : this(text, new Vector3(position.X, position.Y, 0.0), height, rectangleWidth, TextStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>MText</c> class.
        /// </summary>
        /// <param name="text">Text string.</param>
        /// <param name="position">Text <see cref="Vector2">position</see> in world coordinates.</param>
        /// <param name="height">Text height.</param>
        /// <param name="rectangleWidth">Reference rectangle width.</param>
        public MText(string text, Vector3 position, double height, double rectangleWidth)
            : this(text, position, height, rectangleWidth, TextStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>MText</c> class.
        /// </summary>
        /// <param name="text">Text string.</param>
        /// <param name="position">Text <see cref="Vector2">position</see> in world coordinates.</param>
        /// <param name="height">Text height.</param>
        /// <param name="rectangleWidth">Reference rectangle width.</param>
        /// <param name="style">Text <see cref="TextStyle">style</see>.</param>
        public MText(string text, Vector3 position, double height, double rectangleWidth, TextStyle style)
            : base(EntityType.MText, DxfObjectCode.MText)
        {
            this.text = text;
            this.position = position;
            this.attachmentPoint = MTextAttachmentPoint.TopLeft;
            if (style == null)
                throw new ArgumentNullException(nameof(style));
            this.style = style;
            this.rectangleWidth = rectangleWidth;
            if (height <= 0.0)
                throw new ArgumentOutOfRangeException(nameof(height), this.text, "The MText height must be greater than zero.");
            this.height = height;
            this.lineSpacing = 1.0;
            this.paragraphHeightFactor = 1.0;
            this.lineSpacingStyle = MTextLineSpacingStyle.AtLeast;
            this.drawingDirection = MTextDrawingDirection.ByStyle;
            this.rotation = 0.0;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the text rotation in degrees.
        /// </summary>
        public double Rotation
        {
            get { return this.rotation; }
            set { this.rotation = MathHelper.NormalizeAngle(value); }
        }

        /// <summary>
        /// Gets or sets the text height.
        /// </summary>
        public double Height
        {
            get { return this.height; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The MText height must be greater than zero.");
                this.height = value;
            }
        }

        /// <summary>
        /// Gets or sets the line spacing factor.
        /// </summary>
        /// <remarks>
        /// Percentage of default line spacing to be applied. Valid values range from 0.25 to 4.00, the default value 1.0.
        /// </remarks>
        public double LineSpacingFactor
        {
            get { return this.lineSpacing; }
            set
            {
                if (value < 0.25 || value > 4.0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The MText LineSpacingFactor valid values range from 0.25 to 4.00");
                this.lineSpacing = value;
            }
        }

        /// <summary>
        /// Gets or sets the paragraph height factor.
        /// </summary>
        /// <remarks>
        /// Percentage of default paragraph height factor to be applied. Valid values range from 0.25 to 4.00, the default value 1.0.
        /// </remarks>
        public double ParagraphHeightFactor
        {
            get { return this.paragraphHeightFactor; }
            set
            {
                if (value < 0.25 || value > 4.0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The MText ParagraphHeightFactor valid values range from 0.25 to 4.00");
                this.paragraphHeightFactor = value;
            }
        }

        /// <summary>
        /// Get or sets the <see cref="MTextLineSpacingStyle">line spacing style</see>.
        /// </summary>
        public MTextLineSpacingStyle LineSpacingStyle
        {
            get { return this.lineSpacingStyle; }
            set { this.lineSpacingStyle = value; }
        }

        /// <summary>
        /// Get or sets the <see cref="MTextDrawingDirection">text drawing direction</see>.
        /// </summary>
        public MTextDrawingDirection DrawingDirection
        {
            get { return this.drawingDirection; }
            set { this.drawingDirection = value; }
        }

        /// <summary>
        /// Gets or sets the text reference rectangle width.
        /// </summary>
        /// <remarks>
        /// This value defines the width of the box where the text will fit.<br/>
        /// If a paragraph width is longer than the rectangle width it will be broken in several lines, using the word spaces as breaking points.<br/>
        /// If you specify a width of 0, word wrap is turned off and the width of the multiline text object is as wide as the longest line of text.
        ///  </remarks>
        public double RectangleWidth
        {
            get { return this.rectangleWidth; }
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The MText rectangle width must be equals or greater than zero.");
                this.rectangleWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the text <see cref="MTextAttachmentPoint">attachment point</see>.
        /// </summary>
        public MTextAttachmentPoint AttachmentPoint
        {
            get { return this.attachmentPoint; }
            set { this.attachmentPoint = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="TextStyle">text style</see>.
        /// </summary>
        public TextStyle Style
        {
            get { return this.style; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.style = this.OnTextStyleChangedEvent(this.style, value);
            }
        }

        /// <summary>
        /// Gets or sets the Text <see cref="Vector3">position</see> in world coordinates.
        /// </summary>
        public Vector3 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        /// <summary>
        /// Gets or sets the raw text string.
        /// </summary>
        public string Value
        {
            get { return this.text; }
            set { this.text = value; }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Adds the text to the existing paragraph. 
        /// </summary>
        /// <param name="txt">Text string.</param>
        public void Write(string txt)
        {
            this.Write(txt, null);
        }

        /// <summary>
        /// Adds the text to the existing paragraph. 
        /// </summary>
        /// <param name="txt">Text string.</param>
        /// <param name="options">Text formatting options.</param>
        public void Write(string txt, MTextFormattingOptions options)
        {
            if (options == null)
                this.text += txt;
            else
                this.text += options.FormatText(txt);
        }

        /// <summary>
        /// Ends the actual paragraph (adds the end paragraph code and the paragraph height factor). 
        /// </summary>
        public void EndParagraph()
        {
            if (!MathHelper.IsOne(this.paragraphHeightFactor))
                this.text += "{\\H" + this.paragraphHeightFactor + "x;}\\P";
            else
                this.text += "\\P";
        }

        /// <summary>
        /// Obtains the MText text value without the formatting codes, control characters like tab '\t' will be preserved in the result,
        /// the new paragraph command "\P" will be converted to new line feed '\r\n'.
        /// </summary>
        /// <returns>MText text value without the formatting codes.</returns>
        public string PlainText()
        {
            if (string.IsNullOrEmpty(this.text))
                return string.Empty;

            string txt = this.text;

            //text = text.Replace("%%c", "Ø");
            //text = text.Replace("%%d", "°");
            //text = text.Replace("%%p", "±");

            StringBuilder rawText = new StringBuilder();
            CharEnumerator chars = txt.GetEnumerator();

            while (chars.MoveNext())
            {
                char token = chars.Current;
                if (token == '\\') // is a formatting command
                {
                    if (chars.MoveNext())
                        token = chars.Current;
                    else
                        return rawText.ToString(); // premature end of text

                    if (token == '\\' | token == '{' | token == '}') // escape chars
                        rawText.Append(token);
                    else if (token == 'L' | token == 'l' | token == 'O' | token == 'o' | token == 'K' | token == 'k' | token == 'P' | token == 'X') // one char commands
                        if (token == 'P')
                            rawText.Append(Environment.NewLine);
                        else
                        {
                        } // discard other commands
                    else // formatting commands of more than one character always terminate in ';'
                    {
                        bool stacking = token == 'S'; // we want to preserve the text under the stacking command
                        while (token != ';')
                        {
                            if (chars.MoveNext())
                                token = chars.Current;
                            else
                                return rawText.ToString(); // premature end of text

                            if (stacking && token != ';')
                                rawText.Append(token); // append user data of stacking command
                        }
                    }
                }
                else if (token == '{' | token == '}')
                {
                    // discard group markers
                }
                else // char is what it is, a character
                    rawText.Append(token);
            }
            return rawText.ToString();
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new MText that is a copy of the current instance.
        /// </summary>
        /// <returns>A new MText that is a copy of this instance.</returns>
        public override object Clone()
        {
            MText entity = new MText
            {
                //EntityObject properties
                Layer = (Layer) this.Layer.Clone(),
                Linetype = (Linetype) this.Linetype.Clone(),
                Color = (AciColor) this.Color.Clone(),
                Lineweight = this.Lineweight,
                Transparency = (Transparency) this.Transparency.Clone(),
                LinetypeScale = this.LinetypeScale,
                Normal = this.Normal,
                IsVisible = this.IsVisible,
                //MText properties
                Position = this.position,
                Rotation = this.rotation,
                Height = this.height,
                LineSpacingFactor = this.lineSpacing,
                ParagraphHeightFactor = this.paragraphHeightFactor,
                LineSpacingStyle = this.lineSpacingStyle,
                RectangleWidth = this.rectangleWidth,
                AttachmentPoint = this.attachmentPoint,
                Style = (TextStyle) this.style.Clone(),
                Value = this.text
            };

            foreach (XData data in this.XData.Values)
                entity.XData.Add((XData) data.Clone());

            return entity;
        }

        #endregion
    }
}