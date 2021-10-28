namespace DroneCAN
{
    public interface IDroneCANSerialize
    {
        void encode(DroneCAN.dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx);

        void decode(DroneCAN.CanardRxTransfer transfer);
    }
}
