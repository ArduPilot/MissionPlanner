////////////////////////////////////////////////////////////////////////////////
//
//  LibVLCLibrary.Core.ErrorHandling.cs - This file is part of LibVLC.NET.
//
//    Copyright (C) 2011 Boris Richter <himself@boris-richter.net>
//
//  ==========================================================================
//  
//  LibVLC.NET is free software; you can redistribute it and/or modify it 
//  under the terms of the GNU Lesser General Public License as published by 
//  the Free Software Foundation; either version 2.1 of the License, or (at 
//  your option) any later version.
//    
//  LibVLC.NET is distributed in the hope that it will be useful, but WITHOUT 
//  ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
//  FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public 
//  License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License 
//  along with LibVLC.NET; if not, see http://www.gnu.org/licenses/.
//
//  ==========================================================================
// 
//  $LastChangedRevision$
//  $LastChangedDate$
//  $LastChangedBy$
//
////////////////////////////////////////////////////////////////////////////////
using System.Runtime.InteropServices;

#pragma warning disable 1591

namespace LibVLC.NET
{
  
  //****************************************************************************
  partial class LibVLCLibrary
  {

    // const char * libvlc_errmsg (void)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate string libvlc_errmsg_signature();

    //==========================================================================
    private readonly libvlc_errmsg_signature m_libvlc_errmsg;

    //==========================================================================
    public string libvlc_errmsg()
    {
      VerifyAccess();

      string result = m_libvlc_errmsg();
      return result;
    }

    // void libvlc_clearerr (void)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate string libvlc_clearerr_signature();

    //==========================================================================
    private readonly libvlc_clearerr_signature m_libvlc_clearerr;

    //==========================================================================
    public void libvlc_clearerr()
    {
      VerifyAccess();

      m_libvlc_clearerr();
    }

  } // class LibVLCLibrary

}
