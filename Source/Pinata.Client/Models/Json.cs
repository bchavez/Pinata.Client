using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Pinata.Client.Models
{
   public class Json
   {
      /// <summary>
      /// Extra data for/from the JSON serializer/deserializer to included with the object model.
      /// </summary>
      [JsonExtensionData]
      public IDictionary<string, JToken> ExtraJson { get; internal set; } = new Dictionary<string, JToken>();
   }

   public partial class TestAuthenticationResponse : Json
   {
      [JsonProperty("message")]
      public string Message { get; set; }
   }

   public partial class UserPinnedDataTotalResponse : Json
   {
      /// <summary>
      /// The number of pins you currently have pinned with Pinata
      /// </summary>
      [JsonProperty("pin_count")]
      public long PinCount { get; set; }

      /// <summary>
      /// The total size of all unique content you have pinned with Pinata (expressed in bytes).
      /// </summary>
      [JsonProperty("pin_size_total")]
      public long? PinSizeTotal { get; set; }

      /// <summary>
      /// The total size of all content you have pinned with Pinata. This value is derived by multiplying the size of each piece of unique content by the number of times that content is replicated. This value will be expressed in bytes.
      /// </summary>
      [JsonProperty("pin_size_with_replications_total")]
      public long? PinSizeWithReplicationsTotal { get; set; }
   }
}
