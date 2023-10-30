using System.Collections.Generic;

namespace AltitudeAngelWings
{
    public interface IOverlay
    {
        void SetFeatures(IReadOnlyList<OverlayFeature> features);
    }
}
