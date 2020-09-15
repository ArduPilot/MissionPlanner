#region license

/*
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
*/

#endregion

using System;
using System.Runtime.InteropServices;

namespace DirectShowLib.BDA
{
    #region Declarations

    // From Mpeg2Bits.h

    /// <summary>
    /// From PID_BITS & PID_BITS_MIDL
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct PidBits
    {
        public short Bits;

        public short Reserved
        {
            get { return (short)((int)Bits & 0x0007); }
        }

        public short ProgramId
        {
            get { return (short)(((int)Bits & 0xfff8) >> 3); }
        }
    }

    /// <summary>
    /// From MPEG_HEADER_BITS & MPEG_HEADER_BITS_MIDL
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct MpegHeaderBits
    {
        public short Bits;

        public short SectionLength
        {
            get { return (short)((int)Bits & 0x0fff); }
        }

        public short Reserved
        {
            get { return (short)(((int)Bits & 0x3000) >> 12); }
        }

        public short PrivateIndicator
        {
            get { return (short)(((int)Bits & 0x4000) >> 14); }
        }

        public short SectionSyntaxIndicator
        {
            get { return (short)(((int)Bits & 0x8000) >> 15); }
        }
    }

    /// <summary>
    /// From MPEG_HEADER_VERSION_BITS & MPEG_HEADER_VERSION_BITS_MIDL
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MpegHeaderVersionBits
    {
        public byte Bits;

        public byte CurrentNextIndicator
        {
            get { return (byte)((int)Bits & 0x1); }
        }

        public byte VersionNumber
        {
            get { return (byte)(((int)Bits & 0x3e) >> 1); }
        }

        public byte Reserved
        {
            get { return (byte)(((int)Bits & 0xc0) >> 6); }
        }
    }

#if ALLOW_UNTESTED_INTERFACES

    /// <summary>
    /// From MPEG_CURRENT_NEXT_BIT, MPEG_SECTION_IS_*
    /// </summary>
    public enum MpegSectionIs
    {
        Next = 0,
        Current = 1
    }

    /// <summary>
    /// From TID_EXTENSION
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct TidExtension
    {
        public short wTidExt;
        public short wCount;
    }

    /// <summary>
    /// From SECTION
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Section
    {
        public short TableId;
        public MpegHeaderBits Header;
        public byte SectionData; // Must be marshalled manually
    }

    /// <summary>
    /// From LONG_SECTION
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LongSection
    {
        public short TableId;
        public MpegHeaderBits Header;
        public short TableIdExtension;
        public MpegHeaderVersionBits Version;
        public byte SectionNumber;
        public byte LastSectionNumber;
        public byte RemainingData; // Must be marshalled manually
    }

    /// <summary>
    /// From DSMCC_SECTION
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DsmccSection
    {
        public short TableId;
        public MpegHeaderBits Header;
        public short TableIdExtension;
        public MpegHeaderVersionBits Version;
        public byte SectionNumber;
        public byte LastSectionNumber;
        public byte ProtocolDiscriminator;
        public byte DsmccType;
        public short MessageId;
        public int TransactionId;
        public byte Reserved;
        public byte AdaptationLength;
        public short MessageLength;
        public byte RemainingData; // Must be marshalled manually
    }

    /// <summary>
    /// From MPEG_RQST_PACKET
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MpegRqstPacket
    {
        public int dwLength;
        [MarshalAs(UnmanagedType.LPStruct)]
        public Section pSection;
    }

    /// <summary>
    /// From MPEG_DATE
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MpegDate
    {
        public byte Date;
        public byte Month;
        public short Year;

        public DateTime ToDateTime()
        {
            return new DateTime(this.Year, this.Month, this.Date);
        }
    }

    /// <summary>
    /// From MPEG_SERVICE_REQUEST
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MpegServiceRequest
    {
        public MPEGRequestType Type;
        public MPEGContext Context;
        public short Pid;
        public byte TableId;
        public MPEG2Filter Filter;
        public int Flags;
    }

    /// <summary>
    /// From MPEG_SERVICE_RESPONSE
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public class MpegServiceResponse
    {
        public int IPAddress;
        public short Port;
    }

    /// <summary>
    /// From MPEG_STREAM_FILTER
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MpegStreamFilter
    {
        public short wPidValue;
        public int dwFilterSize;
        [MarshalAs(UnmanagedType.Bool)]
        public bool fCrcEnabled;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
        public byte[] rgchFilter;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
        public byte[] rgchMask;
    }
#endif

    /// <summary>
    /// From MPEG_DURATION & MPEG_TIME
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MpegDuration
    {
        public byte Hours;
        public byte Minutes;
        public byte Seconds;

        public TimeSpan ToTimeSpan()
        {
            return new TimeSpan(this.Hours, this.Minutes, this.Seconds);
        }
    }

    /// <summary>
    /// From MPEG_DATE_AND_TIME
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MpegDateAndTime
    {
        //public MpegDate D;
        //public MpegTime T;
        // Marshaling is faster like that...
        public byte Date;
        public byte Month;
        public short Year;
        public byte Hours;
        public byte Minutes;
        public byte Seconds;

        public DateTime ToDateTime()
        {
            return new DateTime(this.Year, this.Month, this.Date, this.Hours, this.Minutes, this.Seconds);
        }
    }

    /// <summary>
    /// From DSMCC_ELEMENT
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class DsmccElement
    {
        public short pid;
        public byte bComponentTag;
        public int dwCarouselId;
        public int dwTransactionId;
        public DsmccElement pNext;
    }

    /// <summary>
    /// From MPE_ELEMENT
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MpeElement
    {
        public short pid;
        public byte bComponentTag;
        public MpeElement pNext;
    }

    #endregion

    #region Interfaces

#if ALLOW_UNTESTED_INTERFACES

#endif

    #endregion
}
