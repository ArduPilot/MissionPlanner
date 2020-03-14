set PATH=c:\python27;%PATH%

rd /s /q "mavlink"

python -c "import urllib; print urllib.urlopen('https://github.com/ArduPilot/mavlink/raw/master/message_definitions/v1.0/ardupilotmega.xml').read()" > message_definitions\ardupilotmega.xml

python -c "import urllib; print urllib.urlopen('https://github.com/ArduPilot/mavlink/raw/master/message_definitions/v1.0/common.xml').read()" > message_definitions\common.xml

rem python -m pymavlink.tools.mavgen --lang=C --wire-protocol=2.0 "message_definitions\ardupilotmega.xml"  
python -m pymavlink.tools.mavgen --lang=CS --wire-protocol=2.0 "message_definitions\ardupilotmega.xml" "message_definitions\offspec.xml"  
rem python -m pymavlink.tools.mavgen --lang=CS_OLD --wire-protocol=2.0 "message_definitions\ardupilotmega.xml"  
copy /y "mavlink\mavlink.cs" "Mavlink.cs"
pause