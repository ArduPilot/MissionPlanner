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
using System.Security;

namespace DirectShowLib
{
    #region Declarations

    /// <summary>
    /// From _AM_PIN_FLOW_CONTROL_BLOCK_FLAGS
    /// </summary>
    [Flags]
    public enum AMPinFlowControl
    {
        None = 0x00000000,
        Block = 0x00000001
    }

    /// <summary>
    /// From AM_GRAPH_CONFIG_RECONNECT_FLAGS
    /// </summary>
    [Flags]
    public enum AMGraphConfigReconnect
    {
        None = 0x00000000,
        DirectConnect = 0x00000001,
        CacheRemovedFilters = 0x00000002,
        UseOnlyCachedFilters = 0x00000004
    }

    /// <summary>
    /// From _AM_FILTER_FLAGS
    /// </summary>
    [Flags]
    public enum AMFilterFlags
    {
        None = 0x00000000,
        Removable = 0x00000001
    }

    /// <summary>
    /// From _REM_FILTER_FLAGS
    /// </summary>
    [Flags]
    public enum RemFilterFlags
    {
        None = 0x00000000,
        LeaveConnected = 0x00000001
    }

    #endregion

    #region Interfaces

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("c56e9858-dbf3-4f6b-8119-384af2060deb"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPinFlowControl
    {
        [PreserveSig]
        int Block(
            [In] AMPinFlowControl dwBlockFlags,
            [In] IntPtr hEvent // HEVENT
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("DCFBDCF6-0DC2-45f5-9AB2-7C330EA09C29"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFilterChain
    {
        [PreserveSig]
        int StartChain(
            [In] IBaseFilter pStartFilter,
            [In] IBaseFilter pEndFilter
            );

        [PreserveSig]
        int PauseChain(
            [In] IBaseFilter pStartFilter,
            [In] IBaseFilter pEndFilter
            );

        [PreserveSig]
        int StopChain(
            [In] IBaseFilter pStartFilter,
            [In] IBaseFilter pEndFilter
            );

        [PreserveSig]
        int RemoveChain(
            [In] IBaseFilter pStartFilter,
            [In] IBaseFilter pEndFilter
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("ade0fd60-d19d-11d2-abf6-00a0c905f375"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IGraphConfigCallback
    {
        [PreserveSig]
        int Reconfigure(
            IntPtr pvContext, // PVOID
            int dwFlags
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("03A1EB8E-32BF-4245-8502-114D08A9CB88"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IGraphConfig
    {
        [PreserveSig]
        int Reconnect(
            [In] IPin pOutputPin,
            [In] IPin pInputPin,
            [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmtFirstConnection,
            [In] IBaseFilter pUsingFilter, // can be NULL
            [In] IntPtr hAbortEvent, // HANDLE
            [In] AMGraphConfigReconnect dwFlags
            );

        [PreserveSig]
        int Reconfigure(
            [In] IGraphConfigCallback pCallback,
            [In] IntPtr pvContext, // PVOID
            [In] int dwFlags,
            [In] IntPtr hAbortEvent // HANDLE
            );

        [PreserveSig]
        int AddFilterToCache(
            [In] IBaseFilter pFilter
            );

        [PreserveSig]
        int EnumCacheFilter(
            [Out] out IEnumFilters pEnum
            );

        [PreserveSig]
        int RemoveFilterFromCache(
            [In] IBaseFilter pFilter
            );

        [PreserveSig]
        int GetStartTime(
            [Out] out long prtStart
            );

        [PreserveSig]
        int PushThroughData(
            [In] IPin pOutputPin,
            [In] IPinConnection pConnection,
            [In] IntPtr hEventAbort // HANDLE
            );

        [PreserveSig]
        int SetFilterFlags(
            [In] IBaseFilter pFilter,
            [In] AMFilterFlags dwFlags
            );

        [PreserveSig]
        int GetFilterFlags(
            [In] IBaseFilter pFilter,
            [Out] out AMFilterFlags pdwFlags
            );

        [PreserveSig]
        int RemoveFilterEx(
            [In] IBaseFilter pFilter,
            RemFilterFlags Flags
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("4a9a62d3-27d4-403d-91e9-89f540e55534"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPinConnection
    {
        [PreserveSig]
        int DynamicQueryAccept(
            [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt
            );

        [PreserveSig]
        int NotifyEndOfStream(
            [In] IntPtr hNotifyEvent // HEVENT
            );

        [PreserveSig]
        int IsEndPin();

        [PreserveSig]
        int DynamicDisconnect();
    }

    #endregion
}
