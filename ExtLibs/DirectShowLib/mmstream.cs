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
    #region Utility classes

    public sealed class MsResults
    {
        private MsResults()
        {
            // Prevent people from trying to instantiate this class
        }

        public const int S_Pending                  = unchecked((int)0x00040001);
        public const int S_NoUpdate                 = unchecked((int)0x00040002);
        public const int S_EndOfStream              = unchecked((int)0x00040003);
        public const int E_SampleAlloc              = unchecked((int)0x80040401);
        public const int E_PurposeId                = unchecked((int)0x80040402);
        public const int E_NoStream                 = unchecked((int)0x80040403);
        public const int E_NoSeeking                = unchecked((int)0x80040404);
        public const int E_Incompatible             = unchecked((int)0x80040405);
        public const int E_Busy                     = unchecked((int)0x80040406);
        public const int E_NotInit                  = unchecked((int)0x80040407);
        public const int E_SourceAlreadyDefined     = unchecked((int)0x80040408);
        public const int E_InvalidStreamType        = unchecked((int)0x80040409);
        public const int E_NotRunning               = unchecked((int)0x8004040a);
    }

    public sealed class MsError
    {
        private MsError()
        {
            // Prevent people from trying to instantiate this class
        }

        public static string GetErrorText(int hr)
        {
            string sRet = null;

            switch(hr)
            {
                case MsResults.S_Pending:
                    sRet = "Sample update is not yet complete.";
                    break;
                case MsResults.S_NoUpdate:
                    sRet = "Sample was not updated after forced completion.";
                    break;
                case MsResults.S_EndOfStream:
                    sRet = "End of stream. Sample not updated.";
                    break;
                case MsResults.E_SampleAlloc:
                    sRet = "An IMediaStream object could not be removed from an IMultiMediaStream object because it still contains at least one allocated sample.";
                    break;
                case MsResults.E_PurposeId:
                    sRet = "The specified purpose ID can't be used for the call.";
                    break;
                case MsResults.E_NoStream:
                    sRet = "No stream can be found with the specified attributes.";
                    break;
                case MsResults.E_NoSeeking:
                    sRet = "Seeking not supported for this IMultiMediaStream object.";
                    break;
                case MsResults.E_Incompatible:
                    sRet = "The stream formats are not compatible.";
                    break;
                case MsResults.E_Busy:
                    sRet = "The sample is busy.";
                    break;
                case MsResults.E_NotInit:
                    sRet = "The object can't accept the call because its initialize function or equivalent has not been called.";
                    break;
                case MsResults.E_SourceAlreadyDefined:
                    sRet = "Source already defined.";
                    break;
                case MsResults.E_InvalidStreamType:
                    sRet = "The stream type is not valid for this operation.";
                    break;
                case MsResults.E_NotRunning:
                    sRet = "The IMultiMediaStream object is not in running state.";
                    break;
                default:
                    sRet = DsError.GetErrorText(hr);
                    break;
            }

            return sRet;
        }

        /// <summary>
        /// If hr has a "failed" status code (E_*), throw an exception.  Note that status
        /// messages (S_*) are not considered failure codes.  If DES or DShow error text
        /// is available, it is used to build the exception, otherwise a generic com error
        /// is thrown.
        /// </summary>
        /// <param name="hr">The HRESULT to check</param>
        public static void ThrowExceptionForHR(int hr)
        {
            // If an error has occurred
            if (hr < 0)
            {
                // If a string is returned, build a com error from it
                string buf = GetErrorText(hr);

                if (buf != null)
                {
                    throw new COMException(buf, hr);
                }
                else
                {
                    // No string, just use standard com error
                    Marshal.ThrowExceptionForHR(hr);
                }
            }
        }
    }

    #endregion

    #region Classes

    /// <summary>
    /// From CLSID_AMMultiMediaStream
    /// </summary>
    [ComImport, Guid("49c47ce5-9ba4-11d0-8212-00c04fc32c45")]
    public class AMMultiMediaStream
    {
    }

    /// <summary>
    /// From CLSID_AMMediaTypeStream
    /// </summary>
    [ComImport, Guid("CF0F2F7C-F7BF-11d0-900D-00C04FD9189D")]
    public class AMMediaTypeStream
    {
    }

    /// <summary>
    /// From CLSID_AMDirectDrawStream
    /// </summary>
    [ComImport, Guid("49c47ce4-9ba4-11d0-8212-00c04fc32c45")]
    public class AMDirectDrawStream
    {
    }

    /// <summary>
    /// From CLSID_AMAudioStream
    /// </summary>
    [ComImport, Guid("8496e040-af4c-11d0-8212-00c04fc32c45")]
    public class AMAudioStream
    {
    }

    /// <summary>
    /// From CLSID_AMAudioData
    /// </summary>
    [ComImport, Guid("f2468580-af8a-11d0-8212-00c04fc32c45")]
    public class AMAudioData
    {
    }

    #endregion

    #region Declarations

    /// <summary>
    /// From COMPLETION_STATUS_FLAGS
    /// </summary>
    [Flags]
    public enum CompletionStatusFlags
    {
        None = 0x0,
        NoUpdateOk = 0x1,
        Wait = 0x2,
        Abort = 0x4
    }

    /// <summary>
    /// From unnamed enum
    /// </summary>
    [Flags]
    public enum SSUpdate
    {
        None = 0x0,
        ASync = 0x1,
        Continuous = 0x2
    }

    /// <summary>
    /// From STREAM_STATE
    /// </summary>
    public enum StreamState
    {
        // Fields
        Run = 1,
        Stop = 0
    }

    /// <summary>
    /// From STREAM_TYPE
    /// </summary>
    public enum StreamType
    {
        // Fields
        Read = 0,
        Transform = 2,
        Write = 1
    }

    /// <summary>
    /// From unnamed enum
    /// </summary>
    public enum MMSSF
    {
        HasClock = 0x1,
        SupportSeek = 0x2,
        Asynchronous = 0x4
    }

    #endregion

    #region GUIDS

    public sealed class MSPID
    {
        private MSPID()
        {
            // Prevent people from trying to instantiate this class
        }

        /// <summary> MSPID_PrimaryVideo </summary>
        public static readonly Guid PrimaryVideo = new Guid(0xa35ff56a, 0x9fda, 0x11d0, 0x8f, 0xdf, 0x0, 0xc0, 0x4f, 0xd9, 0x18, 0x9d);

        /// <summary> MSPID_PrimaryAudio </summary>
        public static readonly Guid PrimaryAudio = new Guid(0xa35ff56b, 0x9fda, 0x11d0, 0x8f, 0xdf, 0x0, 0xc0, 0x4f, 0xd9, 0x18, 0x9d);
    }

    #endregion

    #region Interfaces

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("B502D1BD-9A57-11D0-8FDE-00C04FD9189D")]
    public interface IMediaStream
    {
        [PreserveSig]
        int GetMultiMediaStream(
            [MarshalAs(UnmanagedType.Interface)] out IMultiMediaStream ppMultiMediaStream
            );

        [PreserveSig]
        int GetInformation(
            out Guid pPurposeId,
            out StreamType pType
            );

        [PreserveSig]
        int SetSameFormat(
            [In, MarshalAs(UnmanagedType.Interface)] IMediaStream pStreamThatHasDesiredFormat,
            [In] int dwFlags
            );

        [PreserveSig]
        int AllocateSample(
            [In] int dwFlags,
            [MarshalAs(UnmanagedType.Interface)] out IStreamSample ppSample
            );

        [PreserveSig]
        int CreateSharedSample(
            [In, MarshalAs(UnmanagedType.Interface)] IStreamSample pExistingSample,
            [In] int dwFlags,
            [MarshalAs(UnmanagedType.Interface)] out IStreamSample ppNewSample
            );

        [PreserveSig]
        int SendEndOfStream(
            int dwFlags
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("B502D1BC-9A57-11D0-8FDE-00C04FD9189D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMultiMediaStream
    {
        [PreserveSig]
        int GetInformation(
            out MMSSF pdwFlags,
            out StreamType pStreamType
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
        int GetState(
            out StreamState pCurrentState
            );

        [PreserveSig]
        int SetState(
            [In] StreamState NewState
            );

        [PreserveSig]
        int GetTime(
            out long pCurrentTime
            );

        [PreserveSig]
        int GetDuration(
            out long pDuration
            );

        [PreserveSig]
        int Seek(
            [In] long SeekTime
            );

        [PreserveSig]
        int GetEndOfStreamEventHandle(
            out IntPtr phEOS
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("B502D1BE-9A57-11D0-8FDE-00C04FD9189D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IStreamSample
    {
        [PreserveSig]
        int GetMediaStream(
            [MarshalAs(UnmanagedType.Interface)] out IMediaStream ppMediaStream
            );

        [PreserveSig]
        int GetSampleTimes(
            out long pStartTime,
            out long pEndTime,
            out long pCurrentTime
            );

        [PreserveSig]
        int SetSampleTimes(
            [In] DsLong pStartTime,
            [In] DsLong pEndTime
            );

        [PreserveSig]
        int Update(
            [In] SSUpdate dwFlags,
            [In] IntPtr hEvent,
            [In] IntPtr pfnAPC,
            [In] IntPtr dwAPCData
            );

        [PreserveSig]
        int CompletionStatus(
            [In] CompletionStatusFlags dwFlags,
            [In] int dwMilliseconds
            );
    }

    #endregion
}
