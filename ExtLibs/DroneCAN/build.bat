#!/bin/bash

python3 -m pip install dronecan empy pexpect --user

python3 ./canard_dsdlc/canard_dsdlc.py dsdl/dronecan dsdl/org dsdl/com dsdl/ardupilot dsdl/cuav dsdl/uavcan dsdl/mppt out

pause