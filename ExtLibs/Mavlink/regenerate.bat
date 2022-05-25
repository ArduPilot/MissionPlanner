set PATH=c:\python27;%PATH%

rd /s /q "mavlink"

python -m pymavlink.tools.mavgen --lang=CS --wire-protocol=2.0 "message_definitions\ardupilotmega.xml" "message_definitions\offspec.xml"

copy /y "mavlink\mavlink.cs" "Mavlink.cs"

rem python -m pymavlink.tools.mavgen --lang=CS --wire-protocol=2.0 "message_definitions\offspec.xml"

rem copy /y "mavlink\mavlink.cs" "offspec.cs"

pause