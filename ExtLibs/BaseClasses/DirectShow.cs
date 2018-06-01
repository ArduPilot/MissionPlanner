using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Runtime.InteropServices.ComTypes;
using Sonic;

// namespace for export direct show interfaces
namespace DirectShow
{
    [ComVisible(false)]
    public static class MediaType
    {
        public static readonly Guid Null = Guid.Empty;

        /// <summary> MEDIATYPE_Video 'vids' </summary>
        public static readonly Guid Video = new Guid(0x73646976, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIATYPE_Interleaved 'iavs' </summary>
        public static readonly Guid Interleaved = new Guid(0x73766169, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIATYPE_Audio 'auds' </summary>
        public static readonly Guid Audio = new Guid(0x73647561, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIATYPE_Text 'txts' </summary>
        public static readonly Guid Texts = new Guid(0x73747874, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIATYPE_Stream </summary>
        public static readonly Guid Stream = new Guid(0xe436eb83, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIATYPE_VBI </summary>
        public static readonly Guid VBI = new Guid(0xf72a76e1, 0xeb0a, 0x11d0, 0xac, 0xe4, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> MEDIATYPE_Midi </summary>
        public static readonly Guid Midi = new Guid(0x7364696D, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIATYPE_File </summary>
        public static readonly Guid File = new Guid(0x656c6966, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIATYPE_ScriptCommand </summary>
        public static readonly Guid ScriptCommand = new Guid(0x73636d64, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIATYPE_AUXLine21Data </summary>
        public static readonly Guid AuxLine21Data = new Guid(0x670aea80, 0x3a82, 0x11d0, 0xb7, 0x9b, 0x00, 0xaa, 0x00, 0x37, 0x67, 0xa7);

        /// <summary> MEDIATYPE_Timecode </summary>
        public static readonly Guid Timecode = new Guid(0x0482dee3, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIATYPE_LMRT </summary>
        public static readonly Guid LMRT = new Guid(0x74726c6d, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIATYPE_URL_STREAM </summary>
        public static readonly Guid URLStream = new Guid(0x736c7275, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIATYPE_AnalogVideo </summary>
        public static readonly Guid AnalogVideo = new Guid(0x0482dde1, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIATYPE_AnalogAudio </summary>
        public static readonly Guid AnalogAudio = new Guid(0x0482dee1, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIATYPE_MPEG2_SECTIONS </summary>
        public static readonly Guid Mpeg2Sections = new Guid(0x455f176c, 0x4b06, 0x47ce, 0x9a, 0xef, 0x8c, 0xae, 0xf7, 0x3d, 0xf7, 0xb5);

        /// <summary> MEDIATYPE_DTVCCData </summary>
        public static readonly Guid DTVCCData = new Guid(0xfb77e152, 0x53b2, 0x499c, 0xb4, 0x6b, 0x50, 0x9f, 0xc3, 0x3e, 0xdf, 0xd7);

        /// <summary> MEDIATYPE_MSTVCaption </summary>
        public static readonly Guid MSTVCaption = new Guid(0xB88B8A89, 0xB049, 0x4C80, 0xAD, 0xCF, 0x58, 0x98, 0x98, 0x5E, 0x22, 0xC1);

        /// <summary> MEDIATYPE_AUXTeletextPage </summary>
        public static readonly Guid AUXTeletextPage = new Guid(0x11264acb, 0x37de, 0x4eba, 0x8c, 0x35, 0x7f, 0x4, 0xa1, 0xa6, 0x83, 0x32);

        /// <summary> MEDIATYPE_CC_CONTAINER </summary>
        public static readonly Guid CC_Container = new Guid(0xaeb312e9, 0x3357, 0x43ca, 0xb7, 0x1, 0x97, 0xec, 0x19, 0x8e, 0x2b, 0x62);

    }

    [ComVisible(false)]
    public static class MediaSubType
    {
        public static readonly Guid Null = Guid.Empty;

        /// <summary> MEDIASUBTYPE_CLPL </summary>
        public static readonly Guid CLPL = new Guid(0x4C504C43, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_YUYV </summary>
        public static readonly Guid YUYV = new Guid(0x56595559, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_IYUV </summary>
        public static readonly Guid IYUV = new Guid(0x56555949, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_YVU9 </summary>
        public static readonly Guid YVU9 = new Guid(0x39555659, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_Y411 </summary>
        public static readonly Guid Y411 = new Guid(0x31313459, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_Y41P </summary>
        public static readonly Guid Y41P = new Guid(0x50313459, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_YUY2 </summary>
        public static readonly Guid YUY2 = new Guid(0x32595559, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_YVYU </summary>
        public static readonly Guid YVYU = new Guid(0x55595659, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_UYVY </summary>
        public static readonly Guid UYVY = new Guid(0x59565955, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_Y211 </summary>
        public static readonly Guid Y211 = new Guid(0x31313259, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_CLJR </summary>
        public static readonly Guid CLJR = new Guid(0x524a4c43, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_IF09 </summary>
        public static readonly Guid IF09 = new Guid(0x39304649, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_CPLA </summary>
        public static readonly Guid CPLA = new Guid(0x414c5043, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_MJPG </summary>
        public static readonly Guid MJPG = new Guid(0x47504A4D, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_TVMJ </summary>
        public static readonly Guid TVMJ = new Guid(0x4A4D5654, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_WAKE </summary>
        public static readonly Guid WAKE = new Guid(0x454B4157, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_CFCC </summary>
        public static readonly Guid CFCC = new Guid(0x43434643, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_IJPG </summary>
        public static readonly Guid IJPG = new Guid(0x47504A49, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_Plum </summary>
        public static readonly Guid PLUM = new Guid(0x6D756C50, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_DVCS </summary>
        public static readonly Guid DVCS = new Guid(0x53435644, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_DVSD </summary>
        public static readonly Guid DVSD = new Guid(0x44535644, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_MDVF </summary>
        public static readonly Guid MDVF = new Guid(0x4656444D, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_RGB1 </summary>
        public static readonly Guid RGB1 = new Guid(0xe436eb78, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_RGB4 </summary>
        public static readonly Guid RGB4 = new Guid(0xe436eb79, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_RGB8 </summary>
        public static readonly Guid RGB8 = new Guid(0xe436eb7a, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_RGB565 </summary>
        public static readonly Guid RGB565 = new Guid(0xe436eb7b, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_RGB555 </summary>
        public static readonly Guid RGB555 = new Guid(0xe436eb7c, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_RGB24 </summary>
        public static readonly Guid RGB24 = new Guid(0xe436eb7d, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_RGB32 </summary>
        public static readonly Guid RGB32 = new Guid(0xe436eb7e, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_ARGB1555 </summary>
        public static readonly Guid ARGB1555 = new Guid(0x297c55af, 0xe209, 0x4cb3, 0xb7, 0x57, 0xc7, 0x6d, 0x6b, 0x9c, 0x88, 0xa8);

        /// <summary> MEDIASUBTYPE_ARGB4444 </summary>
        public static readonly Guid ARGB4444 = new Guid(0x6e6415e6, 0x5c24, 0x425f, 0x93, 0xcd, 0x80, 0x10, 0x2b, 0x3d, 0x1c, 0xca);

        /// <summary> MEDIASUBTYPE_ARGB32 </summary>
        public static readonly Guid ARGB32 = new Guid(0x773c9ac0, 0x3274, 0x11d0, 0xb7, 0x24, 0x00, 0xaa, 0x00, 0x6c, 0x1a, 0x01);

        /// <summary> MEDIASUBTYPE_A2R10G10B10 </summary>
        public static readonly Guid A2R10G10B10 = new Guid(0x2f8bb76d, 0xb644, 0x4550, 0xac, 0xf3, 0xd3, 0x0c, 0xaa, 0x65, 0xd5, 0xc5);

        /// <summary> MEDIASUBTYPE_A2B10G10R10 </summary>
        public static readonly Guid A2B10G10R10 = new Guid(0x576f7893, 0xbdf6, 0x48c4, 0x87, 0x5f, 0xae, 0x7b, 0x81, 0x83, 0x45, 0x67);

        /// <summary> MEDIASUBTYPE_AYUV </summary>
        public static readonly Guid AYUV = new Guid(0x56555941, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_AI44 </summary>
        public static readonly Guid AI44 = new Guid(0x34344941, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_IA44 </summary>
        public static readonly Guid IA44 = new Guid(0x34344149, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_RGB32_D3D_DX7_RT </summary>
        public static readonly Guid RGB32_D3D_DX7_RT = new Guid(0x32335237, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_RGB16_D3D_DX7_RT </summary>
        public static readonly Guid RGB16_D3D_DX7_RT = new Guid(0x36315237, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_ARGB32_D3D_DX7_RT </summary>
        public static readonly Guid ARGB32_D3D_DX7_RT = new Guid(0x38384137, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_ARGB4444_D3D_DX7_RT </summary>
        public static readonly Guid ARGB4444_D3D_DX7_RT = new Guid(0x34344137, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_ARGB1555_D3D_DX7_RT </summary>
        public static readonly Guid ARGB1555_D3D_DX7_RT = new Guid(0x35314137, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_RGB32_D3D_DX9_RT </summary>
        public static readonly Guid RGB32_D3D_DX9_RT = new Guid(0x32335239, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_RGB16_D3D_DX9_RT </summary>
        public static readonly Guid RGB16_D3D_DX9_RT = new Guid(0x36315239, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_ARGB32_D3D_DX9_RT </summary>
        public static readonly Guid ARGB32_D3D_DX9_RT = new Guid(0x38384139, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_ARGB4444_D3D_DX9_RT </summary>
        public static readonly Guid ARGB4444_D3D_DX9_RT = new Guid(0x34344139, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_ARGB1555_D3D_DX9_RT </summary>
        public static readonly Guid ARGB1555_D3D_DX9_RT = new Guid(0x35314139, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_YV12 </summary>
        public static readonly Guid YV12 = new Guid(0x32315659, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_NV12 </summary>
        public static readonly Guid NV12 = new Guid(0x3231564E, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_IMC1 </summary>
        public static readonly Guid IMC1 = new Guid(0x31434D49, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_IMC2 </summary>
        public static readonly Guid IMC2 = new Guid(0x32434D49, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_IMC3 </summary>
        public static readonly Guid IMC3 = new Guid(0x33434D49, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_IMC4 </summary>
        public static readonly Guid IMC4 = new Guid(0x34434D49, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_S340 </summary>
        public static readonly Guid S340 = new Guid(0x30343353, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_S342 </summary>
        public static readonly Guid S342 = new Guid(0x32343353, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_Overlay </summary>
        public static readonly Guid Overlay = new Guid(0xe436eb7f, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_MPEG1Packet </summary>
        public static readonly Guid MPEG1Packet = new Guid(0xe436eb80, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_MPEG1Payload </summary>
        public static readonly Guid MPEG1Payload = new Guid(0xe436eb81, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_MPEG1AudioPayload </summary>
        public static readonly Guid MPEG1AudioPayload = new Guid(0x00000050, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

        /// <summary> MEDIATYPE_MPEG1SystemStream </summary>
        public static readonly Guid MPEG1SystemStream = new Guid(0xe436eb82, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_MPEG1System </summary>
        public static readonly Guid MPEG1System = new Guid(0xe436eb84, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_MPEG1VideoCD </summary>
        public static readonly Guid MPEG1VideoCD = new Guid(0xe436eb85, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_MPEG1Video </summary>
        public static readonly Guid MPEG1Video = new Guid(0xe436eb86, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_MPEG1Audio </summary>
        public static readonly Guid MPEG1Audio = new Guid(0xe436eb87, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_Avi </summary>
        public static readonly Guid Avi = new Guid(0xe436eb88, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_Asf </summary>
        public static readonly Guid Asf = new Guid(0x3db80f90, 0x9412, 0x11d1, 0xad, 0xed, 0x00, 0x00, 0xf8, 0x75, 0x4b, 0x99);

        /// <summary> MEDIASUBTYPE_QTMovie </summary>
        public static readonly Guid QTMovie = new Guid(0xe436eb89, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_QTRpza </summary>
        public static readonly Guid QTRpza = new Guid(0x617a7072, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_QTSmc </summary>
        public static readonly Guid QTSmc = new Guid(0x20636d73, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_QTRle </summary>
        public static readonly Guid QTRle = new Guid(0x20656c72, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_QTJpeg </summary>
        public static readonly Guid QTJpeg = new Guid(0x6765706a, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_PCMAudio_Obsolete </summary>
        public static readonly Guid PCMAudio_Obsolete = new Guid(0xe436eb8a, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_PCM </summary>
        public static readonly Guid PCM = new Guid(0x00000001, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

        /// <summary> MEDIASUBTYPE_WAVE </summary>
        public static readonly Guid WAVE = new Guid(0xe436eb8b, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_AU </summary>
        public static readonly Guid AU = new Guid(0xe436eb8c, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_AIFF </summary>
        public static readonly Guid AIFF = new Guid(0xe436eb8d, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_dvhd </summary>
        public static readonly Guid dvhd = new Guid(0x64687664, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_dvsl </summary>
        public static readonly Guid dvsl = new Guid(0x6c737664, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_dv25 </summary>
        public static readonly Guid dv25 = new Guid(0x35327664, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_dv50 </summary>
        public static readonly Guid dv50 = new Guid(0x30357664, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_dvh1 </summary>
        public static readonly Guid dvh1 = new Guid(0x31687664, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_Line21_BytePair </summary>
        public static readonly Guid Line21_BytePair = new Guid(0x6e8d4a22, 0x310c, 0x11d0, 0xb7, 0x9a, 0x00, 0xaa, 0x00, 0x37, 0x67, 0xa7);

        /// <summary> MEDIASUBTYPE_Line21_GOPPacket </summary>
        public static readonly Guid Line21_GOPPacket = new Guid(0x6e8d4a23, 0x310c, 0x11d0, 0xb7, 0x9a, 0x00, 0xaa, 0x00, 0x37, 0x67, 0xa7);

        /// <summary> MEDIASUBTYPE_Line21_VBIRawData </summary>
        public static readonly Guid Line21_VBIRawData = new Guid(0x6e8d4a24, 0x310c, 0x11d0, 0xb7, 0x9a, 0x00, 0xaa, 0x00, 0x37, 0x67, 0xa7);

        /// <summary> MEDIASUBTYPE_TELETEXT </summary>
        public static readonly Guid TELETEXT = new Guid(0xf72a76e3, 0xeb0a, 0x11d0, 0xac, 0xe4, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> MEDIASUBTYPE_WSS </summary>
        public static readonly Guid WSS = new Guid(0x2791D576, 0x8E7A, 0x466F, 0x9E, 0x90, 0x5D, 0x3F, 0x30, 0x83, 0x73, 0x8B);

        /// <summary> MEDIASUBTYPE_VPS </summary>
        public static readonly Guid VPS = new Guid(0xa1b3f620, 0x9792, 0x4d8d, 0x81, 0xa4, 0x86, 0xaf, 0x25, 0x77, 0x20, 0x90);

        /// <summary> MEDIASUBTYPE_DRM_Audio </summary>
        public static readonly Guid DRM_Audio = new Guid(0x00000009, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_IEEE_FLOAT </summary>
        public static readonly Guid IEEE_FLOAT = new Guid(0x00000003, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_DOLBY_AC3_SPDIF </summary>
        public static readonly Guid DOLBY_AC3_SPDIF = new Guid(0x00000092, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_RAW_SPORT </summary>
        public static readonly Guid RAW_SPORT = new Guid(0x00000240, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_SPDIF_TAG_241h </summary>
        public static readonly Guid SPDIF_TAG_241h = new Guid(0x00000241, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_DssVideo </summary>
        public static readonly Guid DssVideo = new Guid(0xa0af4f81, 0xe163, 0x11d0, 0xba, 0xd9, 0x00, 0x60, 0x97, 0x44, 0x11, 0x1a);

        /// <summary> MEDIASUBTYPE_DssAudio </summary>
        public static readonly Guid DssAudio = new Guid(0xa0af4f82, 0xe163, 0x11d0, 0xba, 0xd9, 0x00, 0x60, 0x97, 0x44, 0x11, 0x1a);

        /// <summary> MEDIASUBTYPE_VPVideo </summary>
        public static readonly Guid VPVideo = new Guid(0x5a9b6a40, 0x1a22, 0x11d1, 0xba, 0xd9, 0x00, 0x60, 0x97, 0x44, 0x11, 0x1a);

        /// <summary> MEDIASUBTYPE_VPVBI </summary>
        public static readonly Guid VPVBI = new Guid(0x5a9b6a41, 0x1a22, 0x11d1, 0xba, 0xd9, 0x00, 0x60, 0x97, 0x44, 0x11, 0x1a);

        /// <summary> MEDIASUBTYPE_AnalogVideo_NTSC_M </summary>
        public static readonly Guid AnalogVideo_NTSC_M = new Guid(0x0482dde2, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_PAL_B </summary>
        public static readonly Guid AnalogVideo_PAL_B = new Guid(0x0482dde5, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_PAL_D </summary>
        public static readonly Guid AnalogVideo_PAL_D = new Guid(0x0482dde6, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_PAL_G </summary>
        public static readonly Guid AnalogVideo_PAL_G = new Guid(0x0482dde7, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_PAL_H </summary>
        public static readonly Guid AnalogVideo_PAL_H = new Guid(0x0482dde8, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_PAL_I </summary>
        public static readonly Guid AnalogVideo_PAL_I = new Guid(0x0482dde9, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_PAL_M </summary>
        public static readonly Guid AnalogVideo_PAL_M = new Guid(0x0482ddea, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_PAL_N </summary>
        public static readonly Guid AnalogVideo_PAL_N = new Guid(0x0482ddeb, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_PAL_N_COMBO </summary>
        public static readonly Guid AnalogVideo_PAL_N_COMBO = new Guid(0x0482ddec, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_SECAM_B </summary>
        public static readonly Guid AnalogVideo_SECAM_B = new Guid(0x0482ddf0, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_SECAM_D </summary>
        public static readonly Guid AnalogVideo_SECAM_D = new Guid(0x0482ddf1, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_SECAM_G </summary>
        public static readonly Guid AnalogVideo_SECAM_G = new Guid(0x0482ddf2, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_SECAM_H </summary>
        public static readonly Guid AnalogVideo_SECAM_H = new Guid(0x0482ddf3, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_SECAM_K </summary>
        public static readonly Guid AnalogVideo_SECAM_K = new Guid(0x0482ddf4, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_SECAM_K1 </summary>
        public static readonly Guid AnalogVideo_SECAM_K1 = new Guid(0x0482ddf5, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_SECAM_L </summary>
        public static readonly Guid AnalogVideo_SECAM_L = new Guid(0x0482ddf6, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> not in uuids.h </summary>
        public static readonly Guid I420 = new Guid(0x30323449, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> WMMEDIASUBTYPE_VIDEOIMAGE </summary>
        public static readonly Guid VideoImage = new Guid(0x1d4a45f2, 0xe5f6, 0x4b44, 0x83, 0x88, 0xf0, 0xae, 0x5c, 0x0e, 0x0c, 0x37);

        /// <summary> WMMEDIASUBTYPE_MPEG2_VIDEO </summary>
        public static readonly Guid Mpeg2Video = new Guid(0xe06d8026, 0xdb46, 0x11cf, 0xb4, 0xd1, 0x00, 0x80, 0x5f, 0x6c, 0xbb, 0xea);

        /// <summary> WMMEDIASUBTYPE_WebStream </summary>
        public static readonly Guid WebStream = new Guid(0x776257d4, 0xc627, 0x41cb, 0x8f, 0x81, 0x7a, 0xc7, 0xff, 0x1c, 0x40, 0xcc);

        /// <summary> MEDIASUBTYPE_MPEG2_AUDIO </summary>
        public static readonly Guid Mpeg2Audio = new Guid(0xe06d802b, 0xdb46, 0x11cf, 0xb4, 0xd1, 0x00, 0x80, 0x5f, 0x6c, 0xbb, 0xea);

        /// <summary> MEDIASUBTYPE_DOLBY_AC3 </summary>
        public static readonly Guid DolbyAC3 = new Guid(0xe06d802c, 0xdb46, 0x11cf, 0xb4, 0xd1, 0x00, 0x80, 0x5f, 0x6c, 0xbb, 0xea);

        /// <summary> MEDIASUBTYPE_DVB_SI </summary>
        public static readonly Guid DvbSI = new Guid(0xe9dd31a3, 0x221d, 0x4adb, 0x85, 0x32, 0x9a, 0xf3, 0x09, 0xc1, 0xa4, 0x08);

        /// <summary> MEDIASUBTYPE_ATSC_SI </summary>
        public static readonly Guid AtscSI = new Guid(0xb3c7397c, 0xd303, 0x414d, 0xb3, 0x3c, 0x4e, 0xd2, 0xc9, 0xd2, 0x97, 0x33);

        /// <summary> MEDIASUBTYPE_MPEG2DATA </summary>
        public static readonly Guid Mpeg2Data = new Guid(0xc892e55b, 0x252d, 0x42b5, 0xa3, 0x16, 0xd9, 0x97, 0xe7, 0xa5, 0xd9, 0x95);

        /// <summary> MEDIASUBTYPE_MPEG2_PROGRAM </summary>
        public static readonly Guid Mpeg2Program = new Guid(0xe06d8022, 0xdb46, 0x11cf, 0xb4, 0xd1, 0x00, 0x80, 0x5f, 0x6c, 0xbb, 0xea);

        /// <summary> MEDIASUBTYPE_MPEG2_TRANSPORT </summary>
        public static readonly Guid Mpeg2Transport = new Guid(0xe06d8023, 0xdb46, 0x11cf, 0xb4, 0xd1, 0x00, 0x80, 0x5f, 0x6c, 0xbb, 0xea);

        /// <summary> MEDIASUBTYPE_MPEG2_TRANSPORT_STRIDE </summary>
        public static readonly Guid Mpeg2TransportStride = new Guid(0x138aa9a4, 0x1ee2, 0x4c5b, 0x98, 0x8e, 0x19, 0xab, 0xfd, 0xbc, 0x8a, 0x11);

        /// <summary> MEDIASUBTYPE_None </summary>
        public static readonly Guid None = new Guid(0xe436eb8e, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_H264 </summary>
        public static readonly Guid H264 = new Guid(0x34363248, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_NV24 </summary>
        public static readonly Guid NV24 = new Guid(0x3432564E, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_708_608Data </summary>
        public static readonly Guid Data708_608 = new Guid(0xaf414bc, 0x4ed2, 0x445e, 0x98, 0x39, 0x8f, 0x9, 0x55, 0x68, 0xab, 0x3c);

        /// <summary> MEDIASUBTYPE_DtvCcData </summary>
        public static readonly Guid DtvCcData = new Guid(0xF52ADDAA, 0x36F0, 0x43F5, 0x95, 0xEA, 0x6D, 0x86, 0x64, 0x84, 0x26, 0x2A);

        /// <summary> MEDIASUBTYPE_DVB_SUBTITLES </summary>
        public static readonly Guid DVB_Subtitles = new Guid(0x34FFCBC3, 0xD5B3, 0x4171, 0x90, 0x02, 0xD4, 0xC6, 0x03, 0x01, 0x69, 0x7F);

        /// <summary> MEDIASUBTYPE_ISDB_CAPTIONS </summary>
        public static readonly Guid ISDB_Captions = new Guid(0x059dd67d, 0x2e55, 0x4d41, 0x8d, 0x1b, 0x01, 0xf5, 0xe4, 0xf5, 0x06, 0x07);

        /// <summary> MEDIASUBTYPE_ISDB_SUPERIMPOSE </summary>
        public static readonly Guid ISDB_Superimpose = new Guid(0x36dc6d28, 0xf1a6, 0x4216, 0x90, 0x48, 0x9c, 0xfc, 0xef, 0xeb, 0x5e, 0xba);

        /// <summary> MEDIASUBTYPE_NV11 </summary>
        public static readonly Guid NV11 = new Guid(0x3131564E, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_P208 </summary>
        public static readonly Guid P208 = new Guid(0x38303250, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_P210 </summary>
        public static readonly Guid P210 = new Guid(0x38303250, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_P216 </summary>
        public static readonly Guid P216 = new Guid(0x38303250, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_P010 </summary>
        public static readonly Guid P010 = new Guid(0x38303250, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_P016 </summary>
        public static readonly Guid P016 = new Guid(0x38303250, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_Y210 </summary>
        public static readonly Guid Y210 = new Guid(0x38303250, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_Y216 </summary>
        public static readonly Guid Y216 = new Guid(0x38303250, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_P408 </summary>
        public static readonly Guid P408 = new Guid(0x38303450, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_CC_CONTAINER </summary>
        public static readonly Guid CC_Container = new Guid(0x7ea626db, 0x54da, 0x437b, 0xbe, 0x9f, 0xf7, 0x30, 0x73, 0xad, 0xfa, 0x3c);

        /// <summary> MEDIASUBTYPE_VBI </summary>
        public static readonly Guid VBI = new Guid(0x663da43c, 0x3e8, 0x4e9a, 0x9c, 0xd5, 0xbf, 0x11, 0xed, 0xd, 0xef, 0x76);

        /// <summary> MEDIASUBTYPE_XDS </summary>
        public static readonly Guid XDS = new Guid(0x1ca73e3, 0xdce6, 0x4575, 0xaf, 0xe1, 0x2b, 0xf1, 0xc9, 0x2, 0xca, 0xf3);

        /// <summary> MEDIASUBTYPE_ETDTFilter_Tagged </summary>
        public static readonly Guid ETDTFilter_Tagged = new Guid(0xC4C4C4D0, 0x0049, 0x4E2B, 0x98, 0xFB, 0x95, 0x37, 0xF6, 0xCE, 0x51, 0x6D);

        /// <summary> MEDIASUBTYPE_CPFilters_Processed </summary>
        public static readonly Guid CPFilters_Processed = new Guid(0x46adbd28, 0x6fd0, 0x4796, 0x93, 0xb2, 0x15, 0x5c, 0x51, 0xdc, 0x4, 0x8d);

    }

    [ComVisible(false)]
    public static class FormatType
    {
        public static readonly Guid Null = Guid.Empty;

        /// <summary> FORMAT_None </summary>
        public static readonly Guid None = new Guid(0x0F6417D6, 0xc318, 0x11d0, 0xa4, 0x3f, 0x00, 0xa0, 0xc9, 0x22, 0x31, 0x96);

        /// <summary> FORMAT_VideoInfo </summary>
        public static readonly Guid VideoInfo = new Guid(0x05589f80, 0xc356, 0x11ce, 0xbf, 0x01, 0x00, 0xaa, 0x00, 0x55, 0x59, 0x5a);

        /// <summary> FORMAT_VideoInfo2 </summary>
        public static readonly Guid VideoInfo2 = new Guid(0xf72a76A0, 0xeb0a, 0x11d0, 0xac, 0xe4, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> FORMAT_WaveFormatEx </summary>
        public static readonly Guid WaveEx = new Guid(0x05589f81, 0xc356, 0x11ce, 0xbf, 0x01, 0x00, 0xaa, 0x00, 0x55, 0x59, 0x5a);

        /// <summary> FORMAT_MPEGVideo </summary>
        public static readonly Guid MpegVideo = new Guid(0x05589f82, 0xc356, 0x11ce, 0xbf, 0x01, 0x00, 0xaa, 0x00, 0x55, 0x59, 0x5a);

        /// <summary> FORMAT_MPEGStreams </summary>
        public static readonly Guid MpegStreams = new Guid(0x05589f83, 0xc356, 0x11ce, 0xbf, 0x01, 0x00, 0xaa, 0x00, 0x55, 0x59, 0x5a);

        /// <summary> FORMAT_DvInfo </summary>
        public static readonly Guid DvInfo = new Guid(0x05589f84, 0xc356, 0x11ce, 0xbf, 0x01, 0x00, 0xaa, 0x00, 0x55, 0x59, 0x5a);

        /// <summary> FORMAT_AnalogVideo </summary>
        public static readonly Guid AnalogVideo = new Guid(0x0482dde0, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> FORMAT_MPEG2Video </summary>
        public static readonly Guid Mpeg2Video = new Guid(0xe06d80e3, 0xdb46, 0x11cf, 0xb4, 0xd1, 0x00, 0x80, 0x5f, 0x6c, 0xbb, 0xea);

        /// <summary> FORMAT_DolbyAC3 </summary>
        public static readonly Guid DolbyAC3 = new Guid(0xe06d80e4, 0xdb46, 0x11cf, 0xb4, 0xd1, 0x00, 0x80, 0x5f, 0x6c, 0xbb, 0xea);

        /// <summary> FORMAT_MPEG2Audio </summary>
        public static readonly Guid Mpeg2Audio = new Guid(0xe06d80e5, 0xdb46, 0x11cf, 0xb4, 0xd1, 0x00, 0x80, 0x5f, 0x6c, 0xbb, 0xea);

        /// <summary> FORMAT_525WSS </summary>
        public static readonly Guid WSS525 = new Guid(0xc7ecf04d, 0x4582, 0x4869, 0x9a, 0xbb, 0xbf, 0xb5, 0x23, 0xb6, 0x2e, 0xdf);

        /// <summary> FORMATTYPE_ETDTFilter_Tagged </summary>
        public static readonly Guid ETDTFilter_Tagged = new Guid(0xC4C4C4D1, 0x0049, 0x4E2B, 0x98, 0xFB, 0x95, 0x37, 0xF6, 0xCE, 0x51, 0x6D);

        /// <summary> FORMATTYPE_CPFilters_Processed </summary>
        public static readonly Guid CPFilters_Processed = new Guid(0x6739b36f, 0x1d5f, 0x4ac2, 0x81, 0x92, 0x28, 0xbb, 0xe, 0x73, 0xd1, 0x6a);
    }

    [ComVisible(false)]
    public static class FilterCategory
    {
        /// <summary> CLSID_CPCAFiltersCategory </summary>
        public static readonly Guid CPCAFiltersCategory = new Guid(0xC4C4C4FC, 0x0049, 0x4E2B, 0x98, 0xFB, 0x95, 0x37, 0xF6, 0xCE, 0x51, 0x6D);

        /// <summary> CLSID_MediaEncoderCategory </summary>
        public static readonly Guid MediaEncoderCategory = new Guid(0x7D22E920, 0x5CA9, 0x4787, 0x8C, 0x2B, 0xA6, 0x77, 0x9B, 0xD1, 0x17, 0x81);

        /// <summary> CLSID_MediaMultiplexerCategory </summary>
        public static readonly Guid MediaMultiplexerCategory = new Guid(0x236C9559, 0xADCE, 0x4736, 0xBF, 0x72, 0xBA, 0xB3, 0x4E, 0x39, 0x21, 0x96);

        /// <summary> CLSID_DMOFilterCategory </summary>
        public static readonly Guid DMOFilterCategory = new Guid(0xbcd5796c, 0xbd52, 0x4d30, 0xab, 0x76, 0x70, 0xf9, 0x75, 0xb8, 0x91, 0x99);

        /// <summary> CLSID_AudioInputDeviceCategory, audio capture category </summary>
        public static readonly Guid AudioInputDevice = new Guid(0x33d9a762, 0x90c8, 0x11d0, 0xbd, 0x43, 0x00, 0xa0, 0xc9, 0x11, 0xce, 0x86);

        /// <summary> CLSID_VideoInputDeviceCategory, video capture category </summary>
        public static readonly Guid VideoInputDevice = new Guid(0x860BB310, 0x5D01, 0x11d0, 0xBD, 0x3B, 0x00, 0xA0, 0xC9, 0x11, 0xCE, 0x86);

        /// <summary> CLSID_VideoCompressorCategory, video compressor category </summary>
        public static readonly Guid VideoCompressorCategory = new Guid(0x33d9a760, 0x90c8, 0x11d0, 0xbd, 0x43, 0x00, 0xa0, 0xc9, 0x11, 0xce, 0x86);

        /// <summary> CLSID_AudioCompressorCategory, audio compressor category </summary>
        public static readonly Guid AudioCompressorCategory = new Guid(0x33d9a761, 0x90c8, 0x11d0, 0xbd, 0x43, 0x00, 0xa0, 0xc9, 0x11, 0xce, 0x86);

        /// <summary> CLSID_LegacyAmFilterCategory, legacy filters </summary>
        public static readonly Guid LegacyAmFilterCategory = new Guid(0x083863F1, 0x70DE, 0x11d0, 0xBD, 0x40, 0x00, 0xA0, 0xC9, 0x11, 0xCE, 0x86);

        /// <summary> CLSID_AudioRendererCategory, Audio renderer category</summary>
        public static readonly Guid AudioRendererCategory = new Guid(0xe0f158e1, 0xcb04, 0x11d0, 0xbd, 0x4e, 0x00, 0xa0, 0xc9, 0x11, 0xce, 0x86);

        /// <summary> KSCATEGORY_BDA_RECEIVER_COMPONENT, BDA Receiver Components category</summary>
        public static readonly Guid BDAReceiverComponentsCategory = new Guid(0xFD0A5AF4, 0xB41D, 0x11d2, 0x9c, 0x95, 0x00, 0xc0, 0x4f, 0x79, 0x71, 0xe0);

        /// <summary> KSCATEGORY_BDA_NETWORK_TUNER, BDA Source Filters category</summary>
        public static readonly Guid BDASourceFiltersCategory = new Guid(0x71985f48, 0x1ca1, 0x11d3, 0x9c, 0xc8, 0x00, 0xc0, 0x4f, 0x79, 0x71, 0xe0);

        /// <summary> KSCATEGORY_BDA_IP_SINK, BDA Rendering Filters category</summary>
        public static readonly Guid BDARenderingFiltersCategory = new Guid(0x71985f4a, 0x1ca1, 0x11d3, 0x9c, 0xc8, 0x00, 0xc0, 0x4f, 0x79, 0x71, 0xe0);

        /// <summary> KSCATEGORY_BDA_NETWORK_PROVIDER, BDA Network Providers category</summary>
        public static readonly Guid BDANetworkProvidersCategory = new Guid(0x71985f4b, 0x1ca1, 0x11d3, 0x9c, 0xc8, 0x00, 0xc0, 0x4f, 0x79, 0x71, 0xe0);

        /// <summary> KSCATEGORY_BDA_TRANSPORT_INFORMATION, BDA Transport Information Renderers category</summary>
        public static readonly Guid BDATransportInformationRenderersCategory = new Guid(0xa2e3074f, 0x6c3d, 0x11d3, 0xb6, 0x53, 0x00, 0xc0, 0x4f, 0x79, 0x49, 0x8e);

        /// <summary> CLSID_MidiRendererCategory </summary>
        public static readonly Guid MidiRendererCategory = new Guid(0x4EfE2452, 0x168A, 0x11d1, 0xBC, 0x76, 0x00, 0xc0, 0x4F, 0xB9, 0x45, 0x3B);

        /// <summary> CLSID_TransmitCategory  External Renderers Category</summary>
        public static readonly Guid TransmitCategory = new Guid(0xcc7bfb41, 0xf175, 0x11d1, 0xa3, 0x92, 0x00, 0xe0, 0x29, 0x1f, 0x39, 0x59);

        /// <summary> CLSID_DeviceControlCategory  Device Control Filters</summary>
        public static readonly Guid DeviceControlCategory = new Guid(0xcc7bfb46, 0xf175, 0x11d1, 0xa3, 0x92, 0x00, 0xe0, 0x29, 0x1f, 0x39, 0x59);

        /// <summary> CLSID_VideoEffects1Category </summary>
        public static readonly Guid VideoEffects1Category = new Guid(0xcc7bfb42, 0xf175, 0x11d1, 0xa3, 0x92, 0x00, 0xe0, 0x29, 0x1f, 0x39, 0x59);

        /// <summary> CLSID_VideoEffects2Category </summary>
        public static readonly Guid VideoEffects2Category = new Guid(0xcc7bfb43, 0xf175, 0x11d1, 0xa3, 0x92, 0x00, 0xe0, 0x29, 0x1f, 0x39, 0x59);

        /// <summary> CLSID_AudioEffects1Category </summary>
        public static readonly Guid AudioEffects1Category = new Guid(0xcc7bfb44, 0xf175, 0x11d1, 0xa3, 0x92, 0x00, 0xe0, 0x29, 0x1f, 0x39, 0x59);

        /// <summary> CLSID_AudioEffects2Category </summary>
        public static readonly Guid AudioEffects2Category = new Guid(0xcc7bfb45, 0xf175, 0x11d1, 0xa3, 0x92, 0x00, 0xe0, 0x29, 0x1f, 0x39, 0x59);

        /// <summary> KSCATEGORY_DATADECOMPRESSOR & CLSID_DVDHWDecodersCategory</summary>
        public static readonly Guid KSDataDecompressor = new Guid(0x2721AE20, 0x7E70, 0x11D0, 0xA5, 0xD6, 0x28, 0xDB, 0x04, 0xC1, 0x00, 0x00);

        /// <summary> KSCATEGORY_COMMUNICATIONSTRANSFORM </summary>
        public static readonly Guid KSCommunicationsTransform = new Guid(0xCF1DDA2C, 0x9743, 0x11D0, 0xA3, 0xEE, 0x00, 0xA0, 0xC9, 0x22, 0x31, 0x96);

        /// <summary> KSCATEGORY_DATATRANSFORM </summary>
        public static readonly Guid KSDataTransform = new Guid(0x2EB07EA0, 0x7E70, 0x11D0, 0xA5, 0xD6, 0x28, 0xDB, 0x04, 0xC1, 0x00, 0x00);

        /// <summary> KSCATEGORY_INTERFACETRANSFORM </summary>
        public static readonly Guid KSInterfaceTransform = new Guid(0xCF1DDA2D, 0x9743, 0x11D0, 0xA3, 0xEE, 0x00, 0xA0, 0xC9, 0x22, 0x31, 0x96);

        /// <summary> KSCATEGORY_MIXER </summary>
        public static readonly Guid KSMixer = new Guid(0xAD809C00, 0x7B88, 0x11D0, 0xA5, 0xD6, 0x28, 0xDB, 0x04, 0xC1, 0x00, 0x00);

        /// <summary> KSCATEGORY_AUDIO_DEVICE </summary>
        public static readonly Guid KSAudioDevice = new Guid(0xFBF6F530, 0x07B9, 0x11D2, 0xA7, 0x1E, 0x00, 0x00, 0xF8, 0x00, 0x47, 0x88);

        /// <summary> CLSID_ActiveMovieCategories </summary>
        public static readonly Guid ActiveMovieCategories = new Guid(0xda4e3da0, 0xd07d, 0x11d0, 0xbd, 0x50, 0x00, 0xa0, 0xc9, 0x11, 0xce, 0x86);

        /// <summary> AM_KSCATEGORY_CAPTURE </summary>
        public static readonly Guid AMKSCapture = new Guid(0x65E8773D, 0x8F56, 0x11D0, 0xA3, 0xB9, 0x00, 0xA0, 0xC9, 0x22, 0x31, 0x96);

        /// <summary> AM_KSCATEGORY_RENDER </summary>
        public static readonly Guid AMKSRender = new Guid(0x65E8773E, 0x8F56, 0x11D0, 0xA3, 0xB9, 0x00, 0xA0, 0xC9, 0x22, 0x31, 0x96);

        /// <summary> AM_KSCATEGORY_DATACOMPRESSOR </summary>
        public static readonly Guid AMKSDataCompressor = new Guid(0x1E84C900, 0x7E70, 0x11D0, 0xA5, 0xD6, 0x28, 0xDB, 0x04, 0xC1, 0x00, 0x00);

        /// <summary> AM_KSCATEGORY_AUDIO </summary>
        public static readonly Guid AMKSAudio = new Guid(0x6994AD04, 0x93EF, 0x11D0, 0xA3, 0xCC, 0x00, 0xA0, 0xC9, 0x22, 0x31, 0x96);

        /// <summary> AM_KSCATEGORY_VIDEO </summary>
        public static readonly Guid AMKSVideo = new Guid(0x6994AD05, 0x93EF, 0x11D0, 0xA3, 0xCC, 0x00, 0xA0, 0xC9, 0x22, 0x31, 0x96);

        /// <summary> AM_KSCATEGORY_TVTUNER </summary>
        public static readonly Guid AMKSTVTuner = new Guid(0xa799a800, 0xa46d, 0x11d0, 0xa1, 0x8c, 0x00, 0xa0, 0x24, 0x01, 0xdc, 0xd4);

        /// <summary> AM_KSCATEGORY_CROSSBAR </summary>
        public static readonly Guid AMKSCrossbar = new Guid(0xa799a801, 0xa46d, 0x11d0, 0xa1, 0x8c, 0x00, 0xa0, 0x24, 0x01, 0xdc, 0xd4);

        /// <summary> AM_KSCATEGORY_TVAUDIO </summary>
        public static readonly Guid AMKSTVAudio = new Guid(0xa799a802, 0xa46d, 0x11d0, 0xa1, 0x8c, 0x00, 0xa0, 0x24, 0x01, 0xdc, 0xd4);

        /// <summary> AM_KSCATEGORY_VBICODEC </summary>
        public static readonly Guid AMKSVBICodec = new Guid(0x07dad660, 0x22f1, 0x11d1, 0xa9, 0xf4, 0x00, 0xc0, 0x4f, 0xbb, 0xde, 0x8f);

        /// <summary> AM_KSCATEGORY_SPLITTER </summary>
        public static readonly Guid AMKSSplitter = new Guid(0x0A4252A0, 0x7E70, 0x11D0, 0xA5, 0xD6, 0x28, 0xDB, 0x04, 0xC1, 0x00, 0x00);

        /// <summary> Not defined </summary>
        public static readonly Guid WDMStreamingEncoderDevices = new Guid(0x19689BF6, 0xC384, 0x48FD, 0xAD, 0x51, 0x90, 0xE5, 0x8C, 0x79, 0xF7, 0x0B);

        /// <summary> Not defined </summary>
        public static readonly Guid WDMStreamingMultiplexerDevices = new Guid(0x7A5DE1D3, 0x01A1, 0x452C, 0xB4, 0x81, 0x4F, 0xA2, 0xB9, 0x62, 0x71, 0xE8);

        /// <summary> Not defined </summary>
        public static readonly Guid LTMMVideoProcessors = new Guid(0xE526D606, 0x22E7, 0x494C, 0xB8, 0x1E, 0xAC, 0x0A, 0x94, 0xBF, 0xE6, 0x03);
    }

    [ComVisible(false)]
    public static class TimeFormat
    {
        // 00000000-0000-0000-0000-000000000000 TIME_FORMAT_NONE
        public static readonly Guid None = new Guid(0x0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        // 7b785570-8c82-11cf-bc0c-00aa00ac74f6 TIME_FORMAT_FRAME
        public static readonly Guid Frame = new Guid(0x7b785570, 0x8c82, 0x11cf, 0xbc, 0x0c, 0x00, 0xaa, 0x00, 0xac, 0x74, 0xf6);

        // 7b785571-8c82-11cf-bc0c-00aa00ac74f6 TIME_FORMAT_BYTE
        public static readonly Guid Byte = new Guid(0x7b785571, 0x8c82, 0x11cf, 0xbc, 0x0c, 0x00, 0xaa, 0x00, 0xac, 0x74, 0xf6);

        // 7b785572-8c82-11cf-bc0c-00aa00ac74f6 TIME_FORMAT_SAMPLE
        public static readonly Guid Sample = new Guid(0x7b785572, 0x8c82, 0x11cf, 0xbc, 0x0c, 0x00, 0xaa, 0x00, 0xac, 0x74, 0xf6);

        // 7b785573-8c82-11cf-bc0c-00aa00ac74f6 TIME_FORMAT_FIELD
        public static readonly Guid Field = new Guid(0x7b785573, 0x8c82, 0x11cf, 0xbc, 0x0c, 0x00, 0xaa, 0x00, 0xac, 0x74, 0xf6);

        // 7b785574-8c82-11cf-bc0c-00aa00ac74f6 TIME_FORMAT_MEDIA_TIME
        public static readonly Guid MediaTime = new Guid(0x7b785574, 0x8c82, 0x11cf, 0xbc, 0x0c, 0x00, 0xaa, 0x00, 0xac, 0x74, 0xf6);
    }

    [ComVisible(false)]
    public static class PropSetID
    {
        /// <summary> AMPROPSETID_Pin</summary>
        public static readonly Guid Pin = new Guid(0x9b00f101, 0x1567, 0x11d1, 0xb3, 0xf1, 0x00, 0xaa, 0x00, 0x37, 0x61, 0xc5);

        /// <summary> PROPSETID_VIDCAP_DROPPEDFRAMES </summary>
        public static readonly Guid DroppedFrames = new Guid(0xC6E13344, 0x30AC, 0x11D0, 0xA1, 0x8C, 0x00, 0xA0, 0xC9, 0x11, 0x89, 0x56);

        /// <summary> STATIC_ENCAPIPARAM_BITRATE </summary>
        public static readonly Guid ENCAPIPARAM_BitRate = new Guid(0x49cc4c43, 0xca83, 0x4ad4, 0xa9, 0xaf, 0xf3, 0x69, 0x6a, 0xf6, 0x66, 0xdf);

        /// <summary> STATIC_ENCAPIPARAM_PEAK_BITRATE </summary>
        public static readonly Guid ENCAPIPARAM_PeakBitRate = new Guid(0x703f16a9, 0x3d48, 0x44a1, 0xb0, 0x77, 0x01, 0x8d, 0xff, 0x91, 0x5d, 0x19);

        /// <summary> STATIC_ENCAPIPARAM_BITRATE_MODE </summary>
        public static readonly Guid ENCAPIPARAM_BitRateMode = new Guid(0xee5fb25c, 0xc713, 0x40d1, 0x9d, 0x58, 0xc0, 0xd7, 0x24, 0x1e, 0x25, 0x0f);

        /// <summary> ENCAPIPARAM_SAP_MODE </summary>
        public static readonly Guid ENCAPIPARAM_SAP_MODE = new Guid(0xc0171db, 0xfefc, 0x4af7, 0x99, 0x91, 0xa5, 0x65, 0x7c, 0x19, 0x1c, 0xd1);

        /// <summary> CODECAPI_AVDecMmcssClass </summary>
        public static readonly Guid CODECAPI_AVDecMmcssClass = new Guid(0xe0ad4828, 0xdf66, 0x4893, 0x9f, 0x33, 0x78, 0x8a, 0xa4, 0xec, 0x40, 0x82);

        /// <summary> STATIC_CODECAPI_CHANGELISTS </summary>
        public static readonly Guid CODECAPI_ChangeLists = new Guid(0x62b12acf, 0xf6b0, 0x47d9, 0x94, 0x56, 0x96, 0xf2, 0x2c, 0x4e, 0x0b, 0x9d);

        /// <summary> STATIC_CODECAPI_VIDEO_ENCODER </summary>
        public static readonly Guid CODECAPI_VideoEncoder = new Guid(0x7112e8e1, 0x3d03, 0x47ef, 0x8e, 0x60, 0x03, 0xf1, 0xcf, 0x53, 0x73, 0x01);

        /// <summary> STATIC_CODECAPI_AUDIO_ENCODER </summary>
        public static readonly Guid CODECAPI_AudioEncoder = new Guid(0xb9d19a3e, 0xf897, 0x429c, 0xbc, 0x46, 0x81, 0x38, 0xb7, 0x27, 0x2b, 0x2d);

        /// <summary> STATIC_CODECAPI_SETALLDEFAULTS </summary>
        public static readonly Guid CODECAPI_SetAllDefaults = new Guid(0x6c5e6a7c, 0xacf8, 0x4f55, 0xa9, 0x99, 0x1a, 0x62, 0x81, 0x09, 0x05, 0x1b);

        /// <summary> STATIC_CODECAPI_ALLSETTINGS </summary>
        public static readonly Guid CODECAPI_AllSettings = new Guid(0x6a577e92, 0x83e1, 0x4113, 0xad, 0xc2, 0x4f, 0xce, 0xc3, 0x2f, 0x83, 0xa1);

        /// <summary> STATIC_CODECAPI_SUPPORTSEVENTS </summary>
        public static readonly Guid CODECAPI_SupportsEvents = new Guid(0x0581af97, 0x7693, 0x4dbd, 0x9d, 0xca, 0x3f, 0x9e, 0xbd, 0x65, 0x85, 0xa1);

        /// <summary> STATIC_CODECAPI_CURRENTCHANGELIST </summary>
        public static readonly Guid CODECAPI_CurrentChangeList = new Guid(0x1cb14e83, 0x7d72, 0x4657, 0x83, 0xfd, 0x47, 0xa2, 0xc5, 0xb9, 0xd1, 0x3d);

    }

    [ComVisible(false)]
    public enum FilterState
    {
        Stopped = 0,
        Paused = 1,
        Running = 2,
        NotOpened = 3
    }

    [ComVisible(false)]
    public enum EventCode
    {
        None,
        Complete = 0x01,		// EC_COMPLETE
        UserAbort = 0x02,		// EC_USERABORT
        ErrorAbort = 0x03,		// EC_ERRORABORT
        Time = 0x04,		// EC_TIME
        Repaint = 0x05,		// EC_REPAINT
        StErrStopped = 0x06,		// EC_STREAM_ERROR_STOPPED
        StErrStPlaying = 0x07,		// EC_STREAM_ERROR_STILLPLAYING
        ErrorStPlaying = 0x08,		// EC_ERROR_STILLPLAYING
        PaletteChanged = 0x09,		// EC_PALETTE_CHANGED
        VideoSizeChanged = 0x0a,		// EC_VIDEO_SIZE_CHANGED
        QualityChange = 0x0b,		// EC_QUALITY_CHANGE
        ShuttingDown = 0x0c,		// EC_SHUTTING_DOWN
        ClockChanged = 0x0d,		// EC_CLOCK_CHANGED
        Paused = 0x0e,		// EC_PAUSED
        OpeningFile = 0x10,		// EC_OPENING_FILE
        BufferingData = 0x11,		// EC_BUFFERING_DATA
        FullScreenLost = 0x12,		// EC_FULLSCREEN_LOST
        Activate = 0x13,		// EC_ACTIVATE
        NeedRestart = 0x14,		// EC_NEED_RESTART
        WindowDestroyed = 0x15,		// EC_WINDOW_DESTROYED
        DisplayChanged = 0x16,		// EC_DISPLAY_CHANGED
        Starvation = 0x17,		// EC_STARVATION
        OleEvent = 0x18,		// EC_OLE_EVENT
        NotifyWindow = 0x19,		// EC_NOTIFY_WINDOW
        StreamControlStopped = 0x1A, // EC_STREAM_CONTROL_STOPPED
        StreamControlStarted = 0x1B, //EC_STREAM_CONTROL_STARTED
        DeviceLost = 0x1f,              // EC_DEVICE_LOST
        ProcessingLatency = 0x21,
        // EC_ ....

        // DVDevCod.h
        DvdDomChange = 0x101,	// EC_DVD_DOMAIN_CHANGE
        DvdTitleChange = 0x102,	// EC_DVD_TITLE_CHANGE
        DvdChaptStart = 0x103,	// EC_DVD_CHAPTER_START
        DvdAudioStChange = 0x104,	// EC_DVD_AUDIO_STREAM_CHANGE

        DvdSubPicStChange = 0x105,	// EC_DVD_SUBPICTURE_STREAM_CHANGE
        DvdAngleChange = 0x106,	// EC_DVD_ANGLE_CHANGE
        DvdButtonChange = 0x107,	// EC_DVD_BUTTON_CHANGE
        DvdValidUopsChange = 0x108,	// EC_DVD_VALID_UOPS_CHANGE
        DvdStillOn = 0x109,	// EC_DVD_STILL_ON
        DvdStillOff = 0x10a,	// EC_DVD_STILL_OFF
        DvdCurrentTime = 0x10b,	// EC_DVD_CURRENT_TIME
        DvdError = 0x10c,	// EC_DVD_ERROR
        DvdWarning = 0x10d,	// EC_DVD_WARNING
        DvdChaptAutoStop = 0x10e,	// EC_DVD_CHAPTER_AUTOSTOP
        DvdNoFpPgc = 0x10f,	// EC_DVD_NO_FP_PGC
        DvdPlaybRateChange = 0x110,	// EC_DVD_PLAYBACK_RATE_CHANGE
        DvdParentalLChange = 0x111,	// EC_DVD_PARENTAL_LEVEL_CHANGE
        DvdPlaybStopped = 0x112,	// EC_DVD_PLAYBACK_STOPPED
        DvdAnglesAvail = 0x113,	// EC_DVD_ANGLES_AVAILABLE
        DvdPeriodAStop = 0x114,	// EC_DVD_PLAYPERIOD_AUTOSTOP
        DvdButtonAActivated = 0x115,	// EC_DVD_BUTTON_AUTO_ACTIVATED
        DvdCmdStart = 0x116,	// EC_DVD_CMD_START
        DvdCmdEnd = 0x117,	// EC_DVD_CMD_END
        DvdDiscEjected = 0x118,	// EC_DVD_DISC_EJECTED
        DvdDiscInserted = 0x119,	// EC_DVD_DISC_INSERTED
        DvdCurrentHmsfTime = 0x11a,	// EC_DVD_CURRENT_HMSF_TIME
        DvdKaraokeMode = 0x11b		// EC_DVD_KARAOKE_MODE
    }

    [Flags]
    [ComVisible(false)]
    public enum AMRenderExFlags
    {
        None = 0,
        RenderToExistingRenderers = 1
    }

    [Flags]
    [ComVisible(false)]
    public enum AMInterlace
    {
        None = 0,
        IsInterlaced = 0x00000001,
        OneFieldPerSample = 0x00000002,
        Field1First = 0x00000004,
        Unused = 0x00000008,
        FieldPatternMask = 0x00000030,
        FieldPatField1Only = 0x00000000,
        FieldPatField2Only = 0x00000010,
        FieldPatBothRegular = 0x00000020,
        FieldPatBothIrregular = 0x00000030,
        DisplayModeMask = 0x000000c0,
        DisplayModeBobOnly = 0x00000000,
        DisplayModeWeaveOnly = 0x00000040,
        DisplayModeBobOrWeave = 0x00000080,
    }

    [Flags]
    [ComVisible(false)]
    public enum AMFileSinkFlags
    {
        None = 0,
        OverWrite = 0x00000001
    }

    [ComVisible(false)]
    public enum AMCopyProtect
    {
        None = 0,
        RestrictDuplication = 0x00000001
    }

    [Flags]
    [ComVisible(false)]
    public enum AMControl
    {
        None = 0,
        Used = 0x00000001,
        PadTo4x3 = 0x00000002,
        PadTo16x9 = 0x00000004,
    }

    [StructLayout(LayoutKind.Sequential)]
    [ComVisible(false)]
    public struct DsCAUUID
    {
        public int cElems;
        public IntPtr pElems;

        /// <summary>
        /// Perform a manual marshaling of pElems to retrieve an array of System.Guid.
        /// Assume this structure has been already filled by the ISpecifyPropertyPages.GetPages() method.
        /// </summary>
        /// <returns>A managed representation of pElems (cElems items)</returns>
        public Guid[] ToGuidArray()
        {
            Guid[] retval = new Guid[this.cElems];

            for (int i = 0; i < this.cElems; i++)
            {
                IntPtr ptr = new IntPtr(this.pElems.ToInt64() + (Marshal.SizeOf(typeof(Guid)) * i));
                retval[i] = (Guid)Marshal.PtrToStructure(ptr, typeof(Guid));
            }

            return retval;
        }
    }

    [ComVisible(false)]
    public enum AMPropertyPin
    {
        Category,
        Medium
    }

    [StructLayout(LayoutKind.Sequential)]
    [ComVisible(false)]
    public class DsRect
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        /// <summary>
        /// Empty contructor. Initialize all fields to 0
        /// </summary>
        public DsRect()
        {
            this.left = 0;
            this.top = 0;
            this.right = 0;
            this.bottom = 0;
        }

        /// <summary>
        /// A parametred constructor. Initialize fields with given values.
        /// </summary>
        /// <param name="left">the left value</param>
        /// <param name="top">the top value</param>
        /// <param name="right">the right value</param>
        /// <param name="bottom">the bottom value</param>
        public DsRect(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        /// <summary>
        /// A parametred constructor. Initialize fields with a given <see cref="System.Drawing.Rectangle"/>.
        /// </summary>
        /// <param name="rectangle">A <see cref="System.Drawing.Rectangle"/></param>
        /// <remarks>
        /// Warning, DsRect define a rectangle by defining two of his corners and <see cref="System.Drawing.Rectangle"/> define a rectangle with his upper/left corner, his width and his height.
        /// </remarks>
        public DsRect(Rectangle rectangle)
        {
            this.left = rectangle.Left;
            this.top = rectangle.Top;
            this.right = rectangle.Right;
            this.bottom = rectangle.Bottom;
        }

        /// <summary>
        /// Provide de string representation of this DsRect instance
        /// </summary>
        /// <returns>A string formated like this : [left, top - right, bottom]</returns>
        public override string ToString()
        {
            return string.Format("[{0}, {1} - {2}, {3}]", this.left, this.top, this.right, this.bottom);
        }

        public override int GetHashCode()
        {
            return this.left.GetHashCode() |
                this.top.GetHashCode() |
                this.right.GetHashCode() |
                this.bottom.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is DsRect)
            {
                DsRect cmp = (DsRect)obj;

                return right == cmp.right && bottom == cmp.bottom && left == cmp.left && top == cmp.top;
            }

            if (obj is Rectangle)
            {
                Rectangle cmp = (Rectangle)obj;

                return right == cmp.Right && bottom == cmp.Bottom && left == cmp.Left && top == cmp.Top;
            }

            return false;
        }

        /// <summary>
        /// Define implicit cast between DirectShowLib.DsRect and System.Drawing.Rectangle for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsRect.ToRectangle"/> for similar functionality.
        /// <code>
        ///   // Define a new Rectangle instance
        ///   Rectangle r = new Rectangle(0, 0, 100, 100);
        ///   // Do implicit cast between Rectangle and DsRect
        ///   DsRect dsR = r;
        ///
        ///   Console.WriteLine(dsR.ToString());
        /// </code>
        /// </summary>
        /// <param name="r">a DsRect to be cast</param>
        /// <returns>A casted System.Drawing.Rectangle</returns>
        public static implicit operator Rectangle(DsRect r)
        {
            return r.ToRectangle();
        }

        /// <summary>
        /// Define implicit cast between System.Drawing.Rectangle and DirectShowLib.DsRect for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsRect.FromRectangle"/> for similar functionality.
        /// <code>
        ///   // Define a new DsRect instance
        ///   DsRect dsR = new DsRect(0, 0, 100, 100);
        ///   // Do implicit cast between DsRect and Rectangle
        ///   Rectangle r = dsR;
        ///
        ///   Console.WriteLine(r.ToString());
        /// </code>
        /// </summary>
        /// <param name="r">A System.Drawing.Rectangle to be cast</param>
        /// <returns>A casted DsRect</returns>
        public static implicit operator DsRect(Rectangle r)
        {
            return new DsRect(r);
        }

        /// <summary>
        /// Get the System.Drawing.Rectangle equivalent to this DirectShowLib.DsRect instance.
        /// </summary>
        /// <returns>A System.Drawing.Rectangle</returns>
        public Rectangle ToRectangle()
        {
            return new Rectangle(this.left, this.top, (this.right - this.left), (this.bottom - this.top));
        }

        /// <summary>
        /// Get a new DirectShowLib.DsRect instance for a given <see cref="System.Drawing.Rectangle"/>
        /// </summary>
        /// <param name="r">The <see cref="System.Drawing.Rectangle"/> used to initialize this new DirectShowLib.DsGuid</param>
        /// <returns>A new instance of DirectShowLib.DsGuid</returns>
        public static DsRect FromRectangle(Rectangle r)
        {
            return new DsRect(r);
        }

        public bool IsEmpty()
        {
            return ToRectangle().IsEmpty;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    [ComVisible(false)]
    public class BitmapInfoHeader
    {
        public int Size;
        public int Width;
        public int Height;
        public short Planes;
        public short BitCount;
        public int Compression;
        public int ImageSize;
        public int XPelsPerMeter;
        public int YPelsPerMeter;
        public int ClrUsed;
        public int ClrImportant;

        public BitmapInfoHeader()
        {
            Size = Marshal.SizeOf(this.GetType());
            Width = 0;
            Height = 0;
            Planes = 0;
            BitCount = 0;
            Compression = 0;
            ImageSize = 0;
            XPelsPerMeter = 0;
            YPelsPerMeter = 0;
            ClrUsed = 0;
            ClrImportant = 0;
        }

        public int GetBitmapSize()
        {
            return Width * Math.Abs(Height) * (BitCount + BitCount % 8) / 8;
        }
    }

    /// <summary>
    /// From VIDEOINFOHEADER
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [ComVisible(false)]
    public class VideoInfoHeader
    {
        public DsRect SrcRect;
        public DsRect TargetRect;
        public int BitRate;
        public int BitErrorRate;
        public long AvgTimePerFrame;
        public BitmapInfoHeader BmiHeader;

        public VideoInfoHeader()
        {
            BitRate = 0;
            BitErrorRate = 0;
            AvgTimePerFrame = 0;
            SrcRect = new DsRect();
            TargetRect = new DsRect();
            BmiHeader = new BitmapInfoHeader();
        }
    }

    /// <summary>
    /// From VIDEOINFOHEADER2
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [ComVisible(false)]
    public class VideoInfoHeader2
    {
        public DsRect SrcRect;
        public DsRect TargetRect;
        public int BitRate;
        public int BitErrorRate;
        public long AvgTimePerFrame;
        public AMInterlace InterlaceFlags;
        public AMCopyProtect CopyProtectFlags;
        public int PictAspectRatioX;
        public int PictAspectRatioY;
        public AMControl ControlFlags;
        public int Reserved2;
        public BitmapInfoHeader BmiHeader;

        public VideoInfoHeader2()
        {
            InterlaceFlags = AMInterlace.None;
            CopyProtectFlags = AMCopyProtect.None;
            ControlFlags = AMControl.None;
            BitRate = 0;
            BitErrorRate = 0;
            AvgTimePerFrame = 0;
            PictAspectRatioX = 0;
            PictAspectRatioY = 0;
            Reserved2 = 0;
            SrcRect = new DsRect();
            TargetRect = new DsRect();
            BmiHeader = new BitmapInfoHeader();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    [ComVisible(false)]
    public class Mpeg2VideoInfo
    {
        public VideoInfoHeader2    hdr;
        public uint dwStartTimeCode;
        public uint cbSequenceHeader;
        public uint dwProfile;
        public uint dwLevel;
        public uint dwFlags;
        public byte[] dwSequenceHeader;

        public Mpeg2VideoInfo()
        {
            hdr = new VideoInfoHeader2();
            dwSequenceHeader = null;
            dwStartTimeCode = 0;
            cbSequenceHeader = 0;
            dwProfile = 0;
            dwLevel = 0;
            dwFlags = 0;
        }
    }

    /// <summary>
    /// From WAVEFORMATEX
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    [ComVisible(false)]
    public class WaveFormatEx
    {
        public ushort wFormatTag;
        public ushort nChannels;
        public int nSamplesPerSec;
        public int nAvgBytesPerSec;
        public ushort nBlockAlign;
        public ushort wBitsPerSample;
        public ushort cbSize;
    }

    [Flags]
    [ComVisible(false)]
    public enum SPEAKER : uint
    {
        SPEAKER_FRONT_LEFT = 0x1,
        SPEAKER_FRONT_RIGHT = 0x2,
        SPEAKER_FRONT_CENTER = 0x4,
        SPEAKER_LOW_FREQUENCY = 0x8,
        SPEAKER_BACK_LEFT = 0x10,
        SPEAKER_BACK_RIGHT = 0x20,
        SPEAKER_FRONT_LEFT_OF_CENTER = 0x40,
        SPEAKER_FRONT_RIGHT_OF_CENTER = 0x80,
        SPEAKER_BACK_CENTER = 0x100,
        SPEAKER_SIDE_LEFT = 0x200,
        SPEAKER_SIDE_RIGHT = 0x400,
        SPEAKER_TOP_CENTER = 0x800,
        SPEAKER_TOP_FRONT_LEFT = 0x1000,
        SPEAKER_TOP_FRONT_CENTER = 0x2000,
        SPEAKER_TOP_FRONT_RIGHT = 0x4000,
        SPEAKER_TOP_BACK_LEFT = 0x8000,
        SPEAKER_TOP_BACK_CENTER = 0x10000,
        SPEAKER_TOP_BACK_RIGHT = 0x20000,

        // Bit mask locations reserved for future use
        SPEAKER_RESERVED = 0x7FFC0000,

        // Used to specify that any possible permutation of speaker configurations
        SPEAKER_ALL = 0x80000000,
    }

    [StructLayout(LayoutKind.Sequential)]
    [ComVisible(false)]
    public class WaveFormatExtensible
    {
        public WaveFormatEx Format;
        public ushort wReserved;
        public SPEAKER dwChannelMask;
        public Guid SubFormat;

        public WaveFormatExtensible()
        {
            Format = new WaveFormatEx();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    [ComVisible(false)]
    public class AMMediaType
    {
        #region  Structure Variables

        public Guid majorType;
        public Guid subType;
        [MarshalAs(UnmanagedType.Bool)]
        public bool fixedSizeSamples;
        [MarshalAs(UnmanagedType.Bool)]
        public bool temporalCompression;
        public int sampleSize;
        public Guid formatType;
        public IntPtr unkPtr;
        public int formatSize;
        public IntPtr formatPtr;
        
        #endregion

        #region Constructor

        public AMMediaType()
        {
            unkPtr = IntPtr.Zero;
            formatPtr = IntPtr.Zero;
            Init();
        }

        public AMMediaType(AMMediaType mt)
            : this()
        {
            Set(mt);
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new AMMediaType(this);
        }

        #endregion

        #region Overriden Methods

        public override bool Equals(object obj)
        {
            if (obj is AMMediaType)
            {
                AMMediaType _dst = (obj as AMMediaType);
                if ((_dst.majorType != majorType))
                {
                    return false;
                }
                if (subType != _dst.subType)
                {
                    return false;
                }
                if (formatType != _dst.formatType)
                {
                    return false;
                }
                if (formatSize != _dst.formatSize)
                {
                    return false;
                }
                if (formatSize > 0)
                {
                    byte[] _source = new byte[formatSize];
                    byte[] _dest = new byte[formatSize];
                    Marshal.Copy(formatPtr, _source, 0, _source.Length);
                    Marshal.Copy(_dst.formatPtr, _dest, 0, _dest.Length);
                    for (int i = 0; i < _source.Length; i++)
                    {
                        if (_dest[i] != _source[i]) return false;
                    }
                }
                return true;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        #region Public Methods

        public void Set(AMMediaType mt)
        {
            Free();

            majorType = mt.majorType;
            subType = mt.subType;
            fixedSizeSamples = mt.fixedSizeSamples;
            temporalCompression = mt.temporalCompression;
            sampleSize = mt.sampleSize;
            formatType = mt.formatType;
            unkPtr = mt.unkPtr;
            formatPtr = IntPtr.Zero;
            formatSize = mt.formatSize;
            if (unkPtr != IntPtr.Zero)
            {
                Marshal.AddRef(unkPtr);
            }
            if (formatSize > 0)
            {
                SetFormat(mt.formatPtr, formatSize);
            }
        }

        public void Free()
        {
            FreeFormat();
            if (unkPtr != IntPtr.Zero)
            {
                Marshal.Release(unkPtr);
                unkPtr = IntPtr.Zero;
            }
        }

        public void Init()
        {
            Free();
            majorType = Guid.Empty;
            subType = Guid.Empty;
            fixedSizeSamples = true;
            temporalCompression = false;
            sampleSize = 0;
            formatType = Guid.Empty;
            unkPtr = IntPtr.Zero;
            formatPtr = IntPtr.Zero;
            formatSize = 0;
        }

        public void FreeFormat()
        {
            if (formatPtr != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(formatPtr);
                formatPtr = IntPtr.Zero;
            }
            formatSize = 0;
        }

        public bool IsValid()
        {
            return (majorType != null && majorType != Guid.Empty);
        }

        public bool IsPartiallySpecified()
        {
            if ((majorType == Guid.Empty) || (formatType == Guid.Empty))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool MatchesPartial(AMMediaType _dst)
        {
            if ((_dst.majorType != Guid.Empty) && (majorType != _dst.majorType))
            {
                return false;
            }
            if ((_dst.subType != Guid.Empty) && (subType != _dst.subType))
            {
                return false;
            }
            if (_dst.formatType != Guid.Empty)
            {
                if (formatType != _dst.formatType)
                {
                    return false;
                }
                if (formatSize != _dst.formatSize)
                {
                    return false;
                }
                if (formatSize > 0)
                {
                    byte[] _source = new byte[formatSize];
                    byte[] _dest = new byte[formatSize];
                    Marshal.Copy(formatPtr, _source, 0, _source.Length);
                    Marshal.Copy(_dst.formatPtr, _dest, 0, _dest.Length);
                    for (int i = 0; i < _source.Length; i++)
                    {
                        if (_dest[i] != _source[i]) return false;
                    }
                }

            }
            return true;
        }

        public void AllocFormat(int nSize)
        {
            FreeFormat();
            if (nSize > 0)
            {
                formatPtr = Marshal.AllocCoTaskMem(nSize);
                formatSize = nSize;
            }
        }

        public void SetFormat(byte[] _format)
        {
            SetFormat(_format, _format.Length);
        }

        public void SetFormat(byte[] _format, int nSize)
        {
            if (_format != null && nSize > 0)
            {
                IntPtr _ptr = Marshal.AllocCoTaskMem(nSize);
                Marshal.Copy(_format, 0, _ptr, nSize);
                SetFormat(_ptr, nSize);
                Marshal.FreeCoTaskMem(_ptr);
            }
        }

        public void SetFormat(IntPtr pFormat, int nSize)
        {
            AllocFormat(nSize);
            if (pFormat != IntPtr.Zero)
            {
                COMHelper.API.CopyMemory(formatPtr, pFormat, formatSize);
            }
        }

        public void SetFormat(WaveFormatEx wfx)
        {
            if (wfx != null)
            {
                int cb = Marshal.SizeOf(wfx);
                IntPtr _ptr = Marshal.AllocCoTaskMem(cb);
                try
                {
                    Marshal.StructureToPtr(wfx, _ptr, true);
                    SetFormat(_ptr, cb);
                    formatType = FormatType.WaveEx;
                }
                finally
                {
                    Marshal.FreeCoTaskMem(_ptr);
                }
            }
        }

        public void SetFormat(VideoInfoHeader vih)
        {
            if (vih != null)
            {
                int cb = Marshal.SizeOf(vih);
                IntPtr _ptr = Marshal.AllocCoTaskMem(cb);
                try
                {
                    Marshal.StructureToPtr(vih, _ptr, true);
                    SetFormat(_ptr, cb);
                    formatType = FormatType.VideoInfo;
                }
                finally
                {
                    Marshal.FreeCoTaskMem(_ptr);
                }
            }
        }

        public void SetFormat(VideoInfoHeader2 vih)
        {
            if (vih != null)
            {
                int cb = Marshal.SizeOf(vih);
                IntPtr _ptr = Marshal.AllocCoTaskMem(cb);
                try
                {
                    Marshal.StructureToPtr(vih, _ptr, true);
                    SetFormat(_ptr, cb);
                    formatType = FormatType.VideoInfo2;
                }
                finally
                {
                    Marshal.FreeCoTaskMem(_ptr);
                }
            }
        }

        public byte[] GetExtraData()
        {
            int _size = 0;
            if (formatPtr != IntPtr.Zero && formatSize != 0)
            {
                if (formatType == FormatType.VideoInfo)
                {
                    _size = formatSize - Marshal.SizeOf(typeof(VideoInfoHeader));
                }
                if (formatType == FormatType.VideoInfo2)
                {
                    _size = formatSize - Marshal.SizeOf(typeof(VideoInfoHeader2));
                }
                if (formatType == FormatType.WaveEx || formatType == FormatType.Mpeg2Audio || formatType == FormatType.DolbyAC3)
                {
                    _size = formatSize - Marshal.SizeOf(typeof(WaveFormatEx));
                }
                if (formatType == FormatType.Mpeg2Video)
                {
                    _size = formatSize - Marshal.SizeOf(typeof(VideoInfoHeader2)) - 18;
                }
            }
            byte[] _data = null;
            if (_size > 0)
            {
                _data = new byte[_size];
                IntPtr _ptr = new IntPtr(formatPtr.ToInt32() + (formatSize - _size));
                Marshal.Copy(_ptr, _data, 0, _size);
            }
            return _data;
        }

        public void AddFormatExtraData(IntPtr _data, int _size)
        {
            if (_size > 0 && _data != IntPtr.Zero)
            {
                byte[] _buffer = new byte[_size];
                Marshal.Copy(_data, _buffer, 0, _size);
                AddFormatExtraData(_buffer,_size);
            }
        }

        public void AddFormatExtraData(byte[] _data, int _size)
        {
            if (_data != null && _size > 0)
            {
                if (formatPtr != IntPtr.Zero && formatSize != 0)
                {
                    IntPtr _ptr = Marshal.AllocCoTaskMem(formatSize + _size);
                    COMHelper.API.CopyMemory(_ptr, formatPtr, formatSize);
                    Marshal.FreeCoTaskMem(formatPtr);
                    formatPtr = _ptr;
                    _ptr = new IntPtr(_ptr.ToInt32() + formatSize);
                    Marshal.Copy(_data, 0, _ptr, _size);
                    formatSize += _size;
                }
            }
        }

        public void AddFormatExtraData(byte[] _data)
        {
            AddFormatExtraData(_data, _data.Length);
        }

        public byte[] ToArray()
        {
            byte[] _data = null;
            if (formatPtr != IntPtr.Zero && formatSize != 0)
            {
                _data = new byte[formatSize];
                Marshal.Copy(formatPtr, _data, 0, formatSize);
            }
            return _data;
        }

        public long GetFrameRate()
        {
            if (majorType == MediaType.Video && formatPtr != IntPtr.Zero)
            {
                VideoInfoHeader _vih = (VideoInfoHeader)Marshal.PtrToStructure(formatPtr, typeof(VideoInfoHeader));
                return _vih.AvgTimePerFrame;
            }
            return 0;
        }

        #endregion

        #region Operators

        public static bool operator ==(AMMediaType _src, AMMediaType _dest)
        {
            if (System.Object.ReferenceEquals(_src, _dest))
            {
                return true;
            }
            if (((object)_src == null) || ((object)_dest == null))
            {
                return false;
            }
            return _src.Equals(_dest);

        }

        public static bool operator !=(AMMediaType _src, AMMediaType _dest)
        {
            return !(_src == _dest);
        }

        public static implicit operator WaveFormatEx(AMMediaType mt)
        {
            if (mt.formatPtr != IntPtr.Zero && mt.formatSize != 0 && mt.formatType == FormatType.WaveEx)
            {
                return (WaveFormatEx)Marshal.PtrToStructure(mt.formatPtr, typeof(WaveFormatEx));
            }
            return null;
        }

        public static implicit operator VideoInfoHeader(AMMediaType mt)
        {
            if (mt.formatPtr != IntPtr.Zero && mt.formatSize != 0)
            {
                return (VideoInfoHeader)Marshal.PtrToStructure(mt.formatPtr, typeof(VideoInfoHeader));
            }
            return null;
        }

        public static implicit operator VideoInfoHeader2(AMMediaType mt)
        {
            if (mt.formatPtr != IntPtr.Zero && mt.formatSize != 0 && (mt.formatType == FormatType.VideoInfo2 || mt.formatType == FormatType.Mpeg2Video))
            {
                return (VideoInfoHeader2)Marshal.PtrToStructure(mt.formatPtr, typeof(VideoInfoHeader2));
            }
            return null;
        }

        public static implicit operator BitmapInfoHeader(AMMediaType mt)
        {
            if (mt.formatType == FormatType.VideoInfo2 || mt.formatType == FormatType.Mpeg2Video)
            {
                VideoInfoHeader2 _vih = (VideoInfoHeader2)mt;
                if (_vih != null) return _vih.BmiHeader;
            }
            if (mt.formatType == FormatType.VideoInfo || mt.formatType == FormatType.MpegVideo)
            {
                VideoInfoHeader _vih = (VideoInfoHeader)mt;
                if (_vih != null) return _vih.BmiHeader;
            }
            return null;
        }

        #endregion

        #region Static Methods

        public static bool IsPartiallySpecified(AMMediaType mt)
        {
            if ((mt.majorType == Guid.Empty) || (mt.formatType == Guid.Empty))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool MatchesPartial(AMMediaType _src, AMMediaType _dst)
        {
            if ((_dst.majorType != Guid.Empty) && (_src.majorType != _dst.majorType))
            {
                return false;
            }
            if ((_dst.subType != Guid.Empty) && (_src.subType != _dst.subType))
            {
                return false;
            }
            if (_dst.formatType != Guid.Empty)
            {
                if (_src.formatType != _dst.formatType)
                {
                    return false;
                }
                if (_src.formatSize != _dst.formatSize)
                {
                    return false;
                }
                if (_src.formatSize > 0)
                {
                    byte[] _source = new byte[_src.formatSize];
                    byte[] _dest = new byte[_src.formatSize];
                    Marshal.Copy(_src.formatPtr, _source, 0, _source.Length);
                    Marshal.Copy(_dst.formatPtr, _dest, 0, _dest.Length);
                    for (int i = 0; i < _source.Length; i++)
                    {
                        if (_dest[i] != _source[i]) return false;
                    }
                }

            }
            return true;
        }

        public static void Init(ref AMMediaType mt)
        {
            if (((object)mt) == null)
            {
                mt = new AMMediaType();
            }
            else
            {
                Free(ref mt);
            }
            mt.majorType = Guid.Empty;
            mt.subType = Guid.Empty;
            mt.fixedSizeSamples = true;
            mt.temporalCompression = false;
            mt.sampleSize = 0;
            mt.formatType = Guid.Empty;
            mt.unkPtr = IntPtr.Zero;
            mt.formatPtr = IntPtr.Zero;
            mt.formatSize = 0;
        }

        public static void Copy(AMMediaType mt, ref AMMediaType _dest)
        {
            if (((object)_dest) == null)
            {
                _dest = new AMMediaType();
            }
            else
            {
                Free(ref _dest);
            }

            _dest.majorType = mt.majorType;
            _dest.subType = mt.subType;
            _dest.fixedSizeSamples = mt.fixedSizeSamples;
            _dest.temporalCompression = mt.temporalCompression;
            _dest.sampleSize = mt.sampleSize;
            _dest.formatType = mt.formatType;
            _dest.unkPtr = mt.unkPtr;
            _dest.formatPtr = IntPtr.Zero;
            _dest.formatSize = mt.formatSize;
            if (_dest.unkPtr != IntPtr.Zero)
            {
                Marshal.AddRef(_dest.unkPtr);
            }
            if (_dest.formatSize > 0)
            {
                _dest.formatPtr = Marshal.AllocCoTaskMem(_dest.formatSize);
                COMHelper.API.CopyMemory(_dest.formatPtr, mt.formatPtr, _dest.formatSize);
            }
        }

        public static void Free(ref AMMediaType mt)
        {
            if (mt != null)
            {
                FreeFormat(ref mt);
                if (mt.unkPtr != IntPtr.Zero)
                {
                    Marshal.Release(mt.unkPtr);
                    mt.unkPtr = IntPtr.Zero;
                }
            }
        }

        public static void AllocFormat(ref AMMediaType mt, int nSize)
        {
            FreeFormat(ref mt);
            if (mt != null && nSize > 0)
            {
                mt.formatPtr = Marshal.AllocCoTaskMem(nSize);
                mt.formatSize = nSize;
            }
        }

        public static void SetFormat(ref AMMediaType mt, IntPtr pFormat, int nSize)
        {
            AllocFormat(ref mt, nSize);
            if (mt != null && pFormat != IntPtr.Zero)
            {
                COMHelper.API.CopyMemory(mt.formatPtr, pFormat, mt.formatSize);
            }
        }

        public static void SetFormat(ref AMMediaType mt, ref WaveFormatEx wfx)
        {
            if (wfx != null)
            {
                int cb = Marshal.SizeOf(wfx);
                IntPtr _ptr = Marshal.AllocCoTaskMem(cb);
                try
                {
                    Marshal.StructureToPtr(wfx, _ptr, true);
                    SetFormat(ref mt, _ptr, cb);
                    if (mt != null)
                    {
                        mt.formatType = FormatType.WaveEx;
                    }
                }
                finally
                {
                    Marshal.FreeCoTaskMem(_ptr);
                }
            }
        }

        public static void SetFormat(ref AMMediaType mt, ref VideoInfoHeader vih)
        {
            if (vih != null)
            {
                int cb = Marshal.SizeOf(vih);
                IntPtr _ptr = Marshal.AllocCoTaskMem(cb);
                try
                {
                    Marshal.StructureToPtr(vih, _ptr, true);
                    SetFormat(ref mt, _ptr, cb);
                    if (mt != null)
                    {
                        mt.formatType = FormatType.VideoInfo;
                    }
                }
                finally
                {
                    Marshal.FreeCoTaskMem(_ptr);
                }
            }
        }

        public static void FreeFormat(ref AMMediaType mt)
        {
            if (mt != null)
            {
                if (mt.formatPtr != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(mt.formatPtr);
                    mt.formatPtr = IntPtr.Zero;
                }
                mt.formatSize = 0;
            }
        }

        public static bool IsValid(AMMediaType mt)
        {
            return (mt.majorType != null && mt.majorType != Guid.Empty);
        }

        public static bool AreEquals(AMMediaType _src, AMMediaType _dst)
        {
            if ((_dst.majorType != _src.majorType))
            {
                return false;
            }
            if (_src.subType != _dst.subType)
            {
                return false;
            }
            if (_src.formatType != _dst.formatType)
            {
                return false;
            }
            if (_src.formatSize != _dst.formatSize)
            {
                return false;
            }
            if (_src.formatSize > 0)
            {
                byte[] _source = new byte[_src.formatSize];
                byte[] _dest = new byte[_src.formatSize];
                Marshal.Copy(_src.formatPtr, _source, 0, _source.Length);
                Marshal.Copy(_dst.formatPtr, _dest, 0, _dest.Length);
                for (int i = 0; i < _source.Length; i++)
                {
                    if (_dest[i] != _source[i]) return false;
                }
            }
            return true;
        }

        #endregion
    }

    [ComVisible(false)]
    public enum PinDirection		// PIN_DIRECTION
    {
        Input,		// PINDIR_INPUT
        Output		// PINDIR_OUTPUT
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    [ComVisible(false)]
    public struct PinInfo
    {
        [MarshalAs(UnmanagedType.Interface)]
        public IBaseFilter filter;
        public PinDirection dir;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string name;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    [ComVisible(false)]
    public struct FilterInfo
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string achName;
        [MarshalAs(UnmanagedType.Interface)]
        public IFilterGraph pGraph;
    }

    [ComVisible(false)]
    [StructLayout(LayoutKind.Sequential)]
    public class AllocatorProperties
    {
        public int cBuffers;
        public int cbBuffer;
        public int cbAlign;
        public int cbPrefix;
    }

    [ComVisible(false)]
    public static class PinCategory
    {
        public static readonly Guid AnalogVideoIn = new Guid(0xfb6c4283, 0x353, 0x11d1, 0x90, 0x5f, 0, 0, 0xc0, 0xcc, 0x16, 0xba);
        public static readonly Guid Capture = new Guid(0xfb6c4281, 0x353, 0x11d1, 0x90, 0x5f, 0, 0, 0xc0, 0xcc, 0x16, 0xba);
        public static readonly Guid CC = new Guid(0xfb6c4289, 0x353, 0x11d1, 0x90, 0x5f, 0, 0, 0xc0, 0xcc, 0x16, 0xba);
        public static readonly Guid EDS = new Guid(0xfb6c4287, 0x353, 0x11d1, 0x90, 0x5f, 0, 0, 0xc0, 0xcc, 0x16, 0xba);
        public static readonly Guid NABTS = new Guid(0xfb6c4286, 0x353, 0x11d1, 0x90, 0x5f, 0, 0, 0xc0, 0xcc, 0x16, 0xba);
        public static readonly Guid Preview = new Guid(0xfb6c4282, 0x353, 0x11d1, 0x90, 0x5f, 0, 0, 0xc0, 0xcc, 0x16, 0xba);
        public static readonly Guid Still = new Guid(0xfb6c428a, 0x353, 0x11d1, 0x90, 0x5f, 0, 0, 0xc0, 0xcc, 0x16, 0xba);
        public static readonly Guid TeleText = new Guid(0xfb6c4288, 0x353, 0x11d1, 0x90, 0x5f, 0, 0, 0xc0, 0xcc, 0x16, 0xba);
        public static readonly Guid TimeCode = new Guid(0xfb6c428b, 0x353, 0x11d1, 0x90, 0x5f, 0, 0, 0xc0, 0xcc, 0x16, 0xba);
        public static readonly Guid VBI = new Guid(0xfb6c4284, 0x353, 0x11d1, 0x90, 0x5f, 0, 0, 0xc0, 0xcc, 0x16, 0xba);
        public static readonly Guid VideoPort = new Guid(0xfb6c4285, 0x353, 0x11d1, 0x90, 0x5f, 0, 0, 0xc0, 0xcc, 0x16, 0xba);
        public static readonly Guid VideoPortVBI = new Guid(0xfb6c428c, 0x353, 0x11d1, 0x90, 0x5f, 0, 0, 0xc0, 0xcc, 0x16, 0xba);
    }

    [ComVisible(false)]
    public enum KSPropertySupport
    {
        Get = 1,
        Set = 2
    }

    [ComVisible(false)]
    [StructLayout(LayoutKind.Sequential)]
    public class DsLong
    {
        private long Value;
        public DsLong(long Value)
        {
            this.Value = Value;
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public static implicit operator long(DsLong l)
        {
            return l.Value;
        }

        public static implicit operator DsLong(long l)
        {
            return new DsLong(l);
        }

        public long ToInt64()
        {
            return this.Value;
        }

        public static DsLong FromInt64(long l)
        {
            return new DsLong(l);
        }
    }

    [ComVisible(false)]
    [StructLayout(LayoutKind.Explicit)]
    public class DsGuid
    {
        public static readonly DsGuid Empty = Guid.Empty;
        [FieldOffset(0)]
        private Guid guid;

        public DsGuid()
        {
            this.guid = Guid.Empty;
        }

        public DsGuid(Guid g)
        {
            this.guid = g;
        }

        public DsGuid(string g)
        {
            this.guid = new Guid(g);
        }

        public static DsGuid FromGuid(Guid g)
        {
            return new DsGuid(g);
        }

        public override int GetHashCode()
        {
            return this.guid.GetHashCode();
        }

        public static implicit operator Guid(DsGuid g)
        {
            return g.guid;
        }

        public static implicit operator DsGuid(Guid g)
        {
            return new DsGuid(g);
        }

        public Guid ToGuid()
        {
            return this.guid;
        }

        public override string ToString()
        {
            return this.guid.ToString();
        }

        public string ToString(string format)
        {
            return this.guid.ToString(format);
        }
    }

    [Flags]
    [ComVisible(false)]
    public enum AMSeekingSeekingCapabilities
    {
        CanDoSegments = 0x80,
        CanGetCurrentPos = 8,
        CanGetDuration = 0x20,
        CanGetStopPos = 0x10,
        CanPlayBackwards = 0x40,
        CanSeekAbsolute = 1,
        CanSeekBackwards = 4,
        CanSeekForwards = 2,
        None = 0,
        Source = 0x100
    }

    [Flags]
    [ComVisible(false)]
    public enum AMSeekingSeekingFlags
    {
        AbsolutePositioning = 1,
        IncrementalPositioning = 3,
        NoFlush = 0x20,
        NoPositioning = 0,
        PositioningBitsMask = 3,
        RelativePositioning = 2,
        ReturnTime = 8,
        SeekToKeyFrame = 4,
        Segment = 0x10
    }

    [Flags]
    [ComVisible(false)]
    public enum Merit
    {
        None = 0,
        Preferred = 0x800000,
        Normal = 0x600000,
        Unlikely = 0x400000,
        DoNotUse = 0x200000,
        SWCompressor = 0x100000,
        HWCompressor = 0x100050
    }

    [ComVisible(false)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Quality
    {
        public QualityMessageType Type;
        public int Proportion;
        public long Late;
        public long TimeStamp;
    }

    /// <summary>
    /// From QualityMessageType
    /// </summary>
    [ComVisible(false)]
    public enum QualityMessageType
    {
        Famine,
        Flood
    }

    [Flags]
    [ComVisible(false)]
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
    [ComVisible(false)]
    public enum AMVideoFlag
    {
        FieldMask = 0x0003,
        InterleavedFrame = 0x0000,
        Field1 = 0x0001,
        Field2 = 0x0002,
        Field1First = 0x0004,
        Weave = 0x0008,
        IPBMask = 0x0030,
        ISample = 0x0000,
        PSample = 0x0010,
        BSample = 0x0020,
        RepeatField = 0x0040
    }

    /// <summary>
    /// From AM_SAMPLE_PROPERTY_FLAGS
    /// </summary>
    [Flags]
    [ComVisible(false)]
    public enum AMSamplePropertyFlags
    {
        SplicePoint = 0x01,
        PreRoll = 0x02,
        DataDiscontinuity = 0x04,
        TypeChanged = 0x08,
        TimeValid = 0x10,
        MediaTimeValid = 0x20,
        TimeDiscontinuity = 0x40,
        FlushOnPause = 0x80,
        StopValid = 0x100,
        EndOfStream = 0x200,
        Media = 0,
        Control = 1
    }

    [ComVisible(false)]
    public enum OABool
    {
        False = 0,
        True = -1 // bools in .NET use 1, not -1
    }

    [StructLayout(LayoutKind.Sequential)]
    [ComVisible(false)]
    public class RegPinMedium
    {
        public Guid clsMedium;
        public int dw1;
        public int dw2;
    }

    [StructLayout(LayoutKind.Sequential)]
    [ComVisible(false)]
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

    [StructLayout(LayoutKind.Sequential)]
    [ComVisible(false)]
    public class RegFilter2
    {
        public int dwVersion;
        public Merit dwMerit;
        public int cPins;
        public IntPtr rgPins;
    }

    [StructLayout(LayoutKind.Sequential)]
    [ComVisible(false)]
    public class DsSize
    {
        public Int32 cx;
        public Int32 cy;

        public DsSize(Int32 cx, Int32 cy) { this.cx = cx; this.cy = cy; }
        public DsSize() : this(0, 0) { }
    }

    [Flags]
    [ComVisible(false)]
    public enum PropStatus : int
    {
        Dirty = 0x1,
        Validate = 0x2,
        Clean = 0x4
    }

    [StructLayout(LayoutKind.Sequential)]
    [ComVisible(false)]
    public class PropPageInfo
    {
        public int cb;
        public IntPtr pszTitle;
        public DsSize size;
        public IntPtr pszDocString;
        public IntPtr pszHelpFile;
        [MarshalAs(UnmanagedType.U4)]
        public int dwHelpContext;

        public PropPageInfo()
        {
            size = new DsSize();
            cb = Marshal.SizeOf(this);
            pszTitle = IntPtr.Zero;
            pszDocString = IntPtr.Zero;
            pszHelpFile = IntPtr.Zero;
            dwHelpContext = 0;
        }
    }

    [ComVisible(false)]
    public enum SWOptions
    {
        Hide = 0,
        ShowNormal = 1,
        Normal = ShowNormal,
        ShowMinimized = 2,
        ShowMaximized = 3,
        Maximize = ShowMaximized,
        ShowNoActivate = 4,
        Show = 5,
        Minimize = 6,
        ShowMinNoActive = 7,
        ShowNA = 8,
        Restore = 9,
        ShowDefault = 10,
        ForceMinimize = 11,
        Max = ForceMinimize
    }

    [Flags]
    [ComVisible(false)]
    public enum AMPushSourceFlags
    {
        None = 0,
        InternalRM = 0x00000001,
        NotLive = 0x00000002,
        PrivateClock = 0x00000004,
        UseStreamClock = 0x00010000,
        UseClockChain = 0x00020000,
    }

    [Flags]
    [ComVisible(false)]
    public enum AMFilterMiscFlags
    {
        None = 0,
        IsRenderer = 0x00000001,
        IsSource = 0x00000002
    }

    [Flags]
    [ComVisible(false)]
    public enum AMStreamInfoFlags
    {
        None = 0x00000000,
        StartDefined = 0x00000001,
        StopDefined = 0x00000002,
        Discarding = 0x00000004,
        StopSendExtra = 0x00000010
    }

    [StructLayout(LayoutKind.Sequential)]
    [ComVisible(false)]
    public struct AMStreamInfo
    {
        public long tStart;
        public long tStop;
        public int dwStartCookie;
        public int dwStopCookie;
        public AMStreamInfoFlags dwFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    [ComVisible(false)]
    public class VideoStreamConfigCaps
    {
        public Guid guid;
        public AnalogVideoStandard VideoStandard;
        public Size InputSize;
        public Size MinCroppingSize;
        public Size MaxCroppingSize;
        public int CropGranularityX;
        public int CropGranularityY;
        public int CropAlignX;
        public int CropAlignY;
        public Size MinOutputSize;
        public Size MaxOutputSize;
        public int OutputGranularityX;
        public int OutputGranularityY;
        public int StretchTapsX;
        public int StretchTapsY;
        public int ShrinkTapsX;
        public int ShrinkTapsY;
        public long MinFrameInterval;
        public long MaxFrameInterval;
        public int MinBitsPerSecond;
        public int MaxBitsPerSecond;

        public VideoStreamConfigCaps()
        {
            InputSize = new Size();
            MinCroppingSize = new Size();
            MaxCroppingSize = new Size();
            MinOutputSize = new Size();
            MaxOutputSize = new Size();
        }
    }

    [Flags]
    [ComVisible(false)]
    public enum AnalogVideoStandard
    {
        None = 0x00000000,
        NTSC_M = 0x00000001,
        NTSC_M_J = 0x00000002,
        NTSC_433 = 0x00000004,
        PAL_B = 0x00000010,
        PAL_D = 0x00000020,
        PAL_G = 0x00000040,
        PAL_H = 0x00000080,
        PAL_I = 0x00000100,
        PAL_M = 0x00000200,
        PAL_N = 0x00000400,
        PAL_60 = 0x00000800,
        SECAM_B = 0x00001000,
        SECAM_D = 0x00002000,
        SECAM_G = 0x00004000,
        SECAM_H = 0x00008000,
        SECAM_K = 0x00010000,
        SECAM_K1 = 0x00020000,
        SECAM_L = 0x00040000,
        SECAM_L1 = 0x00080000,
        PAL_N_COMBO = 0x00100000,

        NTSCMask = 0x00000007,
        PALMask = 0x00100FF0,
        SECAMMask = 0x000FF000
    }

    #region Interfaces

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
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

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("b79bb0b0-33c1-11d1-abe1-00a0c905f375"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFilterMapper2
    {
        [PreserveSig]
        int CreateCategory(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid clsidCategory,
            [In] Merit dwCategoryMerit,
            [In, MarshalAs(UnmanagedType.LPWStr)] string Description
            );

        [PreserveSig]
        int UnregisterFilter(
            [In, MarshalAs(UnmanagedType.LPStruct)] DsGuid clsidCategory,
            [In, MarshalAs(UnmanagedType.LPWStr)] string szInstance,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid Filter
            );

        [PreserveSig]
        int RegisterFilter(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid clsidFilter,
            [In, MarshalAs(UnmanagedType.LPWStr)] string Name,
            [In] IntPtr ppMoniker,
            [In, MarshalAs(UnmanagedType.LPStruct)] DsGuid pclsidCategory,
            [In, MarshalAs(UnmanagedType.LPWStr)] string szInstance,
            [In] IntPtr prf2
        );

        [PreserveSig]
        int EnumMatchingFilters(
        [Out] out IEnumMoniker ppEnum,
         [In] int dwFlags,
         [In, MarshalAs(UnmanagedType.Bool)] bool bExactMatch,
         [In] Merit dwMerit,
         [In, MarshalAs(UnmanagedType.Bool)] bool bInputNeeded,
         [In] int cInputTypes,
         [In, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct)] Guid[] pInputTypes, // GUID *
         [In] RegPinMedium pMedIn,
         [In] DsGuid pPinCategoryIn,
         [In, MarshalAs(UnmanagedType.Bool)] bool bRender,
         [In, MarshalAs(UnmanagedType.Bool)] bool bOutputNeeded,
         [In] int cOutputTypes,
         [In, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct)] Guid[] pOutputTypes, // GUID *
         [In] RegPinMedium pMedOut,
         [In] DsGuid pPinCategoryOut
        );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("56a868a9-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IGraphBuilder : IFilterGraph
    {
        #region IFilterGraph Methods

        [PreserveSig]
        new int AddFilter(
            [In] IBaseFilter pFilter,
            [In, MarshalAs(UnmanagedType.LPWStr)] string pName
            );

        [PreserveSig]
        new int RemoveFilter([In] IBaseFilter pFilter);

        [PreserveSig]
        new int EnumFilters([Out] out IEnumFilters ppEnum);

        [PreserveSig]
        new int FindFilterByName(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pName,
            [Out] out IBaseFilter ppFilter
            );

        [PreserveSig]
        new int ConnectDirect(
            [In] IPin ppinOut,
            [In] IPin ppinIn,
            [In, MarshalAs(UnmanagedType.LPStruct)]
            AMMediaType pmt
            );

        [PreserveSig]
        new int Reconnect([In] IPin ppin);

        [PreserveSig]
        new int Disconnect([In] IPin ppin);

        [PreserveSig]
        new int SetDefaultSyncSource();

        #endregion

        [PreserveSig]
        int Connect(
            [In] IPin ppinOut,
            [In] IPin ppinIn
            );

        [PreserveSig]
        int Render([In] IPin ppinOut);

        [PreserveSig]
        int RenderFile(
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpcwstrFile,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpcwstrPlayList
            );

        [PreserveSig]
        int AddSourceFilter(
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpcwstrFileName,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpcwstrFilterName,
            [Out] out IBaseFilter ppFilter
            );

        [PreserveSig]
        int SetLogFile(IntPtr hFile); // DWORD_PTR

        [PreserveSig]
        int Abort();

        [PreserveSig]
        int ShouldOperationContinue();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("36b73882-c2c8-11cf-8b46-00805f6cef60"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFilterGraph2 : IGraphBuilder
    {
        #region IFilterGraph Methods

        [PreserveSig]
        new int AddFilter(
            [In] IBaseFilter pFilter,
            [In, MarshalAs(UnmanagedType.LPWStr)] string pName
            );

        [PreserveSig]
        new int RemoveFilter([In] IBaseFilter pFilter);

        [PreserveSig]
        new int EnumFilters([Out] out IEnumFilters ppEnum);

        [PreserveSig]
        new int FindFilterByName(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pName,
            [Out] out IBaseFilter ppFilter
            );

        [PreserveSig]
        new int ConnectDirect(
            [In] IPin ppinOut,
            [In] IPin ppinIn,
            [In, MarshalAs(UnmanagedType.LPStruct)]
            AMMediaType pmt
            );

        [PreserveSig]
        new int Reconnect([In] IPin ppin);

        [PreserveSig]
        new int Disconnect([In] IPin ppin);

        [PreserveSig]
        new int SetDefaultSyncSource();

        #endregion

        #region IGraphBuilder Method

        [PreserveSig]
        new int Connect(
            [In] IPin ppinOut,
            [In] IPin ppinIn
            );

        [PreserveSig]
        new int Render([In] IPin ppinOut);

        [PreserveSig]
        new int RenderFile(
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpcwstrFile,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpcwstrPlayList
            );

        [PreserveSig]
        new int AddSourceFilter(
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpcwstrFileName,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpcwstrFilterName,
            [Out] out IBaseFilter ppFilter
            );

        [PreserveSig]
        new int SetLogFile(IntPtr hFile); // DWORD_PTR

        [PreserveSig]
        new int Abort();

        [PreserveSig]
        new int ShouldOperationContinue();

        #endregion

        [PreserveSig]
        int AddSourceFilterForMoniker(
                [In] IMoniker pMoniker,
                [In] IBindCtx pCtx,
                [In, MarshalAs(UnmanagedType.LPWStr)] string lpcwstrFilterName,
                [Out] out IBaseFilter ppFilter
             );

        [PreserveSig]
        int ReconnectEx(
            [In] IPin ppin,
            [In] AMMediaType pmt
            );

        [PreserveSig]
        int RenderEx(
            [In] IPin pPinOut,
            [In] AMRenderExFlags dwFlags,
            [In] IntPtr pvContext // DWORD *
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("36b73883-c2c8-11cf-8b46-00805f6cef60"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ISeekingPassThru
    {
        int Init(
            [In, MarshalAs(UnmanagedType.Bool)] bool bSupportRendering,
            [In] IPin pPin
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("56a868a2-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMediaEventSink
    {
        [PreserveSig]
        int Notify(
            [In] EventCode evCode,
            [In] IntPtr EventParam1,
            [In] IntPtr EventParam2
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("B196B28B-BAB4-101A-B69C-00AA00341D07"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ISpecifyPropertyPages
    {
        [PreserveSig]
        int GetPages(out DsCAUUID pPages);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("56a86893-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumFilters
    {
        [PreserveSig]
        int Next(
            [In] int cFilters,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IBaseFilter[] ppFilter,
            [In] IntPtr pcFetched
            );

        [PreserveSig]
        int Skip([In] int cFilters);

        [PreserveSig]
        int Reset();

        [PreserveSig]
        int Clone([Out] out IEnumFilters ppEnum);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
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
        int Reconnect([In] IPin ppin);

        [PreserveSig]
        int Disconnect([In] IPin ppin);

        [PreserveSig]
        int SetDefaultSyncSource();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("0000010c-0000-0000-C000-000000000046"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPersist
    {
        [PreserveSig]
        int GetClassID([Out] out Guid pClassID);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("56a868a5-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IQualityControl
    {
        [PreserveSig]
        int Notify(
            [In] IntPtr pSelf,
            [In] Quality q
            );

        [PreserveSig]
        int SetSink([In] IntPtr piqc);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("56a86891-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPin
    {
        [PreserveSig]
        int Connect(
            [In] IntPtr pReceivePin,
            [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt
            );

        [PreserveSig]
        int ReceiveConnection(
            [In] IntPtr pReceivePin,
            [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt
            );

        [PreserveSig]
        int Disconnect();

        [PreserveSig]
        int ConnectedTo(
            [Out] out IntPtr ppPin);

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
        int EnumMediaTypes([Out] out IntPtr ppEnum);

        [PreserveSig]
        int QueryInternalConnections(
            [Out] IntPtr ppPins,
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

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
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

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("56a86899-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMediaFilter : IPersist
    {
        #region IPersist Methods

        [PreserveSig]
        new int GetClassID([Out] out Guid pClassID);

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
        int SetSyncSource([In] IntPtr pClock);

        [PreserveSig]
        int GetSyncSource([Out] out IntPtr pClock);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
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
        new int SetSyncSource([In] IntPtr pClock);

        [PreserveSig]
        new int GetSyncSource([Out] out IntPtr pClock);

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
            [In] IntPtr pGraph,
            [In, MarshalAs(UnmanagedType.LPWStr)] string pName
            );

        [PreserveSig]
        int QueryVendorInfo([Out, MarshalAs(UnmanagedType.LPWStr)] out string pVendorInfo);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("56a86892-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumPins
    {
        [PreserveSig]
        int Next(
            [In] int cPins,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IPin[] ppPins,
            [In] IntPtr pcFetched
            );

        [PreserveSig]
        int Skip([In] int cPins);

        [PreserveSig]
        int Reset();

        [PreserveSig]
        int Clone([Out] out IEnumPins ppEnum);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
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

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("89c31040-846b-11ce-97d3-00aa0055595a"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumMediaTypes
    {
        [PreserveSig]
        int Next(
            [In] int cMediaTypes,
            [In] IntPtr ppMediaTypes,
            [In] IntPtr pcFetched
            );

        [PreserveSig]
        int Skip([In] int cMediaTypes);

        [PreserveSig]
        int Reset();

        [PreserveSig]
        int Clone([Out] out IntPtr ppEnum);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
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

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("36b73884-c2c8-11cf-8b46-00805f6cef60")
    , InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
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

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
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
            [Out] out IntPtr ppBuffer,
            [In] IntPtr pStartTime,
            [In] IntPtr pEndTime,
            [In, MarshalAs(UnmanagedType.U4)] int dwFlags
            );

        [PreserveSig]
        int ReleaseBuffer(
            [In] IntPtr pBuffer
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("379a0cf0-c1de-11d2-abf5-00a0c905f375"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMemAllocatorCallbackTemp : IMemAllocator
    {
        #region IMemAllocator Methods

        [PreserveSig]
        new int SetProperties(
            [In, MarshalAs(UnmanagedType.LPStruct)] AllocatorProperties pRequest,
            [Out, MarshalAs(UnmanagedType.LPStruct)] AllocatorProperties pActual
            );

        [PreserveSig]
        new int GetProperties(
            [Out, MarshalAs(UnmanagedType.LPStruct)] AllocatorProperties pProps
            );

        [PreserveSig]
        new int Commit();

        [PreserveSig]
        new int Decommit();

        [PreserveSig]
        new int GetBuffer(
            [Out] out IntPtr ppBuffer,
            [In] IntPtr pStartTime,
            [In] IntPtr pEndTime,
            [In, MarshalAs(UnmanagedType.U4)] int dwFlags
            );

        [PreserveSig]
        new int ReleaseBuffer(
            [In] IntPtr pBuffer
            );

        #endregion

        [PreserveSig]
        int SetNotify([In] IMemAllocatorNotifyCallbackTemp pNotify);

        [PreserveSig]
        int GetFreeCount([Out] out int plBuffersFree);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("92980b30-c1de-11d2-abf5-00a0c905f375"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMemAllocatorNotifyCallbackTemp
    {
        [PreserveSig]
        int NotifyRelease();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("56a8689d-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMemInputPin
    {
        [PreserveSig]
        int GetAllocator([Out] out IntPtr ppAllocator);

        [PreserveSig]
        int NotifyAllocator(
            [In] IntPtr pAllocator,
            [In, MarshalAs(UnmanagedType.Bool)] bool bReadOnly
            );

        [PreserveSig]
        int GetAllocatorRequirements([Out] AllocatorProperties pProps);

        [PreserveSig]
        int Receive(IntPtr pSample);

        [PreserveSig]
        int ReceiveMultiple(
            [In] IntPtr pSamples,
            [In] int nSamples,
            [Out] out int nSamplesProcessed
            );

        [PreserveSig]
        int ReceiveCanBlock();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("56a868a6-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFileSourceFilter
    {
        [PreserveSig]
        int Load(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
            [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt
            );

        [PreserveSig]
        int GetCurFile(
            [Out, MarshalAs(UnmanagedType.LPWStr)] out string pszFileName,
            [Out, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("56a868aa-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAsyncReader
    {
        [PreserveSig]
        int RequestAllocator(
            [In] IntPtr pPreferred, // IMemAllocator *
            [In, MarshalAs(UnmanagedType.LPStruct)] AllocatorProperties pProps,
            [Out] out IntPtr ppActual // IMemAllocator **
            );

        [PreserveSig]
        int Request(
            [In] IntPtr pSample, // IMediaSample*
            [In] IntPtr dwUser
            );

        [PreserveSig]
        int WaitForNext(
            [In] int dwTimeout,
            [Out] out IntPtr ppSample, // IMediaSample**
            [Out] out IntPtr pdwUser
            );

        [PreserveSig]
        int SyncReadAligned(
            [In] IntPtr pSample // IMediaSample*
            );

        [PreserveSig]
        int SyncRead(
            [In] long llPosition,
            [In] int lLength,
            [Out] IntPtr pBuffer // BYTE *
            );

        [PreserveSig]
        int Length(
            [Out] out long pTotal,
            [Out] out long pAvailable
            );

        [PreserveSig]
        int BeginFlush();

        [PreserveSig]
        int EndFlush();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("8E1C39A1-DE53-11cf-AA63-0080C744528D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMOpenProgress
    {
        [PreserveSig]
        int QueryProgress(
            [Out] out long pllTotal,
            [Out] out long pllCurrent
            );

        [PreserveSig]
        int AbortOperation();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("B196B28C-BAB4-101A-B69C-00AA00341D07"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPropertyPageSite
    {
        [PreserveSig]
        int OnStatusChange([In] PropStatus dwFlags);

        [PreserveSig]
        int GetLocaleID([Out] out int LocaleID);

        [PreserveSig]
        int GetPageContainer([Out, MarshalAs(UnmanagedType.IUnknown)] out object pUnk);

        [PreserveSig]
        int TranslateAccelerator(IntPtr pMsg);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("B196B28D-BAB4-101A-B69C-00AA00341D07"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPropertyPage
    {
        [PreserveSig]
        int SetPageSite([In] IntPtr pPageSite);

        [PreserveSig]
        int Activate(
            [In] IntPtr hWndParent,
            [In, MarshalAs(UnmanagedType.LPStruct)] DsRect pRect,
            [In, MarshalAs(UnmanagedType.Bool)] bool bModal);

        [PreserveSig]
        int Deactivate();

        [PreserveSig]
        int GetPageInfo([In, Out, MarshalAs(UnmanagedType.LPStruct)] PropPageInfo pPageInfo);

        [PreserveSig]
        int SetObjects(
            [In] uint cObjects,
            [In] IntPtr ppUnk);

        [PreserveSig]
        int Show([In] SWOptions nCmdShow);

        [PreserveSig]
        int Move([In, MarshalAs(UnmanagedType.LPStruct)] DsRect pRect);

        [PreserveSig]
        int IsPageDirty();

        [PreserveSig]
        int Apply();

        [PreserveSig]
        int Help([In, MarshalAs(UnmanagedType.LPWStr)] string sHelpDir);

        [PreserveSig]
        int TranslateAccelerator(IntPtr pMsg);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("00000109-0000-0000-C000-000000000046"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPersistStream : IPersist
    {
        #region IPersist Members

        [PreserveSig]
        new int GetClassID([Out] out Guid pClassID);

        #endregion

        [PreserveSig]
        int IsDirty();
        
        [PreserveSig]
        int Load(IntPtr pStm);
        
        [PreserveSig]
        int Save(IntPtr pStm, [MarshalAs(UnmanagedType.Bool)] bool fClearDirty);
        
        [PreserveSig]
        int GetSizeMax(out long pcbSize);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("29840822-5B84-11D0-BD3B-00A0C911CE86"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ICreateDevEnum
    {
        [PreserveSig]
        int CreateClassEnumerator([In] ref Guid type, [Out] out IEnumMoniker enumMoniker, [In] int flags);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("C6E13340-30AC-11d0-A18C-00A0C9118956"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMStreamConfig
    {
        [PreserveSig]
        int SetFormat([In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);

        [PreserveSig]
        int GetFormat([Out] out AMMediaType pmt);

        [PreserveSig]
        int GetNumberOfCapabilities(IntPtr piCount, IntPtr piSize);

        [PreserveSig]
        int GetStreamCaps(
            [In] int iIndex,
            [In, Out] IntPtr ppmt,
            [In] IntPtr pSCC
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("31EFAC30-515C-11d0-A9AA-00AA0061BE93"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IKsPropertySet
    {
        [PreserveSig]
        int Set(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidPropSet,
            [In] int dwPropID,
            [In] IntPtr pInstanceData,
            [In] int cbInstanceData,
            [In] IntPtr pPropData,
            [In] int cbPropData
            );

        [PreserveSig]
        int Get(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidPropSet,
            [In] int dwPropID,
            [In] IntPtr pInstanceData,
            [In] int cbInstanceData,
            [In, Out] IntPtr pPropData,
            [In] int cbPropData,
            [Out] out int pcbReturned
            );

        [PreserveSig]
        int QuerySupported(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidPropSet,
            [In] int dwPropID,
            [Out] out KSPropertySupport pTypeSupport
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("3127CA40-446E-11CE-8135-00AA004BB851"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IErrorLog
    {
        [PreserveSig]
        int AddError(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszPropName,
            [In] System.Runtime.InteropServices.ComTypes.EXCEPINFO pExcepInfo);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("55272A00-42CB-11CE-8135-00AA004BB851"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPropertyBag
    {
        [PreserveSig]
        int Read(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszPropName,
            [Out, MarshalAs(UnmanagedType.Struct)] out object pVar,
            [In] IErrorLog pErrorLog
            );

        [PreserveSig]
        int Write(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszPropName,
            [In, MarshalAs(UnmanagedType.Struct)] ref object pVar
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("a2104830-7c70-11cf-8bce-00aa00a3f1a6"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFileSinkFilter
    {
        [PreserveSig]
        int SetFileName(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
            [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt
            );

        [PreserveSig]
        int GetCurFile(
            [Out, MarshalAs(UnmanagedType.LPWStr)] out string pszFileName,
            [Out, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("00855B90-CE1B-11d0-BD4F-00A0C911CE86"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFileSinkFilter2 : IFileSinkFilter
    {
        #region IFileSinkFilter Methods

        [PreserveSig]
        new int SetFileName(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
            [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt
            );

        [PreserveSig]
        new int GetCurFile(
            [Out, MarshalAs(UnmanagedType.LPWStr)] out string pszFileName,
            [Out, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt
            );

        #endregion

        [PreserveSig]
        int SetMode([In] AMFileSinkFlags dwFlags);

        [PreserveSig]
        int GetMode([Out] out AMFileSinkFlags dwFlags);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("2dd74950-a890-11d1-abe8-00a0c905f375"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMFilterMiscFlags
    {
        [PreserveSig]
        int GetMiscFlags();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("6B652FFF-11FE-4FCE-92AD-0266B5D7C78F"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ISampleGrabber
    {
        [PreserveSig]
        int SetOneShot([In, MarshalAs(UnmanagedType.Bool)] bool oneShot);

        [PreserveSig]
        int SetMediaType([In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType mediaType);

        [PreserveSig]
        int GetConnectedMediaType([Out, MarshalAs(UnmanagedType.LPStruct)] AMMediaType mediaType);

        [PreserveSig]
        int SetBufferSamples([In, MarshalAs(UnmanagedType.Bool)] bool bufferThem);

        [PreserveSig]
        int GetCurrentBuffer(ref int bufferSize, IntPtr buffer);

        [PreserveSig]
        int GetCurrentSample(IntPtr sample);

        [PreserveSig]
        int SetCallback(ISampleGrabberCB callback, int whichMethodToCallback);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("0579154A-2B53-4994-B0D0-E773148EFF85"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ISampleGrabberCB
    {
        [PreserveSig]
        int SampleCB(double sampleTime, IntPtr sample);

        [PreserveSig]
        int BufferCB(double sampleTime, IntPtr buffer, int bufferLen);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("93E5A4E0-2D50-11d2-ABFA-00A0C9C6E38D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ICaptureGraphBuilder2
    {
        [PreserveSig]
        int SetFiltergraph([In] IGraphBuilder pfg);

        [PreserveSig]
        int GetFiltergraph([Out] out IGraphBuilder ppfg);

        [PreserveSig]
        int SetOutputFileName(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid pType,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpstrFile,
            [Out] out IBaseFilter ppbf,
            [Out] out IFileSinkFilter ppSink
            );

        [PreserveSig]
        int FindInterface(
            [In, MarshalAs(UnmanagedType.LPStruct)] DsGuid pCategory,
            [In, MarshalAs(UnmanagedType.LPStruct)] DsGuid pType,
            [In] IBaseFilter pbf,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [Out, MarshalAs(UnmanagedType.IUnknown)] out object ppint
            );

        [PreserveSig]
        int RenderStream(
            [In, MarshalAs(UnmanagedType.LPStruct)] DsGuid PinCategory,
            [In, MarshalAs(UnmanagedType.LPStruct)] DsGuid MediaType,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pSource,
            [In] IBaseFilter pfCompressor,
            [In] IBaseFilter pfRenderer
            );

        [PreserveSig]
        int ControlStream(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid PinCategory,
            [In, MarshalAs(UnmanagedType.LPStruct)] DsGuid MediaType,
            [In, MarshalAs(UnmanagedType.Interface)] IBaseFilter pFilter,
            [In] DsLong pstart,
            [In] DsLong pstop,
            [In] short wStartCookie,
            [In] short wStopCookie
            );

        [PreserveSig]
        int AllocCapFile(
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpstrFile,
            [In] long dwlSize
            );

        [PreserveSig]
        int CopyCaptureFile(
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpwstrOld,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpwstrNew,
            [In, MarshalAs(UnmanagedType.Bool)] bool fAllowEscAbort,
            [In] IAMCopyCaptureFileProgress pFilter
            );

        [PreserveSig]
        int FindPin(
            [In, MarshalAs(UnmanagedType.IUnknown)] object pSource,
            [In] PinDirection pindir,
            [In, MarshalAs(UnmanagedType.LPStruct)] DsGuid PinCategory,
            [In, MarshalAs(UnmanagedType.LPStruct)] DsGuid MediaType,
            [In, MarshalAs(UnmanagedType.Bool)] bool fUnconnected,
            [In] int ZeroBasedIndex,
            [Out, MarshalAs(UnmanagedType.Interface)] out IPin ppPin
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("670d1d20-a068-11d0-b3f0-00aa003761c5"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMCopyCaptureFileProgress
    {
        [PreserveSig]
        int Progress(int iProgress);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
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
        int GetState(int msTimeout, out FilterState pfs);

        [PreserveSig]
        int RenderFile(string strFilename);

        [PreserveSig]
        int AddSourceFilter(
            [In]											string strFilename,
            [Out, MarshalAs(UnmanagedType.IDispatch)]	out object ppUnk);

        [PreserveSig]
        int get_FilterCollection(
            [Out, MarshalAs(UnmanagedType.IDispatch)]	out object ppUnk);

        [PreserveSig]
        int get_RegFilterCollection(
            [Out, MarshalAs(UnmanagedType.IDispatch)]	out object ppUnk);

        [PreserveSig]
        int StopWhenReady();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("56a868c0-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IMediaEventEx
    {
        #region "IMediaEvent Methods"
        [PreserveSig]
        int GetEventHandle(out IntPtr hEvent);

        [PreserveSig]
        int GetEvent(out EventCode lEventCode, out int lParam1, out int lParam2, int msTimeout);

        [PreserveSig]
        int WaitForCompletion(int msTimeout, [Out] out int pEvCode);

        [PreserveSig]
        int CancelDefaultHandling(int lEvCode);

        [PreserveSig]
        int RestoreDefaultHandling(int lEvCode);

        [PreserveSig]
        int FreeEventParams(EventCode lEvCode, int lParam1, int lParam2);
        #endregion


        [PreserveSig]
        int SetNotifyWindow(IntPtr hwnd, int lMsg, IntPtr lInstanceData);

        [PreserveSig]
        int SetNotifyFlags(int lNoNotifyFlags);

        [PreserveSig]
        int GetNotifyFlags(out int lplNoNotifyFlags);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("56a868b4-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IVideoWindow
    {
        [PreserveSig]
        int put_Caption(string caption);
        [PreserveSig]
        int get_Caption([Out] out string caption);

        [PreserveSig]
        int put_WindowStyle(int windowStyle);
        [PreserveSig]
        int get_WindowStyle(out int windowStyle);

        [PreserveSig]
        int put_WindowStyleEx(int windowStyleEx);
        [PreserveSig]
        int get_WindowStyleEx(out int windowStyleEx);

        [PreserveSig]
        int put_AutoShow(int autoShow);
        [PreserveSig]
        int get_AutoShow(out int autoShow);

        [PreserveSig]
        int put_WindowState(int windowState);
        [PreserveSig]
        int get_WindowState(out int windowState);

        [PreserveSig]
        int put_BackgroundPalette(int backgroundPalette);
        [PreserveSig]
        int get_BackgroundPalette(out int backgroundPalette);

        [PreserveSig]
        int put_Visible(int visible);
        [PreserveSig]
        int get_Visible(out int visible);

        [PreserveSig]
        int put_Left(int left);
        [PreserveSig]
        int get_Left(out int left);

        [PreserveSig]
        int put_Width(int width);
        [PreserveSig]
        int get_Width(out int width);

        [PreserveSig]
        int put_Top(int top);
        [PreserveSig]
        int get_Top(out int top);

        [PreserveSig]
        int put_Height(int height);
        [PreserveSig]
        int get_Height(out int height);

        [PreserveSig]
        int put_Owner(IntPtr owner);
        [PreserveSig]
        int get_Owner(out IntPtr owner);

        [PreserveSig]
        int put_MessageDrain(IntPtr drain);
        [PreserveSig]
        int get_MessageDrain(out IntPtr drain);

        [PreserveSig]
        int get_BorderColor(out int color);
        [PreserveSig]
        int put_BorderColor(int color);

        [PreserveSig]
        int get_FullScreenMode(out int fullScreenMode);
        [PreserveSig]
        int put_FullScreenMode(int fullScreenMode);

        [PreserveSig]
        int SetWindowForeground(int focus);

        [PreserveSig]
        int NotifyOwnerMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

        [PreserveSig]
        int SetWindowPosition(int left, int top, int width, int height);

        [PreserveSig]
        int GetWindowPosition(out int left, out int top, out int width, out int height);

        [PreserveSig]
        int GetMinIdealImageSize(out int width, out int height);

        [PreserveSig]
        int GetMaxIdealImageSize(out int width, out int height);

        [PreserveSig]
        int GetRestorePosition(out int left, out int top, out int width, out int height);

        [PreserveSig]
        int HideCursor(int hideCursor);

        [PreserveSig]
        int IsCursorHidden(out int hideCursor);

    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("56a868b5-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IBasicVideo
    {
        [PreserveSig]
        int get_AvgTimePerFrame(out double pAvgTimePerFrame);
        [PreserveSig]
        int get_BitRate(out int pBitRate);
        [PreserveSig]
        int get_BitErrorRate(out int pBitRate);
        [PreserveSig]
        int get_VideoWidth(out int pVideoWidth);
        [PreserveSig]
        int get_VideoHeight(out int pVideoHeight);
        [PreserveSig]
        int put_SourceLeft([In] int SourceLeft);
        [PreserveSig]
        int get_SourceLeft(out int pSourceLeft);
        [PreserveSig]
        int put_SourceWidth([In] int SourceWidth);
        [PreserveSig]
        int get_SourceWidth(out int pSourceWidth);
        [PreserveSig]
        int put_SourceTop([In] int SourceTop);
        [PreserveSig]
        int get_SourceTop(out int pSourceTop);
        [PreserveSig]
        int put_SourceHeight([In] int SourceHeight);
        [PreserveSig]
        int get_SourceHeight(out int pSourceHeight);
        [PreserveSig]
        int put_DestinationLeft([In] int DestinationLeft);
        [PreserveSig]
        int get_DestinationLeft(out int pDestinationLeft);
        [PreserveSig]
        int put_DestinationWidth([In] int DestinationWidth);
        [PreserveSig]
        int get_DestinationWidth(out int pDestinationWidth);
        [PreserveSig]
        int put_DestinationTop([In] int DestinationTop);
        [PreserveSig]
        int get_DestinationTop(out int pDestinationTop);
        [PreserveSig]
        int put_DestinationHeight([In] int DestinationHeight);
        [PreserveSig]
        int get_DestinationHeight(out int pDestinationHeight);
        [PreserveSig]
        int SetSourcePosition([In] int left, [In] int top, [In] int width, [In] int height);
        [PreserveSig]
        int GetSourcePosition(out int left, out int top, out int width, out int height);
        [PreserveSig]
        int SetDefaultSourcePosition();
        [PreserveSig]
        int SetDestinationPosition([In] int left, [In] int top, [In] int width, [In] int height);
        [PreserveSig]
        int GetDestinationPosition(out int left, out int top, out int width, out int height);
        [PreserveSig]
        int SetDefaultDestinationPosition();
        [PreserveSig]
        int GetVideoSize(out int pWidth, out int pHeight);
        [PreserveSig]
        int GetVideoPaletteEntries([In] int StartIndex, [In] int Entries, out int pRetrieved, out int[] pPalette);
        [PreserveSig]
        int GetCurrentImage([In, Out] ref int pBufferSize, [Out] IntPtr pDIBImage);
        [PreserveSig]
        int IsUsingDefaultSource();
        [PreserveSig]
        int IsUsingDefaultDestination();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("329bb360-f6ea-11d1-9038-00a0c9697298"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IBasicVideo2
    {
        [PreserveSig]
        int AvgTimePerFrame(out double pAvgTimePerFrame);

        [PreserveSig]
        int BitRate(out int pBitRate);

        [PreserveSig]
        int BitErrorRate(out int pBitRate);

        [PreserveSig]
        int VideoWidth(out int pVideoWidth);

        [PreserveSig]
        int VideoHeight(out int pVideoHeight);

        [PreserveSig]
        int put_SourceLeft(int SourceLeft);

        [PreserveSig]
        int get_SourceLeft(out int pSourceLeft);

        [PreserveSig]
        int put_SourceWidth(int SourceWidth);

        [PreserveSig]
        int get_SourceWidth(out int pSourceWidth);

        [PreserveSig]
        int put_SourceTop(int SourceTop);

        [PreserveSig]
        int get_SourceTop(out int pSourceTop);

        [PreserveSig]
        int put_SourceHeight(int SourceHeight);

        [PreserveSig]
        int get_SourceHeight(out int pSourceHeight);

        [PreserveSig]
        int put_DestinationLeft(int DestinationLeft);

        [PreserveSig]
        int get_DestinationLeft(out int pDestinationLeft);

        [PreserveSig]
        int put_DestinationWidth(int DestinationWidth);
        [PreserveSig]
        int get_DestinationWidth(out int pDestinationWidth);

        [PreserveSig]
        int put_DestinationTop(int DestinationTop);

        [PreserveSig]
        int get_DestinationTop(out int pDestinationTop);

        [PreserveSig]
        int put_DestinationHeight(int DestinationHeight);

        [PreserveSig]
        int get_DestinationHeight(out int pDestinationHeight);

        [PreserveSig]
        int SetSourcePosition(int left, int top, int width, int height);

        [PreserveSig]
        int GetSourcePosition(out int left, out int top, out int width, out int height);

        [PreserveSig]
        int SetDefaultSourcePosition();

        [PreserveSig]
        int SetDestinationPosition(int left, int top, int width, int height);

        [PreserveSig]
        int GetDestinationPosition(out int left, out int top, out int width, out int height);

        [PreserveSig]
        int SetDefaultDestinationPosition();

        [PreserveSig]
        int GetVideoSize(out int pWidth, out int pHeight);

        [PreserveSig]
        int GetVideoPaletteEntries(int StartIndex, int Entries, out int pRetrieved, IntPtr pPalette);

        [PreserveSig]
        int GetCurrentImage(ref int pBufferSize, IntPtr pDIBImage);

        [PreserveSig]
        int IsUsingDefaultSource();
        [PreserveSig]
        int IsUsingDefaultDestination();

        [PreserveSig]
        int GetPreferredAspectRatio(out int plAspectX, out int plAspectY);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("56a868b3-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IBasicAudio
    {
        [PreserveSig]
        int put_Volume(int lVolume);
        [PreserveSig]
        int get_Volume(out int plVolume);

        [PreserveSig]
        int put_Balance(int lBalance);
        [PreserveSig]
        int get_Balance(out int plBalance);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("e46a9787-2b71-444d-a4b5-1fab7b708d6a"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVideoFrameStep
    {
        [PreserveSig]
        int Step(
            [In] int dwFrames,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pStepObject
            );

        [PreserveSig]
        int CanStep(
            [In] int bMultiple,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pStepObject
            );

        [PreserveSig]
        int CancelStep();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("36b73881-c2c8-11cf-8b46-00805f6cef60"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMStreamControl
    {
        [PreserveSig]
        int StartAt(
            [In] DsLong ptStart,
            [In] int dwCookie
            );

        [PreserveSig]
        int StopAt(
            [In] DsLong ptStop,
            [In, MarshalAs(UnmanagedType.Bool)] bool bSendExtra,
            [In] int dwCookie
            );

        [PreserveSig]
        int GetInfo([Out] out AMStreamInfo pInfo);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("62EA93BA-EC62-11d2-B770-00C04FB6BD3D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMLatency
    {
        [PreserveSig]
        int GetLatency(out long prtLatency);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("F185FE76-E64E-11d2-B76E-00C04FB6BD3D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMPushSource : IAMLatency
    {
        [PreserveSig]
        new int GetLatency(out long prtLatency);

        [PreserveSig]
        int GetPushSourceFlags([Out] out AMPushSourceFlags pFlags);

        [PreserveSig]
        int SetPushSourceFlags([In] AMPushSourceFlags Flags);

        [PreserveSig]
        int SetStreamOffset([In] long rtOffset);

        [PreserveSig]
        int GetStreamOffset([Out] out long prtOffset);

        [PreserveSig]
        int GetMaxStreamOffset([Out] out long prtMaxOffset);

        [PreserveSig]
        int SetMaxStreamOffset([In] long rtMaxOffset);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("56ED71A0-AF5F-11D0-B3F0-00AA003761C5"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMBufferNegotiation
    {
        [PreserveSig]
        int SuggestAllocatorProperties([In] AllocatorProperties pprop);

        [PreserveSig]
        int GetAllocatorProperties([Out] AllocatorProperties pprop);
    }

    public enum AMSTREAMSELECTINFOFLAGS: uint
    {
        ENABLED = 0x1,
        EXCLUSIVE = 0x2
    }

    public enum AMSTREAMSELECTENABLEFLAGS : uint
    {
        DISABLE = 0x0,
        ENABLED = 0x1,
        ENABLEALL = 0x2
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("c1960960-17f5-11d1-abe1-00a0c905f375"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMStreamSelect
    {
        [PreserveSig]
        int Count(
            [Out] out int pcStreams
            );
        
        [PreserveSig]
        int Info( 
            [In] int lIndex,
            [In, Out] IntPtr ppmt, // AMMediaType
            [In, Out] IntPtr pdwFlags, // DWORD * AMSTREAMSELECTINFOFLAGS
            [In, Out] IntPtr plcid, // DWORD *
            [In, Out] IntPtr pdwGroup, // DWORD *
            [In, Out] IntPtr ppszName, // WCHAR **
            [In, Out] IntPtr ppObject, // IUnknown * 
            [In, Out] IntPtr ppUnk); // IUnknown * 
        
        [PreserveSig]
        int Enable( 
            [In] int lIndex,
            [In] AMSTREAMSELECTENABLEFLAGS dwFlags);
    }

    #endregion

    #region Classes

    [ComVisible(false)]
    [ComImport, Guid("060AF76C-68DD-11d0-8FC1-00C04FD9189D")]
    public class SeekingPassThru
    {
    }

    [ComVisible(false)]
    [ComImport, Guid("CDA42200-BD88-11d0-BD4E-00A0C911CE86")]
    public class FilterMapper2
    {
    }

    [ComVisible(false)]
    [ComImport, Guid("1e651cc0-b199-11d0-8212-00c04fc32c45")]
    public class MemoryAllocator
    {
    }

    [ComVisible(false)]
    [ComImport, Guid("e436ebb3-524f-11ce-9f53-0020af0ba770")]
    public class FilterGraph
    {
    }

    [ComVisible(false)]
    [ComImport, Guid("BF87B6E1-8C27-11d0-B3F0-00AA003761C5")]
    public class CaptureGraphBuilder2
    {
    }

    #endregion

    #region Wrapper Classes

    [ComVisible(false)]
    public class IMediaSampleImpl : VTableInterface, IMediaSample
    {
        #region Delgates

        private delegate int GetPointerProc(
            IntPtr pUnk,
            out IntPtr ppBuffer
            );

        private delegate int GetSizeProc(
            IntPtr pUnk
            );

        private delegate int SetSizeProc(
            IntPtr pUnk,
            int nSize
            );

        private delegate int GetTimeProc(
            IntPtr pUnk,
            out long pTimeStart, 
            out long pTimeEnd
            );

        private delegate int SetTimeProc(
            IntPtr pUnk,
            [In, MarshalAs(UnmanagedType.LPStruct)] DsLong pTimeStart,
            [In, MarshalAs(UnmanagedType.LPStruct)] DsLong pTimeEnd
            );

        private delegate int SetBoolProc(
            IntPtr pUnk,
            [MarshalAs(UnmanagedType.Bool)] bool bValue
            );

        private delegate int GetMediaTypeProc(
            IntPtr pUnk,
            [Out, MarshalAs(UnmanagedType.LPStruct)] out AMMediaType ppMediaType
            );

        private delegate int SetMediaTypeProc(
            IntPtr pUnk,
            [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pMediaType
            );

        #endregion

        #region Constructor

        public IMediaSampleImpl(IntPtr pMediaSample)
            : base(pMediaSample, false)
        {

        }

        #endregion

        #region IMediaSample Members

        public int GetPointer(out IntPtr ppBuffer)
        {
            ppBuffer = IntPtr.Zero;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            GetPointerProc _Proc = GetProcDelegate<GetPointerProc>(3);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        out ppBuffer
                        );
        }

        public int GetSize()
        {
            if (m_pUnknown == IntPtr.Zero) return 0;

            GetSizeProc _Proc = GetProcDelegate<GetSizeProc>(4);

            if (_Proc == null) return 0;

            return _Proc(
                        m_pUnknown
                        );
        }

        public int GetTime(out long pTimeStart, out long pTimeEnd)
        {
            pTimeStart = 0;
            pTimeEnd = 0;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            GetTimeProc _Proc = GetProcDelegate<GetTimeProc>(5);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        out pTimeStart, out pTimeEnd
                        );
        }

        public int SetTime(DsLong pTimeStart, DsLong pTimeEnd)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            SetTimeProc _Proc = GetProcDelegate<SetTimeProc>(6);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        pTimeStart, pTimeEnd
                        );
        }

        public int IsSyncPoint()
        {
            if (m_pUnknown == IntPtr.Zero) return S_FALSE;

            GetSizeProc _Proc = GetProcDelegate<GetSizeProc>(7);

            if (_Proc == null) return S_FALSE;

            return _Proc(
                        m_pUnknown
                        );
        }

        public int SetSyncPoint(bool bIsSyncPoint)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            SetBoolProc _Proc = GetProcDelegate<SetBoolProc>(8);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        bIsSyncPoint
                        );
        }

        public int IsPreroll()
        {
            if (m_pUnknown == IntPtr.Zero) return S_FALSE;

            GetSizeProc _Proc = GetProcDelegate<GetSizeProc>(9);

            if (_Proc == null) return S_FALSE;

            return _Proc(
                        m_pUnknown
                        );
        }

        public int SetPreroll(bool bIsPreroll)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            SetBoolProc _Proc = GetProcDelegate<SetBoolProc>(10);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        bIsPreroll
                        );
        }

        public int GetActualDataLength()
        {
            if (m_pUnknown == IntPtr.Zero) return 0;

            GetSizeProc _Proc = GetProcDelegate<GetSizeProc>(11);

            if (_Proc == null) return 0;

            return _Proc(
                        m_pUnknown
                        );
        }

        public int SetActualDataLength(int len)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            SetSizeProc _Proc = GetProcDelegate<SetSizeProc>(12);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        len
                        );
        }

        public int GetMediaType(out AMMediaType ppMediaType)
        {
            ppMediaType = null;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            GetMediaTypeProc _Proc = GetProcDelegate<GetMediaTypeProc>(13);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        out ppMediaType
                        );
        }

        public int SetMediaType(AMMediaType pMediaType)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            SetMediaTypeProc _Proc = GetProcDelegate<SetMediaTypeProc>(14);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        pMediaType
                        );
        }

        public int IsDiscontinuity()
        {
            if (m_pUnknown == IntPtr.Zero) return S_FALSE;

            GetSizeProc _Proc = GetProcDelegate<GetSizeProc>(15);

            if (_Proc == null) return S_FALSE;

            return _Proc(
                        m_pUnknown
                        );
        }

        public int SetDiscontinuity(bool bDiscontinuity)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            SetBoolProc _Proc = GetProcDelegate<SetBoolProc>(16);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        bDiscontinuity
                        );
        }

        public int GetMediaTime(out long pTimeStart, out long pTimeEnd)
        {
            pTimeStart = 0;
            pTimeEnd = 0;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            GetTimeProc _Proc = GetProcDelegate<GetTimeProc>(17);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        out pTimeStart, out pTimeEnd
                        );
        }

        public int SetMediaTime(DsLong pTimeStart, DsLong pTimeEnd)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            SetTimeProc _Proc = GetProcDelegate<SetTimeProc>(18);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        pTimeStart, pTimeEnd
                        );
        }

        #endregion
    }

    [ComVisible(false)]
    public class IMediaSample2Impl : IMediaSampleImpl, IMediaSample2
    {
        #region Delegates

        private delegate int PropertiesProc(
            IntPtr pUnk,
            int cbProperties, IntPtr pbProperties
            );

        #endregion

        #region Constructor

        public IMediaSample2Impl(IntPtr pMediaSample)
            : base(pMediaSample)
        {

        }

        #endregion

        #region IMediaSample2 Members

        public int GetProperties(int cbProperties, IntPtr pbProperties)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            PropertiesProc _Proc = GetProcDelegate<PropertiesProc>(19);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        cbProperties, pbProperties
                        );
        }

        public int SetProperties(int cbProperties, IntPtr pbProperties)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            PropertiesProc _Proc = GetProcDelegate<PropertiesProc>(20);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        cbProperties, pbProperties
                        );
        }

        #endregion
    }

    [ComVisible(false)]
    public class IMemInputPinImpl : VTableInterface, IMemInputPin
    {
        #region Delegates

        private delegate int GetAllocatorProc(
            IntPtr pUnk,
            out IntPtr ppAllocator
            );

        private delegate int NotifyAllocatorProc(
            IntPtr pUnk,
            [In] IntPtr pAllocator,
            [In, MarshalAs(UnmanagedType.Bool)] bool bReadOnly
            );

        private delegate int GetAllocatorRequirementsProc(
            IntPtr pUnk,
            [Out] AllocatorProperties pProps);

        private delegate int ReceiveProc(
            IntPtr pUnk,
            IntPtr pSample);

        private delegate int ReceiveMultipleProc(
            IntPtr pUnk,
            [In] IntPtr pSamples,
            [In] int nSamples,
            [Out] out int nSamplesProcessed
            );

        private delegate int ReceiveCanBlockProc(
            IntPtr pUnk
            );

        #endregion

        #region Constructor

        public IMemInputPinImpl(IntPtr pMemInputPin)
            : base(pMemInputPin,false)
        {

        }
        #endregion

        #region IMemInputPin Members

        public int GetAllocator(out IntPtr ppAllocator)
        {
            ppAllocator = IntPtr.Zero;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            GetAllocatorProc _Proc = GetProcDelegate<GetAllocatorProc>(3);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        out ppAllocator
                        );
        }

        public int NotifyAllocator(IntPtr pAllocator, bool bReadOnly)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            NotifyAllocatorProc _Proc = GetProcDelegate<NotifyAllocatorProc>(4);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        pAllocator, bReadOnly
                        );
        }

        public int GetAllocatorRequirements(AllocatorProperties pProps)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            GetAllocatorRequirementsProc _Proc = GetProcDelegate<GetAllocatorRequirementsProc>(5);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        pProps
                        );
        }

        public int Receive(IntPtr pSample)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            ReceiveProc _Proc = GetProcDelegate<ReceiveProc>(6);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        pSample
                        );
        }

        public int ReceiveMultiple(IntPtr pSamples, int nSamples, out int nSamplesProcessed)
        {
            nSamplesProcessed = 0;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            ReceiveMultipleProc _Proc = GetProcDelegate<ReceiveMultipleProc>(7);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        pSamples, 
                        nSamples, 
                        out nSamplesProcessed
                        );
        }

        public int ReceiveCanBlock()
        {
            if (m_pUnknown == IntPtr.Zero) return S_OK;

            ReceiveCanBlockProc _Proc = GetProcDelegate<ReceiveCanBlockProc>(8);

            if (_Proc == null) return S_OK;

            return _Proc(
                        m_pUnknown
                        );
        }

        #endregion
    }

    [ComVisible(false)]
    public class IMemAllocatorImpl : VTableInterface, IMemAllocator
    {
        #region Delegates

        private delegate int SetPropertiesProc(
            IntPtr pUnk,
            [In, MarshalAs(UnmanagedType.LPStruct)] AllocatorProperties pRequest,
            [Out, MarshalAs(UnmanagedType.LPStruct)] AllocatorProperties pActual
            );

        private delegate int GetPropertiesProc(
            IntPtr pUnk,
            [Out, MarshalAs(UnmanagedType.LPStruct)] AllocatorProperties pProps
            );

        private delegate int CommitProc(
            IntPtr pUnk
            );

        private delegate int DecommitProc(
            IntPtr pUnk
            );

        private delegate int GetBufferProc(
            IntPtr pUnk,
            [Out] out IntPtr ppBuffer,
            [In] IntPtr pStartTime,
            [In] IntPtr pEndTime,
            [In, MarshalAs(UnmanagedType.U4)] int dwFlags
            );

        private delegate int ReleaseBufferProc(
            IntPtr pUnk,
            [In] IntPtr pBuffer
            );

        #endregion

        #region Constructor

        public IMemAllocatorImpl(IntPtr pMemAllocator)
            : base(pMemAllocator, false)
        {

        }

        #endregion

        #region IMemAllocator Members

        public int SetProperties(AllocatorProperties pRequest, AllocatorProperties pActual)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            SetPropertiesProc _Proc = GetProcDelegate<SetPropertiesProc>(3);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        pRequest, pActual
                        );
        }

        public int GetProperties(AllocatorProperties pProps)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            GetPropertiesProc _Proc = GetProcDelegate<GetPropertiesProc>(4);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        pProps
                        );
        }

        public int Commit()
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            CommitProc _Proc = GetProcDelegate<CommitProc>(5);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown
                        );
        }

        public int Decommit()
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            DecommitProc _Proc = GetProcDelegate<DecommitProc>(6);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown
                        );
        }

        public int GetBuffer(out IntPtr ppBuffer, IntPtr pStartTime, IntPtr pEndTime, int dwFlags)
        {
            ppBuffer = IntPtr.Zero;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            GetBufferProc _Proc = GetProcDelegate<GetBufferProc>(7);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        out ppBuffer, pStartTime, pEndTime, dwFlags
                        );
        }

        public int ReleaseBuffer(IntPtr pBuffer)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            ReleaseBufferProc _Proc = GetProcDelegate<ReleaseBufferProc>(8);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        pBuffer
                        );
        }

        #endregion
    }

    [ComVisible(false)]
    public class IEnumMediaTypesImpl : VTableInterface, IEnumMediaTypes
    {
        #region Delegates

        private delegate int NextProc(
            IntPtr pUnk,
            int cMediaTypes, IntPtr ppMediaTypes, IntPtr pcFetched
            );

        private delegate int SkipProc(
            IntPtr pUnk,
            int cMediaTypes
            );

        private delegate int ResetProc(
            IntPtr pUnk
            );

        private delegate int CloneProc(
            IntPtr pUnk,
            out IntPtr ppEnum
            );

        #endregion

        #region Constructor

        public IEnumMediaTypesImpl(IntPtr pEnum)
            : base(pEnum, false)
        {

        }

        #endregion

        #region IEnumMediaTypes Members

        public int Next(int cMediaTypes, IntPtr ppMediaTypes, IntPtr pcFetched)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            NextProc _Proc = GetProcDelegate<NextProc>(3);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        cMediaTypes, ppMediaTypes, pcFetched
                        );
        }

        public int Skip(int cMediaTypes)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            SkipProc _Proc = GetProcDelegate<SkipProc>(4);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        cMediaTypes
                        );
        }

        public int Reset()
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            ResetProc _Proc = GetProcDelegate<ResetProc>(5);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown
                        );
        }

        public int Clone(out IntPtr ppEnum)
        {
            ppEnum = IntPtr.Zero;

            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            CloneProc _Proc = GetProcDelegate<CloneProc>(6);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        out ppEnum
                        );
        }

        #endregion
    }

    [ComVisible(false)]
    public class IPinImpl : VTableInterface, IPin
    {
        #region Delegates

        private delegate int ConnectProc(
            IntPtr pUnk,
            [In] IntPtr pReceivePin,
            [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt
            );

        private delegate int ReceiveConnectionProc(
            IntPtr pUnk,
            [In] IntPtr pReceivePin,
            [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt
            );

        private delegate int DisconnectProc(
            IntPtr pUnk
            );

        private delegate int ConnectedToProc(
            IntPtr pUnk,
            [Out] out IntPtr ppPin);

        private delegate int ConnectionMediaTypeProc(
            IntPtr pUnk,
            [Out, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);

        private delegate int QueryPinInfoProc(
            IntPtr pUnk,
            [Out] out PinInfo pInfo
            );

        private delegate int QueryDirectionProc(
            IntPtr pUnk,
            out PinDirection pPinDir
            );

        private delegate int QueryIdProc(
            IntPtr pUnk,
            [Out, MarshalAs(UnmanagedType.LPWStr)] out string Id);

        private delegate int QueryAcceptProc(
            IntPtr pUnk,
            [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);

        private delegate int EnumMediaTypesProc(
            IntPtr pUnk,
            [Out] out IntPtr ppEnum);
            //[Out] out IEnumMediaTypes ppEnum);

        private delegate int QueryInternalConnectionsProc(
            IntPtr pUnk,
            [Out] IntPtr ppPins,
            [In, Out] ref int nPin
            );

        private delegate int EndOfStreamProc(
            IntPtr pUnk
            );

        private delegate int BeginFlushProc(
            IntPtr pUnk
            );

        private delegate int EndFlushProc(
            IntPtr pUnk
            );

        private delegate int NewSegmentProc(
            IntPtr pUnk,
            [In] long tStart,
            [In] long tStop,
            [In] double dRate
            );

        #endregion

        #region Constructor

        public IPinImpl(IntPtr pPin)
            : base(pPin, false)
        {

        }

        #endregion

        #region IPin Members

        public int Connect(IntPtr pReceivePin, AMMediaType pmt)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            ConnectProc _Proc = GetProcDelegate<ConnectProc>(3);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        pReceivePin, pmt
                        );
        }

        public int ReceiveConnection(IntPtr pReceivePin, AMMediaType pmt)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            ReceiveConnectionProc _Proc = GetProcDelegate<ReceiveConnectionProc>(4);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        pReceivePin, pmt
                        );
        }

        public int Disconnect()
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            DisconnectProc _Proc = GetProcDelegate<DisconnectProc>(5);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown
                        );
        }

        public int ConnectedTo(out IntPtr ppPin)
        {
            ppPin = IntPtr.Zero;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            ConnectedToProc _Proc = GetProcDelegate<ConnectedToProc>(6);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        out ppPin
                        );
        }

        public int ConnectionMediaType(AMMediaType pmt)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            ConnectionMediaTypeProc _Proc = GetProcDelegate<ConnectionMediaTypeProc>(7);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        pmt
                        );
        }

        public int QueryPinInfo(out PinInfo pInfo)
        {
            pInfo = new PinInfo();
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            QueryPinInfoProc _Proc = GetProcDelegate<QueryPinInfoProc>(8);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        out pInfo
                        );
        }

        public int QueryDirection(out PinDirection pPinDir)
        {
            pPinDir = PinDirection.Input;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            QueryDirectionProc _Proc = GetProcDelegate<QueryDirectionProc>(9);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        out pPinDir
                        );
        }

        public int QueryId(out string Id)
        {
            Id = null;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            QueryIdProc _Proc = GetProcDelegate<QueryIdProc>(10);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        out Id
                        );
        }

        public int QueryAccept(AMMediaType pmt)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            QueryAcceptProc _Proc = GetProcDelegate<QueryAcceptProc>(11);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        pmt
                        );
        }

        //public int EnumMediaTypes(out IEnumMediaTypes ppEnum)
        public int EnumMediaTypes(out IntPtr ppEnum)
        {
            ppEnum = IntPtr.Zero;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            EnumMediaTypesProc _Proc = GetProcDelegate<EnumMediaTypesProc>(12);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        out ppEnum
                        );
        }

        public int QueryInternalConnections(IntPtr ppPins, ref int nPin)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            QueryInternalConnectionsProc _Proc = GetProcDelegate<QueryInternalConnectionsProc>(13);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        ppPins, ref nPin
                        );
        }

        public int EndOfStream()
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            EndOfStreamProc _Proc = GetProcDelegate<EndOfStreamProc>(14);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown
                        );
        }

        public int BeginFlush()
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            BeginFlushProc _Proc = GetProcDelegate<BeginFlushProc>(15);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown
                        );
        }

        public int EndFlush()
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            EndFlushProc _Proc = GetProcDelegate<EndFlushProc>(16);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown
                        );
        }

        public int NewSegment(long tStart, long tStop, double dRate)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            NewSegmentProc _Proc = GetProcDelegate<NewSegmentProc>(17);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        tStart, tStop, dRate
                        );
        }

        #endregion
    }

    [ComVisible(false)]
    public class ISeekingPassThruImpl : VTableInterface, ISeekingPassThru
    {
        #region Delegate
 
        private delegate int InitProc(
            IntPtr pUnk,
            [In, MarshalAs(UnmanagedType.Bool)] bool bSupportRendering,
            [In] IPin pPin
            );

        #endregion

        #region Constructor

        public ISeekingPassThruImpl(IntPtr pSeekingPassThru)
            : base(pSeekingPassThru, false)
        {

        }

        #endregion

        #region ISeekingPassThru Members

        public int Init(bool bSupportRendering, IPin pPin)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            InitProc _Proc = GetProcDelegate<InitProc>(3);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        bSupportRendering, pPin
                        );
        }

        #endregion
    }

    [ComVisible(false)]
    public class IMediaSeekingImpl : VTableInterface, IMediaSeeking
    {
        #region Delegates

        private delegate int GetCapabilitiesProc(
            IntPtr pUnk,
            [Out] out AMSeekingSeekingCapabilities pCapabilities
            );

        private delegate int CheckCapabilitiesProc(
            IntPtr pUnk,
            [In, Out] ref AMSeekingSeekingCapabilities pCapabilities);

        private delegate int IsFormatSupportedProc(
            IntPtr pUnk,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid pFormat);

        private delegate int QueryPreferredFormatProc(
            IntPtr pUnk,
            [Out] out Guid pFormat);

        private delegate int GetTimeFormatProc(
            IntPtr pUnk,
            [Out] out Guid pFormat);

        private delegate int IsUsingTimeFormatProc(
            IntPtr pUnk,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid pFormat);

        private delegate int SetTimeFormatProc(
            IntPtr pUnk,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid pFormat);

        private delegate int GetDurationProc(
            IntPtr pUnk,
            [Out] out long pDuration);

        private delegate int GetStopPositionProc(
            IntPtr pUnk,
            [Out] out long pStop);

        private delegate int GetCurrentPositionProc(
            IntPtr pUnk,
            [Out] out long pCurrent);

        private delegate int ConvertTimeFormatProc(
            IntPtr pUnk,
            [Out] out long pTarget,
            [In, MarshalAs(UnmanagedType.LPStruct)] DsGuid pTargetFormat,
            [In] long Source,
            [In, MarshalAs(UnmanagedType.LPStruct)] DsGuid pSourceFormat
            );

        private delegate int SetPositionsProc(
            IntPtr pUnk,
            [In, Out, MarshalAs(UnmanagedType.LPStruct)] DsLong pCurrent,
            [In] AMSeekingSeekingFlags dwCurrentFlags,
            [In, Out, MarshalAs(UnmanagedType.LPStruct)] DsLong pStop,
            [In] AMSeekingSeekingFlags dwStopFlags
            );

        private delegate int GetPositionsProc(
            IntPtr pUnk,
            [Out] out long pCurrent,
            [Out] out long pStop
            );

        private delegate int GetAvailableProc(
            IntPtr pUnk,
            [Out] out long pEarliest,
            [Out] out long pLatest
            );

        private delegate int SetRateProc(
            IntPtr pUnk,
            [In] double dRate);

        private delegate int GetRateProc(
            IntPtr pUnk,
            [Out] out double pdRate);

        private delegate int GetPrerollProc(
            IntPtr pUnk,
            [Out] out long pllPreroll);

        #endregion

        #region Constructor

        public IMediaSeekingImpl(IntPtr pMediaSeeking)
            : base(pMediaSeeking, typeof(IMediaSeeking))
        {

        }

        #endregion

        #region IMediaSeeking Members

        public int GetCapabilities(out AMSeekingSeekingCapabilities pCapabilities)
        {
            pCapabilities = AMSeekingSeekingCapabilities.None;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            GetCapabilitiesProc _Proc = GetProcDelegate<GetCapabilitiesProc>(3);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        out pCapabilities
                        );
        }

        public int CheckCapabilities(ref AMSeekingSeekingCapabilities pCapabilities)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            CheckCapabilitiesProc _Proc = GetProcDelegate<CheckCapabilitiesProc>(4);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        ref pCapabilities
                        );
        }

        public int IsFormatSupported(Guid pFormat)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            IsFormatSupportedProc _Proc = GetProcDelegate<IsFormatSupportedProc>(5);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        pFormat
                        );
        }

        public int QueryPreferredFormat(out Guid pFormat)
        {
            pFormat = Guid.Empty;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            QueryPreferredFormatProc _Proc = GetProcDelegate<QueryPreferredFormatProc>(6);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        out pFormat
                        );
        }

        public int GetTimeFormat(out Guid pFormat)
        {
            pFormat = Guid.Empty;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            GetTimeFormatProc _Proc = GetProcDelegate<GetTimeFormatProc>(7);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        out pFormat
                        );
        }

        public int IsUsingTimeFormat(Guid pFormat)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            IsUsingTimeFormatProc _Proc = GetProcDelegate<IsUsingTimeFormatProc>(8);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        pFormat
                        );
        }

        public int SetTimeFormat(Guid pFormat)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            SetTimeFormatProc _Proc = GetProcDelegate<SetTimeFormatProc>(9);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        pFormat
                        );
        }

        public int GetDuration(out long pDuration)
        {
            pDuration = 0;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            GetDurationProc _Proc = GetProcDelegate<GetDurationProc>(10);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        out pDuration
                        );
        }

        public int GetStopPosition(out long pStop)
        {
            pStop = 0;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            GetStopPositionProc _Proc = GetProcDelegate<GetStopPositionProc>(11);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        out pStop
                        );
        }

        public int GetCurrentPosition(out long pCurrent)
        {
            pCurrent = 0;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            GetCurrentPositionProc _Proc = GetProcDelegate<GetCurrentPositionProc>(12);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        out pCurrent
                        );
        }

        public int ConvertTimeFormat(out long pTarget, DsGuid pTargetFormat, long Source, DsGuid pSourceFormat)
        {
            pTarget = 0;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            ConvertTimeFormatProc _Proc = GetProcDelegate<ConvertTimeFormatProc>(13);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        out pTarget, pTargetFormat, Source, pSourceFormat
                        );
        }

        public int SetPositions(DsLong pCurrent, AMSeekingSeekingFlags dwCurrentFlags, DsLong pStop, AMSeekingSeekingFlags dwStopFlags)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            SetPositionsProc _Proc = GetProcDelegate<SetPositionsProc>(14);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        pCurrent, dwCurrentFlags, pStop, dwStopFlags
                        );
        }

        public int GetPositions(out long pCurrent, out long pStop)
        {
            pCurrent = 0;
            pStop = 0;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            GetPositionsProc _Proc = GetProcDelegate<GetPositionsProc>(15);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        out pCurrent, out pStop
                        );
        }

        public int GetAvailable(out long pEarliest, out long pLatest)
        {
            pEarliest = 0;
            pLatest = 0;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            GetAvailableProc _Proc = GetProcDelegate<GetAvailableProc>(16);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        out pEarliest, out pLatest
                        );
        }

        public int SetRate(double dRate)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            SetRateProc _Proc = GetProcDelegate<SetRateProc>(17);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        dRate
                        );
        }

        public int GetRate(out double pdRate)
        {
            pdRate = 1.0;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            GetRateProc _Proc = GetProcDelegate<GetRateProc>(18);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        out pdRate
                        );
        }

        public int GetPreroll(out long pllPreroll)
        {
            pllPreroll = 0;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            GetPrerollProc _Proc = GetProcDelegate<GetPrerollProc>(19);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        out pllPreroll
                        );
        }

        #endregion
    }

    [ComVisible(false)]
    public class IPropertyPageSiteImpl : VTableInterface, IPropertyPageSite
    {
        #region delegates

        private delegate int OnStatusChangeProc(
            IntPtr pUnk,
            [In] PropStatus dwFlags);

        private delegate int GetLocaleIDProc(
            IntPtr pUnk,
            [Out] out int LocaleID);

        private delegate int GetPageContainerProc(
            IntPtr pUnk,
            [Out, MarshalAs(UnmanagedType.IUnknown)] out object pUnknown);

        private delegate int TranslateAcceleratorProc(
            IntPtr pUnk,
            IntPtr pMsg);

        #endregion

        #region Constructor

        public IPropertyPageSiteImpl(IntPtr pPropPageSite)
            : base(pPropPageSite, false)
        {

        }

        #endregion

        #region IPropertyPageSite Members

        public int OnStatusChange(PropStatus dwFlags)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            OnStatusChangeProc _Proc = GetProcDelegate<OnStatusChangeProc>(3);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        dwFlags
                        );
        }

        public int GetLocaleID(out int LocaleID)
        {
            LocaleID = 0;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            GetLocaleIDProc _Proc = GetProcDelegate<GetLocaleIDProc>(4);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        out LocaleID
                        );
        }

        public int GetPageContainer(out object pUnk)
        {
            pUnk = null;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            GetPageContainerProc _Proc = GetProcDelegate<GetPageContainerProc>(5);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        out pUnk
                        );
        }

        public int TranslateAccelerator(IntPtr pMsg)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            TranslateAcceleratorProc _Proc = GetProcDelegate<TranslateAcceleratorProc>(6);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        pMsg
                        );
        }

        #endregion
    }

    [ComVisible(false)]
    public class IReferenceClockImpl : VTableInterface, IReferenceClock
    {
        #region Delegate

        private delegate int GetTimeProc(
            IntPtr pUnk,
            [Out] out long pTime);

        private delegate int AdviseTimeProc(
            IntPtr pUnk,
            [In] long baseTime,
            [In] long streamTime,
            [In] IntPtr hEvent,
            [Out] out int pdwAdviseCookie
            );

        private delegate int AdvisePeriodicProc(
            IntPtr pUnk,
            [In] long startTime,
            [In] long periodTime,
            [In] IntPtr hSemaphore,
            [Out] out int pdwAdviseCookie
            );

        private delegate int UnadviseProc(
            IntPtr pUnk,
            [In] int dwAdviseCookie);

        #endregion

        #region Constructor

        public IReferenceClockImpl(IntPtr pReferenceClock)
            : base(pReferenceClock, false)
        {

        }

        #endregion

        #region IReferenceClock Members

        public int GetTime(out long pTime)
        {
            pTime = 0;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            GetTimeProc _Proc = GetProcDelegate<GetTimeProc>(3);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        out pTime
                        );
        }

        public int AdviseTime(long baseTime, long streamTime, IntPtr hEvent, out int pdwAdviseCookie)
        {
            pdwAdviseCookie = 0;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            AdviseTimeProc _Proc = GetProcDelegate<AdviseTimeProc>(4);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        baseTime, streamTime, hEvent, out pdwAdviseCookie 
                        );
        }

        public int AdvisePeriodic(long startTime, long periodTime, IntPtr hSemaphore, out int pdwAdviseCookie)
        {
            pdwAdviseCookie = 0;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            AdvisePeriodicProc _Proc = GetProcDelegate<AdvisePeriodicProc>(5);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        startTime, periodTime, hSemaphore, out pdwAdviseCookie
                        );
        }

        public int Unadvise(int dwAdviseCookie)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            UnadviseProc _Proc = GetProcDelegate<UnadviseProc>(6);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        dwAdviseCookie
                        );
        }

        #endregion
    }

    [ComVisible(false)]
    public class IMediaEventSinkImpl : VTableInterface, IMediaEventSink
    {
        #region Delegates

        private delegate int NotifyProc(
            IntPtr pUnk,
            EventCode evCode,
            IntPtr EventParam1,
            IntPtr EventParam2);

        #endregion

        #region Constructor

        public IMediaEventSinkImpl(IntPtr pUnknown)
            : base(pUnknown, false)
        {

        }

        #endregion

        #region IMediaEventSink Members

        public int Notify(EventCode evCode, IntPtr EventParam1, IntPtr EventParam2)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            NotifyProc _NotifyProc = GetProcDelegate<NotifyProc>(3);

            if (_NotifyProc == null) return E_UNEXPECTED;

            return (HRESULT)_NotifyProc(m_pUnknown, evCode, EventParam1, EventParam2);
        }

        #endregion
    }

    [ComVisible(false)]
    public class IAsyncReaderImpl : VTableInterface, IAsyncReader
    {
        #region Delegate

        private delegate int RequestAllocatorProc(
            IntPtr pUnk,
            [In] IntPtr pPreferred,
            [In, MarshalAs(UnmanagedType.LPStruct)] AllocatorProperties pProps,
            [Out] out IntPtr ppActual
            );

        private delegate int RequestProc(
            IntPtr pUnk,
            [In] IntPtr pSample,
            [In] IntPtr dwUser
            );

        private delegate int WaitForNextProc(
            IntPtr pUnk,
            [In] int dwTimeout,
            [Out] out IntPtr ppSample,
            [Out] out IntPtr pdwUser
            );

        private delegate int SyncReadAlignedProc(
            IntPtr pUnk,
            [In] IntPtr pSample
            );

        private delegate int SyncReadProc(
            IntPtr pUnk,
            [In] long llPosition,
            [In] int lLength,
            [Out] IntPtr pBuffer
            );

        private delegate int LengthProc(
            IntPtr pUnk,
            [Out] out long pTotal,
            [Out] out long pAvailable
            );

        private delegate int BeginFlushProc(
            IntPtr pUnk
            );

        private delegate int EndFlushProc(
            IntPtr pUnk
            );

        #endregion

        #region Constructor

        public IAsyncReaderImpl(IntPtr pAsyncReader)
            : base(pAsyncReader, false)
        {

        }

        #endregion

        #region IAsyncReader Members

        public int RequestAllocator(IntPtr pPreferred, AllocatorProperties pProps, out IntPtr ppActual)
        {
            ppActual = IntPtr.Zero;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            RequestAllocatorProc _Proc = GetProcDelegate<RequestAllocatorProc>(3);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        pPreferred, pProps, out ppActual
                        );
        }

        public int Request(IntPtr pSample, IntPtr dwUser)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            RequestProc _Proc = GetProcDelegate<RequestProc>(4);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        pSample, dwUser
                        );
        }

        public int WaitForNext(int dwTimeout, out IntPtr ppSample, out IntPtr pdwUser)
        {
            ppSample = IntPtr.Zero;
            pdwUser = IntPtr.Zero;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            WaitForNextProc _Proc = GetProcDelegate<WaitForNextProc>(5);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        dwTimeout,out ppSample, out pdwUser
                        );
        }

        public int SyncReadAligned(IntPtr pSample)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            SyncReadAlignedProc _Proc = GetProcDelegate<SyncReadAlignedProc>(6);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        pSample
                        );
        }

        public int SyncRead(long llPosition, int lLength, IntPtr pBuffer)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            SyncReadProc _Proc = GetProcDelegate<SyncReadProc>(7);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        llPosition, lLength, pBuffer
                        );
        }

        public int Length(out long pTotal, out long pAvailable)
        {
            pTotal = 0;
            pAvailable = 0;
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            LengthProc _Proc = GetProcDelegate<LengthProc>(8);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        out pTotal, out pAvailable
                        );
        }

        public int BeginFlush()
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            BeginFlushProc _Proc = GetProcDelegate<BeginFlushProc>(9);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown
                        );
        }

        public int EndFlush()
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            EndFlushProc _Proc = GetProcDelegate<EndFlushProc>(10);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown
                        );
        }

        #endregion
    }

    [ComVisible(false)]
    public class IStreamImpl: VTableInterface, IStream
    {
        #region Delegate

        private delegate int ReadProc(
            IntPtr pUnk,
            [In, Out,MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pv, 
            [In] int cb, 
            [In,Out] IntPtr pcbRead
            );

        private delegate int WriteProc(
            IntPtr pUnk,
            [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pv,
            [In] int cb,
            [In, Out] IntPtr pcbWritten
            );

        private delegate int SeekProc(
            IntPtr pUnk,
            long dlibMove, int dwOrigin, IntPtr plibNewPosition
            );

        private delegate int SetSizeProc(
            IntPtr pUnk,
            long libNewSize
            );

        private delegate int CopyToProc(
            IntPtr pUnk,
            IntPtr pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten
            );

        private delegate int CommitProc(
            IntPtr pUnk,
            int grfCommitFlags
            );

        private delegate int RevertProc(
            IntPtr pUnk
            );

        private delegate int LockRegionProc(
            IntPtr pUnk,
            long libOffset, long cb, int dwLockType
            );

        private delegate int UnlockRegionProc(
            IntPtr pUnk,
            long libOffset, long cb, int dwLockType
            );

        private delegate int StatProc(
            IntPtr pUnk,
            [Out, MarshalAs(UnmanagedType.LPStruct)] out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag
            );

        private delegate int CloneProc(
            IntPtr pUnk,
            [Out] out IntPtr ppstm
            );

        #endregion

        #region Constructor

        public IStreamImpl(IntPtr pStream)
            : base(pStream, false)
        {

        }

        #endregion

        #region IStream Members

        public void Read(byte[] pv, int cb, IntPtr pcbRead)
        {
            if (m_pUnknown == IntPtr.Zero) E_NOINTERFACE.Throw();

            ReadProc _Proc = GetProcDelegate<ReadProc>(3);

            if (_Proc == null) E_UNEXPECTED.Throw();

            HRESULT hr = (HRESULT)_Proc(
                        m_pUnknown,
                        pv, cb, pcbRead
                        );
            hr.Throw();
        }

        public void Write(byte[] pv, int cb, IntPtr pcbWritten)
        {
            if (m_pUnknown == IntPtr.Zero) E_NOINTERFACE.Throw();

            WriteProc _Proc = GetProcDelegate<WriteProc>(4);

            if (_Proc == null) E_UNEXPECTED.Throw();

            HRESULT hr = (HRESULT)_Proc(
                        m_pUnknown,
                        pv, cb, pcbWritten
                        );
            hr.Throw();
        }

        public void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
        {
            if (m_pUnknown == IntPtr.Zero) E_NOINTERFACE.Throw();

            SeekProc _Proc = GetProcDelegate<SeekProc>(5);

            if (_Proc == null) E_UNEXPECTED.Throw();

            HRESULT hr = (HRESULT)_Proc(
                        m_pUnknown,
                        dlibMove, dwOrigin, plibNewPosition
                        );
            hr.Throw();
        }

        public void SetSize(long libNewSize)
        {
            if (m_pUnknown == IntPtr.Zero) E_NOINTERFACE.Throw();

            SetSizeProc _Proc = GetProcDelegate<SetSizeProc>(6);

            if (_Proc == null) E_UNEXPECTED.Throw();

            HRESULT hr = (HRESULT)_Proc(
                        m_pUnknown,
                        libNewSize
                        );
            hr.Throw();
        }

        public void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
        {
            if (m_pUnknown == IntPtr.Zero) E_NOINTERFACE.Throw();

            CopyToProc _Proc = GetProcDelegate<CopyToProc>(7);

            if (_Proc == null) E_UNEXPECTED.Throw();

            HRESULT hr = (HRESULT)_Proc(
                        m_pUnknown,
                        Marshal.GetIUnknownForObject(pstm), cb, pcbRead, pcbWritten
                        );
            hr.Throw();
        }

        public void Commit(int grfCommitFlags)
        {
            if (m_pUnknown == IntPtr.Zero) E_NOINTERFACE.Throw();

            CommitProc _Proc = GetProcDelegate<CommitProc>(8);

            if (_Proc == null) E_UNEXPECTED.Throw();

            HRESULT hr = (HRESULT)_Proc(
                        m_pUnknown,
                        grfCommitFlags
                        );
            hr.Throw();
        }

        public void Revert()
        {
            if (m_pUnknown == IntPtr.Zero) E_NOINTERFACE.Throw();

            RevertProc _Proc = GetProcDelegate<RevertProc>(9);

            if (_Proc == null) E_UNEXPECTED.Throw();

            HRESULT hr = (HRESULT)_Proc(
                        m_pUnknown
                        );
            hr.Throw();
        }

        public void LockRegion(long libOffset, long cb, int dwLockType)
        {
            if (m_pUnknown == IntPtr.Zero) E_NOINTERFACE.Throw();

            LockRegionProc _Proc = GetProcDelegate<LockRegionProc>(10);

            if (_Proc == null) E_UNEXPECTED.Throw();

            HRESULT hr = (HRESULT)_Proc(
                        m_pUnknown,
                        libOffset, cb, dwLockType
                        );
            hr.Throw();
        }

        public void UnlockRegion(long libOffset, long cb, int dwLockType)
        {
            if (m_pUnknown == IntPtr.Zero) E_NOINTERFACE.Throw();

            UnlockRegionProc _Proc = GetProcDelegate<UnlockRegionProc>(11);

            if (_Proc == null) E_UNEXPECTED.Throw();

            HRESULT hr = (HRESULT)_Proc(
                        m_pUnknown,
                        libOffset, cb, dwLockType
                        );
            hr.Throw();
        }
      
        public void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag)
        {
            if (m_pUnknown == IntPtr.Zero) E_NOINTERFACE.Throw();

            StatProc _Proc = GetProcDelegate<StatProc>(12);

            if (_Proc == null) E_UNEXPECTED.Throw();

            HRESULT hr = (HRESULT)_Proc(
                        m_pUnknown,
                        out pstatstg, grfStatFlag
                        );
            hr.Throw();
        }

        public void Clone(out IStream ppstm)
        {
            if (m_pUnknown == IntPtr.Zero) E_NOINTERFACE.Throw();

            CloneProc _Proc = GetProcDelegate<CloneProc>(13);

            if (_Proc == null) E_UNEXPECTED.Throw();

            IntPtr _ptr;
            HRESULT hr = (HRESULT)_Proc(
                        m_pUnknown,
                        out _ptr
                        );
            hr.Throw();
            ppstm = (IStream)Marshal.GetObjectForIUnknown(_ptr);
        }

        #endregion
    }

    #endregion

}