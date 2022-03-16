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

namespace DirectShowLib
{
    #region Declarations

#if ALLOW_UNTESTED_INTERFACES

    /// <summary>
    /// From AMPlayListItemFlags
    /// </summary>
    public enum AMPlayListItemFlags
    {
        CanSkip = 0x1,
        CanBind = 0x2
    }

    /// <summary>
    /// From AMPlayListFlags
    /// </summary>
    [Flags]
    public enum AMPlayListFlags
    {
        None = 0,
        StartInScanMode = 0x1,
        ForceBanner = 0x2
    }

#endif

    #endregion

    #region Interfaces

#if ALLOW_UNTESTED_INTERFACES

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a868ff-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMPlayListItem
    {
        int GetFlags(
            out AMPlayListItemFlags pdwFlags
            );

        int GetSourceCount(
            out int pdwSources
            );

        int GetSourceURL(
            int dwSourceIndex,
            [MarshalAs(UnmanagedType.BStr)] out string pbstrURL
            );

        int GetSourceStart(
            int dwSourceIndex,
            out long prtStart
            );

        int GetSourceDuration(
            int dwSourceIndex,
            out long prtDuration
            );

        int GetSourceStartMarker(
            int dwSourceIndex,
            out int pdwMarker
            );

        int GetSourceEndMarker(
            int dwSourceIndex,
            out int pdwMarker
            );

        int GetSourceStartMarkerName(
            int dwSourceIndex,
            [MarshalAs(UnmanagedType.BStr)] out string pbstrStartMarker
            );

        int GetSourceEndMarkerName(
            int dwSourceIndex,
            [MarshalAs(UnmanagedType.BStr)] out string pbstrEndMarker
            );

        int GetLinkURL(
            [MarshalAs(UnmanagedType.BStr)] out string pbstrURL);

        int GetScanDuration(
            int dwSourceIndex,
            out long prtScanDuration
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a868fe-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMPlayList
    {
        int GetFlags(
            out AMPlayListFlags pdwFlags
            );

        int GetItemCount(
            out int pdwItems
            );

        int GetItem(
            int dwItemIndex,
            out IAMPlayListItem ppItem
            );

        int GetNamedEvent(
            string pwszEventName,
            int dwItemIndex,
            out IAMPlayListItem ppItem,
            out AMPlayListItemFlags pdwFlags
            );

        int GetRepeatInfo(
            out int pdwRepeatCount,
            out int pdwRepeatStart,
            out int pdwRepeatEnd
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("4C437B91-6E9E-11d1-A704-006097C4E476"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ISpecifyParticularPages
    {
        int GetPages(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidWhatPages,
            out DsCAUUID pPages
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("02EF04DD-7580-11d1-BECE-00C04FB6E937"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMRebuild
    {
        int RebuildNow( );
    };

#endif

    #endregion
}
