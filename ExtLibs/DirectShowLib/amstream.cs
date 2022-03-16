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
#if !USING_NET11
using System.Runtime.InteropServices.ComTypes;
#endif

namespace DirectShowLib.MultimediaStreaming
{
    #region Declarations

    /// <summary>
    /// From unnamed enum
    /// </summary>
    [Flags]
    public enum AMMStream
    {
        None = 0x0,
        AddDefaultRenderer = 0x1,
        CreatePeer = 0x2,
        StopIfNoSamples = 0x4,
        NoStall = 0x8
    }

    /// <summary>
    /// From unnamed enum
    /// </summary>
    [Flags]
    public enum AMMMultiStream
    {
        None = 0x0,
        NoGraphThread = 0x1
    }

    /// <summary>
    /// From unnamed enum
    /// </summary>
    [Flags]
    public enum AMOpenModes
    {
        RenderTypeMask = 0x3,
        RenderToExisting = 0,
        RenderAllStreams = 0x1,
        NoRender = 0x2,
        NoClock = 0x4,
        Run = 0x8
    }

    #endregion

    #region Interfaces

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("BEBE595D-9A6F-11D0-8FDE-00C04FD9189D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMMediaStream : IMediaStream
    {
        #region IMediaStream Methods

        [PreserveSig]
        new int GetMultiMediaStream(
            [MarshalAs(UnmanagedType.Interface)] out IMultiMediaStream ppMultiMediaStream
            );

        [PreserveSig]
        new int GetInformation(
            out Guid pPurposeId,
            out StreamType pType
            );

        [PreserveSig]
        new int SetSameFormat(
            [In, MarshalAs(UnmanagedType.Interface)] IMediaStream pStreamThatHasDesiredFormat,
            [In] int dwFlags
            );

        [PreserveSig]
        new int AllocateSample(
            [In] int dwFlags,
            [MarshalAs(UnmanagedType.Interface)] out IStreamSample ppSample
            );

        [PreserveSig]
        new int CreateSharedSample(
            [In, MarshalAs(UnmanagedType.Interface)] IStreamSample pExistingSample,
            [In] int dwFlags,
            [MarshalAs(UnmanagedType.Interface)] out IStreamSample ppNewSample
            );

        [PreserveSig]
        new int SendEndOfStream(
            int dwFlags
            );

        #endregion

        [PreserveSig]
        int Initialize(
            [In, MarshalAs(UnmanagedType.IUnknown)] object pSourceObject,
            [In] AMMStream dwFlags,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid PurposeId,
            [In] StreamType StreamType
            );

        [PreserveSig]
        int SetState(
            [In] FilterState State
            );

        [PreserveSig]
        int JoinAMMultiMediaStream(
            [In, MarshalAs(UnmanagedType.Interface)] IAMMultiMediaStream pAMMultiMediaStream
            );

        [PreserveSig]
        int JoinFilter(
            [In, MarshalAs(UnmanagedType.Interface)] IMediaStreamFilter pMediaStreamFilter
            );

        [PreserveSig]
        int JoinFilterGraph(
            [In, MarshalAs(UnmanagedType.Interface)] IFilterGraph pFilterGraph
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("BEBE595C-9A6F-11D0-8FDE-00C04FD9189D")]
    public interface IAMMultiMediaStream : IMultiMediaStream
    {
        #region IMultiMediaStream Methods

        [PreserveSig]
        new int GetInformation(
            out MMSSF pdwFlags,
            out StreamType pStreamType
            );

        [PreserveSig]
        new int GetMediaStream(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid idPurpose,
            [MarshalAs(UnmanagedType.Interface)] out IMediaStream ppMediaStream
            );

        [PreserveSig]
        new int EnumMediaStreams(
            [In] int Index,
            [MarshalAs(UnmanagedType.Interface)] out IMediaStream ppMediaStream
            );

        [PreserveSig]
        new int GetState(
            out StreamState pCurrentState
            );

        [PreserveSig]
        new int SetState(
            [In] StreamState NewState
            );

        [PreserveSig]
        new int GetTime(
            out long pCurrentTime
            );

        [PreserveSig]
        new int GetDuration(
            out long pDuration
            );

        [PreserveSig]
        new int Seek(
            [In] long SeekTime
            );

        [PreserveSig]
        new int GetEndOfStreamEventHandle(
            out IntPtr phEOS
            );

        #endregion

        [PreserveSig]
        int Initialize(
            [In] StreamType StreamType,
            [In] AMMMultiStream dwFlags,
            [In, MarshalAs(UnmanagedType.Interface)] IGraphBuilder pFilterGraph
            );

        [PreserveSig]
        int GetFilterGraph(
            [MarshalAs(UnmanagedType.Interface)] out IGraphBuilder ppGraphBuilder
            );

        [PreserveSig]
        int GetFilter(
            [MarshalAs(UnmanagedType.Interface)] out IMediaStreamFilter ppFilter
            );

        [PreserveSig]
        int AddMediaStream(
            [In, MarshalAs(UnmanagedType.IUnknown)] object pStreamObject,
            [In] DsGuid PurposeId,
            [In] AMMStream dwFlags,
            [Out] IMediaStream ppNewStream
            );

        [PreserveSig]
        int OpenFile(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
            [In] AMOpenModes dwFlags
            );

        [PreserveSig]
        int OpenMoniker(
#if USING_NET11
            [In, MarshalAs(UnmanagedType.Interface)] UCOMIBindCtx pCtx,
            [In, MarshalAs(UnmanagedType.Interface)] UCOMIMoniker pMoniker,
#else
            [In, MarshalAs(UnmanagedType.Interface)] IBindCtx pCtx,
            [In, MarshalAs(UnmanagedType.Interface)] IMoniker pMoniker,
#endif
            [In] AMOpenModes dwFlags
            );

        [PreserveSig]
        int Render(
            [In] AMOpenModes dwFlags
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("AB6B4AFA-F6E4-11D0-900D-00C04FD9189D")]
    public interface IAMMediaTypeStream : IMediaStream
    {
        #region IMediaStream Methods

        [PreserveSig]
        new int GetMultiMediaStream(
            [MarshalAs(UnmanagedType.Interface)] out IMultiMediaStream ppMultiMediaStream
            );

        [PreserveSig]
        new int GetInformation(
            out Guid pPurposeId,
            out StreamType pType
            );

        [PreserveSig]
        new int SetSameFormat(
            [In, MarshalAs(UnmanagedType.Interface)] IMediaStream pStreamThatHasDesiredFormat,
            [In] int dwFlags
            );

        [PreserveSig]
        new int AllocateSample(
            [In] int dwFlags,
            [MarshalAs(UnmanagedType.Interface)] out IStreamSample ppSample
            );

        [PreserveSig]
        new int CreateSharedSample(
            [In, MarshalAs(UnmanagedType.Interface)] IStreamSample pExistingSample,
            [In] int dwFlags,
            [MarshalAs(UnmanagedType.Interface)] out IStreamSample ppNewSample
            );

        [PreserveSig]
        new int SendEndOfStream(
            int dwFlags
            );

        #endregion

        [PreserveSig]
        int GetFormat(
            [Out, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pMediaType,
            [In] int dwFlags
            );

        [PreserveSig]
        int SetFormat(
            [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pMediaType,
            [In] int dwFlags
            );

        [PreserveSig]
        int CreateSample(
            [In] int lSampleSize,
            [In] IntPtr pbBuffer,
            [In] int dwFlags,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter,
            [MarshalAs(UnmanagedType.Interface)] out IAMMediaTypeSample ppAMMediaTypeSample
            );

        [PreserveSig]
        int GetStreamAllocatorRequirements(
            out AllocatorProperties pProps
            );

        [PreserveSig]
        int SetStreamAllocatorRequirements(
            [In, MarshalAs(UnmanagedType.LPStruct)] AllocatorProperties pProps
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("AB6B4AFB-F6E4-11D0-900D-00C04FD9189D")]
    public interface IAMMediaTypeSample : IStreamSample
    {
        #region IStreamSample Methods

        [PreserveSig]
        new int GetMediaStream(
            [MarshalAs(UnmanagedType.Interface)] out IMediaStream ppMediaStream
            );

        [PreserveSig]
        new int GetSampleTimes(
            out long pStartTime,
            out long pEndTime,
            out long pCurrentTime
            );

        [PreserveSig]
        new int SetSampleTimes(
            [In] DsLong pStartTime,
            [In] DsLong pEndTime
            );

        [PreserveSig]
        new int Update(
            [In] SSUpdate dwFlags,
            [In] IntPtr hEvent,
            [In] IntPtr pfnAPC,
            [In] IntPtr dwAPCData
            );

        [PreserveSig]
        new int CompletionStatus(
            [In] CompletionStatusFlags dwFlags,
            [In] int dwMilliseconds
            );

        #endregion

        [PreserveSig]
        int SetPointer(
            [In] IntPtr pBuffer,
            [In] int lSize
            );

        [PreserveSig]
        int GetPointer(
            [Out] out IntPtr ppBuffer
            );

        [PreserveSig]
        int GetSize();

        [PreserveSig]
        int GetTime(
            out long pTimeStart,
            out long pTimeEnd
            );

        [PreserveSig]
        int SetTime(
            [In] DsLong pTimeStart,
            [In] DsLong pTimeEnd
            );

        [PreserveSig]
        int IsSyncPoint();

        [PreserveSig]
        int SetSyncPoint(
            [In, MarshalAs(UnmanagedType.Bool)] bool IsSyncPoint
            );

        [PreserveSig]
        int IsPreroll();

        [PreserveSig]
        int SetPreroll(
            [In, MarshalAs(UnmanagedType.Bool)] bool IsPreroll
            );

        [PreserveSig]
        int GetActualDataLength();

        [PreserveSig]
        int SetActualDataLength(
            int Size
            );

        [PreserveSig]
        int GetMediaType(
            out AMMediaType ppMediaType
            );

        [PreserveSig]
        int SetMediaType(
            [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pMediaType
            );

        [PreserveSig]
        int IsDiscontinuity();

        [PreserveSig]
        int SetDiscontinuity(
            [In, MarshalAs(UnmanagedType.Bool)] bool Discontinuity
            );

        [PreserveSig]
        int GetMediaTime(
            out long pTimeStart,
            out long pTimeEnd
            );

        [PreserveSig]
        int SetMediaTime(
            [In] DsLong pTimeStart,
            [In] DsLong pTimeEnd
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("BEBE595E-9A6F-11D0-8FDE-00C04FD9189D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMediaStreamFilter : IBaseFilter
    {
        #region IPersist Methods

        [PreserveSig]
        new int GetClassID(
            out Guid pClassID
            );

        #endregion

        #region IMediaFilter Methods

        [PreserveSig]
        new int Stop();

        [PreserveSig]
        new int Pause();

        [PreserveSig]
        new int Run(
            [In] long tStart
            );

        [PreserveSig]
        new int GetState(
            [In] int dwMilliSecsTimeout,
            out FilterState State
            );

        [PreserveSig]
        new int SetSyncSource(
            [In, MarshalAs(UnmanagedType.Interface)] IReferenceClock pClock
            );

        [PreserveSig]
        new int GetSyncSource(
            [MarshalAs(UnmanagedType.Interface)] out IReferenceClock pClock
            );

        #endregion

        #region IBaseFilter Methods

        [PreserveSig]
        new int EnumPins(
            [MarshalAs(UnmanagedType.Interface)] out IEnumPins ppEnum
            );

        [PreserveSig]
        new int FindPin(
            [In, MarshalAs(UnmanagedType.LPWStr)] string Id,
            [MarshalAs(UnmanagedType.Interface)] out IPin ppPin
            );

        [PreserveSig]
        new int QueryFilterInfo(
            out FilterInfo pInfo
            );

        [PreserveSig]
        new int JoinFilterGraph(
            [In, MarshalAs(UnmanagedType.Interface)] IFilterGraph pGraph,
            [In, MarshalAs(UnmanagedType.LPWStr)] string pName
            );

        [PreserveSig]
        new int QueryVendorInfo(
            [MarshalAs(UnmanagedType.LPWStr)] out string pVendorInfo
            );

        #endregion

        [PreserveSig]
        int AddMediaStream(
            [In, MarshalAs(UnmanagedType.Interface)] IAMMediaStream pAMMediaStream
            );

        [PreserveSig]
        int GetMediaStream(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid idPurpose,
            [MarshalAs(UnmanagedType.Interface)] out IMediaStream ppMediaStream
            );

        [PreserveSig]
        int EnumMediaStreams(
            [In] int Index,
            [MarshalAs(UnmanagedType.Interface)] out IMediaStream ppMediaStream
            );

        [PreserveSig]
        int SupportSeeking(
            [In, MarshalAs(UnmanagedType.Bool)] bool bRenderer
            );

        [PreserveSig]
        int ReferenceTimeToStreamTime(
            [In, Out] ref long pTime
            );

        [PreserveSig]
        int GetCurrentStreamTime(
            out long pCurrentStreamTime
            );

        [PreserveSig]
        int WaitUntil(
            [In] long WaitStreamTime
            );

        [PreserveSig]
        int Flush(
            [In, MarshalAs(UnmanagedType.Bool)] bool bCancelEOS
            );

        [PreserveSig]
        int EndOfStream();
    }

    #endregion
}
