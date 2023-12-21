import sys
print (sys.version_info)
import site; 
print(site.getsitepackages())
import math
print("start")

import sysconfig;
print(sysconfig.get_paths()["purelib"])

print(site.USER_SITE)

from distutils.sysconfig import get_python_lib;
print(get_python_lib())

print(dir(sys))
print(dir(math))

print("end")