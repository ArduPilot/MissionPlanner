////////////////////////////////////////////////////////////////////////////////
//
//  LibVLCException.cs - This file is part of LibVLC.NET.
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
using System.Linq;
using System.Text;

namespace LibVLC.NET
{
  
  //****************************************************************************
  /// <summary>
  ///   Represents an error returned by a native <c>LibVLC</c> function.
  /// </summary>
  public class LibVLCException
    : Exception
  {

    //==========================================================================
    private static string GetErrorMessage(LibVLCLibrary library)
    {
      if(library == null)
        throw new ArgumentNullException("library");

      return library.libvlc_errmsg();
    }

    //==========================================================================
    /// <summary>
    ///   Initializes a new <see cref="LibVLCException"/> with the provided
    ///   error message.
    /// </summary>
    /// <param name="errorMessage">
    ///   The message which will be returned by 
    ///   <see cref="Exception.Message"/>.
    /// </param>
    public LibVLCException(string errorMessage)
      : base(errorMessage)
    {
      // ...
    }

    //==========================================================================
    /// <summary>
    ///   Initializes a new <see cref="LibVLCException"/> with the most recent
    ///   <c>LibVLC</c> error.
    /// </summary>
    /// <param name="library">
    ///   The <see cref="LibVLCLibrary"/> which will be used to obtain the 
    ///   most recent error for the calling thread.
    /// </param>
    /// <remarks>
    ///   <see cref="LibVLCLibrary.libvlc_errmsg"/> is called to get the 
    ///   message to initialize <see cref="Exception.Message"/> with.
    /// </remarks>
    public LibVLCException(LibVLCLibrary library)
      : this(GetErrorMessage(library))
    {
      // ...
    }

  } // class LibVLCException

}
