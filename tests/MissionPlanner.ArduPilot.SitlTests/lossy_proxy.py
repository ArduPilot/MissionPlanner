#!/usr/bin/env python3
"""TCP proxy that forwards MAVLink both ways but drops every Nth LOG_DATA
frame (msgid 120) travelling vehicle -> GCS. Listens on LISTEN_PORT and
connects to the SITL vehicle on VEHICLE_PORT."""
import socket
import sys
import threading

LISTEN_PORT = 5770
VEHICLE_PORT = 5760
DROP_EVERY = 20
LOG_DATA = 120


def pump_gcs_to_vehicle(gcs, veh):
    try:
        while True:
            d = gcs.recv(4096)
            if not d:
                break
            veh.sendall(d)
    except OSError:
        pass
    finally:
        try:
            veh.shutdown(socket.SHUT_WR)
        except OSError:
            pass


def pump_vehicle_to_gcs(veh, gcs):
    # bytearray + index cursor: repeated += / slicing on immutable bytes
    # would reallocate the whole buffer per frame (O(n^2) on large logs)
    buf = bytearray()
    seen = 0
    dropped = 0
    try:
        while True:
            d = veh.recv(4096)
            if not d:
                break
            buf.extend(d)
            out = []
            pos = 0
            while pos < len(buf):
                b0 = buf[pos]
                if b0 == 0xFD:  # MAVLink2: 10 byte hdr + payload + 2 crc (+13 sig)
                    if len(buf) - pos < 10:
                        break
                    length = 12 + buf[pos + 1] + (13 if buf[pos + 2] & 0x01 else 0)
                    if len(buf) - pos < length:
                        break
                    msgid = buf[pos + 7] | (buf[pos + 8] << 8) | (buf[pos + 9] << 16)
                elif b0 == 0xFE:  # MAVLink1: 6 byte hdr + payload + 2 crc
                    if len(buf) - pos < 6:
                        break
                    length = 8 + buf[pos + 1]
                    if len(buf) - pos < length:
                        break
                    msgid = buf[pos + 5]
                else:
                    out.append(buf[pos:pos + 1])
                    pos += 1
                    continue

                frame = buf[pos:pos + length]
                pos += length
                if msgid == LOG_DATA:
                    seen += 1
                    if seen % DROP_EVERY == 0:
                        dropped += 1
                        continue
                out.append(frame)
            del buf[:pos]
            if out:
                gcs.sendall(b"".join(out))
    except OSError:
        pass
    finally:
        print(f"[proxy] LOG_DATA seen={seen} dropped={dropped}", flush=True)
        try:
            gcs.shutdown(socket.SHUT_WR)
        except OSError:
            pass


def main():
    srv = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    srv.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    srv.bind(("0.0.0.0", LISTEN_PORT))
    srv.listen(1)
    print(f"[proxy] listening on {LISTEN_PORT}, dropping 1/{DROP_EVERY} LOG_DATA", flush=True)
    gcs, addr = srv.accept()
    print(f"[proxy] client {addr}", flush=True)
    veh = socket.create_connection(("127.0.0.1", VEHICLE_PORT))
    gcs.setsockopt(socket.IPPROTO_TCP, socket.TCP_NODELAY, 1)
    veh.setsockopt(socket.IPPROTO_TCP, socket.TCP_NODELAY, 1)
    t = threading.Thread(target=pump_gcs_to_vehicle, args=(gcs, veh), daemon=True)
    t.start()
    pump_vehicle_to_gcs(veh, gcs)
    t.join(timeout=2)


if __name__ == "__main__":
    sys.exit(main())
