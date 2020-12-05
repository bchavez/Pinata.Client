using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using static System.Net.Http.HttpMethod;

namespace Pinata.Client.Tests.EndpointTests
{
   public class PinningTests : MockServerTest
   {
      private PinataClient client;

      [SetUp]
      public virtual void BeforeEachTest()
      {
         var config = new Config()
            {
               ApiKey = "key",
               ApiSecret = "secret"
            };

         this.client = new PinataClient(config);
      }

      [Test]
      public async Task pin_json_content()
      {
         this.server.RespondWithJsonTestFile();

         var content = @"{""hello"":""world""}";

         var r = await this.client.Pinning.PinJsonToIpfsAsync(content);

         this.server.ShouldHaveCalledPath("/pinning/pinJSONToIPFS")
            .WithVerb(Post)
            .WithRequestBody(content);

         await Verify(r);
      }

      [Test]
      public async Task pin_json_content_with_options()
      {
         this.server.RespondWithJsonTestFile();

         var content = @"{""hello"":""world""}";

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

         var r = await this.client.Pinning.PinJsonToIpfsAsync(content, meta, opts);

         var expectedBody = @"{""pinataContent"":{""hello"":""world""},""pinataOptions"":{""cidVersion"":1,""customPinPolicy"":{""regions"":[{""id"":""FRA1"",""desiredReplicationCount"":1}]},""wrapWithDirectory"":null,""hostNodes"":null},""pinataMetadata"":{""name"":""hello"",""keyvalues"":{""someKey"":""someValue""}}}";

         this.server.ShouldHaveCalledPath("/pinning/pinJSONToIPFS")
            .WithVerb(Post)
            .WithExactBody(expectedBody);

         await Verify(r);
      }

      [Test]
      public async Task pin_json_object()
      {
         this.server.RespondWithJsonTestFile();

         var body = new { hello = "world" };

         var r = await this.client.Pinning.PinJsonToIpfsAsync(body);

         var expectedBody = @"{""pinataOptions"":null,""pinataMetadata"":null,""pinataContent"":{""hello"":""world""}}";

         this.server.ShouldHaveCalledPath("/pinning/pinJSONToIPFS")
            .WithVerb(Post)
            .WithExactBody(expectedBody);

         await Verify(r);
      }

      [Test]
      public async Task pin_json_object_with_options()
      {
         this.server.RespondWithJsonTestFile();

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

         var expectedBody = @"{""pinataOptions"":{""cidVersion"":1,""customPinPolicy"":{""regions"":[{""id"":""FRA1"",""desiredReplicationCount"":1}]},""wrapWithDirectory"":null,""hostNodes"":null},""pinataMetadata"":{""name"":""hello"",""keyvalues"":{""someKey"":""someValue""}},""pinataContent"":{""hello"":""world""}}";

         this.server.ShouldHaveCalledPath("/pinning/pinJSONToIPFS")
            .WithVerb(Post)
            .WithExactBody(expectedBody);

         await Verify(r);
      }

      [Test]
      public async Task new_user_pin_policy()
      {
         this.server.RespondWithJsonTestFile();

         var policy = new PinPolicy();
         policy.AddOrUpdateRegion("FRA1", 1);

         var r = await this.client.Pinning.UserPinPolicyAsync(policy);

         var expectedBody = @"{""newPinPolicy"":{""regions"":[{""id"":""FRA1"",""desiredReplicationCount"":1}]},""migratePreviousPins"":false}";

         this.server.ShouldHaveCalledPath("/pinning/userPinPolicy")
            .WithVerb(Put)
            .WithExactBody(expectedBody);

         await Verify(r);
      }

      [Test]
      public async Task pin_html_file()
      {
         this.server.RespondWithJsonTestFile();

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
         var r = await this.client.Pinning.PinFileToIpfsAsync(payload =>
            {
               var file = new StringContent(html, Encoding.UTF8, MediaTypeNames.Text.Html);
               payload.AddPinataFile(file, "index.html");
            });


         this.server.ShouldHaveCalledPath("/pinning/pinFileToIPFS")
            .WithVerb(Post);
         //.With(fc =>
         //   {

         //      var cmc = fc.HttpRequestMessage.Content as CapturedMultipartContent;
         //      foreach( var c in cmc )
         //      {
         //         var z = c.Headers.ContentDisposition;
         //      }


         //      var content = fc.HttpRequestMessage.Content as CapturedMultipartContent;

         //      var cds = content.Headers.ContentDisposition;
         //      cds.Name.Should().Be("file");
         //      cds.FileName.Should().Be("index.html");
         //      return true;
         //   });
      }

      [Test]
      public async Task change_metadata()
      {
         this.server.RespondWith("OK");

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
         var s = await r.GetStringAsync();
         s.Should().Be("OK");

         var expectedBody = @"{""ipfsPinHash"":""QmR9HwzakHVr67HFzzgJHoRjwzTTt4wtD6KU4NFe2ArYuj"",""name"":""test3.html"",""keyvalues"":{""barbar"":""jarjar""}}";

         this.server.ShouldHaveCalledPath("/pinning/hashMetadata")
            .WithVerb(Put)
            .WithExactBody(expectedBody);
      }

      [Test]
      public async Task change_hash_pinpolicy()
      {
         this.server.RespondWith("OK");

         var policy = new PinPolicy();
         policy.AddOrUpdateRegion("NYC1", 1);
         policy.AddOrUpdateRegion("FRA1", 0);

         var hash = "QmR9HwzakHVr67HFzzgJHoRjwzTTt4wtD6KU4NFe2ArYuj";

         var expectedBody = @"{""ipfsPinHash"":""QmR9HwzakHVr67HFzzgJHoRjwzTTt4wtD6KU4NFe2ArYuj"",""newPinPolicy"":{""regions"":[{""id"":""NYC1"",""desiredReplicationCount"":1},{""id"":""FRA1"",""desiredReplicationCount"":0}]}}";

         var r = await this.client.Pinning.HashPinPolicyAsync(hash, policy);
         var s = await r.GetStringAsync();
         s.Should().Be("OK");

         this.server.ShouldHaveCalledPath("/pinning/hashPinPolicy")
            .WithVerb(Put)
            .WithExactBody(expectedBody);
      }

      [Test]
      public async Task pin_by_hash()
      {
         this.server.RespondWithJsonTestFile();

         var hash = "Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye";

         var r = await this.client.Pinning.PinByHashAsync(hash);

         var expectedBody = @"{""hashToPin"":""Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye"",""pinataMetadata"":null,""pinataOptions"":null}";

         this.server.ShouldHaveCalledPath("/pinning/pinByHash")
            .WithVerb(Post)
            .WithExactBody(expectedBody);

         await Verify(r);
      }

      [Test]
      public async Task pin_jobs()
      {
         this.server.RespondWithJsonTestFile();

         var filters = new
            {
               filter = new
                  {
                     sort = "ASC"
                  },
               additionalFilter = new
                  {
                     limit = 5
                  }
            };

         var r = await this.client.Pinning.PinJobs(filters);

         this.server.ShouldHaveCalledPath("/pinning/pinJobs")
            .WithVerb(Get)
            .WithQueryParams(filters);

         await Verify(r);
      }
   }
}
