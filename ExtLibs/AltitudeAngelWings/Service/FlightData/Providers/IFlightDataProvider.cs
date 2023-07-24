namespace AltitudeAngelWings.Service.FlightData.Providers
{
    public interface IFlightDataProvider
    {
        Model.FlightData GetCurrentFlightData();
    }
}
