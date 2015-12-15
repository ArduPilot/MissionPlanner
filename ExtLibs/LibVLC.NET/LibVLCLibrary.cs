////////////////////////////////////////////////////////////////////////////////
//
//  LibVLCLibrary.cs - This file is part of LibVLC.NET.
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
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace LibVLC.NET
{
  
  //****************************************************************************
  /// <summary>
  ///   Provides access to the functions of a loaded <c>LibVLC</c> library.
  /// </summary>
  public partial class LibVLCLibrary
    : IDisposable
  {

    #region WIN32 API

    //==========================================================================
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetDllDirectory(string lpPathName);

    //==========================================================================
    [DllImport("kernel32", SetLastError = true)]
    private static extern IntPtr LoadLibrary(string lpFileName);

    //==========================================================================
    [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

    //==========================================================================
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FreeLibrary(IntPtr hModule);

    #endregion // WIN32 API

    //==========================================================================
    private IntPtr m_Handle;

    //==========================================================================
    private Delegate LoadDelegate<DelegateType>(string functionName)
    {
      try
      {
        IntPtr address = GetProcAddress(m_Handle, functionName);
        if(address == IntPtr.Zero)
          throw new Win32Exception();
        return Marshal.GetDelegateForFunctionPointer(address, typeof(DelegateType));
      }
      catch(Exception e)
      {
        throw new MissingMethodException(String.Format("The address of the function {0} could not be loaded!", functionName), e);
      }
    }

    //==========================================================================
    /// <summary>
    ///   Initializes a new <see cref="LibVLCLibrary"/> instance with the 
    ///   native <c>LibVLC</c> libraries located in the provided directory.
    /// </summary>                
    /// <param name="libVLCDirectory">
    ///   The location of the libraries <c>libvlc.dll</c>, 
    ///   <c>libvlccore.dll</c> and the <c>plugins</c> folder.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   Will be thrown if <paramref name="libVLCDirectory"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   Will be thrown if no suitable libvlc.dll could be loaded in the 
    ///   directory specified by <paramref name="libVLCDirectory"/>.
    /// </exception>
    public LibVLCLibrary(string libVLCDirectory)
    {
      Debug.WriteLine("Creating LibVLCLibrary...");

      if(libVLCDirectory == null)
        throw new ArgumentNullException("libVLCDirectory");

      try
      {
        libVLCDirectory = Path.GetFullPath(libVLCDirectory);
        bool result = SetDllDirectory(libVLCDirectory);
        if(!result)
          throw new Win32Exception(Marshal.GetLastWin32Error());
      }
      catch(Exception exception)
      {
        throw new ArgumentException("The provided directory is invalid: " + exception.Message,
                                    "libVLCDirectory",
                                    exception);
      }
      try
      {
        string libvlc_path = Path.Combine(libVLCDirectory, "libvlc.dll");

        string directory = System.IO.Directory.GetCurrentDirectory();
        try
        {
          // Setting the current directory is probably not necessary; just to be sure...
          System.IO.Directory.SetCurrentDirectory(libVLCDirectory);
        }
        catch(DirectoryNotFoundException e)
        {
          throw new ArgumentException(String.Format("The directory {0} does not exist!", libVLCDirectory), "libVLCDirectory", e);
        }
        try
        {
          m_Handle = LoadLibrary(libvlc_path);
          if(m_Handle == IntPtr.Zero)
            throw new Win32Exception(Marshal.GetLastWin32Error());
          try
          {
            // Core
            m_libvlc_new = (libvlc_new_signature)LoadDelegate<libvlc_new_signature>("libvlc_new");
            m_libvlc_release = (libvlc_release_signature)LoadDelegate<libvlc_release_signature>("libvlc_release");
            m_libvlc_get_version = (libvlc_get_version_signature)LoadDelegate<libvlc_get_version_signature>("libvlc_get_version");
            m_libvlc_get_compiler = (libvlc_get_compiler_signature)LoadDelegate<libvlc_get_compiler_signature>("libvlc_get_compiler");
            m_libvlc_get_changeset = (libvlc_get_changeset_signature)LoadDelegate<libvlc_get_changeset_signature>("libvlc_get_changeset");
            m_libvlc_free = (libvlc_free_signature)LoadDelegate<libvlc_free_signature>("libvlc_free");
              /*
            // Check version...
            string version = m_libvlc_get_version();
            if(version == null)
              throw new Exception("The version returned by libvlc_get_version is null!");
            string[] tokens = version.Split(new char[] { '.' }, 3);
            
            int major, minor;
            if((tokens.Length < 2) || !Int32.TryParse(tokens[0], out major) || !Int32.TryParse(tokens[1], out minor))
              throw new Exception(String.Format("The result of libvlc_get_version has an unsupported format: {0}", version));
            if((major != 1) && (minor != 2))
              throw new Exception(String.Format("The version is not supported: {0}", version));
              */
            // Core.AsynchronousEvents
            m_libvlc_event_attach = (libvlc_event_attach_signature)LoadDelegate<libvlc_event_attach_signature>("libvlc_event_attach");
            m_libvlc_event_detach = (libvlc_event_detach_signature)LoadDelegate<libvlc_event_detach_signature>("libvlc_event_detach");

            // Core.ErrorHandling
            m_libvlc_errmsg = (libvlc_errmsg_signature)LoadDelegate<libvlc_errmsg_signature>("libvlc_errmsg");
            m_libvlc_clearerr = (libvlc_clearerr_signature)LoadDelegate<libvlc_clearerr_signature>("libvlc_clearerr");

            // Media
            m_libvlc_media_new_location = (libvlc_media_new_location_signature)LoadDelegate<libvlc_media_new_location_signature>("libvlc_media_new_location");
            m_libvlc_media_new_path = (libvlc_media_new_path_signature)LoadDelegate<libvlc_media_new_path_signature>("libvlc_media_new_path");
            m_libvlc_media_new_as_node = (libvlc_media_new_as_node_signature)LoadDelegate<libvlc_media_new_as_node_signature>("libvlc_media_new_as_node");
            m_libvlc_media_release = (libvlc_media_release_signature)LoadDelegate<libvlc_media_release_signature>("libvlc_media_release");
            m_libvlc_media_get_mrl = (libvlc_media_get_mrl_signature)LoadDelegate<libvlc_media_get_mrl_signature>("libvlc_media_get_mrl");
            m_libvlc_media_subitems = (libvlc_media_subitems_signature)LoadDelegate<libvlc_media_subitems_signature>("libvlc_media_subitems");
            m_libvlc_media_event_manager = (libvlc_media_event_manager_signature)LoadDelegate<libvlc_media_event_manager_signature>("libvlc_media_event_manager");
            m_libvlc_media_get_duration = (libvlc_media_get_duration_signature)LoadDelegate<libvlc_media_get_duration_signature>("libvlc_media_get_duration");
            m_libvlc_media_parse = (libvlc_media_parse_signature)LoadDelegate<libvlc_media_parse_signature>("libvlc_media_parse");
            m_libvlc_media_parse_async = (libvlc_media_parse_async_signature)LoadDelegate<libvlc_media_parse_async_signature>("libvlc_media_parse_async");
            m_libvlc_media_is_parsed = (libvlc_media_is_parsed_signature)LoadDelegate<libvlc_media_is_parsed_signature>("libvlc_media_is_parsed");
            m_libvlc_media_get_tracks_info = (libvlc_media_get_tracks_info_signature)LoadDelegate<libvlc_media_get_tracks_info_signature>("libvlc_media_get_tracks_info");

            // MediaPlayer
            m_libvlc_media_player_new = (libvlc_media_player_new_signature)LoadDelegate<libvlc_media_player_new_signature>("libvlc_media_player_new");
            m_libvlc_media_player_new_from_media = (libvlc_media_player_new_from_media_signature)LoadDelegate<libvlc_media_player_new_from_media_signature>("libvlc_media_player_new_from_media");
            m_libvlc_media_player_release = (libvlc_media_player_release_signature)LoadDelegate<libvlc_media_player_release_signature>("libvlc_media_player_release");
            m_libvlc_media_player_set_media = (libvlc_media_player_set_media_signature)LoadDelegate<libvlc_media_player_set_media_signature>("libvlc_media_player_set_media");
            m_libvlc_media_player_get_media = (libvlc_media_player_get_media_signature)LoadDelegate<libvlc_media_player_get_media_signature>("libvlc_media_player_get_media");
            m_libvlc_media_player_event_manager = (libvlc_media_player_event_manager_signature)LoadDelegate<libvlc_media_player_event_manager_signature>("libvlc_media_player_event_manager");
            m_libvlc_media_player_play = (libvlc_media_player_play_signature)LoadDelegate<libvlc_media_player_play_signature>("libvlc_media_player_play");
            m_libvlc_media_player_pause = (libvlc_media_player_pause_signature)LoadDelegate<libvlc_media_player_pause_signature>("libvlc_media_player_pause");
            m_libvlc_media_player_stop = (libvlc_media_player_stop_signature)LoadDelegate<libvlc_media_player_stop_signature>("libvlc_media_player_stop");
            m_libvlc_video_set_callbacks = (libvlc_video_set_callbacks_signature)LoadDelegate<libvlc_video_set_callbacks_signature>("libvlc_video_set_callbacks");
            m_libvlc_video_set_format = (libvlc_video_set_format_signature)LoadDelegate<libvlc_video_set_format_signature>("libvlc_video_set_format");
            m_libvlc_video_set_format_callbacks = (libvlc_video_set_format_callbacks_signature)LoadDelegate<libvlc_video_set_format_callbacks_signature>("libvlc_video_set_format_callbacks");
            m_libvlc_media_player_get_length = (libvlc_media_player_get_length_signature)LoadDelegate<libvlc_media_player_get_length_signature>("libvlc_media_player_get_length");
            m_libvlc_media_player_get_time = (libvlc_media_player_get_time_signature)LoadDelegate<libvlc_media_player_get_time_signature>("libvlc_media_player_get_time");
            m_libvlc_media_player_set_time = (libvlc_media_player_set_time_signature)LoadDelegate<libvlc_media_player_set_time_signature>("libvlc_media_player_set_time");
            m_libvlc_media_player_get_position = (libvlc_media_player_get_position_signature)LoadDelegate<libvlc_media_player_get_position_signature>("libvlc_media_player_get_position");
            m_libvlc_media_player_set_position = (libvlc_media_player_set_position_signature)LoadDelegate<libvlc_media_player_set_position_signature>("libvlc_media_player_set_position");
            m_libvlc_media_player_set_chapter = (libvlc_media_player_set_chapter_signature)LoadDelegate<libvlc_media_player_set_chapter_signature>("libvlc_media_player_set_chapter");
            m_libvlc_media_player_get_chapter = (libvlc_media_player_get_chapter_signature)LoadDelegate<libvlc_media_player_get_chapter_signature>("libvlc_media_player_get_chapter");
            m_libvlc_media_player_get_chapter_count = (libvlc_media_player_get_chapter_count_signature)LoadDelegate<libvlc_media_player_get_chapter_count_signature>("libvlc_media_player_get_chapter_count");
            m_libvlc_media_player_previous_chapter = (libvlc_media_player_previous_chapter_signature)LoadDelegate<libvlc_media_player_previous_chapter_signature>("libvlc_media_player_previous_chapter");
            m_libvlc_media_player_next_chapter = (libvlc_media_player_next_chapter_signature)LoadDelegate<libvlc_media_player_next_chapter_signature>("libvlc_media_player_next_chapter");
            m_libvlc_media_player_get_state = (libvlc_media_player_get_state_signature)LoadDelegate<libvlc_media_player_get_state_signature>("libvlc_media_player_get_state");
            m_libvlc_media_player_get_fps = (libvlc_media_player_get_fps_signature)LoadDelegate<libvlc_media_player_get_fps_signature>("libvlc_media_player_get_fps");
            m_libvlc_media_player_is_seekable = (libvlc_media_player_is_seekable_signature)LoadDelegate<libvlc_media_player_is_seekable_signature>("libvlc_media_player_is_seekable");
            m_libvlc_media_player_can_pause = (libvlc_media_player_can_pause_signature)LoadDelegate<libvlc_media_player_can_pause_signature>("libvlc_media_player_can_pause");
            m_libvlc_media_player_next_frame = (libvlc_media_player_next_frame_signature)LoadDelegate<libvlc_media_player_next_frame_signature>("libvlc_media_player_next_frame");
            m_libvlc_track_description_list_release = (libvlc_track_description_list_release_signature)LoadDelegate<libvlc_track_description_list_release_signature>("libvlc_track_description_list_release");

            // MediaPlayer.AudioControls
            m_libvlc_audio_get_volume = (libvlc_audio_get_volume_signature)LoadDelegate<libvlc_audio_get_volume_signature>("libvlc_audio_get_volume");
            m_libvlc_audio_set_volume = (libvlc_audio_set_volume_signature)LoadDelegate<libvlc_audio_set_volume_signature>("libvlc_audio_set_volume");
            m_libvlc_audio_get_track_count = (libvlc_audio_get_track_count_signature)LoadDelegate<libvlc_audio_get_track_count_signature>("libvlc_audio_get_track_count");
            m_libvlc_audio_get_track_description = (libvlc_audio_get_track_description_signature)LoadDelegate<libvlc_audio_get_track_description_signature>("libvlc_audio_get_track_description");
            m_libvlc_audio_get_track = (libvlc_audio_get_track_signature)LoadDelegate<libvlc_audio_get_track_signature>("libvlc_audio_get_track");
            m_libvlc_audio_set_track = (libvlc_audio_set_track_signature)LoadDelegate<libvlc_audio_set_track_signature>("libvlc_audio_set_track");

            // MediaPlayer.VideoControls
            m_libvlc_video_get_size = (libvlc_video_get_size_signature)LoadDelegate<libvlc_video_get_size_signature>("libvlc_video_get_size");
            m_libvlc_video_get_height = (libvlc_video_get_height_signature)LoadDelegate<libvlc_video_get_height_signature>("libvlc_video_get_height");
            m_libvlc_video_get_width = (libvlc_video_get_width_signature)LoadDelegate<libvlc_video_get_width_signature>("libvlc_video_get_width");
            m_libvlc_video_get_track_count = (libvlc_video_get_track_count_signature)LoadDelegate<libvlc_video_get_track_count_signature>("libvlc_video_get_track_count");
            m_libvlc_video_get_track_description = (libvlc_video_get_track_description_signature)LoadDelegate<libvlc_video_get_track_description_signature>("libvlc_video_get_track_description");
            m_libvlc_video_get_track = (libvlc_video_get_track_signature)LoadDelegate<libvlc_video_get_track_signature>("libvlc_video_get_track");
            m_libvlc_video_set_track = (libvlc_video_set_track_signature)LoadDelegate<libvlc_video_set_track_signature>("libvlc_video_set_track");
            m_libvlc_video_get_spu = (libvlc_video_get_spu_signature)LoadDelegate<libvlc_video_get_spu_signature>("libvlc_video_get_spu");
            m_libvlc_video_get_spu_description = (libvlc_video_get_spu_description_signature)LoadDelegate<libvlc_video_get_spu_description_signature>("libvlc_video_get_spu_description");
            m_libvlc_video_set_spu = (libvlc_video_set_spu_signature)LoadDelegate<libvlc_video_set_spu_signature>("libvlc_video_set_spu");

            // Media Lists
            m_libvlc_media_list_release = (libvlc_media_list_release_signature)LoadDelegate<libvlc_media_list_release_signature>("libvlc_media_list_release");
            m_libvlc_media_list_lock = (libvlc_media_list_lock_signature)LoadDelegate<libvlc_media_list_lock_signature>("libvlc_media_list_lock");
            m_libvlc_media_list_unlock = (libvlc_media_list_unlock_signature)LoadDelegate<libvlc_media_list_unlock_signature>("libvlc_media_list_unlock");
            m_libvlc_media_list_count = (libvlc_media_list_count_signature)LoadDelegate<libvlc_media_list_count_signature>("libvlc_media_list_count");
            m_libvlc_media_list_item_at_index = (libvlc_media_list_item_at_index_signature)LoadDelegate<libvlc_media_list_item_at_index_signature>("libvlc_media_list_item_at_index");


            m_Directory = libVLCDirectory;
          }
          catch(Exception exception)
          {
            FreeLibrary(m_Handle);
            throw new ArgumentException(String.Format("No suitable libvlc.dll could be found in the provided path: {0}", exception.Message), "libVLCDirectory", exception);
          }
        }
        finally
        {
          System.IO.Directory.SetCurrentDirectory(directory);
        }
      }
      finally
      {
        // Just resetting is probably not enough! 
        // It would be better to restore it to the value before setting it!
        SetDllDirectory(null);
      }
    }

    //==========================================================================
    /// <summary>
    ///   Destroys the <see cref="LibVLCLibrary"/> instance.
    /// </summary>
    /// <remarks>
    ///   <see cref="Finalize"/> invokes <see cref="Dispose(bool)"/> passing 
    ///   <c>false</c>.
    /// </remarks>
    ~LibVLCLibrary()
    {
      Dispose(false);
    }

    //==========================================================================
    /// <summary>
    ///   Is called by <see cref="Dispose()"/> or by the destructor to free the 
    ///   native <c>LibVLC</c> library.
    /// </summary>
    /// <param name="disposing">
    ///   Specifies whether <see cref="Dispose(bool)"/> has been called by 
    ///   <see cref="Dispose()"/> or by the destructor.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
      Debug.WriteLine(String.Format("LibVLCLibrary.Dispose({0})", disposing));

      if(m_Handle != IntPtr.Zero)
      {
        FreeLibrary(m_Handle);
        m_Handle = IntPtr.Zero;
      }
    }

    //==========================================================================
    /// <summary>
    ///   Implementes <see cref="IDisposable.Dispose"/> and frees the native 
    ///   <c>LibVLC</c> library.
    /// </summary>
    public void Dispose()
    {
      Debug.WriteLine("LibVLCLibrary.Dispose()");

      VerifyAccess();

      Dispose(true);

      GC.SuppressFinalize(this);
    }

    //==========================================================================
    /// <summary>
    ///   Ensures the <see cref="LibVLCLibrary"/> instance has not been 
    ///   disposed yet.
    /// </summary>
    protected void VerifyAccess()
    {
      if(m_Handle == IntPtr.Zero)
        throw new ObjectDisposedException("LibVLCLibrary");
    }

    #region Properties

    //==========================================================================
    private readonly string m_Directory;

    //==========================================================================
    /// <summary>
    ///   Gets the directory where the native <see cref="LibVLC"/> library is 
    ///   located.
    /// </summary>
    public string Directory
    {
      get
      {
        return m_Directory;
      }
    }

    #endregion // Properties

    #region Static Members

    //==========================================================================
    private class LibraryHandle
    {
      //------------------------------------------------------------------------
      public uint ReferenceCounter;
      public LibVLCLibrary Library;

      //------------------------------------------------------------------------
      public LibraryHandle(string libVLCDirectory)
      {
        ReferenceCounter = 0;
        Library = new LibVLCLibrary(libVLCDirectory);
      }
    }

    //==========================================================================
    private static readonly Dictionary<string, LibraryHandle> m_LibraryHandles = new Dictionary<string, LibraryHandle>();
    private static LibraryHandle m_DefaultLibraryHandle = null;

    //==========================================================================
    private static LibraryHandle GetOrLoadLibrary(string libVLCDirectory)
    {
      LibraryHandle library_handle = null;

      lock(m_LibraryHandles)
      {
        if(libVLCDirectory != null)
        {
          if(!m_LibraryHandles.TryGetValue(libVLCDirectory, out library_handle))
            m_LibraryHandles.Add(libVLCDirectory, library_handle = new LibraryHandle(libVLCDirectory));
          ++library_handle.ReferenceCounter;
          return library_handle;
        }

        if(m_DefaultLibraryHandle != null)
        {
          library_handle = m_DefaultLibraryHandle;
          ++library_handle.ReferenceCounter;
          return library_handle;
        }

        Debug.WriteLine("Searching for libvlc.dll...", "LibVLC");

        string executing_assembly_path = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(Assembly.GetExecutingAssembly().Location)), "VLC");
        string entry_assembly_path = null;
        if(Assembly.GetEntryAssembly() != null)
          entry_assembly_path = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(Assembly.GetEntryAssembly().Location)), "VLC");
        string current_directory_path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "VLC");
        string program_files_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "VideoLAN", "VLC");

        foreach(string directory in
                  new string[] 
                  { 
                    executing_assembly_path,
                    entry_assembly_path,
                    current_directory_path,
                    program_files_path
                  })
          if(System.IO.Directory.Exists(directory))
            try
            {
              m_DefaultLibraryHandle = GetOrLoadLibrary(directory);
              Debug.WriteLine(String.Format("Found libvlc.dll in directory {0}.", directory), "LibVLC");
              return m_DefaultLibraryHandle;
            }
            catch(Exception e)
            {
              Debug.WriteLine(String.Format("Caught exception of type {0} while loading libvlc.dll from {1}: {2}",
                                            e.GetType().Name,
                                            directory,
                                            e.Message),
                              "LibVLC");
            }

        throw new DllNotFoundException("No valid libvlc.dll could be found; VLC is probably not installed!");

      } // lock(m_LibraryHandles)
    }

    //==========================================================================
    /// <summary>
    ///   Loads the library from the specified directory.
    /// </summary>
    /// <param name="libVLCDirectory">
    ///   The directory where <c>libvlc.dll</c> is located; may be <c>null</c>.
    /// </param>
    /// <returns>
    ///   The laoded library.
    /// </returns>
    /// <remarks>
    ///   If the library at the provided 
    /// </remarks>
    public static LibVLCLibrary Load(string libVLCDirectory)
    {
      return GetOrLoadLibrary(libVLCDirectory).Library;
    }

    //==========================================================================
    /// <summary>
    ///   Frees the provided library.
    /// </summary>
    /// <param name="library">
    ///   The library to free.
    /// </param>
    /// <returns>
    ///   The number how ofter the library is still referenced; will be <c>0</c>
    ///   in case the library is no longer referenced and has been disposed.
    /// </returns>
    public static uint Free(LibVLCLibrary library)
    {
      if(library == null)
        throw new ArgumentNullException("library");

      lock(m_LibraryHandles)
      {
        LibraryHandle library_handle;
        if(m_LibraryHandles.TryGetValue(library.Directory, out library_handle))
          if(library_handle.Library == library)
          {
            if(--library_handle.ReferenceCounter == 0)
            {
              library_handle.Library.Dispose();
              m_LibraryHandles.Remove(library.Directory);
            }

            if(library_handle == m_DefaultLibraryHandle)
              m_DefaultLibraryHandle = null;

            return library_handle.ReferenceCounter;
          }
      }

      throw new ArgumentException("The provided library has not been loaded using the Load method!", "library");
    }

    #endregion // Static Members

  } // class LibVLCLibrary

}

