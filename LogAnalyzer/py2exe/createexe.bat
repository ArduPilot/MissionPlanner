C:\Python27x86\python setup.py py2exe

cd dist

"C:\Program Files\7-Zip\7z.exe" a -tzip ..\..\LogAnalyzer.zip *

cd ..

C:\Python27\python setup.py py2exe

cd dist

"C:\Program Files\7-Zip\7z.exe" a -tzip ..\..\LogAnalyzer64.zip *

pause