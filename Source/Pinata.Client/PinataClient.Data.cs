using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Pinata.Client.Models;

namespace Pinata.Client
{
   /// <summary>
   /// The Data endpoint of Pinata Cloud.
   /// </summary>
   public interface IDataEndpoint
   {
      /// <summary>
      /// Performs a connection test to the API with the configured credentials. Success message should read:
      /// Congratulations! You are communicating with the Pinata API!
      /// </summary>
      Task<TestAuthenticationResponse> TestAuthenticationAsync(CancellationToken cancellationToken = default);

      /// <summary>
      /// This endpoint returns the total combined size for all content that you've pinned through Pinata
      /// </summary>
      Task<UserPinnedDataTotalResponse> UserPinnedDataTotalAsync(CancellationToken cancellationToken = default);

      /// <summary>
      /// This endpoint returns data on what content the sender has pinned to IPFS through Pinata.
      /// The purpose of this endpoint is to provide insight into what is being pinned, and how long it has been pinned.
      /// The results of this call can be filtered using multiple query parameters.
      /// </summary>
      Task<object> PinList(CancellationToken cancellationToken = default);
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

      Task<UserPinnedDataTotalResponse> IDataEndpoint.UserPinnedDataTotalAsync(CancellationToken cancellationToken)
      {
         return this.DataEndpoint
            .WithClient(this)
            .AppendPathSegment("userPinnedDataTotal")
            .GetJsonAsync<UserPinnedDataTotalResponse>(cancellationToken);
      }

      Task<object> IDataEndpoint.PinList(CancellationToken cancellationToken)
      {
         throw new System.NotImplementedException();
      }
   }
}
