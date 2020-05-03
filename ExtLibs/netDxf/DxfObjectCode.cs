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

namespace netDxf
{
    /// <summary>
    /// Dxf string codes.
    /// </summary>
    public static class DxfObjectCode
    {
        /// <summary>
        /// Not defined.
        /// </summary>
        public const string Unknown = "";

        /// <summary>
        /// Header section.
        /// </summary>
        public const string HeaderSection = "HEADER";

        /// <summary>
        /// Classes section.
        /// </summary>
        public const string ClassesSection = "CLASSES";

        /// <summary>
        /// Class entry.
        /// </summary>
        public const string Class = "CLASS";

        /// <summary>
        /// Tables section.
        /// </summary>
        public const string TablesSection = "TABLES";

        /// <summary>
        /// Blocks section.
        /// </summary>
        public const string BlocksSection = "BLOCKS";

        /// <summary>
        /// Entities section.
        /// </summary>
        public const string EntitiesSection = "ENTITIES";

        /// <summary>
        /// Objects section.
        /// </summary>
        public const string ObjectsSection = "OBJECTS";

        /// <summary>
        /// Thumbnail section.
        /// </summary>
        public const string ThumbnailImageSection = "THUMBNAILIMAGE";

        /// <summary>
        /// AcdsData section. Currently it is used for storing the data for solids, regions, surfaces, and the preview image.
        /// </summary>
        public const string AcdsDataSection = "ACDSDATA";

        /// <summary>
        /// Begin section code.
        /// </summary>
        public const string BeginSection = "SECTION";

        /// <summary>
        /// End section code.
        /// </summary>
        public const string EndSection = "ENDSEC";

        /// <summary>
        /// Layers table.
        /// </summary>
        public const string LayerTable = "LAYER";

        /// <summary>
        /// Viewports table.
        /// </summary>
        public const string VportTable = "VPORT";

        /// <summary>
        /// Views table.
        /// </summary>
        public const string ViewTable = "VIEW";

        /// <summary>
        /// User coordinate system table.
        /// </summary>
        public const string UcsTable = "UCS";

        /// <summary>
        /// Block records table.
        /// </summary>
        public const string BlockRecordTable = "BLOCK_RECORD";

        /// <summary>
        /// Line types table.
        /// </summary>
        public const string LinetypeTable = "LTYPE";

        /// <summary>
        /// Text styles table.
        /// </summary>
        public const string TextStyleTable = "STYLE";

        /// <summary>
        /// Dimension styles table.
        /// </summary>
        public const string DimensionStyleTable = "DIMSTYLE";

        /// <summary>
        /// Extended data application registries  table.
        /// </summary>
        public const string ApplicationIdTable = "APPID";

        /// <summary>
        /// Begin table code.
        /// </summary>
        public const string Table = "TABLE";

        /// <summary>
        /// End table code.
        /// </summary>
        public const string EndTable = "ENDTAB";

        /// <summary>
        /// Begin block code.
        /// </summary>
        public const string BeginBlock = "BLOCK";

        /// <summary>
        /// End block code.
        /// </summary>
        public const string EndBlock = "ENDBLK";

        /// <summary>
        /// Group dictionary.
        /// </summary>
        public const string GroupDictionary = "ACAD_GROUP";

        /// <summary>
        /// Layouts dictionary.
        /// </summary>
        public const string LayoutDictionary = "ACAD_LAYOUT";

        /// <summary>
        /// Multiline styles dictionary.
        /// </summary>
        public const string MLineStyleDictionary = "ACAD_MLINESTYLE";

        /// <summary>
        /// Multiline styles dictionary.
        /// </summary>
        public const string ImageDefDictionary = "ACAD_IMAGE_DICT";

        /// <summary>
        /// MLine styles dictionary.
        /// </summary>
        public const string ImageVarsDictionary = "ACAD_IMAGE_VARS";

        /// <summary>
        /// DGN underlay definition dictionary.
        /// </summary>
        public const string UnderlayDgnDefinitionDictionary = "ACAD_DGNDEFINITIONS";

        /// <summary>
        /// DWF underlay definition styles dictionary.
        /// </summary>
        public const string UnderlayDwfDefinitionDictionary = "ACAD_DWFDEFINITIONS";

        /// <summary>
        /// PDF underlay definition styles dictionary.
        /// </summary>
        public const string UnderlayPdfDefinitionDictionary = "ACAD_PDFDEFINITIONS";

        /// <summary>
        /// End of file.
        /// </summary>
        public const string EndOfFile = "EOF";

        /// <summary>
        /// Application registry.
        /// </summary>
        public const string AppId = "APPID";

        /// <summary>
        /// Dimension style.
        /// </summary>
        public const string DimStyle = "DIMSTYLE";

        /// <summary>
        /// Block record.
        /// </summary>
        public const string BlockRecord = "BLOCK_RECORD";

        /// <summary>
        /// Line type.
        /// </summary>
        public const string Linetype = "LTYPE";

        /// <summary>
        /// Layer.
        /// </summary>
        public const string Layer = "LAYER";

        /// <summary>
        /// Viewport table object.
        /// </summary>
        public const string VPort = "VPORT";

        /// <summary>
        /// Text style.
        /// </summary>
        public const string TextStyle = "STYLE";

        /// <summary>
        /// Multiline style.
        /// </summary>
        public const string MLineStyle = "MLINESTYLE";

        /// <summary>
        /// View.
        /// </summary>
        public const string View = "VIEW";

        /// <summary>
        /// User coordinate system.
        /// </summary>
        public const string Ucs = "UCS";

        /// <summary>
        /// Block.
        /// </summary>
        public const string Block = "BLOCK";

        /// <summary>
        /// End block.
        /// </summary>
        public const string BlockEnd = "ENDBLK";

        /// <summary>
        /// Line.
        /// </summary>
        public const string Line = "LINE";

        /// <summary>
        /// Ray.
        /// </summary>
        public const string Ray = "RAY";

        /// <summary>
        /// XLine.
        /// </summary>
        public const string XLine = "XLINE";

        /// <summary>
        /// Ellipse.
        /// </summary>
        public const string Ellipse = "ELLIPSE";

        /// <summary>
        /// Polyline.
        /// </summary>
        public const string Polyline = "POLYLINE";

        /// <summary>
        /// Lightweight polyline.
        /// </summary>
        public const string LightWeightPolyline = "LWPOLYLINE";

        /// <summary>
        /// Circle.
        /// </summary>
        public const string Circle = "CIRCLE";

        /// <summary>
        /// Point.
        /// </summary>
        public const string Point = "POINT";

        /// <summary>
        /// Arc.
        /// </summary>
        public const string Arc = "ARC";

        /// <summary>
        /// Shape
        /// </summary>
        public const string Shape = "SHAPE";

        /// <summary>
        /// Spline (nonuniform rational B-splines NURBS).
        /// </summary>
        public const string Spline = "SPLINE";

        /// <summary>
        /// Solid.
        /// </summary>
        public const string Solid = "SOLID";

        /// <summary>
        /// Table made of rows and columns.
        /// </summary>
        public const string AcadTable = "ACAD_TABLE";

        /// <summary>
        /// Trace.
        /// </summary>
        public const string Trace = "TRACE";

        /// <summary>
        /// Text string.
        /// </summary>
        public const string Text = "TEXT";

        /// <summary>
        /// Mesh.
        /// </summary>
        public const string Mesh = "MESH";

        /// <summary>
        /// Multiline text string.
        /// </summary>
        public const string MText = "MTEXT";

        /// <summary>
        /// MLine.
        /// </summary>
        public const string MLine = "MLINE";

        /// <summary>
        /// 3d face.
        /// </summary>
        public const string Face3d = "3DFACE";

        /// <summary>
        /// Block insertion.
        /// </summary>
        public const string Insert = "INSERT";

        /// <summary>
        /// Hatch.
        /// </summary>
        public const string Hatch = "HATCH";

        /// <summary>
        /// Leader.
        /// </summary>
        public const string Leader = "LEADER";

        /// <summary>
        /// Tolerance.
        /// </summary>
        public const string Tolerance = "TOLERANCE";

        /// <summary>
        /// Wipeout.
        /// </summary>
        public const string Wipeout = "WIPEOUT";

        /// <summary>
        /// Underlay.
        /// </summary>
        public const string Underlay = "UNDERLAY";

        /// <summary>
        /// PDF underlay.
        /// </summary>
        public const string UnderlayPdf = "PDFUNDERLAY";

        /// <summary>
        /// DWF underlay.
        /// </summary>
        public const string UnderlayDwf = "DWFUNDERLAY";

        /// <summary>
        /// DGN underlay.
        /// </summary>
        public const string UnderlayDgn = "DGNUNDERLAY";

        /// <summary>
        /// Underlay definition.
        /// </summary>
        public const string UnderlayDefinition = "UNDERLAYDEFINITION";

        /// <summary>
        /// PDF underlay definition.
        /// </summary>
        public const string UnderlayPdfDefinition = "PDFDEFINITION";

        /// <summary>
        /// DWF underlay definition.
        /// </summary>
        public const string UnderlayDwfDefinition = "DWFDEFINITION";

        /// <summary>
        /// DGN underlay definition.
        /// </summary>
        public const string UnderlayDgnDefinition = "DGNDEFINITION";

        /// <summary>
        /// Attribute definition.
        /// </summary>
        public const string AttributeDefinition = "ATTDEF";

        /// <summary>
        /// Attribute.
        /// </summary>
        public const string Attribute = "ATTRIB";

        /// <summary>
        /// Vertex.
        /// </summary>
        public const string Vertex = "VERTEX";

        /// <summary>
        /// End sequence.
        /// </summary>
        public const string EndSequence = "SEQEND";

        /// <summary>
        /// Dimension.
        /// </summary>
        public const string Dimension = "DIMENSION";

        /// <summary>
        /// Dictionary.
        /// </summary>
        public const string Dictionary = "DICTIONARY";

        /// <summary>
        /// Raster image.
        /// </summary>
        public const string Image = "IMAGE";

        /// <summary>
        /// Viewport entity.
        /// </summary>
        public const string Viewport = "VIEWPORT";

        /// <summary>
        /// Image definition.
        /// </summary>
        public const string ImageDef = "IMAGEDEF";

        /// <summary>
        /// Image definition reactor.
        /// </summary>
        public const string ImageDefReactor = "IMAGEDEF_REACTOR";

        /// <summary>
        /// Raster variables.
        /// </summary>
        public const string RasterVariables = "RASTERVARIABLES";

        /// <summary>
        /// Groups.
        /// </summary>
        public const string Group = "GROUP";

        /// <summary>
        /// Layouts.
        /// </summary>
        public const string Layout = "LAYOUT";
    }
}