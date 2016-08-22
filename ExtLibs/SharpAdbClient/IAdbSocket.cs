// <copyright file="IAdbSocket.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using SharpAdbClient.Exceptions;
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a common interface for any class that acts as a client for the
    /// Android Debug Bridge.
    /// </summary>
    public interface IAdbSocket : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether the<see cref="AdbSocket"/> is connected to a remote
        /// host as of the latest send or receive operation.
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// Reconnects the <see cref="IAdbSocket"/> to the same endpoint it was initially connected to.
        /// Use this when the socket was disconnected by adb and you have restarted adb.
        /// </summary>
        void Reconnect();

        /// <summary>
        /// Sends a request to the Android Debug Bridge.To read the response, call
        /// <see cref="ReadAdbResponse()"/>.
        /// </summary>
        /// <param name = "request" >
        /// The request to send.
        /// </param>
        void SendAdbRequest(string request);

        /// <summary>
        /// Sends the specified number of bytes of data to a <see cref="IAdbSocket"/>,
        /// </summary>
        /// <param name="data">
        /// A <see cref="byte"/> array that acts as a buffer, containing the data to send.
        /// </param>
        /// <param name = "length" >
        /// The number of bytes to send.
        /// </param>
        void Send(byte[] data, int length);

        /// <summary>
        /// Sends a sync request to the device.
        /// </summary>
        /// <param name="command" >
        /// The command to send.
        /// </param>
        /// <param name="path">
        ///  The path of the file on which the command should operate.
        /// </param>
        /// <param name="permissions" >
        ///  If the command is a <see cref="SyncCommand.SEND"/> command, the permissions
        ///  to assign to the newly created file.
        /// </param>
        void SendSyncRequest(SyncCommand command, string path, int permissions);

        /// <summary>
        /// Sends a sync request to the device.
        /// </summary>
        /// <param name="command">
        /// The command to send.
        /// </param>
        /// <param name="length">
        /// The length of the data packet that follows.
        /// </param>
        void SendSyncRequest(SyncCommand command, int length);

        /// <summary>
        /// Sends a sync request to the device.
        /// </summary>
        /// <param name="command">
        /// The command to send.
        /// </param>
        /// <param name="path">
        /// The path of the file on which the command should operate.
        /// </param>
        void SendSyncRequest(SyncCommand command, string path);

        /// <summary>
        /// Reads the response to a sync command.
        /// </summary>
        /// <returns>
        /// The response that was sent by the device.
        /// </returns>
        SyncCommand ReadSyncResponse();

        /// <summary>
        /// Receives an <see cref="AdbResponse"/> message, and throws an error
        /// if the message does not indicate success.
        /// </summary>
        /// <returns>
        /// A <see cref="AdbResponse"/> object that represents the response from the
        /// Android Debug Bridge.
        /// </returns>
        AdbResponse ReadAdbResponse();

        /// <summary>
        /// Reads from the socket until the array is filled, or no more data is coming(because
        /// the socket closed or the timeout expired).
        /// </summary>
        /// <param name="data" >
        /// The buffer to store the read data into.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the data was read successfully; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This uses the default time out value.
        /// </remarks>
        void Read(byte[] data);

        /// <summary>
        /// Reads a <see cref="string"/> from an <see cref="IAdbSocket"/> instance.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/> received from the <see cref = "IAdbSocket"/>.
        /// </returns>
        string ReadString();

        /// <summary>
        /// Reads a <see cref="string"/> from an <see cref="IAdbSocket"/> instance when
        /// the connection is in sync mode.
        /// </summary>
        /// <param name="length">
        /// The lenght of the string to retrieve
        /// </param>
        /// <returns>
        /// The <see cref="string"/> received from the <see cref = "IAdbSocket"/>.
        /// </returns>
        string ReadSyncString(int length);

        /// <summary>
        /// Reads the length field from an <see cref="IAdbSocket"/> instance when
        /// the connection is in sync mode.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/> representing the length received from the <see cref = "IAdbSocket"/>.
        /// </returns>
        int ReadSyncLength();

        /// <summary>
        /// Closes the<see cref="AdbSocket"/> connection and releases all associated resources.
        /// </summary>
        void Close();

        /// <summary>
        /// Reads from the socket until the array is filled, the optional<paramref name= "length"/>
        /// is reached, or no more data is coming (because the socket closed or the
        /// timeout expired).
        /// </summary>
        /// <param name="data">
        /// The buffer to store the read data into.
        /// </param>
        /// <param name="length">
        /// The length to read or -1 to fill the data buffer completely
        /// </param>
        /// <exception cref = "AdbException" >
        /// EOF
        /// or
        /// No Data to read: exception.Message
        /// </exception>
        void Read(byte[] data, int length);

        /// <summary>
        /// Gets a <see cref="Stream"/> that can be used to send and receive shell output to and
        /// from the device.
        /// </summary>
        /// <returns>
        /// A <see cref="Stream"/> that can be used to send and receive shell output to and
        /// from the device.
        /// </returns>
        Stream GetShellStream();
    }
}
