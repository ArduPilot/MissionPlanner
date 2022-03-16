set PATH=c:\python27;%PATH%

python -c "import urllib; print urllib.urlopen('https://raw.githubusercontent.com/ArduPilot/MAVProxy/master/MAVProxy/tools/graphs/mavgraphs.xml').read()" > mavgraphs.xml

python -c "import urllib; print urllib.urlopen('https://raw.githubusercontent.com/ArduPilot/MAVProxy/master/MAVProxy/tools/graphs/mavgraphs2.xml').read()" > mavgraphs2.xml

python -c "import urllib; print urllib.urlopen('https://raw.githubusercontent.com/ArduPilot/MAVProxy/master/MAVProxy/tools/graphs/ekfGraphs.xml').read()" > ekfGraphs.xml

python -c "import urllib; print urllib.urlopen('https://raw.githubusercontent.com/ArduPilot/MAVProxy/master/MAVProxy/tools/graphs/ekf3Graphs.xml').read()" > ekf3Graphs.xml


pause