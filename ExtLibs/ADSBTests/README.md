ADS-B reader tests (squawk + units) against recorded live data.

    dotnet test ExtLibs/ADSBTests -p:MinVerSkip=true

Needs a .NET 9 SDK (CsWin32 in WinUSBNet). Fixtures: adsb.lol responses, 1090 MHz frames
received live and CRC-checked, and DF21 frames from pyModeS' golden set. `live_airspy_frames.json`
notes where each came from.
