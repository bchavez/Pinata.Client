using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Pinata.Client.Tests.IntegrationTests
{
   [Explicit]
   public class IntegrationTest : IntegrationTestBase
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
      public async Task data_can_auth()
      {
         var resp = await this.client.Data.TestAuthenticationAsync();
      }

      [Test]
      public async Task data_pin_total()
      {
         var resp = await this.client.Data.UserPinnedDataTotalAsync();
      }

      [Test]
      public async Task pinning_unpin()
      {
         var r = await this.client.Pinning.UnpinAsync("QmR9HwzakHVr67HFzzgJHoRjwzTTt4wtD6KU4NFe2ArYuj");
      }

      [Test]
      public async Task pinning_PinJsonToIpfs_string()
      {
         var body = new { hello = "world" };
         var json = JsonConvert.SerializeObject(body);
         var r = await this.client.Pinning.PinJsonToIpfsAsync(json);
      }

      [Test]
      public async Task pinning_PinJsonToIpfs_with_options()
      {
         var body = new { hello = "world" };
         var json = JsonConvert.SerializeObject(body);
         var opts = new PinataOptions
            {
               CidVersion = 1,
            };
         opts.CustomPinPolicy.AddOrUpdateRegion("FRA1", 1);

         var meta = new PinataMetadata
            {
               Name = "hello",
               KeyValues =
                  {
                     { "someKey", "someValue" }
                  }
            };

         var r = await this.client.Pinning.PinJsonToIpfsAsync(json, meta, opts);
      }

      [Test]
      public async Task pinning_PinJsonToIpfs_as_object()
      {
         var body = new { hello = "world" };
         var r = await this.client.Pinning.PinJsonToIpfsAsync(body);
      }

      [Test]
      public async Task pinning_PinJsonToIpfs_as_object_with_options()
      {
         var body = new { hello = "world" };

         var opts = new PinataOptions
            {
               CidVersion = 1,
            };
         opts.CustomPinPolicy.AddOrUpdateRegion("FRA1", 1);

         var meta = new PinataMetadata
            {
               Name = "hello",
               KeyValues =
                  {
                     { "someKey", "someValue" }
                  }
            };

         var r = await this.client.Pinning.PinJsonToIpfsAsync(body, meta, opts);
      }

      [Test]
      public async Task pinning_UserPinPolicy()
      {
         var policy = new PinPolicy();
         policy.AddOrUpdateRegion("FRA1", 1);
         var r = await this.client.Pinning.UserPinPolicyAsync(policy);
      }

      [Test]
      public async Task pinning_PinFileToIpfs_with_StringContent()
      {
         var html = @"
<html>
   <head>
      <title>Hello IPFS!</title>
   </head>
   <body>
      <h1>Hello World</h1>
   </body>
</html>
";
         var r = await this.client.Pinning.PinFileToIpfsAsync(content =>
            {
               var file = new StringContent(html, Encoding.UTF8, MediaTypeNames.Text.Html);

               content.AddPinataFile(file, "test2.html");
         });
      }

      [Test]
      public async Task pinning_PinFileToIpfs_with_StringContent_with_options()
      {
         var html = @"
<html>
   <head>
      <title>Hello IPFS!</title>
   </head>
   <body>
      <h1>Hello World</h1>
   </body>
</html>
";
         var metadata = new PinataMetadata // optional
            {
               KeyValues =
                  {
                     {"Author", "Brian Chavez"}
                  }
            };

         var options = new PinataOptions(); // optional
         options.CustomPinPolicy.AddOrUpdateRegion("NYC1", desiredReplicationCount: 1);

         var response = await this.client.Pinning.PinFileToIpfsAsync(content =>
               {
                  var file = new StringContent(html, Encoding.UTF8, MediaTypeNames.Text.Html);

                  content.AddPinataFile(file, "index.html");
               },
            metadata,
            options);

         if( response.IsSuccess )
         {
            //File uploaded to Pinata Cloud, now on IPFS!
            var hash = response.IpfsHash;
         }
      }

      [Test]
      public async Task pinning_HashMetadata_set()
      {
         var meta = new PinataMetadata
            {
               Name = "test3.html",
               KeyValues =
                  {
                     {"barbar", "jarjar"}
                  }
            };
         var hash = "QmR9HwzakHVr67HFzzgJHoRjwzTTt4wtD6KU4NFe2ArYuj";

         var r = await this.client.Pinning.HashMetadataAsync(hash, meta);
      }

      [Test]
      public async Task pinning_HashPinPolicy_set()
      {
         var policy = new PinPolicy();
         policy.AddOrUpdateRegion("NYC1", 1);
         policy.AddOrUpdateRegion("FRA1", 0);

         var hash = "QmR9HwzakHVr67HFzzgJHoRjwzTTt4wtD6KU4NFe2ArYuj";

         var r = await this.client.Pinning.HashPinPolicyAsync(hash, policy);
      }

      [Test]
      public async Task pinning_PinByHash()
      {
         var hash = "Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvBye";

         var r = await this.client.Pinning.PinByHashAsync(hash);
      }

      [Test]
      public async Task pinning_PinJobs()
      {
         var filters = new
            {
               filter =  new
                  {
                     sort = "ASC"
                  },
               additionalFilter = new
                  {
                     limit = 5
                  }
            };
         var r = await this.client.Pinning.PinJobs(filters);
      }

      [Test]
      public async Task data_PinList()
      {
         var filter = new
            {
               status = "pinned"
            };
         var r = await this.client.Data.PinList(filter);
      }
   }
}
