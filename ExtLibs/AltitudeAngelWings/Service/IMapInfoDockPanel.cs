using AltitudeAngelWings.ApiClient.Client;

namespace AltitudeAngelWings.Service
{
    public interface IMapInfoDockPanel
    {
        void Show(FeatureProperties[] featureProperties);
        void Hide();
    }
}