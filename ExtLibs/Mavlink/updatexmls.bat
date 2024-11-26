

rem https://github.com/ArduPilot/mavlink/tree/master/message_definitions/v1.0/

echo import urllib.request; > update.py
echo for i, v in enumerate(['ASLUAV.xml', 'AVSSUAS.xml', 'all.xml', 'ardupilotmega.xml', 'common.xml', 'csAirLink.xml', 'cubepilot.xml', 'development.xml', 'icarous.xml', 'loweheiser.xml', 'matrixpilot.xml', 'minimal.xml', 'python_array_test.xml', 'paparazzi.xml', 'standard.xml', 'storm32.xml', 'test.xml', 'uAvionix.xml', 'ualberta.xml']): >> update.py
echo  print(v, "\n")
echo  f = open("message_definitions/"+v, "w")>> update.py
echo  f.write(urllib.request.urlopen('https://github.com/ArduPilot/mavlink/raw/master/message_definitions/v1.0/'+v).read().decode('utf-8'))>> update.py
echo  f.close()>> update.py

python update.py

del update.py

pause