using System;

namespace AltitudeAngelWings.ApiClient.Models
{
    public class WeatherInfo
    {
        public string Summary { get; set; }
        public float TemperatureC { get; set; }
        public DateTime At { get; set; }
        public ushort CloudCoverPercent { get; set; }
        public ushort UVIndex { get; set; }
        public string WindDirection { get; set; }
        public int WindHeadingDegrees { get; set; }
        public float WindSpeedKph { get; set; }
        public ushort PrecipitationPercent { get; set; }
        public ushort HumidityPercent { get; set; }
        public uint MeanSeaLevelPressureHPa { get; set; }
        public float DewPointC { get; set; }
        public uint SnowMillimeters { get; set; }
        public uint RainfallMillimeters { get; set; }
    }
}
