#!/bin/bash

python2 -m pip install dronecan empy pexpect --user

python2 ./canard_dsdlc/canard_dsdlc.py dsdl/dronecan dsdl/org dsdl/com dsdl/ardupilot dsdl/cuav dsdl/uavcan dsdl/mppt out

pause