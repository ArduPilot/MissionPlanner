using System;
using System.Threading.Tasks;

namespace AltitudeAngelWings.ApiClient.Client
{
    public interface IAuthorizeCodeProvider
    {
        Task<Uri> GetCodeUri(Uri authorizeUri, Uri redirectUri);
    }
}
