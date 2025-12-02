# Mission Planner - Titan Dynamics Edition
<p align="center">
   <img width="300" height="300" alt="icon" src="https://github.com/user-attachments/assets/b3e67430-0296-4f09-ada2-d01a03e684ae"/><br><br>
   A customized fork of <a href="https://github.com/ArduPilot/MissionPlanner">Mission Planner</a> with enhanced UI/UX and ease-of-use improvements
   <br><br>
   <img width="2560" alt="image" src="https://github.com/user-attachments/assets/200dddd2-ea26-425a-8dad-b8b8093fb33a" /><br><br>
   <img width="2560" height="1540" alt="image" src="https://github.com/user-attachments/assets/2fbe5217-4a12-43ab-ae10-1fee7a5ada8a" /><br>
</p>

---

# Major Modifications & Improvements:

---

## Full Parameter List UI/UX Overhaul
<img width="2025" height="1239" alt="image" src="https://github.com/user-attachments/assets/980bd92e-58e0-4e50-b9fe-652497b98883" /><br>
The list of parameters is now loaded asynchronously (in terms of the documenation and ranges, which were all loaded at once for nearly 6000 parameters). This greatly reduced the delay in loading the full param list we're all used to.<br><br>
<img width="196" height="155" alt="image" src="https://github.com/user-attachments/assets/cc53241b-c788-4907-b5a1-fd204e86a108" /><br>
Furthermore, the presets and warning message in the upstream GUI (shown above) have been deleted.

## Removal of Confirmation Dialogs on Param Write
The dialogs that pop up after writing params have been removed. These are unneeded for those of us who know what we're doing, and frankly frustrating. "Reboot required" and "Param save failed" dialogs still do show up.

---

## Servo Output Tab
<img width="1668" height="1065" alt="image" src="https://github.com/user-attachments/assets/8e68cbdd-77d3-4fb8-b90a-280367e942ee" /><br>
Trimming servos has never been easier. Use the slider by either clicking on either side of the handle or drag it until your control surface is flat. Then, use the "Equidistant" feature to eliminate mental math once and for all to get your min and max endpoints. The slider increments in steps of 10<br>

---

## Params Tab + Params on Map
<img width="2560" height="1540" alt="image" src="https://github.com/user-attachments/assets/f13a8a95-f4aa-4f9c-8894-ed4963f7227c" /><br><br>
<img width="2560" height="1540" alt="image" src="https://github.com/user-attachments/assets/dedf1481-0797-46a6-a7d3-f9f747f0916b" /><br><br>
<img width="2560" height="1540" alt="image" src="https://github.com/user-attachments/assets/1177f206-d6ac-49e5-a0d3-470af388474c" /><br><br>
That's right... you no longer need to leave the main screen in order to see or change parameters! Not only is there now a Params tab in FlightData, but also one on the bottom of the map that co-exists with the Tuning panel. Both have the tree collapsed by default to conserve space and have identical functionality to the param editing page in the CONFIG tab. If you prefer your param editor on the map side, you can simply hide the one on the FlightData tab to prevent redundancy.<br>

---

## Tuning Tab for Plane, Copter, Rover
<img width="1980" height="1456" alt="image" src="https://github.com/user-attachments/assets/2d98481e-6f42-41a1-97f2-4294338f9cc9" /><br>
There is now a Tuning tab in FlightData which is the same as the one in the main CONFIG tab (based on vehicle type). Same functionality, less clicking around and shuffling around when changing PIDs or angle limits.

---

## 3D Map View Enhancements
<img width="2560" height="1540" alt="image" src="https://github.com/user-attachments/assets/ac83552e-62aa-4317-b3de-682a92e5bb7e" /><br><br>
<img width="2560" height="1540" alt="image" src="https://github.com/user-attachments/assets/08883da8-58f4-4880-8724-eee3a687eb8e" /><br><br>
<img width="2560" height="1540" alt="image" src="https://github.com/user-attachments/assets/c68d680e-a859-4ac3-bc95-114159cd9ba0" /><br><br>
The 3D Maps widget has smoothing algorithms to keep the camera viewpoint steady - It can also now overlay on top of the 2D Map

---

## Double-Click "Fly to Here" on Map
<img width="899" height="1132" alt="image" src="https://github.com/user-attachments/assets/f152cbf6-c710-4155-9918-cb8c1fbd9150" /><br>
With the setting enabled (top of map) you can now double click anywhere on the map to "Fly to Here" aka get into GUIDED mode

---

## Messages Tab Layout
<img width="1980" height="1456" alt="image" src="https://github.com/user-attachments/assets/7211a1c7-8bc1-4f5c-8033-041494a0c4f4" /><br>
The messages tab in FlightData has been fully redesigned to provide not only the color-coded severity of the message, but also higher readability and situational awareness

---

## Video Tab in FlightData
<img width="1818" height="1184" alt="image" src="https://github.com/user-attachments/assets/c022543f-7339-421d-b0a8-6655a0111f05" /><br>
You no longer need to leave the main screen and go to the CONFIG tab to start/stop your camera the way you prefer. You can set up USB cameras or GStreamer all from one place, eliminating friction in debugging video issues mid-flight or costing precious seconds when they matter most.

---

## QuickView Font Size Fix
<img width="1143" height="907" alt="image" src="https://github.com/user-attachments/assets/4c5bd087-197b-42cf-9ea5-9380f148cf6e" /><br><br>
<img width="1633" height="968" alt="image" src="https://github.com/user-attachments/assets/81d97ebf-6d9c-4d24-8d86-a0a0b36adc89" /><br><br>
Gone are the days of your user defined items in the Quick tab having overlapping text. The logic for drawing the label and value text takes into account the space it has to stretch out in.

---

## Flight Mode Channel Selection
<img width="541" height="387" alt="image" src="https://github.com/user-attachments/assets/558e9127-12ca-4a90-b336-df8ee57722cf" /><br>
Added the option to set your flight mode channel from the Flight Modes page. No need to leave this page to set up your main 6 channels.

---

## Battery Wh Used and Wh/km Quick Tab Options
<img width="407" height="447" alt="image" src="https://github.com/user-attachments/assets/6b37cd56-34c4-44ce-b892-3a9b2005c2b1" /><br>
In addition to the mah used and mah/km Quick tab items, there are wh used and wh/km items also available for added monitoring

---

## Link Stats Always on Top
<img width="889" height="647" alt="image" src="https://github.com/user-attachments/assets/dfadfa97-f144-4ed9-8922-4f8cef1fc5bc" /><br>
Never lose your link stats screen again. Especially useful for long-range flights where monitoring Link Stats is crucial while needing to click around other parts of the main GUI.

---

## Reboot Button in Actions Tab
<img width="2001" height="1156" alt="image" src="https://github.com/user-attachments/assets/7d1844b6-9544-4eda-ba1f-283ae2c1c083" /><br>
Saves a bit of time not having to hit CTRL-F and visually identify where the "reboot pixhawk" button is. Btw, that button now says "reboot vehicle." More changes on this screen include the layout cleanliness/tidying and loiter rad incrementing in steps of 10, not 1

---

## MAVLink Inspector in FlightData Tabs
<img width="2560" height="1540" alt="image" src="https://github.com/user-attachments/assets/eec2c0ad-5bb0-4e4e-9689-181791f0e571" /><br>
No need to press CTRL-F to get to the Mavlink inspector. Highly useful for realtime debugging!

---

## Larger Font on Sats & HDP + new VDOP Display
<img width="411" height="152" alt="image" src="https://github.com/user-attachments/assets/65cab556-6a7e-4978-b4ef-f6ef47378625" /><br>
Added VDOP to the label on the bottom left of the map, since HDOP was already on there.

---

## Propagation Settings on Map
<img width="1020" height="585" alt="image" src="https://github.com/user-attachments/assets/ae2d90a7-7b11-4336-8a0d-09ceba6056fc" /><br>
The Propagation Settings window (CTRL-W keyboard shortcut) is now accessible from the top left of the map.

---

## Hide HELP Tab
<img width="575" height="152" alt="image" src="https://github.com/user-attachments/assets/ce0c8d80-cb8a-4ac3-bf63-668b8b74814a" /><br>
This is intended for users who know a thing or two about Ardupilot. THe MP HELP tab is no one's first stop :P

---

## Windows Terminal Removal
The terminal window that opens with Mission Planner is no longer there - poof! Gone.

---

## New Themed Tab Strip Control
<img width="1092" height="513" alt="image" src="https://github.com/user-attachments/assets/a6756f58-ff8c-419f-b12f-4d4ea7fe245d" /><br>
Perhaps the least attractive part of Mission Planner's GUI has been addressed, which is the FlightData tab layout. It now looks much more modern!

---

## Dark Windows Theme Support
<img width="1288" height="742" alt="image" src="https://github.com/user-attachments/assets/7c97f8df-5b3b-4c55-bb90-74445617d946" /><br>
Window title bars are dark, finally!

---

## New Dark and Light Theme Icons
<img width="408" height="159" alt="image" src="https://github.com/user-attachments/assets/94b7d8dd-3cce-4b4a-88b2-06caa168cfbd" /><br><br>
<img width="501" height="152" alt="image" src="https://github.com/user-attachments/assets/7d797468-bcd3-4b32-86a3-1c8bd4ae1c38" /><br><br>
<img width="361" height="166" alt="image" src="https://github.com/user-attachments/assets/b96aebd3-484e-47a9-b968-6ecb91d30473" /><br><br>
<img width="357" height="187" alt="image" src="https://github.com/user-attachments/assets/0af6964c-b92e-4875-9b24-f74deea739d2" /><br>
New icons on the top-most tab give it a fresh look - although further improvements can still be made

---

## Temp Window Renamed to Tools and accessible from Map
<img width="2560" height="1540" alt="image" src="https://github.com/user-attachments/assets/114211cb-4cc9-40e2-8428-1780099911b8" /><br>
Really it should have been called "Tools" from the get-go. This window (commmonly accessed via CTRL-F) is now also accessible from the "Tools" button on top of the Map.

---

## Splash Screen & New Application Icon
<img width="2560" height="1600" alt="image" src="https://github.com/user-attachments/assets/c048106b-b33d-4811-beeb-4e5e1f3e9780" /><br><br>
<img width="521" height="151" alt="image" src="https://github.com/user-attachments/assets/b6cce0cc-bc9a-4804-9279-ed9ad45a3e2d" /><br>
Yes.

---

## Installation

### Windows (Recommended)

#### Requirements
- Visual Studio 2022

#### Build Steps

1. Clone this repository:
   ```bash
   git clone https://github.com/YourUsername/MissionPlanner.git
   cd MissionPlanner
   git submodule update --init
   ```

2. Open `MissionPlanner.sln` in Visual Studio 2022

3. Build → Build MissionPlanner

### Linux (Mono)

```bash
sudo apt install mono-complete mono-runtime libmono-system-windows-forms4.0-cil \
    libmono-system-core4.0-cil libmono-winforms4.0-cil libmono-corlib4.0-cil \
    libmono-system-management4.0-cil libmono-system-xml-linq4.0-cil

mono MissionPlanner.exe
```

---

## Platform Support

| Platform | Status |
|----------|--------|
| Windows | ✅ Full Support |
| Linux (Mono) | ⚠️ Partial Support |
| macOS | ⚠️ Experimental |
| Android | ✅ [Play Store](https://play.google.com/store/apps/details?id=com.michaeloborne.MissionPlanner) |
| iOS | ⚠️ Experimental |

---

## Upstream Information

- **Upstream Repository**: https://github.com/ArduPilot/MissionPlanner
- **ArduPilot Website**: http://ardupilot.org/planner/
- **Forum**: http://discuss.ardupilot.org/c/ground-control-software/mission-planner

---

## License

See [COPYING.txt](COPYING.txt) for details.

---

## Building the Installer
* Setup Wix toolset
* Build a release version of MissionPlanner and Plugins
* Build the WIX project from the MissionPlanner solution, which outputs to the Msi folder of the repo
* Go to the Msi folder
* Run installer.bat
* Run create.bat

