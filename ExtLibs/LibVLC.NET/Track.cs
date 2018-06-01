////////////////////////////////////////////////////////////////////////////////
//
//  Track.cs - This file is part of LibVLC.NET.
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
using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace LibVLC.NET
{

  //****************************************************************************
  /// <summary>
  ///   Base class for all track classes.
  /// </summary>
  /// <seealso cref="VideoTrack"/>
  /// <seealso cref="AudioTrack"/>
  /// <seealso cref="SubtitleTrack"/>
  public class Track
  {
    //==========================================================================
    private static readonly ConcurrentDictionary<string, CultureInfo> m_Cultures = new ConcurrentDictionary<string, CultureInfo>();

    //==========================================================================
    private static CultureInfo GetCulture(string nativeName)
    {
      return m_Cultures.GetOrAdd(nativeName,
        delegate
        {
          foreach(CultureInfo culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
            if(culture.NativeName == nativeName)
              return culture;
          return null;
        });
    }

    //==========================================================================
    private static readonly Regex m_NameLanguageRegex = new Regex(@"^(.*)\s-\s\[(.*)\]$");

    //==========================================================================
    internal Track(int index, LibVLCLibrary.libvlc_track_description_t trackDescription, LibVLCLibrary.libvlc_media_track_info_t? trackInfo)
    {
      m_Name = trackDescription.psz_name;

      // Title 1 - [Deutsch]
      // Title 2 - [English]
      Match match = m_NameLanguageRegex.Match(m_Name);
      if(match.Success)
      {
        m_Title = match.Groups[1].Value;
        m_Language = match.Groups[2].Value;
        m_Culture = GetCulture(m_Language);
      }
      else
      {
        m_Title = trackDescription.psz_name;
        m_Language = null;
        m_Culture = null;
      }

      if(trackInfo.HasValue)
      {
        m_Codec = Encoding.ASCII.GetString(BitConverter.GetBytes(trackInfo.Value.i_codec));
        for(int i = 0; i < m_Codec.Length; ++i)
          if(m_Codec[i] == '\0')
          {
            m_Codec = m_Codec.Substring(0, i);
            break;
          }
      }
      else
        m_Codec = null;

      m_Index = index;
    }

    //==========================================================================
    /// <summary>
    ///   Overrides <see cref="Object.ToString"/> and returns the track's
    ///   name.
    /// </summary>
    /// <returns>
    ///   The name of the track.
    /// </returns>
    public override string ToString()
    {
      return m_Name;
    }

    #region Properties

    #region Index

    //==========================================================================
    private readonly int m_Index;

    //==========================================================================                
    /// <summary>
    ///   Gets the index of the track within the media player's track 
    ///   collection.
    /// </summary>
    public int Index
    {
      get
      {
        return m_Index;
      }
    }

    #endregion // Index

    #region Name

    //==========================================================================
    private readonly string m_Name;

    //==========================================================================                
    /// <summary>
    ///   Gets the name of the track.
    /// </summary>
    public string Name
    {
      get
      {
        return m_Name;
      }
    }

    #endregion // Name

    #region Title

    //==========================================================================
    private readonly string m_Title;

    //==========================================================================                
    /// <summary>
    ///   Gets the name of the track.
    /// </summary>
    public string Title
    {
      get
      {
        return m_Title;
      }
    }

    #endregion // Title

    #region Codec

    //==========================================================================
    private readonly string m_Codec;

    //==========================================================================                
    /// <summary>
    ///   Gets the codec of the track.
    /// </summary>
    public string Codec
    {
      get
      {
        return m_Codec;
      }
    }

    #endregion // Codec

    #region Language

    //==========================================================================
    private readonly string m_Language;

    //==========================================================================                
    /// <summary>
    ///   Gets the language of the track; may be <c>null</c> if the 
    ///   language could not be determined.
    /// </summary>
    /// <seealso cref="Culture"/>
    public string Language
    {
      get
      {
        return m_Language;
      }
    }

    #endregion // Language

    #region Culture

    //==========================================================================
    private readonly CultureInfo m_Culture;

    //==========================================================================                
    /// <summary>
    ///   Gets the culture of the track; may be <c>null</c> if the 
    ///   Culture could not be determined.
    /// </summary>
    /// <seealso cref="Language"/>
    public CultureInfo Culture
    {
      get
      {
        return m_Culture;
      }
    }

    #endregion // Culture

    #endregion // Properties

  } // class AudioStream

}
