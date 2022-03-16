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
    /// From AM_GBF_* defines
    /// </summary>
    [Flags]
    public enum AMGBF
    {
        None = 0,
        PrevFrameSkipped = 1,
        NotAsyncPoint = 2,
        NoWait = 4,
        NoDDSurfaceLock = 8
    }

    /// <summary>
    /// From AM_VIDEO_FLAG_* defines
    /// </summary>
    [Flags]
    public enum AMVideoFlag
    {
        FieldMask         = 0x0003,
        InterleavedFrame  = 0x0000,
        Field1            = 0x0001,
        Field2            = 0x0002,
        Field1First       = 0x0004,
        Weave             = 0x0008,
        IPBMask           = 0x0030,
        ISample           = 0x0000,
        PSample           = 0x0010,
        BSample           = 0x0020,
        RepeatField       = 0x0040
    }

    /// <summary>
    /// From AM_SAMPLE_PROPERTY_FLAGS
    /// </summary>
    [Flags]
    public enum AMSamplePropertyFlags
    {
        SplicePoint = 0x01,
        PreRoll = 0x02,
        DataDiscontinuity = 0x04,
        TypeChanged = 0x08,
        TimeValid = 0x10,
        MediaTimeValid  = 0x20,
        TimeDiscontinuity = 0x40,
        FlushOnPause = 0x80,
        StopValid = 0x100,
        EndOfStream = 0x200,
        Media = 0,
        Control = 1
    }

    /// <summary>
    /// From PIN_INFO
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    public struct PinInfo
    {
        [MarshalAs(UnmanagedType.Interface)] public IBaseFilter filter;
        public PinDirection dir;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=128)] public string name;
    }

    /// <summary>
    /// From AM_MEDIA_TYPE - When you are done with an instance of this class,
    /// it should be released with FreeAMMediaType() to avoid leaking
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class AMMediaType
    {
        public Guid majorType;
        public Guid subType;
        [MarshalAs(UnmanagedType.Bool)] public bool fixedSizeSamples;
        [MarshalAs(UnmanagedType.Bool)] public bool temporalCompression;
        public int sampleSize;
        public Guid formatType;
        public IntPtr unkPtr; // IUnknown Pointer
        public int formatSize;
        public IntPtr formatPtr; // Pointer to a buff determined by formatType

        public override string ToString()
        {
            return "{" + majorType + "},{" + subType + "}";
        }
    }

    /// <summary>
    /// From PIN_DIRECTION
    /// </summary>
    public enum PinDirection
    {
        Input,
        Output
    }

    /// <summary>
    /// From AM_SEEKING_SeekingCapabilities
    /// </summary>
    [Flags]
    public enum AMSeekingSeekingCapabilities
    {
        None = 0,
        CanSeekAbsolute = 0x001,
        CanSeekForwards = 0x002,
        CanSeekBackwards = 0x004,
        CanGetCurrentPos = 0x008,
        CanGetStopPos = 0x010,
        CanGetDuration = 0x020,
        CanPlayBackwards = 0x040,
        CanDoSegments = 0x080,
        Source = 0x100
    }

    /// <summary>
    /// From FILTER_STATE
    /// </summary>
    public enum FilterState
    {
        Stopped,
        Paused,
        Running
    }

    /// <summary>
    /// From FILTER_INFO
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    public struct FilterInfo
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=128)] public string achName;
        [MarshalAs(UnmanagedType.Interface)] public IFilterGraph pGraph;
    }

    /// <summary>
    /// From AM_SEEKING_SeekingFlags
    /// </summary>
    [Flags]
    public enum AMSeekingSeekingFlags
    {
        NoPositioning = 0x00,
        AbsolutePositioning = 0x01,
        RelativePositioning = 0x02,
        IncrementalPositioning = 0x03,
        PositioningBitsMask = 0x03,
        SeekToKeyFrame = 0x04,
        ReturnTime = 0x08,
        Segment = 0x10,
        NoFlush = 0x20
    }

    /// <summary>
    /// From ALLOCATOR_PROPERTIES
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class AllocatorProperties
    {
        public int cBuffers;
        public int cbBuffer;
        public int cbAlign;
        public int cbPrefix;
    }

    /// <summary>
    /// From AM_SAMPLE2_PROPERTIES
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class AMSample2Properties
    {
        public int cbData;
        public AMVideoFlag dwTypeSpecificFlags;
        public AMSamplePropertyFlags dwSampleFlags;
        public int lActual;
        public long tStart;
        public long tStop;
        public int dwStreamId;
        public IntPtr pMediaType;
        public IntPtr pbBuffer; // BYTE *
        public int cbBuffer;
    }

    #endregion

    #region Interfaces

#if ALLOW_UNTESTED_INTERFACES

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("68961E68-832B-41ea-BC91-63593F3E70E3"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMediaSample2Config
    {
        [PreserveSig]
        int GetSurface(
            [MarshalAs(UnmanagedType.IUnknown)] out object ppDirect3DSurface9
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("36b73885-c2c8-11cf-8b46-00805f6cef60"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IReferenceClock2 : IReferenceClock
    {
        #region IReferenceClock Methods

        [PreserveSig]
        new int GetTime([Out] out long pTime);

        [PreserveSig]
        new int AdviseTime(
            [In] long baseTime,
            [In] long streamTime,
            [In] IntPtr hEvent, // System.Threading.WaitHandle?
            [Out] out int pdwAdviseCookie
            );

        [PreserveSig]
        new int AdvisePeriodic(
            [In] long startTime,
            [In] long periodTime,
            [In] IntPtr hSemaphore, // System.Threading.WaitHandle?
            [Out] out int pdwAdviseCookie
            );

        [PreserveSig]
        new int Unadvise([In] int dwAdviseCookie);

        #endregion
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a8689d-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMemInputPin
    {
        [PreserveSig]
        int GetAllocator([Out] out IMemAllocator ppAllocator);

        [PreserveSig]
        int NotifyAllocator(
            [In] IMemAllocator pAllocator,
            [In, MarshalAs(UnmanagedType.Bool)] bool bReadOnly
            );

        [PreserveSig]
        int GetAllocatorRequirements([Out] out AllocatorProperties pProps);

        [PreserveSig]
        int Receive([In] IMediaSample pSample);

        [PreserveSig]
        int ReceiveMultiple(
            [In] IntPtr pSamples, // IMediaSample[]
            [In] int nSamples,
            [Out] out int nSamplesProcessed
            );

        [PreserveSig]
        int ReceiveCanBlock();
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("a3d8cec0-7e5a-11cf-bbc5-00805f6cef20"),
    Obsolete("This interface has been deprecated.", false),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMovieSetup
    {
        [PreserveSig]
        int Register();

        [PreserveSig]
        int Unregister();
    }

#endif

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a86891-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPin
    {
        [PreserveSig]
        int Connect(
            [In] IPin pReceivePin,
            [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt
            );

        [PreserveSig]
        int ReceiveConnection(
            [In] IPin pReceivePin,
            [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt
            );

        [PreserveSig]
        int Disconnect();

        [PreserveSig]
        int ConnectedTo(
            [Out] out IPin ppPin);

        /// <summary>
        /// Release returned parameter with DsUtils.FreeAMMediaType
        /// </summary>
        [PreserveSig]
        int ConnectionMediaType(
            [Out, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);

        /// <summary>
        /// Release returned parameter with DsUtils.FreePinInfo
        /// </summary>
        [PreserveSig]
        int QueryPinInfo([Out] out PinInfo pInfo);

        [PreserveSig]
        int QueryDirection(out PinDirection pPinDir);

        [PreserveSig]
        int QueryId([Out, MarshalAs(UnmanagedType.LPWStr)] out string Id);

        [PreserveSig]
        int QueryAccept([In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);

        [PreserveSig]
        int EnumMediaTypes([Out] out IEnumMediaTypes ppEnum);

        [PreserveSig]
        int QueryInternalConnections(
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=1)] IPin[] ppPins,
            [In, Out] ref int nPin
            );

        [PreserveSig]
        int EndOfStream();

        [PreserveSig]
        int BeginFlush();

        [PreserveSig]
        int EndFlush();

        [PreserveSig]
        int NewSegment(
            [In] long tStart,
            [In] long tStop,
            [In] double dRate
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("36b73880-c2c8-11cf-8b46-00805f6cef60"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMediaSeeking
    {
        [PreserveSig]
        int GetCapabilities([Out] out AMSeekingSeekingCapabilities pCapabilities);

        [PreserveSig]
        int CheckCapabilities([In, Out] ref AMSeekingSeekingCapabilities pCapabilities);

        [PreserveSig]
        int IsFormatSupported([In, MarshalAs(UnmanagedType.LPStruct)] Guid pFormat);

        [PreserveSig]
        int QueryPreferredFormat([Out] out Guid pFormat);

        [PreserveSig]
        int GetTimeFormat([Out] out Guid pFormat);

        [PreserveSig]
        int IsUsingTimeFormat([In, MarshalAs(UnmanagedType.LPStruct)] Guid pFormat);

        [PreserveSig]
        int SetTimeFormat([In, MarshalAs(UnmanagedType.LPStruct)] Guid pFormat);

        [PreserveSig]
        int GetDuration([Out] out long pDuration);

        [PreserveSig]
        int GetStopPosition([Out] out long pStop);

        [PreserveSig]
        int GetCurrentPosition([Out] out long pCurrent);

        [PreserveSig]
        int ConvertTimeFormat(
            [Out] out long pTarget,
            [In, MarshalAs(UnmanagedType.LPStruct)] DsGuid pTargetFormat,
            [In] long Source,
            [In, MarshalAs(UnmanagedType.LPStruct)] DsGuid pSourceFormat
            );

        [PreserveSig]
        int SetPositions(
            [In, Out, MarshalAs(UnmanagedType.LPStruct)] DsLong pCurrent,
            [In] AMSeekingSeekingFlags dwCurrentFlags,
            [In, Out, MarshalAs(UnmanagedType.LPStruct)] DsLong pStop,
            [In] AMSeekingSeekingFlags dwStopFlags
            );

        [PreserveSig]
        int GetPositions(
            [Out] out long pCurrent,
            [Out] out long pStop
            );

        [PreserveSig]
        int GetAvailable(
            [Out] out long pEarliest,
            [Out] out long pLatest
            );

        [PreserveSig]
        int SetRate([In] double dRate);

        [PreserveSig]
        int GetRate([Out] out double pdRate);

        [PreserveSig]
        int GetPreroll([Out] out long pllPreroll);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a8689a-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMediaSample
    {
        [PreserveSig]
        int GetPointer([Out] out IntPtr ppBuffer); // BYTE **

        [PreserveSig]
        int GetSize();

        [PreserveSig]
        int GetTime(
            [Out] out long pTimeStart,
            [Out] out long pTimeEnd
            );

        [PreserveSig]
        int SetTime(
            [In, MarshalAs(UnmanagedType.LPStruct)] DsLong pTimeStart,
            [In, MarshalAs(UnmanagedType.LPStruct)] DsLong pTimeEnd
            );

        [PreserveSig]
        int IsSyncPoint();

        [PreserveSig]
        int SetSyncPoint([In, MarshalAs(UnmanagedType.Bool)] bool bIsSyncPoint);

        [PreserveSig]
        int IsPreroll();

        [PreserveSig]
        int SetPreroll([In, MarshalAs(UnmanagedType.Bool)] bool bIsPreroll);

        [PreserveSig]
        int GetActualDataLength();

        [PreserveSig]
        int SetActualDataLength([In] int len);

        /// <summary>
        /// Returned object must be released with DsUtils.FreeAMMediaType()
        /// </summary>
        [PreserveSig]
        int GetMediaType([Out, MarshalAs(UnmanagedType.LPStruct)] out AMMediaType ppMediaType);

        [PreserveSig]
        int SetMediaType([In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pMediaType);

        [PreserveSig]
        int IsDiscontinuity();

        [PreserveSig]
        int SetDiscontinuity([In, MarshalAs(UnmanagedType.Bool)] bool bDiscontinuity);

        [PreserveSig]
        int GetMediaTime(
            [Out] out long pTimeStart,
            [Out] out long pTimeEnd
            );

        [PreserveSig]
        int SetMediaTime(
            [In, MarshalAs(UnmanagedType.LPStruct)] DsLong pTimeStart,
            [In, MarshalAs(UnmanagedType.LPStruct)] DsLong pTimeEnd
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a86899-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMediaFilter : IPersist
    {
        #region IPersist Methods

        [PreserveSig]
        new int GetClassID(
            [Out] out Guid pClassID);

        #endregion

        [PreserveSig]
        int Stop();

        [PreserveSig]
        int Pause();

        [PreserveSig]
        int Run([In] long tStart);

        [PreserveSig]
        int GetState(
            [In] int dwMilliSecsTimeout,
            [Out] out FilterState filtState
            );

        [PreserveSig]
        int SetSyncSource([In] IReferenceClock pClock);

        [PreserveSig]
        int GetSyncSource([Out] out IReferenceClock pClock);
    }

     [ComImport, SuppressUnmanagedCodeSecurity,
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
     Guid("FA993888-4383-415A-A930-DD472A8CF6F7")]
     public interface IMFGetService
     {
       void GetService(
           [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidService,
           [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
           [MarshalAs(UnmanagedType.Interface)] out object ppvObject
           );
     }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a86895-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBaseFilter : IMediaFilter
    {
        #region IPersist Methods

        [PreserveSig]
        new int GetClassID(
            [Out] out Guid pClassID);

        #endregion

        #region IMediaFilter Methods

        [PreserveSig]
        new int Stop();

        [PreserveSig]
        new int Pause();

        [PreserveSig]
        new int Run(long tStart);

        [PreserveSig]
        new int GetState([In] int dwMilliSecsTimeout, [Out] out FilterState filtState);

        [PreserveSig]
        new int SetSyncSource([In] IReferenceClock pClock);

        [PreserveSig]
        new int GetSyncSource([Out] out IReferenceClock pClock);

        #endregion

        [PreserveSig]
        int EnumPins([Out] out IEnumPins ppEnum);

        [PreserveSig]
        int FindPin(
            [In, MarshalAs(UnmanagedType.LPWStr)] string Id,
            [Out] out IPin ppPin
            );

        [PreserveSig]
        int QueryFilterInfo([Out] out FilterInfo pInfo);

        [PreserveSig]
        int JoinFilterGraph(
            [In] IFilterGraph pGraph,
            [In, MarshalAs(UnmanagedType.LPWStr)] string pName
            );

        [PreserveSig]
        int QueryVendorInfo([Out, MarshalAs(UnmanagedType.LPWStr)] out string pVendorInfo);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a8689f-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFilterGraph
    {
        [PreserveSig]
        int AddFilter(
            [In] IBaseFilter pFilter,
            [In, MarshalAs(UnmanagedType.LPWStr)] string pName
            );

        [PreserveSig]
        int RemoveFilter([In] IBaseFilter pFilter);

        [PreserveSig]
        int EnumFilters([Out] out IEnumFilters ppEnum);

        [PreserveSig]
        int FindFilterByName(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pName,
            [Out] out IBaseFilter ppFilter
            );

        [PreserveSig]
        int ConnectDirect(
            [In] IPin ppinOut,
            [In] IPin ppinIn,
            [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt
            );

        [PreserveSig]
        [Obsolete("This method is obsolete; use the IFilterGraph2.ReconnectEx method instead.")]
        int Reconnect([In] IPin ppin);

        [PreserveSig]
        int Disconnect([In] IPin ppin);

        [PreserveSig]
        int SetDefaultSyncSource();
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a86893-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumFilters
    {
        [PreserveSig]
        int Next(
            [In] int cFilters,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] IBaseFilter[] ppFilter,
            [Out] out int pcFetched
            );

        [PreserveSig]
        int Skip([In] int cFilters);

        [PreserveSig]
        int Reset();

        [PreserveSig]
        int Clone([Out] out IEnumFilters ppEnum);
    }

    /// <summary>
    /// Enumerates pins on a filter.
    /// </summary>
    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a86892-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumPins
    {
        /// <summary>
        /// The Next method retrieves a specified number of pins in the enumeration sequence.
        /// </summary>
        /// <param name="cPins">Number of pins to retrieve.</param>
        /// <param name="ppPins">Array of size cPins that is filled with IPin pointers. The caller must release the interfaces.</param>
        /// <param name="pcFetched">Pointer to a variable that receives the number of pins retrieved.</param>
        /// <returns>Returns one of the following HRESULT values.
        /// <b>S_FALSE</b> - Did not retrieve as many pins as requested.
        /// <b>S_OK</b> - Success.
        /// <b>E_INVALIDARG</b> - Invalid argument.
        /// <b>E_POINTER</b> - <b>null</b> pointer argument.
        /// <b>VFW_E_ENUM_OUT_OF_SYNC</b> - The filter's state has changed and is now inconsistent with the enumerator.
        /// </returns>
        /// <remarks>
        /// This method retrieves pointers to the specified number of pins, starting at the current position in the enumeration, and places them in the 
        /// specified array. If the method succeeds, the IPin pointers all have outstanding reference counts.Be sure to release them when you are done.
        /// If the number of pins changes, the enumerator is no longer consistent with the filter, and the method returns VFW_E_ENUM_OUT_OF_SYNC.
        /// Discard any data obtained from previous calls to the enumerator, because it might be invalid.Update the enumerator by calling the 
        /// <see cref="IEnumPins.Reset"/> method. You can then call the Next method safely.
        /// </remarks>
        [PreserveSig]
        int Next(
            [In] int cPins,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] IPin[] ppPins,
            [Out] out int pcFetched
            );

        /// <summary>
        /// The Skip method skips over a specified number of pins.
        /// </summary>
        /// <param name="cPins">Number of pins to skip.</param>
        /// <returns>Returns one of the following HRESULT
        /// <b>S_FALSE</b> - Skipped past the end of the sequence.
        /// <b>S_OK</b> - Success.
        /// <b>VFW_E_ENUM_OUT_OF_SYNC</b> - The filter's state has changed and is now inconsistent with the enumerator.
        /// </returns>
        [PreserveSig]
        int Skip([In] int cPins);

        /// <summary>
        /// The Reset method resets the enumeration sequence to the beginning.
        /// </summary>
        /// <returns>Returns S_OK.</returns>
        [PreserveSig]
        int Reset();

        /// <summary>
        /// The Clone method makes a copy of the enumerator with the same enumeration state.
        /// </summary>
        /// <param name="ppEnum">Receives a pointer to the <see cref="IEnumPins"/> interface of the new enumerator. The caller must release the interface.</param>
        /// <returns>Returns one of the following HRESULT
        /// <b>S_FALSE</b> - Did not retrieve as many pins as requested.
        /// <b>S_OK</b> - Success.
        /// <b>E_OUTOFMEMORY</b> - Insufficient memory.
        /// <b>E_POINTER</b> - <b>null</b> pointer argument.
        /// <b>VFW_E_ENUM_OUT_OF_SYNC</b> - The filter's state has changed and is now inconsistent with the enumerator.
        /// </returns>
        /// <remarks>
        /// If the number of pins changes, the enumerator is no longer consistent with the filter, and the method returns VFW_E_ENUM_OUT_OF_SYNC. 
        /// Discard any data obtained from previous calls to the enumerator, because it might be invalid. Update the enumerator by calling the 
        /// <see cref="IEnumPins.Reset"/> method. You can then call the Clone method safely.</remarks>
        [PreserveSig]
        int Clone([Out] out IEnumPins ppEnum);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a86897-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IReferenceClock
    {
        [PreserveSig]
        int GetTime([Out] out long pTime);

        [PreserveSig]
        int AdviseTime(
            [In] long baseTime,
            [In] long streamTime,
            [In] IntPtr hEvent, // System.Threading.WaitHandle?
            [Out] out int pdwAdviseCookie
            );

        [PreserveSig]
        int AdvisePeriodic(
            [In] long startTime,
            [In] long periodTime,
            [In] IntPtr hSemaphore, // System.Threading.WaitHandle?
            [Out] out int pdwAdviseCookie
            );

        [PreserveSig]
        int Unadvise([In] int dwAdviseCookie);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("89c31040-846b-11ce-97d3-00aa0055595a"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumMediaTypes
    {
        [PreserveSig]
        int Next(
            [In] int cMediaTypes,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(EMTMarshaler), SizeParamIndex = 0)] AMMediaType[] ppMediaTypes,
            [Out] out int pcFetched
            );

        [PreserveSig]
        int Skip([In] int cMediaTypes);

        [PreserveSig]
        int Reset();

        [PreserveSig]
        int Clone([Out] out IEnumMediaTypes ppEnum);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("36b73884-c2c8-11cf-8b46-00805f6cef60"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMediaSample2 : IMediaSample
    {
        #region IMediaSample Methods

        [PreserveSig]
        new int GetPointer([Out] out IntPtr ppBuffer); // BYTE **

        [PreserveSig]
        new int GetSize();

        [PreserveSig]
        new int GetTime(
            [Out] out long pTimeStart,
            [Out] out long pTimeEnd
            );

        [PreserveSig]
        new int SetTime(
            [In, MarshalAs(UnmanagedType.LPStruct)] DsLong pTimeStart,
            [In, MarshalAs(UnmanagedType.LPStruct)] DsLong pTimeEnd
            );

        [PreserveSig]
        new int IsSyncPoint();

        [PreserveSig]
        new int SetSyncPoint([In, MarshalAs(UnmanagedType.Bool)] bool bIsSyncPoint);

        [PreserveSig]
        new int IsPreroll();

        [PreserveSig]
        new int SetPreroll([In, MarshalAs(UnmanagedType.Bool)] bool bIsPreroll);

        [PreserveSig]
        new int GetActualDataLength();

        [PreserveSig]
        new int SetActualDataLength([In] int len);

        [PreserveSig]
        new int GetMediaType([Out] out AMMediaType ppMediaType);

        [PreserveSig]
        new int SetMediaType([In] AMMediaType pMediaType);

        [PreserveSig]
        new int IsDiscontinuity();

        [PreserveSig]
        new int SetDiscontinuity([In, MarshalAs(UnmanagedType.Bool)] bool bDiscontinuity);

        [PreserveSig]
        new int GetMediaTime(
            [Out] out long pTimeStart,
            [Out] out long pTimeEnd
            );

        [PreserveSig]
        new int SetMediaTime(
            [In, MarshalAs(UnmanagedType.LPStruct)] DsLong pTimeStart,
            [In, MarshalAs(UnmanagedType.LPStruct)] DsLong pTimeEnd
            );

        #endregion

        [PreserveSig]
        int GetProperties(
            [In] int cbProperties,
            [In] IntPtr pbProperties // BYTE *
            );

        [PreserveSig]
        int SetProperties(
            [In] int cbProperties,
            [In] IntPtr pbProperties // BYTE *
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("92980b30-c1de-11d2-abf5-00a0c905f375"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMemAllocatorNotifyCallbackTemp
    {
        [PreserveSig]
        int NotifyRelease();
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("379a0cf0-c1de-11d2-abf5-00a0c905f375"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMemAllocatorCallbackTemp : IMemAllocator
    {
        #region IMemAllocator Methods

        [PreserveSig]
        new int SetProperties(
            [In] AllocatorProperties pRequest,
            [Out, MarshalAs(UnmanagedType.LPStruct)] AllocatorProperties pActual
            );

        [PreserveSig]
        new int GetProperties([Out] AllocatorProperties pProps);

        [PreserveSig]
        new int Commit();

        [PreserveSig]
        new int Decommit();

        [PreserveSig]
        new int GetBuffer(
            [Out] out IMediaSample ppBuffer,
            [In] long pStartTime,
            [In] long pEndTime,
            [In] AMGBF dwFlags
            );

        [PreserveSig]
        new int ReleaseBuffer([In] IMediaSample pBuffer);

        #endregion

        [PreserveSig]
        int SetNotify([In] IMemAllocatorNotifyCallbackTemp pNotify);

        [PreserveSig]
        int GetFreeCount([Out] out int plBuffersFree);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a8689c-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMemAllocator
    {
        [PreserveSig]
        int SetProperties(
            [In, MarshalAs(UnmanagedType.LPStruct)] AllocatorProperties pRequest,
            [Out, MarshalAs(UnmanagedType.LPStruct)] AllocatorProperties pActual
            );

        [PreserveSig]
        int GetProperties(
            [Out, MarshalAs(UnmanagedType.LPStruct)] AllocatorProperties pProps
            );

        [PreserveSig]
        int Commit();

        [PreserveSig]
        int Decommit();

        [PreserveSig]
        int GetBuffer(
            [Out] out IMediaSample ppBuffer,
            [In] long pStartTime,
            [In] long pEndTime,
            [In] AMGBF dwFlags
            );

        [PreserveSig]
        int ReleaseBuffer(
            [In] IMediaSample pBuffer
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("ebec459c-2eca-4d42-a8af-30df557614b8"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IReferenceClockTimerControl
    {
        [PreserveSig]
        int SetDefaultTimerResolution(
            long timerResolution
            );

        [PreserveSig]
        int GetDefaultTimerResolution(
            out long pTimerResolution
            );
    }

    #endregion
}
