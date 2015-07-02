MissionPlanner
==============

[![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/diydrones/MissionPlanner?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

Website : http://planner.ardupilot.com/  
  
Forum : http://ardupilot.com/forum/viewforum.php?f=12  
  
Download latest stable version : http://ardupilot.com/wp-content/plugins/download-monitor/download.php?id=82  
  
Changelog : https://github.com/diydrones/MissionPlanner/blob/master/ChangeLog.txt  
  
License : https://github.com/diydrones/MissionPlanner/blob/master/COPYING.txt  


How to compile
==============

1. Install software

- Git
  http://code.google.com/p/msysgit/downloads/list
  Select a file summarized as "Full installer for official Git for Windows"
   with the highest version
- TortuiseGit
  http://code.google.com/p/tortoisegit/wiki/Download
- Visual Studio
  http://www.visualstudio.com/downloads/download-visual-studio-vs
  Select "Visual Studio Express 2013 for Windows Desktop"
- DirectX Redist
  http://www.microsoft.com/en-us/download/details.aspx?id=35
- Microsoft .NET 4.0

2. Check out

- Create an empty folder anywhere
- In explorer left click and select "Git Clone"
  set URL https://github.com/diydrones/MissionPlanner
  OK

3. Build

- Open ArdupilotMega.sln with Visual Studio express 2013 for windows desktop.
- Compile.


-----------MONO-------------
run using 
mono MissionPlanner.exe

run debuging
MONO_LOG_LEVEL=debug mono MissionPlanner.exe

you need prereq's
sudo apt-get install mono-runtime libmono-system-windows-forms4.0-cil libmono-system-core4.0-cil libmono-winforms2.0-cil libmono-corlib2.0-cil libmono-system-management4.0-cil libmono-system-xml-linq4.0-cil


