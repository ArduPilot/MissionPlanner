#region license

/************************************************************************

DirectShowLib - Provide access to DirectShow interfaces via .NET
Copyright (C) 2007
http://sourceforge.net/projects/directshownet/

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

**************************************************************************/

#endregion

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Reflection;
using System.Security;

using DirectShowLib.Dvd;

#if !USING_NET11
using System.Runtime.InteropServices.ComTypes;
#endif

namespace DirectShowLib
{
    #region Declarations

    /// <summary>
    /// Not from DirectShow
    /// </summary>
    public enum PinConnectedStatus
    {
        Unconnected,

        Connected
    }

    /// <summary>
    /// From BITMAPINFO
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BitmapInfo
    {
        public BitmapInfoHeader bmiHeader;

        public int[] bmiColors;
    }

    /// <summary>
    /// From BITMAPINFOHEADER
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public class BitmapInfoHeader
    {
        public int Size;

        public int Width;

        public int Height;

        public short Planes;

        public short BitCount;

        public int Compression;

        public int ImageSize;

        public int XPelsPerMeter;

        public int YPelsPerMeter;

        public int ClrUsed;

        public int ClrImportant;
    }

    /// <summary>
    /// From DDPIXELFORMAT
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct DDPixelFormat
    {
        [FieldOffset(0)]
        public int dwSize;

        [FieldOffset(4)]
        public int dwFlags;

        [FieldOffset(8)]
        public int dwFourCC;

        [FieldOffset(12)]
        public int dwRGBBitCount;

        [FieldOffset(12)]
        public int dwYUVBitCount;

        [FieldOffset(12)]
        public int dwZBufferBitDepth;

        [FieldOffset(12)]
        public int dwAlphaBitDepth;

        [FieldOffset(16)]
        public int dwRBitMask;

        [FieldOffset(16)]
        public int dwYBitMask;

        [FieldOffset(20)]
        public int dwGBitMask;

        [FieldOffset(20)]
        public int dwUBitMask;

        [FieldOffset(24)]
        public int dwBBitMask;

        [FieldOffset(24)]
        public int dwVBitMask;

        [FieldOffset(28)]
        public int dwRGBAlphaBitMask;

        [FieldOffset(28)]
        public int dwYUVAlphaBitMask;

        [FieldOffset(28)]
        public int dwRGBZBitMask;

        [FieldOffset(28)]
        public int dwYUVZBitMask;
    }

    /// <summary>
    /// From CAUUID
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DsCAUUID
    {
        public int cElems;

        public IntPtr pElems;

        /// <summary>
        /// Perform a manual marshaling of pElems to retrieve an array of System.Guid.
        /// Assume this structure has been already filled by the ISpecifyPropertyPages.GetPages() method.
        /// </summary>
        /// <returns>A managed representation of pElems (cElems items)</returns>
        public Guid[] ToGuidArray()
        {
            Guid[] retval = new Guid[this.cElems];

            for (int i = 0; i < this.cElems; i++)
            {
                IntPtr ptr = new IntPtr(this.pElems.ToInt64() + (Marshal.SizeOf(typeof(Guid)) * i));
                retval[i] = (Guid)Marshal.PtrToStructure(ptr, typeof(Guid));
            }

            return retval;
        }
    }

    /// <summary>
    /// DirectShowLib.DsLong is a wrapper class around a <see cref="System.Int64"/> value type.
    /// </summary>
    /// <remarks>
    /// This class is necessary to enable null paramters passing.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public class DsLong
    {
        private long Value;

        /// <summary>
        /// Constructor
        /// Initialize a new instance of DirectShowLib.DsLong with the Value parameter
        /// </summary>
        /// <param name="Value">Value to assign to this new instance</param>
        public DsLong(long Value)
        {
            this.Value = Value;
        }

        /// <summary>
        /// Get a string representation of this DirectShowLib.DsLong Instance.
        /// </summary>
        /// <returns>A string representing this instance</returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        /// <summary>
        /// Define implicit cast between DirectShowLib.DsLong and System.Int64 for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsLong.ToInt64"/> for similar functionality.
        /// <code>
        ///   // Define a new DsLong instance
        ///   DsLong dsL = new DsLong(9876543210);
        ///   // Do implicit cast between DsLong and Int64
        ///   long l = dsL;
        ///
        ///   Console.WriteLine(l.ToString());
        /// </code>
        /// </summary>
        /// <param name="g">DirectShowLib.DsLong to be cast</param>
        /// <returns>A casted System.Int64</returns>
        public static implicit operator long(DsLong l)
        {
            return l.Value;
        }

        /// <summary>
        /// Define implicit cast between System.Int64 and DirectShowLib.DsLong for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsGuid.FromInt64"/> for similar functionality.
        /// <code>
        ///   // Define a new Int64 instance
        ///   long l = 9876543210;
        ///   // Do implicit cast between Int64 and DsLong
        ///   DsLong dsl = l;
        ///
        ///   Console.WriteLine(dsl.ToString());
        /// </code>
        /// </summary>
        /// <param name="g">System.Int64 to be cast</param>
        /// <returns>A casted DirectShowLib.DsLong</returns>
        public static implicit operator DsLong(long l)
        {
            return new DsLong(l);
        }

        /// <summary>
        /// Get the System.Int64 equivalent to this DirectShowLib.DsLong instance.
        /// </summary>
        /// <returns>A System.Int64</returns>
        public long ToInt64()
        {
            return this.Value;
        }

        /// <summary>
        /// Get a new DirectShowLib.DsLong instance for a given System.Int64
        /// </summary>
        /// <param name="g">The System.Int64 to wrap into a DirectShowLib.DsLong</param>
        /// <returns>A new instance of DirectShowLib.DsLong</returns>
        public static DsLong FromInt64(long l)
        {
            return new DsLong(l);
        }
    }

    /// <summary>
    /// DirectShowLib.DsGuid is a wrapper class around a System.Guid value type.
    /// </summary>
    /// <remarks>
    /// This class is necessary to enable null paramters passing.
    /// </remarks>
    [StructLayout(LayoutKind.Explicit)]
    public class DsGuid
    {
        [FieldOffset(0)]
        private Guid guid;

        public static readonly DsGuid Empty = Guid.Empty;

        /// <summary>
        /// Empty constructor. 
        /// Initialize it with System.Guid.Empty
        /// </summary>
        public DsGuid()
        {
            this.guid = Guid.Empty;
        }

        /// <summary>
        /// Constructor.
        /// Initialize this instance with a given System.Guid string representation.
        /// </summary>
        /// <param name="g">A valid System.Guid as string</param>
        public DsGuid(string g)
        {
            this.guid = new Guid(g);
        }

        /// <summary>
        /// Constructor.
        /// Initialize this instance with a given System.Guid.
        /// </summary>
        /// <param name="g">A System.Guid value type</param>
        public DsGuid(Guid g)
        {
            this.guid = g;
        }

        /// <summary>
        /// Get a string representation of this DirectShowLib.DsGuid Instance.
        /// </summary>
        /// <returns>A string representing this instance</returns>
        public override string ToString()
        {
            return this.guid.ToString();
        }

        /// <summary>
        /// Get a string representation of this DirectShowLib.DsGuid Instance with a specific format.
        /// </summary>
        /// <param name="format"><see cref="System.Guid.ToString"/> for a description of the format parameter.</param>
        /// <returns>A string representing this instance according to the format parameter</returns>
        public string ToString(string format)
        {
            return this.guid.ToString(format);
        }

        public override int GetHashCode()
        {
            return this.guid.GetHashCode();
        }

        /// <summary>
        /// Define implicit cast between DirectShowLib.DsGuid and System.Guid for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsGuid.ToGuid"/> for similar functionality.
        /// <code>
        ///   // Define a new DsGuid instance
        ///   DsGuid dsG = new DsGuid("{33D57EBF-7C9D-435e-A15E-D300B52FBD91}");
        ///   // Do implicit cast between DsGuid and Guid
        ///   Guid g = dsG;
        ///
        ///   Console.WriteLine(g.ToString());
        /// </code>
        /// </summary>
        /// <param name="g">DirectShowLib.DsGuid to be cast</param>
        /// <returns>A casted System.Guid</returns>
        public static implicit operator Guid(DsGuid g)
        {
            return g.guid;
        }

        /// <summary>
        /// Define implicit cast between System.Guid and DirectShowLib.DsGuid for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsGuid.FromGuid"/> for similar functionality.
        /// <code>
        ///   // Define a new Guid instance
        ///   Guid g = new Guid("{B9364217-366E-45f8-AA2D-B0ED9E7D932D}");
        ///   // Do implicit cast between Guid and DsGuid
        ///   DsGuid dsG = g;
        ///
        ///   Console.WriteLine(dsG.ToString());
        /// </code>
        /// </summary>
        /// <param name="g">System.Guid to be cast</param>
        /// <returns>A casted DirectShowLib.DsGuid</returns>
        public static implicit operator DsGuid(Guid g)
        {
            return new DsGuid(g);
        }

        /// <summary>
        /// Get the System.Guid equivalent to this DirectShowLib.DsGuid instance.
        /// </summary>
        /// <returns>A System.Guid</returns>
        public Guid ToGuid()
        {
            return this.guid;
        }

        /// <summary>
        /// Get a new DirectShowLib.DsGuid instance for a given System.Guid
        /// </summary>
        /// <param name="g">The System.Guid to wrap into a DirectShowLib.DsGuid</param>
        /// <returns>A new instance of DirectShowLib.DsGuid</returns>
        public static DsGuid FromGuid(Guid g)
        {
            return new DsGuid(g);
        }
    }

    /// <summary>
    /// DirectShowLib.DsInt is a wrapper class around a <see cref="System.Int32"/> value type.
    /// </summary>
    /// <remarks>
    /// This class is necessary to enable null paramters passing.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public class DsInt
    {
        private int Value;

        /// <summary>
        /// Constructor
        /// Initialize a new instance of DirectShowLib.DsInt with the Value parameter
        /// </summary>
        /// <param name="Value">Value to assign to this new instance</param>
        public DsInt(int Value)
        {
            this.Value = Value;
        }

        /// <summary>
        /// Get a string representation of this DirectShowLib.DsInt Instance.
        /// </summary>
        /// <returns>A string representing this instance</returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        /// <summary>
        /// Define implicit cast between DirectShowLib.DsInt and System.Int64 for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsInt.ToInt64"/> for similar functionality.
        /// <code>
        ///   // Define a new DsInt instance
        ///   DsInt dsI = new DsInt(0x12345678);
        ///   // Do implicit cast between DsInt and Int32
        ///   int i = dsI;
        ///
        ///   Console.WriteLine(i.ToString());
        /// </code>
        /// </summary>
        /// <param name="g">DirectShowLib.DsInt to be cast</param>
        /// <returns>A casted System.Int32</returns>
        public static implicit operator int(DsInt l)
        {
            return l.Value;
        }

        /// <summary>
        /// Define implicit cast between System.Int32 and DirectShowLib.DsInt for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsGuid.FromInt32"/> for similar functionality.
        /// <code>
        ///   // Define a new Int32 instance
        ///   int i = 0x12345678;
        ///   // Do implicit cast between Int64 and DsInt
        ///   DsInt dsI = i;
        ///
        ///   Console.WriteLine(dsI.ToString());
        /// </code>
        /// </summary>
        /// <param name="g">System.Int32 to be cast</param>
        /// <returns>A casted DirectShowLib.DsInt</returns>
        public static implicit operator DsInt(int l)
        {
            return new DsInt(l);
        }

        /// <summary>
        /// Get the System.Int32 equivalent to this DirectShowLib.DsInt instance.
        /// </summary>
        /// <returns>A System.Int32</returns>
        public int ToInt32()
        {
            return this.Value;
        }

        /// <summary>
        /// Get a new DirectShowLib.DsInt instance for a given System.Int32
        /// </summary>
        /// <param name="g">The System.Int32 to wrap into a DirectShowLib.DsInt</param>
        /// <returns>A new instance of DirectShowLib.DsInt</returns>
        public static DsInt FromInt32(int l)
        {
            return new DsInt(l);
        }
    }

    /// <summary>
    /// DirectShowLib.DsShort is a wrapper class around a <see cref="System.Int16"/> value type.
    /// </summary>
    /// <remarks>
    /// This class is necessary to enable null paramters passing.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public class DsShort
    {
        private short Value;

        /// <summary>
        /// Constructor
        /// Initialize a new instance of DirectShowLib.DsShort with the Value parameter
        /// </summary>
        /// <param name="Value">Value to assign to this new instance</param>
        public DsShort(short Value)
        {
            this.Value = Value;
        }

        /// <summary>
        /// Get a string representation of this DirectShowLib.DsShort Instance.
        /// </summary>
        /// <returns>A string representing this instance</returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        /// <summary>
        /// Define implicit cast between DirectShowLib.DsShort and System.Int16 for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsShort.ToInt64"/> for similar functionality.
        /// <code>
        ///   // Define a new DsShort instance
        ///   DsShort dsS = new DsShort(0x1234);
        ///   // Do implicit cast between DsShort and Int16
        ///   short s = dsS;
        ///
        ///   Console.WriteLine(s.ToString());
        /// </code>
        /// </summary>
        /// <param name="g">DirectShowLib.DsShort to be cast</param>
        /// <returns>A casted System.Int16</returns>
        public static implicit operator short(DsShort l)
        {
            return l.Value;
        }

        /// <summary>
        /// Define implicit cast between System.Int16 and DirectShowLib.DsShort for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsGuid.FromInt16"/> for similar functionality.
        /// <code>
        ///   // Define a new Int16 instance
        ///   short s = 0x1234;
        ///   // Do implicit cast between Int64 and DsShort
        ///   DsShort dsS = s;
        ///
        ///   Console.WriteLine(dsS.ToString());
        /// </code>
        /// </summary>
        /// <param name="g">System.Int16 to be cast</param>
        /// <returns>A casted DirectShowLib.DsShort</returns>
        public static implicit operator DsShort(short l)
        {
            return new DsShort(l);
        }

        /// <summary>
        /// Get the System.Int16 equivalent to this DirectShowLib.DsShort instance.
        /// </summary>
        /// <returns>A System.Int16</returns>
        public short ToInt16()
        {
            return this.Value;
        }

        /// <summary>
        /// Get a new DirectShowLib.DsShort instance for a given System.Int64
        /// </summary>
        /// <param name="g">The System.Int16 to wrap into a DirectShowLib.DsShort</param>
        /// <returns>A new instance of DirectShowLib.DsShort</returns>
        public static DsShort FromInt16(short l)
        {
            return new DsShort(l);
        }
    }

    /// <summary>
    /// DirectShowLib.DsRect is a managed representation of the Win32 RECT structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class DsRect
    {
        public int left;

        public int top;

        public int right;

        public int bottom;

        /// <summary>
        /// Empty contructor. Initialize all fields to 0
        /// </summary>
        public DsRect()
        {
            this.left = 0;
            this.top = 0;
            this.right = 0;
            this.bottom = 0;
        }

        /// <summary>
        /// A parametred constructor. Initialize fields with given values.
        /// </summary>
        /// <param name="left">the left value</param>
        /// <param name="top">the top value</param>
        /// <param name="right">the right value</param>
        /// <param name="bottom">the bottom value</param>
        public DsRect(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        /// <summary>
        /// A parametred constructor. Initialize fields with a given <see cref="System.Drawing.Rectangle"/>.
        /// </summary>
        /// <param name="rectangle">A <see cref="System.Drawing.Rectangle"/></param>
        /// <remarks>
        /// Warning, DsRect define a rectangle by defining two of his corners and <see cref="System.Drawing.Rectangle"/> define a rectangle with his upper/left corner, his width and his height.
        /// </remarks>
        public DsRect(Rectangle rectangle)
        {
            this.left = rectangle.Left;
            this.top = rectangle.Top;
            this.right = rectangle.Right;
            this.bottom = rectangle.Bottom;
        }

        /// <summary>
        /// Provide de string representation of this DsRect instance
        /// </summary>
        /// <returns>A string formated like this : [left, top - right, bottom]</returns>
        public override string ToString()
        {
            return string.Format("[{0}, {1} - {2}, {3}]", this.left, this.top, this.right, this.bottom);
        }

        public override int GetHashCode()
        {
            return this.left.GetHashCode() | this.top.GetHashCode() | this.right.GetHashCode()
                   | this.bottom.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is DsRect)
            {
                DsRect cmp = (DsRect)obj;

                return right == cmp.right && bottom == cmp.bottom && left == cmp.left && top == cmp.top;
            }

            if (obj is Rectangle)
            {
                Rectangle cmp = (Rectangle)obj;

                return right == cmp.Right && bottom == cmp.Bottom && left == cmp.Left && top == cmp.Top;
            }

            return false;
        }

        /// <summary>
        /// Define implicit cast between DirectShowLib.DsRect and System.Drawing.Rectangle for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsRect.ToRectangle"/> for similar functionality.
        /// <code>
        ///   // Define a new Rectangle instance
        ///   Rectangle r = new Rectangle(0, 0, 100, 100);
        ///   // Do implicit cast between Rectangle and DsRect
        ///   DsRect dsR = r;
        ///
        ///   Console.WriteLine(dsR.ToString());
        /// </code>
        /// </summary>
        /// <param name="r">a DsRect to be cast</param>
        /// <returns>A casted System.Drawing.Rectangle</returns>
        public static implicit operator Rectangle(DsRect r)
        {
            return r.ToRectangle();
        }

        /// <summary>
        /// Define implicit cast between System.Drawing.Rectangle and DirectShowLib.DsRect for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsRect.FromRectangle"/> for similar functionality.
        /// <code>
        ///   // Define a new DsRect instance
        ///   DsRect dsR = new DsRect(0, 0, 100, 100);
        ///   // Do implicit cast between DsRect and Rectangle
        ///   Rectangle r = dsR;
        ///
        ///   Console.WriteLine(r.ToString());
        /// </code>
        /// </summary>
        /// <param name="r">A System.Drawing.Rectangle to be cast</param>
        /// <returns>A casted DsRect</returns>
        public static implicit operator DsRect(Rectangle r)
        {
            return new DsRect(r);
        }

        /// <summary>
        /// Get the System.Drawing.Rectangle equivalent to this DirectShowLib.DsRect instance.
        /// </summary>
        /// <returns>A System.Drawing.Rectangle</returns>
        public Rectangle ToRectangle()
        {
            return new Rectangle(this.left, this.top, (this.right - this.left), (this.bottom - this.top));
        }

        /// <summary>
        /// Get a new DirectShowLib.DsRect instance for a given <see cref="System.Drawing.Rectangle"/>
        /// </summary>
        /// <param name="r">The <see cref="System.Drawing.Rectangle"/> used to initialize this new DirectShowLib.DsGuid</param>
        /// <returns>A new instance of DirectShowLib.DsGuid</returns>
        public static DsRect FromRectangle(Rectangle r)
        {
            return new DsRect(r);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NormalizedRect
    {
        public float left;

        public float top;

        public float right;

        public float bottom;

        public NormalizedRect(float l, float t, float r, float b)
        {
            this.left = l;
            this.top = t;
            this.right = r;
            this.bottom = b;
        }

        public NormalizedRect(RectangleF r)
        {
            this.left = r.Left;
            this.top = r.Top;
            this.right = r.Right;
            this.bottom = r.Bottom;
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1} - {2}, {3}]", this.left, this.top, this.right, this.bottom);
        }

        public override int GetHashCode()
        {
            return this.left.GetHashCode() | this.top.GetHashCode() | this.right.GetHashCode()
                   | this.bottom.GetHashCode();
        }

        public static implicit operator RectangleF(NormalizedRect r)
        {
            return r.ToRectangleF();
        }

        public static implicit operator NormalizedRect(Rectangle r)
        {
            return new NormalizedRect(r);
        }

        public static bool operator ==(NormalizedRect r1, NormalizedRect r2)
        {
            return ((r1.left == r2.left) && (r1.top == r2.top) && (r1.right == r2.right) && (r1.bottom == r2.bottom));
        }

        public static bool operator !=(NormalizedRect r1, NormalizedRect r2)
        {
            return ((r1.left != r2.left) || (r1.top != r2.top) || (r1.right != r2.right) || (r1.bottom != r2.bottom));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is NormalizedRect)) return false;

            NormalizedRect other = (NormalizedRect)obj;
            return (this == other);
        }


        public RectangleF ToRectangleF()
        {
            return new RectangleF(this.left, this.top, (this.right - this.left), (this.bottom - this.top));
        }

        public static NormalizedRect FromRectangle(RectangleF r)
        {
            return new NormalizedRect(r);
        }
    }

    #endregion

    #region Utility Classes

    public static class DsResults
    {
        public const int E_InvalidMediaType = unchecked((int)0x80040200);

        public const int E_InvalidSubType = unchecked((int)0x80040201);

        public const int E_NeedOwner = unchecked((int)0x80040202);

        public const int E_EnumOutOfSync = unchecked((int)0x80040203);

        public const int E_AlreadyConnected = unchecked((int)0x80040204);

        public const int E_FilterActive = unchecked((int)0x80040205);

        public const int E_NoTypes = unchecked((int)0x80040206);

        public const int E_NoAcceptableTypes = unchecked((int)0x80040207);

        public const int E_InvalidDirection = unchecked((int)0x80040208);

        public const int E_NotConnected = unchecked((int)0x80040209);

        public const int E_NoAllocator = unchecked((int)0x8004020A);

        public const int E_RunTimeError = unchecked((int)0x8004020B);

        public const int E_BufferNotSet = unchecked((int)0x8004020C);

        public const int E_BufferOverflow = unchecked((int)0x8004020D);

        public const int E_BadAlign = unchecked((int)0x8004020E);

        public const int E_AlreadyCommitted = unchecked((int)0x8004020F);

        public const int E_BuffersOutstanding = unchecked((int)0x80040210);

        public const int E_NotCommitted = unchecked((int)0x80040211);

        public const int E_SizeNotSet = unchecked((int)0x80040212);

        public const int E_NoClock = unchecked((int)0x80040213);

        public const int E_NoSink = unchecked((int)0x80040214);

        public const int E_NoInterface = unchecked((int)0x80040215);

        public const int E_NotFound = unchecked((int)0x80040216);

        public const int E_CannotConnect = unchecked((int)0x80040217);

        public const int E_CannotRender = unchecked((int)0x80040218);

        public const int E_ChangingFormat = unchecked((int)0x80040219);

        public const int E_NoColorKeySet = unchecked((int)0x8004021A);

        public const int E_NotOverlayConnection = unchecked((int)0x8004021B);

        public const int E_NotSampleConnection = unchecked((int)0x8004021C);

        public const int E_PaletteSet = unchecked((int)0x8004021D);

        public const int E_ColorKeySet = unchecked((int)0x8004021E);

        public const int E_NoColorKeyFound = unchecked((int)0x8004021F);

        public const int E_NoPaletteAvailable = unchecked((int)0x80040220);

        public const int E_NoDisplayPalette = unchecked((int)0x80040221);

        public const int E_TooManyColors = unchecked((int)0x80040222);

        public const int E_StateChanged = unchecked((int)0x80040223);

        public const int E_NotStopped = unchecked((int)0x80040224);

        public const int E_NotPaused = unchecked((int)0x80040225);

        public const int E_NotRunning = unchecked((int)0x80040226);

        public const int E_WrongState = unchecked((int)0x80040227);

        public const int E_StartTimeAfterEnd = unchecked((int)0x80040228);

        public const int E_InvalidRect = unchecked((int)0x80040229);

        public const int E_TypeNotAccepted = unchecked((int)0x8004022A);

        public const int E_SampleRejected = unchecked((int)0x8004022B);

        public const int E_SampleRejectedEOS = unchecked((int)0x8004022C);

        public const int E_DuplicateName = unchecked((int)0x8004022D);

        public const int S_DuplicateName = unchecked((int)0x0004022D);

        public const int E_Timeout = unchecked((int)0x8004022E);

        public const int E_InvalidFileFormat = unchecked((int)0x8004022F);

        public const int E_EnumOutOfRange = unchecked((int)0x80040230);

        public const int E_CircularGraph = unchecked((int)0x80040231);

        public const int E_NotAllowedToSave = unchecked((int)0x80040232);

        public const int E_TimeAlreadyPassed = unchecked((int)0x80040233);

        public const int E_AlreadyCancelled = unchecked((int)0x80040234);

        public const int E_CorruptGraphFile = unchecked((int)0x80040235);

        public const int E_AdviseAlreadySet = unchecked((int)0x80040236);

        public const int S_StateIntermediate = unchecked((int)0x00040237);

        public const int E_NoModexAvailable = unchecked((int)0x80040238);

        public const int E_NoAdviseSet = unchecked((int)0x80040239);

        public const int E_NoFullScreen = unchecked((int)0x8004023A);

        public const int E_InFullScreenMode = unchecked((int)0x8004023B);

        public const int E_UnknownFileType = unchecked((int)0x80040240);

        public const int E_CannotLoadSourceFilter = unchecked((int)0x80040241);

        public const int S_PartialRender = unchecked((int)0x00040242);

        public const int E_FileTooShort = unchecked((int)0x80040243);

        public const int E_InvalidFileVersion = unchecked((int)0x80040244);

        public const int S_SomeDataIgnored = unchecked((int)0x00040245);

        public const int S_ConnectionsDeferred = unchecked((int)0x00040246);

        public const int E_InvalidCLSID = unchecked((int)0x80040247);

        public const int E_InvalidMediaType2 = unchecked((int)0x80040248);

        public const int E_BabKey = unchecked((int)0x800403F2);

        public const int S_NoMoreItems = unchecked((int)0x00040103);

        public const int E_SampleTimeNotSet = unchecked((int)0x80040249);

        public const int S_ResourceNotNeeded = unchecked((int)0x00040250);

        public const int E_MediaTimeNotSet = unchecked((int)0x80040251);

        public const int E_NoTimeFormatSet = unchecked((int)0x80040252);

        public const int E_MonoAudioHW = unchecked((int)0x80040253);

        public const int S_MediaTypeIgnored = unchecked((int)0x00040254);

        public const int E_NoDecompressor = unchecked((int)0x80040255);

        public const int E_NoAudioHardware = unchecked((int)0x80040256);

        public const int S_VideoNotRendered = unchecked((int)0x00040257);

        public const int S_AudioNotRendered = unchecked((int)0x00040258);

        public const int E_RPZA = unchecked((int)0x80040259);

        public const int S_RPZA = unchecked((int)0x0004025A);

        public const int E_ProcessorNotSuitable = unchecked((int)0x8004025B);

        public const int E_UnsupportedAudio = unchecked((int)0x8004025C);

        public const int E_UnsupportedVideo = unchecked((int)0x8004025D);

        public const int E_MPEGNotConstrained = unchecked((int)0x8004025E);

        public const int E_NotInGraph = unchecked((int)0x8004025F);

        public const int S_Estimated = unchecked((int)0x00040260);

        public const int E_NoTimeFormat = unchecked((int)0x80040261);

        public const int E_ReadOnly = unchecked((int)0x80040262);

        public const int S_Reserved = unchecked((int)0x00040263);

        public const int E_BufferUnderflow = unchecked((int)0x80040264);

        public const int E_UnsupportedStream = unchecked((int)0x80040265);

        public const int E_NoTransport = unchecked((int)0x80040266);

        public const int S_StreamOff = unchecked((int)0x00040267);

        public const int S_CantCue = unchecked((int)0x00040268);

        public const int E_BadVideoCD = unchecked((int)0x80040269);

        public const int S_NoStopTime = unchecked((int)0x00040270);

        public const int E_OutOfVideoMemory = unchecked((int)0x80040271);

        public const int E_VPNegotiationFailed = unchecked((int)0x80040272);

        public const int E_DDrawCapsNotSuitable = unchecked((int)0x80040273);

        public const int E_NoVPHardware = unchecked((int)0x80040274);

        public const int E_NoCaptureHardware = unchecked((int)0x80040275);

        public const int E_DVDOperationInhibited = unchecked((int)0x80040276);

        public const int E_DVDInvalidDomain = unchecked((int)0x80040277);

        public const int E_DVDNoButton = unchecked((int)0x80040278);

        public const int E_DVDGraphNotReady = unchecked((int)0x80040279);

        public const int E_DVDRenderFail = unchecked((int)0x8004027A);

        public const int E_DVDDecNotEnough = unchecked((int)0x8004027B);

        public const int E_DDrawVersionNotSuitable = unchecked((int)0x8004027C);

        public const int E_CopyProtFailed = unchecked((int)0x8004027D);

        public const int S_NoPreviewPin = unchecked((int)0x0004027E);

        public const int E_TimeExpired = unchecked((int)0x8004027F);

        public const int S_DVDNonOneSequential = unchecked((int)0x00040280);

        public const int E_DVDWrongSpeed = unchecked((int)0x80040281);

        public const int E_DVDMenuDoesNotExist = unchecked((int)0x80040282);

        public const int E_DVDCmdCancelled = unchecked((int)0x80040283);

        public const int E_DVDStateWrongVersion = unchecked((int)0x80040284);

        public const int E_DVDStateCorrupt = unchecked((int)0x80040285);

        public const int E_DVDStateWrongDisc = unchecked((int)0x80040286);

        public const int E_DVDIncompatibleRegion = unchecked((int)0x80040287);

        public const int E_DVDNoAttributes = unchecked((int)0x80040288);

        public const int E_DVDNoGoupPGC = unchecked((int)0x80040289);

        public const int E_DVDLowParentalLevel = unchecked((int)0x8004028A);

        public const int E_DVDNotInKaraokeMode = unchecked((int)0x8004028B);

        public const int S_DVDChannelContentsNotAvailable = unchecked((int)0x0004028C);

        public const int S_DVDNotAccurate = unchecked((int)0x0004028D);

        public const int E_FrameStepUnsupported = unchecked((int)0x8004028E);

        public const int E_DVDStreamDisabled = unchecked((int)0x8004028F);

        public const int E_DVDTitleUnknown = unchecked((int)0x80040290);

        public const int E_DVDInvalidDisc = unchecked((int)0x80040291);

        public const int E_DVDNoResumeInformation = unchecked((int)0x80040292);

        public const int E_PinAlreadyBlockedOnThisThread = unchecked((int)0x80040293);

        public const int E_PinAlreadyBlocked = unchecked((int)0x80040294);

        public const int E_CertificationFailure = unchecked((int)0x80040295);

        public const int E_VMRNotInMixerMode = unchecked((int)0x80040296);

        public const int E_VMRNoApSupplied = unchecked((int)0x80040297);

        public const int E_VMRNoDeinterlace_HW = unchecked((int)0x80040298);

        public const int E_VMRNoProcAMPHW = unchecked((int)0x80040299);

        public const int E_DVDVMR9IncompatibleDec = unchecked((int)0x8004029A);

        public const int E_NoCOPPHW = unchecked((int)0x8004029B);
    }


    public static class DsError
    {
        [DllImport("quartz.dll", CharSet = CharSet.Unicode, ExactSpelling = true, EntryPoint = "AMGetErrorTextW"),
         SuppressUnmanagedCodeSecurity]
        public static extern int AMGetErrorText(int hr, StringBuilder buf, int max);

        /// <summary>
        /// If hr has a "failed" status code (E_*), throw an exception.  Note that status
        /// messages (S_*) are not considered failure codes.  If DirectShow error text
        /// is available, it is used to build the exception, otherwise a generic com error
        /// is thrown.
        /// </summary>
        /// <param name="hr">The HRESULT to check</param>
        public static void ThrowExceptionForHR(int hr)
        {
            // If a severe error has occurred
            if (hr < 0)
            {
                string s = GetErrorText(hr);

                // If a string is returned, build a com error from it
                if (s != null)
                {
                    throw new COMException(s, hr);
                }
                else
                {
                    // No string, just use standard com error
                    Marshal.ThrowExceptionForHR(hr);
                }
            }
        }

        /// <summary>
        /// Returns a string describing a DS error.  Works for both error codes
        /// (values < 0) and Status codes (values >= 0)
        /// </summary>
        /// <param name="hr">HRESULT for which to get description</param>
        /// <returns>The string, or null if no error text can be found</returns>
        public static string GetErrorText(int hr)
        {
            const int MAX_ERROR_TEXT_LEN = 160;

            // Make a buffer to hold the string
            StringBuilder buf = new StringBuilder(MAX_ERROR_TEXT_LEN, MAX_ERROR_TEXT_LEN);

            // If a string is returned, build a com error from it
            if (AMGetErrorText(hr, buf, MAX_ERROR_TEXT_LEN) > 0)
            {
                return buf.ToString();
            }

            return null;
        }
    }


    public static class DsUtils
    {
        /// <summary>
        /// Returns the PinCategory of the specified pin.  Usually a member of PinCategory.  Not all pins have a category.
        /// </summary>
        /// <param name="pPin"></param>
        /// <returns>Guid indicating pin category or Guid.Empty on no category.  Usually a member of PinCategory</returns>
        public static Guid GetPinCategory(IPin pPin)
        {
            Guid guidRet = Guid.Empty;

            // Memory to hold the returned guid
            int iSize = Marshal.SizeOf(typeof(Guid));
            IntPtr ipOut = Marshal.AllocCoTaskMem(iSize);

            try
            {
                int hr;
                int cbBytes;
                Guid g = PropSetID.Pin;

                // Get an IKsPropertySet from the pin
                IKsPropertySet pKs = pPin as IKsPropertySet;

                if (pKs != null)
                {
                    // Query for the Category
                    hr = pKs.Get(g, (int)AMPropertyPin.Category, IntPtr.Zero, 0, ipOut, iSize, out cbBytes);
                    DsError.ThrowExceptionForHR(hr);

                    // Marshal it to the return variable
                    guidRet = (Guid)Marshal.PtrToStructure(ipOut, typeof(Guid));
                }
            }
            finally
            {
                Marshal.FreeCoTaskMem(ipOut);
                ipOut = IntPtr.Zero;
            }

            return guidRet;
        }

        /// <summary>
        ///  Free the nested structures and release any
        ///  COM objects within an AMMediaType struct.
        /// </summary>
        public static void FreeAMMediaType(AMMediaType mediaType)
        {
            if (mediaType != null)
            {
                if (mediaType.formatSize != 0)
                {
                    Marshal.FreeCoTaskMem(mediaType.formatPtr);
                    mediaType.formatSize = 0;
                    mediaType.formatPtr = IntPtr.Zero;
                }
                if (mediaType.unkPtr != IntPtr.Zero)
                {
                    Marshal.Release(mediaType.unkPtr);
                    mediaType.unkPtr = IntPtr.Zero;
                }
            }
        }

        /// <summary>
        ///  Free the nested interfaces within a PinInfo struct.
        /// </summary>
        public static void FreePinInfo(PinInfo pinInfo)
        {
            if (pinInfo.filter != null)
            {
                Marshal.ReleaseComObject(pinInfo.filter);
                pinInfo.filter = null;
            }
        }

        public static void FreeFilterInfo(FilterInfo filterInfo)
        {
            if (filterInfo.pGraph != null)
            {
                ReleaseComObject(filterInfo.pGraph);
                filterInfo.pGraph = null;
            }
        }

        public static void ReleaseComObject(object obj)
        {
            if (obj != null)
            {
                Marshal.ReleaseComObject(obj);
            }
        }
    }


    public class DsROTEntry : IDisposable
    {
        [Flags]
        private enum ROTFlags
        {
            RegistrationKeepsAlive = 0x1,

            AllowAnyClient = 0x2
        }

        private int m_cookie = 0;

        #region APIs

        [DllImport("ole32.dll", ExactSpelling = true), SuppressUnmanagedCodeSecurity]
#if USING_NET11
        private static extern int GetRunningObjectTable(int r, out UCOMIRunningObjectTable pprot);
#else
        private static extern int GetRunningObjectTable(int r, out IRunningObjectTable pprot);

#endif

        [DllImport("ole32.dll", CharSet = CharSet.Unicode, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
#if USING_NET11
        private static extern int CreateItemMoniker(string delim, string item, out UCOMIMoniker ppmk);
#else
        private static extern int CreateItemMoniker(string delim, string item, out IMoniker ppmk);

#endif

        #endregion

        public DsROTEntry(IFilterGraph graph)
        {
            int hr = 0;
#if USING_NET11
            UCOMIRunningObjectTable rot = null;
            UCOMIMoniker mk = null;
#else
            IRunningObjectTable rot = null;
            IMoniker mk = null;
#endif
            try
            {
                // First, get a pointer to the running object table
                hr = GetRunningObjectTable(0, out rot);
                DsError.ThrowExceptionForHR(hr);

                // Build up the object to add to the table
                int id = Process.GetCurrentProcess().Id;
                IntPtr iuPtr = Marshal.GetIUnknownForObject(graph);
                string s;
                try
                {
                    s = iuPtr.ToString("x");
                }
                catch
                {
                    s = "";
                }
                finally
                {
                    Marshal.Release(iuPtr);
                }
                string item = string.Format("FilterGraph {0} pid {1:x8}", s, id);
                hr = CreateItemMoniker("!", item, out mk);
                DsError.ThrowExceptionForHR(hr);

                // Add the object to the table
#if USING_NET11
                rot.Register((int)ROTFlags.RegistrationKeepsAlive, graph, mk, out m_cookie);
#else
                m_cookie = rot.Register((int)ROTFlags.RegistrationKeepsAlive, graph, mk);
#endif
            }
            finally
            {
                DsUtils.ReleaseComObject(mk);
                DsUtils.ReleaseComObject(rot);
            }
        }

        ~DsROTEntry()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (m_cookie != 0)
            {
                GC.SuppressFinalize(this);
#if USING_NET11
                UCOMIRunningObjectTable rot;
#else
                IRunningObjectTable rot;
#endif

                // Get a pointer to the running object table
                int hr = GetRunningObjectTable(0, out rot);
                DsError.ThrowExceptionForHR(hr);

                try
                {
                    // Remove our entry
                    rot.Revoke(m_cookie);
                    m_cookie = 0;
                }
                finally
                {
                    DsUtils.ReleaseComObject(rot);
                }
            }
        }
    }


    public class DsDevice : IDisposable
    {
#if USING_NET11
        private UCOMIMoniker m_Mon;
#else

        private IMoniker m_Mon;

#endif

        private string m_Name;

#if USING_NET11
        public DsDevice(UCOMIMoniker Mon)
#else

        public DsDevice(IMoniker Mon)
#endif
        {
            m_Mon = Mon;
            m_Name = null;
        }

#if USING_NET11
        public UCOMIMoniker Mon
#else

        public IMoniker Mon
#endif
        {
            get { return m_Mon; }
        }

        public string Name
        {
            get
            {
                if (m_Name == null)
                {
                    m_Name = GetPropBagValue("FriendlyName");
                }
                return m_Name;
            }
        }

        /// <summary>
        /// Returns a unique identifier for a device
        /// </summary>
        public string DevicePath
        {
            get
            {
                string s = null;

                try
                {
                    m_Mon.GetDisplayName(null, null, out s);
                }
                catch
                {
                }

                return s;
            }
        }

        /// <summary>
        /// Returns the ClassID for a device
        /// </summary>
        public Guid ClassID
        {
            get
            {
                Guid g;

                m_Mon.GetClassID(out g);

                return g;
            }
        }


        /// <summary>
        /// Returns an array of DsDevices of type devcat.
        /// </summary>
        /// <param name="cat">Any one of FilterCategory</param>
        public static DsDevice[] GetDevicesOfCat(Guid FilterCategory)
        {
            int hr;

            // Use arrayList to build the retun list since it is easily resizable
            DsDevice[] devret;
            ArrayList devs = new ArrayList();
#if USING_NET11
            UCOMIEnumMoniker enumMon;
#else
            IEnumMoniker enumMon;
#endif

            ICreateDevEnum enumDev = (ICreateDevEnum)new CreateDevEnum();
            hr = enumDev.CreateClassEnumerator(FilterCategory, out enumMon, 0);
            DsError.ThrowExceptionForHR(hr);

            // CreateClassEnumerator returns null for enumMon if there are no entries
            if (hr != 1)
            {
                try
                {
                    try
                    {
#if USING_NET11
                        UCOMIMoniker[] mon = new UCOMIMoniker[1];
#else
                        IMoniker[] mon = new IMoniker[1];
#endif

#if USING_NET11
                        int j;
                        while ((enumMon.Next(1, mon, out j) == 0))
#else
                        while ((enumMon.Next(1, mon, IntPtr.Zero) == 0))
#endif
                        {
                            try
                            {
                                // The devs array now owns this object.  Don't
                                // release it if we are going to be successfully
                                // returning the devret array
                                devs.Add(new DsDevice(mon[0]));
                            }
                            catch
                            {
                                DsUtils.ReleaseComObject(mon[0]);
                                throw;
                            }
                        }
                    }
                    finally
                    {
                        DsUtils.ReleaseComObject(enumMon);
                    }

                    // Copy the ArrayList to the DsDevice[]
                    devret = new DsDevice[devs.Count];
                    devs.CopyTo(devret);
                }
                catch
                {
                    foreach (DsDevice d in devs)
                    {
                        d.Dispose();
                    }
                    throw;
                }
            }
            else
            {
                devret = new DsDevice[0];
            }

            return devret;
        }

        /// <summary>
        /// Get a specific PropertyBag value from a moniker
        /// </summary>
        /// <param name="sPropName">The name of the value to retrieve</param>
        /// <returns>String or null on error</returns>
        public string GetPropBagValue(string sPropName)
        {
            IPropertyBag bag = null;
            string ret = null;
            object bagObj = null;
            object val = null;

            try
            {
                Guid bagId = typeof(IPropertyBag).GUID;
                m_Mon.BindToStorage(null, null, ref bagId, out bagObj);

                bag = (IPropertyBag)bagObj;

                int hr = bag.Read(sPropName, out val, null);
                DsError.ThrowExceptionForHR(hr);

                ret = val as string;
            }
            catch
            {
                ret = null;
            }
            finally
            {
                bag = null;
                if (bagObj != null)
                {
                    DsUtils.ReleaseComObject(bagObj);
                    bagObj = null;
                }
            }

            return ret;
        }

        public void Dispose()
        {
            if (Mon != null)
            {
                DsUtils.ReleaseComObject(Mon);
                m_Mon = null;
                GC.SuppressFinalize(this);
            }
            m_Name = null;
        }
    }


    public static class DsFindPin
    {
        /// <summary>
        /// Scans a filter's pins looking for a pin in the specified direction
        /// </summary>
        /// <param name="vSource">The filter to scan</param>
        /// <param name="vDir">The direction to find</param>
        /// <param name="iIndex">Zero based index (ie 2 will return the third pin in the specified direction)</param>
        /// <returns>The matching pin, or null if not found</returns>
        public static IPin ByDirection(IBaseFilter vSource, PinDirection vDir, int iIndex)
        {
            int hr;
            IEnumPins ppEnum;
            PinDirection ppindir;
            IPin pRet = null;
            IPin[] pPins = new IPin[1];

            if (vSource == null)
            {
                return null;
            }

            // Get the pin enumerator
            hr = vSource.EnumPins(out ppEnum);
            DsError.ThrowExceptionForHR(hr);

            try
            {
                // Walk the pins looking for a match
                int fetched;
                while (ppEnum.Next(1, pPins, out fetched) >= 0 && (fetched == 1))
                {
                    // Read the direction
                    hr = pPins[0].QueryDirection(out ppindir);
                    DsError.ThrowExceptionForHR(hr);

                    // Is it the right direction?
                    if (ppindir == vDir)
                    {
                        // Is is the right index?
                        if (iIndex == 0)
                        {
                            pRet = pPins[0];
                            break;
                        }
                        iIndex--;
                    }
                    DsUtils.ReleaseComObject(pPins[0]);
                }
            }
            finally
            {
                DsUtils.ReleaseComObject(ppEnum);
            }

            return pRet;
        }

        /// <summary>
        /// Scans a filter's pins looking for a pin with the specified name
        /// </summary>
        /// <param name="vSource">The filter to scan</param>
        /// <param name="vPinName">The pin name to find</param>
        /// <returns>The matching pin, or null if not found</returns>
        public static IPin ByName(IBaseFilter vSource, string vPinName)
        {
            int hr;
            IEnumPins ppEnum;
            PinInfo ppinfo;
            IPin pRet = null;
            IPin[] pPins = new IPin[1];

            if (vSource == null)
            {
                return null;
            }

            // Get the pin enumerator
            hr = vSource.EnumPins(out ppEnum);
            DsError.ThrowExceptionForHR(hr);

            try
            {
                // Walk the pins looking for a match
                int fetched;
                while (ppEnum.Next(1, pPins, out fetched) >= 0 && fetched == 1)
                {
                    // Read the info
                    hr = pPins[0].QueryPinInfo(out ppinfo);
                    DsError.ThrowExceptionForHR(hr);

                    // Is it the right name?
                    if (ppinfo.name == vPinName)
                    {
                        DsUtils.FreePinInfo(ppinfo);
                        pRet = pPins[0];
                        break;
                    }
                    DsUtils.ReleaseComObject(pPins[0]);
                    DsUtils.FreePinInfo(ppinfo);
                }
            }
            finally
            {
                DsUtils.ReleaseComObject(ppEnum);
            }

            return pRet;
        }

        /// <summary>
        /// Scan's a filter's pins looking for a pin with the specified category
        /// </summary>
        /// <param name="vSource">The filter to scan</param>
        /// <param name="guidPinCat">The guid from PinCategory to scan for</param>
        /// <param name="iIndex">Zero based index (ie 2 will return the third pin of the specified category)</param>
        /// <returns>The matching pin, or null if not found</returns>
        public static IPin ByCategory(IBaseFilter vSource, Guid PinCategory, int iIndex)
        {
            int hr;
            IEnumPins ppEnum;
            IPin pRet = null;
            IPin[] pPins = new IPin[1];

            if (vSource == null)
            {
                return null;
            }

            // Get the pin enumerator
            hr = vSource.EnumPins(out ppEnum);
            DsError.ThrowExceptionForHR(hr);

            try
            {
                // Walk the pins looking for a match
                int fetched;
                while (ppEnum.Next(1, pPins, out fetched) >= 0 && fetched == 1)
                {
                    // Is it the right category?
                    if (DsUtils.GetPinCategory(pPins[0]) == PinCategory)
                    {
                        // Is is the right index?
                        if (iIndex == 0)
                        {
                            pRet = pPins[0];
                            break;
                        }
                        iIndex--;
                    }
                    DsUtils.ReleaseComObject(pPins[0]);
                }
            }
            finally
            {
                DsUtils.ReleaseComObject(ppEnum);
            }

            return pRet;
        }

        /// <summary>
        /// Scans a filter's pins looking for a pin with the specified connection status
        /// </summary>
        /// <param name="vSource">The filter to scan</param>
        /// <param name="vStat">The status to find (connected/unconnected)</param>
        /// <param name="iIndex">Zero based index (ie 2 will return the third pin with the specified status)</param>
        /// <returns>The matching pin, or null if not found</returns>
        public static IPin ByConnectionStatus(IBaseFilter vSource, PinConnectedStatus vStat, int iIndex)
        {
            int hr;
            IEnumPins ppEnum;
            IPin pRet = null;
            IPin pOutPin;
            IPin[] pPins = new IPin[1];

            if (vSource == null)
            {
                return null;
            }

            // Get the pin enumerator
            hr = vSource.EnumPins(out ppEnum);
            DsError.ThrowExceptionForHR(hr);

            try
            {
                // Walk the pins looking for a match
                int fetched;
                while (ppEnum.Next(1, pPins, out fetched) >= 0 && fetched == 1)
                {
                    // Read the connected status
                    hr = pPins[0].ConnectedTo(out pOutPin);

                    // Check for VFW_E_NOT_CONNECTED.  Anything else is bad.
                    if (hr != DsResults.E_NotConnected)
                    {
                        DsError.ThrowExceptionForHR(hr);

                        // The ConnectedTo call succeeded, release the interface
                        DsUtils.ReleaseComObject(pOutPin);
                    }

                    // Is it the right status?
                    if ((hr == 0 && vStat == PinConnectedStatus.Connected)
                        || (hr == DsResults.E_NotConnected && vStat == PinConnectedStatus.Unconnected))
                    {
                        // Is is the right index?
                        if (iIndex == 0)
                        {
                            pRet = pPins[0];
                            break;
                        }
                        iIndex--;
                    }
                    DsUtils.ReleaseComObject(pPins[0]);
                }
            }
            finally
            {
                DsUtils.ReleaseComObject(ppEnum);
            }

            return pRet;
        }
    }


    public static class DsToString
    {
        /// <summary>
        /// Produces a usable string that describes the MediaType object
        /// </summary>
        /// <returns>Concatenation of MajorType + SubType + FormatType + Fixed + Temporal + SampleSize.ToString</returns>
        public static string AMMediaTypeToString(AMMediaType pmt)
        {
            return string.Format(
                "{0} {1} {2} {3} {4} {5}",
                MediaTypeToString(pmt.majorType),
                MediaSubTypeToString(pmt.subType),
                MediaFormatTypeToString(pmt.formatType),
                (pmt.fixedSizeSamples ? "FixedSamples" : "NotFixedSamples"),
                (pmt.temporalCompression ? "temporalCompression" : "NottemporalCompression"),
                pmt.sampleSize.ToString());
        }

        /// <summary>
        /// Converts AMMediaType.MajorType Guid to a readable string
        /// </summary>
        /// <returns>MajorType Guid as a readable string or Guid if unrecognized</returns>
        public static string MediaTypeToString(Guid guid)
        {
            // Walk the MediaSubType class looking for a match
            return WalkClass(typeof(MediaType), guid);
        }

        /// <summary>
        /// Converts the AMMediaType.SubType Guid to a readable string
        /// </summary>
        /// <returns>SubType Guid as a readable string or Guid if unrecognized</returns>
        public static string MediaSubTypeToString(Guid guid)
        {
            // Walk the MediaSubType class looking for a match
            string s = WalkClass(typeof(MediaSubType), guid);

            // There is a special set of Guids that contain the FourCC code
            // as part of the Guid.  Check to see if it is one of those.
            if (s.Length == 36 && s.Substring(8).ToUpper() == "-0000-0010-8000-00AA00389B71")
            {
                // Parse out the FourCC code
                byte[] asc =
                    {
                        Convert.ToByte(s.Substring(6, 2), 16), Convert.ToByte(s.Substring(4, 2), 16),
                        Convert.ToByte(s.Substring(2, 2), 16), Convert.ToByte(s.Substring(0, 2), 16)
                    };
                s = Encoding.ASCII.GetString(asc);
            }

            return s;
        }

        /// <summary>
        /// Converts the AMMediaType.FormatType Guid to a readable string
        /// </summary>
        /// <returns>FormatType Guid as a readable string or Guid if unrecognized</returns>
        public static string MediaFormatTypeToString(Guid guid)
        {
            // Walk the FormatType class looking for a match
            return WalkClass(typeof(FormatType), guid);
        }

        /// <summary>
        /// Use reflection to walk a class looking for a property containing a specified guid
        /// </summary>
        /// <param name="MyType">Class to scan</param>
        /// <param name="guid">Guid to scan for</param>
        /// <returns>String representing property name that matches, or Guid.ToString() for no match</returns>
        private static string WalkClass(Type MyType, Guid guid)
        {
            object o = null;

            // Read the fields from the class
            FieldInfo[] Fields = MyType.GetFields();

            // Walk the returned array
            foreach (FieldInfo m in Fields)
            {
                // Read the value of the property.  The parameter is ignored.
                o = m.GetValue(o);

                // Compare it with the sought value
                if ((Guid)o == guid)
                {
                    return m.Name;
                }
            }

            return guid.ToString();
        }
    }


    // This abstract class contains definitions for use in implementing a custom marshaler.
    //
    // MarshalManagedToNative() gets called before the COM method, and MarshalNativeToManaged() gets
    // called after.  This allows for allocating a correctly sized memory block for the COM call,
    // then to break up the memory block and build an object that c# can digest.

    internal abstract class DsMarshaler : ICustomMarshaler
    {
        #region Data Members

        // The cookie isn't currently being used.
        protected string m_cookie;

        // The managed object passed in to MarshalManagedToNative, and modified in MarshalNativeToManaged
        protected object m_obj;

        #endregion

        // The constructor.  This is called from GetInstance (below)
        public DsMarshaler(string cookie)
        {
            // If we get a cookie, save it.
            m_cookie = cookie;
        }

        // Called just before invoking the COM method.  The returned IntPtr is what goes on the stack
        // for the COM call.  The input arg is the parameter that was passed to the method.
        public virtual IntPtr MarshalManagedToNative(object managedObj)
        {
            // Save off the passed-in value.  Safe since we just checked the type.
            m_obj = managedObj;

            // Create an appropriately sized buffer, blank it, and send it to the marshaler to
            // make the COM call with.
            int iSize = GetNativeDataSize() + 3;
            IntPtr p = Marshal.AllocCoTaskMem(iSize);

            for (int x = 0; x < iSize / 4; x++)
            {
                Marshal.WriteInt32(p, x * 4, 0);
            }

            return p;
        }

        // Called just after invoking the COM method.  The IntPtr is the same one that just got returned
        // from MarshalManagedToNative.  The return value is unused.
        public virtual object MarshalNativeToManaged(IntPtr pNativeData)
        {
            return m_obj;
        }

        // Release the (now unused) buffer
        public virtual void CleanUpNativeData(IntPtr pNativeData)
        {
            if (pNativeData != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(pNativeData);
            }
        }

        // Release the (now unused) managed object
        public virtual void CleanUpManagedData(object managedObj)
        {
            m_obj = null;
        }

        // This routine is (apparently) never called by the marshaler.  However it can be useful.
        public abstract int GetNativeDataSize();

        // GetInstance is called by the marshaler in preparation to doing custom marshaling.  The (optional)
        // cookie is the value specified in MarshalCookie="asdf", or "" is none is specified.

        // It is commented out in this abstract class, but MUST be implemented in derived classes
        //public static ICustomMarshaler GetInstance(string cookie)
    }

    // c# does not correctly marshal arrays of pointers.
    //

    internal class EMTMarshaler : DsMarshaler
    {
        public EMTMarshaler(string cookie)
            : base(cookie)
        {
        }

        // Called just after invoking the COM method.  The IntPtr is the same one that just got returned
        // from MarshalManagedToNative.  The return value is unused.
        public override object MarshalNativeToManaged(IntPtr pNativeData)
        {
            AMMediaType[] emt = m_obj as AMMediaType[];

            for (int x = 0; x < emt.Length; x++)
            {
                // Copy in the value, and advance the pointer
                IntPtr p = Marshal.ReadIntPtr(pNativeData, x * IntPtr.Size);
                if (p != IntPtr.Zero)
                {
                    emt[x] = (AMMediaType)Marshal.PtrToStructure(p, typeof(AMMediaType));
                }
                else
                {
                    emt[x] = null;
                }
            }

            return null;
        }

        // The number of bytes to marshal out
        public override int GetNativeDataSize()
        {
            // Get the array size
            int i = ((Array)m_obj).Length;

            // Multiply that times the size of a pointer
            int j = i * IntPtr.Size;

            return j;
        }

        // This method is called by interop to create the custom marshaler.  The (optional)
        // cookie is the value specified in MarshalCookie="asdf", or "" is none is specified.
        public static ICustomMarshaler GetInstance(string cookie)
        {
            return new EMTMarshaler(cookie);
        }
    }

    // c# does not correctly create structures that contain ByValArrays of structures (or enums!).  Instead
    // of allocating enough room for the ByValArray of structures, it only reserves room for a ref,
    // even when decorated with ByValArray and SizeConst.  Needless to say, if DirectShow tries to
    // write to this too-short buffer, bad things will happen.
    //
    // To work around this for the DvdTitleAttributes structure, use this custom marshaler
    // by declaring the parameter DvdTitleAttributes as:
    //
    //    [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(DTAMarshaler))]
    //    DvdTitleAttributes pTitle
    //
    // See DsMarshaler for more info on custom marshalers

    internal class DTAMarshaler : DsMarshaler
    {
        public DTAMarshaler(string cookie)
            : base(cookie)
        {
        }

        // Called just after invoking the COM method.  The IntPtr is the same one that just got returned
        // from MarshalManagedToNative.  The return value is unused.
        public override object MarshalNativeToManaged(IntPtr pNativeData)
        {
            DvdTitleAttributes dta = m_obj as DvdTitleAttributes;

            // Copy in the value, and advance the pointer
            dta.AppMode = (DvdTitleAppMode)Marshal.ReadInt32(pNativeData);
            pNativeData = (IntPtr)(pNativeData.ToInt64() + Marshal.SizeOf(typeof(int)));

            // Copy in the value, and advance the pointer
            dta.VideoAttributes = (DvdVideoAttributes)Marshal.PtrToStructure(pNativeData, typeof(DvdVideoAttributes));
            pNativeData = (IntPtr)(pNativeData.ToInt64() + Marshal.SizeOf(typeof(DvdVideoAttributes)));

            // Copy in the value, and advance the pointer
            dta.ulNumberOfAudioStreams = Marshal.ReadInt32(pNativeData);
            pNativeData = (IntPtr)(pNativeData.ToInt64() + Marshal.SizeOf(typeof(int)));

            // Allocate a large enough array to hold all the returned structs.
            dta.AudioAttributes = new DvdAudioAttributes[8];
            for (int x = 0; x < 8; x++)
            {
                // Copy in the value, and advance the pointer
                dta.AudioAttributes[x] =
                    (DvdAudioAttributes)Marshal.PtrToStructure(pNativeData, typeof(DvdAudioAttributes));
                pNativeData = (IntPtr)(pNativeData.ToInt64() + Marshal.SizeOf(typeof(DvdAudioAttributes)));
            }

            // Allocate a large enough array to hold all the returned structs.
            dta.MultichannelAudioAttributes = new DvdMultichannelAudioAttributes[8];
            for (int x = 0; x < 8; x++)
            {
                // MultichannelAudioAttributes has nested ByValArrays.  They need to be individually copied.

                dta.MultichannelAudioAttributes[x].Info = new DvdMUAMixingInfo[8];

                for (int y = 0; y < 8; y++)
                {
                    // Copy in the value, and advance the pointer
                    dta.MultichannelAudioAttributes[x].Info[y] =
                        (DvdMUAMixingInfo)Marshal.PtrToStructure(pNativeData, typeof(DvdMUAMixingInfo));
                    pNativeData = (IntPtr)(pNativeData.ToInt64() + Marshal.SizeOf(typeof(DvdMUAMixingInfo)));
                }

                dta.MultichannelAudioAttributes[x].Coeff = new DvdMUACoeff[8];

                for (int y = 0; y < 8; y++)
                {
                    // Copy in the value, and advance the pointer
                    dta.MultichannelAudioAttributes[x].Coeff[y] =
                        (DvdMUACoeff)Marshal.PtrToStructure(pNativeData, typeof(DvdMUACoeff));
                    pNativeData = (IntPtr)(pNativeData.ToInt64() + Marshal.SizeOf(typeof(DvdMUACoeff)));
                }
            }

            // The DvdMultichannelAudioAttributes needs to be 16 byte aligned
            pNativeData = (IntPtr)(pNativeData.ToInt64() + 4);

            // Copy in the value, and advance the pointer
            dta.ulNumberOfSubpictureStreams = Marshal.ReadInt32(pNativeData);
            pNativeData = (IntPtr)(pNativeData.ToInt64() + Marshal.SizeOf(typeof(int)));

            // Allocate a large enough array to hold all the returned structs.
            dta.SubpictureAttributes = new DvdSubpictureAttributes[32];
            for (int x = 0; x < 32; x++)
            {
                // Copy in the value, and advance the pointer
                dta.SubpictureAttributes[x] =
                    (DvdSubpictureAttributes)Marshal.PtrToStructure(pNativeData, typeof(DvdSubpictureAttributes));
                pNativeData = (IntPtr)(pNativeData.ToInt64() + Marshal.SizeOf(typeof(DvdSubpictureAttributes)));
            }

            // Note that 4 bytes (more alignment) are unused at the end

            return null;
        }

        // The number of bytes to marshal out
        public override int GetNativeDataSize()
        {
            // This is the actual size of a DvdTitleAttributes structure
            return 3208;
        }

        // This method is called by interop to create the custom marshaler.  The (optional)
        // cookie is the value specified in MarshalCookie="asdf", or "" is none is specified.
        public static ICustomMarshaler GetInstance(string cookie)
        {
            return new DTAMarshaler(cookie);
        }
    }

    // See DTAMarshaler for an explanation of the problem.  This class is for marshaling
    // a DvdKaraokeAttributes structure.
    internal class DKAMarshaler : DsMarshaler
    {
        // The constructor.  This is called from GetInstance (below)
        public DKAMarshaler(string cookie)
            : base(cookie)
        {
        }

        // Called just after invoking the COM method.  The IntPtr is the same one that just got returned
        // from MarshalManagedToNative.  The return value is unused.
        public override object MarshalNativeToManaged(IntPtr pNativeData)
        {
            DvdKaraokeAttributes dka = m_obj as DvdKaraokeAttributes;

            // Copy in the value, and advance the pointer
            dka.bVersion = (byte)Marshal.ReadByte(pNativeData);
            pNativeData = (IntPtr)(pNativeData.ToInt64() + Marshal.SizeOf(typeof(byte)));

            // DWORD Align
            pNativeData = (IntPtr)(pNativeData.ToInt64() + 3);

            // Copy in the value, and advance the pointer
            dka.fMasterOfCeremoniesInGuideVocal1 = Marshal.ReadInt32(pNativeData) != 0;
            pNativeData = (IntPtr)(pNativeData.ToInt64() + Marshal.SizeOf(typeof(bool)));

            // Copy in the value, and advance the pointer
            dka.fDuet = Marshal.ReadInt32(pNativeData) != 0;
            pNativeData = (IntPtr)(pNativeData.ToInt64() + Marshal.SizeOf(typeof(bool)));

            // Copy in the value, and advance the pointer
            dka.ChannelAssignment = (DvdKaraokeAssignment)Marshal.ReadInt32(pNativeData);
            pNativeData = (IntPtr)(pNativeData.ToInt64()
                                   + Marshal.SizeOf(
                                       DvdKaraokeAssignment.GetUnderlyingType(typeof(DvdKaraokeAssignment))));

            // Allocate a large enough array to hold all the returned structs.
            dka.wChannelContents = new DvdKaraokeContents[8];
            for (int x = 0; x < 8; x++)
            {
                // Copy in the value, and advance the pointer
                dka.wChannelContents[x] = (DvdKaraokeContents)Marshal.ReadInt16(pNativeData);
                pNativeData = (IntPtr)(pNativeData.ToInt64()
                                       + Marshal.SizeOf(
                                           DvdKaraokeContents.GetUnderlyingType(typeof(DvdKaraokeContents))));
            }

            return null;
        }

        // The number of bytes to marshal out
        public override int GetNativeDataSize()
        {
            // This is the actual size of a DvdKaraokeAttributes structure.
            return 32;
        }

        // This method is called by interop to create the custom marshaler.  The (optional)
        // cookie is the value specified in MarshalCookie="asdf", or "" is none is specified.
        public static ICustomMarshaler GetInstance(string cookie)
        {
            return new DKAMarshaler(cookie);
        }
    }

    #endregion
}