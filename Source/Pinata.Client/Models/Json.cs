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
}
