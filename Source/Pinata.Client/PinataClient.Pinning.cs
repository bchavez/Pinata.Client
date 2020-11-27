using System.Collections.Generic;
using System.IO;
using System.Linq;
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
   public class PinataOptions : Json
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
      public PinPolicy CustomPinPolicy { get; set; } = new PinPolicy();
   }

   public class PinPolicy : Json
   {
      [JsonProperty("regions")]
      public List<Region> Regions { get; set; } = new List<Region>();

      public void AddOrUpdateRegion(string id, int desiredReplicationCount)
      {
         var existingRegion = this.Regions.FirstOrDefault(r => r.Id == id);
         if( existingRegion is null )
         {
            this.Regions.Add(new Region { Id = id, DesiredReplicationCount = desiredReplicationCount });
         }
         else
         {
            existingRegion.DesiredReplicationCount = desiredReplicationCount;
         }
      }
   }

   public class Region : Json
   {
      [JsonProperty("id")]
      public string Id { get; set; }

      [JsonProperty("desiredReplicationCount")]
      public int DesiredReplicationCount { get; set; }
   }

   public class PinataMetadata : Json
   {
      [JsonProperty("name")]
      public string Name { get; set; }

      [JsonProperty("keyvalues")]
      public Dictionary<string, string> KeyValues { get; set; } = new Dictionary<string, string>();
   }


   public interface IPinningEndpoint
   {
      Task<IFlurlResponse> UnpinAsync(string hashToUnpin, CancellationToken cancellationToken = default);
      Task<PinJsonToIpfsResponse> PinJsonToIpfsAsync(string jsonContent, PinataMetadata pinataMetadata = null,
         PinataOptions pinataOptions = null, CancellationToken cancellationToken = default);
      Task<PinJsonToIpfsResponse> PinJsonToIpfsAsync(object pinataContent, PinataMetadata pinataMetadata = null,
         PinataOptions pinataOptions = null, CancellationToken cancellationToken = default);
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
         PinataMetadata pinataMetadata = null,
         PinataOptions pinataOptions = null, CancellationToken cancellationToken = default)
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
            .PostAsync(new CapturedJsonContent(body), cancellationToken)
            .ReceiveJson<PinJsonToIpfsResponse>();
      }

      Task<PinJsonToIpfsResponse> IPinningEndpoint.PinJsonToIpfsAsync(object pinataContent,
         PinataMetadata pinataMetadata = null,
         PinataOptions pinataOptions = null, CancellationToken cancellationToken = default)
      {
         return this.PinningEndpoint
            .WithClient(this)
            .AppendPathSegment("pinJSONToIPFS")
            .PostJsonAsync(new
            {
               pinataOptions,
               pinataMetadata,
               pinataContent
            }, cancellationToken)
            .ReceiveJson<PinJsonToIpfsResponse>();
      }
   }
}
