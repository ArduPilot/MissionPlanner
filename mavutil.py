#!/usr/bin/env python
'''
mavlink python utility functions
Copyright Andrew Tridgell 2011-2019
Released under GNU LGPL version 3 or later
'''
from __future__ import print_function
#from builtins import object

import socket, math, struct, time, os, fnmatch, array, sys, errno
import select
import copy
import re

# adding these extra imports allows pymavlink to be used directly with pyinstaller
# without having complex spec files. To allow for installs that don't have ardupilotmega
# at all we avoid throwing an exception if it isn't installed
try:
    import json
    from pymavlink.dialects.v10 import ardupilotmega
except Exception:
    pass

# maximum packet length for a single receive call - use the UDP limit
UDP_MAX_PACKET_LEN = 65535

# Store the MAVLink library for the currently-selected dialect
# (set by set_dialect())
mavlink = None

# Store the mavlink file currently being operated on
# (set by mavlink_connection())
mavfile_global = None

# If the caller hasn't specified a particular native/legacy version, use this
default_native = False

# link_id used for signing
global_link_id = 0

class param_state(object):
    '''state for a particular system id/component id pair'''
    def __init__(self):
        self.params = {}

class mavfile(object):
    '''a generic mavlink port'''
    def __init__(self, fd, address, source_system=255, source_component=0, notimestamps=False, input=True, use_native=default_native):
        global mavfile_global
        if input:
            mavfile_global = self

    def param(self, name, default=None):
        '''convenient function for returning an arbitrary MAVLink
           parameter with a default'''
        return default

mavfile(None, None)
