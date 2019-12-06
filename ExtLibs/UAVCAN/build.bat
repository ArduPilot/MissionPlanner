#!/bin/bash

python -m pip install uavcan empy --user

python ./canard_dsdlc/canard_dsdlc.py dsdl/uavcan dsdl/org dsdl/com out

pause