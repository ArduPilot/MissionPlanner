#!/usr/sh

sudo mono --aot /usr/lib/mono/4.5/mscorlib.dll
for i in /usr/lib/mono/gac/*/*/*.dll; do sudo mono --aot $i; done

for i in *.dll; do sudo mono --aot $i; done

sudo mono --aot 'MissionPlanner.exe'

