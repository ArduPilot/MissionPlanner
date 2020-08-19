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

namespace DirectShowLib
{
    #region Declarations

    /// <summary>
    /// From KSMULTIPLE_ITEM - Note that data is returned in the memory IMMEDIATELY following this struct.
    /// The Size parm indicates ths size of the KSMultipleItem plus the extra bytes.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class KSMultipleItem
    {
        public int Size;
        public int Count;
    }

    #endregion

    #region Interfaces

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("00000109-0000-0000-C000-000000000046"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPersistStream : IPersist
    {
        #region IPersist Methods

        [PreserveSig]
        new int GetClassID([Out] out Guid pClassID);

        #endregion

        [PreserveSig]
        int IsDirty();

        [PreserveSig]
#if USING_NET11
        int Load([In] UCOMIStream pStm);
#else
        int Load([In] IStream pStm);
#endif

        [PreserveSig]
        int Save(
#if USING_NET11
            [In] UCOMIStream pStm,
#else
            [In] IStream pStm,
#endif
            [In, MarshalAs(UnmanagedType.Bool)] bool fClearDirty);

        [PreserveSig]
        int GetSizeMax([Out] out long pcbSize);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("0000010c-0000-0000-C000-000000000046"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPersist
    {
        [PreserveSig]
        int GetClassID([Out] out Guid pClassID);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("b61178d1-a2d9-11cf-9e53-00aa00a216a1"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IKsPin
    {
        /// <summary>
        /// The caller must free the returned structures, using the CoTaskMemFree function
        /// </summary>
        [PreserveSig]
        int KsQueryMediums(
            out IntPtr ip);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("B196B28B-BAB4-101A-B69C-00AA00341D07"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ISpecifyPropertyPages
    {
        [PreserveSig]
        int GetPages(out DsCAUUID pPages);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
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

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("3127CA40-446E-11CE-8135-00AA004BB851"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IErrorLog
    {
        [PreserveSig]
        int AddError(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszPropName,
#if USING_NET11
            [In] EXCEPINFO pExcepInfo);
#else
            [In] System.Runtime.InteropServices.ComTypes.EXCEPINFO pExcepInfo);
#endif
    }

    #endregion
}
