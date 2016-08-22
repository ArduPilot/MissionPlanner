// <copyright file="ISyncService.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    /// <summary>
    /// Interface containing methods for file synchronisation.
    /// </summary>
    public interface ISyncService : IDisposable
    {
        /// <include file='.\ISyncService.xml' path='/SyncService/IsOpen/*'/>
        bool IsOpen { get; }

        /// <include file='.\ISyncService.xml' path='/SyncService/Push/*'/>
        void Push(Stream stream, string remotePath, int permissions, DateTime timestamp, CancellationToken cancellationToken);

        /// <include file='.\ISyncService.xml' path='/SyncService/Pull/*'/>
        void Pull(string remotePath, Stream stream, CancellationToken cancellationToken);

        /// <include file='.\ISyncService.xml' path='/SyncService/Stat/*'/>
        FileStatistics Stat(string remotePath);

        /// <include file='.\ISyncService.xml' path='/SyncService/GetDirectoryListing/*'/>
        IEnumerable<FileStatistics> GetDirectoryListing(string remotePath);

        /// <include file='.\ISyncService.xml' path='/SyncService/Open/*'/>
        void Open();
    }
}
