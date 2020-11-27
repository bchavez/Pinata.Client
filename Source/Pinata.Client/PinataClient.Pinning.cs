using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Flurl.Http.Content;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pinata.Client.Models;

namespace Pinata.Client
{
   public class PinataOptions
   {
      /// <summary>
      /// The CID Version IPFS will use when creating a hash for your content. Valid options are:
      /// "0" - CIDv0, "1" - CIDv1
      /// </summary>
      [JsonProperty("cidVersion")]
      public int CidVersion { get; set; }

      /// <summary>
      /// A custom pin policy for the piece of content being pinned.
      /// </summary>
      [JsonProperty("customPinPolicy")]
      public string CustomPinPolicy { get; set; }
   }

   public class CustomPolicy
   {

   }

   public class PinataMetadata
   {
      [JsonProperty("name")]
      public string Name { get; set; }

      [JsonProperty("keyvalues")]
      public Dictionary<string, string> KeyValues { get; set; } = new Dictionary<string, string>();
   }


   public interface IPinningEndpoint
   {
      Task<IFlurlResponse> UnpinAsync(string hashToUnpin, CancellationToken cancellationToken = default);
      Task<PinJsonToIpfsResponse> PinJsonToIpfsAsync(string jsonContent, PinataOptions pinataOptions = null, PinataMetadata pinataMetadata = null);
      Task<IFlurlResponse> PinJsonToIpfsAsync(object pinataContent, PinataOptions pinataOptions = null, PinataMetadata pinataMetadata = null);
   }

   public partial class PinataClient : IPinningEndpoint
   {
      public IPinningEndpoint Pinning => this;

      protected internal Url PinningEndpoint => this.Config.ApiUrl.AppendPathSegment("pinning");

      Task<IFlurlResponse> IPinningEndpoint.UnpinAsync(string hashToUnpin, CancellationToken cancellationToken = default)
      {
         return this.PinningEndpoint
            .WithClient(this)
            .AppendPathSegments("unpin", hashToUnpin)
            .DeleteAsync(cancellationToken);
      }

      Task<PinJsonToIpfsResponse> IPinningEndpoint.PinJsonToIpfsAsync(string jsonContent,
         PinataOptions pinataOptions = null, PinataMetadata pinataMetadata = null)
      {
         string body = jsonContent;

         if( pinataOptions is not null || pinataMetadata is not null )
         {
            const string ReplacePattern = "//[[JSON_CONTENT]]//";
            var payload = new
               {
                  pinataContent = ReplacePattern,
                  pinataOptions,
                  pinataMetadata
               };
            var payloadJson = JsonConvert.SerializeObject(payload);
            var indexOfPatternStart = payloadJson.IndexOf(ReplacePattern);
            var indexOfPatternEnd = indexOfPatternStart + ReplacePattern.Length;

            var sb = new StringBuilder();
            var startJson = payloadJson.Substring(0, indexOfPatternStart);
            sb.Append(startJson);
            sb.Append(jsonContent);
            var endJson = payloadJson.Substring(indexOfPatternEnd);
            sb.Append(endJson);
            body = sb.ToString();
         }

         return this.PinningEndpoint
            .WithClient(this)
            .AppendPathSegment("pinJSONToIPFS")
            .PostAsync(new CapturedJsonContent(body))
            .ReceiveJson<PinJsonToIpfsResponse>();
      }

      Task<IFlurlResponse> IPinningEndpoint.PinJsonToIpfsAsync(object pinataContent, PinataOptions pinataOptions = null, PinataMetadata pinataMetadata = null)
      {
         return this.PinningEndpoint
            .WithClient(this)
            .AppendPathSegment("pinJSONToIPFS")
            .PostJsonAsync(new
            {
               pinataOptions,
               pinataMetadata,
               pinataContent
            });
      }
   }
}
