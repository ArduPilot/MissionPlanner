set PATH=c:\python27;%PATH%

rd /s /q "mavlink"

python -m pymavlink.tools.mavgen --lang=CS --wire-protocol=2.0 "message_definitions\all.xml" "message_definitions\offspec.xml"

python -m pymavlink.tools.mavgen --lang=WLua --wire-protocol=2.0 "message_definitions\all.xml" "message_definitions\offspec.xml"


copy /y "mavlink\mavlink.cs" "Mavlink.cs"

rem python -m pymavlink.tools.mavgen --lang=CS --wire-protocol=2.0 "message_definitions\offspec.xml"

rem copy /y "mavlink\mavlink.cs" "offspec.cs"

pause