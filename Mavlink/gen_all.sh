#!/bin/sh

for xml in message_definitions/*.xml; do
     base=$(basename $xml .xml)
	./mavgen.py --lang=csharp --wire-protocol=1.0 --output=Csharp $xml
 done


