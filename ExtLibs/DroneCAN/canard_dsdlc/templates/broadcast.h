@{
if msg.kind == msg.KIND_MESSAGE:
    from canard_dsdlc_helpers import *
    empy.include('templates/msg.h', get_empy_env_broadcast(msg))
}@
