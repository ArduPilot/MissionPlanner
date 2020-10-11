using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Scripting.Hosting;
using MissionPlanner.Utilities;
using SkiaSharp;
using Vector3 = OpenTK.Vector3;

namespace tlogThumbnailHandler
{
    public class tlogThumbnailHandler
    {
        public static string queuefile = "queue.txt";

    }
}

public delegate int GetInt();

namespace OpenTK.Graphics.ES20
{
    public enum BufferUsageHint
    {
        StreamDraw = 35040,
        StreamRead = 35041,
        StreamCopy = 35042,
        StaticDraw = 35044,
        StaticRead = 35045,
        StaticCopy = 35046,
        DynamicDraw = 35048,
        DynamicRead = 35049,
        DynamicCopy = 35050
    }public enum PrimitiveType
    {
        Points = 0,
        Lines = 1,
        LineLoop = 2,
        LineStrip = 3,
        Triangles = 4,
        TriangleStrip = 5,
        TriangleFan = 6,
        Quads = 7,
        QuadsExt = 7,
        QuadStrip = 8,
        Polygon = 9,
        LinesAdjacency = 10,
        LinesAdjacencyArb = 10,
        LinesAdjacencyExt = 10,
        LineStripAdjacency = 11,
        LineStripAdjacencyArb = 11,
        LineStripAdjacencyExt = 11,
        TrianglesAdjacency = 12,
        TrianglesAdjacencyArb = 12,
        TrianglesAdjacencyExt = 12,
        TriangleStripAdjacency = 13,
        TriangleStripAdjacencyArb = 13,
        TriangleStripAdjacencyExt = 13,
        Patches = 14,
        PatchesExt = 14
    }

}
namespace  IronPython.Runtime
{

}

namespace SharpAdbClient
{[Flags]
    public enum UnixFileMode
    {
        TypeMask = 0x8000,
        Socket = 0xC000,
        SymbolicLink = 0xA000,
        Regular = 0x8000,
        Block = 0x6000,
        Directory = 0x4000,
        Character = 0x2000,
        FIFO = 0x1000
    }

    public class AdbClient
    {
        public static IAdbClient Instance { get; set; }
    }
    public class AdbServer
    {
        public static AdbServer Instance { get; set; }

        public void StartServer(string adbExe, bool b)
        {
           
        }
    }

    public class DeviceData
    {

    }

    public class ConsoleOutputReceiver
    {

    }

    public class FileStatistics
    {
        public UnixFileMode FileMode { get; set; }

        public DateTime Time { get; set; }
    }
    public class SyncService: IDisposable
    {
        public SyncService(DeviceData device)
        {
            throw new NotImplementedException();
        }

        public void Push(Stream stream, string dataFtpInternalApm, int p2, DateTime now, CancellationToken none)
        {
            
        }

        public FileStatistics Stat(string dataFtpInternalApmStartArdupilotSh)
        {
            throw new NotImplementedException();
        }

        public void Pull(string etcInitDRcsModeDefault, MemoryStream stream, CancellationToken none)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
    public interface IAdbClient
    {
        string Connect(DnsEndPoint p0);
        List<DeviceData>  GetDevices();
        void ExecuteRemoteCommand(string mountORemountRw, DeviceData device, ConsoleOutputReceiver consoleOut);
        void KillAdb();
    }
}
namespace IronPython.Hosting
{
    public class Python
    {
        public static ScriptEngine CreateEngine(Dictionary<string, object> options = null)
        {
            throw new NotImplementedException();
        }
    }
}
public class OpenGLtest: UserControl
{
    public static OpenGLtest instance;   
    public Vector3 rpy;

    public PointLatLngAlt LocationCenter { get; set; }

    public List<Locationwp> WPs { get; set; }
}

public class OpenGLtest2: OpenGLtest
{
    public static OpenGLtest2 instance;

}

namespace OpenTK.Graphics.OpenGL
{


    public enum TextureTarget2d
    {
        Texture2D = 3553,
        ProxyTexture2D = 32868,
        TextureRectangle = 34037,
        ProxyTextureRectangle = 34039,
        TextureCubeMap = 34067,
        ProxyTextureCubeMap = 34075,
        Texture1DArray = 35864,
        ProxyTexture1DArray = 35865
    }
    public enum TextureComponentCount
    {
        Alpha = 6406,
        Rgb = 6407,
        Rgba = 6408,
        Luminance = 6409,
        LuminanceAlpha = 6410,
        Alpha8Ext = 32828,
        Luminance8Ext = 32832,
        Luminance8Alpha8Ext = 32837,
        Rgb10Ext = 32850,
        Rgb10A2Ext = 32857,
        R8Ext = 33321,
        Rg8Ext = 33323,
        R16fExt = 33325,
        R32fExt = 33326,
        Rg16fExt = 33327,
        Rg32fExt = 33328,
        Rgba32fExt = 34836,
        Rgb32fExt = 34837,
        Alpha32fExt = 34838,
        Luminance32fExt = 34840,
        LuminanceAlpha32fExt = 34841,
        Rgba16fExt = 34842,
        Rgb16fExt = 34843,
        Alpha16fExt = 34844,
        Luminance16fExt = 34846,
        LuminanceAlpha16fExt = 34847,
        RgbRaw422Apple = 35409,
        Bgra8Ext = 37793
    }public enum BufferUsageHint
    {
        StreamDraw = 35040,
        StreamRead = 35041,
        StreamCopy = 35042,
        StaticDraw = 35044,
        StaticRead = 35045,
        StaticCopy = 35046,
        DynamicDraw = 35048,
        DynamicRead = 35049,
        DynamicCopy = 35050
    }
    public enum PrimitiveType
    {
        Points = 0,
        Lines = 1,
        LineLoop = 2,
        LineStrip = 3,
        Triangles = 4,
        TriangleStrip = 5,
        TriangleFan = 6,
        Quads = 7,
        QuadsExt = 7,
        QuadStrip = 8,
        Polygon = 9,
        LinesAdjacency = 10,
        LinesAdjacencyArb = 10,
        LinesAdjacencyExt = 10,
        LineStripAdjacency = 11,
        LineStripAdjacencyArb = 11,
        LineStripAdjacencyExt = 11,
        TrianglesAdjacency = 12,
        TrianglesAdjacencyArb = 12,
        TrianglesAdjacencyExt = 12,
        TriangleStripAdjacency = 13,
        TriangleStripAdjacencyArb = 13,
        TriangleStripAdjacencyExt = 13,
        Patches = 14,
        PatchesExt = 14
    }public enum GetProgramParameterName
    {
        ProgramBinaryRetrievableHint = 33367,
        ProgramSeparable = 33368,
        GeometryShaderInvocations = 34943,
        GeometryVerticesOut = 35094,
        GeometryInputType = 35095,
        GeometryOutputType = 35096,
        ActiveUniformBlockMaxNameLength = 35381,
        ActiveUniformBlocks = 35382,
        DeleteStatus = 35712,
        LinkStatus = 35714,
        ValidateStatus = 35715,
        InfoLogLength = 35716,
        AttachedShaders = 35717,
        ActiveUniforms = 35718,
        ActiveUniformMaxLength = 35719,
        ActiveAttributes = 35721,
        ActiveAttributeMaxLength = 35722,
        TransformFeedbackVaryingMaxLength = 35958,
        TransformFeedbackBufferMode = 35967,
        TransformFeedbackVaryings = 35971,
        TessControlOutputVertices = 36469,
        TessGenMode = 36470,
        TessGenSpacing = 36471,
        TessGenVertexOrder = 36472,
        TessGenPointMode = 36473,
        MaxComputeWorkGroupSize = 37311,
        ActiveAtomicCounterBuffers = 37593
    }

public enum EnableCap
{
	PointSmooth = 2832,
	LineSmooth = 2848,
	LineStipple = 2852,
	PolygonSmooth = 2881,
	PolygonStipple = 2882,
	CullFace = 2884,
	Lighting = 2896,
	ColorMaterial = 2903,
	Fog = 2912,
	DepthTest = 2929,
	StencilTest = 2960,
	Normalize = 2977,
	AlphaTest = 3008,
	Dither = 3024,
	Blend = 3042,
	IndexLogicOp = 3057,
	ColorLogicOp = 3058,
	ScissorTest = 3089,
	TextureGenS = 3168,
	TextureGenT = 3169,
	TextureGenR = 3170,
	TextureGenQ = 3171,
	AutoNormal = 3456,
	Map1Color4 = 3472,
	Map1Index = 3473,
	Map1Normal = 3474,
	Map1TextureCoord1 = 3475,
	Map1TextureCoord2 = 3476,
	Map1TextureCoord3 = 3477,
	Map1TextureCoord4 = 3478,
	Map1Vertex3 = 3479,
	Map1Vertex4 = 3480,
	Map2Color4 = 3504,
	Map2Index = 3505,
	Map2Normal = 3506,
	Map2TextureCoord1 = 3507,
	Map2TextureCoord2 = 3508,
	Map2TextureCoord3 = 3509,
	Map2TextureCoord4 = 3510,
	Map2Vertex3 = 3511,
	Map2Vertex4 = 3512,
	Texture1D = 3552,
	Texture2D = 3553,
	PolygonOffsetPoint = 10753,
	PolygonOffsetLine = 10754,
	ClipDistance0 = 12288,
	ClipPlane0 = 12288,
	ClipDistance1 = 12289,
	ClipPlane1 = 12289,
	ClipDistance2 = 12290,
	ClipPlane2 = 12290,
	ClipDistance3 = 12291,
	ClipPlane3 = 12291,
	ClipDistance4 = 12292,
	ClipPlane4 = 12292,
	ClipDistance5 = 12293,
	ClipPlane5 = 12293,
	ClipDistance6 = 12294,
	ClipDistance7 = 12295,
	Light0 = 0x4000,
	Light1 = 16385,
	Light2 = 16386,
	Light3 = 16387,
	Light4 = 16388,
	Light5 = 16389,
	Light6 = 16390,
	Light7 = 16391,
	Convolution1D = 32784,
	Convolution1DExt = 32784,
	Convolution2D = 32785,
	Convolution2DExt = 32785,
	Separable2D = 32786,
	Separable2DExt = 32786,
	Histogram = 32804,
	HistogramExt = 32804,
	MinmaxExt = 32814,
	PolygonOffsetFill = 32823,
	RescaleNormal = 32826,
	RescaleNormalExt = 32826,
	Texture3DExt = 32879,
	VertexArray = 32884,
	NormalArray = 32885,
	ColorArray = 32886,
	IndexArray = 32887,
	TextureCoordArray = 32888,
	EdgeFlagArray = 32889,
	InterlaceSgix = 32916,
	Multisample = 32925,
	MultisampleSgis = 32925,
	SampleAlphaToCoverage = 32926,
	SampleAlphaToMaskSgis = 32926,
	SampleAlphaToOne = 32927,
	SampleAlphaToOneSgis = 32927,
	SampleCoverage = 32928,
	SampleMaskSgis = 32928,
	TextureColorTableSgi = 32956,
	ColorTable = 32976,
	ColorTableSgi = 32976,
	PostConvolutionColorTable = 32977,
	PostConvolutionColorTableSgi = 32977,
	PostColorMatrixColorTable = 32978,
	PostColorMatrixColorTableSgi = 32978,
	Texture4DSgis = 33076,
	PixelTexGenSgix = 33081,
	SpriteSgix = 33096,
	ReferencePlaneSgix = 33149,
	IrInstrument1Sgix = 33151,
	CalligraphicFragmentSgix = 33155,
	FramezoomSgix = 33163,
	FogOffsetSgix = 33176,
	SharedTexturePaletteExt = 33275,
	DebugOutputSynchronous = 33346,
	AsyncHistogramSgix = 33580,
	PixelTextureSgis = 33619,
	AsyncTexImageSgix = 33628,
	AsyncDrawPixelsSgix = 33629,
	AsyncReadPixelsSgix = 33630,
	FragmentLightingSgix = 33792,
	FragmentColorMaterialSgix = 33793,
	FragmentLight0Sgix = 33804,
	FragmentLight1Sgix = 33805,
	FragmentLight2Sgix = 33806,
	FragmentLight3Sgix = 33807,
	FragmentLight4Sgix = 33808,
	FragmentLight5Sgix = 33809,
	FragmentLight6Sgix = 33810,
	FragmentLight7Sgix = 33811,
	FogCoordArray = 33879,
	ColorSum = 33880,
	SecondaryColorArray = 33886,
	TextureRectangle = 34037,
	TextureCubeMap = 34067,
	ProgramPointSize = 34370,
	VertexProgramPointSize = 34370,
	VertexProgramTwoSide = 34371,
	DepthClamp = 34383,
	TextureCubeMapSeamless = 34895,
	PointSprite = 34913,
	SampleShading = 35894,
	RasterizerDiscard = 35977,
	PrimitiveRestartFixedIndex = 36201,
	FramebufferSrgb = 36281,
	SampleMask = 36433,
	PrimitiveRestart = 36765,
	DebugOutput = 37600
}
public enum ShadingModel
{
    Flat = 7424,
    Smooth
}

public enum TextureTarget
    {
        Texture1D = 3552,
        Texture2D = 3553,
        ProxyTexture1D = 32867,
        ProxyTexture1DExt = 32867,
        ProxyTexture2D = 32868,
        ProxyTexture2DExt = 32868,
        Texture3D = 32879,
        Texture3DExt = 32879,
        Texture3DOes = 32879,
        ProxyTexture3D = 32880,
        ProxyTexture3DExt = 32880,
        DetailTexture2DSgis = 32917,
        Texture4DSgis = 33076,
        ProxyTexture4DSgis = 33077,
        TextureRectangle = 34037,
        TextureRectangleArb = 34037,
        TextureRectangleNv = 34037,
        ProxyTextureRectangle = 34039,
        ProxyTextureRectangleArb = 34039,
        ProxyTextureRectangleNv = 34039,
        TextureCubeMap = 34067,
        TextureBindingCubeMap = 34068,
        TextureCubeMapPositiveX = 34069,
        TextureCubeMapNegativeX = 34070,
        TextureCubeMapPositiveY = 34071,
        TextureCubeMapNegativeY = 34072,
        TextureCubeMapPositiveZ = 34073,
        TextureCubeMapNegativeZ = 34074,
        ProxyTextureCubeMap = 34075,
        ProxyTextureCubeMapArb = 34075,
        ProxyTextureCubeMapExt = 34075,
        Texture1DArray = 35864,
        ProxyTexture1DArray = 35865,
        ProxyTexture1DArrayExt = 35865,
        Texture2DArray = 35866,
        ProxyTexture2DArray = 35867,
        ProxyTexture2DArrayExt = 35867,
        TextureBuffer = 35882,
        TextureCubeMapArray = 36873,
        TextureCubeMapArrayArb = 36873,
        TextureCubeMapArrayExt = 36873,
        TextureCubeMapArrayOes = 36873,
        ProxyTextureCubeMapArray = 36875,
        ProxyTextureCubeMapArrayArb = 36875,
        Texture2DMultisample = 37120,
        ProxyTexture2DMultisample = 37121,
        Texture2DMultisampleArray = 37122,
        ProxyTexture2DMultisampleArray = 37123
    }
public enum StringName
{
    Vendor = 7936,
    Renderer = 7937,
    Version = 7938,
    Extensions = 7939,
    ShadingLanguageVersion = 35724
}
    public enum GetPName
{
	CurrentColor = 2816,
	CurrentIndex = 2817,
	CurrentNormal = 2818,
	CurrentTextureCoords = 2819,
	CurrentRasterColor = 2820,
	CurrentRasterIndex = 2821,
	CurrentRasterTextureCoords = 2822,
	CurrentRasterPosition = 2823,
	CurrentRasterPositionValid = 2824,
	CurrentRasterDistance = 2825,
	PointSmooth = 2832,
	PointSize = 2833,
	PointSizeRange = 2834,
	SmoothPointSizeRange = 2834,
	PointSizeGranularity = 2835,
	SmoothPointSizeGranularity = 2835,
	LineSmooth = 2848,
	LineWidth = 2849,
	LineWidthRange = 2850,
	SmoothLineWidthRange = 2850,
	LineWidthGranularity = 2851,
	SmoothLineWidthGranularity = 2851,
	LineStipple = 2852,
	LineStipplePattern = 2853,
	LineStippleRepeat = 2854,
	ListMode = 2864,
	MaxListNesting = 2865,
	ListBase = 2866,
	ListIndex = 2867,
	PolygonMode = 2880,
	PolygonSmooth = 2881,
	PolygonStipple = 2882,
	EdgeFlag = 2883,
	CullFace = 2884,
	CullFaceMode = 2885,
	FrontFace = 2886,
	Lighting = 2896,
	LightModelLocalViewer = 2897,
	LightModelTwoSide = 2898,
	LightModelAmbient = 2899,
	ShadeModel = 2900,
	ColorMaterialFace = 2901,
	ColorMaterialParameter = 2902,
	ColorMaterial = 2903,
	Fog = 2912,
	FogIndex = 2913,
	FogDensity = 2914,
	FogStart = 2915,
	FogEnd = 2916,
	FogMode = 2917,
	FogColor = 2918,
	DepthRange = 2928,
	DepthTest = 2929,
	DepthWritemask = 2930,
	DepthClearValue = 2931,
	DepthFunc = 2932,
	AccumClearValue = 2944,
	StencilTest = 2960,
	StencilClearValue = 2961,
	StencilFunc = 2962,
	StencilValueMask = 2963,
	StencilFail = 2964,
	StencilPassDepthFail = 2965,
	StencilPassDepthPass = 2966,
	StencilRef = 2967,
	StencilWritemask = 2968,
	MatrixMode = 2976,
	Normalize = 2977,
	Viewport = 2978,
	Modelview0StackDepthExt = 2979,
	ModelviewStackDepth = 2979,
	ProjectionStackDepth = 2980,
	TextureStackDepth = 2981,
	Modelview0MatrixExt = 2982,
	ModelviewMatrix = 2982,
	ProjectionMatrix = 2983,
	TextureMatrix = 2984,
	AttribStackDepth = 2992,
	ClientAttribStackDepth = 2993,
	AlphaTest = 3008,
	AlphaTestQcom = 3008,
	AlphaTestFunc = 3009,
	AlphaTestFuncQcom = 3009,
	AlphaTestRef = 3010,
	AlphaTestRefQcom = 3010,
	Dither = 3024,
	BlendDst = 3040,
	BlendSrc = 3041,
	Blend = 3042,
	LogicOpMode = 3056,
	IndexLogicOp = 3057,
	LogicOp = 3057,
	ColorLogicOp = 3058,
	AuxBuffers = 3072,
	DrawBuffer = 3073,
	DrawBufferExt = 3073,
	ReadBuffer = 3074,
	ReadBufferExt = 3074,
	ReadBufferNv = 3074,
	ScissorBox = 3088,
	ScissorTest = 3089,
	IndexClearValue = 3104,
	IndexWritemask = 3105,
	ColorClearValue = 3106,
	ColorWritemask = 3107,
	IndexMode = 3120,
	RgbaMode = 3121,
	Doublebuffer = 3122,
	Stereo = 3123,
	RenderMode = 3136,
	PerspectiveCorrectionHint = 3152,
	PointSmoothHint = 3153,
	LineSmoothHint = 3154,
	PolygonSmoothHint = 3155,
	FogHint = 3156,
	TextureGenS = 3168,
	TextureGenT = 3169,
	TextureGenR = 3170,
	TextureGenQ = 3171,
	PixelMapIToISize = 3248,
	PixelMapSToSSize = 3249,
	PixelMapIToRSize = 3250,
	PixelMapIToGSize = 3251,
	PixelMapIToBSize = 3252,
	PixelMapIToASize = 3253,
	PixelMapRToRSize = 3254,
	PixelMapGToGSize = 3255,
	PixelMapBToBSize = 3256,
	PixelMapAToASize = 3257,
	UnpackSwapBytes = 3312,
	UnpackLsbFirst = 3313,
	UnpackRowLength = 3314,
	UnpackSkipRows = 3315,
	UnpackSkipPixels = 3316,
	UnpackAlignment = 3317,
	PackSwapBytes = 3328,
	PackLsbFirst = 3329,
	PackRowLength = 3330,
	PackSkipRows = 3331,
	PackSkipPixels = 3332,
	PackAlignment = 3333,
	MapColor = 3344,
	MapStencil = 3345,
	IndexShift = 3346,
	IndexOffset = 3347,
	RedScale = 3348,
	RedBias = 3349,
	ZoomX = 3350,
	ZoomY = 3351,
	GreenScale = 3352,
	GreenBias = 3353,
	BlueScale = 3354,
	BlueBias = 3355,
	AlphaScale = 3356,
	AlphaBias = 3357,
	DepthScale = 3358,
	DepthBias = 3359,
	MaxEvalOrder = 3376,
	MaxLights = 3377,
	MaxClipDistances = 3378,
	MaxClipPlanes = 3378,
	MaxTextureSize = 3379,
	MaxPixelMapTable = 3380,
	MaxAttribStackDepth = 3381,
	MaxModelviewStackDepth = 3382,
	MaxNameStackDepth = 3383,
	MaxProjectionStackDepth = 3384,
	MaxTextureStackDepth = 3385,
	MaxViewportDims = 3386,
	MaxClientAttribStackDepth = 3387,
	SubpixelBits = 3408,
	IndexBits = 3409,
	RedBits = 3410,
	GreenBits = 3411,
	BlueBits = 3412,
	AlphaBits = 3413,
	DepthBits = 3414,
	StencilBits = 3415,
	AccumRedBits = 3416,
	AccumGreenBits = 3417,
	AccumBlueBits = 3418,
	AccumAlphaBits = 3419,
	NameStackDepth = 3440,
	AutoNormal = 3456,
	Map1Color4 = 3472,
	Map1Index = 3473,
	Map1Normal = 3474,
	Map1TextureCoord1 = 3475,
	Map1TextureCoord2 = 3476,
	Map1TextureCoord3 = 3477,
	Map1TextureCoord4 = 3478,
	Map1Vertex3 = 3479,
	Map1Vertex4 = 3480,
	Map2Color4 = 3504,
	Map2Index = 3505,
	Map2Normal = 3506,
	Map2TextureCoord1 = 3507,
	Map2TextureCoord2 = 3508,
	Map2TextureCoord3 = 3509,
	Map2TextureCoord4 = 3510,
	Map2Vertex3 = 3511,
	Map2Vertex4 = 3512,
	Map1GridDomain = 3536,
	Map1GridSegments = 3537,
	Map2GridDomain = 3538,
	Map2GridSegments = 3539,
	Texture1D = 3552,
	Texture2D = 3553,
	FeedbackBufferSize = 3569,
	FeedbackBufferType = 3570,
	SelectionBufferSize = 3572,
	PolygonOffsetUnits = 10752,
	PolygonOffsetPoint = 10753,
	PolygonOffsetLine = 10754,
	ClipPlane0 = 12288,
	ClipPlane1 = 12289,
	ClipPlane2 = 12290,
	ClipPlane3 = 12291,
	ClipPlane4 = 12292,
	ClipPlane5 = 12293,
	Light0 = 0x4000,
	Light1 = 16385,
	Light2 = 16386,
	Light3 = 16387,
	Light4 = 16388,
	Light5 = 16389,
	Light6 = 16390,
	Light7 = 16391,
	BlendColorExt = 32773,
	BlendEquationExt = 32777,
	BlendEquationRgb = 32777,
	PackCmykHintExt = 32782,
	UnpackCmykHintExt = 32783,
	Convolution1DExt = 32784,
	Convolution2DExt = 32785,
	Separable2DExt = 32786,
	PostConvolutionRedScaleExt = 32796,
	PostConvolutionGreenScaleExt = 32797,
	PostConvolutionBlueScaleExt = 32798,
	PostConvolutionAlphaScaleExt = 32799,
	PostConvolutionRedBiasExt = 32800,
	PostConvolutionGreenBiasExt = 32801,
	PostConvolutionBlueBiasExt = 32802,
	PostConvolutionAlphaBiasExt = 32803,
	HistogramExt = 32804,
	MinmaxExt = 32814,
	PolygonOffsetFill = 32823,
	PolygonOffsetFactor = 32824,
	PolygonOffsetBiasExt = 32825,
	RescaleNormalExt = 32826,
	TextureBinding1D = 32872,
	TextureBinding2D = 32873,
	Texture3DBindingExt = 32874,
	TextureBinding3D = 32874,
	PackSkipImagesExt = 32875,
	PackImageHeightExt = 32876,
	UnpackSkipImagesExt = 32877,
	UnpackImageHeightExt = 32878,
	Texture3DExt = 32879,
	Max3DTextureSize = 32883,
	Max3DTextureSizeExt = 32883,
	VertexArray = 32884,
	NormalArray = 32885,
	ColorArray = 32886,
	IndexArray = 32887,
	TextureCoordArray = 32888,
	EdgeFlagArray = 32889,
	VertexArraySize = 32890,
	VertexArrayType = 32891,
	VertexArrayStride = 32892,
	VertexArrayCountExt = 32893,
	NormalArrayType = 32894,
	NormalArrayStride = 32895,
	NormalArrayCountExt = 32896,
	ColorArraySize = 32897,
	ColorArrayType = 32898,
	ColorArrayStride = 32899,
	ColorArrayCountExt = 32900,
	IndexArrayType = 32901,
	IndexArrayStride = 32902,
	IndexArrayCountExt = 32903,
	TextureCoordArraySize = 32904,
	TextureCoordArrayType = 32905,
	TextureCoordArrayStride = 32906,
	TextureCoordArrayCountExt = 32907,
	EdgeFlagArrayStride = 32908,
	EdgeFlagArrayCountExt = 32909,
	InterlaceSgix = 32916,
	DetailTexture2DBindingSgis = 32918,
	Multisample = 32925,
	MultisampleSgis = 32925,
	SampleAlphaToCoverage = 32926,
	SampleAlphaToMaskSgis = 32926,
	SampleAlphaToOne = 32927,
	SampleAlphaToOneSgis = 32927,
	SampleCoverage = 32928,
	SampleMaskSgis = 32928,
	SampleBuffers = 32936,
	SampleBuffersSgis = 32936,
	Samples = 32937,
	SamplesSgis = 32937,
	SampleCoverageValue = 32938,
	SampleMaskValueSgis = 32938,
	SampleCoverageInvert = 32939,
	SampleMaskInvertSgis = 32939,
	SamplePatternSgis = 32940,
	ColorMatrixSgi = 32945,
	ColorMatrixStackDepthSgi = 32946,
	MaxColorMatrixStackDepthSgi = 32947,
	PostColorMatrixRedScaleSgi = 32948,
	PostColorMatrixGreenScaleSgi = 32949,
	PostColorMatrixBlueScaleSgi = 32950,
	PostColorMatrixAlphaScaleSgi = 32951,
	PostColorMatrixRedBiasSgi = 32952,
	PostColorMatrixGreenBiasSgi = 32953,
	PostColorMatrixBlueBiasSgi = 32954,
	PostColorMatrixAlphaBiasSgi = 32955,
	TextureColorTableSgi = 32956,
	BlendDstRgb = 32968,
	BlendSrcRgb = 32969,
	BlendDstAlpha = 32970,
	BlendSrcAlpha = 32971,
	ColorTableSgi = 32976,
	PostConvolutionColorTableSgi = 32977,
	PostColorMatrixColorTableSgi = 32978,
	MaxElementsVertices = 33000,
	MaxElementsIndices = 33001,
	PointSizeMin = 33062,
	PointSizeMinSgis = 33062,
	PointSizeMax = 33063,
	PointSizeMaxSgis = 33063,
	PointFadeThresholdSize = 33064,
	PointFadeThresholdSizeSgis = 33064,
	DistanceAttenuationSgis = 33065,
	PointDistanceAttenuation = 33065,
	FogFuncPointsSgis = 33067,
	MaxFogFuncPointsSgis = 33068,
	PackSkipVolumesSgis = 33072,
	PackImageDepthSgis = 33073,
	UnpackSkipVolumesSgis = 33074,
	UnpackImageDepthSgis = 33075,
	Texture4DSgis = 33076,
	Max4DTextureSizeSgis = 33080,
	PixelTexGenSgix = 33081,
	PixelTileBestAlignmentSgix = 33086,
	PixelTileCacheIncrementSgix = 33087,
	PixelTileWidthSgix = 33088,
	PixelTileHeightSgix = 33089,
	PixelTileGridWidthSgix = 33090,
	PixelTileGridHeightSgix = 33091,
	PixelTileGridDepthSgix = 33092,
	PixelTileCacheSizeSgix = 33093,
	SpriteSgix = 33096,
	SpriteModeSgix = 33097,
	SpriteAxisSgix = 33098,
	SpriteTranslationSgix = 33099,
	Texture4DBindingSgis = 33103,
	MaxClipmapDepthSgix = 33143,
	MaxClipmapVirtualDepthSgix = 33144,
	PostTextureFilterBiasRangeSgix = 33147,
	PostTextureFilterScaleRangeSgix = 33148,
	ReferencePlaneSgix = 33149,
	ReferencePlaneEquationSgix = 33150,
	IrInstrument1Sgix = 33151,
	InstrumentMeasurementsSgix = 33153,
	CalligraphicFragmentSgix = 33155,
	FramezoomSgix = 33163,
	FramezoomFactorSgix = 33164,
	MaxFramezoomFactorSgix = 33165,
	GenerateMipmapHint = 33170,
	GenerateMipmapHintSgis = 33170,
	DeformationsMaskSgix = 33174,
	FogOffsetSgix = 33176,
	FogOffsetValueSgix = 33177,
	LightModelColorControl = 33272,
	SharedTexturePaletteExt = 33275,
	MajorVersion = 33307,
	MinorVersion = 33308,
	NumExtensions = 33309,
	ContextFlags = 33310,
	ResetNotificationStrategy = 33366,
	ProgramPipelineBinding = 33370,
	MaxViewports = 33371,
	ViewportSubpixelBits = 33372,
	ViewportBoundsRange = 33373,
	LayerProvokingVertex = 33374,
	ViewportIndexProvokingVertex = 33375,
	MaxCullDistances = 33529,
	MaxCombinedClipAndCullDistances = 33530,
	ContextReleaseBehavior = 33531,
	ConvolutionHintSgix = 33558,
	AsyncMarkerSgix = 33577,
	PixelTexGenModeSgix = 33579,
	AsyncHistogramSgix = 33580,
	MaxAsyncHistogramSgix = 33581,
	PixelTextureSgis = 33619,
	AsyncTexImageSgix = 33628,
	AsyncDrawPixelsSgix = 33629,
	AsyncReadPixelsSgix = 33630,
	MaxAsyncTexImageSgix = 33631,
	MaxAsyncDrawPixelsSgix = 33632,
	MaxAsyncReadPixelsSgix = 33633,
	VertexPreclipSgix = 33774,
	VertexPreclipHintSgix = 33775,
	FragmentLightingSgix = 33792,
	FragmentColorMaterialSgix = 33793,
	FragmentColorMaterialFaceSgix = 33794,
	FragmentColorMaterialParameterSgix = 33795,
	MaxFragmentLightsSgix = 33796,
	MaxActiveLightsSgix = 33797,
	LightEnvModeSgix = 33799,
	FragmentLightModelLocalViewerSgix = 33800,
	FragmentLightModelTwoSideSgix = 33801,
	FragmentLightModelAmbientSgix = 33802,
	FragmentLightModelNormalInterpolationSgix = 33803,
	FragmentLight0Sgix = 33804,
	PackResampleSgix = 33838,
	UnpackResampleSgix = 33839,
	CurrentFogCoord = 33875,
	FogCoordArrayType = 33876,
	FogCoordArrayStride = 33877,
	ColorSum = 33880,
	CurrentSecondaryColor = 33881,
	SecondaryColorArraySize = 33882,
	SecondaryColorArrayType = 33883,
	SecondaryColorArrayStride = 33884,
	CurrentRasterSecondaryColor = 33887,
	AliasedPointSizeRange = 33901,
	AliasedLineWidthRange = 33902,
	ActiveTexture = 34016,
	ClientActiveTexture = 34017,
	MaxTextureUnits = 34018,
	TransposeModelviewMatrix = 34019,
	TransposeProjectionMatrix = 34020,
	TransposeTextureMatrix = 34021,
	TransposeColorMatrix = 34022,
	MaxRenderbufferSize = 34024,
	MaxRenderbufferSizeExt = 34024,
	TextureCompressionHint = 34031,
	TextureBindingRectangle = 34038,
	MaxRectangleTextureSize = 34040,
	MaxTextureLodBias = 34045,
	TextureCubeMap = 34067,
	TextureBindingCubeMap = 34068,
	MaxCubeMapTextureSize = 34076,
	PackSubsampleRateSgix = 34208,
	UnpackSubsampleRateSgix = 34209,
	VertexArrayBinding = 34229,
	ProgramPointSize = 34370,
	DepthClamp = 34383,
	NumCompressedTextureFormats = 34466,
	CompressedTextureFormats = 34467,
	NumProgramBinaryFormats = 34814,
	ProgramBinaryFormats = 34815,
	StencilBackFunc = 34816,
	StencilBackFail = 34817,
	StencilBackPassDepthFail = 34818,
	StencilBackPassDepthPass = 34819,
	RgbaFloatMode = 34848,
	MaxDrawBuffers = 34852,
	DrawBuffer0 = 34853,
	DrawBuffer1 = 34854,
	DrawBuffer2 = 34855,
	DrawBuffer3 = 34856,
	DrawBuffer4 = 34857,
	DrawBuffer5 = 34858,
	DrawBuffer6 = 34859,
	DrawBuffer7 = 34860,
	DrawBuffer8 = 34861,
	DrawBuffer9 = 34862,
	DrawBuffer10 = 34863,
	DrawBuffer11 = 34864,
	DrawBuffer12 = 34865,
	DrawBuffer13 = 34866,
	DrawBuffer14 = 34867,
	DrawBuffer15 = 34868,
	BlendEquationAlpha = 34877,
	TextureCubeMapSeamless = 34895,
	PointSprite = 34913,
	MaxVertexAttribs = 34921,
	MaxTessControlInputComponents = 34924,
	MaxTessEvaluationInputComponents = 34925,
	MaxTextureCoords = 34929,
	MaxTextureImageUnits = 34930,
	ArrayBufferBinding = 34964,
	ElementArrayBufferBinding = 34965,
	VertexArrayBufferBinding = 34966,
	NormalArrayBufferBinding = 34967,
	ColorArrayBufferBinding = 34968,
	IndexArrayBufferBinding = 34969,
	TextureCoordArrayBufferBinding = 34970,
	EdgeFlagArrayBufferBinding = 34971,
	SecondaryColorArrayBufferBinding = 34972,
	FogCoordArrayBufferBinding = 34973,
	WeightArrayBufferBinding = 34974,
	VertexAttribArrayBufferBinding = 34975,
	PixelPackBufferBinding = 35053,
	PixelUnpackBufferBinding = 35055,
	MaxDualSourceDrawBuffers = 35068,
	MaxArrayTextureLayers = 35071,
	MinProgramTexelOffset = 35076,
	MaxProgramTexelOffset = 35077,
	SamplerBinding = 35097,
	ClampVertexColor = 35098,
	ClampFragmentColor = 35099,
	ClampReadColor = 35100,
	MaxVertexUniformBlocks = 35371,
	MaxGeometryUniformBlocks = 35372,
	MaxFragmentUniformBlocks = 35373,
	MaxCombinedUniformBlocks = 35374,
	MaxUniformBufferBindings = 35375,
	MaxUniformBlockSize = 35376,
	MaxCombinedVertexUniformComponents = 35377,
	MaxCombinedGeometryUniformComponents = 35378,
	MaxCombinedFragmentUniformComponents = 35379,
	UniformBufferOffsetAlignment = 35380,
	MaxFragmentUniformComponents = 35657,
	MaxVertexUniformComponents = 35658,
	MaxVaryingComponents = 35659,
	MaxVaryingFloats = 35659,
	MaxVertexTextureImageUnits = 35660,
	MaxCombinedTextureImageUnits = 35661,
	FragmentShaderDerivativeHint = 35723,
	CurrentProgram = 35725,
	ImplementationColorReadType = 35738,
	ImplementationColorReadFormat = 35739,
	TextureBinding1DArray = 35868,
	TextureBinding2DArray = 35869,
	MaxGeometryTextureImageUnits = 35881,
	TextureBuffer = 35882,
	MaxTextureBufferSize = 35883,
	TextureBindingBuffer = 35884,
	TextureBufferDataStoreBinding = 35885,
	SampleShading = 35894,
	MinSampleShadingValue = 35895,
	MaxTransformFeedbackSeparateComponents = 35968,
	MaxTransformFeedbackInterleavedComponents = 35978,
	MaxTransformFeedbackSeparateAttribs = 35979,
	StencilBackRef = 36003,
	StencilBackValueMask = 36004,
	StencilBackWritemask = 36005,
	DrawFramebufferBinding = 36006,
	FramebufferBinding = 36006,
	FramebufferBindingExt = 36006,
	RenderbufferBinding = 36007,
	RenderbufferBindingExt = 36007,
	ReadFramebufferBinding = 36010,
	MaxColorAttachments = 36063,
	MaxColorAttachmentsExt = 36063,
	MaxSamples = 36183,
	FramebufferSrgb = 36281,
	MaxGeometryVaryingComponents = 36317,
	MaxVertexVaryingComponents = 36318,
	MaxGeometryUniformComponents = 36319,
	MaxGeometryOutputVertices = 36320,
	MaxGeometryTotalOutputComponents = 36321,
	MaxSubroutines = 36327,
	MaxSubroutineUniformLocations = 36328,
	ShaderBinaryFormats = 36344,
	NumShaderBinaryFormats = 36345,
	ShaderCompiler = 36346,
	MaxVertexUniformVectors = 36347,
	MaxVaryingVectors = 36348,
	MaxFragmentUniformVectors = 36349,
	MaxCombinedTessControlUniformComponents = 36382,
	MaxCombinedTessEvaluationUniformComponents = 36383,
	TransformFeedbackBufferPaused = 36387,
	TransformFeedbackBufferActive = 36388,
	TransformFeedbackBinding = 36389,
	Timestamp = 36392,
	QuadsFollowProvokingVertexConvention = 36428,
	ProvokingVertex = 36431,
	SampleMask = 36433,
	MaxSampleMaskWords = 36441,
	MaxGeometryShaderInvocations = 36442,
	MinFragmentInterpolationOffset = 36443,
	MaxFragmentInterpolationOffset = 36444,
	FragmentInterpolationOffsetBits = 36445,
	MinProgramTextureGatherOffset = 36446,
	MaxProgramTextureGatherOffset = 36447,
	MaxTransformFeedbackBuffers = 36464,
	MaxVertexStreams = 36465,
	PatchVertices = 36466,
	PatchDefaultInnerLevel = 36467,
	PatchDefaultOuterLevel = 36468,
	MaxPatchVertices = 36477,
	MaxTessGenLevel = 36478,
	MaxTessControlUniformComponents = 36479,
	MaxTessEvaluationUniformComponents = 36480,
	MaxTessControlTextureImageUnits = 36481,
	MaxTessEvaluationTextureImageUnits = 36482,
	MaxTessControlOutputComponents = 36483,
	MaxTessPatchComponents = 36484,
	MaxTessControlTotalOutputComponents = 36485,
	MaxTessEvaluationOutputComponents = 36486,
	MaxTessControlUniformBlocks = 36489,
	MaxTessEvaluationUniformBlocks = 36490,
	DrawIndirectBufferBinding = 36675,
	MaxVertexImageUniforms = 37066,
	MaxTessControlImageUniforms = 37067,
	MaxTessEvaluationImageUniforms = 37068,
	MaxGeometryImageUniforms = 37069,
	MaxFragmentImageUniforms = 37070,
	MaxCombinedImageUniforms = 37071,
	ContextRobustAccess = 37107,
	TextureBinding2DMultisample = 37124,
	TextureBinding2DMultisampleArray = 37125,
	MaxColorTextureSamples = 37134,
	MaxDepthTextureSamples = 37135,
	MaxIntegerSamples = 37136,
	MaxVertexOutputComponents = 37154,
	MaxGeometryInputComponents = 37155,
	MaxGeometryOutputComponents = 37156,
	MaxFragmentInputComponents = 37157,
	MaxComputeImageUniforms = 37309,
	ClipOrigin = 37724,
	ClipDepthMode = 37725,
	DeviceUuidExt = 38295,
	DriverUuidExt = 38296,
	DeviceLuidExt = 38297,
	DeviceNodeMaskExt = 38298
}[Flags]
    public enum AttribMask
    {
        CurrentBit = 0x1,
        PointBit = 0x2,
        LineBit = 0x4,
        PolygonBit = 0x8,
        PolygonStippleBit = 0x10,
        PixelModeBit = 0x20,
        LightingBit = 0x40,
        FogBit = 0x80,
        DepthBufferBit = 0x100,
        AccumBufferBit = 0x200,
        StencilBufferBit = 0x400,
        ViewportBit = 0x800,
        TransformBit = 0x1000,
        EnableBit = 0x2000,
        ColorBufferBit = 0x4000,
        HintBit = 0x8000,
        EvalBit = 0x10000,
        ListBit = 0x20000,
        TextureBit = 0x40000,
        ScissorBit = 0x80000,
        MultisampleBit = 0x20000000,
        MultisampleBit3Dfx = 0x20000000,
        MultisampleBitArb = 0x20000000,
        MultisampleBitExt = 0x20000000,
        AllAttribBits = -1
    }public enum HintTarget
    {
        PerspectiveCorrectionHint = 3152,
        PointSmoothHint = 3153,
        LineSmoothHint = 3154,
        PolygonSmoothHint = 3155,
        FogHint = 3156,
        PreferDoublebufferHintPgi = 107000,
        ConserveMemoryHintPgi = 107005,
        ReclaimMemoryHintPgi = 107006,
        NativeGraphicsBeginHintPgi = 107011,
        NativeGraphicsEndHintPgi = 107012,
        AlwaysFastHintPgi = 107020,
        AlwaysSoftHintPgi = 107021,
        AllowDrawObjHintPgi = 107022,
        AllowDrawWinHintPgi = 107023,
        AllowDrawFrgHintPgi = 107024,
        AllowDrawMemHintPgi = 107025,
        StrictDepthfuncHintPgi = 107030,
        StrictLightingHintPgi = 107031,
        StrictScissorHintPgi = 107032,
        FullStippleHintPgi = 107033,
        ClipNearHintPgi = 107040,
        ClipFarHintPgi = 107041,
        WideLineHintPgi = 107042,
        BackNormalsHintPgi = 107043,
        VertexDataHintPgi = 107050,
        VertexConsistentHintPgi = 107051,
        MaterialSideHintPgi = 107052,
        MaxVertexHintPgi = 107053,
        PackCmykHintExt = 32782,
        UnpackCmykHintExt = 32783,
        PhongHintWin = 33003,
        ClipVolumeClippingHintExt = 33008,
        TextureMultiBufferHintSgix = 33070,
        GenerateMipmapHint = 33170,
        GenerateMipmapHintSgis = 33170,
        ProgramBinaryRetrievableHint = 33367,
        ConvolutionHintSgix = 33558,
        ScalebiasHintSgix = 33570,
        LineQualityHintSgix = 33627,
        VertexPreclipSgix = 33774,
        VertexPreclipHintSgix = 33775,
        TextureCompressionHint = 34031,
        TextureCompressionHintArb = 34031,
        VertexArrayStorageHintApple = 34079,
        MultisampleFilterHintNv = 34100,
        TransformHintApple = 34225,
        TextureStorageHintApple = 34236,
        FragmentShaderDerivativeHint = 35723,
        FragmentShaderDerivativeHintArb = 35723,
        FragmentShaderDerivativeHintOes = 35723,
        BinningControlHintQcom = 36784
    }public enum HintMode
    {
        DontCare = 4352,
        Fastest,
        Nicest
    }public enum StencilFunction
    {
        Never = 0x200,
        Less,
        Equal,
        Lequal,
        Greater,
        Notequal,
        Gequal,
        Always
    }public enum StencilOp
    {
        Zero = 0,
        Invert = 5386,
        Keep = 7680,
        Replace = 7681,
        Incr = 7682,
        Decr = 7683,
        IncrWrap = 34055,
        DecrWrap = 34056
    }public enum TextureEnvTarget
    {
        TextureEnv = 8960,
        TextureFilterControl = 34048,
        PointSprite = 34913
    }
    public enum TextureEnvParameter
    {
        AlphaScale = 3356,
        TextureEnvMode = 8704,
        TextureEnvColor = 8705,
        TextureLodBias = 34049,
        CombineRgb = 34161,
        CombineAlpha = 34162,
        RgbScale = 34163,
        Source0Rgb = 34176,
        Src1Rgb = 34177,
        Src2Rgb = 34178,
        Src0Alpha = 34184,
        Src1Alpha = 34185,
        Src2Alpha = 34186,
        Operand0Rgb = 34192,
        Operand1Rgb = 34193,
        Operand2Rgb = 34194,
        Operand0Alpha = 34200,
        Operand1Alpha = 34201,
        Operand2Alpha = 34202,
        CoordReplace = 34914
    }public enum TextureEnvModeCombine
    {
        Add = 260,
        Replace = 7681,
        Modulate = 8448,
        Subtract = 34023,
        AddSigned = 34164,
        Interpolate = 34165,
        Dot3Rgb = 34478,
        Dot3Rgba = 34479
    }



    public enum TextureWrapMode
    {
        Clamp = 10496,
        Repeat = 10497,
        ClampToBorder = 33069,
        ClampToBorderArb = 33069,
        ClampToBorderNv = 33069,
        ClampToBorderSgis = 33069,
        ClampToEdge = 33071,
        ClampToEdgeSgis = 33071,
        MirroredRepeat = 33648
    }

    public enum PixelFormat
    {
        UnsignedShort = 5123,
        UnsignedInt = 5125,
        ColorIndex = 6400,
        StencilIndex = 6401,
        DepthComponent = 6402,
        Red = 6403,
        RedExt = 6403,
        Green = 6404,
        Blue = 6405,
        Alpha = 6406,
        Rgb = 6407,
        Rgba = 6408,
        Luminance = 6409,
        LuminanceAlpha = 6410,
        AbgrExt = 0x8000,
        CmykExt = 32780,
        CmykaExt = 32781,
        Bgr = 32992,
        Bgra = 32993,
        Ycrcb422Sgix = 33211,
        Ycrcb444Sgix = 33212,
        Rg = 33319,
        RgInteger = 33320,
        R5G6B5IccSgix = 33894,
        R5G6B5A8IccSgix = 33895,
        Alpha16IccSgix = 33896,
        Luminance16IccSgix = 33897,
        Luminance16Alpha8IccSgix = 33899,
        DepthStencil = 34041,
        RedInteger = 36244,
        GreenInteger = 36245,
        BlueInteger = 36246,
        AlphaInteger = 36247,
        RgbInteger = 36248,
        RgbaInteger = 36249,
        BgrInteger = 36250,
        BgraInteger = 36251
    }

    public enum PixelInternalFormat
    {
        DepthComponent = 6402,
        Alpha = 6406,
        Rgb = 6407,
        Rgba = 6408,
        Luminance = 6409,
        LuminanceAlpha = 6410,
        R3G3B2 = 10768,
        Alpha4 = 32827,
        Alpha8 = 32828,
        Alpha12 = 32829,
        Alpha16 = 32830,
        Luminance4 = 32831,
        Luminance8 = 32832,
        Luminance12 = 32833,
        Luminance16 = 32834,
        Luminance4Alpha4 = 32835,
        Luminance6Alpha2 = 32836,
        Luminance8Alpha8 = 32837,
        Luminance12Alpha4 = 32838,
        Luminance12Alpha12 = 32839,
        Luminance16Alpha16 = 32840,
        Intensity = 32841,
        Intensity4 = 32842,
        Intensity8 = 32843,
        Intensity12 = 32844,
        Intensity16 = 32845,
        Rgb2Ext = 32846,
        Rgb4 = 32847,
        Rgb5 = 32848,
        Rgb8 = 32849,
        Rgb10 = 32850,
        Rgb12 = 32851,
        Rgb16 = 32852,
        Rgba2 = 32853,
        Rgba4 = 32854,
        Rgb5A1 = 32855,
        Rgba8 = 32856,
        Rgb10A2 = 32857,
        Rgba12 = 32858,
        Rgba16 = 32859,
        DualAlpha4Sgis = 33040,
        DualAlpha8Sgis = 33041,
        DualAlpha12Sgis = 33042,
        DualAlpha16Sgis = 33043,
        DualLuminance4Sgis = 33044,
        DualLuminance8Sgis = 33045,
        DualLuminance12Sgis = 33046,
        DualLuminance16Sgis = 33047,
        DualIntensity4Sgis = 33048,
        DualIntensity8Sgis = 33049,
        DualIntensity12Sgis = 33050,
        DualIntensity16Sgis = 33051,
        DualLuminanceAlpha4Sgis = 33052,
        DualLuminanceAlpha8Sgis = 33053,
        QuadAlpha4Sgis = 33054,
        QuadAlpha8Sgis = 33055,
        QuadLuminance4Sgis = 33056,
        QuadLuminance8Sgis = 33057,
        QuadIntensity4Sgis = 33058,
        QuadIntensity8Sgis = 33059,
        DepthComponent16 = 33189,
        DepthComponent16Sgix = 33189,
        DepthComponent24 = 33190,
        DepthComponent24Sgix = 33190,
        DepthComponent32 = 33191,
        DepthComponent32Sgix = 33191,
        CompressedRed = 33317,
        CompressedRg = 33318,
        R8 = 33321,
        R16 = 33322,
        Rg8 = 33323,
        Rg16 = 33324,
        R16f = 33325,
        R32f = 33326,
        Rg16f = 33327,
        Rg32f = 33328,
        R8i = 33329,
        R8ui = 33330,
        R16i = 33331,
        R16ui = 33332,
        R32i = 33333,
        R32ui = 33334,
        Rg8i = 33335,
        Rg8ui = 33336,
        Rg16i = 33337,
        Rg16ui = 33338,
        Rg32i = 33339,
        Rg32ui = 33340,
        CompressedRgbS3tcDxt1Ext = 33776,
        CompressedRgbaS3tcDxt1Ext = 33777,
        CompressedRgbaS3tcDxt3Ext = 33778,
        CompressedRgbaS3tcDxt5Ext = 33779,
        RgbIccSgix = 33888,
        RgbaIccSgix = 33889,
        AlphaIccSgix = 33890,
        LuminanceIccSgix = 33891,
        IntensityIccSgix = 33892,
        LuminanceAlphaIccSgix = 33893,
        R5G6B5IccSgix = 33894,
        R5G6B5A8IccSgix = 33895,
        Alpha16IccSgix = 33896,
        Luminance16IccSgix = 33897,
        Intensity16IccSgix = 33898,
        Luminance16Alpha8IccSgix = 33899,
        CompressedAlpha = 34025,
        CompressedLuminance = 34026,
        CompressedLuminanceAlpha = 34027,
        CompressedIntensity = 34028,
        CompressedRgb = 34029,
        CompressedRgba = 34030,
        DepthStencil = 34041,
        Rgba32f = 34836,
        Rgb32f = 34837,
        Rgba16f = 34842,
        Rgb16f = 34843,
        Depth24Stencil8 = 35056,
        R11fG11fB10f = 35898,
        Rgb9E5 = 35901,
        Srgb = 35904,
        Srgb8 = 35905,
        SrgbAlpha = 35906,
        Srgb8Alpha8 = 35907,
        SluminanceAlpha = 35908,
        Sluminance8Alpha8 = 35909,
        Sluminance = 35910,
        Sluminance8 = 35911,
        CompressedSrgb = 35912,
        CompressedSrgbAlpha = 35913,
        CompressedSluminance = 35914,
        CompressedSluminanceAlpha = 35915,
        CompressedSrgbS3tcDxt1Ext = 35916,
        CompressedSrgbAlphaS3tcDxt1Ext = 35917,
        CompressedSrgbAlphaS3tcDxt3Ext = 35918,
        CompressedSrgbAlphaS3tcDxt5Ext = 35919,
        DepthComponent32f = 36012,
        Depth32fStencil8 = 36013,
        Rgba32ui = 36208,
        Rgb32ui = 36209,
        Rgba16ui = 36214,
        Rgb16ui = 36215,
        Rgba8ui = 36220,
        Rgb8ui = 36221,
        Rgba32i = 36226,
        Rgb32i = 36227,
        Rgba16i = 36232,
        Rgb16i = 36233,
        Rgba8i = 36238,
        Rgb8i = 36239,
        Float32UnsignedInt248Rev = 36269,
        CompressedRedRgtc1 = 36283,
        CompressedSignedRedRgtc1 = 36284,
        CompressedRgRgtc2 = 36285,
        CompressedSignedRgRgtc2 = 36286,
        CompressedRgbaBptcUnorm = 36492,
        CompressedSrgbAlphaBptcUnorm = 36493,
        CompressedRgbBptcSignedFloat = 36494,
        CompressedRgbBptcUnsignedFloat = 36495,
        R8Snorm = 36756,
        Rg8Snorm = 36757,
        Rgb8Snorm = 36758,
        Rgba8Snorm = 36759,
        R16Snorm = 36760,
        Rg16Snorm = 36761,
        Rgb16Snorm = 36762,
        Rgba16Snorm = 36763,
        Rgb10A2ui = 36975,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4
    }

    public enum MatrixMode
    {
        Modelview = 5888,
        Modelview0Ext = 5888,
        Projection = 5889,
        Texture = 5890,
        Color = 6144
    }

    public enum PixelType
    {
        Byte = 5120,
        UnsignedByte = 5121,
        Short = 5122,
        UnsignedShort = 5123,
        Int = 5124,
        UnsignedInt = 5125,
        Float = 5126,
        HalfFloat = 5131,
        Bitmap = 6656,
        UnsignedByte332 = 32818,
        UnsignedByte332Ext = 32818,
        UnsignedShort4444 = 32819,
        UnsignedShort4444Ext = 32819,
        UnsignedShort5551 = 32820,
        UnsignedShort5551Ext = 32820,
        UnsignedInt8888 = 32821,
        UnsignedInt8888Ext = 32821,
        UnsignedInt1010102 = 32822,
        UnsignedInt1010102Ext = 32822,
        UnsignedByte233Reversed = 33634,
        UnsignedShort565 = 33635,
        UnsignedShort565Reversed = 33636,
        UnsignedShort4444Reversed = 33637,
        UnsignedShort1555Reversed = 33638,
        UnsignedInt8888Reversed = 33639,
        UnsignedInt2101010Reversed = 33640,
        UnsignedInt248 = 34042,
        UnsignedInt10F11F11FRev = 35899,
        UnsignedInt5999Rev = 35902,
        Float32UnsignedInt248Rev = 36269
    }
    public enum TextureParameterName
{
	TextureWidth = 0x1000,
	TextureHeight = 4097,
	TextureComponents = 4099,
	TextureInternalFormat = 4099,
	TextureBorderColor = 4100,
	TextureBorderColorNv = 4100,
	TextureBorder = 4101,
	TextureMagFilter = 10240,
	TextureMinFilter = 10241,
	TextureWrapS = 10242,
	TextureWrapT = 10243,
	TextureRedSize = 32860,
	TextureGreenSize = 32861,
	TextureBlueSize = 32862,
	TextureAlphaSize = 32863,
	TextureLuminanceSize = 32864,
	TextureIntensitySize = 32865,
	TexturePriority = 32870,
	TexturePriorityExt = 32870,
	TextureResident = 32871,
	TextureDepth = 32881,
	TextureDepthExt = 32881,
	TextureWrapR = 32882,
	TextureWrapRExt = 32882,
	TextureWrapROes = 32882,
	DetailTextureLevelSgis = 32922,
	DetailTextureModeSgis = 32923,
	DetailTextureFuncPointsSgis = 32924,
	SharpenTextureFuncPointsSgis = 32944,
	ShadowAmbientSgix = 32959,
	TextureCompareFailValue = 32959,
	DualTextureSelectSgis = 33060,
	QuadTextureSelectSgis = 33061,
	ClampToBorder = 33069,
	ClampToEdge = 33071,
	Texture4DsizeSgis = 33078,
	TextureWrapQSgis = 33079,
	TextureMinLod = 33082,
	TextureMinLodSgis = 33082,
	TextureMaxLod = 33083,
	TextureMaxLodSgis = 33083,
	TextureBaseLevel = 33084,
	TextureBaseLevelSgis = 33084,
	TextureMaxLevel = 33085,
	TextureMaxLevelSgis = 33085,
	TextureFilter4SizeSgis = 33095,
	TextureClipmapCenterSgix = 33137,
	TextureClipmapFrameSgix = 33138,
	TextureClipmapOffsetSgix = 33139,
	TextureClipmapVirtualDepthSgix = 33140,
	TextureClipmapLodOffsetSgix = 33141,
	TextureClipmapDepthSgix = 33142,
	PostTextureFilterBiasSgix = 33145,
	PostTextureFilterScaleSgix = 33146,
	TextureLodBiasSSgix = 33166,
	TextureLodBiasTSgix = 33167,
	TextureLodBiasRSgix = 33168,
	GenerateMipmap = 33169,
	GenerateMipmapSgis = 33169,
	TextureCompareSgix = 33178,
	TextureCompareOperatorSgix = 33179,
	TextureLequalRSgix = 33180,
	TextureGequalRSgix = 33181,
	TextureMaxClampSSgix = 33641,
	TextureMaxClampTSgix = 33642,
	TextureMaxClampRSgix = 33643,
	TextureLodBias = 34049,
	DepthTextureMode = 34891,
	TextureCompareMode = 34892,
	TextureCompareFunc = 34893,
	TextureSwizzleR = 36418,
	TextureSwizzleG = 36419,
	TextureSwizzleB = 36420,
	TextureSwizzleA = 36421,
	TextureSwizzleRgba = 36422,
	DepthStencilTextureMode = 37098,
	TextureTilingExt = 38272
}
    public enum TextureMinFilter
    {
        Nearest = 9728,
        Linear = 9729,
        NearestMipmapNearest = 9984,
        LinearMipmapNearest = 9985,
        NearestMipmapLinear = 9986,
        LinearMipmapLinear = 9987,
        Filter4Sgis = 33094,
        LinearClipmapLinearSgix = 33136,
        PixelTexGenQCeilingSgix = 33156,
        PixelTexGenQRoundSgix = 33157,
        PixelTexGenQFloorSgix = 33158,
        NearestClipmapNearestSgix = 33869,
        NearestClipmapLinearSgix = 33870,
        LinearClipmapNearestSgix = 33871
    }
    public enum TextureMagFilter
    {
        Nearest = 9728,
        Linear = 9729,
        LinearDetailSgis = 32919,
        LinearDetailAlphaSgis = 32920,
        LinearDetailColorSgis = 32921,
        LinearSharpenSgis = 32941,
        LinearSharpenAlphaSgis = 32942,
        LinearSharpenColorSgis = 32943,
        Filter4Sgis = 33094,
        PixelTexGenQCeilingSgix = 33156,
        PixelTexGenQRoundSgix = 33157,
        PixelTexGenQFloorSgix = 33158
    }
    [Flags]
    public enum ClearBufferMask
    {
        None = 0x0,
        DepthBufferBit = 0x100,
        AccumBufferBit = 0x200,
        StencilBufferBit = 0x400,
        ColorBufferBit = 0x4000,
        CoverageBufferBitNv = 0x8000
    }
    public enum LightModelParameter
    {
        LightModelLocalViewer = 2897,
        LightModelTwoSide = 2898,
        LightModelAmbient = 2899,
        LightModelColorControl = 33272,
        LightModelColorControlExt = 33272
    }
  
    public enum BlendingFactor
    {
        Zero = 0,
        SrcColor = 768,
        OneMinusSrcColor = 769,
        SrcAlpha = 770,
        OneMinusSrcAlpha = 771,
        DstAlpha = 772,
        OneMinusDstAlpha = 773,
        DstColor = 774,
        OneMinusDstColor = 775,
        SrcAlphaSaturate = 776,
        ConstantColor = 32769,
        OneMinusConstantColor = 32770,
        ConstantAlpha = 32771,
        OneMinusConstantAlpha = 32772,
        Src1Alpha = 34185,
        Src1Color = 35065,
        One = 1
    }




    public static class GL
    {




        public static void LoadMatrix(ref Matrix4 modelview)
        {
            throw new NotImplementedException();
        }

        public static void ClearColor(Color lightBlue)
        {
            throw new NotImplementedException();
        }


        public static void BlendFunc(BlendingFactor srcAlpha, BlendingFactor oneMinusSrcAlpha)
        {
            throw new NotImplementedException();
        }

        public static void Enable(EnableCap texture2D)
        {
            throw new NotImplementedException();
        }

        public static void BindTexture(TextureTarget texture2D, int gltextureid)
        {
            throw new NotImplementedException();
        }

        public static void Begin(PrimitiveType triangleFan)
        {
            throw new NotImplementedException();
        }

        public static void TexCoord2(int p0, int p1)
        {
            throw new NotImplementedException();
        }

        public static void Vertex2(float p0, float p1)
        {
            throw new NotImplementedException();
        }

        public static void End()
        {
            throw new NotImplementedException();
        }

        public static void Disable(EnableCap texture2D)
        {
            throw new NotImplementedException();
        }

        public static void LineWidth(float pennWidth)
        {
            throw new NotImplementedException();
        }

        public static void Color4(Color pennColor)
        {
            throw new NotImplementedException();
        }

        public static void GetInteger(GetPName viewport, int[] viewPort)
        {
            throw new NotImplementedException();
        }

        public static void MatrixMode(MatrixMode projection)
        {
            throw new NotImplementedException();
        }

        public static void LoadIdentity()
        {
            throw new NotImplementedException();
        }

        public static void Ortho(int i, int width, int height, int i1, int i2, int i3)
        {
            throw new NotImplementedException();
        }

        public static void PushAttrib(AttribMask depthBufferBit)
        {
            throw new NotImplementedException();
        }

        public static string GetString(StringName vendor)
        {
            throw new NotImplementedException();
        }

        public static void Hint(HintTarget perspectiveCorrectionHint, HintMode nicest)
        {
            throw new NotImplementedException();
        }

        public static void GenTextures(int i, out int texture)
        {
            throw new NotImplementedException();
        }

        public static void ShadeModel(ShadingModel smooth)
        {
            throw new NotImplementedException();
        }

        public static void Viewport(int i, int i1, int width, int height)
        {
            throw new NotImplementedException();
        }

        public static void TexImage2D(TextureTarget texture2D, int i, PixelInternalFormat rgba, int dataWidth, int dataHeight, int i1, PixelFormat bgra, PixelType unsignedByte, IntPtr dataScan0)
        {
            throw new NotImplementedException();
        }

        public static void DepthMask(bool p0)
        {
            throw new NotImplementedException();
        }

        public static void Clear(ClearBufferMask colorBufferBit)
        {
            throw new NotImplementedException();
        }

        public static void TexSubImage2D(TextureTarget texture2D, int i, int i1, int i2, int dataWidth, int dataHeight, PixelFormat bgra, PixelType unsignedByte, IntPtr dataScan0)
        {
            throw new NotImplementedException();
        }

        public static bool IsEnabled(EnableCap polygonSmooth)
        {
            throw new NotImplementedException();
        }

        public static void TexParameter(TextureTarget texture2D, TextureParameterName textureMinFilter, int linear)
        {
            throw new NotImplementedException();
        }

        public static void Vertex3(double p0, double heightscale, double increment)
        {
            throw new NotImplementedException();
        }

        public static void TexCoord2(double imgx, double imgy)
        {
            throw new NotImplementedException();
        }

        public static void Color3(Color white)
        {
            throw new NotImplementedException();
        }

        public static void LightModel(LightModelParameter lightModelAmbient, float[] floats)
        {
            throw new NotImplementedException();
        }

        public static void Flush()
        {
            throw new NotImplementedException();
        }

        public static void ClearStencil(int i)
        {
            throw new NotImplementedException();
        }

        public static void ColorMask(bool p0, bool p1, bool p2, bool p3)
        {
            throw new NotImplementedException();
        }

        public static void StencilFunc(StencilFunction always, int p1, int p2)
        {
            throw new NotImplementedException();
        }

        public static void StencilOp(StencilOp invert, StencilOp p1, StencilOp p2)
        {
            throw new NotImplementedException();
        }

        public static void Rotate(float angle, int i, int i1, int i2)
        {
            throw new NotImplementedException();
        }

        public static void Translate(float f, float f1, float f2)
        {
            throw new NotImplementedException();
        }

        internal static void Color4(float v1, float v2, float v3, float v4)
        {
            throw new NotImplementedException();
        }

        public static void Vertex2(double width, double y1)
        {
            throw new NotImplementedException();
        }

        public static void ReadPixels(int i, int i1, int clientSizeWidth, int clientSizeHeight, PixelFormat bgr, PixelType unsignedByte, IntPtr dataScan0)
        {
            throw new NotImplementedException();
        }

        public static void TexEnv(TextureEnvTarget textureEnv, TextureEnvParameter textureEnvMode, float replace)
        {
            throw new NotImplementedException();
        }

        public static void DeleteTexture(int texidGltextureid)
        {
            throw new NotImplementedException();
        }

        public static void PointSize(int p0)
        {
            throw new NotImplementedException();
        }

        public static void Vertex3(Vector3 vec)
        {
            throw new NotImplementedException();
        }
    }
}

namespace DotSpatial.Data{}
namespace DotSpatial.Projections{
    public class ProjectionInfo
    {
        public void ParseEsriString(string readLine)
        {
            throw new NotImplementedException();
        }
    }

    public class FeatureSet:IFeatureSet
    {
        public static IFeatureSet Open(string file)
        {
            throw new NotImplementedException();
        }

        public void CopyTableSchema(DataTable sourceTable)
        {
            throw new NotImplementedException();
        }

        public void FillAttributes()
        {
            throw new NotImplementedException();
        }

        public List<int> Find(string filterExpression)
        {
            throw new NotImplementedException();
        }

        public void InitializeVertices()
        {
            throw new NotImplementedException();
        }

        public void InvalidateVertices()
        {
            throw new NotImplementedException();
        }

        public bool RemoveShapeAt(int index)
        {
            throw new NotImplementedException();
        }

        public void RemoveShapesAt(IEnumerable<int> indices)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void SaveAs(string fileName, bool overwrite)
        {
            throw new NotImplementedException();
        }

        public List<int> SelectIndexByAttribute(string filterExpression)
        {
            throw new NotImplementedException();
        }

        public void UpdateExtent()
        {
            throw new NotImplementedException();
        }

        public int NumRows()
        {
            throw new NotImplementedException();
        }

        public bool AttributesPopulated
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public DataTable DataTable
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public string Filename
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public bool IndexMode
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        double[] IFeatureSet.Vertex
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        double[] IFeatureSet.Z
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public double[] M
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public bool VerticesAreValid => throw new NotImplementedException();

        public event EventHandler VerticesInvalidated;
        public List<string> GetColumnNames(string xlsFilePath)
        {
            throw new NotImplementedException();
        }

        public IFeatureSet Join(string xlsFilePath, string localJoinField, string xlsJoinField)
        {
            throw new NotImplementedException();
        }

        public void AddFid()
        {
            throw new NotImplementedException();
        }

        public void CopyFeatures(IFeatureSet source, bool copyAttributes)
        {
            throw new NotImplementedException();
        }

        public IFeatureSet CopySubset(List<int> indices)
        {
            throw new NotImplementedException();
        }

        public IFeatureSet CopySubset(string filterExpression)
        {
            throw new NotImplementedException();
        }

        public void CopyTableSchema(IFeatureSet source)
        {
            throw new NotImplementedException();
        }

        public IEnumerable Features
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public object Vertex
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public double Z
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        List<Feature> IFeatureSet.Features { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }

public interface IFeatureSet 
{
	bool AttributesPopulated
	{
		get;
		set;
	}


	DataTable DataTable
	{
		get;
		set;
	}


	string Filename
	{
		get;
		set;
	}

	bool IndexMode
	{
		get;
		set;
	}

	double[] Vertex
	{
		get;
		set;
	}

	double[] Z
	{
		get;
		set;
	}

	double[] M
	{
		get;
		set;
	}

	bool VerticesAreValid
	{
		get;
	}

    List<Feature> Features { get; set; }

    event EventHandler VerticesInvalidated;


	List<string> GetColumnNames(string xlsFilePath);

	IFeatureSet Join(string xlsFilePath, string localJoinField, string xlsJoinField);


	void AddFid();

	void CopyFeatures(IFeatureSet source, bool copyAttributes);

	IFeatureSet CopySubset(List<int> indices);

	IFeatureSet CopySubset(string filterExpression);

	void CopyTableSchema(IFeatureSet source);

	void CopyTableSchema(DataTable sourceTable);

	void FillAttributes();


	[Obsolete("Use SelectIndexByAttribute(filterExpression) instead.")]
	List<int> Find(string filterExpression);

	void InitializeVertices();

	void InvalidateVertices();

	bool RemoveShapeAt(int index);

	void RemoveShapesAt(IEnumerable<int> indices);


	void Save();

	void SaveAs(string fileName, bool overwrite);


	List<int> SelectIndexByAttribute(string filterExpression);

	void UpdateExtent();
    int NumRows();
}

public class Feature    
{
    public List<Coord> Coordinates { get; set; }
}

public class Coord
{
    public double X { get; set; }

    public double Y { get; set; }

    public double Z { get; set; }
}

public class Reproject
    {
        public static void ReprojectPoints(double[] xyarray, double[] zarray, ProjectionInfo pStart, ProjectionInfo pEsriEnd, int i, int i1)
        {
            throw new NotImplementedException();
        }
    }

    public class KnownCoordinateSystems
    {
        public class Geographic
        {
            public class World
            {
                public static ProjectionInfo WGS1984;
            }
        }
    }
}
namespace DotSpatial.Symbology {}

namespace SkiaSharp.Views.Desktop
{
[DefaultEvent("PaintSurface")]
[DefaultProperty("Name")]
public class SKControl : Control
{
	private readonly bool designMode;

	private Bitmap bitmap;

	[Bindable(false)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public SKSize CanvasSize
	{
		get
		{
			if (bitmap != null)
			{
				return new SKSize(bitmap.Width, bitmap.Height);
			}
			return SKSize.Empty;
		}
	}

	[Category("Appearance")]
	public event EventHandler<SKPaintSurfaceEventArgs> PaintSurface;

	public SKControl()
	{
		DoubleBuffered = true;
		SetStyle(ControlStyles.ResizeRedraw, value: true);
		designMode = (base.DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime);
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		if (!designMode)
		{
			base.OnPaint(e);
			SKImageInfo info = CreateBitmap();
			if (info.Width != 0 && info.Height != 0)
			{
				BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, base.Width, base.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
				using (SKSurface sKSurface = SKSurface.Create(info, bitmapData.Scan0, bitmapData.Stride))
				{
					OnPaintSurface(new SKPaintSurfaceEventArgs(sKSurface, info));
					sKSurface.Canvas.Flush();
				}
				bitmap.UnlockBits(bitmapData);
				e.Graphics.DrawImage(bitmap, 0, 0);
			}
		}
	}

	protected virtual void OnPaintSurface(SKPaintSurfaceEventArgs e)
	{
		this.PaintSurface?.Invoke(this, e);
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		FreeBitmap();
	}

	private SKImageInfo CreateBitmap()
	{
		SKImageInfo result = new SKImageInfo(base.Width, base.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
		if (bitmap == null || bitmap.Width != result.Width || bitmap.Height != result.Height)
		{
			FreeBitmap();
			if (result.Width != 0 && result.Height != 0)
			{
				bitmap = new Bitmap(result.Width, result.Height, PixelFormat.Format32bppPArgb);
			}
		}
		return result;
	}

	private void FreeBitmap()
	{
		if (bitmap != null)
		{
			bitmap.Dispose();
			bitmap = null;
		}
	}
}
}

namespace MissionPlanner.Log
{
    public partial class LogIndex : Form
    {
    }
}

namespace System.Windows.Forms
{


 

    public class WebBrowser : Control
    {
        public bool CanGoBack { get; internal set; }

        public object Url { get; set; }

        public bool CanGoForward { get; set; }

        public string DocumentText { get; set; }

        public HtmlDocument Document { get; set; }

        public event EventHandler<WebBrowserNavigatingEventArgs> Navigating;
        public event EventHandler<WebBrowserNavigatedEventArgs> Navigated;

        public void Navigate(Uri authorizeUri)
        {
            
        }

        public void GoBack()
        {
          
        }

        public void GoForward()
        {
         
        }

        public void Navigate(string authorizeUri)
        {
            
        }
    }

    public class HtmlDocument
    {
        public void InvokeScript(string script)
        {
        }
    }

    public class WebBrowserNavigatedEventArgs : EventArgs
    {
        public Uri Url;
    }

    public class WebBrowserNavigatingEventArgs : EventArgs
    {
        public Uri Url;

        public bool Cancel;
    }
}

public class GdalConfiguration
{
    public static void ConfigureGdal()
    {
        
    }

    public static void ConfigureOgr()
    {
        
    }
}

public class GdiGraphics: Graphics
{
    public GdiGraphics(SKSurface surface) : base(surface)
    {
    }

    public GdiGraphics(IntPtr handle, int width, int height) : base(handle, width, height)
    {
    }

    public GdiGraphics(Graphics fromImage): base(fromImage.Surface)
    {
    }
}
public class SkiaGraphics: Graphics
{
    public SkiaGraphics(SKSurface surface) : base(surface)
    {
    }

    public SkiaGraphics(IntPtr handle, int width, int height) : base(handle, width, height)
    {
    }
}

namespace System.Speech.Synthesis
{
    public class SpeechSynthesizer
    {
        public SynthesizerState State { get; set; }

        public void SpeakAsyncCancelAll()
        {

        }

        public void Dispose()
        {

        }

        public void SpeakAsync(string text)
        {

        }
    }

    public enum SynthesizerState
    {
        Ready,
        Speaking
    }
}



namespace System.Management
{
    public class ObjectQuery
    {
        private string v;

        public ObjectQuery(string v)
        {
            this.v = v;
        }
    }

    public class ManagementObjectSearcher: IDisposable
    {
        private ObjectQuery query;

        public ManagementObjectSearcher(ObjectQuery query)
        {
            this.query = query;
        }

        public IEnumerable<ManagementObject> Get()
        {
            return new List<ManagementObject>();
        }

        public void Dispose()
        {
        }
    }

    public class ManagementObject
    {
        public PropertyDataCollection Properties = new PropertyDataCollection();

        public class PropertyDataCollection : ICollection
        {
         

            public IEnumerator<extratype> GetEnumerator()
            {
                yield return new extratype();
            }

            public extratype this[string pnpdeviceid]
            {
                get { return null; }
            }

            public void CopyTo(Array array, int index)
            {
                
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public int Count { get; }

            public bool IsSynchronized { get; }

            public object SyncRoot { get; }
        }

        public class extratype
        {
            public object Name { get; set; }
            public object Value { get; set; }
        }
    }
}