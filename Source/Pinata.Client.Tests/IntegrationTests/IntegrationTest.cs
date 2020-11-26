using System.IO;
using System.Net;
using System.Net.Http;
using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Pinata.Client.Tests.IntegrationTests
{
   public class Secrets
   {
      public string ApiKey { get; set; }
      public string ApiSecret { get; set; }
   }

   [Explicit]
   public class IntegrationTest : HasVerifyTest
   {
      protected Secrets secrets;

      public IntegrationTest()
      {
         var startupFolder = Path.GetDirectoryName(typeof(IntegrationTest).Assembly.Location);
         Directory.SetCurrentDirectory(startupFolder);

         ReadSecrets();

#if NETFRAMEWORK
         ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
#endif
      }

      protected void ReadSecrets()
      {
         var json = File.ReadAllText("../../../.secrets.txt");
         this.secrets = JsonConvert.DeserializeObject<Secrets>(json);
      }
   }
}
