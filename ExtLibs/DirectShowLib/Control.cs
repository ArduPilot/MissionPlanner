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
    /// From define AM_MEDIAEVENT_NONOTIFY
    /// </summary>
    [Flags]
    public enum NotifyFlags
    {
        None,
        NoNotify
    }

    /// <summary>
    /// From #define OATRUE/OAFALSE
    /// </summary>
    public enum OABool
    {
        False = 0,
        True = -1 // bools in .NET use 1, not -1
    }

    /// <summary>
    /// From WS_* defines
    /// </summary>
    [Flags]
    public enum WindowStyle
    {
        Overlapped     =  0x00000000,
        Popup       =     unchecked((int)0x80000000), // enum can't be uint for VB
        Child       =     0x40000000,
        Minimize    =     0x20000000,
        Visible     =     0x10000000,
        Disabled    =     0x08000000,
        ClipSiblings =    0x04000000,
        ClipChildren =    0x02000000,
        Maximize      =   0x01000000,
        Caption       =   0x00C00000,
        Border        =   0x00800000,
        DlgFrame      =   0x00400000,
        VScroll       =   0x00200000,
        HScroll       =   0x00100000,
        SysMenu       =   0x00080000,
        ThickFrame    =   0x00040000,
        Group         =   0x00020000,
        TabStop       =   0x00010000,
        MinimizeBox   =   0x00020000,
        MaximizeBox   =   0x00010000
    }

    /// <summary>
    /// From WS_EX_* defines
    /// </summary>
    [Flags]
    public enum WindowStyleEx
    {
        DlgModalFrame   =  0x00000001,
        NoParentNotify  =  0x00000004,
        Topmost         =  0x00000008,
        AcceptFiles     =  0x00000010,
        Transparent     =  0x00000020,
        MDIChild        =  0x00000040,
        ToolWindow      =  0x00000080,
        WindowEdge      =  0x00000100,
        ClientEdge      =  0x00000200,
        ContextHelp     =  0x00000400,
        Right           =  0x00001000,
        Left            =  0x00000000,
        RTLReading      =  0x00002000,
        LTRReading      =  0x00000000,
        LeftScrollBar   =  0x00004000,
        RightScrollBar  =  0x00000000,
        ControlParent   =  0x00010000,
        StaticEdge      =  0x00020000,
        APPWindow       =  0x00040000,
        Layered         =  0x00080000,
        NoInheritLayout =  0x00100000,
        LayoutRTL       =  0x00400000,
        Composited      =  0x02000000,
        NoActivate      =  0x08000000
    }

    /// <summary>
    /// From SW_* defines
    /// </summary>
    public enum WindowState
    {
        Hide = 0,
        Normal,
        ShowMinimized,
        ShowMaximized,
        ShowNoActivate,
        Show,
        Minimize,
        ShowMinNoActive,
        ShowNA,
        Restore,
        ShowDefault,
        ForceMinimize
    }


    /// <summary>
    /// From DISPATCH_* defines
    /// </summary>
    [Flags]
    public enum DispatchFlags : short
    {
        None = 0x0,
        Method =       0x1,
        PropertyGet = 0x2,
        PropertyPut = 0x4,
        PropertyPutRef = 0x8
    }

    #endregion

    #region Interfaces

#if ALLOW_UNTESTED_INTERFACES

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a868b9-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IAMCollection
    {
        [PreserveSig]
        int get_Count([Out] out int plCount);

        [PreserveSig]
        int Item(
            [In] int lItem,
            [Out, MarshalAs(UnmanagedType.IUnknown)] out object ppUnk
            );

        [PreserveSig]
        int get__NewEnum([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppUnk);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a868ba-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IFilterInfo
    {
        [PreserveSig]
        int FindPin(
            [In, MarshalAs(UnmanagedType.BStr)] string strPinID,
            [Out, MarshalAs(UnmanagedType.IDispatch)] out object ppUnk
            );

        [PreserveSig]
        int get_Name([Out, MarshalAs(UnmanagedType.BStr)] out string strName);

        [PreserveSig]
        int get_VendorInfo([Out, MarshalAs(UnmanagedType.BStr)] string strVendorInfo);

        [PreserveSig]
        int get_Filter([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppUnk);

        [PreserveSig]
        int get_Pins([Out, MarshalAs(UnmanagedType.IDispatch)] out object ppUnk);

        [PreserveSig]
        int get_IsFileSource([Out] out int pbIsSource);

        [PreserveSig]
        int get_Filename([Out, MarshalAs(UnmanagedType.BStr)] out string pstrFilename);

        [PreserveSig]
        int put_Filename([In, MarshalAs(UnmanagedType.BStr)] string strFilename);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a868bb-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IRegFilterInfo
    {
        [PreserveSig]
        int get_Name([Out, MarshalAs(UnmanagedType.BStr)] out string strName);

        [PreserveSig]
        int Filter([Out, MarshalAs(UnmanagedType.IDispatch)] out object ppUnk);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a868bc-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IMediaTypeInfo
    {
        [PreserveSig]
        int get_Type([Out, MarshalAs(UnmanagedType.BStr)] out string strType);

        [PreserveSig]
        int get_Subtype([Out, MarshalAs(UnmanagedType.BStr)] out string strType);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a868bd-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IPinInfo
    {
        [PreserveSig]
        int get_Pin([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppUnk);

        [PreserveSig]
        int get_ConnectedTo([Out, MarshalAs(UnmanagedType.IDispatch)] out object ppUnk);

        [PreserveSig]
        int get_ConnectionMediaType([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppUnk);

        [PreserveSig]
        int get_FilterInfo([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppUnk);

        [PreserveSig]
        int get_Name([Out, MarshalAs(UnmanagedType.BStr)] out string ppUnk);

        [PreserveSig]
        int get_Direction([Out] int ppDirection);

        [PreserveSig]
        int get_PinID([Out, MarshalAs(UnmanagedType.BStr)] out string strPinID);

        [PreserveSig]
        int get_MediaTypes([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppUnk);

        [PreserveSig]
        int Connect([In, MarshalAs(UnmanagedType.IUnknown)] object pPin);

        [PreserveSig]
        int ConnectDirect([In, MarshalAs(UnmanagedType.IUnknown)] object pPin);

        [PreserveSig]
        int ConnectWithType(
            [In, MarshalAs(UnmanagedType.IUnknown)] object pPin,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pMediaType
            );

        [PreserveSig]
        int Disconnect();

        [PreserveSig]
        int Render();
    }

#endif

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("bc9bcf80-dcd2-11d2-abf6-00a0c905f375"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IAMStats
    {
        [PreserveSig]
        int Reset();

        [PreserveSig]
        int get_Count([Out] out int plCount);

        [PreserveSig]
        int GetValueByIndex(
            [In] int lIndex,
            [Out, MarshalAs(UnmanagedType.BStr)] out string szName,
            [Out] out int lCount,
            [Out] out double dLast,
            [Out] out double dAverage,
            [Out] out double dStdDev,
            [Out] out double dMin,
            [Out] out double dMax
            );

        [PreserveSig]
        int GetValueByName(
            [In, MarshalAs(UnmanagedType.BStr)] string szName,
            [Out] out int lIndex,
            [Out] out int lCount,
            [Out] out double dLast,
            [Out] out double dAverage,
            [Out] out double dStdDev,
            [Out] out double dMin,
            [Out] out double dMax
            );

        [PreserveSig]
        int GetIndex(
            [In, MarshalAs(UnmanagedType.BStr)] string szName,
            [In, MarshalAs(UnmanagedType.Bool)] bool lCreate,
            [Out] out int plIndex
            );

        [PreserveSig]
        int AddValue(
            [In] int lIndex,
            [In] double dValue
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a868b4-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IVideoWindow
    {
        [PreserveSig]
        int put_Caption([In, MarshalAs(UnmanagedType.BStr)] string caption);

        [PreserveSig]
        int get_Caption([Out, MarshalAs(UnmanagedType.BStr)] out string caption);

        [PreserveSig]
        int put_WindowStyle([In] WindowStyle windowStyle);

        [PreserveSig]
        int get_WindowStyle([Out] out WindowStyle windowStyle);

        [PreserveSig]
        int put_WindowStyleEx([In] WindowStyleEx windowStyleEx);

        [PreserveSig]
        int get_WindowStyleEx([Out] out WindowStyleEx windowStyleEx);

        [PreserveSig]
        int put_AutoShow([In] OABool autoShow);

        [PreserveSig]
        int get_AutoShow([Out] out OABool autoShow);

        [PreserveSig]
        int put_WindowState([In] WindowState windowState);

        [PreserveSig]
        int get_WindowState([Out] out WindowState windowState);

        [PreserveSig]
        int put_BackgroundPalette([In] OABool backgroundPalette);

        [PreserveSig]
        int get_BackgroundPalette([Out] out OABool backgroundPalette);

        [PreserveSig]
        int put_Visible([In] OABool visible);

        [PreserveSig]
        int get_Visible([Out] out OABool visible);

        [PreserveSig]
        int put_Left([In] int left);

        [PreserveSig]
        int get_Left([Out] out int left);

        [PreserveSig]
        int put_Width([In] int width);

        [PreserveSig]
        int get_Width([Out] out int width);

        [PreserveSig]
        int put_Top([In] int top);

        [PreserveSig]
        int get_Top([Out] out int top);

        [PreserveSig]
        int put_Height([In] int height);

        [PreserveSig]
        int get_Height([Out] out int height);

        [PreserveSig]
        int put_Owner([In] IntPtr owner);

        [PreserveSig]
        int get_Owner([Out] out IntPtr owner);

        [PreserveSig]
        int put_MessageDrain([In] IntPtr drain);

        [PreserveSig]
        int get_MessageDrain([Out] out IntPtr drain);

        // Use ColorTranslator to break out RGB
        [PreserveSig]
        int get_BorderColor([Out] out int color);

        // Use ColorTranslator to break out RGB
        [PreserveSig]
        int put_BorderColor([In] int color);

        [PreserveSig]
        int get_FullScreenMode([Out] out OABool fullScreenMode);

        [PreserveSig]
        int put_FullScreenMode([In] OABool fullScreenMode);

        [PreserveSig]
        int SetWindowForeground([In] OABool focus);

        [PreserveSig]
        int NotifyOwnerMessage(
            [In] IntPtr hwnd, // HWND *
            [In] int msg,
            [In] IntPtr wParam, // WPARAM
            [In] IntPtr lParam // LPARAM
            );

        [PreserveSig]
        int SetWindowPosition(
            [In] int left,
            [In] int top,
            [In] int width,
            [In] int height
            );

        [PreserveSig]
        int GetWindowPosition(
            [Out] out int left,
            [Out] out int top,
            [Out] out int width,
            [Out] out int height
            );

        [PreserveSig]
        int GetMinIdealImageSize(
            [Out] out int width,
            [Out] out int height
            );

        [PreserveSig]
        int GetMaxIdealImageSize(
            [Out] out int width,
            [Out] out int height
            );

        [PreserveSig]
        int GetRestorePosition(
            [Out] out int left,
            [Out] out int top,
            [Out] out int width,
            [Out] out int height
            );

        [PreserveSig]
        int HideCursor([In] OABool hideCursor);

        [PreserveSig]
        int IsCursorHidden([Out] out OABool hideCursor);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a868b3-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IBasicAudio
    {
        [PreserveSig]
        int put_Volume([In] int lVolume);

        [PreserveSig]
        int get_Volume([Out] out int plVolume);

        [PreserveSig]
        int put_Balance([In] int lBalance);

        [PreserveSig]
        int get_Balance([Out] out int plBalance);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a868b5-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IBasicVideo
    {
        [PreserveSig]
        int get_AvgTimePerFrame([Out] out double pAvgTimePerFrame);

        [PreserveSig]
        int get_BitRate([Out] out int pBitRate);

        [PreserveSig]
        int get_BitErrorRate([Out] out int pBitRate);

        [PreserveSig]
        int get_VideoWidth([Out] out int pVideoWidth);

        [PreserveSig]
        int get_VideoHeight([Out] out int pVideoHeight);

        [PreserveSig]
        int put_SourceLeft([In] int SourceLeft);

        [PreserveSig]
        int get_SourceLeft([Out] out int pSourceLeft);

        [PreserveSig]
        int put_SourceWidth([In] int SourceWidth);

        [PreserveSig]
        int get_SourceWidth([Out] out int pSourceWidth);

        [PreserveSig]
        int put_SourceTop([In] int SourceTop);

        [PreserveSig]
        int get_SourceTop([Out] out int pSourceTop);

        [PreserveSig]
        int put_SourceHeight([In] int SourceHeight);

        [PreserveSig]
        int get_SourceHeight([Out] out int pSourceHeight);

        [PreserveSig]
        int put_DestinationLeft([In] int DestinationLeft);

        [PreserveSig]
        int get_DestinationLeft([Out] out int pDestinationLeft);

        [PreserveSig]
        int put_DestinationWidth([In] int DestinationWidth);

        [PreserveSig]
        int get_DestinationWidth([Out] out int pDestinationWidth);

        [PreserveSig]
        int put_DestinationTop([In] int DestinationTop);

        [PreserveSig]
        int get_DestinationTop([Out] out int pDestinationTop);

        [PreserveSig]
        int put_DestinationHeight([In] int DestinationHeight);

        [PreserveSig]
        int get_DestinationHeight([Out] out int pDestinationHeight);

        [PreserveSig]
        int SetSourcePosition(
            [In] int left,
            [In] int top,
            [In] int width,
            [In] int height
            );

        [PreserveSig]
        int GetSourcePosition(
            [Out] out int left,
            [Out] out int top,
            [Out] out int width,
            [Out] out int height
            );

        [PreserveSig]
        int SetDefaultSourcePosition();

        [PreserveSig]
        int SetDestinationPosition(
            [In] int left,
            [In] int top,
            [In] int width,
            [In] int height
            );

        [PreserveSig]
        int GetDestinationPosition(
            [Out] out int left,
            [Out] out int top,
            [Out] out int width,
            [Out] out int height
            );

        [PreserveSig]
        int SetDefaultDestinationPosition();

        [PreserveSig]
        int GetVideoSize(
            [Out] out int pWidth,
            [Out] out int pHeight
            );

        [PreserveSig]
        int GetVideoPaletteEntries(
            [In] int StartIndex,
            [In] int Entries,
            [Out] out int pRetrieved,
            [Out] out int [] pPalette
            );

        [PreserveSig]
        int GetCurrentImage(
            [In, Out] ref int pBufferSize,
            [Out] IntPtr pDIBImage // int *
            );

        [PreserveSig]
        int IsUsingDefaultSource();

        [PreserveSig]
        int IsUsingDefaultDestination();
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("329bb360-f6ea-11d1-9038-00a0c9697298"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IBasicVideo2 : IBasicVideo
    {
        #region IBasicVideo Methods

        [PreserveSig]
        new int get_AvgTimePerFrame([Out] out double pAvgTimePerFrame);

        [PreserveSig]
        new int get_BitRate([Out] out int pBitRate);

        [PreserveSig]
        new int get_BitErrorRate([Out] out int pBitRate);

        [PreserveSig]
        new int get_VideoWidth([Out] out int pVideoWidth);

        [PreserveSig]
        new int get_VideoHeight([Out] out int pVideoHeight);

        [PreserveSig]
        new int put_SourceLeft([In] int SourceLeft);

        [PreserveSig]
        new int get_SourceLeft([Out] out int pSourceLeft);

        [PreserveSig]
        new int put_SourceWidth([In] int SourceWidth);

        [PreserveSig]
        new int get_SourceWidth([Out] out int pSourceWidth);

        [PreserveSig]
        new int put_SourceTop([In] int SourceTop);

        [PreserveSig]
        new int get_SourceTop([Out] out int pSourceTop);

        [PreserveSig]
        new int put_SourceHeight([In] int SourceHeight);

        [PreserveSig]
        new int get_SourceHeight([Out] out int pSourceHeight);

        [PreserveSig]
        new int put_DestinationLeft([In] int DestinationLeft);

        [PreserveSig]
        new int get_DestinationLeft([Out] out int pDestinationLeft);

        [PreserveSig]
        new int put_DestinationWidth([In] int DestinationWidth);

        [PreserveSig]
        new int get_DestinationWidth([Out] out int pDestinationWidth);

        [PreserveSig]
        new int put_DestinationTop([In] int DestinationTop);

        [PreserveSig]
        new int get_DestinationTop([Out] out int pDestinationTop);

        [PreserveSig]
        new int put_DestinationHeight([In] int DestinationHeight);

        [PreserveSig]
        new int get_DestinationHeight([Out] out int pDestinationHeight);

        [PreserveSig]
        new int SetSourcePosition(
            [In] int left,
            [In] int top,
            [In] int width,
            [In] int height
            );

        [PreserveSig]
        new int GetSourcePosition(
            [Out] out int left,
            [Out] out int top,
            [Out] out int width,
            [Out] out int height
            );

        [PreserveSig]
        new int SetDefaultSourcePosition();

        [PreserveSig]
        new int SetDestinationPosition(
            [In] int left,
            [In] int top,
            [In] int width,
            [In] int height
            );

        [PreserveSig]
        new int GetDestinationPosition(
            [Out] out int left,
            [Out] out int top,
            [Out] out int width,
            [Out] out int height
            );

        [PreserveSig]
        new int SetDefaultDestinationPosition();

        [PreserveSig]
        new int GetVideoSize(
            [Out] out int pWidth,
            [Out] out int pHeight
            );

        [PreserveSig]
        new int GetVideoPaletteEntries(
            [In] int StartIndex,
            [In] int Entries,
            [Out] out int pRetrieved,
            [Out] out int [] pPalette
            );

        [PreserveSig]
        new int GetCurrentImage(
            [In, Out] ref int pBufferSize,
            [Out] IntPtr pDIBImage // int *
            );

        [PreserveSig]
        new int IsUsingDefaultSource();

        [PreserveSig]
        new int IsUsingDefaultDestination();

        #endregion

        [PreserveSig]
        int GetPreferredAspectRatio(
            [Out] out int plAspectX,
            [Out] out int plAspectY
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a868b6-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IMediaEvent
    {
        [PreserveSig]
        int GetEventHandle([Out] out IntPtr hEvent); // HEVENT

        [PreserveSig]
        int GetEvent(
            [Out] out EventCode lEventCode,
            [Out] out IntPtr lParam1,
            [Out] out IntPtr lParam2,
            [In] int msTimeout
            );

        [PreserveSig]
        int WaitForCompletion(
            [In] int msTimeout,
            [Out] out EventCode pEvCode
            );

        [PreserveSig]
        int CancelDefaultHandling([In] EventCode lEvCode);

        [PreserveSig]
        int RestoreDefaultHandling([In] EventCode lEvCode);

        [PreserveSig]
        int FreeEventParams(
            [In] EventCode lEvCode,
            [In] IntPtr lParam1,
            [In] IntPtr lParam2
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a868c0-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IMediaEventEx : IMediaEvent
    {
        #region IMediaEvent Methods

        [PreserveSig]
        new int GetEventHandle([Out] out IntPtr hEvent); // HEVENT

        [PreserveSig]
        new int GetEvent(
            [Out] out EventCode lEventCode,
            [Out] out IntPtr lParam1,
            [Out] out IntPtr lParam2,
            [In] int msTimeout
            );

        [PreserveSig]
        new int WaitForCompletion(
            [In] int msTimeout,
            [Out] out EventCode pEvCode
            );

        [PreserveSig]
        new int CancelDefaultHandling([In] EventCode lEvCode);

        [PreserveSig]
        new int RestoreDefaultHandling([In] EventCode lEvCode);

        [PreserveSig]
        new int FreeEventParams(
            [In] EventCode lEvCode,
            [In] IntPtr lParam1,
            [In] IntPtr lParam2
            );

        #endregion

        [PreserveSig]
        int SetNotifyWindow(
            [In] IntPtr hwnd, // HWND *
            [In] int lMsg,
            [In] IntPtr lInstanceData // PVOID
            );

        [PreserveSig]
        int SetNotifyFlags([In] NotifyFlags lNoNotifyFlags);

        [PreserveSig]
        int GetNotifyFlags([Out] out NotifyFlags lplNoNotifyFlags);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a868b2-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IMediaPosition
    {
        [PreserveSig]
        int get_Duration([Out] out double pLength);

        [PreserveSig]
        int put_CurrentPosition([In] double llTime);

        [PreserveSig]
        int get_CurrentPosition([Out] out double pllTime);

        [PreserveSig]
        int get_StopTime([Out] out double pllTime);

        [PreserveSig]
        int put_StopTime([In] double llTime);

        [PreserveSig]
        int get_PrerollTime([Out] out double pllTime);

        [PreserveSig]
        int put_PrerollTime([In] double llTime);

        [PreserveSig]
        int put_Rate([In] double dRate);

        [PreserveSig]
        int get_Rate([Out] out double pdRate);

        [PreserveSig]
        int CanSeekForward([Out] out OABool pCanSeekForward);

        [PreserveSig]
        int CanSeekBackward([Out] out OABool pCanSeekBackward);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a868b1-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IMediaControl
    {
        [PreserveSig]
        int Run();

        [PreserveSig]
        int Pause();

        [PreserveSig]
        int Stop();

        [PreserveSig]
        int GetState([In] int msTimeout, [Out] out FilterState pfs);

        [PreserveSig]
        int RenderFile([In, MarshalAs(UnmanagedType.BStr)] string strFilename);

        [PreserveSig,
        Obsolete("Automation interface, for pre-.NET VB.  Use IGraphBuilder::AddSourceFilter instead", false)]
        int AddSourceFilter(
            [In, MarshalAs(UnmanagedType.BStr)] string strFilename,
            [Out, MarshalAs(UnmanagedType.IDispatch)] out object ppUnk
            );

        [PreserveSig,
        Obsolete("Automation interface, for pre-.NET VB.  Use IFilterGraph::EnumFilters instead", false)]
        int get_FilterCollection([Out, MarshalAs(UnmanagedType.IDispatch)] out object ppUnk);

        [PreserveSig,
        Obsolete("Automation interface, for pre-.NET VB.  Use IFilterMapper2::EnumMatchingFilters instead", false)]
        int get_RegFilterCollection([Out, MarshalAs(UnmanagedType.IDispatch)] out object ppUnk);

        [PreserveSig]
        int StopWhenReady();
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a868b7-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IQueueCommand
    {
        [PreserveSig]
        int InvokeAtStreamTime(
            [Out] out IDeferredCommand pCmd,
            [In] double time,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid iid,
            [In] int dispidMethod,
            [In] DispatchFlags wFlags,
            [In] int cArgs,
            [In] object[] pDispParams,
            [In] IntPtr pvarResult,
            [Out] out short puArgErr
            );

        int InvokeAtPresentationTime(
            [Out] out IDeferredCommand pCmd,
            [In] double time,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid iid,
            [In] int dispidMethod,
            [In] DispatchFlags wFlags,
            [In] int cArgs,
            [In] object[] pDispParams,
            [In] IntPtr pvarResult,
            [Out] out short puArgErr
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a868b8-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDeferredCommand
    {
        [PreserveSig]
        int Cancel();

        [PreserveSig]
        int Confidence([Out] out int pConfidence);

        [PreserveSig]
        int Postpone([In] double newtime);

        [PreserveSig]
        int GetHResult([Out] out int phrResult);
    }

    #endregion
}
