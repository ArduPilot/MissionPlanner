namespace UAVCAN
{
    public interface IUAVCANSerialize
    {
        void encode(uavcan.uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx);

        void decode(uavcan.CanardRxTransfer transfer);
    }
}
