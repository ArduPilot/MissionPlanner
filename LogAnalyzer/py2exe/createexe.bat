rmdir /s dist

C:\Python27\python -m pip install numpy

C:\Python27\python setup.py py2exe

cd dist

"C:\Program Files\7-Zip\7z.exe" a -tzip ..\..\LogAnalyzer.zip *

"C:\Program Files\7-Zip\7z.exe" a -tzip ..\..\LogAnalyzer64.zip *


runner.exe ..\examples\mechanical_fail.log

pause