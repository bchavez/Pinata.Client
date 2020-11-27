using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using VerifyNUnit;

namespace Pinata.Client.Tests.IntegrationTests
{
   [Explicit]
   public class AllTests : IntegrationTest
   {
      private PinataClient client;

      [SetUp]
      public void BeforeEachTest()
      {
         var config = new Config
            {
               ApiKey = this.secrets.ApiKey,
               ApiSecret = this.secrets.ApiSecret
            };
         client = new PinataClient(config);
         client.EnableFiddlerDebugProxy("http://localhost.:8888");
      }

      [Test]
      public async Task can_auth()
      {
         var resp = await this.client.Data.TestAuthenticationAsync();
         await Verify(resp);
      }

      [Test]
      public async Task data_pin_total()
      {
         var resp = await this.client.Data.UserPinnedDataTotalAsync();
         await Verify(resp);
      }

      [Test]
      public async Task pinning_unpin()
      {
         var r = await this.client.Pinning.UnpinAsync("foobar");
      }

      [Test]
      public async Task pinning_pinJson_as_string()
      {
         var body = new { hello = "world" };
         var json = JsonConvert.SerializeObject(body);
         var r = await this.client.Pinning.PinJsonToIpfsAsync(json);

      }
   }
}
