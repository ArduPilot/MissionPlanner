set PATH=c:\python27;%PATH%

python -c "import urllib; print urllib.urlopen('https://github.com/ArduPilot/mavlink/raw/master/message_definitions/v1.0/ardupilotmega.xml').read()" > message_definitions\ardupilotmega.xml

python -c "import urllib; print urllib.urlopen('https://github.com/ArduPilot/mavlink/raw/master/message_definitions/v1.0/common.xml').read()" > message_definitions\common.xml

pause