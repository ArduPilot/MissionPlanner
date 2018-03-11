using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Permissions;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using Sonic;
using DirectShow;
using System.Drawing;
using System.IO;

#region Assembly

[assembly: AssemblyTitle("DirectShow BaseClasses")]
[assembly: AssemblyDescription(".NET Implementation of DirectShow BaseClasses")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("")]
[assembly: AssemblyCopyright("Copyright © Maxim Kartavenkov aka Sonic 2012")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(false)]
[assembly: ComVisible(true)]
[assembly: Guid("C842BC09-EFAE-417f-9EAE-6783CDC173A5")]
[assembly: AssemblyVersion("1.0.0.6")]
[assembly: AssemblyFileVersion("1.0.0.6")]

#endregion

namespace DirectShow.BaseClasses
{
    #region AMovieSetup Attribute

    [ComVisible(false)]
    [AttributeUsage(AttributeTargets.Class)]
    public class AMovieSetup : Attribute
    {
        #region Constants

        public const string CLSID_LegacyAmFilterCategory = "083863F1-70DE-11d0-BD40-00A0C911CE86";
        public const string CLSID_VideoInputDeviceCategory = "860BB310-5D01-11d0-BD3B-00A0C911CE86";
        public const string CLSID_VideoCompressorCategory = "33D9A760-90C8-11d0-BD43-00A0C911CE86";
        public const string CLSID_AudioCompressorCategory = "33D9A761-90C8-11d0-BD43-00A0C911CE86";
        public const string CLSID_AudioInputDeviceCategory = "33D9A762-90C8-11d0-BD43-00A0C911CE86";
        public const string CLSID_AudioRendererCategory = "E0F158E1-CB04-11d0-BD4E-00A0C911CE86";
        public const string CLSID_MidiRendererCategory = "4EFE2452-168A-11d1-BC76-00C04FB9453B";

        #endregion

        #region Varables

        protected bool m_bShouldRegister = true;
        protected Merit m_Merit = Merit.DoNotUse;
        protected int m_iVersion = 1;
        protected string m_sName = null;
        protected Guid m_Category = Guid.Empty;

        #endregion

        #region Constructor

        public AMovieSetup()
        {

        }

        public AMovieSetup(bool _register)
            :this()
        {
            m_bShouldRegister = _register;
        }

        public AMovieSetup(string _name)
            :this()
        {
            m_sName = _name;
        }

        public AMovieSetup(Merit _merit)
            :this()
        {
            m_Merit = _merit;
        }

        public AMovieSetup(Merit _merit, string _category)
            : this(_merit)
        {
            m_Category = new Guid(_category);
        }

        public AMovieSetup(string _name, Merit _merit)
            : this(_name)
        {
            m_Merit = _merit;
        }

        public AMovieSetup(string _name, Merit _merit, string _category)
            : this(_name, _merit)
        {
            m_Category = new Guid(_category);
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return m_sName; }
        }

        public bool ShouldRegister
        {
            get { return m_bShouldRegister; }
        }

        public int Version
        {
            get { return m_iVersion; }
        }

        public Merit FilterMerit
        {
            get { return m_Merit; }
        }

        public Guid Category
        {
            get { return m_Category; }
        }

        #endregion
    }

    #endregion

    #region PropertyPages Attribute

    [ComVisible(false)]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PropPageSetup : Attribute
    {
        #region Variables

        private List<Guid> m_Guids = new List<Guid>();

        #endregion

        #region Constructor

        public PropPageSetup(string _guid)
        {
            m_Guids.Add(new Guid(_guid));
        }

        public PropPageSetup(string _guid1, string _guid2)
            : this(_guid1)
        {
            m_Guids.Add(new Guid(_guid2));
        }

        public PropPageSetup(string _guid1, string _guid2, string _guid3)
            :this(_guid1,_guid2)
        {
            m_Guids.Add(new Guid(_guid3));
        }

        public PropPageSetup(Type _type)
        {
            m_Guids.Add(_type.GUID);
        }

        public PropPageSetup(Type _type1, Type _type2)
            : this(_type1)
        {
            m_Guids.Add(_type2.GUID);
        }

        public PropPageSetup(Type _type1,Type _type2,Type _type3)
            :this(_type1,_type2)
        {
            m_Guids.Add(_type3.GUID);
        }

        #endregion

        #region Properties

        public List<Guid> Guids
        {
            get { return m_Guids; }
        }

        #endregion
    }

    #endregion

    #region File Extension Register Attribute

    [ComVisible(false)]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RegisterFileExtension : Attribute
    {
        #region Variables

        private string m_sExtension = "";
        private Guid m_MediaType = Guid.Empty;
        private Guid m_SubType = Guid.Empty;

        #endregion

        #region Constructor

        public RegisterFileExtension(string _extension)
        {
            m_sExtension = _extension;
        }

        public RegisterFileExtension(string _extension,string _MediaType,string _SubType)
            : this(_extension)
        {
            if (!String.IsNullOrEmpty(_MediaType))
            {
                m_MediaType = new Guid(_MediaType);
            }
            if (!String.IsNullOrEmpty(_SubType))
            {
                m_SubType = new Guid(_SubType);
            }
        }

        #endregion

        #region Properties

        public string Extension
        {
            get { return m_sExtension; }
        }

        public Guid MediaType
        {
            get { return m_MediaType; }
        }

        public Guid SubType
        {
            get { return m_SubType; }
        }

        #endregion
    }

    #endregion

    #region Protocol Register Attribute

    [ComVisible(false)]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public class RegisterProtocolExtension : Attribute
    {
        #region Variables

        private string m_sProtocol = "";
        private List<string> m_Extensions = new List<string>();

        #endregion

        #region Constructor

        public RegisterProtocolExtension(string _protocol)
        {
            m_sProtocol = _protocol;
        }

        public RegisterProtocolExtension(string _protocol, string _extension)
            : this(_protocol)
        {
            if (!String.IsNullOrEmpty(_extension))
            {
                m_Extensions.Add(_extension);
            }
        }

        #endregion

        #region Properties

        public string Protocol
        {
            get { return m_sProtocol; }
        }

        public List<string> Extensions
        {
            get { return m_Extensions; }
        }

        #endregion
    }

    #endregion

    #region MediaType Register Attribute

    [ComVisible(false)]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RegisterMediaType : Attribute
    {
        #region Constants

        public const string MEDIATYPE_Stream = "{E436EB83-524F-11CE-9F53-0020AF0BA770}";

        #endregion

        #region Variables

        private Guid m_FilterGuid = typeof(DSFileSourceAsync).GUID;
        private Guid m_MajorType = MediaType.Stream;
        private Guid m_SubType = Guid.Empty;
        private string m_Sequence = "";

        #endregion

        #region Constructor

        public RegisterMediaType(string _subtype, string _sequence)
            : this(null,null,_subtype, _sequence)
        {

        }

        public RegisterMediaType(string _filter, string _subtype, string _sequence)
            : this(_filter, null, _subtype, _sequence)
        {

        }

        public RegisterMediaType(string _filter, string _majortype, string _subtype, string _sequence)
        {
            if (!String.IsNullOrEmpty(_filter))
            {
                m_FilterGuid = new Guid(_filter);
            }
            if (!String.IsNullOrEmpty(_majortype))
            {
                m_MajorType = new Guid(_majortype);
            }
            if (!String.IsNullOrEmpty(_subtype))
            {
                m_SubType = new Guid(_subtype);
            }
            if (!String.IsNullOrEmpty(_sequence))
            {
                m_Sequence = _sequence;
            }
        }

        #endregion

        #region Properties

        public Guid FilterGuid
        {
            get { return m_FilterGuid; }
        }

        public Guid MajorType
        {
            get { return m_MajorType; }
        }

        public Guid SubType
        {
            get { return m_SubType; }
        }

        public string Sequence
        {
            get { return m_Sequence; }
        }

        #endregion
    }

    #endregion

    #region BaseEnum

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class BaseEnum : COMHelper
    {
        #region Variables

        protected int m_iIndex = 0;
        protected object m_Owner = null;
        protected int m_iCount = 0;

        #endregion

        #region Properties

        public object Owner
        {
            get { return m_Owner; }
        }

        public int Index
        {
            get { return m_iIndex; }
        }

        public int Count
        {
            get { return m_iCount; }
        }

        #endregion

        #region Constructor

        public BaseEnum(object _owner)
        {
            ASSERT(_owner != null);
            m_Owner = _owner;
            Reset();
        }

        ~BaseEnum()
        {
            ASSERT(m_Owner);
            m_Owner = null;
        }

        #endregion

        #region Protected Methods

        protected virtual bool IsOutOfSync()
        {
            return false;
        }

        protected virtual void OnReset()
        {

        }

        #endregion

        #region Public Methods

        public virtual int Reset()
        {
            m_iIndex = 0;
            if (IsOutOfSync())
            {
                OnReset();
            }
            return NOERROR;
        }

        public virtual int Skip(int cSkip)
        {
            if (IsOutOfSync()) return VFW_E_ENUM_OUT_OF_SYNC;

            if (m_iIndex + cSkip > m_iCount)
            {
                return S_FALSE;
            }
            m_iIndex += cSkip;
            return NOERROR;
        }

        #endregion
    }

    #endregion

    #region EnumPins

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class EnumPins : BaseEnum, IEnumPins
    {
        #region Constructor

        public EnumPins(BaseFilter _filter)
            : base(_filter)
        {
            m_iCount = _filter.Pins.Count;
        }

        #endregion

        #region Overridden Methods

        protected override bool IsOutOfSync()
        {
            lock ((m_Owner as BaseFilter).FilterLock)
            {
                return m_iCount != (m_Owner as BaseFilter).Pins.Count;
            }
        }

        protected override void OnReset()
        {
            lock ((m_Owner as BaseFilter).FilterLock)
            {
                m_iCount = (m_Owner as BaseFilter).Pins.Count;
            }
        }

        #endregion

        #region IEnumPins Members

        public virtual int Clone(out IEnumPins ppEnum)
        {
            ppEnum = new EnumPins((BaseFilter)m_Owner);
            return NOERROR;
        }

        public virtual int Next(int cPins, IPin[] ppPins, IntPtr pcFetched)
        {
            if (ppPins == null) return E_POINTER;
            ASSERT(ppPins.Length >= cPins);

            if (pcFetched != IntPtr.Zero)
            {
                Marshal.WriteInt32(pcFetched, 0);
            }
            else
                if (cPins > 1)
                {
                    return E_INVALIDARG;
                }
            if (IsOutOfSync())
            {
                OnReset();
            }

            int _count = 0;
            if (m_iCount == m_iIndex) return S_FALSE;

            while (m_iIndex < m_iCount && _count < cPins)
            {
                if (IsOutOfSync()) return VFW_E_ENUM_OUT_OF_SYNC;
                lock ((m_Owner as BaseFilter).FilterLock)
                {
                    ppPins[_count++] = (IPin)(m_Owner as BaseFilter).Pins[m_iIndex++];
                }
            }
            if (pcFetched != IntPtr.Zero)
            {
                Marshal.WriteInt32(pcFetched, _count);
            }
            return (_count == cPins ? S_OK : S_FALSE);
        }

        #endregion
    }

    #endregion

    #region EnumMediaTypes

    /// <summary>
    /// IEnumMediaTypes implementation
    /// </summary>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class EnumMediaTypes : BaseEnum, IEnumMediaTypes
    {
        #region Constructor

        public EnumMediaTypes(BasePin _pin)
            : base(_pin)
        {
        }

        #endregion

        #region Overridden Methods

        protected override bool IsOutOfSync()
        {
            return false;
        }

        protected override void OnReset()
        {
            m_iCount = 0;// (m_Owner as BasePin).AMediaTypes.Count;
        }

        #endregion

        #region IEnumMediaTypes Members

        public virtual int Clone(out IntPtr ppEnum)
        {
            EnumMediaTypes _enum = new EnumMediaTypes((BasePin)m_Owner);
            ppEnum = Marshal.GetComInterfaceForObject(_enum, typeof(IEnumMediaTypes));
            return NOERROR;
        }

        public virtual int Next(int cMediaTypes, IntPtr ppMediaTypes, IntPtr pcFetched)
        {
            if (ppMediaTypes == IntPtr.Zero) return E_POINTER;

            if (pcFetched != IntPtr.Zero)
            {
                Marshal.WriteInt32(pcFetched, 0);
            }
            else
                if (cMediaTypes > 1)
                {
                    return E_INVALIDARG;
                }
            if (IsOutOfSync())
            {
                OnReset();
            }
            int _count = 0;
            while (cMediaTypes > 0)
            {
                AMMediaType mt = null;
                AMMediaType.Init(ref mt);

                int hr = (m_Owner as BasePin).GetMediaType(m_iIndex++, ref mt);
                if (S_OK != hr)
                {
                    break;
                }
                IntPtr _pmt = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(AMMediaType)));
                if (_pmt == IntPtr.Zero)
                {
                    AMMediaType.Free(ref mt);
                    GC.Collect();
                    break;
                }
                Marshal.StructureToPtr(mt, _pmt, true);
                Marshal.WriteIntPtr(ppMediaTypes, _count * Marshal.SizeOf(typeof(IntPtr)), _pmt);
                
                _count++;
                cMediaTypes--;
            }
            if (pcFetched != IntPtr.Zero)
            {
                Marshal.WriteInt32(pcFetched, _count);
            }
            return (0 == cMediaTypes ? S_OK : S_FALSE);
        }

        public override int Skip(int cSkip)
        {
            if (cSkip == 0)
            {
                return S_OK;
            }

            if (IsOutOfSync()) return VFW_E_ENUM_OUT_OF_SYNC;
            m_iIndex += cSkip;

            AMMediaType mt = null;
            try
            {
                AMMediaType.Init(ref mt);
                return (S_OK == (m_Owner as BasePin).GetMediaType(m_iIndex - 1, ref mt)) ? S_OK : S_FALSE;
            }
            finally
            {
                AMMediaType.Free(ref mt);
                GC.Collect();
            }
        }

        #endregion
    };
    
    #endregion

    #region Base Prop Page Support

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class PropPageSupport : COMHelper, ISpecifyPropertyPages
    {
        #region Variables

        protected List<Guid> m_Pages = new List<Guid>();

        #endregion

        #region Properties

        public List<Guid> Pages
        {
            get { return m_Pages; }
        }

        #endregion

        #region Constructor

        protected PropPageSupport()
        {
            Attribute[] _attributes = Attribute.GetCustomAttributes(this.GetType(), typeof(PropPageSetup));
            if (_attributes != null)
            {
                foreach (PropPageSetup _setup in _attributes)
                {
                    if (_setup != null && _setup.Guids.Count > 0)
                    {
                        for (int i = 0; i < _setup.Guids.Count; i++)
                        {
                            m_Pages.Add(_setup.Guids[i]);
                        }
                    }
                }
            }
        }

        #endregion

        #region ISpecifyPropertyPages Members

        public virtual int GetPages(out DsCAUUID pPages)
        {
            pPages = new DsCAUUID();
            try
            {
                if (m_Pages.Count > 0)
                {
                    pPages.cElems = m_Pages.Count;
                    int cb = Marshal.SizeOf(typeof(Guid));
                    pPages.pElems = Marshal.AllocCoTaskMem(cb * m_Pages.Count);
                    IntPtr _ptr = pPages.pElems;
                    for (int i = 0; i < m_Pages.Count; i++)
                    {
                        Marshal.StructureToPtr(m_Pages[i], _ptr, false);
                        _ptr = new IntPtr(_ptr.ToInt32() + cb);
                    }
                    return NOERROR;
                }
                else
                {
                    pPages.cElems = 0;
                    pPages.pElems = IntPtr.Zero;
                    return E_NOTIMPL;
                }
            }
            finally
            {
                GC.Collect();
            }
        }

        #endregion
    }

    #endregion

    #region BasePin

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class BasePin : PropPageSupport, IPin, IQualityControl
    {
        #region Variables

        protected BaseFilter m_Filter = null;
        protected string m_sName = "";
        protected object m_Lock = null;
        protected PinDirection m_Direction = PinDirection.Input;
        protected AMMediaType m_mt = null;
        protected bool m_bCanReconnectWhenActive = false;
        protected bool m_bTryMyTypesFirst = false;
        protected bool m_bRunTimeError = false;
        protected long m_tStart = 0;
        protected long m_tStop = MAX_LONG;
        protected double m_dRate = 1.0;

        protected IntPtr m_QualitySink = IntPtr.Zero;
        protected IntPtr m_ConnectedPin = IntPtr.Zero;
        protected IntPtr m_pAllocator = IntPtr.Zero;

        #endregion

        #region Properties

        public IQualityControl QualitySync
        {
            get 
            {
                if (m_QualitySink != IntPtr.Zero)
                {
                    return (IQualityControl)Marshal.GetObjectForIUnknown(m_QualitySink);
                }
                return null;
            }
        }

        public IMemAllocatorImpl Allocator
        {
            get
            {
                return new IMemAllocatorImpl(m_pAllocator);
            }
        }

        public IntPtr AllocatorPtr
        {
            get { return m_pAllocator; }
            set 
            {
                if (m_pAllocator != IntPtr.Zero) Marshal.Release(m_pAllocator);
                m_pAllocator = value;
                if (m_pAllocator != IntPtr.Zero) Marshal.AddRef(m_pAllocator);
            }
        }

        public PinDirection Direction
        {
            get { return m_Direction; }
        }

        public BaseFilter Filter
        {
            get { return m_Filter; }
        }

        public string Name
        {
            get { return m_sName; }
        }

        public bool IsConnected
        {
            get { return m_ConnectedPin != IntPtr.Zero; }
        }

        public IPinImpl Connected
        {
            get
            {
                return new IPinImpl(m_ConnectedPin);
            }
        }

        public bool IsStopped
        {
            get { return m_Filter.State == FilterState.Stopped; }
        }

        public bool CanReconnectWhenActive
        {
            get { return m_bCanReconnectWhenActive; }
            set { m_bCanReconnectWhenActive = value; }
        }

        public long CurrentStopTime
        {
            get { return m_tStop; }
        }

        public long CurrentStartTime
        {
            get { return m_tStart; }
        }

        public double CurrentRate
        {
            get { return m_dRate; }
        }

        public AMMediaType CurrentMediaType
        {
            get { return m_mt; }
            set { AMMediaType.Copy(value,ref m_mt); }
        }

        #endregion

        #region Constructor

        public BasePin(string _name, BaseFilter _filter, object _lock, PinDirection _direction)
        {
            ASSERT(_filter != null && _lock != null);
            m_Filter = _filter;
            m_Lock = _lock;
            m_sName = _name;
            m_Direction = _direction;
            AMMediaType.Init(ref m_mt);
        }

        ~BasePin()
        {
            if (m_pAllocator != IntPtr.Zero)
            {
                Marshal.Release(m_pAllocator);
            }
            m_pAllocator = IntPtr.Zero;
            AMMediaType.Free(ref m_mt);
            m_Filter = null;
            if (m_ConnectedPin != IntPtr.Zero)
            {
                Marshal.Release(m_ConnectedPin);
            }
            m_ConnectedPin = IntPtr.Zero;
            if (m_QualitySink != IntPtr.Zero)
            {
                Marshal.Release(m_QualitySink);
                m_QualitySink = IntPtr.Zero;
            }
        }

        #endregion

        #region Abstract Methods

        public abstract int CheckMediaType(AMMediaType pmt);

        #endregion

        #region Virtual Methods

        #region Public Methods

        public virtual int BreakConnect()
        {
            return NOERROR;
        }

        public virtual int CompleteConnect(ref IPinImpl pReceivePin)
        {
            return NOERROR;
        }

        public virtual int Active()
        {
            return NOERROR;
        }

        public virtual int Inactive()
        {
            m_bRunTimeError = false;
            return NOERROR;
        }

        public virtual int Run(long tStart)
        {
            return NOERROR;
        }

        public virtual int CheckConnect(ref IPinImpl _pin)
        {
            PinDirection _direction;
            HRESULT hr = (HRESULT)_pin.QueryDirection(out _direction);
            if (hr.Failed) return hr;
            if (_direction == m_Direction)
            {
                return VFW_E_INVALID_DIRECTION;
            }
            return NOERROR;
        }

        public virtual int SetMediaType(AMMediaType mt)
        {
            AMMediaType.Copy(mt, ref m_mt);
            return NOERROR;
        }

        public virtual int GetMediaType(int iPosition, ref AMMediaType pMediaType)
        {
            return E_UNEXPECTED;
        }

        #endregion

        #region Protected Methods
        
        protected virtual int DisconnectInternal()
        {
            if (m_ConnectedPin != IntPtr.Zero)
            {
                int hr = BreakConnect();
                if (FAILED(hr))
                {
                    TRACE("WARNING: BreakConnect() failed in CBasePin::Disconnect().");
                    return hr;
                }
                Marshal.Release(m_ConnectedPin);
                m_ConnectedPin = IntPtr.Zero;
                return S_OK;
            }
            else
            {
                return S_FALSE;
            }
        }

        protected virtual int AgreeMediaType(ref IPinImpl pReceivePin, AMMediaType pmt)
        {
            ASSERT(pReceivePin);
            IEnumMediaTypes pEnumMediaTypes = null;

            if ((pmt != null) && (!AMMediaType.IsPartiallySpecified(pmt)))
            {
                return AttemptConnection(ref pReceivePin, pmt);
            }
            int hrFailure = VFW_E_NO_ACCEPTABLE_TYPES;

            for (int i = 0; i < 2; i++)
            {
                int hr;
                IntPtr _ptr = IntPtr.Zero;
                if (i == (BOOL)m_bTryMyTypesFirst)
                {
                    hr = pReceivePin.EnumMediaTypes(out _ptr);
                    if (_ptr != IntPtr.Zero)
                    {
                        pEnumMediaTypes = (IEnumMediaTypes)new IEnumMediaTypesImpl(_ptr);
                    }
                }
                else
                {
                    hr = EnumMediaTypes(out _ptr);
                    if (_ptr != IntPtr.Zero)
                    {
                        pEnumMediaTypes = (IEnumMediaTypes)Marshal.GetObjectForIUnknown(_ptr);
                    }
                }
                if (SUCCEEDED(hr))
                {
                    ASSERT(pEnumMediaTypes);
                    hr = TryMediaTypes(ref pReceivePin, pmt, pEnumMediaTypes);
                    if (Marshal.IsComObject(pEnumMediaTypes))
                    {
                        Marshal.ReleaseComObject(pEnumMediaTypes);
                    }
                    if (SUCCEEDED(hr))
                    {
                        return NOERROR;
                    }
                    else
                    {
                        if ((hr != E_FAIL) && (hr != E_INVALIDARG) && (hr != VFW_E_TYPE_NOT_ACCEPTED))
                        {
                            hrFailure = hr;
                        }
                    }
                }
            }
            return hrFailure;
        }

        protected virtual int AttemptConnection(ref IPinImpl pReceivePin, AMMediaType pmt)
        {
            int hr = CheckConnect(ref pReceivePin);
            if (FAILED(hr))
            {
                ASSERT(SUCCEEDED(BreakConnect()));
                return hr;
            }
            hr = CheckMediaType(pmt);
            if (hr == NOERROR)
            {
                m_ConnectedPin = pReceivePin.UnknownPtr;
                Marshal.AddRef(m_ConnectedPin);
                hr = SetMediaType(pmt);
                if (SUCCEEDED(hr))
                {
                    IntPtr _ptr = Marshal.GetIUnknownForObject(this);
                    Guid _guid = typeof(IPin).GUID;
                    IntPtr _this;
                    Marshal.QueryInterface(_ptr, ref _guid,out _this);
                    Marshal.Release(_ptr);

                    try
                    {
                        hr = pReceivePin.ReceiveConnection(_this, pmt);
                        if (SUCCEEDED(hr))
                        {
                            hr = CompleteConnect(ref pReceivePin);
                            if (SUCCEEDED(hr))
                            {
                                return hr;
                            }
                            else
                            {
                                pReceivePin.Disconnect();
                            }
                        }
                    }
                    finally
                    {
                        Marshal.Release(_this);
                    }
                }
            }
            else
            {
                if (SUCCEEDED(hr) || (hr == E_FAIL) || (hr == E_INVALIDARG))
                {
                    hr = VFW_E_TYPE_NOT_ACCEPTED;
                }
            }
            ASSERT(SUCCEEDED(BreakConnect()));
            if (m_ConnectedPin != IntPtr.Zero)
            {
                Marshal.Release(m_ConnectedPin);
                m_ConnectedPin = IntPtr.Zero;
            }
            return hr;
        }

        protected virtual int TryMediaTypes(ref IPinImpl pReceivePin, AMMediaType pmt, IEnumMediaTypes pEnum)
        {
            int hr = pEnum.Reset();
            if (FAILED(hr))
            {
                return hr;
            }

            int hrFailure = S_OK;

            IntPtr ulMediaCount = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(uint)));
            IntPtr pTypes = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(IntPtr)));
            try
            {
                for (; ; )
                {
                    hr = pEnum.Next(1, pTypes, ulMediaCount);
                    if (hr != S_OK)
                    {
                        if (S_OK == hrFailure)
                        {
                            hrFailure = VFW_E_NO_ACCEPTABLE_TYPES;
                        }
                        return hrFailure;
                    }

                    ASSERT(Marshal.ReadInt32(ulMediaCount) == 1);

                    IntPtr _ptrStructure = Marshal.ReadIntPtr(pTypes);
                    try
                    {
                        AMMediaType _type = null;
                        if (_ptrStructure != IntPtr.Zero)
                        {
                            _type = (AMMediaType)Marshal.PtrToStructure(_ptrStructure, typeof(AMMediaType));
                        }
                        if ((pmt == null) || AMMediaType.MatchesPartial(_type, pmt))
                        {
                            hr = AttemptConnection(ref pReceivePin, _type);
                            if (FAILED(hr)
                                && SUCCEEDED(hrFailure)
                                && (hr != E_FAIL)
                                && (hr != E_INVALIDARG)
                                && (hr != VFW_E_TYPE_NOT_ACCEPTED))
                            {
                                hrFailure = hr;
                            }
                        }
                        else
                        {
                            hr = VFW_E_NO_ACCEPTABLE_TYPES;
                        }
                        _type = null;
                    }
                    finally
                    {
                        if (_ptrStructure != IntPtr.Zero)
                        {
                            Marshal.FreeCoTaskMem(_ptrStructure);
                        }
                    }
                    if (S_OK == hr)
                    {
                        return hr;
                    }
                }
            }
            finally
            {
                Marshal.FreeCoTaskMem(ulMediaCount);
                Marshal.FreeCoTaskMem(pTypes);
            }
        }

        #endregion

        #endregion

        #region IPin Members

        #region Abstract Methods

        public abstract int BeginFlush();

        public abstract int EndFlush();

        #endregion

        public virtual int ConnectionMediaType(AMMediaType pmt)
        {
            
            if (((object)pmt) == null) return E_POINTER;
            lock (m_Lock)
            {
                if (IsConnected)
                {
                    AMMediaType.Copy(m_mt,ref pmt);
                    return S_OK;
                }
                else
                {
                    AMMediaType.Init(ref pmt);
                    return VFW_E_NOT_CONNECTED;
                }
            }
        }

        public virtual int ReceiveConnection(IntPtr pReceivePin, AMMediaType pmt)
        {
            
            if (pReceivePin == IntPtr.Zero || pmt == null) return E_POINTER;
            lock (m_Lock)
            {
                IPinImpl _pin = new IPinImpl(pReceivePin);
                if (m_ConnectedPin != IntPtr.Zero)
                {
                    return VFW_E_ALREADY_CONNECTED;
                }
                if (!IsStopped && !m_bCanReconnectWhenActive)
                {
                    return VFW_E_NOT_STOPPED;
                }
                int hr = CheckConnect(ref _pin);
                if (FAILED(hr))
                {
                    ASSERT(SUCCEEDED(BreakConnect()));
                    return hr;
                }
                hr = CheckMediaType(pmt);
                if (hr != NOERROR)
                {
                    ASSERT(SUCCEEDED(BreakConnect()));
                    if (SUCCEEDED(hr) || (hr == E_FAIL) || (hr == E_INVALIDARG))
                    {
                        hr = VFW_E_TYPE_NOT_ACCEPTED;
                    }
                    return hr;
                }

                m_ConnectedPin = pReceivePin;
                Marshal.AddRef(m_ConnectedPin);
                hr = SetMediaType(pmt);
                if (SUCCEEDED(hr))
                {
                    hr = CompleteConnect(ref _pin);
                    if (SUCCEEDED(hr))
                    {
                        return NOERROR;
                    }
                }
                Marshal.Release(m_ConnectedPin);
                m_ConnectedPin = IntPtr.Zero;
                {
                    ASSERT(SUCCEEDED(BreakConnect()));
                }
                return hr;
            }
        }

        public virtual int Connect(IntPtr pReceivePin, AMMediaType pmt)
        {

            if (pReceivePin == IntPtr.Zero) return E_POINTER;
            lock (m_Lock)
            {
                if (m_ConnectedPin != IntPtr.Zero)
                {
                    return VFW_E_ALREADY_CONNECTED;
                }
                if (!IsStopped && !m_bCanReconnectWhenActive)
                {
                    return VFW_E_NOT_STOPPED;
                }
                IPinImpl _pin = new IPinImpl(pReceivePin);
                int hr = AgreeMediaType(ref _pin, pmt);
                if (FAILED(hr))
                {
                    ASSERT(SUCCEEDED(BreakConnect()));
                    return hr;
                }
                return NOERROR;
            }
        }

        public virtual int Disconnect()
        {
            
            lock (m_Lock)
            {
                if (!IsStopped)
                {
                    return VFW_E_NOT_STOPPED;
                }
                return DisconnectInternal();
            }
        }

        public virtual int ConnectedTo(out IntPtr ppPin)
        {
            
            ppPin = m_ConnectedPin;
            if (ppPin != IntPtr.Zero)
            {
                Marshal.AddRef(ppPin);
            }
            return (m_ConnectedPin == IntPtr.Zero ? VFW_E_NOT_CONNECTED : S_OK);
        }

        public virtual int QueryPinInfo(out PinInfo pInfo)
        {
            
            pInfo = new PinInfo();
            pInfo.name = m_sName;
            pInfo.dir = m_Direction;
            pInfo.filter = (IBaseFilter)m_Filter;
            return NOERROR;
        }

        public virtual int EndOfStream()
        {
            return NOERROR;
        }

        public virtual int QueryAccept(AMMediaType pmt)
        {
            
            if (pmt == null) return E_POINTER;

            int hr = CheckMediaType(pmt);
            if (FAILED(hr))
            {
                return S_FALSE;
            }
            return hr;
        }

        public virtual int NewSegment(long tStart, long tStop, double dRate)
        {
            m_tStart = tStart;
            m_tStop = tStop;
            m_dRate = dRate;

            return S_OK;
        }

        public virtual int QueryDirection(out PinDirection pPinDir)
        {
            
            pPinDir = m_Direction;
            return S_OK;
        }

        public virtual int QueryId(out string Id)
        {
            
            Id = m_sName;
            return NOERROR;
        }

        public virtual int QueryInternalConnections(IntPtr ppPins, ref int nPin)
        {
            
            nPin = 0;
            return E_NOTIMPL;
        }

        public virtual int EnumMediaTypes(out IntPtr ppEnum)
        {
            int hr = S_OK;
            EnumMediaTypes _enum = new EnumMediaTypes(this);
            ppEnum = Marshal.GetComInterfaceForObject(_enum,typeof(IEnumMediaTypes));
            return hr;
        }

        #endregion

        #region IQualityControl Members

        public virtual int Notify(IntPtr pSelf, Quality q)
        {
            return E_NOTIMPL;
        }

        public virtual int SetSink(IntPtr piqc)
        {
            lock (m_Lock)
            {
                if (m_QualitySink != IntPtr.Zero)
                {
                    Marshal.Release(m_QualitySink);
                }
                m_QualitySink = piqc;
                if (m_QualitySink != IntPtr.Zero)
                {
                    Marshal.AddRef(m_QualitySink);
                }
            }
            return NOERROR;
        }

        #endregion

        #region Static Methods

        protected static HRESULT CreateMemoryAllocator(out IntPtr ppAlloc)
        {
            ppAlloc = IntPtr.Zero;
            HRESULT hr = (HRESULT)API.CoCreateInstance(
                typeof(MemoryAllocator).GUID,
                IntPtr.Zero,
                CLSCTX.CLSCTX_INPROC_SERVER,
                typeof(IMemAllocator).GUID,
                out ppAlloc);
            return hr;
        }

        protected static HRESULT CreatePosPassThru(IntPtr _owner, IPin pPin, bool bRenderer, out IntPtr ppPassThru)
        {
            ppPassThru = IntPtr.Zero;
            HRESULT hr = (HRESULT)API.CoCreateInstance(
                typeof(SeekingPassThru).GUID,
                _owner,
                CLSCTX.CLSCTX_INPROC_SERVER,
                typeof(ISeekingPassThru).GUID,
                out ppPassThru);
            if (FAILED(hr)) return hr;

            ISeekingPassThruImpl _seek = new ISeekingPassThruImpl(ppPassThru);
            hr = (HRESULT)_seek.Init(bRenderer,pPin);
            if (FAILED(hr))
            {
                Marshal.Release(ppPassThru);
                ppPassThru = IntPtr.Zero;
            }
            return hr;
        }

        #endregion
    }

    #endregion

    #region Base InputPin

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class BaseInputPin : BasePin, IMemInputPin
    {
        #region Variables

        protected bool m_bReadOnly = false;

        protected bool m_bFlushing = false;

        protected AMSample2Properties m_SampleProps = new AMSample2Properties();

        protected IMediaSeekingImpl m_pSeeking = null;

        #endregion

        #region Properties

        public IMediaSeekingImpl Seeking
        {
            get
            {
                if (m_pSeeking == null && IsConnected)
                {
                    m_pSeeking = new IMediaSeekingImpl(m_ConnectedPin);
                    if (!m_pSeeking.IsValid)
                    {
                        m_pSeeking = null;
                    }
                    else
                    {
                        m_pSeeking._AddRef();
                    }
                }
                return m_pSeeking;
            }
        }

        public bool IsReadOnly
        {
            get { return m_bReadOnly; }
        }

        public bool IsFlushing
        {
            get { return m_bFlushing; }
        }

        public AMSample2Properties SampleProps
        {
            get 
            { 
                ASSERT(m_SampleProps.cbData != 0);
                return m_SampleProps;
            }
        }

        #endregion
        
        #region Constructor

        public BaseInputPin(string _name, BaseFilter _filter)
            : base(_name, _filter, _filter.FilterLock, PinDirection.Input)
        {

        }

        ~BaseInputPin()
        {
            
        }

        #endregion

        #region Overridden Methods

        public override int BreakConnect()
        {
            if (m_pAllocator != IntPtr.Zero)
            {
                int hr = Allocator.Decommit();
                if (FAILED(hr))
                {
                    return hr;
                }
                Marshal.Release(m_pAllocator);
                m_pAllocator = IntPtr.Zero;
            }
            if (m_pSeeking != null)
            {
                m_pSeeking._Release();
                m_pSeeking = null;
            }
            return NOERROR;
        }

        public override int BeginFlush()
        {
            lock (m_Lock)
            {
                ASSERT(!m_bFlushing);
                m_bFlushing = true;
                return S_OK;
            }
        }

        public override int EndFlush()
        {
            lock (m_Lock)
            {
                ASSERT(m_bFlushing);
                m_bFlushing = false;
                m_bRunTimeError = false;
                return S_OK;
            }
        }

        public override int Notify(IntPtr pSelf, Quality q)
        {
            if (pSelf == IntPtr.Zero) return E_POINTER;
            return NOERROR;
        }

        public override int Inactive()
        {
            m_bRunTimeError = false;

            if (m_pAllocator == IntPtr.Zero)
            {
                return VFW_E_NO_ALLOCATOR;
            }

            m_bFlushing = false;

            return Allocator.Decommit();
        }

        #endregion

        #region Virtual Methods

        public virtual int CheckStreaming()
        {
            ASSERT(IsConnected);
            if (IsStopped)
            {
                return VFW_E_WRONG_STATE;
            }
            if (m_bFlushing)
            {
                return S_FALSE;
            }
            if (m_bRunTimeError)
            {
                return VFW_E_RUNTIME_ERROR;
            }
            return S_OK;
        }

        public virtual int PassNotify(Quality q)
        {
            if (m_QualitySink != IntPtr.Zero)
            {
                return this.QualitySync.Notify(Marshal.GetIUnknownForObject(m_Filter), q);
            }
            else
            {
                try
                {
                    int hr = VFW_E_NOT_FOUND;
                    if (m_ConnectedPin != IntPtr.Zero)
                    {
                        IQualityControl pIQC = (IQualityControl)Marshal.GetObjectForIUnknown(m_ConnectedPin);
                        if (pIQC != null)
                        {
                            hr = pIQC.Notify(Marshal.GetIUnknownForObject(m_Filter), q);
                            if (Marshal.IsComObject(pIQC))
                            {
                                Marshal.ReleaseComObject(pIQC);
                            }
                            pIQC = null;
                        }
                    }
                    return hr;
                }
                finally
                {
                    GC.Collect();
                }
            }
        }

        public virtual int OnReceive(ref IMediaSampleImpl _sample)
        {
            int hr = CheckStreaming();
            if (S_OK != hr)
            {
                return hr;
            }
            Guid _guid = typeof(IMediaSample2).GUID;
            IntPtr pSample2;
            if (S_OK == _sample._QueryInterface(ref _guid,out pSample2))
            {
                IMediaSample2Impl _sample2 = new IMediaSample2Impl(pSample2);
                IntPtr pStructure = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(AMSample2Properties)));
                hr = _sample2.GetProperties(Marshal.SizeOf(typeof(AMSample2Properties)), pStructure);
                m_SampleProps = (AMSample2Properties)Marshal.PtrToStructure(pStructure, typeof(AMSample2Properties));
                Marshal.FreeCoTaskMem(pStructure);
                _sample2._Release();
                if (FAILED(hr))
                {
                    return hr;
                }
            }
            else
            {
                //  Get the properties the hard way 
                m_SampleProps.cbData = Marshal.SizeOf(m_SampleProps);
                m_SampleProps.dwTypeSpecificFlags = 0;
                m_SampleProps.dwStreamId = (int)AMSamplePropertyFlags.Media;
                m_SampleProps.dwSampleFlags = 0;
                if (S_OK == _sample.IsDiscontinuity())
                {
                    m_SampleProps.dwSampleFlags |= AMSamplePropertyFlags.DataDiscontinuity;
                }
                if (S_OK == _sample.IsPreroll())
                {
                    m_SampleProps.dwSampleFlags |= AMSamplePropertyFlags.PreRoll;
                }
                if (S_OK == _sample.IsSyncPoint())
                {
                    m_SampleProps.dwSampleFlags |= AMSamplePropertyFlags.SplicePoint;
                }
                if (SUCCEEDED(_sample.GetTime(out m_SampleProps.tStart,
                                               out m_SampleProps.tStop)))
                {
                    m_SampleProps.dwSampleFlags |= AMSamplePropertyFlags.TimeValid |
                                                   AMSamplePropertyFlags.StopValid;
                }
                AMMediaType mt;
                if (S_OK == _sample.GetMediaType(out mt))
                {
                    Marshal.StructureToPtr(mt, m_SampleProps.pMediaType, true);
                    m_SampleProps.dwSampleFlags |= AMSamplePropertyFlags.TypeChanged;
                }
                _sample.GetPointer(out m_SampleProps.pbBuffer);
                m_SampleProps.lActual = _sample.GetActualDataLength();
                m_SampleProps.cbBuffer = _sample.GetSize();
            }

            // Has the format changed in this sample

            if ((m_SampleProps.dwSampleFlags & AMSamplePropertyFlags.TypeChanged) == 0)
            {
                return NOERROR;
            }

            //Debug.WriteLine("Failed Receive");
            // Check the derived class accepts this format 
            // This shouldn't fail as the source must call QueryAccept first 

            hr = CheckMediaType((AMMediaType)Marshal.PtrToStructure(m_SampleProps.pMediaType, typeof(AMMediaType)));

            if (hr == NOERROR)
            {
                return NOERROR;
            }

            // Raise a runtime error if we fail the media type 
            m_bRunTimeError = true;
            EndOfStream();
            m_Filter.NotifyEvent(EventCode.ErrorAbort, (IntPtr)((int)VFW_E_TYPE_NOT_ACCEPTED), IntPtr.Zero);
            return VFW_E_INVALIDMEDIATYPE;
        }

        #endregion

        #region IMemInputPin Members

        public virtual int GetAllocator(out IntPtr ppAllocator)
        {
            
            lock (m_Lock)
            {
                ppAllocator = IntPtr.Zero;
                if (m_pAllocator == IntPtr.Zero)
                {
                    int hr = CreateMemoryAllocator(out m_pAllocator);
                    if (FAILED(hr))
                    {
                        return hr;
                    }
                }
                ASSERT(m_pAllocator != IntPtr.Zero);
                ppAllocator = m_pAllocator;
                Marshal.AddRef(ppAllocator);
            }
            return NOERROR;
        }

        public virtual int NotifyAllocator(IntPtr pAllocator, bool bReadOnly)
        {
            
            if (pAllocator == IntPtr.Zero) return E_POINTER;
            lock (m_Lock)
            {
                if (m_pAllocator != pAllocator)
                {
                    if (m_pAllocator != IntPtr.Zero)
                    {
                        Marshal.Release(m_pAllocator);
                    }
                    m_pAllocator = pAllocator;
                    if (m_pAllocator != IntPtr.Zero)
                    {
                        Marshal.AddRef(m_pAllocator);
                    }
                }
                m_bReadOnly = bReadOnly;
                return NOERROR;
            }
        }

        public virtual int GetAllocatorRequirements(AllocatorProperties pProps)
        {
            
            return E_NOTIMPL;
        }

        public virtual int Receive(IntPtr pSample)
        {
            IMediaSampleImpl _sample = new IMediaSampleImpl(pSample);
            return OnReceive(ref _sample);
        }

        public virtual int ReceiveMultiple(IntPtr pSamples, int nSamples, out int nSamplesProcessed)
        {
            int hr = S_OK;
            nSamplesProcessed = 0;
            if (pSamples == IntPtr.Zero) return E_POINTER;
            while (nSamples-- > 0)
            {
                IntPtr _sample = Marshal.ReadIntPtr(pSamples, nSamples * IntPtr.Size);
                hr = Receive(_sample);
                if (hr != S_OK)
                {
                    break;
                }
                nSamplesProcessed++;
            }
            return hr;
        }

        public virtual int ReceiveCanBlock()
        {
            
            int cOutputPins = 0;
            for (int i = 0; i < m_Filter.Pins.Count; i++)
            {
                BasePin _pin = m_Filter.Pins[i];
                if (_pin.Direction == PinDirection.Output)
                {
                    IntPtr pPtr;
                    int hr = _pin.ConnectedTo(out pPtr);
                    if (SUCCEEDED(hr))
                    {
                        cOutputPins++;
                        Guid _guid = typeof(IMemInputPin).GUID;
                        IntPtr pInputPin;
                        hr = Marshal.QueryInterface(pPtr, ref _guid, out pInputPin);
                        if (SUCCEEDED(hr))
                        {
                            IMemInputPinImpl _input = new IMemInputPinImpl(pInputPin);
                            Marshal.Release(pInputPin);
                            Marshal.Release(pPtr);
                            if (pInputPin != null)
                            {
                                hr = _input.ReceiveCanBlock();
                                if (hr != S_FALSE)
                                {
                                    return S_OK;
                                }
                            }
                            else
                            {
                                return S_OK;
                            }
                        }
                        else
                        {
                            Marshal.Release(pPtr);
                        }
                    }
                }
            }
            return cOutputPins == 0 ? S_OK : S_FALSE;
        }

        #endregion
    }

    #endregion

    #region Base OutputPin

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class BaseOutputPin : BasePin
    {
        #region Variables

        protected IntPtr m_pInputPin = IntPtr.Zero;

        #endregion

        #region Properties

        public IMemInputPinImpl InputPin
        {
            get 
            {
                return new IMemInputPinImpl(m_pInputPin);
            }
        }

        #endregion

        #region Constructor

        public BaseOutputPin(string _name, BaseFilter _filter)
            : base(_name, _filter, _filter.FilterLock, PinDirection.Output)
        {

        }

        #endregion

        #region Overridden Methods

        public override int BeginFlush()
        {
            return E_UNEXPECTED;
        }

        public override int EndFlush()
        {
            return E_UNEXPECTED;
        }

        public override int EndOfStream()
        {
            return E_UNEXPECTED;
        }

        public override int Inactive()
        {
            m_bRunTimeError = false;
            if (m_pAllocator == IntPtr.Zero)
            {
                return VFW_E_NO_ALLOCATOR;
            }
            return Allocator.Decommit();
        }

        public override int Active()
        {
            if (m_pAllocator == IntPtr.Zero)
            {
                return VFW_E_NO_ALLOCATOR;
            }
            return Allocator.Commit();
        }

        public override int BreakConnect()
        {
            
            if (m_pAllocator != IntPtr.Zero)
            {
                int hr = Allocator.Decommit();
                if (FAILED(hr))
                {
                    return hr;
                }
                Marshal.Release(m_pAllocator);
                m_pAllocator = IntPtr.Zero;
            }
            if (m_pInputPin != IntPtr.Zero)
            {
                Marshal.Release(m_pInputPin);
                m_pInputPin = IntPtr.Zero;
            }
            return NOERROR;
        }

        public override int CheckConnect(ref IPinImpl _pin)
        {
            
            int hr = base.CheckConnect(ref _pin);
            if (FAILED(hr))
            {
                return hr;
            }
            Guid _guid = typeof(IMemInputPin).GUID;
            hr = _pin._QueryInterface(ref _guid, out m_pInputPin);
            if (FAILED(hr))
            {
                return E_NOINTERFACE;
            }
            return NOERROR;
        }

        public override int CompleteConnect(ref IPinImpl pReceivePin)
        {
            
            return DecideAllocator(InputPin, out m_pAllocator);
        }

        #endregion

        #region Abstract Methods

        public abstract int DecideBufferSize(ref IMemAllocatorImpl pAlloc, ref AllocatorProperties prop);

        #endregion

        #region Virtual Methods

        public virtual int InitAllocator(out IntPtr ppAlloc)
        {
            
            return CreateMemoryAllocator(out ppAlloc);
        }

        public virtual int DecideAllocator(IMemInputPinImpl pPin, out IntPtr ppAlloc)
        {
            
            int hr = NOERROR;
            ppAlloc = IntPtr.Zero;
            AllocatorProperties prop = new AllocatorProperties();
            prop.cbAlign = 0; prop.cbBuffer = 0; prop.cbPrefix = 0; prop.cBuffers = 0;

            pPin.GetAllocatorRequirements(prop);

            if (prop.cbAlign == 0)
            {
                prop.cbAlign = 1;
            }
            
            hr = pPin.GetAllocator(out ppAlloc);
            if (SUCCEEDED(hr))
            {
                IMemAllocatorImpl _allocator = new IMemAllocatorImpl(ppAlloc);
                hr = DecideBufferSize(ref _allocator, ref prop);
                if (SUCCEEDED(hr))
                {
                    hr = pPin.NotifyAllocator(ppAlloc, false);
                    if (SUCCEEDED(hr))
                    {
                        return NOERROR;
                    }
                }
            }

            if (ppAlloc != IntPtr.Zero)
            {
                Marshal.Release(ppAlloc);
                ppAlloc = IntPtr.Zero;
            }
            
            hr = InitAllocator(out ppAlloc);

            if (SUCCEEDED(hr))
            {
                // note - the properties passed here are in the same
                // structure as above and may have been modified by
                // the previous call to DecideBufferSize
                IMemAllocatorImpl _allocator = new IMemAllocatorImpl(ppAlloc);
                hr = DecideBufferSize(ref _allocator, ref prop);
                if (SUCCEEDED(hr))
                {
                    hr = pPin.NotifyAllocator(ppAlloc, false);
                    if (SUCCEEDED(hr))
                    {
                        return NOERROR;
                    }
                }
            }

            if (ppAlloc != IntPtr.Zero)
            {
                Marshal.Release(ppAlloc);
                ppAlloc = IntPtr.Zero;
            }

            return hr;
        }


        public virtual int GetDeliveryBuffer(out IntPtr ppSample,
                                    DsLong pStartTime,
                                    DsLong pEndTime,
                                    AMGBF dwFlags)
        {
            if (m_pAllocator != IntPtr.Zero)
            {
                IntPtr _start = IntPtr.Zero;
                IntPtr _stop = IntPtr.Zero;
                return Allocator.GetBuffer(out ppSample, _start, _stop, (int)dwFlags);
            }
            else
            {
                ppSample = IntPtr.Zero;
                return E_NOINTERFACE;
            }
        }

        public virtual int Deliver(ref IMediaSampleImpl _sample)
        {
            return Deliver(_sample.UnknownPtr);
        }

        public virtual int Deliver(IntPtr pSample)
        {
            if (m_pInputPin == IntPtr.Zero)
            {
                return VFW_E_NOT_CONNECTED;
            }
            return InputPin.Receive(pSample);
        }
        
        public virtual int DeliverEndOfStream()
        {
            if (m_ConnectedPin == IntPtr.Zero)
            {
                return VFW_E_NOT_CONNECTED;
            }
            return Connected.EndOfStream();
        }

        public virtual int DeliverBeginFlush()
        {
            if (m_ConnectedPin == IntPtr.Zero)
            {
                return VFW_E_NOT_CONNECTED;
            }
            return Connected.BeginFlush();
        }

        public virtual int DeliverEndFlush()
        {
            if (m_ConnectedPin == IntPtr.Zero)
            {
                return VFW_E_NOT_CONNECTED;
            }
            return Connected.EndFlush();
        }

        public virtual int DeliverNewSegment(long tStart, long tStop, double dRate)
        {
            if (m_ConnectedPin == IntPtr.Zero)
            {
                return VFW_E_NOT_CONNECTED;
            }
            return Connected.NewSegment(tStart, tStop, dRate);
        }

        #endregion

        #region Helper Methods

        public virtual int ReconnectPin()
        {
            if (m_Filter.FilterGraph != null)
            {
                HRESULT hr = E_FAIL;
                IFilterGraph2 pGraph2 = (IFilterGraph2)m_Filter.FilterGraph;
                if (pGraph2 != null)
                {
                    int nIndex = 0;
                    AMMediaType pmt;
                    while (true)
                    {
                        pmt = new AMMediaType();
                        hr = (HRESULT)GetMediaType(nIndex++, ref pmt);
                        if (hr != S_OK) break;
                        hr = (HRESULT)pGraph2.ReconnectEx(this, pmt);
                        pmt.Free();
                        if (hr == S_OK) break;
                    }
                    pGraph2 = null;
                }
                if (!hr.Succeeded)
                {
                    hr = (HRESULT)m_Filter.FilterGraph.Reconnect(this);
                }
                return hr;
            }
            else
            {
                return E_NOINTERFACE;
            }
        }

        #endregion
    }

    #endregion

    #region BaseFilter

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class BaseFilter : PropPageSupport, IPersistStream, IBaseFilter
    {
        #region Constants

        private const string m_csRegistryPath = "Settings";

        #endregion

        #region Variables

        private List<BasePin> m_Pins = null;

        protected string m_sName = "";
        protected object m_Lock = null;
        protected IntPtr m_pClock = IntPtr.Zero;
        protected IntPtr m_pGraph = IntPtr.Zero;
        protected IntPtr m_pSink = IntPtr.Zero;
        protected FilterState m_State = FilterState.Stopped;
        protected long m_tStart = 0;
        protected bool m_bPersistDirty = false;

        #endregion

        #region Properties

        public string Name
        {
            get { return m_sName; }
        }

        public List<BasePin> Pins
        {
            get 
            {
                if (m_Pins == null)
                {
                    m_Pins = new List<BasePin>();
                    int hr = OnInitializePins();
                    ASSERT(SUCCEEDED(hr));
                }
                return m_Pins;
            }
        }

        public FilterState State
        {
            get { return m_State; }
        }

        public IFilterGraph FilterGraph
        {
            get { 
                if (m_pGraph == IntPtr.Zero) return null;
                return (IFilterGraph)Marshal.GetObjectForIUnknown(m_pGraph); 
            }
        }

        public IMediaEventSink Sink
        {
            get 
            {
                if (m_pGraph == IntPtr.Zero) return null;
                return (IMediaEventSink)Marshal.GetObjectForIUnknown(m_pGraph); 
            }
        }

        public bool IsActive
        {
            get 
            {
                lock (m_Lock)
                {
                    return ((m_State == FilterState.Paused) || (m_State == FilterState.Running));
                }
            }
        }

        public bool IsStopped
        {
            get { return m_State == FilterState.Stopped; }
        }

        public object FilterLock
        {
            get { return m_Lock; }
        }

        public IReferenceClockImpl Clock
        {
            get {
                return new IReferenceClockImpl(m_pClock); 
            }
        }

        #endregion

        #region Constructor

        public BaseFilter(string _name)
            : this(_name,new object())
        {

        }

        public BaseFilter(string _name, object _lock)
        {
            ASSERT(_lock != null);
            m_Lock = _lock;
            m_sName = _name;
        }

        ~BaseFilter()
        {
            if (m_Pins != null)
            {
                while (m_Pins.Count > 0)
                {
                    m_Pins.RemoveAt(0);
                }
            }
        }

        #endregion

        #region Abstract Methods

        protected abstract int OnInitializePins();

        #endregion

        #region Pins Helper

        public int AddPin(BasePin _pin)
        {
            lock ( m_Lock )
            {
                Pins.Add(_pin);
            }
            return S_OK;
        }

        public int RemovePin(BasePin _pin)
        {
            lock (m_Lock)
            {
                return Pins.Remove(_pin) ? S_OK : S_FALSE;
            }
        }

        #endregion

        #region Helper Methods

        public virtual int NotifyEvent(EventCode _code, IntPtr _param1, IntPtr _param2)
        {
            IMediaEventSink pSink = Sink;
            if (pSink != null)
            {
                IntPtr _pUnknown = Marshal.GetIUnknownForObject(this);
                try
                {
                    if (_code == EventCode.Complete)
                    {
                        _param2 = _pUnknown;
                    }
                    return pSink.Notify(_code, _param1, _param2);
                }
                finally
                {
                    Marshal.Release(_pUnknown);
                }
            }
            else
            {
                return E_NOTIMPL;
            }
        }

        public virtual int ReconnectPin(IntPtr pPin, AMMediaType pmt)
        {
            return ReconnectPin((IPin)Marshal.GetObjectForIUnknown(pPin), pmt);
        }

        public virtual int ReconnectPin(IPin pPin,AMMediaType pmt)
        {
            if (m_pGraph != null)
            {
                int hr;
                IFilterGraph2 pGraph2 = (IFilterGraph2)FilterGraph;
                if (pGraph2 != null)
                {
                    hr = pGraph2.ReconnectEx(pPin, pmt);
                    pGraph2 = null;
                }
                else
                {
                    hr = FilterGraph.Reconnect(pPin);
                }
                return hr;
            }
            else
            {
                return E_NOINTERFACE;
            }
        }

        public virtual int StreamTime(out long rtStream)
        {
            rtStream = 0;
            if (m_pClock == IntPtr.Zero) return VFW_E_NO_CLOCK;
            int hr = Clock.GetTime(out rtStream);
            if (FAILED(hr)) return hr;

            rtStream -= m_tStart;

            return S_OK;
        }

        #endregion

        #region IBaseFilter Members

        public virtual int EnumPins(out IEnumPins ppEnum)
        {
            int hr = NOERROR;
            if (m_Pins == null)
            {
                m_Pins = new List<BasePin>();
                hr = OnInitializePins();
                if (FAILED(hr))
                {
                    ppEnum = null;
                    return hr;
                }
            }
            ppEnum = new EnumPins(this);
            return hr;
        }

        public virtual int GetClassID(out Guid pClassID)
        {
            pClassID = this.GetType().GUID;
            return NOERROR;
        }

        public virtual int FindPin(string Id, out IPin ppPin)
        {
            lock (m_Lock)
            {
                int hr = NOERROR;
                if (m_Pins == null)
                {
                    m_Pins = new List<BasePin>();
                    hr = OnInitializePins();
                    if (FAILED(hr))
                    {
                        ppPin = null;
                        return hr;
                    }
                }
                ASSERT(m_Pins);
                for (int i = 0; i < m_Pins.Count; i++)
                {
                    BasePin _pin = m_Pins[i];
                    ASSERT(_pin);
                    if (_pin.Name == Id)
                    {
                        ppPin = (IPin)_pin;
                        return S_OK;
                    }
                }
                hr = VFW_E_NOT_FOUND;
                ppPin = null;
                return hr;
            }
        }

        public virtual int GetState(int dwMilliSecsTimeout, out FilterState filtState)
        {
            filtState = m_State;
            return NOERROR;
        }

        public virtual int GetSyncSource(out IntPtr pClock)
        {
            lock (m_Lock)
            {
                pClock = m_pClock;
                if (m_pClock != IntPtr.Zero)
                {
                    Marshal.AddRef(m_pClock);
                }
            }
            return NOERROR;
        }

        public virtual int SetSyncSource(IntPtr pClock)
        {
            lock (m_Lock)
            {
                if (m_pClock != IntPtr.Zero)
                {
                    Marshal.Release(m_pClock);
                }
                m_pClock = pClock;
                if (m_pClock != IntPtr.Zero)
                {
                    Marshal.AddRef(m_pClock);
                }
            }
            return NOERROR;
        }

        public virtual int QueryVendorInfo(out string pVendorInfo)
        {
            pVendorInfo = null;
            return E_NOTIMPL;
        }

        public virtual int QueryFilterInfo(out FilterInfo pInfo)
        {
            pInfo = new FilterInfo();
            pInfo.achName = m_sName;
            pInfo.pGraph = FilterGraph;
            return S_OK;
        }

        public virtual int JoinFilterGraph(IntPtr pGraph, string pName)
        {
            lock (m_Lock)
            {
                if (m_pSink != IntPtr.Zero)
                {
                    Marshal.Release(m_pSink);
                    m_pSink = IntPtr.Zero;
                }
                if (m_pGraph != IntPtr.Zero)
                {
                    Marshal.Release(m_pGraph);
                }
                m_pGraph = pGraph;
                if (m_pGraph != IntPtr.Zero)
                {
                    Marshal.AddRef(m_pGraph);
                    Guid _guid = typeof(IMediaEventSink).GUID;
                    Marshal.QueryInterface(m_pGraph, ref _guid, out m_pSink);
                }
                if (pName != null)
                {
                    m_sName = pName;
                }
            }
            return NOERROR;
        }

        public virtual int Pause()
        {
            lock (m_Lock)
            {
                int hr;
                if (m_State == FilterState.Stopped)
                {
                    if (m_Pins == null)
                    {
                        m_Pins = new List<BasePin>();
                        hr = OnInitializePins();
                        if (FAILED(hr)) return hr;
                    }
                    ASSERT(m_Pins);
                    for (int i = 0; i < m_Pins.Count; i++)
                    {
                        if (m_Pins[i].IsConnected)
                        {
                            hr = m_Pins[i].Active();
                            if (FAILED(hr)) return hr;
                        }
                    }
                }
                m_State = FilterState.Paused;
            }
            return S_OK;
        }

        public virtual int Run(long tStart)
        {
            lock (m_Lock)
            {
                int hr;
                m_tStart = tStart;
                if (m_State == FilterState.Stopped)
                {
                    hr = Pause();
                    if (FAILED(hr)) return hr;
                }
                if (m_State != FilterState.Running)
                {
                    if (m_Pins == null)
                    {
                        m_Pins = new List<BasePin>();
                        hr = OnInitializePins();
                        if (FAILED(hr)) return hr;
                    }
                    ASSERT(m_Pins);
                    for (int i = 0; i < m_Pins.Count; i++)
                    {
                        if (m_Pins[i].IsConnected)
                        {
                            hr = m_Pins[i].Run(tStart);
                            if (FAILED(hr)) return hr;
                        }
                    }
                }
                m_State = FilterState.Running;
            }
            return S_OK;
        }

        public virtual int Stop()
        {
            lock (m_Lock)
            {
                int hr = NOERROR;
                if (m_State != FilterState.Stopped)
                {
                    if (m_Pins == null)
                    {
                        m_Pins = new List<BasePin>();
                        hr = OnInitializePins();
                        if (FAILED(hr)) return hr;
                        hr = NOERROR;
                    }
                    ASSERT(m_Pins);
                    for (int i = 0; i < m_Pins.Count; i++)
                    {
                        if (m_Pins[i].IsConnected)
                        {
                            int hrTmp = m_Pins[i].Inactive();
                            if (FAILED(hrTmp) && SUCCEEDED(hr))
                            {
                                hr = hrTmp;
                            }
                        }
                    }
                }
                m_State = FilterState.Stopped;
                return hr;
            }
        }

        #endregion

        #region IPersistStream Members

        public virtual int IsDirty()
        {
            return (m_bPersistDirty ? S_OK : S_FALSE);
        }

        public virtual int Load(IntPtr pStm)
        {
            if (pStm == IntPtr.Zero) return E_POINTER;
            HRESULT hr = NOERROR;
            Marshal.AddRef(pStm);
            Stream _stream = null;
            try
            {
                _stream = new COMStream(pStm);
                hr = ReadFromStream(_stream);
                if (hr.Succeeded)
                {
                    SetDirty(false);
                }
            }
            catch (Exception _exception)
            {
                hr = (HRESULT)Marshal.GetHRForException(_exception);
            }
            finally
            {
                if (_stream != null)
                {
                    _stream.Dispose();
                }
                Marshal.Release(pStm);
            }
            return hr;
        }

        public virtual int Save(IntPtr pStm, bool fClearDirty)
        {
            if (pStm == IntPtr.Zero) return E_POINTER;
            HRESULT hr = NOERROR;
            Marshal.AddRef(pStm);
            Stream _stream = null;
            try
            {
                _stream = new COMStream(pStm);
                hr = WriteToStream(_stream);
                if (hr.Succeeded && fClearDirty)
                {
                    SetDirty(false);
                }
            }
            catch (Exception _exception)
            {
                hr = (HRESULT)Marshal.GetHRForException(_exception);
            }
            finally
            {
                if (_stream != null)
                {
                    _stream.Dispose();
                }
                Marshal.Release(pStm);
            }
            return hr;
        }

        public virtual int GetSizeMax(out long pcbSize)
        {
            pcbSize = SizeMax();
            return (pcbSize > 0 ? NOERROR : E_NOTIMPL);
        }

        #endregion

        #region Persist Helper Methods

        protected HRESULT SetDirty(bool bDirty)
        {
            m_bPersistDirty = bDirty;
            return NOERROR;
        }

        protected virtual long SizeMax() 
        {
            long _size = 0;
            MemoryStream _stream = new MemoryStream();
            try
            {
                _stream = new MemoryStream();
                HRESULT hr = WriteToStream(_stream);
                if (hr == S_OK)
                {
                    _size = _stream.Length;
                }
            }
            finally
            {
                _stream.Dispose();
            }
            return _size;
        }

        protected virtual HRESULT WriteToStream(Stream _stream)
        {
            return NOERROR;
        }

        protected virtual HRESULT ReadFromStream(Stream _stream)
        {
            return NOERROR;
        }

        #endregion

        #region Registry Helper Functions

        protected object GetFilterRegistryValue(string _name,object _default)
        {
            if (!string.IsNullOrEmpty(_name))
            {
                Microsoft.Win32.RegistryKey _key = null;
                try
                {
                    string _path = "CLSID\\" + this.GetType().GUID.ToString("B") + "\\" + m_csRegistryPath;
                    _key = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(_path);
                    return _key.GetValue(_name, _default);
                }
                catch (Exception _exception)
                {
                    HRESULT hr = (HRESULT)Marshal.GetHRForException(_exception);
                    hr.TraceWrite();
                }
                finally
                {
                    if (_key != null)
                    {
                        _key.Close();
                    }
                }
            }
            return _default;
        }

        protected bool SetFilterRegistryValue(string _name,object _value)
        {
            if (!string.IsNullOrEmpty(_name))
            {
                Microsoft.Win32.RegistryKey _key = null;
                try
                {
                    string _path = "CLSID\\" + this.GetType().GUID.ToString("B") + "\\" + m_csRegistryPath;
                    _key = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(_path);
                    _key.SetValue(_name, _value);
                    return true;
                }
                catch (Exception _exception)
                {
                    HRESULT hr = (HRESULT)Marshal.GetHRForException(_exception);
                    hr.TraceWrite();
                }
                finally
                {
                    if (_key != null)
                    {
                        _key.Close();
                    }
                }
            }
            return false;
        }

        #endregion

        #region Setup Helper Methods

        protected virtual int BeforeInstall(ref RegFilter2 _reginfo,ref IFilterMapper2 _mapper2)
        {
            return NOERROR;
        }

        protected virtual int AfterInstall(HRESULT hr,ref RegFilter2 _reginfo, ref IFilterMapper2 _mapper2)
        {
            return NOERROR;
        }

        protected virtual int BeforeUninstall(ref IFilterMapper2 _mapper2)
        {
            return NOERROR;
        }

        protected virtual int AfterUninstall(HRESULT hr, ref IFilterMapper2 _mapper2)
        {
            return NOERROR;
        }

        #endregion

        #region DLLSetup

        [ComRegisterFunctionAttribute]
        [RegistryPermissionAttribute(SecurityAction.Demand, Unrestricted = true)]
        public static void RegisterFunction(Type _type)
        {
            if (_type.IsClass)
            {
                AMovieSetup _setup = (AMovieSetup)Attribute.GetCustomAttribute(_type, typeof(AMovieSetup));
                if (_setup != null && _setup.ShouldRegister)
                {
                    if (_type.IsSubclassOf(typeof(BaseFilter)) || (_setup.Name != null && _setup.Name != ""))
                    {
                        BaseFilter _filter = (BaseFilter)Activator.CreateInstance(_type);
                        string _instance = null;
                        string _name = "";
                        if (_setup.Name != null && _setup.Name != "")
                        {
                            _name = _setup.Name;
                        }
                        else
                        {
                            _name = _filter.Name;
                        }
                        DsGuid _category = null;
                        if (_setup.Category != null && _setup.Category != Guid.Empty && _setup.Category != FilterCategory.LegacyAmFilterCategory)
                        {
                            _category = new DsGuid(_setup.Category);
                        }
                        IFilterMapper2 _mapper2 = (IFilterMapper2)new FilterMapper2();
                        ASSERT(_mapper2);
                        if (_mapper2 != null)
                        {
                            int hr = _mapper2.UnregisterFilter(_category, _instance, _type.GUID);

                            RegFilter2 _reg2 = new RegFilter2();
                            _reg2.dwVersion = (int)_setup.Version;
                            _reg2.dwMerit = _setup.FilterMerit;
                            _reg2.rgPins = IntPtr.Zero;
                            _reg2.cPins = 0;

                            hr = _filter.BeforeInstall(ref _reg2, ref _mapper2);
                            if (SUCCEEDED(hr))
                            {
                                IntPtr _register = Marshal.AllocCoTaskMem(Marshal.SizeOf(_reg2));
                                Marshal.StructureToPtr(_reg2, _register, true);

                                hr = _mapper2.RegisterFilter(_type.GUID, _name, IntPtr.Zero, _category, _instance, _register);
                                if (E_FILE_NOT_FOUND == hr)
                                {
                                    hr = NOERROR;
                                }

                                Marshal.FreeCoTaskMem(_register);
                                _filter.AfterInstall((HRESULT)hr, ref _reg2, ref _mapper2);
                            }
                            ASSERT(SUCCEEDED(hr));
                            _mapper2 = null;
                        }
                        _filter = null;
                    }
                }
            }
        }

        [ComUnregisterFunctionAttribute]
        [RegistryPermissionAttribute(SecurityAction.Demand, Unrestricted = true)]
        public static void UnregisterFunction(Type _type)
        {
            if (_type.IsClass)
            {
                AMovieSetup _setup = (AMovieSetup)Attribute.GetCustomAttribute(_type, typeof(AMovieSetup));
                if (_setup != null && _setup.ShouldRegister)
                {
                    if (_type.IsSubclassOf(typeof(BaseFilter)) || (_setup.Name != null && _setup.Name != ""))
                    {
                        BaseFilter _filter = (BaseFilter)Activator.CreateInstance(_type);
                        string _instance = null;
                        string _name = "";
                        if (_setup.Name != null && _setup.Name != "")
                        {
                            _name = _setup.Name;
                        }
                        else
                        {
                            _name = _filter.Name;
                        }
                        DsGuid _category = null;
                        if (_setup.Category != null && _setup.Category != Guid.Empty && _setup.Category != FilterCategory.LegacyAmFilterCategory)
                        {
                            _category = new DsGuid(_setup.Category);
                        }
                        IFilterMapper2 _mapper2 = (IFilterMapper2)new FilterMapper2();
                        ASSERT(_mapper2);
                        if (_mapper2 != null)
                        {
                            int hr = _filter.BeforeUninstall(ref _mapper2);

                            hr = _mapper2.UnregisterFilter(_category, _instance, _type.GUID);

                            hr = _filter.AfterUninstall((HRESULT)hr,ref _mapper2);
                            ASSERT(SUCCEEDED(hr));
                            _mapper2 = null;
                        }
                        _filter = null;
                        Microsoft.Win32.RegistryKey _key = null;
                        try
                        {
                            string _path = "CLSID\\" + _type.GUID.ToString("B");
                            _key = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(_path);
                            if (_key != null)
                            {
                                _key.DeleteSubKeyTree(m_csRegistryPath);
                            }
                        }
                        catch
                        {
                        }
                        finally
                        {
                            if (_key != null)
                            {
                                _key.Close();
                            }
                        }
                    }
                }
            }
        }
        
        #endregion
    }

    #endregion

    #region Transform Input Pin

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class TransformInputPin : BaseInputPin
    {
        #region Constructor

        public TransformInputPin(string _name, TransformFilter _filter)
            : base(_name, _filter)
        {

        }

        #endregion

        #region Overriden Methods

        public override int CheckConnect(ref IPinImpl _pin)
        {
            
            int hr = ((TransformFilter)m_Filter).CheckConnect(PinDirection.Input, ref _pin);
            if (FAILED(hr))
            {
                return hr;
            }
            return base.CheckConnect(ref _pin);
        }

        public override int BreakConnect()
        {
            
            ASSERT(IsStopped);
            ((TransformFilter)m_Filter).BreakConnect(PinDirection.Input);
            return base.BreakConnect();
        }

        public override int CompleteConnect(ref IPinImpl pReceivePin)
        {
            
            int hr = ((TransformFilter)m_Filter).CompleteConnect(PinDirection.Input, ref pReceivePin);
            if (FAILED(hr))
            {
                return hr;
            }
            return base.CompleteConnect(ref pReceivePin);
        }

        public override int CheckMediaType(AMMediaType mt)
        {
            
            int hr = ((TransformFilter)m_Filter).CheckInputType(mt);
            if (S_OK != hr)
            {
                return hr;
            }

            // if the output pin is still connected, then we have
            // to check the transform not just the input format

            if ((((TransformFilter)m_Filter).Output != NULL) &&
                (((TransformFilter)m_Filter).Output.IsConnected))
            {
                return ((TransformFilter)m_Filter).CheckTransform(
                          mt,
                  ((TransformFilter)m_Filter).Output.CurrentMediaType);
            }
            else
            {
                return hr;
            }
        }

        public override int SetMediaType(AMMediaType mt)
        {
            int hr = base.SetMediaType(mt);
            if (FAILED(hr))
            {
                return hr;
            }

            ASSERT(SUCCEEDED(((TransformFilter)m_Filter).CheckInputType(mt)));

            return ((TransformFilter)m_Filter).SetMediaType(PinDirection.Input, mt);
        }

        public override int CheckStreaming()
        {
            ASSERT(((TransformFilter)m_Filter).Output != NULL);
            if (!((TransformFilter)m_Filter).Output.IsConnected)
            {
                return VFW_E_NOT_CONNECTED;
            }
            else
            {
                //  Shouldn't be able to get any data if we're not connected!
                ASSERT(IsConnected);

                //  we're flushing
                if (m_bFlushing)
                {
                    return S_FALSE;
                }
                //  Don't process stuff in Stopped state
                if (IsStopped)
                {
                    return VFW_E_WRONG_STATE;
                }
                return S_OK;
            }
        }

        public override int OnReceive(ref IMediaSampleImpl _sample)
        {
            int hr = 0;
            lock (((TransformFilter)m_Filter).ReceiveLock)
            {
                // check all is well with the base class
                hr = base.OnReceive(ref _sample);
                if (S_OK == hr)
                {
                    hr = ((TransformFilter)m_Filter).OnReceive(ref _sample);
                }
                
                return hr;
            }

        }

        public override int EndOfStream()
        {
            lock (((TransformFilter)m_Filter).ReceiveLock)
            {
                int hr = CheckStreaming();
                if (S_OK == hr)
                {
                    hr = ((TransformFilter)m_Filter).EndOfStream();
                }
                return hr;
            }
        }

        public override int BeginFlush()
        {
            lock (m_Filter.FilterLock)
            {
                //  Are we actually doing anything?
                ASSERT(((TransformFilter)m_Filter).Output != NULL);
                if (!IsConnected ||
                    !((TransformFilter)m_Filter).Output.IsConnected)
                {
                    return VFW_E_NOT_CONNECTED;
                }
                int hr = base.BeginFlush();
                if (FAILED(hr))
                {
                    return hr;
                }

                return ((TransformFilter)m_Filter).BeginFlush();
            }
        }

        public override int EndFlush()
        {
            lock (m_Filter.FilterLock)
            {
                //  Are we actually doing anything?
                ASSERT(((TransformFilter)m_Filter).Output != NULL);
                if (!IsConnected ||
                    !((TransformFilter)m_Filter).Output.IsConnected)
                {
                    return VFW_E_NOT_CONNECTED;
                }

                int hr = ((TransformFilter)m_Filter).EndFlush();
                if (FAILED(hr))
                {
                    return hr;
                }

                return base.EndFlush();
            }
        }

        public override int NewSegment(long tStart, long tStop, double dRate)
        {
            base.NewSegment(tStart, tStop, dRate);
            return ((TransformFilter)m_Filter).NewSegment(tStart, tStop, dRate);
        }

        #endregion
    };

    #endregion

    #region Transform Output Pin

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class TransformOutputPin : BaseOutputPin, IMediaSeeking
    {
        #region Constructor

        public TransformOutputPin(string _name, TransformFilter _filter)
            : base(_name, _filter)
        {

        }

        ~TransformOutputPin()
        {
        }

        #endregion

        #region Properties

        protected IMediaSeekingImpl Seeking
        {
            get
            {
                return ((TransformFilter)m_Filter).Input.Seeking;
            }
        }

        #endregion

        #region Overriden Methods

        public override int BreakConnect()
        {
            ASSERT(IsStopped);
            ((TransformFilter)m_Filter).BreakConnect(PinDirection.Output);
            return base.BreakConnect();
        }

        public override int CheckConnect(ref IPinImpl _pin)
        {
            ASSERT(((TransformFilter)m_Filter).Input != null);
            if (((TransformFilter)m_Filter).Input.IsConnected == false)
            {
                return E_UNEXPECTED;
            }

            int hr = ((TransformFilter)m_Filter).CheckConnect(PinDirection.Output, ref _pin);
            if (FAILED(hr))
            {
                return hr;
            }
            return base.CheckConnect(ref _pin);
        }

        public override int CompleteConnect(ref IPinImpl pReceivePin)
        {
            int hr = ((TransformFilter)m_Filter).CompleteConnect(PinDirection.Output, ref pReceivePin);
            if (FAILED(hr))
            {
                return hr;
            }
            return base.CompleteConnect(ref pReceivePin);
        }

        public override int CheckMediaType(AMMediaType mt)
        {
            // must have selected input first
            ASSERT(((TransformFilter)m_Filter).Input != null);
            if (!((TransformFilter)m_Filter).Input.IsConnected)
            {
                return E_INVALIDARG;
            }

            return ((TransformFilter)m_Filter).CheckTransform(
                            ((TransformFilter)m_Filter).Input.CurrentMediaType,
                            mt);
        }

        public override int SetMediaType(AMMediaType mt)
        {
            int hr = NOERROR;
            ASSERT(((TransformFilter)m_Filter).Input != NULL);
            ASSERT(AMMediaType.IsValid(((TransformFilter)m_Filter).Input.CurrentMediaType));

            // Set the base class media type (should always succeed)
            hr = base.SetMediaType(mt);
            if (FAILED(hr))
            {
                return hr;
            }

            return ((TransformFilter)m_Filter).SetMediaType(PinDirection.Output, mt);
        }

        public override int DecideBufferSize(ref IMemAllocatorImpl pAlloc, ref AllocatorProperties prop)
        {
            return ((TransformFilter)m_Filter).DecideBufferSize(ref pAlloc, ref prop);
        }

        public override int GetMediaType(int iPosition, ref AMMediaType pMediaType)
        {
            ASSERT(((TransformFilter)m_Filter).Input != null);

            if (((TransformFilter)m_Filter).Input.IsConnected)
            {
                return ((TransformFilter)m_Filter).GetMediaType(iPosition, ref pMediaType);
            }
            else
            {
                return VFW_S_NO_MORE_ITEMS;
            }
        }

        public override int Notify(IntPtr pSelf, Quality q)
        {
            int hr = ((TransformFilter)m_Filter).AlterQuality(q);
            if (hr != S_FALSE)
            {
                return hr;        // either S_OK or a failure
            }

            ASSERT(((TransformFilter)m_Filter).Input != null);

            return ((TransformFilter)m_Filter).Input.PassNotify(q);
        }

        #endregion

        #region IMediaSeeking Members

        public int CheckCapabilities(ref AMSeekingSeekingCapabilities pCapabilities)
        {
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking != null)
            {
                return _seeking.CheckCapabilities(ref pCapabilities);
            }
            return E_NOINTERFACE;
        }

        public int ConvertTimeFormat(out long pTarget, DsGuid pTargetFormat, long Source, DsGuid pSourceFormat)
        {
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking != null)
            {
                return _seeking.ConvertTimeFormat(out pTarget, pTargetFormat, Source, pSourceFormat);
            }
            pTarget = 0;
            return E_NOINTERFACE;
        }

        public int GetAvailable(out long pEarliest, out long pLatest)
        {
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking != null)
            {
                return _seeking.GetAvailable(out pEarliest, out pLatest);
            }
            pLatest = 0;
            pEarliest = 0;
            return E_NOINTERFACE;
        }

        public int GetCapabilities(out AMSeekingSeekingCapabilities pCapabilities)
        {
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking != null)
            {
                return _seeking.GetCapabilities(out pCapabilities);
            }
            pCapabilities = 0;
            return E_NOINTERFACE;
        }

        public int GetCurrentPosition(out long pCurrent)
        {
            IMediaSeeking _seeking = (IMediaSeeking)Seeking;
            if (_seeking != null)
            {
                return _seeking.GetCurrentPosition(out pCurrent);
            }
            pCurrent = 0;
            return E_NOINTERFACE;
        }

        public int GetDuration(out long pDuration)
        {
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking != null)
            {
                return _seeking.GetDuration(out pDuration);
            }
            pDuration = 0;
            return E_NOINTERFACE;
        }

        public int GetPositions(out long pCurrent, out long pStop)
        {
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking != null)
            {
                return _seeking.GetPositions(out pCurrent, out pStop);
            }
            pCurrent = 0;
            pStop = 0;
            return E_NOINTERFACE;
        }

        public int GetPreroll(out long pllPreroll)
        {
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking != null)
            {
                return _seeking.GetPreroll(out pllPreroll);
            }
            pllPreroll = 0;
            return E_NOINTERFACE;
        }

        public int GetRate(out double pdRate)
        {
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking != null)
            {
                return _seeking.GetRate(out pdRate);
            }
            pdRate = 0;
            return E_NOINTERFACE;
        }

        public int GetStopPosition(out long pStop)
        {
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking != null)
            {
                return _seeking.GetStopPosition(out pStop);
            }
            pStop = 0;
            return E_NOINTERFACE;
        }

        public int GetTimeFormat(out Guid pFormat)
        {
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking != null)
            {
                return _seeking.GetTimeFormat(out pFormat);
            }
            pFormat = Guid.Empty;
            return E_NOINTERFACE;
        }

        public int IsFormatSupported(Guid pFormat)
        {
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking != null)
            {
                return _seeking.IsFormatSupported(pFormat);
            }
            return E_NOINTERFACE;
        }

        public int IsUsingTimeFormat(Guid pFormat)
        {
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking != null)
            {
                return _seeking.IsUsingTimeFormat(pFormat);
            }
            return E_NOINTERFACE;
        }

        public int QueryPreferredFormat(out Guid pFormat)
        {
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking != null)
            {
                return _seeking.QueryPreferredFormat(out pFormat);
            }
            pFormat = Guid.Empty;
            return E_NOINTERFACE;
        }

        public int SetPositions(DsLong pCurrent, AMSeekingSeekingFlags dwCurrentFlags, DsLong pStop, AMSeekingSeekingFlags dwStopFlags)
        {
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking != null)
            {
                return _seeking.SetPositions(pCurrent, dwCurrentFlags, pStop, dwStopFlags);
            };
            return E_NOINTERFACE;
        }

        public int SetRate(double dRate)
        {
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking != null)
            {
                return _seeking.SetRate(dRate);
            };
            return E_NOINTERFACE;
        }

        public int SetTimeFormat(Guid pFormat)
        {
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking != null)
            {
                return _seeking.SetTimeFormat(pFormat);
            };
            return E_NOINTERFACE;
        }

        #endregion
    }

    #endregion

    #region Transform Filter

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class TransformFilter : BaseFilter
    {
        #region Variables

        protected bool m_bEOSDelivered = false;
        protected bool m_bQualityChanged = false;
        protected bool m_bSampleSkipped = false;
        protected object m_csReceive = new object();

        #endregion

        #region Properties

        public object ReceiveLock
        {
            get { return m_csReceive; }
        }

        public TransformInputPin Input
        {
            get
            {
                if (Pins.Count >= 1)
                {
                    return (TransformInputPin)Pins[0];
                }
                return null;
            }
        }

        public TransformOutputPin Output
        {
            get
            {
                if (Pins.Count >= 2)
                {
                    return (TransformOutputPin)Pins[1];
                }
                return null;
            }
        }

        #endregion

        #region Constructor

        public TransformFilter(string _name)
            : base(_name)
        {

        }

        #endregion

        #region Methods

        #region Abstract Methods

        public abstract int CheckInputType(AMMediaType pmt);

        public abstract int CheckTransform(AMMediaType mtIn, AMMediaType mtOut);

        public abstract int DecideBufferSize(ref IMemAllocatorImpl pAlloc, ref AllocatorProperties prop);

        public abstract int GetMediaType(int iPosition, ref AMMediaType pMediaType);

        public abstract int Transform(ref IMediaSampleImpl pIn, ref IMediaSampleImpl pOut);

        #endregion

        #region Overridden Methods

        protected override int OnInitializePins()
        {
            AddPin(new TransformInputPin("XForm In", this));
            AddPin(new TransformOutputPin("XForm Out", this));
            return NOERROR;
        }

        public override int FindPin(string Id, out IPin ppPin)
        {
            if (Id.IndexOf("In") != -1)
            {
                ppPin = Input;
                return NOERROR;
            }
            if (Id.IndexOf("Out") != -1)
            {
                ppPin = Output;
                return NOERROR;
            }
            return base.FindPin(Id, out ppPin);
        }

        public override int Pause()
        {
            lock (m_Lock)
            {
                int hr = NOERROR;
                if (m_State == FilterState.Paused)
                {
                    // (This space left deliberately blank)
                }
                else
                    if (Input == null || !Input.IsConnected)
                    {
                        if (Output != null && !m_bEOSDelivered)
                        {
                            Output.DeliverEndOfStream();
                            m_bEOSDelivered = true;
                        }
                        m_State = FilterState.Paused;
                    }
                    else
                        if (!Output.IsConnected)
                        {
                            m_State = FilterState.Paused;
                        }
                        else
                        {
                            if (m_State == FilterState.Stopped)
                            {
                                lock (m_csReceive)
                                {
                                    hr = StartStreaming();
                                }
                            }
                            if (SUCCEEDED(hr))
                            {
                                hr = base.Pause();
                            }
                        }

                m_bSampleSkipped = false;
                m_bQualityChanged = false;
                return hr;
            }
        }

        public override int Stop()
        {
            lock (m_Lock)
            {
                if (m_State == FilterState.Stopped)
                {
                    return NOERROR;
                }
                ASSERT(Input == null || Output != null);
                if (Input == NULL || !Input.IsConnected || !Output.IsConnected)
                {
                    m_State = FilterState.Stopped;
                    m_bEOSDelivered = false;
                    return NOERROR;
                }
                Input.Inactive();

                lock (m_csReceive)
                {
                    Output.Inactive();
                    int hr = StopStreaming();
                    if (SUCCEEDED(hr))
                    {
                        // complete the state transition
                        m_State = FilterState.Stopped;
                        m_bEOSDelivered = false;
                    }
                    return hr;
                }
            }
        }

        #endregion

        #region Helper Methods for Overload

        public virtual int AlterQuality(Quality q)
        {
            return S_FALSE;
        }

        public virtual int BeginFlush()
        {
            int hr = NOERROR;
            if (Output != null)
            {
                hr = Output.DeliverBeginFlush();
            }
            return hr;
        }

        public virtual int EndFlush()
        {
            ASSERT(Output != null);
            return Output.DeliverEndFlush();
        }

        public virtual int EndOfStream()
        {
            int hr = NOERROR;
            if (Output != null)
            {
                hr = Output.DeliverEndOfStream();
            }
            return hr;
        }

        public virtual int NewSegment(long tStart, long tStop, double dRate)
        {
            int hr = NOERROR;
            if (Output != null)
            {
                hr = Output.DeliverNewSegment(tStart, tStop, dRate);
            }
            return hr;
        }

        public virtual int BreakConnect(PinDirection _direction)
        {
            return NOERROR;
        }

        public virtual int CheckConnect(PinDirection _direction, ref IPinImpl pPin)
        {
            return NOERROR;
        }

        public virtual int CompleteConnect(PinDirection _direction, ref IPinImpl pPin)
        {
            return NOERROR;
        }

        public virtual int SetMediaType(PinDirection _direction, AMMediaType mt)
        {
            return NOERROR;
        }

        public virtual int StartStreaming()
        {
            return NOERROR;
        }

        public virtual int StopStreaming()
        {
            return NOERROR;
        }

        #endregion

        #region Other Methods

        protected virtual int InitializeOutputSample(ref IMediaSampleImpl _sample, out IMediaSampleImpl ppOutSample)
        {
            AMSample2Properties pProps = Input.SampleProps;
            AMGBF dwFlags = m_bSampleSkipped ? AMGBF.PrevFrameSkipped : 0;
            ppOutSample = null;
            if ((pProps.dwSampleFlags & AMSamplePropertyFlags.SplicePoint) == 0)
            {
                dwFlags |= AMGBF.NotAsyncPoint;
            }
            IntPtr pOutputSample;
            int hr = Output.GetDeliveryBuffer(
                    out pOutputSample
                 , ((pProps.dwSampleFlags & AMSamplePropertyFlags.TimeValid) > 0) ? (DsLong)pProps.tStart : null
                 , ((pProps.dwSampleFlags & AMSamplePropertyFlags.StopValid) > 0) ? (DsLong)pProps.tStop : null
                 , dwFlags
                );
            if (FAILED(hr))
            {
                return hr;
            }
            ppOutSample = new IMediaSampleImpl(pOutputSample);
            Guid _guid = typeof(IMediaSample2).GUID;
            IntPtr pSample2;
            if (S_OK == ppOutSample._QueryInterface(ref _guid, out pSample2))
            {
                IMediaSample2Impl _sample2 = new IMediaSample2Impl(pSample2);
                int cb = Marshal.SizeOf(typeof(AMSample2Properties));
                IntPtr _properties = Marshal.AllocCoTaskMem(cb);
                if (SUCCEEDED(_sample2.GetProperties(cb, _properties)))
                {
                    AMSample2Properties OutProps = (AMSample2Properties)Marshal.PtrToStructure(_properties, typeof(AMSample2Properties));
                    OutProps.tStart = pProps.tStart;
                    OutProps.tStop = pProps.tStop;
                    OutProps.dwSampleFlags = (OutProps.dwSampleFlags & AMSamplePropertyFlags.TypeChanged)
                        | (pProps.dwSampleFlags & ~AMSamplePropertyFlags.TypeChanged);
                    OutProps.cbData = Marshal.OffsetOf(typeof(AMSample2Properties), "dwStreamId").ToInt32();
                    Marshal.StructureToPtr(OutProps, _properties, true);
                    hr = _sample2.SetProperties(OutProps.cbData, _properties);
                }
                Marshal.FreeCoTaskMem(_properties);

                if ((pProps.dwSampleFlags & AMSamplePropertyFlags.DataDiscontinuity) > 0)
                {
                    m_bSampleSkipped = false;
                }
                _sample2._Release();
            }
            else
            {
                if ((pProps.dwSampleFlags & AMSamplePropertyFlags.TimeValid) > 0)
                {
                    ppOutSample.SetTime((DsLong)pProps.tStart, (DsLong)pProps.tStop);
                }
                if ((pProps.dwSampleFlags & AMSamplePropertyFlags.SplicePoint) > 0)
                {
                    ppOutSample.SetSyncPoint(true);
                }
                if ((pProps.dwSampleFlags & AMSamplePropertyFlags.DataDiscontinuity) > 0)
                {
                    ppOutSample.SetDiscontinuity(true);
                    m_bSampleSkipped = false;
                }

                long MediaStart, MediaEnd;
                if (_sample.GetMediaTime(out MediaStart, out MediaEnd) == NOERROR)
                {
                    ppOutSample.SetMediaTime((DsLong)MediaStart, (DsLong)MediaEnd);
                }
            }
            return S_OK;
        }

        public virtual int OnReceive(ref IMediaSampleImpl _sample)
        {
            AMSample2Properties pProps = Input.SampleProps;
            IMediaSampleImpl pOutSample = null;
            try
            {
                if (pProps.dwStreamId != (int)AMSamplePropertyFlags.Media)
                {
                    return Output.Deliver(ref _sample);
                }
                int hr = InitializeOutputSample(ref _sample, out pOutSample);

                if (FAILED(hr))
                {
                    return hr;
                }
                hr = Transform(ref _sample, ref pOutSample);
                if (hr == NOERROR)
                {
                    hr = Output.Deliver(ref pOutSample);
                    m_bSampleSkipped = false;	// last thing no long
                }
                else
                {
                    if (S_FALSE == hr)
                    {
                        m_bSampleSkipped = true;
                        if (!m_bQualityChanged)
                        {
                            NotifyEvent(EventCode.QualityChange, IntPtr.Zero, IntPtr.Zero);
                            m_bQualityChanged = true;
                        }
                        return NOERROR;
                    }
                }
                return hr;
            }
            finally
            {
                if (pOutSample != null)
                {
                    pOutSample._Release();
                }
            }
        }

        #endregion

        #endregion

    }

    #endregion

    #region TransInPlace Input Pin

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class TransInPlaceInputPin : TransformInputPin
    {
        #region Constructor

        public TransInPlaceInputPin(string _name, TransInPlaceFilter _filter)
            : base(_name, _filter)
        {
            m_bReadOnly = false;
        }

        #endregion

        #region Overridden Methods

        public override int CheckMediaType(AMMediaType mt)
        {
            int hr = ((TransInPlaceFilter)m_Filter).CheckInputType(mt);
            if (hr != S_OK) return hr;

            if (((TransInPlaceFilter)m_Filter).Output.IsConnected)
                return ((TransInPlaceFilter)m_Filter).Output.Connected.QueryAccept(mt);
            else
                return S_OK;
        }

        public new int CompleteConnect(ref IPinImpl pReceivePin)
        {
            int hr = base.CompleteConnect(ref pReceivePin);
            if (FAILED(hr))
            {
                return hr;
            }

            return ((TransformFilter)m_Filter).CompleteConnect(PinDirection.Input, ref pReceivePin);
        }

        //public override int EnumMediaTypes(out IEnumMediaTypes ppEnum)
        public override int EnumMediaTypes(out IntPtr ppEnum)
        {
            if (!((TransInPlaceFilter)m_Filter).Output.IsConnected)
            {
                ppEnum = IntPtr.Zero;
                return VFW_E_NOT_CONNECTED;
            }
            return ((TransInPlaceFilter)m_Filter).Output.Connected.EnumMediaTypes(out ppEnum);
        }

        public override int GetAllocator(out IntPtr ppAllocator)
        {
            lock (m_Lock)
            {
                int hr = NOERROR;
                if (((TransInPlaceFilter)m_Filter).Output.IsConnected)
                {
                    hr = ((TransInPlaceFilter)m_Filter).Output.InputPin.GetAllocator(out ppAllocator);
                    if (SUCCEEDED(hr))
                    {
                        ((TransInPlaceFilter)m_Filter).Output.AllocatorPtr = ppAllocator;
                    }
                }
                else
                {
                    hr = base.GetAllocator(out ppAllocator);
                }
                return hr;
            }
        }

        public override int GetAllocatorRequirements(AllocatorProperties pProps)
        {
            if (((TransInPlaceFilter)m_Filter).Output.IsConnected)
                return ((TransInPlaceFilter)m_Filter).Output.InputPin.GetAllocatorRequirements(pProps);
            else
                return base.GetAllocatorRequirements(pProps);
        }

        public override int NotifyAllocator(IntPtr pAllocator, bool bReadOnly)
        {
            lock (m_Lock)
            {
                m_bReadOnly = bReadOnly;

                if (!((TransInPlaceFilter)m_Filter).Output.IsConnected)
                {
                    return base.NotifyAllocator(pAllocator, bReadOnly);
                }
                int hr = NOERROR;

                if (bReadOnly && ((TransInPlaceFilter)m_Filter).ModifiesData)
                {
                    IntPtr pOutputAllocator = ((TransInPlaceFilter)m_Filter).Output.AllocatorPtr;
                    if (pOutputAllocator == IntPtr.Zero)
                    {
                        hr = ((TransInPlaceFilter)m_Filter).Output.InputPin.GetAllocator(out pOutputAllocator);
                        if (FAILED(hr))
                        {
                            hr = CreateMemoryAllocator(out pOutputAllocator);
                        }
                        if (SUCCEEDED(hr))
                        {
                            ((TransInPlaceFilter)m_Filter).Output.AllocatorPtr = pOutputAllocator;
                        }
                    }
                    if (pAllocator == pOutputAllocator)
                    {
                        return E_FAIL;
                    }
                    else
                        if (SUCCEEDED(hr))
                        {
                            //  Must copy so set the allocator properties on the output
                            AllocatorProperties Props = new AllocatorProperties(), Actual = new AllocatorProperties();

                            hr = ((IMemAllocator)Marshal.GetObjectForIUnknown(pAllocator)).GetProperties(Props);
                            GC.Collect();
                            if (SUCCEEDED(hr))
                            {
                                hr = ((IMemAllocator)Marshal.GetObjectForIUnknown(pOutputAllocator)).SetProperties(Props, Actual);
                                GC.Collect();
                            }
                            if (SUCCEEDED(hr))
                            {
                                if ((Props.cBuffers > Actual.cBuffers)
                                   || (Props.cbBuffer > Actual.cbBuffer)
                                   || (Props.cbAlign > Actual.cbAlign)
                                   )
                                {
                                    hr = E_FAIL;
                                }
                            }

                            //  Set the allocator on the output pin
                            if (SUCCEEDED(hr))
                            {
                                hr = ((TransInPlaceFilter)m_Filter).Output.InputPin.NotifyAllocator(pOutputAllocator, false);
                            }
                        }
                }
                else
                {
                    hr = ((TransInPlaceFilter)m_Filter).Output.InputPin.NotifyAllocator(pAllocator, bReadOnly);
                    if (SUCCEEDED(hr))
                    {
                        ((TransInPlaceFilter)m_Filter).Output.AllocatorPtr = pAllocator;
                    }
                }

                if (SUCCEEDED(hr))
                {
                    AllocatorPtr = pAllocator;
                }
                return hr;
            }
        }

        #endregion
    }

    #endregion

    #region TransInPlace Output Pin

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class TransInPlaceOutputPin : TransformOutputPin
    {
        #region Constructor

        public TransInPlaceOutputPin(string _name, TransInPlaceFilter _filter)
            : base(_name, _filter)
        {

        }

        #endregion

        #region Overridden Methods

        public override int CheckMediaType(AMMediaType mt)
        {
            if (((TransInPlaceFilter)m_Filter).UsingDifferentAllocators && !m_Filter.IsStopped)
            {
                if (AMMediaType.AreEquals(mt, m_mt))
                {
                    return S_OK;
                }
                else
                {
                    return VFW_E_TYPE_NOT_ACCEPTED;
                }
            }
            // Assumes the type does not change.  That's why we're calling
            // CheckINPUTType here on the OUTPUT pin.
            int hr = ((TransInPlaceFilter)m_Filter).CheckInputType(mt);
            if (hr != S_OK) return hr;

            if (((TransInPlaceFilter)m_Filter).Input.IsConnected)
                return ((TransInPlaceFilter)m_Filter).Input.Connected.QueryAccept(mt);
            else
                return S_OK;
        }

        public new int CompleteConnect(ref IPinImpl pReceivePin)
        {
            int hr = base.CompleteConnect(ref pReceivePin);
            if (FAILED(hr))
            {
                return hr;
            }

            return ((TransformFilter)m_Filter).CompleteConnect(PinDirection.Output, ref pReceivePin);
        }

        //public override int EnumMediaTypes(out IEnumMediaTypes ppEnum)
        public override int EnumMediaTypes(out IntPtr ppEnum)
        {
            if (!((TransInPlaceFilter)m_Filter).Input.IsConnected)
            {
                ppEnum = IntPtr.Zero;
                return VFW_E_NOT_CONNECTED;
            }
            return ((TransInPlaceFilter)m_Filter).Input.Connected.EnumMediaTypes(out ppEnum);
        }

        #endregion
    }

    #endregion

    #region TransInPlace Filter

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class TransInPlaceFilter : TransformFilter
    {
        #region Variabless

        protected bool m_bModifiesData = true;

        #endregion

        #region Properties

        public bool ModifiesData
        {
            get { return m_bModifiesData; }
        }

        public bool UsingDifferentAllocators
        {
            get { return Input.Allocator != Output.Allocator; }
        }

        public bool TypesMatch
        {
            get
            {
                return AMMediaType.AreEquals(Input.CurrentMediaType, Output.CurrentMediaType);
            }
        }

        #endregion

        #region Constructor

        public TransInPlaceFilter(string _name)
            : base(_name)
        {

        }

        public TransInPlaceFilter(string _name, bool bModifiesData)
            : this(_name)
        {
            m_bModifiesData = bModifiesData;
        }

        #endregion

        #region Overridden Methods

        protected override int OnInitializePins()
        {
            AddPin(new TransInPlaceInputPin("XForm In", this));
            AddPin(new TransInPlaceOutputPin("XForm Out", this));
            return NOERROR;
        }

        public override int CheckTransform(AMMediaType mtIn, AMMediaType mtOut)
        {
            return S_OK;
        }

        public override int GetMediaType(int iPosition, ref AMMediaType pMediaType)
        {
            return E_UNEXPECTED;
        }

        public override int CompleteConnect(PinDirection _direction, ref IPinImpl pPin)
        {
            ASSERT(Input);
            ASSERT(Output);

            if (m_pGraph == null)
            {
                return VFW_E_NOT_IN_GRAPH;
            }

            if (_direction == PinDirection.Output)
            {
                if (Input.IsConnected)
                {
                    return ReconnectPin(Input, Output.CurrentMediaType);
                }
                return NOERROR;
            }

            if (Output.IsConnected)
            {
                if (!TypesMatch)
                {
                    return ReconnectPin(Output, Input.CurrentMediaType);
                }
            }
            return NOERROR;
        }

        public override int DecideBufferSize(ref IMemAllocatorImpl pAlloc, ref AllocatorProperties prop)
        {
            AllocatorProperties Request = new AllocatorProperties(), Actual = new AllocatorProperties();
            int hr;
            if (Input.IsConnected)
            {
                // Get the input pin allocator, and get its size and count.
                // we don't care about his alignment and prefix.

                hr = Input.Allocator.GetProperties(Request);
                if (FAILED(hr))
                {
                    // Input connected but with a secretive allocator - enough!
                    return hr;
                }
            }
            else
            {
                // We're reduced to blind guessing.  Let's guess one byte and if
                // this isn't enough then when the other pin does get connected
                // we can revise it.
                Request.cBuffers = 1;
                Request.cbBuffer = 1;
                Request.cbAlign = 0;
                Request.cbPrefix = 0;
            }

            prop.cBuffers = Request.cBuffers;
            prop.cbBuffer = Request.cbBuffer;
            prop.cbAlign = Request.cbAlign;
            if (prop.cBuffers <= 0) { prop.cBuffers = 1; }
            if (prop.cbBuffer <= 0) { prop.cbBuffer = 1; }
            hr = pAlloc.SetProperties(prop, Actual);

            if (FAILED(hr))
            {
                return hr;
            }
            if ((Request.cBuffers > Actual.cBuffers)
                   || (Request.cbBuffer > Actual.cbBuffer)
                   || (Request.cbAlign > Actual.cbAlign)
                   )
            {
                return E_FAIL;
            }
            return NOERROR;
        }

        public override int OnReceive(ref IMediaSampleImpl _sample)
        {
            AMSample2Properties pProps = Input.SampleProps;
            if (pProps.dwStreamId != (int)AMSamplePropertyFlags.Media)
            {
                return Output.Deliver(ref _sample);
            }
            int hr;
            IMediaSampleImpl _output = null;
            try
            {
                if (UsingDifferentAllocators)
                {
                    // We have to copy the data.
                    hr = Copy(ref _sample, out _output);
                    if (hr != S_OK) return hr;
                    if (_output == null)
                    {
                        return E_UNEXPECTED;
                    }
                }
                else
                {
                    _output = _sample;
                }
                hr = Transform(ref _output);

                if (FAILED(hr))
                {
                    return hr;
                }
                if (hr == NOERROR)
                {
                    hr = Output.Deliver(ref _output);
                }
                else
                {
                    if (S_FALSE == hr)
                    {
                        m_bSampleSkipped = true;
                        if (!m_bQualityChanged)
                        {
                            NotifyEvent(EventCode.QualityChange, IntPtr.Zero, IntPtr.Zero);
                            m_bQualityChanged = true;
                        }
                        hr = NOERROR;
                    }
                }
            }
            finally
            {
                if (_output != null && UsingDifferentAllocators)
                {
                    _output._Release();
                }
            }
            return hr;
        }

        public override sealed int Transform(ref IMediaSampleImpl pIn, ref IMediaSampleImpl pOut)
        {
            return E_NOTIMPL;
        }

        #endregion

        #region Abstract Methods

        public abstract int Transform(ref IMediaSampleImpl _sample);

        #endregion

        #region Helper Methods

        protected int Copy(ref IMediaSampleImpl pSource, out IMediaSampleImpl pDest)
        {
            pDest = null;
            int hr;
            long tStart, tStop;
            bool bTime = S_OK == pSource.GetTime(out tStart, out tStop);

            IntPtr _dest;
            hr = Output.GetDeliveryBuffer(
                out _dest
              , bTime ? (DsLong)tStart : null
              , bTime ? (DsLong)tStop : null
              , m_bSampleSkipped ? AMGBF.PrevFrameSkipped : 0
                );
            if (FAILED(hr))
            {
                return hr;
            }
            pDest = new IMediaSampleImpl(_dest);

            Guid _guid = typeof(IMediaSample2).GUID;
            IntPtr pSample2;
            if (S_OK == pDest._QueryInterface(ref _guid, out pSample2))
            {
                IMediaSample2Impl _sample2 = new IMediaSample2Impl(pSample2);

                int cb = Marshal.SizeOf(Input.SampleProps);
                IntPtr _properties = Marshal.AllocCoTaskMem(cb);
                Marshal.StructureToPtr(Input.SampleProps, _properties, true);
                cb = Marshal.OffsetOf(typeof(AMSample2Properties), "pbBuffer").ToInt32();
                hr = _sample2.SetProperties(cb, _properties);
                Marshal.FreeCoTaskMem(_properties);
                _sample2._Release();
                if (FAILED(hr))
                {
                    pDest = null;
                    return hr;
                }
            }
            else
            {
                if (bTime)
                {
                    pDest.SetTime((DsLong)tStart, (DsLong)tStop);
                }

                if (S_OK == pSource.IsSyncPoint())
                {
                    pDest.SetSyncPoint(true);
                }
                if (S_OK == pSource.IsDiscontinuity() || m_bSampleSkipped)
                {
                    pDest.SetDiscontinuity(true);
                }
                if (S_OK == pSource.IsPreroll())
                {
                    pDest.SetPreroll(true);
                }

                // Copy the media type
                AMMediaType mt;
                if (S_OK == pSource.GetMediaType(out mt))
                {
                    pDest.SetMediaType(mt);
                    AMMediaType.Free(ref mt);
                }
            }

                m_bSampleSkipped = false;

                // Copy the sample media times
                long TimeStart, TimeEnd;
                if (pSource.GetMediaTime(out TimeStart, out TimeEnd) == NOERROR)
                {
                    pDest.SetMediaTime((DsLong)TimeStart, (DsLong)TimeEnd);
                }

                {
                    int lDataLength = pSource.GetActualDataLength();
                    pDest.SetActualDataLength(lDataLength);
                    // Copy the sample data
                    {
                        IntPtr pSourceBuffer, pDestBuffer;
                        int lSourceSize = pSource.GetSize();
                        int lDestSize = pDest.GetSize();

                        ASSERT(lDestSize >= lSourceSize && lDestSize >= lDataLength);

                        pSource.GetPointer(out pSourceBuffer);
                        pDest.GetPointer(out pDestBuffer);
                        ASSERT(lDestSize == 0 || pSourceBuffer != IntPtr.Zero && pDestBuffer != IntPtr.Zero);

                        API.CopyMemory(pDestBuffer, pSourceBuffer, lDataLength);
                    }
                }

            return hr;
        }

        #endregion
    }

    #endregion

    #region Base Rendered Pin

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class RenderedInputPin : BaseInputPin
    {
        #region Varibles

        protected bool m_bAtEndOfStream = false;
        protected bool m_bCompleteNotified = false;

        #endregion

        #region Constructor

        public RenderedInputPin(string _name, BaseFilter _filter)
            : base(_name, _filter)
        {

        }

        #endregion

        #region Overridden Methods

        public override int EndOfStream()
        {
            int hr = CheckStreaming();

            //  Do EC_COMPLETE handling for rendered pins
            if (S_OK == hr && !m_bAtEndOfStream)
            {
                m_bAtEndOfStream = true;
                FilterState fs;
                ASSERT(SUCCEEDED(m_Filter.GetState(0, out fs)));
                if (fs == FilterState.Running)
                {
                    DoCompleteHandling();
                }
            }
            return hr;
        }

        public override int EndFlush()
        {
            lock (m_Lock)
            {
                m_bAtEndOfStream = false;
                m_bCompleteNotified = false;
                return base.EndFlush();
            }
        }

        public override int Active()
        {
            m_bAtEndOfStream = false;
            m_bCompleteNotified = false;
            return base.Active();
        }

        public override int Run(long tStart)
        {
            m_bCompleteNotified = false;
            if (m_bAtEndOfStream)
            {
                DoCompleteHandling();
            }
            return S_OK;
        }

        #endregion

        #region Private Methods

        private void DoCompleteHandling()
        {
            ASSERT(m_bAtEndOfStream);
            if (!m_bCompleteNotified)
            {
                m_bCompleteNotified = true;

                m_Filter.NotifyEvent(EventCode.Complete, (IntPtr)((int)S_OK), IntPtr.Zero);
            }
        }

        #endregion
    }

    #endregion

    #region AMThread

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class AMThread : COMHelper, IDisposable
    {
        #region Delgates

        [ComVisible(false)]
        public delegate void ThreadCallback();

        #endregion

        #region Variables

        private ManualResetEvent m_EventSend = new ManualResetEvent(false);
        private AutoResetEvent m_EventComplete = new AutoResetEvent(false);

        private int m_dwParam = 0;
        private int m_dwReturnVal = 0;

        protected object m_AccessLock = new object();
        protected object m_WorkerLock = new object();

        protected string m_sName = "";

        #endregion

        #region Constructor

        public AMThread()
        {

        }

        public AMThread(ThreadCallback _callback)
        {
            OnThreadLoop += _callback;
        }

        ~AMThread()
        {
            Dispose();
        }

        #endregion

        #region Properties

        public bool ThreadExists
        {
            get { return IsExists(); }
        }

        public EventWaitHandle RequestEvent
        {
            get { return m_EventSend; }
        }

        public int RequestParam
        {
            get { return m_dwParam; }
        }

        public string Name
        {
            get { return m_sName; }
            set
            {
                if (value != null && value != m_sName && m_sName == "" && !IsExists())
                {
                    m_sName = value;
                }
            }
        }

        #endregion

        #region Abstract Methods

        protected abstract bool OnCreate();
        protected abstract bool OnClose(int _timeout);
        protected abstract bool IsExists();

        #endregion

        #region Methods

        public bool Join(int _timeout)
        {
            lock (m_AccessLock)
            {
                return OnClose(_timeout);
            }
        }

        public bool Create()
        {
            lock (m_AccessLock)
            {
                if (ThreadExists)
                {
                    return false;
                }
                bool bResult = OnCreate();
                Thread.Sleep(20);
                if (!bResult)
                {
                    Close();
                }
                return bResult;
            }
        }

        public bool Close()
        {
            return Join(Timeout.Infinite);
        }

        public int CallWorker(int dwParam)
        {
            lock (m_AccessLock)
            {
                if (!ThreadExists)
                {
                    return E_FAIL;
                }
                m_dwParam = dwParam;

                // signal the worker thread
                m_EventSend.Set();

                // wait for the completion to be signalled
                m_EventComplete.WaitOne();

                // done - this is the thread's return value
                return m_dwReturnVal;
            }
        }

        public bool CheckRequest(ref int pParam)
        {
            if (!m_EventSend.WaitOne(0,false))
            {
                return false;
            }
            else
            {
                pParam = m_dwParam;
                return true;
            }
        }

        public void Reply(int _param)
        {
            m_dwReturnVal = _param;

            m_EventSend.Reset();

            m_EventComplete.Set();
        }

        public int GetRequest()
        {
            m_EventSend.WaitOne();
            return m_dwParam;
        }

        #endregion

        #region Events

        public event ThreadCallback OnThreadLoop;

        #endregion

        #region Protected Methods

        protected void MainThreadProc()
        {
            if (OnThreadLoop != null) OnThreadLoop();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Close();
        }

        #endregion
    }

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public sealed class ManagedThread : AMThread
    {
        #region Variables

        private Thread m_WorkerThread = null;
        private ExecutionContext m_Context = null;

        #endregion

        #region Constructor

        public ManagedThread()
        {

        }

        public ManagedThread(ThreadCallback _callback)
            : base(_callback)
        {

        }

        #endregion

        #region Overridden Methods

        protected override bool OnCreate()
        {
            m_Context = ExecutionContext.Capture();
            m_WorkerThread = new Thread(new ThreadStart(MainThreadProc));
            if (!string.IsNullOrEmpty(m_sName))
            {
                m_WorkerThread.Name = m_sName;
            }
            m_WorkerThread.Start();
            if (!m_WorkerThread.IsAlive)
            {
                Close();
                return false;
            }
            return true;
        }

        protected override bool OnClose(int _timeout)
        {
            bool bResult = true;
            if (m_WorkerThread != null)
            {
                if (m_WorkerThread.IsAlive)
                {
                    bResult = m_WorkerThread.Join(_timeout);
                }
                if (bResult)
                {
                    m_WorkerThread = null;
                }
            }
            return bResult;
        }

        protected override bool IsExists()
        {
            return m_WorkerThread != null && m_WorkerThread.IsAlive;
        }

        #endregion

        #region Private Methods

        private void ContextRun(object _data)
        {
            MainThreadProc();
        }

        private void CurrentThreadProc()
        {
            ExecutionContext.Run(m_Context, ContextRun, null);
        }

        #endregion
    }

    #endregion

    #region Source Stream Pin

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class SourceStream : BaseOutputPin, IDisposable
    {
        #region Variables

        protected AMThread m_Thread = null;

        #endregion

        #region Constructor

        public SourceStream(string _name, BaseSourceFilter _source)
            : base(_name, _source)
        {
            m_Thread = new ManagedThread(this.ThreadCallbackProc);
        }

        ~SourceStream()
        {
            Dispose();
            m_Thread.Dispose();
            m_Thread = null;
        }

        #endregion

        #region Abstract Methods

        public abstract int FillBuffer(ref IMediaSampleImpl _sample);

        #endregion

        #region Overridden Methods

        public override int Active()
        {
            lock (m_Filter.FilterLock)
            {
                if (m_Filter.IsActive)
                {
                    return S_FALSE;
                }
                if (!IsConnected)
                {
                    return NOERROR;
                }
                int hr = base.Active();
                if (FAILED(hr))
                {
                    return hr;
                }
                ASSERT(!m_Thread.ThreadExists);
                if (!m_Thread.Create())
                {
                    return E_FAIL;
                }
                hr = Init();
                if (FAILED(hr))
                {
                    return hr;
                }
                return Pause();
            }
        }

        public override int Inactive()
        {
            lock (m_Filter.FilterLock)
            {
                if (!IsConnected)
                {
                    return NOERROR;
                }
                int hr = 0;
                if (m_Thread.ThreadExists)
                {
                    hr = Stop();

                    if (FAILED(hr))
                    {
                        return hr;
                    }
                    hr = Exit();
                    if (FAILED(hr))
                    {
                        return hr;
                    }
                    m_Thread.Close();
                }
                hr = base.Inactive();
                if (FAILED(hr))
                {
                    return hr;
                }
                return NOERROR;
            }
        }

        public override int CheckMediaType(AMMediaType pmt)
        {
            lock (m_Filter.FilterLock)
            {
                AMMediaType mt = null;
                AMMediaType.Init(ref mt);
                try
                {
                    GetMediaType(ref mt);
                    if (AMMediaType.AreEquals(mt, pmt))
                    {
                        return NOERROR;
                    }
                }
                finally
                {
                    AMMediaType.Free(ref mt);
                    mt = null;
                }
            }
            return E_FAIL;
        }

        public override int GetMediaType(int iPosition, ref AMMediaType pMediaType)
        {
            lock (m_Filter.FilterLock)
            {
                if (iPosition < 0)
                {
                    return E_INVALIDARG;
                }
                if (iPosition > 0)
                {
                    return VFW_S_NO_MORE_ITEMS;
                }
                return GetMediaType(ref pMediaType);
            }
        }

        public virtual int GetMediaType(ref AMMediaType pMediaType)
        {
            return E_UNEXPECTED;
        }

        #endregion

        #region Cmd Methods

        [ComVisible(false)]
        public enum Command : int { CMD_INIT, CMD_PAUSE, CMD_RUN, CMD_STOP, CMD_EXIT };
        public int Init() { return m_Thread.CallWorker((int)Command.CMD_INIT); }
        public int Exit() { return m_Thread.CallWorker((int)Command.CMD_EXIT); }
        public int Run() { return m_Thread.CallWorker((int)Command.CMD_RUN); }
        public int Pause() { return m_Thread.CallWorker((int)Command.CMD_PAUSE); }
        public int Stop() { return m_Thread.CallWorker((int)Command.CMD_STOP); }

        protected Command GetRequest() { return (Command)m_Thread.GetRequest(); }
        protected bool CheckRequest(ref Command pCom)
        {
            int iParam = (int)pCom;
            if (m_Thread.CheckRequest(ref iParam))
            {
                pCom = (Command)iParam;
                return true;
            }
            return false;
        }

        #endregion

        #region Virtual Methods

        protected virtual int DoBufferProcessingLoop()
        {
            Command com = 0;

            OnThreadStartPlay();

            do
            {
                while (!CheckRequest(ref com))
                {
                    IntPtr pSample;
                    
                    DsLong _start = new DsLong(0), _stop = new DsLong(0);

                    int hr = GetDeliveryBuffer(out pSample, _start, _stop, AMGBF.NoWait);

                    if (FAILED(hr))
                    {
                        Thread.Sleep(hr == VFW_E_TIMEOUT ? 5 : 1);
                        continue;	// go round again. Perhaps the error will go away
                        // or the allocator is decommited & we will be asked to
                        // exit soon.
                    }
                    IMediaSampleImpl _sample = new IMediaSampleImpl(pSample);
                    try
                    {
                        // Virtual function user will override.
                        hr = FillBuffer(ref _sample);

                        if (hr == S_OK)
                        {
                            hr = Deliver(ref _sample);
                            // downstream filter returns S_FALSE if it wants us to
                            // stop or an error if it's reporting an error.
                            if (hr != S_OK)
                            {
                                TRACE(String.Format("Deliver() returned {0}; stopping", hr));
                                return S_OK;
                            }

                        }
                        else if (hr == S_FALSE)
                        {
                            // derived class wants us to stop pushing data
                            DeliverEndOfStream();
                            return S_OK;
                        }
                        else
                        {
                            // derived class encountered an error
                            TRACE(String.Format("Deliver() returned {0}; stopping", hr));
                            DeliverEndOfStream();
                            m_Filter.NotifyEvent(EventCode.ErrorAbort, (IntPtr)hr, IntPtr.Zero);
                            return hr;
                        }
                    }
                    finally
                    {
                        Marshal.Release(pSample);
                        pSample = IntPtr.Zero;
                    }

                    // all paths release the sample
                }

                // For all commands sent to us there must be a Reply call!

                if (com == Command.CMD_RUN || com == Command.CMD_PAUSE)
                {
                    m_Thread.Reply(NOERROR);
                }
                else if (com != Command.CMD_STOP)
                {
                    m_Thread.Reply(E_UNEXPECTED);
                    TRACE("Unexpected command!!!");
                }
            } while (com != Command.CMD_STOP);

            return S_FALSE;
        }

        protected virtual int ThreadProc()
        {
            int hr;  // the return code from calls
            Command com;

            do
            {
                com = GetRequest();
                if (com != Command.CMD_INIT)
                {
                    m_Thread.Reply(E_UNEXPECTED);
                }
            } while (com != Command.CMD_INIT);

            //TRACE("CSourceStream worker thread initializing");

            hr = OnThreadCreate(); // perform set up tasks
            if (FAILED(hr))
            {

                OnThreadDestroy();
                m_Thread.Reply(hr);	// send failed return code from OnThreadCreate
                return 1;
            }
            m_Thread.Reply(NOERROR);

            Command cmd;
            do
            {
                cmd = GetRequest();

                switch (cmd)
                {

                    case Command.CMD_EXIT:
                        m_Thread.Reply(NOERROR);
                        break;

                    case Command.CMD_RUN:
                        //TRACE("CMD_RUN received before a CMD_PAUSE???");
                        m_Thread.Reply(NOERROR);
                        DoBufferProcessingLoop();
                        break;

                    case Command.CMD_PAUSE:
                        m_Thread.Reply(NOERROR);
                        DoBufferProcessingLoop();
                        break;

                    case Command.CMD_STOP:
                        m_Thread.Reply(NOERROR);
                        break;

                    default:
                        //TRACE(String.Format("Unknown command {0} received!", cmd));
                        m_Thread.Reply(E_NOTIMPL);
                        break;
                }
            } while (cmd != Command.CMD_EXIT);

            hr = OnThreadDestroy();	// tidy up.
            if (FAILED(hr))
            {
                //TRACE("CSourceStream::OnThreadDestroy failed. Exiting thread.");
                return 1;
            }

            TRACE("CSourceStream worker thread exiting");
            return 0;
        }

        protected virtual int OnThreadCreate()
        {
            return NOERROR;
        }

        protected virtual int OnThreadDestroy()
        {
            return NOERROR;
        }

        protected virtual int OnThreadStartPlay()
        {
            return NOERROR;
        }

        #endregion

        #region Private Methods

        private void ThreadCallbackProc()
        {
            ThreadProc();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            m_Thread.Close();
            m_Filter.RemovePin(this);
        }

        #endregion
    }

    #endregion

    #region Source Filter

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class BaseSourceFilter : BaseFilter
    {
        #region Constructor

        public BaseSourceFilter(string _name)
            : base(_name)
        {

        }

        #endregion
    }

    #endregion

    #region Output Queue

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class OutputQueue : COMHelper,IDisposable
    {
        #region Constants

        protected enum PacketType: int
        {
            Sample = 0,
            NewSegment = 1,
            EOS = 2,
        };

        #endregion

        #region Structures

        protected struct NewSegmentPacket
        {
            public long tStart;
            public long tStop;
            public double dRate;
        }

        protected class Packet
        {
            public PacketType Type;
            public IntPtr Data;

            public Packet(PacketType _type)
            {
                Type = _type;
                Data = IntPtr.Zero;
            }

            public Packet(PacketType _type,IntPtr _data)
                : this(_type)
            {
                Data = _data;
            }

            public Packet(long _start,long _stop,double _rate)
                : this(PacketType.NewSegment)
            {
                NewSegmentPacket _data = new NewSegmentPacket();
                _data.tStart = _start;
                _data.tStop = _stop;
                _data.dRate = _rate;

                Data = Marshal.AllocCoTaskMem(Marshal.SizeOf(_data));
                Marshal.StructureToPtr(_data,Data,true);
            }

            ~Packet()
            {
                if (Type == PacketType.NewSegment && Data != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(Data);
                    Data = IntPtr.Zero;
                }
            }
        }

        #endregion

        #region Variables

        protected Queue<Packet> m_Packets = new Queue<Packet>();
        protected IntPtr m_pOutputPin = IntPtr.Zero;
        protected EventWaitHandle m_hEventPop = null;
        protected AMThread m_Thread = null;
        protected object m_csPacketsLock = new object();
        protected object m_csPopLock = new object();
        protected object m_csLastResult = new object();
        protected ManualResetEvent m_Flushing = new ManualResetEvent(false);
        protected ManualResetEvent m_Flushed = new ManualResetEvent(false);
        protected ManualResetEvent m_evShutdown = new ManualResetEvent(false);
        protected ManualResetEvent m_evReady = new ManualResetEvent(false);
        protected HRESULT m_hr = S_OK;
        protected bool m_bReset = false;

        #endregion

        #region Constructor

        public OutputQueue(IntPtr pOutputPin)
        {
            m_pOutputPin = pOutputPin;
            m_Thread = new ManagedThread(this.ThreadProc);
            m_Thread.Create();
        }

        ~OutputQueue()
        {
            Dispose();
        }

        #endregion

        #region Properties

        public HRESULT LastResult
        {
            get 
            { 
                HRESULT hr;
                lock (m_csLastResult)
                {
                    hr = m_hr;
                }
                return hr; 
            }
        }

        public IPinImpl OutputPin
        {
            get { return new IPinImpl(m_pOutputPin); }
        }

        public EventWaitHandle PopEvent
        {
            get { return m_hEventPop; }
            set {
                lock (m_csPopLock)
                {
                    m_hEventPop = value;
                }
            }
        }

        #endregion 

        #region Public Methods

        public void BeginFlush()
        {
            bool bNotify = false;
            if (!m_Flushing.WaitOne(0,false))
            {
                m_Flushing.Set();
                bNotify = true;
                lock (m_csPacketsLock)
                {
                    m_evReady.Set();
                }
            }
            if (bNotify) OutputPin.BeginFlush();
        }

        public void EndFlush()
        {
            bool bNotify = false;
            if (m_Flushing.WaitOne(0,false))
            {
                if (0 == WaitHandle.WaitAny(new WaitHandle[] { m_Flushed, m_evShutdown }))
                {
                    m_Flushing.Reset();
                    m_Flushed.Reset();
                    bNotify = true;
                }
            }
            if (bNotify) OutputPin.EndFlush();
        }

        public void EOS()
        {
            lock (m_csPacketsLock)
            {
                m_Packets.Enqueue(new Packet(PacketType.EOS));
                m_evReady.Set();
            }
        }

        public void NewSegment(long tStart, long tStop, double dRate)
        {
            lock (m_csPacketsLock)
            {
                m_Packets.Enqueue(new Packet(tStart, tStop, dRate));
                m_evReady.Set();
            }
        }

        public int Receive(IntPtr pSample)
        {
            if (pSample == IntPtr.Zero) return E_POINTER;
            if (m_Flushing.WaitOne(0,false)) return S_FALSE;
            lock (m_csLastResult)
            {
                if (m_hr != S_OK) return m_hr;
            }
            lock (m_csPacketsLock)
            {
                Marshal.AddRef(pSample);
                m_Packets.Enqueue(new Packet(PacketType.Sample,pSample));
                m_evReady.Set();
            }
            return S_OK;
        }

        public int Receive(ref IMediaSampleImpl pSample)
        {
            return Receive(pSample.UnknownPtr);
        }

        public void Reset()
        {
            if (!m_Flushing.WaitOne(0,false))
            {
                lock (m_csPacketsLock)
                {
                    m_bReset = true;
                    m_evReady.Set();
                }
                m_Flushing.Set();
                WaitHandle.WaitAny(new WaitHandle[] { m_Flushed, m_evShutdown});
            }
            m_Flushed.Reset();
            m_Flushing.Reset();
            lock (m_csLastResult)
            {
                m_hr = S_OK;
            }
            lock (m_csPacketsLock)
            {
                m_bReset = false;
            }
        }

        public bool IsIdle()
        {
            return !m_evReady.WaitOne(0,false);
        }

        #endregion

        #region Protected Methods
 
        protected virtual void ThreadProc()
        {
            Guid _guid = typeof(IMemInputPin).GUID;
            IntPtr pMemInputPin;
            OutputPin._QueryInterface(ref _guid, out pMemInputPin);
            IMemInputPinImpl _pin = new IMemInputPinImpl(pMemInputPin);
            Packet _packet;
            HRESULT hr = NOERROR;
            while (true)
            {
                int nWait = WaitHandle.WaitAny(new WaitHandle[] { m_evReady, m_evShutdown });
                if (nWait != 0)
                {
                    lock (m_csPacketsLock)
                    {
                        while (m_Packets.Count > 0)
                        {
                            _packet = m_Packets.Dequeue();
                            if (_packet.Type == PacketType.Sample)
                            {
                                Marshal.Release(_packet.Data);
                            }
                        }
                    }
                    break;
                }
                bool bReset;
                bool bFlushing = m_Flushing.WaitOne(0,false);
                lock (m_csPacketsLock)
                {
                    bReset = m_bReset;
                    if (m_Packets.Count > 0)
                    {
                        _packet = m_Packets.Dequeue();
                    }
                    else
                    {
                        m_evReady.Reset();
                        if (bFlushing)
                        {
                            m_Flushed.Set();
                        }
                        continue;
                    }
                }
                lock (m_csPopLock)
                {
                    if (m_hEventPop != null)
                    {
                        m_hEventPop.Set();
                    }
                }
                if (_packet.Type == PacketType.EOS && !bReset)
                {
                    lock (m_csLastResult)
                    {
                        if (m_hr != S_OK) continue;
                    }
                    hr = (HRESULT)OutputPin.EndOfStream();
                    if (hr.Failed)
                    {
                        TRACE("EndOfStream() " + hr.ToString());
                    }
                }
                if (_packet.Type == PacketType.NewSegment && !bReset)
                {
                    NewSegmentPacket _segment = (NewSegmentPacket)Marshal.PtrToStructure(_packet.Data, typeof(NewSegmentPacket));
                    OutputPin.NewSegment(_segment.tStart, _segment.tStop, _segment.dRate);
                }
                if (_packet.Type == PacketType.Sample)
                {
                    if (!bFlushing && !bReset)
                    {
                        bool bProceed;
                        lock (m_csLastResult)
                        {
                            bProceed = (m_hr == S_OK);
                        }
                        if (bProceed)
                        {
                            hr = (HRESULT)_pin.Receive(_packet.Data);
                            if (hr.Failed)
                            {
                                lock (m_csLastResult)
                                {
                                    m_hr = hr;
                                }
                            }
                        }

                    }
                    Marshal.Release(_packet.Data);
                }
            }
            _pin._Release();
        }

        #endregion
    
        #region IDisposable Members

        public void  Dispose()
        {
 	        m_evShutdown.Set();
            m_Thread.Dispose();
        }

        #endregion
    }

    #endregion

    #region PropertyPages

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class BasePropertyPage : Form, IPropertyPage
    {
        #region Variables

        protected IPropertyPageSiteImpl m_pPageSite = null;
        protected bool m_bDirty = false;
        protected bool m_bObjectSet = false;

        #endregion

        #region Constructor

        public BasePropertyPage()
        {
            Visible = false;
        }

        public BasePropertyPage(string _title)
            : this()
        {
            this.Text = _title;
        }

        ~BasePropertyPage()
        {
            SetPageSite(IntPtr.Zero);
            SetObjects(0, IntPtr.Zero);
        }
        #endregion

        #region Properties

        public bool Dirty
        {
            get { return m_bDirty; }
            set {
                if (m_bDirty != value)
                {
                    m_bDirty = value;
                    if (m_pPageSite != null)
                    {
                        m_pPageSite.OnStatusChange(m_bDirty ? PropStatus.Dirty : PropStatus.Clean);
                    }
                }
            }
        }

        public string Title
        {
            get
            {
                return this.Text;
            }
            set
            {
                this.Text = value;
            }
        }

        #endregion

        #region Overridden Methods

        protected override CreateParams CreateParams
        {
            get
            {
                new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
                CreateParams _params = base.CreateParams;
                unchecked
                {
                    _params.Style &= (int)~0xF1CF0000;
                    _params.Style |= 0x40000000;
                }
                return _params;
            }
        }

        #endregion

        #region IPropertyPage Members

        public virtual int SetPageSite(IntPtr pPageSite)
        {
            if (pPageSite == IntPtr.Zero)
            {
                if (m_pPageSite != null)
                {
                    m_pPageSite._Release();
                }
                m_pPageSite = null;
            }
            else
            {
                m_pPageSite = new IPropertyPageSiteImpl(pPageSite);
                m_pPageSite._AddRef();
            }
            if (m_pPageSite != null && m_bDirty)
            {
                m_pPageSite.OnStatusChange(PropStatus.Dirty);
            }
            return HRESULT.NOERROR;
        }

        public virtual int Activate(IntPtr hWndParent, DsRect pRect, bool bModal)
        {
            if (pRect == null) return HRESULT.E_POINTER;
            if (m_bObjectSet == false)
            {
                return HRESULT.E_UNEXPECTED;
            }
            if (hWndParent == IntPtr.Zero)
            {
                return HRESULT.E_INVALIDARG;
            }
            this.CreateControl();
            SetParent(Handle, hWndParent);

            OnActivate();
            (this as IPropertyPage).Move(pRect);
            return (this as IPropertyPage).Show(SWOptions.Show);
        }

        public virtual new int Deactivate()
        {
            OnDeactivate();
            Hide();
            DestroyWindow(Handle);
            return HRESULT.NOERROR;
        }

        public virtual int GetPageInfo(PropPageInfo pPageInfo)
        {
            if (pPageInfo == null) return HRESULT.E_POINTER;
            pPageInfo.cb = Marshal.SizeOf(typeof(PropPageInfo));
            pPageInfo.pszTitle = Marshal.StringToCoTaskMemAuto(this.Title);
            pPageInfo.pszDocString = IntPtr.Zero;
            pPageInfo.pszHelpFile = IntPtr.Zero;
            pPageInfo.dwHelpContext = 0;
            pPageInfo.size.cx = this.Width;
            pPageInfo.size.cy = this.Height;

            return HRESULT.NOERROR;
        }

        public virtual int SetObjects(uint cObjects, IntPtr ppUnk)
        {
            if (cObjects == 1)
            {
                if (m_bObjectSet) return HRESULT.E_UNEXPECTED;

                if (ppUnk == IntPtr.Zero)
                {
                    return HRESULT.E_POINTER;
                }
                IntPtr pObject = Marshal.ReadIntPtr(ppUnk);
                if (pObject == IntPtr.Zero)
                {
                    return COMHelper.E_POINTER;
                }
                // Set a flag to say that we have set the Object
                m_bObjectSet = true;
                return OnConnect(pObject);

            }
            else if (cObjects == 0)
            {
                if (!m_bObjectSet) return HRESULT.NOERROR;
                // Set a flag to say that we have not set the Object for the page
                m_bObjectSet = false;
                return OnDisconnect();
            }

            return HRESULT.E_UNEXPECTED;
        }

        public virtual int Show(SWOptions nCmdShow)
        {
            Visible = !(nCmdShow == SWOptions.Hide);
            return HRESULT.NOERROR;
        }

        public virtual new int Move(DsRect pRect)
        {
            MoveWindow(Handle, pRect.left, pRect.top, pRect.right - pRect.left, pRect.bottom - pRect.top, true);
            return HRESULT.NOERROR;
        }

        public virtual int IsPageDirty()
        {
            return m_bDirty ? COMHelper.S_OK : COMHelper.S_FALSE;
        }

        public virtual int Apply()
        {
            if (m_bObjectSet == false)
            {
                return HRESULT.E_UNEXPECTED;
            }

            // Must have had a site set

            if (m_pPageSite == null)
            {
                return HRESULT.E_UNEXPECTED;
            }

            // Has anything changed

            if (m_bDirty == false)
            {
                return HRESULT.NOERROR;
            }

            // Commit derived class changes
            HRESULT hr = OnApplyChanges();
            if (hr.Succeeded)
            {
                Dirty = false;
            }
            return hr;
        }

        public virtual int Help(string sHelpDir)
        {
            return HRESULT.E_NOTIMPL;
        }

        public virtual int TranslateAccelerator(IntPtr pMsg)
        {
            return HRESULT.E_NOTIMPL;
        }

        #endregion

        #region Virtual Methods

        public virtual HRESULT OnConnect(IntPtr pUnknown)
        {
            return HRESULT.NOERROR;
        }

        public virtual HRESULT OnDisconnect()
        {
            return HRESULT.NOERROR;
        }

        public virtual HRESULT OnActivate()
        {
            return HRESULT.NOERROR;
        }

        public virtual HRESULT OnDeactivate()
        {
            return HRESULT.NOERROR;
        }

        public virtual HRESULT OnApplyChanges()
        {
            return HRESULT.NOERROR;
        }

        #endregion

        #region Static Functions

        public static bool ShowPropertyPages(IBaseFilter _filter, IntPtr _hwnd)
        {
            try
            {
                ISpecifyPropertyPages pProp = (ISpecifyPropertyPages)_filter;
                if (pProp != null)
                {
                    DsCAUUID pCAUUID;
                    if (COMHelper.SUCCEEDED(pProp.GetPages(out pCAUUID)))
                    {
                        object oDevice = (object)_filter;
                        int hr = OleCreatePropertyFrame(_hwnd, 0, 0, "Filter ", 1, ref oDevice, pCAUUID.cElems, pCAUUID.pElems, 0, 0, IntPtr.Zero);
                        if (hr < 0) Marshal.ThrowExceptionForHR(hr);
                        Marshal.FreeCoTaskMem(pCAUUID.pElems);
                        pProp = null;
                        return true;
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                GC.Collect();
            }
            return false;
        }

        #endregion

        #region API

        /// <summary>
        /// COM function helper for displaying properties dialog
        /// </summary>
        [DllImport("olepro32.dll")]
        private static extern int OleCreatePropertyFrame(
            IntPtr hwndOwner,
            int x,
            int y,
            [MarshalAs(UnmanagedType.LPWStr)] string lpszCaption,
            int cObjects,
            [MarshalAs(UnmanagedType.Interface, ArraySubType = UnmanagedType.IUnknown)] 
			ref object ppUnk,
            int cPages,
            IntPtr lpPageClsID,
            int lcid,
            int dwReserved,
            IntPtr lpvReserved
            );

        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyWindow(IntPtr hwnd);

        #endregion
    }

    #endregion

    #region PosPassThru

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class PosPassThru : COMHelper, IMediaSeeking, IMediaPosition
    {
        #region Variables

        private IntPtr m_pPin = IntPtr.Zero;
        private object m_csLock = new object();
        private IMediaSeekingImpl m_pSeeking = null;

        #endregion

        #region Constructor

        public PosPassThru(ref HRESULT hr, IntPtr pPin)
        {
            hr = S_OK;
            m_pPin = pPin;
            if (m_pPin == IntPtr.Zero)
            {
                hr = E_POINTER;
            }
        }

        #endregion

        #region Properties

        public HRESULT ForceRefresh
        {
            get { return S_OK; }
        }

        protected IMediaSeekingImpl Seeking
        {
            get 
            {
                lock (m_csLock)
                {
                    if (m_pSeeking == null)
                    {
                        IPinImpl _pin = new IPinImpl(m_pPin);
                        if (_pin.IsValid)
                        {
                            IntPtr _connected;
                            HRESULT hr = (HRESULT)_pin.ConnectedTo(out _connected);
                            if (SUCCEEDED(hr))
                            {
                                m_pSeeking = new IMediaSeekingImpl(_connected);
                                Marshal.Release(_connected);
                            }
                        }
                    }
                    return m_pSeeking;
                }
            }
        }

        #endregion

        #region Virtual Methods

        public virtual HRESULT GetMediaTime(out long pStartTime, out long pEndTime)
        {
            pStartTime = pEndTime = 0;
            return E_FAIL;
        }

        #endregion

        #region Methods

        public void ResetSeeking()
        {
            lock (m_csLock)
            {
                m_pSeeking = null;
            }
        }

        #endregion

        #region IMediaSeeking Members

        public int GetCapabilities(out AMSeekingSeekingCapabilities pCapabilities)
        {
            pCapabilities = 0;
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking == null || !_seeking.IsValid)
            {
                return E_NOTIMPL;
            }
            return _seeking.GetCapabilities(out pCapabilities);
        }

        public int CheckCapabilities(ref AMSeekingSeekingCapabilities pCapabilities)
        {
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking == null || !_seeking.IsValid)
            {
                return E_NOTIMPL;
            }
            return _seeking.CheckCapabilities(ref pCapabilities);
        }

        public int IsFormatSupported(Guid pFormat)
        {
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking == null || !_seeking.IsValid)
            {
                return E_NOTIMPL;
            }
            return _seeking.IsFormatSupported(pFormat);
        }

        public int QueryPreferredFormat(out Guid pFormat)
        {
            pFormat = Guid.Empty;
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking == null || !_seeking.IsValid)
            {
                return E_NOTIMPL;
            }
            return _seeking.QueryPreferredFormat(out pFormat);
        }

        public int GetTimeFormat(out Guid pFormat)
        {
            pFormat = Guid.Empty;
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking == null || !_seeking.IsValid)
            {
                return E_NOTIMPL;
            }
            return _seeking.GetTimeFormat(out pFormat);
        }

        public int IsUsingTimeFormat(Guid pFormat)
        {
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking == null || !_seeking.IsValid)
            {
                return E_NOTIMPL;
            }
            return _seeking.IsUsingTimeFormat(pFormat);
        }

        public int SetTimeFormat(Guid pFormat)
        {
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking == null || !_seeking.IsValid)
            {
                return E_NOTIMPL;
            }
            return _seeking.SetTimeFormat(pFormat);
        }

        public int GetDuration(out long pDuration)
        {
            pDuration = 0;
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking == null || !_seeking.IsValid)
            {
                return E_NOTIMPL;
            }
            return _seeking.GetDuration(out pDuration);
        }

        public int GetStopPosition(out long pStop)
        {
            pStop = 0;
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking == null || !_seeking.IsValid)
            {
                return E_NOTIMPL;
            }
            return _seeking.GetStopPosition(out pStop);
        }

        public int GetCurrentPosition(out long pCurrent)
        {
            pCurrent = 0;
            long lEnd;
            HRESULT hr = GetMediaTime(out pCurrent, out lEnd);
            if (SUCCEEDED(hr))
            {
                hr = NOERROR;
            }
            else
            {
                IMediaSeekingImpl _seeking = Seeking;
                if (_seeking == null || !_seeking.IsValid)
                {
                    return E_NOTIMPL;
                }
                hr = (HRESULT)_seeking.GetCurrentPosition(out pCurrent);
            }
            return hr;
        }

        public int ConvertTimeFormat(out long pTarget, DsGuid pTargetFormat, long Source, DsGuid pSourceFormat)
        {
            pTarget = 0;
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking == null || !_seeking.IsValid)
            {
                return E_NOTIMPL;
            }
            return _seeking.ConvertTimeFormat(out pTarget, pTargetFormat, Source, pSourceFormat);
        }

        public int SetPositions(DsLong pCurrent, AMSeekingSeekingFlags dwCurrentFlags, DsLong pStop, AMSeekingSeekingFlags dwStopFlags)
        {
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking == null || !_seeking.IsValid)
            {
                return E_NOTIMPL;
            }
            return _seeking.SetPositions(pCurrent, dwCurrentFlags, pStop, dwStopFlags);
        }

        public int GetPositions(out long pCurrent, out long pStop)
        {
            pCurrent = pStop = 0;
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking == null || !_seeking.IsValid)
            {
                return E_NOTIMPL;
            }
            return _seeking.GetPositions(out pCurrent, out pStop);
        }

        public int GetAvailable(out long pEarliest, out long pLatest)
        {
            pEarliest = pLatest = 0;
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking == null || !_seeking.IsValid)
            {
                return E_NOTIMPL;
            }
            return _seeking.GetAvailable(out pEarliest, out pLatest);
        }

        public int SetRate(double dRate)
        {
            if (0.0 == dRate)
            {
                return E_INVALIDARG;
            }
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking == null || !_seeking.IsValid)
            {
                return E_NOTIMPL;
            }
            return _seeking.SetRate(dRate);
        }

        public int GetRate(out double pdRate)
        {
            pdRate = 0;
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking == null || !_seeking.IsValid)
            {
                return E_NOTIMPL;
            }
            return _seeking.GetRate(out pdRate);
        }

        public int GetPreroll(out long pllPreroll)
        {
            pllPreroll = 0;
            IMediaSeekingImpl _seeking = Seeking;
            if (_seeking == null || !_seeking.IsValid)
            {
                return E_NOTIMPL;
            }
            return _seeking.GetPreroll(out pllPreroll);
        }

        #endregion

        #region IMediaPosition Members

        public int get_Duration(out double pLength)
        {
            pLength = 0;
            long lDuration;
            int hr = GetDuration(out lDuration);
            if (SUCCEEDED(hr))
            {
                pLength = (double)lDuration / (double)UNITS;
            }
            return hr;
        }

        public int put_CurrentPosition(double llTime)
        {
            long _position = (long)(llTime * UNITS);
            return SetPositions(_position, AMSeekingSeekingFlags.AbsolutePositioning, 0, AMSeekingSeekingFlags.NoPositioning);
        }

        public int get_CurrentPosition(out double pllTime)
        {
            pllTime = 0;
            long _position;
            int hr = GetCurrentPosition(out _position);
            if (SUCCEEDED(hr))
            {
                pllTime = ((double)_position / (double)UNITS);
            }
            return hr;
        }

        public int get_StopTime(out double pllTime)
        {
            pllTime = 0;
            long _position;
            int hr = GetStopPosition(out _position);
            if (SUCCEEDED(hr))
            {
                pllTime = ((double)_position / (double)UNITS);
            }
            return hr;
        }

        public int put_StopTime(double llTime)
        {
            long _position = (long)(llTime * UNITS);
            return SetPositions(0, AMSeekingSeekingFlags.NoPositioning, _position, AMSeekingSeekingFlags.AbsolutePositioning);
        }

        public int get_PrerollTime(out double pllTime)
        {
            pllTime = 0;
            long _position;
            int hr = GetPreroll(out _position);
            if (SUCCEEDED(hr))
            {
                pllTime = ((double)_position / (double)UNITS);
            }
            return hr;
        }

        public int put_PrerollTime(double llTime)
        {
            return E_NOTIMPL;
        }

        public int put_Rate(double dRate)
        {
            return SetRate(dRate);
        }

        public int get_Rate(out double pdRate)
        {
            return GetRate(out pdRate);
        }

        public int CanSeekForward(out OABool pCanSeekForward)
        {
            pCanSeekForward = OABool.False;
            AMSeekingSeekingCapabilities _caps = AMSeekingSeekingCapabilities.CanSeekForwards;
            int hr = CheckCapabilities(ref _caps);
            if (S_OK == hr)
            {
                pCanSeekForward = OABool.True;
            }
            if (S_FALSE == hr) hr = S_OK;
            return hr;
        }

        public int CanSeekBackward(out OABool pCanSeekBackward)
        {
            pCanSeekBackward = OABool.False;
            AMSeekingSeekingCapabilities _caps = AMSeekingSeekingCapabilities.CanSeekBackwards;
            int hr = CheckCapabilities(ref _caps);
            if (S_OK == hr)
            {
                pCanSeekBackward = OABool.True;
            }
            if (S_FALSE == hr) hr = S_OK;
            return hr;
        }

        #endregion
    }

    #endregion

    #region RendererPosPassThru

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class RendererPosPassThru : PosPassThru
    {
        #region Variables

        private object m_PositionLock = new object();
        private long m_StartMedia = 0;
        private long m_EndMedia = 0;
        private bool m_bReset = true;

        #endregion

        #region Constructor

        public RendererPosPassThru(ref HRESULT hr, IntPtr pPin)
            : base(ref hr, pPin)
        {

        }

        #endregion

        #region Methods

        public int RegisterMediaTime(ref IMediaSampleImpl _sample)
        {
            long StartMedia;
            long EndMedia;
            lock (m_PositionLock)
            {
                // Get the media times from the sample
                int hr = _sample.GetTime(out StartMedia, out EndMedia);
                if (FAILED(hr))
                {
                    ASSERT(hr == VFW_E_SAMPLE_TIME_NOT_SET);
                    return hr;
                }
                return RegisterMediaTime(StartMedia, EndMedia);
            }
        }

        public int RegisterMediaTime(long StartTime, long EndTime)
        {
            lock (m_PositionLock)
            {
                m_StartMedia = StartTime;
                m_EndMedia = EndTime;
                m_bReset = false;
                return NOERROR;
            }
        }

        public override HRESULT GetMediaTime(out long pStartTime, out long pEndTime)
        {
            lock (m_PositionLock)
            {
                pStartTime = 0;
                pEndTime = 0;
                if (m_bReset == true)
                {
                    return E_FAIL;
                }

                // We don't have to return the end time
                int hr = ConvertTimeFormat(out pStartTime, DsGuid.Empty, m_StartMedia, (DsGuid)(TimeFormat.MediaTime));
                if (SUCCEEDED(hr))
                {
                    hr = ConvertTimeFormat(out pEndTime, DsGuid.Empty, m_EndMedia, (DsGuid)(TimeFormat.MediaTime));
                }
                return (HRESULT)hr;
            }
        }

        public int ResetMediaTime()
        {
            lock (m_PositionLock)
            {
                m_StartMedia = 0;
                m_EndMedia = 0;
                m_bReset = true;
                ResetSeeking();
                return NOERROR;
            }
        }

        public int EOS()
        {
            int hr;

            if (m_bReset == true) hr = E_FAIL;
            else
            {
                long llStop;
                if (SUCCEEDED(hr = GetStopPosition(out llStop)))
                {
                    lock (m_PositionLock)
                    {
                        m_StartMedia =
                        m_EndMedia = llStop;
                    }
                }
            }
            return hr;
        }

        #endregion
    }

    #endregion

    #region Base Renderer Pin

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class RendererInputPin : BaseInputPin
    {
        #region Constructor

        public RendererInputPin(string _name, BaseRendererFilter _filter)
            : base(_name, _filter)
        {

        }

        #endregion

        #region Overridden Methods

        public override int Active()
        {
            return ((BaseRendererFilter)m_Filter).Active();
        }

        public override int Inactive()
        {
            m_bRunTimeError = false;

            return ((BaseRendererFilter)m_Filter).Inactive();
        }

        public override int BreakConnect()
        {
            int hr = ((BaseRendererFilter)m_Filter).BreakConnect();
            if (FAILED(hr))
            {
                return hr;
            }
            return base.BreakConnect();
        }

        public override int CompleteConnect(ref IPinImpl pReceivePin)
        {
            int hr = (m_Filter as BaseRendererFilter).CompleteConnect(ref pReceivePin);
            if (FAILED(hr))
            {
                return hr;
            }
            return base.CompleteConnect(ref pReceivePin);
        }

        public override int CheckMediaType(AMMediaType pmt)
        {
            return (m_Filter as BaseRendererFilter).CheckMediaType(pmt);
        }

        public override int SetMediaType(AMMediaType mt)
        {
            int hr = base.SetMediaType(mt);
            if (FAILED(hr))
            {
                return hr;
            }
            return (m_Filter as BaseRendererFilter).SetMediaType(mt);
        }

        public override int EndOfStream()
        {
            BaseRendererFilter _filter = (m_Filter as BaseRendererFilter);
            lock (_filter.FilterLock)
            {
                lock (_filter.RendererLock)
                {
                    int hr = CheckStreaming();
                    if (hr != NOERROR)
                    {
                        return hr;
                    }
                    hr = _filter.EndOfStream();
                    if (SUCCEEDED(hr))
                    {
                        hr = base.EndOfStream();
                    }
                    return hr;
                }
            }
        }

        public override int BeginFlush()
        {
            BaseRendererFilter _filter = (m_Filter as BaseRendererFilter);
            lock (_filter.FilterLock)
            {
                lock (_filter.RendererLock)
                {
                    base.BeginFlush();
                    _filter.BeginFlush();
                }
            }
            return _filter.ResetEndOfStream();
        }

        public override int EndFlush()
        {
            BaseRendererFilter _filter = (m_Filter as BaseRendererFilter);
            lock (_filter.FilterLock)
            {
                lock (_filter.RendererLock)
                {
                    int hr = _filter.EndFlush();
                    if (SUCCEEDED(hr))
                    {
                        hr = base.EndFlush();
                    }
                    return hr;
                }
            }
        }

        public override int OnReceive(ref IMediaSampleImpl pSample)
        {
            BaseRendererFilter _filter = (m_Filter as BaseRendererFilter);
            int hr = _filter.OnReceive(ref pSample);
            if (FAILED(hr))
            {
                lock (_filter.FilterLock)
                {
                    if (!IsStopped && !IsFlushing && !_filter.IsAbort && !m_bRunTimeError)
                    {
                        _filter.NotifyEvent(EventCode.ErrorAbort, (IntPtr)hr, IntPtr.Zero);
                        lock (_filter.RendererLock)
                        {
                            if (_filter.IsStreaming && !_filter.IsEndOfStreamDelivered)
                            {
                                _filter.NotifyEndOfStream();
                            }
                        }
                        m_bRunTimeError = true;
                    }
                }
            }
            return hr;
        }

        #endregion

        #region Helper Methods

        public int BaseReceive(ref IMediaSampleImpl pMediaSample)
        {
            return base.OnReceive(ref pMediaSample);
        }

        #endregion
    }

    #endregion

    #region MessageDispatcher

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class MessageDispatcher : IMessageFilter, IDisposable
    {
        #region Constants

        private const string s_Message = "AMUnblock";

        #endregion

        #region Variables

        protected int m_iMessage = 0;
        protected bool m_bDisposed = false;
        protected EventWaitHandle m_hAbort = null;

        #endregion

        #region Constructor

        public MessageDispatcher(EventWaitHandle hAbort)
            : this(hAbort, s_Message)
        {

        }

        public MessageDispatcher(EventWaitHandle hAbort, string _message)
            : this(hAbort, (int)RegisterWindowMessage(_message))
        {

        }

        public MessageDispatcher(EventWaitHandle hAbort, int _message)
        {
            m_bDisposed = (hAbort == null);
            if (!m_bDisposed)
            {
                m_iMessage = _message;
                m_hAbort = hAbort;
                Application.RegisterMessageLoop(this.MessageLoopCallback);
                Application.AddMessageFilter(this);
            }
        }

        ~MessageDispatcher()
        {
            Dispose();
        }

        #endregion

        #region Methods

        public bool MessageLoopCallback()
        {
            return !m_bDisposed;
        }

        #endregion

        #region IMessageFilter Members

        public bool PreFilterMessage(ref Message m)
        {
            if (!m_bDisposed)
            {
                if (m.Msg == m_iMessage)
                {
                    m_hAbort.Set();
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (!m_bDisposed)
            {
                m_bDisposed = true;
                Application.RemoveMessageFilter(this);
            }
        }

        #endregion

        #region API

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "RegisterWindowMessageW")]
        private static extern uint RegisterWindowMessage([In, MarshalAs(UnmanagedType.LPWStr)] string lpString);

        #endregion
    }

    #endregion

    #region Base Renderer Filter

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class BaseRendererFilter : BaseFilter, IMediaSeeking
    {
        #region Constants

        private const int TIMEOUT_DELIVERYWAIT = 50;
        private const int TIMEOUT_RESOLUTION = 10;
        private const int RENDER_TIMEOUT = 10000;

        #endregion

        #region Variables

        private MessageDispatcher m_MessageDispatcher = null;

        protected object m_RendererLock = new object();
        protected bool m_bAbort = false;
        protected bool m_bStreaming = false;
        protected bool m_bEOS = false;
        protected bool m_bEOSDelivered = false;

        protected AutoResetEvent m_RenderEvent = new AutoResetEvent(false);
        protected ManualResetEvent m_ThreadSignal = new ManualResetEvent(false);
        protected ManualResetEvent m_evComplete = new ManualResetEvent(false);
        protected ManualResetEvent m_evAbort = new ManualResetEvent(false);
        protected ManualResetEvent m_evClear = new ManualResetEvent(true);

        protected int m_dwAdvise = 0;
        protected RendererPosPassThru m_pPosition = null;

        protected IMediaSampleImpl m_pMediaSample = null;
        protected bool m_bRepaintStatus = true;
        protected long m_SignalTime = 0;

        protected System.Threading.Timer m_EndOfStreamTimer = null;

        #endregion

        #region Constructor

        public BaseRendererFilter(string _name)
            : base(_name)
        {
            Ready();
        }

        ~BaseRendererFilter()
        {
            ASSERT(m_bStreaming == false);
            ASSERT(m_EndOfStreamTimer == null);
            StopStreaming();
            ClearPendingSample();
            if (m_EndOfStreamTimer != null)
            {
                m_EndOfStreamTimer.Dispose();
                m_EndOfStreamTimer = null;
            }
            m_pPosition = null;
        }

        #endregion

        #region Properties

        public bool IsAbort
        {
            get { return m_bAbort; }
            set { m_bAbort = value; }
        }

        public object RendererLock
        {
            get { return m_RendererLock; }
        }

        public bool IsStreaming
        {
            get { return m_bStreaming; }
        }

        public bool IsEndOfStreamDelivered
        {
            get { return m_bEOSDelivered; }
        }

        public bool IsEndOfStream
        {
            get { return m_bEOS; }
        }

        public RendererInputPin InputPin
        {
            get
            {
                if (Pins.Count >= 1)
                {
                    return (RendererInputPin)Pins[0];
                }
                return null;
            }
        }

        public EventWaitHandle RenderEvent
        {
            get { return m_RenderEvent; }
        }

        public bool IsReady
        {
            get { return m_evComplete.WaitOne(0,false); }
        }

        public FilterState RealState
        {
            get { return m_State; }
        }

        protected bool RepaintStatus
        {
            get { return m_bRepaintStatus; }
            set
            {
                lock (m_RendererLock)
                {
                    if (m_bRepaintStatus != value)
                    {
                        m_bRepaintStatus = value;
                        OnRepaintStatusChanged(value);
                    }
                }
            }
        }

        protected RendererPosPassThru Position
        {
            get
            {
                if (m_pPosition == null)
                {
                    lock (m_Lock)
                    {
                        HRESULT hr = NOERROR;
                        IntPtr _pin = Marshal.GetComInterfaceForObject(InputPin,typeof(IPin));
                        m_pPosition = new RendererPosPassThru(ref hr, _pin);
                        Marshal.Release(_pin);
                        if (FAILED(hr))
                        {
                            m_pPosition = null;
                        }
                    }
                }
                return m_pPosition;
            }
        }

        protected virtual bool HaveCurrentSample
        {
            get
            {
                lock (m_RendererLock)
                {
                    return (m_pMediaSample == null ? false : true);
                }
            }
        }

        #endregion

        #region Abstract Methods

        public abstract int DoRenderSample(ref IMediaSampleImpl pMediaSample);
        public abstract int CheckMediaType(AMMediaType pmt);

        #endregion

        #region Virtual Methods

        public virtual int Active()
        {
            return NOERROR;
        }

        public virtual int Inactive()
        {
            if (Position != null)
            {
                Position.ResetMediaTime();
            }
            //  People who derive from this may want to override this behaviour
            //  to keep hold of the sample in some circumstances
            ClearPendingSample();

            return NOERROR;
        }

        public virtual int BeginFlush()
        {
            if (m_State == FilterState.Paused)
            {
                NotReady();
            }

            SourceThreadCanWait(false);
            CancelNotification();
            ClearPendingSample();
            //  Wait for Receive to complete
            WaitForReceiveToComplete();

            return NOERROR;
        }

        public virtual int EndFlush()
        {
            if (Position != null)
            {
                Position.ResetMediaTime();
            }
            // There should be no outstanding advise

            int hr = CancelNotification();
            ASSERT(hr == S_FALSE);
            SourceThreadCanWait(true);
            return NOERROR;
        }

        public virtual int EndOfStream()
        {
            if (m_State == FilterState.Stopped)
            {
                return NOERROR;
            }

            // If we have a sample then wait for it to be rendered

            m_bEOS = true;
            if (m_pMediaSample != null)
            {
                return NOERROR;
            }

            // If we are waiting for pause then we are now ready since we cannot now
            // carry on waiting for a sample to arrive since we are being told there
            // won't be any. This sets an event that the GetState function picks up

            Ready();

            // Only signal completion now if we are running otherwise queue it until
            // we do run in StartStreaming. This is used when we seek because a seek
            // causes a pause where early notification of completion is misleading

            if (m_bStreaming)
            {
                SendEndOfStream();
            }
            return NOERROR;
        }

        public virtual int ResetEndOfStream()
        {
            ResetEndOfStreamTimer();
            lock (m_RendererLock)
            {
                m_bEOS = false;
                m_bEOSDelivered = false;
                m_SignalTime = 0;

                return NOERROR;
            }
        }

        public virtual int NotifyEndOfStream()
        {
            lock (m_RendererLock)
            {
                ASSERT(m_bEOSDelivered == false);
                ASSERT(m_EndOfStreamTimer == null);

                // Has the filter changed state

                if (m_bStreaming == false)
                {
                    ASSERT(m_EndOfStreamTimer == null);
                    return NOERROR;
                }

                // Reset the end of stream timer
                if (m_EndOfStreamTimer != null)
                {
                    m_EndOfStreamTimer.Dispose();
                    m_EndOfStreamTimer = null;
                }

                if (Position != null)
                {
                    Position.EOS();
                }
                m_bEOSDelivered = true;
                return NotifyEvent(EventCode.Complete, (IntPtr)((int)S_OK), Marshal.GetIUnknownForObject(this));
            }
        }

        public virtual int SendEndOfStream()
        {
            if (m_bEOS == false || m_bEOSDelivered || m_EndOfStreamTimer != null)
            {
                return NOERROR;
            }

            // If there is no clock then signal immediately
            if (m_pClock == IntPtr.Zero)
            {
                return NotifyEndOfStream();
            }

            // How long into the future is the delivery time

            long Signal = m_tStart + m_SignalTime;
            long CurrentTime;
            Clock.GetTime(out CurrentTime);
            int Delay = (int)((Signal - CurrentTime) / 10000);

            // Wait for the delivery time to arrive

            if (Delay < TIMEOUT_DELIVERYWAIT)
            {
                return NotifyEndOfStream();
            }

            // Signal a timer callback on another worker thread
            m_EndOfStreamTimer = new System.Threading.Timer(new TimerCallback(TimerCallbackProc));
            if (!m_EndOfStreamTimer.Change(Delay, Timeout.Infinite))
            {
                return NotifyEndOfStream();
            }
            return NOERROR;
        }

        public virtual int BreakConnect()
        {
            // Check we have a valid connection

            if (InputPin.IsConnected == false)
            {
                return S_FALSE;
            }

            // Check we are stopped before disconnecting
            if (m_State != FilterState.Stopped && !InputPin.CanReconnectWhenActive)
            {
                return VFW_E_NOT_STOPPED;
            }

            RepaintStatus = false;
            ResetEndOfStream();
            ClearPendingSample();
            m_bAbort = false;
            if (FilterState.Running == m_State)
            {
                StopStreaming();
            }
            m_pPosition = null;
            return NOERROR;
        }

        public virtual int CompleteConnect(ref IPinImpl pReceivePin)
        {
            m_bAbort = false;

            if (FilterState.Running == RealState)
            {
                int hr = StartStreaming();
                if (FAILED(hr))
                {
                    return hr;
                }

                RepaintStatus = false;
            }
            else
            {
                RepaintStatus = true;
            }
            return NOERROR;
        }

        public virtual int SetMediaType(AMMediaType mt)
        {
            return NOERROR;
        }

        public virtual int OnReceive(ref IMediaSampleImpl pSample)
        {
            ASSERT(pSample != null);

            // It may return VFW_E_SAMPLE_REJECTED code to say don't bother

            int hr = PrepareReceive(ref pSample);
            if (hr != S_OK)
            {
                if (hr == VFW_E_SAMPLE_REJECTED)
                {
                    return NOERROR;
                }
                return hr;
            }

            // We realize the palette in "PrepareRender()" so we have to give away the
            // filter lock here.
            if (m_State == FilterState.Paused)
            {
                PrepareRender();
                // no need to use InterlockedExchange
                m_evClear.Set();
                {
                    // We must hold both these locks
                    lock (m_Lock)
                    {
                        if (m_State == FilterState.Stopped)
                            return NOERROR;

                        m_evClear.Reset();
                        lock (m_RendererLock)
                        {
                            OnReceiveFirstSample(ref pSample);
                        }
                    }
                }
                Ready();
            }
            // Having set an advise link with the clock we sit and wait. We may be
            // awoken by the clock firing or by a state change. The rendering call
            // will lock the critical section and check we can still render the data

            hr = WaitForRenderTime();
            if (FAILED(hr))
            {
                m_evClear.Set();
                return NOERROR;
            }

            PrepareRender();

            //  Set this here and poll it until we work out the locking correctly
            //  It can't be right that the streaming stuff grabs the interface
            //  lock - after all we want to be able to wait for this stuff
            //  to complete
            m_evClear.Set();

            // We must hold both these locks
            lock (m_Lock)
            {

                // since we gave away the filter wide lock, the sate of the filter could
                // have chnaged to Stopped
                if (m_State == FilterState.Stopped)
                    return NOERROR;

                lock (m_RendererLock)
                {
                    // Deal with this sample

                    Render(ref m_pMediaSample);
                    ClearPendingSample();
                    SendEndOfStream();
                    CancelNotification();
                    return NOERROR;
                }
            }
        }

        public virtual int PrepareReceive(ref IMediaSampleImpl pMediaSample)
        {
            lock (m_Lock)
            {
                m_evClear.Reset();

                // Check our flushing and filter state

                // This function must hold the interface lock because it calls 
                // CBaseInputPin::Receive() and CBaseInputPin::Receive() uses
                // CBasePin::m_bRunTimeError.
                int hr = InputPin.BaseReceive(ref pMediaSample);

                if (hr != NOERROR)
                {
                    m_evClear.Set();
                    return E_FAIL;
                }

                // Has the type changed on a media sample. We do all rendering
                // synchronously on the source thread, which has a side effect
                // that only one buffer is ever outstanding. Therefore when we
                // have Receive called we can go ahead and change the format
                // Since the format change can cause a SendMessage we just don't
                // lock
                if (InputPin.SampleProps.pMediaType != IntPtr.Zero)
                {
                    AMMediaType mt = (AMMediaType)Marshal.PtrToStructure(InputPin.SampleProps.pMediaType, typeof(AMMediaType));
                    hr = InputPin.SetMediaType(mt);
                    if (FAILED(hr))
                    {
                        m_evClear.Set();
                        return hr;
                    }
                }

                lock (m_RendererLock)
                {

                    ASSERT(IsActive == true);
                    ASSERT(InputPin.IsFlushing == false);
                    ASSERT(InputPin.IsConnected == true);
                    ASSERT(m_pMediaSample == null);

                    // Return an error if we already have a sample waiting for rendering
                    // source pins must serialise the Receive calls - we also check that
                    // no data is being sent after the source signalled an end of stream

                    if (m_pMediaSample != null || m_bEOS || m_bAbort)
                    {
                        Ready();
                        m_evClear.Set();
                        return E_UNEXPECTED;
                    }

                    // Store the media times from this sample
                    if (Position != null)
                    {
                        Position.RegisterMediaTime(ref pMediaSample);
                    }
                    // Schedule the next sample if we are streaming

                    if ((m_bStreaming == true) && (ScheduleSample(ref pMediaSample) == false))
                    {
                        ASSERT(m_RenderEvent.WaitOne(0,false) == false);
                        ASSERT(CancelNotification() == S_FALSE);
                        m_evClear.Set();
                        return VFW_E_SAMPLE_REJECTED;
                    }

                    // Store the sample end time for EC_COMPLETE handling
                    m_SignalTime = InputPin.SampleProps.tStop;

                    // BEWARE we sometimes keep the sample even after returning the thread to
                    // the source filter such as when we go into a stopped state (we keep it
                    // to refresh the device with) so we must AddRef it to keep it safely. If
                    // we start flushing the source thread is released and any sample waiting
                    // will be released otherwise GetBuffer may never return (see BeginFlush)

                    m_pMediaSample = pMediaSample;
                    m_pMediaSample._AddRef();
                    if (m_bStreaming == false)
                    {
                        RepaintStatus = true;
                    }
                    return NOERROR;
                }
            }
        }

        public virtual IMediaSampleImpl GetCurrentSample()
        {
            lock (m_RendererLock)
            {
                if (m_pMediaSample != null)
                {
                    m_pMediaSample._AddRef();
                }
                return m_pMediaSample;
            }
        }

        public virtual int Render(ref IMediaSampleImpl pMediaSample)
        {
            if (pMediaSample == null || m_bStreaming == false)
            {
                return S_FALSE;
            }

            OnRenderStart(ref pMediaSample);
            int hr = DoRenderSample(ref pMediaSample);
            OnRenderEnd(ref pMediaSample);

            return hr;
        }

        public virtual int SourceThreadCanWait(bool bCanWait)
        {
            if (bCanWait == true)
            {
                m_ThreadSignal.Reset();
            }
            else
            {
                m_ThreadSignal.Set();
            }
            return NOERROR;
        }

        public virtual int WaitForRenderTime()
        {
            WaitHandle[] WaitObjects = new WaitHandle[] { m_ThreadSignal, m_RenderEvent };
            int Result = WaitHandle.WaitTimeout;

            // Wait for either the time to arrive or for us to be stopped

            OnWaitStart();
            while (Result == WaitHandle.WaitTimeout)
            {
                Result = WaitHandle.WaitAny(WaitObjects, RENDER_TIMEOUT,false);
            }
            OnWaitEnd();

            // We may have been awoken without the timer firing
            if (Result == 0)
            {
                return VFW_E_STATE_CHANGED;
            }
            SignalTimerFired();
            return NOERROR;
        }

        public virtual int CompleteStateChange(FilterState OldState)
        {
            if (InputPin.IsConnected == false)
            {
                Ready();
                return S_OK;
            }

            // Have we run off the end of stream

            if (IsEndOfStream == true)
            {
                Ready();
                return S_OK;
            }

            // Make sure we get fresh data after being stopped

            if (HaveCurrentSample)
            {
                if (OldState != FilterState.Stopped)
                {
                    Ready();
                    return S_OK;
                }
            }
            NotReady();
            return S_FALSE;
        }

        public virtual bool ScheduleSample(ref IMediaSampleImpl pMediaSample)
        {
            long StartSample, EndSample;

            // Is someone pulling our leg

            if (pMediaSample == null)
            {
                return false;
            }

            // Get the next sample due up for rendering.  If there aren't any ready
            // then GetNextSampleTimes returns an error.  If there is one to be done
            // then it succeeds and yields the sample times. If it is due now then
            // it returns S_OK other if it's to be done when due it returns S_FALSE

            int hr = GetSampleTimes(ref pMediaSample, out StartSample, out EndSample);
            if (FAILED(hr))
            {
                return false;
            }

            // If we don't have a reference clock then we cannot set up the advise
            // time so we simply set the event indicating an image to render. This
            // will cause us to run flat out without any timing or synchronisation

            if (hr == S_OK)
            {
                m_RenderEvent.Set();
                return true;
            }

            ASSERT(m_dwAdvise == 0);
            ASSERT(m_pClock != IntPtr.Zero);
            ASSERT(m_RenderEvent.WaitOne(0,false) == false);


            // We do have a valid reference clock interface so we can ask it to
            // set an event when the image comes due for rendering. We pass in
            // the reference time we were told to start at and also the current
            // stream time which is the offset from the start reference time

            long lTemp;
            Clock.GetTime(out lTemp);
            hr = Clock.AdviseTime(
                    m_tStart,                                           // Start run time
                    StartSample,                                        // Stream time
                    m_RenderEvent.SafeWaitHandle.DangerousGetHandle(),  // Render notification
                    out m_dwAdvise);                                    // Advise cookie

            if (SUCCEEDED(hr))
            {
                return true;
            }

            // We could not schedule the next sample for rendering despite the fact
            // we have a valid sample here. This is a fair indication that either
            // the system clock is wrong or the time stamp for the sample is duff

            ASSERT(m_dwAdvise == 0);
            return false;
        }

        public virtual int GetSampleTimes(ref IMediaSampleImpl pMediaSample,
                                       out long pStartTime,
                                       out long pEndTime)
        {
            ASSERT(m_dwAdvise == 0);
            ASSERT(pMediaSample != null);

            // If the stop time for this sample is before or the same as start time,
            // then just ignore it (release it) and schedule the next one in line
            // Source filters should always fill in the start and end times properly!

            if (SUCCEEDED(pMediaSample.GetTime(out pStartTime, out pEndTime)))
            {
                if (pEndTime < pStartTime)
                {
                    return VFW_E_START_TIME_AFTER_END;
                }
            }
            else
            {
                // no time set in the sample... draw it now?
                return S_OK;
            }

            // Can't synchronise without a clock so we return S_OK which tells the
            // caller that the sample should be rendered immediately without going
            // through the overhead of setting a timer advise link with the clock

            if (m_pClock == IntPtr.Zero)
            {
                return S_OK;
            }
            return ShouldDrawSampleNow(ref pMediaSample, ref pStartTime, ref pEndTime);
        }

        public virtual void SignalTimerFired()
        {
            m_dwAdvise = 0;
        }

        public virtual int CancelNotification()
        {
            int dwAdvise = m_dwAdvise;

            // Have we a live advise link

            if (m_dwAdvise != 0)
            {
                Clock.Unadvise(m_dwAdvise);
                SignalTimerFired();
            }

            // Clear the event and return our status

            m_RenderEvent.Reset();
            return (dwAdvise != 0 ? S_OK : S_FALSE);
        }

        public virtual int ClearPendingSample()
        {
            lock (m_RendererLock)
            {
                if (m_pMediaSample != null)
                {
                    m_pMediaSample._Release();
                    m_pMediaSample = null;
                }
                return NOERROR;
            }
        }

        public virtual int StartStreaming()
        {
            lock (m_RendererLock)
            {
                if (m_bStreaming == true)
                {
                    return NOERROR;
                }

                // Reset the streaming times ready for running

                m_bStreaming = true;
                OnStartStreaming();

                // There should be no outstanding advise
                ASSERT(m_RenderEvent.WaitOne(0,false) == false);
                ASSERT(CancelNotification() == S_FALSE);

                // If we have an EOS and no data then deliver it now

                if (m_pMediaSample == null)
                {
                    return SendEndOfStream();
                }

                // Have the data rendered

                ASSERT(m_pMediaSample != null);
                if (!ScheduleSample(ref m_pMediaSample))
                {
                    m_RenderEvent.Set();
                }
                return NOERROR;
            }
        }

        public virtual int StopStreaming()
        {
            lock (m_RendererLock)
            {
                m_bEOSDelivered = false;

                if (m_bStreaming == true)
                {
                    m_bStreaming = false;
                    OnStopStreaming();
                }
                return NOERROR;
            }
        }

        #endregion

        #region Other Methods To Override

        public virtual void OnReceiveFirstSample(ref IMediaSampleImpl pMediaSample)
        {
        }

        public virtual void OnRenderStart(ref IMediaSampleImpl pMediaSample)
        {

        }

        public virtual void OnRenderEnd(ref IMediaSampleImpl pMediaSample)
        {

        }

        public virtual int OnStartStreaming()
        {
            return NOERROR;
        }

        public virtual int OnStopStreaming()
        {
            return NOERROR;
        }

        public virtual void OnWaitStart()
        {
        }

        public virtual void OnWaitEnd()
        {
        }

        public virtual void OnRepaintStatusChanged(bool bRepaint)
        {

        }

        public virtual void PrepareRender()
        {
        }

        public virtual int ShouldDrawSampleNow(ref IMediaSampleImpl pMediaSample,
                                                    ref long ptrStart,
                                                    ref long ptrEnd)
        {
            return S_FALSE;
        }

        #endregion

        #region Overridden Methods

        protected override int OnInitializePins()
        {
            AddPin(new RendererInputPin("In", this));
            return NOERROR;
        }

        public override int Stop()
        {
            lock (m_Lock)
            {
                // Make sure there really is a state change

                if (m_State == FilterState.Stopped)
                {
                    return NOERROR;
                }

                // Is our input pin connected

                if (InputPin.IsConnected == false)
                {
                    TRACE("Input pin is not connected");
                    m_State = FilterState.Stopped;
                    return NOERROR;
                }

                base.Stop();

                // If we are going into a stopped state then we must decommit whatever
                // allocator we are using it so that any source filter waiting in the
                // GetBuffer can be released and unlock themselves for a state change

                if (InputPin.Allocator != null)
                {
                    InputPin.Allocator.Decommit();
                }

                // Cancel any scheduled rendering

                RepaintStatus = true;
                StopStreaming();
                SourceThreadCanWait(false);
                ResetEndOfStream();
                CancelNotification();

                // There should be no outstanding clock advise
                ASSERT(CancelNotification() == S_FALSE);
                ASSERT(m_RenderEvent.WaitOne(0,false) == false);
                ASSERT(m_EndOfStreamTimer == null);

                Ready();
                WaitForReceiveToComplete();
                m_bAbort = false;

                return NOERROR;
            }
        }

        public override int Pause()
        {
            lock (m_Lock)
            {
                FilterState OldState = m_State;
                ASSERT(InputPin.IsFlushing == false);

                if (m_State == FilterState.Stopped)
                {
                    m_evAbort.Reset();
                }
                // Make sure there really is a state change

                if (m_State == FilterState.Paused)
                {
                    return CompleteStateChange(FilterState.Paused);
                }

                // Has our input pin been connected

                if (InputPin.IsConnected == false)
                {
                    m_State = FilterState.Paused;
                    return CompleteStateChange(FilterState.Paused);
                }

                // Pause the base filter class

                int hr = base.Pause();
                if (FAILED(hr))
                {
                    TRACE("Pause failed");
                    return hr;
                }

                // Enable EC_REPAINT events again

                RepaintStatus = true;
                StopStreaming();
                SourceThreadCanWait(true);
                CancelNotification();
                ResetEndOfStreamTimer();

                // If we are going into a paused state then we must commit whatever
                // allocator we are using it so that any source filter can call the
                // GetBuffer and expect to get a buffer without returning an error

                if (InputPin.Allocator != null)
                {
                    InputPin.Allocator.Commit();
                }

                // There should be no outstanding advise
                ASSERT(CancelNotification() == S_FALSE);
                ASSERT(m_RenderEvent.WaitOne(0,false) == false);
                ASSERT(m_EndOfStreamTimer == null);
                ASSERT(InputPin.IsFlushing == false);

                // When we come out of a stopped state we must clear any image we were
                // holding onto for frame refreshing. Since renderers see state changes
                // first we can reset ourselves ready to accept the source thread data
                // Paused or running after being stopped causes the current position to
                // be reset so we're not interested in passing end of stream signals

                if (OldState == FilterState.Stopped)
                {
                    m_bAbort = false;
                    ClearPendingSample();
                }
                return CompleteStateChange(OldState);
            }
        }

        public override int Run(long tStart)
        {
            lock (m_Lock)
            {
                FilterState OldState = m_State;

                // Make sure there really is a state change

                if (m_State == FilterState.Running)
                {
                    return NOERROR;
                }

                // Send EC_COMPLETE if we're not connected

                if (InputPin.IsConnected == false)
                {
                    NotifyEvent(EventCode.Complete, (IntPtr)((int)S_OK), Marshal.GetIUnknownForObject(this));
                    m_State = FilterState.Running;
                    return NOERROR;
                }

                Ready();

                // Pause the base filter class

                int hr = base.Run(tStart);
                if (FAILED(hr))
                {
                    TRACE("Run failed");
                    return hr;
                }

                // Allow the source thread to wait
                ASSERT(InputPin.IsFlushing == false);
                SourceThreadCanWait(true);
                RepaintStatus = false;

                // There should be no outstanding advise
                ASSERT(CancelNotification() == S_FALSE);
                ASSERT(m_RenderEvent.WaitOne(0,false) == false);
                ASSERT(m_EndOfStreamTimer == null);
                ASSERT(InputPin.IsFlushing == false);

                // If we are going into a running state then we must commit whatever
                // allocator we are using it so that any source filter can call the
                // GetBuffer and expect to get a buffer without returning an error

                if (InputPin.Allocator != null)
                {
                    InputPin.Allocator.Commit();
                }

                // When we come out of a stopped state we must clear any image we were
                // holding onto for frame refreshing. Since renderers see state changes
                // first we can reset ourselves ready to accept the source thread data
                // Paused or running after being stopped causes the current position to
                // be reset so we're not interested in passing end of stream signals

                if (OldState == FilterState.Stopped)
                {
                    m_bAbort = false;
                    ClearPendingSample();
                }
                return StartStreaming();
            }
        }

        public override int GetState(int dwMilliSecsTimeout, out FilterState filtState)
        {
            if (!WaitDispatchingMessages(m_evComplete, dwMilliSecsTimeout, IntPtr.Zero, 0, m_evAbort))
            {
                filtState = m_State;
                return VFW_S_STATE_INTERMEDIATE;
            }
            filtState = m_State;
            return NOERROR;
        }

        public override int JoinFilterGraph(IntPtr pGraph, string pName)
        {
            int hr = base.JoinFilterGraph(pGraph, pName);
            if (FAILED(hr)) return hr;
            if (pGraph != IntPtr.Zero)
            {
                m_evAbort.Reset();
                m_MessageDispatcher = new MessageDispatcher(m_evAbort);
            }
            else
            {
                if (m_MessageDispatcher != null)
                {
                    m_MessageDispatcher.Dispose();
                    m_MessageDispatcher = null;
                }
                if (!m_evComplete.WaitOne(0,false))
                {
                    lock (m_Lock)
                    {
                        m_evAbort.Set();
                        SourceThreadCanWait(false);
                        CancelNotification();
                    }
                }
            }
            return hr;
        }

        #endregion

        #region Helper Methods

        private bool WaitDispatchingMessages(EventWaitHandle hObject, int dwWait, IntPtr hwnd, int uMsg, EventWaitHandle hEvent)
        {
            int nCount = null != hEvent ? 2 : 1;

            WaitHandle[] hWaitObjects = new WaitHandle[nCount];
            hWaitObjects[0] = hObject;
            if (null != hEvent)
            {
                hWaitObjects[1] = hEvent;
            }

            return WaitHandle.WaitAny(hWaitObjects,dwWait,false) != WaitHandle.WaitTimeout;
        }

        public void Ready()
        {
            m_evComplete.Set();
        }

        public void NotReady()
        {
            m_evComplete.Reset();
        }

        public void SendRepaint()
        {
            lock (m_RendererLock)
            {
                // We should not send repaint notifications when...
                //    - An end of stream has been notified
                //    - Our input pin is being flushed
                //    - The input pin is not connected
                //    - We have aborted a video playback
                //    - There is a repaint already sent

                if (m_bAbort == false)
                {
                    if (InputPin.IsConnected == true)
                    {
                        if (InputPin.IsFlushing == false)
                        {
                            if (IsEndOfStream == false)
                            {
                                if (m_bRepaintStatus == true)
                                {
                                    NotifyEvent(EventCode.Repaint, Marshal.GetIUnknownForObject(InputPin), IntPtr.Zero);

                                    RepaintStatus = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void SendNotifyWindow(IPin pPin, IntPtr hwnd)
        {
            IMediaEventSink pSink = (IMediaEventSink)pPin;
            if (pSink != null)
            {
                pSink.Notify(EventCode.NotifyWindow, hwnd, IntPtr.Zero);
                pSink = null;
            }
            NotifyEvent(EventCode.NotifyWindow, hwnd, IntPtr.Zero);
        }

        public bool OnDisplayChange()
        {
            lock (m_RendererLock)
            {
                if (InputPin.IsConnected == false)
                {
                    return false;
                }

                // Pass our input pin as parameter on the event

                NotifyEvent(EventCode.DisplayChanged, Marshal.GetIUnknownForObject(InputPin), IntPtr.Zero);
                IsAbort = true;
                ClearPendingSample();

                return true;
            }
        }

        protected void TimerCallbackProc(object state)
        {
            lock (m_RendererLock)
            {
                // See if we should signal end of stream now
                if (m_EndOfStreamTimer != null)
                {
                    m_EndOfStreamTimer.Dispose();
                    m_EndOfStreamTimer = null;
                    SendEndOfStream();
                }
            }
        }

        public void ResetEndOfStreamTimer()
        {
            if (m_EndOfStreamTimer != null)
            {
                m_EndOfStreamTimer.Dispose();
                m_EndOfStreamTimer = null;
            }
        }

        public void WaitForReceiveToComplete()
        {
            WaitHandle.WaitAny(new WaitHandle[] { m_evClear, m_evAbort });
            lock (m_RendererLock)
            {
                if (m_evAbort.WaitOne(0,false))
                {
                    CancelNotification();
                    ClearPendingSample();
                }
            }
        }

        #endregion

        #region IMediaSeeking Members

        public int GetCapabilities(out AMSeekingSeekingCapabilities pCapabilities)
        {
            if (Position != null)
            {
                return Position.GetCapabilities(out pCapabilities);
            }
            pCapabilities = 0;
            return E_NOINTERFACE;
        }

        public int CheckCapabilities(ref AMSeekingSeekingCapabilities pCapabilities)
        {
            if (Position != null)
            {
                return Position.CheckCapabilities(ref pCapabilities);
            }
            return E_NOINTERFACE;
        }

        public int IsFormatSupported(Guid pFormat)
        {
            if (Position != null)
            {
                return Position.IsFormatSupported(pFormat);
            }
            return E_NOINTERFACE;
        }

        public int QueryPreferredFormat(out Guid pFormat)
        {
            if (Position != null)
            {
                return Position.QueryPreferredFormat(out pFormat);
            }
            pFormat = Guid.Empty;
            return E_NOINTERFACE;
        }

        public int GetTimeFormat(out Guid pFormat)
        {
            if (Position != null)
            {
                return Position.GetTimeFormat(out pFormat);
            }
            pFormat = Guid.Empty;
            return E_NOINTERFACE;
        }

        public int IsUsingTimeFormat(Guid pFormat)
        {
            if (Position != null)
            {
                return Position.IsUsingTimeFormat(pFormat);
            }
            return E_NOINTERFACE;
        }

        public int SetTimeFormat(Guid pFormat)
        {
            if (Position != null)
            {
                return Position.SetTimeFormat(pFormat);
            }
            return E_NOINTERFACE;
        }

        public int GetDuration(out long pDuration)
        {
            if (Position != null)
            {
                return Position.GetDuration(out pDuration);
            }
            pDuration = 0;
            return E_NOINTERFACE;
        }

        public int GetStopPosition(out long pStop)
        {
            if (Position != null)
            {
                return Position.GetStopPosition(out pStop);
            }
            pStop = 0;
            return E_NOINTERFACE;
        }

        public int GetCurrentPosition(out long pCurrent)
        {
            if (Position != null)
            {
                return Position.GetCurrentPosition(out pCurrent);
            }
            pCurrent = 0;
            return E_NOINTERFACE;
        }

        public int ConvertTimeFormat(out long pTarget, DsGuid pTargetFormat, long Source, DsGuid pSourceFormat)
        {
            if (Position != null)
            {
                return Position.ConvertTimeFormat(out pTarget, pTargetFormat, Source, pSourceFormat);
            }
            pTarget = 0;
            return E_NOINTERFACE;
        }

        public int SetPositions(DsLong pCurrent, AMSeekingSeekingFlags dwCurrentFlags, DsLong pStop, AMSeekingSeekingFlags dwStopFlags)
        {
            if (Position != null)
            {
                return Position.SetPositions(pCurrent, dwCurrentFlags, pStop, dwStopFlags);
            }
            return E_NOINTERFACE;
        }

        public int GetPositions(out long pCurrent, out long pStop)
        {
            if (Position != null)
            {
                return Position.GetPositions(out pCurrent, out pStop);
            }
            pCurrent = pStop = 0;
            return E_NOINTERFACE;
        }

        public int GetAvailable(out long pEarliest, out long pLatest)
        {
            if (Position != null)
            {
                return Position.GetAvailable(out pEarliest, out pLatest);
            }
            pEarliest = pLatest = 0;
            return E_NOINTERFACE;
        }

        public int SetRate(double dRate)
        {
            if (Position != null)
            {
                return Position.SetRate(dRate);
            }
            return E_NOINTERFACE;
        }

        public int GetRate(out double pdRate)
        {
            if (Position != null)
            {
                return Position.GetRate(out pdRate);
            }
            pdRate = 0;
            return E_NOINTERFACE;
        }

        public int GetPreroll(out long pllPreroll)
        {
            if (Position != null)
            {
                return Position.GetPreroll(out pllPreroll);
            }
            pllPreroll = 0;
            return E_NOINTERFACE;
        }

        #endregion
    }

    #endregion

    #region Async Stream Reader

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class AsyncStream : Stream
    {
        #region Variables

        protected IAsyncReaderImpl m_Reader = null;
        protected long m_lPosition = 0;
        protected long m_lLenth = 0;

        #endregion

        #region Constructor

        public AsyncStream(IntPtr pAsyncReader)
            : this(new IAsyncReaderImpl(pAsyncReader))
        {
        }

        public AsyncStream(IAsyncReaderImpl _reader)
        {
            m_Reader = _reader;
            if (m_Reader != null)
            {
                m_Reader._AddRef();
            }
        }

        #endregion

        #region Overridden Methods

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        protected override void Dispose(bool disposing)
        {
            if (m_Reader != null)
            {
                m_Reader._Release();
                m_Reader = null;
            }
            base.Dispose(disposing);
        }

        public override void Flush()
        {
        }

        public override long Length
        {
            get
            {
                if (m_lLenth == 0)
                {
                    long lTotal;
                    long lAvailable;
                    if (HRESULT.S_OK == m_Reader.Length(out lTotal, out lAvailable))
                    {
                        m_lLenth = lTotal;
                    }
                }
                return m_lLenth;
            }
        }

        public override long Position
        {
            get
            {
                return m_lPosition;
            }
            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int _size = count - offset;
            if (_size <= 0) return 0;
            if (m_lPosition < 0 || m_lPosition >= Length) return 0;
            if (m_Reader != null)
            {
                if (_size > Length - m_lPosition)
                {
                    _size = (int)(Length - m_lPosition);
                }
                IntPtr _ptr = IntPtr.Zero;
                try
                {
                    _ptr = Marshal.AllocCoTaskMem(count - offset);
                    if (HRESULT.S_OK == m_Reader.SyncRead(m_lPosition, _size, _ptr))
                    {
                        Marshal.Copy(_ptr, buffer, offset, _size);
                        m_lPosition += _size;
                    }
                    else
                    {
                        _size = 0;
                    }
                    return _size;
                }
                finally
                {
                    if (_ptr != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(_ptr);
                    }
                }
            }
            return 0;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long _length = Length;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    m_lPosition = offset;
                    break;
                case SeekOrigin.Current:
                    m_lPosition += offset;
                    break;
                case SeekOrigin.End:
                    m_lPosition = _length + offset;
                    break;
            }
            if (m_lPosition < 0)
            {
                m_lPosition = 0;
            }
            else if (m_lPosition >= _length)
            {
                m_lPosition = _length;
            }
            return m_lPosition;
        }

        public override void SetLength(long value)
        {
        }

        public override void Write(byte[] buffer, int offset, int count)
        {

        }

        #endregion
    }

    #endregion

    #region COM Stream Implementation

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class COMStream : Stream
    {
        #region Variables

        protected IStreamImpl m_Reader = null;

        #endregion

        #region Constructor

        public COMStream(IntPtr pStream)
            : this(new IStreamImpl(pStream))
        {
        }

        public COMStream(IStreamImpl _reader)
        {
            m_Reader = _reader;
            if (m_Reader != null)
            {
                m_Reader._AddRef();
            }
        }

        #endregion

        #region Overridden Methods

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        protected override void Dispose(bool disposing)
        {
            if (m_Reader != null)
            {
                m_Reader._Release();
                m_Reader = null;
            }
            base.Dispose(disposing);
        }

        public override void Flush()
        {
        }

        public override long Length
        {
            get
            {
                System.Runtime.InteropServices.ComTypes.STATSTG _stat;
                m_Reader.Stat(out _stat, 1);
                return _stat.cbSize;
            }
        }

        public override long Position
        {
            get
            {
                return Seek(0, SeekOrigin.Current);
            }
            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int _size = count - offset;
            if (_size <= 0) return 0;
            if (m_Reader != null)
            {
                IntPtr _ptr = IntPtr.Zero;
                try
                {
                    _ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)));
                    byte[] _result = buffer;
                    if (offset != 0)
                    {
                        _result = new byte[_size];
                    }
                    m_Reader.Read(_result, _size, _ptr);
                    int _readed = Marshal.ReadInt32(_ptr);
                    if (_readed > 0 && offset != 0)
                    {
                        Array.Copy(_result, 0, buffer, offset, _readed);
                    }
                    return _readed;
                }
                finally
                {
                    if (_ptr != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(_ptr);
                    }
                }
            }
            return 0;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            IntPtr _ptr = IntPtr.Zero;
            long _position = 0;
            try
            {
                _ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(long)));
                m_Reader.Seek(offset, (int)origin, _ptr);
                _position = Marshal.ReadInt64(_ptr);
            }
            finally
            {
                if (_ptr != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(_ptr);
                }
            }
            return _position;
        }

        public override void SetLength(long value)
        {
            m_Reader.SetSize(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            int _size = count - offset;
            if (_size > 0 && m_Reader != null)
            {
                byte[] _write = buffer;
                if (offset != 0)
                {
                    _write = new byte[_size];
                    Array.Copy(buffer, offset, _write, 0, _size);
                }
                m_Reader.Write(_write, _size, IntPtr.Zero);
            }
        }

        #endregion
    }

    #endregion

    #region Bit Stream Reader

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class BitStreamReader : COMHelper, IDisposable, IAsyncReader, IStream
    {
        #region Variables

        protected Stream m_Stream = null;
        protected const int m_nCacheSize = 64 * 1024;
        protected object m_csCacheLock = new object();
        protected object m_csReadingLock = new object();
        protected int m_nBufferSize = 0;
        protected int m_nBitIndex = 0;
        protected int m_nCacheIndex = 0;
        protected byte[] m_pCacheBuffer = null;
        protected byte[] m_btCurrentByte = new byte[1];
        protected long m_llPosition = 0;

        #endregion

        #region Constructor

        public BitStreamReader(Stream _stream)
        {
            m_Stream = _stream;
        }

        ~BitStreamReader()
        {
            Dispose();
        }

        #endregion

        #region Properties

        public long Position
        {
            get
            {
                lock (m_csCacheLock)
                {
                    return (m_llPosition - (m_nBufferSize > 0 ? (m_nBufferSize - m_nCacheIndex) : 0));
                }
            }
            set { Seek(value); }
        }

        public long TotalSize
        {
            get { return m_Stream.Length; }
        }

        public long AvailableSize
        {
            get
            {
                long _position = Position;
                long _size = TotalSize;
                return (_size > _position ? _size - _position : 0);
            }
        }

        public bool CanRead
        {
            get { return IsAvailable(0); }
        }

        #endregion

        #region Methods

        public bool IsAvailable(long _size)
        {
            if (_size > 0)
            {
                return (AvailableSize >= _size);
            }
            return (AvailableSize > 0);
        }

        public HRESULT Seek(long ullPosition)
        {
            lock (m_csCacheLock)
            {
                if (ullPosition < m_llPosition && m_llPosition - (long)m_nBufferSize < ullPosition)
                {
                    m_nCacheIndex = m_nBufferSize - (int)(m_llPosition - ullPosition);
                }
                else
                {
                    m_llPosition = ullPosition;
                    m_nBufferSize = 0;
                    m_nCacheIndex = 0;
                }
                m_nBitIndex = 0;
                if (ullPosition > TotalSize) return S_FALSE;
            }
            return NOERROR;
        }

        public HRESULT ReadData(byte[] pBuffer)
        {
            return ReadData(pBuffer, pBuffer.Length);
        }

        public HRESULT ReadData(byte[] pBuffer, int dwSize)
        {
            int dwReaded;
            HRESULT hr = ReadData(pBuffer, dwSize, out dwReaded);
            if (FAILED(hr)) return hr;
            if (dwReaded != dwSize) return E_FAIL;
            return hr;
        }

        public HRESULT ReadData(byte[] pBuffer, int dwSize, out int pdwReaded)
        {
            lock (m_csCacheLock)
            {
                if (m_pCacheBuffer == null)
                {
                    m_pCacheBuffer = new byte[m_nCacheSize];
                    m_nBufferSize = 0;
                }
                int nOutputIndex = 0;
                pdwReaded = 0;
                while (dwSize > 0)
                {
                    m_nBitIndex = 0;
                    if (m_nBufferSize == 0)
                    {
                        m_nCacheIndex = 0;
                        int _readed = 0;
                        {
                            HRESULT hr = ReadData(m_llPosition, m_pCacheBuffer, m_nCacheSize, out _readed);
                            if (!FAILED(hr) && _readed > 0)
                            {
                                m_nBufferSize = _readed;
                                m_llPosition += _readed;
                            }
                            else
                            {
                                return S_FALSE;
                            }
                        }
                    }
                    int nBytesToWrite = (m_nBufferSize - m_nCacheIndex <= (int)dwSize) ? (m_nBufferSize - m_nCacheIndex) : (int)dwSize;
                    Array.Copy(m_pCacheBuffer, m_nCacheIndex, pBuffer, nOutputIndex, nBytesToWrite);
                    nOutputIndex += nBytesToWrite;
                    dwSize -= nBytesToWrite;
                    pdwReaded += nBytesToWrite;
                    m_nCacheIndex += nBytesToWrite;

                    if (m_nCacheIndex >= m_nBufferSize)
                    {
                        m_nCacheIndex = 0;
                        m_nBufferSize = 0;
                    }
                }
            }
            return NOERROR;
        }

        public HRESULT ReadData(long ullPosition, byte[] pBuffer, int dwSize, out int pdwReaded)
        {
            lock (m_csReadingLock)
            {
                m_Stream.Position = ullPosition;
                pdwReaded = m_Stream.Read(pBuffer, 0, dwSize);
            }
            if (pdwReaded <= 0) return E_FAIL;
            if (pdwReaded < dwSize) return S_FALSE;
            return NOERROR;
        }

        #endregion

        #region Bits Helper

        public bool IsAligned() { return m_nBitIndex == 0; }
        public void AlignNextByte() { if (m_nBitIndex != 0) SkipBits(m_nBitIndex); }
        public int ReadBit()
        {
            if (m_nBitIndex == 0)
            {
                int _readed = 0;
                HRESULT hr = ReadData(m_btCurrentByte, 1, out _readed);
                if (_readed == 0) m_btCurrentByte[0] = 0;
                m_nBitIndex = 8;
            }
            m_nBitIndex--;
            return (m_btCurrentByte[0] >> m_nBitIndex) & 0x01;
        }
        public int ReadByte() { return (int)ReadBits(8); }
        public int ReadWord() { return (int)ReadBits(16); }
        public int ReadDword() { return (int)ReadBits(32); }
        public long ReadQword() { return ReadBits(64); }
        public long ReadBits(int nCount)
        {
            long _result = 0;
            long _bit;
            while (nCount-- > 0)
            {
                _result <<= 1;
                _bit = ReadBit();
                _result |= _bit;
            }
            return _result;
        }
        public long PeekBits(int nCount)
        {
            long llPosition = Position;
            int nBitIndex = m_nBitIndex;
            long _result = ReadBits(nCount);
            Seek(llPosition);
            while (nBitIndex != m_nBitIndex) SkipBit();
            return _result;
        }
        public void SkipBit() { SkipBits(1); }
        public void SkipBits(int nCount) { while (nCount-- > 0) ReadBit(); }
        public void SkipBytes(int nCount) { SkipBits(nCount * 8); }
        public uint ReadUE()
        {
            int cZeros = 0;
            while (ReadBit() == 0)
            {
                cZeros++;
            }
            return (uint)(ReadBits(cZeros) + ((1 << cZeros) - 1));
        }
        public int ReadSE()
        {
            uint UE = ReadUE();
            bool bPositive = (UE & 1) != 0;
            int SE = (int)((UE + 1) >> 1);
            if (!bPositive)
            {
                SE = -SE;
            }
            return SE;
        }

        #endregion

        #region Values Helper

        public object ReadValue(Type _type)
        {
            return ReadValue(_type, Marshal.SizeOf(_type));
        }

        public object ReadValue(Type _type, int nSize)
        {
            IntPtr _ptr = ReadValueAlloc(nSize);
            if (_ptr != IntPtr.Zero)
            {
                try
                {
                    return Marshal.PtrToStructure(_ptr, _type);
                }
                finally
                {
                    Marshal.FreeCoTaskMem(_ptr);
                }
            }
            return null;
        }

        public object ReadValue<T>()
        {
            return ReadValue(typeof(T));
        }

        public object ReadValue<T>(int nSize)
        {
            return ReadValue(typeof(T), nSize);
        }

        public IntPtr ReadValueAlloc(int _size)
        {
            if (_size <= 0) return IntPtr.Zero;
            IntPtr _ptr = Marshal.AllocCoTaskMem(_size);
            byte[] _data = new byte[_size];
            if (IsAligned())
            {
                int dwReaded = 0;
                HRESULT hr = ReadData(_data, _size, out dwReaded);
                if (!(hr == S_OK && dwReaded == _size))
                {
                    Marshal.FreeCoTaskMem(_ptr);
                    _ptr = IntPtr.Zero;
                }
            }
            else
            {
                if (IsAvailable(_size))
                {
                    for (int i = 0; i < _size; i++) _data[i] = (byte)ReadByte();
                }
                else
                {
                    Marshal.FreeCoTaskMem(_ptr);
                    _ptr = IntPtr.Zero;
                }
            }
            if (_ptr != IntPtr.Zero)
            {
                Marshal.Copy(_data, 0, _ptr, _size);
            }
            return _ptr;
        }

        public IntPtr ReadValueAlloc(Type _type)
        {
            return ReadValueAlloc(Marshal.SizeOf(_type));
        }

        public IntPtr ReadValueAlloc<T>()
        {
            return ReadValueAlloc(typeof(T));
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (m_Stream != null)
            {
                m_Stream.Dispose();
                m_Stream = null;
            }
        }

        #endregion

        #region IAsyncReader Members

        public int RequestAllocator(IntPtr pPreferred, AllocatorProperties pProps, out IntPtr ppActual)
        {
            ppActual = IntPtr.Zero;
            return E_NOTIMPL;
        }

        public int Request(IntPtr pSample, IntPtr dwUser)
        {
            return E_NOTIMPL;
        }

        public int WaitForNext(int dwTimeout, out IntPtr ppSample, out IntPtr pdwUser)
        {
            ppSample = IntPtr.Zero;
            pdwUser = IntPtr.Zero;
            return E_NOTIMPL;
        }

        public int SyncReadAligned(IntPtr pSample)
        {
            return E_NOTIMPL;
        }

        public int SyncRead(long llPosition, int lLength, IntPtr pBuffer)
        {
            byte[] _data = new byte[lLength];
            int _readed;
            HRESULT hr = ReadData(llPosition, _data, lLength, out _readed);
            if (_readed > 0 && SUCCEEDED(hr))
            {
                Marshal.Copy(_data, 0, pBuffer, _readed);
            }
            return _readed == lLength ? NOERROR : S_FALSE;
        }

        public int Length(out long pTotal, out long pAvailable)
        {
            pTotal = m_Stream.Length;
            pAvailable = pTotal;
            return NOERROR;
        }

        public int BeginFlush()
        {
            return E_NOTIMPL;
        }

        public int EndFlush()
        {
            return E_NOTIMPL;
        }

        #endregion

        #region IStream Members

        public void Clone(out IStream ppstm)
        {
            throw new NotImplementedException();
        }

        public void Commit(int grfCommitFlags)
        {

        }

        public void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
        {
            throw new NotImplementedException();
        }

        public void LockRegion(long libOffset, long cb, int dwLockType)
        {
            throw new NotImplementedException();
        }

        public void Read(byte[] pv, int cb, IntPtr pcbRead)
        {
            int _readed;
            ReadData(pv, cb, out _readed);
            if (pcbRead != IntPtr.Zero)
            {
                Marshal.WriteInt32(pcbRead, _readed);
            }
        }

        public void Revert()
        {
            throw new NotImplementedException();
        }

        public void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
        {
            long _position = Position;
            long _length = TotalSize;
            switch ((SeekOrigin)dwOrigin)
            {
                case SeekOrigin.Begin:
                    _position = dlibMove;
                    break;
                case SeekOrigin.Current:
                    _position += dlibMove;
                    break;
                case SeekOrigin.End:
                    _position = _length + dlibMove;
                    break;
            }
            if (_position < 0)
            {
                _position = 0;
            }
            else if (_position >= _length)
            {
                _position = _length;
            }
            Seek(_position);
            if (plibNewPosition != IntPtr.Zero)
            {
                Marshal.WriteInt64(plibNewPosition, _position);
            }
        }

        public void SetSize(long libNewSize)
        {
            throw new NotImplementedException();
        }

        public void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag)
        {
            pstatstg = new System.Runtime.InteropServices.ComTypes.STATSTG();
            pstatstg.cbSize = TotalSize;
            pstatstg.clsid = Guid.Empty;
            pstatstg.type = 2;
        }

        public void UnlockRegion(long libOffset, long cb, int dwLockType)
        {
            throw new NotImplementedException();
        }

        public void Write(byte[] pv, int cb, IntPtr pcbWritten)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    #endregion

    #region Base Splitter Filter

    #region Implementation

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class PacketData: IDisposable
    {
        #region Variables

        public long Start = -1;
        public long Stop = -1;
        public int Size = 0;
        public bool SyncPoint = false;
        public byte[] Buffer = null;
        public long Position = -1;

        #endregion

        #region Constructor

        public PacketData()
        {
        }

        ~PacketData()
        {
            Dispose();
        }

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
        }

        #endregion
    }

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class PacketsQueue: IDisposable
    {
        #region Variales

        protected ManualResetEvent m_evNotFull = new ManualResetEvent(true);
        protected List<PacketData> m_Packets = new List<PacketData>();
        protected object m_csLock = new object();
        protected long m_rtQueueStop = -1;
        protected long m_rtCacheDuration = -1;
        protected long m_rtQueueStart = -1;
        protected int m_AddPosition = -1;
        protected bool m_bSortable = false;

        #endregion

        #region Constructor

        public PacketsQueue()
        {

        }

        ~PacketsQueue()
        {
            Clear();
        }

        #endregion

        #region Properties

        public long Duration
        {
            get 
            {
                if (m_rtQueueStop != -1 && m_rtQueueStart != -1)
                {
                    long _duration = m_rtQueueStop - m_rtQueueStart;
                    return _duration > 0 ? _duration : 0;
                }
                return 0;
            }
        }

        public long CacheDuration
        {
            get { return m_rtCacheDuration; }
            set 
            {
                m_rtCacheDuration = value;
                if (m_rtCacheDuration > 0)
                {
                    if (Duration >= m_rtCacheDuration)
                    {
                        m_evNotFull.Reset();
                    }
                    else
                    {
                        m_evNotFull.Set();
                    }
                }
                else
                {
                    m_evNotFull.Set();
                }
            }
        }

        public long StartTime
        {
            get 
            {
                if (m_rtQueueStart == -1)
                {
                    return StopTime;
                }
                return m_rtQueueStart;
            }
        }

        public long StopTime
        {
            get 
            {
                return m_rtQueueStop != -1 ? m_rtQueueStop : 0;
            }
        }

        public bool Sorted
        {
            get { return m_bSortable; }
            set { m_bSortable = value; }
        }

        public bool IsFull
        {
            get { return !m_evNotFull.WaitOne(0,false); }
        }

        public bool IsEmpty
        {
            get { return Count == 0; }
        }

        public int Count
        {
            get 
            {
                lock (m_csLock)
                {
                    return m_Packets.Count;
                }
            }
        }

        #endregion

        #region Methods

        public void Clear()
        {
            lock (m_csLock)
            {
                while (m_Packets.Count > 0)
                {
                    m_Packets[0].Dispose();
                    m_Packets.RemoveAt(0);
                }
                m_rtQueueStop = -1;
                m_rtQueueStart = -1;
                m_AddPosition = -1;
                m_evNotFull.Set();
            }
        }

        public void Add(PacketData pPacket)
        {
            if (pPacket != null)
            {
                lock (m_csLock)
                {
                    if (pPacket.Start < 0 || !m_bSortable)
                    {
                        if (pPacket.Start >= 0 && m_rtQueueStop < pPacket.Start)
                        {
                            m_rtQueueStop = (pPacket.Stop > 0 && pPacket.Stop > pPacket.Start) ? pPacket.Stop : pPacket.Start;
                            if (m_Packets.Count == 0)
                            {
                                m_rtQueueStart = pPacket.Start;
                            }
                        }
                        if (m_AddPosition == -1)
                        {
                            m_Packets.Add(pPacket);
                        }
                        else
                        {
                            m_Packets.Insert(m_AddPosition++, pPacket);
                        }
                    }
                    else
                    {
                        if (m_rtQueueStop <= pPacket.Start)
                        {
                            if (m_Packets.Count == 0)
                            {
                                m_rtQueueStart = pPacket.Start;
                            }
                            m_Packets.Add(pPacket);
                            m_AddPosition = m_Packets.Count - 1;
                            m_rtQueueStop = (pPacket.Stop > 0 && pPacket.Stop > pPacket.Start) ? pPacket.Stop : pPacket.Start;
                        }
                        else
                        {
                            if (m_Packets.Count == 0)
                            {
                                m_Packets.Add(pPacket);
                                m_AddPosition = 0;
                                m_rtQueueStart = pPacket.Start;
                                m_rtQueueStop = (pPacket.Stop > 0 && pPacket.Stop > pPacket.Start) ? pPacket.Stop : pPacket.Start;
                            }
                            else
                            {
                                int _position = m_Packets.Count - 1;
                                int _skip = _position;
                                while (true)
                                {
                                    PacketData _packet = m_Packets[_position];
                                    if (_packet.Start != -1)
                                    {
                                        if (_packet.Start < pPacket.Start)
                                        {
                                            m_Packets.Insert(_skip, pPacket);
                                            m_AddPosition = _skip;
                                            break;
                                        }
                                        _skip = _position;
                                    }
                                    if (--_position < 0)
                                    {
                                        m_Packets.Insert(0, pPacket);
                                        m_AddPosition = 0;
                                        m_rtQueueStart = pPacket.Start;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (m_rtCacheDuration > 0)
            {
                if (Duration >= m_rtCacheDuration)
                {
                    m_evNotFull.Reset();
                }
                else
                {
                    m_evNotFull.Set();
                }
            }
        }

        public bool Peek(out PacketData ppPacket, bool bRemove)
        {
            ppPacket = null;
            bool bShouldReset = false;
            {
                lock (m_csLock)
                {
                    if (m_Packets.Count == 0) return false;
                    ppPacket = m_Packets[0];
                    if (bRemove)
                    {
                        if (m_AddPosition == 0)
                        {
                            m_AddPosition = -1;
                        }
                        m_Packets.Remove(ppPacket);
                        if (ppPacket.Stop != -1)
                        {
                            if (ppPacket.Stop > m_rtQueueStart)
                            {
                                m_rtQueueStart = ppPacket.Stop;
                                bShouldReset = true;
                            }
                        }
                        else
                        {
                            if (ppPacket.Start != -1 && ppPacket.Start > m_rtQueueStart)
                            {
                                m_rtQueueStart = ppPacket.Start;
                                bShouldReset = true;
                            }
                        }
                    }
                }
            }
            if (bRemove && bShouldReset)
            {
                m_evNotFull.Set();
            }
            return true;
        }

        #endregion

        #region Operators

        public static implicit operator EventWaitHandle(PacketsQueue _queue)
        {
            return _queue.m_evNotFull;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Clear();
        }

        #endregion
    }

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class DemuxTrack : COMHelper,IDisposable
    {
        #region Enums

        [ComVisible(false)]
        public enum TrackType
        {
            Unknown = -1,
            Video = 0,
            Audio = 1,
            SubPicture = 2,
            Subtitles = 3,
        }

        #endregion

        #region Variables

        protected PacketsQueue m_Queue = new PacketsQueue();
        protected FileParser m_pParser = null;
        protected TrackType m_Type = TrackType.Unknown;
        protected long m_rtOffset = 0;
        protected long m_rtSampleTime = 0;
        protected long m_rtDuration = 0;
        protected long m_rtPosition = 0;
        protected bool m_bActive = false;
        protected EventWaitHandle m_hFlush = null;
        protected ManualResetEvent m_evReady = new ManualResetEvent(false);
        protected bool m_bFirstSample = true;
        protected bool m_bInvalidTrack = false;
        protected string m_sName = "";
        protected bool m_bEnabled = true;
        protected int m_lcid = 0;

        #endregion

        #region Constructor

        protected DemuxTrack(FileParser _parser,TrackType _type)
        {
            m_pParser = _parser;
            m_Type = _type;
        }

        ~DemuxTrack()
        {
            Dispose();
        }

        #endregion

        #region Properties

        public TrackType Type
        {
            get { return m_Type; }
        }

        public string Name
        {
            get { return m_sName; }
            set { m_sName = value; }
        }

        public bool Enabled
        {
            get { return m_bEnabled; }
            set { m_bEnabled = value; }
        }

        public int LCID
        {
            get { return m_lcid; }
            set { m_lcid = value; }
        }

        public bool IsTrackValid
        {
            get { return !m_bInvalidTrack; }
        }

        public bool IsStartSample
        {
            get { return m_bFirstSample; }
        }

        public long Offset
        {
            get { return m_rtOffset; }
        }

        public long Duration
        {
            get { if (m_rtDuration > 0) return m_rtDuration; else return m_pParser.Duration; }
            set { TrackDuration = value; }
        }

        public long TrackDuration
        {
            get { return m_rtDuration; }
            set { m_rtDuration = value; }
        }

        public long Position
        {
            get { return m_rtPosition; }
        }

        public bool Active
        {
            get { return m_bActive; }
            set { m_bActive = value; }
        }

        public long Allocated
        {
            get
            {
                long _time = m_Queue.CacheDuration;
                if (_time > 0) return _time;
                return 0;
            }
        }

        public bool IsWaiting
        {
            get { return !m_evReady.WaitOne(0,false) && IsEOS; }
        }
        
        public EventWaitHandle FlushEvent
        {
            get { return m_hFlush; }
            set { m_hFlush = value; }
        }

        public bool IsEOS
        {
            get { return m_bActive && m_pParser.EOSEvent.WaitOne(0,false) && m_Queue.IsEmpty; }
        }

        #endregion

        #region Helper Methods

        public virtual bool Reset()
        {
            if (IsWaiting)
            {
                m_evReady.Set();
                return true;
            }
            return false;
        }

        public virtual void Alloc(long _time)
        {
            m_Queue.CacheDuration = _time;
        }

        public virtual void Flush()
        {
            m_Queue.Clear();
            m_evReady.Reset();
        }

        public virtual bool AddToCache(ref PacketData pPacket)
        {
            if (pPacket != null)
            {
                if (Active)
                {
                    if (m_Queue.IsFull && pPacket.Start >= m_Queue.StopTime)
                    {
                        if (0 != WaitHandle.WaitAny(new WaitHandle[] { m_Queue, m_pParser.QuitEvent, m_hFlush }))
                        {
                            pPacket.Dispose();
                            return false;
                        }
                    }
                    m_Queue.Add(pPacket);
                    m_evReady.Set();
                    return true;
                }
                else
                {
                    pPacket.Dispose();
                    return false;
                }
            }
            return false;
        }

        public virtual PacketData GetNextPacket()
        {
            while (0 == WaitHandle.WaitAny(new WaitHandle[] { m_evReady, m_pParser.QuitEvent, m_hFlush }))
            {
                PacketData pPacket;
                if (m_Queue.Peek(out pPacket, true))
                {
                    return pPacket;
                }
                else
                {
                    if (IsEOS) break;
                }
                {
                    if (m_Queue.IsEmpty)
                    {
                        m_evReady.Reset();
                    }
                }
            }
            return null;
        }

        #endregion

        #region Basic Methods For Override

        public virtual HRESULT GetTrackAllocatorRequirements(ref int plBufferSize,ref short pwBuffers)
        {
            return S_FALSE;
        }

        public virtual HRESULT SetMediaType(AMMediaType pmt)
        {
            if (pmt.formatPtr == IntPtr.Zero) return VFW_E_INVALIDMEDIATYPE;
            return NOERROR;
        }

        public virtual HRESULT ReadMediaSample(ref IMediaSampleImpl pSample)
        {
            PacketData _packet = GetNextPacket();
            if (_packet == null) return S_FALSE;
            pSample.SetMediaTime(null, null);
            pSample.SetPreroll(false);
            pSample.SetDiscontinuity(false);
            pSample.SetSyncPoint(_packet.SyncPoint);

            if (_packet.Start >= 0)
            {
                long _start = _packet.Start;
                long _stop = _packet.Stop;
                if (_start < m_rtOffset)
                {
                    pSample.SetPreroll(true);
                    pSample.SetSyncPoint(false);
                    if (m_Type != TrackType.Audio)
                    {
                        pSample.SetDiscontinuity(true);
                    }
                }
                if (_stop < 0 || _stop <= _start || _start == _stop + 1)
                {
                    if (m_rtSampleTime > 0) _stop = _start + m_rtSampleTime;
                }
                _start -= m_rtOffset;
                if (_stop > 0)
                {
                    _stop -= m_rtOffset;
                    pSample.SetTime(_start, _stop);
                }
                else
                {
                    pSample.SetTime(_start, null);
                }
                m_rtPosition = _start;
            }
            IntPtr pBuffer;
            pSample.GetPointer(out pBuffer);
            int _readed = 0;
            ASSERT(pSample.GetSize() >= _packet.Size);

            _readed = FillSampleBuffer(pBuffer, pSample.GetSize(), ref _packet);
            _packet.Dispose();
            pSample.SetActualDataLength(_readed);

            m_bFirstSample = false;

            if (_readed == 0) return S_FALSE;

            return NOERROR;
        }

        public virtual HRESULT SeekTrack(long _time)
        {
            Flush();
            m_rtPosition = _time;
            m_bFirstSample = true;
            return NOERROR;
        }

        protected virtual int FillSampleBuffer(IntPtr pBuffer,int _size,ref PacketData _packet)
        {
            int _readed = 0;
            if (_packet.Buffer != null)
            {
                _readed = _packet.Size <= _size ? _packet.Size : _size;
                Marshal.Copy(_packet.Buffer, 0, pBuffer, _readed);
            }
            else
            {
                byte[] _buffer = new byte[_packet.Size];
                m_pParser.Stream.ReadData(_packet.Position,_buffer,_packet.Size,out _readed);
                if (_readed > 0)
                {
                    Marshal.Copy(_buffer, 0, pBuffer, _readed);
                }
            }
            return _readed;
        }

        #endregion

        #region Abstract Methods

	    public abstract HRESULT GetMediaType(int iPosition, ref AMMediaType pmt);

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            
        }

        #endregion
    }

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class FileParser : COMHelper,IDisposable
    {
        #region Variables

        protected BitStreamReader   m_Stream = null;
        protected List<DemuxTrack>	m_Tracks = new List<DemuxTrack>();
	    protected long	            m_rtDuration = 0;
	    protected EventWaitHandle	m_hQuit = null;
        protected EventWaitHandle   m_hEOS = null;
	    private bool				m_bRequireDemuxThread = false;
        protected string            m_sFileName = "";

        #endregion

        #region Constructor

        protected FileParser()
            : this(true)
        {

        }

        protected FileParser(bool bRequireDemuxThread)
        {
            m_bRequireDemuxThread = bRequireDemuxThread;
        }

	    ~FileParser()
        {
            CloseInput();
        }

        #endregion

        #region Properties

        public string FileName
        {
            get { return m_sFileName; }
        }

        public DemuxTrack this[int iIndex]
        {
            get { return m_Tracks[iIndex]; }
        }

        public bool RequireDemuxThread
        {
            get { return m_bRequireDemuxThread; }
            set { m_bRequireDemuxThread = value; }
        }

        public int Count
        {
            get { return m_Tracks.Count; }
        }

        public long Duration
        {
            get 
            {
                if (m_rtDuration == 0)
                {
                    foreach (DemuxTrack _track in m_Tracks)
                    {
                        long _duration = _track.TrackDuration + _track.Offset;
                        if (m_rtDuration < _duration)
                        {
                            m_rtDuration = _duration;
                        }
                    }
                }
                return m_rtDuration;
            }
        }

        public long Position
        {
            get
            {
                long _time = MAX_LONG;
                foreach (DemuxTrack _track in m_Tracks)
                {
                    if (_track.Active)
                    {
                        long _position = _track.Position;
                        if (_position > 0 && _position < _time)
                        {
                            _time = _position;
                        }
                    }
                }
                return _time == MAX_LONG ? 0 : _time;
            }
        }

        public EventWaitHandle EOSEvent
        {
            get { return m_hEOS; }
            set { m_hEOS = value; }
        }

        public EventWaitHandle QuitEvent
        {
            get { return m_hQuit; }
            set { m_hQuit = value; }
        }

        public BitStreamReader Stream
        {
            get { return m_Stream; }
        }

        #endregion

        #region Helper Methods

        public DemuxTrack GetTrackByType(DemuxTrack.TrackType _type)
        {
            foreach (DemuxTrack _track in m_Tracks)
            {
                if (_track.Type == _type)
                {
                    return _track;
                }
            }
            return null;
        }

        public bool HaveTrack(DemuxTrack.TrackType _type)
        {
            return GetTrackByType(_type) != null;
        }

        public int GetTracksCountByType(DemuxTrack.TrackType _type)
        {
            int nCount = 0;
            foreach (DemuxTrack _track in m_Tracks)
            {
                if (_track.Type == _type)
                {
                    nCount++;
                }
            }
            return nCount;
        }

        public int GetActiveTracksCount()
        {
            return GetActiveTracksCount(DemuxTrack.TrackType.Unknown);
        }

        public int GetActiveTracksCount(DemuxTrack.TrackType _type)
        {
            int nCount = 0;
            foreach (DemuxTrack _track in m_Tracks)
            {
                if ((_type == DemuxTrack.TrackType.Unknown || _track.Type == _type) && _track.Active)
                {
                    nCount++;
                }
            }
            return nCount;
        }

        #endregion

        #region Virtual Methods

        public virtual HRESULT CloseInput()
        {
            while (m_Tracks.Count > 0)
            {
                m_Tracks[0].Dispose();
                m_Tracks.RemoveAt(0);
            }
            if (m_Stream != null)
            {
                m_Stream.Dispose();
                m_Stream = null;
            }
            m_rtDuration = 0;
            m_sFileName = "";
            return NOERROR;
        }

        public virtual HRESULT OpenInput(BitStreamReader _stream)
        {
            CloseInput();
            if (_stream == null) return E_POINTER;
            m_Stream = _stream;
            HRESULT hr = CheckFile();
            if (hr == S_OK)
            {
                hr = LoadTracks();
            }
            if (hr != S_OK)
            {
                CloseInput();
            }
            return hr;
        }

        public virtual HRESULT OpenInput(string sFileName)
        {
            CloseInput();
            if (string.IsNullOrEmpty(sFileName)) return E_POINTER;
            m_sFileName = sFileName;
            HRESULT hr = CheckFile();
            if (hr == S_OK)
            {
                hr = LoadTracks();
            }
            if (hr != S_OK)
            {
                CloseInput();
            }
            return hr;
        }

        public virtual HRESULT SeekToTime(long _time)
        {
            foreach (DemuxTrack _track in m_Tracks)
            {
                _track.SeekTrack(_time);
            }
            return NOERROR;
        }

        public virtual HRESULT OnDemuxStart()
        {
            return NOERROR;
        }

        public virtual HRESULT OnDemuxStop()
        {
            return NOERROR;
        }

        public virtual HRESULT ProcessDemuxPackets()
        {
            return S_FALSE;
        }

        public virtual HRESULT GetOpeningProgress(out long pllTotal, out long pllCurrent)
        {
            pllTotal = 0;
            pllCurrent = 0;
            return E_NOTIMPL;
        }

        #endregion

        #region Abstract Methods

        protected abstract HRESULT CheckFile();
        protected abstract HRESULT LoadTracks();

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            CloseInput();
        }

        #endregion
    }

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class SplitterInputPin : BasePin
    {
        #region Constructor

        public SplitterInputPin(string _name, BaseSplitter _filter)
            : base(_name, _filter, _filter.FilterLock, PinDirection.Input)
        {

        }

        #endregion

        #region Overridden Methods

        public override int CheckMediaType(AMMediaType pmt)
        {
            return NOERROR;
        }

        public override int BeginFlush()
        {
            return (m_Filter as BaseSplitter).BeginFlush();
        }

        public override int EndFlush()
        {
            return (m_Filter as BaseSplitter).EndFlush();
        }

        public override int CompleteConnect(ref IPinImpl pReceivePin)
        {
            int hr = base.CompleteConnect(ref pReceivePin);
	        if (FAILED(hr)) return hr;
            return (m_Filter as BaseSplitter).CompleteConnect(ref pReceivePin);
        }

        public override int BreakConnect()
        {
            (m_Filter as BaseSplitter).UnloadFile();
            return base.BreakConnect();
        }

        #endregion
    }

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class SplitterOutputPin : BaseOutputPin, IMediaSeeking
    {
        #region Variables

        protected DemuxTrack m_pTrack = null;
        protected object m_csThreadLock = new object();
        protected object m_csEnableLock = new object();
        protected object m_csFlushLock = new object();
        protected ManualResetEvent m_evQuit = new ManualResetEvent(false);
        protected ManualResetEvent m_evReady = new ManualResetEvent(true);
        protected ManualResetEvent m_evFlush = new ManualResetEvent(false);
        protected bool m_bEOSDelivered = false;
        protected long m_rtPosition = -1;
        protected bool m_bHaveException = false;
        protected AMThread m_WorkerThread = null;
        protected bool m_bStreamEnabled = true;

        #endregion

        #region Constructor

        public SplitterOutputPin(DemuxTrack _track, string _name, BaseSplitter _filter)
            : base(_name, _filter)
        {
            m_pTrack = _track;
            ASSERT(m_pTrack != null);
            m_pTrack.FlushEvent = m_evFlush;
            m_WorkerThread = new ManagedThread(this.ThreadProc);
        }

        ~SplitterOutputPin()
        {
            StopThread();
        }

        #endregion

        #region Properties

        public bool EOSDelivered
        {
            get { return m_bEOSDelivered; }
        }

        public DemuxTrack Track
        {
            get { return m_pTrack; }
        }

        public bool StreamEnabled
        {
            get { return m_bStreamEnabled; }
            set {
                lock (m_csEnableLock)
                {
                    if (m_bStreamEnabled != value)
                    {
                        m_bStreamEnabled = value;
                    }
                }
            }
        }

        #endregion

        #region Overridden Methods

        public override int Active()
        {
            m_bHaveException = false;
            int hr = base.Active();
            if (SUCCEEDED(hr) && IsConnected)
            {
                StartThread();
            }
            return hr;
        }

        public override int Inactive()
        {
            int hr = base.Inactive();
            StopThread();
            m_rtPosition = -1;
            return hr;
        }

        public override int DeliverBeginFlush()
        {
            HRESULT hr = NOERROR;
            lock (m_csFlushLock)
            {
                if (!m_evFlush.WaitOne(0,false))
                {
                    TRACE("Begin Flush");
                    {
                        m_evFlush.Set();
                        if (IsConnected)
                        {
                            hr = (HRESULT)base.DeliverBeginFlush();
                            hr.Assert();
                        }
                    }
                }
            }
            return hr;
        }

        public override int DeliverEndFlush()
        {
            HRESULT hr = NOERROR;
            lock (m_csFlushLock)
            {
                if (m_evFlush.WaitOne(0,false))
                {
                    m_pTrack.Flush();
                    if (IsConnected)
                    {
                        lock (m_csThreadLock)
                        {
                            hr = (HRESULT)base.DeliverEndFlush();
                        }
                        hr.Assert();
                    }
                    TRACE("End Flush");
                    m_evFlush.Reset();
                }
            }
            return hr;
        }

        public override int CompleteConnect(ref IPinImpl pReceivePin)
        {
            int hr = base.CompleteConnect(ref pReceivePin);
            if (SUCCEEDED(hr) && m_pTrack != null)
            {
                long _time = 2 * UNITS;
                m_pTrack.Alloc(_time);
                m_pTrack.Active = true;
            }
            return hr;
        }

        public override int BreakConnect()
        {
            int hr = base.BreakConnect();
            if (m_pTrack != null)
            {
                m_pTrack.Flush();
                m_pTrack.Active = false;
            }
            (m_Filter as BaseSplitter).DeselectSeekingPin(this);
            return hr;
        }

        public override int CheckMediaType(AMMediaType pmt)
        {
            return NOERROR;
        }

        public override int GetMediaType(int iPosition, ref AMMediaType pMediaType)
        {
            if (iPosition < 0) return E_INVALIDARG;
            return m_pTrack.GetMediaType(iPosition, ref pMediaType);
        }

        public override int SetMediaType(AMMediaType mt)
        {
            HRESULT hr = m_pTrack.SetMediaType(mt);
            if (FAILED(hr)) return hr;
            return base.SetMediaType(mt);
        }

        public override int DecideBufferSize(ref IMemAllocatorImpl pAlloc, ref AllocatorProperties prop)
        {
            if (!IsConnected) return VFW_E_NOT_CONNECTED;
            if (m_mt.majorType == MediaType.Video)
            {
                BitmapInfoHeader _bmi = m_mt;
                if (_bmi == null) return VFW_E_INVALIDMEDIATYPE;
                int lSize = m_mt.sampleSize;
                lSize = Math.Max(lSize, _bmi.ImageSize);
                lSize = Math.Max(lSize, _bmi.Width * Math.Abs(_bmi.Height) * _bmi.BitCount / 8);
                if (_bmi.BitCount == 0)
                {
                    lSize = Math.Max(lSize, _bmi.Width * Math.Abs(_bmi.Height) * 4);
                }
                if (_bmi.Width == 0 || _bmi.Height == 0)
                {
                    lSize = Math.Max(lSize, GetSystemMetrics(0) * GetSystemMetrics(1) * 4);
                }
                if (lSize > prop.cbBuffer)
                {
                    prop.cbBuffer = lSize;
                }
                if (prop.cBuffers < 4)
                {
                    prop.cBuffers = 4;
                }
            }
            if (m_mt.majorType == MediaType.Audio)
            {
                WaveFormatEx _wfx = m_mt;
                if (_wfx == null) return VFW_E_INVALIDMEDIATYPE;
                if (_wfx.nAvgBytesPerSec > 0)
                {
                    prop.cbBuffer = _wfx.nAvgBytesPerSec;
                }
                else
                {
                    prop.cbBuffer = _wfx.nSamplesPerSec * _wfx.nBlockAlign;
                }
                prop.cBuffers = 4;
                prop.cbAlign = _wfx.nBlockAlign;
                if (prop.cbBuffer == 0)
                {
                    prop.cbBuffer = (_wfx.nSamplesPerSec * _wfx.nChannels) << 3;
                }
            }
            {
                int lBufferSize = prop.cbBuffer;
                short wBuffers = (short)prop.cBuffers;
                if (S_OK == m_pTrack.GetTrackAllocatorRequirements(ref lBufferSize, ref wBuffers))
                {
                    if (wBuffers > 0) prop.cBuffers = wBuffers;
                    if (lBufferSize > 0) prop.cbBuffer = lBufferSize;
                }
            }
            if (prop.cbAlign < 1)
            {
                prop.cbAlign = 1;
            }
            prop.cbPrefix = 0;

            if (prop.cbBuffer == 0) return E_UNEXPECTED;

            AllocatorProperties _actual = new AllocatorProperties();
            int hr = pAlloc.SetProperties(prop, _actual);
            if (FAILED(hr)) return hr;
            if (_actual.cbBuffer < prop.cbBuffer) return E_FAIL;

            return NOERROR;
        }

        public override int Notify(IntPtr pSelf, Quality q)
        {
            return NOERROR;
        }

        #endregion

        #region Methods

        public void StartThread()
        {
            m_evQuit.Reset();
            if (m_evReady.WaitOne(0,false))
            {
                m_WorkerThread.Create();
            }
        }

        public void StopThread()
        {
            m_evQuit.Set();
            if (!m_evReady.WaitOne(0,false))
            {
                bool bFlush = false;
                if (m_WorkerThread.ThreadExists)
                {
                    if (!m_WorkerThread.Join(10000))
                    {
                        lock (m_csFlushLock)
                        {
                            if (!m_evFlush.WaitOne(0,false))
                            {
                                m_evFlush.Set();
                                bFlush = true;
                            }
                        }
                    }
                }
                if (bFlush)
                {
                    m_evFlush.Reset();
                }
            }
            m_WorkerThread.Close();
        }

        #endregion

        #region Protected Methods

        protected virtual void ThreadProc()
        {
	        m_bEOSDelivered = false;
	        IMediaSampleImpl _sample;
	        long _start,_stop;
	        double _rate;
	        (m_Filter as BaseSplitter).GetPositions(out _start,out _stop,out _rate);
	        DeliverNewSegment(_start, _stop, _rate);
	        m_evReady.Reset();
            long _shift = 0;
            bool bEnabled;
            lock (m_csEnableLock)
            {
                bEnabled = m_bStreamEnabled;
            }
	        while (!m_evQuit.WaitOne(0,false))
	        {
		        long tStart = 0,tStop = 0;
		        bool bEOS = m_pTrack.IsEOS;
		        _sample = null;
		        HRESULT hr;
		        if (!bEOS)
		        {
                    lock (m_csThreadLock)
                    {
                        IntPtr pSample;
                        hr = (HRESULT)GetDeliveryBuffer(out pSample, null, null, AMGBF.None);
                        if (hr.Succeeded)
                        {
                            _sample = new IMediaSampleImpl(pSample);
                            AMMediaType pmt;
                            if (_sample.GetMediaType(out pmt) == NOERROR)
                            {
                                if (FAILED(SetMediaType(pmt)))
                                {
                                    _sample.SetMediaType(null);
                                }
                                pmt.Free();
                            }
                            hr = m_pTrack.ReadMediaSample(ref _sample);
                            if (hr == S_FALSE || FAILED(hr))
                            {
                                bEOS = true;
                            }
                            else
                            {
                                if (S_OK == _sample.GetTime(out tStart, out tStop))
                                {
                                    if (
                                            (tStart > tStop && tStop > _stop)
                                        || (tStart <= tStop && tStart > _stop)
                                        )
                                    {
                                        bEOS = true;
                                    }
                                    else
                                    {
                                        tStart -= _start;
                                        tStop -= _start;
                                        m_rtPosition = tStart;
                                        if (_rate != 1.0)
                                        {
                                            tStart = (long)(tStart / _rate);
                                            tStop = (long)(tStop / _rate);
                                        }
                                    }
                                    if (bEnabled)
                                    {
                                        tStart -= _shift;
                                        tStop -= _shift;
                                    }
                                    else
                                    {
                                        _shift += tStop - tStart;
                                    }
                                    if (tStart < 0)
                                    {
                                        _sample.SetPreroll(true);
                                    }
                                    _sample.SetTime(tStart, tStop);
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    //TRACE(string.Format("Sample: start {0}, stop {1}", tStart / 10000, tStop / 10000));
		        }
		        if (bEOS)
		        {
			        if (_sample != null) _sample._Release();
			        if (!m_evFlush.WaitOne(0,false) && !(m_Filter as BaseSplitter).QuitEvent.WaitOne(0,false) && !m_evQuit.WaitOne(0,false))
			        {
				        DeliverBeginFlush();
				        DeliverEndFlush();
				        TRACE("EOS Delivered");
#if !DEBUG
				        try {
#endif
				        DeliverEndOfStream();
#if !DEBUG
				        } catch
				        {
					        m_bHaveException = true;
					        TRACE("EOS exception");
				            m_Filter.NotifyEvent(EventCode.ErrorAbort, (IntPtr)((int)E_UNEXPECTED), IntPtr.Zero);	
				        }
#endif                       
				        m_bEOSDelivered = true;
			        }
			        break;
		        }
		        if (_sample != null)
		        {
			        if (m_evFlush.WaitOne(0,false) && m_pTrack.Type == DemuxTrack.TrackType.Audio)
			        {
				        _sample.SetDiscontinuity(true);
			        }
                    lock (m_csEnableLock)
                    {
				        bEnabled = m_bStreamEnabled;
			        }
			        if (!bEnabled)
			        {
                        _sample._Release();
				        continue;
			        }
#if !DEBUG
                    try {
#endif
			        hr = (HRESULT)Deliver(ref _sample);
			        _sample._Release();
			        if (hr != S_OK)
			        {
				        break;
			        }
#if !DEBUG
			        } catch
			        {
				        m_bHaveException = true;
				        TRACE("Pin delivery exception");
				        if (_sample != null)
				        {
					        Allocator.ReleaseBuffer(_sample.UnknownPtr);
                            _sample = null;
				        }
				        if (!m_evFlush.WaitOne(0,false) && !m_bEOSDelivered)
				        {
					        TRACE("EOS Delivered");
					        DeliverEndOfStream();
					        m_bEOSDelivered = true;
				        }
				        m_pTrack.Flush();
				        m_Filter.NotifyEvent(EventCode.ErrorAbort, (IntPtr)((int)E_UNEXPECTED), IntPtr.Zero);
				        break;
			        }
#endif
                }
            }
	        m_evReady.Set();
        }

        #endregion

        #region IMediaSeeking Members

        public virtual int GetCapabilities(out AMSeekingSeekingCapabilities pCapabilities)
        {
            pCapabilities = AMSeekingSeekingCapabilities.CanSeekAbsolute |
                            AMSeekingSeekingCapabilities.CanSeekForwards |
                            AMSeekingSeekingCapabilities.CanSeekBackwards |
                            AMSeekingSeekingCapabilities.CanGetDuration |
                            AMSeekingSeekingCapabilities.CanGetCurrentPos |
                            AMSeekingSeekingCapabilities.CanGetStopPos;
            return NOERROR;
        }

        public virtual int CheckCapabilities(ref AMSeekingSeekingCapabilities pCapabilities)
        {
            AMSeekingSeekingCapabilities dwActual;
            GetCapabilities(out dwActual);
            if ((int)((int)pCapabilities & (~(int)dwActual)) == 0)
            {
                return S_FALSE;
            }
            return S_OK;
        }

        public virtual int IsFormatSupported(Guid pFormat)
        {
            if (pFormat == TimeFormat.MediaTime)
            {
                return S_OK;
            }
            return S_FALSE;
        }

        public virtual int QueryPreferredFormat(out Guid pFormat)
        {
            pFormat = TimeFormat.MediaTime;
            return S_OK;
        }

        public virtual int GetTimeFormat(out Guid pFormat)
        {
            return QueryPreferredFormat(out pFormat);
        }

        public virtual int IsUsingTimeFormat(Guid pFormat)
        {
            Guid guidActual;
            int hr = GetTimeFormat(out guidActual);
            if (SUCCEEDED(hr) && (guidActual == pFormat))
            {
                return S_OK;
            }
            else
            {
                return S_FALSE;
            }
        }

        public virtual int SetTimeFormat(Guid pFormat)
        {
            if (pFormat == TimeFormat.MediaTime)
            {
                return (m_Filter as BaseSplitter).SelectSeekingPin(this) ? S_OK : E_NOTIMPL;
            }
            else
                if (pFormat == TimeFormat.None)
                {
                    (m_Filter as BaseSplitter).DeselectSeekingPin(this);
                    return S_OK;
                }
            return E_NOTIMPL;
        }

        public virtual int GetDuration(out long pDuration)
        {
            pDuration = 0;
            if (m_pTrack != null)
            {
                pDuration = m_pTrack.Duration + m_pTrack.Offset;
            }
            return NOERROR;
        }

        public virtual int GetStopPosition(out long pStop)
        {
            long _current;
            return GetPositions(out _current,out pStop);
        }

        public virtual int GetCurrentPosition(out long pCurrent)
        {
            long _stop;
            return GetPositions(out pCurrent, out _stop);
        }

        public virtual int ConvertTimeFormat(out long pTarget, DsGuid pTargetFormat, long Source, DsGuid pSourceFormat)
        {
            pTarget = 0;
            if (pTargetFormat == null || pTargetFormat == TimeFormat.MediaTime)
            {
                if (pSourceFormat == null || pSourceFormat == TimeFormat.MediaTime)
                {
                    pTarget = Source;
                    return S_OK;
                }
            }
            return E_INVALIDARG;
        }

        public virtual int SetPositions(DsLong pCurrent, AMSeekingSeekingFlags dwCurrentFlags, DsLong pStop, AMSeekingSeekingFlags dwStopFlags)
        {
            BaseSplitter _filter = (m_Filter as BaseSplitter);
            if (_filter.SelectSeekingPin(this))
            {
                long _start, _stop;
                double _rate;
                _filter.GetPositions(out _start, out _stop, out _rate);
                if (((int)dwCurrentFlags & (int)AMSeekingSeekingFlags.AbsolutePositioning) != 0)
                {
                    _start = pCurrent;
                }
                else if (((int)dwCurrentFlags & (int)AMSeekingSeekingFlags.RelativePositioning) != 0)
                {
                    _start += pCurrent;
                }

                if (((int)dwStopFlags & (int)AMSeekingSeekingFlags.AbsolutePositioning) != 0)
                {
                    _stop = pStop;
                }
                else if (((int)dwStopFlags & (int)AMSeekingSeekingFlags.IncrementalPositioning) != 0)
                {
                    _stop = pStop + _start;
                }
                else
                {
                    if (((int)dwStopFlags & (int)AMSeekingSeekingFlags.RelativePositioning) != 0)
                    {
                        _stop += pStop;
                    }
                }

                if (((int)dwCurrentFlags & (int)AMSeekingSeekingFlags.PositioningBitsMask) != 0)
                {
                    return _filter.SetPositions(_start, _stop, -1);
                }
                else if (((int)dwStopFlags & (int)AMSeekingSeekingFlags.PositioningBitsMask) != 0)
                {
                    // stop change only
                    return _filter.SetPositions(-1, _stop, -1);
                }
                else
                {
                    return S_FALSE;
                }
            }
            return NOERROR;
        }

        public virtual int GetPositions(out long pCurrent, out long pStop)
        {
            double _rate;
            (m_Filter as BaseSplitter).GetPositions(out pCurrent, out  pStop, out _rate);
            if (m_rtPosition >= 0)
            {
                pCurrent = m_rtPosition;
            }
            return NOERROR;
        }

        public virtual int GetAvailable(out long pEarliest, out long pLatest)
        {
            pEarliest = 0;
            pLatest = m_pTrack.Duration;
            return NOERROR;
        }

        public virtual int SetRate(double dRate)
        {
            if ((m_Filter as BaseSplitter).SelectSeekingPin(this))
            {
                return (m_Filter as BaseSplitter).SetPositions(-1, -1, dRate);
            }
            return NOERROR;
        }

        public virtual int GetRate(out double pdRate)
        {
            long _start, _stop;
            (m_Filter as BaseSplitter).GetPositions(out _start, out _stop, out pdRate);
            return NOERROR;
        }

        public virtual int GetPreroll(out long pllPreroll)
        {
            pllPreroll = m_pTrack.Offset;
            return NOERROR;
        }

        #endregion

        #region API

        [DllImport("User32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        #endregion
    }

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class BaseSplitter : BaseFilter, IMediaSeeking, IAMStreamSelect
    {
        #region Variables

        protected string m_sFileName = "";
        protected object m_csSeeking = new object();
        protected long m_tStartTime = 0;
        protected long m_tStopTime = MAX_LONG;
        protected double m_dRate = 1.0;
        protected SplitterOutputPin m_pSeekingPin = null;
        protected ManualResetEvent m_evQuit = new ManualResetEvent(false);
        protected ManualResetEvent m_evReady = new ManualResetEvent(true);
        protected ManualResetEvent m_evEOS = new ManualResetEvent(true);
        protected AutoResetEvent m_evDemuxStarted = new AutoResetEvent(false);
        protected bool m_bLoading = false;
        protected object m_csThreadLock = new object();
        protected FileParser m_pFileParser = null;
        protected AMThread m_WorkerThread = null;
        protected List<FileParser> m_Parsers = new List<FileParser>();

        #endregion

        #region Constructor

        protected BaseSplitter(string _name)
            : base(_name)
        {
            m_WorkerThread = new ManagedThread(this.ThreadProc);
        }

        ~BaseSplitter()
        {
            UnloadFile();
        }

        #endregion

        #region Properties

        public string FileName
        {
            get 
            {
                if (string.IsNullOrEmpty(m_sFileName) && InputPin != null)
                {
                    try
                    {
                        PinInfo _info;
                        InputPin.QueryPinInfo(out _info);
                        if (_info.filter != null)
                        {
                            IFileSourceFilter _source = (IFileSourceFilter)_info.filter;
                            _source.GetCurFile(out m_sFileName, null);
                        }
                        _info.filter = null;
                    }
                    catch
                    {
                    }
                }
                return m_sFileName;
            }
        }

        public EventWaitHandle QuitEvent
        {
            get { return m_evQuit; }
        }

        public SplitterInputPin InputPin
        {
            get 
            {
                if (Pins.Count > 0 && Pins[0].Direction == PinDirection.Input)
                {
                    return (Pins[0] as SplitterInputPin);
                }
                return null; 
            }
        }

        public SplitterOutputPin SeekingPin
        {
            get 
            {
                lock (m_csSeeking)
                {
                    if (m_pSeekingPin != null && !m_pSeekingPin.IsConnected)
                    {
                        m_pSeekingPin = null;
                    }
                    if (m_pSeekingPin == null)
                    {
                        foreach (BasePin _pin in Pins)
                        {
                            if (_pin.Direction == PinDirection.Output && _pin.IsConnected)
                            {
                                m_pSeekingPin = (_pin as SplitterOutputPin);
                                break;
                            }
                        }
                        foreach (BasePin _pin in Pins)
                        {
                            if (_pin.Direction == PinDirection.Output)
                            {
                                if (m_pSeekingPin == null)
                                {
                                    m_pSeekingPin = (_pin as SplitterOutputPin);
                                }
                                else
                                {
                                    if (_pin.IsConnected && _pin != m_pSeekingPin)
                                    {
                                        if (m_pSeekingPin.Track.TrackDuration < (_pin as SplitterOutputPin).Track.TrackDuration)
                                        {
                                            m_pSeekingPin = (_pin as SplitterOutputPin);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return m_pSeekingPin;
            }
        }

        #endregion

        #region Overridden Methods

        protected override int OnInitializePins()
        {
            return NOERROR;
        }

        public override int Pause()
        {
            if (m_State == FilterState.Stopped)
            {
                HRESULT hr = StartThread();
                if (hr.Failed) return hr;
            }
            return base.Pause();
        }

        public override int Stop()
        {
            StopThread();
            return base.Stop();
        }

        protected override int AfterInstall(HRESULT hr, ref RegFilter2 _reginfo, ref IFilterMapper2 _mapper2)
        {
            if (hr.Succeeded)
            {
                Type _type = this.GetType();
                RegisterProtocol(_type);
                RegisterFileExtension(_type);
                RegisterMediaType(_type);
                foreach (FileParser _parser in m_Parsers)
                {
                    _type = _parser.GetType();
                    RegisterProtocol(_type);
                    RegisterFileExtension(_type);
                    RegisterMediaType(_type);
                }
            }
            return base.AfterInstall(hr, ref _reginfo, ref _mapper2);
        }

        protected override int AfterUninstall(HRESULT hr, ref IFilterMapper2 _mapper2)
        {
            Type _type = this.GetType();
            UnregisterProtocol(_type);
            UnregisterFileExtension(_type);
            UnregisterMediaType(_type);
            foreach (FileParser _parser in m_Parsers)
            {
                _type = _parser.GetType();
                UnregisterProtocol(_type);
                UnregisterFileExtension(_type);
                UnregisterMediaType(_type);
            }
            return base.AfterUninstall(hr, ref _mapper2);
        }

        #endregion

        #region Virtual Methods

        public virtual HRESULT BeginFlush()
        {
            foreach (BasePin _pin in Pins)
            {
                if (_pin.Direction == PinDirection.Output)
                {
                    (_pin as SplitterOutputPin).DeliverBeginFlush();
                }
            }
            return NOERROR;
        }

        public virtual HRESULT EndFlush()
        {
            foreach (BasePin _pin in Pins)
            {
                if (_pin.Direction == PinDirection.Output)
                {
                    (_pin as SplitterOutputPin).DeliverEndFlush();
                }
            }
            return NOERROR;
        }

        public virtual HRESULT CompleteConnect(ref IPinImpl pReceivePin)
        {
            UnloadFile();

            m_bLoading = true;
            BitStreamReader _stream = null;
            Guid _guid = typeof(IAsyncReader).GUID;
            IntPtr pAsyncReader = IntPtr.Zero;
            if (S_OK == pReceivePin._QueryInterface(ref _guid, out pAsyncReader))
            {
                _stream = new BitStreamReader(new AsyncStream(pAsyncReader));
                try
                {
                    if (S_OK == DecideFileParser(_stream))
                    {
                        m_bLoading = false;
                        return NOERROR;
                    }
                }
                catch
                {
                }
                finally
                {
                    Marshal.Release(pAsyncReader);
                }
            }
            UnloadFile();
            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }
            m_sFileName = FileName;
            if (!string.IsNullOrEmpty(m_sFileName))
            {
                try
                {
                    if (S_OK == DecideFileParser(m_sFileName))
                    {
                        m_bLoading = false;
                        return NOERROR;
                    }
                }
                catch
                {
                }
            }
            m_bLoading = false;
            UnloadFile();
            return VFW_E_CANNOT_CONNECT;
        }

        public virtual HRESULT UnloadFile()
        {
            m_evQuit.Set();
            lock (m_Lock)
            {
                m_sFileName = "";
                {
                    lock (m_csSeeking)
                    {
                        m_pSeekingPin = null;
                    }
	            }
                while (Pins.Count > 0)
                {
                    BasePin _pin = Pins[Pins.Count - 1];
                    if (_pin.Direction == PinDirection.Input) break;
                    Pins.Remove(_pin);
                    if (_pin.IsConnected)
                    {
                        _pin.Connected.Disconnect();
                        _pin.Disconnect();
                    }
                }
                if (m_pFileParser != null)
                {
                    m_pFileParser.CloseInput();
                    m_pFileParser = null;
                }
            }
            m_evQuit.Reset();
            return NOERROR;
        }

        protected virtual HRESULT DecideFileParser(string sFileName)
        {
            foreach (FileParser _parser in m_Parsers)
            {
                _parser.QuitEvent = m_evQuit;
                _parser.EOSEvent = m_evEOS;
                if (S_OK == _parser.OpenInput(sFileName))
                {
                    if (_parser.Count > 0)
                    {
                        m_pFileParser = _parser;
                        if (InitializeOutputPins() == S_OK)
                        {
                            return S_OK;
                        }
                        else
                        {
                            _parser.CloseInput();
                            m_pFileParser = null;
                        }
                    }
                    else
                    {
                        _parser.CloseInput();
                    }
                }
            }
            return E_FAIL;
        }

        protected virtual HRESULT DecideFileParser(BitStreamReader _reader)
        {
            if (_reader == null) return E_POINTER;
            foreach (FileParser _parser in m_Parsers)
            {
                _parser.QuitEvent = m_evQuit;
                _parser.EOSEvent = m_evEOS;
                if (S_OK == _parser.OpenInput(_reader))
                {
                    if (_parser.Count > 0)
                    {
                        m_pFileParser = _parser;
                        if (InitializeOutputPins() == S_OK)
                        {
                            return S_OK;
                        }
                        else
                        {
                            _parser.CloseInput();
                            m_pFileParser = null;
                        }
                    }
                    else
                    {
                        _parser.CloseInput();
                    }
                }
            }
            return E_FAIL;
        }

        protected virtual HRESULT InitializeOutputPins()
        {
            if (m_pFileParser == null) return E_UNEXPECTED;
            int nAdded = 0;
	        int nCount = m_pFileParser.Count;
            int[] _indexes = new int[(int)DemuxTrack.TrackType.Subtitles + 1];
            bool[] _useIndexes = new bool[_indexes.Length];
            for (int i = 0; i < _indexes.Length; i++)
            {
                _indexes[i] = 0;
                _useIndexes[i] = (m_pFileParser.GetTracksCountByType((DemuxTrack.TrackType)i) > 1);
            }
            for (int i = 0; i < nCount; i++)
            {
                DemuxTrack _track = m_pFileParser[i];
                if (_track != null && _track.IsTrackValid)
                {
                    string _name = _track.Name;
                    if (string.IsNullOrEmpty(_name))
                    {
                        _name = "Unknown";
                        switch (_track.Type)
                        {
                            case DemuxTrack.TrackType.Video:
                                _name = "Video";
                                break;
                            case DemuxTrack.TrackType.Audio:
                                _name = "Audio";
                                break;
                            case DemuxTrack.TrackType.Subtitles:
                                _name = "Subtitles";
                                break;
                            case DemuxTrack.TrackType.SubPicture:
                                _name = "Sub Picture";
                                break;
                        }
                        int _type = (int)_track.Type;
                        if (_type >= 0 && _type < _indexes.Length)
                        {
                            if (_useIndexes[_type])
                            {
                                _name += " " + (_indexes[_type]++).ToString();
                            }
                        }
                    }
                    if (!_track.Enabled)
                    {
                        _name = "~" + _name;
                    }
                    AddPin(new SplitterOutputPin(_track, _name, this));
                    nAdded++;
                }
            }
	        return (nAdded == 0 ? VFW_E_NO_ACCEPTABLE_TYPES : NOERROR);
        }

        #endregion

        #region Registration Helpers

        public virtual HRESULT RegisterProtocol(Type _type)
        {
            Attribute[] _attributes = Attribute.GetCustomAttributes(_type, typeof(RegisterProtocolExtension));
            if (_attributes != null)
            {
                foreach (RegisterProtocolExtension _setup in _attributes)
                {
                    if (_setup != null && !string.IsNullOrEmpty(_setup.Protocol))
                    {
                        Microsoft.Win32.RegistryKey _key = null;
                        try
                        {
                            if (_setup.Extensions.Count > 0)
                            {
                                _key = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(_setup.Protocol + "\\Extensions");
                                foreach (string _extension in _setup.Extensions)
                                {
                                    if (!string.IsNullOrEmpty(_extension))
                                    {
                                        _key.SetValue(_extension, this.GetType().GUID.ToString("B"));
                                    }
                                }
                            }
                            else
                            {
                                _key = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(_setup.Protocol);
                                _key.SetValue("Source Filter", this.GetType().GUID.ToString("B"));
                            }
                        }
                        catch (Exception _exception)
                        {
                            HRESULT hr = (HRESULT)Marshal.GetHRForException(_exception);
                            hr.TraceWrite();
                        }
                        finally
                        {
                            if (_key != null)
                            {
                                _key.Close();
                            }
                        }
                    }
                }
            }
            return NOERROR;
        }

        public virtual HRESULT RegisterFileExtension(Type _type)
        {
            Attribute[] _attributes = Attribute.GetCustomAttributes(_type, typeof(RegisterFileExtension));
            if (_attributes != null)
            {
                foreach (RegisterFileExtension _setup in _attributes)
                {
                    if (_setup != null && !string.IsNullOrEmpty(_setup.Extension))
                    {
                        Microsoft.Win32.RegistryKey _key = null;
                        try
                        {
                            _key = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey("Media Type\\Extensions\\" + _setup.Extension);
                            _key.SetValue("Source Filter", this.GetType().GUID.ToString("B"));
                            if (_setup.MediaType != Guid.Empty)
                            {
                                _key.SetValue("Media Type", _setup.MediaType.ToString("B"));
                            }
                            if (_setup.SubType != Guid.Empty)
                            {
                                _key.SetValue("SubType", _setup.SubType.ToString("B"));
                            }
                        }
                        catch (Exception _exception)
                        {
                            HRESULT hr = (HRESULT)Marshal.GetHRForException(_exception);
                            hr.TraceWrite();
                        }
                        finally
                        {
                            if (_key != null)
                            {
                                _key.Close();
                            }
                        }
                    }
                }
            }
            return NOERROR;
        }

        public virtual HRESULT RegisterMediaType(Type _type)
        {
            Attribute[] _attributes = Attribute.GetCustomAttributes(_type, typeof(RegisterMediaType));
            if (_attributes != null)
            {
                foreach (RegisterMediaType _setup in _attributes)
                {
                    if (_setup != null && !string.IsNullOrEmpty(_setup.Sequence))
                    {
                        Microsoft.Win32.RegistryKey _key = null;
                        try
                        {
                            string _path = "Media Type\\";
                            if (_setup.MajorType != Guid.Empty)
                            {
                                _path += _setup.MajorType.ToString("B") + "\\";
                            }
                            else
                            {
                                _path += MediaType.Stream.ToString("B") + "\\";
                            }
                            _path += _setup.SubType.ToString("B");
                            _key = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(_path);
                            if (_setup.FilterGuid == Guid.Empty)
                            {
                                if (this.InputPin == null)
                                {
                                    _key.SetValue("Source Filter", this.GetType().GUID.ToString("B"));
                                }
                                else
                                {
                                    _key.SetValue("Source Filter", typeof(DSFileSourceAsync).GUID.ToString("B"));
                                }
                            }
                            else
                            {
                                _key.SetValue("Source Filter", _setup.FilterGuid.ToString("B"));
                            }
                            int nIndex = 0;
                            while (null != _key.GetValue(nIndex.ToString(), null)) { nIndex++; }
                            _key.SetValue(nIndex.ToString(), _setup.Sequence);
                        }
                        catch (Exception _exception)
                        {
                            HRESULT hr = (HRESULT)Marshal.GetHRForException(_exception);
                            hr.TraceWrite();
                        }
                        finally
                        {
                            if (_key != null)
                            {
                                _key.Close();
                            }
                        }
                    }
                }
            }
            return NOERROR;
        }

        public virtual HRESULT UnregisterProtocol(Type _type)
        {
            Attribute[] _attributes = Attribute.GetCustomAttributes(_type, typeof(RegisterProtocolExtension));
            if (_attributes != null)
            {
                foreach (RegisterProtocolExtension _setup in _attributes)
                {
                    if (_setup != null && !string.IsNullOrEmpty(_setup.Protocol))
                    {
                        Microsoft.Win32.RegistryKey _key = null;
                        try
                        {
                            if (_setup.Extensions.Count > 0)
                            {
                                _key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(_setup.Protocol + "\\Extensions");
                                if (_key != null)
                                {
                                    foreach (string _extension in _setup.Extensions)
                                    {
                                        if (!string.IsNullOrEmpty(_extension))
                                        {
                                            _key.DeleteValue(_extension);
                                        }
                                    }
                                    if (_key.ValueCount == 0 && _key.SubKeyCount == 0)
                                    {
                                        _key.Close();
                                        _key = null;
                                        _key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(_setup.Protocol);
                                        _key.DeleteSubKeyTree("Extensions");
                                        if (_key.ValueCount == 0 && _key.SubKeyCount == 0)
                                        {
                                            _key.Close();
                                            _key = null;
                                            Microsoft.Win32.Registry.ClassesRoot.DeleteSubKeyTree(_setup.Protocol);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                _key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(_setup.Protocol);
                                if (_key != null)
                                {
                                    _key.DeleteValue("Source Filter");
                                    if (_key.ValueCount == 0 && _key.SubKeyCount == 0)
                                    {
                                        _key.Close();
                                        _key = null;
                                        Microsoft.Win32.Registry.ClassesRoot.DeleteSubKeyTree(_setup.Protocol);
                                    }
                                }
                            }
                        }
                        catch (Exception _exception)
                        {
                            HRESULT hr = (HRESULT)Marshal.GetHRForException(_exception);
                            hr.TraceWrite();
                        }
                        finally
                        {
                            if (_key != null)
                            {
                                _key.Close();
                            }
                        }
                    }
                }
            }
            return NOERROR;
        }

        public virtual HRESULT UnregisterFileExtension(Type _type)
        {
            Attribute[] _attributes = Attribute.GetCustomAttributes(_type, typeof(RegisterFileExtension));
            if (_attributes != null)
            {
                foreach (RegisterFileExtension _setup in _attributes)
                {
                    if (_setup != null && !string.IsNullOrEmpty(_setup.Extension))
                    {
                        Microsoft.Win32.RegistryKey _key = null;
                        try
                        {
                            _key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey("Media Type\\Extensions\\" + _setup.Extension);
                            if (_key != null)
                            {
                                string _guid = (string)_key.GetValue("Source Filter","");
                                if (_guid == this.GetType().GUID.ToString("B"))
                                {
                                    _key.Close();
                                    _key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey("Media Type\\Extensions");
                                    _key.DeleteSubKeyTree(_setup.Extension);
                                }
                            }
                        }
                        catch (Exception _exception)
                        {
                            HRESULT hr = (HRESULT)Marshal.GetHRForException(_exception);
                            hr.TraceWrite();
                        }
                        finally
                        {
                            if (_key != null)
                            {
                                _key.Close();
                            }
                        }
                    }
                }
            }
            return NOERROR;
        }

        public virtual HRESULT UnregisterMediaType(Type _type)
        {
            Attribute[] _attributes = Attribute.GetCustomAttributes(_type, typeof(RegisterMediaType));
            if (_attributes != null)
            {
                foreach (RegisterMediaType _setup in _attributes)
                {
                    if (_setup != null && !string.IsNullOrEmpty(_setup.Sequence))
                    {
                        Microsoft.Win32.RegistryKey _key = null;
                        try
                        {
                            string _path = "Media Type\\";
                            if (_setup.MajorType != Guid.Empty)
                            {
                                _path += _setup.MajorType.ToString("B");
                            }
                            else
                            {
                                _path += MediaType.Stream.ToString("B");
                            }
                            _key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(_path);
                            if (_key != null)
                            {
                                _key.DeleteSubKeyTree(_setup.SubType.ToString("B"));
                            }
                        }
                        catch (Exception _exception)
                        {
                            HRESULT hr = (HRESULT)Marshal.GetHRForException(_exception);
                            hr.TraceWrite();
                        }
                        finally
                        {
                            if (_key != null)
                            {
                                _key.Close();
                            }
                        }
                    }
                }
            }
            return NOERROR;
        }

        #endregion

        #region Helper Methods

        public bool SelectSeekingPin(SplitterOutputPin pPin)
        {
            lock (m_csSeeking)
            {
                if (m_pSeekingPin == null)
                {
                    m_pSeekingPin = pPin;
                }
                return (m_pSeekingPin == pPin);
            }
        }

        public void DeselectSeekingPin(SplitterOutputPin pPin)
        {
            lock (m_csSeeking)
            {
                if (pPin == m_pSeekingPin)
                {
                    m_pSeekingPin = null;
                }
            }
        }

        public void GetPositions(out long pStart, out long pStop, out double pdRate)
        {
            pStart = m_tStartTime;
            pStop = m_tStopTime;
            pdRate = m_dRate;
        }

        public HRESULT SetPositions(long _start, long _stop, double _rate)
        {
            bool _seek = false;
            {
                if (_start != -1)
                {
                    if (SeekingPin != null)
                    {
                        long _position;
                        SeekingPin.GetCurrentPosition(out _position);
                        if (_position != _start)
                        {
                            _seek = true;
                        }
                    }
                    else
                    {
                        _seek = (_start != m_tStartTime);
                    }
                }
                else
                {
                    _start = m_tStartTime;
                }
                if (_stop != -1)
                {
                    if (m_tStopTime != _stop)
                    {
                        _seek = true;
                    }
                }
                else
                {
                    _stop = m_tStopTime;
                }
                if (_rate > 0 && _rate <= 2.0)
                {
                    if (m_dRate != _rate)
                    {
                        _seek = true;
                    }
                }
                else
                {
                    _rate = m_dRate;
                }
            }
            if (_seek)
            {
                if (IsActive)
                {
                    BeginFlush();

                    StopThread();

                    foreach (BasePin _pin in Pins)
                    {
                        if (_pin.Direction == PinDirection.Output)
                        {
                            (_pin as SplitterOutputPin).StopThread();
                        }
                    }
                    EndFlush();
                }
                lock (m_csSeeking)
                {
                    m_tStartTime = _start;
                    m_tStopTime = _stop;
                    m_dRate = _rate;
                }

                if (!IsActive || (m_pFileParser != null && !m_pFileParser.RequireDemuxThread))
                {
                    m_pFileParser.SeekToTime(m_tStartTime);
                }
                if (IsActive)
                {
                    StartThread();
                    foreach (BasePin _pin in Pins)
                    {
                        if (_pin.Direction == PinDirection.Output)
                        {
                            (_pin as SplitterOutputPin).StartThread();
                        }
                    }
                }
            }
            return NOERROR;
        }

        #endregion

        #region Thread Methods

        protected virtual HRESULT StartThread()
        {
            if (m_pFileParser != null)
            {
                m_evEOS.Reset();
                m_evQuit.Reset();
                if (m_pFileParser.RequireDemuxThread)
                {
                    m_evDemuxStarted.Reset();
                    lock (m_csThreadLock)
                    {
                        if (m_evReady.WaitOne(0,false))
                        {
                            m_evDemuxStarted.Reset();
                            m_evReady.Reset();
                            m_WorkerThread.Create();
                            int nResult = WaitHandle.WaitAny(new WaitHandle[] { m_evDemuxStarted, m_evReady });
                            if (nResult != 0)
                            {
                                m_evQuit.Set();
                                m_WorkerThread.Close();
                                return E_FAIL;
                            }
                        }
                    }
                }
            }
            return NOERROR;
        }

        protected virtual void StopThread()
        {
            if (m_pFileParser != null)
            {
                m_evQuit.Set();
                if (m_pFileParser.RequireDemuxThread)
                {
                    lock (m_csThreadLock)
                    {
                        m_evReady.WaitOne();
                        m_WorkerThread.Close();
                    }
                }
            }
        }

        protected virtual void ThreadProc()
        {
            HRESULT hr;
            m_evReady.Reset();
            m_evEOS.Reset();
            TRACE("Demux Started");
            ASSERT(m_pFileParser != null);
            m_pFileParser.SeekToTime(m_tStartTime);
            hr = m_pFileParser.OnDemuxStart();
            hr.Assert();
            if (hr.Succeeded)
            {
                m_evDemuxStarted.Set();
                do
                {
                    try
                    {
                        hr = m_pFileParser.ProcessDemuxPackets();
                        if (hr != S_OK)
                        {
                            m_evEOS.Set();
                            break;
                        }
                    }
                    catch (Exception _exception)
                    {
                        TRACE("Demux Exception \"" + _exception.Message + "\"");
                        m_evEOS.Set();
#if DEBUG
                        throw _exception;
#else
                        break;
#endif
                    }
                }
                while (!m_evQuit.WaitOne(0,false) && !m_evEOS.WaitOne(0,false));
                m_pFileParser.OnDemuxStop();
                if (!m_evQuit.WaitOne(0,false))
                {
                    while (!m_evQuit.WaitOne(0,false))
                    {
                        long _duration = UNITS;
                        int nCount = 0;
                        foreach (BasePin pPin in Pins)
                        {
                            if (pPin.Direction == PinDirection.Output)
                            {
                                SplitterOutputPin _pin = (pPin as SplitterOutputPin);
                                if (_pin.EOSDelivered || !_pin.IsConnected)
                                {
                                    nCount++;
                                }
                                else
                                {
                                    if (_pin.Track == null || !_pin.Track.Active)
                                    {
                                        nCount++;
                                    }
                                    else
                                    {
                                        if (_pin.Track.Reset())
                                        {
                                            nCount++;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                nCount++;
                            }
                        }
                        if (nCount != Pins.Count)
                        {
                            m_evQuit.WaitOne((int)(_duration / 10000),false);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            m_evReady.Set();
            TRACE("Demux Quit");
        }

        #endregion

        #region IFileSourceFilter Members

        public int Load(string pszFileName, AMMediaType pmt)
        {
            UnloadFile();
            m_bLoading = true;
            m_sFileName = pszFileName;
            try
            {
                if (S_OK == DecideFileParser(m_sFileName))
                {
                    m_bLoading = false;
                    return NOERROR;
                }
            }
            catch
            {
            }
            UnloadFile();
            m_sFileName = pszFileName;

            BitStreamReader _stream = null;
            try
            {
                _stream = new BitStreamReader(new FileStream(m_sFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                if (S_OK == DecideFileParser(_stream))
                {
                    m_bLoading = false;
                    return NOERROR;
                }
            }
            catch 
            {   
            }
            m_bLoading = false;
            UnloadFile();
            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }
            return VFW_E_CANNOT_CONNECT;
        }

        public int GetCurFile(out string pszFileName, AMMediaType pmt)
        {
            pszFileName = null;
            if (string.IsNullOrEmpty(m_sFileName)) return VFW_E_NOT_CONNECTED;

            pszFileName = m_sFileName;
            return NOERROR;
        }

        #endregion

        #region IAMOpenProgress Members

        public int QueryProgress(out long pllTotal, out long pllCurrent)
        {
            pllTotal = 0;
            pllCurrent = 0;
            if (!m_bLoading) return VFW_E_WRONG_STATE;
            if (m_pFileParser != null)
            {
                return m_pFileParser.GetOpeningProgress(out pllTotal, out pllCurrent);
            }
            return NOERROR;
        }

        public int AbortOperation()
        {
            if (!m_bLoading) return VFW_E_WRONG_STATE;
            m_evQuit.Set();
            if (InputPin != null && InputPin.IsConnected)
            {
                FilterGraph.Disconnect((IPin)Marshal.GetObjectForIUnknown(InputPin.Connected.UnknownPtr));
                FilterGraph.Disconnect(InputPin);
            }
            UnloadFile();
            return NOERROR;
        }

        #endregion

        #region IMediaSeeking Members

        public int GetCapabilities(out AMSeekingSeekingCapabilities pCapabilities)
        {
            pCapabilities = AMSeekingSeekingCapabilities.None;
            if (SeekingPin != null)
            {
                return SeekingPin.GetCapabilities(out pCapabilities);
            }
            return E_NOINTERFACE;
        }

        public int CheckCapabilities(ref AMSeekingSeekingCapabilities pCapabilities)
        {
            if (SeekingPin != null)
            {
                return SeekingPin.CheckCapabilities(ref pCapabilities);
            }
            return E_NOINTERFACE;
        }

        public int IsFormatSupported(Guid pFormat)
        {
            if (SeekingPin != null)
            {
                return SeekingPin.IsFormatSupported(pFormat);
            }
            return E_NOINTERFACE;
        }

        public int QueryPreferredFormat(out Guid pFormat)
        {
            pFormat = TimeFormat.None;
            if (SeekingPin != null)
            {
                return SeekingPin.QueryPreferredFormat(out pFormat);
            }
            return E_NOINTERFACE;
        }

        public int GetTimeFormat(out Guid pFormat)
        {
            pFormat = TimeFormat.None;
            if (SeekingPin != null)
            {
                return SeekingPin.GetTimeFormat(out pFormat);
            }
            return E_NOINTERFACE;
        }

        public int IsUsingTimeFormat(Guid pFormat)
        {
            if (SeekingPin != null)
            {
                return SeekingPin.IsUsingTimeFormat(pFormat);
            }
            return E_NOINTERFACE;
        }

        public int SetTimeFormat(Guid pFormat)
        {
            if (SeekingPin != null)
            {
                return SeekingPin.IsUsingTimeFormat(pFormat);
            }
            return E_NOINTERFACE;
        }

        public int GetDuration(out long pDuration)
        {
            pDuration = 0;
            if (m_pFileParser != null)
            {
                pDuration = m_pFileParser.Duration;
                return NOERROR;
            }
            return E_NOINTERFACE;
        }

        public int GetStopPosition(out long pStop)
        {
            pStop = 0;
            if (SeekingPin != null)
            {
                return SeekingPin.GetStopPosition(out pStop);
            }
            return E_NOINTERFACE;
        }

        public int GetCurrentPosition(out long pCurrent)
        {
            pCurrent = 0;
            if (SeekingPin != null)
            {
                return SeekingPin.GetCurrentPosition(out pCurrent);
            }
            return E_NOINTERFACE;
        }

        public int ConvertTimeFormat(out long pTarget, DsGuid pTargetFormat, long Source, DsGuid pSourceFormat)
        {
            pTarget = 0;
            if (SeekingPin != null)
            {
                return SeekingPin.ConvertTimeFormat(out pTarget, pTargetFormat, Source, pSourceFormat);
            }
            return E_NOINTERFACE;
        }

        public int SetPositions(DsLong pCurrent, AMSeekingSeekingFlags dwCurrentFlags, DsLong pStop, AMSeekingSeekingFlags dwStopFlags)
        {
            if (SeekingPin != null)
            {
                return SeekingPin.SetPositions(pCurrent, dwCurrentFlags, pStop, dwStopFlags);
            }
            return E_NOINTERFACE;
        }

        public int GetPositions(out long pCurrent, out long pStop)
        {
            pCurrent = 0;
            pStop = 0;
            if (SeekingPin != null)
            {
                return SeekingPin.GetPositions(out pCurrent, out pStop);
            }
            return E_NOINTERFACE;
        }

        public int GetAvailable(out long pEarliest, out long pLatest)
        {
            pEarliest = 0;
            pLatest = 0;
            if (SeekingPin != null)
            {
                return SeekingPin.GetAvailable(out pEarliest, out pLatest);
            }
            return E_NOINTERFACE;
        }

        public int SetRate(double dRate)
        {
            if (SeekingPin != null)
            {
                return SeekingPin.SetRate(dRate);
            }
            return E_NOINTERFACE;
        }

        public int GetRate(out double pdRate)
        {
            pdRate = 1.0;
            if (SeekingPin != null)
            {
                return SeekingPin.GetRate(out pdRate);
            }
            return E_NOINTERFACE;
        }

        public int GetPreroll(out long pllPreroll)
        {
            pllPreroll = 0;
            if (SeekingPin != null)
            {
                return SeekingPin.GetPreroll(out pllPreroll);
            }
            return E_NOINTERFACE;
        }

        #endregion

        #region IAMStreamSelect Members

        public int Count(out int pcStreams)
        {
            pcStreams = 0;
            if (m_pFileParser == null) return VFW_E_NOT_CONNECTED;
            pcStreams = m_pFileParser.GetTracksCountByType(DemuxTrack.TrackType.Audio);
            return NOERROR;
        }

        public int Info(int lIndex, IntPtr ppmt, IntPtr pdwFlags, IntPtr plcid, IntPtr pdwGroup, IntPtr ppszName, IntPtr ppObject, IntPtr ppUnk)
        {
            SplitterOutputPin _selected = null;
            int _index = lIndex;
            foreach (BasePin _pin in Pins)
            {
                if (_pin.Direction == PinDirection.Output)
                {
                    if ((_pin as SplitterOutputPin).Track.Type == DemuxTrack.TrackType.Audio)
                    {
                        if ((_index-- == 0))
                        {
                            _selected = (_pin as SplitterOutputPin);
                            break;
                        }
                    }
                }
            }
            if (_selected == null) return S_FALSE;
            if (ppmt != IntPtr.Zero)
            {
                bool bHaveType = true;
                AMMediaType mt = new AMMediaType();
                if (_selected.IsConnected)
                {
                    mt.Set(_selected.CurrentMediaType);
                }
                else
                {
                    if (S_OK != _selected.GetMediaType(0, ref mt))
                    {
                        bHaveType = false;
                    }
                    
                }
                if (bHaveType)
                {
                    IntPtr _pmt = Marshal.AllocCoTaskMem(Marshal.SizeOf(mt));
                    Marshal.StructureToPtr(mt, _pmt, true);
                    Marshal.WriteIntPtr(ppmt, _pmt);
                }
                else
                {
                    Marshal.WriteIntPtr(ppmt, IntPtr.Zero);
                }
            }
            if (pdwFlags != IntPtr.Zero)
            {
                if (!_selected.StreamEnabled)
                {
                    Marshal.WriteInt32(pdwFlags, 0);
                }
                else
                {
                    Marshal.WriteInt32(pdwFlags, (int)AMSTREAMSELECTINFOFLAGS.ENABLED);
                }
            }
            if (plcid != IntPtr.Zero)
            {
                int _lcid = _selected.Track.LCID;
                if (_lcid == 0)
                {
                    _lcid = LOCALE_NEUTRAL;
                }
                Marshal.WriteInt32(plcid, _lcid);
            }
            if (pdwGroup != IntPtr.Zero)
            {
                Marshal.WriteInt32(pdwGroup, 0);
            }
            if (ppszName != IntPtr.Zero)
            {
                string _name = _selected.Track.Name;
                if (string.IsNullOrEmpty(_name))
                {
                    _name = "Audio #" + lIndex.ToString();
                }
                Marshal.WriteIntPtr(ppszName, Marshal.StringToCoTaskMemUni(_name));
            }
            if (ppObject != IntPtr.Zero)
            {
                Marshal.WriteIntPtr(ppObject, Marshal.GetIUnknownForObject(_selected));
            }
            if (ppUnk != IntPtr.Zero)
            {
                Marshal.WriteIntPtr(ppUnk, IntPtr.Zero);
            }
            return NOERROR;
        }

        public int Enable(int lIndex, AMSTREAMSELECTENABLEFLAGS dwFlags)
        {
            if (dwFlags == AMSTREAMSELECTENABLEFLAGS.DISABLE)
            {
                foreach (BasePin _pin in Pins)
                {
                    if (_pin.Direction == PinDirection.Output)
                    {
                        if ((_pin as SplitterOutputPin).Track.Type == DemuxTrack.TrackType.Audio)
                        {
                            (_pin as SplitterOutputPin).StreamEnabled = false;
                        }
                    }
                }
            }
            if (dwFlags == AMSTREAMSELECTENABLEFLAGS.ENABLEALL)
            {
                foreach (BasePin _pin in Pins)
                {
                    if (_pin.Direction == PinDirection.Output)
                    {
                        if ((_pin as SplitterOutputPin).Track.Type == DemuxTrack.TrackType.Audio)
                        {
                            (_pin as SplitterOutputPin).StreamEnabled = true;
                        }
                    }
                }
            }
            if (dwFlags == AMSTREAMSELECTENABLEFLAGS.ENABLED)
            {
                foreach (BasePin _pin in Pins)
                {
                    if (_pin.Direction == PinDirection.Output)
                    {
                        if ((_pin as SplitterOutputPin).Track.Type == DemuxTrack.TrackType.Audio)
                        {
                            (_pin as SplitterOutputPin).StreamEnabled = (lIndex-- == 0);
                        }
                    }
                }
            }
            if (IsActive && dwFlags != AMSTREAMSELECTENABLEFLAGS.DISABLE)
            {
                try
                {
                    IMediaSeeking _seeking = (IMediaSeeking)FilterGraph;
                    if (_seeking != null)
                    {
                        long _current;
                        _seeking.GetCurrentPosition(out _current);
                        _current -= UNITS / 10;
                        _seeking.SetPositions(_current, AMSeekingSeekingFlags.AbsolutePositioning, null, AMSeekingSeekingFlags.NoPositioning);
                        _current += UNITS / 10;
                        _seeking.SetPositions(_current, AMSeekingSeekingFlags.AbsolutePositioning, null, AMSeekingSeekingFlags.NoPositioning);
                    }
                }
                catch
                {
                }
            }
            return NOERROR;
        }

        #endregion
    }

    #endregion

    #region Base Classes

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class BaseSplitterFilter : BaseSplitter
    {
        #region Constructor

        protected BaseSplitterFilter(string _name, FileParser _parser)
            : base(_name)
        {
            if (_parser != null)
            {
                m_Parsers.Add(_parser);
            }
            AddPin(new SplitterInputPin("Input",this));
        }

        #endregion
    }

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class BaseFileSourceFilter : BaseSplitter, IFileSourceFilter, IAMOpenProgress
    {
        #region Constructor

        protected BaseFileSourceFilter(string _name, FileParser _parser)
            : base(_name)
        {
            if (_parser != null)
            {
                m_Parsers.Add(_parser);
            }
        }

        #endregion
    }

    #endregion

    #region Generic Templates

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class BaseSplitterFilterTemplate<Parser> : BaseSplitterFilter where Parser : FileParser, new()
    {
        public BaseSplitterFilterTemplate(string _name)
            : base (_name,new Parser())
        {
        }
    }

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class BaseSourceFilterTemplate<Parser> : BaseFileSourceFilter where Parser : FileParser, new()
    {
        public BaseSourceFilterTemplate(string _name)
            : base(_name, new Parser())
        {
        }
    }

    #endregion

    #endregion

    #region Base Muxer Filter

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class BitStreamWriter : COMHelper, IDisposable
    {
        #region Variables

        protected Stream m_Stream = null;
        protected object m_csWritingLock = new object();
        protected int m_nBitIndex = 0;
        protected byte[] m_btCurrentByte = new byte[1];
        protected long m_llPosition = 0;

        #endregion

        #region Constructor

        public BitStreamWriter(Stream _stream)
        {
            m_Stream = _stream;
        }

        ~BitStreamWriter()
        {
            Dispose();
        }

        #endregion
        /*
        #region Properties

        public long Position
        {
            get
            {
                lock (m_csCacheLock)
                {
                    return (m_llPosition - (m_nBufferSize > 0 ? (m_nBufferSize - m_nCacheIndex) : 0));
                }
            }
            set { Seek(value); }
        }

        public long TotalSize
        {
            get { return m_Stream.Length; }
        }

        #endregion

        #region Methods

        public HRESULT Seek(long ullPosition)
        {
            lock (m_csCacheLock)
            {
                if (ullPosition < m_llPosition && m_llPosition - (long)m_nBufferSize < ullPosition)
                {
                    m_nCacheIndex = m_nBufferSize - (int)(m_llPosition - ullPosition);
                }
                else
                {
                    m_llPosition = ullPosition;
                    m_nBufferSize = 0;
                    m_nCacheIndex = 0;
                }
                m_nBitIndex = 0;
                if (ullPosition > TotalSize) return S_FALSE;
            }
            return NOERROR;
        }

        #endregion

        #region Bits Helper

        public bool IsAligned() { return m_nBitIndex == 0; }
        public void AlignNextByte() { if (m_nBitIndex != 0) WriteBits(m_nBitIndex,0); }
        public void WriteBit(bool bBit) { WriteBit(bBit ? 1 : 0); }
        public void WriteBit(int nBit)
        {
            /*
            if (m_nBitIndex == 0)
            {
                int _readed = 0;
                HRESULT hr = WriteData(m_btCurrentByte, 1, out _readed);
                if (_readed == 0) m_btCurrentByte[0] = 0;
                m_nBitIndex = 8;
            }
            m_nBitIndex--;
            return (m_btCurrentByte[0] >> m_nBitIndex) & 0x01;
            //* /
        }
        public void WriteByte(int _byte) { WriteBits(8, _byte); }
        public void WriteWord(int _word) { WriteBits(16, _word); }
        public void WriteDword(int _dword) { WriteBits(32, _dword); }
        public void WriteQword(long _qword) { WriteBits(64, _qword); }
        public void WriteBits(int nCount, long _value)
        {
            long u = (_value << (Marshal.SizeOf(_value) * 8 - nCount));
            long mask = (((long)0x01) << (Marshal.SizeOf(_value) * 8 - 1));
            while (nCount-- > 0)
            {
                bool _bit = ((u & mask) == mask);
                WriteBit(_bit);
                u <<= 0x01;
            }
        }
        
        #endregion
        */
        #region Value Helper

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (m_Stream != null)
            {
                m_Stream.Dispose();
                m_Stream = null;
            }
        }

        #endregion
    }

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class MuxerTrack : COMHelper, IDisposable
    {
        #region Enums

        [ComVisible(false)]
        public enum TrackType
        {
            Unknown = -1,
            Video = 0,
            Audio = 1,
            SubPicture = 2,
            Subtitles = 3,
        }

        #endregion

        #region Variables

        protected PacketsQueue m_Queue = new PacketsQueue();
        protected FileWriter m_pWriter = null;
        protected TrackType m_Type = TrackType.Unknown;
        protected long m_rtPosition = 0;
        protected EventWaitHandle m_hFlush = null;
        protected ManualResetEvent m_evReady = new ManualResetEvent(false);
        protected bool m_bFirstSample = true;
        protected AMMediaType m_mt = new AMMediaType();
        protected bool m_bEOS = true;
        protected object m_csPostionLock = new object();

        #endregion

        #region Constructor

        protected MuxerTrack(FileWriter _writer, AMMediaType mt)
        {
            m_pWriter = _writer;
            m_mt = mt;
        }

        #endregion

        #region Properties

        public TrackType Type
        {
            get { return m_Type; }
        }

        public bool IsStartSample
        {
            get { return m_bFirstSample; }
        }

        public long Position
        {
            get 
            {
                long _position;
                lock (m_csPostionLock)
                {
                    _position = m_rtPosition;
                }
                return _position;
            }
        }

        public AMMediaType CurrentMediaType
        {
            get { return m_mt; }
            set { m_mt.Set(value); }
        }

        public long Allocated
        {
            get
            {
                long _time = m_Queue.CacheDuration;
                if (_time > 0) return _time;
                return 0;
            }
        }

        public EventWaitHandle FlushEvent
        {
            get { return m_hFlush; }
            set { m_hFlush = value; }
        }

        public bool IsWaiting
        {
            get { return !m_evReady.WaitOne(0,false) && m_Queue.IsEmpty; }
        }

        public bool IsEOS
        {
            get { return m_bEOS; }
            set { m_bEOS = true; }
        }

        #endregion

        #region Helper Methods

        public virtual bool Reset()
        {
            if (IsWaiting)
            {
                m_evReady.Set();
                return true;
            }
            return false;
        }

        public virtual void Alloc(long _time)
        {
            m_Queue.CacheDuration = _time;
        }

        public virtual void Flush()
        {
            m_Queue.Clear();
            m_evReady.Reset();
            lock (m_csPostionLock)
            {
                m_rtPosition = 0;
            }
            m_bEOS = false;
            m_bFirstSample = true;
        }

        public virtual bool AddToCache(ref PacketData pPacket)
        {
            if (pPacket != null)
            {
                if (m_Queue.IsFull && pPacket.Start >= m_Queue.StopTime)
                {
                    if (0 != WaitHandle.WaitAny(new WaitHandle[] { m_Queue, m_pWriter.QuitEvent, m_hFlush }))
                    {
                        pPacket.Dispose();
                        return false;
                    }
                }
                m_Queue.Add(pPacket);
                m_evReady.Set();
                return true;
            }
            return false;
        }

        public virtual PacketData GetNextPacket()
        {
            while (true)
            {
                WaitHandle.WaitAny(new WaitHandle[] { m_evReady, m_pWriter.QuitEvent, m_hFlush });
                if (m_evReady.WaitOne(0,false))
                {
                    PacketData pPacket;
                    if (m_Queue.Peek(out pPacket, true))
                    {
                        return pPacket;
                    }
                    else
                    {
                        if (IsEOS) break;
                    }
                    {
                        if (m_Queue.IsEmpty)
                        {
                            m_evReady.Reset();
                        }
                    }
                }
                else
                {
                    break;
                }
            }
            return null;
        }

        public virtual HRESULT Receive(ref IMediaSampleImpl _sample)
        {
            PacketData _packet = new PacketData();
            _packet.SyncPoint = _sample.IsSyncPoint() == S_OK;
            long _start,_stop;
            _sample.GetTime(out _start, out _stop);
            _packet.Start = _start;
            _packet.Stop = _stop;
            _packet.Size = _sample.GetActualDataLength();
            _packet.Buffer = new byte[_packet.Size];
            IntPtr pBuffer;
            _sample.GetPointer(out pBuffer);
            Marshal.Copy(pBuffer, _packet.Buffer, 0, _packet.Size);
            AddToCache(ref _packet);
            return NOERROR;
        }

        #endregion

        #region Methods To Override

        public virtual HRESULT OnStartWriting()
        {
            return NOERROR;
        }

        public virtual HRESULT OnStopWriting()
        {
            return NOERROR;
        }

        public virtual HRESULT WritePacket(PacketData _packet)
        {
            if (_packet.Start > 0)
            {
                lock (m_csPostionLock)
                {
                    m_rtPosition = _packet.Start;
                }
            }
            m_bFirstSample = false;
            return NOERROR;
        }

        #endregion

        #region Abstract Methods

        public abstract HRESULT InitTrack();

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            m_mt.Free();
        }

        #endregion
    }

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class FileWriter : COMHelper, IDisposable
    {
        #region Variables

        protected BitStreamWriter m_Stream = null;
        protected List<MuxerTrack> m_Tracks = new List<MuxerTrack>();
        private bool m_bRequireMuxThread = false;
        protected string m_sFileName = "";
        protected EventWaitHandle m_hQuit = null;
        protected bool m_bThreadExists = false;
        protected bool m_bRealtimeSync = false;
        protected bool m_bAppendOnSeek = true;
        protected bool m_bOverwrite = true;

        #endregion

        #region Constructor

        protected FileWriter()
            : this(true)
        {

        }

        protected FileWriter(bool bRequireMuxThread)
        {
            m_bRequireMuxThread = bRequireMuxThread;
        }

	    ~FileWriter()
        {
            Dispose();
        }

        #endregion

        #region Properties

        public string FileName
        {
            get { return m_sFileName; }
        }

        public MuxerTrack this[int iIndex]
        {
            get { return m_Tracks[iIndex]; }
        }

        public bool RequireMuxThread
        {
            get { return m_bRequireMuxThread; }
            set { m_bRequireMuxThread = value; }
        }

        public int Count
        {
            get { return m_Tracks.Count; }
        }

        public long Position
        {
            get
            {
                long _time = MAX_LONG;
                foreach (MuxerTrack _track in m_Tracks)
                {
                    long _position = _track.Position;
                    if (_position > 0 && _position < _time)
                    {
                        _time = _position;
                    }
                }
                return _time == MAX_LONG ? 0 : _time;
            }
        }

        public EventWaitHandle QuitEvent
        {
            get { return m_hQuit; }
            set { m_hQuit = value; }
        }

        public BitStreamWriter Stream
        {
            get { return m_Stream; }
        }

        public bool ThreadExists
        {
            get { return m_bThreadExists; }
            set { m_bThreadExists = value; }
        }

        public bool RealtimeSync
        {
            get { return m_bRealtimeSync; }
        }

        public bool AppendOnSeek
        {
            get { return m_bAppendOnSeek; }
        }

        #endregion

        #region Virtual Methods

        public virtual HRESULT OpenOutput(string sFileName, bool bOverwrite)
        {
            m_sFileName = sFileName;
            m_bOverwrite = bOverwrite;
            HRESULT hr = OpenFile();
            if (hr == S_OK) return hr;
            FileStream _stream = new FileStream(sFileName, bOverwrite ? FileMode.Create : FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            return OpenOutput(new BitStreamWriter(_stream), bOverwrite);
        }

        public virtual HRESULT OpenOutput(BitStreamWriter _stream, bool bOverwrite)
        {
            m_Stream = _stream;
            m_bOverwrite = bOverwrite;
            return OpenStream();
        }

        public virtual HRESULT CloseOutput()
        {
            if (m_Stream != null)
            {
                m_Stream.Dispose();
                m_Stream = null;
            }
            return NOERROR;
        }

        public virtual HRESULT CheckInputType(AMMediaType pmt)
        {
            if (pmt == null) return E_POINTER;
            MuxerTrack _track = CreateTrackForType(pmt);
            if (_track == null) return VFW_E_CANNOT_CONNECT;
            HRESULT hr = _track.InitTrack();
            RemoveTrack(_track);
            return hr;
        }

        public virtual MuxerTrack CreateTrackForType(AMMediaType pmt)
        {
            MuxerTrack _track = CreateTrack(pmt);
            if (_track != null)
            {
                m_Tracks.Add(_track);
            }
            return _track;
        }

        public virtual HRESULT RemoveTrack(MuxerTrack _track)
        {
            if (_track != null)
            {
                _track.Dispose();
                if (m_Tracks.Remove(_track))
                {
                    return S_OK;
                }
            }
            return S_FALSE;
        }

        public virtual int GetMaxTrackCount()
        {
            return -1;
        }

        public virtual HRESULT GetMediaType(int iPosition, ref AMMediaType pmt)
        {
            if (iPosition < 0) return E_INVALIDARG;
            if (iPosition > 0) return VFW_S_NO_MORE_ITEMS;
            pmt.majorType = MediaType.Stream;
            pmt.subType = MediaSubType.None;
            pmt.formatType = FormatType.None;

            return NOERROR;
        }

        public virtual HRESULT SetMediaType(AMMediaType pmt)
        {
            if (pmt == null) return E_POINTER;
            if (pmt.majorType != MediaType.Stream) return VFW_E_CANNOT_CONNECT;

            return NOERROR;
        }

        #endregion

        #region Helper Methods

        public MuxerTrack GetTrackByType(MuxerTrack.TrackType _type)
        {
            foreach (MuxerTrack _track in m_Tracks)
            {
                if (_track.Type == _type)
                {
                    return _track;
                }
            }
            return null;
        }

        public bool HaveTrack(MuxerTrack.TrackType _type)
        {
            return GetTrackByType(_type) != null;
        }

        public int GetTracksCountByType(MuxerTrack.TrackType _type)
        {
            int nCount = 0;
            foreach (MuxerTrack _track in m_Tracks)
            {
                if (_track.Type == _type)
                {
                    nCount++;
                }
            }
            return nCount;
        }

        #endregion

        #region Writer Methods

        public virtual HRESULT OnStartWriting()
        {
            foreach (MuxerTrack _track in m_Tracks)
            {
                _track.OnStartWriting();
            }
            return NOERROR;
        }

        public virtual HRESULT OnStopWriting()
        {
            foreach (MuxerTrack _track in m_Tracks)
            {
                _track.OnStopWriting();
            }
            return NOERROR;
        }

        public virtual HRESULT WritePacket(MuxerTrack _track, PacketData _packet)
        {
            return _track.WritePacket(_packet);
        }

        public virtual HRESULT OpenFile()
        {
            return E_NOTIMPL;
        }

        public virtual HRESULT OpenStream()
        {
            return E_NOTIMPL;
        }

        #endregion 

        #region Abstract Methods

        protected abstract MuxerTrack CreateTrack(AMMediaType pmt);

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            CloseOutput();
            while (m_Tracks.Count > 0)
            {
                RemoveTrack(m_Tracks[0]);
            }
        }

        #endregion
    }

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class MuxerInputPin : BaseInputPin
    {
        #region Variables

        protected MuxerTrack m_pTrack = null;
        protected bool m_bEOSDelivered = false;
        protected RendererPosPassThru m_pPosition = null;
        protected long m_rtLastSampleTime = -1;
        protected long m_rtStartPosition = 0;
        protected long m_rtWritingPosition = 0;
        protected long m_lSampleSize = 0;
        protected ManualResetEvent m_evFlush = new ManualResetEvent(false);
        protected ManualResetEvent m_evRenderTime = new ManualResetEvent(true);
        protected IReferenceClockImpl m_pClock = null;
        protected object m_csTrackLock = new object();

        #endregion

        #region Constructor

        public MuxerInputPin(string _name,BaseMuxer _filter)
            : base(_name,_filter)
        {

        }

        #endregion

        #region Properties

        public RendererPosPassThru Position
        {
            get
            {
                if (m_pPosition == null)
                {
                    lock (m_Lock)
                    {
                        HRESULT hr = NOERROR;
                        IntPtr _pin = Marshal.GetComInterfaceForObject(this, typeof(IPin));
                        m_pPosition = new RendererPosPassThru(ref hr, _pin);
                        Marshal.Release(_pin);
                        if (FAILED(hr))
                        {
                            m_pPosition = null;
                        }
                    }
                }
                return m_pPosition;
            }
        }

        public bool EOSDelivered
        {
            get { return m_bEOSDelivered; }
        }

        public MuxerTrack Track
        {
            get { return m_pTrack; }
        }

        #endregion

        #region Overridden Methods

        public override int Active()
        {
            RendererPosPassThru _position = Position;
            if (_position != null)
            {
                _position.ResetMediaTime();
            }
            m_evFlush.Reset();
            m_bEOSDelivered = false;
            if (m_pTrack != null)
            {
                m_rtLastSampleTime = 0;
                m_rtWritingPosition = 0;
                m_rtStartPosition = 0;
                m_evRenderTime.Reset();
                m_pTrack.Flush();
                if ((m_Filter as BaseMuxer).Writer.RealtimeSync)
                {
                    lock (m_Filter.FilterLock)
                    {
                        m_pClock = m_Filter.Clock;
                        if (m_pClock.IsValid)
                        {
                            m_pClock._AddRef();
                        }
                        else
                        {
                            m_pClock = null;
                        }
                    }
                }
            }
            else
            {
                m_bEOSDelivered = true;
            }
            return base.Active();
        }

        public override int Inactive()
        {
            int hr = base.Inactive();
            m_bEOSDelivered = true;
            m_evRenderTime.Set();
            if (m_pTrack != null)
            {
                m_pTrack.Flush();
            }
            if (m_pClock != null)
            {
                m_pClock._Release();
                m_pClock = null;
            }
            return hr;
        }

        public override int EndOfStream()
        {
            m_bEOSDelivered = true;
            int hr = base.EndOfStream();
            RendererPosPassThru _position = Position;
            if (_position != null)
            {
                _position.EOS();
            }
            lock (m_csTrackLock)
            {
                m_pTrack.IsEOS = true;
            }
            (m_Filter as BaseMuxer).EndOfStream();
            return hr;
        }

        public override int CheckMediaType(AMMediaType pmt)
        {
            if (S_OK != (m_Filter as BaseMuxer).Writer.CheckInputType(pmt))
            {
                return VFW_E_INVALIDMEDIATYPE;
            }
            return NOERROR;
        }

        public override int CompleteConnect(ref IPinImpl pReceivePin)
        {
            HRESULT hr = (HRESULT )base.CompleteConnect(ref pReceivePin);
            if (hr.Failed) return hr;
            lock (m_csTrackLock)
            {
                if (m_pTrack != null)
                {
                    (m_Filter as BaseMuxer).Writer.RemoveTrack(m_pTrack);
                }
                m_pTrack = (m_Filter as BaseMuxer).Writer.CreateTrackForType(m_mt);
                if (m_pTrack == null) return VFW_E_CANNOT_CONNECT;
                m_pTrack.Alloc(2 * UNITS);
                m_pTrack.FlushEvent = m_evFlush;
                hr = (m_Filter as BaseMuxer).CompleteConnect(this);
                if (hr.Failed)
                {
                    (m_Filter as BaseMuxer).Writer.RemoveTrack(m_pTrack);
                    m_pTrack = null;
                }
            }
            return hr;
        }

        public override int BreakConnect()
        {
            HRESULT hr = (HRESULT)base.BreakConnect();
            lock (m_csTrackLock)
            {
                (m_Filter as BaseMuxer).Writer.RemoveTrack(m_pTrack);
                m_pTrack = null;
            }
            return hr;
        }

        public override int BeginFlush()
        {
            int hr = base.BeginFlush();
            m_evRenderTime.Set();
            m_evFlush.Set();
            if ((m_Filter as BaseMuxer).SeekingPin == this)
            {
                (m_Filter as BaseMuxer).BeginFlush();
            }
            return hr;
        }

        public override int EndFlush()
        {
            RendererPosPassThru _position = Position;
            if (_position != null)
            {
                _position.ResetMediaTime();
            }

            if ((m_Filter as BaseMuxer).Writer.AppendOnSeek)
            {
                lock (m_csTrackLock)
                {
                    m_rtWritingPosition += m_pTrack.Position;
                }
            }
            if ((m_Filter as BaseMuxer).SeekingPin == this)
            {
                (m_Filter as BaseMuxer).EndFlush();
            }
            lock (m_csTrackLock)
            {
                m_pTrack.Flush();
                m_rtStartPosition = 0;
                m_evFlush.Reset();
            }
            return base.EndFlush();
        }

        public override int OnReceive(ref IMediaSampleImpl _sample)
        {
            HRESULT hr = (HRESULT)CheckStreaming();
            if (hr == S_OK && !m_bEOSDelivered && !m_evFlush.WaitOne(0,false))
            {
                hr = (HRESULT)base.OnReceive(ref _sample);
		        if (hr != S_OK) return hr;
                long _start, _stop;
                hr = (HRESULT)_sample.GetTime(out _start, out _stop);
                int lSize = _sample.GetActualDataLength();
                if (hr != S_OK)
                {
                    if (hr != VFW_S_NO_STOP_TIME)
                    {
                        _start = m_rtStartPosition;
                    }
                    if (m_mt.majorType == MediaType.Audio)
                    {
                        if (m_lSampleSize != 0)
                        {
                            _stop = _start + m_rtLastSampleTime * lSize / m_lSampleSize;
                        }
                        else
                        {
                            _stop = _start + m_rtLastSampleTime;
                        }
                    }
                    if (m_mt.subType == MediaType.Video)
                    {
                        if (m_rtLastSampleTime == 0)
                        {
                            _stop = _start + m_rtLastSampleTime;
                        }
                        else
                        {
                            _stop = _start + m_mt.GetFrameRate();
                        }
                    }
                }
                else
                {
                    m_lSampleSize = lSize;
                    m_rtLastSampleTime = _stop - _start;
                }
                if (m_pClock != null)
                {
                    m_evRenderTime.Reset();
                    int dwAdvise = 0;
                    hr = (HRESULT)m_pClock.AdviseTime(
                        (m_Filter as BaseMuxer).StartTime,		// Start run time
                        _start,								    // Stream time
                        m_evRenderTime.SafeWaitHandle.DangerousGetHandle(),	                // Render notification
                        out dwAdvise);
                    if (SUCCEEDED(hr))
                    {
                        int dwResult = WaitHandle.WaitAny(new WaitHandle[] { m_evRenderTime, (m_Filter as BaseMuxer).Writer.QuitEvent });
                        m_pClock.Unadvise(dwAdvise);
                        if (dwResult != 0)
                        {
                            return S_FALSE;
                        }
                    }
                }
                if (m_pPosition != null)
                {
                    m_pPosition.RegisterMediaTime(_start, _stop);
                }
                lock (m_csTrackLock)
                {
                    m_rtStartPosition = _stop;
                    _start += m_rtWritingPosition;
                    _stop += m_rtWritingPosition;
                    _sample.SetTime(_start, _stop);
                    hr = m_pTrack.Receive(ref _sample);
                }
            }
            return hr;
        }

        #endregion
    }

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class MuxerOutputPin : BaseOutputPin
    {
        #region Constructor

        public MuxerOutputPin(BaseMuxer _filter)
            : base("Output",_filter)
        {

        }

        #endregion

        #region Overridden Methods

        public override int CheckMediaType(AMMediaType pmt)
        {
            return NOERROR;
        }

        public override int SetMediaType(AMMediaType mt)
        {
            int hr = base.SetMediaType(mt);
            if (FAILED(hr)) return hr;
            hr = (m_Filter as BaseMuxer).SetMediaType(mt);
            return hr;
        }

        public override int GetMediaType(int iPosition, ref AMMediaType pMediaType)
        {
            return (m_Filter as BaseMuxer).GetMediaType(iPosition, ref pMediaType);
        }

        public override int CompleteConnect(ref IPinImpl pReceivePin)
        {
            return base.CompleteConnect(ref pReceivePin);
        }

        public override int DecideBufferSize(ref IMemAllocatorImpl pAlloc, ref AllocatorProperties prop)
        {
            if (pAlloc == null) return E_POINTER;
            if (prop == null) return E_POINTER;

            prop.cbPrefix = 0;
            prop.cbAlign = 1;
            prop.cBuffers = 3;
            prop.cbBuffer = 1;

            foreach (BasePin _pin in m_Filter.Pins)
            {
                if (_pin.Direction == PinDirection.Input && _pin.IsConnected)
                {
                    AllocatorProperties _allocated = new AllocatorProperties();
                    if (S_OK == _pin.Allocator.GetProperties(_allocated))
                    {
                        if (_allocated.cbBuffer > prop.cbBuffer)
                        {
                            prop.cbBuffer = _allocated.cbBuffer;
                        }
                    }
                }
            }
            AllocatorProperties _actual = new AllocatorProperties();
            int hr = pAlloc.SetProperties(prop, _actual);
            if (FAILED(hr))
            {
                prop.cbBuffer = 1024;
                hr = pAlloc.SetProperties(prop, _actual);
            }
            return hr;
        }

        #endregion
    }

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class BaseMuxer : BaseFilter, IMediaSeeking, IAMFilterMiscFlags
    {
        #region Variables

        protected FileWriter m_pFileWriter = null;
        protected string m_sFileName = "";
        protected bool m_bOverwrite = true;
        protected MuxerInputPin m_pSeekingPin = null;
        protected bool m_bIsOpened = false;
        protected object m_csThreadLock = new object();
        protected AMThread m_WorkerThread = null;
        protected ManualResetEvent m_evQuit = new ManualResetEvent(false);
        protected ManualResetEvent m_evReady = new ManualResetEvent(true);
        protected AutoResetEvent m_evMuxStarted = new AutoResetEvent(false);

        #endregion

        #region Constructor

        protected BaseMuxer(string _name, FileWriter _writer)
            : base(_name)
        {
            m_pFileWriter = _writer;
            m_pFileWriter.QuitEvent = m_evQuit;
            m_WorkerThread = new ManagedThread(this.ThreadProc);
        }

        #endregion

        #region Properties

        public MuxerOutputPin OutputPin
        {
            get 
            {
                foreach (BasePin _pin in Pins)
                {
                    if (_pin.Direction == PinDirection.Output)
                    {
                        return (_pin as MuxerOutputPin);
                    }
                }
                return null;
            }
        }

        public FileWriter Writer
        {
            get { return m_pFileWriter; }
        }

        public MuxerInputPin SeekingPin
        {
            get
            {
                if (m_pSeekingPin == null)
                {
                    foreach (BasePin _pin in Pins)
                    {
                        if (_pin.Direction == PinDirection.Input && _pin.IsConnected)
                        {
                            if ((_pin as MuxerInputPin).Position != null)
                            {
                                m_pSeekingPin = (_pin as MuxerInputPin);
                                break;
                            }
                        }
                    }
                }
                return m_pSeekingPin;
            }
        }

        public bool ThreadExists
        {
            get { return m_WorkerThread.ThreadExists; }
        }

        public long StartTime
        {
            get { return m_tStart; }
        }

        #endregion

        #region Overridden Methods

        protected override int OnInitializePins()
        {
            AddPin(CreateNewInputPin());
            return NOERROR;
        }

        public override int Pause()
        {
            if (m_State == FilterState.Stopped)
            {
                m_pFileWriter.ThreadExists = false;
                HRESULT hr;
                if (m_pFileWriter.RequireMuxThread)
                {
                    hr = StartThread();
                }
                else
                {
                    hr = OpenFile();
                }
                if (hr.Failed) return hr;
            }
            return base.Pause();
        }

        public override int Stop()
        {
            HRESULT hr = (HRESULT)base.Stop();
            StopThread();
            CloseFile();
            m_pSeekingPin = null;
            return hr;
        }

        #endregion

        #region Virtual Methods

        public virtual HRESULT BeginFlush()
        {
            MuxerOutputPin _pin = OutputPin;
            if (_pin != null)
            {
                _pin.DeliverBeginFlush();
            }
            if (!m_pFileWriter.AppendOnSeek && !m_bOverwrite)
            {
                StopThread();
                CloseFile();
            }
            return NOERROR;
        }

        public virtual HRESULT EndFlush()
        {
            MuxerOutputPin _pin = OutputPin;
            if (_pin != null)
            {
                _pin.DeliverEndFlush();
            }
            if (!m_pFileWriter.AppendOnSeek && !m_bOverwrite)
            {
                m_pFileWriter.ThreadExists = false;
                HRESULT hr;
                if (m_pFileWriter.RequireMuxThread)
                {
                    hr = StartThread();
                }
                else
                {
                    hr = OpenFile();
                }
                return hr;
            }
            return NOERROR;
        }

        public virtual HRESULT EndOfStream()
        {
            if (m_State == FilterState.Stopped)
            {
                return NOERROR;
            }
            int nEOS = 0;
            foreach (BasePin _pin in Pins)
            {
                if (_pin.Direction == PinDirection.Input && _pin.IsConnected)
                {
                    if ((_pin as MuxerInputPin).EOSDelivered) nEOS++;
                }
            }
            if (m_pFileWriter.Count == nEOS)
            {
                MuxerOutputPin _pin = OutputPin;
                if (_pin != null)
                {
                    _pin.DeliverEndOfStream();
                }
                StopThread();
                CloseFile();
                NotifyEvent(EventCode.Complete, (IntPtr)((int)S_OK), Marshal.GetIUnknownForObject(this));
            }
            return NOERROR;
        }

        public virtual HRESULT SetMediaType(AMMediaType pmt)
        {
            if (m_pFileWriter != null)
            {
                return m_pFileWriter.SetMediaType(pmt);
            }
            return NOERROR;
        }

        public virtual HRESULT GetMediaType(int iPosition, ref AMMediaType pmt)
        {
            if (m_pFileWriter != null)
            {
                return m_pFileWriter.GetMediaType(iPosition,ref pmt);
            }
            return E_NOTIMPL;
        }

        public virtual HRESULT CompleteConnect(MuxerInputPin _pin)
        {
            int nConnected = 0;
            int nCount = 0;
            foreach (BasePin _existingPin in Pins)
            {
                if (_existingPin.Direction == PinDirection.Input)
                {
                    nCount++;
                    if (_existingPin.IsConnected)
                    {
                        nConnected++;
                    }
                }
            }
            if (nCount == nConnected)
            {
                int _max = m_pFileWriter.GetMaxTrackCount();
                if (_max > 0)
                {
                    if (nCount >= _max)
                    {
                        return NOERROR;
                    }
                }
                AddPin(CreateNewInputPin());
            }
            return NOERROR;
        }

        protected virtual MuxerInputPin CreateNewInputPin()
        {
            int nIndex = 1;
            foreach (BasePin _pin in Pins)
            {
                if (_pin.Direction == PinDirection.Input) nIndex++;
            }
            return new MuxerInputPin("Input " + nIndex.ToString(), this);
        }

        #endregion

        #region Methods

        protected HRESULT OpenFile()
        {
            if (!m_bIsOpened)
            {
                HRESULT hr = E_FAIL;
                if (hr.Failed)
                {
                    MuxerOutputPin _output = OutputPin;
                    if (_output != null)
                    {
                        Guid _guid = typeof(IStream).GUID;
                        IntPtr pStream;
                        hr = _output.Connected._QueryInterface(ref _guid, out pStream);
                        if (hr.Succeeded)
                        {
                            BitStreamWriter _writer = new BitStreamWriter(new COMStream(pStream));
                            try
                            {
                                hr = m_pFileWriter.OpenOutput(_writer,m_bOverwrite);
                            }
                            catch (Exception _exception)
                            {
                                hr = (HRESULT)Marshal.GetHRForException(_exception);
                                _writer.Dispose();
                            }
                            finally
                            {
                                Marshal.Release(pStream);
                            }
                            if (hr != S_OK)
                            {
                                m_pFileWriter.CloseOutput();
                            }
                        }
                    }
                }
                if (hr.Failed)
                {
                    if (!String.IsNullOrEmpty(m_sFileName))
                    {
                        try
                        {
                            hr = m_pFileWriter.OpenOutput(m_sFileName, m_bOverwrite);
                        }
                        catch (Exception _exception)
                        {
                            hr = (HRESULT)Marshal.GetHRForException(_exception);
                        }
                        if (hr != S_OK)
                        {
                            m_pFileWriter.CloseOutput();
                        }
                    }
                }
                if (hr == S_OK)
                {
                    hr = m_pFileWriter.OnStartWriting();
                    if (hr != S_OK)
                    {
                        m_pFileWriter.CloseOutput();
                    }
                }
                m_bIsOpened = (hr == S_OK);
                return hr;
            }
            return NOERROR;
        }

        protected HRESULT CloseFile()
        {
            if (m_bIsOpened)
            {
                m_pFileWriter.OnStopWriting();
                m_pFileWriter.CloseOutput();
                m_bIsOpened = false;
            }
            return NOERROR;
        }

        #endregion

        #region Thread Methods

        protected virtual HRESULT StartThread()
        {
            m_evQuit.Reset();
            if (m_pFileWriter.RequireMuxThread)
            {
                m_evMuxStarted.Reset();
                lock (m_csThreadLock)
                {
                    if (m_evReady.WaitOne(0,false))
                    {
                        m_evMuxStarted.Reset();
                        m_evReady.Reset();
                        m_WorkerThread.Create();
                        int nResult = WaitHandle.WaitAny(new WaitHandle[] { m_evMuxStarted, m_evReady });
                        if (nResult != 0)
                        {
                            m_evQuit.Set();
                            m_WorkerThread.Close();
                            return E_FAIL;
                        }
                    }
                }
            }
            return NOERROR;
        }

        protected virtual void StopThread()
        {
            m_evQuit.Set();
            if (m_WorkerThread.ThreadExists)
            {
                lock (m_csThreadLock)
                {
                    m_evReady.WaitOne();
                    m_WorkerThread.Close();
                }
            }
        }

        protected virtual void ThreadProc()
        {
            HRESULT hr;
            m_evReady.Reset();
            TRACE("Mux Started");
            ASSERT(m_pFileWriter != null);
            m_pFileWriter.ThreadExists = true;
            hr = OpenFile();
            if (hr.Succeeded)
            {
                m_evMuxStarted.Set();

                bool bFlush = false;
                long _minPosition;
                long _maxPosition;
                while (!bFlush)
                {
                    bFlush = m_evQuit.WaitOne(5,false);
                    if (!bFlush)
                    {
                        _minPosition = MAX_LONG;
                        _maxPosition = 0;
                        foreach (BasePin _pin in Pins)
                        {
                            if (_pin.Direction == PinDirection.Input && _pin.IsConnected)
                            {
                                if (!(_pin as MuxerInputPin).EOSDelivered)
                                {
                                    MuxerTrack _track = (_pin as MuxerInputPin).Track;
                                    if (_minPosition > _track.Position)
                                    {
                                        _minPosition = _track.Position;
                                    }
                                    if (_track.Position > _maxPosition)
                                    {
                                        _maxPosition = _track.Position;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        _minPosition = 0;
                        _maxPosition = MAX_LONG;
                    }
                    if (_minPosition != MAX_LONG)
                    {
                        foreach (BasePin _pin in Pins)
                        {
                            if (_pin.Direction == PinDirection.Input && _pin.IsConnected)
                            {
                                if (!(_pin as MuxerInputPin).EOSDelivered)
                                {
                                    MuxerTrack _track = (_pin as MuxerInputPin).Track;
                                    if (_track.Position >= _minPosition)
                                    {
                                        while (true)
                                        {
                                            PacketData _packet = _track.GetNextPacket();
                                            if (_packet == null) break;
                                            m_pFileWriter.WritePacket(_track, _packet);
                                            _packet.Dispose();
                                            if (_track.Position >= _maxPosition) break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                CloseFile();
            }
            m_evReady.Set();
            TRACE("Mux Quit");
        }

        #endregion

        #region IAMFilterMiscFlags Members

        public int GetMiscFlags()
        {
            return 1;
        }

        #endregion

        #region IFileSinkFilter Members

        public int SetFileName(string pszFileName, AMMediaType pmt)
        {
            if (IsActive) return VFW_E_WRONG_STATE;
            m_sFileName = pszFileName;
            return NOERROR;
        }

        public int GetCurFile(out string pszFileName, AMMediaType pmt)
        {
            pszFileName = m_sFileName;
            return NOERROR;
        }

        #endregion

        #region IFileSinkFilter2 Members

        public int SetMode(AMFileSinkFlags dwFlags)
        {
            m_bOverwrite = (dwFlags == AMFileSinkFlags.OverWrite);
            return NOERROR;
        }

        public int GetMode(out AMFileSinkFlags dwFlags)
        {
            dwFlags = (m_bOverwrite ? AMFileSinkFlags.OverWrite : AMFileSinkFlags.None);
            return NOERROR;
        }

        #endregion

        #region IMediaSeeking Members

        public int GetCapabilities(out AMSeekingSeekingCapabilities pCapabilities)
        {
            pCapabilities = AMSeekingSeekingCapabilities.None;
            if (SeekingPin != null)
            {
                return SeekingPin.Position.GetCapabilities(out pCapabilities);
            }
            return E_NOINTERFACE;
        }

        public int CheckCapabilities(ref AMSeekingSeekingCapabilities pCapabilities)
        {
            if (SeekingPin != null)
            {
                return SeekingPin.Position.CheckCapabilities(ref pCapabilities);
            }
            return E_NOINTERFACE;
        }

        public int IsFormatSupported(Guid pFormat)
        {
            if (SeekingPin != null)
            {
                return SeekingPin.Position.IsFormatSupported(pFormat);
            }
            return E_NOINTERFACE;
        }

        public int QueryPreferredFormat(out Guid pFormat)
        {
            pFormat = TimeFormat.None;
            if (SeekingPin != null)
            {
                return SeekingPin.Position.QueryPreferredFormat(out pFormat);
            }
            return E_NOINTERFACE;
        }

        public int GetTimeFormat(out Guid pFormat)
        {
            pFormat = TimeFormat.None;
            if (SeekingPin != null)
            {
                return SeekingPin.Position.GetTimeFormat(out pFormat);
            }
            return E_NOINTERFACE;
        }

        public int IsUsingTimeFormat(Guid pFormat)
        {
            if (SeekingPin != null)
            {
                return SeekingPin.Position.IsUsingTimeFormat(pFormat);
            }
            return E_NOINTERFACE;
        }

        public int SetTimeFormat(Guid pFormat)
        {
            if (SeekingPin != null)
            {
                return SeekingPin.Position.IsUsingTimeFormat(pFormat);
            }
            return E_NOINTERFACE;
        }

        public int GetDuration(out long pDuration)
        {
            pDuration = 0;
            if (SeekingPin != null)
            {
                SeekingPin.Position.GetDuration(out pDuration);
                return NOERROR;
            }
            return E_NOINTERFACE;
        }

        public int GetStopPosition(out long pStop)
        {
            pStop = 0;
            if (SeekingPin != null)
            {
                return SeekingPin.Position.GetStopPosition(out pStop);
            }
            return E_NOINTERFACE;
        }

        public int GetCurrentPosition(out long pCurrent)
        {
            pCurrent = 0;
            if (SeekingPin != null)
            {
                return SeekingPin.Position.GetCurrentPosition(out pCurrent);
            }
            return E_NOINTERFACE;
        }

        public int ConvertTimeFormat(out long pTarget, DsGuid pTargetFormat, long Source, DsGuid pSourceFormat)
        {
            pTarget = 0;
            if (SeekingPin != null)
            {
                return SeekingPin.Position.ConvertTimeFormat(out pTarget, pTargetFormat, Source, pSourceFormat);
            }
            return E_NOINTERFACE;
        }

        public int SetPositions(DsLong pCurrent, AMSeekingSeekingFlags dwCurrentFlags, DsLong pStop, AMSeekingSeekingFlags dwStopFlags)
        {
            if (SeekingPin != null)
            {
                return SeekingPin.Position.SetPositions(pCurrent, dwCurrentFlags, pStop, dwStopFlags);
            }
            return E_NOINTERFACE;
        }

        public int GetPositions(out long pCurrent, out long pStop)
        {
            pCurrent = 0;
            pStop = 0;
            if (SeekingPin != null)
            {
                return SeekingPin.Position.GetPositions(out pCurrent, out pStop);
            }
            return E_NOINTERFACE;
        }

        public int GetAvailable(out long pEarliest, out long pLatest)
        {
            pEarliest = 0;
            pLatest = 0;
            if (SeekingPin != null)
            {
                return SeekingPin.Position.GetAvailable(out pEarliest, out pLatest);
            }
            return E_NOINTERFACE;
        }

        public int SetRate(double dRate)
        {
            if (SeekingPin != null)
            {
                return SeekingPin.Position.SetRate(dRate);
            }
            return E_NOINTERFACE;
        }

        public int GetRate(out double pdRate)
        {
            pdRate = 1.0;
            if (SeekingPin != null)
            {
                return SeekingPin.Position.GetRate(out pdRate);
            }
            return E_NOINTERFACE;
        }

        public int GetPreroll(out long pllPreroll)
        {
            pllPreroll = 0;
            if (SeekingPin != null)
            {
                return SeekingPin.Position.GetPreroll(out pllPreroll);
            }
            return E_NOINTERFACE;
        }

        #endregion
    }

    #region Base Classes

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class BaseMuxerFilter : BaseMuxer
    {
        #region Constructor

        protected BaseMuxerFilter(string _name,FileWriter _writer)
            : base(_name,_writer)
        {
            AddPin(new MuxerOutputPin(this));
        }
        
        #endregion
    }

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class BaseFileWriterFilter : BaseMuxer, IFileSinkFilter2
    {
        #region Constructor

        protected BaseFileWriterFilter(string _name, FileWriter _writer)
            : base(_name,_writer)
        {
        }
        
        #endregion
    }

    #endregion

    #region Generic Templates

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class BaseMuxerFilterTemplate<Writer> : BaseMuxerFilter where Writer : FileWriter, new()
    {
        public BaseMuxerFilterTemplate(string _name)
            : base(_name, new Writer())
        {
        }
    }

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class BaseFileWriterFilterTemplate<Writer> : BaseFileWriterFilter where Writer : FileWriter, new()
    {
        public BaseFileWriterFilterTemplate(string _name)
            : base(_name, new Writer())
        {
        }
    }

    #endregion

    #endregion
}