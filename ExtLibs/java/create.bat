rem ikvm-8.1.5717.0\bin\ikvmc.exe -debug -target:library android-4.1.1.4.jar httpclient-4.0.1.jar xmlParserAPIs-2.6.2.jar xpp3-1.1.4c.jar commons-logging-1.1.1.jar json-20080701.jar

for /r %%i in (*.jar) do ikvm-8.1.5717.0\bin\ikvmc.exe -debug -target:library %%i

rem ikvm-8.1.5717.0\bin\ikvmc.exe -debug -target:library google-play-services-r22.jar *.jar