import os
from distutils.core import setup
import py2exe

Mydata_files = []
for files in os.listdir('./tests/'):
    f1 = './tests/' + files
    if os.path.isfile(f1): # skip directories
        f2 = 'tests', [f1]
        Mydata_files.append(f2)

setup(
    console=['runner.py'],
    data_files = Mydata_files,
)
