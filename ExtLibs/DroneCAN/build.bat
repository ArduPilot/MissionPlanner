#!/bin/bash

python3 -m pip install dronecan empy pexpect --user

python3 -m pip install --upgrade dronecan

cd canard_dsdlc

python3 canard_dsdlc.py ../dsdl/dronecan ../dsdl/com ../dsdl/ardupilot ../dsdl/cuav ../dsdl/uavcan ../dsdl/mppt ../out

pause