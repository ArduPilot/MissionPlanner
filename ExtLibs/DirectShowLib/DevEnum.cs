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
    /// From CDEF_CLASS_* defines
    /// </summary>
    [Flags]
    public enum CDef
    {
        None = 0,
        ClassDefault = 0x0001,
        BypassClassManager = 0x0002,
        ClassLegacy = 0x0004,
        MeritAboveDoNotUse = 0x0008,
        DevmonCMGRDevice = 0x0010,
        DevmonDMO = 0x0020,
        DevmonPNPDevice = 0x0040,
        DevmonFilter = 0x0080,
        DevmonSelectiveMask = 0x00f0
    }

    #endregion

    #region Interfaces

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("29840822-5B84-11D0-BD3B-00A0C911CE86"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ICreateDevEnum
    {
        [PreserveSig]
        int CreateClassEnumerator(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid pType,
#if USING_NET11
            [Out] out UCOMIEnumMoniker ppEnumMoniker,
#else
            [Out] out IEnumMoniker ppEnumMoniker,
#endif
            [In] CDef dwFlags);
    }

    #endregion
}
