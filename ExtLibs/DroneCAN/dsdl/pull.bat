rmdir /s /q temp
git clone --depth=1 --branch=master https://github.com/dronecan/DSDL temp
rmdir /Q /S temp\.git
xcopy /E /C /R /Y temp\ .\
rmdir /s /q temp
