set PATH=c:\python27;%PATH%

python -c "import urllib.request; print(urllib.request.urlopen('https://raw.githubusercontent.com/ArduPilot/MAVProxy/master/MAVProxy/tools/graphs/mavgraphs.xml').read().decode('utf-8') )" > mavgraphs.xml

python -c "import urllib.request; print(urllib.request.urlopen('https://raw.githubusercontent.com/ArduPilot/MAVProxy/master/MAVProxy/tools/graphs/mavgraphs2.xml').read().decode('utf-8') )" > mavgraphs2.xml

python -c "import urllib.request; print(urllib.request.urlopen('https://raw.githubusercontent.com/ArduPilot/MAVProxy/master/MAVProxy/tools/graphs/ekfGraphs.xml').read().decode('utf-8') )" > ekfGraphs.xml

python -c "import urllib.request; print(urllib.request.urlopen('https://raw.githubusercontent.com/ArduPilot/MAVProxy/master/MAVProxy/tools/graphs/ekf3Graphs.xml').read().decode('utf-8') )" > ekf3Graphs.xml


pause