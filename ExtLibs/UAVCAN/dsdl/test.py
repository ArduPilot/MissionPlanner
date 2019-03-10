#!/usr/bin/env python3

import sys
import logging

logging.basicConfig(stream=sys.stderr, level='DEBUG', format='%(levelname)s: %(message)s')

from uavcan import dsdl

parsed = dsdl.parse_namespaces(['uavcan']);
if parsed:
    logging.info('%d data types parsed successfully', len(parsed))
