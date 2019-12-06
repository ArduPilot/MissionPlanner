////////////////////////////////////////////////////////////////////////////////
//
//  MediaPlayer.cs - This file is part of LibVLC.NET.
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
//  $LastChangedRevision: 4950 $
//  $LastChangedDate: 2011-04-05 21:48:05 +0200 (Tue, 05 Apr 2011) $
//  $LastChangedBy: unknown $
//
////////////////////////////////////////////////////////////////////////////////
#pragma warning disable 1591
#pragma warning disable 0414

using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;

namespace LibVLC.NET
{

  //****************************************************************************
  /// <summary>
  ///   Represents a native LibVLC media player
  /// </summary>
  public class MediaPlayer
    : IDisposable
  {
    #region Static Members

    //==========================================================================
    private static readonly ConcurrentDictionary<IntPtr, WeakReference<MediaPlayer>> m_MediaPlayers = new ConcurrentDictionary<IntPtr, WeakReference<MediaPlayer>>();
    private static readonly ConcurrentDictionary<IntPtr, VideoBuffer> m_VideoBuffers = new ConcurrentDictionary<IntPtr, VideoBuffer>();
    private static readonly LibVLCLibrary.libvlc_video_format_cb m_VideoFormatCallback = Video_Format;
    private static readonly LibVLCLibrary.libvlc_video_lock_cb m_VideoLockCallback = Video_Lock;
    private static readonly LibVLCLibrary.libvlc_video_unlock_cb m_VideoUnlockCallback = Video_Unlock;
    private static readonly LibVLCLibrary.libvlc_video_display_cb m_VideoDisplayCallback = Video_Display;
    private static readonly LibVLCLibrary.libvlc_video_cleanup_cb m_VideoCleanupCallback = Video_Cleanup;
    private static readonly LibVLCLibrary.libvlc_callback_t m_EventManagerEventCallback = EventManager_Event;
    private static GCHandle m_VideoFormatCallbackHandle;
    private static GCHandle m_VideoLockCallbackHandle;
    private static GCHandle m_VideoUnlockCallbackHandle;
    private static GCHandle m_VideoDisplayCallbackHandle;
    private static GCHandle m_VideoCleanupCallbackHandle;
    private static GCHandle m_EventManagerEventCallbackHandle;

    //==========================================================================
    private static readonly LibVLCLibrary.libvlc_event_e[] m_Events = 
    { 
      //LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerBackward,
      /*LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerBuffering,*/
      LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerEncounteredError,
      LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerEndReached,
      //LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerForward,
      LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerLengthChanged,
      LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerMediaChanged,
      LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerNothingSpecial,
      LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerOpening,
      LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerPausableChanged,
      LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerPaused,
      LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerPlaying,
      //LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerPositionChanged,
      LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerSeekableChanged,
      //LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerSnapshotTaken,
      LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerStopped,
      LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerTimeChanged,
      //LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerTitleChanged                     
    };

    //==========================================================================
    private static void CleanupMediaPlayers()
    {
      lock(m_MediaPlayers)
        if(m_MediaPlayers.Count > 0)
        {
          List<IntPtr> orphaned_media_players = new List<IntPtr>();

          foreach(KeyValuePair<IntPtr, WeakReference<MediaPlayer>> pair in m_MediaPlayers)
          {
            MediaPlayer media_player;
            if(pair.Value.TryGetTarget(out media_player))
              if(media_player != null)
                continue;
            orphaned_media_players.Add(pair.Key);
          }

          if(orphaned_media_players.Count > 0)
          {
            Debug.WriteLine(String.Format("Found {0} orphand media players!", orphaned_media_players.Count), "MediaPlayer");
            foreach(IntPtr media_player in orphaned_media_players)
            {
              WeakReference<MediaPlayer> reference;
              m_MediaPlayers.TryRemove(media_player, out reference);
            }
          }

          if(m_MediaPlayers.Count == 0)
          {
            Debug.WriteLine("Freeing callback handles...", "MediaPlayer");

            m_VideoFormatCallbackHandle.Free();
            m_VideoCleanupCallbackHandle.Free();
            m_VideoLockCallbackHandle.Free();
            m_VideoUnlockCallbackHandle.Free();
            m_VideoDisplayCallbackHandle.Free();
            m_EventManagerEventCallbackHandle.Free();
          }

          m_CleaningUpMediaPlayersPending = false;
        }
    }

    //==========================================================================
    private static void RegisterMediaPlayer(IntPtr handle, MediaPlayer mediaPlayer)
    {
      Debug.WriteLine(String.Format("Registering MediaPlayer {0}...", handle), "MediaPlayer");

      lock(m_MediaPlayers)
      {
        if(m_MediaPlayers.Count == 0)
        {
          Debug.WriteLine("Allocating callback handles...", "MediaPlayer");

          m_VideoFormatCallbackHandle = GCHandle.Alloc(m_VideoFormatCallback);
          m_VideoCleanupCallbackHandle = GCHandle.Alloc(m_VideoCleanupCallback);
          m_VideoLockCallbackHandle = GCHandle.Alloc(m_VideoLockCallback);
          m_VideoUnlockCallbackHandle = GCHandle.Alloc(m_VideoUnlockCallback);
          m_VideoDisplayCallbackHandle = GCHandle.Alloc(m_VideoDisplayCallback);
          m_EventManagerEventCallbackHandle = GCHandle.Alloc(m_EventManagerEventCallback);
        }
        m_MediaPlayers.TryAdd(handle, new WeakReference<MediaPlayer>(mediaPlayer));
      }
      StartCleanupMediaPlayers();
    }

    //==========================================================================
    private static void UnregisterMediaPlayer(IntPtr handle)
    {
      Debug.WriteLine(String.Format("Unregistering media player {0}...", handle), "MediaPlayer");

      lock(m_MediaPlayers)
      {
        WeakReference<MediaPlayer> media_player;
        m_MediaPlayers.TryRemove(handle, out media_player);
      }
      StartCleanupMediaPlayers();
    }

    //==========================================================================
    private static volatile bool m_CleaningUpMediaPlayersPending = false;

    //==========================================================================
    private static void StartCleanupMediaPlayers()
    {
      lock(m_MediaPlayers)
        if(!m_CleaningUpMediaPlayersPending)
        {
          m_CleaningUpMediaPlayersPending = true;
          Debug.WriteLine("Starting cleanup task", "MediaPlayer");
          Task.Factory.StartNew(delegate { CleanupMediaPlayers(); });
        }
    }

    //==========================================================================
    private static MediaPlayer GetMediaPlayer(IntPtr handle)
    {
      WeakReference<MediaPlayer> media_player_reference;
      lock (m_MediaPlayers)
          if(m_MediaPlayers.TryGetValue(handle, out media_player_reference))
          {
            MediaPlayer media_player;
            if(media_player_reference.TryGetTarget(out media_player))
              if(media_player != null)
                return media_player;

            Debug.WriteLine(String.Format("MediaPlayer {0} has already been collected!", handle), "MediaPlayer");
          }

      StartCleanupMediaPlayers();
      return null;
    }

    //==========================================================================
    private static uint Video_Format(ref IntPtr opaque, ref uint chroma, ref uint width, ref uint height, ref uint pitches, ref uint lines)
    {
      Debug.WriteLine("Video_Format", "MediaPlayer");

      MediaPlayer media_player = GetMediaPlayer(opaque);
      if(media_player == null)
        return 0;

      VideoBuffer video_buffer;
      if(m_VideoBuffers.TryGetValue(opaque, out video_buffer))
        m_VideoBuffers.TryRemove(opaque, out video_buffer);
      video_buffer = new VideoBuffer(width, height, PixelFormat.RV32);
      m_VideoBuffers.TryAdd(opaque, video_buffer);
      media_player.VideoBuffer = video_buffer;
      media_player.RaiseVideoFormat();

      chroma = BitConverter.ToUInt32(new byte[] { (byte)'R', (byte)'V', (byte)'3', (byte)'2' }, 0);
      width = (uint)video_buffer.Width;
      height = (uint)video_buffer.Height;
      pitches = (uint)video_buffer.Stride;
      lines = (uint)video_buffer.Height;

      return 1;
    }

    //==========================================================================
    private static void Video_Cleanup(IntPtr opaque)
    {
      Debug.WriteLine("Video_Cleanup", "MediaPlayer");

      VideoBuffer video_buffer;
      m_VideoBuffers.TryRemove(opaque, out video_buffer);

      MediaPlayer media_player = GetMediaPlayer(opaque);
      if(media_player != null)
      {
        media_player.RaiseVideoCleanup();
        media_player.VideoBuffer = null;
      }
    }

    //==========================================================================
    private static IntPtr Video_Lock(IntPtr opaque, ref IntPtr planes)
    {
      Debug.WriteLine("Video_Lock", "MediaPlayer");

      planes = IntPtr.Zero;

      VideoBuffer video_buffer;
      m_VideoBuffers.TryGetValue(opaque, out video_buffer);
      if(video_buffer != null)
        planes = video_buffer.Lock();

      MediaPlayer media_player = GetMediaPlayer(opaque);
      if(media_player != null)
        media_player.RaiseVideoLock();

      return IntPtr.Zero;
    }

    //==========================================================================
    private static void Video_Unlock(IntPtr opaque, IntPtr picture, ref IntPtr planes)
    {
      Debug.WriteLine("Video_Unlock", "MediaPlayer");

      MediaPlayer media_player = GetMediaPlayer(opaque);
      if(media_player != null)
        media_player.RaiseVideoUnlock();

      VideoBuffer video_buffer;
      m_VideoBuffers.TryGetValue(opaque, out video_buffer);
      if(video_buffer != null)
        video_buffer.Unlock();
    }

    //==========================================================================
    private static void Video_Display(IntPtr opaque, IntPtr picture)
    {
      Debug.WriteLine("Video_Display", "MediaPlayer");

      MediaPlayer media_player = GetMediaPlayer(opaque);
      if(media_player != null)
        media_player.RaiseDisplay();
    }

    //==========================================================================
    private static void EventManager_Event(ref LibVLCLibrary.libvlc_event_t e, IntPtr userData)
    {
      Debug.WriteLine(String.Format("EventManager_Event({0})", e.type), "MediaPlayer");

      MediaPlayer media_player = GetMediaPlayer(e.p_obj);
      if(media_player == null)
        return;

      if(e.type == LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerPlaying)
      {
        // Ensure no more than one video tracks will be rendered...
        int track_count = media_player.m_Library.libvlc_video_get_track_count(media_player.m_MediaPlayerHandle);
        if(track_count > 2)
        {
          int video_track_index = media_player.m_Library.libvlc_video_get_track(media_player.m_MediaPlayerHandle);
          media_player.m_Library.libvlc_video_set_track(media_player.m_MediaPlayerHandle, -1);
          if(video_track_index > 0)
            media_player.m_Library.libvlc_video_set_track(media_player.m_MediaPlayerHandle, video_track_index - 1);
        }
      }

      media_player.RaiseEvent((MediaPlayerEvent)e.type);
    }

    #endregion Static Members

    //==========================================================================
    private LibVLCLibrary m_Library;
    private bool m_IsLibraryOwner;
    private Instance m_Instance;
    private bool m_IsInstanceOwner;
    private IntPtr m_MediaPlayerHandle;

    #region Construction and Destruction

    //==========================================================================
    private IntPtr CreateHandle()
    {
      IntPtr handle = m_Library.libvlc_media_player_new(m_Instance.Handle);
      if(handle == IntPtr.Zero)
        throw new LibVLCException(m_Library);
      RegisterMediaPlayer(handle, this);
      try
      {
        IntPtr media_player_event_manager = m_Library.libvlc_media_player_event_manager(handle);
        if(media_player_event_manager == IntPtr.Zero)
          throw new LibVLCException(m_Library);
        int event_index = 0;
        try
        {
          while(event_index < m_Events.Length)
            if(m_Library.libvlc_event_attach(media_player_event_manager, m_Events[event_index++], m_EventManagerEventCallback, IntPtr.Zero) != 0)
              throw new LibVLCException(m_Library);
        }
        catch
        {
          while(event_index > 0)
            m_Library.libvlc_event_attach(media_player_event_manager, m_Events[--event_index], m_EventManagerEventCallback, IntPtr.Zero);
          throw;
        }

        m_Library.libvlc_video_set_format_callbacks(handle, m_VideoFormatCallback, m_VideoCleanupCallback);
        m_Library.libvlc_video_set_callbacks(handle, m_VideoLockCallback, m_VideoUnlockCallback, m_VideoDisplayCallback);

        return handle;
      }
      catch
      {
        UnregisterMediaPlayer(handle);
        m_Library.libvlc_media_player_release(handle);
        throw;
      }
    }

    //==========================================================================
    public MediaPlayer(LibVLCLibrary library, Instance instance)
    {
      if(library != null)
        if(instance != null)
          if(instance.Library != library)
            throw new ArgumentException("The provided instance has not been created with the provided library!", "instance");
      if(instance != null)
        if(library == null)
          library = instance.Library;

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
        if(instance == null)
        {
          m_Instance = new Instance(m_Library);
          m_IsInstanceOwner = true;
        }
        else
        {
          m_Instance = instance;
          m_IsInstanceOwner = false;
        }
        try
        {
          m_MediaPlayerHandle = CreateHandle();
        }
        catch
        {
          if(m_IsInstanceOwner)
            m_Instance.Dispose();
          m_Instance = null;
          throw;
        }
      }
      catch
      {
        if(m_IsLibraryOwner)
          LibVLCLibrary.Free(m_Library);
        m_Library = null;
        throw;
      }
    }

    //==========================================================================
    public MediaPlayer()
      : this(null, null)
    {
      // ...
    }

    //==========================================================================
    ~MediaPlayer()
    {
      Debug.WriteLine("Destructing MediaPlayer...", "MediaPlayer");

      Dispose(false);
    }

    //==========================================================================
    private void Dispose(bool disposing)
    {
      if(m_MediaPlayerHandle != IntPtr.Zero)
      {
        m_Library.libvlc_media_player_release(m_MediaPlayerHandle);

        if(disposing)
          UnregisterMediaPlayer(m_MediaPlayerHandle);

        m_MediaPlayerHandle = IntPtr.Zero;
      }

      if(disposing)
      {
        if(m_Instance != null)
        {
          if(m_IsInstanceOwner)
            m_Instance.Dispose();
          m_Instance = null;
        }

        if(m_Library != null)
        {
          if(m_IsLibraryOwner)
            LibVLCLibrary.Free(m_Library);
          m_Library = null;
        }
      }
    }

    //==========================================================================
    /// <summary>
    ///   Disposes the media player.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    #endregion // Construction and Destruction

    #region Playback Controls

    //==========================================================================
    public void Play()
    {
      if(m_Library.libvlc_media_player_play(m_MediaPlayerHandle) != 0)
        throw new LibVLCException(m_Library);
    }

    //==========================================================================
    public void Stop()
    {
      m_Library.libvlc_media_player_stop(m_MediaPlayerHandle);
    }

    //==========================================================================
    public void Pause()
    {
      if(m_Library.libvlc_media_player_pause(m_MediaPlayerHandle) != 0)
        throw new LibVLCException(m_Library);
    }

    //==========================================================================
    public void NextFrame()
    {
      if(m_Library.libvlc_media_player_next_frame(m_MediaPlayerHandle) != 0)
        throw new LibVLCException(m_Library);
    }

    //==========================================================================
    public void PreviousChapter()
    {
      m_Library.libvlc_media_player_previous_chapter(m_MediaPlayerHandle);
    }

    //==========================================================================
    public void NextChapter()
    {
      m_Library.libvlc_media_player_next_chapter(m_MediaPlayerHandle);
    }

    //==========================================================================
    /// <summary>
    ///   Loads the subitem at the provided index.
    /// </summary>
    /// <param name="subitemIndex">
    ///   The zero-based index of the subitem to load; must be between <c>0</c>
    ///   and <see cref="SubitemCount"/>.
    /// </param>
    /// <remarks>
    ///   The current media of the media player will be replaced by the subitem
    ///   with the specified index.
    /// </remarks>
    public void LoadSubitem(int subitemIndex)
    {
      IntPtr media = m_Library.libvlc_media_player_get_media(m_MediaPlayerHandle);
      if(media != IntPtr.Zero)
        try
        {
          IntPtr subitems = m_Library.libvlc_media_subitems(media);
          if(subitems != IntPtr.Zero)
            try
            {
              IntPtr subitem = IntPtr.Zero;
              m_Library.libvlc_media_list_lock(subitems);
              try
              {
                int count = m_Library.libvlc_media_list_count(subitems);
                if((subitemIndex >= 0) && (subitemIndex < count))
                  subitem = m_Library.libvlc_media_list_item_at_index(subitems, 0);
              }
              finally
              {
                m_Library.libvlc_media_list_unlock(subitems);
              }

              if(subitem != IntPtr.Zero)
                try
                {
                  m_Library.libvlc_media_player_set_media(m_MediaPlayerHandle, subitem);
                }
                finally
                {
                  m_Library.libvlc_media_release(subitem);
                }
            }
            finally
            {
              m_Library.libvlc_media_list_release(subitems);
            }
        }
        finally
        {
          m_Library.libvlc_media_release(media);
        }
    }

    #endregion // Playback Controls

    #region Properties

    //==========================================================================
    private volatile VideoBuffer m_VideoBuffer = null;

    //==========================================================================
    public VideoBuffer VideoBuffer
    {
      get
      {
        return m_VideoBuffer;
      }

      private set
      {
        if(m_VideoBuffer != value)
          m_VideoBuffer = value;
      }
    }

    //==========================================================================                
    public bool IsSeekable
    {
      get
      {
        return m_Library.libvlc_media_player_is_seekable(m_MediaPlayerHandle) != 0;
      }
    }

    //==========================================================================                
    public bool CanPause
    {
      get
      {
        return m_Library.libvlc_media_player_can_pause(m_MediaPlayerHandle) != 0;
      }
    }

    //==========================================================================                
    public float FPS
    {
      get
      {
        return m_Library.libvlc_media_player_get_fps(m_MediaPlayerHandle);
      }
    }

    //==========================================================================                
    /// <summary>
    ///   Gets or sets the value of Location of the MediaPlayer.
    /// </summary>
    public Uri Location
    {
      get
      {
        IntPtr media = m_Library.libvlc_media_player_get_media(m_MediaPlayerHandle);
        if(media == IntPtr.Zero)
          return null;
        string mrl = m_Library.libvlc_media_get_mrl(media);
        m_Library.libvlc_media_release(media);
        if(mrl == null)
          return null;
        return new Uri(mrl);
      }

      set
      {
        if(value != Location)
        {
          IntPtr media = IntPtr.Zero;
          if(value != null)
          {
            if(value.IsAbsoluteUri)
            {
              if(value.IsFile)
                media = m_Library.libvlc_media_new_path(m_Instance.Handle, value.LocalPath);
              else
                media = m_Library.libvlc_media_new_location(m_Instance.Handle, value.AbsoluteUri);
            }
            else
              media = m_Library.libvlc_media_new_path(m_Instance.Handle, value.ToString());
            if(media == IntPtr.Zero)
              throw new LibVLCException(m_Library);
          }
          try
          {
            m_Library.libvlc_media_player_set_media(m_MediaPlayerHandle, media);
          }
          finally
          {
            if(media != IntPtr.Zero)
              m_Library.libvlc_media_release(media);
          }
        }
      }
    }

    //==========================================================================
    public TimeSpan Length
    {
      get
      {
        long value = m_Library.libvlc_media_player_get_length(m_MediaPlayerHandle);
        if(value <= 0)
          return TimeSpan.Zero;
        else
          return TimeSpan.FromMilliseconds(value);
      }
    }

    //==========================================================================                
    public VideoTrack[] VideoTracks
    {
      get
      {
        LibVLCLibrary.libvlc_media_track_info_t[] track_infos;
        IntPtr media = m_Library.libvlc_media_player_get_media(m_MediaPlayerHandle);
        if(media == IntPtr.Zero)
          return new VideoTrack[0];
        try
        {
          m_Library.libvlc_media_get_tracks_info(media, out track_infos);
        }
        finally
        {
          m_Library.libvlc_media_release(media);
        }

        LibVLCLibrary.libvlc_track_description_t track_description = m_Library.libvlc_video_get_track_description(m_MediaPlayerHandle);
        if(track_description != null)
          track_description = track_description.p_next; // The first one seems to be always the one for deactivating

        List<VideoTrack> result = new List<VideoTrack>();
        int index = 0;
        while(track_description != null)
        {
          LibVLCLibrary.libvlc_media_track_info_t? track_info = null;
          if(track_infos != null)
            for(int i = 0; i < track_infos.Length; ++i)
              if(track_infos[i].i_id == track_description.i_id)
              {
                track_info = track_infos[i];
                break;
              }

          result.Add(new VideoTrack(index, track_description, track_info));
          track_description = track_description.p_next;
          ++index;
        }

        // Is already done by libvlc_video_get_track_description, the invokation would do nothing though
        // m_Library.libvlc_track_description_list_release(track_description);

        return result.ToArray();
      }
    }

    //==========================================================================
    public int VideoTrackIndex
    {
      get
      {
        int result = m_Library.libvlc_video_get_track(m_MediaPlayerHandle);
        if(result < 0)
          return -1;
        return result - 1;
      }

      set
      {
        int count = m_Library.libvlc_video_get_track_count(m_MediaPlayerHandle);
        if(count > 2)
          m_Library.libvlc_video_set_track(m_MediaPlayerHandle, -1);

        m_Library.libvlc_video_set_track(m_MediaPlayerHandle, value);
      }
    }

    //==========================================================================                
    public AudioTrack[] AudioTracks
    {
      get
      {
        LibVLCLibrary.libvlc_media_track_info_t[] track_infos;
        IntPtr media = m_Library.libvlc_media_player_get_media(m_MediaPlayerHandle);
        if(media == IntPtr.Zero)
          return new AudioTrack[0];
        try
        {
          m_Library.libvlc_media_get_tracks_info(media, out track_infos);
        }
        finally
        {
          m_Library.libvlc_media_release(media);
        }

        LibVLCLibrary.libvlc_track_description_t track_description = m_Library.libvlc_audio_get_track_description(m_MediaPlayerHandle);
        if(track_description != null)
          track_description = track_description.p_next; // The first one seems to be always the one for deactivating

        List<AudioTrack> result = new List<AudioTrack>();
        int index = 0;
        while(track_description != null)
        {
          LibVLCLibrary.libvlc_media_track_info_t? track_info = null;
          if(track_infos != null)
            for(int i = 0; i < track_infos.Length; ++i)
              if(track_infos[i].i_id == track_description.i_id)
              {
                track_info = track_infos[i];
                break;
              }

          result.Add(new AudioTrack(index, track_description, track_info));
          track_description = track_description.p_next;
          ++index;
        }

        // Is already done by libvlc_video_get_track_description, the invokation would do nothing though
        // m_Library.libvlc_track_description_list_release(track_description);

        return result.ToArray();
      }
    }

    //==========================================================================
    public int AudioTrackIndex
    {
      get
      {
        int result = m_Library.libvlc_audio_get_track(m_MediaPlayerHandle);
        if(result < 0)
          return -1;
        return result - 1;
      }

      set
      {
        m_Library.libvlc_audio_set_track(m_MediaPlayerHandle, value + 1);
      }
    }

    //==========================================================================                
    public SubtitleTrack[] SubtitleTracks
    {
      get
      {
        LibVLCLibrary.libvlc_media_track_info_t[] track_infos;
        IntPtr media = m_Library.libvlc_media_player_get_media(m_MediaPlayerHandle);
        if(media == IntPtr.Zero)
          return new SubtitleTrack[0];
        try
        {
          m_Library.libvlc_media_get_tracks_info(media, out track_infos);
        }
        finally
        {
          m_Library.libvlc_media_release(media);
        }

        LibVLCLibrary.libvlc_track_description_t track_description = m_Library.libvlc_video_get_spu_description(m_MediaPlayerHandle);
        if(track_description != null)
          track_description = track_description.p_next; // The first one seems to be always the one for deactivating

        List<SubtitleTrack> result = new List<SubtitleTrack>();
        int index = 0;
        while(track_description != null)
        {
          LibVLCLibrary.libvlc_media_track_info_t? track_info = null;
          if(track_infos != null)
            for(int i = 0; i < track_infos.Length; ++i)
              if(track_infos[i].i_id == track_description.i_id)
              {
                track_info = track_infos[i];
                break;
              }

          result.Add(new SubtitleTrack(index, track_description, track_info));
          track_description = track_description.p_next;
          ++index;
        }

        // Is already done by libvlc_video_get_track_description, the invokation would do nothing though
        // m_Library.libvlc_track_description_list_release(track_description);

        return result.ToArray();
      }
    }

    //==========================================================================
    public int SubtitleTrackIndex
    {
      get
      {
        int result = m_Library.libvlc_video_get_spu(m_MediaPlayerHandle);
        if(result < 0)
          return -1;
        return result - 1;
      }

      set
      {
        m_Library.libvlc_video_set_spu(m_MediaPlayerHandle, value + 1);
      }
    }

    //==========================================================================
    public TimeSpan Time
    {
      get
      {
        long value = m_Library.libvlc_media_player_get_time(m_MediaPlayerHandle);
        if(value <= 0)
          return TimeSpan.Zero;
        else
          return TimeSpan.FromMilliseconds(value);
      }

      set
      {
        m_Library.libvlc_media_player_set_time(m_MediaPlayerHandle, (long)value.TotalMilliseconds);
      }
    }

    //==========================================================================
    public int ChapterCount
    {
      get
      {
        return m_Library.libvlc_media_player_get_chapter_count(m_MediaPlayerHandle);
      }
    }

    //==========================================================================
    public int Chapter
    {
      get
      {
        return m_Library.libvlc_media_player_get_chapter(m_MediaPlayerHandle);
      }

      set
      {
        m_Library.libvlc_media_player_set_chapter(m_MediaPlayerHandle, value);
      }
    }


    //==========================================================================
    public int Volume
    {
      get
      {
        return m_Library.libvlc_audio_get_volume(m_MediaPlayerHandle);
      }

      set
      {
        m_Library.libvlc_audio_set_volume(m_MediaPlayerHandle, value);
      }
    }

    //==========================================================================
    /// <summary>
    ///   Gets the current state of the media player.
    /// </summary>
    public MediaPlayerState State
    {
      get
      {
        return (MediaPlayerState)m_Library.libvlc_media_player_get_state(m_MediaPlayerHandle);
      }
    }

    //==========================================================================
    /// <summary>
    ///   Gets the number of subitems of the current media.
    /// </summary>
    /// <seealso cref="LoadSubitem"/>
    public int SubitemCount
    {
      get
      {
        IntPtr media = m_Library.libvlc_media_player_get_media(m_MediaPlayerHandle);
        if(media == IntPtr.Zero)
          return 0;
        try
        {
          IntPtr subitems = m_Library.libvlc_media_subitems(media);
          if(subitems == IntPtr.Zero)
            return 0;
          try
          {
            m_Library.libvlc_media_list_lock(subitems);
            try
            {
              return m_Library.libvlc_media_list_count(subitems);
            }
            finally
            {
              m_Library.libvlc_media_list_unlock(subitems);
            }
          }
          finally
          {
            m_Library.libvlc_media_list_release(subitems);
          }
        }
        finally
        {
          m_Library.libvlc_media_release(media);
        }
      }
    }

    #endregion // Properties

    #region Events

    #region VideoFormat

    //==========================================================================
    private void RaiseVideoFormat()
    {
      EventHandler video_format = VideoFormat;
      if(video_format != null)
        video_format(this, EventArgs.Empty);
    }

    //==========================================================================
    /// <summary>
    ///   Will be called after the contents of the video buffer has been 
    ///   changed.
    /// </summary>
    public event EventHandler VideoFormat;

    #endregion // VideoFormat

    #region VideoLock

    //==========================================================================
    private void RaiseVideoLock()
    {
      EventHandler video_lock = VideoLock;
      if(video_lock != null)
        video_lock(this, EventArgs.Empty);
    }

    //==========================================================================
    /// <summary>
    ///   Will be called after the contents of the video buffer has been 
    ///   changed.
    /// </summary>
    public event EventHandler VideoLock;

    #endregion // VideoLock

    #region Display

    //==========================================================================
    private void RaiseDisplay()
    {
      EventHandler display = Display;
      if(display != null)
        display(this, EventArgs.Empty);
    }

    //==========================================================================
    /// <summary>
    ///   Will be called after the contents of the video buffer has been 
    ///   changed.
    /// </summary>
    public event EventHandler Display;

    #endregion // Display

    #region VideoUnlock

    //==========================================================================
    private void RaiseVideoUnlock()
    {
      EventHandler video_unlock = VideoUnlock;
      if(video_unlock != null)
        video_unlock(this, EventArgs.Empty);
    }

    //==========================================================================
    /// <summary>
    ///   Will be called after the contents of the video buffer has been 
    ///   changed.
    /// </summary>
    public event EventHandler VideoUnlock;

    #endregion // VideoUnlock

    #region VideoCleanup

    //==========================================================================
    private void RaiseVideoCleanup()
    {
      EventHandler video_format = VideoCleanup;
      if(video_format != null)
        video_format(this, EventArgs.Empty);
    }

    //==========================================================================
    /// <summary>
    ///   Will be called after the contents of the video buffer has been 
    ///   changed.
    /// </summary>
    public event EventHandler VideoCleanup;

    #endregion // VideoCleanup

    #region Event

    //==========================================================================
    private void RaiseEvent(MediaPlayerEvent mediaPlayerEvent)
    {
      MediaPlayerEventHandler event_handler = Event;
      if(event_handler != null)
        event_handler(this, new MediaPlayerEventArgs(mediaPlayerEvent));
    }

    //==========================================================================
    public event MediaPlayerEventHandler Event;

    #endregion // Event

    #endregion // Events

  } // class MediaPlayer

}
