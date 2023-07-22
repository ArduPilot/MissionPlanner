using System;
using System.Threading;
using System.Threading.Tasks;
using AltitudeAngelWings.ApiClient.Models;

namespace AltitudeAngelWings.ApiClient.Client
{
    public interface IAltitudeAngelClient : IDisposable
    {
        void Disconnect(bool resetAuth = false);
        Task<UserProfileInfo> GetUserProfile(CancellationToken cancellationToken = default);
    }
}