
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
- DirectInput fix
  http://www.microsoft.com/en-us/download/details.aspx?id=35

2. Check out

- Create an empty folder anywhere
- In explorer right click and select "Git Clone"
  set URL https://github.com/diydrones/MissionPlanner

3. Build

- Open ArdupilotMega.sln with Visual Studio 2010-2013
- Follow compile1.jpg in the code tree

4. Contribute

- See Readme_pullrequest.txt

-----------MONO-------------
run using 
mono MissionPlanner.exe

run debuging
MONO_LOG_LEVEL=debug mono MissionPlanner.exe

you need prereq's
sudo apt-get install mono-runtime libmono-system-windows-forms4.0-cil libmono-system-core4.0-cil libmono-winforms2.0-cil libmono-corlib2.0-cil libmono-system-management4.0-cil
