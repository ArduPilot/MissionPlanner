// <copyright file="NamespaceDoc.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>
    ///     SharpAdbClient is a .NET library that allows.NET applications to communicate with Android devices.
    ///     It provides a.NET implementation of the <c>adb</c> protocol, giving more flexibility to the developer than launching an
    ///     <c>adb.exe</c> process and parsing the console output.
    /// </para>
    ///
    /// <para>
    ///     Most of the adb functionality is exposed through the <see cref="AdbClient"/> class.
    ///     You can create your own instance of that class, or just use the instance we provide for you at
    ///     <see cref="SharpAdbClient.AdbClient.Instance"/>
    /// </para>
    ///
    /// <para>
    ///     To send and receive files to and from Android devices, you can use the <see cref="SyncService"/> class.
    /// </para>
    /// </summary>
    ///
    /// <example>
    /// <para>
    ///     To list all Android devices that are connected to your PC, you can use the following code:
    /// </para>
    ///
    /// <code>
    /// var devices = devices = AdbClient.Instance.GetDevices();
    ///
    /// foreach(var device in devices)
    /// {
    ///     Console.WriteLine(device.Name
    /// }
    /// </code>
    /// </example>
    [CompilerGeneratedAttribute]
    internal class NamespaceDoc
    {
    }
}
