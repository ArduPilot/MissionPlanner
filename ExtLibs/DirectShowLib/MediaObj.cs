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
using System.Text;
using System.Runtime.InteropServices;
using System.Security;

namespace DirectShowLib.DMO
{
    #region Declarations

#if ALLOW_UNTESTED_INTERFACES

    /// <summary>
    /// From DMO_QUALITY_STATUS_FLAGS
    /// </summary>
    [Flags]
    public enum DMOQualityStatus
    {
        None = 0x0,
        Enabled = 0x1
    }

#endif

    /// <summary>
    /// From _DMO_OUTPUT_DATA_BUFFER_FLAGS
    /// </summary>
    [Flags]
    public enum DMOOutputDataBufferFlags
    {
        None = 0,
        SyncPoint = 0x1,
        Time = 0x2,
        TimeLength = 0x4,
        InComplete = 0x1000000
    } ;

    /// <summary>
    /// From DMO_ENUM_FLAGS
    /// </summary>
    [Flags]
    public enum DMOEnumerator
    {
        None = 0,
        IncludeKeyed = 0x00000001
    }

    /// <summary>
    /// From DMO_REGISTER_FLAGS
    /// </summary>
    [Flags]
    public enum DMORegisterFlags
    {
        None = 0,
        IsKeyed = 0x00000001
    };

    /// <summary>
    /// From DMO_PROCESS_OUTPUT_FLAGS
    /// </summary>
    [Flags]
    public enum DMOProcessOutput
    {
        None = 0x0,
        DiscardWhenNoBuffer = 0x00000001
    }

    /// <summary>
    /// From DMO_INPUT_DATA_BUFFER_FLAGS
    /// </summary>
    [Flags]
    public enum DMOInputDataBuffer
    {
        None = 0,
        SyncPoint = 0x1,
        Time = 0x2,
        TimeLength = 0x4
    }

    /// <summary>
    /// From DMO_INPLACE_PROCESS_FLAGS
    /// </summary>
    [Flags]
    public enum DMOInplaceProcess
    {
        Normal = 0,
        Zero = 0x1
    }

    /// <summary>
    /// From DMO_INPUT_STREAM_INFO_FLAGS
    /// </summary>
    [Flags]
    public enum DMOInputStreamInfo
    {
        None = 0x0,
        WholeSamples = 0x1,
        SingleSamplePerBuffer = 0x2,
        FixedSampleSize = 0x4,
        HoldsBuffers = 0x8
    }

    /// <summary>
    /// From DMO_OUTPUT_STREAM_INFO_FLAGS
    /// </summary>
    [Flags]
    public enum DMOOutputStreamInfo
    {
        None = 0x0,
        WholeSamples = 0x1,
        SingleSamplePerBuffer = 0x2,
        FixedSampleSize = 0x4,
        Discardable = 0x8,
        Optional = 0x10
    }

    /// <summary>
    /// From _DMO_SET_TYPE_FLAGS
    /// </summary>
    [Flags]
    public enum DMOSetType
    {
        None = 0x0,
        TestOnly = 0x1,
        Clear = 0x2
    }

    /// <summary>
    /// From DMO_INPUT_STATUS_FLAGS
    /// </summary>
    [Flags]
    public enum DMOInputStatusFlags
    {
        None = 0x0,
        AcceptData = 0x1
    }


    /// <summary>
    /// From DMO_VIDEO_OUTPUT_STREAM_FLAGS
    /// </summary>
    [Flags]
    public enum DMOVideoOutputStream
    {
        None = 0x0,
        NeedsPreviousSample = 0x1
    }

    /// <summary>
    /// From DMO_PARTIAL_MEDIATYPE
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DMOPartialMediatype
    {
        public Guid type;
        public Guid subtype;
    }

    /// <summary>
    /// From DMO_OUTPUT_DATA_BUFFER
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct DMOOutputDataBuffer
    {
        [MarshalAs(UnmanagedType.Interface)]
        public IMediaBuffer pBuffer;
        public DMOOutputDataBufferFlags dwStatus;
        public long rtTimestamp;
        public long rtTimelength;
    }

    #endregion

    #region GUIDS

    public sealed class DMOCategory
    {
        private DMOCategory()
        {
            // Prevent people from trying to instantiate this class
        }

        /// <summary> DMOCATEGORY_AUDIO_DECODER </summary>
        public static readonly Guid AudioDecoder = new Guid(0x57f2db8b, 0xe6bb, 0x4513, 0x9d, 0x43, 0xdc, 0xd2, 0xa6, 0x59, 0x31, 0x25);

        /// <summary> DMOCATEGORY_AUDIO_ENCODER </summary>
        public static readonly Guid AudioEncoder = new Guid(0x33D9A761, 0x90C8, 0x11d0, 0xBD, 0x43, 0x00, 0xA0, 0xC9, 0x11, 0xCE, 0x86);

        /// <summary> DMOCATEGORY_VIDEO_DECODER </summary>
        public static readonly Guid VideoDecoder = new Guid(0x4a69b442, 0x28be, 0x4991, 0x96, 0x9c, 0xb5, 0x00, 0xad, 0xf5, 0xd8, 0xa8);

        /// <summary> DMOCATEGORY_VIDEO_ENCODER </summary>
        public static readonly Guid VideoEncoder = new Guid(0x33D9A760, 0x90C8, 0x11d0, 0xBD, 0x43, 0x00, 0xA0, 0xC9, 0x11, 0xCE, 0x86);

        /// <summary> DMOCATEGORY_AUDIO_EFFECT </summary>
        public static readonly Guid AudioEffect = new Guid(0xf3602b3f, 0x0592, 0x48df, 0xa4, 0xcd, 0x67, 0x47, 0x21, 0xe7, 0xeb, 0xeb);

        /// <summary> DMOCATEGORY_VIDEO_EFFECT </summary>
        public static readonly Guid VideoEffect = new Guid(0xd990ee14, 0x776c, 0x4723, 0xbe, 0x46, 0x3d, 0xa2, 0xf5, 0x6f, 0x10, 0xb9);

        /// <summary> DMOCATEGORY_AUDIO_CAPTURE_EFFECT </summary>
        public static readonly Guid AudioCaptureEffect = new Guid(0xf665aaba, 0x3e09, 0x4920, 0xaa, 0x5f, 0x21, 0x98, 0x11, 0x14, 0x8f, 0x09);
    }

    #endregion

    #region API Declares

    public sealed class DMOUtils
    {
        [DllImport("msdmo.dll", ExactSpelling = true), SuppressUnmanagedCodeSecurity]
        public static extern int DMOEnum(
            [MarshalAs(UnmanagedType.LPStruct)] Guid DMOCategory,
            DMOEnumerator dwFlags,
            int cInTypes,
            [In] DMOPartialMediatype[] pInTypes,
            int cOutTypes,
            [In] DMOPartialMediatype[] pOutTypes,
            out IEnumDMO ppEnum
            );

        [DllImport("msdmo.dll", ExactSpelling = true), SuppressUnmanagedCodeSecurity]
        public static extern int MoInitMediaType(
            [Out] DirectShowLib.AMMediaType pmt,
            int i
            );

        [DllImport("msdmo.dll", ExactSpelling = true), SuppressUnmanagedCodeSecurity]
        public static extern int MoCopyMediaType(
            [Out, MarshalAs(UnmanagedType.LPStruct)] AMMediaType dst,
            [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType src
            );

        [DllImport("MSDmo.dll", CharSet = CharSet.Unicode, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
        static extern public int DMORegister(
            [MarshalAs(UnmanagedType.LPWStr)] string szName,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid clsidDMO,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid DMOCategory,
            DMORegisterFlags dwFlags,
            int cInTypes,
            [In] DMOPartialMediatype[] pInTypes,
            int cOutTypes,
            [In] DMOPartialMediatype[] pOutTypes
            );

        [DllImport("MSDmo.dll", ExactSpelling = true), SuppressUnmanagedCodeSecurity]
        static extern public int DMOUnregister(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid clsidDMO,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidCategory
            );

        [DllImport("MSDmo.dll", CharSet = CharSet.Unicode, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
        static extern public int DMOGetName(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid clsidDMO,
            [Out, MarshalAs(UnmanagedType.LPWStr, SizeConst = 80)] StringBuilder szName
            );

        [DllImport("MSDmo.dll", ExactSpelling = true), SuppressUnmanagedCodeSecurity]
        static extern public int DMOGetTypes(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid clsidDMO,
            int ulInputTypesRequested,
            out int pulInputTypesSupplied,
            [Out] DMOPartialMediatype[] pInTypes,
            int ulOutputTypesRequested,
            out int pulOutputTypesSupplied,
            [Out] DMOPartialMediatype[] pOutTypes
            );

        private DMOUtils()
        {
        }
    }

    #endregion

    #region Utility Classes

    public sealed class DMOResults
    {
        private DMOResults()
        {
            // Prevent people from trying to instantiate this class
        }

        public const int E_InvalidStreamIndex = unchecked((int)0x80040201);
        public const int E_InvalidType = unchecked((int)0x80040202);
        public const int E_TypeNotSet = unchecked((int)0x80040203);
        public const int E_NotAccepting = unchecked((int)0x80040204);
        public const int E_TypeNotAccepted = unchecked((int)0x80040205);
        public const int E_NoMoreItems = unchecked((int)0x80040206);
    }

    public sealed class DMOError
    {
        private DMOError()
        {
            // Prevent people from trying to instantiate this class
        }

        public static string GetErrorText(int hr)
        {
            string sRet = null;

            switch (hr)
            {
                case DMOResults.E_InvalidStreamIndex:
                    sRet = "Invalid stream index.";
                    break;
                case DMOResults.E_InvalidType:
                    sRet = "Invalid media type.";
                    break;
                case DMOResults.E_TypeNotSet:
                    sRet = "Media type was not set. One or more streams require a media type before this operation can be performed.";
                    break;
                case DMOResults.E_NotAccepting:
                    sRet = "Data cannot be accepted on this stream. You might need to process more output data; see IMediaObject::ProcessInput.";
                    break;
                case DMOResults.E_TypeNotAccepted:
                    sRet = "Media type was not accepted.";
                    break;
                case DMOResults.E_NoMoreItems:
                    sRet = "Media-type index is out of range.";
                    break;
                default:
                    sRet = DsError.GetErrorText(hr);
                    break;
            }

            return sRet;
        }

        /// <summary>
        /// If hr has a "failed" status code (E_*), throw an exception.  Note that status
        /// messages (S_*) are not considered failure codes.  If DMO or DShow error text
        /// is available, it is used to build the exception, otherwise a generic COM error
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

    #region Interfaces

#if ALLOW_UNTESTED_INTERFACES

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("65ABEA96-CF36-453F-AF8A-705E98F16260"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDMOQualityControl
    {
        [PreserveSig]
        int SetNow(
            [In] long rtNow
            );

        [PreserveSig]
        int SetStatus(
            [In] DMOQualityStatus dwFlags
            );

        [PreserveSig]
        int GetStatus(
            out DMOQualityStatus pdwFlags
            );
    }

#endif

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("2C3CD98A-2BFA-4A53-9C27-5249BA64BA0F")]
    public interface IEnumDMO
    {
        [PreserveSig]
        int Next(
            int cItemsToFetch,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] Guid[] pCLSID,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0, ArraySubType = UnmanagedType.LPWStr)] string[] Names,
            [In] IntPtr pcItemsFetched
            );

        [PreserveSig]
        int Skip(
            int cItemsToSkip
            );

        [PreserveSig]
        int Reset();

        [PreserveSig]
        int Clone(
            [MarshalAs(UnmanagedType.Interface)] out IEnumDMO ppEnum
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("651B9AD0-0FC7-4AA9-9538-D89931010741")]
    public interface IMediaObjectInPlace
    {
        [PreserveSig]
        int Process(
            [In] int ulSize,
            [In] IntPtr pData,
            [In] long refTimeStart,
            [In] DMOInplaceProcess dwFlags
            );

        [PreserveSig]
        int Clone(
            [MarshalAs(UnmanagedType.Interface)] out IMediaObjectInPlace ppMediaObject
            );

        [PreserveSig]
        int GetLatency(
            out long pLatencyTime
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("59EFF8B9-938C-4A26-82F2-95CB84CDC837"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMediaBuffer
    {
        [PreserveSig]
        int SetLength(
            int cbLength
            );

        [PreserveSig]
        int GetMaxLength(
            out int pcbMaxLength
            );

        [PreserveSig]
        int GetBufferAndLength(
            out IntPtr ppBuffer,
            out int pcbLength
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("D8AD0F58-5494-4102-97C5-EC798E59BCF4"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMediaObject
    {
        [PreserveSig]
        int GetStreamCount(
            out int pcInputStreams,
            out int pcOutputStreams
            );

        [PreserveSig]
        int GetInputStreamInfo(
            int dwInputStreamIndex,
            out DMOInputStreamInfo pdwFlags
            );

        [PreserveSig]
        int GetOutputStreamInfo(
            int dwOutputStreamIndex,
            out DMOOutputStreamInfo pdwFlags
            );

        [PreserveSig]
        int GetInputType(
            int dwInputStreamIndex,
            int dwTypeIndex,
            [Out] AMMediaType pmt
            );

        [PreserveSig]
        int GetOutputType(
            int dwOutputStreamIndex,
            int dwTypeIndex,
            [Out] AMMediaType pmt
            );

        [PreserveSig]
        int SetInputType(
            int dwInputStreamIndex,
            [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt,
            DMOSetType dwFlags
            );

        [PreserveSig]
        int SetOutputType(
            int dwOutputStreamIndex,
            [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt,
            DMOSetType dwFlags
            );

        [PreserveSig]
        int GetInputCurrentType(
            int dwInputStreamIndex,
            [Out] AMMediaType pmt
            );

        [PreserveSig]
        int GetOutputCurrentType(
            int dwOutputStreamIndex,
            [Out] AMMediaType pmt
            );

        [PreserveSig]
        int GetInputSizeInfo(
            int dwInputStreamIndex,
            out int pcbSize,
            out int pcbMaxLookahead,
            out int pcbAlignment
            );

        [PreserveSig]
        int GetOutputSizeInfo(
            int dwOutputStreamIndex,
            out int pcbSize,
            out int pcbAlignment
            );

        [PreserveSig]
        int GetInputMaxLatency(
            int dwInputStreamIndex,
            out long prtMaxLatency
            );

        [PreserveSig]
        int SetInputMaxLatency(
            int dwInputStreamIndex,
            long rtMaxLatency
            );

        [PreserveSig]
        int Flush();

        [PreserveSig]
        int Discontinuity(
            int dwInputStreamIndex
            );

        [PreserveSig]
        int AllocateStreamingResources();

        [PreserveSig]
        int FreeStreamingResources();

        [PreserveSig]
        int GetInputStatus(
            int dwInputStreamIndex,
            out DMOInputStatusFlags dwFlags
            );

        [PreserveSig]
        int ProcessInput(
            int dwInputStreamIndex,
            IMediaBuffer pBuffer,
            DMOInputDataBuffer dwFlags,
            long rtTimestamp,
            long rtTimelength
            );

        [PreserveSig]
        int ProcessOutput(
            DMOProcessOutput dwFlags,
            int cOutputBufferCount,
            [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] DMOOutputDataBuffer[] pOutputBuffers,
            out int pdwStatus
            );

        [PreserveSig]
        int Lock(
            [MarshalAs(UnmanagedType.Bool)] bool bLock
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("BE8F4F4E-5B16-4D29-B350-7F6B5D9298AC"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDMOVideoOutputOptimizations
    {
        [PreserveSig]
        int QueryOperationModePreferences(
            int ulOutputStreamIndex,
            out DMOVideoOutputStream pdwRequestedCapabilities
            );

        [PreserveSig]
        int SetOperationMode(
            int ulOutputStreamIndex,
            DMOVideoOutputStream dwEnabledFeatures
            );

        [PreserveSig]
        int GetCurrentOperationMode(
            int ulOutputStreamIndex,
            out DMOVideoOutputStream pdwEnabledFeatures
            );

        [PreserveSig]
        int GetCurrentSampleRequirements(
            int ulOutputStreamIndex,
            out DMOVideoOutputStream pdwRequestedFeatures
            );
    }

    #endregion
}
