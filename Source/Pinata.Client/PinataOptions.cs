using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
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

      [JsonProperty("hostNodes")]
      public string[] HostNodes { get; set; }
   }

   public class PinPolicy : Json
   {
      [JsonProperty("regions")]
      public List<Region> Regions { get; set; } = new List<Region>();

      public void AddOrUpdateRegion(string id, int desiredReplicationCount)
      {
         var existingRegion = this.Regions.FirstOrDefault(r => r.Id == id);
         if (existingRegion is null)
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
}
