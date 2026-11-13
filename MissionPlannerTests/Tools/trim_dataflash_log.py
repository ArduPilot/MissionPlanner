"""Trim a real ArduPilot DataFlash log into a small test fixture and record what
pymavlink's DFReader decodes from it, as an independent oracle for MissionPlanner's parser.

    python -I trim_dataflash_log.py 00000001.BIN copter-sitl.bin copter-sitl.expected.json
"""
import json
import struct
import sys

from pymavlink import DFReader

src, dst, expected = sys.argv[1], sys.argv[2], sys.argv[3]
data = open(src, 'rb').read()

# every message of these types is kept
KEEP_ALL = {'FMT', 'FMTU', 'UNIT', 'MULT', 'PARM', 'MSG', 'VER', 'MODE', 'EV', 'ERR', 'ORGN', 'FILE'}
# two in N of these, enough to have several instances and a GPS fix
SAMPLE = {'GPS': 5, 'ATT': 40, 'IMU': 80, 'BARO': 40, 'BAT': 10, 'POWR': 10, 'MAG': 40, 'RATE': 40}

lengths = {}
names = {}
out = bytearray()
seen = {}
pos = 0
while pos + 3 <= len(data):
    if data[pos] != 0xA3 or data[pos + 1] != 0x95:
        pos += 1
        continue
    t = data[pos + 2]
    if t == 0x80:
        ftype, flen = data[pos + 3], data[pos + 4]
        name = data[pos + 5:pos + 9].rstrip(b'\0').decode()
        lengths[ftype] = flen
        names[ftype] = name
        length = 89
    elif t in lengths:
        length = lengths[t]
    else:
        pos += 1
        continue
    msg = data[pos:pos + length]
    if len(msg) < length:
        break
    name = 'FMT' if t == 0x80 else names[t]
    n = seen.get(name, 0)
    seen[name] = n + 1
    # consecutive pairs, so alternating instances are both kept
    if name in KEEP_ALL or (name in SAMPLE and n % SAMPLE[name] < 2):
        out += msg
    pos += length

open(dst, 'wb').write(bytes(out))

# what pymavlink makes of the trimmed log
log = DFReader.DFReader_binary(dst, zero_time_base=False)
counts = {}
first = {}
last = {}
while True:
    m = log.recv_msg()
    if m is None:
        break
    t = m.get_type()
    counts[t] = counts.get(t, 0) + 1
    fields = {}
    for f in m._fieldnames:
        v = getattr(m, f)
        if isinstance(v, bytes):
            v = v.rstrip(b'\0').decode('ascii', 'replace')
        if isinstance(v, (list, tuple)):
            v = list(v)
        fields[f] = v
    if t not in first:
        first[t] = fields
    last[t] = fields

json.dump({'counts': counts, 'first': first, 'last': last}, open(expected, 'w'), indent=1, sort_keys=True)
print(len(out), 'bytes', sum(counts.values()), 'messages')
print(sorted(counts.items()))
