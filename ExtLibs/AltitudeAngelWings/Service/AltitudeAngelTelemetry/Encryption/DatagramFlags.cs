namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption
{
    public static class DatagramFlags
    {
        /// <summary>
        /// No flags
        /// </summary>
        public static sbyte None = 0x0;

        /// <summary>
        /// Payload is encrypted
        /// </summary>
        public static sbyte Encrypted = 0x1;
    }
}
