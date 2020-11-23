using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Pinata.Client.Models;

namespace Pinata.Client
{
   public interface IDataEndpoint
   {
      Task<TestAuthenticationResponse> TestAuthenticationAsync(CancellationToken cancellationToken = default);
   }

   public partial class PinataClient : IDataEndpoint
   {
      public IDataEndpoint Data => this;

      protected internal Url DataEndpoint => this.Config.ApiUrl.AppendPathSegment("data");

      Task<TestAuthenticationResponse> IDataEndpoint.TestAuthenticationAsync(CancellationToken cancellationToken = default)
      {
         return this.DataEndpoint
            .WithClient(this)
            .AppendPathSegment("testAuthentication")
            .GetJsonAsync<TestAuthenticationResponse>(cancellationToken);
      }
   }
}
