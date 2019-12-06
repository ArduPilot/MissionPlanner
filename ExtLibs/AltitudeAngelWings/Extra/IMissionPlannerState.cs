namespace AltitudeAngelWings.Extra
{
    public interface IMissionPlannerState
    {
        bool IsArmed { get; }
        double Longitude { get; }
        double Latitude { get; }
        float Altitude { get; }
        float GroundSpeed { get; }
        float MagneticDeclination { get; }
        bool IsConnected { get; }
    }
}
