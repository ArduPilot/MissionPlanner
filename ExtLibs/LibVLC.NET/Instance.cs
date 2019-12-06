////////////////////////////////////////////////////////////////////////////////
//
//  Instance.cs - This file is part of LibVLC.NET.
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 1591

namespace LibVLC.NET
{

  //****************************************************************************
  /// <summary>
  ///   Represents a <c>LibVLC</c> instance.
  /// </summary>
  public class Instance
    : IDisposable
  {

    //==========================================================================
    private LibVLCLibrary m_Library;
    private bool m_IsLibraryOwner;
    private IntPtr m_Handle;

    //==========================================================================
    /// <summary>
    ///   Initializes a new instance of the class <see cref="Instance"/>.
    /// </summary>
    /// <param name="library">
    ///   May be <c>null</c> to use the default library.
    /// </param>
    public Instance(LibVLCLibrary library)
    {
      if(library == null)
      {
        m_Library = LibVLCLibrary.Load(null);
        m_IsLibraryOwner = true;
      }
      else
      {
        m_Library = library;
        m_IsLibraryOwner = false;
      }

      try
      {
        m_Handle = m_Library.libvlc_new();
        if(m_Handle == IntPtr.Zero)
          throw new LibVLCException(m_Library);
      }
      catch
      {
        if(m_IsLibraryOwner)
          try
          {
            LibVLCLibrary.Free(m_Library);
          }
          catch
          {
            // ...
          }

        throw;
      }
    }

    //==========================================================================
    /// <summary>
    ///   Initializes a new instance of the class <see cref="Instance"/>.
    /// </summary>
    public Instance()
      : this(null)
    {
      // ...
    }

    //==========================================================================
    /// <summary>
    ///   Destroys the <see cref="Instance"/> instance.
    /// </summary>
    ~Instance()
    {
      Debug.WriteLine("Destructing Instance...", "Instance");

      Dispose(false);
    }

    //==========================================================================
    private void Dispose(bool disposing)
    {
      if(m_Handle != IntPtr.Zero)
      {
        m_Library.libvlc_release(m_Handle);
        m_Handle = IntPtr.Zero;
      }

      if(disposing)
        if(m_Library != null)
        {
          if(m_IsLibraryOwner)
            LibVLCLibrary.Free(m_Library);
          m_Library = null;
        }
    }

    //==========================================================================
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);  
    }

    //==========================================================================
    public LibVLCLibrary Library
    {
      get
      {
        if(m_Library == null)
          throw new ObjectDisposedException("Instance");
        return m_Library;
      }
    }

    //==========================================================================
    public IntPtr Handle
    {
      get
      {
        if(m_Handle == IntPtr.Zero)
          throw new ObjectDisposedException("Instance");
        return m_Handle;
      }
    }

  } // class Instance

}
