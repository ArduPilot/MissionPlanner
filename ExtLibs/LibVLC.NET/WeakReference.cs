////////////////////////////////////////////////////////////////////////////////
//
//  WeakReference.cs - This file is part of LibVLC.NET.
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
  ///   Represents a strongly typed weak reference.
  /// </summary>
  /// <typeparam name="TargetType">
  ///   The type of the target.
  /// </typeparam>
  public class WeakReference<TargetType>
    : WeakReference
  {

    //==========================================================================
    /// <summary>
    ///   Initializes a new instance of the class 
    ///   <see cref="WeakReference{TargetType}"/>.
    /// </summary>
    /// <param name="target">
    ///   Specifies the target to reference.
    /// </param>
    public WeakReference(TargetType target)
      : base(target)
    {
      // ...
    }

    //==========================================================================
    /// <summary>
    ///   Tries to get the target of the weak reference.
    /// </summary>
    /// <param name="target">
    ///   Will be set to the referenced target or the default value of 
    ///   <typeparamref name="TargetType"/> if the referenced target is no 
    ///   longer alive.
    /// </param>
    /// <returns>
    ///   <c>true</c> if the target is still alive or <c>false</c> if not.
    /// </returns>
    public bool TryGetTarget(out TargetType target)
    {
      object referenced_target = Target;
      if(referenced_target is TargetType)
      {
        target = (TargetType)referenced_target;
        return true;
      }

      target = default(TargetType);
      return false;
    }

  } // class WeakReference<TargetType>

}
