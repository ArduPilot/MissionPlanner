using System;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace AltitudeAngelWings.Clients
{
    public interface IAuthorizeCodeProvider
    {
        void GetAuthorizeParameters(NameValueCollection parameters);
        Task<string> GetAuthorizeCode(Uri authorizeUri);
    }
}
