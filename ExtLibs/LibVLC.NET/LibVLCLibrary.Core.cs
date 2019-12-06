////////////////////////////////////////////////////////////////////////////////
//
//  LibVLCLibrary.Core.cs - This file is part of LibVLC.NET.
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
using System;
using System.Runtime.InteropServices;

#pragma warning disable 1591

namespace LibVLC.NET
{
  
  //****************************************************************************
  partial class LibVLCLibrary
  {

    // libvlc_instance_t * libvlc_new (int argc, const char *const *argv)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr libvlc_new_signature(int argc, string[] argv);

    //==========================================================================
    public readonly libvlc_new_signature m_libvlc_new;

    //==========================================================================
    /// <summary>
    ///   Create and initialize a libvlc instance. 
    /// </summary>
    /// <returns>
    ///   The libvlc instance or NULL in case of error. 
    /// </returns>
    public IntPtr libvlc_new()
    {
      VerifyAccess();

      return m_libvlc_new(0, null);
    }

    // void libvlc_release (libvlc_instance_t *p_instance)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void libvlc_release_signature(IntPtr p_instance);

    //==========================================================================
    private readonly libvlc_release_signature m_libvlc_release;

    //==========================================================================
    /// <summary>
    ///   Decrement the reference count of a libvlc instance, and destroy it if 
    ///   it reaches zero. 
    /// </summary>
    /// <param name="p_instance">
    ///   The instance to the reference.
    /// </param>
    public void libvlc_release(IntPtr p_instance)
    {
      VerifyAccess();

      m_libvlc_release(p_instance);
    }

    /*
    void libvlc_retain (libvlc_instance_t *p_instance)
    Increments the reference count of a libvlc instance. 

    int libvlc_add_intf (libvlc_instance_t *p_instance, const char *name)
    Try to start a user interface for the libvlc instance. 

    void libvlc_set_exit_handler (libvlc_instance_t *p_instance, void(*cb)(void *), void *opaque)
    Registers a callback for the LibVLC exit event. 

    void libvlc_wait (libvlc_instance_t *p_instance)
    Waits until an interface causes the instance to exit. 

    void libvlc_set_user_agent (libvlc_instance_t *p_instance, const char *name, const char *http)
    Sets the application name. 
    */

    // const char * libvlc_get_version (void)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate string libvlc_get_version_signature();

    //==========================================================================
    private readonly libvlc_get_version_signature m_libvlc_get_version;

    //==========================================================================
    public string libvlc_get_version()
    {
      VerifyAccess();

      return m_libvlc_get_version();
    }

    // const char * libvlc_get_compiler (void)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate string libvlc_get_compiler_signature();

    //==========================================================================
    private readonly libvlc_get_compiler_signature m_libvlc_get_compiler;

    //==========================================================================
    /// <summary>
    ///   Retrieve libvlc compiler version. 
    /// </summary>
    /// <returns>
    ///   A string containing the libvlc compiler version.
    /// </returns>
    public string libvlc_get_compiler()
    {
      VerifyAccess();

      return m_libvlc_get_compiler();
    }

    // const char * libvlc_get_changeset (void)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate string libvlc_get_changeset_signature();

    //==========================================================================
    private readonly libvlc_get_changeset_signature m_libvlc_get_changeset;

    //==========================================================================
    /// <summary>
    ///   Retrieve libvlc changeset. 
    /// </summary>
    /// <returns>
    ///   A string containing the libvlc changeset.
    /// </returns>
    public string libvlc_get_changeset()
    {
      VerifyAccess();

      return m_libvlc_get_changeset();
    }

    //==========================================================================
    // void libvlc_free (void* ptr) 

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void libvlc_free_signature(IntPtr ptr);

    //==========================================================================
    private readonly libvlc_free_signature m_libvlc_free;

    //==========================================================================
    public void libvlc_free(IntPtr ptr)
    {
      VerifyAccess();

      m_libvlc_free(ptr);
    }

  } // class LibVLCLibrary

}
