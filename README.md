# MissionPlanner

![Dot Net](https://github.com/ardupilot/missionplanner/actions/workflows/main.yml/badge.svg) ![Android](https://github.com/ardupilot/missionplanner/actions/workflows/android.yml/badge.svg) ![OSX/IOS](https://github.com/ardupilot/missionplanner/actions/workflows/mac.yml/badge.svg)

Website : http://ardupilot.org/planner/

Forum : http://discuss.ardupilot.org/c/ground-control-software/mission-planner

Download latest stable version : http://firmware.ardupilot.org/Tools/MissionPlanner/MissionPlanner-latest.msi

Changelog : https://github.com/ArduPilot/MissionPlanner/blob/master/ChangeLog.txt

License : https://github.com/ArduPilot/MissionPlanner/blob/master/COPYING.txt


## How to compile

### On Windows (Recommended)

#### 1. Install software

##### Main requirements

Currently, Mission Planner needs:

Visual Studio 2022

##### IDE

###### Visual Studio Community
The recommended way to compile Mission Planner is through Visual Studio.
You could do it with Visual Studio Community [Visual Studio Download page](https://visualstudio.microsoft.com/downloads/ "Visual Studio Download page").
Visual Studio suite is quite complex and comes with Git support. During the Selection phase, please goto More > import configuration, and use the file (https://raw.githubusercontent.com/ArduPilot/MissionPlanner/master/vs2022.vsconfig "vs2022.vsconfig")

###### VSCode
Currently VSCode with C# plugin is able to parse the code but cannot build.

#### 2. Get the code

If you get Visual Studio Community, you should be able to use Git from the IDE. 
Just clone `https://github.com/ArduPilot/MissionPlanner.git` to get the full code.

In case you didn't install an IDE, you will need to manually install Git. Please follow instruction in https://ardupilot.org/dev/docs/where-to-get-the-code.html#downloading-the-code-using-git

#### 3. Build

To build the code:
- Open MissionPlanner.sln with Visual Studio
- Compile just the MissionPlanner project

### On other systems
Building Mission Planner on other systems isn't support currently.

## Launching Mission Planner on other system

Mission Planner is available for Android via the Play Store. https://play.google.com/store/apps/details?id=com.michaeloborne.MissionPlanner
Mission Planner can be used with Mono on Linux systems. Be aware that not all functions are available on Linux.
Native MacOS and iOS support is experimental and not recommended for inexperienced users. https://github.com/ArduPilot/MissionPlanner/releases/tag/osxlatest 
For MacOS users it is recommended to use Mission Planner for Windows via Boot Camp or Parallels (or equivalent).

### On Linux

#### Requirements

Those instructions were tested on Ubuntu 18.04.
Please install Mono, either :
- ` sudo apt install mono-runtime libmono-system-windows-forms4.0-cil libmono-system-core4.0-cil libmono-winforms4.0-cil libmono-corlib4.0-cil libmono-system-management4.0-cil libmono-system-xml-linq4.0-cil`

or full Mono :
- `sudo apt install mono-complete`

#### Launching

- Get the lastest zipped version of Mission Planner here : https://firmware.ardupilot.org/Tools/MissionPlanner/MissionPlanner-latest.zip
- Unzip in the directory you want
- Go into the directory
- run with `mono MissionPlanner.exe`

You can debug Mission Planner on Mono with `MONO_LOG_LEVEL=debug mono MissionPlanner.exe`

### External Services Used

| Source | Use |
|---|---|
| https://firmware.oborne.me  | used as a global cdn for checking for MP updates - checked once per day |
| https://firmware.ardupilot.org  | used for updates to stable, firmware metadata, user alerts, gstreamer, SRTM |
| https://github.com/ | used for updates to beta |
| https://raw.githubusercontent.com | old param metadata |
| https://api.github.com/ | ardupilot param files |
| https://raw.oborne.me/  | used as glocal cdn for parameter metadata generator, no longer primary source - autotest is |
| https://maps.google.com  | used for elevation api - removed due to abuse |
| https://discuss.cubepilot.org/ | use for SB2 reporting - only on affected boards when user enters details |
| https://altitudeangel.com  | utm data - user enabled |
| https://autotest.ardupilot.org  | dataflash log meta data, parameter metadata |
| Many | your choice of map provider google/bing/openstreetmap/etc |
| https://www.cloudflare.com | geo location provider - for NFZ selection |
| https://esua.cad.gov.hk | HK no fly zones - user enabled |
| https://ssl.google-analytics.com | google analytics - disable in Config > Planner > OptOut Anon Stats |
| https://api.dronelogbook.com | logging - disabled |
| https://ardupilot.org | help urls on many pages |
| https://www.youtube.com | help videos on many pages |
| https://files.rfdesign.com.au | RFD firmwares |
| https://teck.airmarket.io | airmarket - disabled |


[![FlagCounter](https://s01.flagcounter.com/count2/A4bA/bg_FFFFFF/txt_000000/border_CCCCCC/columns_8/maxflags_40/viewers_0/labels_1/pageviews_0/flags_0/percent_0/)](https://info.flagcounter.com/A4bA)
