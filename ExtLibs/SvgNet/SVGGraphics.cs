/*
    Copyright © 2003 RiskCare Ltd. All rights reserved.
    Copyright © 2010 SvgNet & SvgGdi Bridge Project. All rights reserved.
    Copyright © 2015, 2017 Rafael Teixeira, Mojmír Němeček, Benjamin Peterson and Other Contributors

    Original source code licensed with BSD-2-Clause spirit, treat it thus, see accompanied LICENSE for more
*/

using SvgNet.SvgElements;
using SvgNet.SvgTypes;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace SvgNet.SvgGdi
{
    namespace MetafileTools
    {
        // Classes in this namespace were inspired by the code in http://wmf.codeplex.com/
        namespace EmfTools
        {
            public interface IBinaryRecord
            {
                void Read(BinaryReader reader);
            }

            public static class BinaryReaderExtensions
            {
                /// <summary>
                /// Skips excess bytes. Work-around for some WMF files that contain undocumented fields.
                /// </summary>
                /// <param name="reader"></param>
                /// <param name="excess"></param>
                public static void Skip(this BinaryReader reader, int excess)
                {
                    if (excess > 0)
                    {
                        //Skip unknown bytes
                        reader.BaseStream.Seek(excess, SeekOrigin.Current);
                        //var dummy = reader.ReadBytes(excess);
                    }
                }
            }

            /// <summary>
            /// Implements a EMF META record
            /// </summary>
            public abstract class EmfBinaryRecord : IBinaryRecord
            {
                /// <summary>
                /// Gets or sets record length
                /// </summary>
                public uint RecordSize
                {
                    get;
                    set;
                }

                /// <summary>
                /// Gets or sets record type (aka RecordFunction)
                /// </summary>
                public EmfPlusRecordType RecordType
                {
                    get;
                    set;
                }

                /// <summary>
                /// Reads a record from binary stream. If this method is not overridden it will skip this record and go to next record.
                /// NOTE: When overriding this method remove the base.Read(reader) line from code.
                /// </summary>
                /// <param name="reader"></param>
                public virtual void Read(BinaryReader reader)
                {
                }

                protected EmfBinaryRecord()
                {
                }
            }

            [Serializable]
            public class EmfException : Exception
            {
                public EmfException(string message)
                    : base(message)
                {
                }
            }

            /// <summary>
            /// Low-level EMF parser
            /// </summary>
            public class EmfReader : IDisposable
            {
                public EmfReader(Stream stream)
                {
                    this.stream = stream;
                    this.reader = new BinaryReader(stream);
                }

                public bool IsEndOfFile
                {
                    get { return stream.Length == stream.Position; }
                }

                public void Dispose()
                {
                    if (this.reader != null)
                    {
                        this.reader.Close();
                        this.reader = null;
                    }
                }

                public IBinaryRecord Read()
                {
                    long begin = reader.BaseStream.Position;

                    var rt = (EmfPlusRecordType)reader.ReadUInt32();
                    var recordSize = reader.ReadUInt32();

                    EmfBinaryRecord record = new EmfUnknownRecord
                    {
                        RecordType = rt,
                        RecordSize = recordSize
                    };
                    record.Read(reader);

                    long end = reader.BaseStream.Position;
                    long rlen = end - begin; //Read length
                    long excess = recordSize - rlen;
                    if (excess > 0)
                    {
                        //Oops, reader did not read whole record?!
                        reader.Skip((int)excess);
                    }

                    return record;
                }

                private readonly Stream stream;
                private BinaryReader reader;
            }

            public class EmfUnknownRecord : EmfBinaryRecord
            {
                public byte[] Data
                {
                    get;
                    set;
                }

                public override void Read(BinaryReader reader)
                {
                    var length = (int)base.RecordSize - sizeof(UInt32) - sizeof(UInt32);
                    if (length > 0)
                    {
                        this.Data = reader.ReadBytes(length);
                    }
                    else
                    {
                        this.Data = EmptyData;
                    }
                }

                private static readonly byte[] EmptyData = new byte[0];
            }
        }

        public delegate void DrawLineDelegate(PointF[] points);

        public delegate void FillPolygonDelegate(PointF[] points, Brush brush);

        public class MetafileParser
        {
            public enum EmfBrushStyle
            {
                BS_SOLID = 0x0000,
                BS_NULL = 0x0001,
                BS_HATCHED = 0x0002,
                BS_PATTERN = 0x0003,
                BS_INDEXED = 0x0004,
                BS_DIBPATTERN = 0x0005,
                BS_DIBPATTERNPT = 0x0006,
                BS_PATTERN8X8 = 0x0007,
                BS_DIBPATTERN8X8 = 0x0008,
                BS_MONOPATTERN = 0x0009
            }

            /// <summary>
            /// https://msdn.microsoft.com/en-us/library/cc231191 without the 0x80000000 bit
            /// </summary>
            public enum EmfStockObject
            {
                WHITE_BRUSH = 0x00000000,
                LTGRAY_BRUSH = 0x00000001,
                GRAY_BRUSH = 0x00000002,
                DKGRAY_BRUSH = 0x00000003,
                BLACK_BRUSH = 0x00000004,
                NULL_BRUSH = 0x00000005,
                WHITE_PEN = 0x00000006,
                BLACK_PEN = 0x00000007,
                NULL_PEN = 0x00000008,
                OEM_FIXED_FONT = 0x0000000A,
                ANSI_FIXED_FONT = 0x0000000B,
                ANSI_VAR_FONT = 0x0000000C,
                SYSTEM_FONT = 0x0000000D,
                DEVICE_DEFAULT_FONT = 0x0000000E,
                DEFAULT_PALETTE = 0x0000000F,
                SYSTEM_FIXED_FONT = 0x00000010,
                DEFAULT_GUI_FONT = 0x00000011,
                DC_BRUSH = 0x00000012,
                DC_PEN = 0x00000013,

                MinValue = WHITE_BRUSH,
                MaxValue = DC_PEN
            }

            public enum EmfTransformMode
            {
                MWT_IDENTITY = 1,
                MWT_LEFTMULTIPLY = 2,
                MWT_RIGHTMULTIPLY = 3
            }

            public void EnumerateMetafile(Stream emf, float unitSize, PointF destination, DrawLineDelegate drawLine, FillPolygonDelegate fillPolygon)
            {
                Transform = new Matrix();
                _drawLine = drawLine;
                _fillPolygon = fillPolygon;
                _zero = destination;
                _lineBuffer = new LineBuffer(unitSize);
                _objects = new System.Collections.Generic.Dictionary<uint, ObjectHandle>();
                _brush = null;

                using (var reader = new EmfTools.EmfReader(emf))
                {
                    while (!reader.IsEndOfFile)
                    {
                        var record = reader.Read() as EmfTools.EmfUnknownRecord;
                        if (record == null)
                            continue;

                        switch (record.RecordType)
                        {
                            case EmfPlusRecordType.EmfHeader:
                            case EmfPlusRecordType.EmfEof:
                            case EmfPlusRecordType.EmfSaveDC:
                            case EmfPlusRecordType.EmfDeleteObject:
                            case EmfPlusRecordType.EmfExtCreatePen:
                            case EmfPlusRecordType.EmfCreatePen:
                            case EmfPlusRecordType.EmfRestoreDC:
                            case EmfPlusRecordType.EmfSetIcmMode:
                            case EmfPlusRecordType.EmfSetMiterLimit:
                            case EmfPlusRecordType.EmfSetPolyFillMode:
                                // Harmless records with no significant side-effects on the shape of the drawn outline
                                break;

                            case EmfPlusRecordType.EmfSelectObject:
                                ProcessSelectObject(record.Data);
                                break;

                            case EmfPlusRecordType.EmfCreateBrushIndirect:
                                ProcessCreateBrushIndirect(record.Data);
                                break;

                            case EmfPlusRecordType.EmfBeginPath:
                                ProcessBeginPath(record.Data);
                                break;

                            case EmfPlusRecordType.EmfEndPath:
                                // TODO:
                                break;

                            case EmfPlusRecordType.EmfStrokeAndFillPath:
                                ProcessStrokeAndFillPath(record.Data);
                                break;

                            case EmfPlusRecordType.EmfMoveToEx:
                                ProcessMoveToEx(record.Data);
                                break;

                            case EmfPlusRecordType.EmfModifyWorldTransform:
                                ProcessModifyWorldTransform(record.Data);
                                break;

                            case EmfPlusRecordType.EmfPolygon16:
                                ProcessPolygon16(record.Data);
                                break;

                            case EmfPlusRecordType.EmfPolyPolygon16:
                                ProcessPolyPolygon16(record.Data);
                                break;

                            case EmfPlusRecordType.EmfPolyline16:
                                ProcessPolyline16(record.Data);
                                break;

                            case EmfPlusRecordType.EmfPolylineTo16:
                                ProcessPolylineTo16(record.Data);
                                break;

                            case EmfPlusRecordType.EmfCloseFigure:
                                ProcessCloseFigure(record.Data);
                                break;

                            case EmfPlusRecordType.EmfPolyBezierTo16:
                                ProcessPolyBezierTo16(record.Data);
                                break;

                            default:
                                throw new NotImplementedException();
                        }
                    }
                }

                CommitLine();
            }

            private static readonly uint StockObjectMaxCode = 0x80000000 + (uint)EmfStockObject.MaxValue;
            private static readonly uint StockObjectMinCode = 0x80000000 + (uint)EmfStockObject.MinValue;
            private Brush _brush;
            private PointF _curveOrigin;
            private DrawLineDelegate _drawLine;
            private FillPolygonDelegate _fillPolygon;
            private LineBuffer _lineBuffer;
            private PointF _moveTo;
            private System.Collections.Generic.Dictionary<uint, ObjectHandle> _objects;
            private PointF _zero;
            private Matrix Transform;

            private void CommitLine()
            {
                if (_lineBuffer.IsEmpty)
                    return;

                var linePoints = _lineBuffer.GetPoints();

                _lineBuffer.Clear();

                if (linePoints == null)
                    return;

                for (var i = 0; i < linePoints.Length; i++)
                {
                    linePoints[i].X += _zero.X;
                    linePoints[i].Y += _zero.Y;
                }
                _drawLine(linePoints);
            }

            private void DrawLine(PointF[] points, int offset, int count)
            {
                if (!_lineBuffer.CanAdd(points, offset, count))
                {
                    CommitLine();
                }

                _lineBuffer.Add(points, offset, count);
            }

            private void FillPolygon(PointF[] linePoints, Brush fillBrush)
            {
                if (linePoints == null || fillBrush == null)
                    return;

                for (var i = 0; i < linePoints.Length; i++)
                {
                    linePoints[i].X += _zero.X;
                    linePoints[i].Y += _zero.Y;
                }
                _fillPolygon(linePoints, fillBrush);
            }

            private void InternalProcessPolyline16(uint numberOfPolygons, uint totalNumberOfPoints, int[] numberOfPoints, BinaryReader reader)
            {
                var points = new PointF[totalNumberOfPoints];

                for (var j = 0; j < points.Length; j++)
                {
                    points[j].X = reader.ReadInt16();
                    points[j].Y = reader.ReadInt16();
                }

                Transform.TransformPoints(points);

                var offset = 0;
                for (var i = 0; i < numberOfPolygons; i++)
                {
                    DrawLine(points, offset, numberOfPoints[i]);
                    offset += numberOfPoints[i];
                }
            }

            private void InternalSelectObject(EmfStockObject stockObject)
            {
                switch (stockObject)
                {
                    case EmfStockObject.BLACK_BRUSH:
                        _brush = new SolidBrush(Color.Black);
                        break;

                    case EmfStockObject.DC_BRUSH:
                        throw new NotImplementedException();

                    case EmfStockObject.DKGRAY_BRUSH:
                        _brush = new SolidBrush(Color.DarkGray);
                        break;

                    case EmfStockObject.GRAY_BRUSH:
                        _brush = new SolidBrush(Color.Gray);
                        break;

                    case EmfStockObject.LTGRAY_BRUSH:
                        _brush = new SolidBrush(Color.LightGray);
                        break;

                    case EmfStockObject.NULL_BRUSH:
                        _brush = null;
                        break;

                    case EmfStockObject.WHITE_BRUSH:
                        _brush = new SolidBrush(Color.White);
                        break;
                }
            }

            private void ProcessBeginPath(byte[] recordData)
            {
                MemoryStream _ms = null;
                BinaryReader _br = null;
                try
                {
                    _ms = new MemoryStream(recordData);
                    _br = new BinaryReader(_ms);

                    System.Diagnostics.Debug.Assert(_ms.Position == _ms.Length);

                    // Clear the line buffer so that it can record the path
                    CommitLine();
                }
                finally
                {
                    if (_br != null)
                        _br.Close();
                    if (_ms != null)
                        _ms.Dispose();
                }
            }

            private void ProcessCloseFigure(byte[] recordData)
            {
                MemoryStream _ms = null;
                BinaryReader _br = null;
                try
                {
                    _ms = new MemoryStream(recordData);
                    _br = new BinaryReader(_ms);

                    var points = new PointF[2];

                    points[0].X = _moveTo.X;
                    points[0].Y = _moveTo.Y;
                    points[1].X = _curveOrigin.X;
                    points[1].Y = _curveOrigin.Y;

                    Transform.TransformPoints(points);

                    DrawLine(points, 0, points.Length);

                    System.Diagnostics.Debug.Assert(_ms.Position == _ms.Length);
                }
                finally
                {
                    if (_br != null)
                        _br.Close();
                    if (_ms != null)
                        _ms.Dispose();
                }
            }

            private void ProcessCreateBrushIndirect(byte[] recordData)
            {
                MemoryStream _ms = null;
                BinaryReader _br = null;
                try
                {
                    _ms = new MemoryStream(recordData);
                    _br = new BinaryReader(_ms);

                    var ihBrush = _br.ReadUInt32();

                    // https://msdn.microsoft.com/en-us/library/cc230581.aspx
                    var brushStyle = (EmfBrushStyle)_br.ReadUInt32();
                    var r = _br.ReadByte();
                    var g = _br.ReadByte();
                    var b = _br.ReadByte();
                    var reserved = _br.ReadByte();
                    var brushColor = Color.FromArgb(r, g, b);
                    var brushHatch = _br.ReadUInt32();

                    _objects.Remove(ihBrush);

                    switch (brushStyle)
                    {
                        case EmfBrushStyle.BS_SOLID:
                            _objects.Add(ihBrush, new ObjectHandle(new SolidBrush(brushColor)));
                            break;

                        case EmfBrushStyle.BS_NULL:
                            _objects.Add(ihBrush, new ObjectHandle(EmfStockObject.NULL_BRUSH));
                            break;

                        case EmfBrushStyle.BS_HATCHED:
                            throw new NotImplementedException();

                        default:
                            throw new NotImplementedException();
                    }

                    System.Diagnostics.Debug.Assert(_ms.Position == _ms.Length);
                }
                finally
                {
                    if (_br != null)
                        _br.Close();
                    if (_ms != null)
                        _ms.Dispose();
                }
            }

            private void ProcessModifyWorldTransform(byte[] recordData)
            {
                MemoryStream _ms = null;
                BinaryReader _br = null;
                try
                {
                    _ms = new MemoryStream(recordData);
                    _br = new BinaryReader(_ms);

                    var eM11 = _br.ReadSingle();
                    var eM12 = _br.ReadSingle();
                    var eM21 = _br.ReadSingle();
                    var eM22 = _br.ReadSingle();
                    var eDx = _br.ReadSingle();
                    var eDy = _br.ReadSingle();
                    var iMode = (EmfTransformMode)_br.ReadInt32();

                    var matrix = new Matrix(eM11, eM12, eM21, eM22, eDx, eDy);

                    switch (iMode)
                    {
                        case EmfTransformMode.MWT_IDENTITY:
                            Transform = new Matrix();
                            break;

                        case EmfTransformMode.MWT_LEFTMULTIPLY:
                            Transform.Multiply(matrix, MatrixOrder.Append /* TODO: is it the correct order? */);
                            break;

                        case EmfTransformMode.MWT_RIGHTMULTIPLY:
                            Transform.Multiply(matrix, MatrixOrder.Prepend /* TODO: is it the correct order? */);
                            break;

                        default:
                            throw new NotImplementedException();
                    }

                    System.Diagnostics.Debug.Assert(_ms.Position == _ms.Length);
                }
                finally
                {
                    if (_br != null)
                        _br.Close();
                    if (_ms != null)
                        _ms.Dispose();
                }
            }

            private void ProcessMoveToEx(byte[] recordData)
            {
                MemoryStream _ms = null;
                BinaryReader _br = null;
                try
                {
                    _ms = new MemoryStream(recordData);
                    _br = new BinaryReader(_ms);

                    _moveTo = new PointF
                    {
                        X = _br.ReadInt32(),
                        Y = _br.ReadInt32()
                    };

                    _curveOrigin = _moveTo;

                    System.Diagnostics.Debug.Assert(_ms.Position == _ms.Length);
                }
                finally
                {
                    if (_br != null)
                        _br.Close();
                    if (_ms != null)
                        _ms.Dispose();
                }
            }

            private void ProcessPolyBezierTo16(byte[] recordData)
            {
                MemoryStream _ms = null;
                BinaryReader _br = null;
                try
                {
                    _ms = new MemoryStream(recordData);
                    _br = new BinaryReader(_ms);

                    _br.ReadBytes(16 /* Bounds */);

                    var totalNumberOfPoints = _br.ReadUInt32();

                    var originalPoints = new PointF[totalNumberOfPoints];

                    for (var j = 0; j < originalPoints.Length; j++)
                    {
                        originalPoints[j].X = _br.ReadInt16();
                        originalPoints[j].Y = _br.ReadInt16();
                    }

                    const int PointsPerCurve = 3;

                    var numberOfCurves = totalNumberOfPoints / PointsPerCurve;

                    var points = new PointF[1 + numberOfCurves];

                    // Clone _moveTo cursor
                    points[0].X = _moveTo.X;
                    points[0].Y = _moveTo.Y;

                    for (var j = 1; j < points.Length; j++)
                    {
                        // Every curve is defined by 3 points. The first two are the Bezier curve's control points.
                        // The 3rd is the endpoint. This is the point we'll use (only)
                        points[j] = originalPoints[((j - 1) * PointsPerCurve) + (PointsPerCurve - 1)];
                    }

                    // Clone last point to the current _moveTo cursor
                    _moveTo = new PointF(points[points.Length - 1].X, points[points.Length - 1].Y);

                    Transform.TransformPoints(points);

                    DrawLine(points, 0, points.Length);

                    System.Diagnostics.Debug.Assert(_ms.Position == _ms.Length);
                }
                finally
                {
                    if (_br != null)
                        _br.Close();
                    if (_ms != null)
                        _ms.Dispose();
                }
            }

            private void ProcessPolygon16(byte[] recordData)
            {
                MemoryStream _ms = null;
                BinaryReader _br = null;
                try
                {
                    _ms = new MemoryStream(recordData);
                    _br = new BinaryReader(_ms);

                    _br.ReadBytes(16 /* Bounds */);

                    var totalNumberOfPoints = _br.ReadUInt32();

                    var numberOfPoints = new int[1];
                    numberOfPoints[0] = (int)totalNumberOfPoints;

                    InternalProcessPolyline16(1, totalNumberOfPoints, numberOfPoints, _br);

                    System.Diagnostics.Debug.Assert(_ms.Position == _ms.Length);
                }
                finally
                {
                    if (_br != null)
                        _br.Close();
                    if (_ms != null)
                        _ms.Dispose();
                }
            }

            private void ProcessPolyline16(byte[] recordData)
            {
                MemoryStream _ms = null;
                BinaryReader _br = null;
                try
                {
                    _ms = new MemoryStream(recordData);
                    _br = new BinaryReader(_ms);

                    _br.ReadBytes(16 /* Bounds */);

                    const uint numberOfPolygons = 1u;
                    var totalNumberOfPoints = _br.ReadUInt32();

                    var numberOfPoints = new int[numberOfPolygons];
                    numberOfPoints[0] = (int)totalNumberOfPoints;

                    InternalProcessPolyline16(numberOfPolygons, totalNumberOfPoints, numberOfPoints, _br);

                    System.Diagnostics.Debug.Assert(_ms.Position == _ms.Length);
                }
                finally
                {
                    if (_br != null)
                        _br.Close();
                    if (_ms != null)
                        _ms.Dispose();
                }
            }

            private void ProcessPolylineTo16(byte[] recordData)
            {
                MemoryStream _ms = null;
                BinaryReader _br = null;
                try
                {
                    _ms = new MemoryStream(recordData);
                    _br = new BinaryReader(_ms);

                    _br.ReadBytes(16 /* Bounds */);

                    var totalNumberOfPoints = _br.ReadUInt32();

                    var points = new PointF[1 + totalNumberOfPoints];

                    // Clone _moveTo cursor
                    points[0].X = _moveTo.X;
                    points[0].Y = _moveTo.Y;

                    for (var j = 1; j < points.Length; j++)
                    {
                        points[j].X = _br.ReadInt16();
                        points[j].Y = _br.ReadInt16();
                    }

                    // Clone last point to the current _moveTo cursor
                    _moveTo = new PointF(points[points.Length - 1].X, points[points.Length - 1].Y);

                    Transform.TransformPoints(points);

                    DrawLine(points, 0, points.Length);

                    System.Diagnostics.Debug.Assert(_ms.Position == _ms.Length);
                }
                finally
                {
                    if (_br != null)
                        _br.Close();
                    if (_ms != null)
                        _ms.Dispose();
                }
            }

            private void ProcessPolyPolygon16(byte[] recordData)
            {
                MemoryStream _ms = null;
                BinaryReader _br = null;
                try
                {
                    _ms = new MemoryStream(recordData);
                    _br = new BinaryReader(_ms);

                    _br.ReadBytes(16 /* Bounds */);

                    var numberOfPolygons = _br.ReadUInt32();
                    var totalNumberOfPoints = _br.ReadUInt32();

                    var numberOfPoints = new int[numberOfPolygons];

                    for (var i = 0; i < numberOfPolygons; i++)
                        numberOfPoints[i] = (int)_br.ReadUInt32();

                    InternalProcessPolyline16(numberOfPolygons, totalNumberOfPoints, numberOfPoints, _br);

                    System.Diagnostics.Debug.Assert(_ms.Position == _ms.Length);
                }
                finally
                {
                    if (_br != null)
                        _br.Close();
                    if (_ms != null)
                        _ms.Dispose();
                }
            }

            private void ProcessSelectObject(byte[] recordData)
            {
                MemoryStream _ms = null;
                BinaryReader _br = null;
                try
                {
                    _ms = new MemoryStream(recordData);
                    _br = new BinaryReader(_ms);

                    var ihObject = _br.ReadUInt32();

                    if (ihObject >= StockObjectMinCode && ihObject <= StockObjectMaxCode)
                    {
                        var stockObject = (EmfStockObject)(ihObject - StockObjectMinCode + (int)EmfStockObject.MinValue);
                        InternalSelectObject(stockObject);
                    }
                    else
                    {
                        ObjectHandle objectHandle;
                        if (_objects.TryGetValue(ihObject, out objectHandle))
                        {
                            if (objectHandle.IsStockObject)
                            {
                                InternalSelectObject(objectHandle.GetStockObject());
                            }
                            else if (objectHandle.IsBrush)
                            {
                                _brush = objectHandle.GetBrush();
                            }
                        }
                    }

                    System.Diagnostics.Debug.Assert(_ms.Position == _ms.Length);
                }
                finally
                {
                    if (_br != null)
                        _br.Close();
                    if (_ms != null)
                        _ms.Dispose();
                }
            }

            private void ProcessStrokeAndFillPath(byte[] recordData)
            {
                MemoryStream _ms = null;
                BinaryReader _br = null;
                try
                {
                    _ms = new MemoryStream(recordData);
                    _br = new BinaryReader(_ms);

                    _br.ReadBytes(16 /* Bounds */);

                    System.Diagnostics.Debug.Assert(_ms.Position == _ms.Length);

                    FillPolygon(_lineBuffer.GetPoints(), _brush);
                }
                finally
                {
                    if (_br != null)
                        _br.Close();
                    if (_ms != null)
                        _ms.Dispose();
                }
            }

            private class LineBuffer
            {
                public LineBuffer(float unitSize)
                {
                    _points = new System.Collections.Generic.List<NormalizedPoint>();
                    _normalizedPoints = new System.Collections.Generic.List<NormalizedPoint>();
                    _visualPoints = new System.Collections.Generic.List<VisualPoint>();
                    _epsilonSquare = (UnitSizeEpsilon * unitSize) * (UnitSizeEpsilon * unitSize);
                }

                public bool IsEmpty
                {
                    get
                    {
                        return _points.Count == 0;
                    }
                }

                public void Add(PointF[] points, int offset, int count)
                {
                    if (IsEmpty)
                    {
                        MakeRoom(count);
                        for (var i = 0; i < count; i++)
                            _points.Add(Add(points[offset + i]));
                    }
                    else
                    {
                        if (!IsVisuallyIdentical(GetLastPoint(), points[offset]))
                            throw new ArgumentOutOfRangeException();

                        MakeRoom(count - 1);

                        Add(points[offset]);

                        for (var i = 1; i < count; i++)
                            _points.Add(Add(points[offset + i]));
                    }
                }

                public bool CanAdd(PointF[] points, int offset, int count)
                {
                    if (IsEmpty)
                        return true;

                    if (IsVisuallyIdentical(GetLastPoint(), points[offset]))
                        return true;

                    return false;
                }

                public void Clear()
                {
                    _points.Clear();
                }

                public PointF[] GetPoints()
                {
                    var points = new System.Collections.Generic.List<NormalizedPoint>();

                    points.Add(_points[0]);
                    for (var i = 1; i < _points.Count; i++)
                    {
                        if (!IsVisuallyIdentical(points[points.Count - 1], _points[i]))
                        {
                            points.Add(_points[i]);
                        }
                    }

                    if (points.Count <= 1)
                        return null;

                    var result = new System.Collections.Generic.List<PointF>();

                    for (var i = 0; i < points.Count; i++)
                    {
                        var visualPoint = _visualPoints[points[i].VisualIndex];
                        if (!visualPoint.IsLocked)
                        {
                            // Calculate the visual point's appearance as "the middle" of all points
                            double sumX = 0;
                            double sumY = 0;
                            for (int j = 0, siblingCount = visualPoint.Weight; siblingCount > 0; j++)
                            {
                                if (_normalizedPoints[j].VisualIndex == visualPoint.VisualIndex)
                                {
                                    sumX += _normalizedPoints[j].Point.X;
                                    sumY += _normalizedPoints[j].Point.Y;
                                    siblingCount--;
                                }
                            }

                            visualPoint.Point = new PointF((float)(sumX / visualPoint.Weight), (float)(sumY / visualPoint.Weight));
                            visualPoint.IsLocked = true;
                        }
                        result.Add(visualPoint.Point);
                    }

                    return result.ToArray();
                }

                private static readonly float UnitSizeEpsilon = 2.0f;
                private readonly float _epsilonSquare;
                private readonly System.Collections.Generic.List<NormalizedPoint> _normalizedPoints;
                private readonly System.Collections.Generic.List<NormalizedPoint> _points;
                private readonly System.Collections.Generic.List<VisualPoint> _visualPoints;

                private static bool IsVisuallyIdentical(NormalizedPoint a, NormalizedPoint b)
                {
                    return a.VisualIndex == b.VisualIndex;
                }

                private NormalizedPoint Add(PointF point)
                {
                    NormalizedPoint result;
                    VisualPoint visualPoint;

                    for (var i = _normalizedPoints.Count - 1; i >= 0; i--)
                    {
                        if (IsVisuallyIdentical(_normalizedPoints[i].Point, point))
                        {
                            visualPoint = _visualPoints[_normalizedPoints[i].VisualIndex];
                            visualPoint.Weight++;
                            result = new NormalizedPoint { Point = point, VisualIndex = visualPoint.VisualIndex };
                            _normalizedPoints.Add(result);
                            return result;
                        }
                    }

                    visualPoint = new VisualPoint { IsLocked = false, VisualIndex = _visualPoints.Count, Weight = 1 };
                    _visualPoints.Add(visualPoint);

                    result = new NormalizedPoint { Point = point, VisualIndex = visualPoint.VisualIndex };
                    _normalizedPoints.Add(result);
                    return result;
                }

                private NormalizedPoint GetLastPoint()
                {
                    return _points[_points.Count - 1];
                }

                // TODO: TUNE: what's the correct value? Shoud it be based on the matafile's DPI?
                private bool IsVisuallyIdentical(NormalizedPoint a, PointF b)
                {
                    for (var i = _normalizedPoints.Count - 1; i >= 0; i--)
                        if (_normalizedPoints[i].VisualIndex == a.VisualIndex)
                            if (IsVisuallyIdentical(_normalizedPoints[i].Point, b))
                                return true;
                    return false;
                }

                private bool IsVisuallyIdentical(PointF a, PointF b)
                {
                    var dx = a.X - b.X;
                    var dy = a.Y - b.Y;
                    return (dx * dx + dy * dy) <= _epsilonSquare;
                }

                private void MakeRoom(int count)
                {
                    if (_points.Capacity < _points.Count + count)
                        _points.Capacity = _points.Count + count;
                }
            }

            private class NormalizedPoint
            {
                public PointF Point;
                public int VisualIndex;
            }

            private class ObjectHandle
            {
                public ObjectHandle(EmfStockObject stockObject)
                {
                    _stockObject = stockObject;
                }

                public ObjectHandle(Brush brush)
                {
                    _brush = brush;
                }

                public bool IsBrush
                {
                    get
                    {
                        return _brush != null;
                    }
                }

                public bool IsStockObject
                {
                    get
                    {
                        return _stockObject != null;
                    }
                }

                public Brush GetBrush()
                {
                    return _brush;
                }

                public EmfStockObject GetStockObject()
                {
                    return _stockObject.Value;
                }

                private readonly Brush _brush;
                private readonly EmfStockObject? _stockObject;
            }

            private class VisualPoint
            {
                public bool IsLocked;
                public PointF Point;
                public int VisualIndex;
                public int Weight;
            }
        }
    }

    /// <summary>
    /// This is an IGraphics implementor that builds up an SVG scene.  Use it like a regular <c>Graphics</c> object, and call
    /// <c>WriteXMLString</c> to output SVG.  In this way, whatever you would normally draw becomes available as an SVG document.
    /// <para>
    /// SvgGraphics has to do quite a lot of work to convert GDI instructions to SVG equivalents.  Some things are approximated and slight differences will
    /// be noticed.  Also, in several places GDI+ does not do what it is supposed to (e.g. arcs of non-circular ellipses, truncating bitmaps).  In these cases
    /// SvgGraphics does do the right thing, so the result will be different.
    /// </para>
    /// <para>
    /// Some GDI instructions such as <c>MeasureString</c>
    /// are meaningless in SVG, usually because there is no physical display device to refer to.  When such a method is called an <see cref="SvgGdiNotImpl"/> exception is thrown.
    /// </para>
    /// <para>
    /// Many parameters used by GDI have no SVG equivalent -- for instance, GDI allows some fine control over how font hints are used.  This detailed information is
    /// thrown away.
    /// </para>
    /// <para>
    /// Some aspects of GDI that can be implemented in SVG are not.  The most important omission is that only solid brushes are supported.
    /// </para>
    /// </summary>
    public class SvgGraphics : IGraphics
    {
        public SvgGraphics() : this(Color.FromName("Control"))
        {
        }

        public SvgGraphics(Color backgroundColor)
        {
            _root = new SvgSvgElement { Id = "SvgGdi_output" };

            _bg = new SvgRectElement(0, 0, "100%", "100%") { Id = "background" };
            _bg.Style.Set("fill", new SvgColor(backgroundColor));
            _root.AddChild(_bg);

            _topgroup = new SvgGroupElement("root_group");
            _topgroup.Style.Set("shape-rendering", "crispEdges");
            _cur = _topgroup;
            _root.AddChild(_topgroup);

            _defs = new SvgDefsElement("clips_hatches_and_gradients");
            _root.AddChild(_defs);

            _transforms = new MatrixStack();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public System.Drawing.Region Clip { get { throw new SvgGdiNotImpl("Clip"); } set { throw new SvgGdiNotImpl("Clip"); } }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public System.Drawing.RectangleF ClipBounds { get { throw new SvgGdiNotImpl("ClipBounds"); } }

        /// <summary>
        /// Get is not implemented (throws an exception).  Set does nothing.
        /// </summary>
        public CompositingMode CompositingMode
        { get { throw new SvgGdiNotImpl("get_CompositingMode"); } set { } }

        /// <summary>
        /// Get is not implemented (throws an exception).  Set does nothing.
        /// </summary>
        public CompositingQuality CompositingQuality
        { get { throw new SvgGdiNotImpl("get_CompositingQuality"); } set { } }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public float DpiX { get { throw new SvgGdiNotImpl("DpiX"); } }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public float DpiY { get { throw new SvgGdiNotImpl("DpiY"); } }

        /// <summary>
        /// Get is not implemented (throws an exception).  Set does nothing.
        /// </summary>
        public InterpolationMode InterpolationMode { get { throw new SvgGdiNotImpl("get_InterpolationMode"); } set { } }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public bool IsClipEmpty { get { throw new SvgGdiNotImpl("IsClipEmpty"); } }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public bool IsVisibleClipEmpty { get { throw new SvgGdiNotImpl("IsVisibleClipEmpty"); } }

        /// <summary>
        /// Get is not implemented (throws an exception).  Set does nothing.
        /// </summary>
        public float PageScale { get { throw new SvgGdiNotImpl("PageScale"); } set { } }

        /// <summary>
        /// Get is not implemented (throws an exception).  Set does nothing.
        /// </summary>
        public System.Drawing.GraphicsUnit PageUnit { get { throw new SvgGdiNotImpl("PageUnit"); } set { } }

        /// <summary>
        /// Get is not implemented (throws an exception).  Set does nothing.
        /// </summary>
        public PixelOffsetMode PixelOffsetMode { get { throw new SvgGdiNotImpl("get_PixelOffsetMode"); } set { } }

        /// <summary>
        /// Get is not implemented (throws an exception).  Set does nothing.
        /// </summary>
        public System.Drawing.Point RenderingOrigin { get { throw new SvgGdiNotImpl("get_RenderingOrigin"); } set { } }

        public SmoothingMode SmoothingMode
        {
            get { return _smoothingMode; }
            set
            {
                switch (value)
                {
                    case SmoothingMode.Invalid:
                        break;

                    case SmoothingMode.None:
                        _cur.Style.Set("shape-rendering", "crispEdges"); break;
                    case SmoothingMode.Default:
                        _cur.Style.Set("shape-rendering", "crispEdges"); break;
                    case SmoothingMode.HighSpeed:
                        _cur.Style.Set("shape-rendering", "optimizeSpeed"); break;
                    case SmoothingMode.AntiAlias:
                        _cur.Style.Set("shape-rendering", "auto"); break;
                    case SmoothingMode.HighQuality:
                        _cur.Style.Set("shape-rendering", "geometricPrecision"); break;

                    default:
                        _cur.Style.Set("shape-rendering", "auto"); break;
                }
                _smoothingMode = value;
            }
        }

        /// <summary>
        /// Get is not implemented (throws an exception).
        /// </summary>
        public int TextContrast
        { get { throw new SvgGdiNotImpl("get_TextContrast"); } set { } }

        /// <summary>
        /// Get is not implemented (throws an exception).
        /// </summary>
        public TextRenderingHint TextRenderingHint
        {
            get { throw new SvgGdiNotImpl("get_TextRenderingHint"); }
            set
            {
                switch (value)
                {
                    case TextRenderingHint.AntiAlias:
                        _cur.Style.Set("text-rendering", "auto"); break;
                    case TextRenderingHint.AntiAliasGridFit:
                        _cur.Style.Set("text-rendering", "auto"); break;
                    case TextRenderingHint.ClearTypeGridFit:
                        _cur.Style.Set("text-rendering", "geometricPrecision"); break;
                    default:
                        _cur.Style.Set("text-rendering", "crispEdges"); break;
                }
            }
        }

        public Matrix Transform
        {
            get
            {
                return _transforms.Result.Clone();
            }

            set
            {
                _transforms.Top = value;
            }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public System.Drawing.RectangleF VisibleClipBounds { get { throw new SvgGdiNotImpl("VisibleClipBounds"); } }

        /// <summary>
        /// Does nothing.  Should perhaps insert a comment into the SVG XML output, but is this really analogous
        /// to a metafile comment.
        /// </summary>
        public void AddMetafileComment(Byte[] data)
        {
            //probably should add xml comment
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public GraphicsContainer BeginContainer(RectangleF dstrect, RectangleF srcrect, GraphicsUnit unit) { throw new SvgGdiNotImpl("BeginContainer (RectangleF dstrect, RectangleF srcrect, GraphicsUnit unit)"); }

        /// <summary>
        /// Implemented, but returns null as SVG has a proper scenegraph, unlike GDI+.  The effect of calling <c>BeginContainer</c> is to create a new SVG group
        /// and apply transformations etc to produce the effect that a GDI+ container would produce.
        /// </summary>
        public GraphicsContainer BeginContainer()
        {
            SvgGroupElement gr = new SvgGroupElement();
            _cur.AddChild(gr);
            _cur = gr;
            _cur.Id += "_BeginContainer";
            _transforms.Push();
            return null;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public GraphicsContainer BeginContainer(Rectangle dstrect, Rectangle srcrect, GraphicsUnit unit) { throw new SvgGdiNotImpl("BeginContainer (Rectangle dstrect, Rectangle srcrect, GraphicsUnit unit)"); }

        /// <summary>
        /// Implemented
        /// </summary>
        public void Clear(Color color)
        {
            _cur.Children.Clear();
            _bg.Style.Set("fill", new SvgColor(color));
        }

        /// <summary>
        /// Implemented.  <c>DrawArc</c> functions work correctly and thus produce different output from GDI+ if the ellipse is not circular.
        /// </summary>
        public void DrawArc(Pen pen, Single x, Single y, Single width, Single height, Single startAngle, Single sweepAngle)
        {
            string s = GDIArc2SVGPath(x, y, width, height, startAngle, sweepAngle, false);

            SvgPathElement arc = new SvgPathElement();
            arc.D = s;
            arc.Style = new SvgStyle(pen);
            if (!_transforms.Result.IsIdentity)
                arc.Transform = new SvgTransformList(_transforms.Result.Clone());
            _cur.AddChild(arc);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawArc(Pen pen, RectangleF rect, Single startAngle, Single sweepAngle)
        {
            DrawArc(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawArc(Pen pen, Int32 x, Int32 y, Int32 width, Int32 height, Int32 startAngle, Int32 sweepAngle)
        {
            DrawArc(pen, (Single)x, (Single)y, (Single)width, (Single)height, (Single)startAngle, (Single)sweepAngle);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawArc(Pen pen, Rectangle rect, Single startAngle, Single sweepAngle)
        {
            DrawArc(pen, (Single)rect.X, (Single)rect.X, (Single)rect.Width, (Single)rect.Height, (Single)startAngle, (Single)sweepAngle);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawBezier(Pen pen, Single x1, Single y1, Single x2, Single y2, Single x3, Single y3, Single x4, Single y4)
        {
            var bez = new SvgPathElement
            {
                D = "M " + x1.ToString("F", CultureInfo.InvariantCulture) + " " + y1.ToString("F", CultureInfo.InvariantCulture) + " C " +
                    x2.ToString("F", CultureInfo.InvariantCulture) + " " + y2.ToString("F", CultureInfo.InvariantCulture) + " " +
                    x3.ToString("F", CultureInfo.InvariantCulture) + " " + y3.ToString("F", CultureInfo.InvariantCulture) + " " +
                    x4.ToString("F", CultureInfo.InvariantCulture) + " " + y4.ToString("F", CultureInfo.InvariantCulture),
                Style = new SvgStyle(pen)
            };
            if (!_transforms.Result.IsIdentity)
                bez.Transform = new SvgTransformList(_transforms.Result.Clone());
            _cur.AddChild(bez);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawBezier(Pen pen, PointF pt1, PointF pt2, PointF pt3, PointF pt4)
        {
            DrawBezier(pen, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawBezier(Pen pen, Point pt1, Point pt2, Point pt3, Point pt4)
        {
            DrawBezier(pen, (Single)pt1.X, (Single)pt1.Y, (Single)pt2.X, (Single)pt2.Y, (Single)pt3.X, (Single)pt3.Y, (Single)pt4.X, (Single)pt4.Y);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawBeziers(Pen pen, PointF[] points)
        {
            SvgPathElement bez = new SvgPathElement();

            string s = "M " + points[0].X.ToString("F", CultureInfo.InvariantCulture) + " " + points[0].Y.ToString("F", CultureInfo.InvariantCulture) + " C ";

            for (int i = 1; i < points.Length; ++i)
            {
                s += points[i].X.ToString("F", CultureInfo.InvariantCulture) + " " + points[i].Y.ToString("F", CultureInfo.InvariantCulture) + " ";
            }

            bez.D = s;

            bez.Style = new SvgStyle(pen);
            if (!_transforms.Result.IsIdentity)
                bez.Transform = new SvgTransformList(_transforms.Result.Clone());
            _cur.AddChild(bez);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawBeziers(Pen pen, Point[] points)
        {
            PointF[] pts = Point2PointF(points);
            DrawBeziers(pen, pts);
        }

        /// <summary>
        /// Implemented.  The <c>DrawClosedCurve</c> functions emulate GDI behavior by drawing a coaligned cubic bezier.  This seems to produce
        /// a very good approximation so probably GDI+ does the same thing -- a
        /// </summary>
        public void DrawClosedCurve(Pen pen, PointF[] points)
        {
            PointF[] pts = Spline2Bez(points, 0, points.Length - 1, true, .5f);
            DrawBeziers(pen, pts);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawClosedCurve(Pen pen, PointF[] points, Single tension, FillMode fillmode)
        {
            PointF[] pts = Spline2Bez(points, 0, points.Length - 1, true, tension);
            DrawBeziers(pen, pts);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawClosedCurve(Pen pen, Point[] points)
        {
            PointF[] pts = Spline2Bez(Point2PointF(points), 0, points.Length - 1, true, .5f);
            DrawBeziers(pen, pts);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawClosedCurve(Pen pen, Point[] points, Single tension, FillMode fillmode)
        {
            PointF[] pts = Spline2Bez(Point2PointF(points), 0, points.Length - 1, true, tension);
            DrawBeziers(pen, pts);
        }

        /// <summary>
        /// Implemented.  The <c>DrawCurve</c> functions emulate GDI behavior by drawing a coaligned cubic bezier.  This seems to produce
        /// a very good approximation so probably GDI+ does the same.
        /// </summary>
        public void DrawCurve(Pen pen, PointF[] points)
        {
            PointF[] pts = Spline2Bez(points, 0, points.Length - 1, false, .5f);
            DrawBeziers(pen, pts);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawCurve(Pen pen, PointF[] points, Single tension)
        {
            PointF[] pts = Spline2Bez(points, 0, points.Length - 1, false, tension);
            DrawBeziers(pen, pts);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawCurve(Pen pen, PointF[] points, Int32 offset, Int32 numberOfSegments)
        {
            PointF[] pts = Spline2Bez(points, offset, numberOfSegments, false, .5f);
            DrawBeziers(pen, pts);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawCurve(Pen pen, PointF[] points, Int32 offset, Int32 numberOfSegments, Single tension)
        {
            PointF[] pts = Spline2Bez(points, offset, numberOfSegments, false, tension);
            DrawBeziers(pen, pts);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawCurve(Pen pen, Point[] points)
        {
            PointF[] pts = Spline2Bez(Point2PointF(points), 0, points.Length - 1, false, .5f);
            DrawBeziers(pen, pts);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawCurve(Pen pen, Point[] points, Single tension)
        {
            PointF[] pts = Spline2Bez(Point2PointF(points), 0, points.Length - 1, false, tension);
            DrawBeziers(pen, pts);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawCurve(Pen pen, Point[] points, Int32 offset, Int32 numberOfSegments, Single tension)
        {
            PointF[] pts = Spline2Bez(Point2PointF(points), offset, numberOfSegments, false, tension);
            DrawBeziers(pen, pts);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawEllipse(Pen pen, RectangleF rect)
        {
            DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawEllipse(Pen pen, Single x, Single y, Single width, Single height)
        {
            SvgEllipseElement el = new SvgEllipseElement(x + width / 2, y + height / 2, width / 2, height / 2);
            el.Style = new SvgStyle(pen);
            if (!_transforms.Result.IsIdentity)
                el.Transform = new SvgTransformList(_transforms.Result.Clone());
            _cur.AddChild(el);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawEllipse(Pen pen, Rectangle rect)
        {
            DrawEllipse(pen, (Single)rect.X, (Single)rect.Y, (Single)rect.Width, (Single)rect.Height);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawEllipse(Pen pen, Int32 x, Int32 y, Int32 width, Int32 height)
        {
            DrawEllipse(pen, (Single)x, (Single)y, (Single)width, (Single)height);
        }

        /// <summary>
        /// Implemented.  The <c>DrawIcon</c> group of functions emulate drawing a bitmap by creating many SVG <c>rect</c> elements.  This is quite effective but
        /// can lead to a very big SVG file.  Alpha and stretching are handled correctly.  No antialiasing is done.
        /// </summary>
        public void DrawIcon(Icon icon, Int32 x, Int32 y)
        {
            Bitmap bmp = icon.ToBitmap();
            DrawBitmapData(bmp, x, y, icon.Width, icon.Height, false);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawIcon(Icon icon, Rectangle targetRect)
        {
            Bitmap bmp = icon.ToBitmap();
            DrawBitmapData(bmp, targetRect.X, targetRect.Y, targetRect.Width, targetRect.Height, true);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawIconUnstretched(Icon icon, Rectangle targetRect)
        {
            Bitmap bmp = icon.ToBitmap();
            DrawBitmapData(bmp, targetRect.X, targetRect.Y, targetRect.Width, targetRect.Height, false);
        }

        /// <summary>
        /// Implemented.  The <c>DrawImage</c> group of functions emulate drawing a bitmap by creating many SVG <c>rect</c> elements.  This is quite effective but
        /// can lead to a very big SVG file.  Alpha and stretching are handled correctly.  No antialiasing is done.
        /// <para>
        /// The GDI+ documentation suggests that the 'Unscaled' functions should truncate the image.  GDI+ does not actually do this, but <c>SvgGraphics</c> does.
        /// </para>
        /// </summary>
        public void DrawImage(Image image, PointF point)
        {
            DrawImage(image, point.X, point.Y);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawImage(Image image, Single x, Single y)
        {
            DrawImage(image, x, y, (Single)image.Width, (Single)image.Height);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawImage(Image image, RectangleF rect)
        {
            DrawImage(image, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawImage(Image image, Single x, Single y, Single width, Single height)
        {
            if (image.GetType() != typeof(Bitmap))
                return;
            DrawBitmapData((Bitmap)image, x, y, width, height, true);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawImage(Image image, Point point)
        {
            DrawImage(image, (Single)point.X, (Single)point.Y);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawImage(Image image, Int32 x, Int32 y)
        {
            DrawImage(image, (Single)x, (Single)y);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawImage(Image image, Rectangle rect)
        {
            DrawImage(image, (Single)rect.X, (Single)rect.Y, (Single)rect.Width, (Single)rect.Height);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawImage(Image image, Int32 x, Int32 y, Int32 width, Int32 height)
        {
            DrawImage(image, (float)x, y, width, height);
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void DrawImage(Image image, PointF[] destPoints) { throw new SvgGdiNotImpl("DrawImage (Image image, PointF[] destPoints)"); }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void DrawImage(Image image, Point[] destPoints) { throw new SvgGdiNotImpl("DrawImage (Image image, Point[] destPoints)"); }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void DrawImage(Image image, Single x, Single y, RectangleF srcRect, GraphicsUnit srcUnit) { throw new SvgGdiNotImpl("DrawImage (Image image, Single x, Single y, RectangleF srcRect, GraphicsUnit srcUnit)"); }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void DrawImage(Image image, Int32 x, Int32 y, Rectangle srcRect, GraphicsUnit srcUnit) { throw new SvgGdiNotImpl("DrawImage (Image image, Int32 x, Int32 y, Rectangle srcRect, GraphicsUnit srcUnit)"); }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void DrawImage(Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit) { throw new SvgGdiNotImpl("DrawImage (Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit)"); }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void DrawImage(Image image, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit) { throw new SvgGdiNotImpl("DrawImage (Image image, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit)"); }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit) { throw new SvgGdiNotImpl("DrawImage (Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit)"); }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr) { throw new SvgGdiNotImpl("DrawImage (Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr)"); }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, Graphics.DrawImageAbort callback) { throw new SvgGdiNotImpl("DrawImage (Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, Graphics.DrawImageAbort callback)"); }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit) { throw new SvgGdiNotImpl("DrawImage (Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit)"); }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr) { throw new SvgGdiNotImpl("DrawImage (Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr)"); }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void DrawImage(Image image, Rectangle destRect, Single srcX, Single srcY, Single srcWidth, Single srcHeight, GraphicsUnit srcUnit) { throw new SvgGdiNotImpl("DrawImage (Image image, Rectangle destRect, Single srcX, Single srcY, Single srcWidth, Single srcHeight, GraphicsUnit srcUnit)"); }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void DrawImage(Image image, Rectangle destRect, Single srcX, Single srcY, Single srcWidth, Single srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs) { throw new SvgGdiNotImpl("DrawImage (Image image, Rectangle destRect, Single srcX, Single srcY, Single srcWidth, Single srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs)"); }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void DrawImage(Image image, Rectangle destRect, Int32 srcX, Int32 srcY, Int32 srcWidth, Int32 srcHeight, GraphicsUnit srcUnit) { throw new SvgGdiNotImpl("DrawImage (Image image, Rectangle destRect, Int32 srcX, Int32 srcY, Int32 srcWidth, Int32 srcHeight, GraphicsUnit srcUnit)"); }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void DrawImage(Image image, Rectangle destRect, Int32 srcX, Int32 srcY, Int32 srcWidth, Int32 srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr) { throw new SvgGdiNotImpl("DrawImage (Image image, Rectangle destRect, Int32 srcX, Int32 srcY, Int32 srcWidth, Int32 srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr)"); }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawImageUnscaled(Image image, Point point)
        {
            DrawImage(image, (Single)point.X, (Single)point.Y);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawImageUnscaled(Image image, Int32 x, Int32 y)
        {
            DrawImage(image, (Single)x, (Single)y);
        }

        /// <summary>
        /// Implemented.  There seems to be a GDI bug in that the image is *not* clipped to the rectangle.  We do clip it.
        /// </summary>
        public void DrawImageUnscaled(Image image, Rectangle rect)
        {
            DrawImageUnscaled(image, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawImageUnscaled(Image image, Int32 x, Int32 y, Int32 width, Int32 height)
        {
            if (image.GetType() != typeof(Bitmap))
                return;
            DrawBitmapData((Bitmap)image, x, y, width, height, false);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawLine(Pen pen, Single x1, Single y1, Single x2, Single y2)
        {
            if (IsEndAnchorSimple(pen.StartCap) && IsEndAnchorSimple(pen.EndCap))
            {
                // This code works, but not for CustomLineCup style
                SvgLineElement lin = new SvgLineElement(x1, y1, x2, y2);
                lin.Style = new SvgStyle(pen);
                if (!_transforms.Result.IsIdentity)
                    lin.Transform = new SvgTransformList(_transforms.Result.Clone());
                _cur.AddChild(lin);

                DrawEndAnchors(pen, new PointF(x1, y1), new PointF(x2, y2));
            }
            else
            {
                DrawLines(pen, new PointF[] { new PointF(x1, y1), new PointF(x2, y2) });
            }
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawLine(Pen pen, PointF pt1, PointF pt2)
        {
            DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawLine(Pen pen, Int32 x1, Int32 y1, Int32 x2, Int32 y2)
        {
            DrawLine(pen, (Single)x1, (Single)y1, (Single)x2, (Single)y2);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawLine(Pen pen, Point pt1, Point pt2)
        {
            DrawLine(pen, (Single)pt1.X, (Single)pt1.Y, (Single)pt2.X, (Single)pt2.Y);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawLines(Pen pen, PointF[] points)
        {
            if (points.Length <= 1)
                return;

            if (IsEndAnchorSimple(pen.StartCap) && IsEndAnchorSimple(pen.EndCap))
            {
                // This code works, but not for CustomLineCap style
                SvgPolylineElement pl = new SvgPolylineElement(points);
                pl.Style = new SvgStyle(pen);
                if (!_transforms.Result.IsIdentity)
                    pl.Transform = new SvgTransformList(_transforms.Top.Clone());
                _cur.AddChild(pl);

                DrawEndAnchors(pen, points[0], points[points.Length - 1]);

                return;
            }

            // GraphicsPaths used in the constructor of CustomLineCap
            // are private to the native GDI+ and for example the shape of AdjustableArrowCap
            // is completely private to the native GDI+
            //
            // So in order to render the possibly any-shaped custom line caps we'll draw the line as GDI metafile and then reverse
            // engineer the GDI metafile drawing and convert it to corresponding SVG commands

            // Calculate the bounding rectangle
            var minX = points[0].X;
            var maxX = points[0].X;
            var minY = points[0].Y;
            var maxY = points[0].Y;
            for (var i = 1; i < points.Length; i++)
            {
                var point = points[i];
                minX = Math.Min(minX, point.X);
                maxX = Math.Max(maxX, point.X);
                minY = Math.Min(minY, point.Y);
                maxY = Math.Max(maxY, point.Y);
            }
            var bounds = new RectangleF(minX, minY, maxX - minX + 1, maxY - minY + 1);

            // Make the rectangle 0-based where "zero" represents the original shift
            var zero = bounds.Location;
            bounds.Offset(-zero.X, -zero.Y);

            // Make the original point-path "zero"-based
            for (var i = 0; i < points.Length; i++)
            {
                points[i].X -= zero.X;
                points[i].Y -= zero.Y;
            }

            using (var metafileBuffer = new System.IO.MemoryStream())
            {
                System.Drawing.Imaging.Metafile metafile = null;

                try
                {
                    /* For discussion of tricky metafile details see:
                     * - http://nicholas.piasecki.name/blog/2009/06/drawing-o-an-in-memory-metafile-in-c-sharp/
                     * - http://stackoverflow.com/a/1533053/2626313
                     */

                    using (var temporaryBitmap = new Bitmap(1, 1))
                    {
                        using (var temporaryCanvas = Graphics.FromImage(temporaryBitmap))
                        {
                            var hdc = temporaryCanvas.GetHdc();
                            metafile = new Metafile(
                                metafileBuffer,
                                hdc,
                                bounds,
                                MetafileFrameUnit.GdiCompatible,
                                EmfType.EmfOnly);

                            temporaryCanvas.ReleaseHdc();
                        }
                    }

                    using (var metafileCanvas = Graphics.FromImage(metafile))
                    {
                        metafileCanvas.DrawLines(pen, points);
                    }
                }
                finally
                {
                    if (metafile != null)
                        metafile.Dispose();
                }

                metafileBuffer.Position = 0;

                var metafileIsEmpty = true;
                var parser = new MetafileTools.MetafileParser();
                parser.EnumerateMetafile(metafileBuffer, pen.Width, zero, (PointF[] linePoints) =>
                {
                    metafileIsEmpty = false;

                    SvgPolylineElement pl = new SvgPolylineElement(linePoints);
                    pl.Style = new SvgStyle(pen);

                    // Make it pretty
                    pl.Style.Set("stroke-linecap", "round");

                    if (!_transforms.Result.IsIdentity)
                        pl.Transform = new SvgTransformList(_transforms.Top.Clone());
                    _cur.AddChild(pl);
                }, (PointF[] linePoints, Brush fillBrush) =>
                {
                    // TODO: received shapes dont' have the vertex list "normalized" correctly
                    // metafileIsEmpty = false;
                    // FillPolygon(fillBrush, linePoints);
                });

                if (metafileIsEmpty)
                {
                    // TODO: metafile recording on OpenSUSE Linux with Mono 3.8.0 does not seem to work at all
                    // as the supposed implementation in https://github.com/mono/libgdiplus/blob/master/src/graphics-metafile.c is
                    // full of "TODO". In this case we should take a graceful fallback approach

                    // Restore points array to the original values they had when entered the function
                    for (var i = 0; i < points.Length; i++)
                    {
                        points[i].X += zero.X;
                        points[i].Y += zero.Y;
                    }

                    SvgPolylineElement pl = new SvgPolylineElement(points);
                    pl.Style = new SvgStyle(pen);
                    if (!_transforms.Result.IsIdentity)
                        pl.Transform = new SvgTransformList(_transforms.Top.Clone());
                    _cur.AddChild(pl);

                    DrawEndAnchors(pen, points[0], points[points.Length - 1], ignoreUnsupportedLineCaps: true);
                }
            }
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawLines(Pen pen, Point[] points)
        {
            PointF[] pts = Point2PointF(points);
            DrawLines(pen, pts);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        /// <remarks>
        /// Mainly based on the libgdi+ implementation: https://github.com/mono/libgdiplus/blob/master/src/graphics-cairo.c
        /// and this SO question reply: https://stackoverflow.com/questions/1790862/how-to-determine-endpoints-of-arcs-in-graphicspath-pathpoints-and-pathtypes-arra
        /// from SiliconMind.
        /// </remarks>
        public void DrawPath(Pen pen, GraphicsPath path)
        {
            //Save the original pen dash style in case we need to change it
            DashStyle originalPenDashStyle = pen.DashStyle;

            GraphicsPathIterator subpaths = new GraphicsPathIterator(path);
            GraphicsPath subpath = new GraphicsPath(path.FillMode);
            subpaths.Rewind();

            //Iterate through all the subpaths in the path. Each subpath will contain either
            //lines or Bezier curves
            for (int s = 0; s < subpaths.SubpathCount; s++)
            {
                bool isClosed;
                if (subpaths.NextSubpath(subpath, out isClosed) == 0)
                {
                    continue; //go to next subpath if this one has zero points.
                }
                PointF start = new PointF(0, 0);
                PointF origin = subpath.PathPoints[0];
                PointF last = subpath.PathPoints[subpath.PathPoints.Length - 1];
                int bezierCurvePointsIndex = 0;
                PointF[] bezierCurvePoints = new PointF[4];
                for (int i = 0; i < subpath.PathPoints.Length; i++)
                {
                    /* Each subpath point has a corresponding path point type which can be:
                     *The point starts the subpath
                     *The point is a line point
                     *The point is Bezier curve point
                     * Another point type like dash-mode
                     */
                    switch ((PathPointType)subpath.PathTypes[i] & PathPointType.PathTypeMask) //Mask off non path-type types
                    {
                        case PathPointType.Start:
                            start = subpath.PathPoints[i];
                            bezierCurvePoints[0] = subpath.PathPoints[i];
                            bezierCurvePointsIndex = 1;
                            pen.DashStyle = originalPenDashStyle; //Reset pen dash mode to original when starting subpath
                            continue;
                        case PathPointType.Line:   
                            DrawLine(pen, start, subpath.PathPoints[i]); //Draw a line segment ftom start point
                            start = subpath.PathPoints[i]; //Move start point to line end
                            bezierCurvePoints[0] = subpath.PathPoints[i]; //A line point can also be the start of a Bezier curve
                            bezierCurvePointsIndex = 1;
                            continue;
                        case PathPointType.Bezier3:
                            bezierCurvePoints[bezierCurvePointsIndex++] = subpath.PathPoints[i];
                            if (bezierCurvePointsIndex == 4) //If 4 points including start have been found then draw the Bezier curve
                            {
                                DrawBezier(pen, bezierCurvePoints[0], bezierCurvePoints[1], bezierCurvePoints[2], bezierCurvePoints[3]);
                                bezierCurvePoints = new PointF[4];
                                bezierCurvePoints[0] = subpath.PathPoints[i];
                                bezierCurvePointsIndex = 1;
                                start = subpath.PathPoints[i]; //Move start point to curve end
                            }
                            continue;
                        default:
                            switch ((PathPointType)subpath.PathTypes[i])
                            {
                                case PathPointType.DashMode:
                                    pen.DashStyle = DashStyle.Dash;
                                    continue;
                                default:
                                    throw new SvgException("Unknown path type value: " + subpath.PathTypes[i]);
                            }
                    }
                }
                if (isClosed) //If the subpath is closed and it is a linear figure then draw the last connecting line segment
                {
                    PathPointType originType = (PathPointType)subpath.PathTypes[0];
                    PathPointType lastType = (PathPointType) subpath.PathTypes[subpath.PathPoints.Length - 1];

                    if (((lastType & PathPointType.PathTypeMask) == PathPointType.Line) && ((originType & PathPointType.PathTypeMask) == PathPointType.Line))
                    {
                        DrawLine(pen, last, origin);
                    }
                }
                
            }
            subpath.Dispose();
            subpaths.Dispose();
            pen.DashStyle = originalPenDashStyle;
        }

        /// <summary>
        /// Implemented.  <c>DrawPie</c> functions work correctly and thus produce different output from GDI+ if the ellipse is not circular.
        /// </summary>
        public void DrawPie(Pen pen, Single x, Single y, Single width, Single height, Single startAngle, Single sweepAngle)
        {
            string s = GDIArc2SVGPath(x, y, width, height, startAngle, sweepAngle, true);

            SvgPathElement pie = new SvgPathElement();
            pie.D = s;
            pie.Style = new SvgStyle(pen);
            if (!_transforms.Result.IsIdentity)
                pie.Transform = new SvgTransformList(_transforms.Result.Clone());

            _cur.AddChild(pie);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawPie(Pen pen, RectangleF rect, Single startAngle, Single sweepAngle)
        {
            DrawPie(pen, rect.X, rect.X, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawPie(Pen pen, Int32 x, Int32 y, Int32 width, Int32 height, Int32 startAngle, Int32 sweepAngle)
        {
            DrawPie(pen, (Single)x, (Single)y, (Single)width, (Single)height, (Single)startAngle, (Single)sweepAngle);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawPie(Pen pen, Rectangle rect, Single startAngle, Single sweepAngle)
        {
            DrawPie(pen, (Single)rect.X, (Single)rect.X, (Single)rect.Width, (Single)rect.Height, (Single)startAngle, (Single)sweepAngle);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawPolygon(Pen pen, PointF[] points)
        {
            SvgPolygonElement pl = new SvgPolygonElement(points);
            pl.Style = new SvgStyle(pen);
            if (!_transforms.Result.IsIdentity)
                pl.Transform = new SvgTransformList(_transforms.Result.Clone());
            _cur.AddChild(pl);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawPolygon(Pen pen, Point[] points)
        {
            PointF[] pts = Point2PointF(points);
            DrawPolygon(pen, pts);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawRectangle(Pen pen, Rectangle rect)
        {
            DrawRectangle(pen, (Single)rect.Left, (Single)rect.Top, (Single)rect.Width, (Single)rect.Height);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawRectangle(Pen pen, Single x, Single y, Single width, Single height)
        {
            SvgRectElement rc = new SvgRectElement(x, y, width, height);
            rc.Style = new SvgStyle(pen);
            if (!_transforms.Result.IsIdentity)
                rc.Transform = new SvgTransformList(_transforms.Result.Clone());
            _cur.AddChild(rc);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawRectangle(Pen pen, Int32 x, Int32 y, Int32 width, Int32 height)
        {
            DrawRectangle(pen, (Single)x, (Single)y, (Single)width, (Single)height);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawRectangles(Pen pen, RectangleF[] rects)
        {
            foreach (RectangleF rc in rects)
            {
                DrawRectangle(pen, rc.Left, rc.Top, rc.Width, rc.Height);
            }
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawRectangles(Pen pen, Rectangle[] rects)
        {
            foreach (Rectangle rc in rects)
            {
                DrawRectangle(pen, (Single)rc.Left, (Single)rc.Top, (Single)rc.Width, (Single)rc.Height);
            }
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawString(String s, Font font, Brush brush, Single x, Single y)
        {
            DrawText(s, font, brush, new RectangleF(x, y, 0, 0), StringFormat.GenericDefault, true);
        }

        /// <summary>
        /// Implemented.
        /// <para>In general, DrawString functions work, but it is impossible to guarantee that an SVG renderer will have a certain font and draw it in the
        /// same way as GDI+.
        /// </para>
        /// <para>
        /// SVG does not do word wrapping and SvgGdi does not emulate it yet (although clipping is working).  The plan is to wait till SVG 1.2 becomes available, since 1.2 contains text
        /// wrapping/flowing attributes.
        /// </para>
        /// </summary>
        public void DrawString(String s, Font font, Brush brush, PointF point)
        {
            DrawText(s, font, brush, new RectangleF(point.X, point.Y, 0, 0), StringFormat.GenericDefault, true);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawString(String s, Font font, Brush brush, Single x, Single y, StringFormat format)
        {
            DrawText(s, font, brush, new RectangleF(x, y, 0, 0), format, true);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawString(String s, Font font, Brush brush, PointF point, StringFormat format)
        {
            DrawText(s, font, brush, new RectangleF(point.X, point.Y, 0, 0), format, true);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawString(String s, Font font, Brush brush, RectangleF layoutRectangle)
        {
            DrawText(s, font, brush, layoutRectangle, StringFormat.GenericDefault, false);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void DrawString(String s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
        {
            DrawText(s, font, brush, layoutRectangle, format, false);
        }

        /// <summary>
        /// The effect of calling this method is to pop out of the closest SVG group.  This simulates restoring GDI+ state from a <c>GraphicsContainer</c>
        /// </summary>
        public void EndContainer(GraphicsContainer container)
        {
            if (_cur == _topgroup)
                return;

            _cur = (SvgStyledTransformedElement)_cur.Parent;

            _transforms.Pop();
        }

        /// <summary>
        /// Not implemented, because SvgGdi does not try and emulate GDI+ regions.  Only rectangular clip regions work.
        /// </summary>
        public void ExcludeClip(Rectangle rect) { throw new SvgGdiNotImpl("ExcludeClip (Rectangle rect)"); }

        /// <summary>
        /// Not implemented, because SvgGdi does not try and emulate GDI+ regions.  Only rectangular clip regions work.
        /// </summary>
        public void ExcludeClip(Region region) { throw new SvgGdiNotImpl("ExcludeClip (Region region)"); }

        /// <summary>
        /// Implemented
        /// </summary>
        public void FillClosedCurve(Brush brush, PointF[] points)
        {
            PointF[] pts = Spline2Bez(points, 0, points.Length - 1, true, .5f);
            FillBeziers(brush, pts, FillMode.Alternate);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode)
        {
            PointF[] pts = Spline2Bez(points, 0, points.Length - 1, true, .5f);
            FillBeziers(brush, pts, fillmode);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode, Single tension)
        {
            PointF[] pts = Spline2Bez(points, 0, points.Length - 1, true, tension);
            FillBeziers(brush, pts, fillmode);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void FillClosedCurve(Brush brush, Point[] points)
        {
            PointF[] pts = Spline2Bez(Point2PointF(points), 0, points.Length - 1, true, .5f);
            FillBeziers(brush, pts, FillMode.Alternate);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode)
        {
            PointF[] pts = Spline2Bez(Point2PointF(points), 0, points.Length - 1, true, .5f);
            FillBeziers(brush, pts, fillmode);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode, Single tension)
        {
            PointF[] pts = Spline2Bez(Point2PointF(points), 0, points.Length - 1, true, tension);
            FillBeziers(brush, pts, fillmode);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void FillEllipse(Brush brush, RectangleF rect)
        {
            FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void FillEllipse(Brush brush, Single x, Single y, Single width, Single height)
        {
            SvgEllipseElement el = new SvgEllipseElement(x + width / 2, y + height / 2, width / 2, height / 2);
            el.Style = HandleBrush(brush);
            if (!_transforms.Result.IsIdentity)
                el.Transform = new SvgTransformList(_transforms.Result.Clone());
            _cur.AddChild(el);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void FillEllipse(Brush brush, Rectangle rect)
        {
            FillEllipse(brush, (Single)rect.X, (Single)rect.Y, (Single)rect.Width, (Single)rect.Height);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void FillEllipse(Brush brush, Int32 x, Int32 y, Int32 width, Int32 height)
        {
            FillEllipse(brush, (Single)x, (Single)y, (Single)width, (Single)height);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void FillPath(Brush brush, GraphicsPath path)
        {
            GraphicsPathIterator subpaths = new GraphicsPathIterator(path);
            GraphicsPath subpath = new GraphicsPath(path.FillMode);
            subpaths.Rewind();
            for (int s = 0; s < subpaths.SubpathCount; s++)
            {
                bool isClosed;
                if (subpaths.NextSubpath(subpath, out isClosed) < 2)
                {
                    continue;
                }
                if (!isClosed)
                {
                    //subpath.CloseAllFigures();
                }
                PathPointType lastType = (PathPointType)subpath.PathTypes[subpath.PathPoints.Length - 1];
                if (subpath.PathTypes.Any(pt => ((PathPointType) pt & PathPointType.PathTypeMask) == PathPointType.Line))
                {
                    FillPolygon(brush, subpath.PathPoints, path.FillMode);
                }
                else
                {
                    FillBeziers(brush, subpath.PathPoints, path.FillMode);
                }                                

            }
            subpath.Dispose();
            subpaths.Dispose();
        }

        /// <summary>
        /// Implemented <c>FillPie</c> functions work correctly and thus produce different output from GDI+ if the ellipse is not circular.
        /// </summary>
        public void FillPie(Brush brush, Rectangle rect, Single startAngle, Single sweepAngle)
        {
            FillPie(brush, (Single)rect.X, (Single)rect.Y, (Single)rect.Width, (Single)rect.Height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void FillPie(Brush brush, Single x, Single y, Single width, Single height, Single startAngle, Single sweepAngle)
        {
            string s = GDIArc2SVGPath(x, y, width, height, startAngle, sweepAngle, true);

            SvgPathElement pie = new SvgPathElement();
            pie.D = s;
            pie.Style = HandleBrush(brush);
            if (!_transforms.Result.IsIdentity)
                pie.Transform = new SvgTransformList(_transforms.Result.Clone());

            _cur.AddChild(pie);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void FillPie(Brush brush, Int32 x, Int32 y, Int32 width, Int32 height, Int32 startAngle, Int32 sweepAngle)
        {
            FillPie(brush, (Single)x, (Single)y, (Single)width, (Single)height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void FillPolygon(Brush brush, PointF[] points)
        {
            FillPolygon(brush, points, FillMode.Alternate);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void FillPolygon(Brush brush, Point[] points)
        {
            PointF[] pts = Point2PointF(points);
            FillPolygon(brush, pts, FillMode.Alternate);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void FillPolygon(Brush brush, PointF[] points, FillMode fillmode)
        {
            SvgPolygonElement pl = new SvgPolygonElement(points);
            pl.Style = HandleBrush(brush);
            if (fillmode == FillMode.Alternate)
            {
                pl.Style.Set("fill-rule", "evenodd");
            }
            else
            {
                pl.Style.Set("fill-rule", "nonzero");
            }

            if (!_transforms.Result.IsIdentity)
                pl.Transform = new SvgTransformList(_transforms.Result.Clone());
            _cur.AddChild(pl);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void FillPolygon(Brush brush, Point[] points, FillMode fillmode)
        {
            PointF[] pts = Point2PointF(points);
            FillPolygon(brush, pts, fillmode);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void FillRectangle(Brush brush, RectangleF rect)
        {
            FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void FillRectangle(Brush brush, Single x, Single y, Single width, Single height)
        {
            SvgRectElement rc = new SvgRectElement(x, y, width, height);
            rc.Style = HandleBrush(brush);
            if (!_transforms.Result.IsIdentity)
                rc.Transform = _transforms.Result.Clone();
            _cur.AddChild(rc);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void FillRectangle(Brush brush, Rectangle rect)
        {
            FillRectangle(brush, (Single)rect.X, (Single)rect.Y, (Single)rect.Width, (Single)rect.Height);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void FillRectangle(Brush brush, Int32 x, Int32 y, Int32 width, Int32 height)
        {
            FillRectangle(brush, (Single)x, (Single)y, (Single)width, (Single)height);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void FillRectangles(Brush brush, RectangleF[] rects)
        {
            foreach (RectangleF rc in rects)
            {
                FillRectangle(brush, rc);
            }
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void FillRectangles(Brush brush, Rectangle[] rects)
        {
            foreach (Rectangle rc in rects)
            {
                FillRectangle(brush, rc);
            }
        }

        /// <summary>
        /// Not implemented, because GDI+ regions/paths are not emulated.
        /// </summary>
        public void FillRegion(Brush brush, Region region) { throw new SvgGdiNotImpl("FillRegion (Brush brush, Region region)"); }

        /// <summary>
        /// Does nothing
        /// </summary>
        public void Flush()
        {
            //nothing to do
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="intention"></param>
        public void Flush(FlushIntention intention)
        {
            //nothing to do
        }

        /// <summary>
        /// Not meaningful when there is no actual display device.
        /// </summary>
        public System.Drawing.Color GetNearestColor(Color color) { throw new SvgGdiNotImpl("GetNearestColor (Color color)"); }

        /// <summary>
        /// Not implemented, because SvgGdi does not try and emulate GDI+ regions.  Only rectangular clip regions work.
        /// </summary>
        public void IntersectClip(Rectangle rect) { throw new SvgGdiNotImpl("IntersectClip (Rectangle rect)"); }

        /// <summary>
        /// Not implemented, because SvgGdi does not try and emulate GDI+ regions.  Only rectangular clip regions work.
        /// </summary>
        public void IntersectClip(RectangleF rect) { throw new SvgGdiNotImpl("IntersectClip (RectangleF rect)"); }

        /// <summary>
        /// Not implemented, because SvgGdi does not try and emulate GDI+ regions.  Only rectangular clip regions work.
        /// </summary>
        public void IntersectClip(Region region) { throw new SvgGdiNotImpl("IntersectClip (Region region)"); }

        /// <summary>
        /// Not implemented, because SvgGdi does not try and emulate GDI+ regions.
        /// </summary>
        public bool IsVisible(Int32 x, Int32 y) { throw new SvgGdiNotImpl("IsVisible (Int32 x, Int32 y)"); }

        /// <summary>
        /// Not implemented, because SvgGdi does not try and emulate GDI+ regions.
        /// </summary>
        public bool IsVisible(Point point) { throw new SvgGdiNotImpl("IsVisible (Point point)"); }

        /// <summary>
        /// Not implemented, because SvgGdi does not try and emulate GDI+ regions.
        /// </summary>
        public bool IsVisible(Single x, Single y) { throw new SvgGdiNotImpl("IsVisible (Single x, Single y)"); }

        /// <summary>
        /// Not implemented, because SvgGdi does not try and emulate GDI+ regions.
        /// </summary>
        public bool IsVisible(PointF point) { throw new SvgGdiNotImpl("IsVisible (PointF point)"); }

        /// <summary>
        /// Not implemented, because SvgGdi does not try and emulate GDI+ regions.
        /// </summary>
        public bool IsVisible(Int32 x, Int32 y, Int32 width, Int32 height) { throw new SvgGdiNotImpl("IsVisible (Int32 x, Int32 y, Int32 width, Int32 height)"); }

        /// <summary>
        /// Not implemented, because SvgGdi does not try and emulate GDI+ regions.
        /// </summary>
        public bool IsVisible(Rectangle rect) { throw new SvgGdiNotImpl("IsVisible (Rectangle rect)"); }

        /// <summary>
        /// Not implemented, because SvgGdi does not try and emulate GDI+ regions.
        /// </summary>
        public bool IsVisible(Single x, Single y, Single width, Single height) { throw new SvgGdiNotImpl("IsVisible (Single x, Single y, Single width, Single height)"); }

        /// <summary>
        /// Not implemented, because SvgGdi does not try and emulate GDI+ regions.
        /// </summary>
        public bool IsVisible(RectangleF rect) { throw new SvgGdiNotImpl("IsVisible (RectangleF rect)"); }

        /// <summary>
        ///  This method is implemented and produces a result which is often correct, but it is impossible to guarantee because 'MeasureString' is a fundamentally inapplicable to device independent output like SVG.
        /// </summary>
        public System.Drawing.Region[] MeasureCharacterRanges(String text, Font font, RectangleF layoutRect, StringFormat stringFormat)
        {
            return GetDefaultGraphics().MeasureCharacterRanges(text, font, layoutRect, stringFormat);
        }

        /// <summary>
        /// This method is implemented and produces a result which is often correct, but it is impossible to guarantee because 'MeasureString' is a fundamentally inapplicable to device independent output like SVG.
        /// </summary>
        public System.Drawing.SizeF MeasureString(String text, Font font, SizeF layoutArea, StringFormat stringFormat, out int charactersFitted, out int linesFilled)
        {
            return GetDefaultGraphics().MeasureString(text, font, layoutArea, stringFormat, out charactersFitted, out linesFilled);
        }

        /// <summary>
        /// This method is implemented and produces a result which is often correct, but it is impossible to guarantee because 'MeasureString' is a fundamentally inapplicable to device independent output like SVG.
        /// </summary>
        public System.Drawing.SizeF MeasureString(String text, Font font, PointF origin, StringFormat stringFormat)
        {
            return GetDefaultGraphics().MeasureString(text, font, origin, stringFormat);
        }

        /// <summary>
        /// This method is implemented and produces a result which is often correct, but it is impossible to guarantee because 'MeasureString' is a fundamentally inapplicable to device independent output like SVG.
        /// </summary>
        public System.Drawing.SizeF MeasureString(String text, Font font, SizeF layoutArea)
        {
            return GetDefaultGraphics().MeasureString(text, font, layoutArea);
        }

        /// <summary>
        /// This method is implemented and produces a result which is often correct, but it is impossible to guarantee because 'MeasureString' is a fundamentally inapplicable to device independent output like SVG.
        /// </summary>
        public System.Drawing.SizeF MeasureString(String text, Font font, SizeF layoutArea, StringFormat stringFormat)
        {
            return GetDefaultGraphics().MeasureString(text, font, layoutArea, stringFormat);
        }

        /// <summary>
        ///  This method is implemented and produces a result which is often correct, but it is impossible to guarantee because 'MeasureString' is a fundamentally inapplicable to device independent output like SVG.
        /// </summary>
        public System.Drawing.SizeF MeasureString(String text, Font font)
        {
            return GetDefaultGraphics().MeasureString(text, font);
        }

        /// <summary>
        ///  This method is implemented and produces a result which is often correct, but it is impossible to guarantee because 'MeasureString' is a fundamentally inapplicable to device independent output like SVG.
        /// </summary>
        public System.Drawing.SizeF MeasureString(String text, Font font, Int32 width)
        {
            return GetDefaultGraphics().MeasureString(text, font, width);
        }

        /// <summary>
        /// This method is implemented and produces a result which is often correct, but it is impossible to guarantee because 'MeasureString' is a fundamentally inapplicable to device independent output like SVG.
        /// </summary>
        public System.Drawing.SizeF MeasureString(String text, Font font, Int32 width, StringFormat format)
        {
            return GetDefaultGraphics().MeasureString(text, font, width, format);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void MultiplyTransform(Matrix matrix)
        {
            _transforms.Top.Multiply(matrix);
        }

        /// <summary>
        /// Implemented, but ignores <c>order</c>
        /// </summary>
        public void MultiplyTransform(Matrix matrix, MatrixOrder order)
        {
            _transforms.Top.Multiply(matrix, order);
        }

        /// <summary>
        /// Implemented.
        /// </summary>
        public void ResetClip()
        {
            _cur.Style.Set("clip-path", null);
        }

        /// <summary>
        /// Implemented
        /// </summary>
        public void ResetTransform()
        {
            _transforms.Pop();
            _transforms.Dup();
        }

        /// <summary>
        /// Not implemented, because SvgGdi does not try and emulate GDI+ regions.
        /// </summary>
        public void Restore(GraphicsState gstate) { throw new SvgGdiNotImpl("Restore (GraphicsState gstate)"); }

        /// <summary>
        /// Implemented
        /// </summary>
        public void RotateTransform(Single angle)
        {
            _transforms.Top.Rotate(angle);
        }

        /// <summary>
        /// Implemented, but ignores <c>order</c>
        /// </summary>
        public void RotateTransform(Single angle, MatrixOrder order)
        {
            _transforms.Top.Rotate(angle, order);
        }

        /// <summary>
        /// Not implemented, because SvgGdi does not try and emulate GDI+ regions.
        /// </summary>
        public GraphicsState Save() { throw new SvgGdiNotImpl("Save ()"); }

        /// <summary>
        /// Implemented
        /// </summary>
        public void ScaleTransform(Single sx, Single sy)
        {
            _transforms.Top.Scale(sx, sy);
        }

        /// <summary>
        /// Implemented, but ignores <c>order</c>
        /// </summary>
        public void ScaleTransform(Single sx, Single sy, MatrixOrder order)
        {
            _transforms.Top.Scale(sx, sy, order);
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void SetClip(Graphics g) { throw new SvgGdiNotImpl("SetClip (Graphics g)"); }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void SetClip(Graphics g, CombineMode combineMode) { throw new SvgGdiNotImpl("SetClip (Graphics g, CombineMode combineMode)"); }

        /// <summary>
        /// Implemented.
        /// </summary>
        public void SetClip(Rectangle rect)
        {
            SetClip(new RectangleF(rect.X, rect.Y, rect.Width, rect.Height));
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void SetClip(Rectangle rect, CombineMode combineMode) { throw new SvgGdiNotImpl("SetClip (Rectangle rect, CombineMode combineMode)"); }

        /// <summary>
        /// Implemented.
        /// </summary>
        public void SetClip(RectangleF rect)
        {
            SvgClipPathElement clipper = new SvgClipPathElement();
            clipper.Id += "_SetClip";
            SvgRectElement rc = new SvgRectElement(rect.X, rect.Y, rect.Width, rect.Height);
            clipper.AddChild(rc);
            _defs.AddChild(clipper);

            _cur.Style.Set("clip-path", new SvgUriReference(clipper));
        }

        /// <summary>
        /// Not implemented, because SvgGdi does not try and emulate GDI+ regions.  Only rectangular clip regions work.
        /// </summary>
        public void SetClip(RectangleF rect, CombineMode combineMode) { throw new SvgGdiNotImpl("SetClip (RectangleF rect, CombineMode combineMode)"); }

        /// <summary>
        /// Not implemented, because SvgGdi does not try and emulate GDI+ regions.  Only rectangular clip regions work.
        /// </summary>
        public void SetClip(GraphicsPath path) { throw new SvgGdiNotImpl("SetClip (GraphicsPath path)"); }

        /// <summary>
        /// Not implemented, because SvgGdi does not try and emulate GDI+ regions.  Only rectangular clip regions work.
        /// </summary>
        public void SetClip(GraphicsPath path, CombineMode combineMode) { throw new SvgGdiNotImpl("SetClip (GraphicsPath path, CombineMode combineMode)"); }

        /// <summary>
        /// Not implemented, because SvgGdi does not try and emulate GDI+ regions.  Only rectangular clip regions work.
        /// </summary>
        public void SetClip(Region region, CombineMode combineMode) { throw new SvgGdiNotImpl("SetClip (Region region, CombineMode combineMode)"); }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, PointF[] pts) { throw new SvgGdiNotImpl("TransformPoints (CoordinateSpace destSpace, CoordinateSpace srcSpace, PointF[] pts)"); }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, Point[] pts) { throw new SvgGdiNotImpl("TransformPoints (CoordinateSpace destSpace, CoordinateSpace srcSpace, Point[] pts)"); }

        /// <summary>
        /// Not implemented, because SvgGdi does not try and emulate GDI+ regions.
        /// </summary>
        public void TranslateClip(Single dx, Single dy) { throw new SvgGdiNotImpl("TranslateClip (Single dx, Single dy)"); }

        /// <summary>
        /// Not implemented, because SvgGdi does not try and emulate GDI+ regions.
        /// </summary>
        public void TranslateClip(Int32 dx, Int32 dy) { throw new SvgGdiNotImpl("TranslateClip (Int32 dx, Int32 dy)"); }

        /// <summary>
        /// Implemented
        /// </summary>
        public void TranslateTransform(Single dx, Single dy)
        {
            _transforms.Top.Translate(dx, dy);
        }

        /// <summary>
        /// Implemented, but ignores <c>order</c>
        /// </summary>
        public void TranslateTransform(Single dx, Single dy, MatrixOrder order)
        {
            _transforms.Top.Translate(dx, dy, order);
        }

        /// <summary>
        /// Get a string containing an SVG document.  The very heart of SvgGdi.  It calls <c>WriteSVGString</c> on the <see cref="SvgElement"/>
        /// at the root of this <c>SvgGraphics</c> and returns the resulting string.
        /// </summary>
        public string WriteSVGString()
        {
            return _root.WriteSVGString(true);
        }

        //a default graphics so that we can make a guess as to functions like MeasureString
        private static Graphics _g;

        private readonly SvgRectElement _bg;

        private readonly SvgDefsElement _defs;
        private readonly SvgSvgElement _root;
        private readonly SvgGroupElement _topgroup;
        private readonly MatrixStack _transforms;
        private SvgStyledTransformedElement _cur;
        private SmoothingMode _smoothingMode = SmoothingMode.Invalid;

        private static void AddHatchBrushDetails(SvgPatternElement patty, SvgColor col, HatchStyle hs)
        {
            SvgStyledTransformedElement l1 = null;
            SvgStyledTransformedElement l2 = null;
            SvgStyledTransformedElement l3 = null;
            SvgStyledTransformedElement l4 = null;

            switch (hs)
            {
                case HatchStyle.Cross:
                    l1 = new SvgLineElement(4, 0, 4, 8);
                    l2 = new SvgLineElement(0, 4, 8, 4);
                    break;

                case HatchStyle.BackwardDiagonal:
                    l1 = new SvgLineElement(8, 0, 0, 8);
                    break;

                case HatchStyle.LightDownwardDiagonal:
                case HatchStyle.DarkDownwardDiagonal:
                    l1 = new SvgLineElement(4, 0, 8, 4);
                    l2 = new SvgLineElement(0, 4, 4, 8);
                    l3 = new SvgLineElement(0, 0, 8, 8);
                    break;

                case HatchStyle.LightHorizontal:
                case HatchStyle.DarkHorizontal:
                    l1 = new SvgLineElement(0, 2, 8, 2);
                    l2 = new SvgLineElement(0, 6, 8, 6);
                    break;

                case HatchStyle.LightUpwardDiagonal:
                case HatchStyle.DarkUpwardDiagonal:
                    l1 = new SvgLineElement(0, 4, 4, 0);
                    l2 = new SvgLineElement(4, 8, 8, 4);
                    l3 = new SvgLineElement(0, 8, 8, 0);
                    break;

                case HatchStyle.LightVertical:
                case HatchStyle.DarkVertical:
                    l1 = new SvgLineElement(2, 0, 2, 8);
                    l2 = new SvgLineElement(6, 0, 6, 8);
                    break;

                case HatchStyle.DashedDownwardDiagonal:
                    l1 = new SvgLineElement(0, 0, 4, 4);
                    l2 = new SvgLineElement(4, 0, 8, 4);
                    break;

                case HatchStyle.DashedHorizontal:
                    l1 = new SvgLineElement(0, 2, 4, 2);
                    l2 = new SvgLineElement(4, 6, 8, 6);
                    break;

                case HatchStyle.DashedUpwardDiagonal:
                    l1 = new SvgLineElement(4, 0, 0, 4);
                    l2 = new SvgLineElement(8, 0, 4, 4);
                    break;

                case HatchStyle.DashedVertical:
                    l1 = new SvgLineElement(2, 0, 2, 4);
                    l2 = new SvgLineElement(6, 4, 6, 8);
                    break;

                case HatchStyle.DiagonalBrick:
                    l1 = new SvgLineElement(0, 8, 8, 0);
                    l2 = new SvgLineElement(0, 0, 4, 4);
                    l3 = new SvgLineElement(7, 9, 9, 7);
                    break;

                case HatchStyle.DiagonalCross:
                    l1 = new SvgLineElement(0, 0, 8, 8);
                    l2 = new SvgLineElement(8, 0, 0, 8);
                    break;

                case HatchStyle.Divot:
                    l1 = new SvgLineElement(2, 2, 4, 4);
                    l2 = new SvgLineElement(4, 4, 2, 6);
                    break;

                case HatchStyle.DottedDiamond:
                    l1 = new SvgLineElement(0, 0, 8, 8);
                    l2 = new SvgLineElement(0, 8, 8, 0);
                    break;

                case HatchStyle.DottedGrid:
                    l1 = new SvgLineElement(4, 0, 4, 8);
                    l2 = new SvgLineElement(0, 4, 8, 4);
                    break;

                case HatchStyle.ForwardDiagonal:
                    l1 = new SvgLineElement(0, 0, 8, 8);
                    break;

                case HatchStyle.Horizontal:
                    l1 = new SvgLineElement(0, 4, 8, 4);
                    break;

                case HatchStyle.HorizontalBrick:
                    l1 = new SvgLineElement(0, 3, 8, 3);
                    l2 = new SvgLineElement(3, 0, 3, 3);
                    l3 = new SvgLineElement(0, 3, 0, 7);
                    l4 = new SvgLineElement(0, 7, 7, 7);
                    break;

                case HatchStyle.LargeCheckerBoard:
                    l1 = new SvgRectElement(0, 0, 3f, 3f);
                    l2 = new SvgRectElement(4, 4, 4, 4f);
                    break;

                case HatchStyle.LargeConfetti:
                    l1 = new SvgRectElement(0, 0, 1, 1);
                    l2 = new SvgRectElement(2, 3, 1, 1);
                    l3 = new SvgRectElement(5, 2, 1, 1);
                    l4 = new SvgRectElement(6, 6, 1, 1);
                    break;

                case HatchStyle.NarrowHorizontal:
                    l1 = new SvgLineElement(0, 1, 8, 1);
                    l2 = new SvgLineElement(0, 3, 8, 3);
                    l3 = new SvgLineElement(0, 5, 8, 5);
                    l4 = new SvgLineElement(0, 7, 8, 7);
                    break;

                case HatchStyle.NarrowVertical:
                    l1 = new SvgLineElement(1, 0, 1, 8);
                    l2 = new SvgLineElement(3, 0, 3, 8);
                    l3 = new SvgLineElement(5, 0, 5, 8);
                    l4 = new SvgLineElement(7, 0, 7, 8);
                    break;

                case HatchStyle.OutlinedDiamond:
                    l1 = new SvgLineElement(0, 0, 8, 8);
                    l2 = new SvgLineElement(8, 0, 0, 8);
                    break;

                case HatchStyle.Plaid:
                    l1 = new SvgLineElement(0, 0, 8, 0);
                    l2 = new SvgLineElement(0, 3, 8, 3);
                    l3 = new SvgRectElement(0, 4, 3, 3);
                    break;

                case HatchStyle.Shingle:
                    l1 = new SvgLineElement(0, 2, 2, 0);
                    l2 = new SvgLineElement(2, 0, 7, 5);
                    l3 = new SvgLineElement(0, 3, 3, 7);
                    break;

                case HatchStyle.SmallCheckerBoard:
                    l1 = new SvgRectElement(0, 0, 1, 1);
                    l2 = new SvgRectElement(4, 4, 1, 1);
                    l3 = new SvgRectElement(4, 0, 1, 1);
                    l4 = new SvgRectElement(0, 4, 1, 1);
                    break;

                case HatchStyle.SmallConfetti:
                    l1 = new SvgLineElement(0, 0, 2, 2);
                    l2 = new SvgLineElement(7, 3, 5, 5);
                    l3 = new SvgLineElement(2, 6, 4, 4);
                    break;

                case HatchStyle.SmallGrid:
                    l1 = new SvgLineElement(0, 2, 8, 2);
                    l2 = new SvgLineElement(0, 6, 8, 6);
                    l3 = new SvgLineElement(2, 0, 2, 8);
                    l4 = new SvgLineElement(6, 0, 6, 8);
                    break;

                case HatchStyle.SolidDiamond:
                    l1 = new SvgPolygonElement("3 0 6 3 3 6 0 3");
                    break;

                case HatchStyle.Sphere:
                    l1 = new SvgEllipseElement(3, 3, 2, 2);
                    break;

                case HatchStyle.Trellis:
                    l1 = new SvgLineElement(0, 1, 8, 1);
                    l2 = new SvgLineElement(0, 3, 8, 3);
                    l3 = new SvgLineElement(0, 5, 8, 5);
                    l4 = new SvgLineElement(0, 7, 8, 7);
                    break;

                case HatchStyle.Vertical:
                    l4 = new SvgLineElement(0, 0, 0, 8);
                    break;

                case HatchStyle.Wave:
                    l3 = new SvgLineElement(0, 4, 3, 2);
                    l4 = new SvgLineElement(3, 2, 8, 4);
                    break;

                case HatchStyle.Weave:
                    l1 = new SvgLineElement(0, 4, 4, 0);
                    l2 = new SvgLineElement(8, 4, 4, 8);
                    l3 = new SvgLineElement(0, 0, 0, 4);
                    l4 = new SvgLineElement(0, 4, 4, 8);
                    break;

                case HatchStyle.WideDownwardDiagonal:
                    l1 = new SvgLineElement(0, 0, 8, 8);
                    l2 = new SvgLineElement(0, 1, 8, 9);
                    l3 = new SvgLineElement(7, 0, 8, 1);
                    break;

                case HatchStyle.WideUpwardDiagonal:
                    l1 = new SvgLineElement(8, 0, 0, 8);
                    l2 = new SvgLineElement(8, 1, 0, 9);
                    l3 = new SvgLineElement(0, 1, -1, 0);
                    break;

                case HatchStyle.ZigZag:
                    l1 = new SvgLineElement(0, 4, 4, 0);
                    l2 = new SvgLineElement(4, 0, 8, 4);
                    break;

                case HatchStyle.Percent05:
                    l1 = new SvgLineElement(0, 0, 1, 0);
                    l2 = new SvgLineElement(4, 4, 5, 4);
                    break;

                case HatchStyle.Percent10:
                    l1 = new SvgLineElement(0, 0, 1, 0);
                    l2 = new SvgLineElement(4, 2, 5, 2);
                    l3 = new SvgLineElement(2, 4, 3, 4);
                    l4 = new SvgLineElement(6, 6, 7, 6);
                    break;

                case HatchStyle.Percent20:
                    l1 = new SvgLineElement(0, 0, 2, 0);
                    l2 = new SvgLineElement(4, 2, 6, 2);
                    l3 = new SvgLineElement(2, 4, 4, 4);
                    l4 = new SvgLineElement(5, 6, 7, 6);
                    break;

                case HatchStyle.Percent25:
                    l1 = new SvgLineElement(0, 0, 3, 0);
                    l2 = new SvgLineElement(4, 2, 6, 2);
                    l3 = new SvgLineElement(2, 4, 5, 4);
                    l4 = new SvgLineElement(5, 6, 7, 6);
                    break;

                case HatchStyle.Percent30:
                    l1 = new SvgRectElement(0, 0, 3, 1);
                    l2 = new SvgLineElement(4, 2, 6, 2);
                    l3 = new SvgRectElement(2, 4, 3, 1);
                    l4 = new SvgLineElement(5, 6, 7, 6);
                    break;

                case HatchStyle.Percent40:
                    l1 = new SvgRectElement(0, 0, 3, 1);
                    l2 = new SvgRectElement(4, 2, 3, 1);
                    l3 = new SvgRectElement(2, 4, 3, 1);
                    l4 = new SvgRectElement(5, 6, 3, 1);
                    break;

                case HatchStyle.Percent50:
                    l1 = new SvgRectElement(0, 0, 3, 3);
                    l2 = new SvgRectElement(4, 4, 4, 4f);
                    break;

                case HatchStyle.Percent60:
                    l1 = new SvgRectElement(0, 0, 4, 3);
                    l2 = new SvgRectElement(4, 4, 4, 4f);
                    break;

                case HatchStyle.Percent70:
                    l1 = new SvgRectElement(0, 0, 4, 5);
                    l2 = new SvgRectElement(4, 4, 4, 4f);
                    break;

                case HatchStyle.Percent75:
                    l1 = new SvgRectElement(0, 0, 7, 3);
                    l2 = new SvgRectElement(0, 2, 3, 7);
                    break;

                case HatchStyle.Percent80:
                    l1 = new SvgRectElement(0, 0, 7, 4);
                    l2 = new SvgRectElement(0, 2, 4, 7);
                    break;

                case HatchStyle.Percent90:
                    l1 = new SvgRectElement(0, 0, 7, 5);
                    l2 = new SvgRectElement(0, 2, 5, 7);
                    break;

                default:

                    break;
            }

            if (l1 != null)
            {
                l1.Style.Set("stroke", col);
                l1.Style.Set("fill", col);
                patty.AddChild(l1);
            }
            if (l2 != null)
            {
                l2.Style.Set("stroke", col);
                l2.Style.Set("fill", col);
                patty.AddChild(l2);
            }
            if (l3 != null)
            {
                l3.Style.Set("stroke", col);
                l3.Style.Set("fill", col);
                patty.AddChild(l3);
            }
            if (l4 != null)
            {
                l4.Style.Set("stroke", col);
                l4.Style.Set("fill", col);
                patty.AddChild(l4);
            }
        }

        private static PointF ControlPoint(PointF l, PointF pt, float t)
        {
            PointF v = new PointF(l.X - pt.X, l.Y - pt.Y);

            float vlen = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
            v.X /= (float)Math.Sqrt(vlen / (10 * t * t));
            v.Y /= (float)Math.Sqrt(vlen / (10 * t * t));

            return new PointF(pt.X + v.X, pt.Y + v.Y);
        }

        private static PointF[] ControlPoints(PointF l, PointF r, PointF pt, float t)
        {
            //points to vectors
            PointF lv = new PointF(l.X - pt.X, l.Y - pt.Y);
            PointF rv = new PointF(r.X - pt.X, r.Y - pt.Y);

            PointF nlv = new PointF(lv.X - rv.X, lv.Y - rv.Y);
            PointF nrv = new PointF(rv.X - lv.X, rv.Y - lv.Y);

            float nlvlen = (float)Math.Sqrt(nlv.X * nlv.X + nlv.Y * nlv.Y);
            nlv.X /= (float)Math.Sqrt(nlvlen / (10 * t * t));
            nlv.Y /= (float)Math.Sqrt(nlvlen / (10 * t * t));

            float nrvlen = (float)Math.Sqrt(nrv.X * nrv.X + nrv.Y * nrv.Y);
            nrv.X /= (float)Math.Sqrt(nrvlen / (10 * t * t));
            nrv.Y /= (float)Math.Sqrt(nrvlen / (10 * t * t));

            PointF[] ret = new PointF[2];

            ret[0] = new PointF(pt.X + nlv.X, pt.Y + nlv.Y);
            ret[1] = new PointF(pt.X + nrv.X, pt.Y + nrv.Y);

            return ret;
        }

        private static void DrawImagePixel(SvgElement container, Color c, float x, float y, float w, float h)
        {
            if (c.A == 0)
                return;

            var rc = new SvgRectElement(x, y, w, h)
            {
                Id = ""
            };
            rc.Style.Set("fill", "rgb(" + c.R + "," + c.G + "," + c.B + ")");
            if (c.A < 255)
                rc.Style.Set("opacity", c.A / 255f);

            container.AddChild(rc);
        }

        private static string GDIArc2SVGPath(float x, float y, float width, float height, float startAngle, float sweepAngle, bool pie)
        {
            int longArc = 0;

            PointF start = new PointF();
            PointF end = new PointF();
            PointF center = new PointF(x + width / 2f, y + height / 2f);

            startAngle = (startAngle / 360f) * 2f * (float)Math.PI;
            sweepAngle = (sweepAngle / 360f) * 2f * (float)Math.PI;

            sweepAngle += startAngle;

            if (sweepAngle > startAngle)
            {
                float tmp = startAngle;
                startAngle = sweepAngle;
                sweepAngle = tmp;
            }

            if (sweepAngle - startAngle > Math.PI || startAngle - sweepAngle > Math.PI)
            {
                longArc = 1;
            }

            start.X = (float)Math.Cos(startAngle) * (width / 2f) + center.X;
            start.Y = (float)Math.Sin(startAngle) * (height / 2f) + center.Y;

            end.X = (float)Math.Cos(sweepAngle) * (width / 2f) + center.X;
            end.Y = (float)Math.Sin(sweepAngle) * (height / 2f) + center.Y;

            string s = "M " + start.X.ToString("F", CultureInfo.InvariantCulture) + "," + start.Y.ToString("F", CultureInfo.InvariantCulture) +
                " A " + (width / 2f).ToString("F", CultureInfo.InvariantCulture) + " " + (height / 2f).ToString("F", CultureInfo.InvariantCulture) + " " +
                "0 " + longArc.ToString() + " 0 " + end.X.ToString("F", CultureInfo.InvariantCulture) + " " + end.Y.ToString("F", CultureInfo.InvariantCulture);

            if (pie)
            {
                s += " L " + center.X.ToString("F", CultureInfo.InvariantCulture) + "," + center.Y.ToString("F", CultureInfo.InvariantCulture);
                s += " L " + start.X.ToString("F", CultureInfo.InvariantCulture) + "," + start.Y.ToString("F", CultureInfo.InvariantCulture);
            }

            return s;
        }

        private static Graphics GetDefaultGraphics()
        {
            if (_g == null)
            {
                Bitmap b = new Bitmap(1, 1);
                _g = Graphics.FromImage(b);
            }

            return _g;
        }

        private static float GetFontDescentPercentage(Font font)
        {
            return (float)font.FontFamily.GetCellDescent(font.Style) / font.FontFamily.GetEmHeight(font.Style);
        }

        /// <summary>
        /// Decides whether the pen's anchor type is simple enough to be drawn by a fast approximation using the DrawEndAnchor
        /// </summary>
        private static bool IsEndAnchorSimple(LineCap lc)
        {
            switch (lc)
            {
                case LineCap.NoAnchor:
                case LineCap.Flat:
                case LineCap.ArrowAnchor:
                case LineCap.DiamondAnchor:
                case LineCap.RoundAnchor:
                case LineCap.SquareAnchor:
                    return true;

                default:
                    return false;
            }
        }

        private static PointF[] Point2PointF(Point[] p)
        {
            PointF[] pf = new PointF[p.Length];
            for (int i = 0; i < p.Length; ++i)
            {
                pf[i] = new PointF(p[i].X, p[i].Y);
            }

            return pf;
        }

        //This seems to be a very good approximation.  GDI must be using a similar simplistic method for some odd reason.
        //If a curve is closed, it uses all points, and ignores start and num.
        private static PointF[] Spline2Bez(PointF[] points, int start, int num, bool closed, float tension)
        {
            ArrayList cp = new ArrayList();
            ArrayList res = new ArrayList();

            int l = points.Length - 1;

            res.Add(points[0]);
            res.Add(ControlPoint(points[1], points[0], tension));

            for (int i = 1; i < l; ++i)
            {
                PointF[] pts = ControlPoints(points[i - 1], points[i + 1], points[i], tension);
                res.Add(pts[0]);
                res.Add(points[i]);
                res.Add(pts[1]);
            }

            res.Add(ControlPoint(points[l - 1], points[l], tension));
            res.Add(points[l]);

            if (closed)
            {
                //adjust rh cp of point 0
                PointF[] pts = ControlPoints(points[l], points[1], points[0], tension);
                res[1] = pts[1];

                //adjust lh cp of point l and add rh cp
                pts = ControlPoints(points[l - 1], points[0], points[l], tension);
                res[res.Count - 2] = pts[0];
                res.Add(pts[1]);

                //add new end point and its lh cp
                pts = ControlPoints(points[l], points[1], points[0], tension);
                res.Add(pts[0]);
                res.Add(points[0]);

                return (PointF[])res.ToArray(typeof(PointF));
            }
            else
            {
                ArrayList subset = new ArrayList();

                for (int i = start * 3; i < (start + num) * 3; ++i)
                {
                    subset.Add(res[i]);
                }

                subset.Add(res[(start + num) * 3]);

                return (PointF[])subset.ToArray(typeof(PointF));
            }
        }

        private void DrawBitmapData(Bitmap b, float x, float y, float w, float h, bool scale)
        {
            SvgGroupElement g = new SvgGroupElement("bitmap_at_" + x.ToString("F", CultureInfo.InvariantCulture) + "_" + y.ToString("F", CultureInfo.InvariantCulture));

            float scalex = 1, scaley = 1;

            if (scale)
            {
                scalex = w / b.Width;
                scaley = h / b.Height;
            }

            SvgImageElement imageElement = new SvgImageElement() {Width = b.Width, Height = b.Height, X = x, Y = y};
            var ms = new MemoryStream();
            b.Save(ms, ImageFormat.Png);

            imageElement.Href = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());

            g.AddChild(imageElement);
/*
            for (int line = 0; line < b.Height; ++line)
            {
                for (int col = 0; col < b.Width; ++col)
                {
                    //This is SO slow, but better than making the whole library 'unsafe'
                    Color c = b.GetPixel(col, line);

                    if (!scale)
                    {
                        if (col <= w && line <= h)
                            DrawImagePixel(g, c, x + col, y + line, 1, 1);
                    }
                    else
                    {
                        DrawImagePixel(g, c, x + (col * scalex), y + (line * scaley), scalex, scaley);
                    }
                }
            }*/

            if (!_transforms.Result.IsIdentity)
                g.Transform = _transforms.Result.Clone();
            _cur.AddChild(g);
        }

        private void DrawEndAnchor(LineCap lc, CustomLineCap clc, Color col, float w, PointF pt, float angle, bool ignoreUnsupportedLineCaps)
        {
            SvgStyledTransformedElement anchor = null;
            PointF[] points = null;

            switch (lc)
            {
                case LineCap.NoAnchor:
                    break;

                case LineCap.Flat:
                    // TODO: what is the correct look?
                    break;

                case LineCap.ArrowAnchor:
                    points = new PointF[3];
                    points[0] = new PointF(0, -w / 2f);
                    points[1] = new PointF(-w, w);
                    points[2] = new PointF(w, w);
                    anchor = new SvgPolygonElement(points);
                    break;

                case LineCap.DiamondAnchor:
                    points = new PointF[4];
                    points[0] = new PointF(0, -w);
                    points[1] = new PointF(w, 0);
                    points[2] = new PointF(0, w);
                    points[3] = new PointF(-w, 0);
                    anchor = new SvgPolygonElement(points);
                    break;

                case LineCap.RoundAnchor:
                    anchor = new SvgEllipseElement(0, 0, w, w);
                    break;

                case LineCap.SquareAnchor:
                    float ww = (w / 3) * 2;
                    anchor = new SvgRectElement(0 - ww, 0 - ww, ww * 2, ww * 2);
                    break;

                case LineCap.Custom:
                    if (clc != null)
                    {
                        if (!ignoreUnsupportedLineCaps)
                            throw new SvgGdiNotImpl("DrawEndAnchor custom");
                    }
                    break;

                default:
                    if (!ignoreUnsupportedLineCaps)
                        throw new SvgGdiNotImpl("DrawEndAnchor " + lc.ToString());
                    break;
            }

            if (anchor == null)
                return;

            anchor.Id += "_line_anchor";
            anchor.Style.Set("fill", new SvgColor(col));
            anchor.Style.Set("stroke", "none");

            Matrix rotation = new Matrix();
            rotation.Rotate((angle / (float)Math.PI) * 180);
            Matrix translation = new Matrix();
            translation.Translate(pt.X, pt.Y);

            anchor.Transform = new SvgTransformList(_transforms.Result.Clone());
            anchor.Transform.Add(translation);
            anchor.Transform.Add(rotation);
            _cur.AddChild(anchor);
        }

        private void DrawEndAnchors(Pen pen, PointF start, PointF end, bool ignoreUnsupportedLineCaps = false)
        {
            float startAngle = (float)Math.Atan((start.X - end.X) / (start.Y - end.Y)) * -1;
            float endAngle = (float)Math.Atan((end.X - start.X) / (end.Y - start.Y)) * -1;

            CustomLineCap clcstart = null;
            CustomLineCap clcend = null;

            //GDI+ native dll throws an exception if someone forgot to specify custom cap
            try
            {
                clcstart = pen.CustomStartCap;
            }
            catch (Exception)
            {
            }
            try
            {
                clcend = pen.CustomEndCap;
            }
            catch (Exception)
            {
            }

            DrawEndAnchor(pen.StartCap, clcstart, pen.Color, pen.Width, start, startAngle, ignoreUnsupportedLineCaps);
            DrawEndAnchor(pen.EndCap, clcend, pen.Color, pen.Width, end, endAngle, ignoreUnsupportedLineCaps);
        }

        private void DrawText(String s, Font font, Brush brush, RectangleF rect, StringFormat fmt, bool ignoreRect)
        {
            if (s != null && s.Contains("\n"))
                throw new SvgGdiNotImpl("DrawText multiline text");

            SvgTextElement txt = new SvgTextElement(s, rect.X, rect.Y);

            //GDI takes x and y as the upper left corner; svg takes them as the lower left.
            //We must therefore move the text one line down, but SVG does not understand about lines,
            //so we do as best we can, applying a downward translation before the current GDI translation.

            txt.Transform = new SvgTransformList(_transforms.Result.Clone());

            txt.Style = HandleBrush(brush);
            txt.Style += new SvgStyle(font);

            switch (fmt.Alignment)
            {
                case StringAlignment.Near:
                    break;

                case StringAlignment.Center:
                    {
                        if (ignoreRect)
                            throw new SvgGdiNotImpl("DrawText automatic rect");

                        txt.Style.Set("text-anchor", "middle");
                        txt.X = rect.X + rect.Width / 2;
                    }
                    break;

                case StringAlignment.Far:
                    {
                        if (ignoreRect)
                            throw new SvgGdiNotImpl("DrawText automatic rect");

                        txt.Style.Set("text-anchor", "end");
                        txt.X = rect.Right;
                    }
                    break;

                default:
                    throw new SvgGdiNotImpl("DrawText horizontal alignment");
            }

            if (!ignoreRect && ((fmt.FormatFlags & StringFormatFlags.NoClip) != StringFormatFlags.NoClip))
            {
                SvgClipPathElement clipper = new SvgClipPathElement();
                clipper.Id += "_text_clipper";
                SvgRectElement rc = new SvgRectElement(rect.X, rect.Y, rect.Width, rect.Height);
                clipper.AddChild(rc);
                _defs.AddChild(clipper);

                txt.Style.Set("clip-path", new SvgUriReference(clipper));
            }

            switch (fmt.LineAlignment)
            {
                case StringAlignment.Near:
                    {
                        // TODO: ??
                        // txt.Style.Set("baseline-shift", "-86%");//a guess.
                        var span = new SvgTspanElement(s);
                        span.DY = new SvgLength(txt.Style.Get("font-size").ToString());
                        txt.Text = null;
                        txt.AddChild(span);
                    }
                    break;

                case StringAlignment.Center:
                    {
                        if (ignoreRect)
                            throw new SvgGdiNotImpl("DrawText automatic rect");

                        txt.Y.Value = txt.Y.Value + (rect.Height / 2);
                        var span = new SvgTspanElement(s);
                        span.DY = new SvgLength(txt.Style.Get("font-size").ToString());
                        span.DY.Value = span.DY.Value * ((1 - GetFontDescentPercentage(font)) - 0.5f);
                        txt.Text = null;
                        txt.AddChild(span);
                    }
                    break;

                case StringAlignment.Far:
                    {
                        if (ignoreRect)
                            throw new SvgGdiNotImpl("DrawText automatic rect");

                        txt.Y.Value = txt.Y.Value + rect.Height;
                        // This would solve the alignment as well, but it's not supported by Internet Explorer
                        //
                        // txt.Attributes["dominant-baseline"] = "text-after-edge";
                        var span = new SvgTspanElement(s);
                        span.DY = new SvgLength(txt.Style.Get("font-size").ToString());
                        span.DY.Value = span.DY.Value * ((1 - GetFontDescentPercentage(font)) - 1);
                        txt.Text = null;
                        txt.AddChild(span);
                    }
                    break;

                default:
                    throw new SvgGdiNotImpl("DrawText vertical alignment");
            }

            _cur.AddChild(txt);
        }

        private void FillBeziers(Brush brush, PointF[] points, FillMode fillmode)
        {
            SvgPathElement bez = new SvgPathElement();

            string s = "M " + points[0].X.ToString("F", CultureInfo.InvariantCulture) + " " + points[0].Y.ToString("F", CultureInfo.InvariantCulture) + " C ";

            for (int i = 1; i < points.Length; ++i)
            {
                s += points[i].X.ToString("F", CultureInfo.InvariantCulture) + " " + points[i].Y.ToString("F", CultureInfo.InvariantCulture) + " ";
            }

            s += "Z";

            bez.D = s;

            bez.Style = HandleBrush(brush);
            bez.Transform = new SvgTransformList(_transforms.Result.Clone());
            _cur.AddChild(bez);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// When a GDI instruction with a brush parameter is called, there can be a lot we have to do to emulate the brush.  The aim is to return a
        /// style that represents the brush.
        /// <para>
        /// Solid brush is very easy.
        /// </para>
        /// <para>
        /// Linear grad brush:  we ignore the blend curve and the transformation (and therefore the rotation parameter if any)
        /// Hatch brush:
        /// </para>
        /// <para>
        /// Other types of brushes are too hard to emulate and are rendered pink.
        /// </para>
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        private SvgStyle HandleBrush(Brush br)
        {
            if (br.GetType() == typeof(SolidBrush))
            {
                return new SvgStyle((SolidBrush)br);
            }

            if (br.GetType() == typeof(LinearGradientBrush))
            {
                LinearGradientBrush grbr = (LinearGradientBrush)br;
                RectangleF rc = grbr.Rectangle;

                SvgLinearGradient grad = new SvgLinearGradient(rc.Left, rc.Top, rc.Right, rc.Bottom);

                switch (grbr.WrapMode)
                {
                    //I have not been able to test Clamp because using a clamped gradient appears to crash the process
                    //under XP (?!?!)
                    case WrapMode.Clamp:
                        grad.SpreadMethod = "pad"; grad.GradientUnits = "objectBoundingBox"; break;
                    case WrapMode.Tile:
                        grad.SpreadMethod = "repeat"; grad.GradientUnits = "userSpaceOnUse"; break;
                    default:
                        grad.SpreadMethod = "reflect"; grad.GradientUnits = "userSpaceOnUse"; break;
                }

                ColorBlend cb = null;

                //GDI dll tends to crash when you try and access some members of gradient brushes that haven't been specified.
                try
                {
                    cb = grbr.InterpolationColors;
                }
                catch (Exception) { }

                if (cb != null)
                {
                    for (int i = 0; i < grbr.InterpolationColors.Colors.Length; ++i)
                    {
                        grad.AddChild(new SvgStopElement(grbr.InterpolationColors.Positions[i], grbr.InterpolationColors.Colors[i]));
                    }
                }
                else
                {
                    grad.AddChild(new SvgStopElement("0%", grbr.LinearColors[0]));
                    grad.AddChild(new SvgStopElement("100%", grbr.LinearColors[1]));
                }

                grad.Id += "_LinearGradientBrush";

                _defs.AddChild(grad);

                SvgStyle s = new SvgStyle();
                s.Set("fill", new SvgUriReference(grad));
                return s;
            }

            if (br.GetType() == typeof(HatchBrush))
            {
                HatchBrush habr = (HatchBrush)br;

                SvgPatternElement patty = new SvgPatternElement(0, 0, 8, 8, null);
                patty.Style.Set("shape-rendering", "crispEdges");
                patty.Style.Set("stroke-linecap", "butt");

                SvgRectElement rc = new SvgRectElement(0, 0, 8, 8);
                rc.Style.Set("fill", new SvgColor(habr.BackgroundColor));
                patty.AddChild(rc);

                AddHatchBrushDetails(patty, new SvgColor(habr.ForegroundColor), habr.HatchStyle);

                patty.Id += "_HatchBrush";
                patty.PatternUnits = "userSpaceOnUse";
                patty.PatternContentUnits = "userSpaceOnUse";
                _defs.AddChild(patty);

                SvgStyle s = new SvgStyle();
                s.Set("fill", new SvgUriReference(patty));
                return s;
            }

            //most types of brush we can't emulate, but luckily they are quite unusual
            return new SvgStyle(new SolidBrush(Color.Salmon));
        }

        /// <summary>
        /// This class is needed because GDI+ does not maintain a proper scene graph; rather it maintains a single transformation matrix
        /// which is applied to each new object.  The matrix is saved and reloaded when 'begincontainer' and 'endcontainer' are called.  SvgGraphics has to
        /// emulate this behaviour.
        /// <para>
        /// This matrix stack caches it's 'result' (ie. the current transformation, the product of all matrices).  The result is
        /// recalculated when necessary.
        /// </para>
        /// </summary>
        private class MatrixStack
        {
            public MatrixStack()
            {
                _mx = new ArrayList();

                //we need 2 identity matrices on the stack.  This is because we do a resettransform()
                //by pop dup (to set current xform to xform of enclosing group).
                Push();
                Push();
            }

            public Matrix Result
            {
                get
                {
                    if (_result != null)
                        return _result;

                    _result = new Matrix();

                    foreach (Matrix mat in _mx)
                    {
                        if (!mat.IsIdentity)
                            _result.Multiply(mat);
                    }

                    return _result;
                }
            }

            public Matrix Top
            {
                get
                {
                    //because we cannot return a const, we have to reset result
                    //even though the caller might not even want to change the matrix.  This a typical
                    //problem with weaker languages that don't have const.
                    _result = null;
                    return (Matrix)_mx[_mx.Count - 1];
                }

                set
                {
                    _mx[_mx.Count - 1] = value;
                    _result = null;
                }
            }

            public void Dup()
            {
                _mx.Insert(_mx.Count, Top.Clone());
                _result = null;
            }

            public void Pop()
            {
                if (_mx.Count <= 1)
                    return;

                _mx.RemoveAt(_mx.Count - 1);
                _result = null;
            }

            public void Push()
            {
                _mx.Add(new Matrix());
            }

            private readonly ArrayList _mx;
            private Matrix _result;
        }
    }
}
