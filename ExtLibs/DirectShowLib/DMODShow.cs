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

namespace DirectShowLib
{
    #region Interfaces

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("52d6f586-9f0f-4824-8fc8-e32ca04930c2"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDMOWrapperFilter
    {
        [PreserveSig]
        int Init(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid clsidDMO,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid catDMO
            );
    }

    #endregion
}
