
python -c "import urllib.request; print(urllib.request.urlopen('https://github.com/ArduPilot/mavlink/raw/master/message_definitions/v1.0/ardupilotmega.xml').read().decode('utf-8') )" > message_definitions\ardupilotmega.xml

python -c "import urllib.request; print(urllib.request.urlopen('https://github.com/ArduPilot/mavlink/raw/master/message_definitions/v1.0/common.xml').read().decode('utf-8') )" > message_definitions\common.xml

python -c "import urllib.request; print(urllib.request.urlopen('https://github.com/ArduPilot/mavlink/raw/master/message_definitions/v1.0/minimal.xml').read().decode('utf-8') )" > message_definitions\minimal.xml

python -c "import urllib.request; print(urllib.request.urlopen('https://github.com/ArduPilot/mavlink/raw/master/message_definitions/v1.0/uAvionix.xml').read().decode('utf-8') )" > message_definitions\uAvionix.xml

python -c "import urllib.request; print(urllib.request.urlopen('https://github.com/ArduPilot/mavlink/raw/master/message_definitions/v1.0/icarous.xml').read().decode('utf-8') )" > message_definitions\icarous.xml

python -c "import urllib.request; print(urllib.request.urlopen('https://github.com/ArduPilot/mavlink/raw/master/message_definitions/v1.0/loweheiser.xml').read().decode('utf-8') )" > message_definitions\loweheiser.xml

pause