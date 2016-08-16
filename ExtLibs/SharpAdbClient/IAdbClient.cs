// <copyright file="IAdbClient.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using Logs;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A common interface for any class that allows you to interact with the
    /// adb server and devices that are connected to that adb server.
    /// </summary>
    public interface IAdbClient
    {
        // The individual services are listed in the same order as
        // https://android.googlesource.com/platform/system/core/+/master/adb/SERVICES.TXT

        /// <include file='IAdbClient.xml' path='/IAdbClient/GetAdbVersion/*'/>
        int GetAdbVersion();

        /// <include file='IAdbClient.xml' path='/IAdbClient/KillAdb/*'/>
        void KillAdb();

        /// <include file='IAdbClient.xml' path='/IAdbClient/GetDevices/*'/>
        List<DeviceData> GetDevices();

        // host:track-devices is implemented by the DeviceMonitor.
        // host:emulator is not implemented

        // host:transport-usb is not implemented
        // host:transport-local is not implemented
        // host:transport-any is not implemented

        // <host-prefix>:get-product is not implemented
        // <host-prefix>:get-serialno is not implemented
        // <host-prefix>:get-devpath is not implemented
        // <host-prefix>:get-state is not implemented

        /// <include file='IAdbClient.xml' path='/IAdbClient/CreateForward/*'/>
        void CreateForward(DeviceData device, string local, string remote, bool allowRebind);

        /// <include file='IAdbClient.xml' path='/IAdbClient/CreateForward/*'/>
        void CreateForward(DeviceData device, ForwardSpec local, ForwardSpec remote, bool allowRebind);

        /// <include file='IAdbClient.xml' path='/IAdbClient/RemoveForward/*'/>
        void RemoveForward(DeviceData device, int localPort);

        /// <include file='IAdbClient.xml' path='/IAdbClient/RemoveAllForwards/*'/>
        void RemoveAllForwards(DeviceData device);

        /// <include file='IAdbClient.xml' path='/IAdbClient/ListForward/*'/>
        IEnumerable<ForwardData> ListForward(DeviceData device);

        /// <include file='IAdbClient.xml' path='/IAdbClient/ExecuteRemoteCommand/*'/>
        void ExecuteRemoteCommand(string command, DeviceData device, IShellOutputReceiver receiver, CancellationToken cancellationToken, int maxTimeToOutputResponse);

        // shell: not implemented
        // remount: not implemented
        // dev:<path> not implemented
        // tcp:<port> not implemented
        // tcp:<port>:<server-name> not implemented
        // local:<path> not implemented
        // localreserved:<path> not implemented
        // localabstract:<path> not implemented

        // jdwp:<pid>: not implemented
        // track-jdwp: not implemented
        // sync: not implemented
        // reverse:<forward-command>: not implemented

        /// <include file='IAdbClient.xml' path='/IAdbClient/RunLogService/*'/>
        IEnumerable<LogEntry> RunLogService(DeviceData device, params LogId[] logNames);

        /// <include file='IAdbClient.xml' path='/IAdbClient/Reboot/*'/>
        void Reboot(string into, DeviceData device);

        /// <include file='IAdbClient.xml' path='/IAdbClient/Connect/*'/>
        string Connect(DnsEndPoint endpoint);

        /// <include file='IAdbClient.xml' path='/IAdbClient/SetDevice/*'/>
        void SetDevice(IAdbSocket socket, DeviceData device);
    }
}
