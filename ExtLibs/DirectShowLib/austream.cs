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

namespace DirectShowLib.MultimediaStreaming
{
    #region Interfaces

#if ALLOW_UNTESTED_INTERFACES

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("327FC560-AF60-11D0-8212-00C04FC32C45")]
    public interface IMemoryData
    {
        [PreserveSig]
        int SetBuffer(
            [In] int cbSize,
            [In] IntPtr pbData,
            [In] int dwFlags
            );

        [PreserveSig]
        int GetInfo(
            out int pdwLength,
            [Out] IntPtr ppbData,
            out int pcbActualData
            );

        [PreserveSig]
        int SetActual(
            [In] int cbDataValid
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("54C719C0-AF60-11D0-8212-00C04FC32C45")]
    public interface IAudioData : IMemoryData
    {
        #region IMemoryData Methods

        [PreserveSig]
        new int SetBuffer(
            [In] int cbSize,
            [In] IntPtr pbData,
            [In] int dwFlags
            );

        [PreserveSig]
        new int GetInfo(
            out int pdwLength,
            [Out] IntPtr ppbData,
            out int pcbActualData
            );

        [PreserveSig]
        new int SetActual(
            [In] int cbDataValid
            );

        #endregion

        [PreserveSig]
        int GetFormat(
            [Out] WaveFormatEx pWaveFormatCurrent
            );

        [PreserveSig]
        int SetFormat(
            [In, MarshalAs(UnmanagedType.LPStruct)] WaveFormatEx lpWaveFormat
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("345FEE00-ABA5-11D0-8212-00C04FC32C45")]
    public interface IAudioStreamSample : IStreamSample
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
        int GetAudioData(
            [MarshalAs(UnmanagedType.Interface)] out IAudioData ppAudio
            );
    }


#endif

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("F7537560-A3BE-11D0-8212-00C04FC32C45")]
    public interface IAudioMediaStream : IMediaStream
    {
        #region IMediaStream Methods

        [PreserveSig]
        new int GetMultiMediaStream(
            [MarshalAs(UnmanagedType.Interface)] out IMultiMediaStream ppMultiMediaStream
            );

        [PreserveSig]
        new int GetInformation(
            out Guid pPurposeId,
            out StreamType pType);

        [PreserveSig]
        new int SetSameFormat(
            [In, MarshalAs(UnmanagedType.Interface)] IMediaStream pStreamThatHasDesiredFormat,
            [In] int dwFlags);

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
            [Out, MarshalAs(UnmanagedType.LPStruct)] WaveFormatEx pWaveFormatCurrent
            );

        [PreserveSig]
        int SetFormat(
            [In] WaveFormatEx lpWaveFormat
            );

        [PreserveSig]
        int CreateSample(
#if ALLOW_UNTESTED_INTERFACES
            [In, MarshalAs(UnmanagedType.Interface)] IAudioData pAudioData,
            [In] int dwFlags,
            [MarshalAs(UnmanagedType.Interface)] out IAudioStreamSample ppSample
#else
            [In, MarshalAs(UnmanagedType.Interface)] object pAudioData,
            [In] int dwFlags,
            [MarshalAs(UnmanagedType.Interface)] out object ppSample
#endif
            );
    }

    #endregion
}
