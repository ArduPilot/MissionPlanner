using AltitudeAngelWings.Clients.Api.Model;

namespace AltitudeAngelWings
{
    public interface IMapInfoDockPanel
    {
        void Show(FeatureProperties[] featureProperties);
        void Hide();
    }
}