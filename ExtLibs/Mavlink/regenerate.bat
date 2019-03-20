set PATH=c:\python27;%PATH%

rd /s /q "mavlink"
python -m pymavlink.tools.mavgen --lang=CS --wire-protocol=2.0 "message_definitions\ardupilotmega.xml"  
copy /y "mavlink\ardupilotmega\mavlink.cs" "Mavlink.cs"
pause