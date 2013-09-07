//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/Properties/AssemblyInfo.cs $
// Last changed by    : $LastChangedBy: sh $
// Revision           : $LastChangedRevision: 255 $
// Last changed date  : $LastChangedDate: 2011-04-30 11:15:00 +0200 (Sat, 30. Apr 2011) $
// Author             : $Author: sh $
// Copyright	      : Copyight (c) 2009-2011 Steffen Habermehl
// Contact            : geoutility@freenet.de
// License            : GNU GENERAL PUBLIC LICENSE Ver. 3, GPLv3
//                      This program is free software; you can redistribute it and/or modify it under the terms 
//                      of the GNU General Public License as published by the Free Software Foundation; 
//                      either version 3 of the License, or (at your option) any later version. 
//                      This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
//                      without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
//                      See the GNU General Public License for more details. You should have received a copy of the 
//                      GNU General Public License along with this program; if not, see <http://www.gnu.org/licenses/>. 
//
// File description   : Assemblyinformationen und Direktiven
//===================================================================================================


using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

// Beschreibung
#if COMPACT_FRAMEWORK
    [assembly: AssemblyTitle("GeoUtility Library for Mobile 5")]
    [assembly: AssemblyDescription("Geographic Coordinate Conversion Library Mobile")]
    [assembly: AssemblyProduct("GeoUtility Library for Mobile Devices")]
    [assembly: AssemblyConfiguration("Retail")]
    [assembly: AssemblyCompany("Steffen Habermehl")]
    [assembly: AssemblyCopyright("Copyright © 2008-2010 (geoutility@freenet.de)")]
    [assembly: AssemblyTrademark("GeoUtility Library")]
    [assembly: AssemblyCulture("")]
#else
    [assembly: AssemblyTitle("GeoUtility Library")]
    [assembly: AssemblyDescription("Geographic Coordinate Conversion Library")]
    [assembly: AssemblyProduct("GeoUtility Library")]
    [assembly: AssemblyConfiguration("Retail")]
    [assembly: AssemblyCompany("Steffen Habermehl")]
    [assembly: AssemblyCopyright("Copyright © 2008-2010 (geoutility@freenet.de)")]
    [assembly: AssemblyTrademark("GeoUtility Library")]
    [assembly: AssemblyCulture("")]
#endif

// COM
[assembly: ComVisible(false)]
[assembly: Guid("a1906fef-31f7-43ab-a8fa-24ade7bbdfef")]

// Version
[assembly: AssemblyVersion("3.1.7.5")]

#if COMPACT_FRAMEWORK
#else
[assembly: AssemblyFileVersion("3.1.7.5")]

 //Sicherheit
[assembly: AllowPartiallyTrustedCallers()]

 //Sonstiges
[assembly: SecurityPermission(SecurityAction.RequestMinimum, UnmanagedCode = false)]
#endif
