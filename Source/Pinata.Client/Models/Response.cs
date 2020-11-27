using System;
using Newtonsoft.Json;

namespace Pinata.Client.Models
{
   public class Response : Json
   {
      public string Error { get; set; }
      public bool IsSuccess => string.IsNullOrWhiteSpace(this.Error);
   }


   public partial class TestAuthenticationResponse : Response
   {
      [JsonProperty("message")]
      public string Message { get; set; }
   }

   public partial class UserPinnedDataTotalResponse : Response
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

   public class UnpinResponse : Response
   {
      public bool IsSuccess => string.IsNullOrWhiteSpace(this.Error);
   }

   public partial class PinJsonToIpfsResponse : Response
   {
      [JsonProperty("IpfsHash")]
      public string IpfsHash { get; set; }

      [JsonProperty("PinSize")]
      public long PinSize { get; set; }

      [JsonProperty("Timestamp")]
      public DateTimeOffset Timestamp { get; set; }
   }
}
