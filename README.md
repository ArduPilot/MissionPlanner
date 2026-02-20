# MissionPlanner

![Dot Net](https://github.com/ardupilot/missionplanner/actions/workflows/main.yml/badge.svg) ![Android](https://github.com/ardupilot/missionplanner/actions/workflows/android.yml/badge.svg) ![OSX/IOS](https://github.com/ardupilot/missionplanner/actions/workflows/mac.yml/badge.svg)

## Table of Contents
- [How to Compile](#how-tocompile)
- [Launching Mission Planner on other Operating systems](launching-mission-planner-on-other-system)
- [External Services Used](#external-services-used)

Website : http://ardupilot.org/planner/

Forum : http://discuss.ardupilot.org/c/ground-control-software/mission-planner

Download latest stable version : http://firmware.ardupilot.org/Tools/MissionPlanner/MissionPlanner-latest.msi

Changelog : https://github.com/ArduPilot/MissionPlanner/blob/master/ChangeLog.txt

License : https://github.com/ArduPilot/MissionPlanner/blob/master/COPYING.txt


## How To Compile

### On Windows (Recommended)

#### 1. Install software

##### Requirements

Recommended IDE - Visual Studio 2022

### Visual Studio Community
You can download Visual Studio Community from the [Visual Studio Download page](https://visualstudio.microsoft.com/downloads/ "Visual Studio Download page").

Visual Studio is a comprehensive suite with built-in Git support. You can customize your installation by selecting the relevant "Workloads" and "Individual components" based on your software development needs. (You can always download them afterwards so don't worry.)

We have provided a configuration file that specifies the components required for MissionPlanner development. Here's how you can use it:

1. Go to "More" in the Visual Studio installer.
2. Select "Import configuration."
3. Use the following file: [vs2022.vsconfig](https://raw.githubusercontent.com/ArduPilot/MissionPlanner/master/vs2022.vsconfig "vs2022.vsconfig").

By following these steps, you'll have the necessary components installed and ready for Mission Planner development.

###### VSCode
Currently VSCode with C# plugin is able to parse the code but cannot build.

#### 2. Get the code

If you get Visual Studio Community, you should be able to use Git from the IDE. 
Clone `https://github.com/ArduPilot/MissionPlanner.git` to get the full code.

To do clone the following repo by:
- `git clone https://github.com/ArduPilot/MissionPlanner.git`

In case you didn't install an IDE, you will need to manually install Git. Please follow instruction in https://ardupilot.org/dev/docs/where-to-get-the-code.html#downloading-the-code-using-git

Open a git bash terminal in the MissionPlanner directory and type, "git submodule update --init" to download all submodules

#### 3. Build

To build the code:
- Download MissionPlanner.sln to your computer
- Right clock MissionPlanner.sln to open with Visual Studio
- From the Build menu, select "Build MissionPlanner"

### On other operating systems
Building Mission Planner on other systems isn't support currently.

## Launching Mission Planner on other Operating systems

Mission Planner is available for Android via the Play Store. https://play.google.com/store/apps/details?id=com.michaeloborne.MissionPlanner
Mission Planner can be used with Mono on Linux systems. Be aware that not all functions are available on Linux.
Native MacOS and iOS support is experimental and not recommended for inexperienced users. https://github.com/ArduPilot/MissionPlanner/releases/tag/osxlatest 
For MacOS users it is recommended to use Mission Planner for Windows via Boot Camp or Parallels (or equivalent).

### On Linux

#### Requirements

Those instructions were tested on Ubuntu 20.04.
Please install Mono, either :
- `sudo apt install mono-complete mono-runtime libmono-system-windows-forms4.0-cil libmono-system-core4.0-cil libmono-winforms4.0-cil libmono-corlib4.0-cil libmono-system-management4.0-cil libmono-system-xml-linq4.0-cil`

If you it says sudo command not found, run the following code to install sudo:
- `apt install sudo`

#### Launching

- Get the lastest zipped version of Mission Planner here : https://firmware.ardupilot.org/Tools/MissionPlanner/MissionPlanner-latest.zip
- Unzip in the directory you want
- Go into the directory
- Run with `mono MissionPlanner.exe`

You can debug Mission Planner on Mono with `MONO_LOG_LEVEL=debug mono MissionPlanner.exe`

## External Services Used

| Source | Use | How to disable | Custodian |
|---|---|---|---|
| https://firmware.oborne.me  | used as a global cdn for checking for MP update check - checked once per day at startup | edit missionplanner.exe.config | Michael Oborne |
| https://firmware.ardupilot.org  | used for updates to stable, firmware metadata, firmware, user alerts, gstreamer, SRTM, SITL | updates to stable (edit missionplanner.exe.config) - all others Not possible | Ardupilot Team |
| https://github.com/ | used for updates to beta | edit missionplanner.exe.config | Michael Oborne |
| https://raw.githubusercontent.com | old param metadata, sitl config files | Not possible | Ardupilot Team |
| https://api.github.com/ | ardupilot preload param files | Not possible | Ardupilot Team |
| https://raw.oborne.me/  | used as glocal cdn for parameter metadata generator, no longer primary source | only used at user request to regenerate, edit missionplanner.exe.config | Michael Oborne |
| https://maps.google.com  | used for elevation api - removed due to abuse | N/A | N/A |
| https://discuss.cubepilot.org/ | use for SB2 reporting - only on affected boards when user enters details | only used at user request | CubePilot |
| https://altitudeangel.com  | utm data - user enabled | only used at user request | Altitude Angel |
| https://autotest.ardupilot.org  | dataflash log meta data, parameter metadata | Not Possible | Ardupilot Team |
| Many | your choice of map provider google/bing/openstreetmap/etc | User selectable | User/Many |
| https://www.cloudflare.com | geo location provider - for NFZ selection | Not Possible | Michael Oborne |
| https://esua.cad.gov.hk | HK no fly zones - user enabled | User selectable | HK Gov |
| https://ssl.google-analytics.com | Google Analytics Anonymous Stats - Screen Loads, Exceptions/Crashs, Events (Connect), Startup Timing, FW upload (FW Type and Board Type) | disable in Config > Planner > OptOut Anon Stats | Michael Oborne |
| https://api.dronelogbook.com | logging - disabled | N/A | N/A |
| https://ardupilot.org | help urls on many pages | User Initiated | ArduPilot Team |
| https://www.youtube.com | help videos on many pages | User Initiated | ArduPilot Team |
| https://files.rfdesign.com.au | RFD firmwares | User Initiated | RFDesign |
| https://teck.airmarket.io | airmarket - disabled | N/A | N/A |

## Offline Use - No Internet

| Location | Use | Transferable between pcs |
|---|---|---|
| C:\ProgramData\Mission Planner\gmapcache | Map cache | yes |
| C:\ProgramData\Mission Planner\srtm | Elevation data cache | yes |
| C:\ProgramData\Mission Planner\\*.pdef.xml | Parameter cache | yes |
| C:\ProgramData\Mission Planner\LogMessages*.xml | DF Log metadata cache | yes |

on linux this is in /home/<user>/.local/share/Mission Planner/

### Offline Data Supported
#### Elevation
* SRTM Cache
* GeoTiff's in WGS84/EGM96
* DTED

#### Images
* Map Cache
* WMS
* WMTS
* GDAL

### Paths used - Default

| Location | Use |
|---|---|
| C:\ProgramData\Mission Planner | All cross user content |
| C:\Users\USERNAME\Documents\Mission Planner | All per user content |

on linux this is in /home/<user>/.local/share/Mission Planner/

### CA Cert
A CA cert is installed to the root store and used to sign the windows serial port drivers, and is installed as part of the MSI install.

[![FlagCounter](https://s01.flagcounter.com/count2/A4bA/bg_FFFFFF/txt_000000/border_CCCCCC/columns_8/maxflags_40/viewers_0/labels_1/pageviews_0/flags_0/percent_0/)](https://info.flagcounter.com/A4bA)
