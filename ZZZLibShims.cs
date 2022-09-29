using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Scripting.Hosting;
using MissionPlanner;
using MissionPlanner.Utilities;
using SkiaSharp;

namespace tlogThumbnailHandler
{
    public class tlogThumbnailHandler
    {
        public static string queuefile = "queue.txt";

    }
}

namespace Microsoft.Scripting.Hosting
{
    public sealed class ScriptSource
    {
        public object Execute(ScriptScope scope)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class ScriptEngine
    {
        public ScriptSource  CreateScriptSourceFromString(string scriptsrc)
        {
            throw new NotImplementedException();
        }

        public void SetSearchPaths(object paths)
        {
            throw new NotImplementedException();
        }

        public ScriptScope CreateScope()
        {
            throw new NotImplementedException();
        }

        public ICollection<string> GetSearchPaths()
        {
            throw new NotImplementedException();
        }

        public ScriptRuntime Runtime { get; set; }

        public TService GetService<TService>(params object[] args) where TService : class
        {
            throw new NotImplementedException();
        }

        public ScriptSource CreateScriptSourceFromFile(string filename)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class ExceptionOperations
    {
        public string FormatException(Exception p0)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class ScriptScope
    {
        public void SetVariable(string name, object value)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class ScriptRuntime
    {
        public void Shutdown()
        {
            throw new NotImplementedException();
        }

        public void LoadAssembly(Assembly ass)
        {
            throw new NotImplementedException();
        }

        public ScriptIO IO { get; set; }
    }

    public sealed class ScriptIO
    {
        public void SetErrorOutput(MemoryStream memoryStream, StringRedirectWriter outputWriter)
        {
            throw new NotImplementedException();
        }

        public void SetOutput(MemoryStream memoryStream, StringRedirectWriter outputWriter)
        {
            throw new NotImplementedException();
        }
    }
}

namespace Microsoft.Scripting.Runtime
{

    public sealed class OperationFailed
    {
        public static readonly OperationFailed Value = new OperationFailed();

        private OperationFailed()
        {
        }
    }
}


namespace Microsoft.Scripting.Utils
{

    public static class ContractUtils
    {
        public static void RequiresNotNull(object value, string paramName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }

}

public delegate int GetInt();

namespace  IronPython.Runtime
{

}

namespace IronPython.Runtime.Operations
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
    public MissionPlanner.Utilities.Vector3 rpy;
    
    public MissionPlanner.Utilities.Vector3 Velocity;

    public PointLatLngAlt LocationCenter { get; set; }

    public List<Locationwp> WPs { get; set; }
}

public class OpenGLtest2: OpenGLtest
{
    public static OpenGLtest2 instance;

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
    public class PromptBuilder
    {
        private CultureInfo currentUICulture;
        private string text;

        public PromptBuilder(CultureInfo currentUICulture)
        {
            this.currentUICulture = currentUICulture;
        }

        public void AppendText(string text)
        {
            this.text = text;
        }

        public static implicit operator string(PromptBuilder d) => d.text;
    }

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