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

using System.Runtime.InteropServices;
using System.Security;

namespace DirectShowLib
{
    #region Declarations

    /// <summary>
    /// From AM_LINE21_CCLEVEL
    /// </summary>
    public enum AMLine21CCLevel
    {
        TC2 = 0,
    }

    /// <summary>
    /// From AM_LINE21_CCSERVICE
    /// </summary>
    public enum AMLine21CCService
    {
        None = 0,
        Caption1,
        Caption2,
        Text1,
        Text2,
        XDS,
        DefChannel = 10,
        Invalid
    }

    /// <summary>
    /// From AM_LINE21_CCSTATE
    /// </summary>
    public enum AMLine21CCState
    {
        Off = 0,
        On
    }

    /// <summary>
    /// From AM_LINE21_DRAWBGMODE
    /// </summary>
    public enum AMLine21DrawBGMode
    {
        Opaque,
        Transparent
    }

    #endregion

    #region Interfaces

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("6E8D4A21-310C-11d0-B79A-00AA003767A7"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMLine21Decoder
    {
        [PreserveSig]
        int GetDecoderLevel([Out] out AMLine21CCLevel lpLevel);

        [PreserveSig]
        int GetCurrentService([Out] out AMLine21CCService lpService);

        [PreserveSig]
        int SetCurrentService([In] AMLine21CCService Service);

        [PreserveSig]
        int GetServiceState([Out] out AMLine21CCState lpState);

        [PreserveSig]
        int SetServiceState([In] AMLine21CCState State);

        [PreserveSig]
        int GetOutputFormat([Out] BitmapInfoHeader lpbmih);

        [PreserveSig]
        int SetOutputFormat([In] BitmapInfoHeader lpbmih);

        [PreserveSig]
        int GetBackgroundColor([Out] out int pdwPhysColor);

        [PreserveSig]
        int SetBackgroundColor([In] int dwPhysColor);

        [PreserveSig]
        int GetRedrawAlways([Out, MarshalAs(UnmanagedType.Bool)] out bool lpbOption);

        [PreserveSig]
        int SetRedrawAlways([In, MarshalAs(UnmanagedType.Bool)] bool bOption);

        [PreserveSig]
        int GetDrawBackgroundMode([Out] out AMLine21DrawBGMode lpMode);

        [PreserveSig]
        int SetDrawBackgroundMode([In] AMLine21DrawBGMode Mode);
    }

    #endregion
}
