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

using DirectShowLib;
using System.Text;

namespace DirectShowLib.DES
{
    #region Utility Classes

    public sealed class DESResults
    {
        private DESResults()
        {
            // Prevent people from trying to instantiate this class
        }

        public const int E_NotInTree = unchecked((int)0x80040400);
        public const int E_RenderEngineIsBroken = unchecked((int)0x80040401);
        public const int E_MustInitRenderer = unchecked((int)0x80040402);
        public const int E_NotDetermind = unchecked((int)0x80040403);
        public const int E_NoTimeline = unchecked((int)0x80040404);
        public const int S_WarnOutputReset = unchecked((int)40404);
    }


    public sealed class DESError
    {
        private DESError()
        {
            // Prevent people from trying to instantiate this class
        }

        public static string GetErrorText(int hr)
        {
            string sRet = null;

            switch(hr)
            {
                case DESResults.E_NotInTree:
                    sRet = "The object is not contained in the timeline.";
                    break;
                case DESResults.E_RenderEngineIsBroken:
                    sRet = "Operation failed because project was not rendered successfully.";
                    break;
                case DESResults.E_MustInitRenderer:
                    sRet = "Render engine has not been initialized.";
                    break;
                case DESResults.E_NotDetermind:
                    sRet = "Cannot determine requested value.";
                    break;
                case DESResults.E_NoTimeline:
                    sRet = "There is no timeline object.";
                    break;
                case DESResults.S_WarnOutputReset:
                    sRet = "The rendering portion of the graph was deleted. The application must rebuild it.";
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
    /// From CLSID_AMTimeline
    /// </summary>
    [ComImport, Guid("78530B75-61F9-11D2-8CAD-00A024580902")]
    public class AMTimeline
    {
    }

    /// <summary>
    /// From CLSID_PropertySetter
    /// </summary>
    [ComImport, Guid("ADF95821-DED7-11d2-ACBE-0080C75E246E")]
    public class PropertySetter
    {
    }

    /// <summary>
    /// From CLSID_AMTimelineObj
    /// </summary>
    [ComImport, Guid("78530B78-61F9-11D2-8CAD-00A024580902")]
    public class AMTimelineObj
    {
    }

    /// <summary>
    /// From CLSID_AMTimelineSrc
    /// </summary>
    [ComImport, Guid("78530B7A-61F9-11D2-8CAD-00A024580902")]
    public class AMTimelineSrc
    {
    }

    /// <summary>
    /// From CLSID_AMTimelineTrack
    /// </summary>
    [ComImport, Guid("8F6C3C50-897B-11d2-8CFB-00A0C9441E20")]
    public class AMTimelineTrack
    {
    }

    /// <summary>
    /// From CLSID_AMTimelineComp
    /// </summary>
    [ComImport, Guid("74D2EC80-6233-11d2-8CAD-00A024580902")]
    public class AMTimelineComp
    {
    }

    /// <summary>
    /// From CLSID_AMTimelineGroup
    /// </summary>
    [ComImport, Guid("F6D371E1-B8A6-11d2-8023-00C0DF10D434")]
    public class AMTimelineGroup
    {
    }

    /// <summary>
    /// From CLSID_AMTimelineTrans
    /// </summary>
    [ComImport, Guid("74D2EC81-6233-11d2-8CAD-00A024580902")]
    public class AMTimelineTrans
    {
    }

    /// <summary>
    /// From CLSID_AMTimelineEffect
    /// </summary>
    [ComImport, Guid("74D2EC82-6233-11d2-8CAD-00A024580902")]
    public class AMTimelineEffect
    {
    }

    /// <summary>
    /// From CLSID_RenderEngine
    /// </summary>
    [ComImport, Guid("64D8A8E0-80A2-11d2-8CF3-00A0C9441E20")]
    public class RenderEngine
    {
    }

    /// <summary>
    /// From CLSID_SmartRenderEngine
    /// </summary>
    [ComImport, Guid("498B0949-BBE9-4072-98BE-6CCAEB79DC6F")]
    public class SmartRenderEngine
    {
    }

    /// <summary>
    /// From CLSID_AudMixer
    /// </summary>
    [ComImport, Guid("036A9790-C153-11d2-9EF7-006008039E37")]
    public class AudMixer
    {
    }

    /// <summary>
    /// From CLSID_Xml2Dex
    /// </summary>
    [ComImport, Guid("18C628EE-962A-11D2-8D08-00A0C9441E20")]
    public class Xml2Dex
    {
    }

    /// <summary>
    /// From CLSID_MediaLocator
    /// </summary>
    [ComImport, Guid("CC1101F2-79DC-11D2-8CE6-00A0C9441E20")]
    public class MediaLocator
    {
    }

    /// <summary>
    /// From CLSID_MediaDet
    /// </summary>
    [ComImport, Guid("65BD0711-24D2-4ff7-9324-ED2E5D3ABAFA")]
    public class MediaDet
    {
    }

    /// <summary>
    /// From CLSID_DxtCompositor
    /// </summary>
    [ComImport, Guid("BB44391D-6ABD-422f-9E2E-385C9DFF51FC")]
    public class DxtCompositor
    {
    }

    /// <summary>
    /// From CLSID_DxtAlphaSetter
    /// </summary>
    [ComImport, Guid("506D89AE-909A-44f7-9444-ABD575896E35")]
    public class DxtAlphaSetter
    {
    }

    /// <summary>
    /// From CLSID_DxtJpeg
    /// </summary>
    [ComImport, Guid("DE75D012-7A65-11D2-8CEA-00A0C9441E20")]
    public class DxtJpeg
    {
    }

    /// <summary>
    /// From CLSID_ColorSource
    /// </summary>
    [ComImport, Guid("0cfdd070-581a-11d2-9ee6-006008039e37")]
    public class ColorSource
    {
    }

    /// <summary>
    /// From CLSID_DxtKey
    /// </summary>
    [ComImport, Guid("C5B19592-145E-11d3-9F04-006008039E37")]
    public class DxtKey
    {
    }

    #endregion

    #region Declarations

#if ALLOW_UNTESTED_INTERFACES

    /// <summary>
    /// From unnamed enum
    /// </summary>
    public enum DXTKeys
    {
        RGB,
        NonRed,
        Luminance,
        Alpha,
        Hue
    }


#endif

    /// <summary>
    /// From TIMELINE_MAJOR_TYPE
    /// </summary>
    [Flags]
    public enum TimelineMajorType
    {
        None = 0,
        Composite = 1,
        Effect = 0x10,
        Group = 0x80,
        Source = 4,
        Track= 2,
        Transition = 8
    }


    /// <summary>
    /// From unnamed enum
    /// </summary>
    public enum TimelineInsertMode
    {
        Insert = 1,
        Overlay = 2
    }


    /// <summary>
    /// From unnamed enum
    /// </summary>
    [Flags]
    public enum SFNValidateFlags
    {
        None         = 0x00000000,
        Check        = 0x00000001,
        Popup        = 0x00000002,
        TellMe       = 0x00000004,
        Replace      = 0x00000008,
        UseLocal     = 0x000000010,
        NoFind       = 0x000000020,
        IgnoreMuted  = 0x000000040,
        End
    }


    /// <summary>
    /// From SCompFmt0
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class SCompFmt0
    {
        public int nFormatId;
        public AMMediaType MediaType;
    }


    /// <summary>
    /// From unnamed enum
    /// </summary>
    public enum ResizeFlags
    {
        Stretch,
        Crop,
        PreserveAspectRatio,
        PreserveAspectRatioNoLetterBox
    }


    /// <summary>
    /// From DEXTERF_TRACK_SEARCH_FLAGS
    /// </summary>
    public enum DexterFTrackSearchFlags
    {
        Bounding = -1,
        ExactlyAt = 0,
        Forwards = 1
    }


    /// <summary>
    /// From DEXTER_PARAM
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack=4)]
    public struct DexterParam
    {
        [MarshalAs(UnmanagedType.BStr)]  public string Name;
        public int dispID;
        public int nValues;
    }


    /// <summary>
    /// From unnamed enum
    /// </summary>
    public enum ConnectFDynamic
    {
        None = 0x00000000,
        Sources = 0x00000001,
        Effects = 0x00000002
    }


    /// <summary>
    /// From DEXTER_VALUE
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack=8)]
    public struct DexterValue
    {
        [MarshalAs(UnmanagedType.Struct)] public object v;
        public long rt;
        public Dexterf dwInterp;
    }


    /// <summary>
    /// From DEXTERF
    /// </summary>
    public enum Dexterf
    {
        Jump,
        Interpolate
    }


    /// <summary>
    /// From DEX_IDS_* defines
    /// </summary>
    public enum DESErrorCode
    {
        BadSourceName = 1400,
        BadSourceName2 = 1401,
        MissingSourceName = 1402,
        UnknownSource = 1403,
        InstallProblem = 1404,
        NoSourceNames = 1405,
        BadMediaType = 1406,
        StreamNumber = 1407,
        OutOfMemory = 1408,
        DIBSeqNotAllSame = 1409,
        ClipTooShort = 1410,
        InvalidDXT = 1411,
        InvalidDefaultDXTT = 1412,
        No3D = 1413,
        BrokenDXT = 1414,
        NoSuchProperty = 1415,
        IllegalPropertyVal = 1416,
        InvalidXML = 1417,
        CantFindFilter = 1418,
        DiskWriteError = 1419,
        InvalidAudioFX = 1420,
        CantFindCompressor = 1421,
        TimelineParse = 1426,
        GraphError = 1427,
        GridError = 1428,
        InterfaceError = 1429
    }

    #endregion

    #region Interfaces

#if ALLOW_UNTESTED_INTERFACES

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("E31FB81B-1335-11D1-8189-0000F87557DB")]
    public interface IDXEffect
    {
        [PreserveSig]
        int get_Capabilities(
            out int pVal
            );

        [PreserveSig]
        int get_Progress(
            out float pVal
            );

        [PreserveSig]
        int put_Progress(
            float newVal
            );

        [PreserveSig]
        int get_StepResolution(
            out float pVal
            );

        [PreserveSig]
        int get_Duration(
            out float pVal
            );

        [PreserveSig]
        int put_Duration(
            float newVal
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("4EE9EAD9-DA4D-43D0-9383-06B90C08B12B"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDxtAlphaSetter : IDXEffect
    {
        #region IDXEffect Methods

        [PreserveSig]
        new int get_Capabilities(
            out int pVal
            );

        [PreserveSig]
        new int get_Progress(
            out float pVal
            );

        [PreserveSig]
        new int put_Progress(
            float newVal
            );

        [PreserveSig]
        new int get_StepResolution(
            out float pVal
            );

        [PreserveSig]
        new int get_Duration(
            out float pVal
            );

        [PreserveSig]
        new int put_Duration(
            float newVal
            );

        #endregion

        [PreserveSig]
        int get_Alpha(
            out int pVal
            );

        [PreserveSig]
        int put_Alpha(
            int newVal
            );

        [PreserveSig]
        int get_AlphaRamp(
            out double pVal
            );

        [PreserveSig]
        int put_AlphaRamp(
            double newVal
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("BB44391E-6ABD-422F-9E2E-385C9DFF51FC"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDxtCompositor : IDXEffect
    {
        #region IDXEffect

        [PreserveSig]
        new int get_Capabilities(
            out int pVal
            );

        [PreserveSig]
        new int get_Progress(
            out float pVal
            );

        [PreserveSig]
        new int put_Progress(
            float newVal
            );

        [PreserveSig]
        new int get_StepResolution(
            out float pVal
            );

        [PreserveSig]
        new int get_Duration(
            out float pVal
            );

        [PreserveSig]
        new int put_Duration(
            float newVal
            );

        #endregion

        [PreserveSig]
        int get_OffsetX(
            out int pVal
            );

        [PreserveSig]
        int put_OffsetX(
            int newVal
            );

        [PreserveSig]
        int get_OffsetY(
            out int pVal
            );

        [PreserveSig]
        int put_OffsetY(
            int newVal
            );

        [PreserveSig]
        int get_Width(
            out int pVal
            );

        [PreserveSig]
        int put_Width(
            int newVal
            );

        [PreserveSig]
        int get_Height(
            out int pVal
            );

        [PreserveSig]
        int put_Height(
            int newVal
            );

        [PreserveSig]
        int get_SrcOffsetX(
            out int pVal
            );

        [PreserveSig]
        int put_SrcOffsetX(
            int newVal
            );

        [PreserveSig]
        int get_SrcOffsetY(
            out int pVal
            );

        [PreserveSig]
        int put_SrcOffsetY(
            int newVal
            );

        [PreserveSig]
        int get_SrcWidth(
            out int pVal
            );

        [PreserveSig]
        int put_SrcWidth(
            int newVal
            );

        [PreserveSig]
        int get_SrcHeight(
            out int pVal
            );

        [PreserveSig]
        int put_SrcHeight(
            int newVal
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("DE75D011-7A65-11D2-8CEA-00A0C9441E20"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDxtJpeg : IDXEffect
    {
        #region IDXEffect

        [PreserveSig]
        new int get_Capabilities(
            out int pVal
            );

        [PreserveSig]
        new int get_Progress(
            out float pVal
            );

        [PreserveSig]
        new int put_Progress(
            float newVal
            );

        [PreserveSig]
        new int get_StepResolution(
            out float pVal
            );

        [PreserveSig]
        new int get_Duration(
            out float pVal
            );

        [PreserveSig]
        new int put_Duration(
            float newVal
            );

        #endregion

        [PreserveSig]
        int get_MaskNum(
            out int MIDL_0018
            );

        [PreserveSig]
        int put_MaskNum(
            int MIDL_0019
            );

        [PreserveSig]
        int get_MaskName(
            [MarshalAs(UnmanagedType.BStr)] out string pVal
            );

        [PreserveSig]
        int put_MaskName(
            [MarshalAs(UnmanagedType.BStr)] string newVal
            );

        [PreserveSig]
        int get_ScaleX(
            out double MIDL_0020
            );

        [PreserveSig]
        int put_ScaleX(
            double MIDL_0021
            );

        [PreserveSig]
        int get_ScaleY(
            out double MIDL_0022
            );

        [PreserveSig]
        int put_ScaleY(
            double MIDL_0023
            );

        [PreserveSig]
        int get_OffsetX(
            out int MIDL_0024
            );

        [PreserveSig]
        int put_OffsetX(
            int MIDL_0025
            );

        [PreserveSig]
        int get_OffsetY(
            out int MIDL_0026
            );

        [PreserveSig]
        int put_OffsetY(
            int MIDL_0027
            );

        [PreserveSig]
        int get_ReplicateX(
            out int pVal
            );

        [PreserveSig]
        int put_ReplicateX(
            int newVal
            );

        [PreserveSig]
        int get_ReplicateY(
            out int pVal
            );

        [PreserveSig]
        int put_ReplicateY(
            int newVal
            );

        [PreserveSig]
        int get_BorderColor(
            out int pVal
            );

        [PreserveSig]
        int put_BorderColor(
            int newVal
            );

        [PreserveSig]
        int get_BorderWidth(
            out int pVal
            );

        [PreserveSig]
        int put_BorderWidth(
            int newVal
            );

        [PreserveSig]
        int get_BorderSoftness(
            out int pVal
            );

        [PreserveSig]
        int put_BorderSoftness(
            int newVal
            );

        [PreserveSig]
        int ApplyChanges();

        [PreserveSig]
        int LoadDefSettings();
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("3255DE56-38FB-4901-B980-94B438010D7B"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDxtKey : IDXEffect
    {
        #region IDXEffect

        [PreserveSig]
        new int get_Capabilities(
            out int pVal
            );

        [PreserveSig]
        new int get_Progress(
            out float pVal
            );

        [PreserveSig]
        new int put_Progress(
            float newVal
            );

        [PreserveSig]
        new int get_StepResolution(
            out float pVal
            );

        [PreserveSig]
        new int get_Duration(
            out float pVal
            );

        [PreserveSig]
        new int put_Duration(
            float newVal
            );

        #endregion

        [PreserveSig]
        int get_KeyType(
            out int MIDL_0028
            );

        [PreserveSig]
        int put_KeyType(
            int MIDL_0029
            );

        [PreserveSig]
        int get_Hue(
            out int MIDL_0030
            );

        [PreserveSig]
        int put_Hue(
            int MIDL_0031
            );

        [PreserveSig]
        int get_Luminance(
            out int MIDL_0032
            );

        [PreserveSig]
        int put_Luminance(
            int MIDL_0033
            );

        [PreserveSig]
        int get_RGB(
            out int MIDL_0034
            );

        [PreserveSig]
        int put_RGB(
            int MIDL_0035
            );

        [PreserveSig]
        int get_Similarity(
            out int MIDL_0036
            );

        [PreserveSig]
        int put_Similarity(
            int MIDL_0037
            );

        [PreserveSig]
        int get_Invert(
            [MarshalAs(UnmanagedType.Bool)] out bool MIDL_0038
            );

        [PreserveSig]
        int put_Invert(
            [MarshalAs(UnmanagedType.Bool)] bool MIDL_0039
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("F03FA8DE-879A-4D59-9B2C-26BB1CF83461")]
    public interface IFindCompressorCB
    {
        [PreserveSig]
        int GetCompressor(
            [MarshalAs(UnmanagedType.LPStruct)] AMMediaType pType,
            [MarshalAs(UnmanagedType.LPStruct)] AMMediaType pCompType,
            out IBaseFilter ppFilter
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsDual),
    Guid("AE9472BE-B0C3-11D2-8D24-00A0C9441E20")]
    public interface IGrfCache
    {
        [PreserveSig]
        int AddFilter(IGrfCache ChainedCache, long Id, IBaseFilter pFilter, [MarshalAs(UnmanagedType.LPWStr)] string pName);

        [PreserveSig]
        int ConnectPins(IGrfCache ChainedCache, long PinID1, IPin pPin1, long PinID2, IPin pPin2);

        [PreserveSig]
        int SetGraph(IGraphBuilder pGraph);

        [PreserveSig]
        int DoConnectionsNow();
    }

#endif

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("E43E73A2-0EFA-11D3-9601-00A0C9441E20")]
    public interface IAMErrorLog
    {
        [PreserveSig]
        int LogError(
            int Severity,
            [MarshalAs(UnmanagedType.BStr)] string pErrorString,
            int ErrorCode,
            int hresult,
            [In] IntPtr pExtraInfo
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("963566DA-BE21-4EAF-88E9-35704F8F52A1")]
    public interface IAMSetErrorLog
    {
        [PreserveSig]
        int get_ErrorLog(
            out IAMErrorLog pVal
            );

        [PreserveSig]
        int put_ErrorLog(
            IAMErrorLog newVal
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("78530B74-61F9-11D2-8CAD-00A024580902")]
    public interface IAMTimeline
    {
        [PreserveSig]
        int CreateEmptyNode(
            out IAMTimelineObj ppObj,
            TimelineMajorType Type
            );

        [PreserveSig]
        int AddGroup(
            IAMTimelineObj pGroup
            );

        [PreserveSig]
        int RemGroupFromList(
            IAMTimelineObj pGroup
            );

        [PreserveSig]
        int GetGroup(
            out IAMTimelineObj ppGroup,
            int WhichGroup
            );

        [PreserveSig]
        int GetGroupCount(
            out int pCount
            );

        [PreserveSig]
        int ClearAllGroups();

        [PreserveSig]
        int GetInsertMode(
            out TimelineInsertMode pMode
            );

        [PreserveSig]
        int SetInsertMode(
            TimelineInsertMode Mode
            );

        [PreserveSig]
        int EnableTransitions(
            [MarshalAs(UnmanagedType.Bool)] bool fEnabled
            );

        [PreserveSig]
        int TransitionsEnabled(
            [MarshalAs(UnmanagedType.Bool)] out bool pfEnabled
            );

        [PreserveSig]
        int EnableEffects(
            [MarshalAs(UnmanagedType.Bool)] bool fEnabled
            );

        [PreserveSig]
        int EffectsEnabled(
            [MarshalAs(UnmanagedType.Bool)] out bool pfEnabled
            );

        [PreserveSig]
        int SetInterestRange(
            long Start,
            long Stop
            );

        [PreserveSig]
        int GetDuration(
            out long pDuration
            );

        [PreserveSig]
        int GetDuration2(
            out double pDuration
            );

        [PreserveSig]
        int SetDefaultFPS(
            double FPS
            );

        [PreserveSig]
        int GetDefaultFPS(
            out double pFPS
            );

        [PreserveSig]
        int IsDirty(
            [MarshalAs(UnmanagedType.Bool)] out bool pDirty
            );

        [PreserveSig]
        int GetDirtyRange(
            out long pStart,
            out long pStop
            );

        [PreserveSig]
        int GetCountOfType(
            int Group,
            out int pVal,
            out int pValWithComps,
            TimelineMajorType majortype
            );

        [PreserveSig]
        int ValidateSourceNames(
            SFNValidateFlags ValidateFlags,
            IMediaLocator pOverride,
            IntPtr NotifyEventHandle
            );

        [PreserveSig]
        int SetDefaultTransition(
            [MarshalAs(UnmanagedType.LPStruct)] Guid pGuid
            );

        [PreserveSig]
        int GetDefaultTransition(
            out Guid pGuid
            );

        [PreserveSig]
        int SetDefaultEffect(
            [MarshalAs(UnmanagedType.LPStruct)] Guid pGuid
            );

        [PreserveSig]
        int GetDefaultEffect(
            out Guid pGuid
            );

        [PreserveSig]
        int SetDefaultTransitionB(
            [MarshalAs(UnmanagedType.BStr)] string pGuid
            );

        [PreserveSig]
        int GetDefaultTransitionB(
            [Out, MarshalAs(UnmanagedType.BStr)] out string sGuid
            );

        [PreserveSig]
        int SetDefaultEffectB(
            [MarshalAs(UnmanagedType.BStr)] string pGuid
            );

        [PreserveSig]
        int GetDefaultEffectB(
            [Out, MarshalAs(UnmanagedType.BStr)] out string sGuid
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("EAE58536-622E-11D2-8CAD-00A024580902")]
    public interface IAMTimelineComp
    {
        [PreserveSig]
        int VTrackInsBefore(
            IAMTimelineObj pVirtualTrack,
            int priority
            );

        [PreserveSig]
        int VTrackSwapPriorities(
            int VirtualTrackA,
            int VirtualTrackB
            );

        [PreserveSig]
        int VTrackGetCount(
            out int pVal
            );

        [PreserveSig]
        int GetVTrack(
            out IAMTimelineObj ppVirtualTrack,
            int Which
            );

        [PreserveSig]
        int GetCountOfType(
            out int pVal,
            out int pValWithComps,
            TimelineMajorType majortype
            );

        [PreserveSig]
        int GetRecursiveLayerOfType(
            out IAMTimelineObj ppVirtualTrack,
            int WhichLayer,
            TimelineMajorType Type
            );

        [PreserveSig]
        int GetRecursiveLayerOfTypeI(
            out IAMTimelineObj ppVirtualTrack,
            [In, Out] ref int pWhichLayer,
            TimelineMajorType Type
            );

        [PreserveSig]
        int GetNextVTrack(
            IAMTimelineObj pVirtualTrack,
            out IAMTimelineObj ppNextVirtualTrack
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("BCE0C264-622D-11D2-8CAD-00A024580902")]
    public interface IAMTimelineEffect
    {
        [PreserveSig]
        int EffectGetPriority(out int pVal);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("EAE58537-622E-11D2-8CAD-00A024580902"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMTimelineEffectable
    {
        [PreserveSig]
        int EffectInsBefore(
            IAMTimelineObj pFX,
            int priority
            );

        [PreserveSig]
        int EffectSwapPriorities(
            int PriorityA,
            int PriorityB
            );

        [PreserveSig]
        int EffectGetCount(
            out int pCount
            );

        [PreserveSig]
        int GetEffect(
            out IAMTimelineObj ppFx,
            int Which
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("9EED4F00-B8A6-11D2-8023-00C0DF10D434"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMTimelineGroup
    {
        [PreserveSig]
        int SetTimeline(
            IAMTimeline pTimeline
            );

        [PreserveSig]
        int GetTimeline(
            out IAMTimeline ppTimeline
            );

        [PreserveSig]
        int GetPriority(
            out int pPriority
            );

        [PreserveSig]
        int GetMediaType(
            [Out, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt
            );

        [PreserveSig]
        int SetMediaType(
            [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt
            );

        [PreserveSig]
        int SetOutputFPS(
            double FPS
            );

        [PreserveSig]
        int GetOutputFPS(
            out double pFPS
            );

        [PreserveSig]
        int SetGroupName(
            [MarshalAs(UnmanagedType.BStr)] string pGroupName
            );

        [PreserveSig]
        int GetGroupName(
            [MarshalAs(UnmanagedType.BStr)] out string pGroupName
            );

        [PreserveSig]
        int SetPreviewMode(
            [MarshalAs(UnmanagedType.Bool)] bool fPreview
            );

        [PreserveSig]
        int GetPreviewMode(
            [MarshalAs(UnmanagedType.Bool)] out bool pfPreview
            );

        [PreserveSig]
        int SetMediaTypeForVB(
            [In] int Val
            );

        [PreserveSig]
        int GetOutputBuffering(
            out int pnBuffer
            );

        [PreserveSig]
        int SetOutputBuffering(
            [In] int nBuffer
            );

        [PreserveSig]
        int SetSmartRecompressFormat(
            SCompFmt0 pFormat
            );

        [PreserveSig]
        int GetSmartRecompressFormat(
            out SCompFmt0 ppFormat
            );

        [PreserveSig]
        int IsSmartRecompressFormatSet(
            [MarshalAs(UnmanagedType.Bool)] out bool pVal
            );

        [PreserveSig]
        int IsRecompressFormatDirty(
            [MarshalAs(UnmanagedType.Bool)] out bool pVal
            );

        [PreserveSig]
        int ClearRecompressFormatDirty();

        [PreserveSig]
        int SetRecompFormatFromSource(
            IAMTimelineSrc pSource
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("78530B77-61F9-11D2-8CAD-00A024580902"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMTimelineObj
    {
        [PreserveSig]
        int GetStartStop(
            out long pStart,
            out long pStop
            );

        [PreserveSig]
        int GetStartStop2(
            out double pStart,
            out double pStop
            );

        [PreserveSig]
        int FixTimes(
            ref long pStart,
            ref long pStop
            );

        [PreserveSig]
        int FixTimes2(
            ref double pStart,
            ref double pStop
            );

        [PreserveSig]
        int SetStartStop(
            long Start,
            long Stop
            );

        [PreserveSig]
        int SetStartStop2(
            double Start,
            double Stop
            );

        [PreserveSig]
        int GetPropertySetter(
            out IPropertySetter pVal
            );

        [PreserveSig]
        int SetPropertySetter(
            IPropertySetter newVal
            );

        [PreserveSig]
        int GetSubObject(
            [MarshalAs(UnmanagedType.IUnknown)] out object pVal
            );

        [PreserveSig]
        int SetSubObject(
            [In, MarshalAs(UnmanagedType.IUnknown)] object newVal
            );

        [PreserveSig]
        int SetSubObjectGUID(
            Guid newVal
            );

        [PreserveSig]
        int SetSubObjectGUIDB(
            [MarshalAs(UnmanagedType.BStr)] string newVal
            );

        [PreserveSig]
        int GetSubObjectGUID(
            out Guid pVal
            );

        [PreserveSig]
        int GetSubObjectGUIDB(
            [MarshalAs(UnmanagedType.BStr)] out string pVal
            );

        [PreserveSig]
        int GetSubObjectLoaded(
            [MarshalAs(UnmanagedType.Bool)] out bool pVal
            );

        [PreserveSig]
        int GetTimelineType(
            out TimelineMajorType pVal
            );

        [PreserveSig]
        int SetTimelineType(
            TimelineMajorType newVal
            );

        [PreserveSig]
        int GetUserID(
            out int pVal
            );

        [PreserveSig]
        int SetUserID(
            int newVal
            );

        [PreserveSig]
        int GetGenID(
            out int pVal
            );

        [PreserveSig]
        int GetUserName(
            [MarshalAs(UnmanagedType.BStr)] out string pVal
            );

        [PreserveSig]
        int SetUserName(
            [MarshalAs(UnmanagedType.BStr)] string newVal
            );

        [PreserveSig]
        int GetUserData(
            IntPtr pData,
            out int pSize
            );

        [PreserveSig]
        int SetUserData(
            IntPtr pData,
            int Size
            );

        [PreserveSig]
        int GetMuted(
            [MarshalAs(UnmanagedType.Bool)] out bool pVal
            );

        [PreserveSig]
        int SetMuted(
            [MarshalAs(UnmanagedType.Bool)] bool newVal
            );

        [PreserveSig]
        int GetLocked(
            [MarshalAs(UnmanagedType.Bool)] out bool pVal
            );

        [PreserveSig]
        int SetLocked(
            [MarshalAs(UnmanagedType.Bool)] bool newVal
            );

        [PreserveSig]
        int GetDirtyRange(
            out long pStart,
            out long pStop
            );

        [PreserveSig]
        int GetDirtyRange2(
            out double pStart,
            out double pStop
            );

        [PreserveSig]
        int SetDirtyRange(
            long Start,
            long Stop
            );

        [PreserveSig]
        int SetDirtyRange2(
            double Start,
            double Stop
            );

        [PreserveSig]
        int ClearDirty();

        [PreserveSig]
        int Remove();

        [PreserveSig]
        int RemoveAll();

        [PreserveSig]
        int GetTimelineNoRef(
            out IAMTimeline ppResult
            );

        [PreserveSig]
        int GetGroupIBelongTo(
            out IAMTimelineGroup ppGroup
            );

        [PreserveSig]
        int GetEmbedDepth(
            out int pVal
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("A0F840A0-D590-11D2-8D55-00A0C9441E20")]
    public interface IAMTimelineSplittable
    {
        [PreserveSig]
        int SplitAt(
            long Time
            );

        [PreserveSig]
        int SplitAt2(
            double Time
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("78530B79-61F9-11D2-8CAD-00A024580902"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMTimelineSrc
    {
        [PreserveSig]
        int GetMediaTimes(
            out long pStart,
            out long pStop
            );

        [PreserveSig]
        int GetMediaTimes2(
            out double pStart,
            out double pStop
            );

        [PreserveSig]
        int ModifyStopTime(
            long Stop
            );

        [PreserveSig]
        int ModifyStopTime2(
            double Stop
            );

        [PreserveSig]
        int FixMediaTimes(
            ref long pStart,
            ref long pStop
            );

        [PreserveSig]
        int FixMediaTimes2(
            ref double pStart,
            ref double pStop
            );

        [PreserveSig]
        int SetMediaTimes(
            long Start,
            long Stop
            );

        [PreserveSig]
        int SetMediaTimes2(
            double Start,
            double Stop
            );

        [PreserveSig]
        int SetMediaLength(
            long Length
            );

        [PreserveSig]
        int SetMediaLength2(
            double Length
            );

        [PreserveSig]
        int GetMediaLength(
            out long pLength
            );

        [PreserveSig]
        int GetMediaLength2(
            out double pLength
            );

        [PreserveSig]
        int GetMediaName(
            [MarshalAs(UnmanagedType.BStr)] out string pVal
            );

        [PreserveSig]
        int SetMediaName(
            [MarshalAs(UnmanagedType.BStr)] string newVal
            );

        [PreserveSig]
        int SpliceWithNext(
            IAMTimelineObj pNext
            );

        [PreserveSig]
        int GetStreamNumber(
            out int pVal
            );

        [PreserveSig]
        int SetStreamNumber(
            int Val
            );

        [PreserveSig]
        int IsNormalRate(
            [MarshalAs(UnmanagedType.Bool)] out bool pVal
            );

        [PreserveSig]
        int GetDefaultFPS(
            out double pFPS
            );

        [PreserveSig]
        int SetDefaultFPS(
            double FPS
            );

        [PreserveSig]
        int GetStretchMode(
            out ResizeFlags pnStretchMode
            );

        [PreserveSig]
        int SetStretchMode(
            ResizeFlags nStretchMode
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("EAE58538-622E-11D2-8CAD-00A024580902"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMTimelineTrack
    {
        [PreserveSig]
        int SrcAdd(
            IAMTimelineObj pSource
            );

        [PreserveSig]
        int GetNextSrc(
            out IAMTimelineObj ppSrc,
            ref long pInOut
            );

        [PreserveSig]
        int GetNextSrc2(
            out IAMTimelineObj ppSrc,
            ref double pInOut
            );

        [PreserveSig]
        int MoveEverythingBy(
            long Start,
            long MoveBy
            );

        [PreserveSig]
        int MoveEverythingBy2(
            double Start,
            double MoveBy
            );

        [PreserveSig]
        int GetSourcesCount(
            out int pVal
            );

        [PreserveSig]
        int AreYouBlank(
            [MarshalAs(UnmanagedType.Bool)] out bool pVal
            );

        [PreserveSig]
        int GetSrcAtTime(
            out IAMTimelineObj ppSrc,
            long Time,
            DexterFTrackSearchFlags SearchDirection
            );

        [PreserveSig]
        int GetSrcAtTime2(
            out IAMTimelineObj ppSrc,
            double Time,
            DexterFTrackSearchFlags SearchDirection
            );

        [PreserveSig]
        int InsertSpace(
            long rtStart,
            long rtEnd
            );

        [PreserveSig]
        int InsertSpace2(
            double rtStart,
            double rtEnd
            );

        [PreserveSig]
        int ZeroBetween(
            long rtStart,
            long rtEnd
            );

        [PreserveSig]
        int ZeroBetween2(
            double rtStart,
            double rtEnd
            );

        [PreserveSig]
        int GetNextSrcEx(
            IAMTimelineObj pLast,
            out IAMTimelineObj ppNext
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("BCE0C265-622D-11D2-8CAD-00A024580902")]
    public interface IAMTimelineTrans
    {
        [PreserveSig]
        int GetCutPoint(
            out long pTLTime
            );

        [PreserveSig]
        int GetCutPoint2(
            out double pTLTime
            );

        [PreserveSig]
        int SetCutPoint(
            long TLTime
            );

        [PreserveSig]
        int SetCutPoint2(
            double TLTime
            );

        [PreserveSig]
        int GetSwapInputs(
            [MarshalAs(UnmanagedType.Bool)] out bool pVal
            );

        [PreserveSig]
        int SetSwapInputs(
            [MarshalAs(UnmanagedType.Bool)] bool pVal
            );

        [PreserveSig]
        int GetCutsOnly(
            [MarshalAs(UnmanagedType.Bool)] out bool pVal
            );

        [PreserveSig]
        int SetCutsOnly(
            [MarshalAs(UnmanagedType.Bool)] bool pVal
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("378FA386-622E-11D2-8CAD-00A024580902"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMTimelineTransable
    {
        [PreserveSig]
        int TransAdd(
            IAMTimelineObj pTrans
            );

        [PreserveSig]
        int TransGetCount(
            out int pCount
            );

        [PreserveSig]
        int GetNextTrans(
            out IAMTimelineObj ppTrans,
            ref long pInOut
            );

        [PreserveSig]
        int GetNextTrans2(
            out IAMTimelineObj ppTrans,
            ref double pInOut
            );

        [PreserveSig]
        int GetTransAtTime(
            out IAMTimelineObj ppObj,
            long Time,
            DexterFTrackSearchFlags SearchDirection
            );

        [PreserveSig]
        int GetTransAtTime2(
            out IAMTimelineObj ppObj,
            double Time,
            DexterFTrackSearchFlags SearchDirection
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("A8ED5F80-C2C7-11D2-8D39-00A0C9441E20"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMTimelineVirtualTrack
    {
        [PreserveSig]
        int TrackGetPriority(
            out int pPriority
            );

        [PreserveSig]
        int SetTrackDirty();
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("65BD0710-24D2-4ff7-9324-ED2E5D3ABAFA")]
    public interface IMediaDet
    {
        [PreserveSig]
        int get_Filter(
            [MarshalAs(UnmanagedType.IUnknown)] out object pVal
            );

        [PreserveSig]
        int put_Filter(
            [MarshalAs(UnmanagedType.IUnknown)] object newVal
            );

        [PreserveSig]
        int get_OutputStreams(
            out int pVal
            );

        [PreserveSig]
        int get_CurrentStream(
            out int pVal
            );

        [PreserveSig]
        int put_CurrentStream(
            int newVal
            );

        [PreserveSig]
        int get_StreamType(
            out Guid pVal
            );

        [PreserveSig]
        int get_StreamTypeB(
            [MarshalAs(UnmanagedType.BStr)] out string pVal
            );

        [PreserveSig]
        int get_StreamLength(
            out double pVal
            );

        [PreserveSig]
        int get_Filename(
            [MarshalAs(UnmanagedType.BStr)] out string pVal
            );

        [PreserveSig]
        int put_Filename(
            [MarshalAs(UnmanagedType.BStr)] string newVal
            );

        [PreserveSig]
        int GetBitmapBits(
            double StreamTime,
            out int pBufferSize,
            [In] IntPtr pBuffer,
            int Width,
            int Height
            );

        [PreserveSig]
        int WriteBitmapBits(
            double StreamTime,
            int Width,
            int Height,
            [In, MarshalAs(UnmanagedType.BStr)] string Filename);

        [PreserveSig]
        int get_StreamMediaType(
            [Out, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pVal);

        [PreserveSig]
        int GetSampleGrabber(
            out ISampleGrabber ppVal);

        [PreserveSig]
        int get_FrameRate(
            out double pVal);

        [PreserveSig]
        int EnterBitmapGrabMode(
            double SeekTime);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("288581E0-66CE-11D2-918F-00C0DF10D434")]
    public interface IMediaLocator
    {
        [PreserveSig]
        int FindMediaFile(
            [MarshalAs(UnmanagedType.BStr)] string Input,
            [MarshalAs(UnmanagedType.BStr)] string FilterString,
            [MarshalAs(UnmanagedType.BStr)] out string pOutput,
            SFNValidateFlags Flags
            );

        [PreserveSig]
        int AddFoundLocation(
            [MarshalAs(UnmanagedType.BStr)] string DirectoryName
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("AE9472BD-B0C3-11D2-8D24-00A0C9441E20")]
    public interface IPropertySetter
    {
        [PreserveSig]
        int LoadXML(
            [In, MarshalAs(UnmanagedType.IUnknown)] object pxml
            );

        [PreserveSig]
        int PrintXML(
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder pszXML,
            [In] int cbXML,
            out int pcbPrinted,
            [In] int indent
            );

        [PreserveSig]
        int CloneProps(
            out IPropertySetter ppSetter,
            [In] long rtStart,
            [In] long rtStop
            );

        [PreserveSig]
        int AddProp(
            [In] DexterParam Param,
            [In, MarshalAs(UnmanagedType.LPArray)] DexterValue [] paValue
            );

        [PreserveSig]
        int GetProps(
            out int pcParams,
            out IntPtr paParam,
            out IntPtr paValue
            );

        [PreserveSig]
        int FreeProps(
            [In] int cParams,
            [In] IntPtr paParam,
            [In] IntPtr paValue
            );

        [PreserveSig]
        int ClearProps();

        [PreserveSig]
        int SaveToBlob(
            out int pcSize,
            out IntPtr ppb
            );

        [PreserveSig]
        int LoadFromBlob(
            [In] int cSize,
            [In] IntPtr pb
            );

        [PreserveSig]
        int SetProps(
            [In, MarshalAs(UnmanagedType.IUnknown)] object pTarget,
            [In] long rtNow
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("6BEE3A81-66C9-11D2-918F-00C0DF10D434")]
    public interface IRenderEngine
    {
        [PreserveSig]
        int SetTimelineObject(
            IAMTimeline pTimeline
            );

        [PreserveSig]
        int GetTimelineObject(
            out IAMTimeline ppTimeline
            );

        [PreserveSig]
        int GetFilterGraph(
            out IGraphBuilder ppFG
            );

        [PreserveSig]
        int SetFilterGraph(
            IGraphBuilder pFG
            );

        [PreserveSig]
        int SetInterestRange(
            long Start,
            long Stop
            );

        [PreserveSig]
        int SetInterestRange2(
            double Start,
            double Stop
            );

        [PreserveSig]
        int SetRenderRange(
            long Start,
            long Stop
            );

        [PreserveSig]
        int SetRenderRange2(
            double Start,
            double Stop
            );

        [PreserveSig]
        int GetGroupOutputPin(
            int Group,
            out IPin ppRenderPin
            );

        [PreserveSig]
        int ScrapIt();

        [PreserveSig]
        int RenderOutputPins();

        [PreserveSig]
        int GetVendorString(
            [MarshalAs(UnmanagedType.BStr)] out string sVendor
            );

        [PreserveSig]
        int ConnectFrontEnd();

        [PreserveSig]
        int SetSourceConnectCallback(
#if ALLOW_UNTESTED_INTERFACES
            IGrfCache pCallback
#else
            object pCallback
#endif
            );

        [PreserveSig]
        int SetDynamicReconnectLevel(
            ConnectFDynamic Level
            );

        [PreserveSig]
        int DoSmartRecompression();

        [PreserveSig]
        int UseInSmartRecompressionGraph();

        [PreserveSig]
        int SetSourceNameValidation(
            [MarshalAs(UnmanagedType.BStr)] string FilterString,
            IMediaLocator pOverride,
            SFNValidateFlags Flags
            );

        [PreserveSig]
        int Commit();

        [PreserveSig]
        int Decommit();

        [PreserveSig]
        int GetCaps(
            int Index,
            out int pReturn
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("F03FA8CE-879A-4D59-9B2C-26BB1CF83461"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ISmartRenderEngine
    {
        [PreserveSig]
        int SetGroupCompressor(
            int Group,
            IBaseFilter pCompressor
            );

        [PreserveSig]
        int GetGroupCompressor(
            int Group,
            out IBaseFilter pCompressor
            );

        [PreserveSig]
        int SetFindCompressorCB(
#if ALLOW_UNTESTED_INTERFACES
            IFindCompressorCB pCallback
#else
            object pCallback
#endif
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsDual),
    Guid("18C628ED-962A-11D2-8D08-00A0C9441E20")]
    public interface IXml2Dex
    {
        [PreserveSig]
        int CreateGraphFromFile(
            [MarshalAs(UnmanagedType.IUnknown)] out object ppGraph,
            [MarshalAs(UnmanagedType.IUnknown)] object pTimeline,
            [MarshalAs(UnmanagedType.BStr)] string Filename
            );

        [PreserveSig]
        int WriteGrfFile(
            [MarshalAs(UnmanagedType.IUnknown)] object pGraph,
            [MarshalAs(UnmanagedType.BStr)] string Filename
            );

        [PreserveSig]
        int WriteXMLFile(
            [MarshalAs(UnmanagedType.IUnknown)] object pTimeline,
            [MarshalAs(UnmanagedType.BStr)] string Filename
            );

        [PreserveSig]
        int ReadXMLFile(
            [MarshalAs(UnmanagedType.IUnknown)] object pTimeline,
            [MarshalAs(UnmanagedType.BStr)] string XMLName
            );

        [PreserveSig]
        int Delete(
            [MarshalAs(UnmanagedType.IUnknown)] object pTimeline,
            double dStart,
            double dEnd
            );

        [PreserveSig]
        int WriteXMLPart(
            [MarshalAs(UnmanagedType.IUnknown)] object pTimeline,
            double dStart,
            double dEnd,
            [MarshalAs(UnmanagedType.BStr)] string Filename
            );

        [PreserveSig]
        int PasteXMLFile(
            [MarshalAs(UnmanagedType.IUnknown)] object pTimeline,
            double dStart,
            [MarshalAs(UnmanagedType.BStr)] string Filename
            );

        [PreserveSig]
        int CopyXML(
            [MarshalAs(UnmanagedType.IUnknown)] object pTimeline,
            double dStart,
            double dEnd
            );

        [PreserveSig]
        int PasteXML(
            [MarshalAs(UnmanagedType.IUnknown)] object pTimeline,
            double dStart
            );

        [PreserveSig]
        int Reset();

        [PreserveSig]
        int ReadXML(
            [MarshalAs(UnmanagedType.IUnknown)] object pTimeline,
            [MarshalAs(UnmanagedType.IUnknown)] object pxml
            );

        [PreserveSig]
        int WriteXML(
            [MarshalAs(UnmanagedType.IUnknown)] object pTimeline,
            [MarshalAs(UnmanagedType.BStr)] out string pbstrXML
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("6BEE3A82-66C9-11d2-918F-00C0DF10D434")]
    public interface IRenderEngine2
    {
        [PreserveSig]
        int SetResizerGUID(
            [In] Guid ResizerGuid
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("4ada63a0-72d5-11d2-952a-0060081840bc")]
    public interface IResize
    {
        [PreserveSig]
        int get_Size(
            out int piHeight,
            out int piWidth,
            out ResizeFlags pFlag
            );

        [PreserveSig]
        int get_InputSize(
            out int piHeight,
            out int piWidth
            );

        [PreserveSig]
        int put_Size(
            int Height,
            int Width,
            ResizeFlags Flag
            );

        [PreserveSig]
        int get_MediaType(
            [Out, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt
            );

        [PreserveSig]
        int put_MediaType(
            [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt
            );
    }

    #endregion
}
