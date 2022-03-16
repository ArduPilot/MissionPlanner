namespace AltitudeAngelWings.Service.FlightData.Providers
{
    public interface IFlightDataProvider
    {
        Models.FlightData GetCurrentFlightData();
    }
}
