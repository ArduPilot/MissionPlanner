using System.Threading.Tasks;

namespace Xamarin.MacOS
{
    internal class GPS : IGPS
    {
        public Task<(double lat, double lng, double alt)> GetPosition()
        {
            throw new System.NotImplementedException();
        }
    }
}