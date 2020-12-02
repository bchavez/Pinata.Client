using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
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

      /// <summary>
      /// Wrap your content inside of a directory when adding to IPFS. This allows users to retrieve content via a filename instead of just a hash.
      /// </summary>
      [JsonProperty("wrapWithDirectory")]
      public bool? WrapWithDirectory { get; set; }
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
      /// <summary>
      /// This endpoint allows the sender to unpin content they previously uploaded to Pinata's IPFS nodes
      /// </summary>
      Task<IFlurlResponse> UnpinAsync(string hashToUnpin, CancellationToken cancellationToken = default);

      /// <summary>
      /// This endpoint allows the sender to add and pin any JSON object they wish to Pinata's IPFS nodes. This endpoint is specifically optimized to only handle JSON content.
      /// </summary>
      /// <param name="jsonContent">Any valid JSON string</param>
      /// <param name="pinataMetadata">Metadata associated with the JSON file</param>
      /// <param name="pinataOptions">Custom replication policy for this file</param>
      /// <returns></returns>
      Task<PinJsonToIpfsResponse> PinJsonToIpfsAsync(string jsonContent, PinataMetadata pinataMetadata = null, PinataOptions pinataOptions = null, CancellationToken cancellationToken = default);

      /// <summary>
      /// This endpoint allows the sender to add and pin any JSON object they wish to Pinata's IPFS nodes. This endpoint is specifically optimized to only handle JSON content.
      /// </summary>
      /// <param name="pinataContent">Any C# object that will be serialized to JSON</param>
      /// <param name="pinataMetadata">Metadata associated with the JSON file</param>
      /// <param name="pinataOptions">Custom replication policy for this file</param>
      /// <returns></returns>
      Task<PinJsonToIpfsResponse> PinJsonToIpfsAsync(object pinataContent, PinataMetadata pinataMetadata = null, PinataOptions pinataOptions = null, CancellationToken cancellationToken = default);

      /// <summary>
      /// This endpoint allows the sender to change the pin policy their account.
      /// Following a successful call of this endpoint, the new pin policy provided will be utilized for every new piece of content pinned to IPFS via Pinata.
      /// </summary>
      Task<UserPinPolicyResponse> UserPinPolicyAsync(PinPolicy newPinPolicy, bool migratePreviousPins = false, CancellationToken cancellationToken = default);

      /// <summary>
      /// This endpoint allows the sender to add and pin any file, or directory, to Pinata's IPFS nodes.
      /// </summary>
      Task<PinFileToIpfsResponse> PinFileToIPFSAsync(Action<CapturedMultipartContent> payloadBuilder, PinataMetadata pinataMetadata = null, PinataOptions pinataOptions = null, CancellationToken cancellationToken = default);
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

      Task<PinJsonToIpfsResponse> IPinningEndpoint.PinJsonToIpfsAsync(string jsonContent, PinataMetadata pinataMetadata = null, PinataOptions pinataOptions = null, CancellationToken cancellationToken = default)
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
            var indexOfPatternStart = payloadJson.IndexOf(ReplacePattern) - 1;
            var indexOfPatternEnd = indexOfPatternStart + ReplacePattern.Length + 2;

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

      Task<PinJsonToIpfsResponse> IPinningEndpoint.PinJsonToIpfsAsync(object pinataContent, PinataMetadata pinataMetadata = null, PinataOptions pinataOptions = null, CancellationToken cancellationToken = default)
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

      public Task<UserPinPolicyResponse> UserPinPolicyAsync(PinPolicy newPinPolicy, bool migratePreviousPins = false, CancellationToken cancellationToken = default)
      {
         return this.PinningEndpoint
            .WithClient(this)
            .AppendPathSegment("userPinPolicy")
            .PutJsonAsync(new
               {
                  newPinPolicy,
                  migratePreviousPins
               }, cancellationToken)
            .ReceiveJson<UserPinPolicyResponse>();
      }

      public Task<PinFileToIpfsResponse> PinFileToIPFSAsync(Action<CapturedMultipartContent> payloadBuilder, PinataMetadata pinataMetadata = null, PinataOptions pinataOptions = null, CancellationToken cancellationToken = default)
      {
         static void AssertAllHttpContentHasFileName(CapturedMultipartContent content)
         {
            foreach( var part in content )
            {
               var name = part?.Headers?.ContentDisposition?.Name;
               var fileName = part?.Headers?.ContentDisposition?.FileName;
               if( string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(fileName) )
               {
                  throw new HttpRequestException("The 'httpContent.Headers.ContentDisposition.(Name|FileName)' is null or whitespace. "+
                                                 "All multi-part upload content must contain a 'Name' and 'FileName' content disposition header value. "+
                                                 "Try using the httpContent.AsPinataFile('name.txt') extension method to set the required fields on the HttpContent you are adding.");
               }
            }
         }

         return this.PinningEndpoint
            .WithClient(this)
            .AppendPathSegment("pinFileToIPFS")
            .PostMultipartAsync(multiPart =>
            {
               payloadBuilder(multiPart);

               AssertAllHttpContentHasFileName(multiPart);

               if( pinataMetadata is not null )
               {
                  multiPart.AddJson("pinataMetadata", pinataMetadata);
               }
               if( pinataOptions is not null )
               {
                  multiPart.AddJson("pinataOptions", pinataOptions);
               }
            }, cancellationToken)
            .ReceiveJson<PinFileToIpfsResponse>();
      }
   }

   public static class ExtensionsForHttpContent
   {
      public static T AsPinataFile<T>(this T content, string remoteFilePath) where T : HttpContent
      {
         var header = new ContentDispositionHeaderValue("form-data")
            {
               Name = "file",
               FileName = remoteFilePath
            };
         content.Headers.ContentDisposition = header;
         return content;
      }
   }
}



